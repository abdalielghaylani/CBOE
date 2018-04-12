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

bDebugPrint = False
bWriteError = False
strError = "Error:UpdateBatch<br />"

BatchID = trim(Request("BatchID"))
batchStatusId = Request("batchStatusId")
MinThreshold = Request("MinThreshold")
Comments = Replace(Request("NewComments"),"'","''")
For each Key in custom_batch_property_fields_dict
	execute(key & " = Request(""" & key & """)")
Next	

' Redirect to help page if no parameters are passed
If Len(Request.QueryString) = 0 AND Len(Request.Form)= 0 then
	Response.Redirect "/cheminv/help/admin/api/UpdateBatch.htm"
	Response.end
End if

'Echo the input parameters if requested
If NOT isEmpty(Request.QueryString("Echo")) then
	Response.Write "FormData = " & Request.form & "<BR>QueryString = " & Request.QueryString
	Response.end
End if

' Check for required parameters
If IsEmpty(p_BatchID) then
	strError = strError & "p_BatchID is a required parameter<BR>"
	bWriteError = True
End if

For each Key in req_custom_batch_property_fields_dict
	if IsEmpty(Request(Key)) then
		strError = strError & Key & " is a required parameter<br>"
		bWriteError = True
	end if
Next	
 
if batchStatusId = "" then batchStatusId = null
if MinThreshold = "" then MinThreshold = NULL 
if Comments = "" then Comments = " " 

'-- Define default values for custom request fields
if FIELD_1 = "" then FIELD_1 = NULL end if
if FIELD_2 = "" then FIELD_2 = NULL end if
if FIELD_3 = "" then FIELD_3 = NULL end if
if FIELD_4 = "" then FIELD_4 = NULL end if
if FIELD_5 = "" then FIELD_5 = NULL end if
if DATE_1 = "" then DATE_1 = NULL end if
if DATE_2 = "" then DATE_2 = NULL end if

' Set up and ADO command
Call GetInvCommand(Application("CHEMINV_USERNAME") & ".BATCH.UPDATEBATCHFIELDS", adCmdStoredProc)
Cmd.Parameters.Append Cmd.CreateParameter("RETURN_VALUE",adNumeric, adParamReturnValue, 0, NULL)
Cmd.Parameters("RETURN_VALUE").Precision = 9
Cmd.Parameters.Append Cmd.CreateParameter("P_BATCHID",131, 1, 0, BatchID)
Cmd.Parameters("P_BATCHID").Precision = 9
Cmd.Parameters.Append Cmd.CreateParameter("P_BATCHSTATUSID",131, 1, 0, batchStatusId)
Cmd.Parameters("P_BATCHSTATUSID").Precision = 6
Cmd.Parameters.Append Cmd.CreateParameter("P_MINSTOCKTHRESHOLD",131, 1, 0, MinThreshold)
Cmd.Parameters("P_MINSTOCKTHRESHOLD").Precision = 6
Cmd.Parameters.Append Cmd.CreateParameter("P_COMMENTS", 201, adParamInput, len(Comments)+1, Comments)
Cmd.Properties("SPPrmsLOB") = TRUE
Cmd.Parameters.Append Cmd.CreateParameter("P_FIELD_1",200, adParamInput, 2000, FIELD_1)
Cmd.Parameters.Append Cmd.CreateParameter("P_FIELD_2",200, adParamInput, 2000, FIELD_2)
Cmd.Parameters.Append Cmd.CreateParameter("P_FIELD_3",200, adParamInput, 2000, FIELD_3)
Cmd.Parameters.Append Cmd.CreateParameter("P_FIELD_4",200, adParamInput, 2000, FIELD_4)
Cmd.Parameters.Append Cmd.CreateParameter("P_FIELD_5",200, adParamInput, 2000, FIELD_5)
Cmd.Parameters.Append Cmd.CreateParameter("P_DATE_1",135, 1, 0, DATE_1)
Cmd.Parameters.Append Cmd.CreateParameter("P_DATE_2",135, 1, 0, DATE_2)

if bDebugPrint then
	For each p in Cmd.Parameters
		Response.Write p.name & " = " & p.value & "<br />"
	Next	
Else
	Call ExecuteCmd(Application("CHEMINV_USERNAME") & ".BATCH.UPDATEBATCHFIELDS")
End if

' Return the newly created LocationID
Response.Write Cmd.Parameters("RETURN_VALUE")

'Clean up
Conn.Close
Set Conn = Nothing
Set Cmd = Nothing
</SCRIPT>
