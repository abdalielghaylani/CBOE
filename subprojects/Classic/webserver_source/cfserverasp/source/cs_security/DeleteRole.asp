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
strError = "Error:DeleteRole<BR>"
dbKey = Request("dbKey")
RoleName = Request("RoleName")
PrivTableName = Request("PrivTableName")
 
 
' Redirect to help page if no parameters are passed
If Len(Request.QueryString) = 0 AND Len(Request.Form)= 0 then
	Response.Redirect "/cfserverasp/help/admin/api/DeleteRole.htm"
	Response.end
End if

'Echo the input parameters if requested
If NOT isEmpty(Request.QueryString("Echo")) then
	Response.Write "FormData = " & Request.form & "<BR>QueryString = " & Request.QueryString
	Response.end
End if

' Check for required parameters
If IsEmpty(RoleName) then
	strError = strError & "Role Name is a required parameter<BR>"
	bWriteError = True
End if
If IsEmpty(PrivTableName) then
	strError = strError & "PrivTableName is a required parameter<BR>"
	bWriteError = True
End if

If bWriteError then
	' Respond with Error
	Response.Write strError
	Response.end
End if

' Set up and ADO command
Set Conn = GetCS_SecurityConnection(dbKey)
Set Cmd = GetCommand(Conn, "CS_SECURITY.DELETEROLE", adCmdStoredProc)
Cmd.Parameters.Append Cmd.CreateParameter("RETURN_VALUE",200, adParamReturnValue, 100, NULL)		
Cmd.Parameters.Append Cmd.CreateParameter("PRoleNAME", 200, adParamInput, 50, RoleName) 
Cmd.Parameters.Append Cmd.CreateParameter("PPrivTableName", 200, adParamInput, 50, PrivTableName)  

if bDebugPrint then
	For each p in Cmd.Parameters
		Response.Write p.name & " = " & p.value & "<BR>"
	Next	
Else
	Call ExecuteCmd("CS_SECURITY.DeleteRole")
End if

' Return code
Response.Write Cmd.Parameters("RETURN_VALUE")

'Clean up
Conn.Close
Set Conn = Nothing
Set Cmd = Nothing
</SCRIPT>
