<%@ Language=VBScript %>
<!--#INCLUDE VIRTUAL = "/cheminv/api/apiUtils.asp"-->
<!--#INCLUDE VIRTUAL = "/cheminv/gui/guiUtils.asp"-->
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/display_func_vbs.asp"-->
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/utility_func_vbs.asp"-->
<%
Dim Conn
action = Request("action")
multiSelect = Request("multiSelect")
if Lcase(multiSelect) = "true" then 
	plateCount =  plate_multiSelect_dict.count
	if plateCount = 0 then 	
		action = "noPlates"
	else
		PlateID = DictionaryToList(plate_multiSelect_dict)
	end if
End if

'Response.Write request("action") & ":<BR>"
'Response.Write request("containerFields") & ":<BR>"


%>
<html>
<head>
<title><%=Application("appTitle")%> -- Update Inventory Plates</title>

<!--#INCLUDE VIRTUAL = "/cheminv/gui/dateFormat_js.asp"-->
<script LANGUAGE="javascript" src="/cheminv/Choosecss.js"></script>
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/navbar_js.js"-->
<script LANGUAGE="javascript" src="/cheminv/gui/validation.js"></script>
<script LANGUAGE="javascript" src="/cheminv/utils.js"></script>
<script language="JavaScript">
<!--Hide JavaScript
	window.focus();
-->
</script>
<SCRIPT LANGUAGE="javascript">
	// Validates container attributes
	function ValidatePlate(strMode){	
		var bWriteError = false;
		var errmsg = "Please fix the following problems:\r";
		
		if (strMode.toLowerCase() == "edit") {
			if (document.form1.plateFields.options[0].selected && document.form1.plateFields.options[1].selected)
			{
				errmsg = errmsg + "- You must select either F/T Cycles or F/T Cycles(+/-) not both.\r";
				bWriteError = true;
			}
			if (document.form1.plateFields.selectedIndex == -1) {
				errmsg = errmsg + "- You must select at least one field to update.\r";
				bWriteError = true;
			}
		}
		else {
			fields = document.form1.fieldList.value.toString();
			//alert(document.form1.fieldList.value);
			//alert(fields);
			arrFields = fields.split(",");
			bRequiredFilled = true;
			for (i=0; i<arrFields.length; i++) {
				if (bRequiredFilled && eval("document.form1.i" + arrFields[i] + ".value.length == 0")) {
					errmsg = errmsg + "- All fields are required\r";
					bWriteError = true;
					bRequiredFilled = false;
				}
				switch (arrFields[i]) {
					case "qty_initial":
						// if present
						if (document.form1.iqty_initial.value.length >0){
							// QtyInit must be a positive number
							if(!isPositiveNumber(document.form1.iqty_initial.value)){
								errmsg = errmsg + "- Initial Quantity must be a positive number.\r";
								bWriteError = true;
							}
						}
						break;
					case "qty_remaining":
						// if present
						if (document.form1.iqty_remaining.value.length >0){
							// QtyRemaining must be a positive number
							if (!isPositiveNumber(document.form1.iqty_remaining.value)){
								errmsg = errmsg + "- Quantity Remaining must be a positive number.\r";
								bWriteError = true;
							}
						}
						// If QtyMax and QtyRemaining are present, QtyRemaining cannot be larger than QtyMax
						//if (document.form1.iqty_remaining.value.length >0 && document.form1.iqty_max.value.length >0 &&  document.form1.iqty_remaining.value/1 > document.form1.iqty_max.value/1){
						//	errmsg = errmsg + "- Quantity Remaining cannot be larger than Container Size.\r";
						//	bWriteError = true;
						//} 
						break;
					case "weight":
						// if present
						if (document.form1.iweight.value.length >0){
							// Weight must be a positive number
							if (!isPositiveNumber(document.form1.iweight.value)){
								errmsg = errmsg + "- Weight must be a positive number.\r";
								bWriteError = true;
							}
						}
						break;
					case "concentration":
						// Concentration if present must be a positive number
						if (document.form1.iconcentration.value.length >0 && !isPositiveNumber(document.form1.iconcentration.value)){
							errmsg = errmsg + "- Concentration must be a positive number.\r";
							bWriteError = true;
						}
						break;
					case "solvent_volume":
						// Solvent Volume if present must be a positive number
						if (document.form1.isolvent_volume.value.length >0 && !isPositiveNumber(document.form1.isolvent_volume.value)){
							errmsg = errmsg + "- Solvent Volume must be a positive number.\r";
							bWriteError = true;
						}
						break;
					case "solution_volume":
						// Solution Volume if present must be a positive number
						if (document.form1.isolution_volume.value.length >0 && !isPositiveNumber(document.form1.isolution_volume.value)){
							errmsg = errmsg + "- Solution Volume must be a positive number.\r";
							bWriteError = true;
						}
						break;

				//Custom field validation
				<%For each Key in custom_fields_dict
					If InStr(lcase(Key), "date") then%>
					case "<%=Key%>":
						if (document.form1.i<%=Key%>.value.length > 0 && !isDate(document.form1.i<%=Key%>.value)){
							errmsg = errmsg + "- <%=custom_fields_dict.Item(Key)%> must be in " + dateFormatString + " format.\r";
							bWriteError = true;
						}
						break;
					<%end if%>
				<%next%>					
					default:
						break;
				}
			}
		}
		// Report problems
		if (bWriteError){
			alert(errmsg);
			return false;
		}
		else{
			var bcontinue = true;
			
			/*if ((document.form1.CompoundID.value.length == 0)&&(document.form1.RegID.value.length == 0)){
				bcontinue = confirm("No Compound has been asigned to this container.\rDo you really want to create a container without an associated chemical compound?");
			}*/
			if (bcontinue) document.form1.submit();
		}
	}
	 

</SCRIPT>
</head>
<body>
<center>
<%
if action="edit" then
	call ShowSelectFieldsForm()
elseif action = "noPlates" then
	call ShowNoContainersError()
else
	call ShowFieldsForm()
end if
%>
</center>
</body>
</html>

<%sub ShowSelectFieldsForm()%>
<form name="form1" action="UpdatePlate.asp" method="POST">
<input Type="hidden" name="iPlateIDs" value="<%=PlateID%>">
<input Type="hidden" name="multiscan" value="<%=Request("multiscan")%>">
<INPUT TYPE="hidden" NAME="multiSelect" VALUE="<%=multiSelect%>">
<INPUT TYPE="hidden" NAME="action" VALUE="dataEntry">
<table border="0">
	<tr>
		<td colspan="2">
			<span class="GuiFeedback"><br><br><center>Select the fields you wish to update.</center></span><br><br>
		</td>
	</tr>
	<tr>
		<td align="right" valign="top" nowrap>Plate Fields:</td>
		<td>		
			<SELECT NAME="plateFields" SIZE="8" MULTIPLE>
				<OPTION VALUE="F/T Cycles:ft_cycles">F/T Cycles</OPTION>
				<OPTION VALUE="F/T Cycles:ft_cycles_increment">F/T Cycles(+/-)</OPTION>
				<OPTION VALUE="Status:status_id_fk">Status</OPTION>
				<OPTION VALUE="Group Name:group_name">Group name</OPTION>
				<OPTION VALUE="Library:library_id_fk">Library</OPTION>
				<OPTION VALUE="Concentration:concentration">Concentration</OPTION>
				<OPTION VALUE="Concentration Unit:conc_unit_fk">Concentration Unit</OPTION>
				<OPTION VALUE="Plate Type:plate_type_id_fk">Plate Type</OPTION>
				<OPTION VALUE="Quantity Initial:qty_initial">Quantity Initial</OPTION>
				<OPTION VALUE="Quantity Remaining:qty_remaining">Quantity Remaining</OPTION>
				<OPTION VALUE="Quantity Unit:qty_unit_fk">Quantity Unit</OPTION>
				<OPTION VALUE="Weight:weight">Weight</OPTION>
				<OPTION VALUE="Weight Unit:weight_unit_fk">Weight Unit</OPTION>
				<OPTION VALUE="Solvent:solvent_id_fk">Solvent</OPTION>
				<OPTION VALUE="Solvent Volume:solvent_volume">Solvent Volume</OPTION>
				<OPTION VALUE="Solution Volume:solution_volume">Solution Volume</OPTION>
				<OPTION VALUE="Solvent Volume:solvent_volume_unit_id_fk">Solvent/Solution Volume Unit</OPTION>
				<OPTION VALUE="Supplier:supplier">Supplier</OPTION>
<%
				For each key in custom_plate_fields_dict
					theOption = "<OPTION VALUE=""" & custom_plate_fields_dict.Item(key) & ":" & Key & """>" & custom_plate_fields_dict.Item(key) & "</OPTION>" & req_custom_plate_fields_dict.Exists(Key)
					Response.Write theOption
				Next 
%>
			</SELECT>
		</td>
	</tr>
	<tr>
		<td colspan="2" align="right"> 
			<a HREF="#" onclick="window.close(); return false;"><img SRC="../graphics/cancel_dialog_btn.gif" border="0" WIDTH="61" HEIGHT="21"></a><a HREF="#" onclick="ValidatePlate('Edit'); return false;"><img SRC="../graphics/ok_dialog_btn.gif" border="0" WIDTH="61" HEIGHT="21"></a>
		</td>
	</tr>	
</table>	
</form>
<%end sub%>

<%sub ShowFieldsForm()%>

<%
fields = Request("plateFields")
arrFields = split(fields,",")

fieldColumns = ""
for i = 0 to ubound(arrFields)
	fieldColumns = fieldColumns & Right(arrFields(i),len(arrFields(i))-instrrev(arrFields(i),":")) & ","
next
fieldColumns = left(fieldColumns, len(fieldColumns)-1)

'for i = 0 to ubound(arrFields)
'	Response.Write "field=" & arrFields(i) & "<BR>"
	'Response.Write "type=" & arrTypes(i) & "<BR>"
'next
%>
<form name="form1" action="UpdatePlate_action.asp" method="POST">
<input Type="hidden" name="iPlateIDs" value="<%=PlateID%>">
<input Type="hidden" name="multiscan" value="<%=Request("multiscan")%>">
<INPUT TYPE="hidden" NAME="multiSelect" VALUE="<%=multiSelect%>">
<INPUT TYPE="hidden" NAME="fieldList" VALUE="<%=fieldColumns%>">

<table border="0">
	<tr>
		<td colspan="2">
			<span class="GuiFeedback"><br><br><center>Enter values for the selected fields.<BR>All selected fields are required.</center></span><br><br>
		</td>
	</tr>
<%
for i = 0 to ubound(arrFields)
	fieldName = trim(arrFields(i))
	'Response.Write right(fieldName,2) & ":<BR>"
	if right(fieldName,2) = "fk" then
		Select case fieldName
			Case "Location:location_id_fk"
				Response.Write "<TR><TD align=""right"" valign=""top"" width=""150"" nowrap>Location ID:</TD><TD VALIGN=""top"">" &GetBarcodeIcon() & "&nbsp;"
				call ShowLocationPicker("document.form1", "ilocation_id_fk", "lpLocationBarCode", "lpLocationName", 10, 25, false)
				Response.Write "</TD></TR>"
			'Case "Solvent:solvent_id_fk"
			'	Response.Write("<tr height=""25""><td align=""right"" nowrap>Select Solvent:</td><td>")
			'	Response.Write(ShowSelectBox2("isolvent_id_fk",solvent_id_fk,"SELECT solvent_id AS Value, solvent_name AS DisplayText FROM " & Application("CHEMINV_USERNAME") & ".inv_solvents ORDER BY lower(DisplayText) ASC", null, "Select a Solvent", "null"))
			'	Response.Write("</td></tr>")
			'Case "Library:library_id_fk"
			Case else	
				call CreateSelectFieldRow(arrFields(i))
		end Select
	else
		Select case fieldName
			Case else
				call CreateTextFieldRow(arrFields(i))
		end select
	end if
next

%>
	<tr>
		<td colspan="2" align="right"> 
			<a HREF="#" onclick="history.back(); return false;"><img SRC="../graphics/cancel_dialog_btn.gif" border="0" WIDTH="61" HEIGHT="21"></a><a HREF="#" onclick="ValidatePlate('Submit'); return false;"><img SRC="../graphics/ok_dialog_btn.gif" border="0" WIDTH="61" HEIGHT="21"></a>
		</td>
	</tr>	
</table>	
</form>

<%end sub%>


<%sub CreateTextFieldRow(field)

	fieldName = Left(field,instr(field,":"))
	fieldColumn = Right(field,len(field)-instrrev(field,":"))
	Response.Write "<TR>"
	Select Case lcase(fieldColumn)
		case "date_1", "date_2"
			theRow = "<TD ALIGN=""right"" VALIGN=""top"" WIDTH=""150"" NOWRAP>" & fieldName & "</TD>"
			theRow = theRow & "<TD>" & getShowInputField("", "", "i" & fieldColumn & ":form1", "DATE_PICKER:TEXT", "15") & "</TD>"
			'<TD><INPUT TYPE=""text"" NAME=""i" & fieldColumn & """ SIZE=""15"" VALUE=""""><A HREF ONCLICK=""return PopUpDate(&quot;i" & fieldColumn & "&quot;,&quot;/cheminv/gui/month_view1.asp&quot;)""><IMG SRC=""/cfserverasp/source/graphics/navbuttons/icon_mview.gif"" HEIGHT=""16"" WIDTH=""16"" BORDER=""0""></A></TD>"
			'theRow = theRow & "<a href onclick=""return PopUpDate(&quot;" & fieldColumn & "&quot;,&quot;/cheminv/gui/month_view1.asp&quot;)""><img src=""/cfserverasp/source/graphics/navbuttons/icon_mview.gif"" height=""16"" width=""16"" border=""0""></a>"
		case "ft_cycles_increment"
			theRow = "<TD ALIGN=""right"" VALIGN=""top"" WIDTH=""150"" NOWRAP>" & fieldName & "</TD>"
			theRow = theRow & "<TD><SELECT NAME=""i" & fieldColumn & """ SIZE=""1""><OPTION VALUE=""+5"">+5</OPTION><OPTION VALUE=""+4"">+4</OPTION><OPTION VALUE=""+3"">+3</OPTION><OPTION VALUE=""+2"">+2</OPTION><OPTION VALUE=""+1"" SELECTED>+1</OPTION>"
			theRow = theRow & "<OPTION VALUE=""-1"">-1</OPTION><OPTION VALUE=""-2"">-2</OPTION><OPTION VALUE=""-3"">-3</OPTION><OPTION VALUE=""-4"">-4</OPTION><OPTION VALUE=""-5"">-5</OPTION></SELECT> (increment/decrement)</TD>"
		case else
			theRow = ShowInputBox(fieldName, fieldColumn, 15, "", False, false)
	end select
	'Response.Write field & "<BR>"
	'Response.Write Left(field,instr(field,":")-1) & "<BR>"
	'Response.Write Right(field,len(field)-instrrev(field,":")) & "<BR>"
	'Response.Write len(field)-instrrev(field,":") & "<BR>"

	Response.Write theRow
	Response.Write "</TR>"
end sub

sub CreateSelectFieldRow(field)

	fieldName = Left(field,instr(field,":"))
	fieldColumn = "i" & Right(field,len(field)-instrrev(field,":"))
	Response.Write "<TR>"
	Select Case fieldColumn
		Case "iplate_type_id_fk"
			sql = "SELECT plate_type_id AS Value, plate_type_name AS DisplayText FROM " & Application("CHEMINV_USERNAME") & ".inv_plate_types ORDER BY lower(DisplayText) ASC"
		Case "isolvent_id_fk"
			sql = "SELECT solvent_id AS Value, solvent_name AS DisplayText FROM " & Application("CHEMINV_USERNAME") & ".inv_solvents ORDER BY lower(DisplayText) ASC"
		Case "ilibrary_id_fk"
			sql = "SELECT enum_id AS Value, enum_value AS DisplayText FROM inv_enumeration WHERE eset_id_fk = 5 ORDER BY lower(DisplayText) ASC"
		Case "istatus_id_fk"
			sql = "SELECT Enum_ID AS Value, Enum_Value AS DisplayText FROM inv_enumeration, inv_enumeration_set WHERE Eset_name = 'Plate Status' and eset_id_fk = eset_id ORDER BY lower(DisplayText) ASC"
		Case "iconc_unit_fk"
			sql = "SELECT UNIT_ID AS Value, Unit_Name AS DisplayText FROM inv_units WHERE Unit_type_id_FK IN (3) ORDER BY lower(DisplayText) ASC"	
		Case "iqty_unit_fk"
			sql = "SELECT UNIT_ID AS Value, Unit_Name AS DisplayText FROM inv_units WHERE Unit_type_id_FK IN (1,2) ORDER BY lower(DisplayText) ASC"
		Case "iweight_unit_fk"
			sql = "SELECT UNIT_ID AS Value, Unit_Name AS DisplayText FROM inv_units WHERE Unit_type_id_FK IN (2) ORDER BY lower(DisplayText) ASC"
		Case "isolvent_volume_unit_id_fk"
			sql = "SELECT UNIT_ID AS Value, Unit_Name AS DisplayText FROM inv_units WHERE Unit_type_id_FK IN (1) ORDER BY lower(DisplayText) ASC"
		Case else
	end select

	theRow = ShowPickList(fieldName, fieldColumn, "", sql)
	Response.Write theRow
	Response.Write "</TR>"

end sub
%>
<%sub ShowNoContainersError()%>
<table border="0">
	<tr>
		<td colspan="2">
			<span class="GuiFeedback"><br><br><center>You must select plates to update.</center></span><br><br>
		</td>
	</tr>
	<tr>
		<td colspan="2" align="right"> 
			<a HREF="#" onclick="window.close(); return false;"><img SRC="../graphics/ok_dialog_btn.gif" border="0" WIDTH="61" HEIGHT="21"></a>
		</td>
	</tr>	
</table>	

<%end sub%>
