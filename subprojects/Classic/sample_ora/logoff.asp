<SCRIPT LANGUAGE=vbscript RUNAT=Server>
	session.abandon
	    
	If Len(Request.Cookies("CS_SEC_UserName")) > 0 then
		response.redirect "/cs_security/login.asp?ClearCookies=true"
	Else
		response.redirect "/chemoffice.asp"
	End if
</SCRIPT>