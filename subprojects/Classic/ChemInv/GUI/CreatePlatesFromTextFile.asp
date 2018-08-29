<%@ Language=VBScript %>
<!--#INCLUDE VIRTUAL = "/cheminv/gui/guiUtils.asp"-->
<!--#INCLUDE VIRTUAL = "/cheminv/api/apiUtils.asp"-->
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/ado.inc"-->
<%
Dim Conn
Dim Cmd
Dim RS

Call GetInvConnection()
'-- get list of valid import templates for creating source plates that require barcode to be entered here
source1MapList = " "
Call GetInvCommand("{CALL " & Application("CHEMINV_USERNAME") & ".DATAMAPS.GetValidDataMaps(?)}", adCmdText)	
Cmd.Parameters.Append Cmd.CreateParameter("PMAPTYPE", 200, 1, 10, "source1")
Cmd.Properties ("PLSQLRSet") = TRUE  
Set rsSource = Cmd.Execute
Cmd.Properties ("PLSQLRSet") = FALSE
if not (rsSource.BOF or rsSource.EOF) then
	arrSourceMaps = rsSource.GetRows
	for j = 0 to ubound(arrSourceMaps,2)
		source1MapList = source1MapList & arrSourceMaps(0,j) & ","
		'Response.Write arrSourceMaps(0,j) &  "<BR>"
	next
	source1MapList = trim(left(source1MapList,len(source1MapList)-1))
	'Response.Write sourceMapList
end if
Set rsSource = nothing

'-- get list of valid import templates for creating source plates where source barcode is mapped
source2MapList = " "
Call GetInvCommand("{CALL " & Application("CHEMINV_USERNAME") & ".DATAMAPS.GetValidDataMaps(?)}", adCmdText)	
Cmd.Parameters.Append Cmd.CreateParameter("PMAPTYPE", 200, 1, 10, "source2")
Cmd.Properties ("PLSQLRSet") = TRUE  
Set rsSource = Cmd.Execute
Cmd.Properties ("PLSQLRSet") = FALSE
if not (rsSource.BOF or rsSource.EOF) then
	arrSourceMaps = rsSource.GetRows
	for j = 0 to ubound(arrSourceMaps,2)
		source2MapList = source2MapList & arrSourceMaps(0,j) & ","
		'Response.Write arrSourceMaps(0,j) &  "<BR>"
	next
	source2MapList = trim(left(source2MapList,len(source2MapList)-1))
	'Response.Write sourceMapList
end if
Set rsSource = nothing


'-- get list of valid import templates for creating target plates
targetMapList = " "
Call GetInvCommand("{CALL " & Application("CHEMINV_USERNAME") & ".DATAMAPS.GetValidDataMaps(?)}", adCmdText)	
Cmd.Parameters.Append Cmd.CreateParameter("PMAPTYPE", 200, 1, 10, "target")
Cmd.Properties ("PLSQLRSet") = TRUE  
Set rsTarget = Cmd.Execute
Cmd.Properties ("PLSQLRSet") = FALSE
if not (rsTarget.BOF or rsTarget.EOF) then
	arrTargetMaps = rsTarget.GetRows
	for j = 0 to ubound(arrTargetMaps,2)
		targetMapList = targetMapList & arrTargetMaps(0,j) & ","
		'Response.Write arrTargetMaps(0,j) &  "<BR>"
	next
	targetMapList = trim(left(targetMapList,len(targetMapList)-1))
	'Response.Write targetMapList
end if
Set rsTargetMaps = nothing

'-- add custom maps to the lists
source2MapList = source2MapList & ",2,3,4,7"
targetMapList = targetMapList & ",2,3,4,7"

'Selecting all Plate Format with 384 wells 
GetInvConnection()
SQL = "Select  INV_PLATE_FORMAT.PLATE_FORMAT_ID FROM INV_PLATE_FORMAT, INV_PHYSICAL_PLATE, INV_GRID_FORMAT Where INV_PLATE_FORMAT.PHYS_PLATE_ID_FK= INV_PHYSICAL_PLATE.PHYS_PLATE_ID and INV_PHYSICAL_PLATE.GRID_FORMAT_ID_FK=INV_GRID_FORMAT.GRID_FORMAT_ID and INV_GRID_FORMAT.row_count=16 and INV_GRID_FORMAT.col_count=24"
SET rs = Conn.Execute(SQL)
dim temp
do while not rs.eof 
	if temp="" then 
		temp=rs("PLATE_FORMAT_ID")
	else 
		temp= temp & "," & rs("PLATE_FORMAT_ID")
	end if 		
	rs.movenext
loop 
arrPlateFormatCount=split(temp,",")
%>
<html>
<head>
<title><%=Application("appTitle")%> -- Create Plates From Text File</title>
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
		var source1MapList = '<%=source1MapList%>';
		var source2MapList = '<%=source2MapList%>';
		var targetMapList = '<%=targetMapList%>';
		var templateID = document.form1.Template.options[document.form1.Template.selectedIndex].value;
		<% if Application("ENABLE_OWNERSHIP")="TRUE" then %>
		!document.form1.Ownershiplst ? document.form1.PrincipalID.value="" :  document.form1.PrincipalID.value=document.form1.Ownershiplst.value;
		<% end if %>
		var bPlateCheck = false;
		// Destination is required
		if (document.form1.File1.value.length == 0) {
			errmsg = errmsg + "- File path is required.\r";
			bWriteError = true;
		}
		if (document.form1.iLocation_ID_FK.value.length > 0)
		{	
			sessionStorage.setItem("LocationID", document.form1.iLocation_ID_FK.value);
			LocationID = sessionStorage.getItem("LocationID");
		}
		else if (document.form1.iLocation_ID_FK.value.length == 0)
		{
			document.form1.iLocation_ID_FK.value = sessionStorage.getItem("LocationID");
		}
		PlateTypeID = document.form1.iPlate_Type_ID_FK.value;
		
		// Validate import template
		if (document.form1.ActionType[0].checked){
			//if (targetMapList.indexOf(document.form1.Template.options[document.form1.Template.selectedIndex].value) == -1){
			if (!InList(targetMapList, templateID)) {
				errmsg = errmsg + "- Invalid Import Template for creating Target Plates.\rPlease choose an Import Template that has Target Plate Barcode,Target Well, Source Plate Barcode, Source Well mapped.\r";
				bWriteError = true;
			}
		}
		else if (document.form1.ActionType[1].checked){
			//if (source1MapList.indexOf(document.form1.Template.options[document.form1.Template.selectedIndex].value) == -1){
			if (!InList(source1MapList, templateID) && !InList(source2MapList, templateID)) {
				errmsg = errmsg + "- Invalid Import Template for creating Source Plates.\rPlease choose an Import Template that has Source Well mapped.\r";
				bWriteError = true;
			}
		}

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
		<%if Application("ENABLE_OWNERSHIP")="TRUE" then%>
		if (document.form1.PrincipalID.value.length==0){
		        errmsg = errmsg + "- Plate Admin is required.\r";
				bWriteError = true;
		}
        if (!bWriteError && document.form1.LocationAdmin.value!='' && document.form1.LocationAdmin.value !=document.form1.PrincipalID.value){
            if (confirm("- The Location Admin and Plate Admin are not the same,\r Do you really want to continue?")!=true){     
                return;
            }
         }
		<% end if %>
		//Check for Z-Patteren plates - These plates can have 384 wells plates as target plate 
		if ((document.form1.Template.value==7)){
		<% for each val in arrPlateFormatCount%>
		if (document.form1.iPlate_Format_ID_FK.value==<%=val%>){
		bPlateCheck=true;
		}
		<%Next%>
		if (bPlateCheck==false){
		errmsg = errmsg + "- Z-Patteren plates can have 384 wells only.\r";
			bWriteError = true;
		}
		}
		// Barcode Desc is required if the Import Template doesn't map Source Plate Barcode
		if (document.form1.ActionType[1].checked){
			// check if this map requires source barcode be mapped
			bInList = InList(source1MapList, templateID);
			//if (source1MapList.indexOf(document.form1.Template.options[document.form1.Template.selectedIndex].value) > -1){
			if (bInList) {
				if (document.form1.BarcodeDescID.value == "-1") {
					errmsg = errmsg + "- Barcode Description is required for this Import Template.\r";
					bWriteError = true;
				}
			}
		}
		
		// LocationID must be valid for this plate type
		if (!(IsPlateTypeAllowed('',document.form1.iLocation_ID_FK.value,document.form1.iPlate_Type_ID_FK.value,'') == 1)){
			errmsg = errmsg + "- This location does not accept this plate type.\r";
			bWriteError = true;
		}
					
		if (bWriteError){
			alert(errmsg);
		}
		else{
			document.form1.submit();
		}
	}
	
	function InList(list, value) {
		var inList = false;
		arrList = list.split(",");
		for(i=0;i<arrList.length;i++){
			if (arrList[i] == value) {
				inList = true;
			}
		}
		return inList;
	}
	function ActionType_OnClick(element) {
		if (element.value == "source") {
			element.checked ? document.all.sourceInstructions.style.display = 'block' :document.all.sourceInstructions.style.display = 'none';
			element.checked ? document.all.targetInstructions.style.display = 'none' :document.all.targetInstructions.style.display = 'block';
			element.checked ? document.all.BarcodeDesc.style.display = 'block' :document.all.BarcodeDesc.style.display = 'none';
			element.checked ? document.all.TargetAmtText.style.display = 'none' :document.all.BarcodeDesc.style.display = 'display';
			element.checked ? document.all.SourceAmtText.style.display = 'block' :document.all.BarcodeDesc.style.display = 'none';
		}
		else if (element.value=="target"){
			element.checked ? document.all.targetInstructions.style.display = 'block' :document.all.targetInstructions.style.display = 'none';
			element.checked ? document.all.sourceInstructions.style.display = 'none' :document.all.sourceInstructions.style.display = 'block';
			element.checked ? document.all.BarcodeDesc.style.display = 'none' :document.all.BarcodeDesc.style.display = 'block';
			element.checked ? document.all.TargetAmtText.style.display = 'block' :document.all.BarcodeDesc.style.display = 'none';
			element.checked ? document.all.SourceAmtText.style.display = 'none' :document.all.BarcodeDesc.style.display = 'block';
		}
	}
	
-->
</script>
</head>
<body>
<center>
<FORM name="form1" method="post" encType="multipart/form-data" action="CreatePlatesFromTextFile_action.asp">
<input type ="hidden" value="test" name="test">
<% if Application("ENABLE_OWNERSHIP")="TRUE" then %>
<input TYPE="hidden" NAME="OwnerShipGroupList" id="OwnerShipGroupList" Value="<%=GetOwnerShipGroupList()%>">
<input TYPE="hidden" NAME="OwnerShipUserList" id="OwnerShipUserList" Value="<%=GetOwnerShipUserList()%>">
<input TYPE="hidden" NAME="PrincipalID" id="PrincipalID" Value>
<input TYPE="hidden" NAME="LocationAdmin" id="LocationAdmin">
<% end if %>
<input TYPE="hidden" NAME="tempCsUserName" id="tempCsUserName" Value="<%=Session("UserName" & "cheminv")%>" >
<input type="hidden" name="tempCsUserID" id="tempCsUserID" value="<%=Server.URLEncode(CryptVBS(Session("UserID" & "cheminv"), "ChemInv\API\GetBatchInfo.asp"))%>" />
<table border="0">
	<tr>
		<td colspan="2">
			<span class="GuiFeedback"><center>
				<span id="targetInstructions" style="display:block">Select the text file and enter information about the target plate(s).</span>
				<span id="sourceInstructions" style="display:none">Select the text file and enter information about the source plate(s).<BR>This will create the source plate(s) with no compound information.</span>
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
<%
	SQL = "SELECT data_map_id AS value, data_map_name AS displayText FROM inv_data_maps ORDER BY lower(displayText) ASC"
	Set RS = Conn.Execute(SQL)
	
	while not RS.EOF
		Response.Write "<OPTION VALUE=""" & RS("value") & """>" & RS("displayText") & "</OPTION>" & chr(13)
		RS.MoveNext
	wend

%>				
<!-- begin custom maps -->
				<!--<option value="1">CS Default-Tab Delimited</option>-->
				<option value="2">Source/Target Plate-Tab Delimited</option>
				<option value="3">Source/Target Well-Tab Delimited</option>
				<option value="4">Source/Target Unified-Comma Delimited</option>
				<option value="7">Source/Target Z Pattern-Comma Delimited</option>
<!-- end custom maps -->
			</select>
			<a href="#" onclick="OpenDialog('<%=Application("HelpFile")%>','HelpDiag',1); return false;">Import Template Help</a>
		</td>
	</tr>
<!-- end custom for Celera -->
	<tr height="25">
		<td align="right" nowrap width="200">
			<span class="required" title="Path of the Text File">Text File:</span>
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
           <%  if Application("ENABLE_OWNERSHIP")="TRUE" then
                authorityFunction = "SetOwnerInfo('location')"
            else
                authorityFunction= ""
            end if %>
			&nbsp;<%=GetBarcodeIcon()%>&nbsp;<%ShowLocationPicker3 "document.form1", "iLocation_ID_FK", "Location_BarCode", "Location_Name", 10, 49, false, authorityFunction%> 
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
	<tr height="25">
		<td colspan="2" align="center"><i>Entering values for any of the following options will override the mapped value.</i></td>
	</tr>
	<td colspan="2">
	<div id="BarcodeDesc" style="display:none">
	<table>
	<tr>
		<td align="right" valign="top" nowrap width="200">Barcode Description:</td>
		<td>
			<%=ShowSelectBox2("BarcodeDescID", Barcode_desc_ID,"SELECT barcode_desc_id AS Value, barcode_desc_name AS Displaytext FROM inv_barcode_desc ORDER BY lower(DisplayText) ASC", null, "None", "-1")%>
		</td>
	</tr>
	</table>
	</div>
	</td>
	</tr>
	<tr height="25">
		<td align="right" width="200" valign=top nowrap>
			<span id="TargetAmtText">Source Amount Taken:</span>
			<span id="SourceAmtText" style="display:none">Initial Quantity:</span>
		</td>
		<td>
			<input TYPE="text" SIZE="15" Maxlength="50" NAME="iQty_Initial" VALUE="">
			<%=ShowSelectBox3("iQty_Unit_FK", Qty_Unit_FK, "SELECT unit_id AS Value, unit_name AS DisplayText FROM inv_units WHERE unit_type_id_fk in (1,2) ORDER BY lower(DisplayText) ASC",null,"Select a Unit",null,"")%>
		</td>
	</tr>
	<tr height="25">
		<td align="right" width="200" valign=top nowrap>Concentration:</td>
		<td>
			<input TYPE="text" SIZE="15" Maxlength="50" NAME="iConcentration" VALUE="">
			<%=ShowSelectBox3("iConc_Unit_FK", Conc_Unit_FK, "SELECT unit_id AS Value, unit_name AS DisplayText FROM inv_units WHERE unit_type_id_fk in (3) ORDER BY lower(DisplayText) ASC", null, "Select a Unit", null,"")%>
		</td>
	</tr>
	<tr height="25">
		<td></td>
		<td align="left">Solvent Added to Plates</td>
	</tr>
	<tr height="25">
		<td align="right" nowrap width="200">Select Solvent:</td>
		<td><%=ShowSelectBox2("iSolvent_ID_FK",solvent_id_fk,"SELECT solvent_id AS Value, solvent_name AS DisplayText FROM " & Application("CHEMINV_USERNAME") & ".inv_solvents ORDER BY lower(DisplayText) ASC", null, "Select a Solvent", "null")%></td>
	</tr>
	<tr>
		<td align="right" width="200" nowrap>Solvent Volume:</td>
		<td>
			<input TYPE="text" SIZE="15" Maxlength="50" NAME="iSolvent_Volume" VALUE="">
			<%=ShowSelectBox3("iSolvent_Volume_Unit_ID_FK", Solvent_Volume_Unit_ID_FK,"SELECT unit_id AS Value, unit_name AS DisplayText FROM inv_units WHERE unit_type_id_fk in (1) ORDER BY lower(DisplayText) ASC",null, "Select a Unit",null,null)%>
		</td>
	</tr>
	<% if Application("ENABLE_OWNERSHIP")="TRUE" then %>
	<tr height="25">
		<td align=right> <span title="Pick an option from the list" class="required">Plate Admin:</span></td>
		<td align=left>
		<table border=.5>
		<tr><td><input type="Radio" name="Ownership" name="User_cb" id="User_cb" onclick="SetOwnerInfo('user');"/>by user</td>
		<td><input type="Radio" name="Ownership" name="Group_cb" id="Group_cb" onclick="SetOwnerInfo('group');" />by Group</td></tr>
		<tr><td colspan="2"><SELECT id="Ownershiplst" ><OPTION></OPTION></SELECT></td></tr></table></td>
	</tr>
	<% end if %>
	<tr>
		<td colspan="2" align="right"> 
			<a HREF="#" onclick="window.location='/cheminv/gui/menu.asp'; return false;"><img SRC="/cheminv/graphics/cancel_dialog_btn.gif" border="0" WIDTH="61" HEIGHT="21"></a>
			<a HREF="#" onclick="Validate(); return false;"><img SRC="/cheminv/graphics/ok_dialog_btn.gif" border="0" WIDTH="61" HEIGHT="21"></a>
		</td>
	</tr>	
</table>	
<% if Application("ENABLE_OWNERSHIP")="TRUE" and PrincipalID="" then %>
<script language="javascript">
    //set the inital location group
    SetOwnerInfo('location');
 </script>
<%end if %>	
</form>
</center>
</body>
</html>
