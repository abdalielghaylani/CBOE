<%@ EnableSessionState=False Language=VBScript%>
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/ado.inc"-->
<!--#INCLUDE VIRTUAL = "/cheminv/api/apiUtils.asp"-->
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/cs_security/cs_security_login_utils_vbs.asp"-->
<SCRIPT RUNAT="Server" Language="VbScript">
Dim Conn
Dim Cmd
Dim strError
Dim bWriteError
Dim bDebugPrint
Dim oReturnXML

bTimer = False
if bTimer then theStart = timer	
bDebugPrint = False
bWriteError = False
'strError = "Error:AuthenticateUser<BR>"
strError = "<?xml version = '1.0'?><ERRORS>"
Response.ContentType = "text/xml"

' Redirect to help page if no parameters are passed
If Len(Request.QueryString) = 0 AND Len(Request.Form)= 0 then
	Response.Redirect "/cheminv/help/admin/api/AuthenticateUser.htm"
	Response.end
End if

UserId = Request("UserID")
Password = Request("Password")
PrivTables = Request("PrivTables")

' Check for required parameters
If IsEmpty(UserID) or UserID = "" then
	strError = strError & "<ERROR>UserID is a required parameter</ERROR>"
	bWriteError = True
End if
If IsEmpty(Password) or Password = "" then
	strError = strError & "<ERROR>Password is a required parameter</ERROR>"
	bWriteError = True
End if
If IsEmpty(PrivTables) or PrivTables = "" then
	strError = strError & "<ERROR>PrivTables is a required parameter</ERROR>"
	bWriteError = True
End if
If bWriteError then
	' Respond with Error
	Response.Write strError & "</ERRORS>"
	Response.end
End if

'CSBR ID     : 128893
'Modified by : sjacob
'Comment     : Adding code to authenticate LDAP users.
'Start of change
IsLDAPUser = IsValidLDAPUser(UserId, Password)

If IsLDAPUser = 0 Then 'non LDAP users
    ON ERROR RESUME NEXT
    ConnStr = "FILE NAME=" & Application("CS_SECURITY_UDL_PATH") & ";User ID=" & UserID & ";Password=" & Password
    'Response.Write "ConnString=" & ConnStr
    'Response.End
    Set Conn = Server.CreateObject("ADODB.Connection")
    Conn.Open ConnStr
ElseIf IsLDAPUser = 1 Then 'LDAP User
    ON ERROR RESUME NEXT
    Password = GeneratePwd(UserId)
    ConnStr = "FILE NAME=" & Application("CS_SECURITY_UDL_PATH") & ";User ID=" & UserID & ";Password=" & Password
    'Response.Write "ConnString=" & ConnStr
    'Response.End
    Set Conn = Server.CreateObject("ADODB.Connection")
    Conn.Open ConnStr        
End If
'End of change

'validate user
If Err.number <> 0 then
	bValidUser = false
	oraErrorDescription = Err.Description
	oraErrorNumber = Mid(oraErrorDescription, InStr(1, oraErrorDescription, "ORA-")+4,5) 
	Select Case oraErrorNumber
		Case "28001"
			strError = strError & "<ERROR title=""Connection Error for user " & UserID & ": "  & Err.Description & """>Your Oracle password has expired.<BR>Contact your system administrator.</ERROR>"
		Case "28000"
			strError = strError & "<ERROR title=""Connection Error for user " & UserID & ": "  & Err.Description & """>Your Oracle account is locked.<BR>Contact your system administrator.</ERROR>"
		Case "28002" 'Seems like OLEDB provider never raises this exception :(
			strError = strError & "<ERROR title=""Connection Error for user " & UserID & ": "  & Err.Description & """>Your Oracle password has expired.<BR>Contact your system administrator.</ERROR>"
		Case "01017"
			strError = strError & "<ERROR title=""Connection Error for user " & UserID & ": "  & Err.Description & """>Invalid username/password.  Logon denied.</ERROR>"	
		Case "12154"
			strError = strError & "<ERROR title=""Connection Error for user " & UserID & ": "  & Err.Description & ". Check the cs_security.udl file"">Invalid Oracle service name.</ERROR>"	
		Case Else
			strError = strError & "<ERROR title=""Connection Error for user " & UserID & ": "  & Err.Description & """>Connection Error: " & Err.Description & "</ERROR>"			
	End Select
Else
	' The OLEDB  provider does not seem to reliably raise the Password Expired Exception even with PwdChgDlg=0
	' connection string attribute. So we check expiration. 
	Set RS = Conn.execute("select 1 from user_users where account_status LIKE 'EXPIRED%'")
	if NOT (RS.EOF AND RS.BOF) then
		strError = strError & "<ERROR title=""Connection Error for user " & UserID & ": "  & Err.Description & """>Your Oracle password has expired.<BR>Contact your system administrator.</ERROR>"
	End if
	bValidUser = True
End if 
if bTimer then
	theEnd = timer	
	trace theEnd-theStart&":validUserTotal", 5
end if

if bValidUser then
	Conn.Close
	ConnStr = "FILE NAME=" & Application("CS_SECURITY_UDL_PATH") & ";User ID=" & Application("SEC_USERNAME") & ";Password=" & Application("SEC_PWD")
	'Response.Write "ConnString=" & ConnStr
	'Response.End
	Conn.Open ConnStr
	'Build priv xml
	'SQL =  "SELECT DBMS_XMLQUERY.getXML('SELECT * FROM " & PrivTables & " WHERE ROLE_INTERNAL_ID IN (SELECT distinct s.role_id FROM security_roles s, privilege_tables p, dba_role_privs rp WHERE s.privilege_table_int_id = p.privilege_table_id AND s.role_name = rp.granted_role AND Upper(p.privilege_table_name) = ''" & ucase(PrivTables) & "'' AND rp.granted_role IN (select granted_role	from dba_role_privs	WHERE grantee = ''" & ucase(UserID) & "'' UNION Select granted_role from dba_role_privs WHERE grantee IN (select granted_role from dba_role_privs WHERE  grantee = ''" & ucase(UserID) & "'')))') as privXML from dual"
	'this works for 8i and 9i
	SQL = "SELECT DBMS_XMLQUERY.getXML(DBMS_XMLQUERY.newContext('SELECT * FROM " & PrivTables & " WHERE ROLE_INTERNAL_ID IN (SELECT distinct s.role_id FROM security_roles s, privilege_tables p, dba_role_privs rp WHERE s.privilege_table_int_id = p.privilege_table_id AND s.role_name = rp.granted_role AND Upper(p.privilege_table_name) = ''" & ucase(PrivTables) & "'' AND rp.granted_role IN (select granted_role	from dba_role_privs	WHERE grantee = ''" & ucase(UserID) & "'' UNION Select granted_role from dba_role_privs WHERE grantee IN (select granted_role from dba_role_privs WHERE  grantee = ''" & ucase(UserID) & "'')))')) as privXML from dual"
	Set Cmd = Server.CreateObject("ADODB.Command")
	'Cmd.ActiveConnection = Conn
	'Cmd.CommandType = adCmdText
	'Cmd.CommandText = SQL
	'Cmd.Properties ("PLSQLRSet") = TRUE  
	'Set RS = Cmd.Execute
	if bTimer then
		theEnd = timer	
		trace theEnd-theStart&":beforeexcecute", 5
	end if
	Set RS = Conn.Execute(SQL)
	if bTimer then
		theEnd = timer	
		trace theEnd-theStart&":afterexcecute", 5
	end if
	if Not(RS.BOF AND RS.EOF) THEN
		privXML = RS("privXML")	
		Set oReturnXML = server.CreateObject("MSXML2.DOMDOCUMENT.6.0")
		oReturnXML.loadXML(privXML)
		Set oDocElement = oReturnXML.documentElement
		oDocElement.setAttribute "ORASERVICENAME", Application("ORA_SERVICENAME")
		oDocElement.setAttribute "INVSCHEMANAME", Application("ORASCHEMANAME")
		theReturn = oReturnXML.xml
		'Response.Write oReturnXML.xml
	else
		strError = strError & "<ERROR>No Privileges found for assigned roles.</ERROR>"
		strError = strError & "</ERRORS>"
		theReturn = strError
		'Response.Write strError & "</ERRORS>"
	end if	
else
	strError = strError & "</ERRORS>"
	theReturn = strError
	'Response.Write strError & "</ERRORS>"
end if

Response.Write theReturn
trace theReturn, 15

if bTimer then
	theEnd = timer	
	trace theEnd-theStart&":authenticateTotal", 5
end if


ServerTraceLevel= 20
if Application("CfwTraceLevel") <> "" then tracelevel = Application("CfwTraceLevel")
if ServerTraceLevel <> 0 then tracelevel = ServerTraceLevel

'-------------------------------------------------------------------------------
' Name: Trace(ByVal inputstr, Level)
' Type: Sub
' Purpose:  conditionaly writes imformation to cfwlog.txt file depending on TraceLevel 
' Inputs:   inputstr  as string - variable to output, Level at which tracing occurs
' Returns:	none
' Comments: TraceLevel is set by page scope variable at the top of this file.
'-------------------------------------------------------------------------------
Sub Trace(ByVal inputstr, Level)
	on error resume next 'put this here so activity is not prevented if this file is locked.
		if Level <= traceLevel then
			filepath = Application("ServerDrive") & "\" & Application("ServerRoot") & "\cfwlog.txt"
			Set fs = Server.CreateObject("Scripting.FileSystemObject")
			Set a = fs.OpenTextFile(filepath, 8, True)  
			a.WriteLine Now & "|level " & Level & "|" & inputstr
			a.WriteLine " "
			a.close
		end if
End Sub


</SCRIPT>