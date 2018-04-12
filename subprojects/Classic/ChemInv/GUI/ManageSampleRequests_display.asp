<%@ Language=VBScript %>
<!-- #include virtual="/cs_security/variables.asp" -->
<!--#INCLUDE VIRTUAL = "/cheminv/cheminv/fieldArrayUtils.asp" -->
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/ado.inc"-->
<!--#INCLUDE VIRTUAL = "/cheminv/api/apiUtils.asp"-->
<!--#INCLUDE VIRTUAL = "/cheminv/gui/guiUtils.asp"-->
<%
dbkey = "ChemInv"
RequestStatusID = Request("RequestStatusID")
%>
<html>
<head>
<title><%=Application("appTitle")%> -- Manage Sample Requests</title>
<script type="text/javascript" language="javascript" src="/cheminv/Choosecss.js"></script>
<script type="text/javascript" language="javascript" src="/cheminv/utils.js"></script>
<script type="text/javascript" language="javascript">

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
</head>
<body>
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
'-- CSBR ID:123488
'-- Change Done by : Manoj Unnikrishnan
'-- Purpose: Updated the Comments variable according to the field added to the column picker
'-- Date: 13/04/2010
Request_Comments = Request("RequestComments")
'-- End of Change #123488#


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

'-- Call GetBatchRequests but batch requests not dependant no Containers as before
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
		Response.Write p.name & " = '" & p.value & "'<BR>"
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
	bShowBatch = true
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
<%if RequestStatusID <> "0" then %>
<a class="MenuLink" href="#" title="Click to choose report columns" onclick="OpenDialog('/cheminv/cheminv/columnPicker2.asp?ArrayID=<%=RequestStatusID%>&amp;showRequests=true', 'CCDiag', 4); return false">Column Chooser</a>
<%end if %>
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
       <th>Proof of Approval </th>
	</tr>

<%
		NumRequests = 0
		While (Not RS1.EOF)

			'-- Commented out batch properties no relavent until fulfillment of batch request
			ContainerID = RS1("container_id_fk")
			'ContainerBarcode = RS("Barcode")
			'ContainerName = RS("Container_Name")
			ShipToName = RS1("ship_to_name")
			BatchID = RS1("batch_id_fk")
			RequestID = RS1("Request_ID")
			RequestUserID = RS1("RUserID")
			BatchStatus = RS1("BatchStatus")
			AmountRequested = RS1("Qty_Required")
			AmountDelivered = RS1("Qty_Delivered")
        '-- CSBR ID:123488
        '-- Change Done by : Manoj Unnikrishnan
        '-- Purpose: Added the Comments variable according to the field added to the column picker
        '-- Date: 13/04/2010
	        Request_Comments = RS1("request_comments")	
        '-- End of Change #123488#
			MinStockThreshold = RS1("minimum_stock_threshold")
			if isBlank(RS1("AmountReserved")) then AmountReserved = 0 else AmountReserved = RS1("AmountReserved") end if
			if isBlank(RS1("AmountRemaining")) then AmountRemaining = 0 else AmountRemaining = RS1("AmountRemaining") end if
			AmountAvailable = cDbl(AmountRemaining)-cDbl(AmountReserved)
			DateRequested = RS1("timestamp")
			DateRequired = RS1("Date_Required")
			DateDelivered = RS1("Date_Delivered")
			DeliveryLocation = RS1("Location_Name")
			DeliveryLocationID = RS1("Delivery_location_id_fk")
			BatchField1 = RS1("batchvalue1")
			BatchField2 = RS1("batchvalue2")
			BatchField3 = RS1("batchvalue3")
			DeclineReason = RS1("decline_reason")
			RequestedBy = RS1("Creator")
			RequestedFor = RS1("user_id_fk")
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
			RequestUOMAbbrv = RS1("RequiredUOMabbrv") & ""

			if Application("isRegBatch")=1 then 
				for each key in reg_fields_dict
					if key <> "BASE64_CDX" then
				 	 execute("Request" & key & " = RS1(""" & key & """)")
					end if
				next
			end if 	
			for each key in custom_createrequest_fields_dict
				execute("Request" & key & " = RS1(""" & key & """)")
			next
			for each key in custom_fulfillrequest_fields_dict
				execute("Request" & key & " = RS1(""" & key & """)")
			next


			if not IsEmpty(Application("DEFAULT_SAMPLE_REQUEST_CONC")) and RequestUOMAbbrv="" then
				arrUOM = split(Application("DEFAULT_SAMPLE_REQUEST_CONC"),"=")
				RequestUOMAbbrv = arrUOM(1)
			'else
			'	RequestUOMAbbrv = RS1("uomabbrv")
			end if

			'-- 1 is "Requestable" status in INV_BATCH_STATUS
			if BatchStatus = "1" then
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

			'if bShowBatch then

				'-- Commented out because there is not one container in a batch
				'SQL = "SELECT container_id, parent_container_id_fk, " & Application("ORASCHEMANAME") & ".GUIUTILS.GETBATCHAMOUNTSTRING(parent_container_id_fk) AS Batch_Amount_String FROM inv_containers where container_id = ?"
				'Cmd.Parameters.Append Cmd.CreateParameter("ContainerID", 5, 1, 0, ContainerID)
				'Set rsBatch = Cmd.Execute
				'BatchString = rsBatch("Batch_Amount_String")
				'ParentContainerID = rsBatch("parent_container_id_fk")

				'-- Custom for Abbott, calculates dry weight from qty and concentration
				'if UOMAbbrv = "ml" then
				'	SQL = "select sum(qty_remaining*concentration) as BatchAmount from inv_containers where batch_id_fk=?"
				'else
				'	SQL = "select sum(qty_remaining) as BatchAmount from inv_containers where batch_id_fk=?"
				'end if

				'Set Cmd = GetCommand(Conn, SQL, adCmdText)
				'Cmd.Parameters.Append Cmd.CreateParameter("BatchID", 5, 1, 0, BatchID)
				'Set rsBatch = Cmd.Execute
				'if IsNull(BatchString) then
				'	BatchAmount = "&nbsp;"
				'else
				'	BatchAmount = rsBatch("BatchAmount")
				'end if
				Call GetInvCommand(Application("CHEMINV_USERNAME") & ".GUIUTILS.GETBATCHUOMAMOUNTSTRING", adCmdStoredProc)
                Cmd.Parameters.Append Cmd.CreateParameter("RETURN_VALUE", adVarChar, adParamReturnValue, 200, NULL)
                Cmd.Parameters.Append Cmd.CreateParameter("pContainerID", adNumeric, 1, 0, NULL)
                Cmd.Parameters.Append Cmd.CreateParameter("pBatchID", adNumeric, 1, 1, clng(BatchID))
                if bDebugPrint then
                 For each p in Cmd.Parameters
                  Response.Write p.name & " = " & p.value & "<BR>"
                 Next 
                Else
                 Call ExecuteCmd(Application("CHEMINV_USERNAME") & ".GUIUTILS.GETBATCHUOMAMOUNTSTRING")
                End if
                BatchAmount = Cmd.Parameters("RETURN_VALUE")
                if  not isnull(BatchAmount) then BatchAmount = Replace(BatchAmount,":"," ")
		'	end if
%>
			<tr class="<%=ActiveRowCSSClass%>">
<%
			link=""
			if RequestStatusID = "2" then
				'link = "<a Class=""MenuLink"" Href=""#"" onclick=""OpenDialog('/Cheminv/GUI/RequestBatch.asp?action=edit&amp;" & editdata & "BatchID=" & BatchID & "&RequestID=" & RequestID & "', 'Diag', 1); return false"">Review</a>"
				link = "<a Class=""MenuLink"" Href=""#"" onclick=""OpenDialog('/Cheminv/GUI/RequestSample.asp?action=edit&amp;" & editdata & "BatchID=" & BatchID &  "&RequestID=" & RequestID & "&UOMAbv=" & RequestUOMAbbrv & "&ContainerID=" & ContainerID & " ', 'Diag', 1); return false"">Review</a>"
				link = link & "&nbsp;"
			elseif RequestStatusID = "3" then
				link = ""
'-- CSBR-138430-SMathur-Added permission on the Links so that only user with INV_SAMPLE_DISPENS can see the icons
				if Session("INV_SAMPLE_DISPENSE" & dbkey) then
				    if lCase(Application("ShowCreateSample")) = "true" then
					    link = link & "&nbsp;<a Class=""MenuLink"" Href=""#"" onclick=""OpenDialog('/Cheminv/GUI/CreateSamplesFromBatch.asp?Action=new&batchid=" & BatchID & "&RequestID=" & RequestID & "', 'Diag', 1); return false"" title=""Create Samples""><img src=""../graphics/sample_create.gif"" border=""0"" WIDTH=""16"" HEIGHT=""15""></a>&nbsp;"
                    end if			
                    if lcase(Application("ShowAddSample")) = "true" then
                        link = link & "&nbsp;<a Class=""MenuLink"" Href=""#"" onclick=""OpenDialog('/Cheminv/GUI/AddBatchToRequest.asp?batchid=" & BatchID & "&MinStockThreshold=" & MinStockThreshold & "&AmountRemaining=" & AmountRemaining & "&RequestID=" & RequestID & "&QtyRequired=" & AmountRequested & "&AllowBatchRequest=" & AllowBatchRequest & "&AmountReserved=" & AmountReserved & "', 'Diag', 2); return false"" title=""Add Samples""><img src=""../graphics/sample_add.gif"" border=""0"" WIDTH=""16"" HEIGHT=""15""></a>&nbsp;"                
                    end if		
				    if Application("ShowRequestSample") and Session("INV_APPROVE_CONTAINER" & dbkey) then
					    'link = link & "&nbsp;<a Class=""MenuLink"" Href=""#"" onclick=""OpenDialog('/Cheminv/GUI/RequestBatch.asp?BatchID=" & BatchID & "&RequestID=" & RequestID & "&action=cancel', 'Diag', 1); return false""><img src=""/cheminv/graphics/ico_cancel.gif"" border=""0""></a>"
					    link = link & "&nbsp;<a Class=""MenuLink"" Href=""#"" onclick=""OpenDialog('/Cheminv/GUI/RequestSample.asp?BatchID=" & BatchID & "&RequestID=" & RequestID & "&action=cancel', 'Diag', 1); return false""><img src=""/cheminv/graphics/ico_cancel.gif"" border=""0""></a>"
				    end if
				   
				    link = link & "&nbsp;<a Class=""MenuLink"" Href=""#"" onclick=""OpenDialog('/Cheminv/GUI/AssignUserToRequest.asp?RequestID=" & RequestID & "', 'Diag', 2); return false"" title=""Assign User""><img src=""/cheminv/graphics/icon_singleuser.gif"" border=""0""></a>&nbsp;"
				 end if
				'CSBR# - 125159
				'Changed by - Soorya Anwar
				'Date of change - 04/Jun/2010
                'Purpose - To add a reporting icon,on clicking which, 
				'it opens the INV Reporting page with query string values of Request_ID and Batch_ID 				
				link = link & "&nbsp;<a Class=""MenuLink"" Href=""#"" onclick=""OpenDialog('/Cheminv/Gui/CreateReport_frset.asp?isCustomReport=1&ReportTypeID=11&BatchID=" & BatchID & "&RequestID=" & RequestID & "', 'Diag', 1); return false"" title=""Generate Samples Report""><img src=""../graphics/ReportIcon.gif"" border=""0"" WIDTH=""16"" HEIGHT=""15""></a>&nbsp;"
				'End of Change for CSBR 125159		
			elseif RequestStatusID = "4" then
				link = "<a Class=""MenuLink"" Href=""#"" onclick=""OpenDialog('/Cheminv/GUI/DeclineRequestSample.asp?DeclinedRequestIDList=" & RequestID & "&action=edit', 'Diag', 1); return false"">Reason</a>"
			elseif RequestStatusID = "5" then
				if ShipToName <> "" then
					ShipToName = server.URLEncode(ShipToName)
				else
					ShipToName = ""
				end if
				link = "<a Class=""MenuLink"" Href=""#"" onclick=""OpenDialog('/Cheminv/GUI/Order_frset.asp?Clear=1&Action=create&CreateType=AssignRequestToOrder&RequestID=" & RequestID & "&DeliveryLocationID=" & DeliveryLocationID & "&ShipToName=" & ShipToName & "', 'Diag', 2); return false"">Create Order</a>"
			elseif RequestStatusID = "6" then
				'link = "<a Class=""MenuLink"" Href=""#"" onclick=""OpenDialog('/Cheminv/GUI/RequestSample.asp?action=edit&amp;" & editdata & "', 'Diag', 1); return false"">Review</a>"
				link = ""
			elseif RequestStatusID = "8" then
				if Application("ShowRequestSample") and Session("INV_APPROVE_CONTAINER" & dbkey) then
					'link = "&nbsp;<a Class=""MenuLink"" Href=""#"" onclick=""OpenDialog('/Cheminv/GUI/RequestBatch.asp?BatchID=" & BatchID & "&RequestID=" & RequestID & "&action=cancel', 'Diag', 1); return false""><img src=""/cheminv/graphics/ico_cancel.gif"" border=""0""></a>"
					link = "&nbsp;<a Class=""MenuLink"" Href=""#"" onclick=""OpenDialog('/Cheminv/GUI/RequestSample.asp?BatchID=" & BatchID & "&RequestID=" & RequestID & "&action=cancel', 'Diag', 1); return false""><img src=""/cheminv/graphics/ico_cancel.gif"" border=""0""></a>"
				end if
			end if
%>
				<td align="center"><%=link%></td>

			<%'-- CSBR-138430-SMathur-Added permission on the Approve and decline checkbx
			if RequestStatusID = "2" then
			if Session("INV_SAMPLE_APPROVE" & dbkey) then
			    disabledVal=""
			else 
			    disabledVal="disabled"
			end if	
			%>
				<td align="center">
					<input type="checkbox" name="ApprovedRequestIDList" value="<%=RequestID%>" <%=disabledVal %> onclick="UncheckDecline(this);" )>
				</td>
				<td align="center">
					<input type="checkbox" name="DeclinedRequestIDList" value="<%=RequestID%>" <%=disabledVal %> onclick="UncheckApprove(this);" )>
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
				<td align="center"><span title="<%=Comments%>"><%=BatchID%></span>&nbsp;</td>
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
						Response.Write("<a class=""MenuLink"" href=""#"" onclick=""OpenDialog('/cheminv/gui/ViewBatchInfo.asp?BatchID=" & BatchID & "&amp;ShowLinks=0','Diag',1);return false"">" & BatchAmount & " &nbsp;</a>")
						Response.Write("</td>")
					else
						if RequestStatusID = "4" and fieldArray(i,0) = "DeclineReason" then													
							Response.Write("<td align=""center"" width=""" & fieldArray(i,2) & """ nowrap>&nbsp</td>")															
						else												
							Response.Write("<td align=""center"" width=""" & fieldArray(i,2) & """ nowrap>")
							execute("response.write(TruncateInSpan(" & fieldArray(i,0) & ", fieldArray(i,2), """"))" )
							Response.Write("</td>")
						end if
					end if
				next

				%>
				<% if RequestStatusID = "0" then %>
				<td align="center">
					<%=TruncateInSpan(StatusName, 11, "")%>
				</td>
				<% end if %>
                <% 'SMathur CBOE-234: Adding the attchment information 
                   ProofApprovalFilename = RS1("proof_approval_filename") 
                   If isNull(ProofApprovalFilename) = FALSE then
                        fname = Replace(ProofApprovalFilename, " ", "%20")
	                    Dim ret
	                    FileFullPath = Session("sessionTempFolder") & "\" & ProofApprovalFilename
                        ret = Application("blobHandler").SaveBlobToFile(Session("UserName" & "cheminv") & "/" & Session("UserID" & "cheminv"), "SELECT * FROM Inv_Requests WHERE REQUEST_ID = " & RequestID, "PROOF_APPROVAL", CStr(FileFullPath))

                        if Instr(ret, "ERROR:") > 0 then
	                        Response.Write ret 'pass on the error
	                    else
	                        fp = Replace(FileFullPath, " ", "%20")
	                        fp = Replace(FileFullPath, "\", "\\")
	                        fname = Replace(ProofApprovalFilename, " ", "%20")
                        end if%>
			            <td>
                        <div id="download_link" name="download_link">
	                    <a href="#" onClick="window.location.href('download.asp?filePath=<%=fp%>&fileName=<%=fname%>')" id="proof_link")"><%=ProofApprovalFilename%></a>
	                    </div></td>
                   <%else %>
                        <td>&nbsp;</td>
                   <%end if  %>
			</tr>
			<%RS1.MoveNext
		Wend%>
		<input type="hidden" name="NumRequests" value="<%=NumRequests%>">
	</table>
	<%if RequestStatusID = 3 then %>
	<table width="100%" cellpadding="2">
		<% '-- CSBR-138430-SMathur- icons will be displayed to the INV_SAMPLE_DISPENSE privlege
		if lCase(Application("ShowCreateSample")) = "true" and Session("INV_SAMPLE_DISPENSE" & dbkey) then %>
		<tr><td colspan="10"><img src="../graphics/sample_create.gif" border="0" WIDTH="16" HEIGHT="15">&nbsp;Create Samples</td></tr>
		<% end if %>
		<%'-- CSBR-138430-SMathur- icons will be displayed to the INV_SAMPLE_DISPENSE privlege
		if Application("ShowRequestSample") and Session("INV_SAMPLE_DISPENSE" & dbkey)  then%>
		<tr><td colspan="10"><img src="../graphics/sample_add.gif" border="0" WIDTH="16" HEIGHT="15">&nbsp;Add Samples</td></tr>
		<tr><td colspan="10"><img src="/cheminv/graphics/ico_cancel.gif" border="0">&nbsp;Cancel Request</td></tr>
		<tr><td colspan="10"><img src="/cheminv/graphics/icon_singleuser.gif" border="0">&nbsp;Assign User</td></tr>
		<%end if %>
		<!--CSBR# 125159-->
		<!--Changed By : Soorya Anwar-->
		<!--Date : 04/Jun/2010 -->		
		<!--Purpose of Change : To add a textual description of the purpose of the reporting icon newly added for this CSBR -->	
		<tr><td colspan="10"><img src="../graphics/ReportIcon.gif" border="0" WIDTH="16" HEIGHT="15">&nbsp;Generate Samples Report</td></tr>		
		<!--End of Change-->		
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



