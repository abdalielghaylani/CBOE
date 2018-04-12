<%@ EnableSessionState=False Language=VBScript%>
<!--#INCLUDE VIRTUAL = "/cheminv/gui/guiUtils.asp"-->
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/ado.inc"-->
<!--#INCLUDE VIRTUAL = "/cheminv/api/apiUtils.asp"-->
<script RUNAT="Server" Language="VbScript">
Dim Conn
Dim Cmd
Dim httpResponse
Dim FormData
Dim Credentials
Dim QueryString
Dim ServerName
Dim arrValues()
Dim ContainerID
Dim Action
Dim NumContainers
Dim ContainerSize
Dim CurrValue
Dim bSameValue
Dim bDebugPrint
Dim strError
Dim bWriteError

Response.Expires = -1

bDebugPrint = FALSE
bWriteError = FALSE

RequestID = Request("RequestID")
SampleContainerIDs = Request("SampleContainerIDs")

'-- CSBR ID:123488
'-- Change Done by : Manoj Unnikrishnan
'-- Purpose: Getting the Custom Fullfill Request fields from the form and replacing the single quote (') with 2 single qoutes ('')
'-- Date: 09/04/2010
For each Key in custom_fulfillrequest_fields_dict
	execute(key & " = Replace(Request(""" & key & """),""'"",""''"")")
Next	
'-- End of Change #123488#

' Check for required parameters
If IsEmpty(RequestID) then
	strError = strError & "RequestID is a required parameter<BR>"
	bWriteError = True
End if
If IsEmpty(SampleContainerIDs) then
	strError = strError & "SampleContainerIDs is a required parameter<BR>"
	bWriteError = True
end if

If bWriteError then
	' Respond with Error
	Response.Write strError
	Response.end
End if

'-- CSBR ID:123488
'-- Change Done by : Manoj Unnikrishnan
'-- Purpose: Define default values for custom fulfill request fields
'-- Date: 09/04/2010
if FIELD_1 = "" then FIELD_1 = NULL end if
if FIELD_2 = "" then FIELD_2 = NULL end if
if FIELD_3 = "" then FIELD_3 = NULL end if
if FIELD_4 = "" then FIELD_4 = NULL end if
if FIELD_5 = "" then FIELD_5 = NULL end if
if DATE_1 = "" then DATE_1 = NULL end if
if DATE_2 = "" then DATE_2 = NULL end if
'-- End of Change #123488#

Call GetInvCommand(Application("CHEMINV_USERNAME") & ".REQUESTS.FulfillRequest", adCmdStoredProc)	
Cmd.Parameters.Append Cmd.CreateParameter("PREQUESTID",131, 1, 0, RequestID)
Cmd.Parameters("PREQUESTID").Precision = 9	
Cmd.Parameters.Append Cmd.CreateParameter("PSAMPLECONTAINERIDS",advarchar, 1, 500, SampleContainerIDs)
'-- CSBR ID:123488
'-- Change Done by : Manoj Unnikrishnan
'-- Purpose: setting the custom fulfill request fields to the parameter
'-- Date: 08/04/2010
Cmd.Parameters.Append Cmd.CreateParameter("P_FIELD_1",200, adParamInput, 2000, FIELD_1)
Cmd.Parameters.Append Cmd.CreateParameter("P_FIELD_2",200, adParamInput, 2000, FIELD_2)
Cmd.Parameters.Append Cmd.CreateParameter("P_FIELD_3",200, adParamInput, 2000, FIELD_3)
Cmd.Parameters.Append Cmd.CreateParameter("P_FIELD_4",200, adParamInput, 2000, FIELD_4)
Cmd.Parameters.Append Cmd.CreateParameter("P_FIELD_5",200, adParamInput, 2000, FIELD_5)
Cmd.Parameters.Append Cmd.CreateParameter("P_DATE_1",135, 1, 0, DATE_1)
Cmd.Parameters.Append Cmd.CreateParameter("P_DATE_2",135, 1, 0, DATE_2)
if bDebugPrint then
    For each p in Cmd.Parameters
        Response.Write p.name & " = " & p.value & "<BR>"
    Next	
Else
    Call ExecuteCmd(Application("CHEMINV_USERNAME") & ".REQUESTS.FulfillRequest")
End if
'-- End of Change #123488#


/>