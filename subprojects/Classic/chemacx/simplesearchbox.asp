<table width="600" cellpadding="1" cellspacing="0" border="0" bgcolor="#0099ff">
<tr>
<td width="100%" valign="top">
<table width="100%" cellpadding="2" cellspacing="0" border="0" bgcolor="#ffffcc">
<tr>
<td align="center" width="100%" valign="top">
<div align="center">
<form name="npSearchForm" onsubmit="ifCAS()" method="POST" target="_top">
Enter a chemical name or CAS Number. Use * for partial names (eg ben*).
<br>
<% 
	'CSBR# 139459
	'Date of Change: 04-Mar-2011
	'Purpose: To append portnumber to the hostname, if a non-standard port number is used.
	Dim portNumber
	Dim serverName
	portNumber = Request.ServerVariables("SERVER_PORT")
	serverName = Request.ServerVariables("SERVER_NAME")   
    if portNumber <> "80" then
	    serverName = serverName & ":" & portNumber 
	end if
	'End of change
if not Session("IsNet") then
	response.write "You are logged into ChemACX Pro (<a href=""login"" onclick=""ACXLogin(); return false;"">Logoff</a>).  Search over 300 catalogs here."
else
	response.write "Search 30 catalogs for free. For professional searching of over 300 catalogs, use <a href=""http://chemfinder.cambridgesoft.com/chemicals/chemacxpro.asp"">ChemACX Pro</a> (<a href=""login"" onclick=""ACXLogin(); return false;"">Login</a>)"
end if
%>
<br>
<input type="text" name="npSearchText" size="60" value="">
<input type="submit" value="Search"><br>
			
<a href="http://<%=serverName%>/chemacx/inputtoggle.asp?formgroup=base_form_group&dbname=chemacx"><B>Substructure Query with Plugin</b></a> |
<a href="http://chemstore.cambridgesoft.com/software/product.cfm?pid=4011" target="_blank"><B>Download Free Plugin</b></a><br>
</form>
<input type="hidden" name="Substance.CAS" value="">
<input type="hidden" name="Synonym.Name" value="">
<% 
if Session("IsNet") then
	response.write "Individual access to ChemACX is complimentary on a limited basis. Access by corporations, academic institutions and government organizations is granted on an enterprise subscription basis. Please contact "
	response.write "<script language=javascript>"
	response.write "<!-- " & vbcrlf
	response.write "var linktext = ""databases @ cambridgesoft.com"";"
	response.write "var email1 = ""databases"";"
	response.write "var email2 = ""cambridgesoft.com"";"
	response.write "document.write(""<a href="" + ""mail"" + ""to:"" + email1 + ""@"" + email2 + "">"" + linktext + ""</a>"")"
	response.write  vbcrlf & "//-->"
	response.write "</script>"
	response.write " for enterprise subscription information.<br>"
end if
%>
</div>
</td>
</tr>
</table>
</td>
</tr>
</table>
