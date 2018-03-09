
<script LANGUAGE="javascript">

if (window.name != null){
self.opener = this;
self.close()
}else{
window.location = 'coemanager/forms/public/contentarea/home2.aspx';
}
</script>
<%@ Language=VBScript %>


<!--#INCLUDE VIRTUAL = "/cfserverasp/source/ado.inc" -->
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/cs_security/cs_security_utils_vbs.asp"-->
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/cs_security/cs_security_login_utils_vbs.asp"-->
<%'Copyright CambridgeSoft Corporation 1998-2001 all rights reserved
Session("MustChangePassword") = false
Response.Expires=0

dbkey= Application("appkey")
PrivTableList = Application("PrivTableList")
StartupLocation = Application("StartupLocation")
bAllowCookieLogin = Application("AllowCookieLogin")
UA = Request.ServerVariables("HTTP_USER_AGENT")
isWin98 = InStr(1,lcase(UA), "windows 98") > 0
isIE = InStr(1, ucase(UA), "MSIE") > 0
'response.write (isWin98 AND isIE)
'response.write "<BR>" & UA
perform_validate = Request.QueryString("perform_validate")
forceManualLogin = CBool(Request("forceManualLogin"))

if  forceManualLogin then bAllowCookieLogin = false
CS_SEC_UserName = Request.Cookies("CS_SEC_UserName") 
CS_SEC_UserID = CryptVBS(Request.Cookies("CS_SEC_UserID").Item, Request.Cookies("CS_SEC_UserName").Item)

' Log Off if requested
If Request("ClearCookies") then
	session.Abandon
	Response.Cookies("CS_SEC_UserName")= ""
	Response.Cookies("CS_SEC_UserName").Path= "/"
	Response.Cookies("CS_SEC_UserID")= ""
	Response.Cookies("CS_SEC_UserID").Path= "/"
	' Log off from all cs_security applications
	Call KillCOWSSessions()
End if
if perform_validate  = 0 then
response.redirect "/cs_security/home.asp"
else
response.redirect "/cs_security/home.asp"
end if
if (Len(CS_SEC_UserName)>0 AND bAllowCookieLogin) then
	perform_validate = 1
End if

if  perform_validate = 1 then
	if  (Len(CS_SEC_UserName)>0 AND bAllowCookieLogin) then
		Session("UserName" & dbkey)= CS_SEC_UserName
		Session("UserID" & dbkey)= CS_SEC_UserID
	Else
		Session("UserName" & dbkey)= Trim(Request.Form("user_name"))
		Session("UserID" & dbkey)= Trim(Request.Form("user_id"))
	End if
	isValid = 0
	isValid = DoUserValidate(dbkey, PrivTableList)
	Session("UserValidated" & dbkey) =isValid
	if isValid = 1 then
		
		' Stamp credential cookies
		'Response.Cookies("CS_SEC_UserName") = Session("UserName" & dbkey)
		'if NOT (isWin98 AND isIE) then Response.Cookies("CS_SEC_UserName").expires = DateAdd("n", Application("CookieExpiresMinutes"), now())
		'Response.Cookies("CS_SEC_UserName").Path= "/"
		ProlongCookie "CS_SEC_UserName", Session("UserName" & dbkey), Application("CookieExpiresMinutes")
		
		'Response.Cookies("CS_SEC_UserID") = CryptVBS(Session("UserID" & dbkey), Session("UserName" & dbkey))
		'if NOT (isWin98 AND isIE) then Response.Cookies("CS_SEC_UserID").expires = DateAdd("n", Application("CookieExpiresMinutes"), now())
		'Response.Cookies("CS_SEC_UserID").Path= "/"
		
		if Session("LDAPUserID" & dbKey) <> "" then
			pwd = Session("LDAPUserID" & dbKey)
		else 
			pwd = Session("UserID" & dbKey)
		end if
		
		
		ProlongCookie "CS_SEC_UserID", CryptVBS(pwd, Session("UserName" & dbkey)), Application("CookieExpiresMinutes")
		
	
		if Session("PostRelay") = 1 then
			if Session("RelayMethod") = "POST" then
				Response.Write "<body onload=""document.form1.submit()""><form name=form1 method=post action=""" & Session("RelayURL")  & """>" & vblf 
				For each key in PostRelay_dict
					Response.Write "<input type=hidden name=""" & Key & """ value=""" & PostRelay_dict.Item(Key) & """>" & vbLf
				Next
				Response.Write "</form></body>"
				Session("PostRelay") = 0
				Response.end
			Else
				Response.Redirect Session("RelayURL")
			End if
		Else
			response.redirect StartupLocation
		End if
	Else
		' Authentication failed
		response.redirect "/coemanager/forms/public/contentarea/home2.aspx"
	End if
else
	Session("UserName" & dbkey)=""
	Session("UserID" & dbkey)= ""
%>

<!-- #include file="variables.asp" -->
<!-- #include file="functions.asp" -->

<%
PAGE_URL = "CambridgeSoft ChemOffice Enterprise"
PAGE_COLOR = color_blue
PAGE_APP = "header_" & color_blue & "_coent.gif"
TOP_NAV = "&nbsp;<a href=""/cfserverasp/help/default.htm"" target=""_blank""><b>Help</b></a>"
%>

<!-- #include file="header.asp" -->

<script language="javascript">
	var isLoginPage = true;
	if (opener){opener.top.location.href = window.location.href; opener.focus(); window.close()}
	if(parent.location.href != window.location.href) parent.location.href = window.location.href;
</script>

<form name="UserLogin" action="login.asp?dbname=cs_security&amp;perform_validate=1" method="post">
<input name="forceManualLogin" value="1" type="hidden">
<div align="center">
<table width="50%"><tr><td>
	<%=renderBoxBegin ("User Login","")%>
		<%
		if Session("LoginErrorMessage" & dbkey) <> "" then
			response.write "<span class=""required""><strong>" & Session("LoginErrorMessage" & dbkey)& "</strong></span><br /><br />"
		else
			Response.Write "&nbsp;"
		end if
		%>
		<table>
			<tr><td colspan="2">Please enter your username and password</td></tr>
			<tr><td>Username:</td><td><input type="text" name="user_name" size="22"></td></tr>
			<tr><td>Password:</td><td><input type="password" name="user_id" size="22"></td></tr>
			<tr><td>&nbsp;</td><td>
				<input type="submit" value="Log In">
				<%
				' Exit button is no longer needed
				'If Len(Request.Cookies("CS_SEC_UserName")) > 0 then
				'	href =  "/cs_security/home.asp"
				'Else
				'	href =  "/chemoffice.asp"
				'End if
				%>
				<!---<a href="<%=href%>">				<img src="graphics/button/btn_exit.gif" border="0"></a>--->
			</td>
			</tr>
		</table>
	<%=renderBoxEnd()%>
</td></tr></table>
</div>

</form>

<!-- #include file="footer.asp" -->
<SCRIPT LANGUAGE=javascript src="/cfserverasp/source/chemdraw.js"></SCRIPT>

<script LANGUAGE="javascript">
document.UserLogin.user_name.focus();
var isPluginInstalled  = false;
if (cd_currentUsing == 2 || cd_currentUsing == 3) {
	isPluginInstalled =	cd_isCDPluginInstalled();		
}
else if (cd_currentUsing == 1) {
	isPluginInstalled =	cd_isCDActiveXInstalled();
}
document.cookie = "isCDP=" + isPluginInstalled + "; path=/"
</script>

<% End If %>


