<%@ Language=VBScript %>
<!--#INCLUDE VIRTUAL = "/cheminv/gui/guiUtils.asp"-->
<!--#INCLUDE VIRTUAL = "/cheminv/api/apiUtils.asp"-->
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/ado.inc"-->

<%
Dim Conn
Dim Cmd
Dim RS
bDebugPrint = FALSE

'Response.Write(replace(Request.Form,"&","<br>") & "<br><br>")

'on error resume next
RequestID = trim(Request("RequestID"))
BatchID = trim(Request("BatchID"))
'ContainerID = Request("ContainerID")
ContainerIDList = Request("ContainerIDList")
ContainerBarcode = Request("ContainerBarcode")
NumContainersDisplay = Request("NumContainersDisplay")
BarcodeDescID = Request("BarcodeDescID")
if BarcodeDescID = "" then BarcodeDescID = 3
dateFormatString = Application("DATE_FORMAT_STRING")
QtyPerSample = Request("QtyPerSample")
ContainerSize = Request("ContainerSize")
FormStep = Request("FormStep")
action = Request("action")
TotalContainerQty = 0
AssignToRack = Request("AssignToRack")
'LocationID = Request("LocationID")
ShowFullRackList = lCase(Request("ShowFullRackList"))
if ShowFullRackList = "true" then CheckFullRack = " checked"
if Request("DisposeOrigContainer") = "on" then DisposeOrigContainer = " checked"

SampleRegID = Request("SampleRegID")
SampleBatchNumber = Request("SampleBatchNumber")



'NumContainers = Request("NumContainers")
'QtyList = Request("QtyList")
'ContainerTypeID = Request("ContainerTypeID")

'-- Get Request Info - always look it up b/c it can be edited from a link on this page
GetInvConnection()
Call GetInvCommand("{CALL " & Application("CHEMINV_USERNAME") & ".REQUESTS.GetBatchRequest(?,?)}", adCmdText)	
Cmd.Parameters.Append Cmd.CreateParameter("PREQUESTID",131, 1, 0, RequestID)
Cmd.Parameters("PREQUESTID").Precision = 9	
Cmd.Parameters.Append Cmd.CreateParameter("PDATEFORMAT",200,1,30, dateFormatString)
Cmd.Properties ("PLSQLRSet") = TRUE  
Set RS = Cmd.Execute
Cmd.Properties ("PLSQLRSet") = FALSE

ContainerID = RS("Container_ID")
BatchID = RS("Batch_ID_FK")
UserID = RS("RUserID")
DateRequired = ConvertDateToStr(Application("DATE_FORMAT"),RS("Date_Required"))
'NumContainers = cint(RS("number_containers"))
ContainerTypeID = RS("container_type_id_fk")			
'LocationID = RS("location_id_fk")

'Response.Write(LocationID)
'-- If user has selected a rack or contents of rack, set the default location to parent of rack

if Session("CurrentLocationID") = "" then Session("CurrentLocationID") = Request("DefLocationID")

rackParent = ""
if APPLICATION("RACKS_ENABLED") then
	'-- ValidateLocationIsRack returns COLLAPSE_CHILD_NODES, PARENT_ID for container
	rackTemp = split(ValidateLocationIsRack(Session("CurrentLocationID")),"::")
	isRack = rackTemp(0)
	rackParent = rackTemp(1)
	'Response.Write("###" & isRack & ":" & rackParent & "###")
	'if isRack = "1" then
	'LocationID = rackParent
	'end if
end if

'Response.Write("@@@" & Request("LocationID") & ":" & Session("CurrentLocationID") &"@@@")
If Request("LocationID") <> "" and ShowFullRackList = "" then 
	'LocationID = Request("LocationID")
	'Session("CurrentLocationID") = Request("LocationID")
	'Response.Write("<br>" & FormStep & ":" & Request("GetSessionLocationID") & "<br>")
	if FormStep = "1" and Request("GetSessionLocationID") = "" then 
		LocationID = Request("LocationID")
		Session("CurrentLocationID") = LocationID
	else
		LocationID = Session("CurrentLocationID")
	end if 
elseif ShowFullRackList = "true" then
	LocationID = rackParent
elseif Session("CurrentLocationID") <> "" then
	LocationID = Session("CurrentLocationID")
else
	LocationID = 0
	Session("CurrentLocationID") = 0
end if

if FormStep = "1" then
	FormStep = FormStep + 1
elseif FormStep = "" then
	FormStep = 1
end if


DeliveryLocationID = RS("delivery_Location_ID_FK")
comments = RS("request_comments")
QtyList = RS("quantity_list")
ShipToName= RS("ship_to_name")
ContainerTypeName = RS("container_type_name")
UOMAbv = RS("unit_abreviation")
SampleQtyUnit = RS("unit_of_meas_id_fk")
Set RS = nothing

'Get Date certified of the requested container
'SQL = "SELECT TO_CHAR(TRUNC(inv_containers.Date_Certified),'" & Application("DATE_FORMAT_STRING") & "') AS Date_Certified  FROM inv_containers WHERE container_id = ?"
'Set Cmd = GetCommand(Conn, SQL, adCmdText)
'Cmd.Parameters.Append Cmd.CreateParameter("ContainerID", 5, 1, 0, ContainerID)
'Set rsDateCertified = server.CreateObject("ADODB.recordset")
'rsDateCertified.Open Cmd
'DateCertified = rsDateCertified("date_certified")
'Set rsDateCertified = nothing
DateCertified = ""

'-- get batch RS
sqlContainerIDList = ""
if ContainerIDList <> "" then sqlContainerIDList = " And inv_containers.container_id in (" & ContainerIDList & ")"
SQL = "SELECT container_id, barcode, qty_remaining, unit_of_meas_id_fk, unit_abreviation FROM inv_containers, inv_units WHERE unit_of_meas_id_fk = unit_id (+) AND batch_id_fk = ? AND inv_containers.location_id_fk not in (1,2,3,4)" & sqlContainerIDList
Set Cmd = GetCommand(Conn, SQL, adCmdText)
Cmd.Parameters.Append Cmd.CreateParameter("BatchID", 5, 1, 0, BatchID)
Set RS = server.CreateObject("ADODB.recordset")
RS.cursorlocation= aduseclient
RS.Open Cmd

batchCount = RS.RecordCount

Dim orderArr()
Redim orderArr(batchCount-1,1)
for i=0 to batchCount-1
	orderArr(i,0) = Request("Order"&i)
	orderArr(i,1) = i
next


%>
<html>
<head>
<title><%=Application("appTitle")%> -- Create Samples from Batch</title>
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
	function ValidateMove(){
		var bWriteError = false;

		if (bWriteError){
			alert(errmsg);
		}
		else{
			document.all.form1.action = "#";
			document.form1.submit();
		}
	}
	
	function Validate(numElements){
		var bWriteError = false;
		var bChecked;
		var bSpotFilled;
		var FormStep = <%=FormStep%>;
		var errmsg = "Please fix the following problems:\r";
		var ContainerSize = document.form1.ContainerSize.value;
		var NumSamples = document.form1.NumContainersDisplay.value;
		var TotalContainerQty = document.form1.TotalContainerQty.value;
		if (numElements == 1){
			i = 0;
			bSpotFilled = false;
			bChecked = eval("document.all.Order" + i + ".checked");
			if (bChecked) 
				bSpotFilled = true; 
			if (!bSpotFilled) {
				errmsg = errmsg + "- You must select a plate for Position" + (i+1) + ".\r";
				bWriteError = true;
			}
		}
		else {
			for (i=0; i<numElements; i++) {
				bSpotFilled = false;
				for (j=0; j<numElements; j++){
					bChecked = eval("document.all.Order" + i + "[" + j + "].checked");
					if (bChecked) 
						bSpotFilled = true; 
				}
				if (!bSpotFilled) {
					errmsg = errmsg + "- You must select a container for Position" + (i+1) + ".\r";
					bWriteError = true;
				}
			}
		}

		<%if Application("RequireBarcode") = "TRUE" then%>
		//barcode is required
		if(document.form1.BarcodeDescID.value.length == 0) {
			errmsg = errmsg + "- Barcode Description is required.\r";
			bWriteError = true;
		}
		<% end if%>
		
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
		<% if FormStep = "2" then %>
		var numSamples = document.form1.NumContainersDisplay.value;
		if (document.form1.AssignToRack.checked) {
			
			// Validate Rack Grid ID
			if (document.form1.iRackGridID.value.length == 0){
				errmsg = errmsg + "- Please select a valid Starting Postion for the Rack.\r";
				bWriteError = true;
			}else if (!isPositiveNumber(document.form1.iRackGridID.value)&&(document.form1.iRackGridID.value != 0)){
				errmsg = errmsg + "- Rack grid location must be a positive number.\r";
				bWriteError = true;
			}else if (document.form1.RackGridList.value.length == 0) {
				errmsg = errmsg + "- Please choose a Rack grid location for this container.\r";
				bWriteError = true;
			}else{
				var tmpRackGridList = document.form1.RackGridList.value;
				tmpRackGridList = tmpRackGridList.split(",");
				if (numSamples != tmpRackGridList.length){
					errmsg = errmsg + "- The number of Rack grid locations (" + tmpRackGridList.length +") does not match the number of copies requested (" + numSamples + ").\r Please reselect Racks and Rack starting position again.\r";
					bWriteError = true;
				}
				//document.form1.LocationID.value = document.form1.iRackGridID.value;
			}
		}
		<% end if%>

		// Destination cannot be assign to a rack parent
		if (document.form1.AssignToRack.checked == false){
			if (ValidateLocationIDType(document.form1.LocationID.value) == 1) {
				errmsg = errmsg + "- Destination can not be a Rack unless \"Assign to Rack\" is checked. Please check \"Assign to Rack\" or choose a different location.\r";
				bWriteError = true;
				//document.form1.AssignToRack.checked = true;
			}
		}
		<% end if %>

		<% if FormStep = "2" then %>
		//bWriteError = true;
		<% end if %>
		
		// Container Size is required
		if (document.form1.ContainerSize.value.length == 0) {
			errmsg = errmsg + "- Container Size is required.\r";
			bWriteError = true;
		}
		else{
			// Container Size must be a number
			if (!isNumber(ContainerSize)){
				errmsg = errmsg + "- Container Size must be a number.\r";
				bWriteError = true;
			}
			if (ContainerSize <= 0){
				errmsg = errmsg + "- Container Size must be a positive number.\r";
				bWriteError = true;
			}
		}

		if (document.form1.FormStep.value == 1) {
			var QtyPerSample = document.form1.QtyPerSample.value;
			// Quantity per Sample is required
			if (document.form1.QtyPerSample.value.length == 0) {
				errmsg = errmsg + "- Quantity per Sample is required.\r";
				bWriteError = true;
			}
			else{
				// Quantity per Sample must be a number
				if (!isNumber(QtyPerSample)){
					errmsg = errmsg + "- Quantity per Sample must be a number.\r";
					bWriteError = true;
				}
				if (QtyPerSample <= 0){
					errmsg = errmsg + "- Quantity per Sample must be a positive number.\r";
					bWriteError = true;
				}
			}
		}
				
		// Container Size is required
		if (document.form1.NumContainersDisplay.value.length == 0) {
			errmsg = errmsg + "- Number of Samples is required.\r";
			bWriteError = true;
		}
		else{
			// Container Size must be a number
			if (!isNumber(document.form1.NumContainersDisplay.value)){
				errmsg = errmsg + "- Number of Samples must be a number.\r";
				bWriteError = true;
			}
			if (document.form1.NumContainersDisplay.value <= 0){
				errmsg = errmsg + "- Number of Samples must be a positive number.\r";
				bWriteError = true;
			}
		}

		//Validate Sample Quantities
		<% if FormStep = 2 then %>
		
		if (!document.form1.DisposeOrigContainer.checked) {
			bConfirmWarning = true;
			warningmsg = "You have chosen not to dispose the original sample. Do you want to continue?";
			bConfirmWarning = confirm(warningmsg);
			if (!bConfirmWarning) return false;
		}

		if (!bWriteError) {
			var Sum,x,y;
			Sum = 0;
			var arrValues = new Array(NumSamples);
			
			for(i=1; i<=NumSamples; i++) {
				x = eval("document.form1.Sample" + String(i) + ".value;");
				if (!isNumber(x) || x <= 0) {
					errmsg = errmsg + "- Sample Qty " + i + " must be positive number greater than zero.\r";
					bWriteError = true;
					break;
				} else {
					arrValues[i-1] = x;
					Sum = Sum + Number(x);
				}
			}
			if (!bWriteError) {
				//check the validity of all of the new quantities
				for (i=0; i<NumSamples; i++) {
					//must be a number
					if (!isNumber(arrValues[i])) {
						errmsg = errmsg + "- All quantities must be numbers.\r";
						bWriteError = true;
						break;
					}
					//must be positive numbers
					if (arrValues[i] <= 0) {
						errmsg = errmsg + "- All quantities must be positive numbers.\r";
						bWriteError = true;
						break;
					}
					if (Number(arrValues[i]) > Number(ContainerSize)) {
						errmsg = errmsg + "- Container Size can not be less than the Sample Qty " + (i+1) + " quantity.\r";
						bWriteError = true;
						//break;
					}
				}
				Sum = Math.round(Sum*100)/100;
				TotalContainerQty = Math.round(TotalContainerQty*100)/100;
				if (Sum > TotalContainerQty) {
					errmsg = errmsg + "- The sum of samples cannot exceed the total available in the listed containers.\r";
					bWriteError = true;
				}
			}
		}	
		<% end if %>
		if (bWriteError){
			alert(errmsg);
		}
		else{
			//order the plates
			//numElements = document.all.numElements.value;
			tempID = "";
			if (numElements == 0){
				ID = document.form1.ContainerID.value;
			}
			else if (numElements == 1){
				ID = document.all.Order0.value;
			}
			else {
			for(i=0; i<numElements; i++) {
				element = eval("document.all.Order" + i);
				for(j=0; j<numElements; j++) {
					if (element[j].checked) {
						tempID = tempID + element[j].value + ",";
					}
				}
			}
			ID = tempID.substr(0,tempID.length-1);
			}
			document.all.BatchContainerIDs.value = ID;
			if (FormStep == 1) {
				//document.form1.FormStep.value = 2;
				document.form1.action = 'CreateSamplesFromBatch.asp';
			}
			
			document.form1.submit();
		}	
	
	}
	
	
	function ClearSelections(ID, element, totalElements) {
		currName = element.name;
		currValue = element.value;
		
		for (i=0; i<totalElements; i++) {
			for (j=0; j<totalElements; j++){
				value = eval("document.all.Order" + i + "[" + j + "].value");
				name = eval("document.all.Order" + i + "[" + j + "].name");
				if (value == currValue && name != currName) {
					eval("document.all.Order" + i + "[" + j + "].checked = false");				
				}
			}
		}
	}


	// Toggles display of Rack editing fields
	function toggleRackDisplay() {
		if (ValidateLocationIDType(document.form1.LocationID.value) == 1 && document.form1.AssignToRack.checked == false) {
			alert("You cannot uncheck \"Assign to Rack\" because you have chosen to assign the sample to a Rack Location ID. \rPlease click \"Back\" and choose a different Location ID if this is correct.");
			document.form1.AssignToRack.checked = true;
		} else {
		
			if (document.form1.AssignToRack.checked) {
				AlterCSS('.singleRackElement','display','block')
				AlterCSS('.singleRackElement','color','red')
				if (!document.form1.SelectRackByLocation.checked && !document.form1.SelectRackBySearch.checked) {
					document.form1.SelectRackByLocation.checked = true;
					toggleRackLocationDisplay();
					document.form1.SelectRackBySearch.checked = false;
					toggleRackSearchDisplay();
				}
			}else{
				AlterCSS('.singleRackElement','display','none')
				AlterCSS('.rackLabel','color','black')
			}
		}
	}	
	
	function toggleRackLocationDisplay() {
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
	
	function toggleRackSearchDisplay() {
		if (document.form1.SelectRackBySearch.checked) {
			AlterCSS('.locationRackList','display','none')
			document.form1.SelectRackByLocation.checked = false;
			AlterCSS('.suggestRackList','display','block')
		} else {
			document.form1.SelectRackBySearch.checked = false;
			document.form1.SelectRackByLocation.checked = true;
			AlterCSS('.suggestRackList','display','none')
			AlterCSS('.locationRackList','display','block')
		}
	}
	


	function toggleRackListDisplay() {
		var strRackShowFullList = "";
		if (document.form1.AssignToRack.checked) {		
			if (document.form1.ShowFullRackList.checked) {
				strRackShowFullList = "true";
			}
			if (document.form1.ShowFullRackList.checked) {
				document.form1.ShowFullRackList.value='true';
			} else {
				document.form1.ShowFullRackList.value='';
				document.form1.GetSessionLocationID.value='true';
			}
			document.form1.FormStep.value = "1";
			document.form1.action = "CreateSamplesFromBatch.asp";
			document.form1.submit();
		}
	}	

	function validateLocation(LocationID){
		CurrLocationID = 0;
		CurrLocationID = <%=LocationID%>;
		if (document.form1.LocationID.value != CurrLocationID){
			
			document.form1.FormStep.value = "1";
			document.form1.action = "CreateSamplesFromBatch.asp";
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
		var numSamples = document.form1.NumContainersDisplay.value;
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
			//alert(totPosCnt + ":" + numSamples);
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
		if (eval(document.form1.NumContainersDisplay.value) == 1){
			selectRackDefaults();
		}
	}   
	
	function OpenSearchDialog() {
		contSize = document.form1.ContainerSize.value;
		numSamples = "<%=NumContainersDisplay%>";
		document.form1.iRegID.value = document.form1.SampleRegID.value;
		document.form1.iBatchNumber.value = document.form1.SampleBatchNumber.value;
		OpenDialog('/cheminv/gui/ManageRacks.asp?numSamples='+numSamples+'&contSize='+contSize,'SuggestRacks',1);
	}

	function GoBack() {
		document.form1.action='/cheminv/gui/createsamplesfrombatch.asp';
		document.form1.FormStep.value = "";
		//document.form1.ContainerSize.value = "";
		document.form1.NumContainersDisplay.value = "<%=NumContainersDisplay%>";
		document.form1.QtyPerSample.value = document.form1.Sample1.value;
		//document.form1.QtyPerSample.value = document.form1.QtyPerSample.value;
		document.form1.Action.value = "new";
		document.form1.submit();
	}
	
-->
</script>
</head>
<body>
<center>
<form name="form1" action="CreateSamplesFromBatchPreview.asp" method="POST">
<input Type="hidden" name="Action" value>
<input Type="hidden" name="RequestID" value="<%=RequestID%>">
<input type="hidden" name="ContainerID" value="<%=ContainerID%>">
<input type="hidden" name="ContainerIDList" value="<%=ContainerIDList%>">
<input type="hidden" name="ContainerBarcode" value="<%=ContainerBarcode%>">
<input type="hidden" name="BatchCount" value="<%=BatchCount%>">
<input type="hidden" name="BatchContainerIDs" value>
<input type="hidden" name="QtyList" value="<%=QtyList%>">
<input type="hidden" name="NumContainers" value="<%=NumContainers%>">
<!--<input type="hidden" name="ContainerTypeID" value="<%=ContainerTypeID%>">-->
<input type="hidden" name="UOMAbv" value="<%=UOMAbv%>">
<input type="hidden" name="SampleQtyUnit" value="<%=SampleQtyUnit%>">
<input type="hidden" name="DateCertified" value="<%=DateCertified%>">
<input type="hidden" name="FormStep" value="<%=FormStep%>">
<input type="hidden" name="SampleRegID" value="<%=SampleRegID%>">
<input type="hidden" name="SampleBatchNumber" value="<%=SampleBatchNumber%>">
<input type="hidden" name="iRegID" value="">
<input type="hidden" name="iBatchNumber" value="">

<% if FormStep = "2" then %>
<input type="hidden" name="QtyPerSample" value>
<% end if %>
<input TYPE="hidden" name="GetSessionLocationID" Value>

<table border="0">
	<%if batchCount > 0 then%>
	<tr>
		<td colspan="2">
			<span class="GuiFeedback">Select the order in which containers will be used to create samples.</span>
		</td>
	</tr>
	<tr><td COLSPAN="2" ALIGN="center">
	<table BORDER="0">
	<tr>
		<th>Container</th>
		<th>Qty Remaining</th>
		<th>Order of Use</th>
	</tr>
	<%
	barcodeList = " "
	currContainerNum = 0
	
	While not RS.EOF 
		currContainerID = RS("container_id")
		currBarcode = RS("barcode")
		barcodeList = barcodeList & currBarcode & ","
		TotalContainerQty = TotalContainerQty + cDbl(RS("qty_remaining"))
		if numsourcePlates = 1 then
			theRow = "<span style=""display:none"">"
		else
			theRow = "<tr><td align=""center"">" & currBarcode & "</td>"
		end if
		theRow = theRow & "<td align=""center"">" & RS("qty_remaining") & " " & RS("unit_abreviation") &  "</td>"
		'theRow = "<tr><td align=""left"">" & curPlateBarcode & ":" & curPlateID & "</td>"
		theRow = theRow & "<td align=""center"">"
		if numsourcePlates = 1 then
			i=0
				theRow = theRow & "<input type=""radio"" name=""Order" & i & """ value=""" & currContainerID & """ checked>" & (i+1)
		else
			sChecked = ""
			for i=0 to batchCount-1
			
				if FormStep = 1 and i = currContainerNum then 
					sChecked = " checked"
				elseif FormStep = 2 and orderArr(i,0) = cStr(currContainerID) then
					sChecked = " checked"
				else
					sChecked = "" 
				end if
				theRow = theRow & "<input type=""radio"" name=""Order" & i & """ value=""" & currContainerID & """ onclick=""ClearSelections(" & currContainerID & ", this, " & batchCount & "); """ & sChecked & ">" & (i+1)
			next
			currContainerNum = currContainerNum + 1
		end if
		if numsourcePlates = 1 then
			theRow = theRow & "</span>"
		else
			theRow = theRow & "</td></tr>" & chr(13)
		end if
		Response.Write theRow
		RS.MoveNext
	wend
	barcodeList = left(barcodeList,len(barcodeList)-1)%>
	</table>
	<input type="hidden" name="TotalContainerQty" value="<%=TotalContainerQty%>">
	</td></tr>
	<%end if%>
	<tr height="25">
<%if Application("RequireBarcode") = "TRUE" then%>
		<td align="right" valign="top" nowrap><span class="required">Barcode Description:</span></td>
<%else%>
		<td align="right" valign="top" nowrap>Barcode Description:</td>
<%end if%>	
		<td>
				<%=ShowSelectBox2("BarcodeDescID", BarcodeDescID,"SELECT barcode_desc_id AS Value, barcode_desc_name AS Displaytext FROM inv_barcode_desc ORDER BY lower(DisplayText) ASC", null," ","")%>
		</td>
	</tr>

	<tr height="25">
		<td align="right">Dispose Original:</td><td>
			<input type="checkbox" name="DisposeOrigContainer" <%=DisposeOrigContainer%>>Dispose remaining amount?
		</td>
	</tr>

	<tr height="25">
		<td align="right" valign="top" nowrap>
			<span class="required">Sample Location ID:</span>
		</td>
		<td colspan="3">
			<!--&nbsp;<%=GetBarcodeIcon()%>&nbsp;<%ShowLocationPicker "document.form1", "LocationID", "lpLocationBarCode", "lpLocationName", 10, 20, false%> -->
			<% if FormStep = "2" then %>
			&nbsp;<%=GetBarcodeIcon()%>&nbsp;<%ShowLocationPicker3 "document.form1", "LocationID", "lpLocationBarCode", "lpLocationName", 10, 20, false,"validateLocation(document.form1.LocationID.value)"%> 
			<% else %>
			&nbsp;<%=GetBarcodeIcon()%>&nbsp;<%ShowLocationPicker "document.form1", "LocationID", "lpLocationBarCode", "lpLocationName", 10, 20, false%>
			<% end if %>
		</td>
	</tr>
	
	<% if Application("RACKS_ENABLED") then %>	
	<% '-- Toggle to turn ON/OFF options to assign containers to Rack %>
	<% '-- ---------------------------------------------------------- %>
	<% if FormStep = "2" then %>
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
		<%= ShowPickList3("Select Rack(s):", "RackID", RackID,"select l.location_id as Value, cheminvdb2.guiutils.GETRACKLOCATIONPATH(l.location_id) || ' :: ' || ((f.row_count * f.col_count) - ((select /*+ index(c CONTAINER_LOCATION_ID_FK_IDX) */count(*) from inv_containers c, inv_vw_grid_location_lite gl where c.location_id_fk = gl.location_id and gl.parent_id = l.location_id) + (select /*+ index(p PLATE_LOCATION_ID_FK_IDX) */count(*) from inv_plates p, inv_vw_grid_location_lite gl where p.location_id_fk = gl.location_id and gl.parent_id = l.location_id) + (select /*+ index(lr INV_LOCATION_PK) */count(*) from inv_locations lr, inv_vw_grid_location_lite gl where lr.parent_id = gl.location_id and gl.parent_id = l.location_id))) || ' open' as DisplayText, (select gl.location_id || '::' || gl.name from inv_vw_grid_location gl where gl.parent_id = l.location_id and not exists (select /*+ index(c CONTAINER_LOCATION_ID_FK_IDX) */ location_id_fk from inv_containers c where c.location_id_fk = gl.location_id) and not exists (select /*+ index(p PLATE_LOCATION_ID_FK_IDX) */ location_id_fk from inv_plates p where p.location_id_fk = gl.location_id) and not exists (select /*+ index(lr INV_LOCATION_PK) */ parent_id from inv_locations l where gl.location_id = l.parent_id and collapse_child_nodes = 1) and rownum = 1 )  as DefaultValue from inv_grid_storage s, inv_locations l, inv_grid_format f where s.location_id_fk = l.location_id and l.collapse_child_nodes = 1 and s.grid_format_id_fk = f.grid_format_id and l.location_id in (select location_id from inv_locations where collapse_child_nodes = 1 connect by prior location_id = parent_id start with location_id = " & LocationID & ") and collapse_child_nodes = 1    and ((f.row_count * f.col_count) - ((select /*+ index(c CONTAINER_LOCATION_ID_FK_IDX) */count(*) from inv_containers c, inv_vw_grid_location_lite gl where c.location_id_fk = gl.location_id and gl.parent_id = l.location_id) + (select /*+ index(p PLATE_LOCATION_ID_FK_IDX) */count(*) from inv_plates p, inv_vw_grid_location_lite gl where p.location_id_fk = gl.location_id and gl.parent_id = l.location_id) + (select /*+ index(lr INV_LOCATION_PK) */count(*) from inv_locations lr, inv_vw_grid_location_lite gl where lr.parent_id = gl.location_id and gl.parent_id = l.location_id and lr.collapse_child_nodes = 1))) > 0  order by DisplayText",100,"","","setDefaultStartingPosition();",5,true,"No Racks in this location")%>
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
		<a class="MenuLink" href="#" onclick="OpenSearchDialog(); return false">Search Racks</a><br />
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
		selectRackDefaults();
	</script>
	<% end if %>
	<% end if %>

	<tr>
		<td align="right" nowrap>
			<span class="required">Sample Container Size (<%=UOMAbv%>):</span>
		</td>
		<td>
			<input TYPE="text" SIZE="10" Maxlength="50" NAME="ContainerSize" VALUE="<%=ContainerSize%>">
		</td>
	</tr>	
	<tr>
		<%=ShowPickList("<SPAN class=required>Sample Container type:</span>", "ContainerTypeID", ContainerTypeID,"SELECT Container_Type_ID AS Value, Container_Type_Name AS DisplayText FROM inv_Container_Types ORDER BY lower(DisplayText) ASC")%>
<!--		<td align="right" nowrap>			Sample Container Type:		</td>		<td>			<input TYPE="text" SIZE="10" Maxlength="50" NAME="ContainerTypeName">		</td>		-->
	</tr>
	<tr>
		<td align="right" nowrap>
			<span class="required">Number of Samples:</span>
		</td>
		<td>
		<% if FormStep = 2 then %>
			<input type="text" size="10" maxlength="50" name="NumContainersDisplay" value="<%=NumContainersDisplay%>" readonly style="background-color:#d3d3d3;">
		<% else %>
			<input type="text" size="10" maxlength="50" name="NumContainersDisplay" value="<%=NumContainersDisplay%>">
		<% end if %>
		</td>
	</tr>
	<% if FormStep = 1 then %>
	<tr>
		<td align="right" nowrap>
			<span class="required">Qty per sample (<%=UOMAbv%>):</span>
		</td>
		<td>
			<input type="text" size="10" maxlength="50" name="QtyPerSample" value="<%=QtyPerSample%>">
		</td>
	</tr>
	<% if Application("RACKS_ENABLED") then %>	
		<tr height="25">
			<td align="right">Assign Rack:</td><td>
				<input class="noborder" type="checkbox" name="AssignToRack">Assign container to Rack
			</td>
		</tr>
	<% end if %>
	<% end if %>

	<%
	if action <> "new" then
		Dim arrValues()
		Redim arrValues(NumContainersDisplay)
		if Request("Sample" & NumContainersDisplay) <> "" then
			for j = 0 to NumContainersDisplay
				arrValues(j) = Request("Sample" & (j+1))
			next
		else
			for j = 0 to NumContainersDisplay
				arrValues(j) = QtyPerSample
			next
		end if
		Response.Write GenerateFields("Sample ", "Qty (" & UOMAbv & ")", "Sample", NumContainersDisplay, arrValues)
	end if
	%>
	<tr><td COLSPAN="2" ALIGN="right"><a HREF="#" class="MenuLink" ONCLICK="OpenDialog('/Cheminv/GUI/RequestBatch.asp?action=edit&amp;RequestID=<%=RequestID%>&amp;BatchID=<%=BatchID%>&amp;ContainerBarcode=<%=ContainerBarcode%>&amp;UOMAbv=<%=UOMAbv%>', 'Diag2',2)">Edit Request</a></td></tr>
	<tr>
		<td colspan="2" align="right"> 
			<a href="#" onclick="window.close(); return false;"><img SRC="/cheminv/graphics/sq_btn/cancel_dialog_btn.gif" border="0"></a>
			<% if FormStep = "2" then %>
			<!--<a href="javascript:document.form1.FormStep.value='';history.go(-1);">-->
			<a href="#" onclick="GoBack()">
			<!--<a href="javascript:document.form1.FormStep.value='';document.form1.action='CreateSamplesFromBatch.asp?Action=new&batchid=<%=BatchID%>&RequestID=<%=RequestID%>';document.form1.submit();">-->
			<img src="/cheminv/graphics/sq_btn/btn_back_61.gif" border="0"></a>
			<% end if %>
			<a href="#" onclick="Validate(<%=batchCount%>); return false;"><img src="/cheminv/graphics/sq_btn/btn_next_61.gif" border="0"></a>
		</td>
	</tr>	
	
</table>	

<%if AssignToRack = "on" and FormStep = "2" then %>
	<script language="javascript">
	document.form1.AssignToRack.checked = true;
	toggleRackDisplay();
	</script>
<% end if %>


</form>
</center>
</body>
</html>
<%
	Conn.Close
	Set Cmd = Nothing
	Set Conn = Nothing


%>