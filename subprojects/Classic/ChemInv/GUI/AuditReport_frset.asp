<%
QS = Request.QueryString
ft = Request("ft")
if ft = "" then ft = "std"
if Request("filter") = "true" then
	dispSrc = "AuditReport_" & ft & "_display.asp?" & QS
Else
	dispSrc = "blank.html"
End if	
	
%>
<html>
<head>
<title><%=Application("appTitle")%> -- Audit Report</title>
<SCRIPT LANGUAGE=javascript>
<!--
window.focus()
//-->
</SCRIPT>

</head>

<frameset rows="300,*">
		<frame name="ActionFrame" src="AuditReport_<%=ft%>_filter.asp?<%=QS%>" MARGINWIDTH="1" MARGINHEIGHT="1" SCROLLING="no">
		<frame name="DisplayFrame" src="<%=dispSrc%>" MARGINWIDTH="1" MARGINHEIGHT="1" SCROLLING="auto">
</frameset>

</html>