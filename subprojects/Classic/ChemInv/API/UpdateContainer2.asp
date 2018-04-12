<%@ EnableSessionState=False Language=VBScript CodePage = 65001%>
<%Response.Charset="UTF-8"%>
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/ado.inc"-->
<!--#INCLUDE VIRTUAL = "/cheminv/api/apiUtils.asp"-->

<Script RUNAT="Server" Language="VbScript">
Dim Conn
Dim Cmd
Dim ContainerIDs
Dim ValuePairs
Dim strError
Dim bWriteError
Dim bDebugPrint

Response.Expires = -1

bDebugPrint = False
bWriteError = FALSE
strError = "Error:UpdateContainer<BR>"

' Redirect to help page if no parameters are passed
If Len(Request.QueryString) = 0 AND Len(Request.Form)= 0 then
	Response.Redirect "/cheminv/help/admin/api/UpdateContainer2.htm"
	Response.end
End if

'Required Paramenters
ContainerIDs = Request("ContainerIDs")
ValuePairs = Request("ValuePairs")

' Set up an ADO command
Call GetInvCommand(Application("CHEMINV_USERNAME") & ".UpdateContainer", adCmdStoredProc)

Cmd.Parameters.Append Cmd.CreateParameter("RETURN_VALUE", adNumeric, adParamReturnValue, 0, NULL)
Cmd.Parameters("RETURN_VALUE").Precision = 9
Cmd.Parameters.Append Cmd.CreateParameter("PCONTAINERIDS", 200, 1, 4000, ContainerIDs)
Cmd.Parameters.Append Cmd.CreateParameter("PVALUEPAIRS", 200,1, 10000, ValuePairs)

if bDebugPrint then
	For each p in Cmd.Parameters
		Response.Write p.name & " = " & p.value & "<BR>"
	Next	
Else
	Call ExecuteCmd(Application("CHEMINV_USERNAME") & ".UpdateContainer")
End if

' Return the newly created ContainerID
out = Cstr(Cmd.Parameters("RETURN_VALUE"))
Conn.Close

Set Cmd = Nothing
Set Conn = Nothing

Response.ContentType = "Text/Plain"
Response.Write out
Response.end
</SCRIPT>
