<%@ Language=VBScript %>
<!--#INCLUDE VIRTUAL = "/cheminv/gui/guiUtils.asp"-->
<!--#INCLUDE VIRTUAL = "/cheminv/api/apiUtils.asp"-->
<%
Dim Conn
mapAction = Request("action")
stepValue = Request("stepValue")
mapNames = ""

TemplateName = Request("TemplateName")
dataMapID = Request("ID")
MapTypeID = Request("MapTypeID")
Remarks = Request("Remarks")
NumHeaderLines = Request("NumHeaderLines")
NumDataCol = Request("NumDataCol")
numExtraDisplayCol = 0
DataColDel = Request("DataColDel")
UseWellCoordinates = Request("UseWellCoordinates")
if UseWellCoordinates = "" then UseWellCoordinates = 1

' Control field input and display options
' ---------------------------------------
bFieldsDisabled = False
if stepValue = "1" then
	stepValue = "2"
	bFieldsDisabled = True
else
	stepValue = "1"
end if

' Get limit for the number of column mappings
' -------------------------------------------
sql_mapNames = "Select data_map_name from INV_DATA_MAPS"
Call GetInvConnection()
Set RS_mapNames = Conn.Execute(sql_mapNames)
While NOT RS_mapNames.EOF
	mapNames = mapNames & "[" & RS_mapNames("data_map_name") & "],"
RS_mapNames.MoveNext
Wend
RS_mapNames.Close()
Set RS_mapNames = Nothing

' Get list of existing template names
' -----------------------------------
sql_mapColLimit = "Select count(*) as Cnt from INV_MAP_FIELDS"
Call GetInvConnection()
Set RS_mapColLimit = Conn.Execute(sql_mapColLimit)
mapColLimit = RS_mapColLimit("Cnt")
Set RS_mapColLimit = Nothing

' Populate hidden form fields if not creating
' -------------------------------------------
If mapAction <> "create" then
	Call GetInvConnection()
	SQL = "SELECT DATA_MAP_NAME,DATA_MAP_TYPE_ID_FK,DATA_MAP_COMMENTS,NUM_HEADER_ROWS,NUM_COLUMNS,COLUMN_DELIMITER, USE_WELL_COORDINATES From INV_DATA_MAPS Where DATA_MAP_ID=" & dataMapID
	Set RS = Conn.Execute(SQL)
	if TemplateName = "" then TemplateName = RS("DATA_MAP_NAME").value end if
	if MapTypeID = "" then MapTypeID = RS("DATA_MAP_TYPE_ID_FK").value end if
	if Remarks = "" then Remarks = RS("DATA_MAP_COMMENTS").value end if
	if NumHeaderLines = "" then NumHeaderLines = RS("NUM_HEADER_ROWS").value end if
	if NumDataCol = "" then 
		NumDataCol = RS("NUM_COLUMNS").value 
	else
		if cInt(NumDataCol) > cInt(RS("NUM_COLUMNS").value) then
			numExtraDisplayCol = cInt(NumDataCol) - cInt(RS("NUM_COLUMNS").value)
		end if
	end if
	if DataColDel = "" then DataColDel = RS("COLUMN_DELIMITER").value end if
	if UseWellCoordinates = "" then UseWellCoordinates = RS("USE_WELL_COORDINATES").value end if
	RS.Close()
	Set RS = Nothing
End if

' Select the value for well column type
' -------------------------------------
if UseWellCoordinates = "" or UseWellCoordinates = "0" then
	Response.Write UseWellCordinates
	selectWellNum = "CHECKED"
	selectWellCoord = ""
else
	selectWellNum = ""
	selectWellCoord = "CHECKED"
end if
%>
<html>
<head>
<title><%=Application("appTitle")%> Administration Menu</title>
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
		
		<%
		if mapAction = "create" or mapAction = "update" then
		' CREATE JavaScript validation
		' --------------------------------
		%>
		!document.form1.iTemplateName ? document.form1.TemplateName.value="" : document.form1.TemplateName.value = document.form1.iTemplateName.value;
		!document.form1.iRemarks ? document.form1.Remarks.value="" : document.form1.Remarks.value = document.form1.iRemarks.value;
		!document.form1.iNumHeaderLines ? document.form1.NumHeaderLines.value="" : document.form1.NumHeaderLines.value = document.form1.iNumHeaderLines.value;
		!document.form1.iNumDataCol ? document.form1.NumDataCol.value="" : document.form1.NumDataCol.value = document.form1.iNumDataCol.value;
		!document.form1.iDataColDel ? document.form1.DataColDel.value="" : document.form1.DataColDel.value = document.form1.iDataColDel.value;

		// Template Name is required
		if (document.form1.TemplateName.value.length == 0) {
			errmsg = errmsg + "- Template Name is required.\r";
			bWriteError = true;
		} else {
			// Template Name must be uniqe
			varMapName = document.form1.mapNames.value;
			if (varMapName.indexOf(document.form1.TemplateName.value) > 0 && document.form1.mapAction.value != 'update'){
				errmsg = errmsg + "- Template Name already exists. Please choose a new name.\r";
				bWriteError = true;
			}
		}

		// Remarks are required
		if (document.form1.Remarks.value.length == 0) {
			errmsg = errmsg + "- Remarks is required.\r";
			bWriteError = true;
		}

		// Mapping Type
		/* if (document.form1.MapTypeID.value == 'NULL') {
			errmsg = errmsg + "- Mapping Type is required.\r";
			bWriteError = true;
		}*/

		// # File Header Lines validation
		if (document.form1.NumHeaderLines.value!=0) {
			if (!isPositiveNumber(document.form1.NumHeaderLines.value) || document.form1.NumHeaderLines.value < 1){
				errmsg = errmsg + "- # File Header Lines must be a positive number greater than zero.\r";
				bWriteError = true;
			} else {
				if (document.form1.NumHeaderLines.value >= 9000 ) {
					errmsg = errmsg + "- # File Header Lines is limited to 9000. Please lower this value.\r";
					bWriteError = true;
				}
			}
		}
		// # Data Columns is required
		if (document.form1.NumDataCol.value.length == 0) {
			errmsg = errmsg + "- # Data Columns is required.\r";
			bWriteError = true;
		} else {
			if (!isPositiveNumber(document.form1.NumDataCol.value) || document.form1.NumDataCol.value < 1){
				errmsg = errmsg + "- # Data Columns Lines must be a positive number greater than zero.\r";
				bWriteError = true;
			} else {
				if (parseInt(document.form1.NumDataCol.value) > parseInt(document.form1.mapColLimit.value)) {
					errmsg = errmsg + "- # Data Columns exceeds the fields available. \rPlease contact your administrator to add more mapping fields.\r";
					bWriteError = true;
				}
				<% if mapAction="update" then %>
				if (document.form1.iNumDataCol.value < <%=NumDataCol%>){
					warningmsg = warningmsg + "- # Data Columns you have entered is less then the existing value.\rIf you save these changes, the existing column mappings will be lost.\rDo you want to proceed?";
					bWriteWarning = true;
				}
				<% end if %>
			}
		}
		// # Data Column Delimeter
		if (document.form1.DataColDel.value.length == 0) {
			errmsg = errmsg + "- Data Column Delimeter.\r";
			bWriteError = true;
		} else {
			if (document.form1.DataColDel.value.length > 3) {
				errmsg = errmsg + "- Data Column Delimeter is limited to three characters.\r";
				bWriteError = true;
			}
		}
		// If this is the second step in process
		// change form action and validate Column Mapping
		if (document.form1.stepValue.value == 2) {
			document.form1.action='ManageDataMaps_action.asp';
			
			// Verify # Data Columns has not changed
			if (document.form1.NumDataCol.value != <%if NumDataCol <> "" then response.write(NumDataCol) else response.write("0") end if %>) {
				errmsg = errmsg + "- # Data Columns do not match the number of Data Columns listed.\rPlease click 'Cancel', return to the previous screen and enter the desired # Data Columns.\r\r";
				bWriteError = true;
			}
			
			// Render and validate the field column mappings
			<%
			response.write("lstColMapValues='';" & vbcrlf)
			response.write("varColMapValue='';" & vbcrlf)
			for i=1 to Request.Form("NumDataCol")
				response.write(vbcrlf)
				response.write(vbtab & vbtab & vbtab & "if (document.form1.ColumnMap" & i & ".value=='NULL') {" & vbcrlf)
				response.write(vbtab & vbtab & vbtab & vbtab & "errmsg = errmsg + ""- Column " & i & " Mapping must be selected.\r"";" & vbcrlf)
				response.write(vbtab & vbtab & vbtab & vbtab & "bWriteError = true;" & vbcrlf)
				response.write(vbtab & vbtab & vbtab & "} else {" & vbcrlf)
				response.write(vbtab & vbtab & vbtab & vbtab & "varColMapValue = lstColMapValues.indexOf('[' + document.form1.ColumnMap" & i & ".value + ']');" & vbcrlf)
				response.write(vbtab & vbtab & vbtab & vbtab & "if (parseInt(varColMapValue) > 0) {" & vbcrlf)
				response.write(vbtab & vbtab & vbtab & vbtab & vbtab & "errmsg = errmsg + ""- Column " & i & " Mapping must be mapped to unique field.\rPlease select a new field from the drop-down list.\r\r"";" & vbcrlf)
				response.write(vbtab & vbtab & vbtab & vbtab & vbtab & "bWriteError = true;" & vbcrlf)
				response.write(vbtab & vbtab & vbtab & vbtab & "} else {" & vbcrlf)
				response.write(vbtab & vbtab & vbtab & vbtab & vbtab & "lstColMapValues = lstColMapValues + ',[' + document.form1.ColumnMap" & i & ".value + ']'; " & vbcrlf)
				response.write(vbtab & vbtab & vbtab & vbtab & "}" & vbcrlf)
				response.write(vbtab & vbtab & vbtab & "}" & vbcrlf)
			next
			%>
			if (lstColMapValues.indexOf(',[7]') > -1){
				if (lstColMapValues.indexOf(',[8]') == -1){
					errmsg = errmsg + "- Concentration Unit ID Column Mapping is required if Concentration is mapped.\rPlease add Concentration Unit ID as a Column Mapping.\r";
					bWriteError = true;
				}
			}
			if (lstColMapValues.indexOf(',[5]') > -1){
				if (lstColMapValues.indexOf(',[6]') == -1){
					errmsg = errmsg + "- Quantity Unit ID Column Mapping is required if Initial Quantity is mapped.\n Please add Quantity Unit ID as a Column Mapping.\r";
					bWriteError = true;
				}
			}
		}
		<%
		elseif mapAction="copy" then
		' COPY JavaScript validation
		' --------------------------------
		%>
		!document.form1.iTemplateName ? document.form1.TemplateName.value="" : document.form1.TemplateName.value = document.form1.iTemplateName.value;
		// Template Name is required
		if (document.form1.TemplateName.value.length == 0) {
			errmsg = errmsg + "- Template Name is required.\r";
			bWriteError = true;
		} else {
			// Template Name must be uniqe
			varMapName = document.form1.mapNames.value;
			if (varMapName.indexOf(document.form1.TemplateName.value) > 0 && document.form1.mapAction.value != 'update'){
				errmsg = errmsg + "- Template Name already exists. Please choose a new name.\r";
				bWriteError = true;
			}
		}
		<%
		end if 
		%>

		if (document.form1.stepValue.value == 2) {
			document.form1.action='ManageDataMaps_action.asp';
		}
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
//-->
</script>
</head>

<body><center>
<form name="form1" method="POST">
<input type="hidden" name="mapAction" value="<%=mapAction%>" />
<input type="hidden" name="mapColLimit" value="<%=mapColLimit%>" />
<input type="hidden" name="mapNames" value="<%=mapNames%>" />
<%
if mapAction = "update" then
' UPDATE Import Template
' -------------------------------------------------------
%>
<input type="hidden" name="stepValue" value="<%=stepValue%>" />
<input type="hidden" name="dataMapID" value="<%=dataMapID%>" />
<input type="hidden" name="TemplateName" value="<%=TemplateName%>" />
<input type="hidden" name="MapTypeID" value="19" />
<input type="hidden" name="Remarks" value="<%=Remarks%>" />
<input type="hidden" name="NumHeaderLines" value="<%=NumHeaderLines%>" />
<input type="hidden" name="NumDataCol" value="<%=NumDataCol%>" />
<input type="hidden" name="DataColDel" value="<%=DataColDel%>" />
<INPUT TYPE="hidden" NAME="UseWellCoordinates" VALUE="<%=UseWellCoordinates%>" />
<table border="0">
	<tr><td colspan="2" align="center">
		<span class="GuiFeedback">Update Import Template.</span><br /><br />
	</td></tr>
	<tr><%=ShowInputBox("Template Name:", "TemplateName", 30, "", bFieldsDisabled, True)%></tr>
	<tr><%=ShowInputBox("Remarks:", "Remarks", 30, "", bFieldsDisabled, True)%></tr>
	<tr><%=ShowInputBox("# File Header Lines:", "NumHeaderLines", 30, "", bFieldsDisabled, False)%></tr>
	<tr><%=ShowInputBox("# Data Columns:", "NumDataCol", 30, "", bFieldsDisabled, True)%></tr>
	<tr><%=ShowInputBox("Data Column Delimiter:", "DataColDel", 30, "", bFieldsDisabled, True)%></tr>
	<TR><TD ALIGN="right"><SPAN CLASS="required">Well Column Type:</SPAN><TD><INPUT TYPE="radio" NAME="iUseWellCoordinates" VALUE="0" ONCLICK="document.form1.UseWellCoordinates.value=this.value;" <%=selectWellNum%>>Well Numbers</TR>
	<TR><TD></TD><TD><INPUT TYPE="radio" NAME="iUseWellCoordinates" VALUE="1" ONCLICK="document.form1.UseWellCoordinates.value=this.value;" <%=selectWellCoord%>>Well Coordinates</TD></TR>
	<% 
	' Render Column Mappings
	' ----------------------
	if stepValue = "2" then
	sql_mappings = "Select map_field_id as Value, display_name as DisplayText from INV_DATA_MAPPINGS IDM Inner Join INV_MAP_FIELDS IMF on IMF.MAP_FIELD_ID=IDM.MAP_FIELD_ID_FK Where DATA_MAP_ID_FK = " & dataMapID & " And rownum < " & (CInt(NumDataCol)+1) & " Order by COLUMN_NUMBER"
	GetInvConnection()
	Set RS_mappings = Conn.Execute(sql_mappings)
	i = 1
	while not RS_mappings.EOF
	%>
	<tr><%=ShowPickList("Column " & i & " Mapping:", "ColumnMap" & i, RS_mappings("Value"), "Select map_field_id as Value, display_name as DisplayText from INV_MAP_FIELDS order by DisplayText")%></tr>
	<% 
		i = i+1
	RS_mappings.MoveNext
	Wend
	RS_mappings.Close()
	Set RS_mappings = Nothing
	end if
	
	' Render extra Column Mappings as needed
	' --------------------------------------
	if numExtraDisplayCol > 0 then
		for i=(numDataCol-numExtraDisplayCol+1) to numDataCol
	%>
	<tr><%=ShowPickList("Column " & i & " Mapping:", "ColumnMap" & i, "", "Select map_field_id as Value, display_name as DisplayText from INV_MAP_FIELDS order by DisplayText")%></tr>
	<% 
		next
	end if
	%>
	<tr><td colspan="2" align="right">
		<a HREF="#" onclick="history.back(); return false;"><img SRC="../graphics/cancel_dialog_btn.gif" border="0" WIDTH="61" HEIGHT="21"></a>
		<a HREF="#" onclick="Validate(); return false;">
		<% 
		if stepValue = "2" then
			Response.Write("<img SRC=""../graphics/ok_dialog_btn.gif"" border=""0"" WIDTH=""61"" HEIGHT=""21"">")
		else 
			Response.Write("<img SRC=""../graphics/btn_next_61.gif"" border=""0"" WIDTH=""61"" HEIGHT=""21"">")
		end if
		%>
	</td></tr>
</table>
<%
elseif mapAction = "delete" then
' DELETE Import Template
' -------------------------------------------------------
%>
<input type="hidden" name="stepValue" value="2" />
<table border="0">
	<tr><td colspan="2" align="center">
		<span class="GuiFeedback">Select an import template to delete.</span><br /><br />
	</td></tr>
	<tr><%=ShowPickList("Import Template:", "dataMapID", dataMapID, "Select data_map_id as Value, data_map_name as DisplayText from INV_DATA_MAPS order by DisplayText")%></tr>
	<tr><td colspan="2" align="right">&nbsp;</td></tr>
	<tr><td colspan="2" align="right">
		<a href="#" onclick="history.back(); return false;"><img src="../graphics/cancel_dialog_btn.gif" border="0" width="61" height="21"></a>
		<a href="#" onclick="Validate(); return false;"><img src="../graphics/ok_dialog_btn.gif" border="0" width="61" height="21">
	</td></tr>
</table>
<%
elseif mapAction = "copy" then
' COPY Import Template
' -------------------------------------------------------
%>
<input type="hidden" name="stepValue" value="2" />
<input type="hidden" name="TemplateName" value="<%=TemplateName%>" />
<input type="hidden" name="dataMapID" value="<%=dataMapID%>" />
<table border="0">
	<tr><td colspan="2" align="center">
		<span class="GuiFeedback">Please select a new name for your copy of <%=TemplateName%>.</span><br /><br />
	</td></tr>
	<tr><%=ShowInputBox("New Template Name:", "TemplateName", 30, "", false, True)%></tr>
	<tr><td colspan="2" align="right">&nbsp;</td></tr>
	<tr><td colspan="2" align="right">
		<a href="#" onclick="history.back(); return false;"><img src="../graphics/cancel_dialog_btn.gif" border="0" width="61" height="21"></a>
		<a href="#" onclick="Validate(); return false;"><img src="../graphics/ok_dialog_btn.gif" border="0" width="61" height="21">
	</td></tr>
</table>
<%
else
' CREATE Import Template
' -------------------------------------------------------
%>
<input type="hidden" name="stepValue" value="<%=stepValue%>" />
<input type="hidden" name="TemplateName" value="<%=TemplateName%>" />
<input type="hidden" name="MapTypeID" value="19" />
<input type="hidden" name="Remarks" value="<%=Remarks%>" />
<input type="hidden" name="NumHeaderLines" value="<%=NumHeaderLines%>" />
<input type="hidden" name="NumDataCol" value="<%=NumDataCol%>" />
<input type="hidden" name="DataColDel" value="<%=DataColDel%>" />
<INPUT TYPE="hidden" NAME="UseWellCoordinates" VALUE="<%=UseWellCoordinates%>" />
<table border="0">
	<tr><td colspan="2" align="center">
		<span class="GuiFeedback">Create an Import Template.</span><br /><br />
		<table bgcolor="#e1e1e1" width="100%"><tr><td align="center">
			Step <strong><%=stepValue%></strong> of 2
		</td></tr></table><br /><br />
	</td></tr>
	<tr><%=ShowInputBox("Template Name:", "TemplateName", 30, "", bFieldsDisabled, True)%></tr>
	<tr><%=ShowInputBox("Remarks:", "Remarks", 30, "", bFieldsDisabled, True)%></tr>
	<tr><%=ShowInputBox("# File Header Lines:", "NumHeaderLines", 30, "", bFieldsDisabled, False)%></tr>
	<tr><%=ShowInputBox("# Data Columns:", "NumDataCol", 30, "", bFieldsDisabled, True)%></tr>
	<tr><%=ShowInputBox("Data Column Delimiter:", "DataColDel", 30, "", bFieldsDisabled, True)%></tr>
	<TR><TD ALIGN="right"><SPAN CLASS="required">Well Column Type:</SPAN><TD><INPUT TYPE="radio" NAME="iUseWellCoordinates" VALUE="0" ONCLICK="document.form1.UseWellCoordinates.value=this.value;" <%=selectWellNum%>>Well Numbers</TR>
	<TR><TD></TD><TD><INPUT TYPE="radio" NAME="iUseWellCoordinates" VALUE="1" ONCLICK="document.form1.UseWellCoordinates.value=this.value;" <%=selectWellCoord%>>Well Coordinates</TD></TR>
	
	<% 
	if Request.Form("NumDataCol") <> "" then 
		for i=1 to Request.Form("NumDataCol")
	%>
	<tr><%=ShowPickList("Column " & i & " Mapping:", "ColumnMap" & i, "display_name", "Select map_field_id as Value, display_name as DisplayText from INV_MAP_FIELDS order by DisplayText")%></tr>
	<% 
		next
	end if 
	%>
	<tr><td colspan="2" align="right">
		<a HREF="#" onclick="history.back(); return false;"><img SRC="../graphics/cancel_dialog_btn.gif" border="0" WIDTH="61" HEIGHT="21"></a>
		<a HREF="#" onclick="Validate(); return false;">
		<% 
		if stepValue = "2" then
			Response.Write("<img SRC=""../graphics/ok_dialog_btn.gif"" border=""0"" WIDTH=""61"" HEIGHT=""21"">")
		else 
			Response.Write("<img SRC=""../graphics/btn_next_61.gif"" border=""0"" WIDTH=""61"" HEIGHT=""21"">")
		end if
		%>
	</td></tr>
</table>
<%
end if
%>
</form>
</center>
</body>
</html>
