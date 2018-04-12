<!--#INCLUDE VIRTUAL = "/cfserverasp/source/ado.inc"-->
<%
'****************************************************************************************
'*	PURPOSE:  Adds a new layer of nodes to the tree each time a plus is checked                                              *
'*  INPUT: 	  The node to add children to, pLevel <=2 for children and grandchildren,
'*            =2 for grandchildren   
'*	OUTPUT:   Adds nodes to a preexisting tree                                          
'****************************************************************************************
Sub PopulateNodeLayer(pNodeID, pLevel)	
	Dim LayerSQL
	Dim bDebugPrint
	bDebugPrint = false
	Dim i
	
	'check if there are different select statements for the different tree types
	'if there are select statements for each tree type, set up the appropriate variables.
	if instr(ItemFilterSelect, "::CS::") > 0 then
		ItemFilterSelect_array = split(ItemFilterSelect, "::CS::", -1)
	end if
	For i = 0 to Ubound(TreeType_ID_arr,1)
		TreeTypeID = TreeType_ID_arr(i)
		if isArray(ItemFilterSelect_array) then
			ItemFilterSelect = ItemFilterSelect_array(i)
		end if
		
		on error resume next
		' Check the list of previously expanded nodes to avoid adding preexisting nodes to tree
		if InStr(1,Session("PrevExpandedNodesList" & TreeID),":" & pNodeID & ":") = 0 Then
			if pNodeID = "NULL" then
				pNID = Null
				pNID2 = 0
			Else 
				pNID = CLng(pNodeID)
			End if
			dbkey = Application("appkey")
			
			Call GetDBCommand ("Tree.GetNodeLayers", adCmdText)
			cmd.CommandText = "{Call BioSARDB.Tree.GetNodeLayers(?,?,?)}"
			' cmd.ActiveConnection.execute("alter session set events '10046 trace name context forever, level 12'")	
			cmd.Parameters.Append cmd.CreateParameter("pStartNodeID", adNumeric, adParamInput, 0, pNID2)
			cmd.Parameters.Append cmd.CreateParameter("pLevel", adNumeric, adParamInput, 0, Clng(pLevel))
			cmd.Parameters.Append cmd.CreateParameter("pTreeTypeID", adNumeric, adParamInput, 0, Clng(TreeTypeID))
						
			cmd.Properties ("PLSQLRSet") = true
			Set RS = cmd.Execute
			cmd.Properties ("PLSQLRSet") = false
			if err then 'try this again to work past starnge 9.2.0.4 oledb problem.
				if Instr(1,err.Description,"ORA-06550")> 0 then
					' Recompile and re-execute the procedure
					err.Clear
					on error resume next
					Set sConn = SchemaConnection("BIOSARDB")
					sConn.execute("alter package tree compile")
					sConn.execute("alter package tree compile body")
					cmd.Properties ("PLSQLRSet") = true
					Set RS = cmd.Execute
					cmd.Properties ("PLSQLRSet") = false
					
					if err then
						if Instr(1,err.Description,"ORA-06550")> 0 then
							Response.Write "<BR>Please click above to refresh the tree data<BR>"
							err.Clear
						else
							Response.Write err.Description
							Response.End
						end if	
					end if
				else 			
					Response.Write err.Description
					Response.end
				end if	
			end if
			Call BuildTree(RS)
			if bShowItems then Call AddItemsToTree(pNID, pLevel)
			if (ExpNode = "Y") then Session("PrevExpandedNodesList" & TreeID) = Session("PrevExpandedNodesList" & TreeID)& pNodeID & ":" 
		End if
	Next	
End Sub

Sub LogAction(ByVal inputstr)
		on error goto 0
		filepath = Application("ServerDrive") & "\" & Application("ServerRoot") & "\treelog.txt"
		Set fs = Server.CreateObject("Scripting.FileSystemObject")
		Set a = fs.OpenTextFile(filepath, 8, True)  
		a.WriteLine Now & ": " & inputstr
		a.WriteLine " "
		a.close
End Sub

Sub AddItemsToTree(pNID, pLevel)


	Dim bDebugPrint, RS
	bDebugPrint = false
	Call GetDBCommand ("Tree.GetTreeItems", adCmdStoredProc)
		Cmd.Parameters.Append Cmd.CreateParameter("pStartNodeID", adNumeric, adParamInput, 0, pNID)
		Cmd.Parameters.Append Cmd.CreateParameter("pLevel", adNumeric, adParamInput, 0, pLevel)
		Cmd.Parameters.Append Cmd.CreateParameter("pItemTypeID", adNumeric, adParamInput, 0, ItemTypeID)
		Cmd.Parameters.Append Cmd.CreateParameter("pTreeTypeID", adNumeric, adParamInput, 0, TreeTypeID)
		Cmd.Parameters.Append Cmd.CreateParameter("pItemFilerSelect", advarchar, adParamInput, len(ItemFilterSelect)+1, ItemFilterSelect)
		Cmd.Parameters.Append Cmd.CreateParameter("pCheckboxSelect", advarchar, adParamInput, len(CheckboxSelect)+1, CheckboxSelect)
	Cmd.Properties ("PLSQLRSet") = true
	Set RS = Cmd.Execute
	Cmd.Properties ("PLSQLRSet") = false
	
	i = 0
	Do While Not RS.EOF
		i = i + 1
		id = RS("id")
		node_id = RS("node_id")
		item_name = RS("item_name")
		item_id = RS("item_id")
		item_desc = ""
		if  isEmpty(RS("ischecked").value)or isNull(RS("ischecked").value) then
			isCheckedTemp = 0
		else
			isCheckedTemp = RS("ischecked").value
		end if
		
		isChecked = CBool(isCheckedTemp)
			
		if bDebugPrint then
			Response.Write "<BR>" & i & ". " & id & "-" & node_id & "-" & item_name & "<BR>"
		End if
			on error resume next			
			' Add a Node to the Tree.
			' Syntax: TREEVIEW.NODES.ADD ([Relative],[Relationship],[Key],[Text],[Image],[ExpandedImage],[URL],[Target],[ToolTipText])
			if ItemCheckbox <> "" then
				
				checked = ""
				if isChecked then checked = "checked" 
				'LJB 3/2005 add ability to name checkboxes with itemid and to add a jscallback to track the state of the checkboxes
				if UCase(ItemCheckbox)= "USERIDS" then
					if request("jscallback")<> "" then
						ItemText = "<input type=checkbox " &  checked  & " name=""" & Item_id & """ value=""" & "checked" & """ onClick=""" & request("jscallback") & """ >" & item_name
					else
						ItemText = "<input type=checkbox " &  checked  & " name=""" & Item_id & """ value=""" & "checked" & """ >" & item_name
					end if
				else
					ItemText = "<input type=checkbox " &  checked  & " name=""" & ItemCheckbox & """ value=""" & item_id & """ >" & item_name
				end if
			else
				ItemText = item_name
			End if	
			Set Nodex = TreeView.Nodes.Add(node_id,4, id, ItemText , , , , , "")			
			' DGB open the top level nodes
			' If the node's parent is a first level node then make sure the node is visible
			if InStr(1,Session("firstLevelNodeList"), ":" & node_id & ":") then Session("SecondLevelNodeList") = Session("SecondLevelNodeList") & Nodex.Key & ":" 	
			
			ItemDHTML = ""
			
			Select Case TreeMode	
				Case "select_item"
					ItemURL = "#"
					ItemDHTML = "opener." & JsCallback & "(" & Nodex.Key & ",'" & Server.URLEncode(Nodex.Text) & "');window.close();"
				Case "select_node"
					ItemURL = ""
				Case "add_item", "move_item"
					ItemURL = ""
				Case "manage_tree"
					ItemURL = ""
					ItemDHTML = "SelectItem(" & Nodex.Key & ");"
				Case "open_item"
					NodeURL = ""
				Case "open_node"
					ItemURL = ""
			End Select

				
			if ItemURL <> "" then
				vItemID = "ItemID"
				vItemName = "ItemName" 
				if ItemQSid <> "" then vItemID = ItemQSid
				if ItemQSName <> "" then vItemName = ItemQSName
				if Request.QueryString("QSHelp") <> "" then
					addQSHelp = "&QSHelp=" & Request.QueryString("QSHelp")
				else
					addQSHelp = ""
				end if
				Nodex.URL = AppendToURL(ItemURL, vItemID & "=" & item_id & "&" & vItemName & "=" & Server.URLEncode(Nodex.Text) & "&dbname=" & dbkey & addqshelp)
				
				
				if ItemTarget <> "" then Nodex.Target = ItemTarget	
			End if
			
			if ItemDHTML <> "" then
				Nodex.DHTML = " id=""e" & Nodex.Key & """ onclick=""" & ItemDHTML & """"	
			End if			
			
			if TreeMode = "manage_tree" then
				Nodex.DHTML = " id=""e" & Nodex.Key & """ onclick=SelectItem(" & Nodex.Key & ");"
			End if
			
		Set Nodex = nothing
		RS.MoveNext 
	Loop
	RS.Close
	Set RS = Nothing
	Set Cmd = Nothing
	Conn.Close
	Set Conn = Nothing 
End sub

Sub BuildTree(RS)
		Dim bDebugPrint
		bDebugPrint = false
		Dim i
		i = 0
	
		Do While Not RS.EOF
	
			i = i + 1
			id = RS("id")
			ParentID = RS("Parent_ID")
			bShowPlates = 0
			
			'DGB open the top level nodes
			if IsNull(ParentID) then Session("firstLevelNodeList") =  Session("firstLevelNodeList") & id & ":" 
			
			if bDebugPrint then
				Response.Write "<BR>" & i & ". " & id & "-" & ParentID & "-" & RS("node_name") & "<BR>"
			End if
			on error resume next			
			' Add a Node to the Tree.
			' Syntax: TREEVIEW.NODES.ADD ([Relative],[Relationship],[Key],[Text],[Image],[ExpandedImage],[URL],[Target],[ToolTipText])
				
			Set Nodex = TreeView.Nodes.Add(ParentID,4,id, Left(RS("node_name"),200) , Icon_clsd, Icon_open, , , "")			
			' DGB open the top level nodes
			' If the node's parent is a first level node then make sure the node is visible
			if InStr(1,Session("firstLevelNodeList"), ":" & ParentID & ":") then Session("SecondLevelNodeList") = Session("SecondLevelNodeList") & Nodex.Key & ":" 	
			NodeDHTML = ""	
			Select Case TreeMode	
				Case "select_item"
					NodeURL = ""	
				Case "select_node"
					NodeURL = "#"
					NodeDHTML = "opener." & JsCallback & "(" & Nodex.Key & ",'" & Server.URLEncode(Nodex.Text) & "');window.close();"
				Case "add_item","move_item"
					if Request.QueryString("QSHelp") <> "" then
						addQSHelp = "&QSHelp=" & Request.QueryString("QSHelp")
					else
						addQSHelp = ""
					end if
					NodeURL = "tree_action.asp?ItemTypeID=" & ItemTypeID & "&action=" & TreeMode & "&ItemID=" & ItemID & addQSHelp
					if JsCallback <> "" then NodeURL = NodeURL & "&JsCallback=" & JsCallback  
				Case "manage_tree"
					NodeURL = ""
					NodeDHTML = "SelectNode(" & Nodex.Key & ");"
				Case "open_item"
					NodeURL = ""
				Case "open_node"
					ItemURL = ""	
			End Select
				
			if NodeURL <> "" then
				vNodeID = "NodeID"
				vNodeName = "NodeName" 
				if NodeQSid <> "" then vNodeID = NodeQSid
				if NodeQSName <> "" then vNodeName = NodeQSName 
				Nodex.URL = AppendToURL(NodeURL, vNodeID & "=" & Nodex.Key & "&" & vnodeName & "=" & Server.URLEncode(Nodex.Text) & "&dbname=" & dbkey)		
				if NodeTarget <> "" then Nodex.Target = NodeTarget	
			End if
			
			if NodeDHTML <> "" then
				Nodex.DHTML = " id=""e" & Nodex.Key & """ onclick=""" & NodeDHTML & """"	
			End if	
					
			Set Nodex = nothing
			RS.MoveNext 
		Loop
		RS.Close
		Set RS = Nothing
		Set Cmd = Nothing
		Conn.Close
		Set Conn = Nothing 
End Sub

Sub GetNodes(pNodeID)
	
	For i = 0 to Ubound(TreeType_ID_arr,1)
		TreeTypeID = TreeType_ID_arr(i)
		if InStr(1,Session("PrevExpandedNodesList" & TreeID),":" & pNodeID & ":") = 0 Then
			Dim PathSQL
			Dim NodesSQL
			Call GetDBCommand ("Tree.GetPathIDs", adCmdStoredProc)
			Cmd.Parameters.Append Cmd.CreateParameter("pTargetNodeID", adNumeric, adParamInput, 0, pNodeID)
			Cmd.Parameters.Append Cmd.CreateParameter("pTreeTypeID", adNumeric, adParamInput, 0, TreeTypeID)
			Cmd.Properties ("PLSQLRSet") = true
			Set RS = Cmd.Execute
			Cmd.Properties ("PLSQLRSet") = false
			
			'if RS.EOF AND RS.BOF then
				' The requested node does not exist so reload at the root instead.
				'Response.Write "<SC" & "RIPT language=Javascript>document.location.href='BrowseTree.asp?ClearNodes=1&TreeID=1&NodeTarget=ListFrame&NodeURL=BuildList.asp&sNode=0'</sc" & "ript>"
			'	Response.Write "GetNodes Error:  The requested Node does not exist (" & pNodeID & ")" 
			'	Response.End
			'Else
				Do While NOt RS.EOF
					Session("TreeViewOpenNodes" & TreeID) = Session("TreeViewOpenNodes" & TreeID) & "&node=" & RS("ID") 
					Session("PrevExpandedNodesList" & TreeID) = Session("PrevExpandedNodesList" & TreeID)& RS("ID") & ":"
					RS.MoveNext 
				Loop
				RS.Close
				Set RS = Nothing
				Call GetDBCommand ("Tree.GotoNode", adCmdStoredProc)
				Cmd.Parameters.Append Cmd.CreateParameter("pTargetNodeID", adNumeric, adParamInput, 0, pNodeID)
				Cmd.Parameters.Append Cmd.CreateParameter("pTreeTypeID", adNumeric, adParamInput, 0, TreeTypeID)
				Cmd.Properties ("PLSQLRSet") = true
				Set RS = Cmd.Execute
				Cmd.Properties ("PLSQLRSet") = false
				
				Call BuildTree(RS)
			'end if		
		end if
	Next
End Sub

Sub TreeError(msg)
	Response.Clear
	Response.Write "Error:TreeView:<BR>" & msg
	Response.End
End sub

function AppendToURL(url, str)
	'LJB added detection of escaped "?"
	'if Instr(1,url, "?") > 0  or Instr(url, "%3F") > 0 then 
	if Instr(1,url, "?") > 0 then
		AppendToURL = url & "&" & str
	else
		AppendToURL = url & "?" & str
	End if	
End Function

Sub GetDBConnection()
	Set Conn = Server.CreateObject("ADODB.Connection")
	'Connection string is now set during authentication in cs_security_login_utils.asp
	connStr = Session("UserBaseConnectionStr")
	Conn.Open(connStr)
End Sub

Sub GetDBCommand(pCommandName, pCommandTypeEnum)
	Dim schemaName
	' Open the connection
	Call GetDBConnection()
	' Create the command
	Set Cmd = Server.CreateObject("ADODB.Command")
	Set Cmd.ActiveConnection = Conn
	
	schemaName = Application(dbkey & "_USERNAME")
	'super kludget so I can get this into testing.
	
	if schemaName = "" then
		Response.Write "Error while getting schema name in GetDBCommand"
		Response.End
	Else	
		Cmd.CommandText = schemaName & "." & pCommandName
	End if
	Cmd.CommandType = pCommandTypeEnum
End sub

function AddItemToTree(NodeID, ItemID, ItemTypeID)
	Call GetDBCommand ("Tree.AddItemToNode", adCmdStoredProc)
	Cmd.Parameters.Append Cmd.CreateParameter("RETURN_VALUE",adVarchar, adParamReturnValue, 255, NULL)
	Cmd.Parameters.Append Cmd.CreateParameter("pNodeID", adNumeric, adParamInput, 0, NodeID)
	Cmd.Parameters.Append Cmd.CreateParameter("pItemID", adNumeric, adParamInput, 0, ItemID)
	Cmd.Parameters.Append Cmd.CreateParameter("pItemTypeID", adNumeric, adParamInput, 0, ItemTypeID)
	Cmd.Execute
	AddItemToTree = Cmd.Parameters(0).Value
	Set Cmd = Nothing
	Conn.Close
	Set Conn = Nothing 
End function

function MoveItem(ItemID, TargetNodeID)
	Call GetDBCommand ("Tree.MoveItem", adCmdStoredProc)
	Cmd.Parameters.Append Cmd.CreateParameter("RETURN_VALUE",adVarchar, adParamReturnValue, 255, NULL)
	Cmd.Parameters.Append Cmd.CreateParameter("pItemID", adNumeric, adParamInput, 0, ItemID)
	Cmd.Parameters.Append Cmd.CreateParameter("pNodeID", adNumeric, adParamInput, 0, TargetNodeID)

	Cmd.Execute
	MoveItem = Cmd.Parameters(0).Value
	Set Cmd = Nothing
	Conn.Close
	Set Conn = Nothing 
End function

function CopyItem(ItemID, TargetNodeID)
	Call GetDBCommand ("Tree.CopyItem", adCmdStoredProc)
	Cmd.Parameters.Append Cmd.CreateParameter("RETURN_VALUE",adVarchar, adParamReturnValue, 255, NULL)
	Cmd.Parameters.Append Cmd.CreateParameter("pItemID", adNumeric, adParamInput, 0, ItemID)
	Cmd.Parameters.Append Cmd.CreateParameter("pNodeID", adNumeric, adParamInput, 0, TargetNodeID)

	Cmd.Execute
	CopyItem = Cmd.Parameters(0).Value
	Set Cmd = Nothing
	Conn.Close
	Set Conn = Nothing 
End function

function RemoveItemFromTree(ItemID, NodeID, ItemTypeID)
	Call GetDBCommand ("Tree.RemoveItemFromTree", adCmdStoredProc)
	Cmd.Parameters.Append Cmd.CreateParameter("RETURN_VALUE",adVarchar, adParamReturnValue, 255, NULL)
	Cmd.Parameters.Append Cmd.CreateParameter("pItemID", adNumeric, adParamInput, 0, Clng(ItemID))
	Cmd.Parameters.Append Cmd.CreateParameter("pNodeID", adNumeric, adParamInput, 0, Clng(NodeID))
	Cmd.Parameters.Append Cmd.CreateParameter("pItemTypeID", adNumeric, adParamInput, 0, Clng(ItemTypeID))

	Cmd.Execute
	RemoveItemFromTree = Cmd.Parameters(0).Value
	Set Cmd = Nothing
	Conn.Close
	Set Conn = Nothing  
End function

function MakeSureNotLastPublicTreeItem(ItemID)
	Call GetDBConnection()
	Set Cmd = Server.CreateObject("ADODB.Command")
	Cmd.ActiveConnection = Conn
	Cmd.CommandType = adCmdText
	Cmd.CommandText = "select  Count(*) as theCount from BIOSARDB.TREE_ITEM inner join BIOSARDB.TREE_node on BIOSARDB.TREE_ITEM.node_id = BIOSARDB.TREE_node.id inner join biosardb.tree_type on BIOSARDB.TREE_node.tree_type_id = BIOSARDB.TREE_TYPE.id where  BIOSARDB.TREE_ITEM.item_id=? and tree_type_id=?"
	Cmd.Parameters.Append Cmd.CreateParameter("pformgroup_id", 5, 1, 0, ItemID) 
	Cmd.Parameters.Append Cmd.CreateParameter("ptype_id", 5, 1, 0, Application("PUBLIC_CATEGORY_TREE_NODE_ROOT_ID")) '
	'Cmd.Parameters.Append Cmd.CreateParameter("pPublicNodeID", 5, 1, 0, "2") 
	Set rs = Cmd.Execute
	MakeSureNotLastPublicTreeItem = CLng(rs("thecount"))
	Set rs = Nothing
	Set Cmd = Nothing
	Conn.Close
	Set Conn = Nothing  
end function

function GetParentID(ItemID,NodeID)
	Call GetDBConnection()
	Set Cmd = Server.CreateObject("ADODB.Command")
	Cmd.ActiveConnection = Conn
	Cmd.CommandType = adCmdText
	Cmd.CommandText = "select BIOSARDB.TREE_node.parent_id as parentid from BIOSARDB.TREE_ITEM inner join BIOSARDB.TREE_node on BIOSARDB.TREE_ITEM.node_id = BIOSARDB.TREE_node.id inner join biosardb.tree_type on BIOSARDB.TREE_node.tree_type_id = BIOSARDB.TREE_TYPE.id where  BIOSARDB.TREE_ITEM.item_id=? and  BIOSARDB.TREE_ITEM.node_id=?"
	Cmd.Parameters.Append Cmd.CreateParameter("pformgroup_id", 5, 1, 0, ItemID) 
	Cmd.Parameters.Append Cmd.CreateParameter("node_id", 5, 1, 0, NodeID) '
	'Cmd.Parameters.Append Cmd.CreateParameter("pPublicNodeID", 5, 1, 0, "2") 
	Set rs = Cmd.Execute
	if IsNull(rs("parentid")) or rs("parentid") = "" then
		parentid = 0
	else
		parentid = clng(rs("parentid") )
	end if
	if (parentid = clng(Application("PUBLIC_CATEGORY_TREE_NODE_ROOT_ID"))) or (Clng(NodeID) = clng(Application("PUBLIC_CATEGORY_TREE_NODE_ROOT_ID"))) then
		thereturn = clng(Application("PUBLIC_CATEGORY_TREE_NODE_ROOT_ID"))
	end if
	GetParentID = thereturn
	Set rs = Nothing
	Set Cmd = Nothing
	Conn.Close
	Set Conn = Nothing  
end function

function AddNodetoTree(ParentID, NodeName, NodeDesc, TreeTypeID)
	NodeName=removeIllegalChars2(NodeName)
	NodeDesc=removeIllegalChars2(NodeDesc)
	Call GetDBCommand ("Tree.AddNodeToTree", adCmdStoredProc)
	Cmd.Parameters.Append Cmd.CreateParameter("RETURN_VALUE",adVarchar, adParamReturnValue, 255, NULL)
	Cmd.Parameters.Append Cmd.CreateParameter("pParentID", adNumeric, adParamInput, 0, ParentID)
	Cmd.Parameters.Append Cmd.CreateParameter("pNodeName", adVarchar, adParamInput, len(NodeName)+1, NodeName)
	Cmd.Parameters.Append Cmd.CreateParameter("pNodeDesc", adVarchar, adParamInput, len(NodeDesc)+1, NodeDesc)
	Cmd.Parameters.Append Cmd.CreateParameter("pTreeTypeID", adNumeric, adParamInput, 0, TreeTypeID)
	Cmd.Execute
	AddNodetoTree = Cmd.Parameters(0).Value
	Set Cmd = Nothing
	Conn.Close
	Set Conn = Nothing  
End function

function EditNode(NodeID, NodeName, NodeDesc)
	NodeName=removeIllegalChars2(NodeName)
	NodeDesc=removeIllegalChars2(NodeDesc)
	Call GetDBCommand ("Tree.EditNode", adCmdStoredProc)
	Cmd.Parameters.Append Cmd.CreateParameter("RETURN_VALUE",adVarchar, adParamReturnValue, 255, NULL)
	Cmd.Parameters.Append Cmd.CreateParameter("pNodeID", adNumeric, adParamInput, 0, NodeID)
	Cmd.Parameters.Append Cmd.CreateParameter("pNodeName", adVarchar, adParamInput, len(NodeName)+1, NodeName)
	Cmd.Parameters.Append Cmd.CreateParameter("pNodeDesc", adVarchar, adParamInput, len(NodeDesc)+1, NodeDesc)
	Cmd.Execute
	EditNode = Cmd.Parameters(0).Value
	Set Cmd = Nothing
	Conn.Close
	Set Conn = Nothing  
End function

function DeleteNode(NodeID)
		Call GetDBCommand ("Tree.DeleteNode", adCmdStoredProc)
		Cmd.Parameters.Append Cmd.CreateParameter("RETURN_VALUE",adVarchar, adParamReturnValue, 255, NULL)
		Cmd.Parameters.Append Cmd.CreateParameter("pNodeID", adNumeric, adParamInput, 0, NodeID)
		Cmd.Execute
		DeleteNode = Cmd.Parameters(0).Value
		Set Cmd = Nothing
		Conn.Close
		Set Conn = Nothing  
End function

function MoveNode(NodeID, ParentID)
	Call GetDBCommand ("Tree.MoveNode", adCmdStoredProc)
	Cmd.Parameters.Append Cmd.CreateParameter("RETURN_VALUE",adVarchar, adParamReturnValue, 255, NULL)
	Cmd.Parameters.Append Cmd.CreateParameter("pNodeID", adNumeric, adParamInput, 0, NodeID)
	Cmd.Parameters.Append Cmd.CreateParameter("pParentID", adNumeric, adParamInput, 0, ParentID)

	Cmd.Execute
	MoveNode = Cmd.Parameters(0).Value
	Set Cmd = Nothing
	Conn.Close
	Set Conn = Nothing 
End function

function GetNode(NodeID)
	Call GetDBCommand ("Tree.GetNode", adCmdStoredProc)
	Cmd.Parameters.Append Cmd.CreateParameter("pNodeID", adNumeric, adParamInput, 0, NodeID)
	Cmd.Properties ("PLSQLRSet") = true
	Set GetNode = Cmd.Execute
	Cmd.Properties ("PLSQLRSet") = false  
End function

function GetItem(ItemID, ItemTypeID)
	Call GetDBCommand ("Tree.GetItem", adCmdStoredProc)
	Cmd.Parameters.Append Cmd.CreateParameter("pItemID", adNumeric, adParamInput, 0, ItemID)
	Cmd.Parameters.Append Cmd.CreateParameter("pItemTypeID", adNumeric, adParamInput, 0, Clng(ItemTypeID))

	Cmd.Properties ("PLSQLRSet") = true
	Set GetItem = Cmd.Execute
	Cmd.Properties ("PLSQLRSet") = false  
End function

Function removeIllegalChars2(vInput)
	
	temp = Application("illegalFormCharcters")
	for i = 0 to UBound(temp)
		vInput=replace(vInput, temp(i), "")
	next
	removeIllegalChars2 = vInput
End Function

%>