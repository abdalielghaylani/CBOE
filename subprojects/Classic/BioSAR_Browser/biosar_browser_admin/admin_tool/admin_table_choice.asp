<%@ Language=VBScript %>




<HTML>
<HEAD>
<!-- #INCLUDE FILE="header.asp" -->
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/utility_func_vbs.asp"-->


<script language="javascript">
	var alertedOnce = false;
	function alertUnchecked(theField){
		var delete_metadata = "<%=Application("DELETE_METADATA_WHEN_UNEXPOSING_TABLE_OR_VIEW")%>"
		if (delete_metadata ==1){
			if ((document.forms['admin_form'].elements[theField.value].value == "checked") && (theField.checked==false) && (!alertedOnce)){
				//this means that user is trying to unexpose a table or view and must be warned on consequence
				if (confirm("Unexposing tables/views will remove them from the BioSAR schema. As a consequence, the tables/views will be removed from any form referencing them as a child table. Further, all forms that use unexposed tables as a base table will be deleted. Alternatively, you can use the Table Refresh button to update all schemas and tables with newly added fields.  Do you still want to continue?")){
						alertedOnce = true;
					}else{
						//user canceled action so reset to checked so the table will not be removed.
						theField.checked=true
					}
			 }
		 }
	}

</script>
</HEAD>

<body>


<%
	Dim sSchemaName
	Dim lSchemaId
	Dim oRSSchema
	
	
	sSchemaName = Request("user_name")
	Set oConn = SysConnection
	lSchemaId = Request("user_id")
	Set oRSSchema = Server.CreateObject("ADODB.Recordset")
	oRSSchema.open SQLSchemaByName(sSchemaName), oconn, 1, 3

	if orsschema.bof and orsSchema.eof then
		oRSSchema.AddNew
		oRSSchema("OWNER") = sSchemaName
		oRSSchema("DISPLAY_NAME") = sSchemaName
		oRSSchema.Update
	end if
	
	on error resume next
	
	Set oSchemaConn = SchemaConnection(sSchemaName)
	'get all tables in schema using the schema password
	if err.number <> 0 then
		if inStr(err.Description, "01017") then
			Response.Write "The password for this schema is incorrect. Please click Cancel, then click Edit Schema Description/Password and correct the password."
		else
			Response.Write "A password for this schema was not found. Please click Cancel, then click Edit Schema Description/Password and set a password."
		end if
	else

%>

<table border = 0><td class="form_purpose_header"  valign = "bottom">
	Schema: </td><td class=form_purpose> <%=sSchemaName%></td>
</table><br>
	
	<table border = 0 width = 200>
	<tr><td><hr></td></tr></table>
	<% if Application("DELETE_METADATA_WHEN_UNEXPOSING_TABLE_OR_VIEW") = 1 then %>
	Select tables and views that you want exposed. 
	<br>
<%else%>
	Select tables and views that you want visible. 
	<br>
<%end if%>

<form action=admin_table_list.asp?dbname=biosar_browser&formgroup=base_form_group&action=expose_tables&owner=<%=sSchemaName%>&user_name=<%=sSchemaName%> method=post name=admin_form>



<%
	Dim oRSTable
	Dim oRSView
	Dim oRSChecked
	Dim sChecked
%>

<table border=1 cellspacing=0 cellpadding=5>
<% if Application("DELETE_METADATA_WHEN_UNEXPOSING_TABLE_OR_VIEW") = 1 then %>
<tr><td class=table_cell_header>Table Name</td><td class=table_cell_header>Exposed?</td></tr>
<%else%>
<tr><td class=table_cell_header>Table Name</td><td class=table_cell_header>Visible?</td></tr>
<%end if%>
<%
	Set oRSTable = oConn.Execute(SQLAllTablesByOwner(sSchemaName))
	Dim oDict
	
	Set oDict = getSchemaTables(sSchemaName,oSchemaConn)
	'Set oDict = AuthorizedTables(sSchemaName)
	if not (oRSTable.BOF and oRSTable.EOF) then
	Do Until oRSTable.EOF
			Set oRSChecked = oConn.Execute("select is_exposed from BIOSARDB.db_table where Upper(table_short_name) = '" & UCase(oRSTable("TABLE_NAME")) & "' and Upper(owner)= '" & UCase(sSchemaName) & "'")
			sChecked = ""
			if not oRSCHecked.EOF then
				if oRSChecked("IS_EXPOSED") = "Y" then
					sChecked = "checked"
				end if
			End If
%>
<%
		if oDict.Exists(cstr(sSchemaName & "." & oRSTable("TABLE_NAME"))) Or IGNORE_ADMIN_PERMISSIONS then
%>
			<tr><td class=table_cell>
			<%=UCase(oRSTable.Fields("TABLE_NAME"))%>
			</td>
			<td align=center class=table_cell>
			
			<input type=checkbox name="table_names" onClick="alertUnchecked(this)" value="<%=oRSTable("OWNER") & "." & oRSTable("TABLE_NAME")%>"  <%=" " & schecked%>>
			
			<input type="hidden" name = "<%=oRSTable("OWNER") & "." & oRSTable("TABLE_NAME")%>" value="<%=schecked%>">
			</td>
			</tr>
<%   
		else
			if sChecked = "checked" then 
%>
				<input type=hidden name="table_names" value="<%=oRSTable("OWNER") & "." & oRSTable("TABLE_NAME")%>">
<%
			end if
		end if
%>
<%		
		oRSTable.MoveNext
	Loop
	else
		Response.Write "<tr><td>No Tables Found</td></tr>"
	
	end if
%>

</table><p>

<table border=1 cellspacing=0 cellpadding=5>
<tr><td class=table_cell_header>View Name</td><td class=table_cell_header>Visible?</td></tr>

<%
	Set oRSView = oConn.Execute(SQLAllViewsByOwner(sSchemaName))
	if not (orsView.BOF and orsView.EOF) then
	Do Until oRSView.EOF
		Set oRSChecked = oConn.Execute("select is_exposed from BIOSARDB.db_table where Upper(table_short_name) = '" & UCase(oRSView("VIEW_NAME")) & "' and Upper(owner) = '" & UCase(sSchemaName) & "'")
		sChecked = ""
		if not oRSCHecked.EOF then
			if oRSChecked("IS_EXPOSED") = "Y" then
				sChecked = "checked"
			end if
		End If
%>
<%
		if oDict.Exists(sSchemaName & "." & oRSView("VIEW_NAME")) Or IGNORE_ADMIN_PERMISSIONS then
%>
			<tr><td class=table_cell>
			<%=UCase(oRSView.Fields("VIEW_NAME"))%>
			</td>
			<td align=center class=table_cell>
			
			<input type=checkbox name="view_names" onClick="alertUnchecked(this)" value="<%=oRSView("OWNER") & "." & oRSView.Fields("VIEW_NAME")%>"   <%=" " & schecked%>>
			
			<input type="hidden" name = "<%=oRSView("OWNER") & "." & oRSView.Fields("VIEW_NAME")%>" value="<%=schecked%>">
			</td>
			</tr>
<%   
		else
			if sChecked = "checked" then 
%>
				<input type=hidden name="view_names" value="<%=oRSView("OWNER") & "." & oRSView("VIEW_NAME")%>">
<%
			end if
		end if
%>
<%		
		oRSView.MoveNext
	Loop
	else
		Response.Write "<tr><td>No Views Found</td></tr>"
	
	end if
%>
<%end if %>
</table><p>

</form>


<!-- #INCLUDE FILE="footer.asp" -->
</body>
</html>