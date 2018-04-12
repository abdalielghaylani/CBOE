///////////////////////////////////////////////////////////////////////////////
// Opens up a dialog box
// Type 1 is standard dialog used for funcitons like Move Container, ChangeQty etc
// Type 2 is the larger dialog used for Create/Edit Container and Substance selector
// Type 3 is the location browser dialog used from the Browse link
// The size and positions of the popups has been optimized to look Ok even at 800 X 600 resolution
function OpenDialog(url, name, type)
{
	WindowDef_1 = "height=530, width= 530px, top=50, left=0";
	WindowDef_2 = "height=580, width= 850px, top=0, left=0";
	WindowDef_3 = "height=450, width= 300px, top=50, left=540";
	WindowDef_4 = "height=450, width= 550px, top=50, left=200";
	WindowDef_5 = "height=600, width= 800px, top=0, left=100";		
	var WindowDef = eval("WindowDef_" + type);
	var attribs = "toolbar=0,location=0,directories=0,status=0,menubar=0,scrollbars,resizable," + WindowDef;
	var DialogWindow = window.open(url,name,attribs);
	return DialogWindow;
}

function EchoNode(id){
	alert ('Node ' + id + ' Selected by user!');
}

function EchoItem(id){
	alert ('Item ' + id + ' Selected by user!');
}

function EchoItemAdded(nodeID, treeItemID){
	alert ('New item ' + treeItemID + ' added to the tree at node ' + nodeID + '!');
}

function EchoItemRemoved(ItemID){
	alert ('item ' + ItemID + ' removed from tree');
}
	
function SelectItem(itemID){
	var ename = "e" +  itemID
	var elm
	elm = document.anchors(ename);
	elm.style.fontWeight = "bold"
	SelectedItemID = itemID;
	SelectedNodeID = 0;
	DeSelectPrevious(elm)
	document.all.NodeActions.style.visibility = "hidden"
	document.all.ItemActions.style.visibility = "visible"
}
	
function SelectNode(nodeID){
	var ename = "e" +  nodeID
	var elm
	elm = document.anchors(ename);
	elm.style.fontWeight = "bold"
	SelectedNodeID = nodeID;
	SelectedItemID = 0;
	DeSelectPrevious(elm);
	document.all.NodeActions.style.visibility = "visible"
	document.all.ItemActions.style.visibility = "hidden"
}
	
function ToggleActions(){
	document.all.NodeActions.style.visibility = "visible"
}
	
function DeSelectPrevious(elm){
	if ((typeof(PrevSel)== "object") && (PrevSel!=elm)){
		PrevSel.style.fontWeight = "normal"
	}
	PrevSel = elm;
}
	
function ManageTree(action){
		
	var url = "tree_dialog.asp?dbname=" + dbkey  +"&action=" + action + "&TreeTypeID=" + TreeTypeID + "&ItemTypeID=" + ItemTypeID + "&SelectedNodeID=" + SelectedNodeID + "&SelectedItemID=" + SelectedItemID;
	OpenDialog(url,'treedialog',3);		

}

function PickNode(action){
	
	var url = "treeview.asp?dbname=" + dbkey  + "&TreeTypeID=" + TreeTypeID + "&TreeID=2&ClearNodes=1&TreeMode=select_node&JsCallback=" + action;
	OpenDialog(url,'nodepicker',3);
}

function move_item(nodeid){
	var url = "tree_action.asp?dbname=" + dbkey  + "&TreeTypeID=" + TreeTypeID + "&action=move_item" + "&NodeID=" + nodeid + "&ItemID=" + SelectedItemID;
	OpenDialog(url,'treeaction',3);
}

function copy_item(nodeid){
	var url = "tree_action.asp?dbname=" + dbkey  + "&TreeTypeID=" + TreeTypeID + "&action=copy_item" + "&NodeID=" + nodeid + "&ItemID=" + SelectedItemID;
	OpenDialog(url,'treeaction',3);
}

function move_node(nodeid){
	var url = "tree_action.asp?dbname=" + dbkey  + "&TreeTypeID=" + TreeTypeID + "&action=move_node" + "&ParentID=" + nodeid + "&NodeID=" + SelectedNodeID;
	OpenDialog(url,'treeaction',3);
}

function RefreshTree(selectedNode){
	// Get the querystring and remove the first character (?)
	var QS = location.search.substring(1);
	// parse name/value pairs by splitting on & and =
	var pairs = QS.split("&");
	var tmpAry;
	var bClearNodes = false;
	var bGotoNode = false;
	var bsNode = false;
	var bExp = false;
	// loop over pairs
	for(var i=0; i < pairs.length; i++){
		tmpAry = pairs[i].split("=");
		aName = tmpAry[0]; // the name
		aValue =tmpAry[1]; // the value for said name
		
		// Replace the ClearNodes
		if (aName.toLowerCase() == "clearnodes"){
			QS = ReplaceToken(QS, pairs[i], aName + "=1")
			bClearNodes = true;
		}
		// Replace the GotoNode
		if (aName.toLowerCase() == "gotonode"){
			QS = ReplaceToken(QS, pairs[i], aName + "=" + selectedNode)
			bGotoNode = true;
		}
		// Replace the sNode
		if (aName.toLowerCase() == "snode"){
			QS = ReplaceToken(QS, pairs[i], aName + "=" + selectedNode)
			bsNode = true;
		}
		// Replace the EXP=Y
		if (aName.toLowerCase() == "exp"){
			QS = ReplaceToken(QS, pairs[i], aName + "=Y#" + selectedNode )
			bExp = true;
		}
	}
	// Add if not found
	if (!bClearNodes) QS = "ClearNodes=1&" + QS;
	if (!bGotoNode) QS = "GotoNode=" + selectedNode + "&" + QS;
	if (!bsNode) QS = "sNode=" + selectedNode + "&" + QS;
	if (!bExp) QS += "&EXP=Y#" + selectedNode;
	
	var url = location.pathname + "?" + QS;
	var currentUrl = location.pathname + location.search + location.hash; 
	// Repoint to new url
	location.href = url;
	// reload with the new url
	if (currentUrl == url) location.reload();
}

function ReplaceToken(s, t, u) {
  /*
  **  Replace a token in a string
  **    s  string to be processed
  **    t  token to be found and removed
  **    u  token to be inserted
  **  returns new String
  */
  var i = s.indexOf(t);
  r = "";
  if (i == -1) return s;
  r += s.substring(0,i) + u;
  if ( i + t.length < s.length)
    r += ReplaceToken(s.substring(i + t.length, s.length), t, u);
  return r;
  }

