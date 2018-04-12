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
	lcurrIndexid = Request("current_index_id")
	ltableID = request("table_id")
	datatype = request("datatype")
	Set oConn = SysConnection
	
	Set oRSCols = oConn.Execute(SQLgetIndexTypes(datatype))

	
	
%>
<table border = 0><td class="form_purpose_header"  valign = "bottom">
	Index Type for:  </td><td class=form_purpose valign = "bottom"><%=lColumnName%>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</td>
</table><br>
	
	<table border = 0 width = 200>
	<tr><td><hr></td></tr></table>


Check the column to mark as the index type for this field.<br>
<form name=cancel_form method=post action="admin_table_view.asp?<%=session("admin_table_view_cancel_url")%>">
</form>

<form name=admin_form method=post action="admin_table_view.asp?formgroup=base_form_group&dbname=biosar_browser&action=edit_index_type&table_id=<%=ltableID%>&base_col_id=<%=lColumnId%>" >



<table class=table_list border=1 cellspacing=0 cellpadding=5>


<td class=table_cell_header>
	Index Type
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
	if isNull(lcurrIndexid) or Not(lcurrIndexid) <> "" then lcurrIndexid = 1
	
	if clng(lcurrIndexid) = clng(oRSCols("INDEX_TYPE_ID")) then	
		scheckedid = " checked"
	End if
	
%>
	<tr>
	
	<td class=table_cell><%=oRSCols("INDEX_TYPE")%></td>
	<td  class=table_cell align=center><input type=radio name=index_type_id value=<%=orscols.fields("INDEX_TYPE_ID")%><%=sCheckedId%>></td>	 
	</tr>
<%
	oRScols.movenext
Loop
%>
</table>

</form>


<P>&nbsp;</P>

<!-- #INCLUDE FILE="footer.asp" -->
</body>
</html>
