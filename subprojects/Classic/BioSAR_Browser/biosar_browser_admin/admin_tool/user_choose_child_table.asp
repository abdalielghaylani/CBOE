<%@ Language=VBScript %>


<HTML>
<HEAD>

<!-- #INCLUDE FILE="header_no_adoinc.asp" -->
<%' LJB 3/2005 wipe out the change list when the referrer is user_tables.asp 
if instr(Request.ServerVariables("HTTP_REFERER"), "user_tables.asp")> 0 then
		Session("ChangeList")=""
		Set Session("TVNodes" & "8") = nothing
		Session("TVNodes" & "8") = ""
end if
%>
<script language="javascript">
//function for keeping track of changes made the the child table tree. Calls a hidden frame and populates Session("ChangeList")
//this variable is used in tree_func_vbs.asp to update the state of the checkboxes. It is also used by user_tables.asp to update the list of child tables.
function updateTableChangeList(id) {
	
	top.frames["helper"].location.replace(app_Path + "/helper.asp?table_id=" + id + "&action=" + "changelist_tableids"  + "&Timer=" + theTimer)	
}
//update the checkbox state based on the changelist when a node is expanded or collapsed
function updateCheckboxState(){
	var changeList= "<%=Session("ChangeList")%>"
	if(changeList.length > 0){
		temp = changeList.split(",")
		for (i=0;i<temp.length;i++){
			if (document.all.item(temp[i])){
				if (document.all.item(temp[i]).checked ==  true){
						document.all.item(temp[i]).checked = false
					}
					else{
						if (document.all.item(temp[i]).checked ==  false){
							document.all.item(temp[i]).checked = true
						}
					}
			}
	
		}
	}	
}

</script>

</HEAD>
<body >
<table border = 0><td class="form_purpose_header"  valign = "bottom">
	Choose Child Tables:</td>
</table><br>
	<form name=cows_input_form  method="post" ID="Form2"></form>

	<table border = 0 width = 200>
	<tr><td><hr></td></tr></table>
	<form name=cancel_form method=post action="user_tables.asp?<%=session("user_columns_cancel_url")%>">
	<input type="hidden" name = "cancel_form">
	
	</form>
<%
dim lBaseTableId
dim lgroupid
lgroupid = request("formgroup_id")
lBaseTableId = Request("base_table_id")

%>

<form action="user_tables.asp?dbname=biosar_browser&formgroup=base_form_group&formgroup_id=<%=lgroupid%>&base_table_id=<%=lBaseTableId%>&action=change_child_tables" method=post id=form1 name=admin_form>


<!--#INCLUDE VIRTUAL="/Biosar_browser/source/inlineTreeview.asp"-->


</form>
<!-- #INCLUDE FILE="footer.asp" -->

</body>
</html>
<script language="javascript">
	updateCheckboxState()
</script>
