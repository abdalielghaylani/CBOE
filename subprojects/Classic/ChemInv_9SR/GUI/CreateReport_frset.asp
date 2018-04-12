<%
Qstr = Request.QueryString

%>
<html>
<head>
<title><%=Application("appTitle")%> -- Reports</title>
<SCRIPT LANGUAGE=javascript>
<!--
window.focus()
//-->
</SCRIPT>

</head>

<frameset rows="150,*">
		<frame name="ActionFrame" src="CreateReport_action.asp?<%=Qstr%>" MARGINWIDTH="1" MARGINHEIGHT="1" SCROLLING="yes">
		<frame name="DisplayFrame" src="about:blank" MARGINWIDTH="1" MARGINHEIGHT="1" SCROLLING="auto">
</frameset>

</html>