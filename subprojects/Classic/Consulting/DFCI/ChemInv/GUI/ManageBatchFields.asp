<!--#INCLUDE VIRTUAL = "/cheminv/api/apiUtils.asp"-->
<!--#INCLUDE VIRTUAL = "/cheminv/gui/guiUtils.asp"-->
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/ado.inc"-->
<%
Dim Conn
Dim RS
Dim Cmd

'-- get the current batching information
Call GetInvCommand("{CALL " & Application("CHEMINV_USERNAME") & ".Batch.GetBatchFields()}", adCmdText)	
Cmd.Properties ("PLSQLRSet") = TRUE  
Set rs = Cmd.Execute
Cmd.Properties ("PLSQLRSet") = FALSE

while not rs.EOF 
	if rs("sort_order") = "1" then
		field1 = rs("field_name")
		displayName1 = rs("display_name")
	end if
	if rs("sort_order") = "2" then
		field2 = rs("field_name")
		displayName2 = rs("display_name")
	end if
	if rs("sort_order") = "3" then
		field3 = rs("field_name")
		displayName3 = rs("display_name")
	end if
	rs.MoveNext
wend

%>
<html>
<head>
<title><%=Application("appTitle")%> -- Manage Batching Fields</title>
<script LANGUAGE="javascript" src="/cheminv/Choosecss.js"></script>
<script LANGUAGE="javascript" src="/cheminv/utils.js"></script>
<script language="JavaScript">
<!--Hide JavaScript
	var DialogWindow;
	window.focus();
	function Validate(){
		var bWriteError = false;
		var errmsg = "";
		var bField1 = false;
		var bField2 = false;
		var bField3 = false;
		// clearing the form fields 
		if (document.form1.clear.checked)
		{
		  ClearFields();
		}
		var field1 = document.form1.field1.value;
		var field2 = document.form1.field2.value;
		var field3 = document.form1.field3.value;
		
		if (field1.length > 0)
			bField1 = true;
		if (field2.length > 0)
			bField2 = true;
		if (field3.length > 0)
			bField3 = true;
		
		// Checking if some one selected  field2 and/or field3 and field 2 or field1 is empty.
		if ((!bField1) && (bField2 || bField3))
		{
		bWriteError = true
		errmsg += "- Select Batching Field 1.\n"
		} 	
		if ((!bField2) && (bField3))
		{
		bWriteError = true
		errmsg += "- Select Batching Field 2.\n"
		}
		
		// Checking if some one selected a field in dropdown and not enterered display name 	
		if ((bField1) && (document.form1.displayName1.value==""))
		{
		bWriteError = true
		errmsg += "- Enter Diaplay Name1.\n"	
		}
		if ((bField2) && (document.form1.displayName2.value==""))
		{
		bWriteError = true
		errmsg += "- Enter Diaplay Name2.\n"	
		}
		if ((bField3) && (document.form1.displayName3.value==""))
		{
		bWriteError = true
		errmsg += "- Enter Diaplay Name3.\n"	
		}
		bWriteError = false;
		// if field1 is selected make sure it's not the same as field2 or field3
		if (bField1)
		{
			if (bField2) {
				if (field1 == field2) {
					bWriteError = true; 
				}
			}
			if (bField3) {
				if (field1 == field3) {
					bWriteError = true;
				}
			}
		}
		// if field2 is selected make sure it's not the same as field1 or field3
		if (bField2)
		{
			if (bField1) {
				if (field2 == field1) {
					bWriteError = true; 
				}
			}
			if (bField3) {
				if (field2 == field3) {
					bWriteError = true;
				}
			}
		}
		// if field3 is selected make sure it's not the same as field1 or field2
		if (bField3)
		{
			if (bField1) {
				if (field3 == field1) {
					bWriteError = true; 
				}
			}
			if (bField2) {
				if (field3 == field2) {
					bWriteError = true;
				}
			}
		}

		if (bWriteError){
			errmsg += "- Batching Fields must be distinct."						
			
		}
		if(errmsg!="")
		{
		errmsg ="Please fix the following problems:\r" + errmsg;
		alert(errmsg);
		}
		 
		else{
			document.form1.submit();
		}
	}
// This function is created to clear all batch fields 
function ClearFields()
{
 document.form1.field1.value="";
 document.form1.field2.value="";
 document.form1.field3.value="";
 document.form1.displayName1.value=""
 document.form1.displayName2.value=""
 document.form1.displayName3.value=""
}
-->
</script>

	
</head>
<body TOPMARGIN="0" LEFTMARGIN="5" BGCOLOR="#FFFFFF">
<br><br>
<form NAME="form1" METHOD="POST" action="ManageBatchFields_action.asp">
<table border="0" align="center">
	<tr>
		<td colspan="4" align="center">
			<span class="GuiFeedback">
			Please select the database columns that will be used to define a container batch.<br/><br/>
			</span>
		</td>
	</tr>
	<tr>
		<td align="right" valign="top">
			<span class="GuiFeedback">
			WARNING: 
			</span>
		</td>
		<td align="left" colspan="3">
			Changing batch fields will delete all current batch information (including batch requests) and recalculate container batches.  This process may take a few minutes.<br/><br/>
		</td>
	</tr>
	<tr>
		<td align="right" valign="top" width="29%">
			Batching Field 1:
		</td>
		<td width="28%">
			<%=ShowSelectBox2("field1", field1, "SELECT column_name as value, column_name as displayText FROM all_tab_columns WHERE table_name = 'INV_CONTAINERS' 	AND column_name NOT IN ('BATCH_ID_FK','RID','TIMESTAMP','WELL_COLUMN','WELL_NUMBER','WELL_ROW') ORDER BY column_name", 27, RepeatString(43, "&nbsp;"), "")%>
		</td>
		<td align="right" valign="top" width="15%">
			Display Name:
		</td>
		<td width="28%">
			<input type="text" name="displayName1" size="20" value="<%=displayName1%>">
		</td>
	</tr>
	<tr>
		<td align="right" valign="top">
			Batching Field 2:
		</td>
		<td>
			<%=ShowSelectBox2("field2", field2, "SELECT column_name as value, column_name as displayText FROM all_tab_columns WHERE table_name = 'INV_CONTAINERS' 	AND column_name NOT IN ('BATCH_ID_FK','RID','TIMESTAMP','WELL_COLUMN','WELL_NUMBER','WELL_ROW') ORDER BY column_name", 27, RepeatString(43, "&nbsp;"), "")%>
		</td>
		<td align="right" valign="top">
			Display Name:
		</td>
		<td>
			<input type="text" name="displayName2" size="20" value="<%=displayName2%>">
		</td>
	</tr>
	<tr>
		<td align="right" valign="top">
			Batching Field 3:
		</td>
		<td>
			<%=ShowSelectBox2("field3", field3, "SELECT column_name as value, column_name as displayText FROM all_tab_columns WHERE table_name = 'INV_CONTAINERS' 	AND column_name NOT IN ('BATCH_ID_FK','RID','TIMESTAMP','WELL_COLUMN','WELL_NUMBER','WELL_ROW') ORDER BY column_name", 27, RepeatString(43, "&nbsp;"), "")%>
		</td>
		<td align="right" valign="top">
			Display Name:
		</td>
		<td>
			<input type="text" name="displayName3" size="20" value="<%=displayName3%>">
		</td>
	</tr>
	<tr>
        <td></td>
        <td></td>
        <td></td>
        <td align="left" colspan="3"><input type="checkbox" name="clear" value="1" />Clear Batching Fields</td>	
	</tr>
	<tr>
	<td colspan="4" align="right">
			<a HREF="#" onclick="history.go(-1); return false;"><img SRC="/cheminv/graphics/sq_btn/cancel_dialog_btn.gif" border="0"></a>&nbsp;
			<a HREF="#" onclick="Validate(); return false;"><img SRC="/cheminv/graphics/sq_btn/ok_dialog_btn.gif" border="0"></a>
	</td>
	</tr>
</form>
</body>
</html>
