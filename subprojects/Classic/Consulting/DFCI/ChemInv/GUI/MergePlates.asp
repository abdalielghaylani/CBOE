<%@ Language=VBScript %>
<!--#INCLUDE VIRTUAL = "/cheminv/api/apiUtils.asp"-->
<!--#INCLUDE VIRTUAL = "/cheminv/gui/guiUtils.asp"-->
<%
Dim Conn

PlateID = Request("PlateID")
ParentPlateID = Request("ParentPlateID")
Action = Request("Action")
NumPlates = Request("NumPlates")
WellAmt = Request("WellAmt")
PlateType = Request("PlateType")

%>
<html>
<head>
<title><%=Application("appTitle")%> -- Merge Plates</title>
<script LANGUAGE="javascript" src="/cheminv/Choosecss.js"></script>
<script LANGUAGE="javascript" src="/cheminv/gui/validation.js"></script>
<script LANGUAGE="javascript" src="/cheminv/utils.js"></script>
<script language="JavaScript">
<!--Hide JavaScript
	window.focus();
	function ValidateQtyChange(){
		
		var bWriteError = false;
		var PlateID = document.form1.PlateID.value;
		//var WellAmt = document.form1.WellAmt.value;
		var PlateType = document.form1.PlateType.value;
		var NumPlates = document.form1.NumPlates.value;
		var errmsg = "Please fix the following problems:\r";
		var AllMergePlateIDs = "";

		
		// Merging Plate ID is required
		if (document.form1.MergePlateIDs.value.length == 0) {
			errmsg = errmsg + "- Merging Plate ID is required.\r";
			bWriteError = true;
		}
		else {
			for (var i = document.form1.MergePlateIDs.selectedIndex; i < document.form1.MergePlateIDs.options.length; i++) {
				if (document.form1.MergePlateIDs.options[i].selected) AllMergePlateIDs += document.form1.MergePlateIDs.options[i].value + ",";
			}
			AllMergePlateIDs = PlateID + "," + AllMergePlateIDs;
			AllMergePlateIDs = AllMergePlateIDs.substr(0,(AllMergePlateIDs.length - 1));
			document.form1.AllMergePlateIDs.value = AllMergePlateIDs;
		}
		
		// Well Amount is required
		/*if (document.form1.WellAmt.value.length == 0) {
			errmsg = errmsg + "- Well Amount is required.\r";
			bWriteError = true;
		}
		else{
			// Well Amount must be a number
			if (!isNumber(WellAmt)){
				errmsg = errmsg + "- Well Amount must be a number.\r";
				bWriteError = true;
			}
			if (WellAmt <= 0){
				errmsg = errmsg + "- Well Amount must be a positive number.\r";
				bWriteError = true;
			}
		}
		*/
		// Plate Type is required
		if (document.form1.PlateType.value.length == 0) {
			errmsg = errmsg + "- Plate Type is required.\r";
			bWriteError = true;
		}
		else{

		}
		// Number of Plates is required
		if (document.form1.NumPlates.value.length == 0) {
			errmsg = errmsg + "- Number of Plates is required.\r";
			bWriteError = true;
		}
		else{
			// Quantity per Sample must be a number
			if (!isNumber(NumPlates)){
				errmsg = errmsg + "- Number of Plates must be a number.\r";
				bWriteError = true;
			}
			if (NumPlates <= 0){
				errmsg = errmsg + "- Number of Plates must be a positive number.\r";
				bWriteError = true;
			}
		}
		/*
		if (!bWriteError) {
			//check for sufficient source quantites, well by well
			var strURL = "http://" + serverName + "/cheminv/api/CheckSufficientSourcePlateQty.asp?Action=Merge&PlateID=" + AllMergePlateIDs + "&WellAmt=" + WellAmt + "&NumPlates=" + NumPlates;	
			var httpResponse = JsHTTPGet(strURL) 
			//errmsg = errmsg + "- " + strURL + "\r";
			//errmsg = errmsg + "- " + httpResponse + "\r";
			//httpResponse = 1;
			if (httpResponse == 1) {
				errmsg = errmsg + "- Source plate quantities are insufficient for the well amount and number of daughter plates chosen.\r";
				bWriteError = true;
			}
			else if (httpResponse == 2) {
				errmsg = errmsg + "- Daughter plate quantities are insufficient to exhaust source plates.\r";
				bWriteError = true;
			}
			else if (httpResponse == 3) {
				errmsg = errmsg + "- Source plate quantities are insufficient for the well amount and number of daughter plates chosen.\r";
				errmsg = errmsg + "- Daughter plate quantities are insufficient to exhaust source plates.\r";
				bWriteError = true;
			}
		}
		*/
		if (bWriteError){
			alert(errmsg);
			return false;
		}
		else{
			return true;
		}
	}
	
-->
</script>

</head>
<body>
<center>
<form name="form1" action="MergePlates_action.asp" method="POST" onsubmit="return ValidateQtyChange();">
<input Type="hidden" name="PlateID" value="<%=PlateID%>">
<input Type="hidden" name="ParentPlateID" value="<%=ParentPlateID%>">
<input Type="hidden" name="AllMergePlateIDs" value="">
<table border="0">
	<tr>
		<td colspan="2">
			<span class="GuiFeedback">Select the merging plates and enter the daughter plate data.</span><br><br>
		</td>
	</tr>
	<%if ParentPlateID = "" then%>
	<tr>
		<td colspan="2">There are no mergeable plates.</td>
	</tr>
	<%else%>
	<tr>
		<td align="right" valign="top" nowrap>Select Merging Plate IDs:</td>
		<td>		
			<%
			SQL = "SELECT plate_id AS Value, plate_id AS DisplayText FROM inv_plates WHERE plate_id = " & ParentPlateID & " OR parent_plate_id_fk = " & ParentPlateID & " AND plate_id != " & PlateID & " ORDER BY DisplayText"
			Response.Write ShowMultiSelectBox("MergePlateIDs", "", SQL, 100,"" ,RepeatString(30, "&nbsp;") , 10, 1)
			%>
		</td>
	</tr>	
	<tr>
		<%=ShowPickList("Select Plate Type:", "PlateType", "", "SELECT plate_type_id AS Value, plate_type_name AS DisplayText FROM inv_plate_types ORDER BY lower(DisplayText) ASC")%>
	</tr>
	<!--<tr>
		<td align="right" nowrap>Well Amount:</td>
		<td><input TYPE="text" SIZE="10" Maxlength="50" NAME="WellAmt" VALUE="<%=WellAmt%>"></td>
	</tr>-->
	<tr>
		<td align="right" nowrap>Number of Daughter Plates:</td>
		<td><input TYPE="text" SIZE="10" Maxlength="50" NAME="NumPlates" VALUE="<%=NumPlates%>"></td>
	</tr>	
	<tr>
		<td colspan="2" align="right"><a HREF="#" onclick="window.close(); return false;"><img SRC="../graphics/cancel_dialog_btn.gif" border="0" WIDTH="61" HEIGHT="21"></a><input type="image" SRC="../graphics/ok_dialog_btn.gif" border="0" WIDTH="61" HEIGHT="21" id=image1 name=image1></td>
	</tr>	
	<%end if%>
	<tr>
		<td colspan="2" align="right"><a HREF="#" onclick="window.close(); return false;"><img SRC="../graphics/cancel_dialog_btn.gif" border="0" WIDTH="61" HEIGHT="21"></a></td>
	</tr>	
</table>	
</form>
</center>
</body>
</html>
