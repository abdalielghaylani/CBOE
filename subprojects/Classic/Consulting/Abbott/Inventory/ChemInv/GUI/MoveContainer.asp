<%@ Language=VBScript %>
<!--#INCLUDE VIRTUAL = "/cheminv/gui/guiUtils.asp"-->
<!--#INCLUDE VIRTUAL = "/cheminv/api/apiUtils.asp"-->
<%

'Response.Write(Request.Form & "<br><br>")
'Response.Write(Request.QueryString & "<br><br>")

Dim Conn
Set myDict = multiSelect_dict
if Lcase(Request("multiSelect")) = "true" then 
	ContainerID = DictionaryToList(myDict)
	ContainerName =  myDict.count & " containers will be moved"
Else
	ContainerID = Session("ContainerID")
	ContainerName = Session("ContainerName")
End if

AssignToRack = Request("AssignToRack")
ShowFullRackList = lCase(Request("ShowFullRackList"))
if ShowFullRackList = "true" then CheckFullRack = " checked"

'-- If user has selected a rack or contents of rack, set the default location to parent of rack
rackParent = ""
if isBlank(Session("CurrentLocationID")) then Session("CurrentLocationID") = 0
if APPLICATION("RACKS_ENABLED") then
	'-- ValidateLocationIsRack returns COLLAPSE_CHILD_NODES, PARENT_ID for container
	'-- Only validate location if location is not ROOT
	if Session("CurrentLocationID") > 0 then
	rackTemp = split(ValidateLocationIsRack(Session("CurrentLocationID")),"::")
	isRack = rackTemp(0)
	rackParent = rackTemp(1)
	end if
end if

if Request("LocationID") <> "" and ShowFullRackList = "" then
	if Request("GetSessionLocationID") = "" then 
		LocationID = Request("LocationID")
		Session("CurrentLocationID") = LocationID
	else
		LocationID = Session("CurrentLocationID")
	end if 
elseif ShowFullRackList = "true" then
	LocationID = rackParent
else
	LocationID = Session("CurrentLocationID")
end if
if isBlank(LocationID) then LocationID = 0
%>
<html>
<head>
<title><%=Application("appTitle")%> -- Move Inventory Container</title>
<script language="javascript" src="/cheminv/choosecss.js"></script>
<script language="javascript" src="/cheminv/utils.js"></script>
<script language="javascript" src="/cheminv/gui/validation.js"></script>

<style>
	.singleRackElement {
		display:none;
	}
	.suggestRackList {
		display:none;
	}
	.locationRackList {
		display:none;
	}
	.rackLabel {color:#000;}
</style>

<script language="javascript">
<!--Hide JavaScript
	var DialogWindow;
	window.focus();
	function ValidateMove(){
		
		var bWriteError = false;
		var errmsg = "Please fix the following problems:\r";
		// Destination is required
		if (document.form1.LocationID.value.length == 0) {
			errmsg = errmsg + "- Destination is required.\r";
			bWriteError = true;
		}
		else{
			// Destination must be a number
			if (!isNumber(document.form1.LocationID.value)){
			errmsg = errmsg + "- Destination must be a number.\r";
			bWriteError = true;
			}
		}
		if (document.form1.LocationID.value == 0) {
			errmsg = errmsg + "- Cannot move containers to root location.\r";
			bWriteError = true;
		}


		<% if Application("RACKS_ENABLED") then %>
		// Validation for Rack assignment
		<% if Lcase(Request("multiSelect")) = "true" then %>
		var numSamples = <%=myDict.count%>;
		<% else %>
		var numSamples = 1;
		<% end if%>
		if (document.form1.AssignToRack.checked) {
			
			// Validate Rack Grid ID
			if (document.form1.iRackGridID.value.length == 0){
				errmsg = errmsg + "- Please select a valid Starting Postion for the Rack.\r";
				bWriteError = true;
			}else if (!isPositiveNumber(document.form1.iRackGridID.value)&&(document.form1.iRackGridID.value != 0)){
				errmsg = errmsg + "- Rack grid location must be a positive number.\r";
				bWriteError = true;
			<% if not isEdit then %>
			}else if (document.form1.RackGridList.value.length == 0) {
				errmsg = errmsg + "- Please choose a Rack grid location for this container.\r";
				bWriteError = true;
			<% end if %>
			}else{
				var tmpRackGridList = document.form1.RackGridList.value;
				tmpRackGridList = tmpRackGridList.split(",");
				if (numSamples != tmpRackGridList.length){
					errmsg = errmsg + "- The number of Rack grid locations (" + tmpRackGridList.length +") does not match the number of copies requested (" + numSamples + ").\r Please reselect Racks and Rack starting position again.\r";
					bWriteError = true;
				}
				document.form1.LocationID.value = document.form1.iRackGridID.value;
			}
		}
		// Destination cannot be assign to a rack parent
		if (document.form1.AssignToRack.checked == false){
			if (ValidateLocationIDType(document.form1.LocationID.value) == 1) {
				//errmsg = errmsg + "- Destination can not be a Rack unless \"Assign to Rack\" is checked. Please check \"Assign to Rack\" or choose a different location.\r";
				//bWriteError = true;
				document.form1.AssignToRack.checked = true;
				toggleRackDisplay();
				errmsg = errmsg + "- To assign selected Containers to Rack \"Assign to Rack\" must be selected.\r";
				bWriteError = true;
			}
		}
		<% end if %>
		
		if (bWriteError){
			alert(errmsg);
		}
		else{
			document.form1.submit();
		}
	}


	// Toggles display of Rack editing fields
	function toggleRackDisplay() {
		if (ValidateLocationIDType(document.form1.LocationID.value) == 1 && document.form1.AssignToRack.checked == false) {
			alert("You cannot uncheck \"Assign to Rack\" because you have chosen to assign the sample to a Rack Location ID. \rPlease click \"Browse\" and choose a different Location ID if this is correct.");
			document.form1.AssignToRack.checked = true;
		} else {
		
			if (document.form1.AssignToRack.checked) {
				AlterCSS('.singleRackElement','display','block')
				AlterCSS('.singleRackElement','color','red')
				if (document.form1.SelectRackByLocation && document.form1.SelectRackBySearch) {
					if (!document.form1.SelectRackByLocation.checked && !document.form1.SelectRackBySearch.checked) {
						document.form1.SelectRackByLocation.checked = true;
						toggleRackLocationDisplay();
						document.form1.SelectRackBySearch.checked = false;
						toggleRackSearchDisplay();
					}
				}
			}else{
				AlterCSS('.singleRackElement','display','none')
				AlterCSS('.rackLabel','color','black')
			}
		}
	}	

	function toggleRackLocationDisplay() {
		if (document.form1.SelectRackByLocation) {
			if (document.form1.SelectRackByLocation.checked) {
				document.form1.SelectRackBySearch.checked = false;
				AlterCSS('.locationRackList','display','block')
				AlterCSS('.suggestRackList','display','none')
			} else {
				document.form1.SelectRackBySearch.checked = true;
				AlterCSS('.locationRackList','display','none')
				AlterCSS('.suggestRackList','display','block')
			}
		}
	}
	
	function toggleRackSearchDisplay() {
		if (document.form1.SelectRackBySearch) {
			if (document.form1.SelectRackBySearch.checked) {
				AlterCSS('.locationRackList','display','none')
				document.form1.SelectRackByLocation.checked = false;
				AlterCSS('.suggestRackList','display','block')
			} else {
				document.form1.SelectRackBySearch.checked = false;
				document.form1.SelectRackByLocation.checked = true;
				//alert("here");
				AlterCSS('.suggestRackList','display','none')
				AlterCSS('.locationRackList','display','block')
			}
		}
	}
	
	// Handles checking of Show Full Rack List	
	function toggleRackListDisplay() {
		var strRackShowFullList = "";
		if (document.form1.AssignToRack.checked) {		
			if (document.form1.ShowFullRackList.checked) {
				document.form1.ShowFullRackList.value='true';
			} else {
				document.form1.ShowFullRackList.value='';
				document.form1.GetSessionLocationID.value='true';
			}
			//alert(document.form1.ShowFullRackList.value);
			<% 
			if Lcase(Request("multiSelect")) = "true" then 
				multiSelect = "?multiSelect=true&" 
			else
				multiSelect = "?" 
			end if
			if Request("multiscan") = "1" then
				multiSelect = multiSelect & "&multiscan=1" 
			end if
			%>
			document.form1.action = "MoveContainer.asp<%=multiSelect%>";
			document.form1.submit();
		}
	}	

	function validateLocation(LocationID){
		CurrLocationID = 0;
		CurrLocationID = <%=LocationID%>;
		if (LocationID != CurrLocationID){
			<% 
			if Lcase(Request("multiSelect")) = "true" then 
				multiSelect = "?multiSelect=true&" 
			else
				multiSelect = "?" 
			end if
			%>
			document.form1.action = "MoveContainer.asp<%=multiSelect%>LocationID="+LocationID;
			document.form1.submit();
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
		<% if Lcase(Request("multiSelect")) = "true" then %>
		var numSamples = <%=myDict.count%>;
		<% else %>
		var numSamples = 1;
		<% end if%>
		if (document.form1.SelectRackBySearch.checked) {
			rackField = document.form1.SuggestedRackID;
		}else if (document.form1.SelectRackByLocation.checked) {
			rackField = document.form1.RackID;
		}
		if (rackField.selectedIndex==-1) {
			alert("Please select at least one Rack");
		}else{
			var totPosCnt = 0;
			for (i=0; i < rackField.length; i++){
				// If Rack list is Empty
				if (rackField[i].value == 'NULL'){
					errmsg = errmsg + "There are no Racks in the current location. \rPlease select a different location or create a New Rack.\r\r";
					bWriteError = true;
				}else{
					if (rackField[i].selected){
						if (i > 0 && TempIDs.length > 0) TempIDs = TempIDs + ',';
						TempIDs = TempIDs + rackField.options[i].value;
						Temp = (rackField.options[i].text).replace(" ","");
						arrTemp = Temp.split("::");
						arrTempName = arrTemp[0];
						arrTempValue = arrTemp[1].replace(" open","");
						if (arrTempValue == 0){
							errmsg = errmsg + "The select Rack, " + arrTempName + ", contains no open positions. \rPlease choose a different Rack.\r\r";
							bWriteError = true;
							rackField.options[i].selected=false;						
						}else{
							totPosCnt = totPosCnt + parseInt(arrTempValue);
						}
					}
				}
			}
			if (totPosCnt < numSamples) {
				var diffTot = numSamples-totPosCnt;
				errmsg = errmsg + "There are not enough open Rack positions to fulfill your request. \rPlease change the number of copies or select Racks with " + diffTot + " more open positions.\r\r";
				bWriteError = true;
			}
			//bWriteError = true;
			if (bWriteError){
				alert(errmsg);
			}else{
				OpenDialog('/cheminv/gui/ViewRackLayout.asp?ActionType=select&PosRequired='+numSamples+'&IsMulti=true&LocationID='+rackField.options[rackField.selectedIndex].value+'&RackIDList='+TempIDs, 'RackGrid', 2);
				return false;	
			}
		}
	}
   
	function setDefaultStartingPosition(){
		<% if Lcase(Request("multiSelect")) = "true" then %>
		numSamples = <%=myDict.count%>;
		<% else %>
		numSamples = 1;
		<% end if%>
		if (eval(numSamples) == 1){
			selectRackDefaults();
		}
	}   
	
-->
</script>
</head>
<body>
<center>
<form name="form1" xaction="echo.asp" action="MoveContainer_action.asp" method="POST">
<input type="hidden" name="ContainerID" value="<%=ContainerID%>">
<input type="hidden" name="GetSessionLocationID" value>
<input type="hidden" name="multiscan" value="<%=Request("multiscan")%>">
<table border="0">
	<tr>
		<td colspan="2">
			<span class="GuiFeedback">Move an inventory container.</span><br><br>
		</td>
	</tr>
	<tr>
		<td align="right" nowrap>
			Container to move:
		</td>
		<td>
			<input type="tetx" size="44" onfocus="blur()" value="<%=ContainerName%>" disabled>
		</td>
	</tr>
	<tr>
		<td align="right" nowrap>
			<span class="required" title="LocationID of the destination">Destination Location: <%=GetBarcodeIcon()%></span>
		</td>
		<td>
			<!--<%ShowLocationPicker "document.form1", "LocationID", "lpLocationBarCode", "lpLocationName", 10, 31, false%> -->
			&nbsp;<%ShowLocationPicker3 "document.form1", "LocationID", "lpLocationBarCode", "lpLocationName", 10, 20, false,"validateLocation(document.form1.LocationID.value)"%> 
		</td>
	</tr>
	<% if Application("RACKS_ENABLED") then %>	
	<% '-- Toggle to turn ON/OFF options to assign containers to Rack %>
	<% '-- ---------------------------------------------------------- %>
	<tr height="25">
		<td align="right">Assign Rack:</td><td>
			<input type="hidden" name="RackGridList" value />
			<input class="noborder" type="checkbox" name="AssignToRack" onclick="toggleRackDisplay()">Assign container to Rack
		</td>
	</tr>
	
	<% '-- Next three rows are used to select Rack by current location %>
	<% '-- ----------------------------------------------------------- %>
	<tr class="singleRackElement" style="color:#000;">
		<td align="right">Select Racks:</td><td>
			<input class="noborder" type="checkbox" name="SelectRackByLocation" onclick="toggleRackLocationDisplay()">Select Rack in current location
		</td>
	</tr>
	<tr class="locationRackList">
		<td align="right">&nbsp;</td><td colspan="3">
			<input class="noborder" type="checkbox" name="ShowFullRackList" onclick="toggleRackListDisplay()" <%=CheckFullRack%>><span style="color:#000000;">Show Full Rack List<br />
		</td>
	</tr>
	<tr class="locationRackList">
		<!-- ShowPickList3("Select Rack(s):", "RackID", RackID,"select l.location_id as Value, cheminvdb2.guiutils.GETRACKLOCATIONPATH(l.location_id) || ' :: ' || ((f.row_count * f.col_count) - ((select /*+ index(c CONTAINER_LOCATION_ID_FK_IDX) */count(*) from inv_containers c, inv_vw_grid_location_lite gl where c.location_id_fk = gl.location_id and gl.parent_id = l.location_id) + (select /*+ index(p PLATE_LOCATION_ID_FK_IDX) */count(*) from inv_plates p, inv_vw_grid_location_lite gl where p.location_id_fk = gl.location_id and gl.parent_id = l.location_id) + (select /*+ index(lr INV_LOCATION_PK) */count(*) from inv_locations lr, inv_vw_grid_location_lite gl where lr.parent_id = gl.location_id and gl.parent_id = l.location_id))) || ' open' as DisplayText, (select gl.location_id || '::' || gl.name from inv_vw_grid_location gl where gl.parent_id = l.location_id and not exists (select /*+ index(c CONTAINER_LOCATION_ID_FK_IDX) */ location_id_fk from inv_containers c where c.location_id_fk = gl.location_id) and not exists (select /*+ index(p PLATE_LOCATION_ID_FK_IDX) */ location_id_fk from inv_plates p where p.location_id_fk = gl.location_id) and not exists (select /*+ index(lr INV_LOCATION_PK) */ parent_id from inv_locations l where gl.location_id = l.parent_id and collapse_child_nodes = 1) and rownum = 1 )  as DefaultValue from inv_grid_storage s, inv_locations l, inv_grid_format f where s.location_id_fk = l.location_id and l.collapse_child_nodes = 1 and s.grid_format_id_fk = f.grid_format_id and l.location_id in (select location_id from inv_locations where collapse_child_nodes = 1 connect by prior location_id = parent_id start with location_id = " & LocationID & ") and collapse_child_nodes = 1    and ((f.row_count * f.col_count) - ((select /*+ index(c CONTAINER_LOCATION_ID_FK_IDX) */count(*) from inv_containers c, inv_vw_grid_location_lite gl where c.location_id_fk = gl.location_id and gl.parent_id = l.location_id) + (select /*+ index(p PLATE_LOCATION_ID_FK_IDX) */count(*) from inv_plates p, inv_vw_grid_location_lite gl where p.location_id_fk = gl.location_id and gl.parent_id = l.location_id) + (select /*+ index(lr INV_LOCATION_PK) */count(*) from inv_locations lr, inv_vw_grid_location_lite gl where lr.parent_id = gl.location_id and gl.parent_id = l.location_id and lr.collapse_child_nodes = 1))) > 0  order by DisplayText",100,"","","setDefaultStartingPosition();",5,true,"No Racks in this location") -->
		<%= ShowPickList3("Select Rack(s):", "RackID", RackID,"select l.location_id as Value, cheminvdb2.guiutils.GETRACKLOCATIONPATH(l.location_id) || ' :: ' || ((f.row_count * f.col_count) - ((select /*+ index(c CONTAINER_LOCATION_ID_FK_IDX) */count(*) from inv_containers c, inv_vw_grid_location_lite gl where c.location_id_fk = gl.location_id and gl.parent_id = l.location_id) + (select /*+ index(p PLATE_LOCATION_ID_FK_IDX) */count(*) from inv_plates p, inv_vw_grid_location_lite gl where p.location_id_fk = gl.location_id and gl.parent_id = l.location_id) + (select /*+ index(lr INV_LOCATION_PK) */count(*) from inv_locations lr, inv_vw_grid_location_lite gl where lr.parent_id = gl.location_id and gl.parent_id = l.location_id))) || ' open' as DisplayText, (select gl.location_id || '::' || gl.name from inv_vw_grid_location gl where gl.parent_id = l.location_id and not exists (select /*+ index(c CONTAINER_LOCATION_ID_FK_IDX) */ location_id_fk from inv_containers c where c.location_id_fk = gl.location_id) and not exists (select /*+ index(p PLATE_LOCATION_ID_FK_IDX) */ location_id_fk from inv_plates p where p.location_id_fk = gl.location_id) and not exists (select /*+ index(lr INV_LOCATION_PK) */ parent_id from inv_locations l where gl.location_id = l.parent_id and collapse_child_nodes = 1) and rownum = 1 )  as DefaultValue from inv_grid_storage s, inv_locations l, inv_grid_format f where s.location_id_fk = l.location_id and l.collapse_child_nodes = 1 and s.grid_format_id_fk = f.grid_format_id and l.location_id in (select location_id from inv_locations where collapse_child_nodes = 1 connect by prior location_id = parent_id start with location_id = " & LocationID & ") and collapse_child_nodes = 1    and ((f.row_count * f.col_count) - ((select /*+ index(c CONTAINER_LOCATION_ID_FK_IDX) */count(*) from inv_containers c, inv_vw_grid_location_lite gl where c.location_id_fk = gl.location_id and gl.parent_id = l.location_id) + (select /*+ index(p PLATE_LOCATION_ID_FK_IDX) */count(*) from inv_plates p, inv_vw_grid_location_lite gl where p.location_id_fk = gl.location_id and gl.parent_id = l.location_id) + (select /*+ index(lr INV_LOCATION_PK) */count(*) from inv_locations lr, inv_vw_grid_location_lite gl where lr.parent_id = gl.location_id and gl.parent_id = l.location_id and lr.collapse_child_nodes = 1))) > 0  order by DisplayText",100,"","","",5,true,"No Racks in this location")%>
	</tr>
	
	<% '-- Next three rows are used to select Rack by Suggestion/Searching %>
	<% '-- --------------------------------------------------------------- %>
	<tr class="singleRackElement" style="color:#000;">
		<td align="right">Suggest Rack(s):</td><td>
			<input class="noborder" type="checkbox" name="SelectRackBySearch" onclick="toggleRackSearchDisplay()">Search for Racks
		</td>
	</tr>
	<tr class="suggestRackList" style="color:#000;">
		<td align="right" valign="top">Suggested Racks:</td>
		<td>
		<a class="MenuLink" href="#" onclick="OpenDialog('/cheminv/gui/ManageRacks.asp','SuggestRacks',1); return false">Search Racks</a><br />
			<table cellpadding="0" cellspacing="4"><tr><td valign="top">
				<select name="SuggestedRackID" size="5" multiple="multiple" onchange="setDefaultStartingPosition();">
				<option value="NULL">Currently no Racks selected</option>
				</select>
			</td><td valign="top"><!--<a class="MenuLink" href="#" onclick="removeFromList(document.form1.SuggestedRackID)">Remove</a>--></td></tr>
			</table>
		</td>
	</tr>
	
	<tr class="singleRackElement" height="25">
		<td align="right" valign="top">Starting Position:</td><td valign="top">
			<table cellpadding="0" cellspacing="0"><tr><td>
				<input type="hidden" name="iRackGridID" value>
				<input type="text" size="25" name="iRackGridName" readonly style="background-color:#d3d3d3;" value>
			</td><td>&nbsp;</td><td valign="bottom">
				<a href="#" onclick="validateRackSelect()"><img src="/cheminv/graphics/btn_search.gif" border="0"></a>
			</td></tr></table>
		</td>
	</tr>
	<script language="javascript">
		numSamples = <%=myDict.count%>;
		if (eval(numSamples) == 1){
			selectRackDefaults();
		}
	</script>
	<% end if %>
	<tr>
		<td colspan="2" align="right"> 
			<a href="#" onclick="if (DialogWindow) {DialogWindow.close()}; window.close(); return false;"><img src="/cheminv/graphics/sq_btn/cancel_dialog_btn.gif" border="0" WIDTH="61" HEIGHT="21"></a>&nbsp;<a href="#" onclick="ValidateMove(); return false;"><img src="/cheminv/graphics/sq_btn/ok_dialog_btn.gif" border="0"></a>
		</td>
	</tr>	
</table>	
</form>

<% if Application("RACKS_ENABLED") then %>	
<%if AssignToRack = "on" then %>
	<script language="javascript">
	document.form1.AssignToRack.checked = true;
	toggleRackDisplay();
	</script>
<% end if %>
<% end if %>

</center>
</body>
</html>
