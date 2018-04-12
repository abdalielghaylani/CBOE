
<%@ LANGUAGE=VBScript  %>
<%response.expires = 0%>
<%'Copyright 1998-2001, CambridgeSoft Corp., All Rights Reserved

dbkey=Request("dbname")


if Not Session("UserValidated" & dbkey) = 1 then
	response.redirect "../login.asp?dbname=" & request("dbname") & "&formgroup=base_form_group&perform_validate=0"
end if


%>


<!DOCTYPE HTML PUBLIC "-//IETF//DTD HTML//EN">
<html>

<head>

<!--#include file="./src/functions.asp"-->

<!--#INCLUDE VIRTUAL="/cfserverasp/source/cows_func_js.asp"-->
<!--#INCLUDE FILE="../source/secure_nav.asp"-->
<!--#INCLUDE FILE="../source/app_js.js"-->
<!--#INCLUDE FILE="../source/app_vbs.asp"-->

<!--#INCLUDE VIRTUAL="/cfserverasp/source/header_vbs.asp"-->
<!--#INCLUDE VIRTUAL="/cfserverasp/source/recordset_vbs.asp"-->
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/recordset_footer_vbs.asp" -->



<title>Document Manager - External Links</title>
</head>


	<frameset rows="50,500,*" border="0">
		<frame name="subnav" src="external_links_nav.asp?<%=Request.QueryString%>">
		<frame name="externallinks" src="external_links.asp?<%=Request.QueryString%>">
		
		<!---frame name="subnav" src="external_links_nav.asp?unique_id=<%=Request("unique_id")%>&dbname=docmanager">
		<frame name="externallinks" src="external_links.asp?unique_id=<%=Request("unique_id")%>&dbname=docmanager"--->
	</frameset>

<body>	
</body>
</html>
	