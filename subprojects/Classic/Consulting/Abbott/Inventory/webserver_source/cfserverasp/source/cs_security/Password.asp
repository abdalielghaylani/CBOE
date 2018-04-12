<%@ Language=VBScript %>



<!--#INCLUDE VIRTUAL = "/cfserverasp/source/ado.inc"-->
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/display_func_vbs.asp"-->
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/cs_security/cs_security_utils_vbs.asp"-->
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/cs_security/cs_security_utils.js"-->

<%'Copyright 1999-2003 CambridgeSoft Corporation. All rights reserved

Dim Conn
Dim Cmd
Dim RS

if UCase(Application("AUTHENTICATION_MODE")) = "LDAP" then
	LDAP = true
else
	LDAP = false
End if	 

dbKey = Request("dbKey")
PrivTableName = Request("PrivTableName") 
if IsEmpty(PrivTableName) OR PrivTableName = "" then PrivTableName = NULL
UserName = Session("UserName" & dbkey)
Set Conn = GetCS_SecurityConnection(dbKey)

	' Get User info
	Set Cmd = GetCommand(Conn, "{CALL " &  "CS_SECURITY.MANAGE_USERS.GETUSER(?)}", adCmdText)	
	Cmd.Parameters.Append Cmd.CreateParameter("PUSERNAME",200, 1, 30, UserName)
	Cmd.Properties ("PLSQLRSet") = TRUE  
	Set RS = Cmd.Execute
	Cmd.Properties ("PLSQLRSet") = FALSE
		
	UserName = RS("UserName")
	Password = RS("Password")
%>

<html>
<head>
<title>--Change Password</title>
<script LANGUAGE="javascript" src="/cfserverasp/source/cs_security/Choosecss.js"></script>
<script language="JavaScript">
	window.focus();
	function ValidateUser(){
		
		var bWriteError = false;
		var errmsg = "Please fix the following problems:\r";
		
		if (document.user_manager.iNewPassword){
			pw = document.user_manager.iNewPassword.value; 
			if (pw.length == 0) {
				errmsg = errmsg + "- Password is required.\r";
				bWriteError = true;
			}
			else{
				errmsg1 = IsValidPassword(pw);
				if (errmsg1.length > 0){
					errmsg += errmsg1;
					bWriteError = true;
				}
				
				if (pw != document.user_manager.iNewPassword2.value) {
					errmsg = errmsg + "- Password confirmation does not match.\r";
					bWriteError = true;
				}
			}			
				document.user_manager.NewPassword.value = pw;
		}
					
		if (bWriteError){
			alert(errmsg);	
		}
		else{
			document.user_manager.submit();
		}
	}
</script>
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/cs_security/admin_utils_js.js"-->
</head>
<body onload="if (document.user_manager.iNewPassword) {document.user_manager.iNewPassword.focus();}">
<center>
<form name="user_manager" action="Users_action.asp?action=changePWD" method="POST">
<input type="hidden" name="PrivTableName" value="<%=PrivTableName%>">
<input type="hidden" name="isAlreadyInOracle" value="0">
<input type="hidden" name="dbKey" value="<%=dbKey%>">
<%if Session("MustChangePassword") then%>
	<font FACE="Arial" SIZE="3" COLOR="Red">Your current Oracle password has expired</font><p>
<%end if%>
<table border="0">
<%if LDAP then%>
	<tr>
		<td>
			<font FACE="Arial" SIZE="3" COLOR="Red">
				<br><br>  
				Password cannot be changed via CSSecurity because LDAP authentication is enabled on the server.<br>
				Please contact the system Adminstrator for instructions on how to change the LDAP password.
			</font>
		</td>
	<tr>
<%else%>
	<tr>
		<td colspan="2">
			<span class="GuiFeedback">Change your password:</span><br><br>
		</td>
	</tr>	
	<tr>
		<td align="right" nowrap>
			User Name:
		</td>
		<td>
			<input tabIndex="1" Name="UserName" TYPE="tetx" SIZE="20" Maxlength="255" VALUE="<%=UserName%>" onfocus="this.blur();">
		</td>
	</tr>
	<tr>
		<td align="right" nowrap>
			Current Password:
		</td>
		<td>
			<input tabIndex="1" Name="Password" TYPE="password" SIZE="20" Maxlength="255" VALUE="<%=Password%>" onfocus="this.blur();">
		</td>
	</tr>
	<tr>
		<td align="right" nowrap>
			<span class="required">New Password:</span>
		</td>
		<td>
			<input tabIndex="1" Name="iNewPassword" TYPE="password" SIZE="20" Maxlength="255" VALUE>
			<input type="Hidden" name="NewPassword" value>
		</td>
	</tr>
	<tr>
		<td align="right" nowrap>
			<span class="required">Confirm New Password:</span>
		</td>
		<td>
			<input tabIndex="1" Name="iNewPassword2" TYPE="password" SIZE="20" Maxlength="255" VALUE>
		</td>
	</tr>
<%End if 'LDAP%>	
	<tr>
		<td colspan="2" align="right"> 
			<%if Session("MustChangePassword") then %>
				<a class="MenuLink" href="/cs_security/login.asp?ClearCookies=true"><img border="0" SRC="../graphics/logoff.gif"></a>
			<%else%>
				<a HREF="#" onclick="top.window.close(); return false;"><img SRC="../graphics/cancel_dialog_btn.gif" border="0"></a>
			<%end if%>
			<%if NOT LDAP then%>
				<a HREF="#" onclick="ValidateUser(); return false;"><img SRC="../graphics/ok_dialog_btn.gif" border="0"></a>
			<%end if%>
		</td>
	</tr>
</table>


</form>
</center>
</body>
</html>

