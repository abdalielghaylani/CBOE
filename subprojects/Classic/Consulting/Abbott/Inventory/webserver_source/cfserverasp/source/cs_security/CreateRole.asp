<!--#INCLUDE VIRTUAL = "/cfserverasp/source/ado.inc"-->
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/cs_security/cs_security_utils_vbs.asp"-->
<Script RUNAT="Server" Language="VbScript">
'Copyright 1999-2003 CambridgeSoft Corporation. All rights reserved
Dim Conn
Dim SEC_Conn
Dim DBA_Conn
Dim Cmd
Dim strError
Dim bWriteError
Dim PrintDebug
Dim bEcho

bEcho = false
bDebugPrint = false
bWriteError = False
strError = "Error:CreateRole<BR>"
dbKey = Request("dbKey")
RoleName = Request("RoleName")
IsAlreadyInOracle = Request("IsAlreadyInOracle")
PrivValueList= Request("PrivValueList")
if not isempty(Request("RID")) then
	PrivValueList= PrivValueList & ",'" & Request("RID") & "'" & ",'" & dbKey &"'" & ",TO_DATE('" & Now() & "','mm/dd/yyyy:hh:mi:ssam')"
end if
PrivNamesList= Request("PrivNamesList")
'CSSPrivValueList= Request("CSSPrivValueList")
PrivTableName = Request("PrivTableName")

'AllPrivValuesList = PrivValueList & ", " & CSSPrivValueList
PrivNamesArray = split(PrivNamesList, ",")
PrivValueArray = split(replace(PrivValueList, "'", ""), ",")


' Redirect to help page if no parameters are passed
If Len(Request.QueryString) = 0 AND Len(Request.Form)= 0 then
	Response.Redirect "/cfserverasp/help/admin/api/CreateRole.htm"
	Response.end
End if

'Echo the input parameters if requested
If bEcho then
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
If IsEmpty(PrivValueList) then
	strError = strError & "PrivValueList is a required parameter<BR>"
	bWriteError = True
End if
'If IsEmpty(CSSPrivValueList) then
'	strError = strError & "CSSPrivValueList is a required parameter<BR>"
'	bWriteError = True
'End if
if IsEmpty(IsAlreadyInOracle) OR IsAlreadyInOracle = "" OR lcase(IsAlreadyInOracle) = "false" then 
	IsAlreadyInOracle = 0
Else
	IsAlreadyInOracle = 1
End if
if IsEmpty(FirstName) OR FirstName = "" then FirstName = NULL



If bWriteError then
	' Respond with Error
	Response.Write strError
	Response.end
End if

' Set up and ADO command
Set Conn = GetCS_SecurityConnection(dbKey)
Set Cmd = GetCommand(Conn, "CS_SECURITY.CREATEROLE", adCmdStoredProc)
Cmd.Parameters.Append Cmd.CreateParameter("RETURN_VALUE",200, adParamReturnValue, 100, NULL)			
Cmd.Parameters.Append Cmd.CreateParameter("PRoleNAME", 200, adParamInput, 50, RoleName) 
Cmd.Parameters.Append Cmd.CreateParameter("PPrivTableName", 200, adParamInput, 50, PrivTableName) 
Cmd.Parameters.Append Cmd.CreateParameter("PISALREADYINORACLE", 5, adParamInput, 0, IsAlreadyInOracle) 
Cmd.Parameters.Append Cmd.CreateParameter("PPrivValueList", 200, adParamInput, 2000, PrivValueList) 
'Cmd.Parameters.Append Cmd.CreateParameter("PCSSPrivValueList", 200, adParamInput, 2000, CSSPrivValueList)
if bDebugPrint then
	For each p in Cmd.Parameters
		Response.Write p.name & " = " & p.value & "<BR>"
	Next	
Else
	Call ExecuteCmd("CS_SECURITY.CreateRole")
End if

RC = Cmd.Parameters("RETURN_VALUE")

if IsNumeric(RC) then
	If RC < 0 then 
		Response.Write RC
		Response.End
	End if
Else
	Response.Write RC
	Response.End	
End if

Set Cmd = Nothing
Set Cmd = GetCommand(Conn, "CS_SECURITY.MapPrivsToRole", adCmdStoredProc)
Cmd.Parameters.Append Cmd.CreateParameter("PROLENAME", 200, adParamInput, 50, "") 
Cmd.Parameters.Append Cmd.CreateParameter("PPRIVNAME", 200, adParamInput, 30, "") 
Cmd.Parameters.Append Cmd.CreateParameter("PACTION", 200, adParamInput, 6, "") 
For i = 0 to Ubound(PrivNamesArray)
	if Trim(PrivValueArray(i)) = "1" then
		Cmd(0) = RoleName 
		Cmd(1) = PrivNamesArray(i) 
		Cmd(2) = "GRANT"
		Call ExecuteCmd("CS_SECURITY.MapPrivsToRole")
	End if	
Next 

Response.Write RC

'Clean up
Conn.Close
Set Conn = Nothing
Set Cmd = Nothing

</SCRIPT>
