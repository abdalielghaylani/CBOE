<%@ Language=VBScript %>
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/ado.inc"-->
<!--#INCLUDE VIRTUAL = "/cheminv/api/apiUtils.asp"-->
<!--#INCLUDE VIRTUAL = "/cheminv/gui/guiUtils.asp"-->
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/display_func_vbs.asp"-->
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/utility_func_vbs.asp"-->
<%
Dim Conn
Dim Cmd
Dim RS

BatchID = Request("batchid")
action = lcase(Request("action"))
subaction = lCase(Request("subaction"))
RequestID = Request("RequestID")
ContainerID = Request("ContainerID")
ContainerName = Request("ContainerName")
UOMAbbrv = Request("UOMAbbrv")
AllowBatchRequest = lcase(Request("AllowBatchRequest"))
AmountAvailable = Request("AmountAvailable")
bAllowReservation = Request("bAllowReservation")

if AllowBatchRequest = "" then AllowBatchRequest = false

if Application("DEFAULT_REQUEST_DELIVERY_LOCATION") <> "" then
	LocationID = Application("DEFAULT_REQUEST_DELIVERY_LOCATION")
else
	LocationID = Session("CurrentLocationID")
end if

cLocationID = Request("LocationID")
if cLocationID = "" then cLocationID = 0
dateFormatString = Application("DATE_FORMAT_STRING")

if action = "" then action = "create"
RequestType = Request("RequestType")
if RequestType = "reservation" then
	RequestText = "Reserve an Inventory Batch"
	RequestTextAction = "Reserve"
	RequestTextPast = "Reserved"
	RequestStatusID = 9
else
	RequestText = "Request an Inventory Batch"
	RequestTextAction = "Request"
	RequestTextPast = "Requested"
	RequestStatusID = 2
end if

Select Case action
	Case "create"
		Caption = RequestText & " delivery of samples from this batch."
		DateRequired = Today
		UserID = uCase(Session("UserName" & "cheminv"))
	Case "edit"
		if subaction = "convert" or subaction = "partial" then
			Caption = "Request this reservation."
		else
			Caption = "Edit this request."
		end if
		'response.write "<!--####" & RequestID & ":" & dateFormatString & "-->"
		Call GetInvCommand("{CALL " & Application("CHEMINV_USERNAME") & ".REQUESTS.GetBatchRequest(?,?)}", adCmdText)	
		Cmd.Parameters.Append Cmd.CreateParameter("PREQUESTID",131, 1, 0, RequestID)
		Cmd.Parameters("PREQUESTID").Precision = 9	
		Cmd.Parameters.Append Cmd.CreateParameter("PDATEFORMAT",200,1,30, dateFormatString)		
		Cmd.Properties ("PLSQLRSet") = TRUE  
		Set RS = Cmd.Execute
		Cmd.Properties ("PLSQLRSet") = FALSE
		If (RS.EOF AND RS.BOF) then
			Response.Write ("<table><tr><td align=center colspan=6><span class=""GUIFeedback"">No " & lcase(RequestText) & " found for this container</Span></TD></tr></table>")
			Response.End 
		Else
			UserID = RS("RUserID")
			OrgUnitID = RS("ORG_UNIT_ID_FK")
			DateRequired = ConvertDateToStr(Application("DATE_FORMAT"),RS("Date_Required"))
			LocationID = RS("delivery_Location_ID_FK")
			QtyRequired = RS("qty_required")
			if isBlank(RS("AmountReserved")) then AmountReserved = 0 else AmountReserved = RS("AmountReserved") end if
			if isBlank(RS("AmountRemaining")) then AmountRemaining = 0 else AmountRemaining = RS("AmountRemaining") end if
			'-- Calculate the amount reserved of batch, minus the amount reserved for current reservation
			AmountAvailable = cDbl(AmountRemaining)-(cDbl(AmountReserved)-cDbl(QtyRequired))
			comments = RS("request_comments")
			Field_1 = RS("field_1")
			Field_2 = RS("field_2")
			Field_3 = RS("field_3")
			Field_4 = RS("field_5")
			Field_5 = RS("field_1")
			Date_1 = RS("date_1")
			Date_2 = RS("date_2")
			UOMAbbrv = RS("unit_abreviation")
		End if	
	Case "cancel"
		RequestText = "Cancel an Inventory Batch Request"
		Caption = "Are you sure you want to cancel this " & RequestText & "?"
End select

if not IsEmpty(Application("DEFAULT_SAMPLE_REQUEST_CONC")) then
	arrUOM = split(Application("DEFAULT_SAMPLE_REQUEST_CONC"),"=")
	UOMAbbrv = arrUOM(1)
end if

'-- If UserID is blank, default to current user
if IsBlank(UserID) then 
	UserID = uCase(Session("UserID" & "cheminv"))
end if

%>
<html>
<head>
<title><%=Application("appTitle")%> -- <%=RequestText%></title>
<!--#INCLUDE VIRTUAL = "/cheminv/gui/dateFormat_js.asp"-->
<script LANGUAGE="javascript" src="/cheminv/Choosecss.js"></script>
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/navbar_js.js"-->
<script LANGUAGE="javascript" src="/cheminv/utils.js"></script>
<script LANGUAGE="javascript" src="/cheminv/gui/validation.js"></script>
<script language="JavaScript">
<!--Hide JavaScript
	var DialogWindow;
	window.focus();
	function ValidateRequest(){
		
		var bWriteError = false;
		var errmsg = "Please fix the following problems:\r";
		//LocationID is required
		//document.form1.LocationID.value = document.form1.lpLocationID.value;
		<%if action <> "cancel" then%>
		if (document.form1.LocationID.value.length == 0) {
			errmsg = errmsg + "- Location ID is required.\r";
			bWriteError = true;
		}
		else{
			// LocationID must be a psositve number
			if (!isPositiveNumber(document.form1.LocationID.value)){
				errmsg = errmsg + "- Location ID must be a positive number.\r";
				bWriteError = true;
			}
		}

		// Date required must be a date
		if (document.form1.DateRequired.value.length == 0){
			errmsg = errmsg + "- Date Required is required.\r";
			bWriteError = true;
		}
		if (document.form1.DateRequired.value.length > 0 && !isDate(document.form1.DateRequired.value)){
			errmsg = errmsg + "- Date Required must be in " + dateFormatString + " format.\r";
			bWriteError = true;
		}

		// Quantity reserved is required
		if (document.form1.QtyRequired){
			if (document.form1.QtyRequired.value.length == 0) {
				errmsg = errmsg + "- Quantity is required.\r";
				bWriteError = true;
			}
			// Quantity required if present must be a number
			if (!isPositiveNumber(document.form1.QtyRequired.value)){
				errmsg = errmsg + "- Quantity required must be a positive number greater than zero.\r";
				bWriteError = true;
			}
		}

		// Validate Amount Available and amount requested/reserved
		if (Math.abs(document.form1.QtyRequired.value) > Math.abs(document.form1.AmountAvailable.value)) {
			errmsg = errmsg + "- The amount of sample you have <%=RequestTextPast%> is more than the " + document.form1.AmountAvailable.value + " mg available.\rPlease change your request amount.\r";
			bWriteError = true;
		}
		if (document.form1.PartialQtyRequired){
			if (document.form1.PartialQtyRequired.value.length == 0) {
				errmsg = errmsg + "- Partial Quantity is required.\r";
				bWriteError = true;
			}
			// Quantity required if present must be a number
			if (!isPositiveNumber(document.form1.PartialQtyRequired.value)){
				errmsg = errmsg + "- Partial Quantity required must be a positive number greater than zero.\r";
				bWriteError = true;
			}
		}
		<% if subaction="partial" then %>
		if (isPositiveNumber(document.form1.PartialQtyRequired.value)) {
			if (document.form1.PartialQtyRequired.value == document.form1.QtyRequired.value) {
				document.form1.subaction.value = 'convert';
			} else if (Math.abs(document.form1.PartialQtyRequired.value) > Math.abs(document.form1.QtyRequired.value)) {
				errmsg = errmsg + "- The partial amount must be less than the original amount.\r";
				bWriteError = true;
			} else {			
				document.form1.QtyRequired.value = document.form1.QtyRequired.value - document.form1.PartialQtyRequired.value;
				document.form1.RequestStatusID.value = 9;
			}
		} else {
			document.form1.PartialQtyRequired.value = '';
		}
		<% end if %>
		
		// Build JS validation for custom Request fields
		<%For each Key in req_custom_createrequest_fields_dict%>
		if (document.form1.<%=Key%>.value.length == 0){
			errmsg = errmsg + "- <%=req_custom_createrequest_fields_dict.item(Key)%> is a required field.\r";
			bWriteError = true;
		}
		<% next %>		
		
		<%end if%>
		//bWriteError = true;
		if (bWriteError){
			alert(errmsg);
		}
		else{
			document.form1.submit();
		}
	}
-->
</script>
</head>
<body>
<center>

<% if AllowBatchRequest = "false" and action="create" then %>

	<br /><br /><br /><br />
	<span class="GuiFeedback">This batch is not requestable.</span><br /><br />
	Reason: Pending Certificate of Testing (COT)
	<br><br>
	<a href="#" onclick="window.close(); return false;"><img SRC="/cheminv/graphics/sq_btn/ok_dialog_btn.gif" border="0"></a>

<% elseif bAllowReservation = "False" then %>

	<br /><br /><br /><br />
	<span class="GuiFeedback">You are not allowed to reserved against this batch.</span><br /><br />
	Reason: Not currently associated with an organization
	<br><br>
	<a href="#" onclick="window.close(); return false;"><img SRC="/cheminv/graphics/sq_btn/ok_dialog_btn.gif" border="0"></a>

<% else %>

<form name="form1" action="RequestBatch_action.asp" method="POST">
<input type="hidden" name="action" value="<%=action%>" />
<input type="hidden" name="subaction" value="<%=subaction%>" />
<input type="hidden" name="cLocationID" value="<%=cLocationID%>" />
<input type="hidden" name="RequestType" value="<%=RequestType%>" />
<input type="hidden" name="RequestTypeID" value="2" />
<input type="hidden" name="RequestStatusID" value="<%=RequestStatusID%>" />
<% if action = "cancel" then %>
	<input type="hidden" name="BatchID" value="<%=BatchID%>" />
<% else %>
	<input type="hidden" name="RequestID" value="<%=RequestID%>" />
<% end if %>
<input type="hidden" name="AmountAvailable" value="<%=AmountAvailable%>" />
<% if RequestType <> "reservation" then %>
<input type="hidden" name="OrgUnitID" value="<%=OrgUnitID%>" />
<% end if %>
<table border="0">

	<tr><td colspan="2">
			<span class="GuiFeedback"><%=caption%></span><br><br>
	</td></tr>
	<% if action = "cancel" then %>
	<tr><td align="right" nowrap>
			<span class="required">Request ID:</span>
		</td><td>
			<input type="text" size="20" maxlength="50" name="RequestID" value="<%=RequestID%>" style="background-color:#d3d3d3;" readonly>
	</td></tr>
	<% else %>
	<tr><td align="right" nowrap>
			<span class="required">Batch ID:</span>
		</td><td>
			<input type="text" size="20" maxlength="50" name="BatchID" value="<%=BatchID%>" style="background-color:#d3d3d3;" readonly>
	</td></tr>
	<% end if %>
<%if action = "cancel" then%>
	<tr><td align="right" nowrap valign="top">
			Cancellation Reason:</span>
		</td><td>
			<textarea name="CancelReason" cols="40" rows="5"></textarea>
	</td></tr>
<% end if %>
<%if action <> "cancel" then%>
	<tr height="25">
		<td align="right" valign="top" nowrap>
			<span class="required">Delivery Location ID:</span>
		</td>
		<td colspan="3">
			&nbsp;<%=GetBarcodeIcon()%>&nbsp;<%ShowLocationPicker "document.form1", "LocationID", "lpLocationBarCode", "lpLocationName", 10, 20, false%> 
		</td>
	</tr>
	<tr>
		<td align="right" valign="top" nowrap width="150">
			<span class="required">Date Required:</span>
		</td>
		<td>
			<%call ShowInputField("", "", "DateRequired:form1:" & DateRequired , "DATE_PICKER:TEXT", "15")%>
			<!--<input type="text" name="DateRequired" size="15" value="<%=DateRequired%>"><a href onclick="return PopUpDate(&quot;DateRequired&quot;,&quot;/cheminv/gui/month_view1.asp&quot;)"><img src="/cfserverasp/source/graphics/navbuttons/icon_mview.gif" height="16" width="16" border="0"></a>-->
		</td>
	</tr>
	<tr>
		<% if RequestType = "reservation" then %>
		<%=ShowPickList("<span class=""required"">Reserved For:</span>", "OrgUnitID", OrgUnitID, "select distinct Org_Unit_ID AS Value, Org_Name AS DisplayText from inv_org_unit orgunit, inv_org_users orguser where orgunit.org_unit_id = orguser.org_unit_id_fk and orguser.user_id_fk='" & UserID & "'")%>
		<% else %>
		<%=ShowPickList("<span class=""required"">Requested For:</span>", "UserID", UserID, "SELECT User_ID AS Value, Last_Name||DECODE(First_Name, NULL, '', ', '||First_Name) AS DisplayText FROM CS_SECURITY.People ORDER BY lower(Last_Name) ASC")%>
		<% end if %>
	</tr>
	<% if subaction = "partial" then %>
	<tr>
		<td align="right" nowrap><span class="required">Original Amount (<%=UOMAbbrv%>):</span></td>
		<td><input type="text" size="10" maxlength="50" name="QtyRequired" value="<%=QtyRequired%>" style="background-color:#d3d3d3;" readonly></td>
	</tr>
	<tr>
		<td align="right" nowrap><span class="required">Partial Amount (<%=UOMAbbrv%>):</span></td>
		<td><input type="text" size="10" maxlength="50" name="PartialQtyRequired" value></td>
	</tr>	
	<% elseif subaction = "convert" then %>
	<tr>
		<td align="right" nowrap><span class="required">Amount <%=RequestTextPast%> (<%=UOMAbbrv%>):</span></td>
		<td><input type="text" size="10" maxlength="50" name="QtyRequired" value="<%=QtyRequired%>" style="background-color:#d3d3d3;" readonly></td>
	</tr>
	<% else %>
	<tr>
		<td align="right" nowrap><span class="required">Amount <%=RequestTextPast%> (<%=UOMAbbrv%>):</span></td>
		<td><input type="text" size="10" maxlength="50" name="QtyRequired" value="<%=QtyRequired%>"></td>
	</tr>	
	<% end if %>
	<%
	'-- Display customer Request fields: custom_createrequest_fields_dict, req_custom_createrequest_fields_dict
	For each Key in custom_createrequest_fields_dict
		Response.write("<tr>" & vbcrlf)
		Response.write(vbtab & "<td align=""right"" nowrap>")
		For each Key1 in req_custom_createrequest_fields_dict
			if Key1 = Key then
				Response.Write("<span class=""required"">")
			end if
		Next
		Response.Write(custom_createrequest_fields_dict.item(key) & ":</span></td>" & vbcrlf)
		execute("FieldValue = " & Key)
		
		'-- When converting reservation into request, do not show the custom field values
		if subaction <> "" then FieldValue = ""
		Response.write(vbtab & "<td><input type=""text"" size=""30"" maxlength=""200"" name=""" & key & """ value=""" & FieldValue & """></td>" & vbcrlf)
		Response.write("</tr>" & vbcrlf)	
	Next	
	%>

	<% end if %>
	
	<tr>
		<td colspan="2" align="right"> 
			<a href="#" onclick="if (DialogWindow) {DialogWindow.close()}; window.close(); return false;"><img src="/cheminv/graphics/sq_btn/cancel_dialog_btn.gif" border="0"></a>&nbsp;<a HREF="#" onclick="ValidateRequest(); return false;"><img SRC="/cheminv/graphics/sq_btn/ok_dialog_btn.gif" border="0"></a>
		</td>
	</tr>		
</table>	
</form>

<% end if %>


</center>
</body>
</html>

