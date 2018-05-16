<%@ Language=VBScript %>
    <!--#INCLUDE VIRTUAL = "/cheminv/gui/guiUtils.asp"-->
    <!--#INCLUDE VIRTUAL = "/cheminv/api/apiUtils.asp"-->
    <%
Dim Conn
Dim RS

action = Request("action")
GridType = Request("GridType")
grid_format_id = Request("ID")
grid_format_type = "10"

row_use_letters= "0"
col_use_letters= "0"
number_start_corner= "13"   ' Upper Left
number_direction= "17"      ' Rows First
zero_padding_count = "2"
cell_naming = "0"
name_delimeter = "/"
if GridType = "rack" then
	GridText = "Rack Format"
else
	GridText = "Grid Format"
end if

If action <> "create" then
	Call GetInvConnection()
	SQL = "SELECT name, description, row_count, col_count, row_prefix, col_prefix, row_use_letters, col_use_letters, name_separator, number_start_corner, number_direction, zero_padding_count, cell_naming, name_delimeter FROM inv_grid_format WHERE Grid_Format_ID=?"
	'Response.Write SQL
	'Response.end
	set Cmd = server.createobject("ADODB.Command")
	Cmd.CommandText = SQL
	Cmd.ActiveConnection = Conn
	Cmd.Parameters.Append Cmd.CreateParameter("grid_format_id",131, 1, 0, grid_format_id)
	Set RS= Cmd.Execute
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
	cell_naming = RS("cell_naming")
	name_delimeter = RS("Name_Delimeter")
	if isNull(zero_padding_count) then zero_padding_count = "2"
	if isNull(cell_naming) then cell_naming = "0"
End if

if cell_naming = "1" then
	displayNamingRowCol = "hidden"
	displayNamingSeq = "visible"
else
	displayNamingRowCol = "visible"
	displayNamingSeq = "hidden"
end if
%>
        <html>

        <head>
            <title>
                <%=Ucase(Left(action,1)) & Mid(action, 2, Len(action)-1)%>
                    <%=GridText%>
            </title>
            <style>
                .namingRowCol {
                    visibility: <%=displayNamingRowCol%>;
                }
                
                .namingSeq {
                    visibility: <%=displayNamingSeq%>;
                }
            </style>
            <script language="javascript" src="/cheminv/choosecss.js"></script>
            <script language="javascript" src="/cheminv/gui/validation.js"></script>
            <script language="javascript" src="/cheminv/utils.js"></script>

            <script language="JavaScript">
                <!--
                var currGridFormat = "<%=currGridFormat%>";
                window.focus();

                function Validate() {

                    var bWriteError = false;
                    var errmsg = "Please fix the following problems:\r";
                    <%if action <> "delete" then%>
                    // Name is required
                    if (document.form1.p_Name.value.length == 0) {
                        errmsg = errmsg + "- <%=GridText%> Name is required.\r";
                        bWriteError = true;
                    } else {
                        var dbName = "<%=name%>";
                        if (document.form1.p_Name.value != dbName) {
                            if (ValidateUniqueName('gridformat', document.form1.p_Name.value) >= 1) {
                                errmsg = errmsg + "- The Name already exists, please enter a different name.\r";
                                bWriteError = true;
                            }
                        }
                    }
                    // RowCount is required
                    if (document.form1.p_row_count.value.length == 0) {
                        errmsg = errmsg + "- Row Count is required.\r";
                        bWriteError = true;
                    } else {
                        // RowCount must be a number
                        if (!isPosLongInteger(document.form1.p_row_count.value)) {
                            errmsg = errmsg + "- Row Count must be a positive integer.\r";
                            bWriteError = true;
                        }
                        if (document.form1.p_row_count.value > 100) {
                            errmsg = errmsg + "- Maximum Row Count allowed is 100.\r";
                            bWriteError = true;
                        }
                    }
                    // ColCount is required
                    if (document.form1.p_col_count.value.length == 0) {
                        errmsg = errmsg + "- Column Count is required.\r";
                        bWriteError = true;
                    } else {
                        // ColCount must be a number
                        if (!isPosLongInteger(document.form1.p_col_count.value)) {
                            errmsg = errmsg + "- Column Count must be a positive integer.\r";
                            bWriteError = true;
                        }
                        if (document.form1.p_col_count.value > 100) {
                            errmsg = errmsg + "- Maximum Column Count allowed is 100.\r";
                            bWriteError = true;
                        }
                    }
                    <%end if%>
                    if (bWriteError) {
                        alert(errmsg);
                    } else {
                        bConfirmWarning = true;
                        bcontinue = true;
                        var errmsg = "Please fix the following problems:\r";
                        <% if action = "update" or action="delete" then 
			if action = "update" then
				warningText = "Updating"
			else
				warningText = "Deleting"
			end if
			Call GetInvConnection()
            SQL= "select count(location_id_fk) as locationCount from inv_grid_element, inv_grid_format,inv_grid_position where inv_grid_element.grid_position_id_fk= inv_grid_position.grid_position_id and inv_grid_position.grid_format_id_fk = inv_grid_format.grid_format_id and inv_grid_format.grid_format_id =" & clng(grid_format_id)
			Set RS= Conn.Execute(SQL)
			
			if cint(RS("locationCount").value) > 0 then 
			%>
                        bcontinue = false;
                        errmsg = errmsg + "- Cannot <%=action%> the selected grid format, one or more locations are using it.\r";
                        <%end if %>
                        <% end if %>
                        if (bcontinue)
                            document.form1.submit();
                        else
                            alert(errmsg);
                    }
                }
                //-->
            </script>

            <script language="javascript">
                // Toggles display of naming preference
                function toggleCellNaming(namVal) {
                    if (namVal == 1) {
                        AlterCSS('.namingSeq', 'visibility', 'visible');
                        AlterCSS('.namingRowCol', 'visibility', 'hidden');
                    } else {
                        AlterCSS('.namingSeq', 'visibility', 'hidden');
                        AlterCSS('.namingRowCol', 'visibility', 'visible');
                    }
                }
            </script>

        </head>

        <body>
            <center>
                <form name="form1" action="ManageGridFormat_action.asp?action=<%=action%>" method="POST">
                    <input type="hidden" name="p_grid_format_id" value="<%=grid_format_id%>">
                    <input type="hidden" name="p_grid_format_type" value="<%=grid_format_type%>">
                    <input type="hidden" name="GridType" value="<%=GridType%>">

                    <table border="0">
                        <%if action = "delete" then%>
                            <tr>
                                <td colspan="2">
                                    <span class="GuiFeedback">Are you sure you want to delete this <%=GridText%>:</span><br><br>
                                </td>
                            </tr>
                            <tr>
                                <td align="right">
                                    <span class="required"><%=GridText%> Name:</span>
                                </td>
                                <td>
                                    <input TYPE="text" SIZE="25" Maxlength="50" NAME="p_Name" value="<%=Name%>" disabled>
                                </td>
                            </tr>

                            <%else%>
                                <tr>
                                    <td colspan="2">
                                        <span class="GuiFeedback"><%=GridText%> Attributes:</span><br><br>
                                    </td>
                                </tr>
                                <tr>
                                    <td align="right">
                                        <span class="required"><%=GridText%> Name:</span>
                                    </td>
                                    <td>
                                        <input TYPE="text" SIZE="25" Maxlength="50" NAME="p_Name" value="<%=Name%>">
                                    </td>
                                </tr>
                                <tr>
                                    <td align="right" nowrap>
                                        <span class="required">Row Count:<span>
								</td>
								<td>
									<input TYPE="text" SIZE="10" Maxlength="3" NAME="p_row_count" value="<%=Row_Count%>">
								</td>
							</tr>
							<tr>
								<td align="right" nowrap>
									<span class="required">Column Count:<span>
								</td>
								<td>
									<input TYPE="text" SIZE="10" Maxlength="3" NAME="p_col_count" value="<%=Col_Count%>">
								</td>
							</tr>
							<tr>
								<td align="right">
									<span>Description:</span>
                                    </td>
                                    <td>
                                        <input TYPE="text" SIZE="40" Maxlength="50" NAME="p_description" value="<%=Description%>">
                                    </td>
                                </tr>
                                <tr>
                                    <td align="right" valign="top" nowrap>
                                        <span>Cell Naming:<span>
		</td>
		<td>
			<input TYPE="radio" SIZE="3" Maxlength="3" NAME="p_cell_naming" value="0" <%if cell_naming = "0" then Response.Write(" checked") %> onclick="toggleCellNaming(0)">Row name and Column name&nbsp;(i.e. Row1Col1, Row1Col2)<br />
			<input TYPE="radio" SIZE="3" Maxlength="3" NAME="p_cell_naming" value="1" <%if cell_naming = "1" then Response.Write(" checked") %> onclick="toggleCellNaming(1)">Row to Column Sequence&nbsp;(i.e. [Row Prefix]1, [Row Prefix]2...[Row Prefix][n+1])
		</td>
	</tr>

	<tr id="rowPrefixNamingSeq" class="namingSeq">
		<td align="right" nowrap>
			<span>Row Prefix:<span>
		</td>
		<td>
			<input TYPE="text" SIZE="3" Maxlength="3" NAME="p_row_prefix_seq" value="<%=Row_Prefix%>">
		</td>
	</tr>	
	
	<tr>
		<td align="right">
			<span>Naming Delimiter:</span>
                                    </td>
                                    <td>
                                        <input type="text" size="3" maxlength="50" name="p_name_delimeter" value="<%=Name_Delimeter%>">&nbsp;(i.e. [Parent
                                        <%=GridText%> Name]<strong>/</strong>[Child
                                            <%=GridText%> Names]
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
			<span>Fill Order:<span>
		</td>
		<td>
			<select name="p_number_direction">
				<option value="17">Rows First
				<option value="18">Columns First
			</select>
		</td>
	</tr>

	<tr id="rowCellNameSeparator" class="namingRowCol">
		<td align="right" nowrap>
			<span>Row/Cell Name Separator:<span>
		</td>
		<td>
			<input TYPE="text" SIZE="1" Maxlength="1" NAME="p_name_separator" value="<%=name_separator%>">
		</td>
	</tr>	

	<tr id="rowPrefix" class="namingRowCol">
		<td align="right" nowrap>
			<span>Row Prefix:<span>
		</td>
		<td>
			<input TYPE="text" SIZE="3" Maxlength="3" NAME="p_row_prefix" value="<%=Row_Prefix%>">
		</td>
	</tr>
	<tr id="columnPrefix" class="namingRowCol">
		<td align="right" nowrap>
			<span>Column Prefix:<span>
		</td>
		<td>
			<input TYPE="text" SIZE="3" Maxlength="3" NAME="p_col_prefix" value="<%=col_Prefix%>">
		</td>
	</tr>
	<tr id="rowLabels" class="namingRowCol">
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
	<tr id="columnLabels" class="namingRowCol">
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
<%
    if false then
    ' CSBR-59043: "Containers should be filled according to the fill order of the grid format"
    ' The fix for this bug involves making changes to the PLATESETTINGS.CreateGridFormat
    ' and PLATESETTINGS.UpdateGridFormat functions.  These are not trivial changes, so
    ' for 10.1 this field has been disabled to avoid user confusion.  Once the CSBR has
    ' been fixed, this <select> field can be re-enabled, and the hidden <input> field
    ' removed.
%>
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
<%
    else
    ' Hard-code to the default of number_start_corner (13, or Upper Left)
%>
    <tr>
		<td colspan="2">
			<input type=hidden name="p_number_start_corner" value="<% =number_start_corner %>">
		</td>
    </tr>    
<%    
    end if
%>
	
<%end if%>
	<tr>
		<td colspan="2" align="right"> 
			<a href="#" onclick="history.back(); return false;"><img SRC="/cheminv/graphics/sq_btn/cancel_dialog_btn.gif" border="0"></a>&nbsp;<a href="#" onclick="Validate(); return false;"><img SRC="/cheminv/graphics/sq_btn/ok_dialog_btn.gif" border="0"></a>
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