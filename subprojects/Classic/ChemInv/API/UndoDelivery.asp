<%@ EnableSessionState=False Language=VBScript CodePage = 65001%>
<%Response.Charset="UTF-8"%>
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
AutomaticReservation=false
if Application("AutomaticReservation") = "1" then AutomaticReservation = true
'End of change for CSBR 135218
bDebugPrint = false
bWriteError = False
strError = "Error:UndoDelivery<BR>"

RequestID = Request("RequestID")

' Redirect to help page if no parameters are passed
If Len(Request.QueryString) = 0 AND Len(Request.Form)= 0 then
	Response.Redirect "/cheminv/help/admin/api/UndoDelivery.htm"
	Response.end
End if

'Echo the input parameters if requested
If NOT isEmpty(Request.QueryString("Echo")) then
	Response.Write "FormData = " & Request.form & "<BR>QueryString = " & Request.QueryString
	Response.end
End if

' Check for required parameters
If IsEmpty(RequestID) then
	strError = strError & "RequestID is a required parameter<BR>"
	bWriteError = True
End if

If bWriteError then
	' Respond with Error
	Response.Write strError
	Response.end
End if

' Set up and ADO command
Call GetInvCommand(Application("CHEMINV_USERNAME") & ".Requests.UndoDelivery", adCmdStoredProc)
Cmd.Parameters.Append Cmd.CreateParameter("RETURN_VALUE",adNumeric, adParamReturnValue, 0, NULL)
Cmd.Parameters("RETURN_VALUE").Precision = 9			
Cmd.Parameters.Append Cmd.CreateParameter("PRequestID",adNumeric, adParamInput, 0, RequestID) 
Cmd.Parameters("PRequestID").Precision = 9

if bDebugPrint then
	For each p in Cmd.Parameters
		Response.Write p.name & " = " & p.value & "<BR>"
	Next	
Else
	Call ExecuteCmd(Application("CHEMINV_USERNAME") & ".Requests.UndoDelivery")
	ReturnValue = Cmd.Parameters("RETURN_VALUE") 'CSBR 135218
End if
'CSBR ID : 135218 SJ
'Comments: Enabling Automatic reservation of containers for container\sample requests
'Start of change
If AutomaticReservation Then
    sSQL = "Select Container_Id_Fk, User_ID_FK, Qty_Required From INV_REQUESTS " & _
           "Where Request_Id = " & RequestID 
    GetInvConnection()
    Set RS = Conn.Execute(sSQL)
	If NOT (RS.BOF AND RS.EOF) then 
		RS.movefirst
	 	ContainerID = RS("Container_ID_FK").Value
	 	UserID = RS("User_ID_FK").Value
	 	QtyRequired = RS("Qty_Required")
	 	ReservationTypeID = 2
        Call GetInvCommand(Application("CHEMINV_USERNAME") & ".RESERVATIONS.CreateReservation", adCmdStoredProc)
        Cmd.Parameters.Append Cmd.CreateParameter("RETURN_VALUE1",adNumeric, adParamReturnValue, 0, NULL)
        Cmd.Parameters("RETURN_VALUE1").Precision = 9			
        Cmd.Parameters.Append Cmd.CreateParameter("PCONTAINERID",adNumeric, adParamInput, , ContainerID) 
        Cmd.Parameters("PCONTAINERID").Precision = 9
        Cmd.Parameters.Append Cmd.CreateParameter("PUSERID",200, adParamInput, 50, UserID)
        Cmd.Parameters.Append Cmd.CreateParameter("PQTYRESERVED",5, adParamInput, 0, QtyRequired)
        Cmd.Parameters.Append Cmd.CreateParameter("PRESERVATIONTYPEID",adNumeric, adParamInput, 0, ReservationTypeID)				
        Cmd.Parameters("PRESERVATIONTYPEID").Precision = 4
        Cmd.Parameters.Append Cmd.CreateParameter("PREQUESTID",adNumeric, adParamInput, 0, RequestID)				
        Cmd.Parameters("PREQUESTID").Precision = 9
	End If 	
    	
    If bDebugPrint Then
	    For each p in Cmd.Parameters
		    Response.Write p.name & " = '" & p.value & "'<BR>"
	    Next
    Else    
        Call ExecuteCmd(Application("CHEMINV_USERNAME") & ".RESERVATIONS.CreateReservation")
    End If
End If
' Return code
Response.Write ReturnValue
'End of change for CSBR 135218
'Clean up
Conn.Close
Set Conn = Nothing
Set Cmd = Nothing
</SCRIPT>
