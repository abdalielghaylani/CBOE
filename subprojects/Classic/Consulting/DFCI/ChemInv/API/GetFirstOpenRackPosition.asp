<%@ EnableSessionState=False Language=VBScript%>
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/ado.inc"-->
<!--#INCLUDE VIRTUAL = "/cheminv/gui/guiUtils.asp"-->
<!--#INCLUDE VIRTUAL = "/cheminv/api/apiUtils.asp"-->
<Script RUNAT="Server" Language="VbScript">
Dim Conn
Dim RS
Dim ConStr
Dim strError
Dim bWriteError
Dim PrintDebug
Dim CSUserName
Dim CSUserID

bDebugPrint = False
bWriteError = False
strError = "Error:GetLocationFromID<BR>"
LocationID = Request("LocationID")

CsUserName = Application("CHEMINV_USERNAME") 
CsUserID = Application("CHEMINV_PWD")

' Redirect to help page if no parameters are passed
If Len(Request.QueryString) = 0 AND Len(Request.Form)= 0 then
	Response.Redirect "/cheminv/help/admin/api/GetLocationFromID.htm"
	Response.end
End if

' Check for required parameters
If IsEmpty(LocationID) then
	strError = strError & "LocationID is a required parameter<BR>"
	bWriteError = True
End if
If bWriteError then
	' Respond with Error
	Response.Write strError
	Response.end
End if
strSQL = "select gl.location_id || '::' || gl.name " &_
	"from inv_vw_grid_location gl " &_
	"where gl.parent_id = " & LocationID & " " &_
	"and not exists (select /*+ index(c CONTAINER_LOCATION_ID_FK_IDX) */ " &_
	"  location_id_fk " &_
	"  from inv_containers c " &_
	"  where c.location_id_fk = gl.location_id) " &_
	"and not exists (select /*+ index(p PLATE_LOCATION_ID_FK_IDX) */ " &_
	"  location_id_fk " &_
	"  from inv_plates p " &_
	"  where p.location_id_fk = gl.location_id) " &_
	"and not exists (select /*+ index(lr INV_LOCATION_PK) */ " &_
	"  parent_id " &_
	"  from inv_locations l " &_
	"  where l.parent_id = gl.location_id " &_
	"  and collapse_child_nodes = 1) " &_
	"and rownum = 1"

'Response.Write strSQL
'Response.end
out = GetListFromSQLRow(strSQL)


' Return success	
Response.Write out 
</SCRIPT>
