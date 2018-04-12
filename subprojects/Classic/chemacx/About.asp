<%@ LANGUAGE=VBScript%>
<% 'Copyright 1998 - 2000, CambridgeSoft Corp., All Rights Reserved
dbkey = Request.QueryString("dbname")%>
<html>

<head>
<title>About</title>
</head>

<body>

<table border="0" bgcolor="#EFEFEF" width="500">
  <tr>
    <td>
		<table border="1" cellspacing="3" cellpadding="3" height="112" width="440">
			<tr>
				<td colspan="2" height="55" width="393" valign="middle" align="left"><p align="left"><!--#INCLUDE VIRTUAL = "/cfserverasp/source/server_about_header.htm" --></p>
					<hr>
				</td>
			</tr>
			<tr>
				<td valign="middle" align="left" width="250">
					<p align="left"><font face="Arial"><strong>ChemOffice WebServer Version: </strong></font>
				</td>
				<td align="left">
					<%=Application("COWSVersion") %>
				</td>
			</tr>
			<tr>
				<td valign="top" align="left" width="250">
					<font face="Arial"><strong>Application Info: </strong> </font>
				</td>
				<td align="left">
					<font face="Arial"><%=Application("AboutWindow" & dbkey)%></font>
				</td>
			</tr>
			<tr>
				<td  align="left">
					<font face="Arial"><strong>ChemACX Database: </strong> </font>
				</td>
				<td align="left">
					<font face="Arial">
						Version <%=Application("ACXDBVersion")%>
						<BR>
						<%=Application("DBRecordCount" & dbkey)%> substances
					</font>
				</td>
			</tr>
			<tr>
				<td align="left">
					<font face="Arial"><strong>MSDX Database: </strong> </font>
				</td>
				<td align="left">
					<font face="Arial">Version <%=Application("MSDXDBVersion")%></font>
				</td>
			</tr>
		</table>
    </td>
  </tr>
</table>
</body>
</html>
