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
Dim ContainerBarcode
Dim ContainerID
Dim IDLen
Dim BarcodeLen
Dim iUseReg

Response.Expires = -1

bDebugPrint = False
bWriteError = False
strError = "Error:GetContainerAttributes<BR>"
ContainerBarcode = Request("ContainerBarcode")
ContainerID = Request("ContainerID")
InvSchema = Application("CHEMINV_USERNAME")
CsUserName = Application("CHEMINV_USERNAME") 
CsUserID = Application("CHEMINV_PWD")

' Redirect to help page if no parameters are passed
If Len(Request.QueryString) = 0 AND Len(Request.Form)= 0 then
	Response.Redirect "/cheminv/help/admin/api/GetContainerAttributes.htm"
	Response.end
End if

' Check for required parameters
If IsEmpty(ContainerID) AND IsEmpty(ContainerBarcode) then
	strError = strError & "Either ContainerID or ContainerBarcode must be specified.<BR>"
	bWriteError = True
End if

If bWriteError then
	' Respond with Error
	Response.Write strError
	Response.end
End if

if IsEmpty(ContainerID) then 
    ContainerID= NULL
    IDLen = 1
else
    IDLen = Len(ContainerID)
end if

if IsEmpty(ContainerBarcode) then 
    ContainerBarcode= NULL
    BarcodeLen = 1
else
    BarcodeLen = Len(ContainerBarcode)
end if

if Application("RegServerName") <> "NULL" then
    iUseReg = 1
else
    iUseReg = 0
end if

GetInvConnection()
Set Cmd = GetCommand(Conn, InvSchema & ".GUIUTILS.GETCONTAINERATTRIBUTES", 4)
Cmd.Parameters.Append Cmd.CreateParameter("CONTAINERID", adVarChar, adParamInput, IDLen, ContainerID) 
Cmd.Parameters.Append Cmd.CreateParameter("CONTAINERBARCODE", adVarChar, adParamInput, BarcodeLen, ContainerBarcode)
Cmd.Parameters.Append Cmd.CreateParameter("USEREG", adInteger, adParamInput, 4, iUseReg)

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
