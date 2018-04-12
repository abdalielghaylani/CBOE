<%@ EnableSessionState=False Language=VBScript%>
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/ado.inc"-->
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/server_const_vbs.asp"-->
<!--#INCLUDE VIRTUAL = "/cheminv/api/apiUtils.asp"-->
<Script RUNAT="Server" Language="VbScript">
Dim Conn
Dim Cmd
Dim strError
Dim bWriteError
Dim PrintDebug
Dim LocationID
Dim ParentID
Dim LocationName
Dim LocationDesc

bDebugPrint = false
bWriteError = False
strError = "Error:CreateReservation<BR>"

ContainerID = Request("ContainerID")
ReservationUserID = Request("ReservationUserID")
QtyReserved = Request("QtyReserved")
ReservationTypeID = Request("ReservationTypeID")

' Redirect to help page if no parameters are passed
If Len(Request.QueryString) = 0 AND Len(Request.Form)= 0 then
	Response.Redirect "/cheminv/help/admin/apiCreateReservation.htm"
	Response.end
End if

'Echo the input parameters if requested
If NOT isEmpty(Request.QueryString("Echo")) then
	Response.Write "FormData = " & Request.form & "<BR>QueryString = " & Request.QueryString
	Response.end
End if

' Check for required parameters
If IsEmpty(ContainerID) then
	strError = strError & "ContainerID is a required parameter<BR>"
	bWriteError = True
End if
If IsEmpty(ReservationUserID) then
	strError = strError & "ReservationUserID is a required parameter<BR>"
	bWriteError = True
End if
If IsEmpty(QtyReserved) then
	strError = strError & "QtyReserved is a required parameter<BR>"
	bWriteError = True
End if

'Optional parameters
If IsEmpty(ReservationTypeID) then ReservationTypeID = 0

If bWriteError then
	' Respond with Error
	Response.Write strError
	Response.end
End if

' Set up and ADO command
Call GetInvCommand(Application("CHEMINV_USERNAME") & ".RESERVATIONS.CreateReservation", adCmdStoredProc)
Cmd.Parameters.Append Cmd.CreateParameter("RETURN_VALUE",adNumeric, adParamReturnValue, 0, NULL)
Cmd.Parameters("RETURN_VALUE").Precision = 9			
Cmd.Parameters.Append Cmd.CreateParameter("PCONTAINERID",adNumeric, adParamInput, , ContainerID) 
Cmd.Parameters("PCONTAINERID").Precision = 9
Cmd.Parameters.Append Cmd.CreateParameter("PUSERID",200, adParamInput, 50, ReservationUserID)
Cmd.Parameters.Append Cmd.CreateParameter("PQTYRESERVED",5, adParamInput, 0, QtyReserved)
Cmd.Parameters.Append Cmd.CreateParameter("PRESERVATIONTYPEID",adNumeric, adParamInput, 0, ReservationTypeID)				
Cmd.Parameters("PRESERVATIONTYPEID").Precision = 4		
if bDebugPrint then
	For each p in Cmd.Parameters
		Response.Write p.name & " = " & p.value & "<BR>"
	Next	
Else
	Call ExecuteCmd(Application("CHEMINV_USERNAME") & ".RESERVATIONS.CreateReservation")
End if

' Return code
Response.Write Cmd.Parameters("RETURN_VALUE")

'Clean up
Conn.Close
Set Conn = Nothing
Set Cmd = Nothing
</SCRIPT>
