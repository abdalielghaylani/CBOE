CREATE OR REPLACE
PACKAGE BODY TREE
IS

function ItemExists(
    	pItemID in tree_item.item_id%type,
    	pItemTypeID in tree_item.item_type_id%type) return boolean AS

    	tName varchar2(30);
    	pkName varchar2(30);
    	mySql varchar2(2000);
    	rc boolean;
    BEGIN
        select table_name , id_field_name into tName, pkName from tree_item_type where id = pItemTypeID;

    	mySql := 'select 1 from ' || tName || ' where ' || pkName || '= :1';

    	Execute immediate mySql into rc using pItemID;
    	return rc;
   	EXCEPTION
    	when no_data_found then
    		return false;
END ItemExists;

FUNCTION AddItemToNode(
		pNodeID IN tree_item.node_id%type,
		pItemID IN tree_item.item_id%type,
		pItemTypeID IN tree_item.item_type_id%type) RETURN varchar2 AS
	    invalid_item exception;
		out varchar2(255);
	BEGIN


		if not ItemExists(pItemID, pItemTypeID) then
			raise invalid_item;
		end if;

		insert into tree_item 	(id, node_id, item_id, item_type_id)
					values		(BIOSARDB.tree_seq.nextval, pNodeID, pItemID, pItemTypeID)
					returning id into out;
	RETURN out;
	EXCEPTION
		when dup_val_on_index then
			return 'That item is already associated with that node!';
		when constraint_violation then
			return 'Invalid node Id or item type';
		when invalid_item then
			return 'ItemID not found in the related table';
		when others then
			return SQLCODE || ':' || SQLERRM;
END AddItemToNode;

FUNCTION RemoveItemFromTree(
		pItemID IN tree_item.item_id%type,
		pNodeID IN tree_item.node_id%type,
		pItemTypeID IN tree_item.item_type_id%type) return varchar2 AS
		id number:=0;
		item_not_found exception;
	BEGIN
		delete from tree_item where item_id = pItemID and node_id = pNodeID and item_type_id = pItemTypeID returning id into id;
		if id = 0  then
			raise item_not_found;
		end if;

		return id;

  	EXCEPTION
   	    when item_not_found then
   	      	return 'Item does not exists on the tree.'  ;
	   	when others then
		  	return SQLCODE || ':' || SQLERRM;
END	RemoveItemFromTree;


FUNCTION CopyItem(
		pItemID IN tree_item.id%type,
		pTargetNodeID IN tree_item.node_id%type) return varchar2 AS

		newid tree_item.id%type;
	BEGIN

		select BIOSARDB.tree_seq.nextval into newid from dual;

		insert into tree_item t (t.id, t.node_id, t.item_id, t.item_type_id)
						select newid, pTargetNodeID, item_id, item_type_id
							from tree_item
							where id = pItemID;

  		return to_char(newid);
  	EXCEPTION
		when dup_val_on_index then
			return 'A item with the same name already exists.';
	   	when others then
		  	return SQLCODE || ':' || SQLERRM;
END	CopyItem;

Function DeleteItem(
		pItemID IN tree_item.id%type) return varchar2 AS
        nodeID int;
        item_not_found exception;
	BEGIN
		select node_id into nodeID from tree_item where id = pItemID;
		if not sql%found then
			raise item_not_found;
		else
		    delete from tree_item where id = pItemID;
		    return nodeID;
		end if;
	EXCEPTION
		when item_not_found then
			return 'Item to be deleted could not be found.';
END	DeleteItem;

FUNCTION MoveItem(
		pItemID IN tree_item.id%type,
		pTargetNodeID IN tree_item.node_id%type) return varchar2 AS
        rc int;
		id varchar2(255);
	BEGIN
		if pItemID=pTargetNodeID then
			raise_application_error(-20000, 'Item cannot be moved into itself');
		end if;
		id := CopyItem(pItemID, pTargetNodeID);
  		If not id ='A item with the same name already exists.' then
          		rc:= DeleteItem(pItemID);
		end if;
		return id;
  	EXCEPTION
	   	when others then
		  	return SQLERRM;
END	MoveItem;


FUNCTION MoveNode(  pNodeID IN tree_node.id%type,
                        pParentID IN tree_node.parent_id%type) return varchar2 AS
                        isSublocationID tree_node.id%type;

			CURSOR isSubLocation_cur(SourceLocationID in tree_node.id%type, DestLocationID in tree_node.parent_id%type) IS

                          SELECT id FROM tree_node  WHERE  id = DestLocationID AND

                          id IN (SELECT id FROM tree_node WHERE tree_node.parent_id IS NOT NULL CONNECT BY prior tree_node.ID = tree_node.parent_id START WITH tree_node.id= SourceLocationID);



            BEGIN


                        -- Cannot move onto a sublocation
                        OPEN isSublocation_cur(pNodeID, pParentID);
                        FETCH isSublocation_cur into isSublocationID;
                        if (isSublocation_cur%FOUND) then
                                    raise_application_error(-20000, 'A folder cannot be moved into one of its subfolders');
                        end if;
			if pNodeID = pParentID   then
				raise_application_error(-20000, 'A folder cannot be moved into itself');
			end if;

                        update tree_node set parent_id = pParentID
                                    where id = pNodeID;
                        return '1';
            EXCEPTION
                        when others then
                        return SQLERRM;

END     MoveNode;

FUNCTION EditNode(
		pNodeID IN tree_node.id%type,
		pNodeName IN tree_node.node_name%type,
		pNodeDesc IN tree_node.node_description%type) RETURN varchar2 AS

		out varchar2(255);
	BEGIN

	   	update tree_node set node_name = pNodeName, node_description = pNodeDesc
	   	 	where id = pNodeID returning id into out;
		RETURN out;
	EXCEPTION
		when dup_val_on_index then
			return 'A node with the same name already exists';
		when others then
			return SQLCODE || ':' || SQLERRM;
END EditNode;

FUNCTION DeleteNode(
		pNodeID IN tree_node.id%type) RETURN varchar2 AS

	    cannot_delete exception;
	    pragma exception_init(cannot_delete, -2292);
	    node_not_found exception;
		id number:=0;
		parentID number;
	BEGIN
        select parent_id into parentID from tree_node where id = pNodeID;
        if not sql%found then
	   	    raise node_not_found;
	   	else
			if parentID is null then
          			raise_application_error(-20000, 'Cannot delete the root node');
			else

	   			delete from tree_node
	   	 		where id = pNodeID;
			end if;
		end if;

		RETURN parentID;
	EXCEPTION
		when cannot_delete then
			return 'Cannot delete node because it is not empty';
		when node_not_found then
			return 'Node to be deleted could not be found.';
		when others then
			return SQLCODE || ':' || SQLERRM;
END DeleteNode;

FUNCTION AddNodeToTree(
		pParentID IN tree_node.parent_id%type,
		pNodeName IN tree_node.node_name%type,
		pNodeDesc IN tree_node.node_description%type,
		pTreeTypeID IN tree_node.tree_type_id%type) RETURN varchar2 AS
	    invalid_item exception;
		out varchar2(255);
	BEGIN

	   	insert into tree_node 	(id, parent_id, node_name, node_description, tree_type_id)
	   			values		(biosardb.tree_seq.nextval, pParentID, pNodeName, pNodeDesc, pTreeTypeID)
	   			returning id into out;

	RETURN out;
	EXCEPTION
		when dup_val_on_index then
			return 'This node already exists on the tree!';
		when constraint_violation then
			return 'Invalid parent node Id or tree type';
		when invalid_item then
			return 'ItemID not found in the related table';
	   	when others then
			return SQLCODE || ':' || SQLERRM;
END AddNodeToTree;

PROCEDURE GetNode(
		pNodeID in tree_node.id%type,
		O_RS out cursor_type
		) AS

	BEGIN
	    OPEN O_RS FOR
	      Select id, parent_id, node_name, node_description, tree_type_id from tree_node where id = pNodeID;
END GetNode;

PROCEDURE GetItem(
		pItemID in tree_item.id%type,
		pItemTypeID in tree_item.item_type_id%type,
		O_RS out cursor_type
		) AS

		tName varchar2(30);
		idFieldName varchar2(30);
		displayFieldName varchar2(255);
	    itemsSQL varchar2(2000);
	BEGIN

		select table_name, id_field_name, display_field_name
				into tName, idFieldName, displayFieldName
		from tree_item_type where id = pItemTypeID;

		itemsSQL := 'select distinct t.id, t.node_id, t.item_id, (select ' || displayFieldName || ' from ' || tName || ' where ' || idFieldName || '= t.item_id) as item_name
						from tree_item t where t.id= :pItemID';
	    OPEN O_RS FOR
	      itemsSQL using pItemID;
END GetItem;

PROCEDURE GetNodeLayers(
		pStartNodeID in number,
		pLevel in number,
		pTreeTypeID in number,
		O_RS out cursor_type
		) AS

	BEGIN

	-- Here we need to use dynamic sql to support the PL/SQL engine shortcomings in version 8i
	if pStartNodeID = 0 or pStartNodeID is NULL then
			OPEN O_RS FOR
			'SELECT node_name, id, parent_id, node_description
			FROM tree_node
			WHERE tree_type_id = :pTreeTypeID AND level <= :pLevel
			CONNECT BY prior id = parent_id AND level <= :pLevel START WITH id IN (SELECT id from tree_node where parent_id IS NULL)
			ORDER BY level, case 	when length(substr(node_name, instr(node_name, '' '', -1) + 1)) = length(node_name)
										then UPPER(node_name)
									when length(substr(node_name , instr(node_name, '' '', -1) + 1)) = length(translate(UPPER(substr(node_name, instr(node_name, '' '', -1) + 1)), ''0123456789ABCDEFGHIJLKMNOPQRSTUVWXYZ()~`!@#$%^&*-_=+{}[]\|;:<>,./?'', ''0123456789''))
										then UPPER(substr(node_name, 1, instr(node_name, '' '', -1)))
									else
										UPPER(node_name)
									end,
							case 	when length(substr(node_name, instr(node_name, '' '', -1) + 1)) = length(node_name)
										then 1000000
									when length(substr(node_name, instr(node_name, '' '', -1) + 1)) = length(translate(UPPER(substr(node_name, instr(node_name, '' '', -1) + 1)), ''0123456789ABCDEFGHIJLKMNOPQRSTUVWXYZ()~`!@#$%^&*-_=+{}[]\|;:<>,./?'', ''0123456789''))
										then to_number(substr(node_name, instr(node_name, '' '', -1) +1))
									else
										1000000
									end' using pTreeTypeID, pLevel, pLevel;
		else
			OPEN O_RS FOR
		  	'SELECT node_name, id, parent_id, node_description
			FROM tree_node
			WHERE tree_type_id = :pTreeTypeID AND level <= :pLevel
			CONNECT BY prior id = parent_id AND level <= :pLevel START WITH id IN (SELECT id from tree_node where parent_id = :pStartNodeID)
			ORDER BY level, case 	when length(substr(node_name, instr(node_name, '' '', -1) + 1)) = length(node_name)
										then UPPER(node_name)
									when length(substr(node_name , instr(node_name, '' '', -1) + 1)) = length(translate(UPPER(substr(node_name, instr(node_name, '' '', -1) + 1)), ''0123456789ABCDEFGHIJLKMNOPQRSTUVWXYZ()~`!@#$%^&*-_=+{}[]\|;:<>,./?'', ''0123456789''))
										then UPPER(substr(node_name, 1, instr(node_name, '' '', -1)))
									else
										UPPER(node_name)
									end,
							case 	when length(substr(node_name, instr(node_name, '' '', -1) + 1)) = length(node_name)
										then 1000000
									when length(substr(node_name, instr(node_name, '' '', -1) + 1)) = length(translate(UPPER(substr(node_name, instr(node_name, '' '', -1) + 1)), ''0123456789ABCDEFGHIJLKMNOPQRSTUVWXYZ()~`!@#$%^&*-_=+{}[]\|;:<>,./?'', ''0123456789''))
										then to_number(substr(node_name, instr(node_name, '' '', -1) +1))
									else
										1000000
									end' using pTreeTypeID, pLevel, pLevel, pStartNodeID;
		end if;
END GetNodeLayers;

PROCEDURE GetTreeItems(
		pStartNodeID in number,
		pLevel in number,
		pItemTypeID in number,
		pTreeTypeID in number,
		pItemFilterSelect in varchar2:='',
		pCheckboxSelect in varchar2:='',
		O_RS out cursor_type
		) AS

		tName varchar2(30);
		idFieldName varchar2(30);
		displayFieldName varchar2(2000);
	    nodesSQL varchar2(4000);
	    itemsSQL varchar2(4000);
	    itemFilterSQL varchar2(4000);
	    isSelectedSQL varchar2(4000);
	    orderSql varchar2(4000);
	    Type t_items is REF CURSOR;
	    c_items t_items;
	    itemid number;
	BEGIN
	    select table_name, id_field_name, display_field_name
				into tName, idFieldName, displayFieldName
		from tree_item_type where id = pItemTypeID;

		if substr(lower(pItemFilterSelect),1,6)= 'select' then
		   itemFilterSQL := ' AND t.item_id in (' || pItemFilterSelect || ')';
		end if;



		if substr(lower(pCheckboxSelect),1,6)= 'select' then
			--Check that selection list is not empty
			open c_items for pCheckboxSelect;
			fetch c_items into itemid;
		   if  c_items%FOUND then
		   		isSelectedSQL := ', (select 1 from (' ||  pCheckboxSelect  || ') x where x.'|| idFieldName ||'=t.item_id) as ischecked ';
			else
				--checkbox select list is empty so nothing is checked
				isSelectedSQL := ',0 as ischecked ';
			end if;
			close c_items;
		else
			   -- No checkbox select filter is passed so everything is selected
			   isSelectedSQL := ',1 as ischecked ';
		end if;

        orderSql :='case 	when length(substr(item_name, instr(item_name, '' '', -1) + 1)) = length(item_name)
										then UPPER(item_name)
									when length(substr(item_name , instr(item_name, '' '', -1) + 1)) = length(translate(UPPER(substr(item_name, instr(item_name, '' '', -1) + 1)), ''0123456789ABCDEFGHIJLKMNOPQRSTUVWXYZ()~`!@#$%^&*-_=+{}[]\|;:<>,./?'', ''0123456789''))
										then UPPER(substr(item_name, 1, instr(item_name, '' '', -1)))
									else
										UPPER(item_name)
									end,
        			 case 	when length(substr(item_name, instr(item_name, '' '', -1) + 1)) = length(item_name)
								then 1000000
							when length(substr(item_name, instr(item_name, '' '', -1) + 1)) = length(translate(UPPER(substr(item_name, instr(item_name, '' '', -1) + 1)), ''0123456789ABCDEFGHIJLKMNOPQRSTUVWXYZ()~`!@#$%^&*-_=+{}[]\|;:<>,./?'', ''0123456789''))
								then to_number(substr(item_name, instr(item_name, '' '', -1) +1))
							else
								1000000
							end';

		if pStartNodeID is Null then
			nodesSQL := 'select id from tree_node
						WHERE tree_type_id = :pTreeTypeID AND level <= :pLevel
						CONNECT BY prior id = parent_id AND level <= :pLevel
						START WITH id IN (SELECT id from tree_node where parent_id IS NULL)';
            
			-- Fix for blank items in the tree			
			--itemsSQL := 'select distinct t.id, t.node_id, (select ' || displayFieldName || ' from ' || tName || ' where ' || idFieldName || '= t.item_id) as item_name ' ||
			--			 isSelectedSQL	|| ', t.item_id from tree_item t where t.node_id in (' || nodesSQL || ')' || ItemFilterSQL || ' ORDER BY ' || orderSql;
			
			itemsSQL := 'select distinct t.id, t.node_id, i.'|| displayFieldName || ' as item_name ' ||
						 isSelectedSQL	|| ', t.item_id from tree_item t, ' || tName || ' i  where i.'|| idFieldName || '= t.item_id and t.node_id in (' || nodesSQL || ')' || ItemFilterSQL || ' ORDER BY ' || orderSql;
			
			
			OPEN O_RS for itemsSQL using pTreeTypeID, pLevel, pLevel;

		else
		   nodesSQL := 'select id from tree_node
						WHERE tree_type_id = :pTreeTypeID AND level <= :pLevel
						CONNECT BY prior id = parent_id AND level <= :pLevel
						START WITH id IN (SELECT id from tree_node where parent_id = :pStartNodeID)';

			itemsSQL := 'select distinct t.id, t.node_id, (select ' || displayFieldName || ' from ' || tName || ' where ' || idFieldName || '= t.item_id) as item_name ' || isSelectedSQL ||  ', t.item_id from tree_item t where t.node_id in (' || nodesSQL || ')'  || ItemFilterSQL || ' ORDER BY ' || orderSql;
			OPEN O_RS for itemsSQL using pTreeTypeID, pLevel, pLevel, pStartNodeID;
		end if;


END GetTreeItems;

	-- Gets nodes required to open the tree at a particular target node
PROCEDURE GotoNode(
		pTargetNodeID in tree_node.id%type,
		pTreeTypeID in tree_type.id%type,
		O_RS out cursor_type) AS

	BEGIN
	    OPEN O_RS FOR
			SELECT node_name, ID, parent_id
			FROM tree_node
			WHERE tree_type_id = pTreeTypeID
			AND level <=3
			CONNECT BY prior id = parent_id AND level <=3
			START WITH parent_id IN (SELECT  id FROM tree_node CONNECT BY id= prior parent_id START WITH id= pTargetNodeID)
			ORDER BY LEVEL, UPPER(node_name);
END GotoNode;

	-- Gets the ids of all nodes in the tagets node's path
PROCEDURE GetPathIDs(
		pTargetNodeID in tree_node.id%type,
		pTreeTypeID in tree_type.id%type,
		O_RS out cursor_type) AS

	BEGIN
	    OPEN O_RS FOR
			SELECT id FROM tree_node WHERE tree_type_id = pTreeTypeID AND parent_id IS NOT NULL
				CONNECT BY id = prior parent_id START WITH id= pTargetNodeID ORDER BY LEVEL, UPPER(node_name);
END  GetPathIDs;

FUNCTION              GetNodePath
	(pNodeID in tree_node.id%type)
	return varchar2
IS
  CURSOR Nodes_cur(NodeID_in in tree_node.id%type) IS
    SELECT Node_Name
    FROM tree_node
    CONNECT BY id = prior Parent_id
    START WITH id = NodeID_in
    ORDER BY Level DESC;
  path_str varchar2(2000);
  NodeName varchar2(200);
BEGIN
  OPEN Nodes_cur(pNodeID);
  LOOP
    FETCH Nodes_cur INTO NodeName;
    EXIT WHEN Nodes_cur%NOTFOUND;
    path_str := path_str || NodeName || '\';
  END LOOP;
  --dbms_output.put_line(path_str);
  RETURN path_str;
END GetNodePath;

END TREE;
/
