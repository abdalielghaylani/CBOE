<%@ LANGUAGE="VBScript" %>
<% 'Copyright 1998, CambridgeSoft Corp., All Rights Reserved%>

<%    session.abandon
    
	If Len(Request.Cookies("CS_SEC_UserName")) > 0 then
		response.redirect "/cs_security/login.asp?ClearCookies=true"
	Else
		response.redirect "/" & Application("AppKey") & "/login.asp"
	End if
%>
<!DOCTYPE HTML PUBLIC "-//IETF//DTD HTML//EN">
<html>

<head>
<meta name="GENERATOR" content="Microsoft FrontPage 3.0">
<title></title>
</head>

<body>
</body>
</html>
