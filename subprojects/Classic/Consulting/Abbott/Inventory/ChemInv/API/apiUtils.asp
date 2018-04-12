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
	Elseif CSUserName <> "" then
		UserName = CSUserName
		UserID = CSUserID
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
	Dim UserName
	Dim UserID

	'UserName = Session("UserName" & "cheminv")
	'UserID = Session("UserID" & "cheminv")
	UserName = "Admin"
	Password = ""
	connection_array = Application("base_connection" & "invacx")
	ConnStr =  connection_array(0) & "="  & connection_array(1) & ";" & Application("UserIDKeyword") & "=" & UserName & ";" & Application("PWDKeyword") & "=" & UserID
	'Response.Write "<BR><BR><BR><BR>" & ConnStr
	'Response.end
	Set Conn = Server.CreateObject("ADODB.Connection")
	Conn.Open ConnStr
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
		notebookSQL = " (SELECT notebook_name FROM notebooks WHERE notebook_number = batches.notebook_internal_id) AS NoteBook,"
	else
		notebookSQL = " batches.notebook_text AS NoteBook,"
	end if

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
	for each key in reg_fields_dict
		'Response.Write(reg_fields_dict.item(key) & " : " & key & "<br>")
		Session(cStr(key)) = RS(cStr(key))
		execute(key & " = RS(cStr(key))")
	next


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


</SCRIPT>
