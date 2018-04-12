<%@ Language=VBScript %>


<HTML>
<HEAD>

<!-- #INCLUDE FILE="utilities.js" -->

<!-- #INCLUDE FILE="header.asp" -->
</HEAD>


<%
Dim lGroupId, sgroupname, sdesc
'sdesc = Request("old_desc")
'sgroupname = Request("old_name")

form_info = Request("form_info")
temp = split(form_info, ":", -1)
sgroupname = trim(temp(0))
sgroupname = Replace(server.HTMLEncode(sgroupname), "&#160;","")
'temp2 = split(temp(1), "(", -1)
'sdesc = temp2(0)
lGroupId = Request("formgroup_id")
sDesc = request("formgroup_desc_" & lGroupID)
%>
<table border = 0><td class="form_purpose_header"  valign = "bottom">
	Rename Form: <%=Server.HTMLEncode(sGroupName)%></td>
</table><br>
	
	<table border = 0 width = 200>
	<tr><td><hr></td></tr></table>

<form action="user_forms.asp?dbname=biosar_browser&formgroup=base_form_group&formgroup_id=<%=lGroupId%>&action=do_rename" method=post id=form1 name=admin_form>
<input type="hidden" name="old_name" value="<%=sgroupname%>">
<table border=1 cellspacing=0 cellpadding=5 ID="Table1" class=table_list>
<tr><td align=right class=table_cell_header>Name: </td><td class=table_cell><input type=text name="new_name" maxlength=50 value="<%=server.HTMLEncode(sgroupName)%>" ID="Hidden1" ></td></tr>
<tr><td align=right class=table_cell_header>Description: </td><td class=table_cell><input type=text name="new_desc" size = 75 maxlength=250 value="<%=server.HTMLEncode(sDesc)%>" ID="Hidden2")></td></tr>
<tr><td colspan=2><br>The following characters are not allowed in name or description and will be automatically removed: <%=Application("illegalFormCharctersHTML")%></td></tr>

</table>
</form>
<form name=cancel_form method=post action="user_forms.asp?dbname=biosar_browser&formgroup=base_form_group&formgroup_id=<%=lGroupId%>">
	</form>
<!-- #INCLUDE FILE="footer.asp" -->
