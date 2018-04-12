<%@ Language=VBScript %>
<!--#INCLUDE VIRTUAL = "/cheminv/gui/GetLocationAttributes.asp"-->
<!--#INCLUDE VIRTUAL = "/cheminv/gui/guiUtils.asp"-->
<!--#INCLUDE VIRTUAL = "/cheminv/api/apiUtils.asp"-->
<%
Call GetInvConnection()
'SQL = "SELECT plate_type_id FROM inv_plate_types WHERE Plate_Type_Name= 'Plate Map'"
'Response.Write SQL
'Response.end
'PlateMapID = RS("plate_type_id")
'Set RS = Nothing
'Set Conn = Nothing
bPlatesEnabled = false

Session("CurrentContainerID")=0
chk1 = "checked"
LocationID = Request("LocationID")
LocationType = Request("LocationType")
RackGridInfo = ""
'Response.Write("isEdit: " & isEdit & "<br>")
if isEdit then 
	if LocationID <> "" then
		DataLocationID = LocationID
		Session("CurrentLocationID") = LocationID
	else
		DataLocationID = Session("CurrentLocationID")
	end if
end if
'Response.Write("Request Location: " & LocationID & ", Session LocationID: " & Session("CurrentLocationID") & ":" & DataLocationID & "<br>")

isRack = ""
rackParent = ""
if LocationType = "rack" and lCase(Application("RACKS_ENABLED")) = "true" then
	LocationText = "Rack"
	FormatText = "Rack Format"
	CollapseChildNodes = 1
	LocationTypeID=27 '-- Rack location type

	'-- If user has selected a rack or contents of rack, set the default location to parent of rack
	'-- ValidateLocationIsRack returns COLLAPSE_CHILD_NODES, PARENT_ID for container
	RackGridInfo = ValidateLocationInGrid(Session("CurrentLocationID"))
	rackTemp = split(ValidateLocationIsRack(Session("CurrentLocationID")),"::")
	isRack = rackTemp(0)
	rackParent = rackTemp(1)
	'Response.Write("###" & isRack & ":" & rackParent & "##")
	'if isRack = "1" and not isEdit then
	if isRack = "1" then
		if rackParent <> "" then 
			Session("CurrentLocationID") = rackParent
		else
			Session("CurrentLocationID") = 0
		end if
		
	end if

else
	LocationText = "Location"
	FormatText = "Grid Format"
	CollapseChildNodes = null
end if

if LocationID <> "" and rackParent <> "" then 
	LocationID = rackParent
elseif LocationID = "" then 
	LocationID = Session("CurrentLocationID")
end if
'Response.Write("Session LocationID: " & Session("CurrentLocationID") & "<br>Data LocationID: " & DataLocationID & "<br>")
'Response.Write("Final LocationID: " & LocationID & "<br>Request Location: " & Request("LocationID") & "<br>")

'-- Get AddressID
SQL="SELECT Address_ID_FK FROM " &  Application("CHEMINV_USERNAME") & ".inv_Locations WHERE Location_ID=" & LocationID

Set RS= Conn.Execute(SQL)
Address_ID=RS("Address_ID_FK")
pageTitle = "Create a New " & LocationText
if isEdit then
	pageTitle = "Edit " & LocationText
	dsbl = "disabled"
	currGridFormat = GetListFromSQL("SELECT grid_format_id_fk from " & Application("CHEMINV_USERNAME") &  ".inv_grid_storage where location_id_fk=" & DataLocationID)	
	currAllowedPlateTypes = GetListFromSQL("SELECT Plate_Type_ID_FK FROM "  & Application("CHEMINV_USERNAME") & ".inv_Allowed_pTypes WHERE Location_ID_FK=" & DataLocationID)
	if currAllowedPlateTypes = "" then currAllowedPlateTypes = "0"
	excludedContainerTypes = GetListFromSQL("SELECT location_id_fk FROM " & Application("CHEMINV_USERNAME") & ".inv_exclude_container_types WHERE Location_ID_FK=" & DataLocationID)
	'Response.Write currAllowedPlateTypes & "plate<BR>"
	'Response.Write excludedContainerTypes & "con<BR>"
	if currAllowedPlateTypes <> "0" then
		chk0 = ""
		chk1 = "checked"
		chk2 = ""
	elseif  excludedContainerTypes <> "" then
		chk0 = "checked"
		chk1 = ""
		chk2 = ""
	end if
end if

%>
<html>
<head>
<title><%=Application("appTitle")%> -- <%=pageTitle%></title>
<script LANGUAGE="javascript" src="/cheminv/Choosecss.js"></script>
<script LANGUAGE="javascript" src="/cheminv/gui/validation.js"></script>
<script LANGUAGE="javascript" src="/cheminv/utils.js"></script>
<style>

	.singleRackElement {
		display:none;
	}
	.rackLabel {color:#000;}
</style>
<script language="JavaScript">
<!--
	var currGridFormat = "<%=currGridFormat%>";
	
	window.focus();
	
	function SetLocationMode(val){
		var vis = "none";
		if (val == "0"){
			//no containers or plates allowed
			document.form1.AllowContainers.value = "false";
			clearSelect();
		}
		else if (val == "1"){
			//no plates allowed
			document.form1.AllowContainers.value = "true";	
			clearSelect();
		} 
		else if (val == "2"){
			// no containers allowed
			document.form1.AllowContainers.value = "false";
			// show allowed plate type selector
			vis = "block";
		}
		else if (val == "3"){
			//no containers allowed
			document.form1.AllowContainers.value = "false";
		}	
		document.all.s0.style.display = vis;
		document.all.s1.style.display = vis;
	}
	
	function clearSelect(){
		var opts = document.form1.PlateTypeList.options;
		for (i=0; i< opts.length; i++){
			opts[i].selected = false;
		}
	}
	
	<% if LocationType <> "rack" then %>
	function CheckPlateMap(){
		var index = document.form1.LocationTypeID.selectedIndex;
		var selectedText = document.form1.LocationTypeID.options[index].text;
		if (selectedText.toLowerCase() == "plate map") {
			document.all.spanPlateMap.style.display = "block";
			document.all.spanPhysicalContainer.style.display = "none";
			document.all.s0.style.display = "none";
			document.all.s1.style.display = "none";
			document.form1.locationContentsMode[2].checked = true;
			SetLocationMode(3);
		}
	}
	<% end if %>
	
	function ValidateLocation(){
		
		var bWriteError = false;
		var errmsg = "Please fix the following problems:\r";
	<% if NOT isEdit then %>
		// ParentID is required
		if (document.form1.ParentID.value.length == 0) {
			errmsg = errmsg + "- Parent Location is required.\r";
			bWriteError = true;
		}
	<%End if%>		
		// Location name is required
		if (document.form1.LocationName.value.length == 0) {
			errmsg = errmsg + "- Location Name is required.\r";
			bWriteError = true;
		}
		
		<% if Application("RACKS_ENABLED") and LocationType = "rack" then %>		
		// Validation for Rack assignment
		if (document.form1.AssignToRack.checked) {
			
			// Validate Rack Grid ID
			if (document.form1.iRackGridID.value.length == 0){
				errmsg = errmsg + "- Please select a valid Rack grid location.\r";
				bWriteError = true;
			}else if (!isPositiveNumber(document.form1.iRackGridID.value)&&(document.form1.iRackGridID.value != 0)){
				errmsg = errmsg + "- Rack grid location must be a positive number.\r";
				bWriteError = true;
			}else{
				<% if isEdit then %>
					document.form1.LocationID.value = <%=DataLocationID%>;
					document.form1.ParentID.value = document.form1.iRackGridID.value;
				<% else %>
					document.form1.ParentID.value = document.form1.iRackGridID.value;
				<% end if %>
			}
		}else{
			document.form1.ParentID.value = <%=LocationID%>;
			<% if DataLocationID <> "" then %>
			document.form1.LocationID.value = <%=DataLocationID%>;
			<% end if %>
		}
		if (document.form1.GridFormatID.value==''){
				errmsg = errmsg + "- Rack format is required.\r";
				bWriteError = true;
		}

		// Destination cannot be assign to a rack parent
		if (!document.form1.AssignToRack.checked){
			<% if not isEdit then %>
			if (ValidateLocationIDType(document.form1.ParentID.value) == 1) {
				errmsg = errmsg + "- Destination can not be a Rack. If you would like to move the Container \rinto a Rack, please click \"Assign to Rack\" and choose a rack position.\r";
				bWriteError = true;
			}
			<% end if %>
		}
		<% end if %>

		<% if not isEdit then %>
			if (ValidateLocationIDType(document.form1.ParentID.value) == 1) {
				errmsg = errmsg + "- Destination can not be a Rack. If you would like to assign the Rack \rinto a Rack grid, please click \"Assign to Rack\" and choose a rack position.\r";
				bWriteError = true;
			}
			// If user Browsed to diff location, use the new location instead of Location from DB pull
			if (document.form1.AssignToRack){
				if (!document.form1.AssignToRack.checked){
					if (document.form1.lpLocationBarCode.value != document.form1.ParentID.value){
						document.form1.ParentID.value = document.form1.lpLocationBarCode.value;
					}
				}
			}
		<% end if %>
		
		//alert(document.form1.ParentID.value + ":" + document.form1.lpLocationBarCode.value);
		//alert(document.form1.ParentID.value + ":");
		//bWriteError = true;
		if (bWriteError){
			alert(errmsg);
		}
		else{
			var bcontinue = true;
			<%if isedit then%>
			if (document.form1.GridFormatID.value != currGridFormat){
				bcontinue =  confirm("Changing the <%=FormatText%> will permanently delete all sublocations and containers below this location!\rDo you really want to recreate the storage grid?");
			}
			<%end if%>
			
			if (bcontinue){
				document.form1.submit();
			}
			else{
				document.form1.GridFormatID.value = currGridFormat; 
			}
		}
	}	
	
	function UpdateAddressInfo(Address1, Address2, Address3, Address4, City, StateID, CountryID, Zip, Fax, Phone, Email)
	{
		document.form1.bAddAddress.value = 'true';
		document.form1.Address1.value = Address1;
		document.form1.Address2.value = Address2;
		document.form1.Address3.value = Address3;
		document.form1.Address4.value = Address4;
		document.form1.City.value = City;
		document.form1.StateIDFK.value = StateID;
		document.form1.CountryIDFK.value = CountryID;
		document.form1.Zip.value = Zip;
		document.form1.Fax.value = Fax;
		document.form1.Phone.value = Phone;
		document.form1.Email.value = Email;
	}
	
	function UpdateAddressLink(LocationID, AddressID)
	{
		document.all.EditAddressLink.style.display = 'block';
		document.all.AddAddressLink.style.display = 'none';
		document.all.EditLink.onclick =	function() {OpenDialog('/cheminv/gui/EditAddress.asp?TableName=inv_locations&TablePKID=' + LocationID + '&AddressID=' + AddressID,'AddressDiag', 4);return false;}
	}
	
	// Toggles display of Rack editing fields
	function toggleRackDisplay() {
		if (document.form1.AssignToRack.checked) {
			AlterCSS('.singleRackElement','display','block')
			AlterCSS('.singleRackElement','color','red')
		}else{
			AlterCSS('.singleRackElement','display','none')
			AlterCSS('.rackLabel','color','black')
		}
		selectRackDefaults()
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
			if (totPosCnt < 1) {
				errmsg = errmsg + "There are not enough open Rack positions to fulfill your request.\r\r";
				bWriteError = true;
			}
			if (bWriteError){
				alert(errmsg);
			}else{
				OpenDialog('/cheminv/gui/ViewRackLayout.asp?ActionType=select&PosRequired=1&IsMulti=true&LocationID='+document.form1.RackID.options[document.form1.RackID.selectedIndex].value+'&RackIDList='+TempIDs, 'RackGrid', 2);
				return false;	
			}
		}
	}

	function refreshPage(LocationID){
		var locationType = "";
		if (ValidateLocationIDType(document.form1.LocationID.value) == 1) {
			locationType = "rack";
		}else{
			locationType = "";
		}
		location.href="/cheminv/gui/NewLocation.asp?LocationID="+LocationID+"&LocationType=" + locationType + "&GetData=db";
	}	

//-->
</script>

</head>

<body>
<center>
<form name="form1" action="<%=action%>" method="POST">
<input type="hidden" name="LocationType" value="<%=LocationType%>">
<input type="hidden" name="CollapseChildNodes" value="<%=CollapseChildNodes%>">
<input TYPE="hidden" NAME="PlateMapID" VALUE="<%=PlateMapID%>">
<input TYPE="hidden" NAME="bAddAddress" VALUE="false">
<input TYPE="hidden" NAME="Address1" VALUE>
<input TYPE="hidden" NAME="Address2" VALUE>
<input TYPE="hidden" NAME="Address3" VALUE>
<input TYPE="hidden" NAME="Address4" VALUE>
<input TYPE="hidden" NAME="City" VALUE>
<input TYPE="hidden" NAME="StateIDFK" VALUE>
<input TYPE="hidden" NAME="CountryIDFK" VALUE>
<input TYPE="hidden" NAME="Zip" VALUE>
<input TYPE="hidden" NAME="Fax" VALUE>
<input TYPE="hidden" NAME="Phone" VALUE>
<input TYPE="hidden" NAME="Email" VALUE>

<%
if locationMismatch then
	Response.write("<div align=""center""><span class=""GuiFeedback"">" & errString & "</span><br /><br />")
	Response.Write("<a href=""#"" onclick=""window.close();""><img src=""/cheminv/graphics/sq_btn/cancel_dialog_btn.gif"" border=""0""></a>")
	Response.end
end if
%>

<table border="0">
	<tr>
		<td colspan="2">
			<span class="GuiFeedback">To <%=Headertxt%> an inventory <%=LocationText%> fill in the required attributes</span><br><br>
		</td>
	</tr>
	<% if NOT isEdit then %>
	<tr>
		<td align="right" nowrap>
			<span class="required">Parent Location: <%=GetBarcodeIcon()%></span>
		</td>
		<td>
			<%ShowLocationPicker "document.form1", "ParentID", "lpLocationBarCode", "lpLocationName", 10, 30, false%> 
		</td>
	</tr>
	<%Else%>
<!--	<tr>		<td align="right" nowrap>			<%=LocationText%> ID:		</td>		<td>			<input type="text" name="LocationID" value="<%=Session("CurrentLocationID")%>">		</td>	</tr>	-->
			<input type="hidden" name="LocationID" value="<%=LocationID%>" onpropertychange="refreshPage(document.form1.LocationID.value);">
			<input type="hidden" name="ParentID" value>
	<%End if%>
	<tr>
		<td align="right" nowrap>
			<%=LocationText%> Barcode: <%=GetBarcodeIcon()%>
		</td>
		<td>
			<input type="text" size="15" name="Barcode" maxlength="15" value="<%=LocationBarcode%>">
			<% if isEdit then %>&nbsp;&nbsp;<a class="MenuLink" HREF="Select%20a%20location" onclick="PickLocation('', '',false,'document.form1','LocationID','Barcode','LocationName');return false">Browse</a><% end if %>
		</td>
	</tr>
	<tr>
		<td align="right">
			<span class="required"><%=LocationText%> Name:</span>
		</td>
		<td>
			<input type="text" size="30" maxlength="50" name="LocationName" value="<%=LocationName%>">
		</td>
	</tr>
	<% if LocationType <> "rack" then %>
	<tr>
		<td align="right" nowrap>
			<span class="required"><%=LocationText%> Type:<span>
		</td>
		<td>
			<%=ShowSelectBox3("LocationTypeID", LocationTypeID,"SELECT Location_Type_ID AS Value, Location_Type_Name AS DisplayText FROM inv_Location_Types ORDER BY Location_Type_Name ASC", 0, "", "","CheckPlateMap();")%>
		</td>
	</tr>
	<% else %>
		<input type="hidden" name="LocationTypeID" value="<%=LocationTypeID%>" />
	<% end if %>
	<tr>
		
		<td align="right" valign="top" nowrap>
			<%=LocationText%> Owner:
		</td>
		<td>
			<%=ShowSelectBox2("LocationOwnerID", LocationOwnerID, "SELECT User_ID AS Value, Last_Name||DECODE(First_Name, NULL, '', ', '||First_Name) AS DisplayText FROM CS_SECURITY.People ORDER BY lower(Last_Name) ASC", 55, "Unknown", "Unknown")%>
		</td>
	</tr>


	<% if Application("RACKS_ENABLED") and LocationType = "rack" then %>	
	<tr height="25">
		<td align="right">Assign Rack:</td><td>
			<input type="hidden" name="RackGridList" value />
			<input class="noborder" type="checkbox" name="AssignToRack" onclick="toggleRackDisplay()">Assign to Rack
		</td>
	</tr>
	
	<tr class="singleRackElement">
		<%
		if isEdit then
			'sqlLocation = "select l.location_id as Value,  l.location_name||' :: '||  ((f.row_count*f.col_count)-(     (select count(*) from inv_containers where location_id_fk in (select location_id from inv_vw_grid_location where parent_id=l.location_id))+     (select count(*) from inv_plates where location_id_fk in (select location_id from inv_vw_grid_location where parent_id=l.location_id))+     (select count(*) from inv_locations where parent_id in (select location_id from inv_vw_grid_location where parent_id=l.location_id)) ))||' open' as DisplayText, (select min(location_id||'::'||name) from inv_vw_grid_location where parent_id=l.location_id and location_id not in (select distinct location_id_fk from inv_containers) and location_id not in (select distinct location_id_fk from inv_plates) and location_id not in (select distinct parent_id from inv_locations where collapse_child_nodes=1)) as DefaultValue from inv_grid_storage s, inv_locations l , inv_grid_format f  where s.location_id_fk = l.location_id  and l.collapse_child_nodes=1  and s.grid_format_id_fk = f.grid_format_id and l.location_id != " & DataLocationID & " and (l.parent_id = " & LocationID & " or  location_id in (     select l2.location_id from inv_locations l2 where l2.parent_id in (         select e2.location_id_fk from inv_grid_element e2, inv_grid_storage s2          where e2.grid_storage_id_fk = s2.grid_storage_id          and s2.location_id_fk in (             select location_id from inv_locations where parent_id=" & LocationID & " and collapse_child_nodes!=1))          and collapse_child_nodes=1))  and collapse_child_nodes=1  and ((f.row_count*f.col_count)-(     (select count(*) from inv_containers where location_id_fk in (select location_id from inv_vw_grid_location where parent_id=l.location_id))+     (select count(*) from inv_plates where location_id_fk in (select location_id from inv_vw_grid_location where parent_id=l.location_id))+     (select count(*) from inv_locations where parent_id in (select location_id from inv_vw_grid_location where parent_id=l.location_id)) )) > 0 order by DisplayText"
			sqlLocation = "select l.location_id as Value, cheminvdb2.guiutils.GETRACKLOCATIONPATH(l.location_id) || ' :: ' || ((f.row_count * f.col_count) - ((select /*+ index(c CONTAINER_LOCATION_ID_FK_IDX) */count(*) from inv_containers c, inv_vw_grid_location_lite gl where c.location_id_fk = gl.location_id and gl.parent_id = l.location_id) + (select /*+ index(p PLATE_LOCATION_ID_FK_IDX) */count(*) from inv_plates p, inv_vw_grid_location_lite gl where p.location_id_fk = gl.location_id and gl.parent_id = l.location_id) + (select /*+ index(lr INV_LOCATION_PK) */count(*) from inv_locations lr, inv_vw_grid_location_lite gl where lr.parent_id = gl.location_id and gl.parent_id = l.location_id))) || ' open' as DisplayText, (select gl.location_id || '::' || gl.name from inv_vw_grid_location gl where gl.parent_id = l.location_id and not exists (select /*+ index(c CONTAINER_LOCATION_ID_FK_IDX) */ location_id_fk from inv_containers c where c.location_id_fk = gl.location_id) and not exists (select /*+ index(p PLATE_LOCATION_ID_FK_IDX) */ location_id_fk from inv_plates p where p.location_id_fk = gl.location_id) and not exists (select /*+ index(lr INV_LOCATION_PK) */ parent_id from inv_locations l where gl.location_id = l.parent_id and collapse_child_nodes = 1) and rownum = 1 )  as DefaultValue from inv_grid_storage s, inv_locations l, inv_grid_format f where s.location_id_fk = l.location_id and l.collapse_child_nodes = 1 and l.location_id not in (" & DataLocationID & ") and s.grid_format_id_fk = f.grid_format_id and l.location_id in (select location_id from inv_locations where collapse_child_nodes = 1 connect by prior location_id = parent_id start with location_id = " & LocationID & ") and collapse_child_nodes = 1    and ((f.row_count * f.col_count) - ((select /*+ index(c CONTAINER_LOCATION_ID_FK_IDX) */count(*) from inv_containers c, inv_vw_grid_location_lite gl where c.location_id_fk = gl.location_id and gl.parent_id = l.location_id) + (select /*+ index(p PLATE_LOCATION_ID_FK_IDX) */count(*) from inv_plates p, inv_vw_grid_location_lite gl where p.location_id_fk = gl.location_id and gl.parent_id = l.location_id) + (select /*+ index(lr INV_LOCATION_PK) */count(*) from inv_locations lr, inv_vw_grid_location_lite gl where lr.parent_id = gl.location_id and gl.parent_id = l.location_id and lr.collapse_child_nodes = 1))) > 0  order by DisplayText"
		else
			sqlLocation = "select l.location_id as Value, cheminvdb2.guiutils.GETRACKLOCATIONPATH(l.location_id) || ' :: ' || ((f.row_count * f.col_count) - ((select /*+ index(c CONTAINER_LOCATION_ID_FK_IDX) */count(*) from inv_containers c, inv_vw_grid_location_lite gl where c.location_id_fk = gl.location_id and gl.parent_id = l.location_id) + (select /*+ index(p PLATE_LOCATION_ID_FK_IDX) */count(*) from inv_plates p, inv_vw_grid_location_lite gl where p.location_id_fk = gl.location_id and gl.parent_id = l.location_id) + (select /*+ index(lr INV_LOCATION_PK) */count(*) from inv_locations lr, inv_vw_grid_location_lite gl where lr.parent_id = gl.location_id and gl.parent_id = l.location_id))) || ' open' as DisplayText, (select gl.location_id || '::' || gl.name from inv_vw_grid_location gl where gl.parent_id = l.location_id and not exists (select /*+ index(c CONTAINER_LOCATION_ID_FK_IDX) */ location_id_fk from inv_containers c where c.location_id_fk = gl.location_id) and not exists (select /*+ index(p PLATE_LOCATION_ID_FK_IDX) */ location_id_fk from inv_plates p where p.location_id_fk = gl.location_id) and not exists (select /*+ index(lr INV_LOCATION_PK) */ parent_id from inv_locations l where gl.location_id = l.parent_id and collapse_child_nodes = 1) and rownum = 1 )  as DefaultValue from inv_grid_storage s, inv_locations l, inv_grid_format f where s.location_id_fk = l.location_id and l.collapse_child_nodes = 1 and s.grid_format_id_fk = f.grid_format_id and l.location_id in (select location_id from inv_locations where collapse_child_nodes = 1 connect by prior location_id = parent_id start with location_id = " & LocationID & ") and collapse_child_nodes = 1    and ((f.row_count * f.col_count) - ((select /*+ index(c CONTAINER_LOCATION_ID_FK_IDX) */count(*) from inv_containers c, inv_vw_grid_location_lite gl where c.location_id_fk = gl.location_id and gl.parent_id = l.location_id) + (select /*+ index(p PLATE_LOCATION_ID_FK_IDX) */count(*) from inv_plates p, inv_vw_grid_location_lite gl where p.location_id_fk = gl.location_id and gl.parent_id = l.location_id) + (select /*+ index(lr INV_LOCATION_PK) */count(*) from inv_locations lr, inv_vw_grid_location_lite gl where lr.parent_id = gl.location_id and gl.parent_id = l.location_id and lr.collapse_child_nodes = 1))) > 0  order by DisplayText"
		end if 
		'Response.Write(sqlLocation)
		Response.write(ShowPickList3("Select Rack(s):", "RackID", RackID,sqlLocation,100,"","","selectRackDefaults();",1,false,"No Racks in this location"))
		%>
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
		<% if isEdit then %>
			var rackGridInfo = "<%=RackGridInfo%>";
			if (rackGridInfo.length > 0){
				var arrRackGridInfo = rackGridInfo.split("::");
				//2741::0104::2665::2661::Plate Rack
				document.form1.AssignToRack.checked = true;
				toggleRackDisplay();
				document.form1.iRackGridID.value = arrRackGridInfo[2];
				document.form1.iRackGridName.value = arrRackGridInfo[1] + " in " + arrRackGridInfo[4];
				document.form1.RackGridList.value = arrRackGridInfo[2];
				for(l=0;l<document.form1.RackID.length;l++){
					var rackIDValue = document.form1.RackID.options[l].value;
					if (rackIDValue == arrRackGridInfo[3]){
						document.form1.RackID.options[l].selected=true;
					}
				}
			} else {
				selectRackDefaults();
			}
		<% else %>
			selectRackDefaults();
		<% end if %>
		</script>

	<% end if %>

	<tr>
		<td align="right" valign="top" nowrap>
			Description:
		</td>
		<td>
			<textarea cols="25" rows="5" name="LocationDesc" wrap="hard"><%=LocationDesc%></textarea>
		</td>
	</tr>
	<tr>
		<td align="right" nowrap valign="top">
			<span title="Creates a grid of sublocations using the selected Grid Format"><% if LocationType="rack" then response.write("<span class=""required"">") %><%=FormatText%>:<span>
		</td>
		<td>
			<%=ShowSelectBox2("GridFormatID", currGridFormat,"SELECT Grid_Format_ID AS Value, Name AS DisplayText FROM inv_Grid_Format WHERE Grid_Format_Type_FK = 10 ORDER BY Name ASC", 55, "Select a " & FormatText, "")%>
			
		</td>
	</tr>
	<% if LocationType <> "rack" then %>
	<tr>
		<td align="right" valign="top" nowrap>
			<%=LocationText%> may contain:
		</td>
		<td>
			<input <%=chk0%> type="radio" name="locationContentsMode" value="0" onclick="SetLocationMode(this.value)">Locations only<br>
			<%if Application("PLATES_ENABLED") then%>
				<span style="display:block" id="spanPhysicalContainer"><input <%=chk1%> type="radio" name="locationContentsMode" value="2" onclick="SetLocationMode(this.value)">Containers, plates and/or locations<br></span>
				<span style="display:none;" id="spanPlateMap"><input <%=chk2%> type="radio" name="locationContentsMode" value="3" onclick="SetLocationMode(this.value)">Plate maps and/or locations<br></span>
			<%else%>
				<input <%=chk1%> type="radio" name="locationContentsMode" value="1" onclick="SetLocationMode(this.value)">Containers and/or locations<br>
			<%end if%>
			<input type="hidden" name="AllowContainers" value="true">
		</td>
	</tr>
	<tr>
		<td valign="top" align="right" width="155">
			<span id="s0" style="display:none"><span title="Select the plate types that can be stored in this location">Allowed Plate Types:</span></span>
		</td>
		<td>
			<span id="s1" style="display:none">
				<%=ShowMultiSelectBox("PlateTypeList", currAllowedPlateTypes ,"SELECT Plate_Type_ID AS Value, Plate_Type_Name AS DisplayText FROM inv_Plate_Types WHERE Plate_Type_Name <> 'Plate Map' ORDER BY Plate_Type_Name ASC", 55, "No Plates Allowed","0", 4, true)%>
			</span>
		</td>
	</tr>
	<tr>
		<td colspan="2" align="right">
		<% 
		editStyle = "none"
		addStyle = "block"
		if Address_ID <> "" then
			editStyle="block"
			addStyle="none"
		end if
		%>
		<%if isEdit then%>
			<span id="EditAddressLink" style="display:<%=editStyle%>;"><a ID="EditLink" HREF="Edit%20an%20Address" onclick="OpenDialog('/cheminv/gui/EditAddress.asp?TableName=inv_locations&amp;TablePKID=<%=LocationID%>&amp;AddressID=<%=Address_ID%>', 'AddressDiag', 4); return false;" CLASS="MenuLink">Edit Address</a>
			<br><input TYPE="checkbox" NAME="propagateAddress" VALUE="1">Propagate address to child locations without addresses</span>
			<span id="AddAddressLink" style="display:<%=addStyle%>;"><a HREF="Add%20an%20Address" onclick="OpenDialog('/cheminv/gui/EditAddress.asp?TableName=inv_locations&amp;TablePKID=' + document.form1.LocationID.value + '&amp;AddressID=','AddressDiag', 4); return false;" CLASS="MenuLink">Add Address</a></span>
		<%else%>
			<span id="AddAddressLink"><a HREF="Add%20an%20Address" onclick="OpenDialog('/cheminv/gui/EditAddress.asp?TableName=inv_locations&amp;TablePKID=&amp;AddressID=','AddressDiag', 4); return false;" CLASS="MenuLink">Add Address</a></span>
		<%end if%>
		</td>
	</tr>
	
	<% end if %>
	
	<tr>
		<td colspan="2" align="right" width="420"> 
			<a HREF="#" onclick="opener.top.ListFrame.location.reload(); window.close();"><img SRC="/cheminv/graphics/sq_btn/cancel_dialog_btn.gif" border="0"></a>&nbsp;<a HREF="#" onclick="ValidateLocation(); return false;"><img SRC="/cheminv/graphics/sq_btn/ok_dialog_btn.gif" border="0"></a>
		</td>
	</tr>

</table>
</center>
</form>
<% if LocationType <> "rack" then %>
<script language="javascript">
	if (document.form1.locationContentsMode[1]){
		if ((document.form1.locationContentsMode[1].checked)&&(document.form1.locationContentsMode[1].value==2)){
			 document.all.s1.style.display = "block";
			 document.all.s0.style.display = "block";
		}	 
	}
	<% if isEdit then %>
		if (document.form1.locationContentsMode[0]) document.form1.locationContentsMode[0].disabled = true;
		if (document.form1.locationContentsMode[1]) document.form1.locationContentsMode[1].disabled = true;	
	<%end if%>
</script>
<% end if %>

</body>
</html>