<%@ EnableSessionState=False Language=VBScript%>
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/ado.inc"-->
<!--#INCLUDE VIRTUAL = "/cheminv/gui/guiUtils.asp"-->
<!--#INCLUDE VIRTUAL = "/cheminv/api/apiUtils.asp"-->
<Script RUNAT="Server" Language="VbScript">
Dim Conn
Dim Cmd
Dim RS
Dim strError
Dim bWriteError
Dim bDebugPrint
Dim CSUserName
Dim CSUserID

Response.Expires = -1

bDebugPrint = false
bWriteError = False
strError = "Error:GetKeyContainerAttributes<BR>"
ContainerBarcode = Request("ContainerBarcode")
ContainerID = Request("ContainerID")
InvSchema = Application("CHEMINV_USERNAME")

CsUserName = Application("CHEMINV_USERNAME") 
CsUserID = Application("CHEMINV_PWD")

' Redirect to help page if no parameters are passed
If Len(Request.QueryString) = 0 AND Len(Request.Form)= 0 then
	Response.Redirect "/cheminv/help/admin/api/GetKeyContainerAttributes.htm"
	Response.end
End if

' Check for required parameters
If IsEmpty(ContainerID) AND IsEmpty(ContainerBarcode) then
	strError = strError & "Either ContainerID or ContainerBarcode is a required parameters<BR>"
	bWriteError = True
End if

If bWriteError then
	' Respond with Error
	Response.Write strError
	Response.end
End if

if NOT IsEmpty(ContainerID) then 
	strWhere = "Container_ID= ? "
Else
	strWhere = "barcode = ? "
End if
if IsEmpty(ContainerID) then ContainerID= NULL
if IsEmpty(ContainerBarcode) then ContainerBarcode= NULL
'1 container_id
'2 barcode
'3 current user
'4 uom
'5 qty remaining
'6 path

GetInvConnection()
Set Cmd = GetCommand(Conn, InvSchema & ".GUIUTILS.GETKEYCONTAINERATTRIBUTES", 4)		 
Cmd.Parameters.Append Cmd.CreateParameter("CONTAINERID", 200, 1, 2000, ContainerID) 
Cmd.Parameters.Append Cmd.CreateParameter("CONTAINERBARCODE", 200, 1, 2000, ContainerBarcode) 


if bdebugPrint then
	Response.Write "Parameters:<BR>"
	For each p in Cmd.Parameters
		Response.Write p.name & " = " & p.value & "<BR>"
	Next	
	Response.end
else
	Cmd.Properties ("PLSQLRSet") = TRUE  
	Set RS = Cmd.Execute
	Cmd.Properties ("PLSQLRSet") = FALSE
	if NOT (RS.EOF AND RS.BOF) then
		Response.write RS.GetString(2,,",","|","")
	else
		Response.Write ""
	end if
end if
</SCRIPT>
