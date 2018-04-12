<%@ EnableSessionState=False Language=VBScript%>
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/ado.inc"-->
<!--#INCLUDE VIRTUAL = "/cheminv/gui/guiUtils.asp"-->
<!--#INCLUDE VIRTUAL = "/cheminv/api/apiUtils.asp"-->
<Script RUNAT="Server" Language="VbScript">
Dim Conn
Dim Cmd
Dim ConStr
Dim strError
Dim bWriteError
Dim bDebugPrint
Dim CSUserName
Dim CSUserID

bDebugPrint = False
bWriteError = False
strError = "Error:CheckDuplicateBarcode<BR>"
Barcodes = Request("Barcodes")
BarcodeType = Request("BarcodeType") 'location,container,plate

CsUserName = Application("CHEMINV_USERNAME") 
CsUserID = Application("CHEMINV_PWD")

' Redirect to help page if no parameters are passed
If Len(Request.QueryString) = 0 AND Len(Request.Form)= 0 then
	Response.Redirect "/cheminv/help/admin/api/CheckDuplicateBarcode.htm"
	Response.end
End if

' Check for required parameters
If IsEmpty(Barcodes) then
	strError = strError & "Barcodes is a required parameter<BR>"
	bWriteError = True
End if
If IsEmpty(BarcodeType) then
	strError = strError & "BarcodeType is a required parameter<BR>"
	bWriteError = True
End if
If bWriteError then
	' Respond with Error
	Response.Write strError
	Response.end
End if

arrBarcodes = split(Barcodes,",")
for i = 0 to ubound(arrBarcodes)
	barcodeList = barcodeList & "'" & arrBarcodes(i) & "',"
next
Select Case lcase(BarcodeType)
	Case "location"
	Case "container"
	Case "plate"
		SQL = "SELECT plate_barcode FROM inv_plates WHERE plate_barcode IN (" & left(barcodeList,len(barcodeList)-1) & ")"
End Select


call GetInvConnection()
Set Cmd = GetCommand(Conn, SQL, adCmdText)
'Cmd.Parameters.Append Cmd.CreateParameter("Barcodes", 200, 1, 2000, Barcodes)
Set RS = Server.CreateObject("ADODB.recordset")
'RS.Open SQL, Conn, adOpenKeyset, adLockOptimistic, adCmdText 
Set RS = Cmd.Execute
duplicates = " "
while not rs.EOF
	duplicates = duplicates & RS("Plate_Barcode") & ","
	RS.MoveNext
wend
duplicates = trim(left(duplicates,len(duplicates)-1))

Response.Write duplicates

Conn.Close
Set Conn = nothing
Set RS = nothing

</SCRIPT>
