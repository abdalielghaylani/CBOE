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

dateFormatString = Application("DATE_FORMAT_STRING")

'-- get request data
BatchID = Request("batchid")
action = lcase(Request("action"))
subaction = lCase(Request("subaction"))
RequestID = Request("RequestID")
UOMAbbrv = Request("UOMAbbrv")
AllowBatchRequest = lcase(Request("AllowBatchRequest"))
AmountAvailable = Request("AmountAvailable")
bAllowReservation = Request("bAllowReservation")
cLocationID = Request("LocationID")
bContainerinNonSysLoc= true
'--set defaults
requestType = "reservation"
if AllowBatchRequest = "" then AllowBatchRequest = false
if Application("DEFAULT_REQUEST_DELIVERY_LOCATION") <> "" then
	LocationID = Application("DEFAULT_REQUEST_DELIVERY_LOCATION")
else
	LocationID = Session("CurrentLocationID")
end if
if cLocationID = "" then cLocationID = 0
if not IsEmpty(Application("DEFAULT_SAMPLE_REQUEST_CONC")) and UOMAbbrv="" then
	arrUOM = split(Application("DEFAULT_SAMPLE_REQUEST_CONC"),"=")
	UOMAbbrv = arrUOM(1)
end if
if action = "" then action = "create"
RequestText = "Reserve"
RequestTextAction = "Reserve"
RequestTextPast = "Reserved"
RequestStatusID = 9

Call GetInvCommand(Application("CHEMINV_USERNAME") & ".GUIUTILS.GETBATCHUOMAMOUNTSTRING", adCmdStoredProc)
Cmd.Parameters.Append Cmd.CreateParameter("RETURN_VALUE", adVarChar, adParamReturnValue, 200, NULL)
Cmd.Parameters.Append Cmd.CreateParameter("pContainerID", adNumeric, 1, 0, NULL)
Cmd.Parameters.Append Cmd.CreateParameter("pBatchID", adNumeric, 1, 200, cint(BatchID))
if bDebugPrint then
    For each p in Cmd.Parameters
	    Response.Write p.name & " = " & p.value & "<BR>"
    Next	
Else
    Call ExecuteCmd(Application("CHEMINV_USERNAME") & ".GUIUTILS.GETBATCHUOMAMOUNTSTRING")
End if
BatchAmountAvailable = Cmd.Parameters("RETURN_VALUE")
if BatchAmountAvailable <>"" then 
    tempArr = split(BatchAmountAvailable, ",")
    for each element in tempArr
        tempArr2 = split(element,":")
        AvailabeUOM = AvailabeUOM & ",'" & tempArr2(1) & "'"    
    next
    AvailabeUOM = mid(AvailabeUOM,2,len(AvailabeUOM)) 
else
    bContainerinNonSysLoc=false
end if 

Select Case action
	Case "create"
		Caption = RequestText & " an amount from this batch."
		DateRequired = Today
		UserID = uCase(Session("UserName" & "cheminv"))
	Case "edit"
		if subaction = "convert" or subaction = "partial" then
			Caption = "Request this reservation."
		else
			Caption = "Edit this reservation."
		end if
		Call GetInvCommand("{CALL " & Application("CHEMINV_USERNAME") & ".REQUESTS.GetBatchRequest(?,?)}", adCmdText)	
		Cmd.Parameters.Append Cmd.CreateParameter("PREQUESTID",131, 1, 0, RequestID)
		Cmd.Parameters("PREQUESTID").Precision = 9	
		Cmd.Parameters.Append Cmd.CreateParameter("PDATEFORMAT",200,1,30, dateFormatString)		
		Cmd.Properties ("PLSQLRSet") = TRUE  
		Set RS = Cmd.Execute
		Cmd.Properties ("PLSQLRSet") = FALSE
		If (RS.EOF AND RS.BOF) then
		    recordFound = false
		Else
		    recordFound = true
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
			unitstring = RS("unitstring")
			'UOMAbbrv = RS("unit_abreviation")
		End if	
	Case "cancel"
		RequestText = "Cancel a Reservation"
		Caption = "Are you sure you want to cancel this " & RequestText & "?"
End select

'-- If UserID is blank, default to current user
if IsBlank(UserID) then 
	UserID = uCase(Session("UserID" & "cheminv"))
end if

'Fixing CSBR-87551
if bAllowReservation then 
    SQL= "select distinct Org_Unit_ID AS Value, Org_Name AS DisplayText from inv_org_unit orgunit, inv_org_users orguser where orgunit.org_unit_id = orguser.org_unit_id_fk and orguser.user_id_fk='" & UserID & "'"
    call GetInvConnection()
    Set Cmd = GetCommand(Conn, SQL, adCmdText)
    Set RS1 = Server.CreateObject("ADODB.recordset")
    Set RS1 = Cmd.Execute
    if RS1.EOF or RS1.BOF then bAllowReservation="False"
end if  
%>
<html>
<head>
<title><%=Application("appTitle")%> -- Reserve a batch amount</title>
<!--#INCLUDE VIRTUAL = "/cheminv/gui/dateFormat_js.asp"-->
<script type="text/javascript" language="javascript" src="/cheminv/Choosecss.js"></script>
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/navbar_js.js"-->
<script type="text/javascript" language="javascript" src="/cheminv/utils.js"></script>
<script type="text/javascript" language="javascript" src="/cheminv/gui/validation.js"></script>
<script type="text/javascript" language="javascript">
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
		/*if (Math.abs(document.form1.QtyRequired.value) > Math.abs(document.form1.AmountAvailable.value)) {
			errmsg = errmsg + "- The amount of sample you have <%=RequestTextPast%> is more than the " + document.form1.AmountAvailable.value + " mg available.\rPlease change your request amount.\r";
			bWriteError = true;
		}*/
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
		
	//Checking for the batch quantity available for the selected UOM
	   var ValueSelected; 
	   var myArray;
	   var i;
	   var quantity;
	   var arraySelectedVal;
	   myArray = document.form1.BatchAmountAvailable.value.split(",");
	   ValueSelected = document.form1.UOM.options[document.form1.UOM.selectedIndex].value;
	   arraySelectedVal = ValueSelected.split("=");
	   for(i=0;i<myArray.length;i++){
	       quantity = myArray[i].split(":");
	       if (quantity[1] == arraySelectedVal[1]){
	            document.form1.RequiredUOM.value = arraySelectedVal[0];
	            if (Number(document.form1.QtyRequired.value) > Number(quantity[0])){
	                errmsg = errmsg + "- Quantity required can not be exceed than Quantity Available with selected Unit of Measure.\r";
		            bWriteError = true;
		            break;
	            }
	       }
	   }
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
<%if action = "edit" and recordFound = false then%>
    <br /><br /><br /><br />
	<span class="GuiFeedback">No reservation found.</span>
	<br/><br/>
	<a href="#" onclick="opener.focus();window.close(); return false;"><img SRC="/cheminv/graphics/sq_btn/ok_dialog_btn.gif" border="0"></a>

<% elseif AllowBatchRequest = "false" and action="create" then %>

	<br /><br /><br /><br />
	<span class="GuiFeedback">This batch is not requestable.</span><br /><br />
	Reason: Pending Certificate of Testing (COT)
	<br/><br/>
	<a href="#" onclick="opener.focus();window.close(); return false;"><img SRC="/cheminv/graphics/sq_btn/ok_dialog_btn.gif" border="0"></a>

<% elseif bAllowReservation = "False" then %>

	<br /><br /><br /><br />
	<span class="GuiFeedback">You are not allowed to make reservations on batches.</span><br /><br />
	Reason: You are not currently associated with an organization.
	<br/><br/>
	<a href="#" onclick="opener.focus();window.close(); return false;"><img SRC="/cheminv/graphics/sq_btn/ok_dialog_btn.gif" border="0"></a>
<% elseif bContainerinNonSysLoc = "False" then %>
	<br /><br /><br /><br />
	<span class="GuiFeedback">You are not allowed to make reservations on this batch.</span><br /><br />
	Reason: Containers of this batch are not available in non system locations.  
	<br/><br/>
	<a href="#" onclick="opener.focus();window.close(); return false;"><img SRC="/cheminv/graphics/sq_btn/ok_dialog_btn.gif" border="0"></a>
<% else %>

<form name="form1" action="ReserveBatch_action.asp" method="POST">
<input type="hidden" name="action" value="<%=action%>" />
<input type="hidden" name="subaction" value="<%=subaction%>" />
<input type="hidden" name="cLocationID" value="<%=cLocationID%>" />
<input type="hidden" name="RequestType" value="<%=RequestType%>" />
<input type="hidden" name="RequestTypeID" value="2" />
<input type="hidden" name="RequestStatusID" value="<%=RequestStatusID%>" />
<input type="hidden" name="AmountAvailable" value="<%=AmountAvailable%>" />
<% if action = "cancel" then %>
	<input type="hidden" name="BatchID" value="<%=BatchID%>" />
<% else %>
	<input type="hidden" name="RequestID" value="<%=RequestID%>" />
<% end if %>
<% if RequestType <> "reservation" then %>
<input type="hidden" name="OrgUnitID" value="<%=OrgUnitID%>" />
<% end if %>
<input type="hidden" name="BatchAmountAvailable" value="<%=BatchAmountAvailable%>"/>
<input type="hidden" name="RequiredUOM" value=""/>
<table border="0">
	<tr><td colspan="2" align="center">
			<span class="GuiFeedback"><%=caption%></span><br><br>
	</td></tr>
	<% if action = "cancel" then %>
	<tr><td align="right" nowrap>
			<span class="required">Request ID:</span>
		</td><td>
			<input type="text" size="20" maxlength="50" name="RequestID" value="<%=RequestID%>" class="readOnly" readonly>
	</td></tr>
	<% else %>
	<tr><td align="right" nowrap>
			<span class="required">Batch ID:</span>
		</td><td>
			<input type="text" size="20" maxlength="50" name="BatchID" value="<%=BatchID%>" class="readOnly" readonly>
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
		<td>
			&nbsp;<%=GetBarcodeIcon()%>&nbsp;<%ShowLocationPicker "document.form1", "LocationID", "lpLocationBarCode", "lpLocationName", 10, 20, false%> 
		</td>
	</tr>
	<tr>
		<td align="right" valign="top" nowrap width="150">
			<span class="required">Date Required:</span>
		</td>
		<td>
			<%call ShowInputField("", "", "DateRequired:form1:" & DateRequired , "DATE_PICKER:TEXT", "15")%>
		</td>
	</tr>
	<tr>
		<%=ShowPickList("<span class=""required"">Reserved For:</span>", "OrgUnitID", OrgUnitID, "select distinct Org_Unit_ID AS Value, Org_Name AS DisplayText from inv_org_unit orgunit, inv_org_users orguser where orgunit.org_unit_id = orguser.org_unit_id_fk and orguser.user_id_fk='" & UserID & "'")%>
	</tr>
	<% if subaction = "partial" then %>
	<tr>
		<td align="right" nowrap><span class="required">Original Amount (<%=UOMAbbrv%>):</span></td>
		<td><input type="text" size="10" maxlength="50" name="QtyRequired" value="<%=QtyRequired%>" class="readOnly" readonly>
		<%=ShowSelectBox("UOM", unitstring, "Select UNIT_ID||'='||Unit_Abreviation AS Value, Unit_Name AS DisplayText FROM inv_units WHERE UNIT_ABREVIATION in (" & AvailabeUOM & ") ORDER BY lower(DisplayText) ASC") %>
		</td>
		
	</tr>
	<tr>
		<td align="right" nowrap><span class="required">Partial Amount (<%=UOMAbbrv%>):</span></td>
		<td><input type="text" size="10" maxlength="50" name="PartialQtyRequired" value><%=ShowSelectBox("UOM", unitstring, "Select UNIT_ID||'='||Unit_Abreviation AS Value, Unit_Name AS DisplayText FROM inv_units WHERE UNIT_ABREVIATION in (" & AvailabeUOM & ") ORDER BY lower(DisplayText) ASC") %></td>
	</tr>	
	<% elseif subaction = "convert" then %>
	<tr>
		<td align="right" nowrap><span class="required">Amount <%=RequestTextPast%> (<%=UOMAbbrv%>):</span></td>
		<td><input type="text" size="10" maxlength="50" name="QtyRequired" value="<%=QtyRequired%>" class="readOnly" readonly><%=ShowSelectBox("UOM", unitstring, "Select UNIT_ID||'='||Unit_Abreviation AS Value, Unit_Name AS DisplayText FROM inv_units WHERE UNIT_ABREVIATION in (" & AvailabeUOM & ") ORDER BY lower(DisplayText) ASC") %></td>
	</tr>
	<% else %>
	<tr>
		<td align="right" nowrap><span class="required">Amount <%=RequestTextPast%> (<%=UOMAbbrv%>):</span></td>
		<td><input type="text" size="10" maxlength="50" name="QtyRequired" value="<%=QtyRequired%>"><%=ShowSelectBox("UOM", unitstring, "Select UNIT_ID||'='||Unit_Abreviation AS Value, Unit_Name AS DisplayText FROM inv_units WHERE UNIT_ABREVIATION in (" & AvailabeUOM & ") ORDER BY lower(DisplayText) ASC") %></td>
	</tr>	
	<% end if %>
	<%if action <> "cancel" then%>
	<tr><td align="right" nowrap>
			<span class="required">Amount Available (<%=UOMAbbrv%>):</span>
		</td><td>
			<input type="text" size="10" maxlength="50" name="Amount_Available" value="<%=AmountAvailable%>" class="readOnly" readonly>
	</td></tr>
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
		Response.write(vbtab & "<td><input type=""text"" size=""30"" maxlength=""300"" name=""" & key & """ value=""" & FieldValue & """></td>" & vbcrlf)
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

