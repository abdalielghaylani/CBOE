<%@ EnableSessionState=False Language=VBScript%>
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/ado.inc"-->
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/server_const_vbs.asp"-->
<!--#INCLUDE VIRTUAL = "/cheminv/api/apiUtils.asp"-->
<Script RUNAT="Server" Language="VbScript">
Dim Conn
Dim Cmd
Dim strError
Dim bDebugPrint

bDebugPrint = false
bWriteError = false
strError = "Error:ShipOrder<BR>"

'required parameters
ReceivedContainerIDs = Request("ReceivedContainerIDs")
OrderID = Request("OrderID")

'optional paramters
StatusID = Request("StatusID")
if isEmpty(StatusID) then StatusID = Application("StatusApproved")

' Redirect to help page if no parameters are passed
If Len(Request.QueryString) = 0 AND Len(Request.Form)= 0 then
	Response.Redirect "/cheminv/help/admin/api/ReceiveOrder.htm"
	Response.end
End if

'Echo the input parameters if requested
If NOT isEmpty(Request.QueryString("Echo")) then
	Response.Write "FormData = " & Request.form & "<BR>QueryString = " & Request.QueryString
	Response.end
End if

' Check for required parameters
If IsEmpty(ReceivedContainerIDs) then
	strError = strError & "ReceivedContainerIDs is a required parameter<BR>"
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
Call GetInvCommand(Application("CHEMINV_USERNAME") & ".Requests.ReceiveContainers", adCmdStoredProc)
Cmd.Parameters.Append Cmd.CreateParameter("RETURN_VALUE",adVarchar, adParamReturnValue, 500, NULL)
Cmd.Parameters.Append Cmd.CreateParameter("PORDERID", 5, adParamInput, 0, OrderID) 
Cmd.Parameters.Append Cmd.CreateParameter("PCONTAINERIDLIST", adVarchar, adParamInput, 500, ReceivedContainerIDs) 
Cmd.Parameters.Append Cmd.CreateParameter("PSTATUSID", 5, adParamInput, 0, StatusID) 

if bDebugPrint then
	For each p in Cmd.Parameters
		Response.Write p.name & " = " & p.value & "<BR>"
	Next	
Else
	Call ExecuteCmd(Application("CHEMINV_USERNAME") & ".Requests.ReceiveContainers")
End if

' Return code
Response.Write Cmd.Parameters("RETURN_VALUE")

'Clean up
Conn.Close
Set Conn = Nothing
Set Cmd = Nothing
</SCRIPT>
