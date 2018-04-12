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
stop
Response.Expires = -1

bDebugPrint = false
bWriteError = False
strError = "Error:GetKeyContainerAttributes<BR>"
objectID = Request("objectID")
objectType = Request("objectType")
CsUserName = Application("CHEMINV_USERNAME") 
CsUserID = Application("CHEMINV_PWD")

If bWriteError then
	' Respond with Error
	Response.Write strError
	Response.end
End if

GetInvConnection()
Call GetInvCommand(Application("CHEMINV_USERNAME") & ".Authority.GetPrincipalID", adCmdStoredProc)
Cmd.Parameters.Append Cmd.CreateParameter("RETURN_VALUE",adNumeric, adParamReturnValue, 2, NULL)
Cmd.Parameters("RETURN_VALUE").Precision = 9		
Cmd.Parameters.Append Cmd.CreateParameter("pObjectID",adVarchar, adParamInput, 255, ObjectID)	
Cmd.Parameters.Append Cmd.CreateParameter("pObjectType",adVarchar, adParamInput, 255, ObjectType)	
Cmd.Properties("SPPrmsLOB") = TRUE

if bdebugPrint then
	Response.Write "Parameters:<BR>"
	For each p in Cmd.Parameters
		Response.Write p.name & " = " & p.value & "<BR>"
	Next	
	Response.end
else
	Call ExecuteCmd(Application("CHEMINV_USERNAME") & ".Authority.GetOwnerInfo")
	out=Cmd.Parameters("RETURN_VALUE")
	Cmd.Properties ("PLSQLRSet") = FALSE
	if out<>"" then
	Response.write out
	else
	Response.write ""
	
	end if
end if
</SCRIPT>
