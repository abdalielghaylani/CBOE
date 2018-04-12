<%@ Language=VBScript %>
<!-- #include virtual="/cs_security/variables.asp" -->
<%
dbkey = "ChemInv"
RequestStatusID = Request("RequestStatusID")


%>
<html>
<head>
<meta NAME="GENERATOR" Content="Microsoft Visual Studio 6.0">
<title><%=Application("appTitle")%> -- Manage Sample Requests</title>
<script LANGUAGE="javascript" src="/cheminv/Choosecss.js"></script>
<script LANGUAGE="javascript" src="/cheminv/utils.js"></script>
<script LANGUAGE="javascript">

function UncheckDecline(element){
	var NumRequests = document.form1.NumRequests.value;
	if (NumRequests == 1) {
		document.form1.DeclinedRequestIDList.checked = false;
	}
	else {
		for (i=0; i<NumRequests; i++) {
			if (document.form1.DeclinedRequestIDList[i].value == element.value){
				document.form1.DeclinedRequestIDList[i].checked = false;
			}
		}	
	
	}
}

function UncheckApprove(element){
	var NumRequests = document.form1.NumRequests.value;
	if (NumRequests == 1) {
		document.form1.ApprovedRequestIDList.checked = false;
	}
	else {
		for (i=0; i<NumRequests; i++) {
			if (document.form1.ApprovedRequestIDList[i].value == element.value){
				document.form1.ApprovedRequestIDList[i].checked = false;
			}
		}	
	
	}
}

function ChooseAction(){
	if (document.form1.ApprovedRequestIDList){
		var NumRequests = document.form1.NumRequests.value;
		var ShowDeclineDialogue = false;
		if (NumRequests == 1) {
			if (document.form1.DeclinedRequestIDList.checked) 
				ShowDeclineDialogue = true;
		}
		else {
			for (i=0; i<NumRequests; i++) {
				if (document.form1.DeclinedRequestIDList[i].checked)
					ShowDeclineDialogue = true;
			}	
		}
	
		if (ShowDeclineDialogue){
			action = "DeclineRequestSample.asp";
		}
		else {
			action = "ApproveRequestSample_action.asp";
		}
	}
	else if (document.form1.ClosedRequestIDList) {
		action="CloseRequestSample_action.asp";	
	}
	
	document.form1.action = action;	
	document.form1.submit();
	//alert(document.form1.ApprovedRequestIDList.value);
	//alert(document.form1.DeclinedRequestIDList.value);
	//ApproveRequestSample_action.asp
}

</script>
<style>
.ActiveRow {
	background: #ffff99;
}
</style>


</head>
<body>
<!--#INCLUDE VIRTUAL = "/cheminv/cheminv/fieldArrayUtils.asp" -->
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/ado.inc"-->
<!--#INCLUDE VIRTUAL = "/cheminv/api/apiUtils.asp"-->
<!--#INCLUDE VIRTUAL = "/cheminv/gui/guiUtils.asp"-->
<%
Dim Conn
Dim Cmd
Dim RS1

bDebugPrint = false
bWriteError = False
strError = "Error:ManageSampleRequests_display.asp<BR>"

dateFormatString = Application("DATE_FORMAT_STRING")
UserID = request("userID")
DeliverToLocationID = request("DeliverToLocationID")
CurrentLocationID = request("CurrentLocationID")
containerBarcode = request("containerBarcode")
fromDate = request("fromDate")
toDate = request("toDate")
RequestTypeID = Request("RequestTypeID")
Comments = Request("RequestComments")


if UserID = "" then username = "NULL"
if RequestTypeID = "" then RequestTypeID = 2
if DeliverToLocationID = "" then DeliverToLocationID =0
if CurrentLocationID = "" then CurrentLocationID =0
if fromDate = "" then
	fromDate = "NULL"
Elseif IsDate(fromDate) then
	fromDate = GetOracleDateString3(fromDate)
Else
	strError = strError & "From Date could not be interpreted as a valid date<BR>"
	bWriteError = True
End if
if toDate = "" then
	toDate = "NULL"
Elseif IsDate(toDate) then
	toDate = GetOracleDateString2(toDate)
Else
	strError = strError & "To Date could not be interpreted as a valid date<BR>"
	bWriteError = True
End if

if RequestStatusID = "" then RequestStatusID = null
if ShipToName = "" then ShipToName = null

If bWriteError then
	' Respond with Error
	Response.Write strError
	Response.end
End if

Response.Expires = -1
Call GetInvCommand("{CALL " & Application("CHEMINV_USERNAME") & ".REQUESTS.GetBatchRequests(?,?,?,?,?,?,?,?,?)}", adCmdText)	
Cmd.Parameters.Append Cmd.CreateParameter("PDELIVERTOLOCATIONID",131, 1, 0, DeliverToLocationID)
Cmd.Parameters.Append Cmd.CreateParameter("PCURRENTLOCATIONID",131, 1, 0, CurrentLocationID)
Cmd.Parameters.Append Cmd.CreateParameter("PFROMDATE",200, 1, 200, FromDate)
Cmd.Parameters.Append Cmd.CreateParameter("PTODATE",200, 1, 200, ToDate)
Cmd.Parameters.Append Cmd.CreateParameter("PUSERID",200, 1, 30, UserID)
Cmd.Parameters.Append Cmd.CreateParameter("PRequestType",200, 1, 30, RequestTypeID)
Cmd.Parameters.Append Cmd.CreateParameter("PREQUESTTYPEID",131, 1, 0, RequestTypeID)
Cmd.Parameters.Append Cmd.CreateParameter("PREQUESTSTATUSID",131, 1, 0, RequestStatusID)
Cmd.Parameters.Append Cmd.CreateParameter("PDATEFORMAT",200,1,30, dateFormatString)

if bDebugPrint then
	For each p in Cmd.Parameters
		Response.Write p.name & " = " & p.value & "<BR>"
	Next	
	Response.End
Else
	Cmd.Properties ("PLSQLRSet") = TRUE  
	Set RS1 = Cmd.Execute
	Cmd.Properties ("PLSQLRSet") = FALSE

bShowBatch = false
if RequestStatusID = "2" then
	caption = "The following requests are waiting to be approved:"
	disable1 = "disabled"
	bShowBatch = true

	' Set up fieldArray containing the column definition for the on screen report
	If NOT IsArray(Session("RequestReportFieldArray2")) then
		ColDefStr = GetUserProperty(Session("UserNameCheminv"),"RequestChemInvFA2")
		If ColDefStr = "" OR IsNull(ColDefStr) then 
			' Default column definition
			ColDefstr= Application("RequestReportColDef2")
			rc = WriteUserProperty(Session("UserNameCheminv"), "RequestChemInvFA2", ColDefstr)
		End if
		fieldArray = GetFieldArray(colDefstr, Application("RequestFieldMap"))
		Session("RequestReportFieldArray2") = fieldArray
	Else
		fieldArray = Session("RequestReportFieldArray2")
	End if

elseif RequestStatusID = "7" then
	caption = "The following requests are cancelled:"
	disable6 = "disabled"
	bShowBatch = false

	' Set up fieldArray containing the column definition for the on screen report
	If NOT IsArray(Session("RequestReportFieldArray7")) then
		ColDefStr = GetUserProperty(Session("UserNameCheminv"),"RequestChemInvFA7")
		If ColDefStr = "" OR IsNull(ColDefStr) then 
			' Default column definition
			ColDefstr= Application("RequestReportColDef7")
			rc = WriteUserProperty(Session("UserNameCheminv"), "RequestChemInvFA7", ColDefstr)
		End if
		fieldArray = GetFieldArray(colDefstr, Application("RequestFieldMap"))
		Session("RequestReportFieldArray7") = fieldArray
	Else
		fieldArray = Session("RequestReportFieldArray7")
	End if

elseif RequestStatusID = "8" then
	caption = "The following requests are pending:"
	disable7 = "disabled"
	bShowBatch = true

	' Set up fieldArray containing the column definition for the on screen report
	If NOT IsArray(Session("RequestReportFieldArray8")) then
		ColDefStr = GetUserProperty(Session("UserNameCheminv"),"RequestChemInvFA8")
		If ColDefStr = "" OR IsNull(ColDefStr) then 
			' Default column definition
			ColDefstr= Application("RequestReportColDef8")
			rc = WriteUserProperty(Session("UserNameCheminv"), "RequestChemInvFA8", ColDefstr)
		End if
		fieldArray = GetFieldArray(colDefstr, Application("RequestFieldMap"))
		Session("RequestReportFieldArray8") = fieldArray
	Else
		fieldArray = Session("RequestReportFieldArray8")
	End if

elseif RequestStatusID = "3" then
	caption = "The following requests have been approved:"
	disable2 = "disabled"
	bShowBatch = true
	' Set up fieldArray containing the column definition for the on screen report
	If NOT IsArray(Session("RequestReportFieldArray3")) then
		ColDefStr = GetUserProperty(Session("UserNameCheminv"),"RequestChemInvFA3")
		If ColDefStr = "" OR IsNull(ColDefStr) then 
			' Default column definition
			ColDefstr= Application("RequestReportColDef3")
			rc = WriteUserProperty(Session("UserNameCheminv"), "RequestChemInvFA3", ColDefstr)
		End if
		fieldArray = GetFieldArray(colDefstr, Application("RequestFieldMap"))
		Session("RequestReportFieldArray3") = fieldArray
	Else
		fieldArray = Session("RequestReportFieldArray3")
	End if

elseif RequestStatusID = "4" then
	caption = "The following requests have been declined:"
	disable3 = "disabled"
	' Set up fieldArray containing the column definition for the on screen report
	If NOT IsArray(Session("RequestReportFieldArray4")) then
		ColDefStr = GetUserProperty(Session("UserNameCheminv"),"RequestChemInvFA4")
		If ColDefStr = "" OR IsNull(ColDefStr) then 
			' Default column definition
			ColDefstr= Application("RequestReportColDef4")
			rc = WriteUserProperty(Session("UserNameCheminv"), "RequestChemInvFA4", ColDefstr)
		End if
		fieldArray = GetFieldArray(colDefstr, Application("RequestFieldMap"))
		Session("RequestReportFieldArray4") = fieldArray
	Else
		fieldArray = Session("RequestReportFieldArray4")
	End if

elseif RequestStatusID = "5" then
	caption = "The following requests have been filled:"
	disable4 = "disabled"
	' Set up fieldArray containing the column definition for the on screen report
	If NOT IsArray(Session("RequestReportFieldArray5")) then
		ColDefStr = GetUserProperty(Session("UserNameCheminv"),"RequestChemInvFA5")
		If ColDefStr = "" OR IsNull(ColDefStr) then 
			' Default column definition
			ColDefstr= Application("RequestReportColDef5")
			rc = WriteUserProperty(Session("UserNameCheminv"), "RequestChemInvFA5", ColDefstr)
		End if
		fieldArray = GetFieldArray(colDefstr, Application("RequestFieldMap"))
		Session("RequestReportFieldArray5") = fieldArray
	Else
		fieldArray = Session("RequestReportFieldArray5")
	End if

elseif RequestStatusID = "6" then
	caption = "The following requests are closed:"
	disable5 = "disabled"
	' Set up fieldArray containing the column definition for the on screen report
	If NOT IsArray(Session("RequestReportFieldArray6")) then
		ColDefStr = GetUserProperty(Session("UserNameCheminv"),"RequestChemInvFA6")
		If ColDefStr = "" OR IsNull(ColDefStr) then 
			' Default column definition
			ColDefstr= Application("RequestReportColDef6")
			rc = WriteUserProperty(Session("UserNameCheminv"), "RequestChemInvFA6", ColDefstr)
		End if
		fieldArray = GetFieldArray(colDefstr, Application("RequestFieldMap"))
		Session("RequestReportFieldArray6") = fieldArray
	Else
		fieldArray = Session("RequestReportFieldArray6")
	End if

elseif RequestStatusID = "0" then
	caption = "All Requests:"
	disable = "disabled"
	' Set up fieldArray containing the column definition for the on screen report
	If NOT IsArray(Session("RequestReportFieldArray0")) then
		ColDefStr = GetUserProperty(Session("UserNameCheminv"),"RequestChemInvFA0")
		If ColDefStr = "" OR IsNull(ColDefStr) then 
			' Default column definition
			ColDefstr= Application("RequestReportColDef0")
			rc = WriteUserProperty(Session("UserNameCheminv"), "RequestChemInvFA0", ColDefstr)
		End if
		fieldArray = GetFieldArray(colDefstr, Application("RequestFieldMap"))
		Session("RequestReportFieldArray0") = fieldArray
	Else
		fieldArray = Session("RequestReportFieldArray0")
	End if

end if


%>

<div style="margin:10px;">
<a class="MenuLink" href="#" title="Click to choose report columns" onclick="OpenDialog('/cheminv/cheminv/columnPicker2.asp?ArrayID=<%=RequestStatusID%>&amp;showRequests=true', 'CCDiag', 4); return false">Column Chooser</a>
</div>

<center>
<p><span class="GUIFeedback"><%=Caption%></span></p>
<form name="form1" action method="POST" OnSubmit="ChooseAction();return false;">


<table border="1" cellpadding="4">
<%
bRequestsFound = true
If (RS1.EOF AND RS1.BOF) then
	bRequestsFound = false
	Response.Write ("<tr><td align=""center"" colspan=""20""><span class=""GUIFeedback"">No requests found</Span></td></tr>")

Else


if (RequestStatusID = "0" and Request("submit1") = "Filter") or RequestStatusID <> "" then
	
%>
	<tr>
		<th>&nbsp;</th>
		<%if RequestStatusID = "2" then%>
			<th>Accept</th>
			<th>Decline</th>
		<%elseif RequestStatusID = "5" then%>
			<th>Close</th>
		<% elseif RequestStatusID = "6" then %>
			<th>Docs</th>
		<%end if%>

		<th>Request ID</th>
		<th>BatchID</th>
		<th>Amount Requested</th>
		<%
		for i=0 to ubound(fieldArray)
			'Response.Write(fieldArray(i,0) & ":" & fieldArray(i,1) & ":" & fieldArray(i,2) & ":" & fieldArray(i,3) & "<br>")
			if fieldArray(i,0) = BatchAmount and bShowBatch then
				Response.Write("<th nowrap width=""" & fieldArray(i,2) & """>" & fieldArray(i,1) & "</th>")
			else
				Response.Write("<th nowrap width=""" & fieldArray(i,2) & """>" & fieldArray(i,1) & "</th>")
			end if
		next
		
		if RequestStatusID = "0" then
			response.write("<th>Status</th>")
		end if 
		%>
	</tr>

<%
		NumRequests = 0
		While (Not RS1.EOF)
			BatchID = RS1("batch_id_fk")
			RequestID = RS1("Request_ID")
			RequestUserID = RS1("RUserID")
			BatchStatus = RS1("BatchStatus")
			AmountRequested = RS1("Qty_Required")
			AmountDelivered = RS1("Qty_Delivered")
			MinStockThreshold = RS1("minimum_stock_threshold")
			if isBlank(RS1("AmountReserved")) then AmountReserved = 0 else AmountReserved = RS1("AmountReserved") end if
			if isBlank(RS1("AmountRemaining")) then AmountRemaining = 0 else AmountRemaining = RS1("AmountRemaining") end if
			AmountAvailable = cDbl(AmountRemaining)-cDbl(AmountReserved)
			DateRequested = RS1("timestamp")
			DateRequired = RS1("Date_Required")
			DateDelivered = RS1("Date_Delivered")
			DeliveryLocation = RS1("Location_Name")
			BatchField1 = RS1("batchvalue1")
			BatchField2 = RS1("batchvalue2")
			BatchField3 = RS1("batchvalue3")
			DeclineReason = RS1("decline_reason")
			RequestedBy = RS1("Creator")
			AssignedUser = RS1("assigned_user_id_fk")
			if AssignedUser <> "" then
				ActiveRowCSSClass = "ActiveRow"
			else
				ActiveRowCSSClass = "InActiveRow"
			end if
			NumRequests = NumRequests + 1
			UOMAbbrv = RS1("uomabbrv")
			StatusName = RS1("request_status_name")
			RequestReceiptDocID = RS1("RequestReceiptDocID")
			RequestWorksheetDocID = RS1("RequestWorksheetDocID")			
			for each key in reg_fields_dict
				if key <> "BASE64_CDX" then
				execute("Request" & key & " = RS1(""" & key & """)")
				end if
			next
			for each key in custom_createrequest_fields_dict
				execute("Request" & key & " = RS1(""" & key & """)")
			next
			for each key in custom_fulfillrequest_fields_dict
				execute("Request" & key & " = RS1(""" & key & """)")
			next


			if not IsEmpty(Application("DEFAULT_SAMPLE_REQUEST_CONC")) then
				arrUOM = split(Application("DEFAULT_SAMPLE_REQUEST_CONC"),"=")
				RequestUOMAbbrv = arrUOM(1)
			else
				RequestUOMAbbrv = RS1("uomabbrv")
			end if
			
			if BatchStatus = "23" then 
				AllowBatchRequest = true
			else
				AllowBatchRequest = false
			end if
			
			'-- Create edit order links
			OrderLinks = ""
			if not isNull(OrderList) and not isEmpty(OrderList) then
				arrOrders = split(OrderList, ",")
				for i=0 to ubound(arrOrders)
					arrOrders2 = split(arrOrdeRS1(i),":")
					if arrOrders2(1) = "1" then
						OrderLinks = OrderLinks & "<a Class=""MenuLink"" HREF=""#"" onclick=""OpenDialog('/Cheminv/GUI/Order_frset.asp?Clear=1&Action=edit&OrderID=" & arrOrders2(0) & "', 'Diag', 2); return false"">" & arrOrders2(0) & "</a><BR>" 
					else
						OrderLinks = OrderLinks & "<a Class=""MenuLink"" HREF=""#"" onclick=""OpenDialog('/Cheminv/GUI/Order_list.asp?Clear=1&Action=edit&OrderID=" & arrOrders2(0) & "', 'Diag', 2); return false"">" & arrOrders2(0) & "</a><BR>" 
					end if
				next
			end if
			if OrderLinks = "" then OrderLinks = "&nbsp;"
									
			if bShowBatch then
				'SQL = "SELECT container_id, parent_container_id_fk, " & Application("ORASCHEMANAME") & ".GUIUTILS.GETBATCHAMOUNTSTRING(parent_container_id_fk) AS Batch_Amount_String FROM inv_containers where container_id = ?"
				if UOMAbbrv = "ml" then
					SQL = "select sum(qty_remaining*concentration) as BatchAmount from inv_containers where batch_id_fk=?"
				else
					SQL = "select sum(qty_remaining) as BatchAmount from inv_containers where batch_id_fk=?"
				end if
				
				Set Cmd = GetCommand(Conn, SQL, adCmdText)
				Cmd.Parameters.Append Cmd.CreateParameter("BatchID", 5, 1, 0, BatchID)
				Set rsBatch = Cmd.Execute
				if IsNull(BatchString) then 
					BatchAmount = "&nbsp;"
				else
					BatchAmount = rsBatch("BatchAmount")
				end if
			end if

%>
			<tr class="<%=ActiveRowCSSClass%>">
<%
			if RequestStatusID = "2" then
				link = "<a Class=""MenuLink"" Href=""#"" onclick=""OpenDialog('/Cheminv/GUI/RequestBatch.asp?action=edit&amp;" & editdata & "BatchID=" & BatchID & "&RequestID=" & RequestID & "', 'Diag', 1); return false"">Review</a>"
				link = link & "&nbsp;"
			elseif RequestStatusID = "3" then
				if lCase(Application("ShowCreateSample")) = "true" then
					link = "&nbsp;<a Class=""MenuLink"" Href=""#"" onclick=""OpenDialog('/Cheminv/GUI/CreateSamplesFromBatch.asp?Action=new&batchid=" & BatchID & "&RequestID=" & RequestID & "', 'Diag', 1); return false"" title=""Create Samples""><img src=""../graphics/sample_create.gif"" border=""0"" WIDTH=""16"" HEIGHT=""15""></a>&nbsp;"
					link = link & "&nbsp;<a Class=""MenuLink"" Href=""#"" onclick=""OpenDialog('/Cheminv/GUI/AddBatchToRequest.asp?batchid=" & BatchID & "&MinStockThreshold=" & MinStockThreshold & "&AmountRemaining=" & AmountRemaining & "&RequestID=" & RequestID & "&QtyRequired=" & AmountRequested & "&AllowBatchRequest=" & AllowBatchRequest & "&AmountReserved=" & AmountReserved & "', 'Diag', 2); return false"" title=""Add Samples""><img src=""../graphics/sample_add.gif"" border=""0"" WIDTH=""16"" HEIGHT=""15""></a>&nbsp;"
					if Application("ShowRequestSample") and Session("INV_APPROVE_CONTAINER" & dbkey) then
						link = link & "&nbsp;<a Class=""MenuLink"" Href=""#"" onclick=""OpenDialog('/Cheminv/GUI/RequestBatch.asp?BatchID=" & BatchID & "&RequestID=" & RequestID & "&action=cancel', 'Diag', 1); return false""><img src=""/cheminv/graphics/ico_cancel.gif"" border=""0""></a>"
					end if
				else
					link = "&nbsp;<a Class=""MenuLink"" Href=""#"" onclick=""OpenDialog('/Cheminv/GUI/AddBatchToRequest.asp?batchid=" & BatchID & "&MinStockThreshold=" & MinStockThreshold & "&AmountRemaining=" & AmountRemaining & "&RequestID=" & RequestID & "&QtyRequired=" & AmountRequested & "&AllowBatchRequest=" & AllowBatchRequest & "&AmountReserved=" & AmountReserved & "', 'Diag', 2); return false"" title=""Add Samples""><img src=""../graphics/sample_add.gif"" border=""0"" WIDTH=""16"" HEIGHT=""15""></a>&nbsp;"
					if Application("ShowRequestSample") and Session("INV_APPROVE_CONTAINER" & dbkey) then
						link = link & "&nbsp;<a Class=""MenuLink"" Href=""#"" onclick=""OpenDialog('/Cheminv/GUI/RequestBatch.asp?BatchID=" & BatchID & "&RequestID=" & RequestID & "&action=cancel', 'Diag', 1); return false""><img src=""/cheminv/graphics/ico_cancel.gif"" border=""0""></a>"
					end if
				end if
				link = link & "&nbsp;<a Class=""MenuLink"" Href=""#"" onclick=""OpenDialog('/Cheminv/GUI/AssignUserToRequest.asp?RequestID=" & RequestID & "', 'Diag', 2); return false"" title=""Assign User""><img src=""/cheminv/graphics/icon_singleuser.gif"" border=""0""></a>&nbsp;"
			elseif RequestStatusID = "4" then
				link = "<a Class=""MenuLink"" Href=""#"" onclick=""OpenDialog('/Cheminv/GUI/DeclineRequestSample.asp?DeclinedRequestIDList=" & RequestID & "&action=edit', 'Diag', 1); return false"">Reason</a>"
			elseif RequestStatusID = "5" then
				if ShipToName <> "" then
					ShipToName = server.URLEncode(ShipToName)
				else
					ShipToName = ""
				end if
				link = "<a Class=""MenuLink"" Href=""#"" onclick=""OpenDialog('/Cheminv/GUI/Order_frset.asp?Clear=1&Action=create&RequestID=" & RequestID & "&DeliveryLocationID=" & DeliveryLocationID & "&ShipToName=" & ShipToName & "', 'Diag', 2); return false"">Create Order</a>"
			elseif RequestStatusID = "6" then
				'link = "<a Class=""MenuLink"" Href=""#"" onclick=""OpenDialog('/Cheminv/GUI/RequestSample.asp?action=edit&amp;" & editdata & "', 'Diag', 1); return false"">Review</a>"
				link = ""
			elseif RequestStatusID = "8" then
				if Application("ShowRequestSample") and Session("INV_APPROVE_CONTAINER" & dbkey) then
					link = "&nbsp;<a Class=""MenuLink"" Href=""#"" onclick=""OpenDialog('/Cheminv/GUI/RequestBatch.asp?BatchID=" & BatchID & "&RequestID=" & RequestID & "&action=cancel', 'Diag', 1); return false""><img src=""/cheminv/graphics/ico_cancel.gif"" border=""0""></a>"
				end if
			end if
%>
				<td align="center"><%=link%></td>
			<%if RequestStatusID = "2" then%>
				<td align="center">
					<input type="checkbox" name="ApprovedRequestIDList" value="<%=RequestID%>" onclick="UncheckDecline(this);" )>
				</td>	
				<td align="center">
					<input type="checkbox" name="DeclinedRequestIDList" value="<%=RequestID%>" onclick="UncheckApprove(this);" )>
				</td>	
			<%elseif RequestStatusID = "5" then%>
				<td align="center">
					<input type="checkbox" name="ClosedRequestIDList" value="<%=RequestID%>" )>
				</td>	
			<%elseif RequestStatusID = "6" then%>
				<td align="center">
					<%
						if RequestReceiptDocID <> "" then
							Response.Write("&nbsp;&nbsp;<a class=""MenuLink"" Href=""#"" onclick=""OpenDialog('/cheminv/gui/viewdoc.asp?docid=" & RequestReceiptDocID & "', 'Diag1', 2); return false;"" title=""Print Receipt"">")
							Response.Write("Receipt</a>&nbsp;")
						end if
						if RequestWorksheetDocID <> "" then
							Response.Write("|&nbsp;<a class=""MenuLink"" Href=""#"" onclick=""OpenDialog('/cheminv/gui/viewdoc.asp?docid=" & RequestWorksheetDocID & "', 'Diag1', 2); return false;"" title=""Print Worksheet"">")
							Response.Write("Worksheet</a>")
						end if
					%>&nbsp;
				</td>	
			<%end if%>	
			
				<td align="center"><%=RequestID%></td>
				<td align="center"><span title="<%=Comments%>"><%=BatchID%></span></td>
				<td align="center"><%Response.write(TruncateInSpan(AmountRequested & " " & RequestUOMAbbrv, 11, ""))%></td>
				<%
				for i=0 to ubound(fieldArray)
					'Response.Write(fieldArray(i,0) & ":" & fieldArray(i,1) & ":" & fieldArray(i,2) & ":" & fieldArray(i,3) & "<br>")
					if fieldArray(i,0) = "BatchAmount" and bShowBatch then
						if Application("DEFAULT_SAMPLE_REQUEST_CONC") <> "" and UOMAbbrv = "ml" then
							tmpConc = split(Application("DEFAULT_SAMPLE_REQUEST_CONC"),"=")
							UOMAbbrv = tmpConc(1)
						end if
						Response.Write("<td align=""right"">")
						Response.Write("<a class=""MenuLink"" href=""#"" onclick=""OpenDialog('/cheminv/gui/ViewBatchInfo.asp?BatchID=" & BatchID & "&amp;ShowLinks=0','Diag',1);return false"">" & BatchAmount & " " & UOMAbbrv & "</a>")
						Response.Write("</td>")
					else
						Response.Write("<td align=""center"" width=""" & fieldArray(i,2) & """ nowrap>")
						execute("response.write(TruncateInSpan(" & fieldArray(i,0) & ", fieldArray(i,2), """"))" )
						Response.Write("</td>")
					end if
				next
				
				%>
				<% if RequestStatusID = "0" then %>
				<td align="center">
					<%=TruncateInSpan(StatusName, 11, "")%>
				</td>
				<% end if %>
			</tr>
			<%RS1.MoveNext
		Wend%>
		<input type="hidden" name="NumRequests" value="<%=NumRequests%>">		
	</table>
	<%if RequestStatusID = 3 then %>
	<table width="100%" cellpadding="2">
		<% if lCase(Application("ShowCreateSample")) = "true" then %>
		<tr><td colspan="10"><img src="../graphics/sample_create.gif" border="0" WIDTH="16" HEIGHT="15">&nbsp;Create Samples</td></tr>
		<% end if %>
		<%if Application("ShowRequestSample") and Session("INV_APPROVE_CONTAINER" & dbkey) then%>
		<tr><td colspan="10"><img src="../graphics/sample_add.gif" border="0" WIDTH="16" HEIGHT="15">&nbsp;Add Samples</td></tr>
		<tr><td colspan="10"><img src="/cheminv/graphics/ico_cancel.gif" border="0">&nbsp;Cancel Request</td></tr>
		<tr><td colspan="10"><img src="/cheminv/graphics/icon_singleuser.gif" border="0">&nbsp;Assign User</td></tr>
		<%end if %>
	</table>
	<%elseif RequestStatusID = 8 then %>
		<%if Application("ShowRequestSample") and Session("INV_APPROVE_CONTAINER" & dbkey) then%>
		<table width="100%" cellpadding="2">
			<tr><td colspan="10"><img src="/cheminv/graphics/ico_cancel.gif" border="0">&nbsp;Cancel Request</td></tr>
		</table>
		<%end if%>
	<% end if %>
<%	
	End if
	RS1.Close
	Conn.Close
	Set RS1 = nothing
	Set Cmd = nothing
	Set Conn = nothing
End if

end if

%>

<table>
	<tr>
		<td colspan="8" align="right"> 
			<%
			'only show for new and filled
			if RequestStatusID = "2" or RequestStatusID = "5" then
				if bRequestsFound then
			%>
			<input type="image" src="/cheminv/graphics/sq_btn/ok_dialog_btn.gif" border="0">
			<%
				end if
			end if
			%>
		</td>
	</tr>	
</table>
</form>
</center>
</body>
</html>



