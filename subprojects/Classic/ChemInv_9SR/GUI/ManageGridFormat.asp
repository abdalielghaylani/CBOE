<%@ Language=VBScript %>
<!--#INCLUDE VIRTUAL = "/cheminv/gui/guiUtils.asp"-->
<!--#INCLUDE VIRTUAL = "/cheminv/api/apiUtils.asp"-->
<%
Dim Conn
Dim RS

action = Request("action")
grid_format_id = Request("ID")
grid_format_type = "10"

row_use_letters= "0"
col_use_letters= "0"
number_start_corner= "13"
number_direction= "17"
zero_padding_count = "2"

If action <> "create" then
	Call GetInvConnection()
	SQL = "SELECT name, description, row_count, col_count, row_prefix, col_prefix, row_use_letters, col_use_letters, name_separator, number_start_corner, number_direction, zero_padding_count FROM inv_grid_format WHERE Grid_Format_ID=" & grid_format_id
	'Response.Write SQL
	'Response.end
	Set RS= Conn.Execute(SQL)
	name=RS("name").value
	description= RS("description").value
	row_count= RS("row_count").value
	col_count= RS("col_count").value
	row_prefix= RS("row_prefix").value
	col_prefix= RS("col_prefix").value
	row_use_letters= RS("row_use_letters").value
	col_use_letters= RS("col_use_letters").value
	name_separator= RS("name_separator").value
	number_start_corner= RS("number_start_corner")
	number_direction= RS("number_direction")
	zero_padding_count = RS("zero_padding_count")
	if isNull(zero_padding_count) then zero_padding_count = "2"
End if

%>
<html>
<head>
<title><%=Ucase(Left(action,1)) & Mid(action, 2, Len(action)-1)%> Grid Format</title>
<script LANGUAGE="javascript" src="/cheminv/Choosecss.js"></script>
<script LANGUAGE="javascript" src="/cheminv/gui/validation.js"></script>
<script language="JavaScript">
<!--
	var currGridFormat = "<%=currGridFormat%>";
	window.focus();
	
	function Validate(){
		
		var bWriteError = false;
		var errmsg = "Please fix the following problems:\r";
	<%if action <> "delete" then%>
		// Name is required
		if (document.form1.p_Name.value.length == 0) {
			errmsg = errmsg + "- Grid Format Name is required.\r";
			bWriteError = true;
		}
		// RowCount is required
		if (document.form1.p_row_count.value.length == 0) {
			errmsg = errmsg + "- Row Count is required.\r";
			bWriteError = true;
		}
		else{
			// RowCount must be a number
			if (!isPositiveNumber(document.form1.p_row_count.value)){
			errmsg = errmsg + "- Row Count must be a positive number.\r";
			bWriteError = true;
			}
			if (document.form1.p_row_count.value > 100){
			errmsg = errmsg + "- Maximum Row Count allowed is 100.\r";
			bWriteError = true;
			}
		}
		// ColCount is required
		if (document.form1.p_col_count.value.length == 0) {
			errmsg = errmsg + "- Column Count is required.\r";
			bWriteError = true;
		}
		else{
			// ColCount must be a number
			if (!isPositiveNumber(document.form1.p_col_count.value)){
			errmsg = errmsg + "- Column Count must be a positive number.\r";
			bWriteError = true;
			}
			if (document.form1.p_col_count.value > 100){
			errmsg = errmsg + "- Maximum Column Count allowed is 100.\r";
			bWriteError = true;
			}
		}
		<%end if%>
		if (bWriteError){
			alert(errmsg);
		}
		else{
			document.form1.submit();
		}
	}
//-->
</script>
</head>

<body>
<center>
<form name="form1" action="ManageGridFormat_action.asp?action=<%=action%>" method="POST">
<input type="hidden" name="p_grid_format_id" value="<%=grid_format_id%>">
<input type="hidden" name="p_grid_format_type" value="<%=grid_format_type%>">

<table border="0">
<%if action = "delete" then%>
	<tr>
		<td colspan="2">
			<span class="GuiFeedback">Are you sure you want to delete this Grid Format:</span><br><br>
		</td>
	</tr>
	<tr>
		<td align="right">
			<span class="required">Grid Format Name:</span>
		</td>
		<td>
			<input TYPE="tetx" SIZE="25" Maxlength="50" NAME="p_Name" value="<%=Name%>" disabled>
		</td>
	</tr>

<%else%>
	<tr>
		<td colspan="2">
			<span class="GuiFeedback">Grid Format Attributes:</span><br><br>
		</td>
	</tr>
	<tr>
		<td align="right">
			<span class="required">Grid Format Name:</span>
		</td>
		<td>
			<input TYPE="tetx" SIZE="25" Maxlength="50" NAME="p_Name" value="<%=Name%>">
		</td>
	</tr>
	<tr>
		<td align="right" nowrap>
			<span class="required">Row Count:<span>
		</td>
		<td>
			<input TYPE="tetx" SIZE="10" Maxlength="3" NAME="p_row_count" value="<%=Row_Count%>">
		</td>
	</tr>
	<tr>
		<td align="right" nowrap>
			<span class="required">Column Count:<span>
		</td>
		<td>
			<input TYPE="tetx" SIZE="10" Maxlength="3" NAME="p_col_count" value="<%=Col_Count%>">
		</td>
	</tr>
	<tr>
		<td align="right">
			<span>Description:</span>
		</td>
		<td>
			<input TYPE="tetx" SIZE="40" Maxlength="50" NAME="p_description" value="<%=Description%>">
		</td>
	</tr>
	<tr>
		<td align="right" nowrap>
			<span>Row Prefix:<span>
		</td>
		<td>
			<input TYPE="tetx" SIZE="3" Maxlength="3" NAME="p_row_prefix" value="<%=Row_Prefix%>">
		</td>
	</tr>
	<tr>
		<td align="right" nowrap>
			<span>Column Prefix:<span>
		</td>
		<td>
			<input TYPE="tetx" SIZE="3" Maxlength="3" NAME="p_col_prefix" value="<%=col_Prefix%>">
		</td>
	</tr>
	<tr>
		<td align="right" nowrap>
			<span>Row Labels:<span>
		</td>
		<td>
			<select name="p_row_use_letters">
				<option value="0">Use Numbers
				<option value="1">Use Letters
			</select>
		</td>
	</tr>
	<tr>
		<td align="right" nowrap>
			<span>Column Labels:<span>
		</td>
		<td>
			<select name="p_col_use_letters">
				<option value="0">Use Numbers
				<option value="1">Use Letters
			</select>
		</td>
	</tr>
	<tr>
		<td align="right" nowrap>
			<span title="Final number of digits with zero padding.  If # is 2 then 1 becomes 01.  If # is 3 then 1 becomes 001.">Zero Pad to # of digits:<span>
		</td>
		<td>
			<select name="p_zero_padding_count">
				<option value="1">No Padding</option>
				<option value="2">2</option>
				<option value="3">3</option>
			</select>
		</td>
	</tr>
	<tr>
		<td align="right" nowrap>
			<span>Name Separator:<span>
		</td>
		<td>
			<input TYPE="tetx" SIZE="1" Maxlength="1" NAME="p_name_separator" value="<%=name_separator%>">
		</td>
	</tr>
	<tr>
		<td align="right" nowrap>
			<span>Start at:<span>
		</td>
		<td>
			<select name="p_number_start_corner">
				<option value="13">Upper Left
				<option value="14">Upper Right
				<option value="15">Lower Left
				<option value="16">Lower Right
			</select>
		</td>
	</tr>
	<tr>
		<td align="right" nowrap>
			<span>Fill Order:<span>
		</td>
		<td>
			<select name="p_number_direction">
				<option value="17">Rows First
				<option value="18">Columns First
			</select>
		</td>
	</tr>
<%end if%>
	<tr>
		<td colspan="2" align="right"> 
			<a HREF="#" onclick="history.back(); return false;"><img SRC="../graphics/cancel_dialog_btn.gif" border="0" WIDTH="61" HEIGHT="21"></a><a HREF="#" onclick="Validate(); return false;"><img SRC="../graphics/ok_dialog_btn.gif" border="0" WIDTH="61" HEIGHT="21"></a>
		</td>
	</tr>
</table>	
</form>
</center>
<%if action <> "delete" then%> 
<script LANGUAGE="javascript">
	document.form1.p_row_use_letters.value = "<%=row_use_letters%>";
	document.form1.p_col_use_letters.value = "<%=col_use_letters%>";
	document.form1.p_number_start_corner.value = "<%=number_start_corner%>";
	document.form1.p_number_direction.value = "<%=number_direction%>";
	document.form1.p_zero_padding_count.value = "<%=Zero_Padding_Count%>";
</script>
<%end if%>
</body>
</html>
