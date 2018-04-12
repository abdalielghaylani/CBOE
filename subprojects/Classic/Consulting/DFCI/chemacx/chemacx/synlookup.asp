<%@ LANGUAGE="VBScript"%>
<!--#INCLUDE VIRTUAL = "/chemacx/api/apiutils.asp"-->
<%dbkey = request("dbkey")%>
<!--#INCLUDE VIRTUAL = "/chemacx/chemacx/getSynRS.asp"-->
<%
if NOT (SynRS.EOF AND SynRS.BOF) then
	While Not SynRS.EOF
	synList = synList & SynRS("Name") & "<BR>"
	SynRS.MoveNext
	Wend
else
	synList = "No names found"
end if
%>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN">
<html>
<head>
	<title>Record #<%=Request.QueryString("recordNum") %></title>
	<SCRIPT>focus();</SCRIPT>	
</head>

<body bgcolor="#FFFFFF">
<%
response.write "<table border=0 bgcolor=#FFFFD6><tr><th align=left>Synonyms:</th></tr><tr><td>" & synList & "</td></tr></table>"
%>
</body>
</html>




