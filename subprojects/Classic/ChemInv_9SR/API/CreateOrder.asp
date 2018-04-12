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
strError = "Error:CreateOrder<BR>"

'required parameters
DeliveryLocationID = Request("DeliveryLocationID")
ShipToName = Request("ShipToName")

'optional paramters
ShippingConditions = Request("ShippingConditions")
SampleContainerIDs = Request("SampleContainerIDs")
StatusID = Request("StatusID")
if isEmpty(StatusID) then StatusID = Application("StatusOrderContainers")

' Redirect to help page if no parameters are passed
If Len(Request.QueryString) = 0 AND Len(Request.Form)= 0 then
	Response.Redirect "/cheminv/help/admin/api/CreateOrder.htm"
	Response.end
End if

'Echo the input parameters if requested
If NOT isEmpty(Request.QueryString("Echo")) then
	Response.Write "FormData = " & Request.form & "<BR>QueryString = " & Request.QueryString
	Response.end
End if

' Check for required parameters
If IsEmpty(DeliveryLocationID) then
	strError = strError & "DeliveryLocationID is a required parameter<BR>"
	bWriteError = True
End if
If IsEmpty(ShipToName) then
	strError = strError & "ShipToName is a required parameter<BR>"
	bWriteError = True
End if

If bWriteError then
	' Respond with Error
	Response.Write strError
	Response.end
End if

' Set up and ADO command
Call GetInvCommand(Application("CHEMINV_USERNAME") & ".Requests.CreateOrder", adCmdStoredProc)
Cmd.Parameters.Append Cmd.CreateParameter("RETURN_VALUE",adNumeric, adParamReturnValue, 0, NULL)
Cmd.Parameters("RETURN_VALUE").Precision = 9			
Cmd.Parameters.Append Cmd.CreateParameter("PDELIVERYLOCATIONID", 5, adParamInput, 0, DeliveryLocationID) 
Cmd.Parameters.Append Cmd.CreateParameter("PSHIPTONAME", 200, adParamInput, 255, ShipToName) 
Cmd.Parameters.Append Cmd.CreateParameter("PSHIPPINGCONDITIONS", 200, adParamInput, 255, ShippingConditions) 
Cmd.Parameters.Append Cmd.CreateParameter("PSAMPLECONTAINERIDS", 200, adParamInput, len(SampleContainerIDs)+1, SampleContainerIDs) 
Cmd.Parameters.Append Cmd.CreateParameter("PSTATUSID", 5, adParamInput, 0, StatusID) 

if bDebugPrint then
	For each p in Cmd.Parameters
		Response.Write p.name & " = " & p.value & "<BR>"
	Next	
Else
	Call ExecuteCmd(Application("CHEMINV_USERNAME") & ".Requests.CreateOrder")
End if

' Return code
Response.Write Cmd.Parameters("RETURN_VALUE")

'Clean up
Conn.Close
Set Conn = Nothing
Set Cmd = Nothing
</SCRIPT>
