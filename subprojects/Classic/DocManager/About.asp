<%@ LANGUAGE=VBScript%>
<% 'Copyright 1998 - 2000, CambridgeSoft Corp., All Rights Reserved
'dbkey = Request.QueryString("dbname")
dbkey = "docmanager"
Set cnn = Server.CreateObject("ADODB.Connection")
with cnn
	.ConnectionString = Application("cnnStr")
end with
cnn.Open

set countRS = Server.CreateObject("ADODB.Recordset")
sql = "Select Count(*) From docmgr.docmgr_documents"

countRS.Open sql, cnn

%>
<html>

<head>
<title>About</title>
</head>

<body>

<table border="1" bgcolor="#EFEFEF" width="537">
	<tr>
		<td width="632">
			<table border="0" cellspacing="3" cellpadding="3" height="112" width="517">
				<tr>
					<td height="55" width="501" valign="middle" align="left"><p align="left"><!--#INCLUDE VIRTUAL = "/cfserverasp/source/server_about_header.htm" --></p>
						<hr>
					</td>
				</tr>
			</table>
			
			<table>
				<tr>
					<td width="300">
						<p align="left"><font face="Arial"><strong>ChemOffice WebServer Version:</strong></font>
					</td>
        
					<td><%=Application("COWSVersion")%>
					</td>
				</tr>
				
				<tr>
					<td>
						<font face="Arial"><strong>Database Info: </strong> </font>
					</td>
            
					<td  valign="middle" align="left" height="19" width="266"><font face="Arial"><%=Application("AboutWindow" & dbkey)%></font>
					</td>
				</tr>
          
				<tr>
					<td nowrap valign="top" align="left" height="38">
					</td>
								
					<td nowrap valign="middle" align="left" height="19"><font face="Arial">Registered Documents: 
						<%if NOT (countRS.BOF and countRS.EOF)then%>
							<%=countRS.Fields(0)%>
						<%end if%>
						</font>
					</td>
				 </tr>
				 
				 
				<tr>
					<td>
						<font face="Arial"><strong>Application Info: </strong> </font>
					</td>
            
					
					<td  valign="middle" align="left" height="19" width="266"><strong>Date Format: </strong><font face="Arial"><%=Application("DATE_FORMAT_DISPLAY")%></font>
					</td>
				 </tr>
				 				 	 
				 
			</table>
		</td>
  </tr>
</table>
</body>
</html>
<%countRS.Close%>
<%Set countRS = nothing%>
<%cnn.Close%>
<%Set cnn = nothing%>