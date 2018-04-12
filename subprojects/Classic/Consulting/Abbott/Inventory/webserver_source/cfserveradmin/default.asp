<%@ LANGUAGE=VBScript%>
<%'Copyright 1998, CambridgeSoft Corp., All Rights Reserved%>

<% appname = request.querystring("appname")
task = request.querystring("task")
if Not appname <> "" then
response.redirect "/AdminSource/login.asp" 
else
response.redirect "/cfserveradsi/AdminADSI.asp?appname=" & appname & "&task=" & task
end if%>
<html>

<head>
<title></title>
</head>

<body>
</body>
</html>
