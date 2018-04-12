<%@ EnableSessionState=False Language=VBScript%>
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/ado.inc"-->
<!--#INCLUDE VIRTUAL = "/cheminv/api/apiUtils.asp"-->
<!--#INCLUDE VIRTUAL = "/cheminv/gui/BatchFunctions.asp" -->
<Script RUNAT="Server" Language="VbScript">
Dim Conn
Dim Cmd
Dim strError
Dim bWriteError
Dim bDebugPrint

bDebugPrint = false
bWriteError = false
strError = "Error:ClearBatchFields<BR>"
' Set up an ADO command
Call GetInvCommand(Application("CHEMINV_USERNAME") & ".Batch.ClearBatchingFields", adCmdStoredProc)
if bDebugPrint then
	For each p in Cmd.Parameters
		Response.Write p.name & " = " & p.value & "<BR>"
	Next	
Else
	Call ExecuteCmd(Application("CHEMINV_USERNAME") & ".Batch.ClearBatchingFields")
End if
if err.number = 0 then
	Response.Write "1"
    Set Cmd = nothing
    '-- update batch fields dict which stores the batching fields and their display name
	Dim RS
	Set Cmd = Server.CreateObject("ADODB.Command")
	Set Cmd.ActiveConnection = Conn
	Cmd.CommandText = "{CALL " & Application("CHEMINV_USERNAME") & ".BATCH.GETBATCHFIELDS()}"
	Cmd.CommandType = adCmdText

    Cmd.Properties ("PLSQLRSet") = TRUE  
    Set RS = Cmd.Execute
    Cmd.Properties ("PLSQLRSet") = FALSE
    
	Application.Lock
    call isaRegBatch(rs)
    batching_fields_dict.RemoveAll
    while not RS.EOF
        batching_fields_dict.add cstr(RS("sort_order"))&"_"&cstr(RS("BATCH_TYPE_ID_FK")), cstr(RS("display_name"))
        RS.MoveNext
    wend
   Application("NumBatchTypes")= GetNumBatchTypes(Conn)
    Application.UnLock
	
else
	Response.Write "0"
end if

'Clean up
Conn.Close
Set Conn = Nothing
Set Cmd = Nothing
</SCRIPT>
