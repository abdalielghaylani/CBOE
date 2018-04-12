<%@ Language=VBScript %>
<!--#INCLUDE VIRTUAL = "/cheminv/gui/guiUtils.asp"-->
<!--#INCLUDE VIRTUAL = "/cheminv/api/apiUtils.asp"-->
<%
Dim Conn
Dim RS

action = Request("action")
PhysPlateID = Request("ID")

RowUseLetters= "0"
ColUseLetters= "0"
NumberStartCorner= "13"
NumberDirection= "17"
isPreBarcoded = "0"
SupplierID = "0"
ZeroPaddingCount = "2"
cell_naming = "0"

If action <> "create" then
	Call GetInvConnection()
	SQL = "SELECT grid_format_id_fk, phys_plate_name, supplier_id_fk, is_pre_barcoded, well_capacity, capacity_unit_id_fk, is_active, inv_grid_format.row_count, inv_grid_format.col_count, inv_grid_format.row_prefix, inv_grid_format.col_prefix, inv_grid_format.row_use_letters, inv_grid_format.col_use_letters, inv_grid_format.name_separator, inv_grid_format.number_start_corner, inv_grid_format.number_direction, inv_grid_format.zero_padding_count FROM inv_physical_plate, inv_grid_format WHERE inv_physical_plate.grid_format_id_fk = inv_grid_format.grid_format_id AND inv_physical_plate.phys_plate_ID=?"
	'Response.Write SQL
	'Response.end
	set Cmd = server.createobject("ADODB.Command")
	Cmd.CommandText = SQL
	Cmd.ActiveConnection = Conn
	Cmd.Parameters.Append Cmd.CreateParameter("PhysPlateID",131, 1, 0, PhysPlateID)
	Set RS= Cmd.Execute
	GridFormatID = RS("grid_format_id_fk").value
	PhysPlateName=RS("phys_plate_name").value
	SupplierID= RS("supplier_id_fk").value
	isPreBarcoded= RS("is_pre_barcoded").value
	WellCapacity= RS("well_capacity").value
	CapacityUnitID= RS("capacity_unit_id_fk").value
	is_active= RS("is_active").value
	RowCount= RS("row_count").value
	ColCount= RS("col_count").value
	RowPrefix= RS("row_prefix").value
	ColPrefix= RS("col_prefix").value
	RowUseLetters= RS("row_use_letters").value
	ColUseLetters= RS("col_use_letters").value
	NameSeparator= RS("name_separator").value
	NumberStartCorner= RS("number_start_corner")
	NumberDirection= RS("number_direction")
	ZeroPaddingCount = RS("zero_padding_count")
	if isNull(ZeroPaddingCount) then ZeroPaddingCount = "2"
End if
if cell_naming = "1" then
	displayNamingRowCol = "none"
	displayNamingSeq = "block"
else
	displayNamingRowCol = "block"
end if
%>
<html>
<head>
<title><%=Ucase(Left(action,1)) & Mid(action, 2, Len(action)-1)%> Physical Plate Format</title>
<style>
	.namingRowCol { display:<%=displayNamingRowCol%>; }
	.namingSeq { display:<%=displayNamingSeq%>; }
</style>
<script LANGUAGE="javascript" src="/cheminv/Choosecss.js"></script>
<script LANGUAGE="javascript" src="/cheminv/gui/validation.js"></script>
<script language="javascript" src="/cheminv/utils.js"></script>
<script language="javascript">
	// Toggles display of naming preference
	function toggleCellNaming(namVal) {
		if (namVal == 1) {
			AlterCSS('.namingRowCol','display','none');
		}else{
			AlterCSS('.namingRowCol','display','block');
		}
	}	
</script>
<script language="JavaScript">
<!--
	var currGridFormat = "<%=currGridFormat%>";
	window.focus();
	
	function Validate(){
		
		var bWriteError = false;
		var errmsg = "Please fix the following problems:\r";
	<%if action <> "delete" then%>
		// Name is required
		if (document.form1.pPhysPlateName.value.length == 0) {
			errmsg = errmsg + "- Name is required.\r";
			bWriteError = true;
		}
		// QtyMax is required
		if (document.form1.pWellCapacity.value.length == 0) {
			errmsg = errmsg + "- Well Capacity is required.\r";
			bWriteError = true;
		}
		else{
			// Capacity must be a number
			if (!isPositiveNumber(document.form1.pWellCapacity.value)){
			errmsg = errmsg + "- Well Capacity must be a positive number.\r";
			bWriteError = true;
			}
			if (document.form1.pWellCapacity.value > 999999999){
			errmsg = errmsg + "- Maximum Well Capacity allowed is 999999999.\r";
			bWriteError = true;
			}
		}
		// RowCount is required
		if (document.form1.pRowCount.value.length == 0) {
			errmsg = errmsg + "- Row Count is required.\r";
			bWriteError = true;
		}
		else{
			// RowCount must be a number
			if (!isPositiveNumber(document.form1.pRowCount.value)){
			errmsg = errmsg + "- Row Count must be a positive number.\r";
			bWriteError = true;
			}
			if (document.form1.pRowCount.value > 100){
			errmsg = errmsg + "- Maximum Row Count allowed is 100.\r";
			bWriteError = true;
			}
		}
		// ColCount is required
		if (document.form1.pColCount.value.length == 0) {
			errmsg = errmsg + "- Column Count is required.\r";
			bWriteError = true;
		}
		else{
			// ColCount must be a number
			if (!isPositiveNumber(document.form1.pColCount.value)){
			errmsg = errmsg + "- Column Count must be a positive number.\r";
			bWriteError = true;
			}
			if (document.form1.pColCount.value > 100){
			errmsg = errmsg + "- Maximum Column Count allowed is 100.\r";
			bWriteError = true;
			}
		}
		<%end if%>
		if (bWriteError){
			alert(errmsg);
		}
		else{
			document.form1.submit();
		}
	}
//-->
</script>
</head>

<body>
<center>
<form name="form1" action="ManagePhysPlate_action.asp?action=<%=action%>" method="POST">
<input type="hidden" name="pPhysPlateID" value="<%=PhysPlateID%>">
<input type="hidden" name="pGridFormatID" value="<%=GridFormatID%>">

<table border="0">
<%if action = "delete" then%>
	<tr>
		<td colspan="2">
			<span class="GuiFeedback">Are you sure you want to delete this Physical Plate Type:</span><br><br>
		</td>
	</tr>
	<tr>
		<td align="right">
			<span class="required">Physical Plate Name:</span>
		</td>
		<td>
			<input TYPE="tetx" SIZE="25" Maxlength="50" NAME="pPhysPlateName" value="<%=PhysPlateName%>" disabled>
		</td>
	</tr>

<%else%>
	<tr>
		<td colspan="2">
			<span class="GuiFeedback">Physical Plate Format Attributes:</span><br><br>
		</td>
	</tr>
	<tr>
		<td align="right">
			<span class="required">Physical Plate Name:</span>
		</td>
		<td>
			<input TYPE="tetx" SIZE="25" Maxlength="50" NAME="pPhysPlateName" value="<%=PhysPlateName%>">
		</td>
	</tr>
	<tr>
		<td align="right">
			<span class="required">Well Capacity:</span>
		</td>
		<td>
			<input TYPE="tetx" SIZE="10" Maxlength="10" NAME="pWellCapacity" value="<%=WellCapacity%>">
			<%=ShowSelectBox2 ("pCapacityUnitID", CapacityUnitID, "SELECT UNIT_ID AS Value, Unit_Name AS DisplayText FROM inv_units WHERE Unit_type_id_FK = 1 ORDER BY lower(DisplayText) ASC", 30, "", "")%>
		</td>
	</tr>
	<tr>
		<td align="right" nowrap>
			<span class="required">Row Count:<span>
		</td>
		<td>
			<input TYPE="tetx" SIZE="10" Maxlength="3" NAME="pRowCount" value="<%=RowCount%>">
		</td>
	</tr>
	<tr>
		<td align="right" nowrap>
			<span class="required">Column Count:<span>
		</td>
		<td>
			<input TYPE="tetx" SIZE="10" Maxlength="3" NAME="pColCount" value="<%=ColCount%>">
		</td>
	</tr>
	<tr>
		<td align="right" valign="top" nowrap>
			<span>Cell Naming:<span>
		</td>
		<td>
			<input TYPE="radio" SIZE="3" Maxlength="3" NAME="p_cell_naming" value="0" <%if cell_naming = "0" then Response.Write(" checked") %> onclick="toggleCellNaming(0)">Row name and Column name&nbsp;(i.e. Row1Col1, Row1Col2)<br />
			<input TYPE="radio" SIZE="3" Maxlength="3" NAME="p_cell_naming" value="1" <%if cell_naming = "1" then Response.Write(" checked") %> onclick="toggleCellNaming(1)">Row to Column Sequence&nbsp;(i.e. [Row Prefix]1, [Row Prefix]2...[Row Prefix][n+1])
		</td>
	</tr>
	<tr class="namingSeq">
		<td align="right" nowrap >
			<span>Row Prefix:<span>
		</td>
		<td>
			<input TYPE="tetx" SIZE="3" Maxlength="3" NAME="pRowPrefix" value="<%=RowPrefix%>">
		</td>
	</tr>
	<tr class="namingRowCol">
		<td align="right" nowrap >
			<span>Column Prefix:<span>
		</td>
		<td>
			<input TYPE="tetx" SIZE="3" Maxlength="3" NAME="pColPrefix" value="<%=ColPrefix%>">
		</td>
	</tr>
	<tr class="namingRowCol" >
		<td align="right" nowrap>
			<span>Row Labels:<span>
		</td>
		<td>
			<select name="pRowUseLetters">
				<option value="0">Use Numbers
				<option value="1">Use Letters
			</select>
		</td>
	</tr>
	<tr class="namingRowCol">
		<td align="right" nowrap>
			<span>Column Labels:<span>
		</td>
		<td>
			<select name="pColUseLetters">
				<option value="0">Use Numbers
				<option value="1">Use Letters
			</select>
		</td>
	</tr>
	<tr>
		<td align="right" nowrap>
			<span title="Final number of digits with zero padding.  If # is 2 then 1 becomes 01.  If # is 3 then 1 becomes 001.">Zero Pad to # of digits:<span>
		</td>
		<td>
			<select name="pZeroPaddingCount">
				<option value="0">No Padding</option>
				<option value="2">2</option>
				<option value="3">3</option>
			</select>
		</td>
	</tr>
	<tr class="namingRowCol">
		<td align="right" nowrap>
			<span>Name Separator:<span>
		</td>
		<td>
			<input TYPE="tetx" SIZE="1" Maxlength="1" NAME="pNameSeparator" value="<%=NameSeparator%>">
		</td>
	</tr>
	<tr>
		<td align="right" nowrap>
			<span>Start at:<span>
		</td>
		<td>
			<select name="pNumberStartCorner">
				<option value="13">Upper Left
				<option value="14">Upper Right
				<option value="15">Lower Left
				<option value="16">Lower Right
			</select>
		</td>
	</tr>
	<tr>
		<td align="right" nowrap>
			<span>Fill Order:<span>
		</td>
		<td>
			<select name="pNumberDirection">
				<option value="17">Rows First
				<option value="18">Columns First
			</select>
		</td>
	</tr>
	<tr>
		<td align="right">
			<span title="Pick an option from the list">Supplier Name:</span>
		</td>
		<td>
			<%= ShowSelectBox2("SupplierID", SupplierID, "SELECT Supplier_ID AS Value, Supplier_Short_Name AS DisplayText FROM inv_suppliers ORDER BY lower(Supplier_Short_Name) ASC", 25, "", "")%>        
		</td>		
	</tr>
	<tr>
		<td align="right" nowrap>
			<span>Pre-Barcoded?:<span>
		</td>
		<td>
			<select name="pIsPreBarcoded">
				<option value="0">No
				<option value="1">Yes
			</select>
		</td>
	</tr>
<%end if%>
	<tr>
		<td colspan="2" align="right"> 
			<a HREF="#" onclick="history.back(); return false;"><img SRC="../graphics/cancel_dialog_btn.gif" border="0" WIDTH="61" HEIGHT="21"></a><a HREF="#" onclick="Validate(); return false;"><img SRC="../graphics/ok_dialog_btn.gif" border="0" WIDTH="61" HEIGHT="21"></a>
		</td>
	</tr>
</table>	
</form>
</center>
<%if action <> "delete" then%> 
<script LANGUAGE="javascript">
	document.form1.pRowUseLetters.value = "<%=RowUseLetters%>";
	document.form1.pColUseLetters.value = "<%=ColUseLetters%>";
	document.form1.pNumberStartCorner.value = "<%=NumberStartCorner%>";
	document.form1.pNumberDirection.value = "<%=NumberDirection%>";
	document.form1.pIsPreBarcoded.value = "<%=IsPreBarcoded%>";
	document.form1.pZeroPaddingCount.value = "<%=ZeroPaddingCount%>";
</script>
<%end if%>
</body>
</html>
