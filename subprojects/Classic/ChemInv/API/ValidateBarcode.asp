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
strError = "Error:ValidateBarcode<BR>"
Barcode = Request("Barcode")

CsUserName = Application("INVREG_USERNAME") 
CsUserID = Application("INVREG_PWD")

' Redirect to help page if no parameters are passed
If Len(Request.QueryString) = 0 AND Len(Request.Form)= 0 then
	Response.Redirect "/cheminv/help/admin/api/ValidateBarcode.htm"
	Response.end
End if

' Check for required parameters
If IsEmpty(Barcode) then
	strError = strError & "Barcode is required<br>"
	bWriteError = True
End if

If bWriteError then
	' Respond with Error
	Response.Write strError
	Response.end
End if

strSQL = "SELECT Count(Barcode) as Cnt, 'END' as End_Char FROM inv_containers Where Barcode = '" & Barcode & "'"

if bdebugPrint then
	Response.Write strSQL
	Response.end
else
	Response.write GetListFromSQLRow_REG(strSQL)
end if
</SCRIPT>
