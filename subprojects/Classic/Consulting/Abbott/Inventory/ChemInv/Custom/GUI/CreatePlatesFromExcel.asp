<%@ Language=VBScript %>
<!--#INCLUDE VIRTUAL = "/cheminv/gui/guiUtils.asp"-->
<!--#INCLUDE VIRTUAL = "/cheminv/api/apiUtils.asp"-->
<%
Dim Conn
errmsg = Request("errmsg")
%>
<html>
<head>
<title><%=Application("appTitle")%> -- Create Plates From Excel</title>
<script LANGUAGE="javascript" src="/cheminv/Choosecss.js"></script>
<script LANGUAGE="javascript" src="/cheminv/utils.js"></script>
<script LANGUAGE="javascript" src="/cheminv/gui/validation.js"></script>
<script language="JavaScript">
<!--Hide JavaScript
	var DialogWindow;
	window.focus();
	function ValidateFile(){
		var bWriteError = false;
		var errmsg = "Please fix the following problems:\r\r";
		// Destination is required
		if (document.form1.File1.value.length == 0) {
			errmsg = errmsg + "- File path is required.\r";
			bWriteError = true;
		} else {
			var fileName = document.form1.File1.value; 
			if (fileName.indexOf('.xls') < 0) {
				errmsg = errmsg + "- File type does not appear to Excel.\rPlease choose an excel file.\r";
				bWriteError = true;
			}
		}
		
		if (bWriteError){
			alert(errmsg);
		}
		else{
			document.form1.submit();
		}
	}
-->
</script>
</head>
<body>
<center>
<FORM name="form1" method="post" encType="multipart/form-data" action="CreatePlatesFromExcel_action.asp">
<span class="GuiFeedback"><% if errmsg <> "" then %><%=errmsg%><% else %>
Plates will be created from the first Sheet in the excel file.<% end if %>
</span>
<table border="0">
	<tr>
		<td colspan="2">
			<span class="GuiFeedback"><%=Instructions%></span><br><br>
		</td>
	</tr>
	<tr height="25">
		<td align="right" nowrap>
			<span class="required" title="Path of the Excel Spreadsheet">Excel Spreadsheet:</span>
		</td>
		<td>
			<INPUT type="File" name="File1">
		</td>
	</tr>
	<tr>
		<td colspan="2" align="right"> 
			<a HREF="#" onclick="if (DialogWindow) {DialogWindow.close()}; window.close(); return false;"><img SRC="/cheminv/graphics/cancel_dialog_btn.gif" border="0" WIDTH="61" HEIGHT="21"></a><a HREF="#" onclick="ValidateFile(); return false;"><img SRC="/cheminv/graphics/ok_dialog_btn.gif" border="0" WIDTH="61" HEIGHT="21"></a>
		</td>
	</tr>	
</table>	
</form>
</center>
</body>
</html>
