<%@ Language=VBScript %>
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



</head>
<body>
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/ado.inc"-->
<!--#INCLUDE VIRTUAL = "/cheminv/api/apiUtils.asp"-->
<!--#INCLUDE VIRTUAL = "/cheminv/gui/guiUtils.asp"-->
<%
Dim Conn
Dim Cmd
Dim RS

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
RequestStatusID = Request("RequestStatusID")
Comments = Request("RequestComments")

if UserID = "" then username = "NULL"
if RequestTypeID = "" then RequestTypeID = "2"
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
Call GetInvCommand("{CALL " & Application("CHEMINV_USERNAME") & ".REQUESTS.GetRequests(?,?,?,?,?,?,?,?,?,?,?,?)}", adCmdText)	
Cmd.Parameters.Append Cmd.CreateParameter("PDELIVERTOLOCATIONID",131, 1, 0, DeliverToLocationID)
Cmd.Parameters.Append Cmd.CreateParameter("PCURRENTLOCATIONID",131, 1, 0, CurrentLocationID)
Cmd.Parameters.Append Cmd.CreateParameter("PFROMDATE",200, 1, 200, FromDate)
Cmd.Parameters.Append Cmd.CreateParameter("PTODATE",200, 1, 200, ToDate)
Cmd.Parameters.Append Cmd.CreateParameter("PREQUESTCOMMENTS",200, 1, 2000, Comments)
Cmd.Parameters.Append Cmd.CreateParameter("PUSERID",200, 1, 30, UserID)
Cmd.Parameters.Append Cmd.CreateParameter("PCONTAINERBARCODE",200, 1, 30, ContainerBarcode)
Cmd.Parameters.Append Cmd.CreateParameter("PRequestType",200, 1, 30, RequestType)
Cmd.Parameters.Append Cmd.CreateParameter("PREQUESTTYPEID",131, 1, 0, RequestTypeID)
Cmd.Parameters.Append Cmd.CreateParameter("PREQUESTSTATUSID",131, 1, 0, RequestStatusID)
Cmd.Parameters.Append Cmd.CreateParameter("PSHIPTONAME",200, 1, 255, ShipToName)
Cmd.Parameters.Append Cmd.CreateParameter("PDATEFORMAT",200,1,30, dateFormatString)
if bDebugPrint then
	For each p in Cmd.Parameters
		Response.Write p.name & " = " & p.value & "<BR>"
	Next	
	Response.End
Else
	Cmd.Properties ("PLSQLRSet") = TRUE  
	Set RS = Cmd.Execute
	Cmd.Properties ("PLSQLRSet") = FALSE

bShowBatch = false
if RequestStatusID = "2" then
	caption = "The following requests are waiting to be approved:"
	disable1 = "disabled"
	bShowBatch = true
elseif RequestStatusID = "3" then
	caption = "The following requests have been approved:"
	disable2 = "disabled"
	bShowBatch = true
elseif RequestStatusID = "4" then
	caption = "The following requests have been declined:"
	disable3 = "disabled"
elseif RequestStatusID = "5" then
	caption = "The following requests have been filled:"
	disable4 = "disabled"
elseif RequestStatusID = "6" then
	caption = "The following requests are closed:"
	disable5 = "disabled"
end if

%>
	<center>
	<br>
	<p><span class="GUIFeedback"><%=Caption%></span></p>
	<form name="form1" action="" method="POST" OnSubmit="ChooseAction();return false;">
	<table border="1">
	<tr>
		<th>
			&nbsp;
		</th>
		<th>
			Request ID
		</th>
		<%if RequestStatusID = "5" then%>
			<th>
				<SPAN TITLE="The number of samples not in an order."># of Samples</SPAN>
			</th>
		<%end if%>
		<%if RequestStatusID = "5" or RequestStatusID = "6" then%>
			<th>
				Orders
			</th>
		<%else%>
			<th>
				ContainerID
			</th>
			<th>
				Container Name
			</th>
		<%end if%>
		<th>
			Requested By
		</th>
		<th>
			Delivery Location
		</th>
		<%if bShowBatch then%>
		<th>
			Batch Amount
		</th>
		<%end if%>
		<th>
			Date Requested
		</th>
		<th>
			Date Required
		</th>
		<%if RequestStatusID = "2" then%>
		<th>
			Accept
		</th>
		<th>
			Decline
		</th>
		<%elseif RequestStatusID = "5" then%>
		<th>
			Close
		</th>
		<%end if%>
	</tr>
<%

	If (RS.EOF AND RS.BOF) then
		Response.Write ("<TR><TD align=center colspan=""20""><span class=""GUIFeedback"">No requests found</Span></TD></tr>")
	Else
		NumRequests = 0
		While (Not RS.EOF)
			ContainerID = RS("container_id_fk")
			ContainerBarcode = RS("Barcode")
			ContainerName = RS("Container_Name") 
			RequestID = RS("Request_ID")
			RequestUserID = RS("RUserID")
			QtyRequired = RS("Qty_Required")
			UOMAbv = RS("unit_abreviation")
			DateRequested = RS("timestamp")
			DateRequired = RS("Date_Required")
			DateDelivered = RS("Date_Delivered")
			DestinationLocationName = RS("Location_Name")
			editData = "ContainerID=" & ContainerID & "&ContainerBarcode= " & ContainerBarcode & "&ContainerName=" & ContainerName & "&UOMAbv=" & UOMAbv &  "&RequestID=" & RequestID 
			Comments = RS("request_comments")	
			NumContainers = RS("number_containers")
			QtyList = RS("quantity_list")
			ContainerTypeID = RS("container_type_id_fk")
			NumRequests = NumRequests + 1
			DeliveryLocationID = RS("delivery_location_id_fk")
			ShipToName = RS("ship_to_name")
			NumSamples = RS("NumSamples")
			OrderList = RS("OrderList")
			RequestCreator = RS("Creator")
			
			'create edit order links
			OrderLinks = ""
			if not isNull(OrderList) and not isEmpty(OrderList) then
				arrOrders = split(OrderList, ",")
				for i=0 to ubound(arrOrders)
					arrOrders2 = split(arrOrders(i),":")
					if arrOrders2(1) = "1" then
						OrderLinks = OrderLinks & "<a Class=""MenuLink"" HREF=""#"" onclick=""OpenDialog('/Cheminv/GUI/Order_frset.asp?Clear=1&Action=edit&OrderID=" & arrOrders2(0) & "', 'Diag', 2); return false"">" & arrOrders2(0) & "</a><BR>" 
					else
						OrderLinks = OrderLinks & "<a Class=""MenuLink"" HREF=""#"" onclick=""OpenDialog('/Cheminv/GUI/Order_list.asp?Clear=1&Action=edit&OrderID=" & arrOrders2(0) & "', 'Diag', 2); return false"">" & arrOrders2(0) & "</a><BR>" 
					end if
				next
			end if
			if OrderLinks = "" then OrderLinks = "&nbsp;"
									
			if bShowBatch then
				SQL = "SELECT container_id, parent_container_id_fk, " & Application("ORASCHEMANAME") & ".GUIUTILS.GETBATCHAMOUNTSTRING(parent_container_id_fk) AS Batch_Amount_String FROM inv_containers where container_id = ?"
				Set Cmd = GetCommand(Conn, SQL, adCmdText)
				Cmd.Parameters.Append Cmd.CreateParameter("ContainerID", 5, 1, 0, ContainerID)
				Set rsBatch = Cmd.Execute
				BatchString = rsBatch("Batch_Amount_String")
				ParentContainerID = rsBatch("parent_container_id_fk")
				if IsNull(BatchString) then 
					BatchLink = "&nbsp;"
				else
					BatchLink = "<A CLASS=""MenuLink"" HREF=""#"" TITLE=""Batch Information"" ONCLICK=""OpenDialog('/Cheminv/GUI/ViewBatchInfo.asp?ContainerID=" & ParentContainerID & "&ShowLinks=0', 'Diag', 5); return false;"" onmouseover=""javascript:this.style.cursor='hand';"" onmouseout=""javascript:this.style.cursor='default';"">" & rsBatch("Batch_Amount_String") & "</a>"
				end if
			end if

%>
			<tr>
<%
			if RequestStatusID = "2" then
				link = "<a Class=""MenuLink"" Href=""#"" onclick=""OpenDialog('/Cheminv/GUI/RequestSample.asp?action=edit&amp;" & editdata & "', 'Diag', 1); return false"">Review</a>"
			elseif RequestStatusID = "3" then
				link = "<a Class=""MenuLink"" Href=""#"" onclick=""OpenDialog('/Cheminv/GUI/CreateSamplesFromBatch.asp?RequestID=" & RequestID & "&ContainerID=" & ContainerID & "&NumContainers=" & NumContainers & "&QtyList=" & QtyList & "&ContainerTypeID=" & ContainerTypeID & "&ContainerBarcode=" & ContainerBarcode & "', 'Diag', 1); return false"">Create Samples</a>"
				'link = "<a Class=""MenuLink"" disabled>Create Samples</a>"
			elseif RequestStatusID = "4" then
				link = "<a Class=""MenuLink"" Href=""#"" onclick=""OpenDialog('/Cheminv/GUI/DeclineRequestSample.asp?DeclinedRequestIDList=" & RequestID & "&action=edit', 'Diag', 1); return false"">Reason</a>"
			elseif RequestStatusID = "5" then
				link = "<a Class=""MenuLink"" Href=""#"" onclick=""OpenDialog('/Cheminv/GUI/Order_frset.asp?Clear=1&Action=create&RequestID=" & RequestID & "&DeliveryLocationID=" & DeliveryLocationID & "&ShipToName=" & server.URLEncode(ShipToName) & "', 'Diag', 2); return false"">Create Order</a>"
			elseif RequestStatusID = "6" then
				'link = "<a Class=""MenuLink"" Href=""#"" onclick=""OpenDialog('/Cheminv/GUI/RequestSample.asp?action=edit&amp;" & editdata & "', 'Diag', 1); return false"">Review</a>"
				link = ""
			end if
%>
				<td align="center"><%=link%></td>
<!--
				<td align="center"> 
					<a Class="MenuLink" Href="#" onclick="OpenDialog('/Cheminv/GUI/Request.asp?action=undodelivery&amp;<%=editdata%>', 'Diag', 1); return false">Undo</a>
				</td>
-->
				<td align="center"> 
					<%=RequestID%> 
				</td>
				<%if RequestStatusID = "5" then%>
					<td align="center"> 
						<SPAN TITLE="The number of samples not in an order."><%=NumSamples%></SPAN>
					</td>
				<%end if%>
				<%if RequestStatusID = "5" or RequestStatusID = "6" then%>
					<td align="center">
						<%=OrderLinks%>
					</td>
				<%else%>
					<td align="center"> 
						<span title="<%=Comments%>"><%=ContainerBarcode%></span> 
					</td>
					<td align="center"> 
						<%=TruncateInSpan(ContainerName, 15, "")%> 
					</td>
				<%end if%>
				<td align="center"> 
					<%=TruncateInSpan(RequestCreator, 15, "")%> 
				</td>
				<td align="center"> 
					<%=TruncateInSpan(DestinationLocationName, 15, "")%> 
				</td>
				<%if bShowBatch then%>
					<td align="right"> 
						<%=BatchLink%>
						<%'=BatchString & " " & UOMAbv%>
					</td>
				<%end if%>
				<td align="center">
					<%=TruncateInSpan(DateRequested, 11, "")%>
				</td>
				<td align="center">
					<%=TruncateInSpan(DateRequired, 11, "")%>
				</td>
			<%if RequestStatusID = "2" then%>
				<td align="center">
					<input type="checkbox" name="ApprovedRequestIDList" value="<%=RequestID%>" onclick="UncheckDecline(this);")>
				</td>	
				<td align="center">
					<input type="checkbox" name="DeclinedRequestIDList" value="<%=RequestID%>" onclick="UncheckApprove(this);")>
				</td>	
			<%elseif RequestStatusID = "5" then%>
				<td align="center">
					<input type="checkbox" name="ClosedRequestIDList" value="<%=RequestID%>")>
				</td>	
			<%end if%>	
			</tr>
			<%rs.MoveNext
		Wend%>
		<input type="hidden" name="NumRequests" value="<%=NumRequests%>">		
	</table>
<%	
	End if
	RS.Close
	Conn.Close
	Set RS = nothing
	Set Cmd = nothing
	Set Conn = nothing
End if
%>
<table>
	<tr>
		<td colspan="8" align="right"> 
			<%
			'only show for new and filled
			if RequestStatusID = "2" or RequestStatusID = "5" then
			%>
			<input type="image" src="../graphics/ok_dialog_btn.gif" border="0" WIDTH="61" HEIGHT="21">
			<%end if%>
		</td>
	</tr>	
</table>
</form>
</center>
</body>
</html>



