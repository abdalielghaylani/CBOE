<%@ Language=VBScript %>
<!--#INCLUDE FILE= "tree_funcs_vbs.asp" -->
<%
Dim Conn, Cmd

action = Request.QueryString("action")
dbkey = Request.QueryString("dbname")
SelectedNodeID = Request.QueryString("SelectedNodeID")
SelectedItemID = Request.QueryString("SelectedItemID")
TreeTypeID = Request.QueryString("TreeTypeID")
ItemTypeID = Request.QueryString("ItemTypeID")


NodeName = ""
NodeDesc = ""

Select Case action
	Case "edit_node","delete_node" 
		Set RS = GetNode(SelectedNodeID)
		if not (RS.eof and RS.bof) then 
			NodeName = RS("node_name").value
			NodeDesc = RS("node_description").value
		else
			Response.Write "Could not retrieve node " & SelectedNodeID
			Response.end
		end if
		Set Cmd = Nothing
		Conn.Close
		Set Conn = Nothing
	Case "remove_item","move_item","copy_item"
		Set RS = GetItem(SelectedItemID,ItemTypeID)
		if not (RS.eof and RS.bof) then 
			ItemName = RS("item_name").value
			ItemID = RS("item_id").value
			NodeID = RS("node_id").value
		else
			Response.Write "That item no longer exists (itemid=" & SelectedItemID & ")"
			Response.end
		end if
		Set Cmd = Nothing
		Conn.Close
		Set Conn = Nothing
End Select



%>
<html>
<head>
<meta NAME="GENERATOR" Content="Microsoft Visual Studio 6.0">
<title>Manage Tree dialog</title>
<script LANGUAGE="javascript" src="/biosar_browser/biosar_browser_admin/admin_tool/Choosecss.js"></script>
<script LANGUAGE="javascript">
		window.focus();
		function validate(){
		
			var bWriteError = false;
			var errmsg = "Please fix the following problems:\r";
	
			// Node name is required
			if (document.form1.NodeName){
				if (document.form1.NodeName.value.length == 0) {
					errmsg = errmsg + "- Node name is required.\r";
					bWriteError = true;
				}
				if (bWriteError){
					alert(errmsg);
					return false;
				}
			}
		}
	</script>
</head>
<body>
<br><br><br>
<form name="form1" method="POST" onsubmit="JavaScript: return validate()" action="tree_action.asp?action=<%=action%>&amp;TreeTypeID=<%=TreeTypeID%>&amp;dbname=<%=dbkey%>">
<div align="center">
<table border="0" width="80%">

<%
Select Case action
	Case "create_node","edit_node"%>
	<input type="hidden" name="NodeID" value="<%=SelectedNodeID%>">
	<input type="hidden" name="ParentID" value="<%=SelectedNodeID%>">
	<tr>
		<td align="right" nowrap>
			<span class="required">Node Name:</span>	
		</td>
		<td>
			<input type="text" width="30" name="NodeName" value="<%=NodeName%>">
		</td>
	</tr>
	<tr>
		<td align="right" nowrap>
			Node Description:	
		</td>
		<td>
			<input type="text" width="30" name="NodeDesc" value="<%=NodeDesc%>">
		</td>
	</tr>
<%	Case "delete_node"%>
	<input type="hidden" name="NodeID" value="<%=SelectedNodeID%>">
	<input type="hidden" name="ParentID" value="<%=SelectedNodeID%>">
	<tr>
		<td>
			<input type="hidden" width="30" name="NodeName" value="<%=NodeName%>">
			<input type="hidden" width="30" name="NodeDesc" value="<%=NodeDesc%>">
			<span class="requrired"><p>Do you want to delete <%=NodeName%>?</p></span><br><br>
		</td>
	</tr>
<%	Case "remove_item"%>
	<tr>
		<td>
			<input type="hidden" width="30" name="ItemID" value="<%=ItemID%>">
			<input type="hidden" name="NodeID" value="<%=NodeID%>">
			<input type="hidden" name="ItemTypeID" value="<%=ItemTypeID%>">
			<span class="requrired"><p>Do you want to delete <%=ItemName%>?</p></span><br><br>
		</td>
	</tr>	 	
<%End Select%>
	<tr>
		<td colspan="2" align="right">
			<a HREF="#" onclick="window.close(); return false;"><img SRC="../graphics/cancel_dialog_btn.gif" border="0" WIDTH="61" HEIGHT="21"></a><input type="image" SRC="../graphics/ok_dialog_btn.gif" border="0" WIDTH="61" HEIGHT="21">
		</td>
	</tr>
</table>
</div>
</form>
</body>
</html>
