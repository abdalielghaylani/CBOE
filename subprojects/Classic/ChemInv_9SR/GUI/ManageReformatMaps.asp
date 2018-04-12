<%@ Language=VBScript %>
<!--#INCLUDE VIRTUAL = "/cheminv/gui/guiUtils.asp"-->
<!--#INCLUDE VIRTUAL = "/cheminv/api/apiUtils.asp"-->
<%
Dim Conn
mapAction = Request("action")
stepValue = Request("stepValue")
xmldocID = Request("xmldocID")

'-- constant values
xmldocTypeID = 1 '-- always create a reformat map

GetInvConnection()
SQL = "SELECT count(*) as plateCount FROM inv_vw_plate_format WHERE row_count*col_count = 384"
Set rsCount = Conn.Execute(SQL)
count384 = cint(rsCount("plateCount"))

SQL = "SELECT count(*) as plateCount FROM inv_vw_plate_format WHERE row_count*col_count = 1536"
Set rsCount = Conn.Execute(SQL)
count1536 = cint(rsCount("plateCount"))
%>
<html>
<head>
<title><%=Application("appTitle")%> -- Manage Reformat Maps</title>
<script LANGUAGE="javascript" src="/cheminv/Choosecss.js"></script>
<script LANGUAGE="javascript" src="/cheminv/gui/validation.js"></script>
<script LANGUAGE="javascript">
<!--
	window.focus();
	
	function Validate(){

		var bWriteError = false;
		var bWriteWarning = false;
		var errmsg = "Please fix the following problems:\r\r";
		var warningmsg = "Please address the following warnings:\r\r";

		<%if mapAction = "create" then%>
		// Reformat Map Name is required
		if (document.form1.iReformatMapName.value.length == 0) {
			errmsg = errmsg + "- Reformat Map Name is required.\r";
			bWriteError = true;
		}
		
		// validate the order if its a z-index
		if (document.form1.reformatMapType.selectedIndex == 1)
		{
			for (i=1; i<=4; i++) {
				bSpotFilled = false;
				for (j=0; j<4; j++){
					bChecked = eval("document.all.position" + i + "[" + j + "].checked");
					if (bChecked) 
						bSpotFilled = true; 
				}
				if (!bSpotFilled) {
					errmsg = errmsg + "- You must select a plate for Position" + String.fromCharCode(64 + i) + ".\r";
					bWriteError = true;
				}
			}
		
		}
		
		// validate that a target format has been chosen
		if (document.all.Targets384.style.display == "block") {
			if (document.form1.targetPlateFormatID384.options[document.form1.targetPlateFormatID384.selectedIndex].value == 'NULL') {
					errmsg = errmsg + "- No 384 well plate formats are available.\r";
					bWriteError = true;
			}
		}
		else {
			if (document.form1.targetPlateFormatID1536.options[document.form1.targetPlateFormatID1536.selectedIndex].value == 'NULL') {
					errmsg = errmsg + "- No 1536 well plate formats are available.\r";
					bWriteError = true;
			}
		}
		
		// set the target format ID
		if (document.all.Targets384.style.display == "block")
			document.form1.targetPlateFormatID.value = document.form1.targetPlateFormatID384.options[document.form1.targetPlateFormatID384.selectedIndex].value;
		else
			document.form1.targetPlateFormatID.value = document.form1.targetPlateFormatID1536.options[document.form1.targetPlateFormatID1536.selectedIndex].value;
		
		
		
		<%end if%>
		
		document.form1.action='ManageReformatMaps_action.asp';
		if (bWriteError){
			alert(errmsg);
		}
		else{
			var bcontinue = true;

			// Report warnings, user can choose to accept or cancel
			bConfirmWarning = true;
			if (bWriteWarning) {
				bConfirmWarning = confirm(warningmsg);
			}
			if (!bConfirmWarning) var bcontinue = false;
			if (bcontinue) document.form1.submit();
		}
	}
	
	function DisplayZOrder(selectedIndex) {
		if (selectedIndex == "0")
			document.all.zOrder.style.display="none";
		else if (selectedIndex == "1")
			document.all.zOrder.style.display="block";
	}

	function ClearSelections(plateNum, element) {
		curPlateName = element.name;
		curPlateValue = element.value;
		
		for (i=1; i<=4; i++) {
			for (j=0; j<4; j++){
				value = eval("document.all.position" + i + "[" + j + "].value");
				name = eval("document.all.position" + i + "[" + j + "].name");
				//alert(name + ":" + curPlateName + "   " + value + ":" + curPlateValue);
				if (value == curPlateValue && name != curPlateName) {
					eval("document.all.position" + i + "[" + j + "].checked = false");				
				}
			}
		}
	}
	
	function ShowTargetFormat(element)
	{
		var arrSplit = element.options[element.selectedIndex].value.split(":");
		var plateSize = arrSplit[1];
		//alert(plateSize);
		if (plateSize == "96") {
			document.all.Targets384.style.display = "block";	
			document.all.Targets1536.style.display = "none";	
		}
		else if (plateSize == "384") {
			document.all.Targets384.style.display = "none";	
			document.all.Targets1536.style.display = "block";	
		}
	}
//-->
</script>
</head>

<body><center>
<form name="form1" method="POST">
<input type="hidden" name="mapAction" value="<%=mapAction%>" />
<input type="hidden" name="targetPlateFormatID" />

<%
if mapAction = "delete" then
%>
<input type="hidden" name="stepValue" value="2" />
<table border="0">
	<tr>
		<td colspan="2" align="center">
			<span class="GuiFeedback">Select a reformat map to delete.</span><br /><br />
		</td>
	</tr>
	<tr><%=ShowPickList("Reformat Map:", "XmlDocID", XmlDocID, "SELECT XMLDOC_ID AS Value, NAME AS DisplayText FROM inv_xmldocs WHERE xmldoc_type_id_FK IN (1) ORDER BY lower(DisplayText) ASC")%></tr>
	<tr><td colspan="2" align="right">&nbsp;</td></tr>
	<tr><td colspan="2" align="right">
		<a href="#" onclick="history.back(); return false;"><img src="../graphics/cancel_dialog_btn.gif" border="0" width="61" height="21"></a>
		<a href="#" onclick="Validate(); return false;"><img src="../graphics/ok_dialog_btn.gif" border="0" width="61" height="21">
	</td></tr>
</table>
<%
else
%>
<table border="0">
	<tr><td colspan="2" align="center">
		<span class="GuiFeedback">Create a Reformat Map.</span><br /><br />
	</td></tr>
	<tr><%=ShowInputBox("Reformat Map Name:", "ReformatMapName", 30, "", bFieldsDisabled, True)%></tr>
	<tr>
		<td align="right"><span class="required">Reformat Map Type:</span></td>
		<td><select name="reformatMapType" onchange="DisplayZOrder(this.selectedIndex);">
			<option value="1">Stamped</option>
			<option value="2" selected>Dithered</option>
			</select>
		</td>
	</tr>
	<tr>
		<td></td>
		<td>
		<div id="zOrder" style="display:none;">
			<table>
			<tr>
				<td><strong>Plate Fill Order</strong></td>
				<td><strong>Well Position</strong></td>
				<td><strong>Well Position Layout</strong></td>
			</tr>
			<tr>
				<td width="110" align="center"><strong>1</strong></td>
				<td><input type="radio" name="position1" value="1" checked ONCLICK="ClearSelections(1, this);">A<input type="radio" name="position2" value="1" ONCLICK="ClearSelections(1, this);">B<input type="radio" name="position3" value="1" ONCLICK="ClearSelections(1, this);">C<input type="radio" name="position4" value="1" ONCLICK="ClearSelections(1, this);">D</td>
				<td rowspan="4">&nbsp;&nbsp;&nbsp;&nbsp;<img src="../graphics/well_position.jpg" width="102" height="102">
			</tr>
			<tr>
				<td align="center"><strong>2</strong></td>
				<td><input type="radio" name="position1" value="2" ONCLICK="ClearSelections(2, this);">A<input type="radio" name="position2" value="2" checked ONCLICK="ClearSelections(2, this);">B<input type="radio" name="position3" value="2" ONCLICK="ClearSelections(2, this);">C<input type="radio" name="position4" value="2" ONCLICK="ClearSelections(2, this);">D</td>
			</tr>
			<tr>
				<td align="center"><strong>3</strong></td>
				<td><input type="radio" name="position1" value="3" ONCLICK="ClearSelections(3, this);">A<input type="radio" name="position2" value="3" ONCLICK="ClearSelections(3, this);">B<input type="radio" name="position3" value="3" checked ONCLICK="ClearSelections(3, this);">C<input type="radio" name="position4" value="3" ONCLICK="ClearSelections(3, this);">D</td>
			</tr>
			<tr>
				<td align="center"><strong>4</strong></td>
				<td><input type="radio" name="position1" value="4" ONCLICK="ClearSelections(4, this);">A<input type="radio" name="position2" value="4" ONCLICK="ClearSelections(4, this);">B<input type="radio" name="position3" value="4" ONCLICK="ClearSelections(4, this);">C<input type="radio" name="position4" value="4" checked ONCLICK="ClearSelections(4, this);">D</td>
			</tr>
			</table>
		</div>
		</td>
	</tr>
	<script language="javascript">
		DisplayZOrder(document.form1.reformatMapType.selectedIndex);
	</script>
		<tr height="25">
			<td align=right valign=top nowrap><span class="required">Source Plate Format:</span></td>
			<td>
			<%=ShowSelectBox3("sourcePlateFormatID", sourcePlateFormatID, "SELECT plate_format_id || ':' || row_count*col_count AS Value, plate_format_name AS DisplayText FROM inv_vw_plate_format WHERE row_count*col_count = 96 or row_count*col_count = 384 ORDER BY lower(DisplayText) ASC", null, "", "", "ShowTargetFormat(this);")
		%>
		<%'=ShowPickList("<span class=""required"">Source Plate Format:</span>", "sourcePlateFormatID", sourcePlateFormatID, "SELECT plate_format_id || ':' || row_count*col_count AS Value, plate_format_name AS DisplayText FROM inv_vw_plate_format WHERE row_count*col_count = 96 or row_count*col_count = 384 ORDER BY lower(DisplayText) ASC")%>
			</td>
	</tr>
	<tr height="25">
			<td align=right valign=top nowrap><span class="required">Target Plate Format:</span></td>
			<td>
			<div id="Targets384" style="display:block">
			<%
			if count384 = 0 then
				Response.Write "<INPUT TYPE=""text"" NAME=""message"" SIZE=""50"" onfocus=""blur()"" VALUE=""No 384 well plate formats are available."" disabled>"
				Response.Write "<span style=""display:none;""><SELECT NAME=""targetPlateFormatID384""><OPTION VALUE=""NULL"" SELECTED></SELECT></span>"
			else
				Response.Write ShowSelectBox("targetPlateFormatID384", targetPlateFormatID, "SELECT plate_format_id AS Value, plate_format_name AS DisplayText FROM inv_vw_plate_format WHERE row_count*col_count = 384 ORDER BY lower(DisplayText) ASC")
			end if
			%>			
			</div>
			<div id="Targets1536" style="display:none">
			<%
			if count1536 = 0 then
				Response.Write "<INPUT TYPE=""text"" NAME=""message"" SIZE=""50"" onfocus=""blur()"" VALUE=""No 1536 well plate formats are available."" disabled>"
				Response.Write "<span style=""display:none;""><SELECT NAME=""targetPlateFormatID1536""><OPTION VALUE=""NULL"" SELECTED></SELECT></span>"
			else
				Response.Write ShowSelectBox("targetPlateFormatID1536", targetPlateFormatID, "SELECT plate_format_id AS Value, plate_format_name AS DisplayText FROM inv_vw_plate_format WHERE row_count*col_count = 1536 ORDER BY lower(DisplayText) ASC")
			end if
			%>			
			</div>
			</td>
			<%'=ShowPickList("<span class=""required"">Target Plate Format:</span>", "targetPlateFormatID", targetPlateFormatID, "SELECT plate_format_id AS Value, plate_format_name AS DisplayText FROM inv_vw_plate_format WHERE row_count*col_count = 384  or row_count*col_count = 1536 ORDER BY lower(DisplayText) ASC")%>
	</tr>
	<tr><td colspan="2" align="right">
		<a HREF="#" onclick="history.go(-1); return false;"><img SRC="../graphics/cancel_dialog_btn.gif" border="0" WIDTH="61" HEIGHT="21"></a>&nbsp;
		<a HREF="#" onclick="Validate(); return false;"><img SRC="../graphics/ok_dialog_btn.gif" border="0" WIDTH="61" HEIGHT="21"></a>
	</td></tr>
</table>
<script language="javascript">
	ShowTargetFormat(document.all.sourcePlateFormatID);
</script>
<%
end if
%>
</form>
</center>
</body>
</html>
