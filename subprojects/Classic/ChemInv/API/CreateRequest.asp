<%@ EnableSessionState=False Language=VBScript CodePage = 65001%>
<%Response.Charset="UTF-8"%>
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/ado.inc"-->
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/server_const_vbs.asp"-->
<!--#INCLUDE VIRTUAL = "/cheminv/api/apiUtils.asp"-->
<!--#INCLUDE VIRTUAL = "/cheminv/api/submitBlob.asp"-->
<script RUNAT="Server" Language="VbScript">
Dim Conn
Dim Cmd
Dim strError
Dim bWriteError
Dim PrintDebug
Dim LocationID
'CSBR ID : 135218 SJ
'Comments: Enabling Automatic reservation of containers for container\sample requests
'Start of change
Dim AutomaticReservation
AutomaticReservation=false
If Application("AutomaticReservation") = "1" Then AutomaticReservation = true
'End of change for CSBR 135218
bDebugPrint = false
bWriteError = False
strError = "Error:CreateRequest<BR>"
RequestSampleByAmount = false
if Application("RequestSampleByAmount") = "1" then RequestSampleByAmount = true

ContainerID = Request("ContainerID")
BatchID = Request("BatchID")
UserID = Request("UserID")
DateRequired = Request("DateRequired")
LocationID = Request("LocationID")
RequestComments = Request("RequestComments")
RequestTypeID = Request("RequestTypeID")
RequestStatusID = Request("RequestStatusID")
ContainerTypeID = Request("ContainerTypeID")
QtyList = Request("QtyList")
ShipToName = Request("ShipToName")
ExpenseCenter = Request("ExpenseCenter")
OrgUnitID = Request("OrgUnitID")
AssignedUserID = Request("AssignedUserID")
FileFullPath = Request("FileFullPath") 
fName = Request("fName") 
SpecialInstructions = Request("specialInstructions")

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
	Response.Redirect "/cheminv/help/admin/api/CreateRequest.htm"
	Response.end
End if

'Echo the input parameters if requested
If NOT isEmpty(Request.QueryString("Echo")) then
	Response.Write "FormData = " & Request.form & "<BR>QueryString = " & Request.QueryString
	Response.end
End if


' Check for required parameters
'-- there are different required parameters based on request type
if RequestTypeID = "1" then
    If IsEmpty(ContainerID) then
	    strError = strError & "ContainerID is a required parameter<BR>"
	    bWriteError = True
    End if
    If IsEmpty(UserID) then
	    strError = strError & "UserID is a required parameter<BR>"
	    bWriteError = True
    End if

elseif RequestTypeID = "2" then
    If IsEmpty(BatchID) then
	    strError = strError & "BatchID is a required parameter<BR>"
	    bWriteError = True
    End if
    If IsEmpty(UserID) and IsEmpty(OrgUnitID) then
	    strError = strError & "UserID or OrgUnitID is a required parameter<BR>"
	    bWriteError = True
    End if
    If IsEmpty(RequestStatusID) then
	    strError = strError & "RequestStatusID is a required parameter<BR>"
	    bWriteError = True
    End if
    If IsEmpty(RequiredUOM) then
	    strError = strError & "UOM is a required parameter<BR>"
	    bWriteError = True
    End if
    
end if
'Request containers  required fields
If RequestTypeID = "1" or RequestSampleByAmount then
	If IsEmpty(QtyRequired) then
		strError = strError & "QtyRequired is a required parameter<BR>"
		bWriteError = True
	End if
	If IsEmpty(BatchID) or BatchID = "" then BatchID = null
'Request samples required fields
elseif RequestTypeID = "2" and not RequestSampleByAmount then
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
	QtyRequired = QtyRequired
	'set QtyRequired to 0
	'QtyRequired = 0
end if
If IsEmpty(RequestTypeID) then
	strError = strError & "RequestTypeID is a required parameter<BR>"
	bWriteError = True
End if
If IsEmpty(LocationID) then
	strError = strError & "LocationID is a required parameter<BR>"
	bWriteError = True
End if
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
    DateRequired = ConvertStrToDate(Application("DATE_FORMAT"),DateRequired) 
	'DateRequired = CDate(DateRequired)
Else
	strError = strError & "DateRequired could not be interpreted as a valid date<BR>"
	bWriteError = True
End if
	
'-- set default values
if RequestComments = "" OR IsEmpty(RequestComments) then RequestComments = NULL
if UserID = "" then UserID = NULL end if
if OrgUnitID = "" then OrgUnitID = NULL end if
if RequestTypeID = "2" then
    if ContainerID = "" or isEmpty(ContainerID) then ContainerID = null
end if

'-- Define default values for custom request fields
if FIELD_1 = "" then FIELD_1 = NULL end if
if FIELD_2 = "" then FIELD_2 = NULL end if
if FIELD_3 = "" then FIELD_3 = NULL end if
if FIELD_4 = "" then FIELD_4 = NULL end if
if FIELD_5 = "" then FIELD_5 = NULL end if
if DATE_1 = "" then DATE_1 = NULL end if
if DATE_2 = "" then DATE_2 = NULL end if

if FileFullPath = "" or isEmpty(FileFullPath) then 
    FileFullPath = NULL
    fName = NULL
    fSize = 0
    fType = NULL 
else
    Set fso = CreateObject("Scripting.FileSystemObject")

    if fso.FileExists(FileFullPath) then
	    Set f = fso.GetFile(FileFullPath)
	    fSize = f.size
	    fType = GetFileType(fName)
    end if    
end if

If bWriteError then
	' Respond with Error
	Response.Write strError
	Response.end
End if


' Set up and ADO command
Call GetInvCommand(Application("CHEMINV_USERNAME") & ".REQUESTS.CreateRequest", adCmdStoredProc)
Cmd.Parameters.Append Cmd.CreateParameter("RETURN_VALUE",adNumeric, adParamReturnValue, 0, NULL)
Cmd.Parameters("RETURN_VALUE").Precision = 9			
Cmd.Parameters.Append Cmd.CreateParameter("PCONTAINERID",adNumeric, adParamInput, , ContainerID) 
Cmd.Parameters("PCONTAINERID").Precision = 9
Cmd.Parameters.Append Cmd.CreateParameter("PBATCHID",adNumeric, adParamInput, , BatchID) 
Cmd.Parameters("PBATCHID").Precision = 9
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

Cmd.Parameters.Append Cmd.CreateParameter("PSPECIALINSTRUCTIONS",200, adParamInput, 4000, SpecialInstructions)
Cmd.Parameters.Append Cmd.CreateParameter("PPROOFAPPROVALFILENAME",200, adParamInput, 100, fName)
Cmd.Parameters.Append Cmd.CreateParameter("PPROOFAPPROVALFILETYPE",200, adParamInput, 10, fType)
Cmd.Parameters.Append Cmd.CreateParameter("PPROOFAPPROVALFILESIZE",adNumeric, adParamInput, 0, fSize)
if bDebugPrint then
	For each p in Cmd.Parameters
		Response.Write p.name & " = '" & p.value & "'<BR>"
	Next	
Else
	Call ExecuteCmd(Application("CHEMINV_USERNAME") & ".REQUESTS.CreateRequest")
	RequestID = Cmd.Parameters("RETURN_VALUE") 'CSBR 135218
End if

uploadResult = SaveUploadToDB(FileFullPath, RequestID)




'CSBR ID : 135218 SJ
'Comments: Enabling Automatic reservation of containers for container\sample requests
'Start of change
If AutomaticReservation Then
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
    
    If bDebugPrint Then
	    For each p in Cmd.Parameters
		    Response.Write p.name & " = '" & p.value & "'<BR>"
	    Next
    Else    
        Call ExecuteCmd(Application("CHEMINV_USERNAME") & ".RESERVATIONS.CreateReservation")
    End If
End If
' Return code
Response.Write RequestID
'End of change for CSBR 135218
'Clean up
Conn.Close
Set Conn = Nothing
Set Cmd = Nothing
</script>
