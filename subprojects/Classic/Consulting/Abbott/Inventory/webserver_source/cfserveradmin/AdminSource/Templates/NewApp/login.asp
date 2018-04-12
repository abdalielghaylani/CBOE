<%@ Language=VBScript %>
<%
'Copyright 1999-2003 CambridgeSoft Corporation. All rights reserved
Response.Expires=0
dbkey=Request("dbname")
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
<%	
Session("CurrentLocation" & dbkey & formgroup) = ""
formgroup=Request("formgroup")
if formgroup = "" Then formgroup="base_form_group"
BaseTable = GetTableGroupVal(dbkey, formgroup, kBaseTable)
perform_validate = Request.QueryString("perform_validate")
%>
<!--#INCLUDE FILE = "cs_security/login_security_vbs.asp"-->

<%if  perform_validate = 1 then
	 'these session variables are looked at in any GetConnection calls
	
	Session("UserName" & dbkey)= Trim(Request.Form("user_name"))
	Session("UserID" & dbkey)= Trim(Request.Form("user_id"))
	'choose the ado_connection group for validation
	appType = UCase(Application("App_Type"))
	BaseTable = GetTableGroupVal(dbkey, formgroup, kBaseTable)
	isValid = 0
	
	isValid = DoUserValidate(dbkey, formgroup, appType, basetable, Session("UserName" & dbkey))
	Session("UserValidated" & dbkey) =isValid
	if Application("MainPage" & dbkey) = 1 then
		newlocation ="/" & Application("AppKey")& "/" & dbkey & "/mainpage.asp?dbname=" & dbkey & "&timer=" & Timer
	else
		newlocation ="/" & Application("AppKey")& "/" & "default.asp?formgroup=base_form_group&dbname=" & dbkey & "&timer=" & Timer 
	end if
	%>
	<script language="javascript">
	document.location.href = "<%=newlocation%>"
	</script>
<%else%>
</head>
<body>
<img src="/cfserverasp/source/graphics/cnco.gif" > <img src="/<%=Application("appkey")%>/graphics/login_bnr.gif"> 
<form name="UserLogin" action="/<%=Application("AppKey")%>/login.asp?dbname=<%=dbkey%>&amp;formgroup=base_form_group&amp;perform_validate=1" method="post" )>
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
		<font size="5" face="Arial" color="#990000" style="bold"></font><font size="4" face="Arial" color="#990000" style="bold"></font></td>
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
					<!--<td width="77%"><a href="javascript:window.close()"><img src="/<%=Application("AppKey")%>/graphics/exit_btn.gif" border="0"></a></td>-->
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
