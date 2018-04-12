<%@ EnableSessionState=False Language=VBScript%>
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/ado.inc"-->
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/server_const_vbs.asp"-->
<!--#INCLUDE VIRTUAL = "/cheminv/api/apiUtils.asp"-->
<Script RUNAT="Server" Language="VbScript">
Dim DeliveryLocationID
Dim ContainerStatusID
Dim barcode
Dim Cmd
Dim Conn

Dim strError
Dim bWriteError
Dim PrintDebug

CsUserName = Application("CHEMINV_USERNAME") 
CsUserID = Application("CHEMINV_PWD")

bDebugPrint = false
bWriteError = False
strError = "Error:ReceiveContainer<BR>"

ContainerID = Request("ContainerID")
DeliveryLocationID= Request("DeliveryLocationID")
ContainerStatusID= Request("ContainerStatusID")
barcode= Request("barcode")
if barcode = "" then barcode = NULL

' Redirect to help page if no parameters are passed
if Len(Request.QueryString) = 0 AND Len(Request.Form)= 0 then
	Response.Redirect "/cheminv/help/admin/api/ReceiveContainer.htm"
	Response.end
End if


' Check for required parameters
if IsEmpty(ContainerID ) then
	strError = strError & "ContainerID a required parameter<BR>"
	bWriteError = True
End if

if IsEmpty(DeliveryLocationID) then
	strError = strError & "DeliveryLocationIDis a required parameter<BR>"
	bWriteError = True
End if
if IsEmpty(ContainerStatusID) then
	strError = strError & "ContainerStatusID is a required parameter<BR>"
	bWriteError = True
End if

' Respond with Error
if bWriteError then
	Response.Write strError
	Response.end
End if


' Set up and ADO command
Call GetInvCommand(Application("CHEMINV_USERNAME") & ".Requests.ReceiveContainer", adCmdStoredProc)
Cmd.Parameters.Append Cmd.CreateParameter("RETURN_VALUE",adNumeric, adParamReturnValue, 0, NULL)
Cmd.Parameters.Append Cmd.CreateParameter("PCONTAINERID",adNumeric, 1, 0, ContainerID)
Cmd.Parameters.Append Cmd.CreateParameter("PDELIVERYLOCATIONID",adNumeric, 1, 0, DeliveryLocationID)
Cmd.Parameters.Append Cmd.CreateParameter("PBARCODE",200, 1, 50, barcode)
Cmd.Parameters.Append Cmd.CreateParameter("PCONTAINERSTATUSID",adNumeric, 1, 0, ContainerStatusID)


if bDebugPrint then
	For each p in Cmd.Parameters
		Response.Write p.name & " = " & p.value & "<BR>"
	Next	
Else
	Call ExecuteCmd(Application("CHEMINV_USERNAME") & ".Requests.ReceiveContainer")
	Response.Write Cmd.Parameters("RETURN_VALUE")
End if


'Clean up
Conn.Close
Set Conn = Nothing
Set Cmd = Nothing
</SCRIPT>
