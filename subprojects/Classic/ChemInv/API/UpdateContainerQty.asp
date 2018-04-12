<%@ EnableSessionState=False Language=VBScript%>
<!--#INCLUDE VIRTUAL = "/cheminv/gui/guiUtils.asp"-->
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/ado.inc"-->
<!--#INCLUDE VIRTUAL = "/cheminv/api/apiUtils.asp"-->
<Script RUNAT="Server" Language="VbScript">
Dim Conn
Dim Cmd
Dim httpResponse
Dim FormData
Dim Credentials
Dim QueryString
Dim ServerName
Dim arrValues()
Dim ContainerID
Dim Action
Dim NumContainers
Dim ContainerSize
Dim CurrValue
Dim bSameValue
Dim bDebugPrint
Dim strError
Dim bWriteError
bDebugPrint = FALSE
bWriteError = FALSE


valuepair = Request("valuepair")
Credentials = "&CSUserName=" & Request("CSUserName") & "&CSUSerID=" & Request("CSUSerID")
Call GetInvCommand(Application("CHEMINV_USERNAME") & ".UPDATECONTAINERQTY", adCmdStoredProc)

Cmd.Parameters.Append Cmd.CreateParameter("RETURN_VALUE", adNumeric, adParamReturnValue, 0, NULL)
Cmd.Parameters("RETURN_VALUE").Precision = 9
Cmd.Parameters.Append Cmd.CreateParameter("pValuePairs", 200, 1, 4000, valuepair)

if bDebugPrint then
	For each p in Cmd.Parameters
		Response.Write p.name & " = " & p.value & "<BR>"
	Next	
Else
	Call ExecuteCmd(Application("CHEMINV_USERNAME") & ".UPDATECONTAINERQTY")
End if

' Return the newly created ContainerID
out = Cstr(Cmd.Parameters("RETURN_VALUE"))
Conn.Close
Response.End
Set Cmd = Nothing
Set Conn = Nothing
	
Response.ContentType = "Text/Plain"
Response.Write out
Response.end
</SCRIPT>
