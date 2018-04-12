<%@ LANGUAGE="VBScript" %>
<% 'Copyright 1999-2003 CambridgeSoft Corporation. All rights reserved%>

<%  dbkey = request("dbname")
	session.abandon
    Session("UserName" & dbkey) = ""
    Session("UserValidated" & dbkey) = 0
    Session("UserID" & dbkey) = ""
    if Application("LoginRequired")=1 then
	response.redirect "/" & Application("AppKey") & "/default.asp?formgroup=base_form_group&dbname=" & dbkey
	else
	response.redirect "/chemoffice.asp"
	end if
%>
<html>

<head>
<meta http-equiv="Content-Type" content="text/html; charset=iso-8859-1">
<meta name="GENERATOR" content="Microsoft FrontPage 3.0">
<title></title>
</head>

<body>
</body>
</html>
