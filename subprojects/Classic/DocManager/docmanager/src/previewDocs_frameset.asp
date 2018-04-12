<%Response.Expires = -1
if Not Session("UserValidated" & dbkey) = 1 then
	'response.redirect "../../login.asp?dbname=" & request("dbname") & "&formgroup=base_form_group&perform_validate=0"
end if
%>
<HTML>
<HEAD>
<TITLE>Document submission</TITLE>

</HEAD>

<frameset rows="520,*" border="0">
	<frame name="upper" src="previewDocs_upper.asp?<%=Request.QueryString%>">
	<!--frame name="lower" src="previewDocs_lower.asp"-->
	<frame name="lower" src="<%=Replace(Session("htmlFullPath"), " ", "%20")%>">
</frameset>
</HTML>
			
