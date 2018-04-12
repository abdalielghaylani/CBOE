<%@ EnableSessionState=False Language=VBScript%>
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/ado.inc"-->
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/server_const_vbs.asp"-->
<!--#INCLUDE VIRTUAL = "/cheminv/api/apiUtils.asp"-->
<Script RUNAT="Server" Language="VbScript">
Dim Conn
Dim Cmd
Dim strError
Dim bWriteError
Dim PrintDebug


bDebugPrint = False
bWriteError = False
strError = "Error:UpdateOrganization<br />"

p_OrgUnitID = Request("p_OrgUnitID")
p_Name = Request("p_Name")
p_OrgType = Request("p_OrgType")
p_Users = Request("p_Users")
p_Roles = Request("p_Roles")

' Redirect to help page if no parameters are passed
If Len(Request.QueryString) = 0 AND Len(Request.Form)= 0 then
	Response.Redirect "/cheminv/help/admin/api/UpdateOrganization.htm"
	Response.end
End if

'Echo the input parameters if requested
If NOT isEmpty(Request.QueryString("Echo")) then
	Response.Write "FormData = " & Request.form & "<BR>QueryString = " & Request.QueryString
	Response.end
End if

' Check for required parameters
If IsEmpty(p_OrgUnitID) then
	strError = strError & "p_OrgUnitID is a required parameter<BR>"
	bWriteError = True
End if
If IsEmpty(p_Name) then
	strError = strError & "p_Name is a required parameter<BR>"
	bWriteError = True
End if
If IsEmpty(p_OrgType) then
	strError = strError & "p_OrgType is a required parameter<BR>"
	bWriteError = True
End if

if p_Users = "" then p_Users = NULL
if p_Roles = "" then p_Roles = NULL

' Set up and ADO command
Call GetInvCommand(Application("CHEMINV_USERNAME") & ".ORGANIZATION.UPDATEORGANIZATION", adCmdStoredProc)
Cmd.Parameters.Append Cmd.CreateParameter("RETURN_VALUE",adNumeric, adParamReturnValue, 0, NULL)
Cmd.Parameters("RETURN_VALUE").Precision = 9
Cmd.Parameters.Append Cmd.CreateParameter("p_OrgUnitID",131, 1, 0, p_OrgUnitID)
Cmd.Parameters("p_OrgUnitID").Precision = 9
Cmd.Parameters.Append Cmd.CreateParameter("p_OrgName", 200, 1, 4000, p_Name)
Cmd.Parameters.Append Cmd.CreateParameter("p_OrgType",131, 1, 0, p_OrgType)
Cmd.Parameters("p_OrgType").Precision = 9
Cmd.Parameters.Append Cmd.CreateParameter("p_Users", 200, 1, 4000, p_Users)
Cmd.Parameters.Append Cmd.CreateParameter("p_Roles", 200, 1, 4000, p_Roles)
if bDebugPrint then
	For each p in Cmd.Parameters
		Response.Write p.name & " = " & p.value & "<BR>"
	Next	
Else
	Call ExecuteCmd(Application("CHEMINV_USERNAME") & ".ORGANIZATION.UPDATEORGANIZATION")
End if

' Return the newly created LocationID
Response.Write Cmd.Parameters("RETURN_VALUE")

'Clean up
Conn.Close
Set Conn = Nothing
Set Cmd = Nothing
</SCRIPT>
