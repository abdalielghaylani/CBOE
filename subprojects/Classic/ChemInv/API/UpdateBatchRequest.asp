<%@ EnableSessionState=False Language=VBScript%>
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/ado.inc"-->
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/server_const_vbs.asp"-->
<!--#INCLUDE VIRTUAL = "/cheminv/api/apiUtils.asp"-->
<!--#INCLUDE VIRTUAL = "/cheminv/api/submitBlob.asp"-->
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
BatchID = Request("BatchID")
UserID = Request("UserID")
OrgUnitID = Request("OrgUnitID")
QtyRequired = Request("QtyRequired")
specialInstructions=Request("specialInstructions")
fName = Request("FName")
FileFullPath = Request("FileFullPath") 

'CSBR ID:121371
'Change Done by : Siby Jacob
'Date: 18/02/2010		
RequestComments = Request("RequestComments") ' Get the Request Comments from the UI
'End of Change #121371#
LocationID = Request("LocationID")
DateRequired = Request("DateRequired")
RequestTypeID = Request("RequestTypeID")
RequestStatusID = Request("RequestStatusID")
shipToName = Request("shipToName")
Field_1 = Request("Field_1")
Field_2 = Request("Field_2")
Field_3 = Request("Field_3")
Field_4 = Request("Field_4")
Field_5 = Request("Field_5")
Date_1 = Request("Date_1")
Date_2 = Request("Date_2")
OldOrgUnitID = Request("OldOrgUnitID")
OldDateRequired = Request("OldDateRequired")
OldLocationID = Request("OldLocationID")
OldQtyRequired = Request("OldQtyRequired")
OldAmountReserved = Request("OldAmountReserved")
OldAmountRemaining = Request("OldAmountRemaining")
OldAmountAvailable = Request("OldAmountAvailable")
Oldcomments = Request("Oldcomments")
OldField_1 = Request("OldField_1")
OldField_2 = Request("OldField_2")
OldField_3 = Request("OldField_3")
OldField_4 = Request("OldField_4")
OldField_5 = Request("OldField_5")
OldDate_1 = Request("OldDate_1")
OldDate_2 = Request("OldDate_2")
OldUOMAbbrv = Request("OldUOMAbbrv")
Oldunitstring = Request("Oldunitstring")
Oldfname = Request("Oldfname")
OldRequestStatusID = Request("OldRequestStatusID")
OldspecialInstructions = Request("OldspecialInstructions")
OldUserID = Request("OldUserID")
OldDateRequired = Request("OldDateRequired")
InvSampleApprove = Request("InvSampleApprove")
If InvSampleApprove = "Approve" then 
    RequestStatusID = OldRequestStatusID
elseif (FIELD_1 = OldFIELD_1) and (FIELD_2 = OldFIELD_2) and (FIELD_3 = OldFIELD_3) and (FIELD_4 = OldFIELD_4) and (OrgUnitID = OldOrgUnitID) and (DATE_1 = OldDATE_1) and (DATE_2 = OldDATE_2) and (RequestComments = Oldcomments)  and (QtyRequired = OldQtyRequired) and (specialInstructions = OldspecialInstructions) and (UserID = OldUserID) and (DateRequired = OldDateRequired) then
    RequestStatusID = OldRequestStatusID
end if
For each Key in custom_createrequest_fields_dict
	execute(key & " = Request(""" & key & """)")
Next	
if Request("RequiredUOM")<>""  then 
   RequiredUOM = Request("RequiredUOM")    
end if 

' Redirect to help page if no parameters are passed
If Len(Request.QueryString) = 0 AND Len(Request.Form)= 0 then
	Response.Redirect "/cheminv/help/admin/api/CreateBatchRequest.htm"
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
If IsEmpty(BatchID) then
	strError = strError & "BatchID is a required parameter<BR>"
	bWriteError = True
End if
If IsEmpty(UserID) and IsEmpty(OrgUnitID) then
	strError = strError & "UserID or OrgUnitID is a required parameter<BR>"
	bWriteError = True
End if
If IsEmpty(RequestTypeID) then
	strError = strError & "RequestTypeID is a required parameter<BR>"
	bWriteError = True
End if
If IsEmpty(RequestStatusID) then
	strError = strError & "RequestStatusID is a required parameter<BR>"
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

'-- Validate date parameters
if DateRequired = "" then
	DateRequired = NULL
Elseif IsDate(DateRequired) then
	DateRequired = CDate(DateRequired)
Else
	strError = strError & "DateRequired could not be interpreted as a valid date<BR>"
	bWriteError = True
End if

If IsEmpty(QtyRequired) then
	strError = strError & "QtyRequired is a required parameter<BR>"
	bWriteError = True
End if

'CSBR ID:121371
'Change Done by : Siby Jacob
'Purpose: Restrict Comments to 4000 characters
'Date: 18/02/2010		
'Comments Field must not have more than 4000 characters
If Len(RequestComments) > 4000 then
	strError = strError & "Comments cannot be greater than 4000 characters<BR>"
	bWriteError = True
End if 
'End of Change #121371#*/	

If bWriteError then
	Response.Write strError
	Response.end
End if

'-- Define default values for custom request fields
if FIELD_1 = "" then FIELD_1 = NULL end if
if FIELD_2 = "" then FIELD_2 = NULL end if
if FIELD_3 = "" then FIELD_3 = NULL end if
if FIELD_4 = "" then FIELD_4 = NULL end if
if FIELD_5 = "" then FIELD_5 = NULL end if
if DATE_1 = "" then DATE_1 = NULL end if
if DATE_2 = "" then DATE_2 = NULL end if

if FileFullPath = "" and fDeleted = "0" then 
    FileFullPath = NULL
    fName = NULL
    fSize = -1
    fType = NULL 
else
    if fDeleted = "1" then
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
end if
' Set up and ADO command
Call GetInvCommand(Application("CHEMINV_USERNAME") & ".REQUESTS.UpdateBatchRequest", adCmdStoredProc)
Cmd.Parameters.Append Cmd.CreateParameter("RETURN_VALUE",adNumeric, adParamReturnValue, 0, NULL)
Cmd.Parameters("RETURN_VALUE").Precision = 9			
Cmd.Parameters.Append Cmd.CreateParameter("P_REQUESTID",adNumeric, adParamInput, , RequestID) 
Cmd.Parameters("P_REQUESTID").Precision = 9
Cmd.Parameters.Append Cmd.CreateParameter("P_BATCHID",adNumeric, adParamInput, , BatchID) 
Cmd.Parameters("P_BATCHID").Precision = 9
Cmd.Parameters.Append Cmd.CreateParameter("P_QTYREQUIRED",5, adParamInput, 0, QtyRequired)
Cmd.Parameters.Append Cmd.CreateParameter("P_DATEREQUIRED",135, 1, 0, DateRequired)
Cmd.Parameters.Append Cmd.CreateParameter("P_USERID",200, adParamInput, 30, UserID)
Cmd.Parameters.Append Cmd.CreateParameter("P_ORGUNITID",200, adParamInput, 30, OrgUnitID)
Cmd.Parameters.Append Cmd.CreateParameter("P_DELIVERYLOCATION",adNumeric, adParamInput, 0, LocationID)				
'CSBR ID:121371
'Change Done by : Siby Jacob
'Date: 18/02/2010		
Cmd.Parameters.Append Cmd.CreateParameter("P_REQUESTCOMMENTS",200, adParamInput, 4000, RequestComments) ' Add the Comments parameter
'End of Change #121371#*/	
Cmd.Parameters.Append Cmd.CreateParameter("P_REQUESTTYPEID",adNumeric, adParamInput, 0, RequestTypeID)				
Cmd.Parameters.Append Cmd.CreateParameter("P_REQUESTSTATUSID",adNumeric, adParamInput, 0, RequestStatusID)			
Cmd.Parameters.Append Cmd.CreateParameter("P_FIELD_1",200, adParamInput, 2000, FIELD_1)
Cmd.Parameters.Append Cmd.CreateParameter("P_FIELD_2",200, adParamInput, 2000, FIELD_2)
Cmd.Parameters.Append Cmd.CreateParameter("P_FIELD_3",200, adParamInput, 2000, FIELD_3)
Cmd.Parameters.Append Cmd.CreateParameter("P_FIELD_4",200, adParamInput, 2000, FIELD_4)
Cmd.Parameters.Append Cmd.CreateParameter("P_FIELD_5",200, adParamInput, 2000, FIELD_5)
Cmd.Parameters.Append Cmd.CreateParameter("P_DATE_1",135, 1, 0, DATE_1)
Cmd.Parameters.Append Cmd.CreateParameter("P_DATE_2",135, 1, 0, DATE_2)
Cmd.Parameters.Append Cmd.CreateParameter("PREQUIREDUNITID",200, adParamInput, 30, RequiredUOM)
Cmd.Parameters.Append Cmd.CreateParameter("P_shipToName",200, adParamInput, 30, shipToName)
Cmd.Parameters.Append Cmd.CreateParameter("P_SPECIAL_INSTRUCTIONS",200, adParamInput, 1000, specialInstructions)
Cmd.Parameters.Append Cmd.CreateParameter("PPROOFAPPROVALFILENAME",200, adParamInput, 100, fName)
Cmd.Parameters.Append Cmd.CreateParameter("PPROOFAPPROVALFILETYPE",200, adParamInput, 10, fType)
Cmd.Parameters.Append Cmd.CreateParameter("PPROOFAPPROVALFILESIZE",adNumeric, adParamInput, 0, fSize)


if bDebugPrint then
	For each p in Cmd.Parameters
		Response.Write p.name & " = " & p.value & "<BR>"
	Next	
Else
	Call ExecuteCmd(Application("CHEMINV_USERNAME") & ".REQUESTS.UpdateBatchRequest")
End if

' Return code
Response.Write Cmd.Parameters("RETURN_VALUE")

call SaveUploadToDB(FileFullPath, RequestID)
'Clean up
Conn.Close
Set Conn = Nothing
Set Cmd = Nothing
</SCRIPT>
