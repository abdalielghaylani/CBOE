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
Dim CompoundID

Response.Expires = -1

bDebugPrint = false
bWriteError = False
strError = "Error:SetContainerSubstance<BR>"
ContainerBarcode = Request("ContainerBarcode")
ContainerID = Request("ContainerID")
CompoundID = Request("CompoundID")
InvSchema = Application("CHEMINV_USERNAME")

CsUserName = Application("CHEMINV_USERNAME") 
CsUserID = Application("CHEMINV_PWD")

' Redirect to help page if no parameters are passed
If Len(Request.QueryString) = 0 AND Len(Request.Form)= 0 then
	Response.Redirect "/cheminv/help/admin/api/SetContainerSubstance.htm"
	Response.end
End if

' Check for required parameters

If IsEmpty(ContainerID) AND IsEmpty(ContainerBarcode) then
	strError = strError & "Either ContainerID or ContainerBarcode must be specified.<BR>"
	bWriteError = True
End if

if IsEmpty(CompoundID) then
    strError = strError & "CompoundID must be specified.<BR>"
	bWriteError = True    
end if

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

GetInvConnection()
Set Cmd = GetCommand(Conn, InvSchema & ".GUIUTILS.SETCONTAINERSUBSTANCE", 4)		 
Cmd.Parameters.Append Cmd.CreateParameter("CONTAINERID", adVarChar, adParamInput, IDLen, ContainerID) 
Cmd.Parameters.Append Cmd.CreateParameter("CONTAINERBARCODE", adVarChar, adParamInput, BarcodeLen, ContainerBarcode)
Cmd.Parameters.Append Cmd.CreateParameter("COMPOUNDID", adInteger, adParamInput, 4, CompoundID)

if bdebugPrint then
	Response.Write "Parameters:<BR>"
	For each p in Cmd.Parameters
		Response.Write p.name & " = " & p.value & "<BR>"
	Next	
	Response.end
else
	Cmd.Properties ("PLSQLRSet") = TRUE  
	Cmd.Execute
	Cmd.Properties ("PLSQLRSet") = FALSE	
end if
</SCRIPT>
