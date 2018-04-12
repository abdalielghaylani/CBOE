<%@ Language=VBScript %>




<HTML>
<HEAD>
<!-- #INCLUDE FILE="header.asp" -->
</HEAD>
<body>
		<form name=cancel_form method=post action="user_tables.asp?<%=session("user_columns_cancel_url")%>">
		<input type="hidden" name = "cancel_form">
	</form>

<%
lFormType = Request("form_type")
lFormgroupId = Request("formgroup_id")
lTableId = Request("table_id")
Set oConn = SysConnection
Set oRS = oConn.Execute(SQLFormsByFormgroupIdAndFormType(lFormgroupId, lFormType))
lformid = ors("form_id")

select case lFormtype
	Case 1
		sDesc = "Query View"
	case 2
		sdesc = "List View"
	case 3
		sdesc = "Detail View"
end select

select case request("action")
	case "switch"
		bResync=false	
		lid1 = request("column_id_1")
		lid2 = request("column_id_2")
		Set oRSNew = Server.CreateObject("ADODB.Recordset")
		Set oRSNew2 = Server.CreateObject("ADODB.Recordset")
		oRSNew.Open SQLFormitemsByFormItemIdAndFormId(lid1, lformid), oConn, 1, 3
		oRSNew2.Open SQLFormitemsByFormItemIdAndFormId(lid2, lformid), oConn, 1, 3
		corder1 = orsnew("column_order")
		corder2 = orsnew2("column_order")
		if CLng(corder1) = CLng(corder1) then
			bResync = true
		end if
		oRSNew("COLUMN_ORDER") = corder2
		oRSNew2("COLUMN_ORDER") = corder1
		orsnew.Update
		orsnew.Close
		orsnew2.Update
		orsnew2.close
		'go  through form items for this table and resync the column order when to column_order are the same
		if bResync = true then
			Set oRSResync= server.CreateObject("ADODB.RECORDSET")
			sql = "select column_order from BIOSARDB.db_form_item where form_id = '" & lformid & "' and table_id ='" & lTableID & "' order by column_order"
			dim new_counter
			new_counter = 1
			oRSResync.Open sql, oConn, 1, 3
			if not (oRSResync.bof and oRSResync.eof) then
				do while not oRSResync.EOF
					oRSResync("column_order") = new_counter
					oRSResync.Update
					oRSResync.movenext
					new_counter =new_counter+1
				loop
				oRSResync.close
			end if
		end if
end select


Set oRS = Server.CreateObject("ADODB.Recordset")
oRS.Open SQLFormitemsByFormIdAndTableId(lformid, lTableId), oConn, 1, 3
' dbgbreak SQLFormitemsByFormIdAndTableId(lformid, lTableId)
if ors.bof and ors.eof then
%>
	<p>
	No fields have been chosen yet for display from this table.  Please cancel to go back to the previous screen and choose some fields for display.
	</p>
	<form action="user_tables.asp?dbname=biosar_browser&formgroup=base_form_group&cancel=cancel&formgroup_id=<%=lFormgroupId%>&table_id=<%=lTableId%>" id="Form2" name=admin_form method=post>
<input type="hidden" name = "cancel_form2"></form>
<%
else
	sTableName = ors.fields("table_display_name")


	%>
<table border = 0><td class="form_purpose_header"  valign = "bottom">
	Order Fields for <%=Server.HTMLEncode(sTableName)%> - <%=sDesc%></td>
</table><br>
	
	<table border = 0 width = 200>
	<tr><td><hr></td></tr></table>


	<p>
	Move fields into the order in which they should be displayed on the form.</p>

	<form action="user_tables.asp?cancel=cancel&dbname=biosar_browser&formgroup=base_form_group&formgroup_id=<%=lFormgroupId%>&table_id=<%=lTableId%>" id=form1 name=admin_form method=post>

	
	<table border=1 cellspacing=0 cellpadding=5 class=table_list>
	<tr>
	<td class=table_cell_header>Field</td>
	<td class=table_cell_header>Description</td>
	<td class=table_cell_header>Move Up/Down</td>
	</tr>

	<%
	if not (ors.bof and ors.eof) then
		' get dict so we know which formitems precede and follow
		set odict = server.CreateObject("scripting.dictionary")
		ors.movefirst
		lcount = 1
		do until ors.eof
			s = ors("form_item_id")
			odict.Add lcount, s
			lcount = lCount + 1
			ors.movenext
		loop

		lmax = lcount - 1
		lcount = 1
		ors.movefirst
		Do until ors.eof
			select case clng(ors("disp_typ_id"))
				' struc, form, mw is not stored in db
				case 7
					sname = "Structure"
					sdesc = "Graphical Chemical Structure"
				case 9
					sname = "Formula"
					sdesc = "Chemical Formula"
				case 10
					sname = "Mol Wt"
					sdesc = "Molecular Weight"
				case else
					sname = ors("display_name")
					sdesc = ors("description")
			end select
		%>
			<tr>
			<td class=table_cell><%=server.HTMLEncode(sname)%></td>
			<td class=table_cell><%=server.HTMLEncode(sdesc)%></td>
			<td class=table_cell align=center>
		<%
			if lCount > 1 then
		%>
				<a class=MenuLink href="user_column_order.asp?dbname=biosar_browser&formgroup=base_form_group&formgroup_id=<%=lformgroupid%>&table_id=<%=ltableid%>&form_type=<%=lformtype%>&action=switch&column_id_1=<%=oRS("form_item_id")%>&column_id_2=<%=odict(lcount - 1)%>">
				<img src="up.gif" border=0></a>
		<%
			end if
			if lCount < lmax then
		%>
				<a class=MenuLink href="user_column_order.asp?dbname=biosar_browser&formgroup=base_form_group&formgroup_id=<%=lformgroupid%>&table_id=<%=ltableid%>&form_type=<%=lformtype%>&action=switch&column_id_1=<%=oRS("form_item_id")%>&column_id_2=<%=odict(lcount + 1)%>">
				<img src="down.gif" border=0></a>
		<%
			end if	
		%>
			</td>
			</tr>
		<%
			lcount = lcount + 1
			ors.movenext
		loop
	end if
	%>

	
	</table>
	
</form>
<%
end if
%>

<!-- #INCLUDE FILE="footer.asp" -->

</body>
</html>

