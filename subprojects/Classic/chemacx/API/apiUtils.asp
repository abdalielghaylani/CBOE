<SCRIPT RUNAT="Server" Language="VbScript">
'****************************************************************************************
'*	PURPOSE: Returns a string suited to pass Oracle a date	                            
'*	INPUT: A VbScript variant of date-time subtype		                    			
'*	OUTPUT: A string of the form to_date('01/01/2001 23:11:00', 'MM/DD/YYY HH24:MM:SS') 
'* 			which can be used in Oracle SQL statement to insert, update or select dates  							*						*
'****************************************************************************************
Function GetOracleDateString(byval pDate)
	dash = "-"
	cln = ":"
	strDate = Year(pDate) & dash & Month(pDate) & dash & Day(pDate) & " " & Hour(pDate) & cln & Minute(pDate) & cln & Second(pDate) 
	GetOracleDateString =  "to_date('" & strDate & "','YYYY-MM-DD HH24:MI:SS')"
End Function

'****************************************************************************************
'*	PURPOSE: Create and Open an ADO to the Database using connection info from ini file 	                            *    	*			*
'*	INPUT: 	Uses COWS Application scope variables and expects a global Conn variable	                    				 
'*	OUTPUT: Creates and Opens an ADO connection called Conn in the pages global scope   
'****************************************************************************************
Sub GetACXConnection()

	Set Conn = Server.CreateObject("ADODB.Connection")
	Conn.Open GetACXConStr() 
End Sub

Function GetACXConStr()
	Dim UserName
	Dim UserID
	Dim ConnStr
	Dim connection_array
	
	'get connection string from application variable
	connection_array = Application("base_connection" & "chemacx")
	' Distinguish Oracle and Access DBMS
	DBMS = Ucase(connection_array(6))
	if Lcase(connection_array(4)) = "login_required" then
		if Request("CsUserName") <> "" then
			UserName =  Request("CsUserName")
			UserID = Request("CSUserID")
		Elseif IsObject(Session) then
			UserName = Session("UserName" & "chemacx")
			UserID = Session("UserID" & "chemacx")
		Elseif CSUserName <> "" then
			UserName = CSUserName
			UserID = CSUserID
		End if
		If Len(UserName)=0 OR Len(UserID)= 0 then
			Response.Write "Credentials Error"
			Response.end
		End if
		
		UserName = Application("UserIDKeyword") & "=" & UserName
		UserID = Application("PWDKeyword") & "=" & UserID
	Else
		UserName = connection_array(4)
		UserID = connection_array(5)
	End if	
	ConnStr = connection_array(0) & "="  & connection_array(1) 
	If UserName <> "" then ConnStr = ConnStr & ";" & UserName 
	If UserID <> "" then ConnStr = ConnStr & ";" & UserID
	'Response.Write ConnStr
	'Response.end
	GetACXConStr = ConnStr
End function


'****************************************************************************************
'*	PURPOSE: Create an ADO command to the Database using connection info from ini file 	                            *    	*			*
'*	INPUT: 	Uses COWS Application scope variables, expects global variables Conn and Cmd	                    				 
'*	OUTPUT: Creates ADO command called Cmd in the pages global scope using global Conn   
'****************************************************************************************
Sub GetACXCommand(pCommandName, pCommandTypeEnum)
	' Open the connection
	Call GetACXConnection()
	' Create the command
	Set Cmd = Server.CreateObject("ADODB.Command")
	Set Cmd.ActiveConnection = Conn
	Cmd.CommandText = pCommandName
	Cmd.CommandType = pCommandTypeEnum
End sub

'add abitlity to log api calls
Sub LogAction(ByVal inputstr)
		on error resume next
		filepath = Application("ServerDrive") & "\" & Application("ServerRoot") & "\cfwlog.txt"
		Set fs = Server.CreateObject("Scripting.FileSystemObject")
		Set a = fs.OpenTextFile(filepath, 8, True)  
		a.WriteLine Now & ": " & inputstr
		a.WriteLine " "
		a.close
End Sub


Sub ExecuteCmd(strID)
	On Error Resume Next
	Cmd.Execute
	If Err then
		Select Case True 
			Case inStr(Err.description,"ORA-20003") > 0	
		'If inStr(Err.description,"ORA-20003") > 0 then
			Response.Write  strID & ": Cannot execute invalid Oracle procedure."
			Case inStr(Err.description,"access violation") > 0
			Response.Write  strID & ": Current user is not allowed to execute Oracle procedure."
			Case Else
			Response.Write strID & ": " & Err.description	
		'End if
		End Select
		Response.end
	End if
End Sub
'****************************************************************************************
'*	PURPOSE: Performs an HTTP request using CShttp and returns the HTTP response	                            
'*	INPUT: Method = POST or GET, HostName, Target path, Referrer, form-url encoded data		                    			
'*	OUTPUT: The raw http response from the server  											*
'****************************************************************************************
Function CShttpRequest(pMethod, pHostName, pTarget, pReferrer, pData)
	Dim Connection,HttpRequest
	Dim HTTP_QUERY_STATUS_CODE, HTTP_QUERY_STATUS_TEXT
	Dim RequestType
	Dim httpResponse
	
	HTTP_QUERY_STATUS_CODE = 19
	HTTP_QUERY_STATUS_TEXT = 20
	
	on error resume next
	
	if UCASE(pMethod) = "GET" then
		RequestType = 0 ' This is a GET
	Else 
		Requesttype = 1 ' This is a POST
	End if
	
	FormHeaders = "Content-Type: application/x-www-form-urlencoded"
	
	set Connection = Server.CreateObject("CSHttp.Connection")
	Connection.Open (pHostName)
	set HttpRequest = Connection.Requests.Add(RequestType, pTarget, pReferrer, pData, FormHeaders)
	HttpRequest.Send
	StatusCode = httpRequest.Info(HTTP_QUERY_STATUS_CODE, 0) + ""

	If StatusCode <> "200" then
		'httpResponse = httpRequest.Response
		httpResponse = "HTTP Error: " & StatusCode & " : " & httpRequest.Info(HTTP_QUERY_STATUS_TEXT, 0)
	Else
		httpResponse = httpRequest.Response
	End If
	
    CShttpRequest = httpResponse
End Function

'****************************************************************************************
'*	PURPOSE: Performs an HTTP request using ServerXMLHTTP object from the MSxml2 library
'*  Note that this object is better than CSHTTP because it has better threading and can be 
'*  nested in relay posts 	                            
'*	INPUT: Method = POST or GET, HostName, Target path, Referrer, form-url encoded data		                    			
'*	OUTPUT: The raw http response from the server  									
'****************************************************************************************
Function CShttpRequest2(pMethod, pHostName, pTarget, pUserAgent, pData)
	Dim objXmlHttp
	Dim httpResponse
	Dim URL
	Dim StatusCode
	Dim ResolveTimeout
	Dim ConnectTimeout
	Dim SendTimeout
	Dim ReceiveTimeout

	'CSBR# 139459
	'Date of Change: 04-Mar-2011
	'Purpose: To append portnumber to the hostname, if a non-standard port number is used.
	Dim portNumber
	portNumber = Request.ServerVariables("SERVER_PORT")
    if portNumber = "80" or inStr(pHostName, ":" & portNumber) > 0 then
	    URL = Application("SERVER_TYPE") & pHostName & "/" & pTarget
	else
	    URL = Application("SERVER_TYPE") & pHostName & ":" & portNumber & "/" & pTarget	   
	end if
	'End of change

	
	
	' This is the server safe version from MSXML3.
	Set objXmlHttp = Server.CreateObject("WinHTTP.WinHTTPRequest.5.1")
	'default connect and resolve timeouts
	ResolveTimeout = 5 * 1000
	ConnectTimeout = 5 * 1000
	objXmlHttp.setTimeouts ResolveTimeout, ConnectTimeout, SendTimeout, ReceiveTimeout

	' Syntax:
	'  .open(bstrMethod, bstrUrl, bAsync, bstrUser, bstrPassword)
	objXmlHttp.open pMethod, URL, False
	objXmlHttp.setRequestHeader "Content-Type", "application/x-www-form-urlencoded"
	objXmlHttp.setRequestHeader "User-Agent", pUserAgent
	objXmlHttp.send pData

	' Print out the request status:
	StatusCode = objXmlHttp.status

	If StatusCode <> "200" then
		httpResponse = objXmlHttp.responseText
		'httpResponse = "HTTP Error: " & StatusCode & " : " & objXmlHttp.statusText
	Else
		httpResponse = objXmlHttp.responseText
	End If
	
    CShttpRequest2 = httpResponse
	Set objXmlHttp = Nothing
End Function

'SYAN added 3/26/2004 to return recordset with parameterized SQL
'-------------------------------------------------------------------------------
' Purpose: Construct the Command object by parsing the input arguments,
'		   execute the command, returns a recordset
' Inputs: *SQL statement
'		  *Semi-colon delimited parameters in  format of
'          Name|Type|Direction|Size|Value;...
' Output: A recordset 
'-------------------------------------------------------------------------------
Function GetRecordSet(ByVal sql, ByVal parameters)
	Dim cmd, cmd2, dataConn, rst
	Dim parametersArr, paramAttributes
	Dim param, paramName, paramType, paramDirection, paramSize, paramValue
	Dim ClobToTableSPName
	
	ConnStr = GetACXConStr() 
	Set dataConn = Server.CreateObject("ADODB.Connection")
	dataConn.Open ConnStr
	
	Set cmd = Server.CreateObject("ADODB.Command")
	Set param = Server.CreateObject("ADODB.Parameter")
	Set rst = Server.CreateObject("ADODB.RecordSet")
	
	 
	cmd.ActiveConnection = dataConn
	cmd.CommandText = sql
	cmd.CommandTimeout = 100
	dataConn.Begintrans()	
		If parameters <> "" then
			parametersArr = Split(parameters, ";")
			For i = 0 to UBound(parametersArr)
				
				on error resume next
				paramAttributes = Split(parametersArr(i), "|")

				paramName = paramAttributes(0)					
				paramType = CInt(paramAttributes(1))
				paramDirection = CInt(paramAttributes(2))
				
				if paramAttributes(3) <> "" then
					paramSize = CInt(paramAttributes(3))
				else
					paramSize = Len(paramAttributes(4))
				end if
				
				paramValue = paramAttributes(4)
				if paramValue = "" then
					paramValue = Empty
				end if
				
				if paramName = "InClause" then
					' Insert into temp_ids table
					ClobToTableSPName =  "CHEMACXDB.ClobToTable"
					Set cmd2 = Server.CreateObject("ADODB.Command")
					cmd2.ActiveConnection = dataConn
					cmd2.Parameters.Append cmd2.CreateParameter("c", 201, 1, len(paramValue), paramValue)	
					cmd2.CommandText = "{call " & ClobToTableSPName & "(?)}" 
					'response.write cmd2.CommandText & "<BR>"
					'response.write "cmd2 ParamValue: " & paramValue & "<BR>" 
					cmd2.properties("SPPRMSLOB") = true
					cmd2.Execute 
					cmd2.properties("SPPRMSLOB") = false
				Else
					on error resume next
					Set param = cmd.CreateParameter(paramName, paramType, paramDirection, paramSize, paramValue)
					cmd.Parameters.Append param
				End if
			Next
		End if
		
		on error resume next  
		rst.LockType=3
		'response.write "cmd sql: " & sql & "<BR>"
		'response.write "cmd ParamCount: " & cmd.Parameters.count & "<BR>"
		'response.end
		Set rst = cmd.Execute
		'ljb 11/26/2008 setting LockType = 3 (adLockOptimistic) allows us to close the connection and still forward page through the recordset.
		dataConn.CommitTrans
		If Err then
			response.write "Error in apiUtils GetRecordSet: " & err.source & " " & err.description
			'if an error occurs close the connection. this was causing problems with mdb file locking
			Set GetRecordSet = Nothing		
			Set param = Nothing
			Set cmd = Nothing		
			Set dataConn = Nothing	
		End if	
	
	'ljb 11/26/2008 close everything before passing back the recordsset. since rst.locktype = 3 we can do this
	Set param = Nothing
	Set cmd = Nothing
	Set dataConn = Nothing	
	Set GetRecordSet = rst
End Function

'DGB  10/19/2010 remove Varchar2 limitation by using temp_id temporary table strategy
'-------------------------------------------------------------------------------
' Purpose: Build IN clause referencing temp_ids table with cardinality hint based on number of values
' Inputs: Comma delimited value list
' Output: IN clause with no parameters
' This function populates the temp_ids temporary table
'-------------------------------------------------------------------------------
Function BuildInClause(ByVal valueList)
	Dim retVal
	Dim numOfValues, magnitude
	Dim temptableName
	Dim Cmd

	temptableName = "CHEMACXDB.TEMP_IDS" 
	
	valueArr = Split(valueList, ",")
	numOfValues = UBound(valueArr) + 1
	
	if numOfValues < 10 then
		magnitude = 1
	elseif numOfValues < 100 then
		magnitude = 10
	elseif numOfValues < 1000 then
		magnitude = 100
	elseif numOfValues < 1000 then
		magnitude = 100
	elseif numOfValues < 10000 then
		magnitude = 1000
	elseif numOfValues < 100000 then
		magnitude = 10000
	else
		magnitude = "NONE"
	end if
	
	if magnitude = "NONE" then
		retVal = "select * from " & temptableName
	else
		retVal = "select /*+ cardinality(t " & magnitude & ") */ * from " & temptableName 
	end if
	
	BuildInClause = retVal
End Function

'******OBSOLETE******
'SYAN added 3/31/2004 to return recordset with parameterized SQL
'-------------------------------------------------------------------------------
' Purpose: Build IN clause with cardinality hint based on number of values
' Inputs: Comma delimited value list
' Output: IN clause with bind variable 
'-------------------------------------------------------------------------------
Function BuildInClause_OLD(ByVal valueList)
	Dim retVal
	Dim numOfValues, magnitude
	
	valueArr = Split(valueList, ",")
	numOfValues = UBound(valueArr) + 1
	
	if numOfValues < 10 then
		magnitude = 1
	elseif numOfValues < 100 then
		magnitude = 10
	elseif numOfValues < 1000 then
		magnitude = 100
	else
		magnitude = "NONE"
	end if
	
	if magnitude = "NONE" then
		retVal = "select * from Table(cast(CHEMACXDB.STR2TBL(?) as CHEMACXDB.MYTABLETYPE)) t where rownum >= 0"
	else
		retVal = "select /*+ cardinality(t " & magnitude & ") */ * from Table(cast(CHEMACXDB.STR2TBL(?) as CHEMACXDB.MYTABLETYPE)) t where rownum >= 0"
	end if
	
	BuildInClause = retVal
End Function

</SCRIPT>
