<%@ Language=VBScript %>


<HTML>
<HEAD>
<!-- #INCLUDE FILE="header.asp" -->
</HEAD>


<%
	
	Dim oRSCol
	Dim lColumnId
	Dim sTableName
	Dim sColumnName
	Dim sOwnerName
	dim lcurrlookupid
	dim lcurrlookupval
	
	lColumnId = Request("column_id")
	lColumnName = Request("column_name")
	lcurrContentTypeid = Request("current_content_type_id")
	lcurrIndexType = Request("current_index_type_id")
	ltableID = request("table_id")
	Set oConn = SysConnection
	if not lcurrIndexType <> "" then lcurrIndexType = 1
	Set oRSCols = oConn.Execute(SQLgetContentTypes(lcurrIndexType))

	
	
%>
<table border = 0><td class="form_purpose_header"  valign = "bottom">
	Content Type for:  </td><td class=form_purpose valign = "bottom"><%=lColumnName%>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</td>
</table><br>
	
	<table border = 0 width = 200>
	<tr><td><hr></td></tr></table>

<br>Check the column to mark as the content type for this field.<br>
<form name=cancel_form method=post action="admin_table_view.asp?<%=session("admin_table_view_cancel_url")%>">
</form>
<form name=admin_form method=post action="admin_table_view.asp?formgroup=base_form_group&dbname=biosar_browser&action=edit_content_type&table_id=<%=ltableID%>&base_col_id=<%=lColumnId%>&user_name=<%=request("user_name")%>" >


<table border=1 cellspacing=0 cellpadding=5><tr>


<td class=table_cell_header>
	Content Type
</td>
<td class=table_cell_header>
	Description
</td>
</tr>



<%
Dim sCheckedId
Dim sCheckedVal
Dim currOwner
Dim currName


Do until oRScols.eof
	
	scheckedid = ""
	scheckedval = ""
	if isNull(lcurrContentTypeid) or Not(lcurrContentTypeid) <> "" then lcurrContentTypeid = 1
	
	if clng(lcurrContentTypeid) = clng(oRSCols("CONTENT_TYPE_ID")) then	
		scheckedid = " checked"
	End if
	
%>
	<tr>
	
	<td class=table_cell><%=oRSCols("MIME_TYPE")%></td>
	<td class=table_cell><%=oRSCols("DESCRIPTION")%></td>
	<td class=table_cell align=center><input type=radio name=content_type_id value=<%=orscols.fields("CONTENT_TYPE_ID")%><%=sCheckedId%>></td>	 
	</tr>
<%
	oRScols.movenext
Loop
%>
</table>

</form>


<P>&nbsp;</P>

<!-- #INCLUDE FILE="footer.asp" -->