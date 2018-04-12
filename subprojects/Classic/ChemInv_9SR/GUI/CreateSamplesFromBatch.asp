<%@ Language=VBScript %>
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
'ContainerID = Request("ContainerID")
ContainerBarcode = Request("ContainerBarcode")
BarcodeDescID = Request("BarcodeDescID")
dateFormatString = Application("DATE_FORMAT_STRING")
'NumContainers = Request("NumContainers")
'QtyList = Request("QtyList")
'ContainerTypeID = Request("ContainerTypeID")

'get Request Info - always look it up b/c it can be edited from a link on this page
GetInvConnection()
Call GetInvCommand("{CALL " & Application("CHEMINV_USERNAME") & ".REQUESTS.GetRequest(?,?)}", adCmdText)	
Cmd.Parameters.Append Cmd.CreateParameter("PREQUESTID",131, 1, 0, RequestID)
Cmd.Parameters("PREQUESTID").Precision = 9	
Cmd.Parameters.Append Cmd.CreateParameter("PDATEFORMAT",200,1,30, dateFormatString)
Cmd.Properties ("PLSQLRSet") = TRUE  
Set RS = Cmd.Execute
Cmd.Properties ("PLSQLRSet") = FALSE

ContainerID = RS("Container_ID_FK")
UserID = RS("RUserID")
DateRequired = ConvertDateToStr(Application("DATE_FORMAT"),RS("Date_Required"))
NumContainers = cint(RS("number_containers"))
ContainerTypeID = RS("container_type_id_fk")			
LocationID = RS("location_id_fk")
DeliveryLocationID = RS("delivery_Location_ID_FK")
comments = RS("request_comments")
QtyList = RS("quantity_list")
ShipToName= RS("ship_to_name")
ContainerTypeName = RS("container_type_name")
UOMAbv = RS("unit_abreviation")
SampleQtyUnit = RS("unit_of_meas_id_fk")
Set RS = nothing

'Get Date certified of the requested container
SQL = "SELECT TO_CHAR(TRUNC(inv_containers.Date_Certified),'" & Application("DATE_FORMAT_STRING") & "') AS Date_Certified  FROM inv_containers WHERE container_id = ?"
Set Cmd = GetCommand(Conn, SQL, adCmdText)
Cmd.Parameters.Append Cmd.CreateParameter("ContainerID", 5, 1, 0, ContainerID)
Set rsDateCertified = server.CreateObject("ADODB.recordset")
rsDateCertified.Open Cmd
DateCertified = rsDateCertified("date_certified")
Set rsDateCertified = nothing

'get batch RS
SQL = "SELECT container_id, barcode, qty_remaining, unit_of_meas_id_fk, unit_abreviation FROM inv_containers, inv_units WHERE unit_of_meas_id_fk = unit_id (+) AND parent_container_id_fk = (SELECT parent_container_id_fk FROM inv_containers WHERE container_id = ?)"
'Response.Write SQL & ":" & ContainerID
Set Cmd = GetCommand(Conn, SQL, adCmdText)
Cmd.Parameters.Append Cmd.CreateParameter("ContainerID", 5, 1, 0, ContainerID)
Set RS = server.CreateObject("ADODB.recordset")
RS.cursorlocation= aduseclient
RS.Open Cmd

batchCount = RS.RecordCount
%>
<html>
<head>
<title><%=Application("appTitle")%> -- Create Samples from Batch</title>
<script LANGUAGE="javascript" src="/cheminv/Choosecss.js"></script>
<script LANGUAGE="javascript" src="/cheminv/utils.js"></script>
<script LANGUAGE="javascript" src="/cheminv/gui/validation.js"></script>
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
	
	function ValidateOrder(numElements){
		var bWriteError = false;
		var bChecked;
		var bSpotFilled;
		var errmsg = "Please fix the following problems:\r";
		var ContainerSize = document.form1.ContainerSize.value;
		var NumSamples = document.form1.NumContainersDisplay.value;
		
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

<%if lcase(Application("RequireBarcode")) = "true" then%>
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

		//Container Size can not be less than what is requested
		if (!bWriteError) {
			var Sum,x,y;
			Sum = 0;
			var arrValues = new Array(NumSamples);
			
			for(i=1; i<=NumSamples; i++) {
				x = eval("document.form1.Sample" + String(i) + ".value;");
				arrValues[i-1] = x;
				Sum = Sum + Number(x);
			}
			//check the validity of all of the new quantities
			for (i=0; i<NumSamples; i++) {
				//must be a number
				/*
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
				*/
				if (Number(arrValues[i]) > Number(ContainerSize)) {
					errmsg = errmsg + "- Container Size can not be less than the Sample " + (i+1) + " quantity.\r";
					bWriteError = true;
					//break;
				}
			}
		}		

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
		//document.all.item("
/*
		test = document.all.Plate1[1].value;
		document.all.Plate1[2].checked = true;
		alert(curPlateValue);
*/
	}
	
-->
</script>
</head>
<body>
<center>
<form name="form1" action="CreateSamplesFromBatchPreview.asp"method="POST">
<input Type="hidden" name="RequestID" value="<%=RequestID%>">
<INPUT TYPE="hidden" NAME="ContainerID" VALUE="<%=ContainerID%>">
<INPUT TYPE="hidden" NAME="ContainerBarcode" VALUE="<%=ContainerBarcode%>">
<INPUT TYPE="hidden" NAME="BatchCount" VALUE="<%=BatchCount%>">
<INPUT TYPE="hidden" NAME="BatchContainerIDs" VALUE="">
<INPUT TYPE="hidden" NAME="QtyList" VALUE="<%=QtyList%>">
<INPUT TYPE="hidden" NAME="NumContainers" VALUE="<%=NumContainers%>">
<INPUT TYPE="hidden" NAME="ContainerTypeID" VALUE="<%=ContainerTypeID%>">
<INPUT TYPE="hidden" NAME="UOMAbv" VALUE="<%=UOMAbv%>">
<INPUT TYPE="hidden" NAME="SampleQtyUnit" VALUE="<%=SampleQtyUnit%>">
<input type="hidden" name="DateCertified" value="<%=DateCertified%>">
<table border="0">
	<%if batchCount > 0 then%>
	<tr>
		<td colspan="2">
			<span class="GuiFeedback">Select the order in which containers will be used to create samples.</span>
		</td>
	</tr>
	<TR><TD COLSPAN="2" ALIGN="center">
	<TABLE BORDER="0">
	<TR>
		<TH>Container</TH>
		<TH>Qty Remaining</TH>
		<TH>Order of Use</TH>
	</TR>
	<%
	barcodeList = " "
	currContainerNum = 0
	While not RS.EOF 
		currContainerID = RS("container_id")
		currBarcode = RS("barcode")
		barcodeList = barcodeList & currBarcode & ","
		if numsourcePlates = 1 then
			theRow = "<span style=""display:none"">"
		else
			theRow = "<TR><TD ALIGN=""center"">" & currBarcode & "</TD>"
		end if
		theRow = theRow & "<TD ALIGN=""center"">" & RS("qty_remaining") & " " & RS("unit_abreviation") &  "</TD>"
		'theRow = "<TR><TD ALIGN=""left"">" & curPlateBarcode & ":" & curPlateID & "</TD>"
		theRow = theRow & "<TD ALIGN=""center"">"
		if numsourcePlates = 1 then
			i=0
				theRow = theRow & "<INPUT TYPE=""radio"" NAME=""Order" & i & """ VALUE=""" & currContainerID & """ CHECKED>" & (i+1)
		else
			sChecked = ""
			for i=0 to batchCount-1
				if i = currContainerNum then 
					sChecked = " CHECKED"
				else
					sChecked = "" 
				end if
				theRow = theRow & "<INPUT TYPE=""radio"" NAME=""Order" & i & """ VALUE=""" & currContainerID & """ ONCLICK=""ClearSelections(" & currContainerID & ", this, " & batchCount & "); """ & sChecked & ">" & (i+1)
			next
			currContainerNum = currContainerNum + 1
		end if
		if numsourcePlates = 1 then
			theRow = theRow & "</span>"
		else
			theRow = theRow & "</TD></TR>" & chr(13)
		end if
		Response.Write theRow
		RS.MoveNext
	wend
	barcodeList = left(barcodeList,len(barcodeList)-1)%>
	</TABLE>
	</TD></TR>
	<%end if%>
	<tr height="25">
<%if lcase(Application("RequireBarcode")) = "true" then%>
		<td align="right" valign="top" nowrap><span class="required">Barcode Description:</span></td>
<%else%>
		<td align="right" valign="top" nowrap>Barcode Description:</td>
<%end if%>	
		<td>
				<%=ShowSelectBox2("BarcodeDescID", BarcodeDescID,"SELECT barcode_desc_id AS Value, barcode_desc_name AS Displaytext FROM inv_barcode_desc ORDER BY lower(DisplayText) ASC", null," ","")%>
		</td>
	</tr>
	<tr height="25">
		<td align="right" valign="top" nowrap>
			<span class="required">Sample Location ID:</span>
		</td>
		<td colspan="3">
			&nbsp;<%=GetBarcodeIcon()%>&nbsp;<%ShowLocationPicker "document.form1", "LocationID", "lpLocationBarCode", "lpLocationName", 10, 20, false%> 
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
		<td align="right" nowrap>
			Sample Container Type:
		</td>
		<td>
			<input TYPE="text" SIZE="10" Maxlength="50" NAME="ContainerTypeName" STYLE="background-color:#d3d3d3;" VALUE="<%=ContainerTypeName%>" READONLY>
		</td>
	</tr>
	<tr>
		<td align="right" nowrap>
			Number of Samples:
		</td>
		<td>
			<input TYPE="text" SIZE="10" Maxlength="50" NAME="NumContainersDisplay" STYLE="background-color:#d3d3d3;" VALUE="<%=NumContainers%>" READONLY>
		</td>
	</tr>
	<%	
		arrValues = split(QtyList,",")
		'Response.Write GenerateFields("Sample ", CaptionSuffix, "Sample", NumContainers, arrValues)
		CaptionPrefix = "Sample "
		CaptionSuffix = "(" & UOMAbv & ")"
		NamePrefix = "Sample"
		NumFields = NumContainers
		html = ""
		for i = 1 to NumFields
			html = html &"<tr><td align=""right"" nowrap>" & CaptionPrefix & i & " " & CaptionSuffix & ":</td>"
			html = html & "<td><input TYPE=""text"" SIZE=""10""  STYLE=""background-color:#d3d3d3;"" Maxlength=""50"" NAME=""" & NamePrefix & i & """ VALUE=""" & arrValues(i-1) & """ READONLY></td></tr>"
		next
		Response.Write html
	%>
	<TR><TD COLSPAN="2" ALIGN="right"><A HREF="#" class="MenuLink" ONCLICK="OpenDialog('/Cheminv/GUI/RequestSample.asp?action=edit&RequestID=<%=RequestID%>&ContainerID=<%=ContainerID%>&ContainerBarcode=<%=ContainerBarcode%>&UOMAbv=<%=UOMAbv%>', 'Diag2',2)">Edit Request</A></TD></TR>
	<tr>
		<td colspan="2" align="right"> 
			<a HREF="#" onclick="window.close(); return false;"><img SRC="../graphics/cancel_dialog_btn.gif" border="0" WIDTH="61" HEIGHT="21"></a><a HREF="#" onclick="ValidateOrder(<%=batchCount%>); return false;"><img SRC="../graphics/btn_next_61.gif" border="0" WIDTH="61" HEIGHT="21"></a>
		</td>
	</tr>	
	
</table>	
</form>
</center>
</body>
</html>
<%
	Conn.Close
	Set Cmd = Nothing
	Set Conn = Nothing


%>