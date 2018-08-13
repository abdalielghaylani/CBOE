<%
'Prevent page from being cached
Response.ExpiresAbsolute = Now()

%>
<!--#INCLUDE VIRTUAL = "/cheminv/gui/guiUtils.asp"-->
<!--#INCLUDE VIRTUAL = "/cheminv/gui/GetOrderContainerAttributes.asp"-->
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/display_func_vbs.asp"-->
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/server_const_vbs.asp"-->
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/search_func_vbs.asp"-->
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/csdo_utils_vbs.asp"-->
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/utility_func_vbs.asp"-->
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/cs_security/cs_security_login_utils_vbs.asp"-->
<% if Application("ENABLE_OWNERSHIP")="TRUE" then %>
<!--#INCLUDE VIRTUAL = "/cheminv/gui/GetLocationAuthorizedAttribute.asp" -->
<% end if 
ContainerCost = Trim(ContainerCost)
if Application("ENABLE_OWNERSHIP")="TRUE" then
PrincipalID=Session("PrincipalID")
LocationTypeID = trim(Replace(Session("LocationTypeID"),",","",1))
end if
%>
<html>
<head>
<style>
		A {Text-Decoration: none;}
		.TabView {color:#000000; font-size:8pt; font-family: arial}
		A.TabView:LINK {Text-Decoration: none; color:#000000; font-size:8pt; font-family: arial}
		A.TabView:VISITED {Text-Decoration: none; color:#000000; font-size:8pt; font-family: arial}
		A.TabView:HOVER {Text-Decoration: underline; color:#4682b4; font-size:8pt; font-family: arial}
</style>
<title><%=Application("appTitle")%> -- Order New Substance</title>
<!--#INCLUDE VIRTUAL = "/cheminv/gui/dateFormat_js.asp"-->
<script LANGUAGE="javascript" src="/cheminv/Choosecss.js"></script>
<script LANGUAGE="javascript" src="/cheminv/utils.js"></script>
<script LANGUAGE="javascript" src="/cheminv/gui/CalculateFromPlugin.js"></script>
<script LANGUAGE="javascript" src="/cheminv/gui/validation.js"></script>
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/navbar_js.js"-->
<!--#INCLUDE FILE="../source/app_js.js"-->
<script language="JavaScript">
<!--Hide JavaScript
	var DialogWindow;
	window.focus();
	var cd_plugin_threshold= <%=Application("CD_PLUGIN_THRESHOLD")%>;
function setPrincipalID(element)
{
    <% if Application("ENABLE_OWNERSHIP")="TRUE" then %>
    !document.form1.Ownershiplst ? document.form1.PrincipalID.value="" :  document.form1.PrincipalID.value=document.form1.Ownershiplst.value;
    <% end if %>
}


function setOwnership()
{
  <%if Application("ENABLE_OWNERSHIP")="TRUE" then%>
   var type="";
    <%if isEdit and (lcase(sTab) = "required" or (sTab = "" and lcase(Session("sTab2")) = "required"))  then %>
       type=document.getElementById("OwnershipType").value;  
    <%elseif PrincipalID <> "" and lcase(sTab) = "required" then %>
  
       type=<%=PrincipalID%>
        document.getElementById("PrincipalID").value=type;
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
    }
   <% end if %>   
}
	// Validates container attributes
	function ValidateContainer(strMode){	
		var bWriteError = false;
		var errmsg = "Please fix the following problems:\r";
		
		//Populate hidden variables
		!document.form1.iBarcode ? document.form1.Barcode.value="<%=Barcode%>" : document.form1.Barcode.value = document.form1.iBarcode.value;
		!document.form1.iContainerName ? document.form1.ContainerName.value="<%=ContainerName%>" : document.form1.ContainerName.value = document.form1.iContainerName.value;
		!document.form1.iComments ? document.form1.Comments.value=document.form1.Comments.value : document.form1.Comments.value = document.form1.iComments.value;
		!document.form1.iUOMID ? document.form1.UOMID.value="<%=UOMID%>" : document.form1.UOMID.value = document.form1.iUOMID.value;
		document.form1.UOMID.value = document.form1.UOMID.value.split("=")[0];
		!document.form1.iUOWID ? document.form1.UOWID.value="<%=UOWID%>" : document.form1.UOWID.value = document.form1.iUOWID.value;
		document.form1.UOWID.value = document.form1.UOWID.value.split("=")[0];
		!document.form1.iUOCID ? document.form1.UOCID.value="<%=UOCID%>" : document.form1.UOCID.value = document.form1.iUOCID.value;
		document.form1.UOCID.value = document.form1.UOCID.value.split("=")[0];
		!document.form1.iUOPID ? document.form1.UOPID.value="<%=UOPID%>" : document.form1.UOPID.value = document.form1.iUOPID.value;
		document.form1.UOPID.value = document.form1.UOPID.value.split("=")[0];
		!document.form1.iContainerTypeID ? document.form1.ContainerTypeID.value="<%=ContainerTypeID%>" : document.form1.ContainerTypeID.value = document.form1.iContainerTypeID.value;
		!document.form1.iContainerStatusID ? document.form1.ContainerStatusID.value="<%=ContainerStatusID%>" : document.form1.ContainerStatusID.value = document.form1.iContainerStatusID.value;
		!document.form1.iQtyMax ? document.form1.QtyMax.value="<%=QtyMax%>" : document.form1.QtyMax.value = document.form1.iQtyMax.value;
		!document.form1.iQtyInitial ? document.form1.QtyInitial.value="<%=QtyInitial%>" : document.form1.QtyInitial.value = document.form1.iQtyInitial.value;
		!document.form1.iQtyRemaining ? document.form1.QtyRemaining.value="<%=QtyRemaining%>" : document.form1.QtyRemaining.value = document.form1.iQtyRemaining.value;
		!document.form1.iMinStockQty ? document.form1.MinStockQty.value="<%=MinStockQty%>" : document.form1.MinStockQty.value = document.form1.iMinStockQty.value;
		!document.form1.iMaxStockQty ? document.form1.MaxStockQty.value="<%=MaxStockQty%>" : document.form1.MaxStockQty.value = document.form1.iMaxStockQty.value;
		!document.form1.iCompoundID ? document.form1.CompoundID.value="<%=CompoundID%>" : document.form1.CompoundID.value = document.form1.iCompoundID.value;
		!document.form1.iConcentration ? document.form1.Concentration.value="<%=Concentration%>" : document.form1.Concentration.value = document.form1.iConcentration.value;
		!document.form1.iRegID ? document.form1.RegID.value="<%=RegID%>" : document.form1.RegID.value = document.form1.iRegID.value;
		!document.form1.iBatchNumber ? document.form1.BatchNumber.value="<%=BatchNumber%>" : document.form1.BatchNumber.value = document.form1.iBatchNumber.value;
		!document.form1.iPurity ? document.form1.Purity.value="<%=Purity%>" : document.form1.Purity.value = document.form1.iPurity.value;
		!document.form1.iGrade ? document.form1.Grade.value="<%=Grade%>" : document.form1.Grade.value = document.form1.iGrade.value;
		!document.form1.iSolvent ? document.form1.Solvent.value="<%=Solvent%>" : document.form1.Solvent.value = document.form1.iSolvent.value;
		!document.form1.iExpDate ? document.form1.ExpDate.value="<%=ExpDate%>" : document.form1.ExpDate.value = document.form1.iExpDate.value;
		!document.form1.iTareWeight ? document.form1.TareWeight.value="<%=TareWeight%>" : document.form1.TareWeight.value = document.form1.iTareWeight.value;
		!document.form1.iSupplierID ? document.form1.SupplierID.value="<%=SupplierID%>" : document.form1.SupplierID.value = document.form1.iSupplierID.value;
		!document.form1.iLotNum ? document.form1.LotNum.value="<%=LotNum%>" : document.form1.LotNum.value = document.form1.iLotNum.value;
		!document.form1.iPONumber ? document.form1.PONumber.value="<%=PONumber%>" : document.form1.PONumber.value = document.form1.iPONumber.value;
		!document.form1.iReqNumber ? document.form1.ReqNumber.value="<%=ReqNumber%>" : document.form1.ReqNumber.value = document.form1.iReqNumber.value;
		!document.form1.iSupplierCatNum ? document.form1.SupplierCatNum.value="<%=SupplierCatNum%>" : document.form1.SupplierCatNum.value = document.form1.iSupplierCatNum.value;
		!document.form1.iDateProduced ? document.form1.DateProduced.value="<%=DateProduced%>" : document.form1.DateProduced.value = document.form1.iDateProduced.value;	
		!document.form1.iDateOrdered ? document.form1.DateOrdered.value="<%=DateOrdered%>" : document.form1.DateOrdered.value = document.form1.iDateOrdered.value;	
		!document.form1.iDateReceived ? document.form1.DateReceived.value="<%=DateReceived%>" : document.form1.DateReceived.value = document.form1.iDateReceived.value;		
		!document.form1.iContainerCost ? document.form1.ContainerCost.value= "<%=ContainerCost%>" : document.form1.ContainerCost.value = document.form1.iContainerCost.value;		
		!document.form1.iUOCostID ? document.form1.UOCostID.value="<%=UOCostID%>" : document.form1.UOCostID.value = document.form1.iUOCostID.value;
		document.form1.UOCostID.value = document.form1.UOCostID.value.split("=")[0];
		//Owner is now hard-coded to "Array"
		!document.form1.iOwnerID ? document.form1.OwnerID.value="<%=OwnerID%>" : document.form1.OwnerID.value = document.form1.iOwnerID.value;		
		!document.form1.iCurrentUserID ? document.form1.CurrentUserID.value="<%=CurrentUserID%>" : document.form1.CurrentUserID.value = document.form1.iCurrentUserID.value;		
		!document.form1.iLocationID ? document.form1.LocationID.value="<%=LocationID%>" : document.form1.LocationID.value = document.form1.iLocationID.value;
		!document.form1.iNumCopies ? document.form1.NumCopies.value="<%=NumCopies%>" : document.form1.NumCopies.value = document.form1.iNumCopies.value;

		// added fields for order integration
		!document.form1.iDueDate ? document.form1.DueDate.value="<%=DueDate%>" : document.form1.DueDate.value = document.form1.iDueDate.value;	
		!document.form1.iProject ? document.form1.Project.value="<%=Project%>" : document.form1.Project.value = document.form1.iProject.value;	
		!document.form1.iJob ? document.form1.Job.value="<%=Job%>" : document.form1.Job.value = document.form1.iJob.value;	
		!document.form1.iRushOrder ? document.form1.RushOrder.value="<%=RushOrder%>" : document.form1.RushOrder.value = document.form1.iRushOrder.value;	
		!document.form1.iDeliveryLocationID ? document.form1.DeliveryLocationID.value="<%=DeliveryLocationID%>" : document.form1.DeliveryLocationID.value = document.form1.iDeliveryLocationID.value;
		!document.form1.iUnknownSupplierName ? document.form1.UnknownSupplierName.value="<%=UnknownSupplierName%>" : document.form1.UnknownSupplierName.value = document.form1.iUnknownSupplierName.value;	
		!document.form1.iUnknownSupplierContact ? document.form1.UnknownSupplierContact.value="<%=UnknownSupplierContact%>" : document.form1.UnknownSupplierContact.value = document.form1.iUnknownSupplierContact.value;	
		!document.form1.iUnknownSupplierPhoneNumber ? document.form1.UnknownSupplierPhoneNumber.value="<%=UnknownSupplierPhoneNumber%>" : document.form1.UnknownSupplierPhoneNumber.value = document.form1.iUnknownSupplierPhoneNumber.value;	
		!document.form1.iUnknownSupplierFAXNumber ? document.form1.UnknownSupplierFAXNumber.value="<%=UnknownSupplierFAXNumber%>" : document.form1.UnknownSupplierFAXNumber.value = document.form1.iUnknownSupplierFAXNumber.value;	
	    //Ownership
	    <%if Application("ENABLE_OWNERSHIP")="TRUE" then%>
	    !document.form1.Ownershiplst ? document.form1.PrincipalID.value="<%=PrincipalID%>" :  document.form1.PrincipalID.value=document.form1.Ownershiplst.value;
        !document.form1.iLocationTypeID ? document.form1.LocationTypeID.value="<%=LocationTypeID%>" : document.form1.LocationTypeID.value = document.form1.iLocationTypeID.value;
	    <% end if %>
		// MCD: added for 'Order Reason'		
		!document.form1.iOrderReason ? document.form1.OrderReason.value="<%=OrderReason%>" : document.form1.OrderReason.value = document.form1.iOrderReason.value;	
		!document.form1.iOrderReasonOther ? document.form1.OrderReasonOther.value=document.form1.OrderReasonOther.value : document.form1.OrderReasonOther.value = document.form1.iOrderReasonOther.value;
		// MCD: end changes

		// fill in fields for the integration
		document.form1.QtyInitial.text = document.form1.QtyMax.value;

		// if compoundID is present must be a positive number
		if (document.form1.RegID.value.length == 0) {
			if (document.form1.CompoundID.value.length >0 && (!isPositiveNumber(document.form1.CompoundID.value) || document.form1.CompoundID.value < 1)){
				errmsg = errmsg + "- Compound ID must be a positive number greater than zero.";
				bWriteError = true;
			}
		}
	
		/* DJP: CompoundID is not required as per CSBR-49916
		// CompoundID is required
		if (document.form1.RegBatchID.value.length == 0) {
			if (document.form1.CompoundID.value.length == 0) {
				errmsg = errmsg + "- CompoundID is required.\r";
				bWriteError = true;
			}
			else if (!isPositiveNumber(document.form1.iCompoundID.value) || document.form1.iCompoundID.value < 1){
				errmsg = errmsg + "- Compound ID must be a positive number greater than zero.";
				bWriteError = true;
			}
		}
		*/
			
		//LocationID is required
		
		if (document.form1.LocationID.value.length == 0) {
			errmsg = errmsg + "- Location ID is required.\r";
			bWriteError = true;
		}
		else{
			// LocationID must be a psositve number
			if (!isPositiveNumber(document.form1.LocationID.value)){
				errmsg = errmsg + "- Location ID must be a positive number.\r";
				bWriteError = true;
			}
		}	
		
        //Container Admin is required
       <%if Application("ENABLE_OWNERSHIP")="TRUE" then%>
		if (document.form1.PrincipalID.value.length == 0) {
			errmsg = errmsg + "- Container Admin is required.\r";
			bWriteError = true;
		}
        if (document.form1.LocationAdmin.value!='' && document.form1.LocationAdmin.value !=document.form1.PrincipalID.value){
            if (confirm("- The Location Admin and Container Admin are not the same,\r Do you really want to continue?")!=true){     
                return;
            }
        }
	   <% end if %>
		//DeliveryLocationID is required
		if (document.form1.DeliveryLocationID.value.length == 0) {
			errmsg = errmsg + "- Delivery Location ID is required.\r";
			bWriteError = true;
		}
		else{
			// DeliveryLocationID must be a positive number
			if (!isPositiveNumber(document.form1.DeliveryLocationID.value)){
				errmsg = errmsg + "- Delivery Location ID must be a positive number.\r";
				bWriteError = true;
			}
		}	
		<%if Application("ENABLE_OWNERSHIP")="TRUE" then%>
		    if(GetAuthorizedLocation(document.form1.DeliveryLocationID.value,document.getElementById("tempCsUserName").value,document.getElementById("tempCsUserID").value)==0)
            {
                errmsg = errmsg + "- Not athorized for this delivery location.\r";
			    alert(errmsg);
			    return;
            }
        
          if(document.form1.LocationTypeID.value!=0)
        {
                if(GetValidLocation(document.form1.DeliveryLocationID.value,"",document.form1.LocationTypeID.value)==0)
              {
                    if (confirm("- Location type is not match with delivery location.\r - Do you want to continue...")!=true)
                    {
                        return;
                    }
			        
              }
        }
        <%end if%>
		//Container Name is Required
		
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
		//QtyMax if present should not have comma
		var m = document.form1.QtyMax.value.toString();		
		if(m.indexOf(",") != -1){
			errmsg = errmsg + "- Quantity Max has wrong decimal operator.\r";
			bWriteError = true;
		}
<% if NOT isEdit then%>
		
		// NumCopies is required
		if (document.form1.NumCopies.value.length == 0) {
			errmsg = errmsg + "- Quantity is required.\r";
			bWriteError = true;
		}
		else{
			// NumCopies must be a number
			if (!isPositiveNumber(document.form1.NumCopies.value)){
				errmsg = errmsg + "- Quantity must be a positive number.\r";
				bWriteError = true;
			}
			else {
				if (document.form1.NumCopies.value > 399){
					errmsg = errmsg + "- Quantity must be less than 400.\r";
					bWriteError = true;	
				}
			}
		}
<%else%>
		// QtyRemaining is required
		if (document.form1.QtyRemaining.value.length == 0) {
			errmsg = errmsg + "- Quantity Remaining is required.\r";
			bWriteError = true;
		}
		else{
			// QtyMax must be a number
			if (!isPositiveNumber(document.form1.QtyRemaining.value)){
			errmsg = errmsg + "- Quantity Remaining must be a positive number.\r";
			bWriteError = true;
			}
			if (document.form1.QtyRemaining.value/1 > document.form1.QtyMax.value/1){
			errmsg = errmsg + "- Quantity Remaining cannot be larger than Container Size.\r";
			bWriteError = true;
			} 
		}
<% end if%>
		
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
			errmsg = errmsg + "- TareWeight must be a positive number.\r";
			bWriteError = true;
		}
		
		// Purity if present must be a number
		if (document.form1.Purity.value.length >0 && !isPositiveNumber(document.form1.Purity.value)){
			errmsg = errmsg + "- Purity must be a positive number.\r";
			bWriteError = true;
		}
		//Purity if present should not have comma
		var m = document.form1.Purity.value.toString();		
		if(m.indexOf(",") != -1){
			errmsg = errmsg + "- Purity has wrong decimal operator.\r";
			bWriteError = true;
		}
		// TareWeight if present must be a number
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
		// Date Produced must be a date
		if (document.form1.DateProduced.value.length > 0 && !isDate(document.form1.DateProduced.value)){
			errmsg = errmsg + "- Date Produced must be in MM/DD/YYYY format.\r";
			bWriteError = true;
		}
		/*
		// Expiration must be a date
		if (document.form1.ExpDate.value.length > 0 && !isDate(document.form1.ExpDate.value)){
			errmsg = errmsg + "- Expiration Date must be in MM/DD/YYYY format.\r";
			bWriteError = true;
		}
		*/
		// Date Ordered must be a date
		if (document.form1.DateOrdered.value.length > 0 && !isDate(document.form1.DateOrdered.value)){
			errmsg = errmsg + "- Date Ordered must be in MM/DD/YYYY format.\r";
			bWriteError = true;
		}
		/*
		// Data Received must be a date
		if (document.form1.DateReceived.value.length > 0 && !isDate(document.form1.DateReceived.value)){
			errmsg = errmsg + "- Date Received must be in MM/DD/YYYY format.\r";
			bWriteError = true;
		}
		*/
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
		
		// Validation added for order integration
		
		// Container cost is required
		if (document.form1.ContainerCost.value.length ==0){
			errmsg = errmsg + "- The Container Cost is required.\r";
			bWriteError = true;
		}

		// Project is required.
		if (document.form1.Project.value.length == 0){
			errmsg = errmsg + "- The Project is required.\r";
			bWriteError = true;
		}

		// Job is required.
		if (document.form1.Job.value.length == 0){
			errmsg = errmsg + "- The Job is required.\r";
			bWriteError = true;
		}

		// Supplier ID is required.
		if (document.form1.SupplierID.value.length == 0){
			errmsg = errmsg + "- The Supplier is required.\r";
			bWriteError = true;
		}
		
		// If Supplier is "New Supplier", supplier name, etc. are required.
		if ((document.form1.SupplierID.value.length > 0) && (document.form1.SupplierID.value == "1000")){
			if (document.form1.UnknownSupplierName.value.length == 0) {
				errmsg = errmsg + "- The New Supplier Name is required.\r";
				bWriteError = true;
			}
			if (document.form1.UnknownSupplierContact.value.length == 0) {
				errmsg = errmsg + "- The New Supplier Contact is required.\r";
				bWriteError = true;
			}
			if (document.form1.UnknownSupplierPhoneNumber.value.length == 0) {
				errmsg = errmsg + "- The New Supplier Phone Number is required.\r";
				bWriteError = true;
			}
			if (document.form1.UnknownSupplierFAXNumber.value.length == 0) {
				errmsg = errmsg + "- The New Supplier Fax Number is required.\r";
				bWriteError = true;
			}
		}
		
		// Supplier Catalog Number is required.
		if (document.form1.SupplierCatNum.value.length == 0){
			errmsg = errmsg + "- The Catalog Number is required.\r";
			bWriteError = true;
		}
				
		// The due date must be present.
		if (document.form1.DueDate.value.length == 0){
			errmsg = errmsg + "- The Due Date is required.\r";
			bWriteError = true;
		}

		// Due Date must be a date
		if (document.form1.DueDate.value.length > 0 && !isDate(document.form1.DueDate.value)){
			errmsg = errmsg + "- The Due Date must be in MM/DD/YYYY format.\r";
			bWriteError = true;
		}

		// Due Date must be at least tomorrow
	    var d, dueDate;
	    dueDate = getDate(document.form1.DueDate.value);
	    d = new Date();
	    d.setHours(0,0,0,0);		// set time to 00:00:00.000
	    d.setDate(d.getDate()+1);	// set date to "tomorrow"
		if (document.form1.DueDate.value.length > 0 && isDate(document.form1.DueDate.value) && Date.parse(dueDate) < Date.parse(d)) {
			errmsg = errmsg + "- The Due Date must be at least tomorrow.\r";
			bWriteError = true;
		}
		

		//MCD: Added for 'Order Reason'
		// Order Reason may be required.
		if (<%=Session("Hassle")%> == 1) {
			if (document.form1.OrderReason.value.length == 0){
				errmsg = errmsg + "- The Order Reason is required.\r";
				bWriteError = true;
			}
			if ((document.form1.OrderReason.value == 3) && (document.form1.OrderReasonOther.value.length == 0)) {
				errmsg = errmsg + "- The Order Reason is required.\r";
				bWriteError = true;
			}
		}
		//MCD: end changes

		
		// Report problems
		if (bWriteError){
			alert(errmsg);
		}
		else{
			var bcontinue = true;
			<%
				If IsEmpty(CAS) Then
			%>
			bcontinue = confirm("The substance you have ordered does not have a CAS number.\rThis will cause difficulties tracking the container.  Continue?");
			<%
				End If
			%>
			
			if (bcontinue) {
				document.form1.action = "OrderContainer_action.asp";
			
				var bcontinue2 = true;
				
				if ((document.form1.CompoundID.value.length == 0)&&(document.form1.RegID.value.length == 0)){
					bcontinue2 = confirm("No Compound has been asigned to this container.\rDo you really want to create a container without an associated chemical compound?");
				}
				if (bcontinue2) document.form1.submit();
			}
		}
	}
	 
	// Post data between tabs
	function postDataFunction(sTab, newFocus) {
		document.form1.action = "OrderContainer.asp?GetData=form&TB=" + sTab + "&setFocus=" + newFocus;
		document.form1.submit();
	}

//-->
</script>
<% if Session("isCDP") = "TRUE" then %>
<script language="JavaScript" src="/cfserverasp/source/chemdraw.js"></script>
<script>cd_includeWrapperFile("/cfserverasp/source/")</script>
<SCRIPT LANGUAGE="javascript" src="<%=Application("CDJSUrl")%>"></SCRIPT>
<% end if %>

</head>
<body onload="setOwnership();">
<!--#INCLUDE VIRTUAL = "/cheminv/gui/OrderContainerTabs.asp"-->
<form name="form1" method="POST">
<input TYPE="hidden" NAME="ContainerID" Value="<%=ContainerID%>">
<input TYPE="hidden" NAME="Barcode" Value>
<input TYPE="hidden" NAME="LocationID" Value> 
<input TYPE="hidden" NAME="ContainerTypeID" Value="=">
<input TYPE="hidden" NAME="ContainerStatusID" Value="=">  
<input TYPE="hidden" NAME="UOMID" Value>
<input TYPE="hidden" NAME="UOWID" Value>
<input TYPE="hidden" NAME="UOCID" Value>
<input TYPE="hidden" NAME="UOPID" Value>
<input TYPE="hidden" NAME="QtyMax" Value>
<input TYPE="hidden" NAME="QtyInitial" Value>
<input TYPE="hidden" NAME="QtyRemaining" Value="<%=QtyRemaining%>">
<input TYPE="hidden" NAME="CompoundID" Value>
<input TYPE="hidden" NAME="ContainerName" Value>
<input TYPE="hidden" NAME="Comments" Value="<%=Comments%>">
<input TYPE="hidden" NAME="MinStockQty" Value>
<input TYPE="hidden" NAME="MaxStockQty" Value>
<input TYPE="hidden" NAME="ExpDate" Value>
<input TYPE="hidden" NAME="TareWeight" Value>
<input TYPE="hidden" NAME="RegID" Value>
<input TYPE="hidden" NAME="BatchNumber" Value>
<input TYPE="hidden" NAME="Purity" Value>
<input TYPE="hidden" NAME="Concentration" Value>
<input TYPE="hidden" NAME="Grade" Value>
<input TYPE="hidden" NAME="Solvent" Value>
<input TYPE="hidden" NAME="SupplierID" Value>
<input TYPE="hidden" NAME="SupplierCatNum" Value>
<input TYPE="hidden" NAME="LotNum" Value>
<input TYPE="hidden" NAME="PONumber" Value>
<input TYPE="hidden" NAME="ReqNumber" Value>
<input TYPE="hidden" NAME="DateProduced" Value>
<input TYPE="hidden" NAME="DateOrdered" Value>
<input TYPE="hidden" NAME="DateReceived" Value>
<input TYPE="hidden" NAME="ContainerCost" Value>
<input TYPE="hidden" NAME="UOCostID" Value>
<input TYPE="hidden" NAME="OwnerID" Value>
<input TYPE="hidden" NAME="CurrentUserID" Value>
<input TYPE="hidden" NAME="NumCopies" Value>
<input TYPE="hidden" NAME="AutoGen" Value="<%=AutoGen%>">
<input TYPE="hidden" NAME="AutoPrint" Value="<%=AutoPrint%>">
<input TYPE="hidden" NAME="returnToSearch" Value="<%=ReturnToSearch%>">

<!--MWS: Added extra attributes -->
<input TYPE="hidden" NAME="DueDate" Value>
<input TYPE="hidden" NAME="Project" Value>
<input TYPE="hidden" NAME="Job" Value>
<input TYPE="hidden" NAME="RushOrder" Value="<%=RushOrder%>">
<input TYPE="hidden" NAME="DeliveryLocationID" Value> 
<input TYPE="hidden" NAME="UnknownSupplierName" Value> 
<input TYPE="hidden" NAME="UnknownSupplierContact" Value> 
<input TYPE="hidden" NAME="UnknownSupplierPhoneNumber" Value> 
<input TYPE="hidden" NAME="UnknownSupplierFAXNumber" Value> 
<!--MCD: Added for 'Order Reason' -->
<input TYPE="hidden" NAME="OrderReason" Value>
<input TYPE="hidden" NAME="OrderReasonOther" Value="<%=OrderReasonOther%>">
<input TYPE="hidden" NAME="tempCsUserName" ID="tempCsUserName" Value="<%=Session("UserName" & "cheminv")%>" >
<input TYPE="hidden" NAME="tempCsUserID" ID="tempCsUserID" Value=<%=Server.URLEncode(CryptVBS(Session("UserID" & "cheminv"),"ChemInv\API\GetBatchInfo.asp"))%>>
<%if Application("ENABLE_OWNERSHIP")="TRUE" then%>
<input TYPE="hidden" NAME="OwnerShipGroupList" id="OwnerShipGroupList" Value="<%=GetOwnerShipGroupList()%>">
<input TYPE="hidden" NAME="OwnerShipUserList" id="OwnerShipUserList" Value="<%=GetOwnerShipUserList()%>">
<input TYPE="hidden" NAME="PrincipalID" id="PrincipalID" Value=<%=PrincipalID%>>
<input type="hidden" NAME="OwnershipType" id="OwnershipType" value="<%=OwnershipType%>" />
<input TYPE="hidden" NAME="LocationAdmin" id="LocationAdmin" Value="<%=LocationAdmin%>">
<input TYPE="hidden" NAME="LocationTypeID" id="LocationTypeID" Value>
<% end if %>
<br/>
<table border="0" cellspacing="0" cellpadding="0" width="700">
<%
Select Case sTab
	Case "Required"
%>
	<tr height="25">
		<td align="right" valign="top" nowrap width="150">
			<span class="required">Delivery loc ID: <%=GetBarcodeIcon()%></span>
		</td>
		<td colspan="3">
        <%if Application("ENABLE_OWNERSHIP")="TRUE" then
                authorityFunction = "SetOwnerInfo('location');"
            else
                authorityFunction= ""
            end if %>
			<%ShowLocationPicker9 "document.form1", "iDeliveryLocationID", "lpDeliveryLocationBarCode", "lpDeliveryLocationName", 10, 49, false, DeliveryLocationID, authorityFunction%> 
		</td>
	</tr>
	<tr height="25">
		<td align="right" valign="top" nowrap width="150">
			On Order loc ID: <%=GetBarcodeIcon()%>
		</td>
		<td colspan="3">
			<%ShowReadOnlyLocationWithoutPicker "document.form1", "iLocationID", "lpLocationBarCode", "lpLocationName", 10, 49, false%> 
		</td>
	</tr>
	<% if Application("ENABLE_OWNERSHIP")="TRUE" then %>
	<tr height="25">
		                <td align="right" valign="top" nowrap>
			                <span class="">Location Type:<span></td>
			             <td>
			                <%=ShowSelectBox3("LocationTypeID", LocationTypeID,"SELECT Location_Type_ID AS Value, Location_Type_Name AS DisplayText FROM inv_Location_Types ORDER BY Location_Type_Name ASC", 0, "None", "0","")%>
		                </td>
	</tr>
	<tr height="25">
		<td align=right> <span title="Pick an option from the list" class="required">Container Admin:</span></td>
		<td align=left>
		<table border=.5>
		<tr><td><input type="Radio" name="Ownership" name="User_cb" id="User_cb" onclick="SetOwnerInfo('user');"/>by user</td>
		<td><input type="Radio" name="Ownership" name="Group_cb" id="Group_cb" onclick="SetOwnerInfo('group');" />by Group</td></tr>
		<tr><td colspan="2"><SELECT id="Ownershiplst" onchange="setPrincipalID(this);" ><OPTION></OPTION></SELECT></td></tr></table></td></tr>
    <% end if %>
	<tr height="25">
		<td align="right" valign="top" nowrap width="150">
			<span class="required">Container name:</span>
		</td>
		<td colspan="3">
			<input type="text" name="iContainerName" size="70" value="<%=ContainerName%>">
		</td>
	</tr>
	<tr height="25">	
		<%= ShowPickList("<SPAN class=required>Container type:</span>", "iContainerTypeID", ContainerTypeID,"SELECT Container_Type_ID AS Value, Container_Type_Name AS DisplayText FROM inv_Container_Types ORDER BY lower(DisplayText) ASC")%>
		<td></td><td></td>	
	</tr>
	<tr height="25">
		<td align="right" valign="top">
			<span title="Pick an option from the list" class="required">Unit of measure:</span>
		</td>
		<td>
		<%=ShowSelectBox2("iUOMID", Session("UOMIDOptionValue"),"SELECT UNIT_ID||'='||Unit_Abreviation AS Value, Unit_Name AS DisplayText FROM inv_units WHERE Unit_type_id_FK IN (1,2,4) ORDER BY lower(DisplayText) ASC", 20, "Track by weight", "0")%>
		</td>
		<td></td><td></td>
	</tr>
	<tr height="25">
		<%=ShowInputBox("Container Size:", "QtyMax", 15, "", False, true)%>
		<td></td><td></td>
	</tr>
	<% if not IsEdit then	
	QtyInitial = QtyMax
	%>
	<tr height="25">
		<%=ShowInputBox("Quantity:", "NumCopies", 5, "", False, True)%>
		<%If SupplierID = 1000 Then%>
			<td colspan="2"><b>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;New Supplier Info:</b></td>
		<%End if%>
	</tr>
	<%else%>
		<%=ShowInputBox("Quantity Remaining:", "QtyRemaining", 5, "", False, true)%>
		<%If SupplierID = 1000 Then%>
			<td colspan="2"><b>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;New Supplier Info:</b></td>
		<%End if%>
	<%End if%>
	<tr height="25">
		<td align="right" valign="top">
			<span class="required" title="Pick an option from the list">Supplier Name:</span>
		</td>
		<td>
			<%=ShowSelectBox3("iSupplierID", SupplierID, "SELECT Supplier_ID AS Value, Supplier_Short_Name AS DisplayText FROM inv_suppliers ORDER BY lower(Supplier_Short_Name) ASC", 27, RepeatString(43, "&nbsp;"), "", "postDataFunction('Required', 'iSupplierCatNum');")%>
		</td>
		<%If SupplierID = 1000 Then%>
			<%=ShowInputBox("Name:", "UnknownSupplierName", 25, "", False, False)%>
		<%End if%>
	</tr>
	<tr height="25">
		<%=ShowInputBox("Catalog Number:", "SupplierCatNum", 15, "", False, true)%>
		<%If SupplierID = 1000 Then%>
			<%=ShowInputBox("Contact:", "UnknownSupplierContact", 25, "", False, False)%>
		<%End if%>
		<td></td><td></td>
	</tr>
	<tr height="25">
		<td align="right" valign="top">
			<span class="required" title="Pick an option from the list">Project:</span>
		</td>
		<td>
		<%=ShowSelectBox3("iProject", Project, "SELECT DISTINCT project_no AS Value, project_description AS DisplayText FROM " &  Application("CHEMINV_USERNAME") &  ".inv_Project_Job_Info ORDER BY UPPER(project_description)", 27, RepeatString(43, "&nbsp;"), "", "postDataFunction('Required', 'iJob');")%>
		<%If SupplierID = 1000 Then%>
			<%=ShowInputBox("Phone#:", "UnknownSupplierPhoneNumber", 25, "", False, False)%>
		<%End if%>
		</td>
	</tr>
	<tr height="25">
		<td align="right" valign="top">
			<span class="required" title="Pick an option from the list">Job:</span>
		</td>
		<td>
		<%=ShowSelectBox2("iJob", Job, "SELECT job_no AS Value, job_description AS DisplayText FROM " &  Application("CHEMINV_USERNAME") &  ".inv_Project_Job_Info WHERE project_no = '" & Project & "' ORDER BY UPPER(job_description)", 27, RepeatString(43, "&nbsp;"), "")%>
		<%If SupplierID = 1000 Then%>
			<%=ShowInputBox("FAX#:", "UnknownSupplierFAXNumber", 25, "", False, False)%>
		<%End if%>
	</tr>
	<tr height="25">
		<td align="right" valign="top" nowrap width="150"><span class="required">Due Date:</span></td>
		<td><%call ShowInputField("", "", "iDueDate:form1:" & DueDate , "DATE_PICKER:TEXT", "15")%>
		<!--<td align="right" valign="top" nowrap width="150"><span class="required">Due Date:</span></td><td><input type="text" name="iDueDate" size="15" value="<%=DueDate%>"><a href onclick="return PopUpDate(&quot;iDueDate&quot;,&quot;/cheminv/gui/month_view1.asp&quot;)"><img src="/cfserverasp/source/graphics/navbuttons/icon_mview.gif" height="16" width="16" border="0"></a></td>-->
	</tr>
	<tr height="25">
		<td align="right" valign="top" nowrap width="150"><span class="required">Container Cost:</span></td>
		<td>
			<%=ShowSelectBox("iUOCostID", Session("UOCostIDOptionValue"),"SELECT UNIT_ID||'='||Unit_Abreviation AS Value, Unit_Abreviation AS DisplayText FROM inv_units WHERE Unit_type_id_FK = 5 ORDER BY Unit_Name ASC")%>
			<input type="text" name="iContainerCost" size="15" value="<%=ContainerCost%>">
		</td>
		<%'=ShowInputBox("Container Cost($):", "ContainerCost", 15, "", False, true)%>
	</tr>
	<tr height="25">
		<%= ShowPickList("<SPAN class=required>Container Status:</span>", "iContainerStatusID", ContainerStatusID,"SELECT Container_Status_ID AS Value, Container_Status_Name AS DisplayText FROM inv_Container_Status where Container_Status_ID = 3")%>
		<td colspan="2">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<a class="MenuLink" href="Create%20a%20new%20inventory%20substance%20and%20assign%20it%20to%20this%20container" onclick="OpenDialog('/cheminv/gui/CreateOrEditSubstance.asp', 'newDialog', 2); return false;" title="Create a new inventory substance and assign it to this container">New Substance</a>&nbsp;|&nbsp;<a class="MenuLink" HREF="Select%20an%20existing%20substance%20and%20assign%20it%20to%20this%20container" target="SubstancesFormGroup" onclick="OpenDialog('/cheminv/inputtoggle.asp?formgroup=global_substanceselect_form_group&amp;dataaction=db&amp;dbname=cheminv&amp;showbanner=false', 'newDialog', 2); return false;" title="Select an existing substance and assign it to this container">Select Substance</a></td>
	</tr>
	<% if not isEdit then%>
	    <%if Application("RegServerName") <> "NULL" then%>
            <tr height="25">
		        <td>&nbsp;</td><td><input type="checkbox" name="rushOrder_cb" onclick="document.form1.RushOrder.value = this.checked;">Rush order<br></td>
		        <script language="Javascript">if (document.form1.RushOrder.value == "true") document.form1.rushOrder_cb.click(); </script>
		        <td width="200" align="right">Registry Batch ID:</td><td><input type="text" Size="15" name="RegBatchID" value="<%=RegBatchID%>" readonly onchange="GetRegIDFromRegNum(this.value); iCompoundID.value=''; CompoundID.value='';" class=readonly><input type="hidden" name="iRegID" value="<%=RegID%>"><input type="hidden" name="iBatchNumber" value="<%=BatchNumber%>"></td>
		        <input type="hidden" name="NewRegID">
		        <input type="hidden" name="NewBatchNumber">	
            </tr>
        <%end if %>
	    <tr>
		    <td>&nbsp;</td><td><input type="checkbox" name="returnToSearch_cb" onclick="document.form1.returnToSearch.value = this.checked;">New search when done<br></td>
		    <script language="Javascript">if (document.form1.returnToSearch.value == "true") document.form1.returnToSearch_cb.click(); </script>
		    <td width="200" align="right">Compound ID:</td><td><input type="text" Size="15" name="iCompoundID" value="<%=CompoundID%>" readonly onchange="NewCompoundID.value= this.value; RegBatchID.value=''; iRegID.value=''; iBatchNumber.value='';" onpropertychange="NewCompoundID.value= this.value; RegBatchID.value=''; iRegID.value=''; iBatchNumber.value=''; if(this.value != '') {postDataFunction('Required', 'iCompoundID');}" class=readonly></td>
		    <input Type="hidden" name="NewCompoundID">	
	<%Else%>
	    <tr height="25">
		    <td align="right" valign="top" nowrap width="150">Container ID (barcode):</td><td><input type="text" name="iBarcode" size="15" value="<%=Barcode%>"></td>
	<%End if%>
	</tr>

	<!-- MCD: Added for 'Order Reason' -->
	<%
		If Session("Hassle") = 1 Then
	%>
	<tr height="25">
		<td align="right" valign="top">
			<span class="required" title="Pick an option from the list">Order Reason:</span>
		</td>
		<td>
		<%=ShowSelectBox3("iOrderReason", OrderReason, "SELECT DISTINCT container_order_reason_id AS Value, name AS DisplayText, sort_order FROM Inv_Container_Order_Reason ORDER BY sort_order ASC", 27, RepeatString(43, "&nbsp;"), "", "postDataFunction('Required', 'iOrderReasonOther');")%>
		</td>
	</tr>
	<%
			If Session("OrderReason") = "3" Then
	%>
	<tr height="50">
		<td align="right" valign="top" nowrap>
			Reason (other):
		</td>
		<td valign="top">
			<textarea rows="2" cols="30" name="iOrderReasonOther" wrap="hard"><%=OrderReasonOther%></textarea>
		</td>
	</tr>
	<%
			End If
		End If
	%>
	<!-- MCD: end change -->
	
	<tr>
		<td></td>
		<td></td>
		<td></td>
		<td align="right"> 
				<a HREF="#" onclick="if (DialogWindow) {DialogWindow.close()}; window.close();return false"><img SRC="/cheminv/graphics/cancel_dialog_btn.gif" border="0" alt="Cancel"></a>
				<%if NOT isEdit then%>
					<a HREF="#" onclick="ValidateContainer('Create'); return false;"><img SRC="/cheminv/graphics/ok_dialog_btn.gif" border="0"></a>
				<%Else%>
					<a HREF="#" onclick="ValidateContainer('Edit'); return false;"><img SRC="/cheminv/graphics/ok_dialog_btn.gif" border="0"></a>
				<%End if%>		
		</td>
	</tr>
<%	
Case "Substance"
	Session("IsPopUP") = false
%>
	<input type="hidden" name="isSubstanceTab">
    <%if NOT IsEmpty(CompoundID) then %>
	<tr height="150">
		<td>
			<table border="1">
				<tr>
					<!-- Structure -->
					<td>
						<%
						if Session("isCDP") = "TRUE" and detectModernBrowser = false then
							specifier = 185
						else
							specifier = "185:gif"
						end if
						Base64DecodeDirect "ChemInv", "substances_form_group", Base64_CDX, "inv_compounds.BASE64_CDX", CompoundID, CompoundID, specifier, 130%>
					</td>
				</tr>
			</table>
		</td>
		<td valign="top">
			<table border="0" cellpadding="1" cellspacing="2">
				<tr><!-- Header Row -->
					<td colspan="4" align="center">
						&nbsp;<em><b><%=TruncateInSpan(SubstanceName, 50, "")%></b></em>
					</td>
				</tr>
				<tr>
					<!-- Row 1 Col 1-->
					<!-- for formula id must be MOLWEIGHT -->		
					<%=ShowField("Molecular Weight:", "MOLWEIGHT0", 15, "MOLWEIGHT0")%>		
					<!-- Row 1 Col 2-->
					<%=ShowField("CompoundID:", "CompoundID", 15, "")%>
				</tr>
				<tr>
					<!-- Row 2 Col 1-->
					<%=ShowField("Molecular Formula:", "FORMULA0", 15, "FORMULA0")%>
					<%Response.Write "<script languaje='javascript'> GetMolWeightAndFormula('MOLWEIGHT0', 'FORMULA0', '" & Round(ConvertBase64toMW(Mid(Base64_CDX, InStr(Base64_CDX, "VmpD"))),3) &"','" & ConvertBase64toMFormula(Mid(Base64_CDX, InStr(Base64_CDX, "VmpD"))) &"'); </script>" %>
					<!-- Row 2 Col 2-->
					<%=ShowField("CAS Number:", "CAS", 15, "")%>
				</tr>
				<tr>
					<!-- Row 3 Col 1-->
					<td></td><td></td>
					<!-- Row 3 Col 2-->
					<%=ShowField("ACX ID:", "ACX_ID", 15, "")%>
				</tr>			
			</table>
		</td>
	</tr>
    <%end if %>
    <tr>
        <td colspan="4" align="right">
            <a class="MenuLink" href="Create%20a%20new%20inventory%20substance%20and%20assign%20it%20to%20this%20container" onclick="OpenDialog('/cheminv/gui/CreateOrEditSubstance.asp', 'newDialog', 2); return false;" title="Create a new inventory substance and assign it to this container">New Substance</a>
            &nbsp;|&nbsp;
            <a class="MenuLink" HREF="Select%20an%20existing%20substance%20and%20assign%20it%20to%20this%20container" target="SubstancesFormGroup" onclick="OpenDialog('/cheminv/inputtoggle.asp?formgroup=global_substanceselect_form_group&amp;dataaction=db&amp;dbname=cheminv&amp;showbanner=false', 'newDialog', 2); return false;" title="Select an existing substance and assign it to this container">Select Substance</a>			
        </td>
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
						if Session("isCDP") = "TRUE" and detectModernBrowser = false then
							specifier = 185
						else
							specifier = "185:gif"
						end if
						Base64DecodeDirect "invreg", "base_form_group", BASE64_CDX, "Structures.BASE64_CDX", cpdID, cpdID, specifier, 130%>
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
					<!-- Row 1 Col 1-->
					<!-- for formula id must be MOLWEIGHT -->		
					<%=ShowField("Molecular Weight:", "MOLWEIGHT0", 15, "MOLWEIGHT0")%>
					<%Response.Write "<script languaje='javascript'> var mol = document.getElementById('MOLWEIGHT0'); var molValue = '" & Round(ConvertBase64toMW(Mid(Base64_CDX, InStr(Base64_CDX, "VmpD"))),3) &"'; mol.innerHTML = molValue; </script>" %>
					<!-- Row 1 Col 2-->
					<%=ShowField("RegBatchID:", "RegBatchID", 15, "")%>
					<input type="hidden" name="iRegID" value="<%=RegID%>">
					<input type="hidden" name="iBatchNumber" value="<%=BatchNumber%>">
				</tr>
				<tr>
					<!-- Row 2 Col 1-->
					<%=ShowField("Molecular Formula:", "FORMULA0", 15, "FORMULA0")%>
					<%Response.Write "<script languaje='javascript'> var formula = document.getElementById('FORMULA0'); var formulaValue = '" & ConvertBase64toMFormula(Mid(Base64_CDX, InStr(Base64_CDX, "VmpD"))) &"'; if (formulaValue.length > 15) { formula.innerHTML = formulaValue.substring(0, 15); formula.title = formulaValue; } else { formula.innerHTML = formulaValue; } </script>" %>
					<!-- Row 2 Col 2-->
					<%=ShowField("RegBatchAmount:", "RegAmount", 15, "")%>
				</tr>
				<tr>
					<!-- Row 3 Col 1-->
					<%=ShowField("NoteBook:", "NoteBook", 15, "")%>
					<!-- Row 3 Col 2-->
					<%=ShowField("Page:", "Page", 15, "")%>
				</tr>
				<tr>
					<!-- Row 4 Col 1-->
					<%=ShowField("Chemist:", "RegScientist", 15, "")%>
					<!-- Row 4 Col 2-->
					<%=ShowField("Purity:", "Purity", 15, "")%>
				</tr>		
			</table>
		</td>
	</tr>
    <tr>
        <td colspan="4" align="right">
            <a class="MenuLink" href="Create%20a%20new%20inventory%20substance%20and%20assign%20it%20to%20this%20container" onclick="OpenDialog('/cheminv/gui/CreateOrEditSubstance.asp', 'newDialog', 2); return false;" title="Create a new inventory substance and assign it to this container">New Substance</a>
            &nbsp;|&nbsp;
            <a class="MenuLink" HREF="Select%20an%20existing%20substance%20and%20assign%20it%20to%20this%20container" target="SubstancesFormGroup" onclick="OpenDialog('/cheminv/inputtoggle.asp?formgroup=global_substanceselect_form_group&amp;dataaction=db&amp;dbname=cheminv&amp;showbanner=false', 'newDialog', 2); return false;" title="Select an existing substance and assign it to this container">Select Substance</a>			
        </td>
    </tr>	
<!--<%	Case "Supplier"%>	<tr height="25">		<td align="right">			<span title="Pick an option from the list">Supplier Name:</span>		</td>		<td colspan="3">			<%= ShowSelectBox2("iSupplierID", SupplierID, "SELECT Supplier_ID AS Value, Supplier_Short_Name AS DisplayText FROM inv_suppliers ORDER BY lower(Supplier_Short_Name) ASC", 27, RepeatString(43, "&nbsp;"), "")%>        		<b>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Unknown Supplier Info:</b>			</td>	</tr>	<tr height="25">		<%=ShowInputBox("Catalog Number:", "SupplierCatNum", 15, "", False, False)%>		<%=ShowInputBox("Name:", "UnknownSupplierName", 25, "", False, False)%>	</tr>	<tr height="25">		<%=ShowInputBox("Container Cost($):", "ContainerCost", 15, "", False, False)%>		<%=ShowInputBox("Contact:", "UnknownSupplierContact", 25, "", False, False)%>	</tr>		<tr height="25">		<td align="right" valign="top" nowrap width="150">Due Date:</td><td><input type="text" name="iDueDate" size="15" value="<%=DueDate%>"><a href onclick="return PopUpDate(&quot;iDueDate&quot;,&quot;/cheminv/gui/month_view1.asp&quot;)"><img src="/cfserverasp/source/graphics/navbuttons/icon_mview.gif" height="16" width="16" border="0"></a></td>		<%=ShowInputBox("Phone#:", "UnknownSupplierPhoneNumber", 25, "", False, False)%>	</tr>	<tr height="25">		<td align=right valign=top nowrap width=0></td>	    <td><input type=text disabled size=0 style="border-width: 0;"></td>		<%=ShowInputBox("FAX#:", "UnknownSupplierFAXNumber", 25, "", False, False)%>	</tr>-->
<%
	Case "Contents"
%>
	<tr height="25">
		<%=ShowInputBox("Purity:", "Purity", 15, ShowSelectBox("iUOPID", Session("UOPIDOptionValue"),"SELECT UNIT_ID||'='||Unit_Abreviation AS Value, Unit_Name AS DisplayText FROM inv_units WHERE Unit_type_id_FK = 3 ORDER BY Unit_Name ASC"), False, False)%>
	</tr>
	<tr height="25">
		<%=ShowInputBox("Concentration:", "Concentration", 15, ShowSelectBox("iUOCID", Session("UOCIDOptionValue"),"SELECT UNIT_ID||'='||Unit_Abreviation AS Value, Unit_Name AS DisplayText FROM inv_units WHERE Unit_type_id_FK = 3 ORDER BY Unit_Name ASC"), False, False)%>
	</tr>
	<tr height="25">
		<%=ShowInputBox("Grade:", "Grade", 15, "", False, False)%>
	</tr>
	<tr height="25">
		<%'=ShowInputBox("Solvent:", "Solvent", 36, "", False, False)%>
			<td align="right" valign="top" nowrap width="150">Solvent:</td>
		<td><%=ShowSelectBox2("iSolvent" ,SolventIDFK,"SELECT solvent_id AS Value, solvent_name AS DisplayText FROM " & Application("CHEMINV_USERNAME") & ".inv_solvents ORDER BY lower(DisplayText) ASC", null, "Select a Solvent", "")%></td>
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
		<%= ShowPickList("Current user:", "iCurrentUserID", CurrentUserID, "SELECT User_ID AS Value, Last_Name||DECODE(First_Name, NULL, '', ', '||First_Name) AS DisplayText FROM CS_SECURITY.People ORDER BY lower(Last_Name) ASC")%>
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
<%
End Select
%>
	<% if sTab <> "Required" then%>
	<tr>
		<td colspan="4" align="right" height="20" valign="bottom"> 
				<a HREF="#" onclick="if (DialogWindow) {DialogWindow.close()}; window.close();return false"><img SRC="/cheminv/graphics/cancel_dialog_btn.gif" border="0" alt="Cancel"></a>
				<%if NOT isEdit then%>
					<a HREF="#" onclick="ValidateContainer('Create'); return false;"><img SRC="/cheminv/graphics/ok_dialog_btn.gif" border="0"></a>
				<%Else%>
					<a HREF="#" onclick="ValidateContainer('Edit'); return false;"><img SRC="/cheminv/graphics/ok_dialog_btn.gif" border="0"></a>
				<%End if%>		
		</td>
	</tr>
	<%end if%>
</table>	
<% if Application("ENABLE_OWNERSHIP")="TRUE" and PrincipalID="" then %>
<script language="javascript">
    //set the inital location group
    SetOwnerInfo('location');
 </script>
<%end if %>
</form>
</body>
</html>
<%If Not IsEmpty(Request("setFocus")) Then%>
<script language="JavaScript">
<!--Hide JavaScript
	var a = document.all.item("<%=Request("setFocus")%>");
	if (a!=null) {
	    document.all.<%=Request("setFocus")%>.focus();
	}
//-->
</script>
<%End If%>
