<%@ Language=VBScript %>
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/ado.inc"-->
<!--#INCLUDE VIRTUAL = "/cheminv/api/apiUtils.asp"-->
<!--#INCLUDE VIRTUAL = "/cheminv/gui/guiUtils.asp"-->
<%
Dim Conn
Dim Cmd
Dim RS
BatchID = Request("BatchID")
RequestID = Request("RequestID")
QtyRequired = Request("QtyRequired")
AllowBatchRequest = lcase(Request("AllowBatchRequest"))

MinStockThreshold = Request("MinStockThreshold")
AmountRemaining = Request("AmountRemaining")
AmountReserved =  Request("AmountReserved")

if AllowBatchRequest = "" then AllowBatchRequest = false

ContainerCnt = 0
if lCase(Application("RetireFulfilledContainers")) = "true" then
	retireContainer = " checked"
else
	retireContainer = ""
end if

If bWriteError then
	' Respond with Error
	Response.Write strError
	Response.end
End if

'if not IsEmpty(Application("DEFAULT_SAMPLE_REQUEST_CONC")) then
'	arrUOM = split(Application("DEFAULT_SAMPLE_REQUEST_CONC"),"=")
'	UOMAbbrv = arrUOM(1)
'end if


Response.Expires = -1
SampleRegID = ""
SampleBatchNumber = ""
Call GetInvCommand("{CALL " & Application("CHEMINV_USERNAME") & ".REQUESTS.GetBatchContainers(?,?)}", adCmdText)	
Cmd.Parameters.Append Cmd.CreateParameter("PBATCHID",131, 1, 0, BatchID)
Cmd.Parameters.Append Cmd.CreateParameter("pRequestID",131, 1, 0, RequestID)
if bDebugPrint then
	For each p in Cmd.Parameters
		Response.Write p.name & " = " & p.value & "<BR>"
	Next	
Else
	Cmd.Properties ("PLSQLRSet") = TRUE  
	Set RS = Cmd.Execute
	Cmd.Properties ("PLSQLRSet") = FALSE
	html = ""
	If (RS.EOF AND RS.BOF) then
		html = html & vbtab & "<tr><td align=center colspan=""" & colSpan & """><span class=""GUIFeedback"">No containers found</span></td></tr>" & vbcrlf
	Else
		While (Not RS.EOF)
			SampleRegID = RS("reg_id_fk")
			SampleBatchNumber = RS("batch_number_fk")
			ContainerCnt = ContainerCnt + 1
			BatchID = RS("batch_id")
			ContainerID = RS("container_id")
			Barcode = RS("barcode")
			QtyAvailable = RS("qty_remaining")
			QtyUnit = RS("uomAbbrv")
			Conc = RS("concentration")
			ConcUnit = RS("uocAbbrv")
			Location_Path = RS("Location_Path")
			RackPositionID = RS("location_id_fk")
			Rack_Path = RS("Rack_Path")
			Location_Name = RS("Location_Name")
			RackID = RS("RackID")
			isRack = RS("IsRack")
			if Rack_Path <> "" then
				DisplayPath = Rack_Path
			else
				DisplayPath = Location_Path
			end if
            '-- CSBR ID:123488
            '-- Change Done by : Manoj Unnikrishnan
            '-- Purpose: Display the previously saved values
            '-- Date: 08/04/2010
            Field_1 = RS("field_1")
            Field_2 = RS("field_2")
            Field_3 = RS("field_3")
            Field_4 = RS("field_4")
            Field_5 = RS("field_5")
            Date_1 = RS("date_1")
            Date_2 = RS("date_2")
            'End of Change #123488#
			html = html & vbtab & "<tr>" & vbcrlf
			html = html & vbtab & vbtab & "<td align=""right""><input type=""checkbox"" onclick=""updateQtySelected()"" name=""ContainerID"" value=""" & ContainerID & """></td>" & vbcrlf
			html = html & vbtab & vbtab & "<td align=""right""><span title="""">" & Barcode & "</span></td>" & vbcrlf
			html = html & vbtab & vbtab & "<td align=""right"">"
			if not isBlank(QtyAvailable) then html = html & QtyAvailable & "&nbsp;" & QtyUnit
			html = html & "</span></td>" & vbcrlf
			html = html & vbtab & vbtab & "<td align=""right"">"
			if not isBlank(Conc) then html = html & Conc & "&nbsp;" & ConcUnit
			html = html & "</td><td align=""right"">"
			if isNull(Conc) or Conc="" then Conc=1 
			if QtyAvailable <> "" and Conc <> "" then
				calcAmount = cDbl(QtyAvailable)*cDbl(Conc)
				html = html & calcAmount & " " & QtyUnit
				html = html & "<input type=""hidden"" name=""QtyAvailable" & ContainerID & """ value=""" & calcAmount & """>"
			else
				html = html & "<input type=""hidden"" name=""QtyAvailable" & ContainerID & """ value=""0"">"
			end if
			html = html & "<input type=""hidden"" name=""RackID" & ContainerID & """ value=""" & RackID & """>"
			html = html & "<input type=""hidden"" name=""RackPositionID" & ContainerID & """ value=""" & RackPositionID & """>"
			if isRack = "1" then
				html = html & "</td><td><span title=""" & Location_Path & """><a class=""MenuLink"" href=""#"" onclick=""OpenDialog('/cheminv/gui/ViewSimpleRackLayout.asp?rackid=" & RackID & "&locationid=" & RackPositionID & "&containerid=" & ContainerID & "&RackPath=" & server.urlencode(Location_Path) & "', 'Diag1', 2); return false;"">" & DisplayPath & "</a></span>"
			else
				html = html & "</td><td><span title=""" & Location_Path & """ style=""font-size:10px;"">" & Location_Path & "</span>"
			end if
			html = html & "</td><td>&nbsp;</td>" & vbcrlf
			html = html & vbtab & "</tr>" & vbcrlf
		rs.MoveNext
		Wend
end if
%>
<html>
<head>
<title><%=Application("appTitle")%> -- Add Container to Request</title>
<script LANGUAGE="javascript" src="/cheminv/Choosecss.js"></script>
<script LANGUAGE="javascript" src="/cheminv/utils.js"></script>
<script LANGUAGE="javascript" src="/cheminv/gui/validation.js"></script>
<script language="JavaScript">
<!--Hide JavaScript
	window.focus();
	
	function Validate(){
		var bWriteError = false;
		var errmsg = "Please fix the following problems:\r";
		var bWriteWarning = false;
		var warningmsg = "Please address the following warnings:\r\r";

		var QtyRequired = <%=QtyRequired%>;
		var ContainerCnt = <%=ContainerCnt%>;
		var Sum = document.form1.QtyAvailable.value;
		/*
		if (ContainerCnt == 1) {
			if (document.form1.ContainerID.checked) {
				Sum = Sum + parseInt(document.form1.QtyAvailable.value);
			}
		} else {
			for (i = 0; i < ContainerCnt ; i++) {
				if (document.form1.ContainerID[i].checked) {
					Sum = Sum + parseInt(document.form1.QtyAvailable[i].value);
				}
			}
		}
		*/
		//alert(Sum + ":" + QtyRequired);
		// Validate quantity for fulfillment
		if (Sum == 0) {
			errmsg = errmsg + "- Please select at least one Container to fulfill request.\r";
			bWriteError = true;
		}
		/*else if (Sum < QtyRequired){
			errmsg = errmsg + "- The quantity from the containers selected does not meet the amount requested.\rPlease select additional containers or Create New Samples.\r";
			bWriteError = true;
		}*/

		// Build JS validation for custom Request fields
		<%For each Key in req_custom_fulfillrequest_fields_dict%>
		if (document.form1.<%=Key%>.value.length == 0){
			errmsg = errmsg + "- <%=req_custom_fulfillrequest_fields_dict.item(Key)%> is a required field.\r";
			bWriteError = true;
		}
		<% next %>		
        /*CSBR ID:121959
		 *Change Done by : Manoj Unnikrishnan 
		 *Purpose: Validate the length of Custom Fulfill Request Fields
		 *Date: 24/02/2010
		*/
		// Build JS validation for validating the length of custom Fulfill Request fields not more than 2000
		<%For each Key in custom_fulfillrequest_fields_dict%>
		if (document.form1.<%=Key%>.value.length > 2000){
			errmsg = errmsg + "- <%=custom_fulfillrequest_fields_dict.item(Key)%> cannot be greater than 2000 characters.\r";
			bWriteError = true;
		}
		<% next %>
		/*End of Change #121959#*/	

		if (Sum > QtyRequired){
			warningmsg = warningmsg + "- The total sample amount for the selected containers is more than the amount requested.\rDo you wish to continue?\r\r";
			bWriteWarning = true;
		}

		if (Sum < QtyRequired){
			warningmsg = warningmsg + "- The quantity from the containers selected is less than the amount requested.\rDo you wish to continue?\r\r";
			bWriteWarning = true;
		}

		var MinStockThreshold = Math.round(eval(document.form1.MinStockThreshold.value)*100)/100;
		var AmountRemaining = Math.round(eval(document.form1.AmountRemaining.value-Sum)*100)/100;
		var AmountReserved = <% if AmountReserved <> "" then response.write "Math.round(eval(" & AmountReserved & ")*100)/100" else response.write 0 end if %>;

		//alert(MinStockThreshold +":"+ AmountRemaining + ":" + AmountReserved);
		if (MinStockThreshold > AmountRemaining ) {
			warningmsg = warningmsg + "- You have selected sample that exceeds the minimum stock threshold of " + MinStockThreshold + " for this batch.\r\r";
			bWriteWarning = true;
		}
		if (AmountReserved > AmountRemaining ) {
			warningmsg = warningmsg + "- You have selected sample that exceeds the amount reserved of " + AmountReserved + " for this batch.\r\r";
			bWriteWarning = true;
		}

		Sum = Math.round(eval(Sum)*100)/100;
		var CompareVal = 0;
		if (Sum > AmountRemaining){
			CompareVal = Sum;
		}else{
			CompareVal = Math.round(eval(AmountRemaining-Sum)*100)/100;
		}
		if (CompareVal <= MinStockThreshold) {
			//warningmsg = warningmsg + "- The amount you are fulfilling will make the amount available \rfall below the minimum threshold of " + MinStockThreshold + ".\rDo you wish to continue?\r\r";
			//bWriteWarning = true;
		}
		//bWriteError = true;
		if (bWriteError){
			alert(errmsg);
		} else {
			var bcontinue = true;
			// Report warnings, user can choose to accept or cancel
			bConfirmWarning = true;
			if (bWriteWarning) {
				bConfirmWarning = confirm(warningmsg);
			}
			if (!bConfirmWarning) var bcontinue = false;
			//bcontinue = false;
			if (bcontinue) document.form1.submit();
			
		}	
	}
	function updateQtySelected(){
		var qtySelected = 0;
		var RackIDList = "";
		var ContainerIDList = "";
		var ContainerGridPositionIDList = "";
		if (document.form1.ContainerID.length){
			for (i=0;i<document.form1.ContainerID.length;i++){
				if (document.form1.ContainerID[i].checked){
					currQty = eval("document.form1.QtyAvailable" + document.form1.ContainerID[i].value + ".value")
					rackID = eval("document.form1.RackID" + document.form1.ContainerID[i].value + ".value")
					rackPositionID = eval("document.form1.RackPositionID" + document.form1.ContainerID[i].value + ".value")
					containerID = document.form1.ContainerID[i].value;
					if (currQty > 0){
						qtySelected = eval(qtySelected) + eval(currQty);
						if (rackID.length > 0){
							rackExists = parseInt(RackIDList.indexOf(rackID));
							if (rackExists == -1){
								RackIDList = rackID + ", " + RackIDList;
							}
						}
						containerExists = parseInt(ContainerIDList.indexOf(containerID));
						if (containerExists == -1){
							ContainerIDList = containerID + ", " + ContainerIDList;
						}
						containerGridExists = parseInt(ContainerGridPositionIDList.indexOf(containerID));
						if (containerGridExists == -1){
							ContainerGridPositionIDList = containerID + "::" + rackPositionID + ", " + ContainerGridPositionIDList;
						}
					}
				}
			}
		} else {
			if (document.form1.ContainerID.checked){
				currQty = eval("document.form1.QtyAvailable" + document.form1.ContainerID.value + ".value")
				rackID = eval("document.form1.RackID" + document.form1.ContainerID.value + ".value")
				rackPositionID = eval("document.form1.RackPositionID" + document.form1.ContainerID.value + ".value")
				containerID = document.form1.ContainerID.value;
				if (currQty > 0){
					qtySelected = eval(qtySelected) + eval(currQty);
					if (rackID.length > 0){
						rackExists = parseInt(RackIDList.indexOf(rackID));
						if (rackExists == -1){
							RackIDList = rackID + ", " + RackIDList;
						}
					}
					containerExists = parseInt(ContainerIDList.indexOf(containerID));
					if (containerExists == -1){
						ContainerIDList = containerID + ", " + ContainerIDList;
					}
					containerGridExists = parseInt(ContainerGridPositionIDList.indexOf(containerID));
					if (containerGridExists == -1){
						ContainerGridPositionIDList = containerID + "::" + rackPositionID + ", " + ContainerGridPositionIDList;
					}
				}
			}
		}
		document.form1.RackIDList.value = RackIDList;
		document.form1.ContainerIDList.value = ContainerIDList;
		document.form1.ContainerGridPositionIDList.value = ContainerGridPositionIDList;
		document.form1.QtyAvailable.value = qtySelected;
		var dispText = Math.round(eval(qtySelected)*100)/100 + " <%=QtyUnit%>";
		changeContent(document.all.amountSelected,dispText);
	}
	
	function validateAliquot(){
		var cntContainers = 0;
		var strContainerIDList = "";
		var defLocationID = "";
		if (document.form1.ContainerID.length){
			for (i=0;i<document.form1.ContainerID.length;i++){
				if (document.form1.ContainerID[i].checked){
					if (strContainerIDList.length > 0){
						strContainerIDList = strContainerIDList + "," + document.form1.ContainerID[i].value;
					} else {
						strContainerIDList = strContainerIDList + document.form1.ContainerID[i].value;
					}
					cntContainers = cntContainers + 1;
					if (defLocationID.length == 0) {
						defLocationID = eval("document.form1.RackID" + document.form1.ContainerID[i].value + ".value")
					}
				}
			}
		} else {
			if (document.form1.ContainerID.checked){
				if (strContainerIDList.length > 0){
					strContainerIDList = strContainerIDList + "," + document.form1.ContainerID.value;
				} else {
					strContainerIDList = strContainerIDList + document.form1.ContainerID.value;
				}
				cntContainers = cntContainers + 1;
				if (defLocationID.length == 0) {
					defLocationID = eval("document.form1.RackID" + document.form1.ContainerID.value + ".value")
				}
			}
		}
		if (cntContainers == 0){
			alert("Please select at least one sample to aliquot");
		}else{
		    //-- CSBR ID:122385
		    //-- Change Done by : Manoj Unnikrishnan
		    //-- Purpose: If using Re-Aliquot Samples we need to use the CreateSamples workflow; This is to withhold the user from duplicating the fulfill request
		    //-- Date: 14/03/2010
		    window.location.href = '/Cheminv/GUI/CreateSamplesFromBatch.asp?Action=new&batchid=<%=BatchID%>&DefLocationID=' + defLocationID + '&RequestID=<%=RequestID%>&SampleRegID=<%=SampleRegID%>&SampleBatchNumber=<%=SampleBatchNumber%>&ContainerIDList='+strContainerIDList; return false;
		    /*End of Change #122385#*/	
		}		
	}
	
-->
</script>

</head>
<body>
<center>

<% if AllowBatchRequest = "false" then %>

	<br /><br /><br /><br />
	<span class="GuiFeedback">This batch is not dispensable.</span><br />
	Reason: Pending Certificate of Testing (COT)
	<br><br>
	<a HREF="#" onclick="window.close(); return false;"><img SRC="/cheminv/graphics/sq_btn/ok_dialog_btn.gif" border="0"></a>

<% else %>

<form name="form1" action="../gui/AddBatchToRequest_action.asp" method="POST">
<input Type="hidden" name="BatchID" value="<%=BatchID%>">
<input Type="hidden" name="RequestID" value="<%=RequestID%>">
<input Type="hidden" name="RackIDList" value>
<input Type="hidden" name="ContainerIDList" value>
<input Type="hidden" name="ContainerGridPositionIDList" value>
<input type="hidden" name="QtyAvailable" value>
<input type="hidden" name="QtyRequired" value="<%=QtyRequired%>">
<input type="hidden" name="MinStockThreshold" value="<%=MinStockThreshold%>">
<input type="hidden" name="AmountRemaining" value="<%=AmountRemaining%>">

<table border="0" cellpadding="0">
	<tr><td colspan="2">
		<span class="GuiFeedback">Add Container to Request.</span><br><br>
	</td></tr>
	<tr><td colspan="2" align="right">
		<!--<a class="MenuLink" HREF="Create Container Samples" onclick="OpenDialog('../gui/CreateContainerSample.asp?Action=new&amp;', 'ConDiag', 2); return false" target="_top">New Samples</a> | -->
		<a class="MenuLink" Href="#" onclick="validateAliquot()" title="Create New Samples">Re-Aliquot Samples from Batch</a>
	</td></tr>
	<tr><td>
	<table border="0" cellspacing="3">
	<tr><td colspan="2">
		<div style="height:300px; overflow: auto;">	
		<table border="0" cellspacing="5" cellpadding="1">
		<tr>
			<th>&nbsp;</th>
			<th>Barcode</th>
			<th>Qty Remaining</th>
			<th>Conc</th>
			<th>Amount</th>
			<th>Location</td>
			<td>&nbsp;&nbsp;</td>
		</tr>
		<%=html%>
		</table>
		</div>	
	</td></tr>
	<tr><td colspan="2"><hr noshade size="1"></td></tr>
	<tr><td align="right">Requested Amount:</td><td><strong><%=QtyRequired%>&nbsp;<%=QtyUnit%></td></tr>
	<tr><td align="right">Selected Amount:</td><td><strong><div id="amountSelected">0 <%=QtyUnit%></div></td></tr>
	<tr><td colspan="2">&nbsp;</td></tr>
	<tr><td align="right"><input type="checkbox" name="RetireContainer" <%=retireContainer%>></td><td><strong>Retire selected container(s)?</strong></td></tr>
	<%
	'-- Display customer Request fields: custom_createrequest_fields_dict, req_custom_createrequest_fields_dict
	For each Key in custom_fulfillrequest_fields_dict
		Response.write("<tr>" & vbcrlf)
		Response.write(vbtab & "<td align=""right"" nowrap>" & vbcrlf)
		For each Key1 in req_custom_fulfillrequest_fields_dict
			if Key1 = Key then
				Response.Write("<span class=""required"">" & vbcrlf)
			end if
		Next
		Response.Write(custom_fulfillrequest_fields_dict.item(key) & ":</span></td>" & vbcrlf)
		execute("FieldValue = " & Key)
	'-- CSBR ID:121959
	'-- Change Done by : Manoj Unnikrishnan
	'-- Purpose: To show the Custom Fullfill Request field as a text area
	'-- Date: 24/02/2010
	'---CSBR ID : 123277 & 144631 SJ 02/09/2011
	'---Comment  : Changing the textarea wrap property to 'soft' so that the string will not contain any carriage return sent to the server.
		Response.write(vbtab & "<td ><textarea rows=""5"" cols=""30"" wrap=""soft"" name=""" & key & """>" & FieldValue & "</textarea></td>" & vbcrlf)
	'-- End of Change #121959#
		Response.write("</tr>" & vbcrlf)	
	Next	
	%>
	</table>
	
	</td></tr>		
	<!-- CSBR ID:121959 -->
	<!-- Change Done by : Manoj Unnikrishnan -->
	<!-- Purpose: To show the Edit Request Link -->
	<!-- Date: 24/02/2010 -->
	<tr><td COLSPAN="2" ALIGN="right"><a HREF="#" class="MenuLink" ONCLICK="OpenDialog('/Cheminv/GUI/RequestBatch.asp?action=edit&amp;RequestID=<%=RequestID%>&amp;BatchID=<%=BatchID%>&amp;ContainerBarcode=<%=""%>&amp;UOMAbv=<%=QtyUnit%>', 'Diag2',2)">Edit Request</a></td></tr>
	<!-- End of Change #121959# -->
	<tr>
		<td colspan="2" align="right"> 
			<a href="#" onclick="window.close(); return false;"><img SRC="/cheminv/graphics/sq_btn/cancel_dialog_btn.gif" border="0"></a>
			<a href="#" onclick="Validate(); return false;"><input type="image" SRC="/cheminv/graphics/sq_btn/ok_dialog_btn.gif" border="0"></a>
		</td>
	</tr>	
<%	
	End if
	RS.Close
	Conn.Close
	Set RS = nothing
	Set Cmd = nothing
	Set Conn = nothing
%>

</table>	
</form>

<% end if %>


</center>
</body>
</html>
