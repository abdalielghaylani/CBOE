<%@ LANGUAGE=VBScript %>
<%' Copyright 1999-2003 CambridgeSoft Corporation. All rights reserved%>
<%
'DGB Redirection to Login screen is now managed by Session on Start
'if Application("LoginRequired" & dbkey) =1 then
'	if Not Session("UserValidated" & dbkey) = 1 then  response.redirect "/" & Application("Appkey") & "/logged_out.asp"
'end if
dbkey = request("dbname")
formgroup = request("formgroup")
Response.ExpiresAbsolute = Now()
%>
<html>
<head>
	<script language="JavaScript" src= "/cfserverasp/source/chemdraw.js"></script>

	<script language="JavaScript"> 
	if (window.doPluginDetect != null) {
		cd_includeWrapperFile("/cfserverasp/source/", doPluginDetect)
	}
	else {
		cd_includeWrapperFile("/cfserverasp/source/")
	}
	</script>
<script LANGUAGE="javascript" src="/biosar_browser/source/Choosecss.js"></script>

<title>Right</title>


</head>
<body <%=body_default%>>
<table  width="400" >
	<tr>
		<%Response.Write Session("RIGHT" & "DETAIL" &  dbkey & formgroup)%>
	</tr>
</table>	
</body>
