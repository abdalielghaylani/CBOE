<%@ EnableSessionState=False Language=VBScript%>
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/ado.inc"-->
<!--#INCLUDE VIRTUAL = "/cheminv/gui/guiUtils.asp"-->
<!--#INCLUDE VIRTUAL = "/cheminv/api/apiUtils.asp"-->
<Script RUNAT="Server" Language="VbScript">
Dim Conn
Dim ConStr
Dim strError
Dim bWriteError
Dim PrintDebug
Dim CSUserName
Dim CSUserID

Response.Expires = -1

bDebugPrint = false
bWriteError = False
strError = "Error:GetOpenRackPositions<BR>"
RackID = Request("RackID")
StartPos = Request("StartPos")
if isBlank(StartPos) then StartPos = 0

CsUserName = Application("CHEMINV_USERNAME") 
CsUserID = Application("CHEMINV_PWD")

' Redirect to help page if no parameters are passed
If Len(Request.QueryString) = 0 AND Len(Request.Form)= 0 then
	Response.Redirect "/cheminv/help/admin/api/GetOpenRackPositions.htm"
	Response.end
End if

' Check for required parameters
If IsEmpty(RackID) then
	strError = strError & "RackID is required<br>"
	bWriteError = True
End if

If bWriteError then
	' Respond with Error
	Response.Write strError
	Response.end
End if

strStartPos = ""
if StartPos > 0 then strStartPos = " and gl.location_id >= " & StartPos
strSQL = "select gl.location_id from inv_vw_grid_location gl where parent_id=" & RackID & " and gl.location_id not in (select location_id_fk from inv_containers where location_id_fk=gl.location_id) and gl.location_id not in (select location_id_fk from inv_plates where location_id_fk=gl.location_id) and gl.location_id not in (select parent_id from inv_locations where parent_id=gl.location_id)" & strStartPos & " order by gl.row_index,gl.col_index"

if bdebugPrint then
	Response.Write strSQL
	Response.end
else
	'Response.Write(strSQL)
	'Response.End
	Response.write GetListFromSQL(strSQL)
end if
</SCRIPT>
