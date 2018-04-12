<%@ Language=VBScript %>




<HTML>
<HEAD>
<!-- #INCLUDE FILE="header.asp" -->
</HEAD>

<body>


<%
	Dim sSchemaName
	Dim lSchemaId
	Dim oRSSchema
	
	Set oConn = SysConnection
	
	sSchemaName = Request("user_name")
	lSchemaId = Request("user_id")
	Set oRSSchema = Server.CreateObject("ADODB.Recordset")
	oRSSchema.open SQLSchemaByName(sSchemaName), oconn, 1, 3
	if orsschema.bof and orsSchema.eof then
		oRSSchema.AddNew
		oRSSchema("OWNER") = sSchemaName
		oRSSchema("DISPLAY_NAME") = sSchemaName
		oRSSchema.Update
	end if

%>

<table border = 0><td class="form_purpose_header"  valign = "bottom">
	Schema:  </td><td class=form_purpose valign = "bottom"><%=sSchemaName%>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</td>
</table><br>
	
	<table border = 0 width = 200>
	<tr><td><hr></td></tr></table>


<form action=admin_table_list.asp?dbname=biosar_browser&formgroup=base_form_group&action=change_password&owner=<%=sSchemaName%>&user_name=<%=sSchemaName%> method=post name=admin_form>

<table border=1 cellspacing=0 cellpadding=5>
<tr><td align=right>Name: </td><td><%=sSchemaName%></td></tr>
<tr><td align=right>Display Name: </td><td><input type=textbox name="new_desc" size=50 maxlength=50 value="<%=oRSSchema("DISPLAY_NAME")%>"></td></tr>
<tr><td align=right>Schema Password: </td><td><input type=password name="new_password" size=50 maxlength=50 value="<%=SAR_CryptVBS(oRSSchema("schema_password"),UCase(trim(sSchemaName)))%>"></td></tr>
</table>
<table><tr><td colspan=2><br>The following characters are not allowed in schema display name and will be automatically removed: <%=Application("illegalFormCharctersHTML")%></td></tr></table>


</form>


<!-- #INCLUDE FILE="footer.asp" -->
</body>
</html>