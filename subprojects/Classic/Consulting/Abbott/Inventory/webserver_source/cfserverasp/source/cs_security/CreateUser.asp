<!--#INCLUDE VIRTUAL = "/cfserverasp/source/ado.inc"-->
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/cs_security/cs_security_utils_vbs.asp"-->
<Script RUNAT="Server" Language="VbScript">
'Copyright 1999-2003 CambridgeSoft Corporation. All rights reserved

Dim Conn
Dim Cmd
Dim strError
Dim bWriteError
Dim PrintDebug

bDebugPrint = false
bWriteError = False
strError = "Error:CreateUser<BR>"

dbKey = Request("dbKey")
UserName = Request("UserName")
IsAlreadyInOracle = Request("IsAlreadyInOracle")
Password = Request("Password")
RolesGranted = Request("RolesGranted")
FirstName = Request("FirstName")
MiddleName = Request("MiddleName")
LastName = Request("LastName")
Email = Request("Email")
Telephone = Request("Telephone")
Address = Request("Address")
UserCode = Request("UserCode")
SupervisorID = Request("SupervisorID")
SiteID = Request("SiteID")
Active = Request("Active") 

if UCase(Application("AUTHENTICATION_MODE")) = "LDAP" then
	Password = GeneratePwd(UserName)
end if
 
 
' Redirect to help page if no parameters are passed
If Len(Request.QueryString) = 0 AND Len(Request.Form)= 0 then
	Response.Redirect "/cfserverasp/help/admin/api/CreateUser.htm"
	Response.end
End if

'Echo the input parameters if requested
If false then
	Response.Write "FormData=" & Request.form & "<BR>QueryString = " & Request.QueryString
	'Response.end
End if

' Check for required parameters
If IsEmpty(UserName) then
	strError = strError & "User Name is a required parameter<BR>"
	bWriteError = True
End if
If IsEmpty(LastName) then
	strError = strError & "Last Name is a required parameter<BR>"
	bWriteError = True
End if
If IsEmpty(Password) then
	strError = strError & "Password is a required parameter<BR>"
	bWriteError = True
End if
If IsEmpty(RolesGranted) then
	strError = strError & "RolesGranted is a required parameter<BR>"
	bWriteError = True
End if

if IsEmpty(IsAlreadyInOracle) OR IsAlreadyInOracle = "" then IsAlreadyInOracle = 0
if IsEmpty(FirstName) OR FirstName = "" then FirstName = NULL
if IsEmpty(MiddleName) OR MiddleName = "" then MiddleName = NULL
if IsEmpty(Email) OR Email = "" then Email = NULL
if IsEmpty(Telephone) OR Telephone = "" then Telephone = NULL
if IsEmpty(Address) OR Address = "" then Address = NULL
if IsEmpty(UserCode) OR UserCode = "" then UserCode = NULL
if IsEmpty(SupervisorID) OR SupervisorID = "" then SupervisorID = NULL
if IsEmpty(SiteID) OR SiteID = "" then SiteID = NULL
if IsEmpty(Active) OR Active = "" OR Active = "0" then Active = -1


If bWriteError then
	' Respond with Error
	Response.Write strError
	Response.end
End if

' Set up and ADO command
Set Conn = GetCS_SecurityConnection(dbKey)
Set Cmd = GetCommand(Conn, "CS_SECURITY.CREATEUSER", adCmdStoredProc)
Cmd.Parameters.Append Cmd.CreateParameter("RETURN_VALUE",200, adParamReturnValue, 100, NULL)
Cmd.Parameters("RETURN_VALUE").Precision = 8			
Cmd.Parameters.Append Cmd.CreateParameter("PUSERNAME", 200, adParamInput, 50, Ucase(UserName)) 
Cmd.Parameters.Append Cmd.CreateParameter("PISALREADYINORACLE", 5, adParamInput, 0, IsAlreadyInOracle) 
Cmd.Parameters.Append Cmd.CreateParameter("PASSWORD", 200, adParamInput, 30, """" & Password & """") 
Cmd.Parameters.Append Cmd.CreateParameter("PROLESGRANTED", 200, adParamInput, 2000, RolesGranted)
Cmd.Parameters.Append Cmd.CreateParameter("PFirstName", 200, adParamInput, 50, FirstName)
Cmd.Parameters.Append Cmd.CreateParameter("PMiddleName", 200, adParamInput, 50, MiddleName)
Cmd.Parameters.Append Cmd.CreateParameter("PLastName", 200, adParamInput, 50, LastName)
Cmd.Parameters.Append Cmd.CreateParameter("PTelephone", 200, adParamInput, 50, Telephone)
Cmd.Parameters.Append Cmd.CreateParameter("PEmail", 200, adParamInput, 50, Email)
Cmd.Parameters.Append Cmd.CreateParameter("PAddress", 200, adParamInput, 50, Address)
Cmd.Parameters.Append Cmd.CreateParameter("PUserCode", 200, adParamInput, 50, Ucase(UserCode))  
Cmd.Parameters.Append Cmd.CreateParameter("PSupervisorID", 131, adParamInput, 0, SupervisorID)
Cmd.Parameters.Append Cmd.CreateParameter("PSiteID", 131, adParamInput, 0, SiteID)
Cmd.Parameters.Append Cmd.CreateParameter("PActive", 5, adParamInput, 0, Active)

if bDebugPrint then
	For each p in Cmd.Parameters
		Response.Write p.name & " = " & p.value & "<BR>"
	Next	
Else
	Call ExecuteCmd("CS_SECURITY.CreateUser")
End if

' Return code
Response.Write Cmd.Parameters("RETURN_VALUE")

'Clean up
Conn.Close
Set Conn = Nothing
Set Cmd = Nothing
</SCRIPT>
