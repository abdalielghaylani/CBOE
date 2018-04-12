<%@ EnableSessionState=False Language=VBScript%>
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/ado.inc"-->
<!--#INCLUDE VIRTUAL = "/cheminv/api/apiUtils.asp"-->
<Script RUNAT="Server" Language="VbScript">
Dim Conn
Dim Cmd
Dim strError
Dim bWriteError
Dim bDebugPrint

bDebugPrint = false
bWriteError = false
strError = "Error:UpdateBatchFields<BR>"
field1 = Request("BatchingField1")
field2 = Request("BatchingField2")
field3 = Request("BatchingField3")

If Len(Request.QueryString) = 0 AND Len(Request.Form)= 0 then
	Response.Redirect "/cheminv/help/admin/api/UpdateBatchFields.htm"
	Response.end
	End if
	
Call GetInvCommand(Application("CHEMINV_USERNAME") & ".Batch.UpdateBatchFields", adCmdStoredProc)
Cmd.Parameters.Append Cmd.CreateParameter("pBatchField1", 200 , adParamInput, 80, field1)
Cmd.Parameters.Append Cmd.CreateParameter("pBatchField2", 200 , adParamInput, 80, field2)
Cmd.Parameters.Append Cmd.CreateParameter("pBatchField3", 200 , adParamInput, 80, field3)
'bDebugPrint=true
 if bDebugPrint then
	For each p in Cmd.Parameters
		Response.Write p.name & " = " & p.value & "<BR>"
	Next	
Else
	Call ExecuteCmd(Application("CHEMINV_USERNAME") & ".Batch.UpdateBatchFields")
End if



   if err.number = 0 then
	Response.Write "1"
    Set Cmd = nothing
    '-- update batch fields dict which stores the batching fields and their display name
	
else
	Response.Write "0"
end if

'Clean up
Conn.Close
Set Conn = Nothing
Set Cmd = Nothing
</SCRIPT>