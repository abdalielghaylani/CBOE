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

Response.Expires = -1

bDebugPrint = false
bWriteError = False
strError = "Error:GetLocationFromBarcode<BR>"
LocationBarcode = Request("LocationBarcode")


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
strSQL = "SELECT Location_ID, Location_Barcode, Location_Name FROM inv_Locations WHERE Location_Barcode='" & LocationBarcode & "'"
if bdebugPrint then
	Response.Write strSQL
	Response.end
else
	out = GetListFromSQLRow(strSQL)
end if

' Return success	
Response.Write out 
</SCRIPT>
