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


bDebugPrint = false
bWriteError = False
strError = "Error:UpdateRequest<BR>"

RequestID = Request("RequestID")
UserID = Request("UserID")
QtyRequired = Request("QtyRequired")
LocationID = Request("LocationID")
DateRequired = Request("DateRequired")
RequestComments = Request("RequestComments")
RequestTypeID = Request("RequestTypeID")
ContainerTypeID = Request("ContainerTypeID")
NumContainers = Request("NumContainers")
QtyList = Request("QtyList")
ShipToName = Request("ShipToName")
ExpenseCenter = Request("ExpenseCenter")

' Redirect to help page if no parameters are passed
If Len(Request.QueryString) = 0 AND Len(Request.Form)= 0 then
	Response.Redirect "/cheminv/help/admin/api/UpdateRequest.htm"
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
If IsEmpty(UserID) then
	strError = strError & "UserID is a required parameter<BR>"
	bWriteError = True
End if
If IsEmpty(LocationID) then
	strError = strError & "LocationID is a required parameter<BR>"
	bWriteError = True
End if
'Validate date parameters
	if DateRequired = "" then
		DateRequired = NULL
	Elseif IsDate(DateRequired) then
		DateRequired = CDate(DateRequired)
	Else
		strError = strError & "DateRequired could not be interpreted as a valid date<BR>"
		bWriteError = True
	End if

if RequestComments = "" OR IsEmpty(RequestComments) then RequestComments = NULL

'Request containers  required fields
If RequestTypeID = "1" then
	If IsEmpty(QtyRequired) then
		strError = strError & "QtyRequired is a required parameter<BR>"
		bWriteError = True
	End if
'Request samples required fields
elseif RequestTypeID = "2" then
	If IsEmpty(ContainerTypeID) then
		strError = strError & "ContainerTypeID is a required parameter<BR>"
		bWriteError = True
	End if
	If IsEmpty(NumContainers) then
		strError = strError & "NumContainers is a required parameter<BR>"
		bWriteError = True
	End if
	If IsEmpty(QtyList) then
		strError = strError & "QtyList is a required parameter<BR>"
		bWriteError = True
	End if
	If IsEmpty(QtyRequired) then
		strError = strError & "QtyRequired is a required parameter<BR>"
		bWriteError = True
	End if
end if
	
If bWriteError then
	' Respond with Error
	Response.Write strError
	Response.end
End if

' Set up and ADO command
Call GetInvCommand(Application("CHEMINV_USERNAME") & ".REQUESTS.UpdateRequest", adCmdStoredProc)
Cmd.Parameters.Append Cmd.CreateParameter("RETURN_VALUE",adNumeric, adParamReturnValue, 0, NULL)
Cmd.Parameters("RETURN_VALUE").Precision = 9			
Cmd.Parameters.Append Cmd.CreateParameter("PRequestID",adNumeric, adParamInput, 0, RequestID) 
Cmd.Parameters("PRequestID").Precision = 9
Cmd.Parameters.Append Cmd.CreateParameter("PQTYREQUIRED",5, adParamInput, 0, QtyRequired)
Cmd.Parameters.Append Cmd.CreateParameter("PDATEREQUIRED",135, 1, 0, DateRequired)
Cmd.Parameters.Append Cmd.CreateParameter("PUSERID",200, adParamInput, 30, UserID)
Cmd.Parameters.Append Cmd.CreateParameter("PDELIVERYLOCATION",adNumeric, adParamInput, 0, LocationID)				
Cmd.Parameters.Append Cmd.CreateParameter("PREQUESTCOMMNETS",200, adParamInput, 4000, RequestComments)
Cmd.Parameters.Append Cmd.CreateParameter("PCONTAINERTYPEID",adNumeric, adParamInput, 0, ContainerTypeID)				
Cmd.Parameters.Append Cmd.CreateParameter("PNUMBERCONTAINERS",adNumeric, adParamInput, 0, NumContainers)				
Cmd.Parameters.Append Cmd.CreateParameter("PQUANTITYLIST",200, adParamInput, 255, QtyList)				
Cmd.Parameters.Append Cmd.CreateParameter("PSHIPTONAME",200, adParamInput, 50, ShipToName)			
Cmd.Parameters.Append Cmd.CreateParameter("PEXPENSECENTER",200, adParamInput, 255, ExpenseCenter)	
if PrintDebug then
	For each p in Cmd.Parameters
		Response.Write p.name & " = " & p.value & "<BR>"
	Next	
Else
	Call ExecuteCmd(Application("CHEMINV_USERNAME") & ".REQUESTS.UpdateRequest")
End if

' Return code
Response.Write Cmd.Parameters("RETURN_VALUE")

'Clean up
Conn.Close
Set Conn = Nothing
Set Cmd = Nothing
</SCRIPT>
