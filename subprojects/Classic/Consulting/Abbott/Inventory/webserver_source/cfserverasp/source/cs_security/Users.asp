<%@ Language=VBScript %>
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/ado.inc"-->
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/display_func_vbs.asp"-->
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/cs_security/cs_security_utils_vbs.asp"-->
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/cs_security/cs_security_utils.js"-->


<%
'Copyright 1999-2003 CambridgeSoft Corporation. All rights reserved

Dim Conn
Dim Cmd
Dim RS
dbKey = Request("dbKey")
UserName = Request.form("UserName")
if UserName = "" then UserName = Request.QueryString("UserName")
PrivTableName = Request("PrivTableName") 

if UCase(Application("AUTHENTICATION_MODE")) = "LDAP" then
	LDAP = true
	UnameDisbled = "onFocus=""blur()"""	
else
	LDAP = false
end if

if IsEmpty(PrivTableName) OR PrivTableName = "" then PrivTableName = NULL

Set Conn = GetCS_SecurityConnection(dbKey)

isDelete = false
If UserName = "" then
	Caption = "Add new User"
	formAction = "action=create"
	ActiveChecked = "checked"
	isCreate = true
Else
	' Get User info
	Set Cmd = GetCommand(Conn, "{CALL " &  "CS_SECURITY.MANAGE_USERS.GETUSER(?)}", adCmdText)	
	Cmd.Parameters.Append Cmd.CreateParameter("PUSERNAME",200, 1, 30, UserName)
	Cmd.Properties ("PLSQLRSet") = TRUE  
	Set RS = Cmd.Execute
	Cmd.Properties ("PLSQLRSet") = FALSE
		
	UserName = RS("UserName")
	Password = RS("Password")
	FirstName = RS("FirstName")
	MiddleName = RS("MiddleName")
	LastName = RS("LastName")
	Email = RS("Email")
	Telephone = RS("Telephone")
	Address = RS("Address")
	UserCode = RS("UserCode")
	SupervisorID = RS("SupervisorID")
	SiteID = RS("SiteID")
	Active = RS("isActive")
	SupervisorID = RS("SupervisorID")
	
	if Active = "1" then activeChecked = "checked"	
	' Get roles granted to user
	Set Cmd = GetCommand(Conn, "{CALL " &  "CS_SECURITY.MANAGE_ROLES.GETUSERROLES(?,?)}", adCmdText)	
	Cmd.Parameters.Append Cmd.CreateParameter("PUSERNAME",200, 1, 30, UserName)
	Cmd.Parameters.Append Cmd.CreateParameter("PPRIVTABLENAME",200, 1, 30, PrivTableName)
	Cmd.Properties ("PLSQLRSet") = TRUE  
	Set RS = Cmd.Execute
	Cmd.Properties ("PLSQLRSet") = FALSE
	if NOT (RS.BOF and RS.EOF) then
		user_roles_list = RS.GetString(2,,,",","")
		user_roles_list = Left(user_roles_list, Len(user_roles_list)-1)
	Else
		user_roles_list = ""
	End if
	if Lcase(Request("action")) = "delete" then
		Caption = "Are you sure you want to delete this User?"
		formAction = "action=delete"
		isDelete = true
		disabled = " onfocus=""blur()"" "
		checkDisabled = " disabled"
	Else
		Caption = "Update User:"
		formAction = "action=edit"
		disbled = ""
		
		isEdit = true
	End if
	UnameDisbled = "onFocus=""blur()"""
End if

if isCreate then
	' Get all roles
	Set Cmd = GetCommand(Conn, "{CALL CS_SECURITY.MANAGE_ROLES.GETROLES(?)}", adCmdText)	
	Cmd.Parameters.Append Cmd.CreateParameter("PPRIVTABLENAME",200, 1, 100, PrivTableName)
	Cmd.Properties ("PLSQLRSet") = TRUE  
	Set RS = Cmd.Execute
	Cmd.Properties ("PLSQLRSet") = FALSE
	all_roles_string = GetLookupList(RS, "ROLE_NAME", "ROLE_NAME")
Elseif isEdit then
	' Get roles available to user
	Set Cmd = GetCommand(Conn, "{CALL " &  "CS_SECURITY.MANAGE_ROLES.GETAVAILABLEROLES(?,?)}", adCmdText)		
	Cmd.Parameters.Append Cmd.CreateParameter("PUSERNAME",200, 1, 30, UserName)
	Cmd.Parameters.Append Cmd.CreateParameter("PPRIVTABLENAME",200, 1, 30, NULL)
	Cmd.Properties ("PLSQLRSet") = TRUE  
	Set RS = Cmd.Execute
	Cmd.Properties ("PLSQLRSet") = FALSE
	if NOT (RS.BOF and RS.EOF) then
		available_roles_list = RS.GetString(2,,,",","")
		available_roles_list = Left(available_roles_list, Len(available_roles_list)-1)	
	Else
		available_roles_list = ""
	End if
End if
%>

<html>
<head>
<title>--Add/Update/Delete a User</title>
<script LANGUAGE="javascript" src="/cfserverasp/source/cs_security/Choosecss.js"></script>
<script language="JavaScript">
	window.focus();
	w_current_list =  "<%=user_roles_list%>";
	orig_current_list =  w_current_list;
	orig_user_password = "<%=password%>";
	w_avail_list = "<%=UCase(available_roles_list)%>";
	user_name = "<%=UserName%>";
	
	var wDiag;
	
	function lockFields(){
		document.user_manager.UserName.onfocus = function(){this.blur()};
		document.user_manager.iPassword.onfocus = function(){this.blur()};
		document.user_manager.iPassword2.onfocus = function(){this.blur()};
	}
	//MRE added 11/30/04
	//simple email validation
	function IsEmailVaild(sEmail) {
		var at="@"
		var dot="."
		var lat=sEmail.indexOf(at)
		var lsEmail=sEmail.length
		var ldot=sEmail.indexOf(dot)
			
		if (sEmail.indexOf(at)==-1 || sEmail.indexOf(at)==0 || sEmail.indexOf(at)==lsEmail){
			return false
		}

		if (sEmail.indexOf(dot)==-1 || sEmail.indexOf(dot)==0 || sEmail.indexOf(dot)==lsEmail){
			return false
		}
		
		if (sEmail.substring(lat-1,lat)==dot || sEmail.substring(lat+1,lat+2)==dot){
			return false
		}

		if (sEmail.indexOf(dot,(lat+2))==-1){
			return false
		}
		
		if (sEmail.indexOf(" ")!=-1){
			return false
		}
		return true					
	}
	
	function ValidateUser(){
		var bWriteError = false;
		var errmsg = "Please fix the following problems:\r";
		<%if CBool(Application("UserInfoRequiredFieldList" & "USERNAME")) = True then%>
		// UserName is required
		if (document.user_manager.UserName){
			var un = document.user_manager.UserName.value; 
			if (un.length == 0) {
				errmsg += "- User Name is required.\r";
				bWriteError = true;
			}
			else{
				errmsg1 = IsValidOracleObjectName("User Name", un);
				if (errmsg1.length > 0){
					errmsg += errmsg1;
					bWriteError = true;
				}
			}
		}
		<%end if%>
		<%if CBool(Application("UserInfoRequiredFieldList" & "LASTNAME")) = True then%>
		if (document.user_manager.LastName){
			if (document.user_manager.LastName.value.length == 0) {
				errmsg = errmsg + "- Last Name is required.\r";
				bWriteError = true;
			}
		}
		<%end if%>
		<%if CBool(Application("UserInfoRequiredFieldList" & "PASSWORD")) = True then%>
		if ((document.user_manager.iPassword)&& (document.user_manager.isAlreadyInOracle.value != 1)){
			pw = document.user_manager.iPassword.value
			if ((pw != orig_user_password)||(pw.length == 0)){
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
					
					if (pw != document.user_manager.iPassword2.value) {
						errmsg = errmsg + "- Password confirmation does not match.\r";
						bWriteError = true;
					}
				}
			
				document.user_manager.Password.value = document.user_manager.iPassword.value; 	
			}
			else{	
				document.user_manager.Password.value = "";
			}
		}
		<%end if%>
		bVaildEmail = true;
		<%if CBool(Application("UserInfoRequiredFieldList" & "EMAIL")) = True then%>
		if (document.user_manager.Email){
			if (document.user_manager.Email.value.length == 0) {
				errmsg = errmsg + "- Email is required.\r";
				bWriteError = true;
				bVaildEmail = false;
			}
		}
		
		<%end if%>
		if (document.user_manager.Email && bVaildEmail ){
			if (document.user_manager.Email.value.length != 0) {
				bVaildEmail = IsEmailVaild(document.user_manager.Email.value)
				if (!bVaildEmail) {
					errmsg = errmsg + "- Please enter a valid email.\r";
					bWriteError = true;
				}
			}
		}
		
		<%if CBool(Application("UserInfoRequiredFieldList" & "USERCODE")) = True then%>
		if (document.user_manager.UserCode){
			if (document.user_manager.UserCode.value.length == 0) {
				errmsg = errmsg + "- User Code is required.\r";
				bWriteError = true;
			}
		}
		<%end if%>
		
		<%if CBool(Application("UserInfoRequiredFieldList" & "FIRSTNAME")) = True then%>
		if (document.user_manager.FirstName){
			if (document.user_manager.FirstName.value.length == 0) {
				errmsg = errmsg + "- First Name is required.\r";
				bWriteError = true;
			}
		}
		<%end if%>
		
		<%if CBool(Application("UserInfoRequiredFieldList" & "MIDDLENAME")) = True then%>
		if (document.user_manager.MiddleName){
			if (document.user_manager.MiddleName.value.length == 0) {
				errmsg = errmsg + "- Middle Name is required.\r";
				bWriteError = true;
			}
		}
		<%end if%>
		<%if CBool(Application("UserInfoRequiredFieldList" & "TELEPHONE")) = True then%>
		if (document.user_manager.Telephone){
			if (document.user_manager.Telephone.value.length == 0) {
				errmsg = errmsg + "- Telephone is required.\r";
				bWriteError = true;
			}
		}
		<%end if%>
		
		<%if CBool(Application("UserInfoRequiredFieldList" & "ADDRESS")) = True then%>
		if (document.user_manager.Address){
			if (document.user_manager.Address.value.length == 0) {
				errmsg = errmsg + "- Address is required.\r";
				bWriteError = true;
			}
		}
		<%end if%>
		
		<%if CBool(Application("UserInfoRequiredFieldList" & "SUPERVISORID")) = True then%>
		if (document.user_manager.SupervisorID){
			if (document.user_manager.SupervisorID.selectedIndex == -1) {
				errmsg = errmsg + "- At least one supervisor is required.\r";
				bWriteError = true;
			}
		}
		<%end if%>
		
		<%if CBool(Application("UserInfoRequiredFieldList" & "SITEID")) = True then%>
		if (document.user_manager.SiteID){
			if (document.user_manager.SiteID.selectedIndex == -1) {
				errmsg = errmsg + "- At least one site is required.\r";
				bWriteError = true;
			}
		}
		<%end if%>
		
		<%if isEdit then%>
		if (document.user_manager.original_roles_hidden.value != document.user_manager.current_roles_hidden.value) {
			document.user_manager.RolesGranted.value = document.user_manager.current_roles_hidden.value;
			document.user_manager.RolesRevoked.value = document.user_manager.original_roles_hidden.value;
			if (document.user_manager.RolesGranted){
				if (document.user_manager.RolesGranted.selectedIndex == -1) {
					errmsg = errmsg + "- At least one role is required.\r";
					bWriteError = true;
				}
			}		
		}
		else{
			document.user_manager.RolesGranted.value = "";
			document.user_manager.RolesRevoked.value = "";
		}
		<%end if%>
		<%if isCreate then%>	
		if (document.user_manager.RolesGranted){
			if (document.user_manager.RolesGranted.selectedIndex == -1) {
				errmsg = errmsg + "- At least one role is required.\r";
				bWriteError = true;
			}
		}
		<%end if %>			
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
<body onunload="if (wDiag) wDiag.close();" onload="if(document.user_manager.roles) fill_lists(); if (!document.user_manager.UserName.disabled) document.user_manager.UserName.focus()">
<center>
<form name="user_manager" action="Users_action.asp?<%=formAction%>" method="POST">
<input type="hidden" name="PrivTableName" value="<%=PrivTableName%>">
<input type="hidden" name="isAlreadyInOracle" value="0">
<input type="hidden" name="dbKey" value="<%=dbKey%>">
<table border="0">
	<tr>
		<td colspan="2">
			<span class="GuiFeedback"><%=Caption%></span><br><br>
		</td>
	</tr>
	
	<tr>
		<td align="right" nowrap>
		<%if CBool(Application("UserInfoRequiredFieldList" & "USERNAME")) = True then%>
			<span class="required">User Name:</span>
		<%else%>
			User Name:
		<%end if%>
		</td>
		<td>
			<input tabIndex="1" Name="UserName" TYPE="tetx" SIZE="35" Maxlength="30" VALUE="<%=UserName%>" <%=UnameDisbled%>>
			<% if isCreate or isEdit then%>
				<%if LDAP then%>
					<%if isEdit then%>
						<a class="MenuLink" href="#" onclick="wDiag = OpenDialog('SelectLDAPUser.asp?dbKey=<%=dbKey%>&amp;LDAPUserName=<%=UserName%>&amp;isEdit=1', 'LDAPDiag', 2); return false">Refresh from LDAP</a>
					<%else%>
						<a class="MenuLink" href="#" onclick="wDiag = OpenDialog('SelectLDAPUser.asp?dbKey=<%=dbKey%>', 'LDAPDiag', 2); return false">Select LDAP User</a>
					<%end if%>
				<%else%>
					<a class="MenuLink" href="#" onclick="wDiag = OpenDialog('SelectOracleUser.asp?dbKey=<%=dbKey%>', 'OraDiag', 3); return false">Select Oracle User</a>
				<%end if%>
			<%end if%>
		</td>
	</tr>
<%if NOT isDelete then%>
	<%if LDAP then%>
		<input type="Hidden" name="Password" value>
	<%else%>
	<tr>
		<td align="right" nowrap>
			<%if CBool(Application("UserInfoRequiredFieldList" & "PASSWORD")) = True then%>
			<span class="required">Password:</span>
			<%else%>
			Password:
			<%end if%>
		</td>
		<td>
			<input tabIndex="1" Name="iPassword" TYPE="password" SIZE="35" Maxlength="30" VALUE="<%=Password%>" <%=disabled%>>
			<input type="Hidden" name="Password" value>
		</td>
	</tr>
	<tr>
		<td align="right" nowrap>
			<%if CBool(Application("UserInfoRequiredFieldList" & "PASSWORD")) = True then%>
			<span class="required">Confirm Password:</span>
			<%else%>
			Confirm Password:
			<%end if%>
			
		</td>
		<td>
			<input tabIndex="1" Name="iPassword2" TYPE="password" SIZE="35" Maxlength="30" VALUE="<%=Password%>" <%=disabled%>>
		</td>
	</tr>
	<%end if 'LDAP%>
<%end if%>
	<tr>
		<td align="right" nowrap>
			<%if CBool(Application("UserInfoRequiredFieldList" & "USERCODE")) = True then%>
			<span class="required">User Code:</span>
			<%else%>
			User Code:
			<%end if%>
		</td>
		<td>
			<input tabIndex="1" Name="UserCode" TYPE="tetx" SIZE="20" Maxlength="50" VALUE="<%=UserCode%>" <%=disabled%>>
		</td>
	</tr>
	<tr>
		<td align="right" nowrap>
			<%if CBool(Application("UserInfoRequiredFieldList" & "FIRSTNAME")) = True then%>
			<span class="required">First Name:</span>
			<%else%>
			First Name:
			<%end if%>
		</td>
		<td>
			<input tabIndex="1" Name="FirstName" TYPE="tetx" SIZE="20" Maxlength="50" VALUE="<%=FirstName%>" <%=disabled%>>
		</td>
	</tr>
	<tr>
		<td align="right" nowrap>
			<%if CBool(Application("UserInfoRequiredFieldList" & "MIDDLENAME")) = True then%>
			<span class="required">Middle Name:</span>
			<%else%>
			Middle Name:
			<%end if%>
			
		</td>
		<td>
			<input tabIndex="1" Name="MiddleName" TYPE="tetx" SIZE="30" Maxlength="50" VALUE="<%=MiddleName%>" <%=disabled%>>
		</td>
	</tr>
	<tr>
		<td align="right" nowrap>
			<%if CBool(Application("UserInfoRequiredFieldList" & "LASTNAME")) = True then%>
			<span class="required">Last Name:</span>
			<%else%>
			Last Name:
			<%end if%>
		</td>
		<td>
			<input tabIndex="1" Name="LastName" TYPE="tetx" SIZE="30" Maxlength="50" VALUE="<%=LastName%>" <%=disabled%>>
		</td>
	</tr>
	<tr>
		<td align="right" nowrap>
			<%if CBool(Application("UserInfoRequiredFieldList" & "EMAIL")) = True then%>
			<span class="required">Email:</span>
			<%else%>
			Email:
			<%end if%>
		</td>
		<td>
			<input tabIndex="1" Name="Email" TYPE="tetx" SIZE="30" Maxlength="50" VALUE="<%=Email%>" <%=disabled%>>
		</td>
	</tr>
	<tr>
		<td align="right" nowrap>
			<%if CBool(Application("UserInfoRequiredFieldList" & "TELEPHONE")) = True then%>
			<span class="required">Telephone:</span>
			<%else%>
			Telephone:
			<%end if%>
			
		</td>
		<td>
			<input tabIndex="1" Name="Telephone" TYPE="tetx" SIZE="30" Maxlength="50" VALUE="<%=Telephone%>" <%=disabled%>>
		</td>
	</tr>
	<tr>
		<td align="right" nowrap>
			<%if CBool(Application("UserInfoRequiredFieldList" & "ADDRESS")) = True then%>
			<span class="required">Address:</span>
			<%else%>
			Address:
			<%end if%>
		</td>
		<td>
			<input tabIndex="1" Name="Address" TYPE="tetx" SIZE="30" Maxlength="50" VALUE="<%=Address%>" <%=disabled%>>
		</td>
	</tr>
	<tr>
		<td align="right" nowrap>
			<%if CBool(Application("UserInfoRequiredFieldList" & "SUPERVISOR")) = True then%>
			<span class="required">Supervisor:</span>
			<%else%>
			Supervisor:
			<%end if%>
		</td>
		<td>
			<%=ShowSelectBox2(conn, "SupervisorID", SupervisorID, "SELECT Person_ID AS Value, Last_Name||DECODE(First_Name, NULL, '', ', '||First_Name) AS DisplayText FROM CS_SECURITY.People ORDER BY lower(Last_Name) ASC", 30, "None Assigned", "")%>
		</td>
	</tr>
	<tr>
		<td align="right" nowrap>
			<%if CBool(Application("UserInfoRequiredFieldList" & "SITE")) = True then%>
			<span class="required">Site:</span>
			<%else%>
			Site:
			<%end if%>
			
		</td>
		<td>
			<%=ShowSelectBox2(conn, "SiteID", SiteID, "SELECT Site_ID AS Value, Site_Name AS DisplayText FROM CS_SECURITY.Sites ORDER BY lower(Site_Name) ASC", 30, "None Assigned", "")%>
		</td>
	</tr>
	<tr>
		<td align="right" nowrap>
			<%if CBool(Application("UserInfoRequiredFieldList" & "ACTIVE")) = True then%>
			<span class="required">Active:</span>
			<%else%>
			Active:
			<%end if%>
		</td>
		<td>
			<input tabIndex="1" Name="Active" TYPE="checkbox" VALUE="1" <%=ActiveChecked%> <%=checkDisabled%>>
		</td>
	</tr>
<%if isCreate then %>	
	<tr>
		<td align="right" valign="top" nowrap>
			<%if CBool(Application("UserInfoRequiredFieldList" & "ROLES")) = True then%>
			<span class="required">Roles:</span>
			<%else%>
			Roles:
			<%end if%>
		</td>
		<td>
			<input TYPE="Hidden" Name="RolesRevoked" value>
			<script language="javascript">
				buildMultiSelectBox("RolesGranted", "<%=all_roles_string%>", "6")
			</script>
		</td>
	</tr>
<% elseif isEdit then%>
	<tr>
		<td colspan="2">
			<table border="0" width="100%">
			  <tr>
			    <td width="20%">Available Roles
			    </td>
			    <td width="20%">
			    </td>
			    <td width="20%">Current Roles
			    </td>
			  </tr>
			  <tr>
			    <td width="20%">
					<select name="roles" width="220" size="6"></select>
			        <input type="hidden" value name="roles_hidden">
			    </td>
			    <td width="20%">
			      <table border="0" width="100%">
			        <tr>
			          <td width="100%">
							<a href="javascript:addCurrentList()"><img SRC="../graphics/add_role_btn.gif" BORDER="0"></a>
			          </td>
			        </tr>
			        <tr>
			          <td width="100%">
							<a href="javascript:removeCurrentList()"><img SRC="../graphics/remove_role_btn.gif" BORDER="0"></a>
			          </td>
			        </tr>
			      </table>
			    </td>
			    <td width="20%">
					<select name="current_roles" width="220" size="6"></select>
					<input type="hidden" value name="current_roles_hidden">
					<input type="hidden" value="<%=user_roles_list%>" name="original_roles_hidden">
			        <input TYPE="Hidden" Name="RolesGranted" value>
			        <input TYPE="Hidden" Name="RolesRevoked" value>
			    </td>
			  </tr>
			</table>
		</td>
	</tr>	
<%end if%>
	<tr>
		<td colspan="2" align="right"> 
			<a HREF="#" onclick="top.window.close(); return false;"><img SRC="../graphics/cancel_dialog_btn.gif" border="0"></a><a HREF="#" onclick="ValidateUser(); return false;"><img SRC="../graphics/ok_dialog_btn.gif" border="0"></a>
		</td>
	</tr>
</table>


</form>
</center>
<%if LDAP and isCreate then%>
	<script language="JavaScript">
		wDiag = OpenDialog('SelectLDAPUser.asp?dbKey=<%=dbKey%>', 'LDAPDiag', 2);
	</script>
<%end if%>
</body>
</html>

