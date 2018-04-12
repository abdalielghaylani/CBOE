<%
Qstr = Request.QueryString

%>
<html>
<head>
<title>BioSar_Reports</title>
<SCRIPT LANGUAGE=javascript>
<!--
window.focus()
//-->
</SCRIPT>

</head>

<frameset rows="240,*">
		<frame name="ActionFrame" src="reports/CreateReport_action.asp?<%=Qstr%>" MARGINWIDTH="1" MARGINHEIGHT="1" SCROLLING="no">
		<frame name="DisplayFrame" src="reports/processing.html" MARGINWIDTH="1" MARGINHEIGHT="1" SCROLLING="auto">
</frameset>

</html>
