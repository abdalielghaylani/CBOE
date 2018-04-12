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
'*	PURPOSE: Returns a string suited to pass Oracle a date. Forces time to 23:59:59
'*	INPUT: A VbScript variant of date-time subtype
'*	OUTPUT: A string of the form to_date('01/01/2001 23:11:00', 'MM/DD/YYY HH24:MM:SS')
'* 			which can be used in Oracle SQL statement to insert, update or select dates  							*						*
'****************************************************************************************
Function GetOracleDateString2(byval pDate)
	dash = "-"
	cln = ":"
	strDate = Year(pDate) & dash & Month(pDate) & dash & Day(pDate) & " 23:59:59"
	GetOracleDateString2 =  "to_date('" & strDate & "','YYYY-MM-DD HH24:MI:SS')"
End Function


'****************************************************************************************
'*	PURPOSE: Returns a string suited to pass Oracle a date. Forces time to 00:00:00
'*	INPUT: A VbScript variant of date-time subtype
'*	OUTPUT: A string of the form to_date('01/01/2001 23:11:00', 'MM/DD/YYY HH24:MM:SS')
'* 			which can be used in Oracle SQL statement to insert, update or select dates  							*						*
'****************************************************************************************
Function GetOracleDateString3(byval pDate)
	dash = "-"
	cln = ":"
	strDate = Year(pDate) & dash & Month(pDate) & dash & Day(pDate) & " 00:00:00"
	GetOracleDateString3 =  "to_date('" & strDate & "','YYYY-MM-DD HH24:MI:SS')"
End Function

'****************************************************************************************
'*	PURPOSE: Returns a string suited to pass to Oracle. 
'*	INPUT: A string value                    			
'*	OUTPUT: A string with single quotes ' replaces with double single quotes ''
'* 			which can be used in Oracle SQL statement to insert, update or varchar values  							*						*
'****************************************************************************************
Function GetOracleString(text)
	GetOracleString = replace(text, "'", "''")
End Function

'****************************************************************************************
'*	PURPOSE: Create and Open an ADO connection to the Database using connection info from ini file 	                            *    	*			*
'*	INPUT: 	Uses COWS Application scope variables and expects a global Conn variable
'*	Uses COWS Session credentials to connect and redirects to login page if unavailable
'*	OUTPUT: Creates and Opens an ADO connection called Conn in the pages global scope
'****************************************************************************************
Sub GetInvConnection()
	Dim UserName
	Dim UserID
	if IsObject(Session) then
		UserName = Session("UserName" & "cheminv")
		UserID = Session("UserID" & "cheminv")
	Elseif Request("CsUserName") <> "" then
		UserName =  Request("CsUserName")
		UserID = Request("CSUserID")
	Elseif Request("UserID") <> "" then 'Incase of InvLoader
		UserName =  CsUserName 'Deliberately using CHEMINVDB2 as credentials, which is set in InvLoaderSQL.asp
		UserID = CSUserID
	Elseif CSUserName <> "" then
		if Request("Ticket") <> "" then
			ticket = Request("Ticket")
		End if	
		if ticket = "" AND Request.Cookies("COESSO") <> "" then
			ticket = Request.Cookies("COESSO")
		End if	
		if ticket <> "" then
			
			set SSOobj = Server.CreateObject("CambridgeSoft.COE.Security.Services.SingleSignOnCom")
			if SSOobj.ValidateTicket(ticket) then
				UserName = CSUserName
				UserID = CSUserID
			else
				Response.write "Invalid or expired authentication ticket"
				Response.end
			end if
		Else
			Response.write "Credentials or authentication ticket required for this call"
			Response.end
		End if
	End if
	If Len(UserName)=0 OR Len(UserID)= 0 then
		Response.redirect "/ChemInv/login.asp"
		Response.end
	End if
	connection_array = Application("base_connection" & "cheminv")
	ConnStr =  connection_array(0) & "="  & connection_array(1) & ";" & Application("UserIDKeyword") & "=" & UserName & ";" & Application("PWDKeyword") & "=" & UserID
	'Response.Write "<br><br><br><br>" & ConnStr
	'Response.end
	Set Conn = Server.CreateObject("ADODB.Connection")
	Conn.Open ConnStr
End Sub

Sub GetSessionlessInvConnection()
	Dim UserName
	Dim UserID
	if CSUserName <> "" then
		UserName = CSUserName
		UserID = CSUserID
	End if
	connection_array = Application("base_connection" & "cheminv")
	ConnStr =  connection_array(0) & "="  & connection_array(1) & ";" & Application("UserIDKeyword") & "=" & UserName & ";" & Application("PWDKeyword") & "=" & UserID
	Set Conn = Server.CreateObject("ADODB.Connection")
	Conn.Open ConnStr
End Sub
Sub GetInvShapeConnection()
	Dim UserName
	Dim UserID
	if IsObject(Session) then
		UserName = Session("UserName" & "cheminv")
		UserID = Session("UserID" & "cheminv")
	Elseif Request("CsUserName") <> "" then
		UserName =  Request("CsUserName")
		UserID = Request("CSUserID")
	Elseif CSUserName <> "" then
		UserName = CSUserName
		UserID = CSUserID
	End if
	If Len(UserName)=0 OR Len(UserID)= 0 then
		Response.redirect "/ChemInv/login.asp"
		Response.end
	End if
	connection_array = Application("base_connection" & "cheminv")
	'ConnStr =  connection_array(0) & "="  & connection_array(1) & ";" & Application("UserIDKeyword") & "=" & UserName & ";" & Application("PWDKeyword") & "=" & UserID
	ConnStr = "Provider=OraOLEDB.Oracle.1;User ID=" & UserName & ";Password=" & UserID & ";Data Source=" & Application("ORA_SERVICENAME")
	ShapeConnStr = "Provider=MSDataShape; Data " & ConnStr

	'Response.Write "<br><br><br><br>" & ShapeConnStr
	'Response.end
	Set ShapeConn = server.CreateObject("ADODB.Connection")
	ShapeConn.Open ShapeConnStr
End Sub

Sub GetChemInvShapeConnection()
	Dim UserName
	Dim UserID
	UserName = Application("CHEMINV_USERNAME")
	UserID = Application("CHEMINV_PWD")
	If Len(UserName)=0 OR Len(UserID)= 0 then
		Response.redirect "/ChemInv/login.asp"
		Response.end
	End if
	connection_array = Application("base_connection" & "cheminv")
	'ConnStr =  connection_array(0) & "="  & connection_array(1) & ";" & Application("UserIDKeyword") & "=" & UserName & ";" & Application("PWDKeyword") & "=" & UserID
	ConnStr = "Provider=OraOLEDB.Oracle.1;User ID=" & UserName & ";Password=" & UserID & ";Data Source=" & Application("ORA_SERVICENAME")
	ShapeConnStr = "Provider=MSDataShape; Data " & ConnStr

	'Response.Write "<br><br><br><br>" & ShapeConnStr
	'Response.end
	Set ShapeConn = server.CreateObject("ADODB.Connection")
	ShapeConn.Open ShapeConnStr
End Sub

Sub GetRegConnection()
    on error resume next
	Dim UserName
	Dim UserID
	if IsObject(Session) then
		UserName = Session("UserName" & "cheminv")
		UserID = Session("UserID" & "cheminv")
	Elseif Request("CsUserName") <> "" then
		UserName =  Request("CsUserName")
		UserID = Request("CSUserID")
	Elseif Request("UserID") <> "" then 'CBOE-1365 SJ Incase of InvLoader
		UserName =  CSUserName 
		UserID = CSUserID
	Elseif CSUserName <> "" then
		UserName = CSUserName
		UserID = CSUserID
	End if
	If Len(UserName)=0 OR Len(UserID)= 0 then
		Response.Write "Credentials Error"
		Response.end
	End if
	connection_array = Application("base_connection" & "invreg")
	'Cannot use the oledb connection because it truncates the Base64_cdx Long field
	'ConnStr =  "dsn=chem_reg;UID=" & UserName & ";PWD=" & UserID
	'Now that reg uses clobs we can use the udl
	ConnStr =  connection_array(0) & "="  & connection_array(1) & ";" & Application("UserIDKeyword") & "=" & UserName & ";" & Application("PWDKeyword") & "=" & UserID
	'Response.Write "<BR><BR><BR><BR>" & ConnStr
	'Response.end
	Set Conn = Server.CreateObject("ADODB.Connection")
	Conn.Open ConnStr
	if err.number="-2147467259" then 
	    call ShowConnectionError("REG_SERVER_NAME")
        Response.End
	end if 
End Sub

Sub GetChemInvConnection()
	Dim UserName
	Dim UserID
	UserName = Application("CHEMINV_USERNAME")
	UserID = Application("CHEMINV_PWD")
	connection_array = Application("base_connection" & "cheminv")
	ConnStr =  connection_array(0) & "="  & connection_array(1) & ";" & Application("UserIDKeyword") & "=" & UserName & ";" & Application("PWDKeyword") & "=" & UserID
	'Response.Write "<br><br><br><br>" & ConnStr
	Set Conn = Server.CreateObject("ADODB.Connection")
	Conn.Open ConnStr
End Sub

Sub GetACXConnection()
	on error resume next
	Dim UserName
	Dim UserID
	'UserName = Session("UserName" & "cheminv")
	'UserID = Session("UserID" & "cheminv")
	connection_array = Application("base_connection" & "invacx")
	if ucase(connection_array (6))= "ORACLE"  then 
	    if IsObject(Session) then
		    UserName = Session("UserName" & "cheminv")
		    UserID = Session("UserID" & "cheminv")
	    Elseif Request("CsUserName") <> "" then
		    UserName =  Request("CsUserName")
		    UserID = Request("CSUserID")
	    Elseif CSUserName <> "" then
		    UserName = CSUserName
		    UserID = CSUserID
	    End if
    else
		UserName = "Admin"
		Password = ""
	end if
	ConnStr =  connection_array(0) & "="  & connection_array(1) & ";" & Application("UserIDKeyword") & "=" & UserName & ";" & Application("PWDKeyword") & "=" & UserID
	'Response.Write "<BR><BR><BR><BR>" & ConnStr
	'Response.end
	Set Conn = Server.CreateObject("ADODB.Connection")
	Conn.Open ConnStr
	if err.number="-2147467259" then 
	    call ShowConnectionError("ACX_SERVER_NAME")
        Response.End
	end if 

End Sub

Sub GetInvRPTConnection()
	Dim ConnStr
	ConnStr = "Provider=Microsoft.Jet.OLEDB.4.0; Data Source= " & Application("ReportDBPath")
	'Response.Write "RPTConnStr= " & ConnStr
	'Response.end
	Set RPTConn = Server.CreateObject("ADODB.Connection")
	RPTConn.Open ConnStr
End Sub

'****************************************************************************************
'*	PURPOSE: Create an ADO command to the Database using connection info from ini file 	                            *    	*			*
'*	INPUT: 	Uses COWS Application scope variables, expects global variables Conn and Cmd
'*	OUTPUT: Creates ADO command called Cmd in the pages global scope using global Conn
'****************************************************************************************
Sub GetInvCommand(pCommandName, pCommandTypeEnum)
	' Open the connection
	Call GetInvConnection()
	' Create the command
	Set Cmd = Server.CreateObject("ADODB.Command")
	Set Cmd.ActiveConnection = Conn
	Cmd.CommandText = pCommandName
	Cmd.CommandType = pCommandTypeEnum
End sub

Sub GetInvCommandbyRefObj(pCommandName, pCommandTypeEnum,byRef obj)
	' Open the connection
	Call GetInvConnection()
	' Create the command
	Set obj = Server.CreateObject("ADODB.Command")
	Set obj.ActiveConnection = Conn
	obj.CommandText = pCommandName
	obj.CommandType = pCommandTypeEnum
End sub


Sub GetChemInvCommand(pCommandName, pCommandTypeEnum)
	' Open the connection
	Call GetChemInvConnection()
	' Create the command
	Set Cmd = Server.CreateObject("ADODB.Command")
	Set Cmd.ActiveConnection = Conn
	Cmd.CommandText = pCommandName
	Cmd.CommandType = pCommandTypeEnum
End sub

Function GetCommand(ByRef Conn, CommandText, CommandType)
	Dim Cmd
	Set Cmd = Server.CreateObject("ADODB.Command")
	Cmd.ActiveConnection = Conn
	Cmd.CommandType = CommandType
	Cmd.CommandText = CommandText
	Set GetCommand = Cmd
End Function

Sub GetAdminCommand(pCommandName, pCommandTypeEnum)
	' Open the connection
	Call GetAdminConnection()
	' Create the command
	Set Cmd = Server.CreateObject("ADODB.Command")
	Set Cmd.ActiveConnection = Conn
	Cmd.CommandText = pCommandName
	Cmd.CommandType = pCommandTypeEnum
End sub

Sub GetChemInvCommand(pCommandName, pCommandTypeEnum)
	' Open the connection
	Call GetChemInvConnection()
	' Create the command
	Set Cmd = Server.CreateObject("ADODB.Command")
	Set Cmd.ActiveConnection = Conn
	Cmd.CommandText = pCommandName
	Cmd.CommandType = pCommandTypeEnum
End sub

Sub GetSECCommand(pCommandName, pCommandTypeEnum)
	' Open the connection
	Call GetSECConnection()
	' Create the command
	Set Cmd = Server.CreateObject("ADODB.Command")
	Set Cmd.ActiveConnection = Conn
	Cmd.CommandText = pCommandName
	Cmd.CommandType = pCommandTypeEnum
End sub

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

Sub ExecuteCmdbyRefObj(strID, byref obj)
	On Error Resume Next
	obj.Execute
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
Function CnvString(byval input)
	' change "" to null
	if input = "" then
		CnvString = null
	else
		CnvString = input
	end if
end function

Function SetVariableStr(byval name,byval value)
	Dim str
	if value = "" then
		str = name & " = NULL"
	Else
		str = name & " = """ & value & """"
	End if
	SetVariableStr = str
End function

Function DictionaryToList(oDict)
	For each Key in oDict
			str = str & Key & ","
	Next
	if inStr(str, ",") then str = left(str, len(str)-1)
	DictionaryToList = str
End function

Function GetFirstInList(list,delimiter)
	hasdelimiter = InStr(1,list, delimiter)
	if hasdelimiter > 0 then
		endpos = hasdelimiter -1
		GetFirstInList =  left(list, endpos)
	else
		GetFirstInList =  list
	end if
End function

Sub DumpRequest()
	Response.Write "QueryString= " & Request.QueryString & "<BR>"
	for each fld in Request.Form
	Response.Write "&" & fld & "=" & Request(fld) & "<BR><BR>"
	next
	Response.end
End Sub

Sub GetRegBatchAttributes(RegID, BatchNumber)
	'If NOT Session("SEARCH_REG" & "cheminv") OR (Application("RegServerName") = "NULL")then
	If (Application("RegServerName") = "NULL")then
		Response.Write "<center><BR><BR><BR><BR><span class=""GUIFeedback"">You have insufficient Privileges to view Registration Database<BR>Contact your system administrator.</span></center>"
		Response.end
	End if
	if CBool(Application("UseNotebookTable")) then
		'notebookSQL = " (SELECT notebook_name FROM notebooks,batches WHERE notebook_number = batches.notebook_internal_id) AS NoteBook,"
		notebookSQL = " (SELECT distinct notebooks.notebook_name FROM notebooks,inv_vw_reg_batches WHERE notebook_number = inv_vw_reg_batches.RegId and " & RegId & " ) AS notebook_text," 

	else
		notebookSQL = " batches.notebook_text AS NoteBook,"
	end if
	if CLng(RegID) = 0 then BatchNumber = 0 'CBOE-1370 SJ Setting BatchNumber to 0, so the sql query syntax may remain intact.
	strSQL = " Select *" &_
			" From cheminvdb2.inv_vw_reg_batches b, cheminvdb2.inv_vw_reg_structures s " &_
			" Where b.regid=" & RegID &_
			" AND b.batchnumber=" & BatchNumber &_
			" AND s.regid = b.regid"
	'Response.Write(strSQL)
	GetRegConnection()
	Set RS = Conn.Execute(strSQL)
	'Response.End

	'-- Write Reg values to Session
	'-- Write Reg values to local variables
	Session("RegID") = RegID
	RegID = Session("RegID")

	if not (RS.EOF or RS.BOF) then 
		for each key in reg_fields_dict
			'Response.Write(reg_fields_dict.item(key) & " : " & key & "<br>")
			Session(cStr(key)) = RS(cStr(key))
			execute(key & " = RS(cStr(key))")
		next
	end if 

	'-- Get synonyms
	strSQL = "SELECT a.identifier as RegSynonym FROM cheminvdb2.inv_vw_reg_altids a WHERE a.regid=" & RegID

	Set RS = Conn.Execute(strSQL)
	synList = " "
	while not RS.EOF
		synList = synList & RS("RegSynonym") & ","
		RS.movenext
	wend
	Session("RegSynonym") = trim(left(synList,len(synList)-1))
	RegSynonym = Session("RegSynonym")
End Sub

'SYAN added 12/16/2003 to fix CSBR-35466
'DJP added  2/13/2004 to support date_format sans core
Function ConvertStrToDate(date_format, dateStr)

	if dateStr <> "" then
		sections = split(dateStr, "/")

		Select case CStr(date_format)
			case "8"
				dateObj = DateSerial(CInt(sections(2)), CInt(sections(0)), CInt(sections(1)))
			case "9"
				dateObj = DateSerial(CInt(sections(2)), CInt(sections(1)), CInt(sections(0)))
			case "10"
				dateObj = DateSerial(CInt(sections(0)), CInt(sections(1)), CInt(sections(2)))
		End Select
	else
		dateObj = dateStr
	end if
		ConvertStrToDate = dateObj
End Function

'SYAN added 12/16/2003 to fix CSBR-35466
'DJP added 2/13/2004 to support date_format sans core
Function ConvertDateToStr(date_format, dateObj)    
	if IsNull(dateObj) = false then
	    Select case CStr(date_format)
			case "8"
				dateStr = Month(dateObj) & "/" & Day(dateObj) & "/" & Year(dateObj)
			case "9"
				dateStr = Day(dateObj) & "/" & Month(dateObj) & "/" & Year(dateObj)
			case "10"
				dateStr = Year(dateObj) & "/" & Month(dateObj) & "/" & Day(dateObj)
		End Select
	else
		dateStr = ""
	end if
	ConvertDateToStr = dateStr
End Function

Function GetLocalNowPlusDate( Days, Months, Years)
    dateObj = Now()
    TargetMonth = Month(dateObj) + Months 
    TargetDay = Day(dateObj) + Days
    TargetYear = Year(dateObj)+ Years
    while TargetMonth>12
        TargetMonth=TargetMonth-12
        TargetYear=TargetYear+1
    wend
   'isChanged is used to check if the month changed inside the while loop, 
   ' It is here for a sittuation of >31 days, where we need more than one itration.  
   isChanged= true
   while isChanged
       isChanged = false
        select case TargetMonth 
        case 1, 3, 5, 7, 8, 10, 12
            if TargetDay>31 then 
              TargetDay=TargetDay-31
              TargetMonth=TargetMonth+1
              isChanged= true
            end if
        case 4, 6, 9, 11     
             if TargetDay>30 then 
                  TargetDay=TargetDay-30
                  TargetMonth=TargetMonth+1
                  isChanged= true
             end if
        case 2           
             if isLeapYear(TargetYear) then 
                    if TargetDay>29 then 
                        TargetDay=TargetDay-29
                        TargetMonth=TargetMonth+1
                        isChanged= true
                    end if
             else 
                    if TargetDay =29 then 
                        TargetDay = TargetDay -1 
                        isChanged= true
                    Elseif TargetDay>28 then 
                        TargetDay=TargetDay-28
                        TargetMonth=TargetMonth+1
                        isChanged= true
                    end if
              end if 
      end select
      if TargetMonth>12 Then
         TargetMonth=TargetMonth-12
         TargetYear=TargetYear+1
      end if 
  wend 
 Select case CStr(Application("DATE_FORMAT"))
  case "8"
   dateStr = (TargetMonth) & "/" & (TargetDay) & "/" & (TargetYear)
  case "9"
   dateStr = (TargetDay) & "/" & (TargetMonth) & "/" & (TargetYear)
  case "10"
   dateStr = (TargetYear) & "/" & (TargetMonth) & "/" & (TargetDay)
 End Select 
 GetLocalNowPlusDate = dateStr
End Function

Function isLeapYear(ExpYear)
    If(ExpYear mod 400 = 0) or (ExpYear mod 100 <> 0 And ExpYear mod 4 = 0) then 
        isLeapYear = true
    else
        isLeapYear = false     
    end if   
End Function  

Sub ShowFormVars(bEnd)
	for each key in Request.Form
		Response.Write key & "=" & Request(key) & "<BR>"
	next
	if bEnd then
		Response.End
	end if
End Sub


Function IsBlank(ByRef TempVar)
    IsBlank = False
    Select Case VarType(TempVar)
        'Empty & Null
        Case 0, 1
            IsBlank = True
        'String
        Case 8
            If Len(TempVar) = 0 Then
                IsBlank = True
            End If
        'Object
        Case 9
            tmpType = TypeName(TempVar)
            If (tmpType = "Nothing") Or (tmpType = "Empty") Then
                IsBlank = True
            End If
        'Array
        Case 8192, 8204, 8209
            'does it have at least one element?
            If UBound(TempVar) = -1 Then
                IsBlank = True
            End If
    End Select
End Function

'****************************************************************************************
'*	PURPOSE: Prints out the parameter values of a command for debugging purposes
'*	INPUT: A command object, flag to end or not
'*	OUTPUT: Writes out a parameter on its own line
'****************************************************************************************
Function DebugCommand(ByRef Cmd, bEnd)
	For each p in Cmd.Parameters
		Response.Write p.name & " = " & p.value & "<BR>"
	Next
	if beEnd then Response.End
End Function

'****************************************************************************************
'*	PURPOSE: To Check and reset the Application("isRegBatch") variable, if the batching fields are from RegBatch 
'*	INPUT: A recordset from BATCH.GETBATCHFIELDS procedure
'****************************************************************************************
 Sub IsaRegBatch(RsSet)
 dim isREG_ID, isBATCH_NUMBER, isRecordExits
    isREG_ID=0
    isBATCH_NUMBER=0
    isRecordExits=0
    if  not (RsSet.EOF or RsSet.BOF) then   isRecordExits=1 
   
     while not RsSet.EOF
        if RsSet("field_name")="REG_ID_FK" then isREG_ID=1
        if RsSet("field_name")="BATCH_NUMBER_FK" then isBATCH_NUMBER=1
        RsSet.MoveNext
    wend
    Application("isRegBatch")=0
    if (isREG_ID=1) and (isBATCH_NUMBER=1) then Application("isRegBatch")=1
    if isRecordExits then RsSet.MoveFirst
End Sub

'-------------------------------------------------------------------------------
' Name: CFWLogAction(inputstr)
' Type: Sub
' Purpose:  writes imformation to a output file 
' Inputs:   inputstr  as string - variable to output
' Returns:	none
' Comments: writes informtion to /inetpub/cfwlog.txt file
'-------------------------------------------------------------------------------
Sub CFWLogAction(ByVal inputstr)
		on error resume next
		filepath = Application("ServerDrive") & "\" & Application("ServerRoot") & "\cfwlog.txt"
		Set fs = Server.CreateObject("Scripting.FileSystemObject")
		Set a = fs.OpenTextFile(filepath, 8, True)  
		a.WriteLine Now & ": " & inputstr
		a.WriteLine " "
		a.close
		on error goto 0
End Sub
'Show a meaningful error message 
Sub ShowConnectionError(ServerKey)
   	   Response.Write 	"<br><br><center><b><font color=""maroon"" >Requested Operation cannot be completed.<br>Please contact your system administrator.</font></b><br><br> Reason:Invalid "  & ServerKey & " in Invconfig.ini file.  <br>"
	   Response.Write  	"<br><br> <a href=""#"" onclick=""opener.focus();window.close(); return false;""><img SRC=""/cheminv/graphics/sq_btn/ok_dialog_btn.gif"" border=""0""></a> </center>"
End Sub
'Function to Decode the encrypted string 
Function URLDecode(str) 
	str = Replace(str, "+", " ") 
    For i = 1 To Len(str) 
		sT = Mid(str, i, 1) 
        If sT = "%" Then 
			If i+2 < Len(str) Then 
				sR = sR & _ 
                Chr(CLng("&H" & Mid(str, i+1, 2))) 
                i = i+2 
            End If 
        Else 
			sR = sR & sT 
        End If 
   Next 
   URLDecode = sR 
End Function 
'****************************************************************************************
'*	PURPOSE: To convert the dateformat for ELN singleclick, since ELN pass only in DD/MM/YYYY. 
'*           it will convert the format to dd/mm/yyyy while passing to ELN 
'*           and for Inventory it will convert in format defined in INI
'*	INPUT: datestring and bool (true - if it is passed to ELN and false - to convert to INI format)
'****************************************************************************************
Function convertDateFormatELN(strDate)
    ArrCreateDate = split(strDate,"/")
    Select case CStr(Application("DATE_FORMAT"))
    case "8"
	    convertDateFormatELN = ArrCreateDate(1) & "/" & ArrCreateDate(0) & "/" & ArrCreateDate(2)
    case "9"
	    ' no changes needed
        convertDateFormatELN = strDate
    case "10"
	    convertDateFormatELN = ArrCreateDate(2) & "/" & ArrCreateDate(1) & "/" & ArrCreateDate(0)
    End Select
    End Function 
</SCRIPT>
