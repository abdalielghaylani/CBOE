<%@ LANGUAGE="VBScript" %>
<%  
session.abandon    
response.redirect "/cs_security/login.asp?ClearCookies=true"
%>
