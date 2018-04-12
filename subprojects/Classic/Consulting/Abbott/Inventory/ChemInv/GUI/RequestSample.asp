<%@ Language=VBScript %>
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/ado.inc"-->
<!--#INCLUDE VIRTUAL = "/cheminv/api/apiUtils.asp"-->
<!--#INCLUDE VIRTUAL = "/cheminv/gui/guiUtils.asp"-->
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/display_func_vbs.asp"-->
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/utility_func_vbs.asp"-->
<%
Dim Conn
Dim Cmd
Dim RS

action = lcase(Request("action"))
pageAction = lcase(Request("pageAction"))
RequestID = Request("RequestID")
ContainerID = Request("ContainerID")
BatchID = Request("BatchID")
ContainerBarcode = Request("ContainerBarcode")
UOMAbv = Request("UOMAbv")
cLocationID = Request("LocationID")
DateRequired = Request("DateRequired")
UserID = Request("UserID")
NumContainers = Request("NumContainers")
SampleQty = Request("SampleQty")
ContainerTypeID = Request("ContainerTypeID")
ExpenseCenter = Request("ExpenseCenter")
Family = Request("Family")
if cLocationID = "" then cLocationID = 0
if pageAction = "" then pageAction = "page1"
LocationID = Session("CurrentLocationID")
if ContainerBarcode = "" Then ContainerBarcode = Session("barcode")
dateFormatString = Application("DATE_FORMAT_STRING")

Select Case action
	Case "create"
		Caption = "Request delivery of samples from this container."
		'DateRequired = Month(Now()) & "/" & Day(Now()) & "/" & Year(Now())
		if pageAction = "page1" then
			DateRequired = Today
			QtyRequired = Request("QtyRequired")
			LocationID = Session("DefaultLocation")
			UserID = Ucase(Session("UserNameChemInv"))
			ContainerTypeID = Session("ContainerTypeID")
			Family = Session("Family")
		end if
	Case "edit"
		Caption = "Edit this request"
		if BatchID <> "" then
			Call GetInvCommand("{CALL " & Application("CHEMINV_USERNAME") & ".REQUESTS.GetBatchRequest(?,?)}", adCmdText)	
		else
			Call GetInvCommand("{CALL " & Application("CHEMINV_USERNAME") & ".REQUESTS.GetRequest(?,?)}", adCmdText)	
		end if
		Cmd.Parameters.Append Cmd.CreateParameter("PREQUESTID",131, 1, 0, RequestID)
		Cmd.Parameters("PREQUESTID").Precision = 9	
		Cmd.Parameters.Append Cmd.CreateParameter("PDATEFORMAT",200,1,30, dateFormatString)		
		Cmd.Properties ("PLSQLRSet") = TRUE  
		Set RS = Cmd.Execute
		Cmd.Properties ("PLSQLRSet") = FALSE
		If (RS.EOF AND RS.BOF) then
			Response.Write ("<table><TR><TD align=center colspan=6><span class=""GUIFeedback"">No requests found for this container</Span></TD></tr></table>")
			Response.End 
		Else
			if pageAction = "page1" then
				UserID = RS("RUserID")
				DateRequired = ConvertDateToStr(Application("DATE_FORMAT"),RS("Date_Required"))
				NumContainers = cint(RS("number_containers"))
				ContainerTypeID = RS("container_type_id_fk")			
			end if
			LocationID = RS("delivery_Location_ID_FK")
			comments = RS("request_comments")
			QtyList = RS("quantity_list")
			ShipToName= RS("ship_to_name")
			ExpenseCenter = RS("expense_center")
			Family = RS("Family")
		End if	
	Case "cancel"
			Caption = "Are you sure you want to cancel this request?"
	Case "undodelivery"
			Caption = "This request has been marked as delivered.<BR>Are you sure you want to undo the delivery?"		
End select

%>
<html>
<head>
<title><%=Application("appTitle")%> -- Request Samples from an Inventory Container</title>
<!--#INCLUDE VIRTUAL = "/cheminv/gui/dateFormat_js.asp"-->
<script LANGUAGE="javascript" src="/cheminv/Choosecss.js"></script>
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/navbar_js.js"-->
<script LANGUAGE="javascript" src="/cheminv/utils.js"></script>
<script LANGUAGE="javascript" src="/cheminv/gui/validation.js"></script>
<script language="JavaScript">
<!--Hide JavaScript

	var DialogWindow;
	window.focus();

	function ValidateRequest(action, pageAction){
		
		var bWriteError = false;
		var errmsg = "Please fix the following problems:\r";
		var NumContainers = document.form1.NumContainers.value;
		var SampleQty = document.form1.SampleQty.value;
	
		// Number of Containers is required
		if (document.form1.NumContainers.value.length == 0) {
			errmsg = errmsg + "- Number of Samples is required.\r";
			bWriteError = true;
		}
		else{
			// Number of Containers must be a number
			if (!isNumber(NumContainers)){
				errmsg = errmsg + "- Number of Samples must be a number.\r";
				bWriteError = true;
			}
			if (NumContainers < 1){
				errmsg = errmsg + "- Number of Samples must be a positive number.\r";
				bWriteError = true;
			}
		}

		if (action != "edit" && pageAction == "page1"){
			// Quantity per Sample is required
			if (document.form1.SampleQty.value.length == 0) {
				errmsg = errmsg + "- Quantity per Sample is required.\r";
				bWriteError = true;
			}
			else{
				// Quantity per Sample must be a number
				if (!isNumber(SampleQty)){
					errmsg = errmsg + "- Quantity per Sample must be a number.\r";
					bWriteError = true;
				}
				if (SampleQty <= 0){
					errmsg = errmsg + "- Quantity per Sample must be a positive number.\r";
					bWriteError = true;
				}
			}
		
		}

		
		if (pageAction == "page2") {
			//LocationID is required
			//document.form1.LocationID.value = document.form1.lpLocationID.value;
			<%'if action <> "cancel" AND action <> "undodelivery" then%>
			if (document.form1.LocationID.value.length == 0) {
				errmsg = errmsg + "- Delivery Location is required.\r";
				bWriteError = true;
			}
			else{
				// LocationID must be a psositve number
				if (!isPositiveNumber(document.form1.LocationID.value)){
					errmsg = errmsg + "- Location ID must be a positive number.\r";
					bWriteError = true;
				}
			}
		}

		// Date required must be a date
		if (document.form1.DateRequired.value.length > 0 && !isDate(document.form1.DateRequired.value)){
			errmsg = errmsg + "- Date Required must be in " + dateFormatString + " format.\r";
			bWriteError = true;
		}
					
		<%'end if%>
		if (bWriteError){
			alert(errmsg);
		}
		else{
			if (pageAction=="page1")
				{formAction = "#";}
			else
				{formAction = "request_action.asp?action=<%=action%>";}
			document.form1.action = formAction;
			document.form1.submit();
		}
	}
	
	function UpdateAddress(value)
	{
		//handle the page load case
		if (document.form1.isLoad.value == 'false'){
			
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

				if (httpResponse.length>0) {
					document.all.EditAddressLink.style.display = "block";
			
					arrAddress = httpResponse.split("::");
					document.form1.AddressID.value = arrAddress[0];
					document.form1.Address1.value = arrAddress[2];
					document.form1.Address2.value = arrAddress[3];
					document.form1.Address3.value = arrAddress[4];
					document.form1.Address4.value = arrAddress[5];
					document.form1.CityState.value = arrAddress[6] + ', ' + arrAddress[14] + ' ' + arrAddress[9];
					document.form1.Country.value = arrAddress[15];
				
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
		}
		else 
		{
			document.form1.isLoad.value = 'false';
		}
		
		//http://dpdev2/cheminv/api/GetLocationAddress.asp?locationid=0	
	}
	function UpdateShipTo(name)
	{
		var ShipTo;
		//Parameters: name (last, first)
		//Purpose: reverses the order of the anem and updates ShipTo field
		if (name.indexOf(',') > 0)
		{
			ShipTo = name.substr(name.indexOf(',')+1) + ' ' + name.substr(0, name.indexOf(','));
		}
		else
		{
			ShipTo = name;
		}
		document.form1.ShipToName.value = Trim(ShipTo);
	}
	function Trim(s) 
	{
	  // Remove leading spaces and carriage returns
	  
	  while ((s.substring(0,1) == ' ') || (s.substring(0,1) == '\n') || (s.substring(0,1) == '\r'))
	  {
	    s = s.substring(1,s.length);
	  }

	  // Remove trailing spaces and carriage returns

	  while ((s.substring(s.length-1,s.length) == ' ') || (s.substring(s.length-1,s.length) == '\n') || (s.substring(s.length-1,s.length) == '\r'))
	  {
	    s = s.substring(0,s.length-1);
	  }
	  return s;
	}
	
	function Initialize()
	{	
		if (document.form1.ShipToName)
			UpdateShipTo(document.form1.UserID.options[document.form1.UserID.selectedIndex].text);
	}
	
-->
</script>
</head>
<body ONLOAD="Initialize();">
<center>
<form name="form1" xaction="echo.asp" action="" method="POST">
<input Type="hidden" name="ContainerID" value="<%=ContainerID%>">
<input Type="hidden" name="BatchID" value="<%=BatchID%>">
<input Type="hidden" name="cLocationID" value="<%=cLocationID%>">
<input Type="hidden" name="RequestID" value="<%=RequestID%>">
<INPUT TYPE="hidden" NAME="Family" VALUE="<%=Family%>">
<INPUT TYPE="hidden" NAME="pageAction" VALUE="page2">
<INPUT TYPE="hidden" NAME="isLoad" VALUE="true">
<INPUT TYPE="hidden" NAME="AddressID" VALUE="">
<INPUT TYPE="hidden" NAME="isRequestSamples" VALUE="true">
<INPUT TYPE="hidden" NAME="RequestTypeID" VALUE="2">
<INPUT TYPE="hidden" NAME="RequestStatusID" VALUE="2">

<table border="0">
	<tr>
		<td colspan="2">
			<span class="GuiFeedback"><%=Caption%></span><br><br>
		</td>
	</tr>
	<% if BatchID <> "" then %>
	<tr>
		<td align="right" nowrap>
			Batch ID:
		</td>
		<td>
			<input TYPE="text" SIZE="44" Maxlength="50" VALUE="<%=BatchID%>" onfocus="blur()" disabled id="Text1" name="Text1">
		</td>
	</tr>
	<% else %>
	<tr>
		<td align="right" nowrap>
			Container ID:
		</td>
		<td>
			<input TYPE="text" SIZE="44" Maxlength="50" VALUE="<%=ContainerBarcode%>" onfocus="blur()" disabled id="Text1" name="Text1">
		</td>
	</tr>
	<tr>
		<td align="right" nowrap>
			Family:
		</td>
		<td>
			<input TYPE="text" SIZE="44" Maxlength="50" VALUE="<%=Family%>" onfocus="blur()" disabled id="Text2" name="Text2">
		</td>
	</tr>
	<% end if %>
	<%if action <> "cancel" AND action <> "undodelivery" then%>
	<tr>
		<%'=ShowPickList("Requested For:", "UserID", UserID, "SELECT User_ID AS Value, Last_Name||DECODE(First_Name, NULL, '', ', '||First_Name) AS DisplayText FROM CS_SECURITY.People ORDER BY lower(Last_Name) ASC")%>
		<td align="right" nowrap>Requested For:</td>
		<td><%=ShowSelectBox3("UserID", UserID, "SELECT User_ID AS Value, Last_Name||DECODE(First_Name, NULL, '', ', '||First_Name) AS DisplayText FROM CS_SECURITY.People ORDER BY lower(Last_Name) ASC", 25, "", "", iif(pageAction="page1","","UpdateShipTo(this.options[selectedIndex].text);//alert(this.options[selectedIndex].text);"))%>
		</td>
	</tr>
	<tr>
		<td align="right" valign="top" nowrap>
			Date Required:
		</td>
		<td>
			<%call ShowInputField("", "", "DateRequired:form1:" & DateRequired , "DATE_PICKER:TEXT", "15")%>
			<!--<input type="text" name="DateRequired" size="15" value="<%=DateRequired%>"><a href onclick="return PopUpDate(&quot;DateRequired&quot;,&quot;/cheminv/gui/month_view1.asp&quot;)"><img src="/cfserverasp/source/graphics/navbuttons/icon_mview.gif" height="16" width="16" border="0"></a>-->
		</td>
	</tr>
	<tr>
		<%=ShowPickList("<SPAN class=required>Sample Container Type:</span>", "ContainerTypeID", ContainerTypeID,"SELECT Container_Type_ID AS Value, Container_Type_Name AS DisplayText FROM inv_Container_Types ORDER BY lower(DisplayText) ASC")%>
	</tr>
	<tr>
		<td align="right" nowrap>
			<span class="required">Number of Samples:</span>
		</td>
		<td>
		<%if pageAction="page1" then%>
			<input TYPE="text" SIZE="10" Maxlength="50" NAME="NumContainers" VALUE="<%=NumContainers%>">
		<%else%>
			<input TYPE="text" SIZE="10" Maxlength="50" NAME="NumContainers" STYLE="background-color:#d3d3d3;" VALUE="<%=NumContainers%>" READONLY>
		<%end if%>
		</td>
	</tr>
		<%if pageAction="page1" and action <> "edit" then%>
		<tr>
			<td align="right" nowrap>
				<span class="required">Quantity per Sample (<%=UOMAbv%>):</span>
			</td>
			<td>
				<input TYPE="text" SIZE="10" Maxlength="50" NAME="SampleQty" VALUE="">
			</td>
		</tr>	
		<%else%>
			<input TYPE="hidden" SIZE="10" Maxlength="50" NAME="SampleQty" VALUE="<%=SampleQty%>">
		<%end if%>
		<tr>
			<td align="right" nowrap>
				Expense Center:
			</td>
			<td>
				<input TYPE="text" SIZE="10" Maxlength="50" NAME="ExpenseCenter" VALUE="<%=ExpenseCenter%>">
			</td>
		</tr>	

	<%end if%>

	<%if pageAction = "page2" then%>
	<%
		if action = "edit" then
			arrValues = split(QtyList,",")
			diff = NumContainers - ubound(arrValues)
			if diff > 0 then
				for i =0 to diff
					QtyList = QtyList & ","
				next
				arrValues = split(QtyList,",")
			end if
		else 
			Dim arrValues()
			reDim  arrValues(NumContainers)
			for i = 0 to (NumContainers-1)
				arrValues(i) = SampleQty
			next
		end if
		Response.Write GenerateFields("Sample", "Quantity (" & UOMAbv & ")", "sample", NumContainers, arrValues)
	%>
	<tr height="100">
		<td align="right" valign="top" nowrap>
			Comments:
		</td>
		<td valign="top">
			<textarea rows="5" cols="30" name="RequestComments" wrap="hard"><%=Comments%></textarea>
		</td>
	</tr>
	<tr>
		<td align="right" nowrap>
			<span class="required" title="Location where the container should be delivered to">Delivery Location: <%=GetBarcodeIcon()%></span>
		</td>
		<td>
		<%LocationID = iif((action="create"), "", LocationID)'LocationID = Session("LocationID")%>
			<%ShowLocationPicker3 "document.form1", "LocationID", "lpLocationBarCode", "lpLocationName", 10, 30, false, "UpdateAddress(this.value);"%> 
		</td>
	</tr>
	<tr>
		<td align="right" nowrap>
			Ship To:
		</td>
		<td>
			<input TYPE="text" SIZE="30" Maxlength="50" NAME="ShipToName" VALUE="" STYLE="background-color:#d3d3d3;" READONLY>
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
				<td><span id="EditAddressLink" style="display:none;"><A HREF="Edit Address" onclick="OpenDialog('/cheminv/gui/EditAddress.asp?TableName=inv_locations&TablePKID=' +  document.form1.LocationID.value + '&AddressID=' + document.form1.AddressID.value,'AddressDiag', 4); return false;" CLASS="MenuLink">Edit Address</a></span></td>
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
	<script language="JavaScript">
		//Update the address when the page loads
		UpdateAddress(document.form1.LocationID.value);
	</script>
	<%end if%>	
	<%if action = "cancel" or action = "undodelivery"  then%>
	<tr>
		<td colspan="2" align="right"> 
			<a HREF="#" onclick="if (DialogWindow) {DialogWindow.close()}; window.close(); return false;"><img SRC="../graphics/cancel_dialog_btn.gif" border="0" WIDTH="61" HEIGHT="21"></a><a HREF="#" onclick="document.form1.action = 'request_action.asp?action=<%=action%>';document.form1.submit(); return false;"><img SRC="../graphics/ok_dialog_btn.gif" border="0" alt WIDTH="61" HEIGHT="21"></a>
		</td>
	</tr>		
	<%elseif pageAction="page2" then%>
	<tr>
		<td colspan="2" align="right"> 
			<a HREF="#" onclick="if (DialogWindow) {DialogWindow.close()}; window.close(); return false;"><img SRC="../graphics/cancel_dialog_btn.gif" border="0" WIDTH="61" HEIGHT="21"></a><a HREF="#" onclick="history.go(-1); return false;"><img SRC="../graphics/btn_back_61.gif" border="0" WIDTH="61" HEIGHT="21"></a><a HREF="#" onclick="ValidateRequest('<%=action%>','<%=pageAction%>'); return false;"><img SRC="../graphics/ok_dialog_btn.gif" border="0" alt WIDTH="61" HEIGHT="21"></a>
		</td>
	</tr>		
	<%else%>
	<tr>
		<td colspan="2" align="right"> 
			<a HREF="#" onclick="if (DialogWindow) {DialogWindow.close()}; window.close(); return false;"><img SRC="../graphics/cancel_dialog_btn.gif" border="0" WIDTH="61" HEIGHT="21"></a><a HREF="#" onclick="ValidateRequest('<%=action%>','<%=pageAction%>'); return false;"><img SRC="../graphics/btn_next_61.gif" border="0" alt WIDTH="61" HEIGHT="21"></a>
		</td>
	</tr>		
	<%end if%>
	
</table>	
</form>
</center>
</body>
</html>
