<%@ Language=VBScript %>


<HTML>
<HEAD>

<!-- #INCLUDE FILE="utilities.js" -->

<!-- #INCLUDE FILE="header.asp" -->
</HEAD>


<%

%>
<table border = 0><td class="form_purpose_header"  valign = "bottom">
	Please Enter a Name and Description for the New Form:</td>
</table><br>
	
	<table border = 0 width = 200>
	<tr><td><hr></td></tr></table>

<form action="user_forms.asp?dbname=biosar_browser&formgroup=base_form_group&action=new_form" method=post id=form1 name=admin_form>
<input type="hidden" name="old_name" value="<%=sgroupname%>">
<table border=1 cellspacing=0 cellpadding=5 width="400" class = table_list>
<tr><td align=right class=table_cell_description>Name: </td><td class=table_cell><input type=text name="formgroup_name" maxlength=50 value="<%=server.HTMLEncode(sgroupName)%>" ID="Hidden1"></td></tr>
<tr><td align=right class=table_cell_description>Description: </td><td class=table_cell><input type=text name="formgroup_description" size=75 maxlength=250 value="<%=server.HTMLEncode(sDesc)%>"></td></tr>
<tr><td colspan=2><br>The following characters are not allowed in name or description and will be automatically removed: <%=Application("illegalFormCharctersHTML")%></td></tr>
</table>
</form>

<!-- #INCLUDE FILE="footer.asp" -->
