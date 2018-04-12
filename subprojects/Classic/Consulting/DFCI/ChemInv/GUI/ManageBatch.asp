<%@ Language=VBScript %>
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/ado.inc"-->
<!--#INCLUDE VIRTUAL = "/cheminv/gui/guiUtils.asp"-->
<!--#INCLUDE VIRTUAL = "/cheminv/api/apiUtils.asp"-->
<%
Dim Conn
Dim Cmd
Dim RS

action = Request("action")
p_BatchID = Request("BatchID")
p_MinThreshold = Request("MinThreshold")
p_UOMAbbrv = Request("UOMAbbrv")
p_BatchAmountValue = Request("BatchAmountValue")
if p_BatchAmountValue = "" then p_BatchAmountValue = 0

'-- Show Batch Details
Call GetInvCommand("{CALL " & Application("CHEMINV_USERNAME") & ".BATCH.GETBATCHDETAILS(?)}", adCmdText)
Cmd.Parameters.Append Cmd.CreateParameter("P_BATCHID",131, 1, 0, p_BatchID)
Cmd.Properties ("PLSQLRSet") = true
Set RS = Cmd.Execute

if (RS.EOF and RS.BOF) then
	Response.Write ("<center><table><tr><td align=center colspan=6><span class=""GUIFeedback"">No batch found.</Span></TD></tr></table></center><BR><BR>")
else
	p_MinThreshold = RS("MINIMUM_STOCK_THRESHOLD")
	batchStatusId = RS("batch_status_id_fk")
	Comments = RS("Comments")
	Field_1 = RS("Field_1")
	Field_2 = RS("Field_2")
	Field_3 = RS("Field_3")
	Field_4 = RS("Field_4")
	Field_5 = RS("Field_5")
	Date_1 = RS("Date_1")
	Date_2 = RS("Date_2")
end if

%>
<html>
<head>
<title>Manage Batch</title>
<script type="text/javascript" language="javascript" src="/cheminv/choosecss.js"></script>
<script type="text/javascript" language="javascript" src="/cheminv/gui/validation.js"></script>
<script type="text/javascript" language="javascript" src="/cheminv/utils.js"></script>

<script type="text/javascript" language="JavaScript">
<!--
	var currGridFormat = "<%=currGridFormat%>";
	window.focus();

	function Validate(){

		var bWriteError = false;
		var errmsg = "Please fix the following problems:\r";
		var BatchAmountValue = <%=p_BatchAmountValue%>;
	<%if action <> "delete" then%>

		// Validate Threshold value if one is entered
		if (document.form1.MinThreshold.value.length > 0) {
			if (!isPositiveNumber(document.form1.MinThreshold.value)){
				errmsg = errmsg + "- Minimum stock threshold must be a positive number greater than zero.\r";
				bWriteError = true;
			} else {
				/*
				Removed based on feedback from Abbott
				if (document.form1.MinThreshold.value > BatchAmountValue) {
					errmsg = errmsg + "- Minimum stock threshold must be less than the amount of sample on hand.\r";
					bWriteError = true;
				}
				*/
			}
		}

		// Add new comments to old comments
		if (document.form1.Comments.value.length > 0) {
			var updateComments = "<%=Now() & " : " & Session("UserID" & "cheminv") %>" + "\r";
			updateComments = updateComments + "----------------------------------------------" + "\r";
			updateComments = updateComments + document.form1.Comments.value;
			document.form1.NewComments.value = updateComments + "\r\r\r" + document.form1.OrigComments.value;
		}else{
			document.form1.NewComments.value = document.form1.OrigComments.value;
		}
		// Build JS validation for custom Batch Property fields, if exists
		<%For each Key in req_custom_batch_property_fields_dict%>
		if (document.form1.<%=Key%>.value.length == 0){
			errmsg = errmsg + "- <%=req_custom_batch_property_fields_dict.item(Key)%> is a required field.\r";
			bWriteError = true;
		}
		<% next %>

	<%end if%>
		//bWriteError = true;
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
<form name="form1" action="ManageBatch_action.asp?action=<%=action%>" method="post">
<input type="hidden" name="BatchID" value="<%=p_BatchID%>" />
<input type="hidden" name="NewComments" value />

<table border="0">
	<tr>
		<td colspan="2" align="center">
			<span class="GuiFeedback">Batch Details</span><br><br>
		</td>
	</tr>
	<tr>
		<td valign="top" align="right">Batch Status:</td>
		<td>
		<%Response.Write(ShowSelectBox2("batchStatusId",batchStatusId,"SELECT batch_status_id AS Value, batch_status_name AS DisplayText FROM " & Application("CHEMINV_USERNAME") & ".inv_batch_status ORDER BY lower(DisplayText) ASC", null, " ", ""))%>
		</td>
	</tr>
	<tr>
		<td align="right">
			Minimum Stock Threshold (<%=p_UOMAbbrv%>):
		</td>
		<td>
			<input type="text" size="5" maxlength="10" name="MinThreshold" value="<%=p_MinThreshold%>">
		</td>
	</tr>
	<tr>
		<td align="right">
			Existing Comments:</span>
		</td>
		<td>
			<textarea name="OrigComments" rows="10" cols="60" class="readOnly" readonly><%=Comments%></textarea>
		</td>
	</tr>
	<tr>
		<td align="right">
			New Comments:</span>
		</td>
		<td>
			<textarea name="Comments" rows="10" cols="60"></textarea>
		</td>
	</tr>
	<%
	'-- Display customer Request fields: custom_createrequest_fields_dict, req_custom_createrequest_fields_dict
	For each Key in custom_batch_property_fields_dict
		Response.write("<tr>" & vbcrlf)
		Response.write(vbtab & "<td align=""right"" nowrap>")
		For each Key1 in req_custom_batch_property_fields_dict
			if Key1 = Key then
				Response.Write("<span class=""required"">")
			end if
		Next
		Response.Write(custom_batch_property_fields_dict.item(key) & ":</span></td>" & vbcrlf)
		execute("FieldValue = RS(""" & Key & """)")
		Response.write(vbtab & "<td><input type=""text"" size=""30"" maxlength=""50"" name=""" & key & """ value=""" & FieldValue & """></td>" & vbcrlf)
		Response.write("</tr>" & vbcrlf)
	Next
	%>
	<tr>
		<td colspan="2" align="right">
			<a href="#" onclick="window.close(); return false;"><img SRC="/cheminv/graphics/sq_btn/cancel_dialog_btn.gif" border="0"></a>&nbsp;<a href="#" onclick="Validate(); return false;"><img SRC="/cheminv/graphics/sq_btn/ok_dialog_btn.gif" border="0"></a>
		</td>
	</tr>
</table>
</form>
</center>

</body>
</html>
