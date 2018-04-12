<%@ Language=VBScript %>
<%
WellList = Request("WellList")

%>
<html>
<head>
</head>

<frameset rows="*,1">
	<frame name="ViewFrame" src="ExportToFileView.asp?WellList=<%=WellList%>" MARGINWIDTH="1" MARGINHEIGHT="1" SCROLLING="auto">
	<frame name="DownloadFrame" src="ExportToFile.asp?WellList=<%=WellList%>" MARGINWIDTH="1" MARGINHEIGHT="1" SCROLLING="auto">
</frameset>  

</html>