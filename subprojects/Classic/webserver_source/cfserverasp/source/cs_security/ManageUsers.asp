<%@ Language=VBScript %>
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/ado.inc"-->
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/cs_security/cs_security_utils_vbs.asp"-->
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/cs_security/cs_security_utils.js"-->

<%
'Copyright 1999-2003 CambridgeSoft Corporation. All rights reserved

	Dim Cmd
	Dim Conn
		
	PrivTableName = Request("PrivTableName") 
	dbkey = Request("dbkey")
	
	
	if IsEmpty(PrivTableName) OR PrivTableName = "" then PrivTableName = NULL
	
	Set Conn = GetCS_SecurityConnection(dbKey)
	Set Cmd = GetCommand(Conn, "{CALL CS_SECURITY.MANAGE_USERS.GETUSERS(?)}", adCmdText)
	Cmd.Parameters.Append Cmd.CreateParameter("PPRIVTABLENAME",200, 1, 100, PrivTableName)
	Cmd.Properties ("PLSQLRSet") = TRUE  
	Set RS = Cmd.Execute
	Cmd.Properties ("PLSQLRSet") = FALSE
	
	if Session("CSS_EDIT_USER" & dbkey) then dblClickHandler = "ondblClick=""Doit(this.selectedIndex,'edit');return false"""
	 
%>
<html>
<head>
	<title>Manage Users</title>
	<script LANGUAGE="javascript">
		window.focus();
		function Doit(id, action){
			if (id == -1){
				 alert('Select a user to ' + action + ' from the list.')
			}
			else{
				UName = document.form1.UserName[id].value;
				 OpenDialog('users_frset.asp?dbKey=<%=dbKey%>&PrivTableName=<%=PrivTableName%>&action=' + action + '&UserName=' + UName, 'Diag', 2);
			}
		}
	</script>

	<script LANGUAGE="javascript" src="/cfserverasp/source/cs_security/Choosecss.js"></script>
</head>
<body>
<table width="100%">
	 <%if Session("CSS_CREATE_ROLE" & dbkey) OR Session("CSS_EDIT_ROLE" & dbkey) OR Session("CSS_DELETE_ROLE" & dbkey) then%>
	<tr>
		<td align="right">
			<a Class="MenuLink" disabled Href="ManageUsers.asp?dbkey=<%=dbkey%>&amp;PrivTableName=<%=PrivTableName%>">Manage Users</a> | <a Class="MenuLink" Href="ManageRoles.asp?dbkey=<%=dbkey%>&amp;PrivTableName=<%=PrivTableName%>">Manage Roles</a>
		</td>
	</tr>
	<%end if%>
</table>	
	<center>	
<%	
	If (RS.EOF AND RS.BOF) then
		Response.Write ("<BR><BR><BR><BR><BR><table><TR><TD align=center colspan=6><span class=""GUIFeedback"">No Users found</Span></TD></tr></table><Br><BR><BR>")
		Response.write GetCancelButton()
	Else	
%>
<form name="form1" METHOD="POST" action="manageUsers.asp?dbkey=<%=dbKey%>">

<table border="0">
	<tr>
		<th>
			View Users from:
		</th>
		<td>
			<%= ShowSelectBox2(Conn, "PrivTableName", PrivTableName, "SELECT Privilege_Table_Name AS Value, App_Name AS DisplayText FROM CS_SECURITY.Privilege_Tables WHERE Upper(app_name) NOT LIKE '%_EXCLUDE' ORDER BY lower(App_Name) ASC", 30, "All CS Applications", "")%>&nbsp;<a class="MenuLink" href="#" onclick="document.form1.submit(); return false;">Refresh</a>
		</td>
	</tr>
	<tr>
		<th align="right" valign="top">Select a User:</th>
		<td>
			<select name="UserName" size="25" <%=dblClickHandler%>>
<%			
	
		While (Not RS.EOF)
			UserName = Ucase(RS("UserName"))
			FName = RS("FirstName")
			LName = RS("LastName")
			if Len(FName) + Len(LName) > 0 then
				if Len(LName) > 0 then LName = LName & ","
				RealName = " (" & LName & FName & ")"
			Else
				RealName = ""
			End if		
			Response.Write "<OPTION value=""" & UserName & """>" & UserName & RealName &  vblf 
	        
		RS.MoveNext
		Wend
%>
			</select>
		</td>
	</tr>
	<tr>
		<td colspan="2" align="right" valign="bottom">
		     <%=GetCancelButton()%>
		     <%if Session("CSS_CREATE_USER" & dbkey) then%>
		     <a Class="MenuLink" Href="#" onclick="OpenDialog('users_frset.asp?dbKey=<%=dbKey%>&amp;PrivTableName=<%=PrivTableName%>', 'Diag', 2); return false"><img SRC="../graphics/add_user_btn.gif" border="0"></a>
		     <%end if%>
		     <%if Session("CSS_EDIT_USER" & dbkey) then%>
		     <a Class="MenuLink" Href="#" onclick="Doit(document.form1.UserName.selectedIndex, 'edit'); return false;"><img SRC="../graphics/update_user_btn.gif" border="0"></a>
		     <%end if%>
		     <% 'DGB disabled the create user button because it is not sensible to delete users from the database.  They should be deactivated via the update user.
		        'if Session("CSS_DELETE_USER" & dbkey) then
		       if false then
		     %>
		     <a Class="MenuLink" Href="#" onclick="Doit(document.form1.UserName.selectedIndex, 'delete'); return false;"><img SRC="../graphics/delete_user_btn.gif" border="0"></a>
			 <%end if%>
		</td>
	</tr>
</table>
</form>
<%
	RS.Close
	Conn.Close
	Set RS = nothing
	Set Cmd = nothing
	Set Conn = nothing		
End if
%>
<br>
</center>
</body>
</html>
