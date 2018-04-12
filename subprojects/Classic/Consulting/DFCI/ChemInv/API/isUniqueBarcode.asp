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

'-- this api checks to see if a given barcode exists in the inv_containers table

bDebugPrint = False
bWriteError = False
strError = "Error:isUniqueBarcode<BR>"
Barcode = Request("Barcode")
CsUserName = Application("CHEMINV_USERNAME") 
CsUserID = Application("CHEMINV_PWD")

'-- Check for required parameters
If IsEmpty(Barcode) then
	strError = strError & "Barcode is a required parameter<BR>"
	bWriteError = True
End if
If bWriteError then
	'-- Respond with Error
	Response.Write strError
	Response.end
End if

Call GetInvConnection()
SQL = "SELECT count(*) as theCount FROM inv_containers where Barcode = '" & Barcode & "'"
Set Cmd = GetCommand(Conn, SQL, adCmdText)
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
