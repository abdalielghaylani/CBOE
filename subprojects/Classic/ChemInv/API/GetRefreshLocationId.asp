<%@ EnableSessionState=False Language=VBScript%>
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/ado.inc"-->
<!--#INCLUDE VIRTUAL = "/cheminv/api/apiUtils.asp"-->
<script runat="Server" language="VbScript">
Dim Conn
Dim Cmd
Dim bWriteError
Dim bDebugPrint

'-- Get the appropriate location_id to select in the tree view

bDebugPrint = False
bWriteError = False
strError = "Error:GetRefreshLocationId<BR>"

locationId = Request("locationId")
CsUserName = Application("CHEMINV_USERNAME") 
CsUserID = Application("CHEMINV_PWD")

'-- Redirect to help page if no parameters are passed
If Len(Request.QueryString) = 0 AND Len(Request.Form)= 0 then
	Response.Redirect "/cheminv/help/admin/api/GetRefreshLocationId.htm"
	Response.end
End if

'-- Check for required parameters
If IsEmpty(locationId) then
	strError = strError & "LocationId is a required parameter<BR>"
	bWriteError = True
End if
If bWriteError then
	'-- Respond with Error
	Response.Write strError
	Response.end
End if


GetInvConnection()

Call GetInvCommand(Application("CHEMINV_USERNAME") & ".GUIUtils.GetRefreshLocationID", adCmdStoredProc)
Cmd.Parameters.Append Cmd.CreateParameter("RETURN_VALUE",131, adparamreturnvalue, 0, NULL)
Cmd.Parameters.Append Cmd.CreateParameter("PLOCATIONID",131, 1, 0, locationId)
if bDebugPrint then
	For each p in Cmd.Parameters
		Response.Write p.name & " = " & p.value & "<BR>"
	Next
	Response.End	
Else
	Call ExecuteCmd(Application("CHEMINV_USERNAME") & ".GUIUtils.GetRefreshLocatoinID")
end if
	
out = trim(Cstr(Cmd.Parameters("RETURN_VALUE")))

Conn.Close
Set Conn = nothing
Set Cmd = nothing
Set RS = nothing

Response.Write out

</script>
