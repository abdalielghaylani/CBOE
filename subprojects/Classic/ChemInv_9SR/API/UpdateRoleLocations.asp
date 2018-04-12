<%@ EnableSessionState=False Language=VBScript%>
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/ado.inc"-->
<!--#INCLUDE VIRTUAL = "/cheminv/api/apiUtils.asp"-->
<Script RUNAT="Server" Language="VbScript">
Dim Conn
Dim Cmd
Dim strError
Dim bWriteError
Dim bDebugPrint

bDebugPrint = false
bWriteError = false
strError = "Error:UpdateRoleLocations<BR>"

roleID = Request("roleID")
locationID = Request("locationID")

' Redirect to help page if no parameters are passed
If Len(Request.QueryString) = 0 AND Len(Request.Form)= 0 then
	Response.Redirect "/cheminv/help/admin/api/UpdateRoleLocations.htm"
	Response.end
End if

'Echo the input parameters if requested
If NOT isEmpty(Request.QueryString("Echo")) then
	Response.Write "FormData = " & Request.form & "<BR>QueryString = " & Request.QueryString
	Response.end
End if

' Check for required parameters
If IsEmpty(RoleID) then
	strError = strError & "RoleID is a required parameter<BR>"
	bWriteError = True
End if
If IsEmpty(LocationID) then
	strError = strError & "LocationID is a required parameter<BR>"
	bWriteError = True
End if
'Response.Write(dataMapName & "<br>" & dataMapTypeID & "<br>"& dataMapComments & "<br>Header Rows:[" & numHeaderRows & "]<br>" & numColumns & "<br>" & columnDelimiter & "<br>"& dataMapFieldList & "<br>" & dataMapColumnList)
'Response.end
' Set up and ADO command
Call GetInvCommand(Application("CHEMINV_USERNAME") & ".ManageRLS.UpdateRoleLocations", adCmdStoredProc)
'Cmd.Parameters.Append Cmd.CreateParameter("RETURN_VALUE",200, adParamReturnValue, 200, NULL)
Cmd.Parameters.Append Cmd.CreateParameter("pRoleID",200 , 1, 55, roleID)
Cmd.Parameters.Append Cmd.CreateParameter("pLocationIDs",200, 1, 2000, locationID)
if bDebugPrint then
	For each p in Cmd.Parameters
		Response.Write p.name & " = " & p.value & "<BR>"
	Next	
Else
	Call ExecuteCmd(Application("CHEMINV_USERNAME") & ".ManageRLS.UpdateRoleLocations")
End if

' Return the newly created LocationID
'Response.Write Cmd.Parameters("RETURN_VALUE")
if err.number = 0 then
	Response.Write "1"
else
	Response.write "0"
end if


'Clean up
Conn.Close
Set Conn = Nothing
Set Cmd = Nothing
</SCRIPT>
