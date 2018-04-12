<%@ EnableSessionState=False Language=VBScript%>
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/ado.inc"-->
<!--#INCLUDE VIRTUAL = "/cheminv/gui/guiUtils.asp"-->
<!--#INCLUDE VIRTUAL = "/cheminv/api/apiUtils.asp"-->
<Script RUNAT="Server" Language="VbScript">
Dim Conn
Dim rsInsufficient
Dim rsRemaining
Dim ConStr
Dim strError
Dim bWriteError
Dim PrintDebug
Dim CSUserName
Dim CSUserID

bDebugPrint = False
bWriteError = False
strError = "Error:CheckSufficientSourcePlateQty<BR>"
Action = Request("Action")
PlateID = Request("PlateID")
WellAmt = Request("WellAmt")
NumPlates = Request("NumPlates")

CsUserName = Application("CHEMINV_USERNAME") 
CsUserID = Application("CHEMINV_PWD")

' Redirect to help page if no parameters are passed
If Len(Request.QueryString) = 0 AND Len(Request.Form)= 0 then
	Response.Redirect "/cheminv/help/admin/api/CheckSufficientSourcePlateQty.htm"
	Response.end
End if

' Check for required parameters
If IsEmpty(PlateID) then
	strError = strError & "PlateID is a required parameter<BR>"
	bWriteError = True
End if
If IsEmpty(WellAmt) then
	strError = strError & "WellAmt is a required parameter<BR>"
	bWriteError = True
End if
If IsEmpty(NumPlates) then
	strError = strError & "NumPlates is a required parameter<BR>"
	bWriteError = True
End if
If bWriteError then
	' Respond with Error
	Response.Write strError
	Response.end
End if

totalSourceAmtNeeded = NumPlates * WellAmt

'strSQL = "SELECT count(*) as total FROM inv_wells WHERE plate_id_fk = " & PlateID & " AND qty_remaining < " & totalSourceAmtNeeded
strSQL = "SELECT count(*) as InsufficentWellsCount FROM " _
			& "(SELECT sum(qty_remaining) AS total FROM inv_wells w, inv_grid_position gp WHERE plate_ID_FK in (" & PlateID & ") AND w.grid_position_id_fk = gp.grid_position_id GROUP BY gp.sort_order) " _ 
			& "WHERE total < " & totalSourceAmtNeeded & " OR total is null"
'Response.Write strSQL
'Response.end

call GetInvConnection()
Set rsInsufficient = Conn.Execute(strSQL)

if cint(rsInsufficient("InsufficentWellsCount")) > 0 then out = 1

if Action = "Merge" then
	strSQL = "SELECT count(*) as RemainingWellsCount FROM " _
				& "(SELECT sum(qty_remaining) AS total FROM inv_wells w, inv_grid_position gp WHERE plate_ID_FK in (" & PlateID & ") AND w.grid_position_id_fk = gp.grid_position_id GROUP BY gp.sort_order) " _ 
				& "WHERE total > " & totalSourceAmtNeeded & " OR total is null"
	'Response.Write strSQL
	Set rsRemaining = Conn.Execute(strSQL)
	if cint(rsRemaining("RemainingWellsCount")) > 0 then out = out + 2	
	
	rsRemaining.Close
	Set rsRemaining = nothing
end if

rsInsufficient.Close
Set rsInsufficient = nothing
Conn.Close
Set Conn = nothing

' Return success	
Response.Write out

' return value of 0 = everything ok
' return value of 1 = insufficienct qty
' return value of 2 = remaining qty for a merge
' return value of 3 = insufficient in some and remaining in others

</SCRIPT>
