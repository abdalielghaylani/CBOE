<%@ Language=VBScript %>
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/ado.inc"-->
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/cs_security/cs_security_utils_vbs.asp"-->
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/cs_security/cs_security_utils.js"-->
<%
'Copyright 1999-2003 CambridgeSoft Corporation. All rights reserved

	
	Dim Cmd
	Dim Conn
	
	dbkey = Request("dbkey")
	PrivTableName = Ucase(Request("PrivTableName")) 
	
	if IsEmpty(PrivTableName) OR PrivTableName = "" then PrivTableName = Ucase(Application("PRIV_TABLE_NAME"))
	Set Conn = GetCS_SecurityConnection(dbKey)
	Set Cmd = GetCommand(Conn, "{CALL " &  "CS_SECURITY.MANAGE_ROLES.GETROLES(?)}", adCmdText)
	Cmd.Parameters.Append Cmd.CreateParameter("PPRIVTABLENAME",200, 1, 100, PrivTableName)
	Cmd.Properties ("PLSQLRSet") = TRUE  
	Set RS = Cmd.Execute
	Cmd.Properties ("PLSQLRSet") = FALSE
	
	if Session("CSS_EDIT_ROLE" & dbkey) then dblClickHandler = "ondblClick=""Doit(this.selectedIndex,'edit');return false"""
	
%>
<html>
<head>
	<title>Manage Roles</title>
	<script LANGUAGE="javascript">
		window.focus();
		function Doit(id, action){
			if (id == -1){
				 alert('Select a role to ' + action + ' from the list.')
			}
			else{
				 roleName = document.form1.RoleName[id].value
				 OpenDialog('roles_frset.asp?dbKey=<%=dbKey%>&PrivTableName=' + document.form1.PrivTableName[document.form1.PrivTableName.selectedIndex].value + '&action=' + action + '&RoleName=' + roleName , 'Diag', 2);
			}
		}
	</script>

	<script LANGUAGE="javascript" src="/cfserverasp/source/cs_security/Choosecss.js"></script>

</head>
<body>
<table width="100%">
	<%if Session("CSS_CREATE_USER" & dbkey) OR Session("CSS_EDIT_USER" & dbkey) OR Session("CSS_DELETE_USER" & dbkey) then%>
	<tr>
		<td align="right">
			<a Class="MenuLink" Href="ManageUsers.asp?dbkey=<%=dbkey%>&amp;PrivTableName=<%=PrivTableName%>">Manage Users</a> | <a Class="MenuLink" disabled Href="ManageRoles.asp?dbkey=<%=dbkey%>&amp;PrivTableName=<%=PrivTableName%>">Manage Roles</a>
		</td>
	</tr>
	<%end if%>
</table>	
	<center>	
<form name="form1" METHOD="POST" action="manageRoles.asp?dbkey=<%=dbKey%>">
<table border="0">
	<tr>
		<th align="right">
			View Roles from:
		</th>
		<td>
			<%= ShowSelectBox2(Conn, "PrivTableName", PrivTableName, "SELECT Privilege_Table_Name AS Value, App_Name AS DisplayText FROM CS_SECURITY.Privilege_Tables WHERE Upper(app_name) NOT LIKE '%_EXCLUDE' ORDER BY lower(App_Name) ASC", 30, "", "")%>&nbsp;<a class="MenuLink" href="#" onclick="document.form1.submit(); return false;">Refresh</a>
		</td>
	</tr>
	<tr>
		<th align="right" valign="top">Select a Role:</th>
		<td>
			<select name="RoleName" size="25" <%=dblclickHandler%>>
<%			
	If NOT (RS.EOF AND RS.BOF) then
		While (Not RS.EOF)
			RoleName = Ucase(RS("ROLE_NAME"))
			Response.Write "<OPTION value=""" & RoleName & """>" & RoleName &  vblf 
	        
		RS.MoveNext
		Wend
		RS.Close
	Conn.Close
	Set RS = nothing
	Set Cmd = nothing
	Set Conn = nothing	
	end if
%>
			</select>
		</td>
	</tr>
	<tr>
		<td colspan="2" align="right" valign="bottom">
		     <%=GetCancelButton()%>
		     <%if Session("CSS_CREATE_ROLE" & dbkey) then%>
		     <a Class="MenuLink" Href="#" onclick="OpenDialog('roles_frset.asp?dbKey=<%=dbKey%>&amp;action=create&amp;PrivTableName=' + document.form1.PrivTableName[document.form1.PrivTableName.selectedIndex].value, 'Diag', 2); return false"><img SRC="../graphics/add_new_role_btn.gif" border="0"></a>
		     <%end if%>
		     <%if Session("CSS_EDIT_ROLE" & dbkey) then%>
		     <a Class="MenuLink" Href="#" onclick="Doit(document.form1.RoleName.selectedIndex, 'edit'); return false;"><img SRC="../graphics/update_role_btn.gif" border="0"></a>
			 <a Class="MenuLink" Href="#" onclick="Doit(document.form1.RoleName.selectedIndex, 'users'); return false;"><img SRC="../graphics/usr_mgr_btn.gif" border="0"></a>
		     <a Class="MenuLink" Href="#" onclick="Doit(document.form1.RoleName.selectedIndex, 'roles'); return false;"><img SRC="../graphics/manage_roles_btn.gif" border="0"></a>
			 <%end if%>
			 <%if Session("CSS_DELETE_ROLE" & dbkey) then%>
			 <a Class="MenuLink" Href="#" onclick="Doit(document.form1.RoleName.selectedIndex, 'delete'); return false;"><img SRC="../graphics/delete_role_btn.gif" border="0"></a>	
			 <%end if%>
		</td>
	</tr>
</table>
</form>
<br>
</center>
</body>
</html>
