<!--#INCLUDE FILE= "tree_funcs_vbs.asp" -->
<%
Dim Conn, Cmd
Dim RS
Dim dbkey
Response.ExpiresAbsolute = Now()

' Kill session if requested --for debugging use
if Request.QueryString("killsession")= 1 then 
	Session.Abandon
	Response.Write "Session Abandoned"
	Response.End
End if

' Script Variables

bRefresh = false
' Define tree icons
imgpath = "/" & Application("appkey") & "/graphics/images/treeview"
' These are the arrow icons
Icon_clsd = "right.gif"
Icon_open = "down2.gif"

'Read QueryString parameters
dbkey = Request.QueryString("dbname")
if not dbkey <> "" then
	dbkey = Application("appkey")
end if
TreeMode = lcase(Request.QueryString("TreeMode"))
TreeID = Request.QueryString("TreeID")
TreeTypeID = Request.QueryString("TreeTypeID")
ClearNodes = Request("ClearNodes")
RemoveNode = Request.QueryString("RemoveNode")
sNode = Request.QueryString("sNode")
openNodesList = Request.QueryString("Node")
ExpNode = Request.QueryString("Exp")
GotoNode = Request.QueryString("GotoNode")
SelectNode = Request.QueryString("SelectNode")
formelm= Request("formelm")
elm1= Request("elm1")
elm2= Request("elm2")
elm3= Request("elm3")
lStyle = Request.QueryString("style")
NodeTarget = Request.QueryString("NodeTarget")
NodeURL = Request.QueryString("NodeURL")
ItemTarget = Request.QueryString("ItemTarget")
ItemURL = Request.QueryString("ItemURL")
ItemID = Request.QueryString("ItemID")
NodeID = Request.QueryString("NodeID")
ItemTypeID = Request.QueryString("ItemTypeID")
JsCallback = Request.QueryString("JsCallback")
bShowItems = Request.QueryString("ShowItems")
ItemQSid=Request.QueryString("ItemQSid")
ItemQSName=Request.QueryString("ItemQSName")
NodeQSid=Request.QueryString("NodeQSid")
NodeQSName=Request.QueryString("NodeQSName")
ItemCheckbox=Request.QueryString("ItemCheckbox") 
QsRelay = Request.QueryString("QSRelay")
ExpandedRoot = Request.QueryString("ExpandedRoot")
QSHelp = Request.QueryString("QSHelp")
refreshPath =Request.QueryString("refreshPath") 


if IsEmpty(ItemFilterSelect) then  
	ItemFilterSelect = Request("ItemFilterSelect")
	if isEmpty(ItemFilterSelect) then ItemFilterSelect=""
	'add this to the query string
	qsItemFilterSelect = "&ItemFilterSelect=" & ItemFilterSelect
else
	'if it is a page variable don't add it to the query string
	qsItemFilterSelect =""
end if
	


if  IsEmpty(CheckboxSelect) then  
	CheckboxSelect = Request("CheckboxSelect")
	if isEmpty(CheckboxSelect) then CheckboxSelect=""
end if


' Set default values
If lStyle = "" Then lStyle = 7
if dbkey = "" then TreeError "dbkey is a required parameter."
if TreeMode = "" Then TreeError "TreeMode is a required parameter." 
if TreeTypeID = "" Then TreeError "TreeTypeID is a required parameter."
if TreeID = "" then TreeID =1
if TreeMode="multi_select" AND ItemCheckbox = "" then ItemCheckbox = "ItemChkbx"
if bShowItems = "" then bShowItems = false
if ExpandedRoot = "" then ExpandedRoot = 1

If  GotoNode= "" OR GotoNode = "0" then
	Session("CurrentnodeID")= 0
	bGotoNode = False
Else
	bGotoNode = True
End if 
Qs = "dbname=" & dbkey & "&TreeTypeID=" & TreeTypeID & "&NodeURL=" & NodeURL & "&NodeTarget=" & NodeTarget & "&ItemURL=" & ItemURL & "&ItemTarget=" & ItemTarget & "&TreeID=" & TreeID & "&TreeMode=" & TreeMode & "&ItemID=" & ItemID & "&ItemTypeID=" & ItemTypeID & "&JsCallback=" & JsCallback & qsItemFilterSelect & "&CheckboxSelect=" & CheckboxSelect & "&ItemCheckbox=" & ItemCheckbox & "&ExpandedRoot=" & ExpandedRoot

if QSRelay <> "" then 
	QS = QS & "&QSRelay=" & QSRelay
	temp_arr = split(QSRelay,",")
	For i = 0 to Ubound(temp_arr,1)
		QS = QS & "&" & temp_arr(i) & "=" & Request.QueryString(temp_arr(i))
	next
end if
 


'Validate Input
Select Case TreeMode	
	Case "select_item"
		bShowItems = true
		if JsCallback = "" then TreeError "JsCallback is required for select_item TreeMode."		
		Title = "Select an Item from the Tree"
	Case "multi_select"
		bShowItems = true		
		Title = "Select Items from the Tree"		
	Case "select_node"
		if JsCallback = "" then TreeError "JsCallback is required for select_node TreeMode."
		Title = "Select a Node from the Tree"
	Case "add_item"
		if ItemID = "" then TreeError "ItemID is required for add_item TreeMode."	
		Title = "Select a Node to which to Add item " & ItemID
	Case "move_item"
		if ItemID = "" then TreeError "ItemID is required for move_item TreeMode."	
		Title = "Select a Node to which to move item " & ItemID	
	Case "remove_item"
		Response.Redirect("tree_action.asp?action=remove_item&dbname="& dbkey & "&JsCallback=" & JsCallBack & "&ItemID=" & ItemID & "&ItemTypeID=" & ItemTypeID & "&NodeID=" & NodeID )	 
	Case "manage_tree"
	
		'bShowItems = true
		Title = "Manage Tree"
	Case "open_item"
		bShowItems = true
		if ItemURL = "" then TreeError "ItemURL is required for open_item TreeMode."
	Case "open_node"
		if NodeURL = "" then TreeError "NodeURL is required for open_node TreeMode."			
End Select

if bShowItems AND ItemTypeID = "" then TreeError "ItemTypeID is required to display items."

TreeType_ID_arr = split(TreeTypeId, ",")
 
' Initialize tree control
Set TreeView = Server.CreateObject("VASPTV.ASPTreeView")
TreeView.Class = "TreeView"
TreeView.Style = clng(lStyle)
TreeView.LineStyle = 1
TreeView.ImagePath = imgpath
TreeView.AutoScrolling = True
TreeView.SingleSelect = False
TreeView.QueryString = ""
TreeView.LicenseKey = "8712-0DFC-5CEB"
TreeView.QueryString = Qs 
GetDBConnection()

%>
<script LANGUAGE="javascript" src="/biosar_browser/biosar_browser_admin/admin_tool/Choosecss.js"></script>
<SCRIPT LANGUAGE=javascript src="/biosar_browser/source/tree_functions.js"></SCRIPT>
<SCRIPT LANGUAGE=javascript>
	window.focus()
	var PrevSel
	var SelectedItemID
	var SelectedNodeID
	var TreeTypeID="<%=TreeTypeID%>";
	var ItemTypeID="<%=ItemTypeID%>";
	var dbkey = "<%=dbkey%>";
	
	
</SCRIPT>

<STYLE>
	A {Text-Decoration: none;}
	.TreeView {color:#000000; font-size:8pt; font-family: Verdana, arial, helvetica, sans-serif}
	A.TreeView:LINK {Text-Decoration: none; color:#4682b4; font-size:8pt; font-family: Verdana, arial, helvetica, sans-serif}
	A.TreeView:VISITED {Text-Decoration: none; color:#4682b4; font-size:8pt; font-family: Verdana, arial, helvetica, sans-serif}
	A.TreeView:HOVER {Text-Decoration: underline; color:#4682b4; font-size:8pt; font-family: Verdana, arial, helvetica, sans-serif}
</STYLE>

<TABLE border=0 width=100%>
	<TR>
		<TD>
			<%if UCase(refreshPath) <> "" then%>
				<A CLASS="Treeview" ID="refresh" HREF="<%=refreshPath%>?<%=Qs%>&GotoNode=0&ClearNodes=1" title="Refresh the contents of the tree">Refresh</A> 
			<%else%>
				<A CLASS="Treeview" ID="refresh" HREF="treeview.asp?<%=Qs%>&ClearNodes=1&GotoNode=0" title="Refresh the contents of the tree">Refresh</A> 
			<%end if%>
			<span id="ItemActions" style="visibility:hidden;position:absolute; left:45px; top:3px">	
				&nbsp;|&nbsp;<A CLASS="Treeview" HREF="#" onclick="PickNode('move_item','<%=qshelp%>');return false;" title="Move the selected item...">Move</a>&nbsp;|&nbsp;<A CLASS="Treeview" HREF="#" onclick="PickNode('copy_item','<%=qshelp%>');return false;" title="Copy the selected item...">Copy</a>&nbsp;|&nbsp;<A CLASS="Treeview" HREF="#" onclick="ManageTree('remove_item','<%=qshelp%>');return false;" title="Remove the selected item...">Remove</a>
			</span>
			<span id="NodeActions" style="visibility:hidden;position:absolute; left:45px; top:3px">
				&nbsp;|&nbsp;<A CLASS="Treeview" HREF="#" onclick="ManageTree('create_node','<%=qshelp%>');return false;" title="Create a new node on the tree...">New</a>&nbsp;|&nbsp;<A CLASS="Treeview" HREF="#" onclick="ManageTree('edit_node','<%=qshelp%>');return false;" title="Edit the selected node...">Edit</a>&nbsp;|&nbsp;<A CLASS="Treeview" HREF="#" onclick="PickNode('move_node','<%=qshelp%>');return false;" title="Move the selected node...">Move</a>&nbsp;|&nbsp;<A CLASS="Treeview" HREF="#" onclick="ManageTree('delete_node','<%=qshelp%>');return false;" title="Delete the selected node...">Delete</a>
			</span>
		</TD>
		
	</TR>
</TABLE>
<br>
<% if QSHelp <> "" then%>
<table border = 0 width = "320"><tr><td><span id="tree_display_text"><%=qsHelp%></span></td></tr><tr><td width = "200"><hr></td></tr><table>
<br>
<%end if%>

<%
' Render the tree

If (not IsObject(Session("TVNodes" & TreeID)) OR ClearNodes = 1) Then
	' DGB Always open top level nodes
	Session("FirstLevelNodeList") = ":"
	Session("SecondLevelNodeList") = ":"
	'Start with an empty tree
	Set Session("TVNodes" & TreeID)= Nothing
	Session("TreeViewOpenNodes" & TreeID) = ""
	Session("PrevExpandedNodesList" & TreeID) = ":"
	'Read the default inventory location
	
	if  bGotoNode then
		Call PopulateNodeLayer("NULL",12)
		Call GetNodes(Clng(GotoNode)) ' Get the tree leading to the requested node
	Else
		sNode = "NULL"
		Call PopulateNodeLayer(sNode,12) ' Add the first three layers of nodes
	End if
	' Save the current nodes collection in session var for reuse
	Set Session("TVNodes" & TreeID)= TreeView.Nodes 
Elseif (ExpNode = "Y") then
	' DGB Always open top level nodes
	Session("FirstLevelNodeList") = ":"
	Session("SecondLevelNodeList") = ":"
	' Fetch the nodes collection from session var
	Set TreeView.Nodes = Session("TVNodes" & TreeID)
	if NOT IsEmpty(RemoveNode) then
		On Error resume next
		'Remove the node from the tree
		TreeView.Nodes.Remove(CStr(RemoveNode))
	End if
	
	if  bGotoNode OR GotoNode = "0" then
		Call GetNodes(CLng(GotoNode)) ' Get the tree leading to the requested node
	Else
		Call PopulateNodeLayer(sNode,2) ' Add the two layers below the selected node
	End if
	'Save the nodes collection in session var for reuse
	Set Session("TVNodes" & TreeID)= TreeView.Nodes 
End if

If IsObject(Session("TVNodes" & TreeID)) then
	Set TreeView.Nodes = Session("TVNodes" & TreeID)
	If TreeView.Nodes.Count > 0 then
		
		TreeView.Nodes(1).EnsureVisible
		' DGB Always open top level nodes
		t_arr = split(Session("SecondLevelNodeList"),":")
		arrub = Ubound(t_arr,1)
		if arrub >= 2 then
			for n = 1 to arrub -1
				on error resume next
				TreeView.Nodes(CStr(t_arr(n))).EnsureVisible
			next 
		End if
		if NOT bDebugPrint then
			on error resume next
			TreeView.Show
			if err.number > 0 then bRefresh = true
		End if
	End if
	
	' Keep track of open nodes as URL string that can be appended when recalling the tree page
	ClickedNode = Request.QueryString("sNode")
	If Not IsEmpty(ClickedNode) then
		if InStr(1,Session("TreeViewOpenNodes" & TreeID), "&node=" & ClickedNode) > 0 then
			Session("TreeViewOpenNodes" & TreeID) = Replace(Session("TreeViewOpenNodes" & TreeID), "&node=" & ClickedNode & "&" , "&")
		Else
			Session("TreeViewOpenNodes" & TreeID) = Session("TreeViewOpenNodes" & TreeID) & "&node=" & ClickedNode
		End if
	End if
	Set TreeView = Nothing
End if

'Open the target node
if bRefresh then
	response.write "<Script language=javascript>"
	Response.Write "var Aelm = document.anchors(""refresh""); "
	Response.Write "if (Aelm){"
	Response.Write "Aelm.click();}</script>"
elseif bGotoNode then
	response.write "<Script language=javascript>"
	Response.Write "var Aelm = document.anchors(""e" & Request.QueryString("Gotonode") & """); "
	Response.Write "if (Aelm){" 
	Response.Write "Aelm.click();}</script>"
End if
%>