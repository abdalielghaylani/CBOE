<%Response.Expires = -1
if Not Session("UserValidated" & dbkey) = 1 then
	'response.redirect "../../login.asp?dbname=" & request("dbname") & "&formgroup=base_form_group&perform_validate=0"
end if
%>
<HTML>
<HEAD>
<TITLE>Document submission</TITLE>

</HEAD>

<BODY background="<%=Application("UserWindowBackground")%>">
<table width="660" border="0" cellpadding="0" cellspacing="0">
	<tr><td width="80"></td>
		<td width="560">
			<%if Session("previewFeedback") <> "" then%>
				<table width="560">
					<tr height="20"><td><font><b>Document preview:</b></font></td></tr>
				</table>
			
				<table border="1" bordercolor="#000099" background="/docmanager/docmanager/gifs/bk.jpg">
					<tr><td><%=Session("previewFeedback")%>
						</td>
					</tr>
				</table>
			<%end if%>
		</td>
	</tr>
</table>
</BODY>
</HTML>
	
