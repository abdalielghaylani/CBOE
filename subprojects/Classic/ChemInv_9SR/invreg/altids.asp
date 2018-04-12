<%@ LANGUAGE="VBScript"%>
<!--#INCLUDE VIRTUAL = "/cheminv/api/apiutils.asp"-->
<%
Dim Conn
Dim Cmd

GetRegConnection()

SQL = "SELECT identifier, identifier_descriptor FROM alt_ids a, identifiers i WHERE a.identifier_type = i.identifier_type and a.identifier_type IN (1,2) AND Reg_Internal_ID=? AND a.identifier is not null"
Set Cmd = GetCommand(Conn, SQL, &H0001)
Cmd.Parameters.Append Cmd.CreateParameter("RegID", 5, 1, 0, Request.QueryString("RegID"))
Set RS = Cmd.Execute

set Conn= nothing

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
response.write "<center><table border=0 bgcolor=#FFFFFF><tr><th align=left nowrap colspan=""2"">Alternate IDs:</th></tr>"
if NOT (RS.EOF AND RS.BOF) then
	While Not RS.EOF
		Response.Write "<TR><TD>" & rs("identifier_descriptor") & "</TD><TD>" & rs("identifier") & "</TD></TR>"
		RS.MoveNext
	Wend
else
	Response.Write "<TR><TD colspan=""2""><span class=""GUIFeedback"">No alt ids found.</span></TD></TR>"
end if

Response.Write "</table></center>"
%>
</body>
</html>






