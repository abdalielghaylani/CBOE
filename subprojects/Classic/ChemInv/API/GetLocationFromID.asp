<%@ EnableSessionState=False Language=VBScript CodePage = 65001%>
<%Response.Charset="UTF-8"%>
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
UserID = Ucase(Request("CSuserName")) 'invloader only

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
if Application("ENABLE_OWNERSHIP")="TRUE" then
strSQL = "SELECT Location_ID, Location_Barcode, Location_Name, " & Application("CHEMINV_USERNAME") & ".racks.isRackLocation(location_id) + " & Application("CHEMINV_USERNAME") & ".racks.isRack(location_id),isPublic "
    if userid <> "" then ' this if block will be used by the invloader only
        strSQL = strSQL & ", " & Application("CHEMINV_USERNAME") & ".Authority.LocationIsAuthorized(Location_ID, '" & UserID & "'), Principal_ID_FK "
    end if    
    strSQL = strSQL & " FROM inv_Locations WHERE Location_ID= ?"
else
    strSQL = "SELECT Location_ID, Location_Barcode, Location_Name, " & Application("CHEMINV_USERNAME") & ".racks.isRackLocation(location_id) + " & Application("CHEMINV_USERNAME") & ".racks.isRack(location_id) FROM inv_Locations WHERE Location_ID= ?"
end if

'Response.Write strSQL
'Response.end
'out = GetListFromSQLRow(strSQL)
out = GetListFromSQLRow1(strSQL, LocationID)

' Return success	
Response.Write out 
</SCRIPT>
