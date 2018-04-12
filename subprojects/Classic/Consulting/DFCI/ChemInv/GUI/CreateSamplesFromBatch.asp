<!--#INCLUDE VIRTUAL = "/cheminv/gui/guiUtils.asp"-->
<!--#INCLUDE VIRTUAL = "/cheminv/api/apiUtils.asp"-->
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/ado.inc"-->
<% 
Dim Conn
Dim Cmd
Dim RS
bDebugPrint = FALSE

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
Call GetInvCommand("{CALL " & Application("CHEMINV_USERNAME") & ".REQUESTS.GETBATCHREQUESTBYREQUESTEDUNIT(?,?)}", adCmdText)	
Cmd.Parameters.Append Cmd.CreateParameter("PREQUESTID",131, 1, 0, RequestID)
Cmd.Parameters("PREQUESTID").Precision = 9	
Cmd.Parameters.Append Cmd.CreateParameter("PDATEFORMAT",200,1,30, dateFormatString)
Cmd.Properties ("PLSQLRSet") = TRUE  
Set RS = Cmd.Execute
Cmd.Properties ("PLSQLRSet") = FALSE
if not rs.eof then
ContainerID = RS("Container_ID")
BatchID = RS("Batch_ID_FK")
UserID = RS("RUserID")
DateRequired = ConvertDateToStr(Application("DATE_FORMAT"),RS("Date_Required"))
'NumContainers = cint(RS("number_containers"))
ContainerTypeID = RS("container_type_id_fk")			
'LocationID = RS("location_id_fk")
AmountRequested = RS("qty_required")
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
end if
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

if isNull(BatchID) or BatchID="" then
    '-- get the qty_remaining of the single container
    SQL = "SELECT qty_remaining FROM inv_containers, inv_requests WHERE container_id = container_id_fk and request_id = " & RequestID
    Set rsQty = Conn.Execute(SQL)
    TotalContainerQty = rsQty("qty_remaining")
end if    

'-- get batch RS
sqlContainerIDList = ""
if ContainerIDList <> "" then sqlContainerIDList = " And inv_containers.container_id in (" & ContainerIDList & ")"
SQL = "SELECT container_id, barcode, qty_remaining, unit_of_meas_id_fk, unit_abreviation FROM inv_containers, inv_units ,inv_requests, inv_locations WHERE inv_containers.location_id_fk=inv_locations.location_id and inv_locations.location_type_id_fk in (1000,1003,1004) and unit_of_meas_id_fk = unit_id (+) AND inv_containers.batch_id_fk = ? AND  inv_containers.unit_of_meas_id_fk = inv_requests.required_unit_id_fk AND inv_requests.Request_ID = ? AND " & Application("ORASCHEMANAME") & ".GUIUTILS.IS_CONTAINER_AVAILABLE(inv_containers.container_id) = 1 " & sqlContainerIDList & " order by inv_containers.date_received asc" ' pjd added locations
Set Cmd = GetCommand(Conn, SQL, adCmdText)
Cmd.Parameters.Append Cmd.CreateParameter("BatchID", 5, 1, 0, BatchID)
Cmd.Parameters.Append Cmd.CreateParameter("Request_ID", 5, 1, 0, RequestID)
Set RS = server.CreateObject("ADODB.recordset")
RS.cursorlocation= aduseclient
RS.Open Cmd

batchCount = RS.RecordCount
%>
<html>
<head>
<title><%=Application("appTitle")%> -- Create Samples from Batch</title>
<script type="text/javascript" language="javascript" src="/cheminv/Choosecss.js"></script>
<script type="text/javascript" language="javascript" src="/cheminv/utils.js"></script>
<script type="text/javascript" language="javascript" src="/cheminv/gui/validation.js"></script>
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
<script type="text/javascript" language="javascript">
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
function sortList(containerlist){
   
    var arrcontainerlist= new Array;
    arrcontainerlist = containerlist.split(",");    
    var list = window.document.forms[0].containerList;
    var total = list.options.length-1;
    var items = new Array;
    var values = new Array;     
    for (i = 0; i <=total; i++) {
        items[i] = list.options[i].text;
        values[i] = list.options[i].value;
    }
    
    for(i=0;i<=arrcontainerlist.length-1;i++){
       for(j=0;j<=total;j++){                
         if(arrcontainerlist[i]==values[j]) {                   
            list.options[i] = new Option(items[j], values[j]);
            break; 
        }
       }
     }  
     list.focus();  
}	
function move(index,to) {
    var list = window.document.forms[0].containerList;
    var total = list.options.length-1;
    if (index == -1) return false;
    if (to == +1 && index == total) return false;
    if (to == -1 && index == 0) return false;
    var items = new Array;
    var values = new Array;
    for (i = total; i >= 0; i--) {
    items[i] = list.options[i].text;
    values[i] = list.options[i].value;
    }
    for (i = total; i >= 0; i--) {
    if (index == i) {
    list.options[i + to] = new Option(items[i],values[i], 0, 1);
    list.options[i] = new Option(items[i + to], values[i + to]);
    i--;
    }
    else {
    list.options[i] = new Option(items[i], values[i]);
       }
    }
    list.focus();
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
/*
code has been removed from here to fix the bug CSBR-76483
*/
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
        var numSamples = document.form1.NumContainersDisplay.value;
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
	
    var values='';
    for (i = 0; i <= window.document.forms[0].containerList.options.length-1; i++) {
        if (values == ''){
        values = window.document.forms[0].containerList.options[i].value;
        }
        else
        {
        values = values + ',' + window.document.forms[0].containerList.options[i].value;
        }
    }
            ID = values;
	
			document.all.BatchContainerIDs.value = ID;			
			if (FormStep == 1) {
				//document.form1.FormStep.value = 2;
				document.form1.action = 'CreateSamplesFromBatch.asp';
			}
			
			document.form1.submit();
		}	
	
	}

//Remove function toggleRackListDisplay() to Fix CSBR-76483
	function validateLocation(LocationID){
		CurrLocationID = 0;
		CurrLocationID = <%=LocationID%>;
		if (document.form1.LocationID.value != CurrLocationID){
			
			document.form1.FormStep.value = "1";
			document.form1.action = "CreateSamplesFromBatch.asp";
			document.form1.submit();
		}
	}
//Remove function validateRackSelect() to Fix CSBR-76483	
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
		<th colspan=2 ><font size=1> Container ID : Qty Remaining</font></th>		
	</tr>
	<%
	barcodeList = " "
	currContainerNum = 0
	theRow ="<tr><td align=""center""><select size=""6"" name=""containerList"" multiple>"
	While not RS.EOF 
		currContainerID = RS("container_id")
		currBarcode = RS("barcode")
		barcodeList = barcodeList & currBarcode & ","
		TotalContainerQty = TotalContainerQty + cDbl(RS("qty_remaining"))
		theRow= theRow & "<OPTION value=""" & currContainerID & """> &nbsp;&nbsp; " & currBarcode & "  :  " & RS("qty_remaining") & " " & RS("unit_abreviation") & "&nbsp;&nbsp;" 
		RS.MoveNext
	wend
	theRow = theRow & "</select></td>"
	theRow = theRow & "<td><a href=""#"" onClick=""move(document.forms[0].containerList.selectedIndex,-1);return false"" title=""Move Up""><img border=""0"" src=""/cheminv/graphics/up_arrow.gif""></a><br><a href=""#"" onClick=""move(document.forms[0].containerList.selectedIndex,+1);return false""><img border=""0"" src=""/cheminv/graphics/down_arrow.gif"" title=""Move Down""></a></td></tr>"

	Response.Write theRow
	barcodeList = left(barcodeList,len(barcodeList)-1)
	if Request("BatchContainerIDs")<>"" then 
	%>
	<script language=javascript>
	    sortList('<%=Request("BatchContainerIDs")%>');
	</script>
	
	<%end if %>
	</table>
	</td></tr>
	<%end if%>
<input type="hidden" name="TotalContainerQty" value="<%=TotalContainerQty%>">
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
	
	<tr>
		<td align="right" nowrap>Amount Requested (<%=UOMAbv%>):</td>
		<td>
			<input TYPE="text" SIZE="10" Maxlength="50" NAME="AmountRequested" VALUE="<%=AmountRequested%>" readonly class="readonly">
		</td>
	</tr>	
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
			<input type="text" size="10" maxlength="50" name="NumContainersDisplay" value="<%=NumContainersDisplay%>" readonly class="readOnly">
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
