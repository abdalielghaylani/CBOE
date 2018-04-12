<%@ Language=VBScript %>
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/ado.inc"-->
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/cs_security/cs_security_utils_vbs.asp"-->
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/cs_security/cs_security_utils.js"-->
<%
'Copyright 1999-2003 CambridgeSoft Corporation. All rights reserved

Dim Conn
Dim Cmd
Dim RS

dbkey = Request("dbkey")
RoleName = Request("RoleName")
PrivTableName = Request("PrivTableName") 
IsAlreadyInOracle = Request("IsAlreadyInOracle")
if IsEmpty(IsAlreadyInOracle) OR IsAlreadyInOracle = "" then IsAlreadyInOracle = false

if IsEmpty(PrivTableName) OR PrivTableName = "" then PrivTableName = Application("PRIV_TABLE_NAME")
action = Lcase(Request("action"))
isDelete = false
Select Case action
	Case "create"
		Caption = "Add new Role"
		formAction = "action=create"
		ActiveChecked = "checked"
		isCreate = true
	Case "edit","delete"
		if action = "delete" then
			Caption = "Are you sure you want to delete this Role?"
			formAction = "action=delete"
			isDelete = true
			disabled = " onfocus=""blur()"" "
			checkDisabled = " disabled"
		Else
			Caption = "Update Role:"
			formAction = "action=edit"
			disbled = ""
			isEdit = true
		End if
		RnameDisbled = "onFocus=""blur()"""
	Case "roles"
		Caption = "Manage Roles granted to this Role"
		SelectCaption1 = "Available Roles:"
		SelectCaption2 = "Current Roles:"
		formAction = "action=roles"
		ActiveChecked = "checked"
		isRoles = true
	Case "users"
		Caption = "Manage Grantees for this Role"
		SelectCaption1 = "Available Users:"
		SelectCaption2 = "Current Grantees:"
		formAction = "action=users"
		ActiveChecked = "checked"
		isUsers = true
End Select

Set Conn = GetCS_SecurityConnection(dbKey)

if isCreate  and NOT IsAlreadyInOracle then
	' Get Privs
	Set Cmd = GetCommand(Conn, "{CALL " &  "CS_SECURITY.LOGIN.GETPRIVS(?)}", adCmdText)	
	Cmd.Parameters.Append Cmd.CreateParameter("PPRIVTABLENAME",200, 1, 100, PrivTableName)
	Cmd.Properties ("PLSQLRSet") = TRUE  
	Set RS = Cmd.Execute
	Cmd.Properties ("PLSQLRSet") = FALSE
Elseif isEdit then
	' Get Role Privs
	Set Cmd = GetCommand(Conn, "{CALL " &  "CS_SECURITY.MANAGE_ROLES.GETROLEPRIVS(?)}", adCmdText)	
	Cmd.Parameters.Append Cmd.CreateParameter("PROLENAME",200, 1, 30, RoleName)
	Cmd.Properties ("PLSQLRSet") = TRUE  
	Set RS = Cmd.Execute
	Cmd.Properties ("PLSQLRSet") = FALSE
Elseif isRoles then
	'Get all available roles for this privtable
	Set Cmd = GetCommand(Conn, "{CALL " &  "CS_SECURITY.MANAGE_ROLES.GETROLEAVAILABLEROLES(?,?)}", adCmdText)	
	Cmd.Parameters.Append Cmd.CreateParameter("PPRIVTABLENAME",200, 1, 30, NULL)	
	Cmd.Parameters.Append Cmd.CreateParameter("PROLENAME",200, 1, 30, RoleName)
	Cmd.Properties ("PLSQLRSet") = TRUE  
	Set RS = Cmd.Execute
	Cmd.Properties ("PLSQLRSet") = FALSE
	if NOT (RS.EOF AND RS.BOF) then
		AllRoles_list = RS.GetString(2,,,",","")
		AllRoles_list = Left(AllRoles_list, Len(AllRoles_list)-1)
	Else
		AllRoles_list = ""
	End if
	'Response.Write AllRoles_list & "<BR>"
	'Get Roles assigned to this role
	Set Cmd = GetCommand(Conn, "{CALL " &  "CS_SECURITY.MANAGE_ROLES.GETROLEROLES(?)}", adCmdText)	
	Cmd.Parameters.Append Cmd.CreateParameter("PROLENAME",200, 1, 30, RoleName)	
	Cmd.Properties ("PLSQLRSet") = TRUE  
	Set RS = Cmd.Execute
	Cmd.Properties ("PLSQLRSet") = FALSE
	if NOT (RS.EOF AND RS.BOF) then
		RoleRoles_list = RS.GetString(2,,,",","")
		RoleRoles_list = Left(RoleRoles_list, Len(RoleRoles_list)-1)
	Else
		RoleRoles_list = ""
	End if
Elseif isUsers then
	'Get All available users for this role
	'Get all available roles for this privtable
	Set Cmd = GetCommand(Conn, "{CALL " &  "CS_SECURITY.MANAGE_USERS.GETROLEAVAILABLEUSERS(?,?)}", adCmdText)
	Cmd.Parameters.Append Cmd.CreateParameter("PPRIVTABLENAME",200, 1, 30, NULL)	
	Cmd.Parameters.Append Cmd.CreateParameter("PROLENAME",200, 1, 30, RoleName)
	Cmd.Properties ("PLSQLRSet") = TRUE  
	Set RS = Cmd.Execute
	Cmd.Properties ("PLSQLRSet") = FALSE
	if NOT (RS.EOF AND RS.BOF) then
		AllRoles_list = RS.GetString(2,,,",","")
		AllRoles_list = Left(AllRoles_list, Len(AllRoles_list)-1)
	Else
		AllRoles_list = ""
	End if
	
	'Get All grantees for this role
	Set Cmd = GetCommand(Conn, "{CALL " &  "CS_SECURITY.MANAGE_USERS.GETROLEGRANTEES(?)}", adCmdText)
	Cmd.Parameters.Append Cmd.CreateParameter("PROLENAME",200, 1, 30, RoleName)	
	Cmd.Properties ("PLSQLRSet") = TRUE  
	Set RS = Cmd.Execute
	Cmd.Properties ("PLSQLRSet") = FALSE
	if NOT (RS.EOF AND RS.BOF) then
		RoleRoles_list = RS.GetString(2,,,",","")
		RoleRoles_list = Left(RoleRoles_list, Len(RoleRoles_list)-1)
	Else
		RoleRoles_list = ""
	End if
End if

%>

<html>
<head>
<title>--Add/Update/Delete a Role</title>
<script LANGUAGE="javascript" src="/cfserverasp/source/cs_security/Choosecss.js"></script>

<script language="JavaScript">
	window.focus();
    w_current_list =  "<%=RoleRoles_list%>";
	orig_current_list =  w_current_list;
	w_avail_list = "<%=UCase(AllRoles_list)%>";
	user_name = "<%=RoleName%>";
    function Validate(){
		var PrivList = "";
		var CSSPrivList = "";
		var chkCount = 0;
		var bWriteError = false;
		var errmsg = "Please fix the following problems:\r";
	<%if NOT (isRoles OR isUsers) then%>	
		if (document.user_manager.RoleName){
			var rn = document.user_manager.RoleName.value;
			if (rn.length == 0) {
				errmsg = errmsg + "- Role Name is required.\r";
				bWriteError = true;
			}
			else{
				errmsg1 = IsValidOracleObjectName("Role Name", rn);
				if (errmsg1.length > 0){
					errmsg += errmsg1;
					bWriteError = true;
				}
			}
		}
		if (document.user_manager.Privs){
			for (i=0; i< document.user_manager.Privs.length; i+=1){
				if (document.user_manager.Privs[i].checked){
					PrivList += "'1', ";
					chkCount += 1;
				}
				else{
					PrivList += "'0', ";
				}
			}
			if (chkCount == 0){
				errmsg = errmsg + "- At least one privilege is required.\r";
				bWriteError = true;
			}
			  
			PrivList = PrivList.substring(0, PrivList.length -2);
			document.user_manager.PrivValueList.value = PrivList;
			//alert(document.user_manager.PrivValueList.value)
		}
		//if (document.user_manager.CSSPrivs){
		//	for (i=0; i< document.user_manager.CSSPrivs.length; i+=1){
		//		if (document.user_manager.CSSPrivs[i].checked){
		//			CSSPrivList += "'1', ";
		//		}
		//		else{
		//			CSSPrivList += "'0', ";
		//		}
		//	} 
		//	CSSPrivList = CSSPrivList.substring(0, CSSPrivList.length -2);
		//	document.user_manager.CSSPrivValueList.value = CSSPrivList;
		//}
		<%else%>
			if (document.user_manager.original_roles_hidden.value != document.user_manager.current_roles_hidden.value) {
				document.user_manager.RolesGranted.value = document.user_manager.current_roles_hidden.value;
				document.user_manager.RolesRevoked.value = document.user_manager.original_roles_hidden.value;	
			}
			else{
				document.user_manager.RolesGranted.value = "";
				document.user_manager.RolesRevoked.value = "";
			}	
		<%end if%>
		
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
<body onload="if(document.user_manager.roles) fill_lists(); if (!document.user_manager.RoleName.disabled) document.user_manager.RoleName.focus()">
<center>
<form name="user_manager" action="roles_action.asp?<%=formAction%>" method="POST">
<input type="hidden" name="PrivTableName" value="<%=PrivTableName%>">
<input type="hidden" name="PrivValueList">
<input type="hidden" name="CSSPrivValueList">
<input type="hidden" name="dbKey" value="<%=dbKey%>">
<input type="hidden" name="isAlreadyInOracle" value="<%=isAlreadyInOracle%>">
<table border="0">
	<tr>
		<td colspan="2">
			<span class="GuiFeedback"><%=Caption%></span><br><br>
		</td>
	</tr>
	
	<tr>
		<td align="right" nowrap>
			<span class="required">Role Name:</span>
		</td>
		<td>
			<input tabIndex="1" Name="RoleName" TYPE="tetx" SIZE="40" Maxlength="255" VALUE="<%=RoleName%>" <%=RnameDisbled%>>
			<% if isCreate then%>
				<a class="MenuLink" href="#" onclick="OpenDialog('SelectOracleRole.asp?dbKey=<%=dbKey%>&amp;PrivTableName=<%=PrivTableName%>', 'OraDiag', 3); return false">Select Oracle Role</a>
			<%end if%>
		</td>
	</tr>
<%Select Case action%>	
	<%Case "edit","create"%>
		<%if (NOT IsAlreadyInOracle) then%>			
	<tr>
		<th align="right" valign="top" nowrap>
			Privileges:
		</th>
		<td>
			<%	MinRequiredPriv = Application("MinRequiredPriv")
				Dim HiddenFormFields
				Response.Write "<table border=0>"
				For each fld in RS.Fields
					
					If RS.BOF AND RS.EOF then 
						privValue = ""
					Else
						privValue = StrNull(RS(fld.name))
					End if
					if UCase(fld.name) <> "ROLE_INTERNAL_ID" then
						if MinRequiredPriv <> "" then
							if InStr(Ucase(MinRequiredPriv), UCase(fld.name)) then 
								mustbeChecked = " disabled "
								privValue = 1
							Else
								mustbeChecked = ""
							end if
						end if
						'DGB is_formgroup_role must be visible but not setable
						if UCase(fld.name) = "IS_FORMGROUP_ROLE" then
							mustbeChecked = " disabled "
								privValue = 0
						end if
						
						eName = "Privs"
						if (fld.name = "RID" or fld.name = "CREATOR" or fld.name = "TIMESTAMP") then
							HiddenFormFields = HiddenFormFields & "<input type=hidden name=""" & fld.Name & """ " & " value="""
							if action = "edit" then
								HiddenFormFields = HiddenFormFields & fld.value
							end if 
							HiddenFormFields = HiddenFormFields & """>" & vblf	 
						else
							Response.Write "<tr>" & vblf
							Response.Write "	<td>" & vblf
							Response.Write "		<input type=checkbox name=""" & eName & """ " & isChecked(privValue) & " value=""1""" & mustbeChecked & ">" & vblf	 
							Response.Write "	</td>" & vblf
							Response.Write "	<td>" & vblf
							Response.Write			fld.name & vblf
							Response.Write "	</td>" & vblf
							Response.Write "</tr>" & vblf
						end if

						PrivNamesList = PrivNamesList & fld.name & ","
					end if
					
				 Next
				 PrivNamesList = Left(PrivNamesList, Len(PrivNamesList) - 1)
				 Response.Write "</table>"
				 Response.Write(HiddenFormFields)
				 Response.Write "<input type=hidden name=PrivNamesList value=""" & PrivNamesList & """>" & vblf
			%>
		</td>
	</tr>
	<%Else%>
	<tr>
		<td colspan="2" width="300"><br>Note:<br>This Role will be added to CS Security and will appear when adding or editing users.  However, it will not appear in the manage roles list as it is not an editable role.<br></td>
	</tr>	
	<%end if%>
<%	Case "delete"%>
	<tr>
		<td colspan="2" width="300"><br>Note:<br>This Role and all of its object privileges will be removed from Oracle.<br></td>
	</tr>
<%	Case "roles","users" %>
	<tr>
		<td colspan="2">
			<table border="0" width="100%">
			  <tr>
			    <th width="20%"><%=SelectCaption1%>
			    </th>
			    <td width="20%">
			    </td>
			    <th width="20%"><%=SelectCaption2%>
			    </th>
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
					<input type="hidden" value="<%=RoleRoles_list%>" name="original_roles_hidden">
			        <input TYPE="Hidden" Name="RolesGranted" value>
			        <input TYPE="Hidden" Name="RolesRevoked" value>
			    </td>
			  </tr>
			</table>
		</td>
	</tr>	
<%End Select%>
	<tr>
		<td colspan="2" align="right"> 
			<a HREF="#" onclick="top.window.close(); return false;"><img SRC="../graphics/cancel_dialog_btn.gif" border="0"></a><a HREF="#" onclick="Validate(); return false;"><img SRC="../graphics/ok_dialog_btn.gif" border="0"></a>
		</td>
	</tr>
</table>

</form>
</center>
</body>
</html>
<%
Function IsChecked(str)
	if str = "1" then
		IsChecked = "checked"
	Else
		IsChecked = ""
	End if
End Function
%>
