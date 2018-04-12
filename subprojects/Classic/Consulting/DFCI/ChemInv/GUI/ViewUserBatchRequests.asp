<%@ Language=VBScript %>
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/ado.inc"-->
<!--#INCLUDE VIRTUAL = "/cheminv/api/apiUtils.asp"-->
<!--#INCLUDE VIRTUAL = "/cheminv/gui/guiUtils.asp"-->
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/display_func_vbs.asp"-->
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/utility_func_vbs.asp"-->
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/cs_security/cs_security_login_utils_vbs.asp" -->
<!--#INCLUDE VIRTUAL = "/cheminv/cheminv/fieldArrayUtils.asp" -->
<%

'if not Session("IsFirstRequest") then 
'	StoreASPSessionID()
'else
'	Session("IsFirstRequest") = false
'	Response.Write("<html><body onload=""window.location.reload()"">&nbsp;<form name=""form1""></form></body></html>")	
'end if

Dim Conn
Dim Cmd
Dim RS

dateFormatString = Application("DATE_FORMAT_STRING")
UserName = uCase(Session("UserName" & "cheminv"))
if Session("rTab") = "" or Request("rTab") = "" then
	rTab = "MyRequests"
	Session("rTab") = "MyRequests"
	pageTitle = "My Requests"
else
	if Request("rTab") <> "" then
		rTab = Request("rTab") 
	elseif Session("rTab") <> "" then 
		rTab = Session("rTab") 
	end if
	Session("rTab") = rTab
	pageTitle = "My Reservations"
end if

if Application("AllowRequests") then

				
%>
<html>
<head>
<title><%=Application("appTitle")%> -- <%=pageTitle%></title>
<!--#INCLUDE VIRTUAL = "/cheminv/gui/dateFormat_js.asp"-->
<script LANGUAGE="javascript" src="/cheminv/Choosecss.js"></script>
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/navbar_js.js"-->
<script LANGUAGE="javascript" src="/cheminv/utils.js"></script>
<script LANGUAGE="javascript" src="/cheminv/gui/validation.js"></script>
<script language="JavaScript">
<!--Hide JavaScript
	var DialogWindow;
	window.focus();
-->
</script>
</head>
<body>

<!-- INCLUDE VIRTUAL = "/cheminv/gui/ViewUserBatchRequestTabs.asp" -->
<div style="margin-left: 10px;">
<a class="MenuLink" href="ViewUserBatchRequests.asp?rtab=MyRequests" <%if rtab="MyRequests" then Response.Write("style=""color: #666;""") %>>My Requests</a> | 
<a class="MenuLink" href="ViewUserBatchRequests.asp?rtab=Reservations" <%if rtab="Reservations" then Response.Write("style=""color: #666;""") %>>My Reservations</a>
<% if rTab = "MyRequests" then %>
	| <a class="MenuLink" href="#" title="Click to choose report columns" onclick="OpenDialog('/cheminv/cheminv/columnPicker2.asp?ArrayID=10&amp;showRequests=true', 'CCDiag', 4); return false">Column Chooser</a>
<% else %>
	| <a class="MenuLink" href="#" title="Click to choose report columns" onclick="OpenDialog('/cheminv/cheminv/columnPicker2.asp?ArrayID=1&amp;showReservations=true', 'CCDiag', 4); return false">Column Chooser</a>
<% end if %>
</div>

<center>
<form name="form1" method="post">

<%
Select Case rTab
	Case "MyRequests"
%>
<span class="GuiFeedback">My Requests</span><br /><br />

<table>
<%

'-- Set up fieldArray containing the column definition for the on screen report
If NOT IsArray(Session("RequestReportFieldArray10")) then
	ColDefStr = GetUserProperty(Session("UserNameCheminv"),"RequestChemInvFA10")
	If ColDefStr = "" OR IsNull(ColDefStr) then 
		' Default column definition
		ColDefstr= Application("RequestReportColDef10")
		rc = WriteUserProperty(Session("UserNameCheminv"), "RequestChemInvFA10", ColDefstr)
	End if
	fieldArray = GetFieldArray(colDefstr, Application("RequestFieldMap"))
	Session("RequestReportFieldArray10") = fieldArray
Else
	fieldArray = Session("RequestReportFieldArray10")
End if

'-- Show sample requests
RequestTypeID = 1
Call GetInvCommand("{CALL " & Application("CHEMINV_USERNAME") & ".REQUESTS.GETBATCHREQUEST2(?,?,?,?)}", adCmdText)	
Cmd.Parameters.Append Cmd.CreateParameter("P_REQUESTTYPEID",131, 1, 0, 2) '-- Batch Request Types = 2
'Cmd.Parameters("P_REQUESTTYPEID").Precision = 9	
Cmd.Parameters.Append Cmd.CreateParameter("P_USERID",200, 1, 30, UserName)
Cmd.Parameters.Append Cmd.CreateParameter("P_DATEFORMAT",200,1,30, dateFormatString)		
Cmd.Parameters.Append Cmd.CreateParameter("P_REGSERVER", 200, 1, 30, Application("RegServerName"))
Cmd.Properties ("PLSQLRSet") = TRUE  
Set RS = Cmd.Execute

If (RS.EOF AND RS.BOF) then
	Response.Write ("<center><table><tr><td align=center colspan=6><span class=""GUIFeedback"">You have no unfulfilled requests.</Span></TD></tr></table></center><BR><BR>")
Else
%>
<tr>
	<th nowrap>Request ID</th>
	<%
	for i=0 to ubound(fieldArray)
		if fieldArray(i,0) = BatchAmount and bShowBatch then
			Response.Write("<th nowrap>" & fieldArray(i,1) & "</th>")
		else
			Response.Write("<th nowrap>" & fieldArray(i,1) & "</th>")
		end if
	next
	%>
	<th>Status</th>
	<th align="center">&nbsp;</td>
</tr>
<%
While Not RS.EOF
	RequestID = RS("Request_ID")
	BatchID = RS("Batch_ID_FK")
	ContainerID = RS("Container_ID_FK")
	CreatorUserID = RS("Creator")
	RequestedBy = RS("Creator")
	'RequestedBy = RS("RUserID")
	RequestReceiptDocID = RS("RequestReceiptDocID")
	RequestWorksheetDocID = RS("RequestWorksheetDocID")
	AmountRequested = RS("Qty_Required")
	DateRequested = RS("timestamp")
	DateRequired = RS("Date_Required")
	DestinationLocationID = RS("delivery_Location_ID_FK")
	DeliveryLocation = RS("Location_Name")
	Comments = RS("request_comments")	
	RequestTypeID = RS("request_type_id_fk")
	NumContainers = RS("number_containers")
	ContainerTypeID = RS("container_type_id_fk")
	QtyList = RS("quantity_list")
	ShipToName = RS("ship_to_name")
	Status = RS("request_status_name")
	RequestedForName = RS("RequestedForName")
	CreatorName = RS("CreatorName")
	DeclineReason = RS("decline_reason")
	BatchField1 = RS("batch_field_1")
	BatchField2 = RS("batch_field_2")
	BatchField3 = RS("batch_field_3")
	RequiredUOMabbrv = RS("RequiredUOMabbrv")
	for each key in reg_fields_dict
		if key <> "BASE64_CDX" then
		execute("Request" & key & " = RS(""" & key & """)")
		end if
	next

%>
	<tr>
		<td align=center nowrap><%=TruncateInSpan(RequestID, 20, "")%>&nbsp;</td>
		<%
		for i=0 to ubound(fieldArray)
			if fieldArray(i,0) = "AmountRequested" then
				Response.Write("<td align=""right"" width=""" & fieldArray(i,2) & """ nowrap>")
				execute("response.write(FormatNumber(" & fieldArray(i,0) & ",2) & RequiredUOMabbrv)")
				Response.Write("</td>")
			else
				Response.Write("<td align=""left"" width=""" & fieldArray(i,2) & """ nowrap>")
				execute("response.write(TruncateInSpan(" & fieldArray(i,0) & ", fieldArray(i,2), """"))" )
				Response.Write("</td>")
			end if
		next
		%>
		<td align=center><%=TruncateInSpan(Status, 10, "")%>&nbsp;</td>
		<td align="right">
		<% if BatchID <> "" then %>
			<% if Status = "New" then %>
				<a Class="MenuLink" Href="#" onclick="OpenDialog('/Cheminv/GUI/RequestBatch.asp?BatchID=<%=BatchID%>&RequestID=<%=RequestID%>&action=edit', 'Diag', 1); return false">&nbsp;Edit</a> | 
				<a Class="MenuLink" Href="#" onclick="OpenDialog('/Cheminv/GUI/RequestBatch.asp?BatchID=<%=BatchID%>&RequestID=<%=RequestID%>&action=cancel', 'Diag', 1); return false">Cancel</a>
			<% elseif Status = "Approved" then %>
				<a Class="MenuLink" Href="#" onclick="OpenDialog('/Cheminv/GUI/RequestBatch.asp?BatchID=<%=BatchID%>&RequestID=<%=RequestID%>&action=cancel', 'Diag', 1); return false">Cancel</a>
			<% else %>
			&nbsp;
			<% end if %>
		<% end if %>
		<% 
		if Status = "Closed" or Status = "Filled" then
			if RequestReceiptDocID <> "" then
				Response.Write("&nbsp;&nbsp;<a class=""MenuLink"" Href=""#"" onclick=""OpenDialog('/cheminv/gui/viewdoc.asp?docid=" & RequestReceiptDocID & "', 'Diag1', 2); return false;"" title=""Print Receipt"">")
				Response.Write("Receipt</a>&nbsp;")
			end if
			if RequestWorksheetDocID <> "" then
				Response.Write("|&nbsp;<a class=""MenuLink"" Href=""#"" onclick=""OpenDialog('/cheminv/gui/viewdoc.asp?docid=" & RequestWorksheetDocID & "', 'Diag1', 2); return false;"" title=""Print Worksheet"">")
				Response.Write("Worksheet</a>")
			end if
		end if 
		%>
		</td>
	</tr>
<%
RS.MoveNext
Wend
	Response.Write("<tr><td colspan=""9"" align=""right""><a HREF=""#"" onclick=""window.close(); return false;""><img src=""/cheminv/graphics/sq_btn/cancel_dialog_btn.gif"" border=""0"" width=""61"" height=""21""></a></td></tr>")
Response.Write "</table>"
End if

	Case "Reservations"
	%>
	<span class="GuiFeedback">My Reservations</span><br /><br />
<table>
<%
'-- Set up fieldArray containing the column definition for the on screen report
If NOT IsArray(Session("ReservationReportFieldArray1")) then
	ColDefStr = GetUserProperty(Session("UserNameCheminv"),"ReservationChemInvFA1")
	If ColDefStr = "" OR IsNull(ColDefStr) then 
		' Default column definition
		ColDefstr= Application("ReservationReportColDef1")
		rc = WriteUserProperty(Session("UserNameCheminv"), "ReservationChemInvFA1", ColDefstr)
	End if
	fieldArray = GetFieldArray(colDefstr, Application("ReservationFieldMap"))
	Session("ReservationReportFieldArray1") = fieldArray
Else
	fieldArray = Session("ReservationReportFieldArray1")
End if

'-- Show sample requests
RequestTypeID = 1
Call GetInvCommand("{CALL " & Application("CHEMINV_USERNAME") & ".REQUESTS.GETBATCHRESERVATION(?,?,?,?)}", adCmdText)	
Cmd.Parameters.Append Cmd.CreateParameter("P_REQUESTTYPEID",131, 1, 0, 2) '-- Batch Request Types = 2
'Cmd.Parameters("P_REQUESTTYPEID").Precision = 9	
Cmd.Parameters.Append Cmd.CreateParameter("P_USERID",200, 1, 30, UserName)
Cmd.Parameters.Append Cmd.CreateParameter("P_DATEFORMAT",200,1,30, dateFormatString)		
Cmd.Parameters.Append Cmd.CreateParameter("P_REGSERVER", 200, 1, 30, Application("RegServerName"))
Cmd.Properties ("PLSQLRSet") = TRUE  
Set RS = Cmd.Execute

If (RS.EOF AND RS.BOF) then
	Response.Write ("<center><table><tr><td align=center colspan=6><span class=""GUIFeedback"">You have no pending reservations.</Span><br /><br />")
	Response.Write ("<a HREF=""#"" onclick=""window.close(); return false;""><img src=""/cheminv/graphics/sq_btn/cancel_dialog_btn.gif"" border=""0"" width=""61"" height=""21""></a>")
	Response.Write ("</td></tr></table></center><BR><BR>")
Else
%>
<tr>
	<th>Reservation ID</th>
	<!--
	<th>Created By</th>
	<th>Batch ID</th>
	<th>Reserved For</th>
	<th>Delivery Location</th>
	<th>Amount</th>
	<th>Date Reserved</th>
	<th>Date Required</th>
	-->
	<%
	for i=0 to ubound(fieldArray)
		if fieldArray(i,0) = BatchAmount and bShowBatch then
			Response.Write("<th nowrap>" & fieldArray(i,1) & "</th>")
		else
			Response.Write("<th nowrap>" & fieldArray(i,1) & "</th>")
		end if
	next
	%>
	<th align="center">&nbsp;</td>
</tr>
<% 
While Not RS.EOF
	RequestID = RS("Request_ID")
	BatchID = RS("Batch_ID_FK")
	ContainerID = RS("Container_ID_FK")
	CreatorUserID = RS("Creator")
	RequestUserID = RS("RUserID")
	RequestedBy = RS("Creator")
	Organization = RS("Organization")
	RequestReceiptDocID = RS("RequestReceiptDocID")
	RequestWorksheetDocID = RS("RequestWorksheetDocID")
	AmountRequested = RS("Qty_Required")
	DateRequested = RS("timestamp")
	DateRequired = RS("Date_Required")
	DestinationLocationID = RS("delivery_Location_ID_FK")
	DeliveryLocation = RS("Location_Name")
	Comments = RS("request_comments")	
	RequestTypeID = RS("request_type_id_fk")
	NumContainers = RS("number_containers")
	ContainerTypeID = RS("container_type_id_fk")
	QtyList = RS("quantity_list")
	ShipToName = RS("ship_to_name")
	Status = RS("request_status_name")
	RequestedForName = RS("RequestedForName")
	CreatorName = RS("CreatorName")
	RequiredUOMabbrv = RS("RequiredUOMabbrv")
	for each key in reg_fields_dict
		if key <> "BASE64_CDX" then
		execute("Request" & key & " = RS(""" & key & """)")
		end if
	next

%>
	<tr>
		<td align=center> 
			<%=TruncateInSpan(RequestID, 20, "")%> &nbsp;</td>
		<%
		for i=0 to ubound(fieldArray)
			if fieldArray(i,0) = "AmountRequested" then
				Response.Write("<td align=""right"" width=""" & fieldArray(i,2) & """>")
				execute("response.write(FormatNumber(" & fieldArray(i,0) & ",2) & RequiredUOMabbrv)")
				Response.Write("</td>")
			else
				Response.Write("<td align=""left"" width=""" & fieldArray(i,2) & """ nowrap>")
				execute("response.write(TruncateInSpan(" & fieldArray(i,0) & ", fieldArray(i,2), """"))" )
				Response.Write("</td>")
			end if
		next
		%>
		<td align="right">
		<% if BatchID <> "" then %>
			<!--<a class="menulink" href="#" onclick="OpenDialog('/Cheminv/GUI/RequestBatch.asp?action=edit&subaction=convert&BatchID=<%=BatchID%>&RequestID=<%=RequestID%>', 'Diag1', 1); return false">All</a> | -->
			<a class="menulink" href="#" onclick="OpenDialog('/Cheminv/GUI/RequestBatch.asp?action=edit&subaction=partial&BatchID=<%=BatchID%>&RequestID=<%=RequestID%>', 'Diag2', 1); return false">Request</a> | 
			<a class="menulink" href="#" onclick="OpenDialog('/Cheminv/GUI/RequestBatch.asp?action=edit&BatchID=<%=BatchID%>&RequestID=<%=RequestID%>&RequestType=reservation', 'Diag3', 1); return false">Edit</a> |			
			<a class="menulink" href="#" onclick="OpenDialog('/Cheminv/GUI/RequestBatch.asp?action=cancel&subaction=convert&RequestType=reservation&BatchID=<%=BatchID%>&RequestID=<%=RequestID%>', 'Diag4', 1); return false">Cancel</a>
		<% end if %>
		</td>
	</tr>
<%
	RS.MoveNext
	Wend
		Response.Write("<tr><td colspan=""9"" align=""right""><br /><a HREF=""#"" onclick=""window.close(); return false;""><img src=""/cheminv/graphics/sq_btn/cancel_dialog_btn.gif"" border=""0"" width=""61"" height=""21""></a></td></tr>")
	Response.Write "</table>"
	End if
	
End Select

End If
%>

</center>
</form>

</body>
</html>

