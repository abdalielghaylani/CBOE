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
				<td height="19" valign="left">
					<%=Application("COWSVersion") %>
				</td>
			</tr>
			<tr>
				<td valign="top" align="left" height="38" rowspan="2">
					<font face="Arial"><strong>Application Info: </strong> </font>
				</td>
				<td valign="middle" align="left" height="19">
					<font face="Arial"><%=Application("AboutWindow" & dbkey)%></font>
				</td>
			</tr>
			<tr>
				<td valign="middle" align="left" height="19">
					<font face="Arial">
						<%=Application("DBRecordCount" & dbkey)%>
						<% if Application("DBType") = "RXN" then 
							Response.Write " reactions." 
						else 
							Response.Write " containers."
						end if%>
					</font>
				</td>
			</tr>
		</table>
    </td>
  </tr>
</table>
</body>
</html>
