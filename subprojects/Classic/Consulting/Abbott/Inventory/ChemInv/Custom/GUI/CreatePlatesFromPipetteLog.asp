<%@ Language=VBScript %>
<!--#INCLUDE VIRTUAL = "/cheminv/gui/guiUtils.asp"-->
<!--#INCLUDE VIRTUAL = "/cheminv/api/apiUtils.asp"-->
<%
Dim Conn
%>
<html>
<head>
<title><%=Application("appTitle")%> -- Create Plates From Pipette Log</title>
<script LANGUAGE="javascript" src="/cheminv/Choosecss.js"></script>
<script LANGUAGE="javascript" src="/cheminv/utils.js"></script>
<script LANGUAGE="javascript" src="/cheminv/gui/validation.js"></script>
<script language="JavaScript">
<!--Hide JavaScript
	var DialogWindow;
	window.focus();
	function Validate(){
		var bWriteError = false;
		var errmsg = "Please fix the following problems:\r";
		// Destination is required
		if (document.form1.File1.value.length == 0) {
			errmsg = errmsg + "- File path is required.\r";
			bWriteError = true;
		}
				
		LocationID = document.form1.iLocation_ID_FK.value;
		PlateTypeID = document.form1.iPlate_Type_ID_FK.value;

		//LocationID is required
		if (document.form1.iLocation_ID_FK.value.length == 0) {
			errmsg = errmsg + "- Location ID is required.\r";
			bWriteError = true;
		}
		else{
			// LocationID can't be the root
			if (document.form1.iLocation_ID_FK.value == 0){
				errmsg = errmsg + "- Cannot create plate at the root location.\r";
				bWriteError = true;
			}
			// LocationID must be a positive number
			if (!isPositiveNumber(document.form1.iLocation_ID_FK.value)&&(document.form1.iLocation_ID_FK.value != 0)){
				errmsg = errmsg + "- Location ID must be a positive number.\r";
				bWriteError = true;
			}
		}	
		// Initial amount must be a number
		if ((document.form1.iQty_Initial.value.length  > 0) && !isPositiveNumber(document.form1.iQty_Initial.value)){
			errmsg = errmsg + "- Initial amount must be a positive number.\r";
			bWriteError = true;
		}
		if ((document.form1.iQty_Initial.value.length > 0) && (document.form1.iQty_Initial.value > 999999999)){
			errmsg = errmsg + "- Initial amount is too large.\r";
			bWriteError = true;
		}
		// Concentration if present must be a number
		if (document.form1.iConcentration.value.length >0 && !isPositiveNumber(document.form1.iConcentration.value)){
			errmsg = errmsg + "- Concentration must be a positive number.\r";
			bWriteError = true;
		}
		if (document.form1.iConcentration.value.length >0 && document.form1.iConcentration.value > 999999999){
			errmsg = errmsg + "- Concentration is too large.\r";
			bWriteError = true;
		}
		
		if (bWriteError){
			alert(errmsg);
		}
		else{
			document.form1.submit();
		}
	}
	function ActionType_OnClick(element) {
		if (element.value == "source") {
			element.checked ? document.all.sourceInstructions.style.display = 'block' :document.all.sourceInstructions.style.display = 'none';
			element.checked ? document.all.targetInstructions.style.display = 'none' :document.all.targetInstructions.style.display = 'block';
			element.checked ? document.all.BarcodeDesc.style.display = 'block' :document.all.BarcodeDesc.style.display = 'none';
		}
		else if (element.value=="target"){
			element.checked ? document.all.targetInstructions.style.display = 'block' :document.all.targetInstructions.style.display = 'none';
			element.checked ? document.all.sourceInstructions.style.display = 'none' :document.all.sourceInstructions.style.display = 'block';
			element.checked ? document.all.BarcodeDesc.style.display = 'none' :document.all.BarcodeDesc.style.display = 'block';
		}
	}
	
-->
</script>
</head>
<body>
<center>
<FORM name="form1" method="post" encType="multipart/form-data" action="CreatePlatesFromPipetteLog_action.asp">
<input type ="hidden" value="test" name="test">
<table border="0">
	<tr>
		<td colspan="2">
			<span class="GuiFeedback"><center>
				<span id="targetInstructions" style="display:block">Select the pipette log and enter information about the target plate(s).</span>
				<span id="sourceInstructions" style="display:none">Select the pipette log and enter information about the source plate(s).<BR>This will create the source plate(s) with no compound information.</span>
			</center></span><br><br>
		</td>
	</tr>
	<tr height="25">
		<td colspan="2" align="center"><input type="radio" name="ActionType" value="target" onclick="ActionType_OnClick(this);" checked>Create Target Plate(s)<input type="radio" name="ActionType" value="source" onclick="ActionType_OnClick(this);">Create Source Plate(s)</td>
	</tr>
<!-- begin custom for Celera -->
	<tr height="25">
		<td align="right" nowrap width="200">
			<span class="required" title="Import Template">Import Template:</span>
		</td>
		<td>
			<select name="Template">
				<option value="1">CS Default-Tab Delimited</option>
				<option value="2">Source/Target Plate-Tab Delimited</option>
				<option value="3">Source/Target Well-Tab Delimited</option>
				<option value="4">Source/Target Unified-Comma Delimited</option>
				<!-- 
				<option value="5">Single Plate1-Tab Delimited</option>
				<option value="6">Single Plate2-Tab Delimited</option>
				-->
				<option value="7">Source/Target Z Pattern-Comma Delimited</option>
			</select>
			<a href="#" onclick="OpenDialog('/cheminv/help/userguide/plate_inventory_pipette_formats.htm','HelpDiag',1);" CLASS="MenuLink">Import Template Help</a>
		</td>
	</tr>
<!-- end custom for Celera -->
	<tr height="25">
		<td align="right" nowrap width="200">
			<span class="required" title="Path of the Pipette Log">Pipette Log:</span>
		</td>
		<td>
			<INPUT type="File" name="File1">
		</td>
	</tr>
	<tr height="25">
		<td align="right" valign="top" nowrap width="200">
			<span class="required">Location ID:</span>
		</td>
		<td>
			&nbsp;<%=GetBarcodeIcon()%>&nbsp;<%ShowLocationPicker "document.form1", "iLocation_ID_FK", "Location_BarCode", "Location_Name", 10, 49, false%> 
		</td>
	</tr>
	<tr height="25">
		<%=ShowPickList("<span class=""required"">Plate Type:</span>", "iPlate_Type_ID_FK", Plate_Type_ID_FK, "SELECT plate_type_id AS Value, plate_type_name AS DisplayText FROM inv_plate_types ORDER BY lower(DisplayText) ASC")%>
	</tr>
	<tr height="25">
		<%=ShowPickList("<span class=""required"">Plate Format:</span>", "iPlate_Format_ID_FK", Plate_Format_ID_FK, "SELECT plate_format_id AS Value, plate_format_name AS DisplayText FROM inv_plate_format ORDER BY lower(DisplayText) ASC")%>
	</tr>
	<tr height="25">
		<%=ShowPickList("<span class=""required"">Plate Status:</span>", "iStatus_ID_FK", Status_ID_FK,"SELECT Enum_ID AS Value, Enum_Value AS DisplayText FROM inv_enumeration, inv_enumeration_set WHERE Eset_name = 'Plate Status' and eset_id_fk = eset_id ORDER BY lower(DisplayText) ASC")%>
	</tr>
	<tr>
	<td colspan="2">
	<div id="BarcodeDesc" style="display:none">
	<table cellspacing="0">
	<tr>
		<td align="right" valign="top" nowrap width="200">Barcode Description:</td>
		<td>
			<%=ShowSelectBox2("BarcodeDescID", Barcode_desc_ID,"SELECT barcode_desc_id AS Value, barcode_desc_name AS Displaytext FROM inv_barcode_desc ORDER BY lower(DisplayText) ASC", null, "None", "-1")%>&nbsp;(Selecting an option will override a mapped barcode)
		</td>
	</tr>
	</table>
	</div>
	</td>
	</tr>
	<tr height="25">
		<td align="right" nowrap width="200">Unit of quantity:</td>
		<td><%=ShowSelectBox2("iQty_Unit_FK", Qty_Unit_FK, "SELECT UNIT_ID AS Value, Unit_Name AS DisplayText FROM inv_units WHERE Unit_type_id_FK IN (1,2) ORDER BY lower(DisplayText) ASC", null, "Select a Unit", "null")%></td>
	</tr>
	<tr height="25">
		<%=ShowInputBox2("Initial Quantity:", "Qty_Initial", 200, 15, "", False, False)%>
	</tr>
	<tr height="25">
		<td align="right" nowrap width="200">Unit of Concentration:</td>
		<td><%=ShowSelectBox2("iConc_Unit_FK", Conc_Unit_FK,"SELECT UNIT_ID AS Value, Unit_Name AS DisplayText FROM inv_units WHERE Unit_type_id_FK IN (3) ORDER BY lower(DisplayText) ASC", null, "Select a Unit", "null")%></td>
	</tr>
	<tr height="25">
		<%=ShowInputBox2("Concentration:", "Concentration", 200, 15, "", False, false)%>
	</tr>
	<tr height="25">
		<td align="right" nowrap width="200">Select Solvent:</td>
		<td><%=ShowSelectBox2("iSolvent_ID_FK",solvent_id_fk,"SELECT solvent_id AS Value, solvent_name AS DisplayText FROM " & Application("CHEMINV_USERNAME") & ".inv_solvents ORDER BY lower(DisplayText) ASC", null, "Select a Solvent", "null")%></td>
	</tr>
	<tr>
		<td colspan="2" align="right"> 
			<a HREF="#" onclick="if (DialogWindow) {DialogWindow.close()}; window.close(); return false;"><img SRC="/cheminv/graphics/cancel_dialog_btn.gif" border="0" WIDTH="61" HEIGHT="21"></a>
			
			
			
			<a HREF="#" onclick="Validate(); return false;"><img SRC="/cheminv/graphics/ok_dialog_btn.gif" border="0" WIDTH="61" HEIGHT="21"></a>
		</td>
	</tr>	
</table>	
</form>
</center>
</body>
</html>
