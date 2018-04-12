<%@ EnableSessionState=False Language=VBScript%>
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/ado.inc"-->
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/server_const_vbs.asp"-->
<!--#INCLUDE VIRTUAL = "/cheminv/api/apiUtils.asp"-->
<Script RUNAT="Server" Language="VbScript">
Dim Conn
Dim Cmd
Dim strError
Dim bDebugPrint
Dim bWriteError
Dim RemovedContainerIDs
Dim OrderID

bDebugPrint = false
bWriteError = false
strError = "Error:RemoveOrderContainers<BR>"

'required parameters
RemovedContainerIDs = Request("RemovedContainerIDs")
OrderID = Request("OrderID")

' Redirect to help page if no parameters are passed
If Len(Request.QueryString) = 0 AND Len(Request.Form)= 0 then
	Response.Redirect "/cheminv/help/admin/api/RemoveOrderContainers.htm"
	Response.end
End if

'Echo the input parameters if requested
If NOT isEmpty(Request.QueryString("Echo")) then
	Response.Write "FormData = " & Request.form & "<BR>QueryString = " & Request.QueryString
	Response.end
End if

' Check for required parameters
If IsEmpty(RemovedContainerIDs) then
	strError = strError & "RemovedContainerIDs is a required parameter<BR>"
	bWriteError = True
End if
If IsEmpty(OrderID) then
	strError = strError & "OrderID is a required parameter<BR>"
	bWriteError = True
End if

If bWriteError then
	' Respond with Error
	Response.Write strError
	Response.end
End if

' Set up and ADO command
Call GetInvCommand(Application("CHEMINV_USERNAME") & ".Requests.RemoveContainers", adCmdStoredProc)
Cmd.Parameters.Append Cmd.CreateParameter("RETURN_VALUE",adVarchar, adParamReturnValue, 500, NULL)
Cmd.Parameters.Append Cmd.CreateParameter("PORDERID", 5, adParamInput, 0, OrderID) 
Cmd.Parameters.Append Cmd.CreateParameter("PCONTAINERIDLIST", adVarchar, adParamInput, len(RemovedContainerIDs), RemovedContainerIDs) 

if bDebugPrint then
	For each p in Cmd.Parameters
		Response.Write p.name & " = " & p.value & "<BR>"
	Next	
Else
	Call ExecuteCmd(Application("CHEMINV_USERNAME") & ".Requests.RemoveContainers")
End if

' Return code
Response.Write Cmd.Parameters("RETURN_VALUE")

'Clean up
Conn.Close
Set Conn = Nothing
Set Cmd = Nothing
</SCRIPT>
