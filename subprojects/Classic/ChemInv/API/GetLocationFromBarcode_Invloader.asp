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

Response.Expires = -1

bDebugPrint = false
bWriteError = False
strError = "Error:GetLocationFromBarcode<BR>"
LocationBarcode = Request("LocationBarcode")
UserID = Ucase(Request("UserID")) ' to be used for Group Security
CsUserName = Application("CHEMINV_USERNAME") 
CsUserID = Application("CHEMINV_PWD")

' Redirect to help page if no parameters are passed
If Len(Request.QueryString) = 0 AND Len(Request.Form)= 0 then
	Response.Redirect "/cheminv/help/admin/api/GetLocationFromBarcode.htm"
	Response.end
End if

' Check for required parameters
If IsEmpty(LocationBarcode) then
	strError = strError & "LocationBarcode is a required parameter<BR>"
	bWriteError = True
End if
If bWriteError then
	' Respond with Error
	Response.Write strError
	Response.end
End if
if Application("ENABLE_OWNERSHIP")="TRUE" then
    strSQL = "SELECT Location_ID, Location_Barcode, Location_Name, " & Application("CHEMINV_USERNAME") & ".racks.isRackLocation(location_id) + " & Application("CHEMINV_USERNAME") & ".racks.isRack(location_id)," & Application("CHEMINV_USERNAME") & ".GUIUTILS.GETLOCATIONPATH(location_id), isPublic "
    if userid <> "" then ' this if block will be used by the invloader only
        strSQL = strSQL & ", " & Application("CHEMINV_USERNAME") & ".Authority.LocationIsAuthorized(Location_ID, '" & UserID & "'), Principal_ID_FK "
    end if    
    strSQL = strSQL & " FROM inv_Locations WHERE Location_Barcode=?" 
else
    strSQL = "SELECT Location_ID, Location_Barcode, Location_Name, " & Application("CHEMINV_USERNAME") & ".racks.isRackLocation(location_id) + " & Application("CHEMINV_USERNAME") & ".racks.isRack(location_id)," & Application("CHEMINV_USERNAME") & ".GUIUTILS.GETLOCATIONPATH(location_id) FROM inv_Locations WHERE Location_Barcode=?"
end if
if bdebugPrint then
	Response.Write strSQL
	Response.end
else
	'out = GetListFromSQLRow(strSQL)
	out = GetListFromSQLRow2(strSQL, LocationBarcode)
end if

' Return success	
Response.Write out 
</SCRIPT>
