<%@ Language=VBScript %>
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/ado.inc"-->
<!--#INCLUDE VIRTUAL = "/cheminv/gui/guiUtils.asp"-->
<!--#INCLUDE VIRTUAL = "/cheminv/api/apiUtils.asp"-->
<%
Response.Expires = -1

Dim Conn
Dim Cmd
Dim RS
Dim bDebugPrint
bDebugPrint = false
bLoad = true

InvSchema = Application("CHEMINV_USERNAME")
dbkey = "cheminv"

clear = Request.QueryString("clear")
Message = Request("message")
RequestID = Request("RequestID")
LocationID = Request("DeliveryLocationID")
ShipToName = Request("ShipToName")
OrderID = Request("OrderID")
OrderStatusID = Request("OrderStatusID")
'Response.Write OrderID
'Response.End
'if (isEmpty(OrderID)) then OrderID = null
'if not IsEmpy(OrderID) then clear = 0

GetInvConnection()
if len(clear) = 0 then clear = 0 
Set myDict = multiSelect_dict

if clear then
	myDict.RemoveAll
	'Response.Redirect "order_list.asp?message=" & Message
Else
	AddContainerID = Request("AddContainerID")
	if len(AddContainerID) > 0 then
		bLoad = false
		'verify that this container isn't already in an either new or shipped order
		SQL = "SELECT count(oc.container_id_fk) as Count FROM inv_order_containers oc, inv_orders o WHERE container_id_fk = ? AND order_id_fk = order_id AND order_status_id_fk not in (3,4)"
		'Response.Write SQL & ":" & AddContainerID
		Set Cmd = GetCommand(Conn, SQL, adCmdText)
		Cmd.Parameters.Append Cmd.CreateParameter("AddContainerID", 5, 1, 0, AddContainerID)
		Set RS = server.CreateObject("ADODB.recordset")
		RS.Open Cmd

		Count = 0
		if not (RS.BOF or RS.EOF) then Count = RS("Count")
		if cint(Count) = 0 then
			'add the container to the order
			if NOT myDict.Exists(Trim(AddContainerID)) then	myDict.Add Trim(AddContainerID), true
		else
			'error message
			msg = msg & "Container not added.  It is part of another order.<BR>"
		end if
	End if
	RmvContainerID = Request("RmvContainerID")
	if len(RmvContainerID) > 0 then
		bLoad = false
		if myDict.Exists(Trim(RmvContainerID)) then	myDict.Remove(Trim(RmvContainerID))
	End if
End if

if not isEmpty(OrderID) and OrderID <> "" and bLoad then
	Set Cmd = GetCommand(Conn, InvSchema & ".REQUESTS.GETORDER", 4)		 
	Cmd.Parameters.Append Cmd.CreateParameter("ORDERID", 5, 1, 0, OrderID) 
	if bdebugPrint then
		Response.Write "Parameters:<BR>"
		For each p in Cmd.Parameters
			Response.Write p.name & " = " & p.value & "<BR>"
		Next	
		Response.end
	else
		Cmd.Properties ("PLSQLRSet") = TRUE  
		Set RS = Cmd.Execute
		Cmd.Properties ("PLSQLRSet") = FALSE
		if NOT (RS.EOF OR RS.BOF) then
			LocationID = RS("delivery_location_id_fk")
			ShipToName = RS("ship_to_name")
			SampleContainerIDs = RS("SampleContainerIDs")
			OrderStatusID = RS("Order_Status_ID_FK")
			ShippingConditions = RS("Shipping_Conditions")
			CancelReason = RS("Cancel_Reason")
			'Response.Write SampleContainerIDs

			if not isNull(SampleContainerIDs) then		
				arrSampleContainerIDs = split(SampleContainerIDs,",")
				for i = 0 to ubound(arrSampleContainerIDs)
					currContainerID = cint(arrSampleContainerIDs(i))
					'Response.Write DictionaryToList(myDict) & "<BR>"
					'Response.Write currContainerID & "<BR>"
					'Response.Write myDict.Exists(currContainerID) & "<BR>"
					'Response.Write myDict.Exists(cint(currContainerID)) & "<BR>"
					if not myDict.Exists(cstr(currContainerID)) then myDict.Add cstr(currContainerID), true
					'RS.MoveNext
				next
			end if
			'Response.Write SampleContainerIDs
			'Response.End
		else
			Response.Write ""
		end if
	end if

elseif not IsEmpty(RequestID) and RequestID <> "" and bLoad then
	'add samples associated with this request to the order, only if the container isn't in another order
	SQL = "SELECT container_id_fk FROM inv_request_samples rs WHERE rs.container_id_fk not in (select container_id_fk from inv_order_containers) AND request_id_fk = ?"
	'Response.Write SQL & ":" & RequestID
	Set Cmd = GetCommand(Conn, SQL, adCmdText)
	Cmd.Parameters.Append Cmd.CreateParameter("RequestID", 5, 1, 0, RequestID)
	Set RS = server.CreateObject("ADODB.recordset")
	RS.Open Cmd

	While not (RS.BOF or RS.EOF)
		rsContainerID = RS("Container_ID_FK")
		if not myDict.Exists(cstr(rsContainerID)) then myDict.Add cstr(rsContainerID), true
		RS.MoveNext
	wend
end if

bEdit = false
if OrderStatusID = "1" or isEmpty(OrderID) or OrderID = "" then
	bEdit = true
end if

%>
<html>
<head>
<script LANGUAGE="javascript" src="/cheminv/Choosecss.js"></script>
<script LANGUAGE="javascript" src="/cheminv/utils.js"></script>
<script LANGUAGE="javascript" src="/cheminv/gui/validation.js"></script>
<script language="JavaScript">
<!--Hide JavaScript
	var DialogWindow;
	window.focus();
	function UpdateAddress(value)
	{
		//handle the page load case

		if (document.form1.isLoad.value == 'false'){
			if (top.ScanFrame) {
				top.ScanFrame.document.form1.LocationID.value = value;		
				//also update the ship to name
				parent.ScanFrame.document.form1.ShipToName.value = document.form1.ShipToName.value;
				//also update the order status
				parent.ScanFrame.document.form1.OrderStatusID.value = document.form1.OrderStatusID.value;
				//alert(top.ScanFrame.document.form1.LocationID.value);
				//alert(top.ScanFrame.document.form1.ShipToName.value);
				//alert(document.form1.ShipToName.value);
			}
			
			//alert(value);
			if (value == ""){
				document.form1.AddressID.value = "";
				document.form1.Address1.value = "";
				document.form1.Address2.value = "";
				document.form1.Address3.value = "";
				document.form1.Address4.value = "";
				document.form1.CityState.value = "";
				document.form1.Country.value = "";
			
				document.all.EditAddressLink.style.display = "none";
			}
			else {
				var strURL = "/cheminv/api/GetLocationAddress.asp?locationid=" + value;
				//alert(strURL);
				var httpResponse = JsHTTPGet(strURL); 
				//alert(httpResponse);

				document.all.EditAddressLink.style.display = "block";
				
				if (httpResponse.length>0) {
					arrAddress = httpResponse.split("::");
					document.form1.AddressID.value = arrAddress[0];
					document.form1.Address1.value = arrAddress[2];
					document.form1.Address2.value = arrAddress[3];
					document.form1.Address3.value = arrAddress[4];
					document.form1.Address4.value = arrAddress[5];
					document.form1.CityState.value = arrAddress[6] + ', ' + arrAddress[14] + ' ' + arrAddress[9];
					document.form1.Country.value = arrAddress[15];
				}				
				/*
				0=ADDRESS_ID
				1=Contact_Name
				2=ADDRESS1
				3=ADDRESS2
				4=ADDRESS3
				5=ADDRESS4
				6=CITY
				7=STATE_ID_FK
				8=a.COUNTRY_ID_FK
				9=ZIP 
				10=FAX
				11=PHONE
				12=EMAIL
				13=State_Name
				14=State_Abbreviation
				15=Country_Name		
				*/
			}
		}
		else 
		{
			document.form1.isLoad.value = 'false';
		}
		
		//http://dpdev2/cheminv/api/GetLocationAddress.asp?locationid=0	
	}

	function Validate()
	{
		var bWriteError = false;
		var errmsg = "Please fix the following problems:\r";

		//delivery location is required
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
		<%if (OrderStatusID="1" or OrderStatusID="") then%>
		//ship to name is required
		if (document.form1.ShipToName.value.length == 0) {
			errmsg = errmsg + "- Ship to is required.\r";
			bWriteError = true;
		}
		<%end if%>

		if (bWriteError){
			alert(errmsg);
		}
		else{
			document.form1.submit();	
			/*
			if (pageAction=="page1")
				{formAction = "#";}
			else
				{formAction = "request_action.asp?action=<%=action%>";}
			document.form1.action = formAction;
			document.form1.submit();
			*/
		}
	}
-->
</script>

</head>
<body>
<center>
<table BORDER="0" CELLSPACING="0" CELLPADDING="0">
<tr><td>
<table border="0" cellspacing="0" cellpadding="2" width="100%" align="left">
	<tr>
		<td align="right" valign="top" nowrap>
			<%if bEdit then%>
				<%if isEmpty(OrderID) or OrderID = "" then%>
				<a class="MenuLink" HREF="#" onclick="document.form1.action = 'CreateOrEditOrder_action.asp'; document.form1.Action.value='save';Validate(); return false">Create Order</a>
				<%elseif not isEmpty(OrderID) and OrderID <> "" then %>
				<a class="MenuLink" HREF="#" onclick="document.form1.action = 'CreateOrEditOrder_action.asp'; document.form1.Action.value='save';Validate(); return false">Update Order</a>
				|
				<a class="MenuLink" HREF="#" onclick="document.form1.action = 'ShipOrder_action.asp'; document.form1.Action.value='ship'; document.form1.submit(); return false">Ship Order</a>
				<%elseif myDict.Count > 0 then%>
				|
				<a class="MenuLink" HREF="Order_list.asp?clear=1" target="ListFrame">Clear List</a>			
				<%end if%>
			<%end if%>
		</td>
	</tr>
</table>
</td></tr>
<%if myDict.Count > 0 then%>
<tr><td>
<br clear="all"><br>
<%
	'GetInvConnection()
	Set Cmd = GetCommand(Conn, InvSchema & ".GUIUTILS.GETKEYCONTAINERATTRIBUTES", 4)		 
	Cmd.Parameters.Append Cmd.CreateParameter("CONTAINERIDLIST", 200, 1, 2000, DictionaryToList(myDict)) 
	Cmd.Parameters.Append Cmd.CreateParameter("CONTAINERBARCODELIST", 200, 1, 2000, NULL) 
	if bdebugPrint then
		Response.Write "Parameters:<BR>"
		For each p in Cmd.Parameters
			Response.Write p.name & " = " & p.value & "<BR>"
		Next
			Response.write Session("awolContainerBarcodeList") & "<BR>"	
		'Response.end
	else
		Cmd.Properties ("PLSQLRSet") = TRUE  
		'Get AwolContainer Attributes
		Set RS = Cmd.Execute
		Cmd.Properties ("PLSQLRSet") = FALSE
		If NOT (RS.EOF AND RS.BOF) then
			temparr = RS.GetRows()
			RecordCount = Ubound(temparr,2) + 1
			RS.MoveFirst
		Else
			RecordCount = 0
		End if
	end if
%>
<table border="1" bordercolor="666666" cellpadding="1" cellspacing="0">
	<tr>
		<td>
			<table border="0">
			<tr height="40">
				<td colspan="3">
					<font size="1">Click on a link above to perform an action on <br>all containers on the list.</font>
				</td>
				
				<th colspan="3" align="center">
					&nbsp;
				</th>
			</tr>
			<tr>
				<th>
					Barcode
				</th>
				<th>
					Container Name
				</th>
				<th>
					Location
				</th>
				<th>
					User
				</th>
				<th>
					Qty Remaining
				</th>
				<%if bEdit then%>
				<th>
					Remove?
				</th>
				<%end if%>
			</tr>
		<%
			If (RS.EOF AND RS.BOF) then
				Response.Write ("<TR><TD align=center colspan=6><span class=""GUIFeedback"">No containers</Span></TD></tr>")
			Else
				While (Not RS.EOF)
					ContainerID = RS("Container_ID")
					ContainerBarcode = RS("barcode")
					ContainerName = RS("Container_Name")
					LocationName = RS("Location_Name")
					CurrentUserID = RS("User_ID")
					QtyRemaining = RS("Qty_Remaining") & " " & RS("Unit_Abreviation")
					Path = RS("Path")
		%>
					<tr>
						<td align="center">
							<%=ContainerBarcode%>
						</td>
						<td align="right"> 
							<%=TruncateInSpan(ContainerName, 15, "")%>
						</td>
						<td align="center"> 
							<span title="<%=Path%>"><%=LocationName%></span> 
						</td>
						<td align="center">
							<%=CurrentUserID%>
						</td>
						<td align="center">
							<%=QtyRemaining%>
						</td>
							<%if bEdit then%>
						<td align="center">
							<a class="MenuLink" title="Remove this container from the selection list" HREF="#" ONCLICK="document.location = 'order_list.asp?OrderID=<%=OrderID%>&amp;RmvContainerID=<%=ContainerID%>&amp;DeliveryLocationID=' + document.form1.LocationID.value + '&amp;ShipToName=' + document.form1.ShipToName.value + '&amp;OrderStatusID=' + document.form1.OrderStatusID.value;">Remove</a>
						</td>
							<%end if%>
					</tr>
					<%rs.MoveNext
				Wend
				Response.Write "</table></center>"
			End if
			RS.Close
			Conn.Close
			Set RS = nothing
			Set Cmd = nothing
			Set Conn = nothing
			%>
		</td>
	</tr>
</table>
</td></tr>
<%end if%>
<tr><td>
<form name="form1" action method="POST">
<input TYPE="hidden" NAME="isLoad" VALUE="true">
<input TYPE="hidden" NAME="AddressID" VALUE>
<input TYPE="hidden" NAME="SampleContainerIDs" VALUE="<%=DictionaryToList(myDict)%>">
<input TYPE="hidden" NAME="Action" VALUE>
<input TYPE="hidden" NAME="bUpdateAddress" VALUE="true">
<input TYPE="hidden" NAME="OrderID" VALUE="<%=OrderID%>">
<input TYPE="hidden" NAME="OrderStatusID" VALUE="<%=OrderStatusID%>">
<script LANGUAGE="JavaScript">
	//parent.ScanFrame.document.form1.ShipToName.value = "<%=ShipToName%>";
	//alert(parent.ScanFrame.document.form1.LocationID.value);
</script>

<table border="0">
<%
	if myDict.count = 0 then 
		msg = msg & "Scan container barcodes to add them to the selection list."
	Elseif myDict.count > 1 then
		msg = msg & "There are " & myDict.Count & " containers in the selection list."
	End if
	Response.Write "<TR><TD COLSPAN=""2"" ALIGN=""center""><BR><center><span class=""GUIFeedback"">" & Request("message") & "<BR>" & msg &  "<BR><BR></span></center></TD></TR>"
	
%>
	<%if len(OrderID) > 0 then%>
	<tr><td ALIGN="RIGHT" COLSPAN="2"><a CLASS="MenuLink" HREF="#" ONCLICK="OpenDialog('/cheminv/Gui/CreateReport_frset.asp?isCustomReport=1&amp;ReportTypeID=4', 'ReportDiag', 2);">Print Shipping Label</a></td></tr>
	<%end if%>
	<%if bEdit then%>
	<tr>
		<td align="right" nowrap>
			<span class="required" title="Location where the samples should be delivered to">Delivery Location: <%=GetBarcodeIcon()%></span>
		</td>
		<td>
		<%'LocationID = Session("LocationID")%>
			<%ShowLocationPicker3 "document.form1", "LocationID", "lpLocationBarCode", "lpLocationName", 10, 30, false, "UpdateAddress(this.value);"%> 
		</td>
	</tr>
	<%else%>
	<tr>
		<td align="right" nowrap>
			Delivery Location: <img align="absmiddle" border="0" src="/cheminv/graphics/barcode_icon.gif">
		</td>
		<td>
		<input type="hidden" Value name="LocationID">
		<input TYPE="Text" SIZE="10" Value name="lpLocationBarCode" onchange="UpdateLocationPickerFromBarCode(this.value, document.form1,'LocationID','lpLocationBarCode', 'lpLocationName')" <%=IIF((OrderStatusID="1" or OrderStatusID=""),"","STYLE=""background-color:#d3d3d3;"" READONLY")%>>
		<input type="text" name="lpLocationName" size="30" value disabled>
		<script language="javascript">
			UpdateLocationPickerFromID('<%=LocationID%>',document.form1,'LocationID','lpLocationBarCode', 'lpLocationName'); 
		</script> 
		</td>
	</tr>
	<%end if%>
	<tr>
		<td align="right" nowrap>Shipping Conditions:</td>
		<td>
			<input TYPE="text" SIZE="30" Maxlength="50" NAME="ShippingConditions" VALUE="<%=ShippingConditions%>" <%=IIF((OrderStatusID="1" or OrderStatusID=""),"","STYLE=""background-color:#d3d3d3;"" READONLY")%>>
		</td>
	</tr>	
	<tr>
		<td align="right" nowrap>
			<%=IIF((OrderStatusID="1" or OrderStatusID=""),"<span class=""required"">Ship To:</span>","Ship To:")%>
		</td>
		<td>
			<input TYPE="text" SIZE="30" Maxlength="50" NAME="ShipToName" VALUE="<%=ShipToName%>" <%=IIF((OrderStatusID="1" or OrderStatusID=""),"","STYLE=""background-color:#d3d3d3;"" READONLY")%>>
		</td>
	</tr>	
	<tr>
		<td align="right" nowrap>
			Address1:
		</td>
		<td>
			<table cellspacing="0" cellpadding="0" border="0">
			<tr>
				<td><input TYPE="text" SIZE="30" Maxlength="50" NAME="Address1" STYLE="background-color:#d3d3d3;" READONLY>&nbsp;</td>
				<td><span id="EditAddressLink" style="display:none;">
				<%if bEdit then%>
					<a CLASS="MenuLink" HREF="Edit%20Address" onclick="OpenDialog('/cheminv/gui/EditAddress.asp?TableName=inv_locations&amp;TablePKID=' +  document.form1.LocationID.value + '&amp;AddressID=' + document.form1.AddressID.value,'AddressDiag', 4); return false;">Edit Address</a>			
				<%end if%>				
				</span></td>
			</tr>
			</table>
			
		</td>
	</tr>
	<tr>
		<td align="right" nowrap>
			Address2:
		</td>
		<td>
			<input TYPE="text" SIZE="30" Maxlength="50" NAME="Address2" STYLE="background-color:#d3d3d3;" READONLY>
		</td>
	</tr>
	<tr>
		<td align="right" nowrap>
			Address3:
		</td>
		<td>
			<input TYPE="text" SIZE="30" Maxlength="50" NAME="Address3" STYLE="background-color:#d3d3d3;" READONLY>
		</td>
	</tr>
	<tr>
		<td align="right" nowrap>
			Address4:
		</td>
		<td>
			<input TYPE="text" SIZE="30" Maxlength="50" NAME="Address4" STYLE="background-color:#d3d3d3;" READONLY>
		</td>
	</tr>
	<tr>
		<td align="right" nowrap>
			City,State Postal Code:
		</td>
		<td>
			<input TYPE="text" SIZE="30" Maxlength="50" NAME="CityState" STYLE="background-color:#d3d3d3;" READONLY>
		</td>
	</tr>
	<tr>
		<td align="right" nowrap>
			Country:
		</td>
		<td>
			<input TYPE="text" SIZE="30" Maxlength="50" NAME="Country" STYLE="background-color:#d3d3d3;" READONLY>
		</td>
	</tr>
	<%if OrderStatusID = "4" then%>
		<tr>
			<td align="right">
				Reason for Cancel:
			</td>
			<td>
				<textarea rows="5" cols="32" name="CancelReason" wrap="hard" DISABLED><%=CancelReason%></textarea>
			</td>
		</tr>
	<%end if%>
	<%if bEdit then%>
	<script language="JavaScript">
		//Update the address when the page loads
		UpdateAddress(document.form1.LocationID.value);
	</script>
	<%else%>
	<script language="JavaScript">
		document.form1.isLoad.value = 'false';
		UpdateAddress(<%=LocationID%>);
	</script>
	<tr>
		<td colspan="2" align="right"> 
			<a HREF="#" onclick="if(top.ScanFrame){top.window.close();}else{window.close();} return false;"><img SRC="../graphics/ok_dialog_btn.gif" border="0" WIDTH="61" HEIGHT="21"></a>
		</td>
	</tr>	
	<%end if%>
</table>
</form>
</td></tr>
</table>
</center>
<%
	Set Session("multiSelectDict") = myDict
	Set myDict = Nothing
%>
</body>
</html>
