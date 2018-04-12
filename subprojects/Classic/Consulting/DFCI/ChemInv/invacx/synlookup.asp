<%@ LANGUAGE="VBScript"%>
<!--#INCLUDE VIRTUAL = "/cheminv/api/apiutils.asp"-->
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/server_const_vbs.asp"-->
<%
	Dim Conn
	GetACXConnection()
	
    If Ucase(Application("base_connection" & "invacx")(kDBMS)) = "ORACLE" then
		SQLQuery = "SELECT DISTINCT ACX_Synonym.Name FROM ACX_Synonym WHERE ACX_Synonym.CsNum =" & Request.QueryString("CsNum")
	Else
		SQLQuery = "SELECT DISTINCT Synonym.Name FROM Synonym WHERE Synonym.CsNum =" & Request.QueryString("CsNum")
	End if
	'Response.Write sqlquery
	'Response.End
	set RS = Conn.Execute(SQLQuery)
	set Conn= nothing
if NOT (RS.EOF AND RS.BOF) then
	While Not RS.EOF
	synList = synList & RS("Name") & "<BR>"
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
response.write "<center><table border=0 bgcolor=#FFFFFF><tr><th align=left nowrap>ChemACX Synonyms:</th></tr><tr><td nowrap>" & synList & "</td></tr></table></center>"
%>
</body>
</html>






