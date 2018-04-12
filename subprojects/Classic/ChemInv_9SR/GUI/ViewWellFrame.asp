<%@ Language=VBScript %>
<%
Session("viewWellPlateFilter") = Request("filter")
%>
<html>
<head>
<script language="JavaScript" src="/cheminv/utils.js"></script>
</head>
<body>
<script language="JavaScript">
 OpenDialog('ViewWell.asp?wellID=<%=Request("WellID")%>&refresh=true', 'Diag', 2);
</script>
</body>
</html>