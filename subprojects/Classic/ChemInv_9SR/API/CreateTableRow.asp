<%@ EnableSessionState=False Language=VBScript%>
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/ado.inc"-->
<!--#INCLUDE VIRTUAL = "/cheminv/api/apiUtils.asp"-->

<Script RUNAT="Server" Language="VbScript">
Dim Conn
Dim Cmd
Dim TableName
Dim ValuePairs
Dim strError
Dim bWriteError
Dim bDebugPrint

Response.Expires = -1

bDebugPrint = false
bWriteError = FALSE
strError = "Error:CreateTableRow<BR>"

' Redirect to help page if no parameters are passed
If Len(Request.QueryString) = 0 AND Len(Request.Form)= 0 then
	Response.Redirect "/cheminv/help/admin/apiCreateTableRow.htm"
	Response.end
End if

'Required Paramenters
TableName = Request("TableName")
'ValuePairs = server.URLEncode(Request("ValuePairs"))
ValuePairs = Request("ValuePairs")

' Set up an ADO command
Call GetInvCommand(Application("CHEMINV_USERNAME") & ".CreateTableRow", adCmdStoredProc)

Cmd.Parameters.Append Cmd.CreateParameter("RETURN_VALUE", 200, adParamReturnValue, 4000, NULL)
Cmd.Parameters.Append Cmd.CreateParameter("PTABLENAME", 200, 1, 4000, TableName)
Cmd.Parameters.Append Cmd.CreateParameter("PVALUEPAIRS", 200,1, 4000, ValuePairs)

if bDebugPrint then
	For each p in Cmd.Parameters
		Response.Write p.name & " = " & p.value & "<BR>"
	Next	
Else
	Call ExecuteCmd(Application("CHEMINV_USERNAME") & ".CreateTableRow")
End if

out = Cstr(Cmd.Parameters("RETURN_VALUE"))
Conn.Close

Set Cmd = Nothing
Set Conn = Nothing

Response.ContentType = "Text/Plain"
Response.Write out
Response.end
</SCRIPT>
