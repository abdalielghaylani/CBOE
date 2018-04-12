<%@ LANGUAGE=VBScript%>
<% 'Copyright 1998 - 2001, CambridgeSoft Corp., All Rights Reserved
dbkey = Request.QueryString("dbname")%>
<html>

<head>
<title>About</title>
</head>

<body>

<table border="1" bgcolor="#EFEFEF" width="500">
  <tr>
    <td>
		<table border="0" cellspacing="3" cellpadding="3" height="112" width="440">
			<tr>
				<td colspan="2" height="55" width="393" valign="middle" align="left"><p align="left"><!--#INCLUDE VIRTUAL = "/cfserverasp/source/server_about_header.htm" --></p>
				<hr>
				</td>
			</tr>
			<tr>
				<td valign="middle" align="left" width="200"><p align="left">
					<font face="Arial" size="-1"><strong>ChemOffice WebServer Version:</strong></font>
				</td>
				<td height="19" width="240" valign="left">
					<font face="Arial" size="-1"><%=Application("COWSVersion") %></font>
				</td>
			</tr>
			<tr>
				<td width="270" valign="top" align="left" height="38">
					<font face="Arial" size="-1"><strong>Database Info: </strong> </font>
				</td>
				<td width="270" valign="middle" align="left" height="19">
					<font face="Arial" size="-1"><%=Application("AboutWindow" & dbkey)%></font>
				</td>
			</tr>
			<tr>
				<td width="270" valign="top" align="left" height="38" colspan="2">
					<font face="Arial" size="-1">
					<%
					Set c = Server.CreateObject("ADODB.Connection")
					c.open("file name=" & Server.MapPath("/drugdeg/") & "\config\drugdeg.udl;password=" & Application("DRUGDEG_PWD") & ";User id=" & Application("DRUGDEG_USERNAME"))
										
					'response.write "There are currently " & RS("degcount").value & " degradants in the database."
					set RS = c.execute ("select count(*) parcount from DRUGDEG_PARENTS")
					Response.write "<b>Parent Compounds:</b>" &  " " & RS("parcount").value & "<br>"
					set RS = c.execute ("select count(*) expcount from DRUGDEG_EXPTS")
					Response.write "<b>Experiments:</b>" &  " " & RS("expcount").value & "<br>"
					set RS = c.execute ("select count(*) degcount from DRUGDEG_DEGS")
					Response.write "<b>Degradant Compounds:</b>" &  " " & RS("degcount").value & "<br>"
					set RS = c.execute ("select count(*) mechcount from DRUGDEG_MECHS")
					Response.write "<b>Mechanisms:</b>" &  " " & RS("mechcount").value & "<br>"
					%>
					</font>
				</td>

			</tr>
		</table>
    </td>
  </tr>
</table>

</body>
</html>
