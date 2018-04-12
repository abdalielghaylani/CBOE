<%@ LANGUAGE="VBScript" %>
<%  
	session.abandon
	If Application("UseCSWebUserAccounts") then
		' The user is logged in via CSWebUsers security
		response.write "<SCRIPT language=JavaScript>top.location.href='https://accounts.cambridgesoft.com/login.cfm?ServiceID=" & Application("CSWebUsers_ServiceID") & "&promptsid=true&showheader=false'</SCRIPT>"
	Elseif Len(Request.Cookies("CS_SEC_UserName")) > 0 then
		' The user is logged in via global security
		response.redirect "/cs_security/login.asp?ClearCookies=true"
	Else
		' No login required
		response.redirect "/chemoffice.asp"
	End if
%>
