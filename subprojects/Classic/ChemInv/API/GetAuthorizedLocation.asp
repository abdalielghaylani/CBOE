<%@ EnableSessionState=False Language=VBScript CodePage = 65001%>
<%Response.Charset="UTF-8"%>
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/ado.inc"-->
<!--#INCLUDE VIRTUAL = "/cheminv/gui/guiUtils.asp"-->
<!--#INCLUDE VIRTUAL = "/cheminv/api/apiUtils.asp"-->
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/cs_security/cs_security_login_utils_vbs.asp"-->
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
CsUserName =Request("tempCsUserName") 
' The key to CryptVBS can be anything, so long as it matches what was used to generate the 
' encrypted string inside InvLoader
CsUserID = URLDecode(CryptVBS(request("tempCsUserID"),"ChemInv\API\GetAuthorizedLocation.asp"))
if CsUserID=NULL or CsUserID="" then CsUserID=CsUserName
'CsUserName = Application("CHEMINV_USERNAME") 
'CsUserID = Application("CHEMINV_PWD")
'Response.Write CsUserName & CsUserID
'Response.End
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
strSQL = "Select Decode(CHEMINVDB2.Authority.LocationIsAuthorized(" & LocationID &"),NULL,0,CHEMINVDB2.Authority.LocationIsAuthorized(" & LocationID &")) as isAuthorised from Dual"

'Response.Write strSQL
'Response.end
out = GetListFromSQLRow(strSQL)


' Return success	
Response.Write out 
</SCRIPT>
