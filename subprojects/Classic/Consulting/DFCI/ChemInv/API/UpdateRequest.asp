<%@ EnableSessionState=False Language=VBScript%>
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/ado.inc"-->
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/server_const_vbs.asp"-->
<!--#INCLUDE VIRTUAL = "/cheminv/api/apiUtils.asp"-->
<script RUNAT="Server" Language="VbScript">
Dim Conn
Dim Cmd
Dim strError
Dim bWriteError
Dim PrintDebug
Dim LocationID

bDebugPrint = false
bWriteError = False
strError = "Error:UpdateRequest<BR>"
RequestSampleByAmount = false
if Application("RequestSampleByAmount") = "1" then RequestSampleByAmount = true

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
RequestStatusID = Request("RequestStatusID")
OrgUnitID = Request("OrgUnitID")
AssignedUserID = Request("AssignedUserID")
if Request("RequiredUOM")<>""  then 
   RequiredUOM = Request("RequiredUOM")    
end if 
For each Key in custom_createrequest_fields_dict
	execute(key & " = Request(""" & key & """)")
Next	

if RequestTypeID = "1" then
    QtyRequired = Request("ContainerQtyRequired")
    NumContainers = null
else
    QtyRequired = Request("QtyRequired")
    NumContainers = Request("NumContainers")
end if

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
If IsEmpty(LocationID) then
	strError = strError & "LocationID is a required parameter<BR>"
	bWriteError = True
End if
If IsEmpty(RequestTypeID) then
	strError = strError & "RequestTypeID is a required parameter<BR>"
	bWriteError = True
End if
If IsEmpty(QtyRequired) then
	strError = strError & "QtyRequired is a required parameter<BR>"
	bWriteError = True
End if
'-- there are different required parameters based on request type
if RequestTypeID = "1" then
    If IsEmpty(UserID) then
	    strError = strError & "UserID is a required parameter<BR>"
	    bWriteError = True
    End if
elseif RequestTypeID = "2" then
    If IsEmpty(UserID) and IsEmpty(OrgUnitID) then
	    strError = strError & "UserID or OrgUnitID is a required parameter<BR>"
	    bWriteError = True
    End if
    If IsEmpty(RequestStatusID) then
	    strError = strError & "RequestStatusID is a required parameter<BR>"
	    bWriteError = True
    End if
    if not RequestSampleByAmount then
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
    end if	   
end if
For each Key in req_custom_createrequest_fields_dict
	if IsEmpty(Request(Key)) then
		strError = strError & Key & " is a required parameter<br>"
		bWriteError = True
	end if
Next	

'Validate date parameters
if DateRequired = "" then
	DateRequired = NULL
Elseif IsDate(DateRequired) then
	DateRequired = CDate(DateRequired)
Else
	strError = strError & "DateRequired could not be interpreted as a valid date<BR>"
	bWriteError = True
End if

'-- set default values
if RequestComments = "" OR IsEmpty(RequestComments) then RequestComments = NULL
if UserID = "" then UserID = NULL end if
if OrgUnitID = "" then OrgUnitID = NULL end if

'-- Define default values for custom request fields
if FIELD_1 = "" then FIELD_1 = NULL end if
if FIELD_2 = "" then FIELD_2 = NULL end if
if FIELD_3 = "" then FIELD_3 = NULL end if
if FIELD_4 = "" then FIELD_4 = NULL end if
if FIELD_5 = "" then FIELD_5 = NULL end if
if DATE_1 = "" then DATE_1 = NULL end if
if DATE_2 = "" then DATE_2 = NULL end if

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
Cmd.Parameters.Append Cmd.CreateParameter("PUSERID",200, adParamInput, 30, UserID)
Cmd.Parameters.Append Cmd.CreateParameter("PDATEREQUIRED",135, 1, 0, DateRequired)
Cmd.Parameters.Append Cmd.CreateParameter("PQTYREQUIRED",5, adParamInput, 0, QtyRequired)
Cmd.Parameters.Append Cmd.CreateParameter("PDELIVERYLOCATION",adNumeric, adParamInput, 0, LocationID)				
Cmd.Parameters.Append Cmd.CreateParameter("PREQUESTCOMMENTS",200, adParamInput, 4000, RequestComments)
Cmd.Parameters.Append Cmd.CreateParameter("PREQUESTTYPEID",adNumeric, adParamInput, 0, RequestTypeID)				
Cmd.Parameters.Append Cmd.CreateParameter("PREQUESTSTATUSID",adNumeric, adParamInput, 0, RequestStatusID)			
Cmd.Parameters.Append Cmd.CreateParameter("PCONTAINERTYPEID",adNumeric, adParamInput, 0, ContainerTypeID)				
Cmd.Parameters.Append Cmd.CreateParameter("PNUMBERCONTAINERS",adNumeric, adParamInput, 0, NumContainers)				
Cmd.Parameters.Append Cmd.CreateParameter("PQUANTITYLIST",200, adParamInput, 255, QtyList)				
Cmd.Parameters.Append Cmd.CreateParameter("PSHIPTONAME",200, adParamInput, 50, ShipToName)				
Cmd.Parameters.Append Cmd.CreateParameter("PEXPENSECENTER",200, adParamInput, 255, ExpenseCenter)	
Cmd.Parameters.Append Cmd.CreateParameter("PORGUNITID",200, adParamInput, 30, OrgUnitID)
Cmd.Parameters.Append Cmd.CreateParameter("PASSIGNEDUSERID",200, adParamInput, 30, AssignedUserId)
Cmd.Parameters.Append Cmd.CreateParameter("PFIELD1",200, adParamInput, 2000, FIELD_1)
Cmd.Parameters.Append Cmd.CreateParameter("PFIELD2",200, adParamInput, 2000, FIELD_2)
Cmd.Parameters.Append Cmd.CreateParameter("PFIELD3",200, adParamInput, 2000, FIELD_3)
Cmd.Parameters.Append Cmd.CreateParameter("PFIELD4",200, adParamInput, 2000, FIELD_4)
Cmd.Parameters.Append Cmd.CreateParameter("PFIELD5",200, adParamInput, 2000, FIELD_5)
Cmd.Parameters.Append Cmd.CreateParameter("PDATE1",135, 1, 0, DATE_1)
Cmd.Parameters.Append Cmd.CreateParameter("PDATE2",135, 1, 0, DATE_2)
Cmd.Parameters.Append Cmd.CreateParameter("PREQUIREDUNITID",200, adParamInput, 30, RequiredUOM)
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
</script>
