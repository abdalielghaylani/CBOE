CREATE OR REPLACE
PACKAGE TREE AUTHID CURRENT_USER
AS

	TYPE  CURSOR_TYPE IS REF CURSOR;
	
	constraint_violation exception;
 	pragma exception_init(constraint_violation, -2291);
	
	-- This package is used to manage hierarchies stored in TREE_NODE and TREE_ITEM tables and
	-- to support the Tree GUI pages

	function ItemExists(
    	pItemID in tree_item.item_id%type,
    	pItemTypeID in tree_item.item_type_id%type) return boolean;
	
	FUNCTION AddItemToNode(
		pNodeID IN tree_item.node_id%type,
		pItemID IN tree_item.item_id%type,
		pItemTypeID IN tree_item.item_type_id%type) return varchar2;
	
	FUNCTION RemoveItemFromTree(
		pItemID IN tree_item.item_id%type,
		pNodeID IN tree_item.node_id%type,
		pItemTypeID IN tree_item.item_type_id%type) return varchar2;
	
	FUNCTION AddNodeToTree(
		pParentID IN tree_node.parent_id%type,
		pNodeName IN tree_node.node_name%type,
		pNodeDesc IN tree_node.node_description%type,
		pTreeTypeID IN tree_node.tree_type_id%type) return varchar2;
	
	FUNCTION EditNode(
		pNodeID IN tree_node.id%type,
		pNodeName IN tree_node.node_name%type,
		pNodeDesc IN tree_node.node_description%type) return varchar2;
	
	FUNCTION DeleteNode(
		pNodeID IN tree_node.id%type) return varchar2;
	
	FUNCTION MoveNode(
		pNodeID IN tree_node.id%type,
		pParentID IN tree_node.parent_id%type) return varchar2;
		
	PROCEDURE GetNodeLayers(
		pStartNodeID in number,
		pLevel in number,
		pTreeTypeID in number,
		O_RS out cursor_type);

	PROCEDURE GetTreeItems(
			pStartNodeID in number,
		pLevel in number,
		pItemTypeID in number,
		pTreeTypeID in number,
		pItemFilterSelect in varchar2:='',
		pCheckboxSelect in varchar2:='',
		O_RS out cursor_type);
	
	FUNCTION CopyItem(
		pItemID IN tree_item.id%type,
		pTargetNodeID IN tree_item.node_id%type) return varchar2;
		
	FUNCTION DeleteItem(
		pItemID IN tree_item.id%type) return varchar2;
		
	FUNCTION MoveItem(
		pItemID IN tree_item.id%type,
		pTargetNodeID IN tree_item.node_id%type) return varchar2;	
	
	PROCEDURE GetNode(
		pNodeID in tree_node.id%type, 
		O_RS out cursor_type);
	
	PROCEDURE GetItem(
		pItemID in tree_item.id%type,
 		pItemTypeID in tree_item.item_type_id%type,  
		O_RS out cursor_type);
		
	PROCEDURE GotoNode(
		pTargetNodeID in tree_node.id%type, 
		pTreeTypeID in tree_type.id%type,
		O_RS out cursor_type);
		
	PROCEDURE GetPathIDs(
		pTargetNodeID in tree_node.id%type, 
		pTreeTypeID in tree_type.id%type,
		O_RS out cursor_type);
		
	FUNCTION GetNodePath(pNodeID in tree_node.id%type) return varchar2;		

END TREE;
/
