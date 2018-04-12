<%@ Language=VBScript %>
<!--#INCLUDE FILE= "tree_funcs_vbs.asp" -->
<%
Response.Expires = -1

Dim Conn, Cmd
Dim dbkey
if dbkey = "" then dbkey = Request.QueryString("dbname")
if action = "" then action = lcase(Request("action"))
NodeID = Request("NodeID")
ParentID = Request("ParentID")
NodeName = Request("NodeName")
NodeDesc = Request("NodeDesc")
if ItemID = "" then ItemID = Request("ItemID")
ItemTypeID= Request("ItemTypeID")
TreeTypeID= Request("TreeTypeID")
if JsCallback= "" then JsCallback = Request("JsCallback") 

Select Case action
	Case "add_item"
		Title = "Add Item Action"
		rc =  AddItemToTree(NodeID, ItemID, ItemTypeID)
		gotoNode = NodeID
	Case "move_item"
		Title = "Move Item Action"
		rc =  MoveItem(ItemID, NodeID)
		gotoNode = NodeID
	Case "copy_item"
		Title = "Copy Item Action"
		rc =  CopyItem(ItemID, NodeID)		
		gotoNode = NodeID
	Case "remove_item"
		Title = "Remove Item Action"

		'First determine if it is a public or private node.....
		parentid = GetParentID(ItemID, NodeID)
		if parentid = clng(Application("PUBLIC_CATEGORY_TREE_NODE_ROOT_ID")) then
			cofpublic = MakeSureNotLastPublicTreeItem(ItemID)
		else
			'this is a cheat that handle more than one public category and private categories....
			cofpublic = 2
		end if
		
		'there must be at least 2 public categories to delete.  The one you are deleting plus one other.
		if cofpublic < 2 then
			rc = "You cannot remove the last public node from the tree.  To remove all public nodes, please go to the security tab and remove all users and roles."
		else		
			rc = RemoveItemFromTree(ItemID, NodeID, ItemTypeID)
			gotoNode = NodeID
		end if
	Case "create_node"
		Title = "Create Node Action"
		rc = AddNodetoTree(ParentID, NodeName, NodeDesc, TreeTypeID)
		GotoNode = rc
	Case "edit_node"
		Title="Edit Node Action"
		rc = EditNode(NodeID, NodeName, NodeDesc)
		gotoNode = NodeID
	Case "delete_node"
	
		Title="Delete Node Action"
		rc = DeleteNode(NodeID)
		gotoNode = rc
	Case "move_node"
		Title = "Move Node Action"
		rc =  MoveNode(NodeID, ParentID)
		gotoNode = NodeID 		
End Select
%>
<HTML>
<HEAD>
<title><%=title%></title>
<META NAME="GENERATOR" Content="Microsoft Visual Studio 6.0">

</HEAD>
<BODY onload="window.focus()">
<%
if NOT IsNumeric(rc) then
	Response.Write "<center><SPAN class=""GuiFeedback"">Error during action: " & action & "<BR>"
	Response.Write "Action could not be completed.</SPAN></center><BR>"
	Response.Write "<P><CODE><center>Error Info: <BR>" & rc &  "</CODE></P></center>"

	Response.Write "<P><center><a HREF=""3"" onclick=""window.close(); return false;""><img SRC=""../graphics/ok_dialog_btn.gif"" border=""0""></a></center>"
else
	if JsCallback <> "" then 
		Response.Write "<script language=""JavaScript"">" & vbcrlf
		Select Case action
			Case "add_item"
				rc = NodeID
				Response.Write "opener." & JsCallback & "(" & rc & ", " & rc & ");" & vbcrlf
			Case "remove_item"
				Response.Write "opener." & JsCallback & "(" & rc & ");" & vbcrlf
			'LJB add move_item
			Case "move_item"
				Response.Write "opener." & JsCallback & "(" & rc & ");" & vbcrlf
		End Select
		
		Response.Write "window.close();" & vbcrlf 
		Response.Write "</script>"
	else
		'Response.Write "<center><SPAN class=""GuiFeedback"">Tree action has been completed</SPAN></center>"
		'Response.Write "<P><center><a HREF=""3"" onclick=""opener.RefreshTree(" & gotoNode & ");window.close(); return false;""><img SRC=""../graphics/ok_dialog_btn.gif"" border=""0""></a></center>"
		Response.Write "<scr" & "ipt language=javascript>opener.RefreshTree(" & gotoNode & ");window.close();</scri" & "pt>"
	end if	
end if
%>
</BODY>
</HTML>
