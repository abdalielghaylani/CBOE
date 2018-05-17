<%
'Prevent page from being cached
Response.ExpiresAbsolute = Now()
Session("bManageMode") = false
%>
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/ado.inc"-->
<!--#INCLUDE VIRTUAL = "/cheminv/gui/guiUtils.asp"-->
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/search_func_vbs.asp"-->
<!--#INCLUDE VIRTUAL = "/cheminv/gui/GetContainerAttributes.asp"-->
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/server_const_vbs.asp"-->
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/csdo_utils_vbs.asp"-->
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/utility_func_vbs.asp"-->
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/display_func_vbs.asp"-->
<!--#INCLUDE VIRTUAL = "/cheminv/gui/compound_utils_vbs.asp"-->
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/cs_security/cs_security_login_utils_vbs.asp"-->
<% if Application("ENABLE_OWNERSHIP")="TRUE" then %>
<!--#INCLUDE VIRTUAL = "/cheminv/gui/GetLocationAuthorizedAttribute.asp" -->
<% end if %>
<% 
PageSource = Request("Source")
if lcase(PageSource) = "eln" and Session("isCDP") = "" then
    if ucase(Request.Cookies("isCDP")) = "" Then
%>
        <SCRIPT LANGUAGE="javascript" src="/cfserverasp/source/chemdraw.js"></SCRIPT>
        <script language="javascript" type="text/javascript" >
            cd_setIsCDPPluginCookie();   
        </script>
<%
        end if
        Session("isCDP") = ucase(Request.Cookies("isCDP"))
end if
ContainerCost = Trim(ContainerCost)
ContainerID = Request("ContainerID")
toggleDefaults = Request("toggleDefaults")
DefaultLocationOverride = Request("DefaultLocationOverride")

if Application("ENABLE_OWNERSHIP")="TRUE" then
  PrincipalID=Request("PrincipalID")
  iLocationTypeID=Request("LocationTypeID")
  LocationAdmin= Request("LocationAdmin")
end if
if ContainerID <> "" then 
	Session("ContainerID") = ContainerID
elseif ContainerID="" and Session("ContainerID")<>"" then 
      ContainerID=Session("ContainerID")  
end if 	
if Session("sTab2")="" then Session("sTab2")="Required"

'-- If user browses to a diff. location, get the new LocationID and use this
'-- LocationID to display the Racks in new location
NewSession = Request("NewSession")
if NewSession = "true" then Session("CurrentLocationID") = Request("LocationID")

if Session("ParentRackLocationID") <> "" then
	LocationID = Session("ParentRackLocationID")
elseif Request("LocationID") <> "" then
	LocationID = Request("LocationID")
elseif ContainerID = "" then
	if LocationID = "" then
		LocationID = OrigLocationID
	end if
end if

'CSBR ID : 124062 : SJ
'Comments: Reading the values from invconfig.ini 
ContainerTypeID = Application("DefaultContainerTypeID")
if Session("ContainerTypeID") <> "" then
	ContainerTypeID = Session("ContainerTypeID")
end if
if Len(Application("DefaultSupplierID")) > 0 and isEdit = false then
	if Session("SupplierID") <> "" then
		SupplierID = Session("SupplierID")
	else
SupplierID = Application("DefaultSupplierID")
	end if
elseif Session("SupplierID") <> "" then
   SupplierID=Session("SupplierID")  
else
	SupplierID = ""
end if
if ( DefaultLocationOverride = "true" ) then
    DefaultLocationProperty = Application( "DEFAULT_LOCATION_OVERRIDE" )
    if not isBlank( DefaultLocationProperty ) then
        LocationID = GetUserProperty( Session("UserNameCheminv"), DefaultLocationProperty )
    end if
end if

' Default to root
if isBlank(LocationID) then LocationID = 0

'-- Request data from location refresh to Racks
if Request("ContainerName") <> "" then ContainerName = Request("ContainerName")
if Request("QtyInitial") <> "" then QtyInitial = Request("QtyInitial")
if Request("QtyMax") <> "" then QtyMax = Request("QtyMax")
if Request("RegBatchID") <> "" then RegBatchID = Request("RegBatchID")
if Request("CompoundID") <> "" then CompoundID = Request("CompoundID")
if Request("Concentration") <> "" then Concentration = Request("Concentration")
if Request("UOCIDOptionValue") <> "" then Session("UOCIDOptionValue") = Request("UOCIDOptionValue")
if Request("isRack") <> "" then isRack = Request("isRack")
%>

<html>
<head>
<style type="text/css">
		A {Text-Decoration: none;}
		.TabView {color:#000000; font-size:8pt; font-family: arial}
		A.TabView:LINK {Text-Decoration: none; color:#000000; font-size:8pt; font-family: arial}
		A.TabView:VISITED {Text-Decoration: none; color:#000000; font-size:8pt; font-family: arial}
		A.TabView:HOVER {Text-Decoration: underline; color:#4682b4; font-size:8pt; font-family: arial}
		.singleRackElement { display:none; }
		.rackLabel {color:#000;}
</style>
<title><%=Application("appTitle")%> -- Create or Edit an Inventory Container</title>
<!--#INCLUDE VIRTUAL = "/cheminv/gui/dateFormat_js.asp"-->
<script type="text/javascript" language="javascript" src="/cheminv/Choosecss.js"></script>
<script type="text/javascript" language="javascript" src="/cheminv/utils.js"></script>
<script type="text/javascript" language="javascript" src="/cheminv/gui/CalculateFromPlugin.js"></script>
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/navbar_js.js"-->
<script type="text/javascript" language="javascript" src="/cheminv/gui/validation.js"></script>
<script type="text/javascript" language="JavaScript">
<!--Hide JavaScript
	var DialogWindow;
	window.focus();
	var cd_plugin_threshold= <%=Application("CD_PLUGIN_THRESHOLD")%>;
    var CD_AUTODOWNLOAD_PLUGIN = "<%=APPLICATION("CD_PLUGIN_DOWNLOAD_PATH")%>";
function setPrincipalID(element)
{
    <% if Application("ENABLE_OWNERSHIP")="TRUE" then %>
    !document.form1.Ownershiplst ? document.form1.PrincipalID.value="" :  document.form1.PrincipalID.value=document.form1.Ownershiplst.value;
    <% end if %>
}

function setOwnership()
{
  <% if Application("ENABLE_OWNERSHIP")="TRUE" then %>
   var type="";
    <%if (isEdit or lcase(PageSource) = "eln") and (lcase(sTab) = "required" or (sTab = "" and lcase(Session("sTab2")) = "required"))  then %>
       type=document.getElementById("OwnershipType").value;  
    <%elseif PrincipalID <> "" and lcase(sTab) = "required" then %>
       type=<%=PrincipalID%>
    <%end if %>
    
    if(type!="")
    {
         document.getElementById("PrincipalID").value=document.getElementById("OwnershipType").value
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
       document.form1.PrincipalID.value=type;
    }
   <% end if %>
       
}
	// Validates container attributes
	function ValidateContainer(strMode){
		var bWriteError = false;
		var bWriteWarning = false;
		var errmsg = "Please fix the following problems:\r\r";
		var warningmsg = "Please read the following warnings:\r\r";

		//Populate hidden variables
		!document.form1.iBarcode ? document.form1.Barcode.value="<%=Barcode%>" : document.form1.Barcode.value = document.form1.iBarcode.value;
		!document.form1.iBarcodeDescID ? document.form1.BarcodeDescID.value="<%=BarcodeDescID%>" : document.form1.BarcodeDescID.value = document.form1.iBarcodeDescID.value;
		!document.form1.iContainerName ? document.form1.ContainerName.value="<%=ContainerName%>" : document.form1.ContainerName.value = document.form1.iContainerName.value;
		!document.form1.iComments ? document.form1.Comments.value=document.form1.Comments.value : document.form1.Comments.value = document.form1.iComments.value;
		!document.form1.iStorageConditions ? document.form1.StorageConditions.value=document.form1.StorageConditions.value : document.form1.StorageConditions.value = document.form1.iStorageConditions.value;
		!document.form1.iHandlingProcedures ? document.form1.HandlingProcedures.value=document.form1.HandlingProcedures.value : document.form1.HandlingProcedures.value = document.form1.iHandlingProcedures.value;
		!document.form1.iUOMID ? document.form1.UOMID.value="<%=UOMID%>" : document.form1.UOMID.value = document.form1.iUOMID.value;
		document.form1.UOMID.value = document.form1.UOMID.value.split("=")[0];
		!document.form1.iUOWID ? document.form1.UOWID.value="<%=UOWID%>" : document.form1.UOWID.value = document.form1.iUOWID.value;
		document.form1.UOWID.value = document.form1.UOWID.value.split("=")[0];
		!document.form1.iUOCID ? document.form1.UOCID.value="<%=UOCID%>" : document.form1.UOCID.value = document.form1.iUOCID.value;
		document.form1.UOCID.value = document.form1.UOCID.value.split("=")[0];
		!document.form1.iUODID ? document.form1.UODID.value="<%=UODID%>" : document.form1.UODID.value = document.form1.iUODID.value;
		document.form1.UODID.value = document.form1.UODID.value.split("=")[0];
		!document.form1.iUOPID ? document.form1.UOPID.value="<%=UOPID%>" : document.form1.UOPID.value = document.form1.iUOPID.value;
		document.form1.UOPID.value = document.form1.UOPID.value.split("=")[0];
		!document.form1.iUOCostID ? document.form1.UOCostID.value="<%=UOCostID%>" : document.form1.UOCostID.value = document.form1.iUOCostID.value;
		document.form1.UOCostID.value = document.form1.UOCostID.value.split("=")[0];
		!document.form1.iContainerTypeID ? document.form1.ContainerTypeID.value="<%=ContainerTypeID%>" : document.form1.ContainerTypeID.value = document.form1.iContainerTypeID.value;
		!document.form1.iContainerStatusID ? document.form1.ContainerStatusID.value="<%=ContainerStatusID%>" : document.form1.ContainerStatusID.value = document.form1.iContainerStatusID.value;
		!document.form1.iContainerDesc ? document.form1.ContainerDesc.value="<%=ContainerDesc%>" : document.form1.ContainerDesc.value = document.form1.iContainerDesc.value;
		!document.form1.iQtyMax ? document.form1.QtyMax.value="<%=QtyMax%>" : document.form1.QtyMax.value = document.form1.iQtyMax.value;
		!document.form1.iQtyInitial ? document.form1.QtyInitial.value="<%=QtyInitial%>" : document.form1.QtyInitial.value = document.form1.iQtyInitial.value;
		!document.form1.iFinalWeight ? document.form1.FinalWeight.value="<%=FinalWeight%>" : document.form1.FinalWeight.value = document.form1.iFinalWeight.value;
		!document.form1.iTareWeight ? document.form1.TareWeight.value="<%=TareWeight%>" : document.form1.TareWeight.value = document.form1.iTareWeight.value;
		!document.form1.iNetWeight ? document.form1.NetWeight.value="<%=NetWeight%>" : document.form1.NetWeight.value = document.form1.iNetWeight.value;
		!document.form1.iQtyRemaining ? document.form1.QtyRemaining.value="<%=QtyRemaining%>" : document.form1.QtyRemaining.value = document.form1.iQtyRemaining.value;
		!document.form1.iMinStockQty ? document.form1.MinStockQty.value="<%=MinStockQty%>" : document.form1.MinStockQty.value = document.form1.iMinStockQty.value;
		!document.form1.iMaxStockQty ? document.form1.MaxStockQty.value="<%=MaxStockQty%>" : document.form1.MaxStockQty.value = document.form1.iMaxStockQty.value;
		!document.form1.iCompoundID ? document.form1.CompoundID.value="<%=CompoundID%>" : document.form1.CompoundID.value = document.form1.iCompoundID.value;
		!document.form1.iConcentration ? document.form1.Concentration.value="<%=Concentration%>" : document.form1.Concentration.value = document.form1.iConcentration.value;
		!document.form1.iDensity ? document.form1.Density.value="<%=Density%>" : document.form1.Density.value = document.form1.iDensity.value;
		!document.form1.iRegID ? document.form1.RegID.value="<%=RegID%>" : document.form1.RegID.value = document.form1.iRegID.value;
		!document.form1.iBatchNumber ? document.form1.BatchNumber.value="<%=Batchnumber%>" : document.form1.BatchNumber.value = document.form1.iBatchNumber.value;
		!document.form1.iPurity ? document.form1.Purity.value="<%=Purity%>" : document.form1.Purity.value = document.form1.iPurity.value;
		!document.form1.iGrade ? document.form1.Grade.value="<%=Grade%>" : document.form1.Grade.value = document.form1.iGrade.value;
		!document.form1.iSolventIDFK ? document.form1.SolventIDFK.value="<%=SolventIDFK%>" : document.form1.SolventIDFK.value = document.form1.iSolventIDFK.value;
		!document.form1.iExpDate ? document.form1.ExpDate.value="<%=ExpDate%>" : document.form1.ExpDate.value = document.form1.iExpDate.value;
		!document.form1.iDateCertified ? document.form1.DateCertified.value="<%=DateCertified%>" : document.form1.DateCertified.value = document.form1.iDateCertified.value;
		!document.form1.iDateApproved ? document.form1.DateApproved.value="<%=DateApproved%>" : document.form1.DateApproved.value = document.form1.iDateApproved.value;
		!document.form1.iSupplierID ? document.form1.SupplierID.value="<%=SupplierID%>" : document.form1.SupplierID.value = document.form1.iSupplierID.value;
		!document.form1.iLotNum ? document.form1.LotNum.value="<%=LotNum%>" : document.form1.LotNum.value = document.form1.iLotNum.value;
		!document.form1.iPONumber ? document.form1.PONumber.value="<%=PONumber%>" : document.form1.PONumber.value = document.form1.iPONumber.value;
		!document.form1.iPOLineNumber ? document.form1.POLineNumber.value="<%=POLineNumber%>" : document.form1.POLineNumber.value = document.form1.iPOLineNumber.value;
		!document.form1.iReqNumber ? document.form1.ReqNumber.value="<%=ReqNumber%>" : document.form1.ReqNumber.value = document.form1.iReqNumber.value;
		!document.form1.iSupplierCatNum ? document.form1.SupplierCatNum.value="<%=SupplierCatNum%>" : document.form1.SupplierCatNum.value = document.form1.iSupplierCatNum.value;
		!document.form1.iDateProduced ? document.form1.DateProduced.value="<%=DateProduced%>" : document.form1.DateProduced.value = document.form1.iDateProduced.value;
		!document.form1.iDateOrdered ? document.form1.DateOrdered.value="<%=DateOrdered%>" : document.form1.DateOrdered.value = document.form1.iDateOrdered.value;
		!document.form1.iDateReceived ? document.form1.DateReceived.value="<%=DateReceived%>" : document.form1.DateReceived.value = document.form1.iDateReceived.value;
		!document.form1.iContainerCost ? document.form1.ContainerCost.value= "<%=ContainerCost%>" : document.form1.ContainerCost.value = document.form1.iContainerCost.value;
		!document.form1.iOwnerID ? document.form1.OwnerID.value="<%=OwnerID%>" : document.form1.OwnerID.value = document.form1.iOwnerID.value;
		!document.form1.iCurrentUserID ? document.form1.CurrentUserID.value="<%=CurrentUserID%>" : document.form1.CurrentUserID.value = document.form1.iCurrentUserID.value;
		!document.form1.iLocationID ? document.form1.LocationID.value="<%=LocationID%>" : document.form1.LocationID.value = document.form1.iLocationID.value;
		!document.form1.iNumCopies ? document.form1.NumCopies.value="<%=NumCopies%>" : document.form1.NumCopies.value = document.form1.iNumCopies.value;
		<% if Application("ENABLE_OWNERSHIP")="TRUE" then %>
		!document.form1.Ownershiplst ? document.form1.PrincipalID.value="<%=PrincipalID%>" :  document.form1.PrincipalID.value=document.form1.Ownershiplst.value;
		!document.form1.iLocationTypeID ? document.form1.LocationTypeID.value="<%=LocationTypeID%>" : document.form1.LocationTypeID.value = document.form1.iLocationTypeID.value;
		<% end if %>
		<%if NOT lcase(Application("HideOtherTab")) = "true" then%>
		<% For each Key in custom_fields_dict%>!document.form1.i<%=Key%> ? document.form1.<%=Key%>.value="<%=Eval(Key)%>" : document.form1.<%=Key%>.value = document.form1.i<%=Key%>.value;<%=vblf%><%next%>
		<%end if%>

		//LocationID is required
		document.form1.HREF.value = location.href;

		if (document.form1.LocationID.value.length == 0) {
			errmsg = errmsg + "- Location ID is required.\r";
			bWriteError = true;
		}
		else{
			// LocationID must be a psositve number
			if (document.form1.LocationID.value == 0){
				errmsg = errmsg + "- Cannot create container at the root location.\r";
				bWriteError = true;
			}
			// LocationID must be a psositve number
			if (!isPositiveNumber(document.form1.LocationID.value)&&(document.form1.LocationID.value != 0)){
				errmsg = errmsg + "- Location ID must be a positive number.\r";
				bWriteError = true;
			}
		}
		<%if Application("ENABLE_OWNERSHIP")="TRUE" and not isEdit then%>
		     if(GetAuthorizedLocation(document.form1.LocationID.value,document.getElementById("tempCsUserName").value,document.getElementById("tempCsUserID").value)==0)
            {
                errmsg = errmsg + "- Not authorized for this location.\r";
			    alert(errmsg);
			    return;
            }
        if(document.form1.LocationTypeID.value!=0)
        {
                if(GetValidLocation(document.form1.LocationID.value,"",document.form1.LocationTypeID.value)==0)
              {
                    if (confirm("- Location type is not match with this location.\r - Do you want to continue...")!=true)
                    {
                        return;
                    }
			        
              }
        }
		
		//Container Admin is required
		if (document.form1.PrincipalID.value.length == 0) {
			errmsg = errmsg + "- Container Admin is required.\r";
			bWriteError = true;
		}
        //check if location and Container admin are not same
        if (document.form1.LocationAdmin.value!='' && document.form1.LocationAdmin.value !=document.form1.PrincipalID.value){
            if (confirm("- The Location Admin and Container Admin are not the same,\r Do you really want to continue?")!=true){     
                return;
            }
        }
	<% end if %>
       <%if Application("ENABLE_OWNERSHIP")="TRUE" and  isEdit then%>
          //Container Admin is required
		if (document.form1.PrincipalID.value.length == 0) {
			errmsg = errmsg + "- Container Admin is required.\r";
			bWriteError = true;
		}
        <% end if %>
/*
		<% if Application("RACKS_ENABLED") then %>
		// Validation for Rack assignment
		!document.form1.iRackGridID ? document.form1.RackGridID.value="<%=RackGridID%>" : document.form1.RackGridID.value = document.form1.iRackGridID.value;
		if (document.form1.AssignToRack) {
			if (document.form1.AssignToRack.checked) {

				// Validate Rack Grid ID
				if (document.form1.RackGridID.value.length == 0){
					errmsg = errmsg + "- Please select a valid Rack grid location.\r";
					bWriteError = true;
				}else if (!isPositiveNumber(document.form1.RackGridID.value)&&(document.form1.RackGridID.value != 0)){
					errmsg = errmsg + "- Rack grid location must be a positive number.\r";
					bWriteError = true;
				<% if not isEdit then %>
				}else if (document.form1.RackGridList.value.length == 0) {
					errmsg = errmsg + "- Please choose a Rack grid location for this container.\r";
					bWriteError = true;
				<% end if %>
				}else{
					<% if not isEdit then %>
					var tmpRackGridList = document.form1.RackGridList.value;
					tmpRackGridList = tmpRackGridList.split(",");
					if (document.form1.NumCopies.value != tmpRackGridList.length){
						//errmsg = errmsg + "- The number of Rack grid locations (" + tmpRackGridList.length +") does not match the number of copies requested (" + document.form1.NumCopies.value + ").\r Please selects Racks and Rack starting position again. When creating multi copies, you must manually select a starting position.";
						errmsg = errmsg + "- When creating multiple copies, please select a starting position for the multiple copies.\r\r";
						bWriteError = true;
					}
					<% end if %>
					document.form1.LocationID.value = document.form1.RackGridID.value;
				}

			}
			// Destination cannot be assign to a rack parent
			if (!document.form1.AssignToRack.checked){
				if (ValidateLocationIDType(document.form1.LocationID.value) == 1) {
					errmsg = errmsg + "- Destination can not be a Rack. If you would like to move the Container \rinto a Rack, please click \"Assign to Rack\" and choose a rack position.\r";
					bWriteError = true;
					//document.form1.AssignToRack.checked = true;
					//toggleRackDisplay();
				}
				if (document.form1.iLocationID) {
					document.form1.LocationID.value = document.form1.iLocationID.value;
				}
			}
		}
		<% end if %>
*/
		//bWriteError = true;
		// If in any other tab than Required, use the actual location id instead of parent location id in case of Rack

		<% if sTab <> "Required" and sTab <> "" and Session("OrigLocationID") <> "" and LocationID="" then %>
		document.form1.LocationID.value = "<%=Session("OrigLocationID")%>";
		<% end if %>

		// Container Name is required
		if (document.form1.ContainerName.value.length == 0) {
			errmsg = errmsg + "- Container Name is required.\r";
			bWriteError = true;
		}

		// QtyMax is required
		if (document.form1.QtyMax.value.length == 0) {
			errmsg = errmsg + "- Container size is required.\r";
			bWriteError = true;
		}
		else{
			// QtyMax must be a number
			if (!isPositiveNumber(document.form1.QtyMax.value)){
			errmsg = errmsg + "- Container size must be a positive number.\r";
			bWriteError = true;
			}
			if (document.form1.QtyMax.value > 999999999){
			errmsg = errmsg + "- Container size is too large.\r";
			bWriteError = true;
			}
		}
		//QtyMax should not have comma
		var m = document.form1.QtyMax.value.toString();		
		if(m.indexOf(",") != -1){
			errmsg = errmsg + "- Container size has wrong decimal operator.\r";
			bWriteError = true;
		}
<% if NOT isEdit then%>
		// Initial amount is required
		if (document.form1.QtyInitial.value.length == 0) {
			errmsg = errmsg + "- Initial amount is required.\r";
			bWriteError = true;
		}
		else{
			// Initial amount must be a number
			if (!isWholeNumber(document.form1.QtyInitial.value)){
			errmsg = errmsg + "- Initial amount must be a number zero or greater.\r";
			bWriteError = true;
			}
			if (document.form1.QtyInitial.value > 999999999){
			errmsg = errmsg + "- Initial amount is too large.\r";
			bWriteError = true;
			}
		}
		    //Initial amount must be less than Container size 
		    if (Number(document.form1.QtyInitial.value) > Number(document.form1.QtyMax.value)){
			errmsg = errmsg + "- Initial amount cannot exceed Container size.\r";
			bWriteError = true;
			}
			//Initial amount should not have comma
			var m = document.form1.QtyInitial.value.toString();		
		if(m.indexOf(",") != -1){
			errmsg = errmsg + "- Initial amount has wrong decimal operator.\r";
			bWriteError = true;
		}
			
		// NumCopies is required
		if (document.form1.NumCopies.value.length == 0) {
			errmsg = errmsg + "- Number of Copies is required.\r";
			bWriteError = true;
		}
		else{
			// NumCopies must be a number
			if (!isPositiveNumber(document.form1.NumCopies.value)){
				errmsg = errmsg + "- Number of copies must be a positive number.\r";
				bWriteError = true;
			}
			else {
				if (document.form1.NumCopies.value > 399){
					errmsg = errmsg + "- Number of copies must be less than 400.\r";
					bWriteError = true;
				}
			}
		}
		// determine whether there are enough open rack positions
		if (document.form1.isRack.value == "1") {
		    if (AreEnoughRackPositions(document.form1.NumCopies.value,document.form1.LocationID.value) == false) {
				errmsg = errmsg + "- There are not enough open positions from the selected start position\rin the rack(s) you have selected.\r";
				bWriteError = true;
		    }
		}
		if ((document.form1.AutoGen.value == 'false') && (document.form1.Barcode.value.length != 0))  {
		    if (isUniqueBarcode(document.form1.Barcode.value) == false) {
		    errmsg = errmsg + "- Not a Unique Container ID.\r";
			bWriteError = true;
		    }
		}
		
        /*
		alert(document.form1.isRack.value);
		alert(document.form1.NumCopies.value);
		alert(document.form1.LocationID.value);
		alert(AreEnoughRackPositions(document.form1.NumCopies.value,document.form1.LocationID.value));
		bWriteError = true
		*/
<%else%>
		// QtyRemaining is required
		if (document.form1.QtyRemaining.value.length == 0) {
			errmsg = errmsg + "- Quantity Remaining is required.\r";
			bWriteError = true;
		}
		else{
			// QtyMax must be a number
			if (!isWholeNumber(document.form1.QtyRemaining.value)){
			errmsg = errmsg + "- Quantity Remaining must be a number zero or greater.\r";
			bWriteError = true;
			}
			if (document.form1.QtyRemaining.value/1 > document.form1.QtyMax.value/1){
			errmsg = errmsg + "- Quantity Remaining cannot be larger than Container Size.\r";
			bWriteError = true;
			}
			if (document.form1.QtyRemaining.value < <%=TotalQtyReserved%>){
			warningmsg = warningmsg + "- The Quantity Remaining is less than the quantity currently reserved.\rIf you choose 'OK', all active reservations will be removed.\rDo you want to proceed?\r";
			bWriteWarning = true;
			}
			//Quantity Remaining if present should not have comma
			var m = document.form1.QtyRemaining.value.toString();		
			if(m.indexOf(",") != -1){
			errmsg = errmsg + "- Quantity Remaining amount has wrong decimal operator.\r";
			bWriteError = true;
			}
		}
		if (document.form1.Barcode.value.length != 0)  {
		        if (document.form1.Barcode.value != '<%=Barcode%>'){
		        if (isUniqueBarcode(document.form1.Barcode.value) == false) {
		            errmsg = errmsg + "- Not a Unique Container ID.\r";
			        bWriteError = true;
		        }
		        }
		}		
<% end if%>
<%if lcase(Application("RequireBarcode")) = "true" and NOT isEdit then%>
		//barcode description or Container ID is required
		if (document.form1.AutoGen.value == "true") {
			if(document.form1.BarcodeDescID.value.length == 0) {
				errmsg = errmsg + "- Barcode Description is required.\r";
				bWriteError = true;
			}
		}
		else {
			if(document.form1.Barcode.value.length == 0) {
				errmsg = errmsg + "- Container ID is required.\r";
				bWriteError = true;
			}
		}
<% end if%>
		<% if isEdit then %>
		if(document.form1.Barcode.value.length == 0) {
			errmsg = errmsg + "- Container ID is required.\r";
			bWriteError = true;
		}
		<% end if %>
		// MaxStockQty if present must be a number
		if (document.form1.MaxStockQty.value.length >0 && !isPositiveNumber(document.form1.MaxStockQty.value)){
			errmsg = errmsg + "- Maximum stock threshold must be a positive number.\r";
			bWriteError = true;
		}
		// MinStockQty if present must be a number
		if (document.form1.MinStockQty.value.length >0 && !isPositiveNumber(document.form1.MinStockQty.value)){
			errmsg = errmsg + "- Minimum stock threshold must be a positive number.\r";
			bWriteError = true;
		}
		// TareWeight if present must be a number
		if (document.form1.TareWeight.value.length >0 && !isPositiveNumber(document.form1.TareWeight.value)){
			errmsg = errmsg + "- Tare Weight must be a positive number.\r";
			bWriteError = true;
		}

		// NetWeight if present must be a number
		if (document.form1.NetWeight.value.length >0 && !isPositiveNumber(document.form1.NetWeight.value)){
			errmsg = errmsg + "- Net Weight must be a positive number.\r";
			bWriteError = true;
		}

		// FinalWeight if present must be a number
		if (document.form1.FinalWeight.value.length >0 && !isPositiveNumber(document.form1.FinalWeight.value)){
			errmsg = errmsg + "- Total Weight must be a positive number.\r";
			bWriteError = true;
		}

		// Catalog Number if present can't be longer than 50 characters
		if (document.form1.SupplierCatNum.value.length > 50){
			errmsg = errmsg + "- Catalog Number must be 50 characters or less.\r";
			bWriteError = true;
		}

		// Purity if present must be a number
		if (document.form1.Purity.value.length >0 && !isPositiveNumber(document.form1.Purity.value)){
			errmsg = errmsg + "- Purity must be a positive number.\r";
			bWriteError = true;
		}		
		// Purity if present should not have comma
		var m = document.form1.Purity.value.toString();		
		if(m.indexOf(",") != -1){
		errmsg = errmsg + "- Purity has wrong decimal operator.\r";
		bWriteError = true;
		}
		// Concentration if present must be a number
		if (document.form1.Concentration.value.length >0 && !isPositiveNumber(document.form1.Concentration.value)){
			errmsg = errmsg + "- Concentration must be a positive number.\r";
			bWriteError = true;
		}		
		//Concentration if present should not have comma
		var m = document.form1.Concentration.value.toString();		
		if(m.indexOf(",") != -1){
		errmsg = errmsg + "- Concentration has wrong decimal operator.\r";
		bWriteError = true;
		}
		// PO Line Number if present must be a number and 4 digits or less
		if (document.form1.POLineNumber.value.length > 0) {
			if (!isPositiveNumber(document.form1.POLineNumber.value)) {
				errmsg = errmsg + "- PO Line Number must be a positive number.\r";
				bWriteError = true;
			}
			else {
				if (document.form1.POLineNumber.value > 9999){
					errmsg = errmsg + "- PO Line Number is too large.  It must less than 10000.\r";
					bWriteError = true;
				}
			}
		}
		// Density if present must be a number
		if (document.form1.Density.value.length >0 && !isPositiveNumber(document.form1.Density.value)){
			errmsg = errmsg + "- Density must be a positive number.\r";
			bWriteError = true;
		}
		// Density if present should not have comma
		var m = document.form1.Density.value.toString();		
		if(m.indexOf(",") != -1){
		errmsg = errmsg + "- Density has wrong decimal operator.\r";
		bWriteError = true;
		}
		// Date Produced must be a date
		if (document.form1.DateProduced.value.length > 0 && !isDate(document.form1.DateProduced.value)){
			errmsg = errmsg + "- Date Produced must be in " + dateFormatString + " format.\r";
			bWriteError = true;
		}
		// Expiration must be a date
		if (document.form1.ExpDate.value.length > 0 && !isDate(document.form1.ExpDate.value)) {
			errmsg = errmsg + "- Expiration Date must be in " + dateFormatString + " format.\r";
			bWriteError = true;
		}
		// Date Certified must be a date
		if (document.form1.DateCertified.value.length > 0 && !isDate(document.form1.DateCertified.value)) {
			errmsg = errmsg + "- Date Certified must be in " + dateFormatString + " format.\r";
			bWriteError = true;
		}
		// Date Approved must be a date
		if (document.form1.DateApproved.value.length > 0 && !isDate(document.form1.DateApproved.value)) {
			errmsg = errmsg + "- Date Approved must be in " + dateFormatString + " format.\r";
			bWriteError = true;
		}
		// Date Ordered must be a date
		if (document.form1.DateOrdered.value.length > 0 && !isDate(document.form1.DateOrdered.value)){
			errmsg = errmsg + "- Date Ordered must be in " + dateFormatString + " format.\r";
			bWriteError = true;
		}
		// Data Received must be a date
		if (document.form1.DateReceived.value.length > 0 && !isDate(document.form1.DateReceived.value)){
			errmsg = errmsg + "- Date Received must be in " + dateFormatString + " format.\r";
			bWriteError = true;
		}

		// Final validation of RegBatch
		if (document.form1.RegBatchID) {
			if (document.form1.RegBatchID.value.length > 0) {
				GetRegIDFromRegNumValidateOnly(document.form1.RegBatchID.value);
				if (document.form1.RegNumFinalValidate.value == 1) {
					errmsg = errmsg + "- " + document.form1.RegNumFinalValidateMsg.value + ".\r";
					bWriteError = true;
				}
			}
		}

		// Cost must be a number
		if (document.form1.ContainerCost.value.length >0){
			if (!isPositiveNumber(document.form1.ContainerCost.value)){
			errmsg = errmsg + "- Container cost must be a positive number.\r";
			bWriteError = true;
			}
			else{
				if (document.form1.ContainerCost.value > 999999999){
					errmsg = errmsg + "- Container cost is too large.\r";
					bWriteError = true;
				}
			}
		}

		/*CSBR ID:107836
		 *Change Done by : Soorya
		 *Purpose: Restrict Comments Textfield to 2000 characters
		 *Date: 21/01/2010		
		*/
		//Comments Field must not have more than 2000 characters
		if (document.form1.Comments.value.length > 2000)
		    {			
			errmsg = errmsg + "- Comments Field cannot be greater than 2000 characters.\r";
			bWriteError = true;
			}
		//Storage Conditions Field must not have more than 2000 characters
		if (document.form1.StorageConditions.value.length > 2000)
		    {			
			errmsg = errmsg + "- Storage Conditions Field cannot be greater than 2000 characters.\r";
			bWriteError = true;
			}
        //Handling Procedures Field must not have more than 2000 characters
		if (document.form1.HandlingProcedures.value.length > 2000)
		    {			
			errmsg = errmsg + "- Handling Procedures Field cannot be greater than 2000 characters.\r";
			bWriteError = true;
			} 
    
		/*End of Change #107836#*/	

		//Custom field validation

		<%if NOT lcase(Application("HideOtherTab")) = "true" then
			For each Key in custom_fields_dict
				If InStr(Key, "DATE") then%>
					if (document.form1.<%=Key%>.value.length > 0 && !isDate(document.form1.<%=Key%>.value)){
						errmsg = errmsg + "- <%=custom_fields_dict.Item(Key)%> must be in " + dateFormatString + " format.\r";
						bWriteError = true;
					}
				<%end if%>
			<%next%>

			//Validate requried custom fields
			<% For each Key in req_custom_fields_dict%>
				if (document.form1.<%=Key%>.value.length == 0) {
					errmsg = errmsg + "- <%=req_custom_fields_dict.Item(Key)%> is required.\r";
					bWriteError = true;
				}
			<%Next%>
		<%end if%>
		// Report problems

		//bWriteError = true;
		if (bWriteError){
			alert(errmsg);
		}
		else{
			if (strMode.toLowerCase() == "edit"){
				document.form1.action = "EditContainer_action.asp";
			}
			else{
				document.form1.action = "CreateContainer_action.asp";
			}
			
			var bcontinue = true;

			// Report warnings, user can choose to accept or cancel
			bConfirmWarning = true;
			if (bWriteWarning) {
				bConfirmWarning = confirm(warningmsg);
			}
			if (!bConfirmWarning) var bcontinue = false;
			<%if Application("WARN_FOR_NO_COMPOUND") then %>
			if ((document.form1.CompoundID.value.length == 0)&&(document.form1.RegID.value.length == 0)){
				bcontinue = confirm("No Compound has been asigned to this container.\rDo you really want to create a container without an associated chemical compound?");
			}
			<%END IF%>
			if (bcontinue) document.form1.submit();
		}
	}

	// Post data between tabs
	function postDataFunction(sTab) {
		document.form1.action = "CreateOrEditContainer.asp?GetData=form&Source=<%=PageSource%>&TB=" + sTab
		document.form1.submit()
	}

	function validateLocation(LocationID){
		/*
		if (ValidateLocationIDType(LocationID) == 1){
			alert("You are not allowed to choose a Rack location. \rTo assign container(s) to a Rack, please check \"Assign to Rack\".");
			UpdateLocationPickerFromID(<%=LocationID%>,document.form1,'iLocationID','lpLocationBarCode', 'lpLocationName');
			document.form1.iLocationID.value=<%=LocationID%>;
			document.form1.lpLocationBarCode.value=<%=LocationID%>;
			document.form1.lpLocationName.disabled = false;
			document.form1.IsRack.value=true;
		} else {
		*/
		/*
		if (ValidateLocationIDType(LocationID) == 1){
			if (document.form1.AssignToRack){
				document.form1.AssignToRack.checked = true;
			}
		}
		*/
		CurrLocationID = 0;
		CurrLocationID = <%=LocationID%>;
		if (document.form1.iLocationID.value != CurrLocationID){
			//alert("in edit, refresh page " + CurrLocationID + ": " + document.form1.iLocationID.value);
			//alert("/cheminv/GUI/CreateOrEditContainer.asp?isEdit=true&getData=db&ContainerID=" + <%=ContainerID%> + "&LocationID="+LocationID);
			<% if isEdit then %>
				document.form1.action = "/cheminv/GUI/CreateOrEditContainer.asp?isEdit=true&getData=db&ContainerID=<%=Session("ContainerID")%>&LocationID="+LocationID+"&ContainerName=<%=ContainerName%>&QtyInitial=<%=QtyInitial%>&QtyMax=<%=QtyMax%>&RegBatchID=<%=RegBatchID%>&CompoundID=<%=CompoundID%>&Concentration=<%=Concentration%>&UOCIDOptionValue=<%=Session("UOCIDOptionValue")%>&toggleDefaults=true";
				document.form1.submit();
			<% else %>
				document.form1.action = "/cheminv/GUI/CreateOrEditContainer.asp?GetData=new&LocationID="+LocationID+"&AssignToRack=&NewSession=true&ContainerName=<%=ContainerName%>&QtyInitial=<%=QtyInitial%>&QtyMax=<%=QtyMax%>&RegBatchID=<%=RegBatchID%>&CompoundID=<%=CompoundID%>&Concentration=<%=Concentration%>&UOCIDOptionValue=<%=Session("UOCIDOptionValue")%>&toggleDefaults=true";
				document.form1.submit();
			<% end if %>
		}

		//}
	}

	// set compound info
	function validateCompoundID(compoundElement)
	{
		if(IsValidCompoundID(compoundElement.value, true) == 1){
			setTimeout("setCompoundValues('" + compoundElement.value + "');", 1);
		}
		else{
			setTimeout("clearCompoundID();", 1);
		}
	}

	function setCompoundValues(value) {
		document.form1.NewCompoundID.value = value;
		document.form1.RegBatchID.value = '';
		document.form1.iRegID.value = '';
		document.form1.iBatchNumber.value = '';
	}

	function clearCompoundID() {
		document.form1.iCompoundID.value = '';
	}
	
	function ValidateCompoundID2( obj, bReg )
	{
	    if( obj.value.length == 0 )
	    {
	        return;
	    }
	    if( IsValidCompoundID( obj.value, true ) == 1 )
	    {
	        document.form1.NewCompoundID.value= obj.value;
	        if( bReg )
	        {
	            document.form1.RegBatchID.value='';
                document.form1.iRegID.value='';
                document.form1.iBatchNumber.value='';
            }
        }
        else
        {
            obj.value = '';
        }
	}
//-->
</script>
<!-- CSBR# 137130 -->
<!-- Purpose of Change - To remove the onload = setOwnership() from body attribute and to have it added to the window.onload-->
<!--                     The additional onload in the body attribute caused the window.onload not to fire. So this change was required-->
 
<%if Session("isCDP") = "TRUE" then%>
<script language="JavaScript" src="/cfserverasp/source/chemdraw.js"></script>
<script>cd_includeWrapperFile("/cfserverasp/source/")</script>
<script language="javascript">
	<!--
	// Calculates molw and formula from plugin data
		var holdTime = 3000;
		if (cd_getBrowserVersion() >= 6) holdTime = 1;
		window.onload = function(){setTimeout("GetFormula();GetMolWeight();setOwnership();",holdTime)}			
	//-->
</script>
<% else %>
<script language="javascript">
	window.onload = function(){setOwnership();}
</script>
<% end if %>

</head>
<body style="overflow:auto">
<!--End of Change for CSBR 137130-->
<!--#INCLUDE VIRTUAL = "/cheminv/gui/CreateOrEditContainerTabs.asp"-->

<form name="form1" method="POST">
<input TYPE="hidden" NAME="ContainerID" Value="<%=ContainerID%>">
<input TYPE="hidden" NAME="Barcode" Value>
<input TYPE="hidden" NAME="BarcodeDescID" Value>
<input TYPE="hidden" NAME="LocationID" Value>
<input TYPE="hidden" NAME="ContainerTypeID" Value="=">
<input TYPE="hidden" NAME="ContainerStatusID" Value="=">
<input TYPE="hidden" NAME="UOMID" Value>
<input TYPE="hidden" NAME="UOWID" Value>
<input TYPE="hidden" NAME="UOCID" Value>
<input TYPE="hidden" NAME="UODID" Value>
<input TYPE="hidden" NAME="UOPID" Value>
<input TYPE="hidden" NAME="ContainerDesc" VALUE="<%=ContainerDesc%>">
<input TYPE="hidden" NAME="QtyMax" Value>
<input TYPE="hidden" NAME="QtyInitial" Value>
<input TYPE="hidden" NAME="QtyRemaining" Value="<%=QtyRemaining%>">
<input TYPE="hidden" NAME="CompoundID" Value>
<input TYPE="hidden" NAME="ContainerName" Value>
<input TYPE="hidden" NAME="Comments" Value="<%=Comments%>">
<input TYPE="hidden" NAME="StorageConditions" Value="<%=StorageConditions%>">
<input TYPE="hidden" NAME="HandlingProcedures" Value="<%=HandlingProcedures%>">
<input TYPE="hidden" NAME="MinStockQty" Value>
<input TYPE="hidden" NAME="MaxStockQty" Value>
<input TYPE="hidden" NAME="ExpDate" Value>
<input TYPE="hidden" NAME="DateCertified">
<input TYPE="hidden" NAME="DateApproved">
<input TYPE="hidden" NAME="TareWeight" Value>
<input TYPE="hidden" NAME="NetWeight" Value>
<input TYPE="hidden" NAME="FinalWeight" Value>
<input TYPE="hidden" NAME="RegID" Value>
<input TYPE="hidden" NAME="BatchNumber" Value>
<input TYPE="hidden" NAME="Purity" Value>
<input TYPE="hidden" NAME="Concentration" Value>
<input TYPE="hidden" NAME="Density" Value>
<input TYPE="hidden" NAME="Grade" Value>
<input TYPE="hidden" NAME="SolventIDFK" Value>
<input TYPE="hidden" NAME="SupplierID" Value>
<input TYPE="hidden" NAME="SupplierCatNum" Value>
<input TYPE="hidden" NAME="LotNum" Value>
<input TYPE="hidden" NAME="PONumber" Value>
<input TYPE="hidden" NAME="POLineNumber" Value>
<input TYPE="hidden" NAME="ReqNumber" Value>
<input TYPE="hidden" NAME="DateProduced" Value>
<input TYPE="hidden" NAME="DateOrdered" Value>
<input TYPE="hidden" NAME="DateReceived" Value>
<input TYPE="hidden" NAME="ContainerCost" Value>
<input TYPE="hidden" NAME="UOCostID" Value>
<input TYPE="hidden" NAME="OwnerID" Value>
<input TYPE="hidden" NAME="CurrentUserID" Value>
<input TYPE="hidden" NAME="NumCopies" Value>
<input TYPE="hidden" NAME="RegNumFinalValidate" Value>
<input TYPE="hidden" NAME="RegNumFinalValidateMsg" Value>
<input type="hidden" name="RackGridID" value="<%=RackGridID%>" />
<input TYPE="hidden" NAME="AutoGen" Value="<%=AutoGen%>">
<input TYPE="hidden" NAME="tempCsUserName" id="tempCsUserName" Value="<%=Session("UserName" & "cheminv")%>" >
<input TYPE="hidden" NAME="tempCsUserID" id="tempCsUserID" Value=<%=Server.URLEncode(CryptVBS(Session("UserID" & "cheminv"),"ChemInv\API\GetBatchInfo.asp"))%>>
<% if Application("ENABLE_OWNERSHIP")="TRUE" then %>
<input TYPE="hidden" NAME="OwnerShipGroupList" id="OwnerShipGroupList" Value="<%=GetOwnerShipGroupList()%>">
<input TYPE="hidden" NAME="OwnerShipUserList" id ="OwnerShipUserList" Value="<%=GetOwnerShipUserList()%>">
<input TYPE="hidden" NAME="PrincipalID" id="PrincipalID" Value=<%=PrincipalID%>>
<input type="hidden" NAME="OwnershipType" id="OwnershipType" value="<%=OwnershipType%>" />
<input TYPE="hidden" NAME="LocationTypeID" Value>
<input TYPE="hidden" NAME="LocationAdmin" id="LocationAdmin" Value="<%=LocationAdmin%>">
<% end if %>
<% if Request("RefreshOpenerLocation") <> "" then %>
<input TYPE="hidden" NAME="RefreshOpenerLocation" Value="<%=lcase(Request("RefreshOpenerLocation")) %>" />
<% end if %>
<input TYPE="hidden" NAME="PageSource" Value="<%=PageSource%>">
<!--<% if Session("sTab2") <> "Required" then %>
<input TYPE="hidden" NAME="AssignToRack" Value="<%=AssignToRack%>">
	<% if AssignToRack = "on" then %>
	<input TYPE="hidden" NAME="toggleDefaults" Value="true">
	<% end if %>
<% end if %>
-->

<%if sTab <> "Required" then %>
<input TYPE="hidden" NAME="isRack" Value="<%=isRack%>">
<% end if %>
<input TYPE="hidden" NAME="AutoPrint" Value="<%=AutoPrint%>">
<input TYPE="hidden" NAME="returnToSearch" Value="<%=ReturnToSearch%>">
<input TYPE="hidden" NAME="ReturnToReconcile" Value="<%=ReturnToReconcile%>">
<input TYPE="hidden" NAME="HREF" VALUE>
<%if NOT lcase(Application("HideOtherTab")) = "true" then%>
	<% For each Key in custom_fields_dict%><input TYPE="hidden" NAME="<%=Key%>" Value="<%=Key%>"><%=vblf%><%next%>
<%end if%>
<table border="0" cellspacing="0" cellpadding="0" width="700">
<%
if Request("TB")="" then    ' WJC true first time in
    if not isEdit then      ' WJC true if not editing
        NumCopies = 1
        if LocationID = "1" then    ' WJC 1 is always 'On Order'
            DateOrdered  = split(Now()," ",2)(0)    ' WJC today's date
            DateReceived = ""
        end if
    end if
end if
Select Case sTab
	Case "Required"
%>
	<tr height="25">
		<td align="right" valign="top" nowrap width="150">
			<span class="required">Location ID:</span>
		</td>
		<td colspan="3">
            <%  if Application("ENABLE_OWNERSHIP")="TRUE" then
                authorityFunction = "SetOwnerInfo('location');"
            else
                authorityFunction= ""
            end if %>
			&nbsp;<%'=GetBarcodeIcon()%>&nbsp;<%'ShowLocationPicker3 "document.form1", "iLocationID", "lpLocationBarCode", "lpLocationName", 10, 49, false,"validateLocation(document.form1.iLocationID.value)"%>
            &nbsp;<%=GetBarcodeIcon()%>&nbsp;<%ShowLocationPicker7 "document.form1", "iLocationID", "lpLocationBarCode", "lpLocationName", 10, 49, false, authorityFunction, LocationID %>
		</td>
	</tr>
	<tr height="25">
		<td align="right" valign="top" nowrap width="150">
			<span class="required">Container Name:</span>
		</td>
		<td>
			<input type="text" name="iContainerName" size="30" value="<%=ContainerName%>">
		</td>
		<%= ShowPickList("Unit of Weight:", "iUOWID", Session("UOWIDOptionValue"),"SELECT UNIT_ID||'='||Unit_Abreviation AS Value, Unit_Name AS DisplayText FROM inv_units WHERE Unit_type_id_FK IN (2) ORDER BY lower(DisplayText) ASC")%>
	</tr>
	<tr height="25">
	<%=ShowPickList("<SPAN class=required>Container Type:</span>", "iContainerTypeID", ContainerTypeID,"SELECT Container_Type_ID AS Value, Container_Type_Name AS DisplayText FROM inv_Container_Types ORDER BY lower(DisplayText) ASC")%>
		
		<%=ShowInputBox("Total weight:", "FinalWeight", 15, "", False, false)%>	

	</tr>
	<tr height="25">
		<%'ShowPickList("<SPAN class=required>Unit of measure:</span>", "iUOMID", Session("UOMIDOptionValue"),"SELECT UNIT_ID||'='||Unit_Abreviation AS Value, Unit_Name AS DisplayText FROM inv_units WHERE Unit_type_id_FK IN (1,2) ORDER BY lower(DisplayText) ASC")%>
		<td align="right" valign="top">
			<span title="Pick an option from the list" class="required">Unit of Measure:</span>
		</td>
		<td>
		<%=ShowSelectBox2("iUOMID", Session("UOMIDOptionValue"),"SELECT UNIT_ID||'='||Unit_Abreviation AS Value, Unit_Name AS DisplayText FROM inv_units WHERE Unit_type_id_FK IN (1,2,4) ORDER BY lower(DisplayText) ASC", 20, "", "")%>
		</td>
		<%=ShowInputBox("Tare Weight:", "TareWeight", 15, "", False, false)%>
	</tr>
	<tr height="25">
		<%=ShowInputBox("Container Size:", "QtyMax", 15, "", False, true)%>
		<%=ShowInputBox("Net Weight:", "NetWeight", 15, "", False, false)%>
	</tr>
	<% if not IsEdit then
	'QtyInitial = QtyMax
	%>
	<tr height="25">
		<%=ShowInputBox("Initial Amount:", "QtyInitial", 15, "", False, True)%>
		<% if Application("ENABLE_OWNERSHIP")="TRUE" then %>
		<td rowspan=3 align=right> <span title="Pick an option from the list" class="required">Container Admin:</span></td>
		<td rowspan=3 align=left>
		<table border=.5>
		<tr><td><input type="Radio" name="Ownership" name="User_cb" id="User_cb" onclick="SetOwnerInfo('user');"/>by user</td>
		<td><input type="Radio" name="Ownership" name="Group_cb" id="Group_cb" onclick="SetOwnerInfo('group');" />by Group</td></tr>
		<tr><td colspan="2"><SELECT id="Ownershiplst" onchange="setPrincipalID(this);" ><OPTION></OPTION></SELECT></td></tr></table></td>
		<% end if %>
	</tr>
	<tr height="25">
		<%=ShowInputBox("Number of Copies:", "NumCopies", 5, "", False, True)%>
	</tr>
	<%else%>
		<%=ShowInputBox("Quantity Remaining:", "QtyRemaining", 5, "", False, true)%>
		<% if Application("ENABLE_OWNERSHIP")="TRUE" then %>
		<td rowspan=3 align=right> <span title="Pick an option from the list" class="required">Container Admin:</span></td>
		<td rowspan=3 align=left>
		<table border=.5>
		<tr><td><input type="Radio" name="Ownership" name="User_cb" id="User_cb" onclick="getList(OwnerShipUserList.value,OwnershipType.value);"/>by user</td>
		<td><input type="Radio" name="Ownership" name="Group_cb" id="Group_cb" onclick="getList(OwnerShipGroupList.value,OwnershipType.value);" />by Group</td></tr>
		<tr><td colspan="2"><SELECT id="Ownershiplst" name="Ownershiplst" onchange="setPrincipalID(this)"><OPTION></OPTION></SELECT></td></tr></table></td>
		<% end if %>
	<%End if%>

	<tr height="25">
		<%= ShowPickList("Container Status:", "iContainerStatusID", ContainerStatusID,"SELECT Container_Status_ID AS Value, Container_Status_Name AS DisplayText FROM inv_Container_Status ORDER BY lower(DisplayText) ASC")%>
		<td></td><td></td>
	</tr>
	<tr height="25">
		<td align="right" valign="top">
			<span title="Pick an option from the list">Supplier Name:</span>
		</td>
		<td>
			<%= ShowSelectBox2("iSupplierID", SupplierID, "SELECT Supplier_ID AS Value, Supplier_Short_Name AS DisplayText FROM inv_suppliers ORDER BY lower(Supplier_Short_Name) ASC", 27, RepeatString(43, "&nbsp;"), "")%>
		</td>
		<% if  Application("ENABLE_OWNERSHIP")="TRUE" and not IsEdit then
	'Set LocationType
	%>
		<td align="right"><span class="">Location Type:<span></td>
		<td><%=ShowSelectBox3("iLocationTypeID", LocationTypeID,"SELECT Location_Type_ID AS Value, Location_Type_Name AS DisplayText FROM inv_Location_Types ORDER BY Location_Type_Name ASC", 0, "None", "0","")%></td>
	<%End if%>
	</tr>
	<tr height="25">
		<%=ShowInputBox("Catalog Number:", "SupplierCatNum", 15, "", False, False)%>
		<% if  Application("ENABLE_OWNERSHIP")="TRUE" and IsEdit then
	'Set LocationType
	%>
		<td align="right"><span class="">Location Type:<span></td>
		<td><%=ShowSelectBox3("iLocationTypeID", LocationTypeID,"SELECT Location_Type_ID AS Value, Location_Type_Name AS DisplayText FROM inv_Location_Types ORDER BY Location_Type_Name ASC", 0, "None", "0","")%></td>
	<%End if%>
	</tr>
	<tr height="25">
		<%=ShowInputBox("Lot Number:", "LotNum", 15, "", False, False)%>
		<td></td>
		<%if lcase(PageSource) <>"eln"  then%>
		<td>&nbsp;
			<%if Session("INV_MANAGE_SUBSTANCES" & "cheminv") then%>
			<a class="MenuLink" href="Create%20a%20new%20inventory%20substance%20and%20assign%20it%20to%20this%20container" onclick="OpenDialog('/cheminv/gui/CreateOrEditSubstance.asp', 'SubsManager', 2); return false;" title="Create a new inventory substance and assign it to this container">New Substance</a>
			&nbsp;|&nbsp;
			<%end if%>
			<%
			substanceFG = iif((Session("isCDP") = "TRUE"),"global_substanceselect_form_group","global_substanceselect_np_form_group")
			%>
			<a class="MenuLink" HREF="Select%20an%20existing%20substance%20and%20assign%20it%20to%20this%20container" target="SubstancesFormGroup" onclick="OpenDialog('/cheminv/inputtoggle.asp?formgroup=<%=substanceFG%>&amp;dataaction=db&amp;dbname=cheminv&amp;showbanner=false', 'SubsManager', 2); return false;" title="Select an existing substance and assign it to this container">Select Substance</a>
		</td>
        <%end if %>
	</tr>
	<%if Application("RegServerName") <> "NULL" then%>
	<tr height="25">
		<td align="right" valign="top" nowrap width="150">Expiration Date:</td>
		<td><%call ShowInputField("", "", "iExpDate:form1:" & ExpDate , "DATE_PICKER:TEXT", "15")%>
		<!--<input type="text" name="iExpDate" size="15" value="<%=ExpDate%>"><a href onclick="return PopUpDate(&quot;iExpDate&quot;,&quot;/cheminv/gui/month_view1.asp&quot;)"><img src="/cfserverasp/source/graphics/navbuttons/icon_mview.gif" height="16" width="16" border="0"></a>-->
		</td>
		<td width="200" align="right">Registry Batch ID:</td><td><input type="text" Size="15" name="RegBatchID" value="<%=RegBatchID%>" onchange="GetRegIDFromRegNum(this.value); iCompoundID.value=''; CompoundID.value='';"><input type="hidden" name="iRegID" value="<%=RegID%>"><input type="hidden" name="iBatchNumber" value="<%=BatchNumber%>"></td>
		<input type="hidden" name="NewRegID">
		<input type="hidden" name="NewBatchNumber">
	</tr>
	<%
	else
        '--even without reg integration these form elements are expected for substance selection
    %>
	    <input type="hidden" name="iRegID" value="<%=RegID%>"><input type="hidden" name="iBatchNumber" value="<%=BatchNumber%>">
	<%End if%>
	<tr height="25">
		<td align="right" valign="top" nowrap width="150">Date Ordered:</td>
		<td><%call ShowInputField("", "", "iDateOrdered:form1:" & DateOrdered , "DATE_PICKER:TEXT", "15")%>
		<!--<input type="text" name="iDateOrdered" size="15" value="<%=DateOrdered%>"><a href onclick="return PopUpDate(&quot;iDateOrdered&quot;,&quot;/cheminv/gui/month_view1.asp&quot;)"><img src="/cfserverasp/source/graphics/navbuttons/icon_mview.gif" height="16" width="16" border="0"></a>-->
		</td>
		<%if Application("RegServerName") <> "NULL" then%>
			<td width="200" align="right">Compound ID:</td><td><input type="text" Size="15" name="iCompoundID" value="<%=CompoundID%>" onchange="ValidateCompoundID2(this, true);"></td>
		<%else%>
			<td width="200" align="right">Compound ID:</td><td><input type="text" Size="15" name="iCompoundID" value="<%=CompoundID%>" onchange="ValidateCompoundID2(this, false);"></td>
		<%End if%>
		<input Type="hidden" name="NewCompoundID">
	</tr>
	<tr height="25">
		<td align="right" valign="top" nowrap width="150">Date Received:</td>
		<td><%call ShowInputField("", "", "iDateReceived:form1:" & DateReceived , "DATE_PICKER:TEXT", "15")%>
		<!--<input type="text" name="iDateReceived" size="15" value="<%=DateReceived%>"><a href onclick="return PopUpDate(&quot;iDateReceived&quot;,&quot;/cheminv/gui/month_view1.asp&quot;)"><img src="/cfserverasp/source/graphics/navbuttons/icon_mview.gif" height="16" width="16" border="0"></a>-->
		</td>
		<%=ShowInputBox("Grade:", "Grade", 15, "", False, False)%>
	</tr>
	<tr height="25">
		<td align="right" valign="top" nowrap width="150">Container Cost:</td>
		<td>
			<%=ShowSelectBox("iUOCostID", Session("UOCostIDOptionValue"),"SELECT UNIT_ID||'='||Unit_Abreviation AS Value, Unit_Abreviation AS DisplayText FROM inv_units WHERE Unit_type_id_FK = 5 ORDER BY Unit_Name ASC")%>
			<input type="text" name="iContainerCost" size="15" value="<%=ContainerCost%>">
		</td>
		<%=ShowInputBox("Purity:", "Purity", 15, ShowSelectBox("iUOPID", Session("UOPIDOptionValue"),"SELECT UNIT_ID||'='||Unit_Abreviation AS Value, Unit_Name AS DisplayText FROM inv_units WHERE Unit_type_id_FK = 3 ORDER BY Unit_Name ASC"), False, False)%>
	</tr>
	<tr height="25">
		<%=ShowInputBox("PO Number:", "PONumber", 25, "", False, False)%>
		<%=ShowInputBox("Concentration:", "Concentration", 15, ShowSelectBox("iUOCID", Session("UOCIDOptionValue"),"SELECT UNIT_ID||'='||Unit_Abreviation AS Value, Unit_Name AS DisplayText FROM inv_units WHERE Unit_type_id_FK in (3,6) ORDER BY Unit_Name ASC"), False, False)%>
	</tr>
	<tr height="25">
		<%=ShowInputBox("PO Line Number:", "POLineNumber", 10, "", False, False)%>
		<%=ShowInputBox("Density:", "Density", 15, ShowSelectBox("iUODID", Session("UODIDOptionValue"),"SELECT UNIT_ID||'='||Unit_Abreviation AS Value, Unit_Name AS DisplayText FROM inv_units WHERE Unit_type_id_FK = 6 ORDER BY Unit_Name ASC"), False, False)%>
	</tr>
	<% if not isEdit then%>
	<tr height="25">
		<%if lcase(Application("RequireBarcode")) = "true" then%>
			<td align="right" valign="top" nowrap width="150"><span id="sp0" style="display:block;"><span class="required">Container ID:</span></span><span id="sp1" style="display:block;"><span class="required">Barcode Description:</span></span></td>
		<%else%>
			<td align="right" valign="top" nowrap width="150"><span id="sp0" style="display:block;">Container ID:</span><span id="sp1" style="display:block;">Barcode Description:</span></td>
		<%end if%>
		<td>
			<script language="JavaScript">
			function AutoGen_OnClick(element) {
				document.form1.iBarcode.value='';
				document.form1.AutoGen.value = element.checked.toString();
				element.checked ? document.all.sp0.style.display = 'none' :document.all.sp0.style.display = 'block';
				element.checked ? document.all.sp1.style.display = 'block':document.all.sp1.style.display = 'none';
				document.all.sp2.style.display = document.all.sp0.style.display;
				document.all.sp3.style.display = document.all.sp1.style.display;
				//update the bUseBarcodeDesc value
				element.checked ? document.all.bUseBarcodeDesc.value = 'true' : document.all.bUseBarcodeDesc.value = 'false';
			}
			</script>
			<input type="hidden" name="bUseBarcodeDesc" value>
			<span id="sp2" style="display:block;">
				&nbsp;<%=GetBarcodeIcon()%>&nbsp;<input type="text" name="iBarcode" size="15" value="<%=Barcode%>"><br>
			</span>
			<span id="sp3" style="display:block">
				<%=ShowSelectBox2("iBarcodeDescID", BarcodeDescID,"SELECT barcode_desc_id AS Value, barcode_desc_name AS Displaytext FROM inv_barcode_desc ORDER BY lower(DisplayText) ASC", null," ","")%>
			</span>
			<!-- //SM Fix for CBSR-65197  -->
			<input type="checkbox" name="AutoGen_cb" onclick="AutoGen_OnClick(this);">Autogenerate Container ID
			<script language="Javascript">if (document.form1.AutoGen.value == "true") document.form1.AutoGen_cb.click();</script>
		</td>
	</tr>

<!--	<tr height="25">		<td align="right" valign="top" nowrap width="150"><span id="sp0" style="visibility:visible">Container ID:</span></td><td><span id="sp2" style="visibility:visible">&nbsp;<%=GetBarcodeIcon()%>&nbsp;<input type="text" name="iBarcode" size="15" value="<%=Barcode%>"><br></span><span id="sp1" style="visibility:visible"><input type="checkbox" name="AutoGen_cb" onclick="document.form1.iBarcode.value=''; document.form1.AutoGen.value = this.checked; this.checked ? document.all.sp0.style.visibility = 'hidden' :document.all.sp0.style.visibility = 'visible' ; document.all.sp2.style.visibility = document.all.sp0.style.visibility;">Autogenerate barcode ID</span></td>		<script language="Javascript">if (document.form1.AutoGen.value == "true") document.form1.AutoGen_cb.click(); </script>	</tr>-->
	<tr>
		<td>&nbsp;</td><td><input type="checkbox" name="AutoPrint_cb" onclick="document.form1.AutoPrint.value = this.checked;">Print barcodes immediately<br></td>
		<script language="Javascript">if (document.form1.AutoPrint.value == "true") document.form1.AutoPrint_cb.click(); </script>
	</tr>
	<tr>
		<td>&nbsp;</td><td><input type="checkbox" name="returnToSearch_cb" onclick="document.form1.returnToSearch.value = this.checked;">New search when done<br></td>
		<script language="Javascript">if (document.form1.returnToSearch.value == "true") document.form1.returnToSearch_cb.click(); </script>

	<%Else%>
	<tr height="25">
		<td align="right" valign="top" nowrap width="150">Container ID:</td><td>&nbsp;<%=GetBarcodeIcon()%>&nbsp;<input type="text" name="iBarcode" size="15" value="<%=Barcode%>"></td>
	<%End if%>
	<td></td><td align="right"><br />
                <% if lcase(PageSource) <> "eln" then %>
				    <a href="#" onclick="if(typeof(parent.CloseModal) == 'function'){parent.CloseModal(false);} if (DialogWindow) {DialogWindow.close()}; window.close();return false"><img SRC="/cheminv/graphics/sq_btn/cancel_dialog_btn.gif" border="0" alt="Cancel"></a>
                <%End if
                if NOT isEdit then%>
					<a HREF="#" onclick="ValidateContainer('Create'); return false;"><img SRC="/cheminv/graphics/sq_btn/ok_dialog_btn.gif" border="0"></a>
				<%Else%>
					<a HREF="#" onclick="ValidateContainer('Edit'); return false;"><img SRC="/cheminv/graphics/sq_btn/ok_dialog_btn.gif" border="0"></a>
				<%End if%>
		</td>
	</tr>

<%
Case "Substance"
	Session("IsPopUP") = false
%>
	<input type="hidden" name="isSubstanceTab">
	<tr>
		<td>
<%
	if NOT IsEmpty(CompoundID) then
		GetSubstanceAttributesFromDb(CompoundID)
		hdrText = ""
		bConflicts = false
		if ConflictingFields <> "" then
			hdrText = "<font color=red>Warning: Duplicate Substance</font>"
			bConflicts = true
		End if
		DisplaySubstance "", hdrText, false, false, false, false, false, inLineMarker & dBStructure
		'if Session("isCDP") = "TRUE" then
		'	specifier = 185
		'else
		'	specifier = "185:gif"
		'end if
		'Base64DecodeDirect "cheminv", "base_form_group", dBStructure, "Structures.BASE64_CDX", CompoundID, CompoundID, specifier, 130

	End if
%>


		</td>
	</tr>
	<tr>
		<%if lcase(PageSource) <> "eln"  then%>
		<td colspan="4" align="right">
			<%if Session("INV_MANAGE_SUBSTANCES" & "cheminv") then%>
			<a class="MenuLink" href="Create%20a%20new%20inventory%20substance%20and%20assign%20it%20to%20this%20container" onclick="OpenDialog('/cheminv/gui/CreateOrEditSubstance.asp', 'newDialog', 2); return false;" title="Create a new inventory substance and assign it to this container">New Substance</a>
			&nbsp;|&nbsp;
			<%end if%>
			<a class="MenuLink" HREF="Select%20an%20existing%20substance%20and%20assign%20it%20to%20this%20container" target="SubstancesFormGroup" onclick="OpenDialog('/cheminv/inputtoggle.asp?formgroup=global_substanceselect_form_group&amp;dataaction=db&amp;dbname=cheminv&amp;showbanner=false', 'newDialog', 2); return false;" title="Select an existing substance and assign it to this container">Select Substance</a>
		</td>
		<%end if %>
	</tr>
<%
Case "RegSubstance"
	Session("IsPopUP") = false
%>
	<input type="hidden" name="isSubstanceTab">
	<tr height="150">
		<td>
			<table border="1">
				<tr>
					<!-- Structure -->
					<td>
						<%
						if Session("isCDP") = "TRUE" then
							specifier = 185
						else
							specifier = "185:gif"
						end if
						Base64DecodeDirect "invreg", "base_form_group", BASE64_CDX, "Structures.BASE64_CDX", RegID, RegID, specifier, 130%>
					</td>
				</tr>
			</table>
		</td>
		<td valign="top">
			<table border="0" cellpadding="1" cellspacing="2">
				<tr><!-- Header Row -->
					<td colspan="4" align="center">
						&nbsp;<em><b><%=TruncateInSpan(RegName, 50, "")%></b></em>
					</td>
				</tr>
				<tr>
					<%=ShowField("Molecular Weight:", "MOLWEIGHT0", 15, "MOLWEIGHT0")%>
					<%=ShowField("Molecular Formula:", "FORMULA0", 15, "FORMULA0")%>
				</tr>
				<!--				<tr>					<%=ShowField("RegBatchID:", "RegBatchID", 15, "")%>					<input type="hidden" name="iRegID" value="<%=RegID%>">					<input type="hidden" name="iBatchNumber" value="<%=BatchNumber%>">					<%=ShowField("RegBatchAmount:", "RegAmount", 15, "")%>				</tr>				<tr>					<%=ShowField("NoteBook:", "NoteBook", 15, "")%>					<%=ShowField("Page:", "Page", 15, "")%>				</tr>				<tr>					<%=ShowField("Chemist:", "RegScientist", 15, "")%>					<%=ShowField("Purity:", "Purity", 15, "")%>				</tr>				-->
				<%
				k = 0
				for each key in reg_fields_dict
					if key <> "BASE64_CDX" and key <> "REGNAME" then
						if k = 0 then
							Response.Write("<tr>" & vbcrlf)
						end if
						Response.Write(ShowField(reg_fields_dict.item(key) & ":", key, 15, "") & vbcrlf)
						if key = "REGID" then
							Response.Write("<input type=""hidden"" name=""iRegID"" value=""" & RegID & """>" & vbcrlf)
						end if
						if key = "BATCHNUMBER" then
							Response.Write("<input type=""hidden"" name=""iBatchNumber"" value=""" & BatchNumber & """>" & vbcrlf)
						end if
						if k = cInt(1) then
							Response.write("</tr>")
							k = 0
						else
							k = k + 1
						end if
					end if
				next
				%>
				<tr>
                     <%if lcase(PageSource) <>"eln"  then%>
					<td colspan="4" align="right">
						<%if Session("INV_MANAGE_SUBSTANCES" & "cheminv") then%>
						<a class="MenuLink" href="Create%20a%20new%20inventory%20substance%20and%20assign%20it%20to%20this%20container" onclick="OpenDialog('/cheminv/gui/CreateOrEditSubstance.asp', 'newDialog', 2); return false;" title="Create a new inventory substance and assign it to this container">New Substance</a>
						&nbsp;|&nbsp;
						<%end if%>
						<a class="MenuLink" HREF="Select%20an%20existing%20substance%20and%20assign%20it%20to%20this%20container" target="SubstancesFormGroup" onclick="OpenDialog('/cheminv/inputtoggle.asp?formgroup=global_substanceselect_form_group&amp;dataaction=db&amp;dbname=cheminv&amp;showbanner=false', 'newDialog', 2); return false;" title="Select an existing substance and assign it to this container">Select Substance</a>
					</td>
                    <%end if %>
				</tr>
			</table>
		</td>
	</tr>
<%
	Case "Supplier"
%>
	<tr height="25">
		<td align="right">
			<span title="Pick an option from the list">Supplier Name:</span>
		</td>
		<td colspan="3">
			<%= ShowSelectBox2("iSupplierID", SupplierID, "SELECT Supplier_ID AS Value, Supplier_Short_Name AS DisplayText FROM inv_suppliers ORDER BY lower(Supplier_Short_Name) ASC", 27, RepeatString(43, "&nbsp;"), "")%>
		</td>
		<%' ShowPickList("Supplier Name:", "iSupplierID", SupplierID,"SELECT Supplier_ID AS Value, Supplier_Name AS DisplayText FROM inv_suppliers ORDER BY lower(DisplayText) ASC")%>
	</tr>
	<tr height="25">
		<%=ShowInputBox("Catalog Number:", "SupplierCatNum", 15, "", False, False)%>

	</tr>
	<tr height="25">
		<%=ShowInputBox("Lot Number:", "LotNum", 15, "", False, False)%>

	</tr>
	<tr height="25">
		<td align="right" valign="top" nowrap width="150">Date Produced:</td>
		<td><%call ShowInputField("", "", "iDateProduced:form1:" & DateProduced , "DATE_PICKER:TEXT", "15")%>
		<!--<input type="text" name="iDateProduced" size="15" value="<%=DateProduced%>"><a href onclick="return PopUpDate(&quot;iDateProduced&quot;,&quot;/cheminv/gui/month_view1.asp&quot;)"><img src="/cfserverasp/source/graphics/navbuttons/icon_mview.gif" height="16" width="16" border="0"></a>-->
		</td>
	</tr>
	<tr height="25">
		<td align="right" valign="top" nowrap width="150">Date Ordered:</td>
		<td><%call ShowInputField("", "", "iDateOrdered:form1:" & DateOrdered , "DATE_PICKER:TEXT", "15")%>
		<!--<input type="text" name="iDateOrdered" size="15" value="<%=DateOrdered%>"><a href onclick="return PopUpDate(&quot;iDateOrdered&quot;,&quot;/cheminv/gui/month_view1.asp&quot;)"><img src="/cfserverasp/source/graphics/navbuttons/icon_mview.gif" height="16" width="16" border="0"></a>-->
		</td>
		<%=ShowInputBox("PO Number:", "PONumber", 25, "", False, False)%>
	</tr>
	<tr height="25">
		<td align="right" valign="top" nowrap width="150">Date Received:</td>
		<td><%call ShowInputField("", "", "iDateReceived:form1:" & DateReceived , "DATE_PICKER:TEXT", "15")%>
		<!--<input type="text" name="iDateReceived" size="15" value="<%=DateReceived%>"><a href onclick="return PopUpDate(&quot;iDateReceived&quot;,&quot;/cheminv/gui/month_view1.asp&quot;)"><img src="/cfserverasp/source/graphics/navbuttons/icon_mview.gif" height="16" width="16" border="0"></a>-->
		</td>
		<%=ShowInputBox("PO Line Number:", "POLineNumber", 25, "", False, False)%>
	</tr>
	<tr height="25">
		<%=ShowInputBox("Container Cost($):", "ContainerCost", 15, "", False, False)%>
		<%=ShowInputBox("Requisition Number:", "ReqNumber", 25, "", False, False)%>
	</tr>
<%
	Case "Contents"
%>
	<tr height="25">
		<%=ShowInputBox("Purity:", "Purity", 15, ShowSelectBox("iUOPID", Session("UOPIDOptionValue"),"SELECT UNIT_ID||'='||Unit_Abreviation AS Value, Unit_Name AS DisplayText FROM inv_units WHERE Unit_type_id_FK = 3 ORDER BY Unit_Name ASC"), False, False)%>
	</tr>
	<tr height="25">
		<%=ShowInputBox("Concentration:", "Concentration", 15, ShowSelectBox("iUOCID", Session("UOCIDOptionValue"),"SELECT UNIT_ID||'='||Unit_Abreviation AS Value, Unit_Name AS DisplayText FROM inv_units WHERE Unit_type_id_FK in (3,6) ORDER BY Unit_Name ASC"), False, False)%>
	</tr>
	<tr height="25">
		<%=ShowInputBox("Density:", "Density", 15, ShowSelectBox("iUODID", Session("UODIDOptionValue"),"SELECT UNIT_ID||'='||Unit_Abreviation AS Value, Unit_Name AS DisplayText FROM inv_units WHERE Unit_type_id_FK = 6 ORDER BY Unit_Name ASC"), False, False)%>
	</tr>
	<tr height="25">
		<%=ShowInputBox("Grade:", "Grade", 15, "", False, False)%>
	</tr>
	<tr height="25">
		<td align="right" valign="top" nowrap width="150">Solvent:</td>
		<td><%=ShowSelectBox2("iSolventIDFK" & i,SolventIDFK,"SELECT solvent_id AS Value, solvent_name AS DisplayText FROM " & Application("CHEMINV_USERNAME") & ".inv_solvents ORDER BY lower(DisplayText) ASC", null, "Select a Solvent", "null")%></td>
	</tr>
	<%if IsEdit then%>
	<tr height="25">
		<td align="right" valign="top" nowrap width="150">Date Certified:</td>
		<td><%call ShowInputField("", "", "iDateCertified:form1:" & DateCertified , "DATE_PICKER:TEXT", "15")%>
		<!--<input type="text" name="iExpDate" size="15" value="<%=ExpDate%>"><a href onclick="return PopUpDate(&quot;iExpDate&quot;,&quot;/cheminv/gui/month_view1.asp&quot;)"><img src="/cfserverasp/source/graphics/navbuttons/icon_mview.gif" height="16" width="16" border="0"></a>-->
		</td>
	</tr>
	<tr height="25">
		<td align="right" valign="top" nowrap width="150">Date Approved:</td>
		<td><%call ShowInputField("", "", "iDateApproved:form1:" & DateApproved , "DATE_PICKER:TEXT", "15")%>
		<!--<input type="text" name="iExpDate" size="15" value="<%=ExpDate%>"><a href onclick="return PopUpDate(&quot;iExpDate&quot;,&quot;/cheminv/gui/month_view1.asp&quot;)"><img src="/cfserverasp/source/graphics/navbuttons/icon_mview.gif" height="16" width="16" border="0"></a>-->
		</td>
	</tr>
	<%end if%>

	<tr height="25">
		<td align="right" valign="top" nowrap width="150">Expiration Date:</td>
		<td>
		<%call ShowInputField("", "", "iExpDate:form1:" & ExpDate , "DATE_PICKER:TEXT", "15")%>
		<!--<input type="text" name="iExpDate" size="15" value="<%=ExpDate%>"><a href onclick="return PopUpDate(&quot;iExpDate&quot;,&quot;/cheminv/gui/month_view1.asp&quot;)"><img src="/cfserverasp/source/graphics/navbuttons/icon_mview.gif" height="16" width="16" border="0"></a>-->
		</td>
	</tr>
	<tr height="25">
		<td colspan="2">
			&nbsp;
		</td>
	</tr>
<%
	Case "Optional"
%>
	<tr height="25">
		<%= ShowPickList("Container Status:", "iContainerStatusID", ContainerStatusID,"SELECT Container_Status_ID AS Value, Container_Status_Name AS DisplayText FROM inv_Container_Status ORDER BY lower(DisplayText) ASC")%>
	</tr>
	<tr height="25">
		<%=ShowInputBox("Description:", "ContainerDesc", 15, "", False, False)%>
	</tr>
	<tr height="25">
		<%=ShowInputBox("Min stock thresh" & "&nbsp;(" & UOMAbv & "):", "MinStockQty", 15, "", False, False)%>
	</tr>
	<tr height="25">
		<%=ShowInputBox("Max stock thresh" & "&nbsp;(" & UOMAbv & "):", "MaxStockQty", 15, "", False, False)%>
	</tr>
	<tr height="25">
		<td colspan="2">
			&nbsp;
		</td>
	</tr>
	<tr height="25">
		<td colspan="2">
			&nbsp;
		</td>
	</tr>
	<tr height="25">
		<td colspan="2">
			&nbsp;
		</td>
	</tr>
	<tr height="25">
		<td colspan="2">
			&nbsp;
		</td>
	</tr>
<%
	Case "Owner"
%>
	<tr height="25">
		<td align="right" valign="top">
			<span title="Pick an option from the list">Owner:</span>
		</td>
		<td>
			<%=ShowSelectBox2("iOwnerID", OwnerID, "SELECT owner_ID AS Value, description AS DisplayText FROM " & Application("CHEMINV_USERNAME") & ".inv_owners ORDER BY lower(description) ASC", 27, RepeatString(43, "&nbsp;"), "")%>
		</td>
	</tr>
	<tr height="25">
		<td align="right" valign="top">
			<span title="Pick an option from the list">Current user:</span>
		</td>
		<td>
			<%=ShowSelectBox2("iCurrentUserID", UCASE(CurrentUserID), "SELECT Upper(Last_Name||' '||First_Name) AS Value, Last_Name||DECODE(First_Name, NULL, '', ', '||First_Name) AS DisplayText FROM CS_SECURITY.People ORDER BY lower(Last_Name) ASC", 27, RepeatString(43, "&nbsp;"), "")%>
		</td>
	</tr>
	<tr height="25">
		<td colspan="2">
			&nbsp;
		</td>
	</tr>
	<tr height="25">
		<td colspan="2">
			&nbsp;
		</td>
	</tr>
	<tr height="25">
		<td colspan="2">
			&nbsp;
		</td>
	</tr>
	<tr height="25">
		<td colspan="2">
			&nbsp;
		</td>
	</tr>
<%
	Case "Comments"
%>
	<tr height="150">
		<td align="right" valign="top" nowrap>
			Comments:
		</td>
		<td valign="top">
			<textarea rows="8" cols="60" name="iComments" wrap="hard"><%=Comments%></textarea>
		</td>
	</tr>
	<tr height="150">
		<td align="right" valign="top" nowrap>
			Storage Conditions:
		</td>
		<td valign="top">
			<textarea rows="8" cols="60" name="iStorageConditions" wrap="hard"><%=StorageConditions%></textarea>
		</td>
	</tr>
	<tr height="150">
		<td align="right" valign="top" nowrap>
			Handling Procedures:
		</td>
		<td valign="top">
			<textarea rows="8" cols="60" name="iHandlingProcedures" wrap="hard"><%=HandlingProcedures%></textarea>
		</td>
	</tr>
<%
	Case Application("OtherTabText")
	j=1
	For each key in custom_fields_dict
		if (j Mod 2) = 1 then 	Response.Write "<tr height=""25"">" & vblf
		if inStr(uCase(Key), "DATE_") then
			Response.Write "<td align=""right"" valign=""top"" nowrap width=""150"">"
			if req_custom_fields_dict.Exists(Key) then Response.Write "<span class=""required"">"
			Response.Write custom_fields_dict.Item(key)
			if req_custom_fields_dict.Exists(Key) then Response.Write "</span>"
			Response.Write ":</td>"
			Response.Write "<td>"
			call ShowInputField("", "", "i" & Key & ":form1:" & eval(Key) , "DATE_PICKER:TEXT", "15")
			Response.Write "</td>"

			'str = ShowInputBox(custom_fields_dict.Item(key), Key, 25, "", False, req_custom_fields_dict.Exists(Key))
			'Response.write Left(str,len(str)-5)
			'Response.Write "<a href onclick=""return PopUpDate(&quot;i" & Key & "&quot;,&quot;/cheminv/gui/month_view1.asp&quot;)""><img src=""/cfserverasp/source/graphics/navbuttons/icon_mview.gif"" height=""16"" width=""16"" border=""0""></a>"
			'Response.Write "</td>" & vblf
		Else
			if custom_lists_dict.Exists("CUSTOM_FIELDS." & key) then
				Response.Write "<td align=right valign=top nowrap width=""150"">" 
				if req_custom_fields_dict.Exists(Key) then
					Response.Write "<span class=""required"">" & custom_fields_dict.Item(key) & ":</span></td>"
				else
					Response.Write custom_fields_dict.Item(key) & ":</td>"
				end if
				
				Response.Write "<td><select name=""i" & key & """>"

				'-- build the select from the custom list
				Response.Write GetCustomListOptions("CUSTOM_FIELDS", key, eval(key), null)
				
				Response.Write "</td>"
			else
			Response.write ShowInputBox(custom_fields_dict.Item(key) & ":", Key, 15, "", False, req_custom_fields_dict.Exists(Key)) & vblf
			end if
		end if
		if (j Mod 2) = 0 then Response.Write "</TR>"
		j = j + 1
	Next
	'take care of odd number of custom fields
	if (j Mod 2) = 0 then Response.Write "<td></td></TR>"

End Select
%>
	<% if sTab <> "Required" then%>
	<tr>
		<td colspan="4" align="right" height="20" valign="bottom">
                <%if lcase(PageSource) <> "eln" then %>
				<a HREF="#" onclick="if(typeof(parent.CloseModal) == 'function'){parent.CloseModal(false);} if (DialogWindow) {DialogWindow.close()}; window.close();return false"><img SRC="/cheminv/graphics/sq_btn/cancel_dialog_btn.gif" border="0" alt="Cancel"></a>
                <%end if 
                if NOT isEdit then%>
					<a HREF="#" onclick="ValidateContainer('Create'); return false;"><img SRC="/cheminv/graphics/sq_btn/ok_dialog_btn.gif" border="0"></a>
				<%Else%>
					<a HREF="#" onclick="ValidateContainer('Edit'); return false;"><img SRC="/cheminv/graphics/sq_btn/ok_dialog_btn.gif" border="0"></a>
				<%End if%>
		</td>
	</tr>
	<%end if%>
</table>
<%if lcase(PageSource) = "eln" and sTab = "Required" Then
    if Session("RegID") <>"" and Session("BatchNumber")<>"" then%>
        <script language="javascript"">
        if (document.form1.NewRegID) 
            document.form1.NewRegID.value = <%=Session("RegID")%>;
        if(document.form1.NewBatchNumber)
            document.form1.NewBatchNumber.value = <%=Session("BatchNumber")%>;
        if(document.form1.NewCompoundID)
            document.form1.NewCompoundID.value = '';
        </script>      
    <%elseif Session("CompoundID") <> "" then%>
        <script language="javascript"">
        if (document.form1.NewRegID) 
            document.form1.NewRegID.value = '';
        if (document.form1.NewBatchNumber)
            document.form1.NewBatchNumber.value = '';
        if(document.form1.NewCompoundID)
            document.form1.NewCompoundID.value =  <%=Session("CompoundID")%>;
        </script>
    <%end if
End IF
%>
</form>
<!--
<%if AssignToRack = "on" then %>
	<script language="javascript">
	if (document.form1.AssignToRack){
		document.form1.AssignToRack.checked = true;
		<% if toggleDefaults = "true" then %>
			<% if ContainerID = "" then %>
			toggleRackDisplay();
			<% end if %>
		<% end if %>
	}
	</script>
<% end if %>
-->

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
<% if Application("ENABLE_OWNERSHIP")="TRUE" then %>
<script language="javascript">
    //set the inital location group
   <% if isCopy then %>
     SetOwnerInfo('container');
   <%elseif not isEdit then %>
    SetOwnerInfo('location');
   <%end if %>
 </script>
<%end if %>
</body>
</html>
