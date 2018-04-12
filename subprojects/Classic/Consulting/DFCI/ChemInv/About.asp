<!--#INCLUDE VIRTUAL = "/cheminv/api/apiUtils.asp"-->
<% 'Copyright 1998 - 2000, CambridgeSoft Corp., All Rights Reserved
dbkey = Request.QueryString("dbname")

Dim Conn
Dim rs

GetInvConnection()
SQL = "SELECT value FROM " & Application("OraSchemaName") & ".globals WHERE id = 'VERSION_SCHEMA'"
SET rs = Conn.Execute(SQL)
schemaVersion = rs("value")
%>


<html>

<head>
<title>About <%=Application("appTitle")%></title>
<script type="text/javascript" language="javascript" src="/cheminv/Choosecss.js"></script>
</head>

<body>
<br />
<br />
<br />
<br />
<br />
<center>
		<table border="1" bgcolor="e1e1e1" cellspacing="0" cellpadding="3" height="112" width="440">
			<tr>
				<td colspan="2" height="55" width="393" valign="middle" align="left"><p align="left"><!--#INCLUDE VIRTUAL = "/cfserverasp/source/server_about_header.htm" --></p>
					<hr>
				</td>
			</tr>
			<tr>
				<td valign="middle" align="left" width="250">
					<p align="left"><strong>ChemOffice WebServer Version: </strong>
				</td>
				<td height="19" valign="left">
					<%=Application("COWSVersion") %>
				</td>
			</tr>
			<tr>
				<td valign="top" align="left" rowspan="2">
					<strong>Application Info: </strong>
				</td>
				<td valign="middle" align="left">
					<%=Application("AboutWindow" & dbkey)%>
				</td>
			</tr>
			<tr>
				<td valign="middle" align="left">
                    Schema v<%=schemaVersion%>                    				
				</td>
			</tr>
		</table>
</center>		
</body>
</html>
