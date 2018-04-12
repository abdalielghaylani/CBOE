<%@ Language=VBScript %>
<%'Copyright 1999-2003 CambridgeSoft Corporation. All rights reserved

Response.Expires=0
dbkey=Request("dbname")
if dbkey = "" Then dbkey="reg"
Dim lo_DEBUG
lo_DEBUG=false
%>
<script language="javascript">
<!--
	if(parent.location.href != window.location.href) parent.location.href = window.location.href;
// -->
</script>
<html>
<head>
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/ado.inc" -->
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/server_const_vbs.asp" -->
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/search_func_vbs.asp" -->
<!--#INCLUDE FILE = "custom_functions.asp"-->
<%	

Session("CurrentLocation" & dbkey & formgroup) = ""
formgroup=Request("formgroup")
if formgroup = "" Then formgroup="base_form_group"

BaseTable = GetTableGroupVal(dbkey, formgroup, kBaseTable)
perform_validate = Request.QueryString("perform_validate")


%>
<!--#INCLUDE FILE = "../cs_security/login_security_vbs.asp"-->

<%
'!DGB! 10/24/01 Cookielogin Code
bAllowCookieLogin = false
CS_SEC_UserName = Request.Cookies("CS_SEC_UserName") 
CS_SEC_UserID = Request.Cookies("CS_SEC_UserID")
if (Len(CS_SEC_UserName)>0 AND bAllowCookieLogin) then
	'Bypass the login screen if cookie credentials are found
	perform_validate = 1
End if

if  perform_validate = 1 then
	 'these session variables are looked at in any GetConnection calls
	'!DGB! 10/24/01 Cookielogin Code
	if  (Len(CS_SEC_UserName)>0 AND bAllowCookieLogin) then
		Session("UserName" & dbkey)= CS_SEC_UserName
		Session("UserID" & dbkey)= CS_SEC_UserID
	Else
		Session("UserName" & dbkey)= Trim(Request.Form("user_name"))
		Session("UserID" & dbkey)= Trim(Request.Form("user_id"))
	End if
	'choose the ado_connection group for validation
	appType = UCase(Application("App_Type"))
	basetable = "compound_molecule"
	isValid = 0
	isValid = DoUserValidate(dbkey, formgroup, appType, basetable, Session("UserName" & dbkey))
	Session("UserValidated" & dbkey) =isValid
	
	newlocation ="/" & Application("AppKey")& "/reg/mainpage.asp?dbname=reg&formgroup=base_form_group&timer=" & Timer%>
	<script language="javascript">
	document.location.href = "<%=newlocation%>"
	</script>
<%else
Session("UserName" & dbkey)=""
Session("UserID" & dbkey)= ""
session.abandon%>
</head>
<body <%=default_body%>>
<nobr><img src="/cfserverasp/source/graphics/cnco.gif" > <img src="/<%=Application("appkey")%>/graphics/<%=Application("appkey")%>_frontpage_bnr.gif"> </nobr>
<form name="UserLogin" action="/<%=Application("AppKey")%>/login.asp?dbname=reg&amp;formgroup=base_form_group&amp;perform_validate=1" method="post" )>
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
		<td colspan="2" align="left">
		<font size="5" face="Arial" color="#990000" style="bold"><%=Application("Display_Appkey")& "&nbsp;"%></font><font size="4" face="Arial" color="#990000" style="bold">Chemical Registration Enterprise</font></td>
	</tr>
	<tr>
		<td colspan="2" align="left"><font size="3" face="Arial" color="Navy" style="bold">Please enter your Username and Password</font></td>
	</tr>
	<tr>
		<td align="left"><font size="2" face="Arial"><b>Username</b></font></td><td><input type="Text" name="user_name" size="22"></td></tr>
	<tr>
		<td align="left"><font size="2" face="Arial"><b>Password</b></font></td><td><input type="password" name="user_id" size="22"></td>
	</tr>
	<tr>
		<td colspan="2" align="left">
			<div align="center">
			<table border="0" width="47%">
				<tr>
					<td width="23%"><a href="javascript:document.UserLogin.submit()"><img src="/<%=Application("AppKey")%>/graphics/login_btn.gif" border="0"></a></td>
					<td width="77%"><a href="/chemoffice.asp"><img src="/<%=Application("AppKey")%>/graphics/exit_btn.gif" border="0"></a></td>
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
