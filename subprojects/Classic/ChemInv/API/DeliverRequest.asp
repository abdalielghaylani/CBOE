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
'CSBR ID : 135218 SJ
'Comments: Enabling Automatic reservation of containers for container\sample requests
'Start of change
Dim AutomaticReservation
Dim sSQL
sSQL = ""
AutomaticReservation=false
If Application("AutomaticReservation") = "1" then AutomaticReservation = true
'End of change for CSBR 135218
bDebugPrint = false
bWriteError = False
strError = "Error:DeliverRequest<BR>"

DeliveredRequestIDList = Request("DeliveredRequestIDList")

' Redirect to help page if no parameters are passed
If Len(Request.QueryString) = 0 AND Len(Request.Form)= 0 then
	Response.Redirect "/cheminv/help/admin/api/DeliverRequest.htm"
	Response.end
End if

'Echo the input parameters if requested
If NOT isEmpty(Request.QueryString("Echo")) then
	Response.Write "FormData = " & Request.form & "<BR>QueryString = " & Request.QueryString
	Response.end
End if

' Check for required parameters
If IsEmpty(DeliveredRequestIDList) then
	strError = strError & "DeliveredRequestIDList is a required parameter<BR>"
	bWriteError = True
End if

If bWriteError then
	' Respond with Error
	Response.Write strError
	Response.end
End if

' Set up and ADO command
Call GetInvCommand(Application("CHEMINV_USERNAME") & ".Requests.DeliverRequests", adCmdStoredProc)
Cmd.Parameters.Append Cmd.CreateParameter("RETURN_VALUE",adNumeric, adParamReturnValue, 0, NULL)
Cmd.Parameters("RETURN_VALUE").Precision = 9			
Cmd.Parameters.Append Cmd.CreateParameter("PRequestIDList",200, adParamInput, 2000, DeliveredRequestIDList) 

if bDebugPrint then
	For each p in Cmd.Parameters
		Response.Write p.name & " = " & p.value & "<BR>"
	Next	
Else
	Call ExecuteCmd(Application("CHEMINV_USERNAME") & ".Requests.DeliverRequests")
	ReturnValue = Cmd.Parameters("RETURN_VALUE") 'CSBR 135218
End if
'CSBR ID : 135218 SJ
'Comments: Enabling Automatic reservation of containers for container\sample requests
'Start of change
If AutomaticReservation Then
    sSQL = "Select R.Request_Id, R.Container_Id_fk, V.Reservation_Id From INV_REQUESTS R,INV_RESERVATIONS V " & _
           "Where R.Request_Id = V.Request_Id_Fk And V.Request_Id_Fk In (" & DeliveredRequestIDList & ")"
    GetInvConnection()
    Set RS = Conn.Execute(sSQL)
	If NOT (RS.BOF AND RS.EOF) then 
		RS.movefirst
		While NOT RS.EOF
		    ReservationID = RS("Reservation_ID").Value
	 	    ContainerID = RS("Container_ID_FK").Value
            Call GetInvCommand(Application("CHEMINV_USERNAME") & ".RESERVATIONS.DeleteReservation", adCmdStoredProc)
            Cmd.Parameters.Append Cmd.CreateParameter("RETURN_VALUE_1",adNumeric, adParamReturnValue, 0, NULL)
            Cmd.Parameters("RETURN_VALUE_1").Precision = 9			
            Cmd.Parameters.Append Cmd.CreateParameter("PRESERVATIONID",adNumeric, adParamInput, 0, ReservationID) 
            Cmd.Parameters("PRESERVATIONID").Precision = 9
            Cmd.Parameters.Append Cmd.CreateParameter("PCONTAINERID",adNumeric, adParamInput, , ContainerID) 
            Cmd.Parameters("PCONTAINERID").Precision = 9
            If bDebugPrint Then
	            For each p in Cmd.Parameters
		            Response.Write p.name & " = '" & p.value & "'<BR>"
	            Next
            Else    
                Call ExecuteCmd(Application("CHEMINV_USERNAME") & ".RESERVATIONS.DeleteReservation")
            End If
            RS.MoveNext
        Wend
    End If
    RS.Close()
    Set RS = Nothing
End If

' Return code
Response.Write ReturnValue
'End of change for CSBR 135218
'Clean up
Conn.Close
Set Conn = Nothing
Set Cmd = Nothing
</SCRIPT>
