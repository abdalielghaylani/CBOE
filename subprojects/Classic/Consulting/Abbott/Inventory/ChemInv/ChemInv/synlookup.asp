<%@ LANGUAGE="VBScript"%>
<!--#INCLUDE VIRTUAL = "/cheminv/api/apiUtils.asp"-->
<%
Dim Conn
Dim Cmd
Dim RS
sql = "SELECT Substance_Name FROM inv_synonyms WHERE Compound_ID_FK =" & Request.QueryString("CompoundID")

Call GetInvCommand(sql, 1)
Set RS = Cmd.Execute


if NOT (RS.EOF AND RS.BOF) then
	While Not RS.EOF
	synList = synList & RS("Substance_Name") & "<BR>"
	RS.MoveNext
	Wend
else
	synList = "<span class=""GUIFeedback"">No names found</span>"
end if
%>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN">
<html>
<head>
	<SCRIPT LANGUAGE=javascript src="/cheminv/Choosecss.js"></SCRIPT>
	<title>Record #<%=Request.QueryString("recordNum") %></title>
	<SCRIPT>focus();</SCRIPT>	
</head>

<body bgcolor="#FFFFFF">
<%
response.write "<center><table border=0 bgcolor=#FFFFFF><tr><th align=left nowrap>ChemInv Synonyms:</th></tr><tr><td nowrap>" & synList & "</td></tr></table></center>"
%>
</body>
</html>




