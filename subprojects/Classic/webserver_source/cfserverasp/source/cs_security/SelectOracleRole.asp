<%@ Language=VBScript %>
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/ado.inc"-->
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/cs_security/cs_security_utils_vbs.asp"-->
<%
'Copyright 1999-2003 CambridgeSoft Corporation. All rights reserved

	
	Dim Cmd
	Dim Conn
	dbkey = Request("dbkey")
	PrivTableName = Request("PrivTableName")
	Set Conn = GetCS_SecurityConnection(dbKey)
	Set Cmd = GetCommand(Conn, "{CALL CS_SECURITY.MANAGE_ROLES.GETALLROLES()}", adCmdText)	
	Cmd.Properties ("PLSQLRSet") = TRUE  
	Set RS = Cmd.Execute
	Cmd.Properties ("PLSQLRSet") = FALSE
%>
<html>
<head>
	<title>Select Oracle Roles</title>
	<script LANGUAGE="javascript">
		window.focus();
		function Doit(id){
			if (id == -1){
				 alert('Select a Role to ' + action + ' from the list.')
			}
			else{
				// opener.document.role_manager.RoleName.value = id;
				// opener.document.role_manager.isAlreadyInOracle.value = "1";
				// opener.lockFields();
				var UName = document.form1.UserName[id].value;
				opener.location.href = "roles.asp?dbKey=<%=dbKey%>&action=create&IsAlreadyInOracle=true&PrivTableName=<%=PrivTableName%>&RoleName=" + UName
				 window.close();
			}
		}
	</script>

	<script LANGUAGE="javascript" src="/cfserverasp/source/cs_security/Choosecss.js"></script>
</head>
<body>
	<center>	
<%	
RS.MoveFirst
	If (RS.EOF AND RS.BOF) then
		Response.Write ("<table><TR><TD align=center colspan=6><span class=""GUIFeedback"">No Users found</Span></TD></tr></table>")
	Else	
%>
<form name="form1" METHOD="POST" action="ManageRoles.asp">

<table border="0">
	<tr>
		<th align="center" valign="top">Select a Role:</th>
	</tr>
	<tr>
		<td>
			<select name="UserName" size="25" ondblClick="Doit(this.selectedIndex);return false">
<%			
	
		While (Not RS.EOF)
			RoleName = Ucase(RS("Role_Name"))
				
			Response.Write "<OPTION value=""" & RoleName & """>" & RoleName  &  vblf 
	        
		RS.MoveNext
		Wend
%>
			</select>
		</td>
	</tr>
	<tr>
		<td align="right" valign="bottom">
		     <a Class="MenuLink" Href="#" onclick="window.close(); return false;"><img SRC="../graphics/cancel_dialog_btn.gif" border="0"></a>
		     <a Class="MenuLink" Href="#" onclick="Doit(document.form1.UserName.selectedIndex); return false;"><img SRC="../graphics/ok_dialog_btn.gif" border="0"></a>
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
