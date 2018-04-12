<%
'Prevent page from being cached
Response.ExpiresAbsolute = Now()
Session("bManageMode") = false

Dim Conn
PlateID = Request("PlateID")
isEdit = Request("isEdit")
sTab = Request("sTab")
refresh = Request("refresh")

if refresh then
	'edit well values are refreshed
	Session("plwQty_Remaining") = ""
	Session("plwQty_Unit_FK") = ""
	Session("plwWeight") = ""
	Session("plwWeight_Unit_FK") = ""
	Session("plwSolventIDFK") = ""
	Session("plwConcentration") = ""
	Session("plwConc_Unit_FK") = ""
	Session("plwSolvent_Volume") = ""
	Session("plwSolvent_Volume_Unit_ID_FK") = ""
	Session("plwSolution_Volume") = ""
end if

%>
<!--#INCLUDE VIRTUAL = "/cheminv/api/apiUtils.asp"-->
<!--#INCLUDE VIRTUAL = "/cheminv/gui/guiUtils.asp"-->
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/utility_func_vbs.asp"-->
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/display_func_vbs.asp"-->
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/search_func_vbs.asp"-->
<!--#INCLUDE VIRTUAL = "/cheminv/gui/GetPlateAttributes.asp"-->

<html>
<head>
<style>
		A {Text-Decoration: none;}
		.TabView {color:#000000; font-size:8pt; font-family: arial}
		A.TabView:LINK {Text-Decoration: none; color:#000000; font-size:8pt; font-family: arial}
		A.TabView:VISITED {Text-Decoration: none; color:#000000; font-size:8pt; font-family: arial}
		A.TabView:HOVER {Text-Decoration: underline; color:#4682b4; font-size:8pt; font-family: arial}
</style>
<title><%=Application("appTitle")%> -- Create or Edit an Inventory Plate</title>
<!--#INCLUDE VIRTUAL = "/cheminv/gui/dateFormat_js.asp"-->
<script LANGUAGE="javascript" src="/cheminv/Choosecss.js"></script>
<script LANGUAGE="javascript" src="/cheminv/utils.js"></script>
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/navbar_js.js"-->
<script LANGUAGE="javascript" src="/cheminv/gui/validation.js"></script>
<script language="JavaScript">
<!--Hide JavaScript
	var DialogWindow;
	window.focus();
	var cd_plugin_threshold= <%=Application("CD_PLUGIN_THRESHOLD")%>;

	function IsPlateTypeAllowed(LocationID, PlateTypeID, IsPlateMap){
		var strURL = "http://" + serverName + "/cheminv/api/IsPlateTypeAllowed.asp?LocationID=" + LocationID + "&PlateTypeID=" + PlateTypeID + "&IsPlateMap=" + IsPlateMap;	
		var httpResponse = JsHTTPGet(strURL)	
		/*
		alert(strURL);
		alert(httpResponse);
		httpResponse = 0;
		*/
		return httpResponse;
	}    

	// Validates plate attributes
	function ValidatePlate(strMode){	
		var bWriteError = false;
		var errmsg = "Please fix the following problems:\r";
		
		LocationID = document.form1.iLocation_ID_FK.value;
		PlateTypeID = document.form1.iPlate_Type_ID_FK.value;
		IsPlateMap = document.form1.iIs_Plate_Map.value;
		if (IsPlateMap == "")
			IsPlateMap = 0; 
		
		//LocationID is required
		if (document.form1.iLocation_ID_FK.value.length == 0) {
			errmsg = errmsg + "- Location ID is required.\r";
			bWriteError = true;
		}
		else{
			// LocationID can't be the root
			if (document.form1.iLocation_ID_FK.value == 0){
				errmsg = errmsg + "- Cannot create container at the root location.\r";
				bWriteError = true;
			}
			// LocationID must be a positive number
			if (!isPositiveNumber(document.form1.iLocation_ID_FK.value)&&(document.form1.iLocation_ID_FK.value != 0)){
				errmsg = errmsg + "- Location ID must be a positive number.\r";
				bWriteError = true;
			}
		}	
<%if NOT isEdit then%>
		<%if lcase(Application("RequireBarcode")) = "true" then%>
		//barcode description or Plate Barcode is required
		if (document.form1.AutoGen.value == "true") {
			if(document.form1.iBarcode_Desc_ID.value.length == 0) {
				errmsg = errmsg + "- Barcode Description is required.\r";
				bWriteError = true;
			}
		}
		else {
			if(document.form1.iPlate_Barcode.value.length == 0) {
				errmsg = errmsg + "- Plate Barcode is required.\r";
				bWriteError = true;
			}
		}		
		<% end if%>

		//check for duplicate barcodes
		if (document.form1.iPlate_Barcode.value.length > 0){
			var strURL = "http://" + serverName + "/cheminv/api/CheckDuplicateBarcode.asp?Barcodes=" + document.form1.iPlate_Barcode.value + "&BarcodeType=plate";	
			var httpResponse = JsHTTPGet(strURL) 
			if (httpResponse.length > 0) {
				errmsg = errmsg + "- Barcode conflict for barcode: " + httpResponse + " .\r"
				bWriteError = true;
			}
		}
		
		// Initial amount is required
		if (document.form1.iQty_Initial.value.length == 0) {
			errmsg = errmsg + "- Initial amount is required.\r";
			bWriteError = true;
		}
		else{
			// Initial amount must be a number
			if (!isWholeNumber(document.form1.iQty_Initial.value)){
				errmsg = errmsg + "- Initial amount must be zero or greater.\r";
				bWriteError = true;
			}
			if (document.form1.iQty_Initial.value > 999999999){
				errmsg = errmsg + "- Initial amount is too large.\r";
				bWriteError = true;
			}
		}
		// set qty_remaining = qty_initial
		document.form1.iQty_Remaining.value = document.form1.iQty_Initial.value;
		
		// NumCopies is required
		if (document.form1.iNumCopies.value.length == 0) {
			errmsg = errmsg + "- Number of Copies is required.\r";
			bWriteError = true;
		}
		else{
			// NumCopies must be a number
			if (!isPositiveNumber(document.form1.iNumCopies.value)){
				errmsg = errmsg + "- Number of copies must be a positive number.\r";
				bWriteError = true;
			}
			else {
				if (document.form1.iNumCopies.value > 399){
					errmsg = errmsg + "- Number of copies must be less than 400.\r";
					bWriteError = true;	
				}
			}
		}
<%else%>
		// Plate ID is required
		if(document.form1.iPlate_Barcode.value.length == 0) {
			errmsg = errmsg + "- Plate Barcode is required.\r";
			bWriteError = true;
		}

		//check for duplicate barcodes
		if (document.form1.iPlate_Barcode.value.length > 0 && document.form1.iPlate_Barcode.value != '<%=Plate_Barcode%>'){
			var strURL = "http://" + serverName + "/cheminv/api/CheckDuplicateBarcode.asp?Barcodes=" + document.form1.iPlate_Barcode.value + "&BarcodeType=plate";	
			var httpResponse = JsHTTPGet(strURL) 
			if (httpResponse.length > 0) {
				errmsg = errmsg + "- Barcode conflict for barcode: " + httpResponse + " .\r"
				bWriteError = true;
			}
		}

		// QtyRemaining is required
		if (document.form1.iQty_Remaining.value.length == 0) {
			errmsg = errmsg + "- Quantity Remaining is required.\r";
			bWriteError = true;
		}
		else{
			// QtyRemaining must be a number
			if (!isWholeNumber(document.form1.iQty_Remaining.value)){
			errmsg = errmsg + "- Quantity Remaining must be zero or greater.\r";
			bWriteError = true;
			}
			if (document.form1.iQty_Remaining.value > 999999999){
				errmsg = errmsg + "- Quantity Remaining is too large.\r";
				bWriteError = true;
			}
		}
<% end if%>


/*		
		// Plate Barcode OR Plate Barcode Desc ID is required
		if (document.form1.bUseBarcodeDesc == "true"){
			//Barcode Desc ID is required
			if (document.form1.iBarcode_Desc_ID.value.length == 0) {
				errmsg = errmsg + "- Barcode Type is required.\r";
				bWriteError = true;
			}
			
		}
		else {
			//Plate Barcode Desc ID is required
			if (document.form1.iPlate_Barcode.value.length == 0) {
				errmsg = errmsg + "- Plate Barcode is required.\r";
				bWriteError = true;
			}
		}
*/		
		// Plate Name if present can't be too long
		if (document.form1.iPlate_Name.value.length >0 && document.form1.iPlate_Name.value.length > 50) {
			errmsg = errmsg + "- Plate Name is too long.\r";
			bWriteError = true;
		}

		// Supplier Shipment Code if present can't be too long
		if (document.form1.iSupplier_Shipment_Code.value.length >0 && document.form1.iSupplier_Shipment_Code.value.length > 50){
			errmsg = errmsg + "- Supplier Shipment Code is too long.\r";
			bWriteError = true;
		}

		// Supplier Shipment Number if present must be a number
		if (document.form1.iSupplier_Shipment_Number.value.length >0 && !isPositiveNumber(document.form1.iSupplier_Shipment_Number.value)){
			errmsg = errmsg + "- Supplier Shipment Number must be a positive number.\r";
			bWriteError = true;
		}
		if (document.form1.iSupplier_Shipment_Number.value.length >0 && document.form1.iSupplier_Shipment_Number.value > 999999999){
			errmsg = errmsg + "- Supplier Shipment Number is too large.\r";
			bWriteError = true;
		}

		// Group Name if present can't be too long
		if (document.form1.iGroup_Name.value.length >0 && document.form1.iGroup_Name.value.length > 50){
			errmsg = errmsg + "- Group Name is too long.\r";
			bWriteError = true;
		}

		// FT Cycles if present must be a number
		if (document.form1.iFT_Cycles.value.length >0){
			if (!isNumber(document.form1.iFT_Cycles.value)){
				errmsg = errmsg + "- FT Cycles must be a number.\r";
				bWriteError = true;
			}
			else if (document.form1.iFT_Cycles.value > 9999){
				errmsg = errmsg + "- FT Cycles must be less than 10000.\r";
				bWriteError = true;
			}
		}

		// Weight if present must be a number
		if (document.form1.iWeight.value.length >0 && !isWholeNumber(document.form1.iWeight.value)){
			errmsg = errmsg + "- Weight must be zero or greater.\r";
			bWriteError = true;
		}
		if (document.form1.iWeight.value.length >0 && document.form1.iWeight.value > 999999999){
			errmsg = errmsg + "- Weight is too large.\r";
			bWriteError = true;
		}

		// Concentration if present must be a number
		if (document.form1.iConcentration.value.length >0 && !isWholeNumber(document.form1.iConcentration.value)){
			errmsg = errmsg + "- Concentration must be zero or greater.\r";
			bWriteError = true;
		}
		if (document.form1.iConcentration.value.length >0 && document.form1.iConcentration.value > 999999999){
			errmsg = errmsg + "- Concentration is too large.\r";
			bWriteError = true;
		}

		// Solvent Volume if present must be a number
		if (document.form1.iSolvent_Volume.value.length >0 && !isWholeNumber(document.form1.iSolvent_Volume.value)){
			errmsg = errmsg + "- Solvent Volume must be zero or greater.\r";
			bWriteError = true;
		}
		if (document.form1.iSolvent_Volume.value.length >0 && document.form1.iSolvent_Volume.value > 999999999){
			errmsg = errmsg + "- Solvent Volume is too large.\r";
			bWriteError = true;
		}
		// Solution Volume if present must be a number
		if (document.form1.iSolution_Volume.value.length >0 && !isWholeNumber(document.form1.iSolution_Volume.value)){
			errmsg = errmsg + "- Solution Volume must be zero or greater.\r";
			bWriteError = true;
		}
		if (document.form1.iSolution_Volume.value.length >0 && document.form1.iSolution_Volume.value > 999999999){
			errmsg = errmsg + "- Solution Volume is too large.\r";
			bWriteError = true;
		}
		
		// Supplier Shipment Date must be a date
		if (document.form1.iSupplier_Shipment_Date.value.length > 0 && !isDate(document.form1.iSupplier_Shipment_Date.value)){
			errmsg = errmsg + "- Supplier Shipment Date must be in " + dateFormatString + " format.\r";
			bWriteError = true;
		}
		
		//Custom field validation
		<%For each Key in custom_plate_fields_dict
			response.write "//" & key
			If InStr(lcase(Key), "date_") then%>
				if (document.form1.i<%=Key%>.value.length > 0 && !isDate(document.form1.i<%=Key%>.value)){
					errmsg = errmsg + "- <%=custom_plate_fields_dict.Item(Key)%> must be in " + dateFormatString + " format.\r";
					bWriteError = true;
				}
			<%end if%>
		<%next%>
		
		//Validate required custom fields
		<% For each Key in req_custom_plate_fields_dict%>
			if (document.form1.i<%=Key%>.value.length == 0) {
				errmsg = errmsg + "- <%=req_custom_plate_fields_dict.Item(Key)%> is required.\r";
				bWriteError = true;
			}
		<%Next%>

		// LocationID must be valid for this plate type
		if (!(IsPlateTypeAllowed(LocationID,PlateTypeID,IsPlateMap) == 1)){
			errmsg = errmsg + "- This location does not accept this plate type.\r";
			bWriteError = true;
		}

		// Report problems
		if (bWriteError){
			alert(errmsg);
		}
		else{
			if (strMode.toLowerCase() == "edit"){
				document.form1.action = "EditPlate_action.asp";
			}
			else{
				document.form1.action = "CreatePlate_action.asp";
			}
			var bcontinue = true;
			
			if (bcontinue) document.form1.submit();
		}
	}
	// Validates well attributes
	function ValidateWell(strMode){	

		var bWriteError = false;
		var errmsg = "Please fix the following problems:\r";
		// Qty Remaining if present must be a number or "null"
		if (document.form1.iwQty_Remaining.value.length >0 && !isWholeNumber(document.form1.iwQty_Remaining.value) && document.form1.iwQty_Remaining.value.toLowerCase() != "null"){
			errmsg = errmsg + "- Qty Remaining must be zero or greater.\r";
			bWriteError = true;
		}	 
		if (document.form1.iwQty_Remaining.value.length >0 && document.form1.iwQty_Remaining.value > 999999999){
			errmsg = errmsg + "- Qty Remaining is too large.\r";
			bWriteError = true;
		}	 

		// Weight if present must be a number
		if (document.form1.iwWeight.value.length >0 && !isWholeNumber(document.form1.iwWeight.value) && document.form1.iwWeight.value.toLowerCase() != "null"){
			errmsg = errmsg + "- Weight must be zero or greater.\r";
			bWriteError = true;
		}
		if (document.form1.iwWeight.value.length >0 && document.form1.iwWeight.value > 999999999){
			errmsg = errmsg + "- Weight is too large.\r";
			bWriteError = true;
		}

		// Solvent Volume if present must be a number
		if (document.form1.iwSolvent_Volume.value.length >0 && !isWholeNumber(document.form1.iwSolvent_Volume.value) && document.form1.iwSolvent_Volume.value.toLowerCase() != "null"){
			errmsg = errmsg + "- Solvent Volume must be zero or greater.\r";
			bWriteError = true;
		}
		if (document.form1.iwSolvent_Volume.value.length >0 && document.form1.iwSolvent_Volume.value > 999999999){
			errmsg = errmsg + "- Solvent Volume is too large.\r";
			bWriteError = true;
		}

		// Solution Volume if present must be a number
		if (document.form1.iwSolution_Volume.value.length >0 && !isWholeNumber(document.form1.iwSolution_Volume.value) && document.form1.iwSolution_Volume.value.toLowerCase() != "null"){
			errmsg = errmsg + "- Solution Volume must be zero or greater.\r";
			bWriteError = true;
		}
		if (document.form1.iwSolution_Volume.value.length >0 && isWholeNumber(document.form1.iwSolution_Volume.value) && document.form1.iwSolution_Volume.value > 999999999){
			errmsg = errmsg + "- Solution Volume is too large.\r";
			bWriteError = true;
		}
		
		// Concentration if present must be a number
		if (document.form1.iwConcentration.value.length >0 && !isWholeNumber(document.form1.iwConcentration.value) && document.form1.iwConcentration.value.toLowerCase() != "null"){
			errmsg = errmsg + "- Concentration must be zero or greater.\r";
			bWriteError = true;
		}	 
		if (document.form1.iwConcentration.value.length >0 && isWholeNumber(document.form1.iwConcentration.value) && document.form1.iwConcentration.value > 999999999){
			errmsg = errmsg + "- Concentration is too large.\r";
			bWriteError = true;
		}	 

		// Report problems
		if (bWriteError){
			alert(errmsg);
		}
		else{
			if (strMode.toLowerCase() == "edit"){
				document.form1.action = "EditPlate_action.asp?GetData=form";
			}
			else{
				document.form1.action = "CreatePlate_action.asp?GetData=form";
			}
			var bcontinue = true;
			
			if (bcontinue) document.form1.submit();
		}

	}
	
	function CheckNumCopies(element) {
		if (element.value > 1 && document.form1.bUseBarcodeDesc.value == 'false'){ document.form1.AutoGen_cb.click();}
	}
	 
	// Post data between tabs
	function postDataFunction(sTab) {
		document.form1.action = "CreateOrEditPlate.asp?isEdit=<%=isEdit%>&GetData=form&sTab=" + sTab;
		document.form1.submit();
	}
//-->
</script>
</head>
<body>
<!--#INCLUDE VIRTUAL = "/cheminv/gui/CreateOrEditPlateTabs.asp"-->
<form name="form1" method="POST">
<table border="0" cellspacing="0" cellpadding="0" width="700">
<INPUT TYPE="hidden" NAME="iPlateIDs" VALUE="<%=Plate_ID%>">
<INPUT TYPE="hidden" NAME="iPlate_Format_ID_FK" VALUE="<%=Plate_Format_ID_FK%>">
<INPUT TYPE="hidden" NAME="iIs_Plate_Map" VALUE="<%=Is_Plate_Map%>">
<input TYPE="hidden" NAME="AutoGen" Value="<%=AutoGen%>">
<input TYPE="hidden" NAME="isCopy" VALUE="<%=isCopy%>">
<%
NumCopies = 1

Select Case sTab
	Case "Plate"
	'Response.Write "<BR>plPlateTypeName=" & plPlateTypeName
	LocationID = Location_ID_FK
%>
	<% if isEdit then %>
	<tr>
		<td></td>
		<td colspan="3">
			<span class="GuiFeedback" align="center">Plate values represent aggregates over the wells.  Please update the well attributes if you wish to update all the wells for this plate.</span><br><br>
		</td>
	</tr>
	<% end if %>
	<tr height="25">
		<td align="right" valign="top" nowrap width="200">
			<span class="required">Location ID:</span>
		</td>
		<td colspan="3">
			&nbsp;<%=GetBarcodeIcon()%>&nbsp;<%ShowLocationPicker "document.form1", "iLocation_ID_FK", "Location_BarCode", "Location_Name", 10, 49, false%> 
		</td>
	</tr>
	<tr height="25">
		<%=ShowPickList("<span class=""required"">Plate type:</span>", "iPlate_Type_ID_FK", Plate_Type_ID_FK, "SELECT plate_type_id AS Value, plate_type_name AS DisplayText FROM inv_plate_types ORDER BY lower(DisplayText) ASC")%>
		<td></td>
		<td></td>
	</tr>
	<tr height="25">
		<td align="right" valign="top" nowrap width="200">
			<span class="required">Plate format:</span>
		</td>
		<td>
			<input type="text" name="iPlate_Format_Name" size="30" value="<%=Plate_Format_Name%>" CLASS="GrayedText" READONLY>
		</td>
		<td></td>
		<td></td>
	</tr>
	<tr height="25">
		<%'=ShowPickList("<span class=""required"">Unit of quantity:</span>", "iUOMID", UOWID, "SELECT UNIT_ID||'='||Unit_Abreviation AS Value, Unit_Name AS DisplayText FROM inv_units WHERE Unit_type_id_FK IN (1,2) ORDER BY lower(DisplayText) ASC")%>
		<%=ShowPickList("<span class=""required"">Unit of quantity:</span>", "iQty_Unit_FK", iif((isEmpty(Qty_Unit_FK) or Qty_Unit_FK=""),Application("plDefMassUnitID"),Qty_Unit_FK), "SELECT UNIT_ID AS Value, Unit_Name AS DisplayText FROM inv_units WHERE Unit_type_id_FK IN (1,2) ORDER BY lower(DisplayText) ASC")%>
		<td></td>
		<td></td>
	</tr>
	<%if not IsEdit then %>
	<tr height="25">
		<%=ShowInputBox2("Initial quantity:", "Qty_Initial", 200, 15, "", False, True)%>
		<input type="hidden" name="iQty_Remaining">
		<td></td>
		<td></td>
	</tr>
	<tr height="25">
		<TD ALIGN="right" VALIGN="top" NOWRAP WIDTH="200"><SPAN CLASS="required">Number of copies:</SPAN></TD>
		<TD><INPUT TYPE="text" NAME="iNumCopies" SIZE="5" VALUE="<%=NumCopies%>" ONCHANGE="CheckNumCopies(this);"></TD>
		<td></td>
		<td></td>
	</tr>
	<%else%>
	<tr height="25">
		<%=ShowInputBox2("Quantity remaining:", "Qty_Remaining", 200, 5, "", False, true)%>
		<td></td>
		<td></td>
	</tr>		
	<%End if%>
	<tr height="25">
		<%=ShowPickList("Plate status:", "iStatus_ID_FK", Status_ID_FK,"SELECT Enum_ID AS Value, Enum_Value AS DisplayText FROM inv_enumeration, inv_enumeration_set WHERE Eset_name = 'Plate Status' and eset_id_fk = eset_id ORDER BY lower(DisplayText) ASC")%>
		<%=ShowInputBox2("Group name:", "Group_Name", 200, 15,"", False, false)%>
	</tr>
	<tr height="25">
		<td align="right" VALIGN="top" nowrap>Select Solvent:</td>
		<td><%=ShowSelectBox2("iSolvent_ID_FK",solvent_id_fk,"SELECT solvent_id AS Value, solvent_name AS DisplayText FROM " & Application("CHEMINV_USERNAME") & ".inv_solvents ORDER BY lower(DisplayText) ASC", null, "Select a Solvent", "null")%></td>
		<td align="right" VALIGN="top" nowrap>Library:</td>
		<td><%=ShowSelectBox2("iLibrary_ID_FK", Library_ID_FK, "SELECT enum_id AS Value, enum_value AS DisplayText FROM inv_enumeration WHERE eset_id_fk = 5 ORDER BY lower(DisplayText) ASC", null, "Select a Library", "null")%></td>
	</tr>
	<tr height="25">
		<%=ShowInputBox2("Solvent volume:", "Solvent_Volume", 200, 15, "", False, False)%>
		<%=ShowInputBox2("Plate name:", "Plate_Name", 200, 15, "", False, false)%>
	</tr>
	<tr height="25">
		<%=ShowInputBox2("Solution volume:", "Solution_Volume", 200, 15, "", False, False)%>
		<%=ShowInputBox2("Supplier:", "Supplier", 200, 15, "", False, False)%>
	</tr>
	<tr height="25">
		<td align="right" VALIGN="top" nowrap>Solvent/Solution Unit:</td>
		<td><%=ShowSelectBox("iSolvent_Volume_Unit_ID_FK", iif((isEmpty(Solvent_Volume_Unit_ID_FK) or Solvent_Volume_Unit_ID_FK=""),Application("plDefVolUnitID"),Solvent_Volume_Unit_ID_FK),"SELECT UNIT_ID AS Value, Unit_Name AS DisplayText FROM inv_units WHERE Unit_type_id_FK IN (1) ORDER BY lower(DisplayText) ASC")%></td>
		<%=ShowInputBox2("Supplier barcode:", "Supplier_Barcode", 200, 15, "", False, False)%>
	</tr>
	<tr height="25">
		<%=ShowInputBox2("Concentration:", "Concentration", 200, 15,  ShowSelectBox("iConc_Unit_FK", iif((isEmpty(Conc_Unit_FK) or Conc_Unit_FK=""),Application("plDefConcUnitID"),Conc_Unit_FK),"SELECT UNIT_ID AS Value, Unit_Name AS DisplayText FROM inv_units WHERE Unit_type_id_FK IN (3) ORDER BY lower(DisplayText) ASC"), False, false)%>
		<%=ShowInputBox2("Supplier shipment number:", "Supplier_Shipment_Number", 200, 15, "", False, False)%>
	</tr>
	<tr height="25">
		<%=ShowInputBox2("Weight:", "Weight", 200, 15, ShowSelectBox("iWeight_Unit_FK", iif((isEmpty(Weight_Unit_FK) or Weight_Unit_FK=""),Application("plDefMassUnitID"),Weight_Unit_FK),"SELECT UNIT_ID AS Value, Unit_Name AS DisplayText FROM inv_units WHERE Unit_type_id_FK IN (2) ORDER BY lower(DisplayText) ASC"), False, False)%>
		<%=ShowInputBox2("Supplier shipment code:", "Supplier_Shipment_Code", 200, 15, "", False, False)%>
	</tr>
	<tr height="25">
		<%=ShowInputBox2("FT cycles:", "FT_Cycles", 200, 15, "", False, False)%>
		<TD ALIGN="right" VALIGN="top" NOWRAP WIDTH="200">Supplier Shipment Date:</TD>
		<TD><%call ShowInputField("", "", "iSupplier_Shipment_Date:form1:" & Supplier_Shipment_Date , "DATE_PICKER:TEXT", "15")%></TD>
	</tr>
	<% if not isEdit then%>
	<tr height="25">
		<%if lcase(Application("RequireBarcode")) = "true" then%>
			<td align="right" valign="top" nowrap width="200"><span id="sp0" style="display:block;"><span class="required">Plate Barcode:</span></span><span id="sp1" style="display:block;"><span class="required">Barcode Description:</span></span></td>
		<%else%>
			<td align="right" valign="top" nowrap width="200"><span id="sp0" style="display:block;">Plate Barcode:</span><span id="sp1" style="display:none;">Barcode Description:</span></td>
		<%end if%>
		<td>
			<script language="JavaScript">
			function AutoGen_OnClick(element) {

				document.form1.iPlate_Barcode.value=''; 
				document.form1.AutoGen.value = element.checked; 
				numCopies = document.form1.iNumCopies.value;
				if (numCopies > 1) {
					if (element.checked == false) {
						alert('You must autogenerate multiple barcodes.')
						element.checked = true;
					}				
				}
				else {
					element.checked ? document.all.sp0.style.display = 'none' :document.all.sp0.style.display = 'block';
					element.checked ? document.all.sp1.style.display = 'block':document.all.sp1.style.display = 'none';
					document.all.sp2.style.display = document.all.sp0.style.display;
					document.all.sp3.style.display = document.all.sp1.style.display;
				}
				//update the bUseBarcodeDesc value
				element.checked ? document.all.bUseBarcodeDesc.value = 'true' : document.all.bUseBarcodeDesc.value = 'false';
			}
			</script>
			<input type="hidden" name="bUseBarcodeDesc" value="">
			<span id="sp2" style="display:block;">
				&nbsp;<%=GetBarcodeIcon()%>&nbsp;<input type="text" name="iPlate_Barcode" size="15" value=""><br>
			</span>
			<span id="sp3" style="display:none">
				<%=ShowSelectBox2("iBarcode_Desc_ID", Barcode_desc_ID,"SELECT barcode_desc_id AS Value, barcode_desc_name AS Displaytext FROM inv_barcode_desc ORDER BY lower(DisplayText) ASC", null," ","")%>
			</span>
			<input type="checkbox" name="AutoGen_cb" onclick="AutoGen_OnClick(this);">Autogenerate barcode ID
			<script language="Javascript">if (document.form1.AutoGen.value == "true") document.form1.AutoGen_cb.click();</script>
		</td>
		<td></td>
		<td></td>
	</tr>
	
	<%Else%>
	<tr height="25">
		<td align="right" valign="top" nowrap width="200"><span class="required">Plate Barcode:</span></td><td>&nbsp;<%=GetBarcodeIcon()%>&nbsp;<input type="text" name="iPlate_Barcode" size="15" value="<%=Plate_Barcode%>"></td>
		<td></td>
		<td></td>
	</tr>
	<%End if%>
	

	<%

	j=1
	For each key in custom_plate_fields_dict
		if (j Mod 2) = 1 then 	Response.Write "<tr height=""25"">" & vblf
		if inStr(uCase(Key), "DATE_") then
			Response.Write "<td align=""right"" valign=""top"" nowrap width=""200"">" 
			if req_custom_plate_fields_dict.Exists(Key) then Response.Write "<span class=""required"">"
			Response.Write custom_plate_fields_dict.Item(key)
			if req_custom_plate_fields_dict.Exists(Key) then Response.Write "</span>"
			Response.Write ":</td>"
			Response.Write "<td>"
			call ShowInputField("", "", "i" & Key & ":form1:" & eval(Key) , "DATE_PICKER:TEXT", "15")
			Response.Write "</td>"

			'str = ShowInputBox2(custom_plate_fields_dict.Item(key) & ":", Key, 250, 25, "", False, req_custom_plate_fields_dict.Exists(Key))
			'Response.write Left(str,len(str)-5)
			'Response.Write "<a href onclick=""return PopUpDate(&quot;i" & Key & "&quot;,&quot;/cheminv/gui/month_view1.asp&quot;)""><img src=""/cfserverasp/source/graphics/navbuttons/icon_mview.gif"" height=""16"" width=""16"" border=""0""></a>"
			'Response.Write "</td>" & vblf
		Else
			if custom_lists_dict.Exists("CUSTOM_PLATE_FIELDS." & key) then
				Response.Write "<td align=right valign=top nowrap width=""200"">" 
				if req_custom_plate_fields_dict.Exists(Key) then
					Response.Write "<span class=""required"">" & custom_plate_fields_dict.Item(key) & ":</span></td>"
				else
					Response.Write custom_plate_fields_dict.Item(key) & ":</td>"
				end if
				
				Response.Write "<td><select name=""i" & key & """>"

				'-- build the select from the custom list
				Response.Write GetCustomListOptions("CUSTOM_PLATE_FIELDS", key, eval(key), null)
				
				Response.Write "</td>"
			else
				Response.write ShowInputBox2(custom_plate_fields_dict.Item(key) & ":", Key, 200, 25, "", False, req_custom_plate_fields_dict.Exists(Key)) & vblf
			end if
		
		end if
		if (j Mod 2) = 0 then Response.Write "</TR>" & vblf
		j = j + 2
	Next 
	'take care of odd number of custom fields
	if (j Mod 2) = 0 then Response.Write "<td></td></TR>"
	
	%>
	<tr height="25">
		<td align="right" colspan="4"> 
				<a HREF="#" onclick="if (DialogWindow) {DialogWindow.close()}; window.close();return false"><img SRC="../graphics/cancel_dialog_btn.gif" border="0" alt="Cancel" WIDTH="61" HEIGHT="21"></a>
				<%if NOT isEdit then%>
					<a HREF="#" onclick="ValidatePlate('Create'); return false;"><img SRC="../graphics/ok_dialog_btn.gif" border="0" WIDTH="61" HEIGHT="21"></a>
				<%Else%>
					<a HREF="#" onclick="ValidatePlate('Edit'); return false;"><img SRC="../graphics/ok_dialog_btn.gif" border="0" WIDTH="61" HEIGHT="21"></a>
				<%End if%>		
		</td>
	</tr>
<%
	Case "Well"
%>
	<INPUT TYPE="hidden" NAME="isWellTab" VALUE="true">
	<tr height="25">
		<%=ShowInputBox2("Quantity remaining:", "wQty_Remaining", 150, 15, ShowSelectBox("iwQty_Unit_FK", Qty_Unit_FK,"SELECT unit_id AS Value, unit_name AS DisplayText FROM inv_units WHERE unit_type_id_fk in (1,2) ORDER BY lower(DisplayText) ASC"), False, False)%>
	</tr>
	<tr height="25">
		<td CLASS="required" align=right width="150" nowrap>Select Solvent:</td>
		<td><%Response.Write(ShowSelectBox2("iwSolvent_ID_FK",wSolvent_ID_FK,"SELECT solvent_id AS Value, solvent_name AS DisplayText FROM " & Application("CHEMINV_USERNAME") & ".inv_solvents ORDER BY lower(DisplayText) ASC", null, "Select a Solvent", "null"))%></td>
	</tr>
	<tr height="25">
		<%=ShowInputBox2("Solvent Volume:", "wSolvent_Volume", 150, 15, "", False, False)%>
	</tr>
	<tr height="25">
		<%=ShowInputBox2("Solution volume:", "wSolution_Volume", 150, 15, "", False, False)%>
	</tr>
	<tr height="25">
		<td align="right" VALIGN="top" nowrap>Solvent/Solution Unit:</td>
		<td><%=ShowSelectBox("iwSolvent_Volume_Unit_ID_FK", Solvent_Volume_Unit_ID_FK,"SELECT unit_id AS Value, unit_name AS DisplayText FROM inv_units WHERE unit_type_id_fk in (1) ORDER BY lower(DisplayText) ASC")%></td>
	</tr>
	<tr height="25">
		<%=ShowInputBox2("Concentration:", "wConcentration", 150, 15, ShowSelectBox("iwConc_Unit_FK",Conc_Unit_FK,"SELECT unit_id AS Value, unit_name AS DisplayText FROM inv_units WHERE unit_type_id_fk in (3) ORDER BY lower(DisplayText) ASC"), False, False)%>
	</tr>
	<tr height="25">
		<%=ShowInputBox2("Weight:", "wWeight", 150, 15, ShowSelectBox("iwWeight_Unit_FK", Weight_Unit_FK,"SELECT unit_id AS Value, unit_name AS DisplayText FROM inv_units WHERE unit_type_id_fk in (2) ORDER BY lower(DisplayText) ASC"), False, False)%>
	</tr>
	<% if isEdit then %>
	<tr>
		<td></td>
		<td align="left">
			<i>To set a value to NULL enter "null".</i>
		</td>
	</tr>
	<% end if %>
	<tr>
		<td colspan=2" align="right" height="20" valign="bottom"> 
				<a HREF="#" onclick="if (DialogWindow) {DialogWindow.close()}; window.close();return false"><img SRC="../graphics/cancel_dialog_btn.gif" border="0" alt="Cancel" WIDTH="61" HEIGHT="21"></a>
				<%if NOT isEdit then%>
					<a HREF="#" onclick="ValidateWell('Create'); return false;"><img SRC="../graphics/ok_dialog_btn.gif" border="0" WIDTH="61" HEIGHT="21"></a>
				<%Else%>
					<a HREF="#" onclick="ValidateWell('Edit'); return false;"><img SRC="../graphics/ok_dialog_btn.gif" border="0" WIDTH="61" HEIGHT="21"></a>
				<%End if%>		
		</td>
	</tr>
<%End Select%>
</table>	
</form>
<%If Request("showconflicts") then%>
<script LANGUAGE="javascript">
	OpenDialog('CreateSubstance2.asp?action=showconflicts&CompoundID=<%=CompoundID%>', 'SubsManager', 2);
</script>
<%end if%>
<%If Request("editsubstance") then%>
<script LANGUAGE="javascript">
	OpenDialog('CreateOrEditSubstance.asp?action=edit&CompoundID=<%=CompoundID%>', 'SubsManager', 2);
</script>
<%end if%>
</body>
</html>
