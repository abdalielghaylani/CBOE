<%@ Language=VBScript %>
<!--#INCLUDE VIRTUAL = "/cheminv/gui/guiUtils.asp"-->
<!--#INCLUDE VIRTUAL = "/cheminv/api/apiUtils.asp"-->
<%
Dim Conn
%>
<html>
<head>
<title><%=Application("appTitle")%> Administration Menu</title>
<script LANGUAGE="javascript" src="/cheminv/Choosecss.js"></script>
<script LANGUAGE="javascript">
	window.focus();
	function ValidateClick(elmName, url, action){
		if ((document.form1[elmName].value == "") && (action != "create")){
			alert("Please select an item to " + action + " from the list.")
		}
		else{
			document.form1.action = url + "?action=" + action + "&ID=" + document.form1[elmName].value;
			document.form1.submit();
		}
	}
</script>

</head>
<body>

<center>
<form name="form1" method="POST">
<table border="0">
	<tr>
		<td colspan="2">
			<span class="GuiFeedback">Select an item from a list and click the link next to it.</span><br><br>
		</td>
	</tr>

	<!--<tr>
		<td align="right" nowrap>
			<span title="define a grid of sublocations where you can store inventory containers or plates">Grid Formats:</span>
		</td>
		<td>
			<%=ShowSelectBox2("GridFormatID", "", "SELECT Grid_Format_ID AS Value, Name AS DisplayText FROM inv_Grid_Format WHERE Grid_Format_Type_FK = 10 ORDER BY Name ASC", 45, RepeatString(50, "&nbsp;"), "")%>
			<%=GetNewEditDelLinks("GridFormatID", "ManageGridFormat.asp")%>
		</td>
	</tr>-->
	<tr>
		<td align="right" nowrap>
			<span title="define the physical attributes of a plate such as rows, columns, well capacity...">Physical Plate Types:</span>
		</td>
		<td>
			<%=ShowSelectBox2("PhysPlateFormatID", "", "SELECT phys_plate_ID AS Value, phys_plate_name AS DisplayText FROM inv_physical_plate ORDER BY phys_plate_Name ASC", 45, RepeatString(50, "&nbsp;"), "")%>
			<%=GetNewEditDelLinks("PhysPlateFormatID", "ManagePhysPlates.asp")%>
	
		</td>
	</tr>
	<tr>
		<td align="right" nowrap>
			<span title="define layout of wells within a plate such as empty rows or control rows">Plate Formats:</span>
		</td>
		</td>
		<td>
			<%=ShowSelectBox2("PlateFormatID", "", "SELECT plate_format_ID AS Value, plate_format_name AS DisplayText FROM inv_plate_Format ORDER BY plate_format_name ASC", 45, RepeatString(50, "&nbsp;"), "")%>
			<%=GetNewEditDelLinks("PlateFormatID", "ManagePlateFormats.asp")%>			
		</td>
	</tr>
	<tr>
		<td align="right" nowrap>
			<span title="Groups plates by allowed number of freeze/thaw cycles">Plate Types:</span>
		</td>
		<td>
			<%=ShowSelectBox2("PlateTypeID", "", "SELECT plate_type_ID AS Value, plate_type_name AS DisplayText FROM inv_plate_types ORDER BY plate_type_name ASC", 45, RepeatString(50, "&nbsp;"), "")%>
			<%=GetNewEditDelLinks("PlateTypeID", "ManagePlateTypes.asp")%>					
		</td>
	</tr>
	<tr>
		<td align="right" nowrap></span>
			<span title="define the possible contents of a well such as empty, compoud, control...">Well Formats:</span>
		</td>
		<td>
			<%=ShowSelectBox2("WellFormatID", "", "SELECT enum_ID AS Value, enum_value AS DisplayText FROM inv_enumeration WHERE eset_id_fk = 1 ORDER BY enum_value ASC", 45, RepeatString(50, "&nbsp;"), "")%>
			<%=GetNewEditDelLinks("WellFormatID", "ManageWellContentTypes.asp")%>					
		</td>
	</tr>
	<tr>
		<td align="right" nowrap></span>
			<span title="Create maps for reformatting plates.">Reformatting Maps:</span>
		</td>
		<td>		
			<%=ShowSelectBox2("XmlDocID", "", "SELECT XMLDOC_ID AS Value, NAME AS DisplayText FROM inv_xmldocs WHERE xmldoc_type_id_FK IN (1) ORDER BY lower(DisplayText) ASC", 45, RepeatString(50, "&nbsp;"), "")%>
			<%
			url = "ManageReformatMaps.asp"
			elmName = "XmlDocID"
			str = "&nbsp;"
			str = str & GetLink("#", "ValidateClick('" & elmName & "','" & url & "','create'); return false", "New")
			str = str & " | " & GetLink(url & "Delete", "ValidateClick('" & elmName & "','" & url & "','delete'); return false", "Delete")
			'str= str & " | " & GetLink(url & "Delete", "ValidateClick('" & elmName & "','" & url & "','delete'); return false", "Delete")
			Response.Write str
			%>					
		</td>
	</tr>
	<tr>
		<td align="right" nowrap></span>
			<span title="Create templates to be used for importing plates.">Plate Import Templates:</span>
		</td>
		<td>
			<%=ShowSelectBox2("DataMapID", "", "SELECT data_map_id AS Value, data_map_name AS DisplayText FROM inv_data_maps ORDER BY data_map_name ASC", 45, RepeatString(50, "&nbsp;"), "")%>
			<%
			url = "ManageDataMaps.asp"
			elmName = "DataMapID"
			str = "&nbsp;"
			str = str & GetLink("#", "ValidateClick('" & elmName & "','" & url & "','create'); return false", "New")
			str = str & " | " & GetLink(url & "Edit", "ValidateClick('" & elmName & "','" & url & "','update'); return false", "Edit")
			str = str & " | " & GetLink(url & "Edit", "ValidateClick('" & elmName & "','" & url & "','copy'); return false", "Copy")
			str= str & " | " & GetLink(url & "Delete", "ValidateClick('" & elmName & "','" & url & "','delete'); return false", "Delete")
			Response.Write str
			%>
		</td>	
	</tr>
	<tr>
		<td colspan="2" align="right"><a HREF="#" onclick="window.location='/cheminv/gui/menu.asp'; return false;"><img SRC="../graphics/close.gif" border="0" WIDTH="61" HEIGHT="21"></a></td>
	</tr>
</table>	
</form>
</center>
</body>
</html>
