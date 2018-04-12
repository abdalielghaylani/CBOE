<%@ LANGUAGE="VBScript"%>
<!--#INCLUDE VIRTUAL = "/cheminv/api/apiutils.asp"-->
<%
	Dim Conn
	GetRegConnection()
	SQLQuery = "SELECT identifier FROM alt_ids WHERE identifier_type IN (0,2) AND Reg_Internal_ID=" & Request.QueryString("RegID") & " AND identifier is not null"
	'Response.Write sqlquery
	'Response.end
	set RS = Conn.Execute(SQLQuery)
	set Conn= nothing
if NOT (RS.EOF AND RS.BOF) then
	While Not RS.EOF
	synList = synList & RS("identifier") & "<BR>"
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
	<title><%=Application("appTitle")%> -- Record #<%=Request.QueryString("recordNum") %></title>
	<SCRIPT>focus();</SCRIPT>	
</head>

<body bgcolor="#FFFFFF">
<%
response.write "<center><table border=0 bgcolor=#FFFFFF><tr><th align=left nowrap>ChemReg Synonyms:</th></tr><tr><td nowrap>" & synList & "</td></tr></table></center>"
%>
</body>
</html>






