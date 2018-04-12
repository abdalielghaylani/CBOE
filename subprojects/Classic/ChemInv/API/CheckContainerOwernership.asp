
<%@ EnableSessionState=False Language=VBScript%>
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/ado.inc"-->
<!--#INCLUDE VIRTUAL = "/cheminv/gui/guiUtils.asp"-->
<!--#INCLUDE VIRTUAL = "/cheminv/api/apiUtils.asp"-->
<Script RUNAT="Server" Language="VbScript">
Dim Conn
Dim Cmd
Dim SQL
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
Barcode = Request("Barcode")
InvSchema = Application("CHEMINV_USERNAME")
username= Ucase(Request("username"))
CsUserName = Application("CHEMINV_USERNAME") 
CsUserID = Application("CHEMINV_PWD")

If bWriteError then
	' Respond with Error
	Response.Write strError
	Response.end
End if

GetInvConnection()
Call GetInvCommand(Application("CHEMINV_USERNAME") & ".Authority.ContainerIsAuthorized", adCmdStoredProc)
Cmd.Parameters.Append Cmd.CreateParameter("RETURN_VALUE",adNumeric, adParamReturnValue, 2, NULL)
Cmd.Parameters("RETURN_VALUE").Precision = 9		
Cmd.Parameters.Append Cmd.CreateParameter("pContainerID",adNumeric, adParamInput, 255, ContainerID)	
Cmd.Parameters.Append Cmd.CreateParameter("pUserID", adVarChar, adParamInput, 255, username) 
Cmd.Parameters.Append Cmd.CreateParameter("pBarcode", adVarChar, adParamInput, 255, Barcode) 
Cmd.Properties("SPPrmsLOB") = TRUE

if bdebugPrint then
	Response.Write "Parameters:<BR>"
	For each p in Cmd.Parameters
		Response.Write p.name & " = " & p.value & "<BR>"
	Next	
	Response.end
else
	Call ExecuteCmd(Application("CHEMINV_USERNAME") & ".Authority.ContainerIsAuthorized")
	out=Cmd.Parameters("RETURN_VALUE")
	Cmd.Properties ("PLSQLRSet") = FALSE
	if out<>"" then
	Response.write out
	else
	Response.write ""
	
	end if
end if
</SCRIPT>
