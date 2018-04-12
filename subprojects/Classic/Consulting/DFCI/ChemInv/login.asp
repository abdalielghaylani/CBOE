<%@ Language=VBScript %>
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/ado.inc" -->
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/cs_security/cs_security_utils_vbs.asp"-->
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/cs_security/cs_security_login_utils_vbs.asp"-->
<%
Response.Expires=0

dbkey= Application("appkey")
PrivTableList = Application("PrivTableList")
StartupLocation = Application("StartupLocation")
bAllowCookieLogin = Application("AllowCookieLogin")

perform_validate = Request.QueryString("perform_validate")
forceManualLogin = CBool(Request("forceManualLogin"))
if  forceManualLogin then bAllowCookieLogin = false
CS_SEC_UserName = Request.Cookies("CS_SEC_UserName") 
CS_SEC_UserID = CryptVBS(Request.Cookies("CS_SEC_UserID").Item, Request.Cookies("CS_SEC_UserName").Item)

if (Len(CS_SEC_UserName)>0 AND bAllowCookieLogin) then
	perform_validate = 1
End if

if  perform_validate = 1 then
	if  (Len(CS_SEC_UserName)>0 AND bAllowCookieLogin) then
		Session("UserName" & dbkey)= CS_SEC_UserName
		Session("UserID" & dbkey)= CS_SEC_UserID
	Else
		Name = Trim(Request.Form("user_name"))
		ID = Trim(Request.Form("user_id"))
		Session("UserName" & dbkey)= Name
		Session("UserID" & dbkey)= ID
	End if
	isValid = 0
	isValid = DoUserValidate(dbkey, PrivTableList)
	Session("UserValidated" & dbkey) =isValid
	if isValid = 1 then
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
		Response.Redirect "login.asp?performvalidate=0&forceManualLogin=1"
	End if
else
	Session("UserName" & dbkey)=""
	Session("UserID" & dbkey)= ""
	Session.Abandon
	if CBool(Application("UseCSSecurityApp")) = true then 
		response.Redirect "/cs_security/login.asp?clearcookies=true&forceManualLogin=1"
	end if
%>
<html>
<head>
<script language="javascript">
	if (opener){opener.top.location.href = window.location.href; opener.focus(); window.close()}
	if(parent.location.href != window.location.href) parent.location.href = window.location.href;
</script>
</head>
<body bgcolor="#FFFFFF" onload="document.UserLogin.user_name.focus()">
<table border="0" cellspacing="0" cellpadding="0" width="600">
	<tr>
		<td valign="top">
			<img src="<%=Application("NavButtonGifPath")%>cheminventory_banner.gif" border="0">
		</td>
		<td align="right" valign="top">
			&nbsp;
		</td>
	</tr>
</table>
<form name="UserLogin" action="/<%=Application("AppKey")%>/login.asp?dbname=cheminv&amp;formgroup=base_form_group&amp;perform_validate=1&forceManualLogin=1" method="post" )>
<div align="center">
<table border="0" cellpadding="2" cellspacing="0" bordercolor="White" width="400">
	<%
	if Session("LoginErrorMessage" & dbkey) <> "" then
		response.write "<tr><td colspan=""2""><FONT FACE=""Arial"" SIZE=""3"" COLOR=""Red"">" & Session("LoginErrorMessage" & dbkey)& "</FONT></td></tr>"
	else
		Response.Write "<tr><td colspan=""2"">&nbsp;</td></tr>"
	end if
	%>
	<tr>
		<td colspan="2" align="center">
			&nbsp;
		</td>
	</tr>
	<tr>
		<td colspan="2" align="left">
			<font size="3" face="Arial" color="Navy" style="bold">Please enter your Username and Password</font>
		</td>
	</tr>
	<tr>
		<td align="right">
			<font size="2" face="Arial"><b>Username:&nbsp;</b></font>
		</td>
		<td>
			<input type="Text" name="user_name" size="22">
		</td>
	</tr>
	<tr>
		<td align="right">
			<font size="2" face="Arial"><b>Password:&nbsp;</b></font>
		</td>
		<td>
			<input type="password" name="user_id" size="22">
		</td>
	</tr>
	<tr>
		<td colspan="2" align="left">
			<div align="center">
			<table border="0" width="47%">
				<tr>
					<td colspan="2" align="right">
						<input Type="Image" src="graphics/login_btn.gif" border="0">
						<%
						If Len(Request.Cookies("CS_SEC_UserName")) > 0 then
							href =  "/cs_security/home.asp"
						Else
							href =  "/chemoffice.asp"
						End if
						%>
						<a href="<%=href%>">
						<img SRC="graphics/exit_btn.gif" border="0"></a>
					</td>
				</tr>
			</table>
			</div>
		</td>
	</tr>
</table>
</div>
</form>
<%end if%>
</body>
</html>

