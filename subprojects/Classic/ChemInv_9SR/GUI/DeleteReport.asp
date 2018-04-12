<%@ Language=VBScript %>
<!--#INCLUDE VIRTUAL = "/cheminv/gui/guiUtils.asp"-->
<!--#INCLUDE VIRTUAL = "/cheminv/api/apiUtils.asp"-->
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/display_func_vbs.asp"-->
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/utility_func_vbs.asp"-->


<%
'Prevent page from being cached
Response.ExpiresAbsolute = Now()

ReportDisplayName = Request("ReportDisplayName")
Report_ID = Request("Report_ID")
%>
<html>
<head>
<title><%=Application("appTitle")%> -- Delete a Report Layout</title>
<script LANGUAGE="javascript" src="/cheminv/Choosecss.js"></script>
<script language="JavaScript">
<!--Hide JavaScript
	window.focus();
//-->

</script>
</head>
<body>
<center>
<form name="form1" action="DeleteReport_action.asp" method="POST">
<input type="hidden" value="<%=Report_ID%>" name="Report_ID">	
<table border="0">
	<tr>
		<td colspan="2">
			<span class="GuiFeedback"><br><br><center>Are you sure you want to delete this report layout?</center></span><br><br>
		</td>
	</tr>
	<tr>
		<td align="right" nowrap>Report Layout to delete:</td>
		<td>
			<!--<input TYPE="text" SIZE="60" Maxlength="50" onfocus="blur()" VALUE="<%=ReportDisplayName%>" disabled id="text1" name="text1">-->
			<input TYPE="text" SIZE="40" STYLE="background-color:#d3d3d3;" NAME="ReportDisplayName" VALUE="<%=ReportDisplayName%>" READONLY>
		</td>
	<tr>
		<td colspan="2" align="right"> 
			<a HREF="#" onclick="history.go(-1);return false;"><img SRC="../graphics/cancel_dialog_btn.gif" border="0" WIDTH="61" HEIGHT="21"></a><a HREF="#" onclick="submit(); return false;"><img SRC="../graphics/ok_dialog_btn.gif" border="0" WIDTH="61" HEIGHT="21"></a>
		</td>
	</tr>	
</table>
</form>
</center>
</body>
</html>
