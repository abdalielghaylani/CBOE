<%@ Language=VBScript %>
<!--#INCLUDE VIRTUAL = "/cheminv/api/apiUtils.asp"-->
<!--#INCLUDE VIRTUAL = "/cheminv/gui/guiUtils.asp"-->
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/ado.inc"-->
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/display_func_vbs.asp"-->
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/utility_func_vbs.asp"-->
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/cs_security/cs_security_login_utils_vbs.asp"-->
<%
Dim Conn
Dim Cmd
Dim RS
action = Request("action")
multiSelect = Request("multiSelect")
editMode = Request("editMode")

if editMode = "single" then
	containerFieldName = Left(Request("containerFields"),inStrRev(Request("containerFields"),":")-1)
	pageTitle = "Change " & containerFieldName
	pageInstruction = "Please choose a new value for " & containerFieldName
	actionCancel = "window.close(); return false;"
else
	pageTitle = "Update Inventory Container"
	pageInstruction = "Enter values for the selected fields.<br>All selected fields are required."
	actionCancel = "history.back(); return false;"
end if

if Lcase(multiSelect) = "true" then 
	ContainerCount =  multiSelect_dict.count
	if ContainerCount = 0 then 	
		action = "noContainers"
	else
		ContainerID = DictionaryToList(multiSelect_dict)

		ContainerName =  ContainerCount & " containers will be deleted"
	end if
Else
	ContainerID = Request("ContainerID")
	if isEmpty(ContainerID) then ContainerID = Session("ContainerID")
	ContainerName = Request("ContainerName")
	if isEmpty(ContainerName) then ContainerName = Session("ContainerName")
End if

if ContainerID <> "" then
	'get the smallest qty_max and the greatest qty_remaining for validation purposes
	GetInvConnection()
	SQL = "SELECT min(qty_max) minQtyMax, max(qty_remaining) as maxQtyRemaining, min(CHEMINVDB2.RESERVATIONS.GETTOTALQTYRESERVED(container_id)) as TotalQtyReserved, CONTAINER_STATUS_ID_FK as containerStatus  FROM inv_containers WHERE container_id in (" & ContainerID & ") GROUP BY CONTAINER_STATUS_ID_FK"
	Set Cmd = GetCommand(Conn, SQL, adCmdText)
	Set RS = Cmd.Execute
	minQtyMax = RS("minQtyMax")
	maxQtyRemaining = RS("maxQtyRemaining")
	minQtyReserved = RS("TotalQtyReserved")
	'Response.Write request("action") & ":<BR>"
	'Response.Write request("containerFields") & ":<BR>"
	containerStatus = RS("containerStatus")
end if

%>
<html>
<head>
<title><%=Application("appTitle")%> -- <%=pageTitle%></title>

<!--#INCLUDE VIRTUAL = "/cheminv/gui/dateFormat_js.asp"-->
<script LANGUAGE="javascript" src="/cheminv/Choosecss.js"></script>
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/navbar_js.js"-->
<script LANGUAGE="javascript" src="/cheminv/gui/validation.js"></script>
<script LANGUAGE="javascript" src="/cheminv/utils.js"></script>
<script language="JavaScript">
<!--Hide JavaScript
	window.focus();
-->
</script>
<SCRIPT LANGUAGE="javascript">
	// Validates container attributes
	minQtyMax = <%=minQtyMax%>;
	maxQtyRemaining = <%=maxQtyRemaining%>;
	minQtyReserved = <%=minQtyReserved%>;
	function ValidateContainer(strMode){	
		var bWriteError = false;
		var bWriteWarning = false;
		var errmsg = "Please fix the following problems:\r\r";
		var warningmsg = "Please read the following warnings:\r\r";
		
		//alert(document.form1.containerFields.options.length);
		if (strMode.toLowerCase() == "edit") {
			if (document.form1.containerFields.selectedIndex == -1) {
				errmsg = errmsg + "- You must select at least one field to update.\r";
				bWriteError = true;
			}
		}
		else {
			fields = document.form1.fieldList.value.toString();
			//alert(document.form1.fieldList.value);
			//alert(fields);
			arrFields = fields.split(",");
			bRequiredFilled = true;
			for (i=0; i<arrFields.length; i++) {
				if (arrFields[i] != 'compound_id_fk') {
					if (bRequiredFilled && eval("document.form1.i" + arrFields[i] + ".value.length == 0")) {
						errmsg = errmsg + "- All fields are required\r";
						bWriteError = true;
						bRequiredFilled = false;
					}
				}
				switch (arrFields[i]) {
					case "compound_id_fk":
						// if present
						if (document.form1.iCompoundID.value.length == 0 && document.form1.RegBatchID.value.length == 0){
							errmsg = errmsg + "- Please select a Registry Batch ID or Compound ID.\r";
							bWriteError = true;
						}
						break;
					case "location_id_fk":
						// if present
						if (document.form1.ilocation_id_fk.value.length >0){
							//LocationID cannot be zero
							if (document.form1.ilocation_id_fk.value == 0){
								errmsg = errmsg + "- Cannot create container at the root location.\r";
								bWriteError = true;
							}
							// LocationID must be a positive number
							else if (!isPositiveNumber(document.form1.ilocation_id_fk.value)){
								errmsg = errmsg + "- Location ID must be a positive number.\r";
								bWriteError = true;
							}
						}
						break;
					case "qty_max":
						// if present 
						if (document.form1.iqty_max.value.length >0){
							// QtyMax must be a positive number
							if (!isPositiveNumber(document.form1.iqty_max.value)){
								errmsg = errmsg + "- Container size must be a positive number.\r";
								bWriteError = true;
							}
							// QtyMax cannot be 1 billion or greater
							else if (document.form1.iqty_max.value > 999999999){
								errmsg = errmsg + "- Container size is too large.\r";
								bWriteError = true;
							}
							// QtyMax cannot be less than the any of the qty_remaining values
							else if (!document.form1.iqty_remaining && document.form1.iqty_max.value < maxQtyRemaining){
								errmsg = errmsg + "- Container size cannot be smaller than the quantity remaining in any of the containers.\r";
								bWriteError = true;
							}
						}
						break;
					case "qty_initial":
						// if present
						if (document.form1.iqty_initial.value.length >0){
							// QtyInit must be a positive number
							if(!isWholeNumber(document.form1.iqty_initial.value)){
								errmsg = errmsg + "- Initial Quantity must be a whole number.\r";
								bWriteError = true;
							}
						}
						break;
					case "qty_remaining":
						// if present
						if (document.form1.iqty_remaining.value.length >0){
							// QtyRemaining must be a positive number
							if (!isWholeNumber(document.form1.iqty_remaining.value)){
								errmsg = errmsg + "- Quantity Remaining must be a whole number.\r";
								bWriteError = true;
							}
							// QtyRemaining can't be larger than the container size
							if (!document.form1.iqty_max && document.form1.iqty_remaining.value > minQtyMax){
								errmsg = errmsg + "- Quantity Remaining cannot be greater than the container size of any of the containers.\r";
								bWriteError = true;
							}
						}
						//If QtyMax and QtyRemaining are present, QtyRemaining cannot be larger than QtyMax
						if (document.form1.iqty_max && document.form1.iqty_remaining.value.length >0 && document.form1.iqty_max.value.length >0 &&  document.form1.iqty_remaining.value/1 > document.form1.iqty_max.value/1){
							errmsg = errmsg + "- Quantity Remaining cannot be larger than Container Size.\r";
							bWriteError = true;
						} 
						
						if (document.form1.iqty_remaining.value < minQtyReserved) {
							warningmsg = warningmsg + "- The Quantity Remaining is less than the quantity currently reserved for the selected containers.\rIf you choose 'OK', all active reservations will be removed from the containers.\rDo you want to proceed?\r";
							bWriteWarning = true;
						}
						
						break;
					case "qty_available":
						// if present
						if (document.form1.iqty_available.value.length >0){
							// QtyAvailable must be a positive number
							if (!isWholeNumber(document.form1.iqty_available.value)){
								errmsg = errmsg + "- Quantity Available must be a whole number.\r";
								bWriteError = true;
							}
						}
						break;
					case "qty_maxstock":
						// if present
						if (document.form1.iqty_maxstock.value.length >0){
							// MaxStockQty must be a positive number
							if (!isPositiveNumber(document.form1.iqty_maxstock.value)){
								errmsg = errmsg + "- Maximum stock threshold must be a positive number.\r";
								bWriteError = true;
							}
						}
						break;
					case "qty_minstock":
						// if present
						if (document.form1.iqty_minstock.value.length >0){
							// MinStockQty must be a positive number
							if (!isPositiveNumber(document.form1.iqty_minstock.value)){
								errmsg = errmsg + "- Minimum stock threshold must be a positive number.\r";
								bWriteError = true;
							}
						}
						break;
					case "weight":
						// if present
						if (document.form1.iweight.value.length >0){
							// Weight must be a positive number
							if (!isPositiveNumber(document.form1.iweight.value)){
								errmsg = errmsg + "- Weight must be a positive number.\r";
								bWriteError = true;
							}
						}
						break;
					case "tare_weight":
						// if present
						if (document.form1.itare_weight.value.length >0){
							// TareWeight must be a positive number
							if (!isPositiveNumber(document.form1.itare_weight.value)){
								errmsg = errmsg + "- Container Weight must be a positive number.\r";
								bWriteError = true;
							}
						}
						break;
					case "net_wght":
						// if present
						if (document.form1.inet_wght.value.length >0){
							// NetWeight must be a positive number
							if (!isPositiveNumber(document.form1.inet_wght.value)){
								errmsg = errmsg + "- Contents Weight must be a positive number.\r";
								bWriteError = true;
							}
						}
						break;
					case "final_wght":
						// if present
						if (document.form1.ifinal_wght.value.length >0){
							// FinalWeight must be a positive number
							if  (!isPositiveNumber(document.form1.ifinal_wght.value)){
								errmsg = errmsg + "- Final Weight must be a positive number.\r";
								bWriteError = true;
							}
						}
						break;
					case "purity":
						// Purity if present must be a positive number
						if (document.form1.ipurity.value.length >0 && !isPositiveNumber(document.form1.ipurity.value)){
							errmsg = errmsg + "- Purity must be a positive number.\r";
							bWriteError = true;
						}
						break;
					case "concentration":
						// Concentration if present must be a positive number
						if (document.form1.iconcentration.value.length >0 && !isPositiveNumber(document.form1.iconcentration.value)){
							errmsg = errmsg + "- Concentration must be a positive number.\r";
							bWriteError = true;
						}
						break;
					case "density":
						// Density if present must be a positive number
						if (document.form1.idensity.value.length >0 && !isPositiveNumber(document.form1.idensity.value)){
							errmsg = errmsg + "- Density must be a positive number.\r";
							bWriteError = true;
						}
						break;
					case "lot_num":
						// Lot Number if present must be a positive number
						if (document.form1.ilot_num.value.length >0 && !isPositiveNumber(document.form1.ilot_num.value)){
							errmsg = errmsg + "- Lot Number must be a positive number.\r";
							bWriteError = true;
						}
						break;
					case "po_number":
						// PO Number if present must be a positive number
						if (document.form1.ipo_number.value.length >0 && !isPositiveNumber(document.form1.ipo_number.value)){
							errmsg = errmsg + "- PO Number must be a positive number.\r";
							bWriteError = true;
						}
						break;
					case "req_number":
						// Req Number if present must be a positive number
						if (document.form1.ireq_number.value.length >0 && !isPositiveNumber(document.form1.ireq_number.value)){
							errmsg = errmsg + "- Req Number must be a positive number.\r";
							bWriteError = true;
						}
						break;
					case "po_line_number":
						// PO Line Number if present must be a positive number
						if (document.form1.ipo_line_number.value.length >0 && !isPositiveNumber(document.form1.ipo_line_number.value)){
							errmsg = errmsg + "- PO Line Number must be a positive number.\r";
							bWriteError = true;
						}
						break;
					case "date_produced":
						// Date Produced must be a date
						if (document.form1.idate_produced.value.length > 0 && !isDate(document.form1.idate_produced.value)){
							errmsg = errmsg + "- Date Produced must be in " + dateFormatString + " format.\r";
							bWriteError = true;
						}
						break;
					case "date_expires":
						// Expiration must be a date
						if (document.form1.idate_expires.value.length > 0 && !isDate(document.form1.idate_expires.value)){
							errmsg = errmsg + "- Expiration Date must be in " + dateFormatString + " format.\r";
							bWriteError = true;
						}
						break;
					case "date_ordered":
						// Date Ordered must be a date
						if (document.form1.idate_ordered.value.length > 0 && !isDate(document.form1.idate_ordered.value)){
							errmsg = errmsg + "- Date Ordered must be in " + dateFormatString + " format.\r";
							bWriteError = true;
						}
						break;
					case "date_received":
						// Data Received must be a date
						if (document.form1.idate_received.value.length > 0 && !isDate(document.form1.idate_received.value)){
							errmsg = errmsg + "- Date Received must be in " + dateFormatString + " format.\r";
							bWriteError = true;
						}
						break;
					case "container_cost":
						if (document.form1.icontainer_cost.value.length >0){
							// Cost must be a number
							if (!isPositiveNumber(document.form1.icontainer_cost.value)){
								errmsg = errmsg + "- Container cost must be a positive number.\r";
								bWriteError = true;
							}
							// Cost cannot equal or exceed 1billion
							else if (document.form1.icontainer_cost.value > 999999999){
								errmsg = errmsg + "- Container cost is too large.\r";
								bWriteError = true;
							}
						}
						break;

				//Custom field validation
				<%For each Key in custom_fields_dict
					If InStr(lcase(Key), "date") then%>
					case "<%=Key%>":
						if (document.form1.i<%=Key%>.value.length > 0 && !isDate(document.form1.i<%=Key%>.value)){
							errmsg = errmsg + "- <%=custom_fields_dict.Item(Key)%> must be in " + dateFormatString + " format.\r";
							bWriteError = true;
						}
						break;
					<%end if%>
				<%next%>					
					default:
						break;
				}
			}
		}
		// Report problems
		if (bWriteError){
			alert(errmsg);
			return false;
		}
		else{

			var bcontinue = true;
			
			// Report warnings, user can choose to accept or cancel
			bConfirmWarning = true;
			if (bWriteWarning) {
				bConfirmWarning = confirm(warningmsg);
			}
			if (!bConfirmWarning) var bcontinue = false;
			
			/*if ((document.form1.CompoundID.value.length == 0)&&(document.form1.RegID.value.length == 0)){
				bcontinue = confirm("No Compound has been asigned to this container.\rDo you really want to create a container without an associated chemical compound?");
			}*/
			if (bcontinue) document.form1.submit();
		}
	}
	 

</SCRIPT>
</head>
<body>
<center>

<%
if ContainerID = "" then
	Response.Write("<p>&nbsp;</p><span class=""GuiFeedback"">Please select Containers to update.</span><br /><br /><a href=""#"" onclick=""window.close(); return false;""><img SRC=""/cheminv/graphics/sq_btn/ok_dialog_btn.gif"" border=""0""></a>")
	Response.end
end if
%>
<%
if ContainerCount>cint(Application("MAX_UPDATEABLE_CONTAINERS")) then%>
<table border="0">
	<tr>
		<td colspan="2">
			<span class="GuiFeedback"><br><br><center>You cannot update more then <%=cint(Application("MAX_UPDATEABLE_CONTAINERS")) %> containers at a time.</center></span><br><br>
		</td>
	</tr>
	<tr>
		<td colspan="2" align="right">
			<a HREF="#" onclick="if (DialogWindow) {DialogWindow.close()}; window.close(); return false;"><img SRC="../graphics/ok_dialog_btn.gif" border="0" WIDTH="61" HEIGHT="21"></a>
		</td>
	</tr>
</table><%
 elseif action="edit" then
	call ShowSelectFieldsForm()
elseif action = "noContainers" then
	call ShowNoContainersError()
else
	call ShowFieldsForm()
end if%>
</center>
</body>
</html>

<%sub ShowSelectFieldsForm()%>
<form name="form1" action="UpdateContainer.asp" method="POST">
<input Type="hidden" name="iContainerIDs" value="<%=ContainerID%>">
<input Type="hidden" name="multiscan" value="<%=Request("multiscan")%>">
<INPUT TYPE="hidden" NAME="multiSelect" VALUE="<%=multiSelect%>">
<INPUT TYPE="hidden" NAME="action" VALUE="dataEntry">
<table border="0">
	<tr>
		<td colspan="2">
			<span class="GuiFeedback"><br><br><center>Select the fields you wish to update.</center></span><br><br>
		</td>
	</tr>
	<tr>
		<td align="right" valign="top" nowrap>Container Fields:</td>
		<td>		
			<SELECT NAME="containerFields" SIZE="8" MULTIPLE>
				<OPTION VALUE="Catalog Number:supplier_catnum">Catalog Number</OPTION>
				<OPTION VALUE="Comments:container_comments">Comments</OPTION>
				<OPTION VALUE="Compound:compound_id_fk">Compound</OPTION>
				<OPTION VALUE="Concentration:concentration">Concentration</OPTION>
				<OPTION VALUE="Container Cost:container_cost">Container Cost</OPTION>
				<OPTION VALUE="Container Name:container_name">Container Name</OPTION>
				<OPTION VALUE="Container Size:qty_max">Container Size</OPTION>
				<OPTION VALUE="Container Status:container_status_id_fk">Container Status</OPTION>
				<OPTION VALUE="Container Type:container_type_id_fk">Container Type</OPTION>
				<OPTION VALUE="Current User:current_user_id_fk">Current User</OPTION>
				<OPTION VALUE="Date Ordered:date_ordered">Date Ordered</OPTION>
				<OPTION VALUE="Date Produced:date_produced">Date Produced</OPTION>
				<OPTION VALUE="Date Received:date_received">Date Received</OPTION>
				<OPTION VALUE="Default Location:def_location_id_fk">Default Location</OPTION>
				<OPTION VALUE="Density:density">Density</OPTION>
				<OPTION VALUE="Expiration Date:date_expires">Expiration Date</OPTION>
				<OPTION VALUE="Final Weight:final_wght">Final Weight</OPTION>
				<OPTION VALUE="Grade:grade">Grade</OPTION>
				<OPTION VALUE="Initial Quantity:qty_initial">Initial Quantity</OPTION>
				<OPTION VALUE="Location:location_id_fk">Location</OPTION>
				<OPTION VALUE="Lot Number:lot_num">Lot Number</OPTION>
				<OPTION VALUE="Ordered By:ordered_by_id_fk">Ordered By</OPTION>
				<OPTION VALUE="Max stock thresh:qty_maxstock">Max stock thresh</OPTION>
				<OPTION VALUE="Min stock thresh:qty_minstock">Min stock thresh</OPTION>
				<OPTION VALUE="Net Weight:net_wght">Net Weight</OPTION>
				<OPTION VALUE="Owner:owner_id_fk">Owner</OPTION>
				<OPTION VALUE="Physical State:physical_state_id_fk">Physical State</OPTION>
				<OPTION VALUE="PO Line Number:po_line_number">PO Line Number</OPTION>
				<OPTION VALUE="PO Number:po_number">PO Number</OPTION>
				<OPTION VALUE="Purity:purity">Purity</OPTION>
				<OPTION VALUE="Quantity Remaining:qty_remaining">Quantity Remaining</OPTION>
				<OPTION VALUE="Received By:received_by_id_fk">Received By</OPTION>
				<OPTION VALUE="Req Number:req_number">Req Number</OPTION>
				<OPTION VALUE="Solvent:solvent_id_fk">Solvent</OPTION>
				<OPTION VALUE="Supplier:supplier_id_fk">Supplier</OPTION>
				<OPTION VALUE="Tare Weight:tare_weight">Tare Weight</OPTION>
				<OPTION VALUE="Weight:weight">Weight</OPTION>
				<OPTION VALUE="Unit of Measure:unit_of_meas_id_fk">Unit of Measure</OPTION>
				<OPTION VALUE="Unit of Weight:unit_of_wght_id_fk">Unit of Weight</OPTION>
<%
				For each key in custom_fields_dict
					theOption = "<OPTION VALUE=""" & custom_fields_dict.Item(key) & ":" & Key & """>" & custom_fields_dict.Item(key) & "</OPTION>" & req_custom_fields_dict.Exists(Key)
					Response.Write theOption
				Next 
%>				
			</SELECT>
		</td>
	</tr>
	<tr>
		<td colspan="2" align="right"> 
			<a HREF="#" onclick="window.close(); return false;"><img SRC="../graphics/cancel_dialog_btn.gif" border="0" WIDTH="61" HEIGHT="21"></a><a HREF="#" onclick="ValidateContainer('Edit'); return false;"><img SRC="../graphics/ok_dialog_btn.gif" border="0" WIDTH="61" HEIGHT="21"></a>
		</td>
	</tr>	
</table>	
</form>
<%end sub%>

<%sub ShowFieldsForm()%>

<%
fields = Request("containerFields")
arrFields = split(fields,",")

fieldColumns = ""
for i = 0 to ubound(arrFields)
	fieldColumns = fieldColumns & Right(arrFields(i),len(arrFields(i))-instrrev(arrFields(i),":")) & ","
next
fieldColumns = left(fieldColumns, len(fieldColumns)-1)

'for i = 0 to ubound(arrFields)
'	Response.Write "field=" & arrFields(i) & "<BR>"
'	Response.Write "type=" & arrTypes(i) & "<BR>"
'next
%>
<form name="form1" action="UpdateContainer_action.asp" method="POST">
<input Type="hidden" name="iContainerIDs" value="<%=ContainerID%>">
<input Type="hidden" name="multiscan" value="<%=Request("multiscan")%>">
<INPUT TYPE="hidden" NAME="multiSelect" VALUE="<%=multiSelect%>">
<INPUT TYPE="hidden" NAME="fieldList" VALUE="<%=fieldColumns%>">

<table border="0">
	<tr>
		<td colspan="2">
			<span class="GuiFeedback"><br><br><center><%=pageInstruction%></center></span><br><br>
		</td>
	</tr>
<%
for i = 0 to ubound(arrFields)
	fieldName = trim(arrFields(i))
	'Response.Write right(fieldName,2) & ":<BR>"
	if right(fieldName,2) = "fk" then
		Select case fieldName
			Case "Location:location_id_fk"
				Response.Write "<TR><TD align=""right"" valign=""top"" width=""150"" nowrap>Location ID:</TD><TD VALIGN=""top"">" &GetBarcodeIcon() & "&nbsp;"
				call ShowLocationPicker("document.form1", "ilocation_id_fk", "lpLocationBarCode", "lpLocationName", 10, 25, false)
				Response.Write "</TD></TR>"
			Case "Compound:compound_id_fk"
				Session("bManageMode") = false
				theRow = "<TR><TD ALIGN=""center"" VALIGN=""top"" COLSPAN=""2"">"
				if Session("INV_MANAGE_SUBSTANCES" & "cheminv") then
					theRow = theRow & "<a class=""MenuLink"" href=""Create%20a%20new%20inventory%20substance%20and%20assign%20it%20to%20this%20container"" onclick=""OpenDialog('/cheminv/gui/CreateOrEditSubstance.asp', 'SubsManager', 2); return false;"" title=""Create a new inventory substance and assign it to this container"">New Substance</a>&nbsp;|&nbsp;"
				end if
				
				theRow = theRow & "<a class=""MenuLink"" HREF=""Select%20an%20existing%20substance%20and%20assign%20it%20to%20this%20container"" target=""SubstancesFormGroup"" onclick=""OpenDialog('/cheminv/inputtoggle.asp?formgroup=global_substanceselect_form_group&amp;dataaction=db&amp;dbname=cheminv&amp;showbanner=false', 'SubsManager', 2); return false;"" title=""Select an existing substance and assign it to this container"">Select Substance</a></TD></TR>"
				if Application("RegServerName") <> "NULL" then
				theRow = theRow &"<tr>" & vbcrlf
				theRow = theRow & 	"<td align=""right"">Registry Batch ID:</td><td><input type=""text"" Size=""15"" name=""RegBatchID"" value="""" onchange=""GetRegIDFromRegNum(this.value); iCompoundID.value=''; NewCompoundID.value='';""></td>" & vbcrlf
				theRow = theRow & 	"<input type=""hidden"" name=""iRegID"" value="""">" & vbcrlf
				theRow = theRow & 	"<input type=""hidden"" name=""iBatchNumber"" value="""">" & vbcrlf
				theRow = theRow & 	"<input type=""hidden"" name=""NewRegID"">" & vbcrlf
				theRow = theRow & 	"<input type=""hidden"" name=""NewBatchNumber"">" & vbcrlf
				theRow = theRow & "</tr>" & vbcrlf
				end if
				theRow = theRow & "<TR><TD align=""right"" valign=""top"" width=""150"" nowrap>Compound ID:</TD><TD VALIGN=""top"">"
				theRow = theRow & "<input type=""text"" Size=""15"" name=""iCompoundID"" OnChange=""if(IsValidCompoundID(this.value, true)==1){NewCompoundID.value=this.value;icompound_id_fk.value= this.value; RegBatchID.value=''; iRegID.value=''; iBatchNumber.value='';}else{if (this.value !=''){this.value = '';}}""><input Type=""hidden"" name=""icompound_id_fk""><input Type=""hidden"" name=""iContainerName""><input type=""hidden"" name=""NewCompoundID"">"
				theRow = theRow & "<input TYPE=""hidden"" NAME=""tempCsUserName"" Value=""" & Session("UserName" & "cheminv") & """>"
				theRow = theRow & "<input TYPE=""hidden"" NAME=""tempCsUserID"" Value=""" & Server.URLEncode(CryptVBS(Session("UserID" & "cheminv"),"ChemInv\API\GetBatchInfo.asp")) & """></TD></TR>"
				Response.Write theRow
			Case "Solvent:solvent_id_fk"
				Response.Write("<tr height=""25""><td align=""right"" nowrap>Select Solvent:</td><td>")
				Response.Write(ShowSelectBox2("isolvent_id_fk",solvent_id_fk,"SELECT solvent_id AS Value, solvent_name AS DisplayText FROM " & Application("CHEMINV_USERNAME") & ".inv_solvents ORDER BY lower(DisplayText) ASC", null, "Select a Solvent", "null"))
				Response.Write("</td></tr>")
			Case else	
				call CreateSelectFieldRow(arrFields(i))
		end Select
	else
		Select case fieldName
			Case "Comments:container_comments"
				theRow = "<TR HEIGHT=""150""><TD ALIGN=""right"" VALIGN=""top"" NOWRAP>Comments:</TD>"
				theRow = theRow & "<TD VALIGN=""top""><TEXTAREA ROWS=""8"" COLS=""60"" NAME=""icontainer_comments"" WRAP=""hard""></TEXTAREA></TD></TR>"
				Response.Write theRow
			Case else
				call CreateTextFieldRow(arrFields(i))
		end select
	end if
next

%>
	<tr>
		<td colspan="2" align="right"> 
			<a HREF="#" onclick="<%=actionCancel%>"><img SRC="../graphics/cancel_dialog_btn.gif" border="0" WIDTH="61" HEIGHT="21"></a>
			<a HREF="#" onclick="ValidateContainer('Submit'); return false;"><img SRC="../graphics/ok_dialog_btn.gif" border="0" WIDTH="61" HEIGHT="21"></a>
		</td>
	</tr>	
</table>	
</form>

<%end sub%>

<%sub ShowNoContainersError()%>
<table border="0">
	<tr>
		<td colspan="2">
			<span class="GuiFeedback"><br><br><center>You must select containers to update.</center></span><br><br>
		</td>
	</tr>
	<tr>
		<td colspan="2" align="right"> 
			<a HREF="#" onclick="window.close(); return false;"><img SRC="../graphics/cancel_dialog_btn.gif" border="0" WIDTH="61" HEIGHT="21"></a>
		</td>
	</tr>	
</table>	

<%end sub%>

<%sub CreateTextFieldRow(field)

	fieldName = Left(field,instr(field,":"))
	fieldColumn = Right(field,len(field)-instrrev(field,":"))
	Response.Write "<TR>"
	Select Case lcase(fieldColumn)
		case "date_expires", "date_ordered", "date_received", "date_produced", "date_1", "date_2", "date_3", "date_4", "date_5"
			theRow = "<TD ALIGN=""right"" VALIGN=""top"" WIDTH=""150"" NOWRAP>" & fieldName & "</TD>"
			theRow = theRow & "<TD>" & getShowInputField("", "", "i" & fieldColumn & ":form1", "DATE_PICKER:TEXT", "15") & "</TD>"
			'<TD><INPUT TYPE=""text"" NAME=""i" & fieldColumn & """ SIZE=""15"" VALUE=""""><A HREF ONCLICK=""return PopUpDate(&quot;i" & fieldColumn & "&quot;,&quot;/cheminv/gui/month_view1.asp&quot;)""><IMG SRC=""/cfserverasp/source/graphics/navbuttons/icon_mview.gif"" HEIGHT=""16"" WIDTH=""16"" BORDER=""0""></A></TD>"
			'theRow = theRow & "<a href onclick=""return PopUpDate(&quot;" & fieldColumn & "&quot;,&quot;/cheminv/gui/month_view1.asp&quot;)""><img src=""/cfserverasp/source/graphics/navbuttons/icon_mview.gif"" height=""16"" width=""16"" border=""0""></a>"
		case "purity"
			theRow = ShowInputBox(fieldName, fieldColumn, 15, ShowSelectBox("iunit_of_purity_id_fk", Session("UOPIDOptionValue"),"SELECT UNIT_ID||'='||Unit_Abreviation AS Value, Unit_Name AS DisplayText FROM inv_units WHERE Unit_type_id_FK = 3 ORDER BY Unit_Name ASC"), False, False)
		case "concentration"
			theRow = ShowInputBox(fieldName, fieldColumn, 15, ShowSelectBox("iunit_of_conc_id_fk", Session("UOCIDOptionValue"),"SELECT UNIT_ID||'='||Unit_Abreviation AS Value, Unit_Name AS DisplayText FROM inv_units WHERE Unit_type_id_FK in (3,6) ORDER BY Unit_Name ASC"), False, False)
		case "density"
			theRow = ShowInputBox(fieldName, fieldColumn, 15, ShowSelectBox("iunit_of_density_id_fk", Session("UODIDOptionValue"),"SELECT UNIT_ID||'='||Unit_Abreviation AS Value, Unit_Name AS DisplayText FROM inv_units WHERE Unit_type_id_FK = 6 ORDER BY Unit_Name ASC"), False, False)
		case "container_cost"
			theRow = ShowInputBox(fieldName, fieldColumn, 15, ShowSelectBox("iunit_of_cost_id_fk", Session("UOCostIDOptionValue"),"SELECT UNIT_ID||'='||Unit_Abreviation AS Value, Unit_Name AS DisplayText FROM inv_units WHERE Unit_type_id_FK = 5 ORDER BY Unit_Name ASC"), False, False)
		case else
			if custom_lists_dict.Exists("CUSTOM_FIELDS." & fieldColumn) then
				theRow = "<TD align=""right"" valign=""top"" width=""150"">" & fieldName & "</TD><TD><SELECT name=""i" & fieldColumn & """ size=""1"">"
				'-- build the select from the custom list
				theRow = theRow & GetCustomListOptions("CUSTOM_FIELDS", fieldColumn, eval(fieldColumn), null)	
				theRow = theRow & "</TD>"
			else
				theRow = ShowInputBox(fieldName, fieldColumn, 15, "", False, false)
			end if
	end select
	'Response.Write field & "<BR>"
	'Response.Write Left(field,instr(field,":")-1) & "<BR>"
	'Response.Write Right(field,len(field)-instrrev(field,":")) & "<BR>"
	'Response.Write len(field)-instrrev(field,":") & "<BR>"

	Response.Write theRow
    Response.Write "</TR>"
end sub

sub CreateSelectFieldRow(field)

	fieldName = Left(field,instr(field,":"))
	fieldColumn = Right(field,len(field)-instrrev(field,":"))
	Response.Write "<TR>"
	Select Case fieldColumn
		Case "location_id_fk"
			'sql = "SELECT location_id AS Value, location_name AS DisplayText FROM inv_locations ORDER BY lower(DisplayText) ASC"	
		Case "compound_id_fk"
			sql = "SELECT compound_id AS Value, substance_name AS DisplayText FROM inv_compounds ORDER BY lower(DisplayText) ASC"	
		Case "container_type_id_fk"
			sql = "SELECT Container_Type_ID AS Value, Container_Type_Name AS DisplayText FROM inv_Container_Types ORDER BY lower(DisplayText) ASC"	
		Case "unit_of_meas_id_fk"
			sql = "SELECT UNIT_ID||'='||Unit_Abreviation AS Value, Unit_Name AS DisplayText FROM inv_units WHERE Unit_type_id_FK IN (1,2,4) ORDER BY lower(DisplayText) ASC"	
		Case "unit_of_wght_id_fk"
			sql = "SELECT UNIT_ID||'='||Unit_Abreviation AS Value, Unit_Name AS DisplayText FROM inv_units WHERE Unit_type_id_FK IN (2) ORDER BY lower(DisplayText) ASC"	
		Case "unit_of_purity_id_fk"
			sql = "SELECT UNIT_ID||'='||Unit_Abreviation AS Value, Unit_Name AS DisplayText FROM inv_units WHERE Unit_type_id_FK = 3 ORDER BY Unit_Name ASC"	
		Case "owner_id_fk"
			sql = "SELECT owner_ID AS Value, description AS DisplayText FROM " & Application("CHEMINV_USERNAME") & ".inv_owners ORDER BY lower(description) ASC"	
		Case "ordered_by_id_fk"
			sql = "SELECT Upper(User_ID) AS Value, Last_Name||DECODE(First_Name, NULL, '', ', '||First_Name) AS DisplayText FROM CS_SECURITY.People ORDER BY lower(Last_Name) ASC"	
		Case "container_status_id_fk"
			sql = "SELECT Container_Status_ID AS Value, Container_Status_Name AS DisplayText FROM inv_Container_Status ORDER BY lower(DisplayText) ASC"	
		Case "received_by_id_fk"
			sql = "SELECT Upper(User_ID) AS Value, Last_Name||DECODE(First_Name, NULL, '', ', '||First_Name) AS DisplayText FROM CS_SECURITY.People ORDER BY lower(Last_Name) ASC"	
		Case "physical_state_id_fk"
			sql = "SELECT Physical_State_ID AS Value, Physical_State_Name AS DisplayText FROM inv_physical_state ORDER BY Physical_State_Name ASC"	
		Case "current_user_id_fk"
			sql = "SELECT Upper(User_ID) AS Value, Last_Name||DECODE(First_Name, NULL, '', ', '||First_Name) AS DisplayText FROM CS_SECURITY.People ORDER BY lower(Last_Name) ASC"	
		Case "supplier_id_fk"
			sql = "SELECT Supplier_ID AS Value, Supplier_Short_Name AS DisplayText FROM inv_suppliers ORDER BY lower(Supplier_Short_Name) ASC"	
		Case "def_location_id_fk"
			sql = "SELECT location_id AS Value, location_name AS DisplayText FROM inv_locations ORDER BY lower(DisplayText) ASC"	
		Case "unit_of_density_id_fk"
			sql = "SELECT UNIT_ID||'='||Unit_Abreviation AS Value, Unit_Name AS DisplayText FROM inv_units WHERE Unit_type_id_FK = 6 ORDER BY Unit_Name ASC"	
		Case else

	end select
    if (fieldColumn = "container_status_id_fk") then
        theRow = ShowPickList(fieldName, "i" & fieldColumn, containerStatus, sql)
	else
	    theRow = ShowPickList(fieldName, "i" & fieldColumn, "", sql)
	end if 
	Response.Write "</TR>"
	Response.Write theRow

end sub
%>
