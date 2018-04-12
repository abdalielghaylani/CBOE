<%@ Language=VBScript %>



<HTML>
<HEAD>
<!-- #INCLUDE FILE="header.asp" -->
</HEAD>


<%
	
	Dim oRSSchema
	Dim oRSSchemaExposed
	Dim sChecked
	
	Set oConn = SysConnection	
%>
<table border = 0><td class="form_purpose_header"  valign = "bottom">
	All Schemas  </td>
</table><br>
	
	<table border = 0 width = 200>
	<tr><td><hr></td></tr></table>


<form action=admin_table_list.asp?formgroup=base_form_group&dbname=biosar_browser&action=expose_schemas method=post id=form1 name=admin_form>

Checked schemas will be available for use in the system.<p>

<table border=1 cellspacing=0 cellpadding=5 class=table_list>
<tr><td class=table_cell_header>Schema Name</td><td class=table_cell_header>Exposed</td></tr>

<%
	Set oRSSchema = oConn.Execute(SQLGetAllOwners)
	Set oRSSchemaExposed = oConn.Execute(SQLAllExposedSchemas)
	Dim oDict, sOwner
	Set oDict = DictFromRS(oRSSchemaExposed, "OWNER", "OWNER", "")
	do until orsschema.eof	
		sOwner = orsschema.fields("USERNAME")
		If oDict.Exists(sOwner) Then
			sChecked = "checked"
		Else
			sChecked = ""
		End If
%>
		<tr><td class=table_cell>
		<%=sOwner%>
		</td>
		<td align=centerclass=table_cell>
		<input type=checkbox name="schema_names" value="<%=sOwner%>"  <%=" " & schecked%>>
		</td>
		</tr>
<%		
		oRSSchema.MoveNext
	Loop
%>

</table><p>

</form>

<form name=cancel_form action=admin_table_list.asp?formgroup=base_form_group&dbname=biosar_browser&action=get_schema_list&user_name=<%=sOwner%> method=post id=form1 >
<input type = "hidden" name="cancel_form">
</form>

<!-- #INCLUDE FILE="footer.asp" -->

</body>
</html>