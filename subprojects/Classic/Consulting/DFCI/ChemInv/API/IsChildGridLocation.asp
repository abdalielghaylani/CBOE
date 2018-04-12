<%@ EnableSessionState=False Language=VBScript%>
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/ado.inc"-->
<!--#INCLUDE VIRTUAL = "/cheminv/api/apiUtils.asp"-->
<Script RUNAT="Server" Language="VbScript">
Dim Conn
Dim Cmd
Dim bWriteError
Dim bDebugPrint
Dim CSUserName
Dim CSUserID

'this api tests if the location is a parent grid location
'unlike the function, which tests if the location is a member 
'of a grid

bDebugPrint = False
bWriteError = False
strError = "Error:IsGridLocation<BR>"
LocationID = Request("LocationID")

CsUserName = Application("CHEMINV_USERNAME") 
CsUserID = Application("CHEMINV_PWD")

' Redirect to help page if no parameters are passed
If Len(Request.QueryString) = 0 AND Len(Request.Form)= 0 then
	Response.Redirect "/cheminv/help/admin/api/IsGridLocation.htm"
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


Call GetInvConnection()
SQL = "select count(*) as theCount from " &  Application("CHEMINV_USERNAME") & ".inv_locations l, " &  Application("CHEMINV_USERNAME") & ".inv_vw_grid_location vl, " &  Application("CHEMINV_USERNAME") & ".inv_locations p where l.location_id=? and l.location_id = vl.location_id and vl.parent_id = p.location_id "
Set Cmd = GetCommand(Conn, SQL, adCmdText)
Cmd.Parameters.Append Cmd.CreateParameter("LocationID", 5, 1, 0, LocationID)
Set RS = Server.CreateObject("ADODB.recordset")
if bDebugPrint then
	For each p in Cmd.Parameters
		Response.Write p.name & " = " & p.value & "<BR>"
	Next
	Response.End	
Else
	Set RS = Cmd.Execute
end if
out = trim(Cstr(RS("theCount")))

Conn.Close
Set Conn = nothing
Set Cmd = nothing
Set RS = nothing

Response.Write out

</SCRIPT>
