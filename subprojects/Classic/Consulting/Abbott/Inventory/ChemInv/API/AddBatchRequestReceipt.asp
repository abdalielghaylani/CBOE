<!--#INCLUDE VIRTUAL = "/cfserverasp/source/ado.inc"-->
<!--#INCLUDE VIRTUAL = "/cheminv/api/apiUtils.asp"-->
<!--#INCLUDE VIRTUAL = "/cheminv/gui/guiUtils.asp"-->

<%
Dim Conn
Dim Cmd
Dim strError
Dim bWriteError
Dim PrintDebug

bDebugPrint = false
bWriteError = false
strError = "Error:CancelRequest<BR>"

RequestID = Request("RequestID")

'-- Redirect to help page if no parameters are passed
If Len(Request.QueryString) = 0 AND Len(Request.Form)= 0 then
	Response.Redirect "/cheminv/help/admin/api/AddBatchRequestWorkSheet.htm"
	Response.end
End if

'-- Echo the input parameters if requested
If NOT isEmpty(Request.QueryString("Echo")) then
	Response.Write "FormData = " & Request.form & "<BR>QueryString = " & Request.QueryString
	Response.end
End if

'-- Check for required parameters
If IsEmpty(RequestID) then
	strError = strError & "RequestID is a required parameter<BR>"
	bWriteError = True
End if

If bWriteError then
	Response.Write strError
	Response.end
End if

UOMAbbrv = Request("UOMAbbrv")
if not IsEmpty(Application("DEFAULT_SAMPLE_REQUEST_CONC")) then
	arrUOM = split(Application("DEFAULT_SAMPLE_REQUEST_CONC"),"=")
	UOMAbbrv = arrUOM(1)
end if

LocationID = Session("CurrentLocationID")
cLocationID = Request("LocationID")
if cLocationID = "" then cLocationID = 0
dateFormatString = Application("DATE_FORMAT_STRING")

'-- Build Receipt HTML
caption = "Request Receipt"
strHtml = ""

'Response.Write("@@@" & Request.Form & "<br>")
'Response.End


'-- Retreive general request information
Call GetInvCommand("{CALL " & Application("CHEMINV_USERNAME") & ".REQUESTS.GetBatchRequest(?,?)}", adCmdText)
Cmd.Parameters.Append Cmd.CreateParameter("PREQUESTID",131, 1, 0, RequestID)
Cmd.Parameters("PREQUESTID").Precision = 9
Cmd.Parameters.Append Cmd.CreateParameter("PDATEFORMAT",200,1,30, dateFormatString)
Cmd.Properties ("PLSQLRSet") = TRUE
Set RS = Cmd.Execute
Cmd.Properties ("PLSQLRSet") = FALSE
If (RS.EOF AND RS.BOF) then
	Response.Write ("<table><tr><td align=center colspan=6><span class=""GUIFeedback"">No requests found for this container</span></td></tr></table>")
	Response.End
Else
	UserID = RS("RUserID")
	DateRequired = ConvertDateToStr(Application("DATE_FORMAT"),RS("Date_Required"))
	DateDelivered = ConvertDateToStr(Application("DATE_FORMAT"),RS("Date_Delivered"))
	LocationID = RS("delivery_Location_ID_FK")
	comments = RS("request_comments")
	QtyRequired = RS("qty_required")
	BatchID = RS("batch_id_fk")
	'RUserID = RS("RUserID")
	'DUserID = RS("DUserID")
	RequestedFor = RS("RUserID")
	RequestedBy = RS("DUserID")
	Field_1 = RS("field_1")
	Field_2 = RS("field_2")
	Field_3 = RS("field_3")
	Field_4 = RS("field_4")
	Field_5 = RS("field_5")
	Date_1 = RS("date_1")
	Date_2 = RS("date_2")
End if

'-- Retreive Custom Request fields
reqHtml = ""
For each Key in custom_createrequest_fields_dict
	reqHtml = reqHtml & "<tr><td align=""right"">" & custom_createrequest_fields_dict.item(key) & "</td>" & vbcrlf
	execute("TempValue = " & Key)
	reqHtml = reqHtml & "<td bgcolor=""#d3d3d3"">" & TempValue & "</td></tr>" & vbcrlf
Next
For each Key in custom_fulfillrequest_fields_dict
	reqHtml = reqHtml & "<tr><td align=""right"">" & custom_fulfillrequest_fields_dict.item(key) & "</td>" & vbcrlf
	execute("if Request(""" & Key & """) <> """" then TempValue = Request(""" & Key & """) else TempValue = " & Key & " end if")
	'execute("TempValue = " & Key)
	reqHtml = reqHtml & "<td bgcolor=""#d3d3d3"">" & TempValue & "</td></tr>" & vbcrlf
Next

QtySelected=Request("QtyAvailable")
QtyRequired=Request("QtyRequired")

'-- Retreive Batch information from view
dbkey = "ChemInv"
GetInvConnection()
sql = "SELECT * FROM inv_vw_reg_batches vb, inv_container_batches b "
sql = sql & "WHERE b.batch_field_1 = vb.RegID and b.batch_field_2 = vb.BatchNumber "
sql = sql & "And b.batch_id = "  & BatchID
Set RS = Conn.Execute(sql)
regHtml = ""
if Not(RS.BOF and RS.EOF) then
	regHtml = regHtml & "<tr><td colspan=""2"" align=""center""><em><b>"
	if RS("RegName") <> "" then
		regHtml = regHtml & RS("RegName")
	else
		regHtml = regHtml & "No Substance Name"
	end if
	regHtml = regHtml & "</b></em></td></tr>"
	for each key in reg_fields_dict
		if key <> "BASE64_CDX" and key <> "REGNAME" then
			regHtml = regHtml & "<tr><td align=""right"">" & reg_fields_dict.item(key) & ":</td><td bgcolor=""#d3d3d3"">" & RS(key) & "</td></tr>"
		end if
	next
end if
RS.Close()
Set RS = Nothing

strHtml = strHtml & "<html>" & vbcrlf
strHtml = strHtml & "<head>" & vbcrlf
strHtml = strHtml & "<title>" & Application("appTitle") & "-- Request an Inventory Container</title>" & vbcrlf
strHtml = strHtml & "<script language=""javascript"" src=""/cheminv/choosecss.js""></script>" & vbcrlf
strHtml = strHtml & "<script language=""javascript"" src=""/cheminv/utils.js""></script>" & vbcrlf
strHtml = strHtml & "<script language=""javascript"" src=""/cheminv/gui/validation.js""></script>" & vbcrlf
strHtml = strHtml & "<style>" & vbcrlf
strHtml = strHtml & "table.receipt { font-size: 9px; font-family: verdana, arial, helvetica, sans-serif }" & vbcrlf
strHtml = strHtml & "</style></head><body><center>" & vbcrlf
strHtml = strHtml & "Date Created: " & Now() & "<br>" & vbcrlf
strHtml = strHtml & "<form name=""form1"" action="""" method=""post"">" & vbcrlf
strHtml = strHtml & "<table border=""0"" cellpadding=""0"" cellspacing=""0"" class=""receipt"" width=""400"" style=""border-right:1px dashed #999999;border-bottom:1px dashed #999999;border-left:1px dashed #999999;border-top:1px dashed #999999;"">" & vbcrlf
strHtml = strHtml & "<tr><td colspan=""2"" align=""center"" bgcolor=""#cccccc"" style=""border-bottom:1px solid #000000;""><b>" & caption & "</b></td></tr>" & vbcrlf
strHtml = strHtml & "<tr><td valign=""top"">" & vbcrlf
strHtml = strHtml & "	<table border=""0"" class=""receipt"">" & vbcrlf
strHtml = strHtml & "		<tr><td align=""right"">Request ID:</td><td bgcolor=""#d3d3d3"">" & RequestID & "</td></tr>" & vbcrlf
strHtml = strHtml & "		<tr><td align=""right"">Date Required:</td><td bgcolor=""#d3d3d3"">" & DateRequired & "</td></tr>" & vbcrlf
strHtml = strHtml & "		<tr><td align=""right"">Date Delivered:</td><td bgcolor=""#d3d3d3"">" & DateDelivered & "</td></tr>" & vbcrlf
strHtml = strHtml & "		<tr><td align=""right"">Requested By:</td><td bgcolor=""#d3d3d3"">" & RequestedBy & "</td></tr>" & vbcrlf
strHtml = strHtml & "		<tr><td align=""right"">Requested For:</td><td bgcolor=""#d3d3d3"">" & RequestedFor & "</td></tr>" & vbcrlf
strHtml = strHtml & "		<tr><td align=""right"">Amount Requested:</td><td bgcolor=""#d3d3d3"">" & FormatNumber(QtyRequired,2) & "&nbsp;" & UOMAbbrv & "</td></tr>" & vbcrlf
strHtml = strHtml & "		<tr><td align=""right"">Amount Delivered:</td><td bgcolor=""#d3d3d3"">" & FormatNumber(QtySelected,2) & "&nbsp;" & UOMAbbrv & "</td></tr>" & vbcrlf
strHtml = strHtml & reqHtml & vbcrlf
strHtml = strHtml & "		<tr><td align=""right"" valign=""top"">Comments:</td><td bgcolor=""#d3d3d3"">" & comments & "</td></tr>" & vbcrlf
strHtml = strHtml & "		<tr><td colspan=""2"">&nbsp;</td></tr>" & vbcrlf
strHtml = strHtml & "		<tr><td colspan=""2"">&nbsp;</td></tr>" & vbcrlf
strHtml = strHtml & "	</table>" & vbcrlf
strHtml = strHtml & "</td><td valign=""top"">" & vbcrlf
strHtml = strHtml & "	<table border=""0"" class=""receipt"">" & vbcrlf
strHtml = strHtml & regHtml & vbcrlf
strHtml = strHtml & "	</table>" & vbcrlf
strHtml = strHtml & "</td></tr>" & vbcrlf
strHtml = strHtml & "</table>" & vbcrlf
strHtml = strHtml & "<a href=""javascript:window.print()""><img src=""/cheminv/graphics/sq_btn/print_btn.gif"" border=""0""></a>" & vbcrlf
strHtml = strHtml & "&nbsp;<a href=""javascript:window.close()""><img src=""/cheminv/graphics/sq_btn/close_dialog_btn.gif"" border=""0""></a>" & vbcrlf
strHtml = strHtml & "</form></center></body></html>" & vbcrlf

'Response.Write(strHtml)
'Response.End

'-- Define variables for insert
TableName = "INV_REQUESTS"
FieldName = "REQUEST_ID"
FieldValue = RequestID
DocType = 1
Doc = strHtml
'Response.End

'-- Set up and ADO command
Call GetInvCommand(Application("CHEMINV_USERNAME") & ".DOCS.INSERTDOC", adCmdStoredProc)
Cmd.Parameters.Append Cmd.CreateParameter("RETURN_VALUE",adNumeric, adParamReturnValue, 0, NULL)
Cmd.Parameters("RETURN_VALUE").Precision = 9
Cmd.Parameters.Append Cmd.CreateParameter("P_TABLENAME", 200, adParamInput, 255, TableName)
Cmd.Parameters.Append Cmd.CreateParameter("P_FIELDNAME", 200, adParamInput, 255, FieldName)
Cmd.Parameters.Append Cmd.CreateParameter("P_FIELDVALUE", 200, adParamInput, 255, FieldValue)
Cmd.Parameters.Append Cmd.CreateParameter("P_DOCTYPE",adNumeric, adParamInput, 0, DocType)
Cmd.Parameters("P_DOCTYPE").Precision = 9
Cmd.Parameters.Append Cmd.CreateParameter("P_DOC", 201, adParamInput, len(Doc)+1, Doc)
Cmd.Properties("SPPrmsLOB") = TRUE

if bDebugPrint then
	For each p in Cmd.Parameters
		Response.Write p.name & " = " & p.value & "<br>"
	Next
Else
	Call ExecuteCmd(Application("CHEMINV_USERNAME") & ".DOCS.INSERTDOC")
End if
Cmd.Properties("SPPrmsLOB") = FALSE

'-- Return code
Response.Write Cmd.Parameters("RETURN_VALUE")

'-- Clean up
Conn.Close
Set Conn = Nothing
Set Cmd = Nothing

%>
