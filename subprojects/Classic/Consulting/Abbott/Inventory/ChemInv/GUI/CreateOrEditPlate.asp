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
end if


%>
<!--#INCLUDE VIRTUAL = "/cheminv/api/apiUtils.asp"-->
<!--#INCLUDE VIRTUAL = "/cheminv/gui/guiUtils.asp"-->
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/utility_func_vbs.asp"-->
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/display_func_vbs.asp"-->
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/search_func_vbs.asp"-->

<html>
<head>
<style>
		A {Text-Decoration: none;}
		.TabView {color:#000000; font-size:8pt; font-family: arial}
		A.TabView:LINK {Text-Decoration: none; color:#000000; font-size:8pt; font-family: arial}
		A.TabView:VISITED {Text-Decoration: none; color:#000000; font-size:8pt; font-family: arial}
		A.TabView:HOVER {Text-Decoration: underline; color:#4682b4; font-size:8pt; font-family: arial}
		.singleRackElement { display:none; }
		.rackLabel {color:#000;}
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

		<% if Application("RACKS_ENABLED") then %>
		// Validation for Rack assignment
		if (document.form1.AssignToRack) {
			if (document.form1.AssignToRack.checked) {
				
				// Validate Rack Grid ID
				if (document.form1.iRackGridID.value.length == 0){
					errmsg = errmsg + "- Please select a valid Rack grid location.\r";
					bWriteError = true;
				}else if (!isPositiveNumber(document.form1.iRackGridID.value)&&(document.form1.iRackGridID.value != 0)){
					errmsg = errmsg + "- Rack grid location must be a positive number.\r";
					bWriteError = true;
				}else{
					document.form1.iLocation_ID_FK.value = document.form1.iRackGridID.value;
				}
			}

			// Destination cannot be assign to a rack parent
			if (!document.form1.AssignToRack.checked){
				if (ValidateLocationIDType(document.form1.iLocation_ID_FK.value) == 1) {
					errmsg = errmsg + "- Destination can not be a Rack. If you would like to move the Plate \rinto a Rack, please click \"Assign to Rack\" and choose a rack position.\r";
					bWriteError = true;
				}
			}
		}
		<% end if %>
		
		//bWriteError = true;
<%if NOT isEdit then%>
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
		if (document.form1.iFT_Cycles.value.length >0 && !isNumber(document.form1.iFT_Cycles.value)){
			errmsg = errmsg + "- FT Cycles must be a number.\r";
			bWriteError = true;
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

		// Qty Remaining if present must be a number
		if (document.form1.iwQty_Remaining.value.length >0 && !isWholeNumber(document.form1.iwQty_Remaining.value)){
			errmsg = errmsg + "- Qty Remaining must be zero or greater.\r";
			bWriteError = true;
		}	 
		if (document.form1.iwQty_Remaining.value.length >0 && document.form1.iwQty_Remaining.value > 999999999){
			errmsg = errmsg + "- Qty Remaining is too large.\r";
			bWriteError = true;
		}	 

		// Weight if present must be a number
		if (document.form1.iwWeight.value.length >0 && !isWholeNumber(document.form1.iwWeight.value)){
			errmsg = errmsg + "- Weight must be zero or greater.\r";
			bWriteError = true;
		}
		if (document.form1.iwWeight.value.length >0 && document.form1.iwWeight.value > 999999999){
			errmsg = errmsg + "- Weight is too large.\r";
			bWriteError = true;
		}

		// Solvent Volume if present must be a number
		if (document.form1.iwSolvent_Volume.value.length >0 && !isWholeNumber(document.form1.iwSolvent_Volume.value)){
			errmsg = errmsg + "- Solvent Volume must be zero or greater.\r";
			bWriteError = true;
		}
		if (document.form1.iwSolvent_Volume.value.length >0 && document.form1.iwSolvent_Volume.value > 999999999){
			errmsg = errmsg + "- Solvent Volume is too large.\r";
			bWriteError = true;
		}
		
		// Concentration if present must be a number
		if (document.form1.iwConcentration.value.length >0 && !isWholeNumber(document.form1.iwConcentration.value)){
			errmsg = errmsg + "- Concentration must be zero or greater.\r";
			bWriteError = true;
		}	 
		if (document.form1.iwConcentration.value.length >0 && document.form1.iwConcentration.value > 999999999){
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

	// Toggles display of Rack editing fields
	function toggleRackDisplay() {
		if (document.form1.AssignToRack.checked) {
			AlterCSS('.singleRackElement','display','block')
			//AlterCSS('.rackDisplay','display','block')
			//AlterCSS('.locationDisplay','display','none')
			AlterCSS('.singleRackElement','color','red')
		}else{
			AlterCSS('.singleRackElement','display','none')
			//AlterCSS('.rackDisplay','display','none')
			//AlterCSS('.locationDisplay','display','block')
			AlterCSS('.rackLabel','color','black')
		}
	}	
	

	function validateRackSelect() {
		var bWriteError = false;
		var errmsg = "Please fix the following problems:\r\r";
		var Temp = "";
		var TempIDs = "";
		var arrTemp = "";
		var arrTempName = "";
		var arrTempValue = "";
		if (document.form1.RackID.selectedIndex==-1) {
			alert("Please select at least one Rack");
		}else{

			var totPosCnt = 0;

			for (i=0; i < document.form1.RackID.length; i++){
				// If Rack list is Empty
				if (document.form1.RackID[i].value == 'NULL'){
					errmsg = errmsg + "There are no Racks in the current location. \rPlease select a different location or create a New Rack.\r\r";
					bWriteError = true;
				}else{
					if (document.form1.RackID[i].selected){
						if (i > 0 && TempIDs.length > 0) TempIDs = TempIDs + ',';
						TempIDs = TempIDs + document.form1.RackID.options[i].value;
						Temp = (document.form1.RackID.options[i].text).replace(" ","");
						arrTemp = Temp.split("::");
						arrTempName = arrTemp[0];
						arrTempValue = arrTemp[1].replace(" open","");
						if (arrTempValue == 0){
							errmsg = errmsg + "The select Rack, " + arrTempName + ", contains no open positions. \rPlease choose a different Rack.\r\r";
							bWriteError = true;
							document.form1.RackID.options[i].selected=false;						
						}else{
							totPosCnt = totPosCnt + parseInt(arrTempValue);
						}
					}
				}
			}

			if (bWriteError){
				alert(errmsg);
			}else{
				OpenDialog('/cheminv/gui/ViewRackLayout.asp?ActionType=select&PosRequired=1&IsMulti=true&LocationID='+document.form1.RackID.options[document.form1.RackID.selectedIndex].value+'&RackIDList='+TempIDs, 'RackGrid', 2);
				return false;	
			}
		}
	}

//-->
</script>
</head>
<body>
<!--#INCLUDE VIRTUAL = "/cheminv/gui/CreateOrEditPlateTabs.asp"-->
<form name="form1" method="POST">
<table border="0" cellspacing="0" cellpadding="0" width="700">
<!--#INCLUDE VIRTUAL = "/cheminv/gui/GetPlateAttributes.asp"-->
<input TYPE="hidden" NAME="iPlateIDs" VALUE="<%=Plate_ID%>">
<input TYPE="hidden" NAME="iPlate_Format_ID_FK" VALUE="<%=Plate_Format_ID_FK%>">
<input TYPE="hidden" NAME="iIs_Plate_Map" VALUE="<%=Is_Plate_Map%>">
<input TYPE="hidden" NAME="AutoGen" Value="<%=AutoGen%>">
<input TYPE="hidden" NAME="isCopy" VALUE="<%=isCopy%>">
<%
NumCopies = 1

Select Case sTab
	Case "Plate"

	LocationID = Location_ID_FK
	if Application("RACKS_ENABLED") then
		'-- If user has selected a rack or contents of rack, set the default location to parent of rack
		'-- ValidateLocationIsRack returns COLLAPSE_CHILD_NODES, PARENT_ID for container
		rackTemp = split(ValidateLocationIsRack(LocationID),"::")
		isRack = rackTemp(0)
		rackParent = rackTemp(1)
		if isRack = "1" then
			LocationID = rackParent
		end if
	end if
%>
	<tr>
		<td></td>
		<td colspan="3">
			<span class="GuiFeedback" align="center">Plate values represent aggregates over the plate.  Please update the well attributes if you wish to update all the wells for this plate.</span><br><br>
		</td>
	</tr>
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
		<td></td>
		<td></td>
	</tr>
	<tr height="25">
		<td ALIGN="right" VALIGN="top" NOWRAP WIDTH="200"><span CLASS="required">Number of copies:</span></td>
		<td><input TYPE="text" NAME="iNumCopies" SIZE="5" VALUE="<%=NumCopies%>" ONCHANGE="CheckNumCopies(this);"></td>
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
	<% if Application("RACKS_ENABLED") then %>	
	<tr height="25">
		<td align="right">Assign Rack:</td><td>
			<input type="hidden" name="RackGridList" value />
			<input class="noborder" type="checkbox" name="AssignToRack" onclick="toggleRackDisplay()">Assign container to Rack
		</td>
	</tr>
	<tr class="singleRackElement">
		<!-- ShowPickList3("Select Rack(s):", "RackID", RackID,"select l.location_id as Value,  l.location_name||' :: '||  ((f.row_count*f.col_count)-(     (select count(*) from inv_containers where location_id_fk in (select location_id from inv_vw_grid_location where parent_id=l.location_id))+     (select count(*) from inv_plates where location_id_fk in (select location_id from inv_vw_grid_location where parent_id=l.location_id))+     (select count(*) from inv_locations where parent_id in (select location_id from inv_vw_grid_location where parent_id=l.location_id)) ))||' open' as DisplayText, (select min(location_id||'::'||name) from inv_vw_grid_location where parent_id=l.location_id and location_id not in (select distinct location_id_fk from inv_containers) and location_id not in (select distinct location_id_fk from inv_plates) and location_id not in (select distinct parent_id from inv_locations where collapse_child_nodes=1)) as DefaultValue from inv_grid_storage s, inv_locations l , inv_grid_format f  where s.location_id_fk = l.location_id  and l.collapse_child_nodes=1  and s.grid_format_id_fk = f.grid_format_id  and l.location_id in (select location_id from inv_locations where collapse_child_nodes=1 connect by prior location_id = parent_id start with location_id=" & LocationID & ")  and collapse_child_nodes=1  and ((f.row_count*f.col_count)-(     (select count(*) from inv_containers where location_id_fk in (select location_id from inv_vw_grid_location where parent_id=l.location_id))+     (select count(*) from inv_plates where location_id_fk in (select location_id from inv_vw_grid_location where parent_id=l.location_id))+     (select count(*) from inv_locations where parent_id in (select location_id from inv_vw_grid_location where parent_id=l.location_id)) )) > 0 order by DisplayText",100,"","","selectRackDefaults();",1,false,"No Racks in this location") -->
		<%= ShowPickList3("Select Rack(s):", "RackID", RackID,"select l.location_id as Value, cheminvdb2.guiutils.GETRACKLOCATIONPATH(l.location_id) || ' :: ' || ((f.row_count * f.col_count) - ((select /*+ index(c CONTAINER_LOCATION_ID_FK_IDX) */count(*) from inv_containers c, inv_vw_grid_location_lite gl where c.location_id_fk = gl.location_id and gl.parent_id = l.location_id) + (select /*+ index(p PLATE_LOCATION_ID_FK_IDX) */count(*) from inv_plates p, inv_vw_grid_location_lite gl where p.location_id_fk = gl.location_id and gl.parent_id = l.location_id) + (select /*+ index(lr INV_LOCATION_PK) */count(*) from inv_locations lr, inv_vw_grid_location_lite gl where lr.parent_id = gl.location_id and gl.parent_id = l.location_id))) || ' open' as DisplayText, (select gl.location_id || '::' || gl.name from inv_vw_grid_location gl where gl.parent_id = l.location_id and not exists (select /*+ index(c CONTAINER_LOCATION_ID_FK_IDX) */ location_id_fk from inv_containers c where c.location_id_fk = gl.location_id) and not exists (select /*+ index(p PLATE_LOCATION_ID_FK_IDX) */ location_id_fk from inv_plates p where p.location_id_fk = gl.location_id) and not exists (select /*+ index(lr INV_LOCATION_PK) */ parent_id from inv_locations l where gl.location_id = l.parent_id and collapse_child_nodes = 1) and rownum = 1 )  as DefaultValue from inv_grid_storage s, inv_locations l, inv_grid_format f where s.location_id_fk = l.location_id and l.collapse_child_nodes = 1 and s.grid_format_id_fk = f.grid_format_id and l.location_id in (select location_id from inv_locations where collapse_child_nodes = 1 connect by prior location_id = parent_id start with location_id = " & LocationID & ") and collapse_child_nodes = 1    and ((f.row_count * f.col_count) - ((select /*+ index(c CONTAINER_LOCATION_ID_FK_IDX) */count(*) from inv_containers c, inv_vw_grid_location_lite gl where c.location_id_fk = gl.location_id and gl.parent_id = l.location_id) + (select /*+ index(p PLATE_LOCATION_ID_FK_IDX) */count(*) from inv_plates p, inv_vw_grid_location_lite gl where p.location_id_fk = gl.location_id and gl.parent_id = l.location_id) + (select /*+ index(lr INV_LOCATION_PK) */count(*) from inv_locations lr, inv_vw_grid_location_lite gl where lr.parent_id = gl.location_id and gl.parent_id = l.location_id and lr.collapse_child_nodes = 1))) > 0  order by DisplayText",100,"","","selectRackDefaults();",1,false,"No Racks in this location")%>
	</tr>
	<tr class="singleRackElement" height="25">
		<td align="right" valign="top">Rack Position:</td><td valign="top">
			<table cellpadding="0" cellspacing="0"><tr><td>
				<input type="hidden" name="iRackGridID" value>
				<input type="text" size="25" name="iRackGridName" readonly style="background-color:#d3d3d3;" value>
			</td><td>&nbsp;</td><td valign="bottom">
				<a href="#" onclick="validateRackSelect()"><img src="/cheminv/graphics/btn_search.gif" border="0"></a>
			</td></tr></table>
		</td>
	</tr>
	<script language="javascript">
		selectRackDefaults();
	</script>
	<% end if %>
	<tr height="25">
		<%=ShowPickList("Plate status:", "iStatus_ID_FK", Status_ID_FK,"SELECT Enum_ID AS Value, Enum_Value AS DisplayText FROM inv_enumeration, inv_enumeration_set WHERE Eset_name = 'Plate Status' and eset_id_fk = eset_id ORDER BY lower(DisplayText) ASC")%>
		<%=ShowInputBox2("FT cycles:", "FT_Cycles", 200, 15, "", False, False)%>
	</tr>
	<tr height="25">
		<td align="right" VALIGN="top" nowrap>Select Solvent:</td>
		<td><%=ShowSelectBox2("iSolvent_ID_FK",solvent_id_fk,"SELECT solvent_id AS Value, solvent_name AS DisplayText FROM " & Application("CHEMINV_USERNAME") & ".inv_solvents ORDER BY lower(DisplayText) ASC", null, "Select a Solvent", "null")%></td>
		<%=ShowInputBox2("Group name:", "Group_Name", 200, 15,"", False, false)%>
	</tr>
	<tr height="25">
		<%=ShowInputBox2("Solvent volume:", "Solvent_Volume", 200, 15, ShowSelectBox("iSolvent_Volume_Unit_ID_FK", iif((isEmpty(Solvent_Volume_Unit_ID_FK) or Solvent_Volume_Unit_ID_FK=""),Application("plDefVolUnitID"),Solvent_Volume_Unit_ID_FK),"SELECT UNIT_ID AS Value, Unit_Name AS DisplayText FROM inv_units WHERE Unit_type_id_FK IN (1) ORDER BY lower(DisplayText) ASC"), False, False)%>
		<td align="right" VALIGN="top" nowrap>Library:</td>
		<td><%=ShowSelectBox2("iLibrary_ID_FK", Library_ID_FK, "SELECT enum_id AS Value, enum_value AS DisplayText FROM inv_enumeration WHERE eset_id_fk = 5 ORDER BY lower(DisplayText) ASC", null, "Select a Library", "null")%></td>
	</tr>
	<tr height="25">
		<%=ShowInputBox2("Concentration:", "Concentration", 200, 15,  ShowSelectBox("iConc_Unit_FK", iif((isEmpty(Conc_Unit_FK) or Conc_Unit_FK=""),Application("plDefConcUnitID"),Conc_Unit_FK),"SELECT UNIT_ID AS Value, Unit_Name AS DisplayText FROM inv_units WHERE Unit_type_id_FK IN (3) ORDER BY lower(DisplayText) ASC"), False, false)%>
		<%=ShowInputBox2("Plate name:", "Plate_Name", 200, 15, "", False, false)%>
	</tr>
	<tr height="25">
		<%=ShowInputBox2("Weight:", "Weight", 200, 15, ShowSelectBox("iWeight_Unit_FK", iif((isEmpty(Weight_Unit_FK) or Weight_Unit_FK=""),Application("plDefMassUnitID"),Weight_Unit_FK),"SELECT UNIT_ID AS Value, Unit_Name AS DisplayText FROM inv_units WHERE Unit_type_id_FK IN (2) ORDER BY lower(DisplayText) ASC"), False, False)%>
		<td></td>
		<td></td>
	</tr>
	<tr height="25">
		<%=ShowInputBox2("Supplier barcode:", "Supplier_Barcode", 200, 15, "", False, False)%>
		<td></td>
		<td></td>
	</tr>
	<tr height="25">
		<%=ShowInputBox2("Supplier shipment number:", "Supplier_Shipment_Number", 200, 15, "", False, False)%>
		<td></td>
		<td></td>
	</tr>
	<tr height="25">
		<%=ShowInputBox2("Supplier shipment code:", "Supplier_Shipment_Code", 200, 15, "", False, False)%>
		<td></td>
		<td></td>
	</tr>
	<tr height="25">
		<td ALIGN="right" VALIGN="top" NOWRAP WIDTH="200">Supplier Shipment Date:</td>
		<td><%call ShowInputField("", "", "iSupplier_Shipment_Date:form1:" & Supplier_Shipment_Date , "DATE_PICKER:TEXT", "15")%></td>
		<td></td>
		<td></td>
	</tr>
	<% if not isEdit then%>
	<tr height="25">
		<td align="right" valign="top" nowrap width="200"><span id="sp0" style="display:block;">Plate Barcode:</span><span id="sp1" style="display:none;">Select Plate Barcode Type:</span></td>
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
			<input type="hidden" name="bUseBarcodeDesc" value>
			<span id="sp2" style="display:block;">
				&nbsp;<%=GetBarcodeIcon()%>&nbsp;<input type="text" name="iPlate_Barcode" size="15" value><br>
			</span>
			<span id="sp3" style="display:none">
				<%=ShowSelectBox("iBarcode_Desc_ID", Barcode_desc_ID,"SELECT barcode_desc_id AS Value, barcode_desc_name AS Displaytext FROM inv_barcode_desc ORDER BY lower(DisplayText) ASC")%>
			</span>
			<input type="checkbox" name="AutoGen_cb" onclick="AutoGen_OnClick(this);">Autogenerate barcode ID
			<script language="Javascript">if (document.form1.AutoGen.value == "true") document.form1.AutoGen_cb.click();</script>
		</td>
		<td></td>
		<td></td>
	</tr>
	
	<%Else%>
	<tr height="25">
		<td align="right" valign="top" nowrap width="200">Plate Barcode:</td><td>&nbsp;<%=GetBarcodeIcon()%>&nbsp;<input type="text" name="iPlate_Barcode" size="15" value="<%=Plate_Barcode%>"></td>
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
			Response.write ShowInputBox2(custom_plate_fields_dict.Item(key) & ":", Key, 200, 25, "", False, req_custom_plate_fields_dict.Exists(Key)) & vblf
		end if
		if (j Mod 2) = 0 then Response.Write "</TR>" & vblf
		j = j + 2
	Next 
	'take care of odd number of custom fields
	if (j Mod 2) = 0 then Response.Write "<td></td></TR>"
	
	%>
	<tr height="25">
		<td></td>
		<td align="right"> 
				<a HREF="#" onclick="if (DialogWindow) {DialogWindow.close()}; window.close();return false"><img SRC="/cheminv/graphics/sq_btn/cancel_dialog_btn.gif" border="0" alt="Cancel"></a>
				<%if NOT isEdit then%>
					<a HREF="#" onclick="ValidatePlate('Create'); return false;"><img SRC="/cheminv/graphics/sq_btn/ok_dialog_btn.gif" border="0"></a>
				<%Else%>
					<a HREF="#" onclick="ValidatePlate('Edit'); return false;"><img SRC="/cheminv/graphics/sq_btn/ok_dialog_btn.gif" border="0"></a>
				<%End if%>		
		</td>
	</tr>
<%
	Case "Well"
%>
	<input TYPE="hidden" NAME="isWellTab" VALUE="true">
	<tr height="25">
		<%=ShowInputBox2("Quantity remaining:", "wQty_Remaining", 150, 15, ShowSelectBox("iwQty_Unit_FK", Qty_Unit_FK,"SELECT unit_id AS Value, unit_name AS DisplayText FROM inv_units WHERE unit_type_id_fk in (1,2) ORDER BY lower(DisplayText) ASC"), False, False)%>
	</tr>
	<tr height="25">
		<td CLASS="required" align="right" width="150" nowrap>Select Solvent:</td>
		<td><%Response.Write(ShowSelectBox2("iwSolvent_ID_FK",wSolvent_ID_FK,"SELECT solvent_id AS Value, solvent_name AS DisplayText FROM " & Application("CHEMINV_USERNAME") & ".inv_solvents ORDER BY lower(DisplayText) ASC", null, "Select a Solvent", "null"))%></td>
	</tr>
	<tr height="25">
		<%=ShowInputBox2("Solvent Volume:", "wSolvent_Volume", 150, 15, ShowSelectBox("iwSolvent_Volume_Unit_ID_FK", Solvent_Volume_Unit_ID_FK,"SELECT unit_id AS Value, unit_name AS DisplayText FROM inv_units WHERE unit_type_id_fk in (1) ORDER BY lower(DisplayText) ASC"), False, False)%>
	</tr>
	<tr height="25">
		<%=ShowInputBox2("Concentration:", "wConcentration", 150, 15, ShowSelectBox("iwConc_Unit_FK",Conc_Unit_FK,"SELECT unit_id AS Value, unit_name AS DisplayText FROM inv_units WHERE unit_type_id_fk in (3) ORDER BY lower(DisplayText) ASC"), False, False)%>
	</tr>
	<tr height="25">
		<%=ShowInputBox2("Weight:", "wWeight", 150, 15, ShowSelectBox("iwWeight_Unit_FK", Weight_Unit_FK,"SELECT unit_id AS Value, unit_name AS DisplayText FROM inv_units WHERE unit_type_id_fk in (2) ORDER BY lower(DisplayText) ASC"), False, False)%>
	</tr>
	<tr>
		<td colspan="2&quot;" align="right" height="20" valign="bottom"> 
				<a HREF="#" onclick="if (DialogWindow) {DialogWindow.close()}; window.close();return false"><img SRC="/cheminv/graphics/sq_btn/cancel_dialog_btn.gif" border="0" alt="Cancel"></a>
				<%if NOT isEdit then%>
					<a HREF="#" onclick="ValidateWell('Create'); return false;"><img SRC="/cheminv/graphics/sq_btn/ok_dialog_btn.gif" border="0"></a>
				<%Else%>
					<a HREF="#" onclick="ValidateWell('Edit'); return false;"><img SRC="/cheminv/graphics/sq_btn/ok_dialog_btn.gif" border="0"></a>
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
