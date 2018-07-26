<%@ Language=VBScript %>
<!--#INCLUDE VIRTUAL = "/cheminv/api/apiUtils.asp"-->
<!--#INCLUDE VIRTUAL = "/cheminv/gui/guiUtils.asp"-->
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/cs_security/cs_security_login_utils_vbs.asp" -->
<%

Dim Conn
Dim RS

'if not Session("IsFirstRequest") then 
'	StoreASPSessionID()
'else
'	Session("IsFirstRequest") = false
'	Response.Write("<html><body onload=""window.location.reload()"">&nbsp;<form name=""form1""></form></body></html>")	
'end if

'Response.Write(Request.QueryString & "<br><br>")
'Response.Write(replace(Request.Form,"&","<br>") & "<br><br>")

ContainerID = Request("ContainerID")
QtyRemaining = Request("QtyRemaining")
CompoundID = Request("CompoundID")
RegBatchID = Request("RegBatchID")
RegID = Request("RegID")
BatchNumber = Request("BatchNumber")
ContainerName = Request("ContainerName")
ContainerSize = Request("ContainerSize")
ContainerStatus = Request("ContainerStatus")
PContainerID = Request("ParentContainerBarcode")

'-- Sampling fields for additional samples creation
SamplingContainerID=Request("SamplingContainerID")
SamplingUOMID=Request("SamplingUOMID")
SamplingConcentration=Request("SamplingConcentration")
SamplingUOCID=Request("SamplingUOCID")
SamplingQtyMax=Request("SamplingQtyMax")
SamplingQtyInitial=Request("SamplingQtyInitial")
SamplingCompoundID=Request("SamplingCompoundID")
SamplingRegBatchID=Request("SamplingRegBatchID")
SamplingContainerName=Request("SamplingContainerName")
SamplingRegID = Request("SamplingRegID")
SamplingBatchNumber = Request("SamplingBatchNumber")

Action = Request("Action")
pageAction = Request("Action")
ActionStep = Request("ActionStep")
SampleQty = Request("SampleQty")
OriginalQty = Request("OriginalQty")

BarcodeDescID = Request("BarcodeDescID")
if Application("ENABLE_OWNERSHIP")="TRUE" then
    LocationTypeID = Request("LocationTypeID")
   ' Response.Write BarcodeDescID &","&LocationTypeID
    PrincipalID = Request("PrincipalID")
    LocationAdmin= Request("LocationAdmin")
end if
NumContainers = Request("NumContainers")
RequestID = Request("RequestID")

AutoGen_cb = Request("AutoGen_cb")
'AutoGen_cb = "on"

if Request("AutoPrint") = "on" then AutoPrint = " checked"
if Request("DisposeOrigContainer") = "on" then DisposeOrigContainer = " checked"

'-- Set default values for Unit of Concentration, Unit of Measure and Container Type
if not IsEmpty(Application("DEFAULT_SAMPLE_UOM")) and pageAction = "new" then
	UOMAbv = Application("DEFAULT_SAMPLE_UOM")
else
	UOMAbv = Request("UOMAbv")
	if isEmpty(UOMAbv) then UOMAbv = "1=ml" 
end if
if not IsEmpty(Application("DEFAULT_SAMPLE_UOC")) and pageAction = "new"  then
	UOCAbv = Application("DEFAULT_SAMPLE_UOC")
else
	UOCAbv = Request("UOCAbv")
	if isEmpty(UOCAbv) then UOCAbv = "26=mg/ml" 
end if
if not IsEmpty(Application("DEFAULT_SAMPLE_CONTAINER_TYPE_ID")) and pageAction = "new"  then
	ContainerTypeID = Application("DEFAULT_SAMPLE_CONTAINER_TYPE_ID")
else
	ContainerTypeID = Request("ContainerTypeID")
	if isEmpty(ContainerTypeID) then ContainerTypeID = "2"
end if

if NumContainers = "" then NumContainers = 1

if Application("SAMPLE_CREATE_LIMIT") <> "" then
	DefNumSamplesLimit = Application("SAMPLE_CREATE_LIMIT")
else
	DefNumSamplesLimit = 25
end if

'-- If user has selected a rack or contents of rack, set the default location to parent of rack

if isBlank(Session("CurrentLocationID")) then Session("CurrentLocationID") = "0"


if Request("LocationID") <> "" then
	if ActionStep = "1" and Request("GetSessionLocationID") = "" then 
		LocationID = Request("LocationID")
		Session("CurrentLocationID") = LocationID
	elseif Request("LocationID") <> "" then
		LocationID = Request("LocationID")
	else
		LocationID = Session("CurrentLocationID")
	end if 
else
	LocationID = Session("CurrentLocationID")
end if

if ActionStep = "" then
	ActionStep = 1
else
	ActionStep = ActionStep + 1
end if

if ContainerID <> "" then
	titleText = "Create New Samples from the Original Container"
	instructionTextA = "Enter the number of sample types you want to create."
	instructionTextB = "Adjust the amounts in each sample."
	actionText = "Number of Sample Volume Types"
	CaptionPrefix = "Sample"
	CaptionSuffix = "Quantity (" & UOMAbv & ")"
	NamePrefix = Action	
else
	titleText = "Create New Samples"
	instructionTextA = "Enter the number of sample types you want to create."
	instructionTextB = "Adjust the amounts in each sample."
	actionText = "Number of Sample Volume Types"
	CaptionPrefix = "Sample"
	CaptionSuffix = "Quantity (" & UOMAbv & ")"
	NamePrefix = Action	
end if


%>


<html>
<head>
<title><%=Application("appTitle")%> -- <%=titleText%></title>
<script LANGUAGE="javascript" src="/cheminv/Choosecss.js"></script>
<script LANGUAGE="javascript" src="/cheminv/utils.js"></script>
<script LANGUAGE="javascript" src="/cheminv/gui/validation.js"></script>
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
<script language="JavaScript">
<!--Hide JavaScript
	window.focus();
	function setOwnership()
{
 <% if Application("ENABLE_OWNERSHIP")="TRUE" then %>
  <%if Principalid<>"" then %>
     var type="<%=Principalid%>";
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
   <% end if %>
  <%end if %>
    
}
	function Validate(){
		var bWriteError = false;
		var Action = "<%=Action%>";
		var ActionStep = "<%=ActionStep%>";

		//Populate hidden variables
		!document.form1.iContainerName ? document.form1.ContainerName.value="<%=Server.URLEncode(ContainerName)%>" : document.form1.ContainerName.value = document.form1.iContainerName.value;
		!document.form1.iContainerTypeID ? document.form1.ContainerTypeID.value="<%=ContainerTypeID%>" : document.form1.ContainerTypeID.value = document.form1.iContainerTypeID.value;
		!document.form1.iContainerSize ? document.form1.ContainerSize.value="<%=ContainerSize%>" : document.form1.ContainerSize.value = document.form1.iContainerSize.value;
		!document.form1.iContainerStatus ? document.form1.ContainerStatus.value="<%=ContainerStatus%>" : document.form1.ContainerStatus.value = document.form1.iContainerStatus.value;
		!document.form1.iBarcodeDescID ? document.form1.BarcodeDescID.value="<%=BarcodeDescID%>" : document.form1.BarcodeDescID.value = document.form1.iBarcodeDescID.value;
		!document.form1.iNumContainers ? document.form1.NumContainers.value="<%=NumContainers%>" : document.form1.NumContainers.value = document.form1.iNumContainers.value;
		!document.form1.iSampleQty ? document.form1.SampleQty.value="<%=SampleQty%>" : document.form1.SampleQty.value = document.form1.iSampleQty.value;
		<% if Application("ENABLE_OWNERSHIP")="TRUE" then %>
		!document.form1.Ownershiplst ? document.form1.PrincipalID.value="" :  document.form1.PrincipalID.value=document.form1.Ownershiplst.value;
		<% end if %>
		<% if Action = "new" then %>
		!document.form1.iCompoundID ? document.form1.CompoundID.value="<%=CompoundID%>" : document.form1.CompoundID.value = document.form1.iCompoundID.value;
		!document.form1.iRegID ? document.form1.RegID.value="<%=RegID%>" : document.form1.RegID.value = document.form1.iRegID.value;
		!document.form1.iBatchNumber ? document.form1.BatchNumber.value="<%=BatchNumber%>" : document.form1.BatchNumber.value = document.form1.iBatchNumber.value;
		<% end if %>

		var NumContainers = document.form1.NumContainers.value;
		var ContainerSize = document.form1.ContainerSize.value;
		var SampleQty = document.form1.SampleQty.value;
		<% if Action = "sample" then %>
		var OriginalQty = Number(document.form1.iOriginalQty.value);
		<% end if %>
		var errmsg = "Please fix the following problems:\r";

		//LocationID is required
		if (document.form1.LocationID.value.length == 0) {
			errmsg = errmsg + "- Location ID is required.\r";
			bWriteError = true;
		}
		else{
			// LocationID must be a positive number
			if (document.form1.LocationID.value == 0){
				errmsg = errmsg + "- Cannot create container at the root location.\r";
				bWriteError = true;
			}
			// LocationID must be a positive number
			if (!isPositiveNumber(document.form1.LocationID.value)&&(document.form1.LocationID.value != 0)){
				errmsg = errmsg + "- Location ID must be a positive number.\r";
				bWriteError = true;
			}
		}	

		<% if Application("RACKS_ENABLED") then %>
		// Validation for Rack assignment
		<% if ActionStep <> "1" then %>
		var numSamples = 0;
		<% end if%>
		// Destination cannot be assign to a rack parent
		/*
		if (document.form1.AssignToRack.checked == false){
			if (ValidateLocationIDType(document.form1.LocationID.value) == 1) {
				//errmsg = errmsg + "- Destination can not be a Rack unless \"Assign to Rack\" is checked. Please check \"Assign to Rack\" or choose a different location.\r";
				//bWriteError = true;
				document.form1.AssignToRack.checked = true;
			}
		}
		*/
		<% end if %>

		// Container Name Required
		if (document.form1.ContainerName.value.length == 0) {
			errmsg = errmsg + "- Container Name is required.\r";
			bWriteError = true;
		} else if (document.form1.ContainerName.value.length > 255) {
			errmsg = errmsg + "- Container Name must be less then 255 characters.\r";
		}

		// Barcode Desc Required
		if (document.form1.BarcodeDescID.value.length == 0) {
			errmsg = errmsg + "- Barcode Description is required.\r";
			bWriteError = true;
		}

		// Only allow non-Auto barcodes if creating single sample volume type
		/*
		if (document.form1.AutoGen_cb) {
			if (!document.form1.AutoGen_cb.checked) {
				if (document.form1.iNumContainers.value > 1) {
					errmsg = errmsg + "- You can only manually assign a barcode to one sample.\r";
					bWriteError = true;
				}
			}
		}*/
				
		// Number of Containers is required
		if (document.form1.NumContainers.value.length == 0) {
			errmsg = errmsg + "- <%=actionText%> is required.\r";
			bWriteError = true;
		}
		else{
			// Number of Containers must be a number
			if (!isPosLongInteger(NumContainers)){
				errmsg = errmsg + "- <%=actionText%> must be a positive integer.\r";
				bWriteError = true;
			}
			else if (NumContainers < 1){
				errmsg = errmsg + "- <%=actionText%> must be a positive number.\r";
				bWriteError = true;
			}
			else if (NumContainers > <%=DefNumSamplesLimit%>){
				errmsg = errmsg + "- <%=actionText%> must be less or equal to <%=DefNumSamplesLimit%>.\r";
				bWriteError = true;
			}
		}
		
		// Container Admin is required
	<% if Application("ENABLE_OWNERSHIP")="TRUE" then %>
		<%if Principalid="" then%> 
               if (document.form1.PrincipalID.value.length==0){
		                errmsg = errmsg + "- Container Admin is required.\r";
				        bWriteError = true;
		    }
		    <% 
		    else%>
		    document.form1.PrincipalID.value='<%=Principalid%>';
		   <%end if %>
		   
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
       if (!bWriteError && document.form1.LocationAdmin.value!='' && document.form1.LocationAdmin.value !=document.form1.PrincipalID.value && ActionStep == 2)
        {
             if (confirm("- The Location Admin and Container Admin are not the same,\r Do you really want to continue?")!=true)
               {     
                 return;
                }
        }
    <% end if %>
		
		// Quantity per Sample is required
		if (ActionStep == 1 && Action == "sample") {
			if (document.form1.iSampleQty.value.length == 0) {
				errmsg = errmsg + "- Quantity per Sample is required.\r";
				bWriteError = true;
			// Quantity per Sample must be a number greater than zero
			} else if(isNumber(document.form1.iSampleQty.value)==false) {
				document.form1.iSampleQty.focus();
				document.form1.iSampleQty.select();
				errmsg = errmsg + "- Quantity per Sample must be a number.\r";
				bWriteError = true;
			} else if(document.form1.iSampleQty.value == 0 ){
				document.form1.iSampleQty.focus();
				document.form1.iSampleQty.select();
				errmsg = errmsg + "- Quantity per Sample must be a number.\r";
				bWriteError = true;
			} else {
				if ((eval(document.form1.iSampleQty.value)*NumContainers) > eval(document.form1.iOriginalQty.value)){
					errmsg = errmsg + "- The number of Samples and Qty per sample cannot exceed the Original Qty.\r";
					bWriteError = true;
				}
			}
		}
		
		
		//validation for the form with the new quantity fields
		if (ActionStep == 2) {
			var varSampleTotal = 0;
			var numContainers = 0;
			//alert(document.form1.iNumCopies.length);
			if (document.form1.iNumCopies.length > 1) {
				for (i=0; i < document.form1.iNumCopies.length; i++) {
					if (!isPosLongInteger(document.form1.iNumCopies[i].value)) {
						errmsg = errmsg + "- All Num Copies values must be positive integers.\r";
						bWriteError = true;
						break;
					} else if (document.form1.iNumCopies[i].value.length == 0){
						errmsg = errmsg + "- All Num Copies values are required.\r";
						bWriteError = true;
						break;
					} else if (document.form1.iNumCopies[i].value < 1){
						errmsg = errmsg + "- All Num Copies values must be valid positive numbers.\r";
						bWriteError = true;
						break;	
					} else if (document.form1.iNumCopies[i].value > <%=DefNumSamplesLimit%>){
						errmsg = errmsg + "- All Num Copies must be less or equal to <%=DefNumSamplesLimit%>.\r";
						bWriteError = true;
						break;
					}
					numContainers = numContainers + parseInt(document.form1.iNumCopies[i].value);

					<% if AutoGen_cb = "" then %>
					if (document.form1.iBarcode[i].value.length == 0){
						errmsg = errmsg + "- Barcode for Sample " + (i+1) + " must be entered.\r";
						bWriteError = true;
						break;
					} else if (document.form1.iBarcode[i].value.length > 0 && document.form1.iNumCopies[i].value > 1){
						errmsg = errmsg + "- You can only assign one barcode to each unique sample.\rPlease change the number of copies to \r1 or to create multiple copies, click \"Back\" and increase the number of samples.\r";
						bWriteError = true;
						break;
					} else if (ValidateBarcode(document.form1.iBarcode[i].value)==1){
						errmsg = errmsg + "- The barcode for Sample " + (i+1) + " already exists. Please enter a different barcode.\r";
						bWriteError = true;
						document.form1.iBarcode[i].value = '';
					}
					<% end if %>
					if (!isNumber(document.form1.iSampleQty[i].value)) {
						errmsg = errmsg + "- All Qty values must be numbers.\r";
						bWriteError = true;
						break;
					} else if (document.form1.iSampleQty[i].value.length == 0){
						errmsg = errmsg + "- All Qty values are required.\r";
						bWriteError = true;
						break;
					}
					if (!isNumber(document.form1.iContainerSize[i].value)) {
						errmsg = errmsg + "- All Size values must be numbers.\r";
						bWriteError = true;
						break;
					} else if (document.form1.iContainerSize[i].value.length==0){
						errmsg = errmsg + "- All Size values are required.\r";
						bWriteError = true;
						break;
					}
					if (!isNumber(document.form1.iConc[i].value)) {
						errmsg = errmsg + "- All Conc values must be numbers.\r";
						bWriteError = true;
						break;
					}
					//ContainerSize if present should not have comma
						var m = document.form1.iContainerSize[i].value.toString();		
					if(m.indexOf(",") != -1){
						errmsg = errmsg + "- ContainerSize has wrong decimal operator.\r";
						bWriteError = true;
						break;
						}
					<%if lcase(Application("SAMPLE_REQUIRE_CONCENTRATION")) = "true" then %>
					if (document.form1.iConc[i].value.length == 0) {
						errmsg = errmsg + "- All Conc values are required.\r";
						bWriteError = true;
						break;
					}
					<% end if %>
					if (eval(document.form1.iSampleQty[i].value) > parseInt(document.form1.iContainerSize[i].value)) {
						errmsg = errmsg + "- All Qty must be less than the Size defined.\r";
						bWriteError = true;
						break;
					}
					varSampleTotal = eval(varSampleTotal) + (eval(document.form1.iSampleQty[i].value)*parseInt(document.form1.iNumCopies[i].value));
				}
			} else {
					if (!isPosLongInteger(document.form1.iNumCopies.value)) {
						errmsg = errmsg + "- All Num Copies values must be positive integers.\r";
						bWriteError = true;
					} else if (document.form1.iNumCopies.value.length == 0){
						errmsg = errmsg + "- All Num Copies values are required.\r";
						bWriteError = true;
					} else if (document.form1.iNumCopies.value < 1){
						errmsg = errmsg + "- Num Copies value must be a valid positive number.\r";
						bWriteError = true;
					} else if (document.form1.iNumCopies.value > <%=DefNumSamplesLimit%>){
						errmsg = errmsg + "- All Num Copies must be less or equal to <%=DefNumSamplesLimit%>.\r";
						bWriteError = true;
					}
					if (bWriteError == false) {
					    numContainers = document.form1.iNumCopies.value;
                    }					    
					<% if AutoGen_cb = "" then %>
					if (document.form1.iBarcode.value.length == 0){
						errmsg = errmsg + "- Barcode for Sample 1 must be entered.\r";
						bWriteError = true;
					} else if (document.form1.iBarcode.value.length > 0 && document.form1.iNumCopies.value > 1){
						errmsg = errmsg + "- You can only assign one barcode to each unique sample.\rTo create multiple copies, click \"Back\" and increase the number of samples.\r";
						bWriteError = true;
					} else if (ValidateBarcode(document.form1.iBarcode.value)==1){
						errmsg = errmsg + "- The barcode for Sample 1 already exists. Please enter a different barcode.\r";
						bWriteError = true;
						document.form1.iBarcode.value = '';
					}
					<% end if %>
					if (!isNumber(document.form1.iSampleQty.value)) {
						errmsg = errmsg + "- All Qty values must be numbers.\r";
						bWriteError = true;
					} else if (document.form1.iSampleQty.value.length == 0){
						errmsg = errmsg + "- All Qty values are required.\r";
						bWriteError = true;
					}
					if (!isNumber(document.form1.iContainerSize.value)) {
						errmsg = errmsg + "- All Container Size values must be numbers.\r";
						bWriteError = true;
					} else if (document.form1.iContainerSize.value.length==0){
						errmsg = errmsg + "- All Container Size values are required.\r";
						bWriteError = true;
					}
					if (!isNumber(document.form1.iConc.value) || document.form1.iConc.value.length == 0) {
						errmsg = errmsg + "- All Conc values must be numbers.\r";
						bWriteError = true;
					}
					//ContainerSize if present should not have comma
					var m = document.form1.iContainerSize.value.toString();		
					if(m.indexOf(",") != -1){
						errmsg = errmsg + "- ContainerSize has wrong decimal operator.\r";
						bWriteError = true;
					}
					<%if lcase(Application("SAMPLE_REQUIRE_CONCENTRATION")) = "true" then %>
					if (document.form1.iConc.length == 0) {
						errmsg = errmsg + "- All Conc values are required.\r";
						bWriteError = true;
					}
					<% end if %>
					
					if (eval(document.form1.iSampleQty.value) > parseInt(document.form1.iContainerSize.value)) {
						errmsg = errmsg + "- All Qty must be less than the Size defined.\r";
						bWriteError = true;
					}
					varSampleTotal = eval(varSampleTotal) + (eval(document.form1.iSampleQty.value)*parseInt(document.form1.iNumCopies.value));
			}
			
		    // determine whether there are enough open rack positions
		    if (document.form1.isRack.value == "1") {
		        if (AreEnoughRackPositions(numContainers,document.form1.LocationID.value) == false) {
				    errmsg = errmsg + "- There are not enough open positions from the selected start position\rin the rack(s) you have selected.\r";
				    bWriteError = true;
		        }
		    }
			//alert(numContainers);
			//bWriteError = true;
			
			<% if Action = "sample" then %>	

			if (varSampleTotal > OriginalQty) {
				errmsg = errmsg + "- The total sample quantity for the new samples cannot\rexceed the original sample quantity.\r";
				bWriteError = true;
			}
			<% end if %>
		}

		// if compoundID is present must be a positive number and a valid compoundID
		if (document.form1.RegBatchID.value.length == 0 && document.form1.iCompoundID.value.length > 0) {
			if (!isPositiveNumber(document.form1.iCompoundID.value) || document.form1.iCompoundID.value < 1){
				errmsg = errmsg + "- Compound ID must be a positive number.";
				bWriteError = true;
			}
			else {
				if (IsValidCompoundID(document.form1.iCompoundID.value, false)!=1) {
					errmsg = errmsg + "- The Compound ID you have selected is not valid.\r";
					bWriteError = true;
				}
			}
		}

//     CSBR-149793 - JHS - This is an obsolete methodology. This has to have been broken for a while.  With commenting this and adding the new lines
//     I was able to successfully complete the workflow.		
//		if (document.form1.RegBatchID) {
//			if (document.form1.RegBatchID.value.length > 0){
//				var tmpRegBatchID = document.form1.RegBatchID.value;
//				//var temp = RegBatchId.split("-");
//				var dash = tmpRegBatchID.lastIndexOf("-");
//				if (dash == -1){
//					alert("Registry Batch ID should be entered as RegNumber-BatchNumber");
//					return false;
//				}
//				var RegNumber = tmpRegBatchID.substring(0, dash);
//				var BatchNumber = tmpRegBatchID.substring(dash+1,tmpRegBatchID.length);
//				if (BatchNumber == "") BatchNumber = 0 
//				var strURL = serverType + serverName + "/cheminv/api/GetBatchInfo.asp?RegNumber=" + RegNumber + "&BatchNumber=" + BatchNumber;
//				var httpResponse = JsHTTPGet(strURL)
//				if (httpResponse.length == 0) {
//					errmsg = errmsg + "- The Registry Batch ID " + tmpRegBatchID + " is not valid. Please select a valid substance.\r";
//					bWriteError = true;
//					document.form1.RegBatchID.value = '';
//				}
//				
//			}
//		}

        if (document.form1.RegBatchID) {
			if (document.form1.RegBatchID.value.length > 0) {
		        var strURL = serverType + serverName + "/cheminv/api/GetRegistrationAttributes.asp?Action=GETREGATTRIBUTES&regbatchid=" + encodeURIComponent(document.form1.RegBatchID.value) + "&tempCsUserName=" + document.form1.tempCsUserName.value + "&tempCsUserID=" + document.form1.tempCsUserID.value;
		        var httpResponse = JsHTTPGet(strURL)
			    if (httpResponse.length == 0){
    			    alert("no bueno");
			    }
			}
		}

		<% if ActionStep = 2 and Action = "sample" then %>
		// Validate the Dispose sample buton	
		if (!document.form1.DisposeOrigContainer.checked) {
			bConfirmWarning = true;
			warningmsg = "You have chosen not to dispose the original sample. Do you want to continue?";
			bConfirmWarning = confirm(warningmsg);
			if (!bConfirmWarning) return false;
		}
		<% end if %>
		//bWriteError = true;
		if (bWriteError){
			alert(errmsg);
			return false;
		}
		else{
		<% if Action = "new" then %>
			if (document.form1.CompoundID.value.length == 0 && document.form1.RegBatchID.value.length == 0) {
				<% if ActionStep = 2 and Application("WARN_FOR_NO_COMPOUND") then %>
				bContinue = confirm("- No compound has been assigned to this container.\rDo you really want to create a container with an associated compound?\r");
				<% else %>
				bContinue = true;
				<% end if %>
				if (bContinue) {
					document.form1.submit();
					return true;
				} else {
					return false;
				}
			} else {
				document.form1.submit();
				return true;
			}
		<% else %>
			document.form1.submit();
			return true;
		<% end if %>
		}
	}
<% if ActionStep = 2 and Action = "sample" then %>	
function calcQtyRemaining(item) {

	var NumContainers = document.form1.NumContainers.value;
	var OriginalQty = document.form1.iOriginalQty.value;
	var bBreak = false;
	var totNewQty = 0;
	
	if (NumContainers > 1) {

		for (i=0; i < NumContainers; i++) {

			if (isNumber(document.form1.iSampleQty[i].value) && document.form1.iSampleQty[i].value > 0) {
				/*if (IsWholeNumber(document.form1.iNumCopies[i].value) == false){
					alert("- The value Num Copies must be a whole number.\r");
					document.form1.iNumCopies[i].value = '';
					document.form1.iNumCopies[i].focus();
					document.form1.iNumCopies[i].select();
					document.form1.iSampleQty[i].value = <%=SampleQty%>;
					bBreak = true;
				} else */
				if (isNumber(document.form1.iNumCopies[i].value) && document.form1.iNumCopies[i].value > 0 && document.form1.iNumCopies[i].value < 26) {
					totNewQty = totNewQty + (eval(document.form1.iSampleQty[i].value)*parseInt(document.form1.iNumCopies[i].value));
					if (totNewQty > OriginalQty){
						alert("- The total Qty for the samples cannot exceed the original quantity.\r");
						document.form1.iNumCopies[i].value = '';
						document.form1.iSampleQty[i].value = <%=SampleQty%>;
						bBreak = true;
					}
				} else {
					if (document.form1.iSampleQty[i].value.length > 0 && item == (i+1)) {
						document.form1.iNumCopies[i].value = '';
						document.form1.iNumCopies[i].focus();
						document.form1.iNumCopies[i].select();
						alert("- The value Num Copies must be a positive number less then 25.\r");
						bBreak = true;
					}
				}
			} else {
				alert("- The value Qty per sample must be a positive number greater than zero.\r");
				document.form1.iSampleQty[i].focus();
				document.form1.iSampleQty[i].select();
				document.form1.iSampleQty[i].value = <%=SampleQty%>;
				bBreak = true;
			}
			if (bBreak == true) break;
		}
	} else {

		if (isNumber(document.form1.iSampleQty.value) && document.form1.iSampleQty.value > 0) {

			if (isNumber(document.form1.iNumCopies.value) && document.form1.iNumCopies.value > 0 && document.form1.iNumCopies.value < 26) {

				totNewQty = totNewQty + (eval(document.form1.iSampleQty.value)*eval(document.form1.iNumCopies.value));

				if (totNewQty > OriginalQty){
					alert("- The total Qty for the samples cannot exceed the original quantity.\r");
					document.form1.iNumCopies.value = '';
					document.form1.iSampleQty.value = <%=SampleQty%>;
				}
			} else {

				if (document.form1.iSampleQty.value.lenth > 0) {
					document.form1.iNumCopies.value = '';
					document.form1.iNumCopies.focus();
					document.form1.iNumCopies.select();
					alert("- The value Num Copies must be a positive number less then 25.\r");
				}
			}
		} else {

			alert("- The value Qty per sample must be a positive number greater than zero.\r");
			document.form1.iSampleQty.focus();
			document.form1.iSampleQty.select();
			document.form1.iSampleQty.value = <%=SampleQty%>;
		}	
	}
	if (totNewQty > OriginalQty && bBreak == false){
		alert("- The total Qty for the samples cannot exceed the original quantity.\r");
	} else {
		if (bBreak == true){
			//Math.round(eval(qtySelected)*100)/100
			//alert(OriginalQty + ":" + NumContainers + ":" + "<%=SampleQty%>");
			tmpQtyRemaining = Math.round(eval(OriginalQty-(NumContainers*<%=SampleQty%>))*100)/100;
			document.form1.QtyRemaining.value = tmpQtyRemaining;
		} else {
			tmpQtyRemaining = Math.round(eval(OriginalQty-totNewQty)*100)/100;
			document.form1.QtyRemaining.value = tmpQtyRemaining;
		}
	}
	
}
<% end if %>
	
function FormatNumber(num, decimalNum, bolLeadingZero, bolParens)
   /* IN - num:            the number to be formatted
           decimalNum:     the number of decimals after the digit
           bolLeadingZero: true / false to use leading zero
           bolParens:      true / false to use parenthesis for - num
      RETVAL - formatted number
   */
   {
       var tmpNum = num;
       // Return the right number of decimal places
       tmpNum *= Math.pow(10,decimalNum);
       tmpNum = Math.floor(tmpNum);
       tmpNum /= Math.pow(10,decimalNum);
       var tmpStr = new String(tmpNum);
       // See if we need to hack off a leading zero or not
       if (!bolLeadingZero && num < 1 && num > -1 && num !=0)
           if (num > 0)
               tmpStr = tmpStr.substring(1,tmpStr.length);
           else
               // Take out the minus sign out (start at 2)
               tmpStr = "-" + tmpStr.substring(2,tmpStr.length);                        
       // See if we need to put parenthesis around the number
       if (bolParens && num < 0)
           tmpStr = "(" + tmpStr.substring(1,tmpStr.length) + ")";
       return tmpStr;
   }	
   
	function validateLocation(LocationID){
		CurrLocationID = 0;
		CurrLocationID = <%=LocationID%>;
		if (document.form1.LocationID.value != CurrLocationID){
			
			document.form1.ActionStep.value = "1";
			<% if Action = "sample" then %>
			document.form1.QtyRemaining.value=document.form1.iOriginalQty.value;
			<% end if %>
			document.form1.action = "CreateContainerSample.asp";
			document.form1.submit();
		}
	}
	
   
	function setDefaultStartingPosition(){
		if (eval(document.form1.iNumContainers.value) == 1){
			selectRackDefaults();
		}
	}   

	function OpenSearchDialog() {
		var numSamples = 0;
		var contSize = 0;
		if (parseInt(document.form1.iNumContainers.value) > 1) {
			for (j=0; j<document.form1.iNumContainers.value; j++){
				if (document.form1.iNumCopies[j].value.length == 0 || !isNumber(document.form1.iNumCopies[j].value) || document.form1.iNumCopies[j].value == 0) {
					errmsg = errmsg + "Num Copies for Sample " + (j+1) + " must be a positive number greater than 1.\r\r";
					bWriteError = true;
				}else{
					numSamples = numSamples + parseInt(document.form1.iNumCopies[j].value);
				}
			}
			contSize = document.form1.iContainerSize[0].value;
		} else {
			if (document.form1.iNumCopies.value.length == 0 || !isNumber(document.form1.iNumCopies.value) || document.form1.iNumCopies.value == 0) {
				errmsg = errmsg + "Num Copies for Sample 1 must be a positive number greater than 1.\r\r";
				bWriteError = true;
			}else{
				numSamples = numSamples + parseInt(document.form1.iNumCopies.value);
				contSize = document.form1.iContainerSize.value;
			}
		}
		OpenDialog('/cheminv/gui/ManageRacks.asp?numSamples='+numSamples+'&contSize='+contSize,'SuggestRacks',1);
	}
	
	<% if Action = "sample" then %>
	function GoBack() {
		document.form1.action='/cheminv/gui/CreateContainerSample.asp?action=sample&ActionStep=';
		document.form1.ContainerName.value = document.form1.ContainerName.value;
		document.form1.iNumContainers.value = document.form1.NumContainers.value;
		document.form1.iOriginalQty.value = document.form1.OriginalQty.value;
		document.form1.ContainerID.value = document.form1.ContainerID.value;
		document.form1.iSampleQty.value = document.form1.SampleQty.value;
		document.form1.submit();
	}
	<% end if %>
	
   function SetOwnerShipCode()
   {
        if(document.form1.PrincipalID)
        {
            document.form1.PrincipalID.value=document.form1.Ownershiplst.value;
        }        
		document.form1.ActionStep.value='';
   }
-->
</script>
</head>
<body onload="setOwnership();">
<center>
<%if ActionStep = 2 then%>
<form name="form1" action="../gui/CreateContainerSample_action.asp" method="POST" onsubmit="return Validate('True','<%=Action%>');">
<input TYPE="hidden" NAME="AutoGen_cb" Value="<%=AutoGen_cb%>">
<%else%>
<form name="form1" action="../gui/CreateContainerSample.asp" method="POST" onsubmit="return Validate()">
<%end if%>

<input type="hidden" name="UOMAbv" value="<%=UOMAbv%>">
<input type="hidden" name="RequestID" value="<%=RequestID%>">
<input type="hidden" name="ContainerID" value="<%=ContainerID%>">
<input type="hidden" name="PContainerID" value="<%=PContainerID%>">
<input type="hidden" name="ContainerName" value="<%=ContainerName%>">
<input type="hidden" name="ContainerTypeID" value>
<input type="hidden" name="ContainerSize" value="<%=ContainerSize%>">
<input type="hidden" name="ContainerStatus" value="<%=ContainerStatus%>">
<input type="hidden" name="BarcodeDescID" value>
<input type="hidden" name="RegID" value="<%=RegID%>">
<input TYPE="hidden" NAME="tempCsUserName" id="tempCsUserName" Value="<%=Session("UserName" & "cheminv")%>" >
<input TYPE="hidden" NAME="tempCsUserID" id="tempCsUserID" Value="<%=Server.URLEncode(CryptVBS(Session("UserID" & "cheminv"),"ChemInv\API\GetBatchInfo.asp"))%>">
<% if ActionStep = "2" then %>
	<input type="hidden" name="OriginalQty" value="<%=QtyRemaining%>">
<% else %>
	<input type="hidden" name="OriginalQty" value="<%=OriginalQty%>">
<% end if %>
<input type="hidden" name="BatchNumber" value="<%=BatchNumber%>">
<input type="hidden" name="NumContainers" value="<%=NumContainers%>">
<input type="hidden" name="NewNum" value>
<input type="hidden" name="SampleQty" value="<%=SampleQty%>">
<input type="hidden" name="CompoundID" value="<%=CompoundID%>">
<input type="hidden" name="Action" value="<%=Action%>">
<input type="hidden" name="ActionStep" value="<%=ActionStep%>">
<input type="hidden" name="GetData" value="db">
<input type="hidden" name="DateCertified" value="<%=Session("DateCertified")%>">
<input type="hidden" name="AutoGen" value="<%=AutoGen%>">
<input type="hidden" name="GetSessionLocationID" value>

<input type="hidden" name="SamplingContainerID" value="<%=SamplingContainerID%>">
<input type="hidden" name="SamplingUOMID" value="<%=SamplingUOMID%>">
<input type="hidden" name="SamplingConcentration" value="<%=SamplingConcentration%>">
<input type="hidden" name="SamplingUOCID" value="<%=SamplingUOCID%>">
<input type="hidden" name="SamplingQtyMax" value="<%=SamplingQtyMax%>">
<input type="hidden" name="SamplingQtyInitial" value="<%=SamplingQtyInitial%>">
<input type="hidden" name="SamplingCompoundID" value="<%=SamplingCompoundID%>">
<input type="hidden" name="SamplingRegBatchID" value="<%=SamplingRegBatchID%>">
<input type="hidden" name="SamplingContainerName" value="<%=SamplingContainerName%>">
<% if Application("ENABLE_OWNERSHIP")="TRUE" then %>
<input TYPE="hidden" NAME="OwnerShipGroupList" id="OwnerShipGroupList" Value="<%=GetOwnerShipGroupList()%>">
<input TYPE="hidden" NAME="OwnerShipUserList" id="OwnerShipUserList" Value="<%=GetOwnerShipUserList()%>">
<input TYPE="hidden" NAME="PrincipalID" id="PrincipalID" Value>
<input TYPE="hidden" NAME="LocationAdmin" id="LocationAdmin" Value="<%=LocationAdmin%>">
<% end if %>
<table border="0">
	<tr>
		<td colspan="2">
			<span class="GuiFeedback">
			<%
			if pageAction = "new" then
				Response.Write instructionTextA
			else
				Response.Write instructionTextB
			end if
			%>
			</span><br><br>
		</td>
	</tr>
	
	<tr height="25">
		<td align="right" valign="top" nowrap>
			<span class="required">Sample Location ID:</span>
		</td>
		<td>
        <%  if Application("ENABLE_OWNERSHIP")="TRUE" then
                authorityFunction = "SetOwnerInfo('location');"
            else
                authorityFunction= ""
            end if %>
			&nbsp;<%=GetBarcodeIcon()%>&nbsp;<%ShowLocationPicker3 "document.form1", "LocationID", "lpLocationBarCode", "lpLocationName", 10, 20, false,authorityFunction%> 
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
	        <% end if %>

	<% if pageAction = "new" then %>
	<tr>
		<td>&nbsp;</td><td>
			<%if Session("INV_MANAGE_SUBSTANCES" & "cheminv") then%>
			<a class="MenuLink" href="Create%20a%20new%20inventory%20substance%20and%20assign%20it%20to%20this%20container" onclick="OpenDialog('/cheminv/gui/CreateOrEditSubstance.asp', 'SubsManager', 2); return false;" title="Create a new inventory substance and assign it to this container">New Substance</a>
			&nbsp;|&nbsp;
			<%end if%>
			<a class="MenuLink" HREF="Select%20an%20existing%20substance%20and%20assign%20it%20to%20this%20container" target="SubstancesFormGroup" onclick="OpenDialog('/cheminv/inputtoggle.asp?formgroup=global_substanceselect_form_group&amp;dataaction=db&amp;dbname=cheminv&amp;showbanner=false', 'SubsManager', 2); return false;" title="Select an existing substance and assign it to this container">Select Substance</a>
		</td>
	</tr>
	<tr>
		<td align="right">Registry Batch ID:</td><td><input type="text" Size="15" name="RegBatchID" value="<%=RegBatchID%>" onchange="GetRegIDFromRegNum(this.value); iCompoundID.value=''; CompoundID.value='';"></td>
		<input type="hidden" name="iRegID" value="<%=RegID%>">
		<input type="hidden" name="iBatchNumber" value="<%=BatchNumber%>">
		<input type="hidden" name="NewRegID">
		<input type="hidden" name="NewBatchNumber">	
	</tr>
	<tr>
		<%if Application("RegServerName") <> "NULL" then%>
			<td align="right">Compound ID:</td><td><input type="text" Size="15" name="iCompoundID" value="<%=CompoundID%>" onchange="NewCompoundID.value= this.value; RegBatchID.value=''; iRegID.value=''; iBatchNumber.value='';"></td>
		<%else%>
			<td align="right">Compound ID:</td><td><input type="text" Size="15" name="iCompoundID" value="<%=CompoundID%>" onchange="NewCompoundID.value= this.value;"></td>
		<%End if%>
		<input Type="hidden" name="NewCompoundID">	
	</tr>
	<% end if %>
	<tr>
		<td align="right" nowrap>
			<span class="required">Container Name:</span>
		</td>
		<td><input TYPE="text" SIZE="20" Maxlength="50" NAME="iContainerName" VALUE="<%=ContainerName%>"></td>
	</tr>

	<% if action = "new" and ActionStep = "1" then %>
		<input type="hidden" name="iContainerSize" value>
	<% end if %>
	
	<%if pageAction = "sample" then%>
	<tr>
		<td align="right" nowrap>Original Quantity (<%=UOMAbv%>):</td>
		<td><input type="text" size="10" maxlength="50" name="iOriginalQty" class="readOnly" VALUE="<% if OriginalQty <> "" then Response.Write(OriginalQty) else Response.write(QtyRemaining) end if %>" READONLY></td>
	</tr>
	<%end if%>
	
	<tr>
		<td align="right" nowrap valign="top"><span class="required">Barcode Description:</span></td>
		<td>
			<% if AutoGen_cb = "on" or ActionStep = "1" then %>
				<%=ShowSelectBox2("iBarcodeDescID", BarcodeDescID,"SELECT barcode_desc_id AS Value, barcode_desc_name AS Displaytext FROM inv_barcode_desc ORDER BY lower(DisplayText) ASC", null,"","")%><br />
			<% else %>
				Scan/Enter your barcode below
			<% end if %>
			<% if ActionStep = "1" then %>
			<input type="checkbox" name="AutoGen_cb" <% if AutoGen_cb = "on" or ActionStep="1" then Response.write(" checked") end if%>>Autogenerate Barcode ID
			<% end if %>
		</td>
	</tr>
	
	<tr>
		<td align="right" nowrap><span class="required"><%=actionText%>:</span></td>
		<td>
		<%if ActionStep = 1 then%>
			<input type="text" size="10" maxlength="50" name="iNumContainers" value="<%=NumContainers%>">
		<%else%>
			<input type="text" size="10" maxlength="50" name="iNumContainers" class="readOnly" VALUE="<%=NumContainers%>" READONLY>
		<%end if%>
		</td>
	</tr>
	<% if Application("ENABLE_OWNERSHIP")="TRUE" then %>
	<tr height="25">
		<td align=right> <span title="Pick an option from the list" class="required">Container Admin:</span></td>
		<td align=left>
		<table border=.5>
		<tr><td><input type="Radio" name="Ownership" id="User_cb" onclick="SetOwnerInfo('user');"/>by user</td>
		<td><input type="Radio" name="Ownership"  id="Group_cb" onclick="SetOwnerInfo('group');" />by Group</td></tr>
		<tr><td colspan="2"><SELECT id="Ownershiplst" ><OPTION></OPTION></SELECT></td></tr></table></td>
	</tr>
	<% end if %>
	<% if ActionStep = 1 then %>
	<tr>
		<td align="right">Barcodes</td><td><input type="checkbox" name="AutoPrint" <%=AutoPrint%>>Print barcodes immediately<br></td>
	</tr>
	<% if Action = "sample" then %>
	<tr height="25">
		<td align="right">Dispose Original:</td><td>
			<input type="checkbox" name="DisposeOrigContainer" <%=DisposeOrigContainer%>>Dispose remaining amount?
		</td>
	</tr>
	<% end if %>
	
	<% end if %>
	
	<%if action = "sample" and actionstep = "1" then %>
	<tr>
		<td align="right" nowrap><span class="required">Quantity per Sample<% if action <> "new" then Response.Write(" (" & UOMAbv & ")") end if%>:</span></td>
		<td><input TYPE="text" SIZE="10" Maxlength="50" NAME="iSampleQty" value="<%=SampleQty%>"></td>
	</tr>
	<% elseif ActionStep <> "2" then %>
	<input type="hidden" name="iSampleQty" value>
	<% end if %>
	
	<%if action = "sample" and ActionStep = "2" then%>
	<tr>
		<td align="right" nowrap>Quantity Remaining (<%=UOMAbv%>):</td>
		<% 
		if OriginalQty <> "" then
			valQtyRemaining = OriginalQty
		else
			valQtyRemaining = QtyRemaining
		end if
		%>
		
		<td><input TYPE="text" SIZE="10" Maxlength="50" NAME="QtyRemaining" VALUE="<%=(valQtyRemaining - (NumContainers*SampleQty))%>"></td>
	</tr>
	<% elseif action = "sample" then %>
		<input type="hidden" name="QtyRemaining" value="<% if OriginalQty <> "" then Response.Write(OriginalQty) else Response.Write(QtyRemaining) end if %>">
	<%end if%>

	<% if actionStep = 1 then %>
	<tr>
		<td colspan="2" align="right"> 
			<a HREF="#" onclick="window.close(); return false;"><img SRC="/cheminv/graphics/sq_btn/cancel_dialog_btn.gif" border="0"></a>&nbsp;&nbsp;
			<a HREF="#" onclick="Validate(); return false;"><img SRC="/cheminv/graphics/sq_btn/btn_next_61.gif" border="0"></a>
		</td>
	</tr>	
	<%else %>
	<tr>
		<td align="right">Barcodes</td><td><input type="checkbox" name="AutoPrint" <%=AutoPrint%>>Print barcodes immediately<br></td>
	</tr>
	<% if Action = "sample" then %>
	<tr height="25">
		<td align="right">Dispose Original:</td><td>
			<input type="checkbox" name="DisposeOrigContainer" <%=DisposeOrigContainer%>>Dispose remaining amount?
		</td>
	</tr>
	<% end if %>
	<%	
		Response.Write("<tr><td colspan=""2"">&nbsp;</td></tr>")
		Response.Write("<tr><td colspan=""2"">")
		arrValues = SetInitialQty(Action, QtyRemaining, NumContainers, SampleQty)
		Response.Write(vbtab & "<table border=""0"">")
		Response.Write(vbtab & "<tr>")
			Response.Write("<th>Sample</th>")
			if AutoGen_cb = "" then
				Response.Write("<th><span class=""required"">Barcode</span></th>")
			end if
			Response.Write("<th><span class=""required"">Num Copies</span></th>")
			Response.Write("<th><span class=""required"">Type</span></th>")
			Response.Write("<th><span class=""required"">Qty</span></th>")
			if Action = "new" then
				Response.Write("<th><span class=""required"">Container Size</span></th>")
				Response.Write("<th><span class=""required"">UOM</span></th>")
			else
				Response.Write("<th><span class=""required"">Container Size(" & UOMAbv & ")</span></th>")
			end if
			if lCase(Application("SAMPLE_REQUIRE_CONCENTRATION")) = "true" then
				Response.Write("<th><span class=""required"">Conc</span></th>")
				Response.Write("<th><span class=""required"">UOC</span></th>")
			else
				Response.Write("<th>Conc</th>")
				Response.Write("<th>UOC</th>")
			end if
		Response.Write("</tr>" & vbcrlf)
		for i = 1 to NumContainers
			TestQty = 1
			if Action = "sample" then
				jsValidation = " onchange=""calcQtyRemaining(" & i & ")"""
			else
				jsValidation = ""
			end if
			Qty = arrValues(i-1)
			Response.Write(vbtab & "<tr>" & vbcrlf)
			Response.Write(vbtab & "<td>" & i & "</td>" & vbcrlf)
			'Response.Write(i & " : " & arrValues(i-1))
			if AutoGen_cb = "" then
				Response.Write("<td><input type=""text"" size=""10"" name=""iBarcode"" value=""""></td>" & vbcrlf)
			end if

			if Request("iNumCopies") <> "" then
				if instr(Request("iNumCopies"),",") = 0 then
					NumCopies = Trim(Request("iNumCopies"))
				else
					arriNumCopies = split(Request("iNumCopies"),",")
					NumCopies = Trim(arriNumCopies(i-1))
				end if
			end if
			if Request("iSampleQty") <> "" then
				if instr(Request("iSampleQty"),",") = 0 then
					SampleQty = Trim(Request("iSampleQty"))
				else
					arriSampleQty = split(Request("iSampleQty"),",")
					SampleQty = Trim(arriSampleQty(i-1))
				end if
			end if
			if Request("iContainerSize") <> "" then
				if instr(Request("iContainerSize"),",") = 0 then
					ContainerSize = Trim(Request("iContainerSize"))
				else
					arriContainerSize = split(Request("iContainerSize"),",")
					ContainerSize = Trim(arriContainerSize(i-1))
				end if
			end if
			if Request("iConc") <> "" then
				if instr(Request("iConc"),",") = 0 then
					Conc = Trim(Request("iConc"))
				else
					arriConc = split(Request("iConc"),",")
					Conc = Trim(arriConc(i-1))
				end if
			end if
			if Request("iContainerTypeID") <> "" then
				if instr(Request("iContainerTypeID"),",") = 0 then
					ContainerTypeID = Trim(Request("iContainerTypeID"))
				else
					arriContainerTypeID = split(Request("iContainerTypeID"),",")
					ContainerTypeID = Trim(arriContainerTypeID(i-1))
				end if
			end if
			if Request("iUOM") <> "" then
				if instr(Request("iUOM"),",") = 0 then
					UOMAbv = Trim(Request("iUOM"))
				else
					arriUOM = split(Request("iUOM"),",")
					UOMAbv = Trim(arriUOM(i-1))
				end if
			end if
			if Request("iUOC") <> "" then
				if instr(Request("iUOC"),",") = 0 then
					UOCAbv = Trim(Request("iUOC"))
				else
					arriUOC = split(Request("iUOC"),",")
					UOCAbv = Trim(arriUOC(i-1))
				end if
			end if

			Response.Write("<td><input type=""text"" size=""4"" name=""iNumCopies"" value=""" & NumCopies & """" & jsValidation & "></td>" & vbcrlf)
			Response.write ShowPickList("", "iContainerTypeID", ContainerTypeID,"SELECT Container_Type_ID AS Value, Container_Type_Name AS DisplayText FROM inv_Container_Types ORDER BY lower(DisplayText) ASC") & vbcrlf
			Response.Write("<td><input type=""text"" size=""6"" name=""iSampleQty"" value=""" & SampleQty & """" & jsValidation & "></td>" & vbcrlf)
			Response.write ShowInputBox("", "ContainerSize", 4, "", False, False) & vbcrlf
			if Action = "new" then
				Response.write ShowPickList("", "iUOM", UOMAbv,"SELECT UNIT_ID||'='||Unit_Abreviation AS Value, Unit_Abreviation AS DisplayText FROM inv_units WHERE Unit_type_id_FK IN (1,2) ORDER BY lower(DisplayText) ASC") & vbcrlf
			end if
			Response.write ShowInputBox("", "Conc", 6, "", False, False) & vbcrlf
			Response.write ShowPickList("", "iUOC", UOCAbv,"SELECT UNIT_ID||'='||Unit_Abreviation AS Value, Unit_Abreviation AS DisplayText FROM inv_units WHERE Unit_type_id_FK IN (3,6) ORDER BY lower(DisplayText) ASC") & vbcrlf
			Response.Write(vbtab & "</tr>" & vbcrlf)
		next
		Response.Write("</table>")
		Response.Write("</td></tr>")
	%>
<!--	<tr>		<td colspan="2" align="right"> 			<a href="#" onclick="window.close(); return false;"><img SRC="/cheminv/graphics/sq_btn/cancel_dialog_btn.gif" border="0"></a>&nbsp;&nbsp;&nbsp;&nbsp;			<a href="#" onclick="document.form1.ActionStep.value='';document.form1.action='CreateContainerSample.asp';pageAction='<%=Action%>'; if (pageAction == 'new'){document.form1.submit();}else{history.go(-1);}"><img SRC="/cheminv/graphics/sq_btn/btn_back_61.gif" border="0"></a>			<a href="#" onclick="Validate(); return false;"><img SRC="/cheminv/graphics/sq_btn/ok_dialog_btn.gif" border="0"></a>		</td>	</tr>	-->
	<%end if%>


	<% if ActionStep = 2 then %>

	<tr><td colspan="2&quot;">&nbsp;</td></tr>
	<tr>
		<td colspan="2" align="right"> 
			<a href="#" onclick="window.close(); return false;"><img SRC="/cheminv/graphics/sq_btn/cancel_dialog_btn.gif" border="0"></a>&nbsp;&nbsp;&nbsp;&nbsp;
			<% if Action = "sample" then %>
				<a href="#" onclick="GoBack()"><img SRC="/cheminv/graphics/sq_btn/btn_back_61.gif" border="0"></a>
			<% else %>
				<a href="#" onclick="SetOwnerShipCode();document.form1.ActionStep.value='';document.form1.action='CreateContainerSample.asp';pageAction='<%=Action%>'; if (pageAction == 'new'){document.form1.submit();}else{document.form1.submit();}"><img SRC="/cheminv/graphics/sq_btn/btn_back_61.gif" border="0"></a>
			<% end if %>			
			<!---- <a href="#" onclick="Validate(); return false;"><img SRC="/cheminv/graphics/sq_btn/ok_dialog_btn.gif" border="0"></a>  %> -->
			<!-- Fixing Bug CSBR-77461 -->
            <a  href="#" onclick="document.body.style.cursor='wait' ; document.getElementById('button_ok').disabled=true; Validate();  document.body.style.cursor='Default' ; document.getElementById('button_ok').disabled=false; return false;"><img id ="button_ok" SRC="/cheminv/graphics/sq_btn/ok_dialog_btn.gif" border="0"></a>

		</td>
	</tr>	

	<% end if %>

</table>	
</form>
</center>

<script type="text/javascript" language="javascript">
<% 
if action = "new" and ActionStep=1 then
	if SamplingRegBatchID <> "" then %>
	document.form1.RegBatchID.value = "<%=SamplingRegBatchID%>";
	<% 
	end if
	if SamplingRegID <> "" then %>
	document.form1.iRegID.value = "<%=SamplingRegID%>";
	<% 
	end if
	if SamplingBatchNumber <> "" then %>
	document.form1.iBatchNumber.value = "<%=SamplingBatchNumber%>";
	<% 
	end if
	if SamplingCompoundID <> "" then %>
	document.form1.iCompoundID.value = "<%=SamplingCompoundID%>";
	<% 
	end if
	if SamplingContainerName <> "" then %>
	document.form1.iContainerName.value = "<%=SamplingContainerName	%>";
	<% end if
elseif action = "new" and ActionStep=2 then
	if SamplingQtyInitial <> "" then %>
	var NumContainers = document.form1.NumContainers.value;
	if (NumContainers > 1) {
		for (i=0; i < NumContainers; i++) {
			document.form1.iSampleQty[i].value=<%=SamplingQtyInitial%>;
		}
	} else {
		document.form1.iSampleQty.value = <%=SamplingQtyInitial%>;
	}
	<% 
	end if
	if SamplingQtyMax <> "" then %>
	var NumContainers = document.form1.NumContainers.value;
	if (NumContainers > 1) {
		for (i=0; i < NumContainers; i++) {
			document.form1.iContainerSize[i].value=<%=SamplingQtyMax%>;
		}
	} else {
		document.form1.iContainerSize.value = <%=SamplingQtyMax%>;
	}
	<% end if
	if SamplingConcentration <> "" then %>
	var NumContainers = document.form1.NumContainers.value;
	if (NumContainers > 1) {
		for (i=0; i < NumContainers; i++) {
			document.form1.iConc[i].value=<%=SamplingConcentration%>;
		}
	} else {
		document.form1.iConc.value = <%=SamplingConcentration%>;
	}
	<% end if
end if %>

<% if Application("ENABLE_OWNERSHIP")="TRUE" and PrincipalID="" then %>
//set the inital location group
SetOwnerInfo('location');
<%end if %>
</script>



</body>
</html>
<%

'****************************************************************************************
'*	PURPOSE: 
'*	INPUT: 
'*	OUTPUT: 
'****************************************************************************************
Function SetInitialQty(Action, QtyRemaining, NumContainers, SampleQty)
	Dim arrValues()
	Redim arrValues(NumContainers-1)
	for i = 0 to (NumContainers-1)
		if action = "split" then
			arrValues(i) = FormatNumber(QtyRemaining/NumContainers)
		else 
			arrValues(i) = SampleQty
		end if
	next
	SetInitialQty = arrValues
End function					


%>
