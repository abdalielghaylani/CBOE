<%@ Language=VBScript %>
<!--#INCLUDE VIRTUAL = "/cheminv/gui/GetLocationAttributes.asp"-->
<!--#INCLUDE VIRTUAL = "/cheminv/gui/guiUtils.asp"-->
<!--#INCLUDE VIRTUAL = "/cheminv/api/apiUtils.asp"-->
<% if Application("ENABLE_OWNERSHIP")="TRUE" then %>
<!--#INCLUDE VIRTUAL = "/cheminv/gui/GetLocationAuthorizedAttribute.asp" -->
<% end if %>
<%
Call GetInvConnection()

bPlatesEnabled = false
isLeafNode = false 
clearReturnURL = Request("clearReturnURL")
if clearReturnURL = "1" then Session("GUIReturnURL") = ""
allowContainers = "true"

Session("CurrentContainerID")=0
chk1 = "checked"
LocationID = Request("LocationID")
if LocationID = "" then LocationID = Session("CurrentLocationID")
LocationType = Request("LocationType")
RackGridInfo = ""

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
LocationText = "Location"
'LocationTypeID=27 '-- Rack location type

'Response.Write("Session LocationID: " & Session("CurrentLocationID") & "<br>Data LocationID: " & DataLocationID & "<br>")
'Response.Write("Final LocationID: " & LocationID & "<br>Request Location: " & Request("LocationID") & "<br>")
SQL="SELECT LOCATION_ID FROM " &  Application("CHEMINV_USERNAME") & ".inv_Locations WHERE LOCATION_TYPE_ID_FK=27" 
Dim racks
Set RS= Conn.Execute(SQL)

IF Not RS.EOF  THEN
RS.MoveFirst
While NOT RS.EOF
    racks=racks & "," & RS("LOCATION_ID")
    RS.movenext
Wend
End if

Set RS=Nothing
'-- Get AddressID
Dim Cmd
SQL="SELECT Address_ID_FK FROM " &  Application("CHEMINV_USERNAME") & ".inv_Locations WHERE Location_ID=?"
set cmd = server.createobject("ADODB.Command")
cmd.CommandText = SQL
cmd.ActiveConnection = Conn
Cmd.Parameters.Append Cmd.CreateParameter("LocationID", 131, 1, 0, LocationID)
Set RS = Cmd.Execute
Address_ID=RS("Address_ID_FK")
pageTitle = "Create a New " & LocationText
if isEdit then
	pageTitle = "Edit " & LocationText
	dsbl = "disabled"
	collapseChildNodes = GetListFromSQL1("SELECT Collapse_Child_Nodes from " & Application("CHEMINV_USERNAME") & ".inv_locations where location_id=?" , DataLocationID)	
	currGridFormat = GetListFromSQL1("SELECT grid_format_id_fk from " & Application("CHEMINV_USERNAME") &  ".inv_grid_storage where location_id_fk=?" , DataLocationID)	
	currAllowedPlateTypes = GetListFromSQL1("SELECT Plate_Type_ID_FK FROM "  & Application("CHEMINV_USERNAME") & ".inv_Allowed_pTypes WHERE Location_ID_FK=?" , DataLocationID)
	if currAllowedPlateTypes = "" then currAllowedPlateTypes = "0"
	excludedContainerTypes = GetListFromSQL1("SELECT location_id_fk FROM " & Application("CHEMINV_USERNAME") & ".inv_exclude_container_types WHERE Location_ID_FK=?" , DataLocationID)
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
		allowContainers = "false"
	end if
	'-- check if this location is a leaf node
	'-- a leaf node is defined here as a location with no child locations, no containers, and no plates
	SQL = "	SELECT (SELECT COUNT(*) FROM inv_locations WHERE parent_id = l.location_id) AS location_count, " & _
				"(SELECT COUNT(*) FROM inv_containers WHERE location_id_fk = L.location_id) AS container_count, " & _
				"(SELECT COUNT(*) FROM inv_plates WHERE location_id_fk = L.location_id) AS plate_count " & _
				"FROM inv_locations l WHERE location_id = ?"
	set cmd = server.createobject("ADODB.Command")
	cmd.CommandText = SQL
	cmd.ActiveConnection = Conn		
	Cmd.Parameters.Append Cmd.CreateParameter("LocationID", 131, 1, 0, LocationID)
	Set rsLeaf = Cmd.Execute
	locationCount = cint(rsLeaf("location_count"))
	containerCount = cint(rsLeaf("container_count"))
	plateCount = cint(rsLeaf("plate_count"))
	sum = locationCount + containerCount + plateCount
	if sum = 0 then isLeafNode = true
end if

%>
<html>
<head>
<title><%=Application("appTitle")%> -- <%=pageTitle%></title>
<script language="javascript" type="text/javascript" src="/cheminv/Choosecss.js"></script>
<script language="javascript" type="text/javascript" src="/cheminv/gui/validation.js"></script>
<script language="javascript" type="text/javascript" src="/cheminv/utils.js"></script>
<style type="text/css">

	.singleRackElement {
		display:none;
	}
	.rackLabel {color:#000;}
	.locationDisplay
	{
	    display:none;
	}
</style>
<script language="javascript" type="text/javascript">
<!--
	var currGridFormat = "<%=currGridFormat%>";
	var pageBeingRefreshed = false;
	
	window.focus();
	
	function setPrincipalID(element)
    {
        <% if Application("ENABLE_OWNERSHIP")="TRUE" then %>
        !document.form1.Ownershiplst ? document.form1.PrincipalID.value="" :  document.form1.PrincipalID.value=element.value;
        <% end if %>
    }
    function setOwnership()
    {
      <% if Application("ENABLE_OWNERSHIP")="TRUE" then %>

       var type="";
        type=document.form1.PrincipalID.value;
        
        if(type!="NULL" && type!="")
        {       
             var tempString ="|" + document.getElementById("OwnerShipUserList").value;
           if (tempString.indexOf("|" + type + ",")>=0)
           {
             getList(document.getElementById("OwnerShipUserList").value,type);
             document.getElementById("User_cb").checked=true;
           }
           else
           { 
                getList(document.getElementById("OwnerShipGroupList").value,type);
                document.getElementById("Group_cb").checked=true;
                
           }
        }
        <% end if %>
           
    }
       
	
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
			// containers allowed
			document.form1.AllowContainers.value = "true";
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
		var racks="<%=racks%>";
		var bWriteError = false;
		var errmsg = "Please fix the following problems:\r";
		  
	<% if NOT isEdit then %>
		// ParentID is required
		if (document.form1.ParentID.value.length == 0) {
			errmsg = errmsg + "- Parent Location is required.\r";
			bWriteError = true;
		}
		else
		{
		   <% if Application("ENABLE_OWNERSHIP")="TRUE" then %>
	    	 if(GetAuthorizedLocation(document.form1.ParentID.value,document.getElementById("tempCsUserName").value,document.getElementById("tempCsUserID").value)==0)
            {
                errmsg = errmsg + "- Not athorized for this location.\r";
			    alert(errmsg);
			    return;
            }
          <% end if %>
	    }
	    <% if Application("ENABLE_OWNERSHIP")="TRUE" then %>
	   	
		if((document.form1.ParentID.value==0 && document.form1.PrincipalID.value=="") && (!document.getElementById("makePublic").checked))
		{
		    errmsg = errmsg + "-Location Admin is required.\r";
			bWriteError = true;
		}
        if ((document.form1.LocationAdmin.value!='' && document.form1.PrincipalID.value!='') && document.form1.LocationAdmin.value !=document.form1.PrincipalID.value)
        {
            if (confirm("- The Location Admin and Parent Location Admin are not the same,\r Do you really want to continue?")!=true)
               {     
                    return;
                }
         }
		<% end if %>
	<%else %>
	    if(ValidateLocationIDType(document.form1.LocationID.value) == 1){
	        var flag=true;	        
	        racks=racks.split(",");
	        for(i=0;i<racks.length;i++){                       		    	    
		        if(racks[i] == document.form1.LocationID.value ){      			            
			        flag = false;		
		        }			     
		    }
		    if(flag){
		        errmsg = errmsg + "- Can not edit a grid position\r";
		        bWriteError = true;
		    }
        }        
        <% if Application("ENABLE_OWNERSHIP")="TRUE" then %>               
        if(document.form1.Parent_ID.value==0 && document.form1.PrincipalID.value=="")
		{
		    errmsg = errmsg + "- Location Admin is required.\r";
			bWriteError = true;
		}
		if(document.form1.User_cb.checked == false && document.form1.Group_cb.checked == false && !document.getElementById("makePublic").checked)
		{
		    errmsg = errmsg + "- Location Admin is required.\r";
			bWriteError = true;
		}
        if (document.form1.LocationAdmin.value!='' && document.form1.LocationAdmin.value !=document.form1.PrincipalID.value)
        {
            if (confirm("- The  Location Admin and Parent Location Admin are not the same,\r Do you really want to continue?")!=true)
               {     
                    return;
                }
         }
		<% end if %>     
	<%End if%>
		// Location name is required
		if (document.form1.LocationName.value.length == 0) {
			errmsg = errmsg + "- Location Name is required.\r";
			bWriteError = true;
		}
		if (document.form1.ParentID){
			if(ValidateLocationIDType(document.form1.ParentID.value) == 1){		    	    
			    if(racks.match(document.form1.ParentID.value)== document.form1.ParentID.value ){      
			        errmsg = errmsg + "- Select a rack location\r";
				    bWriteError = true;		
			    }		
			}
		}
		<% if NOT isEdit then %>
		if ((document.form1.isRack.value==1) && (document.form1.LocationTypeID.value!=27)){
			errmsg = errmsg + "- Cannot create a location under a Rack.\r";
			bWriteError = true;		
		}
		<%else%>
		//Cannot convert a rack within another rack into a location		
		if (isaRackLocation(document.form1.Parent_ID.value)==1){
		    if((document.form1.LocationTypeID.value!=<%=LocationTypeID%>)) {
		        errmsg = errmsg + "- Cannot edit Location Type of a Rack which is part of another Rack.\r";
			    bWriteError = true;		
		    }
            if(document.form1.GridFormatID.value=="") {
		        errmsg = errmsg + "- Cannot remove Grid Format of Rack which is part of another Rack.\r";
			    bWriteError = true;		
		    }
		}
		<%end if %>
		<% if NOT isEdit then %>
		if (document.form1.AutoGen_cb.checked == false && document.form1.Barcode.value.length == 0) {
		    errmsg = errmsg + "- Location Barcode is required if auto generate is not selected.\r";
			bWriteError = true;
		}
		<%end if %>
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
				bcontinue =  confirm("Changing the Grid Format will permanently delete all sublocations and containers below this location!\rDo you really want to recreate the storage grid?");
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

	function refreshPage(LocationID){
		var locationType = "";
		if (ValidateLocationIDType(document.form1.LocationID.value) == 1) {
			locationType = "rack";
		}else{
			locationType = "";
		}
		location.href="/cheminv/gui/NewLocation.asp?LocationID="+LocationID+"&LocationType=" + locationType + "&GetData=db";
	}	
	
	/**
	* This anonymous function works as a client-side listener that listens to value changes to document.form1.LocationID.value,..
	* ..every 20 milliseconds
	*/
	setInterval(function() {
		if (document.form1.LocationID && document.form1.ID) {
			if (document.form1.LocationID.value != document.form1.ID.value && pageBeingRefreshed == false) {
				pageBeingRefreshed = true;
				// Later the value of pageBeingRefreshed identifier will be reset to false after successful page refresh
				refreshPage(document.form1.LocationID.value);
			}
		}
	}, 20);

	function AutoGen_OnClick(element) {
			document.form1.Barcode.value=''; 
			document.form1.AutoGen.value = element.checked.toString(); 
			element.checked ? document.all.sp0.style.display = 'none' :document.all.sp0.style.display = 'block';
			element.checked ? document.all.sp1.style.display = 'block':document.all.sp1.style.display = 'none';
			document.all.sp2.style.display = document.all.sp0.style.display;
			document.all.sp3.style.display = document.all.sp1.style.display;
			//update the bUseBarcodeDesc value
			element.checked ? document.all.bUseBarcodeDesc.value = 'true' : document.all.bUseBarcodeDesc.value = 'false';
	}
    function ShowGridOptionsOnEdit(collapseChildNodes)
    {
     <%if isedit then%>
        if (collapseChildNodes != 0)
        {
            AlterCSS('.locationDisplay','display','block');
            // select display as rack
            document.form1.collapseChildNodes[1].checked = true;
            document.form1.collapseChildNodes[0].disabled = true;
            document.form1.collapseChildNodes[1].disabled = true;
        }
       else
       {
           AlterCSS('.locationDisplay','display','block');
           // select display as rack
           document.form1.collapseChildNodes[0].checked = true;
           document.form1.collapseChildNodes[1].disabled = true;
           document.form1.collapseChildNodes[0].disabled = true;
       }
        <%end if  %>
    }
    function GridFormat_OnChange(element) {
        
        if (element.selectedIndex == 0)
        {
            // it's the default
            AlterCSS('.locationDisplay','display','none');
            // select display as tree element
            document.form1.collapseChildNodes[0].checked = true;
        }
        else
        {
            // a grid format was selected
            AlterCSS('.locationDisplay','display','block');
            // select display as rack
            document.form1.collapseChildNodes[1].checked = true;
        }
    }
    
 function SetLocationTypeAsRack(){
    if (document.form1.collapseChildNodes[1].checked){ 
			document.form1.LocationTypeID.value=27;
		}
 }	
   <% if Application("ENABLE_OWNERSHIP")="TRUE" then %>
 function changeLocationtypeonPublic(elem)
 {  
    if(elem.checked)
    {
            for(i=0;i<(document.form1.LocationTypeID.options.length);i++)
            {
                if(document.form1.LocationTypeID.options[i].innerText.toLowerCase()=="public")
                   {
                        document.form1.LocationTypeID.options[i].selected=true;
                   }
            }
        if (document.all.GroupBlock1)
            document.all.GroupBlock1.style.display = "none";
        if (document.all.GroupBlock2)
            document.all.GroupBlock2.style.display = "none";
        if (document.all.GroupBlock3)
            document.all.GroupBlock3.style.display = "none";
    }
    else
    {
        document.form1.LocationTypeID.options[0].selected=true;
        alert(" Location type has been reset. Please select valid location type");
        if (document.all.GroupBlock1)
            document.all.GroupBlock1.style.display = "block";
        if (document.all.GroupBlock2)
            document.all.GroupBlock2.style.display = "block";
        if (document.all.GroupBlock3)
            document.all.GroupBlock3.style.display = "block";
    }
    
 }
 <%end if %>

//-->
</script>

</head>
<% if Application("ENABLE_OWNERSHIP")="TRUE" then%>
    <body onload="setOwnership();">
 <%else %>
    <body>
 <%end if %>   
<center>
<form name="form1" action="<%=action%>" method="POST">
<input type="hidden" id="LocationType" name="LocationType" value="<%=LocationType%>"/>
<input type="hidden" id="PlateMapID" name="PlateMapID" value="<%=PlateMapID%>"/>
<input type="hidden" id="bAddAddress" name="bAddAddress" value="false"/>
<input type="hidden" id="Address1" name="Address1" />
<input type="hidden" id="Address2" name="Address2" />
<input type="hidden" id="Address3" name="Address3" />
<input type="hidden" id="Address4" name="Address4" />
<input type="hidden" id="City" name="City" />
<input type="hidden" id="StateIDFK" name="StateIDFK" />
<input type="hidden" id="CountryIDFK" name="CountryIDFK" />
<input type="hidden" id="Zip" name="Zip" />
<input type="hidden" id="Fax" name="Fax" />
<input type="hidden" id="Phone" name="Phone" />
<input type="hidden" id="Email" name="Email" />
<input type="hidden" id="AutoGen" name="AutoGen" value="<%=AutoGen%>"/>
<input TYPE="hidden" id="OwnerShipGroupList" name="OwnerShipGroupList" Value="<%=GetOwnerShipGroupList()%>">
<input TYPE="hidden" id="OwnerShipUserList" name="OwnerShipUserList" Value="<%=GetOwnerShipUserList()%>">
<input TYPE="hidden" id="PrincipalID" name="PrincipalID" Value='<%=PrincipalID%>'>
<input TYPE="hidden" id="isAuthorised" name="isAuthorised" Value='<%=isAuthorised%>'>
<input TYPE="hidden" id="LocationAdmin" name="LocationAdmin" Value>
<input TYPE="hidden" id="tempCsUserName" name="tempCsUserName" Value="<%=Session("UserName" & "cheminv")%>" >

<table border="0" width="575px">
	<tr>
		<td colspan="2">
            <center>
            <%if isAuthorised = "0" then%>
                <span class="GuiFeedback">You are not authorized to edit this location.</span><br /><br />
            <%else%>
                <span class="GuiFeedback">To <%=Headertxt%> an inventory <%=LocationText%> fill in the required attributes</span><br /><br />
			<% end if %>
            </center>
		</td>
	</tr>
	<% if NOT isEdit then %>
	<tr>
		<td align="right" nowrap>
			<span class="required">Parent Location: <%=GetBarcodeIcon()%></span>
		</td>
		<td>
            <%  if Application("ENABLE_OWNERSHIP")="TRUE" then
                authorityFunction = "SetOwnerInfo('location');"
            else
                authorityFunction= ""
            end if %>
			<%ShowLocationPicker3 "document.form1", "ParentID", "lpLocationBarCode", "lpLocationName", 10, 30, false,authorityFunction%> 
		</td>
	</tr>
	<tr>
		<%if lcase(Application("RequireBarcode")) = "true"  then%>
			<td align="right" valign="top" nowrap width="150"><span id="sp0" style="display:block;"><span class="required">Location Barcode:</span></span><span id="sp1" style="display:none;"><span class="required">Barcode Description:</span></span></td>
		<%else%>
			<td align="right" valign="top" nowrap width="150"><span id="sp0" style="display:block;">Location Barcode:</span><span id="sp1" style="display:none;">Barcode Description:</span></td>
		<%end if%>
		<td>
			<input type="hidden" name="bUseBarcodeDesc" value>
			<span id="sp2" style="display:block;">
				&nbsp;<%=GetBarcodeIcon()%>&nbsp;<input type="text" name="Barcode" size="15" value="<%=Barcode%>"><br>
			</span>
			<span id="sp3" style="display:none">
				<%=ShowSelectBox2("BarcodeDescID", BarcodeDescID,"SELECT barcode_desc_id AS Value, barcode_desc_name AS Displaytext FROM inv_barcode_desc ORDER BY lower(DisplayText) ASC", null," ","")%>
			</span>
			<input type="checkbox" name="AutoGen_cb" onclick="AutoGen_OnClick(this);">Autogenerate
			<script language="Javascript">if (document.form1.AutoGen.value == "true") document.form1.AutoGen_cb.click();</script>
		</td>
	</tr>
	<%Else%>
<!--	<tr>		<td align="right" nowrap>			<%=LocationText%> ID:		</td>		<td>			<input type="text" name="LocationID" value="<%=Session("CurrentLocationID")%>">		</td>	</tr>	-->
			<input type="hidden" name="LocationID" value="<%=LocationID%>">
			<input type="hidden" name="Parent_ID" value=<%=Parent_ID%>>
		<tr>
	<td align="right" nowrap> 
	    <% Response.Write "Location ID:"%>
	</td>
	<td>
		<input type="text" size="15" name="ID" maxlength="15" value="<%=LocationID%>" readonly class="readonly">
    </td>
	</tr>		
	<tr>
		<td align="right" nowrap>
		<%
		if lcase(Application("RequireBarcode")) = "true" then
			Response.Write "<span class=""required"">" & "Location Barcode: " & GetBarcodeIcon() & "</span>"
		else
			Response.Write "Location Barcode: " & GetBarcodeIcon()		
		end if
		%>
		</td>
		<td>
			<input type="text" size="15" name="Barcode" maxlength="15" value="<%=LocationBarcode%>">
			<% if isEdit then %>&nbsp;&nbsp;<a class="MenuLink" id="barcodeLink" HREF="Select%20a%20location" onclick="PickLocation1('', '',false,'document.form1','LocationID','Barcode','LocationName',false);return false">Browse</a><% end if %>
		</td>
	</tr>
	<%End if%>
	<tr>
		<td align="right">
			<span class="required"><%=LocationText%> Name:</span>
		</td>
		<td>
			<input type="text" size="30" maxlength="50" name="LocationName" value="<%=LocationName%>">
		</td>
	</tr>
	<tr>
		<td align="right" nowrap>
			<span class="required"><%=LocationText%> Type:<span>
		</td>
		<td>
			<%=ShowSelectBox3("LocationTypeID", LocationTypeID,"SELECT Location_Type_ID AS Value, Location_Type_Name AS DisplayText FROM inv_Location_Types ORDER BY Location_Type_Name ASC", 0, "", "","CheckPlateMap();")%>
		</td>
	</tr>
	<tr>
		
		<td align="right" valign="top" nowrap>
			<%=LocationText%> Owner:
		</td>
		<td>
			<%=ShowSelectBox2("LocationOwnerID", LocationOwnerID, "SELECT User_ID AS Value, Last_Name||DECODE(First_Name, NULL, '', ', '||First_Name) AS DisplayText FROM CS_SECURITY.People ORDER BY lower(Last_Name) ASC", 55, "Unknown", "Unknown")%>
		</td>
	</tr>
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
			<span title="Creates a grid of sublocations using the selected Grid Format">Grid Format:</span>
		</td>
		<td>
			<%=ShowSelectBox3("GridFormatID", currGridFormat,"SELECT Grid_Format_ID AS Value, Name AS DisplayText FROM inv_Grid_Format WHERE Grid_Format_Type_FK = 10 ORDER BY Name ASC", 55, "Select a Grid Format","","GridFormat_OnChange(this)")%>
			<br />
			<div class="locationDisplay">
			<input type="radio" name="collapseChildNodes" value="" checked/>Display as tree element
			<input type="radio" name="collapseChildNodes" value="1" onPropertyChange="SetLocationTypeAsRack();" />Display as rack
            </div>
		</td>
	</tr>
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
			<input type="hidden" name="AllowContainers" value="<%=allowContainers%>">
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
	<% if Application("ENABLE_OWNERSHIP")="TRUE" then %>
	<tr height="25">
		<td align=right> <span title="Pick an option from the list" class="required" id="GroupBlock1">Location Admin:</span></td>
		<td align=left><div id="GroupBlock2" >
		<table border=.5>
		<tr><td><input type="Radio" name="Ownership" name="User_cb" id="User_cb" onclick="SetOwnerInfo('user');setPrincipalID(Ownershiplst)"/>by user</td>
		<td><input type="Radio" name="Ownership" name="Group_cb" id="Group_cb" onclick="SetOwnerInfo('group');setPrincipalID(Ownershiplst)" />by Group</td></tr>
		<tr><td colspan="2"><SELECT id="Ownershiplst" onchange="setPrincipalID(this);" ><OPTION></OPTION></SELECT></td></tr></table>
        	</div>
        </td>
	</tr>
	<tr>
		<td colspan="2" align="right">
		    <div id="GroupBlock3" >
		<%if isEdit then%>
						<br><input TYPE="checkbox" NAME="propagateownership" id="propagateownership" VALUE="1" >Propagate the Location Admin to child locations/containers/plates.</span>
		<%end if%>
		</td>
            </div>
	</tr>
	
	<tr>
		<td align=right>
		<%if Session("Inv_Manage_Locations")=True then %>
						<input TYPE="checkbox" id="makePublic" name="makePublic" VALUE="1" <%=isPublic%> onclick="changeLocationtypeonPublic(this);">Access as Public.
		<%end if%>
		</td>
	</tr>
	<%end if%>
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
			<span id="AddAddressLink"><a HREF="Add%20an%20Address" id="addAddressLink" onclick="OpenDialog('/cheminv/gui/EditAddress.asp?TableName=inv_locations&amp;TablePKID=&amp;AddressID=','AddressDiag', 4); return false;" CLASS="MenuLink">Add Address</a></span>
		<%end if%>
		</td>
	</tr>
	<tr>
		<td colspan="2" align="right" width="420"> 
			<a HREF="#" onclick="opener.top.ListFrame.location.reload(); window.close();"><img src="/cheminv/graphics/sq_btn/cancel_dialog_btn.gif" border="0"></a>&nbsp;<a href="#" id="okLink" onclick="ValidateLocation(); return false;"><img id="okBtn" SRC="/cheminv/graphics/sq_btn/ok_dialog_btn.gif" border="0"></a>
		</td>
	</tr>

</table>
</form>
</center>
<script language="javascript" type="text/javascript">
if(document.form1.isAuthorised && document.form1.isAuthorised.value!="0")
{
	if (document.form1.locationContentsMode[1]){
		if ((document.form1.locationContentsMode[1].checked)&&(document.form1.locationContentsMode[1].value==2)){
			 document.all.s1.style.display = "block";
			 document.all.s0.style.display = "block";
		}	 
	}
	<% 
	 if isEdit and not isLeafNode then %>
    		if (document.form1.locationContentsMode[0]) document.form1.locationContentsMode[0].disabled = true;
		if (document.form1.locationContentsMode[1]) document.form1.locationContentsMode[1].disabled = true;	
	<%end if%>
}
<% if Application("ENABLE_OWNERSHIP")="TRUE" then %>
    //set the inital location group
    <%if PrincipalID="" Then%>
        SetOwnerInfo('location');
    <%end if%>
    <%if isEdit then %>
    if(document.form1.isAuthorised && document.form1.isAuthorised.value == 0)
    {

        document.form1.Barcode.readOnly = true;
        document.form1.LocationName.readOnly = true;
        document.form1.LocationDesc.readOnly = true;
        document.form1.LocationTypeID.disabled = true;
        document.form1.LocationOwnerID.disabled = true;
        document.form1.GridFormatID.disabled = true;
        document.form1.locationContentsMode[0].disabled = true;
        document.form1.locationContentsMode[1].disabled = true;
        document.form1.PlateTypeList.disabled = true;
        document.form1.Ownership[0].disabled = true;
        document.form1.Ownership[1].disabled = true;
        document.form1.Ownershiplst.disabled = true;
        document.form1.propagateownership.disabled = true;
        document.form1.makePublic.disabled = true;
        document.getElementById("okBtn").style.visibility = "hidden";
        document.getElementById("okBtn").disabled = true;
        document.getElementById("okBtn").onclick = "";
        document.getElementById("okLink").disabled = true;
        document.getElementById("okLink").onclick = "";
        document.getElementById("barcodeLink").style.visibility = "hidden";
        document.getElementById("barcodeLink").disabled = true;
        document.getElementById("barcodeLink").onclick = "";
		document.getElementById("AddAddressLink").style.visibility = "hidden";
        document.getElementById("AddAddressLink").disabled = true;
        document.getElementById("AddAddressLink").onclick = "";
    }
    if (document.getElementById("makePublic").checked){
        if (document.all.GroupBlock1)
            document.all.GroupBlock1.style.display = "none";
        if (document.all.GroupBlock2)
            document.all.GroupBlock2.style.display = "none";
        if (document.all.GroupBlock3)
            document.all.GroupBlock3.style.display = "none";
    }
    <%end if %>
<%end if %>
<%if isEdit then %>
    ShowGridOptionsOnEdit('<%=collapseChildNodes %>');
<%end if %>
</script>

</body>
</html>
