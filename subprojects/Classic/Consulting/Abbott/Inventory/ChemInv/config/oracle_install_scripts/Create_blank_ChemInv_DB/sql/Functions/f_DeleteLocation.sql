-- Create procedure/function DELETELOCATION.
CREATE OR REPLACE  FUNCTION "&&SchemaName"."DELETELOCATION"           
(pLocationID in Inv_Locations.Location_ID%Type,
 pRecursively in integer:=0
)
return Inv_Locations.Location_ID%Type
IS
ContainerCount integer;
LocationCount integer;
ParentID inv_Locations.Parent_ID%Type;
cannot_delete_system_location exception;
GridID inv_grid_storage.grid_storage_id%type;
numDeleted integer;
BEGIN
  if pLocationID < 500 then
    RAISE cannot_delete_system_location;
  end if;
  GridID := GetGridID(pLocationID);
  --RETURN GridID;
  if pRecursively = 0 then
    SELECT Count(Container_ID) into ContainerCount FROM Inv_Containers WHERE Location_ID_FK = pLocationID;
    SELECT Count(Location_ID) into LocationCount FROM Inv_Locations WHERE Parent_ID = pLocationID;
    if (ContainerCount + LocationCount + GridID) > 0 then
      --RETURN 'Cannot delete location because of related objects.';
      RETURN -108;
    end if;
  end if;

  -- Get the parentid for the return value
  SELECT parent_id into ParentID FROM inv_Locations Where Location_ID = pLocationID;

  if GridID > 0 then  
    numDeleted := DeleteLocationGrid(GridID);
  else
    -- Deal with orphaned default location values
    -- Deal with orphaned default location values
	if pRecursively = 0 then    
	    UPDATE inv_containers SET Def_Location_ID_FK = Location_ID_FK WHERE Def_Location_ID_FK = pLocationID;
	else
	    UPDATE inv_containers SET Def_Location_ID_FK = Location_ID_FK WHERE Def_Location_ID_FK  in (select location_id from inv_locations start with location_id = pLocationID connect by prior location_id = parent_id);
	end if;
    -- Sublocations and Containers are deleted because of cascade constraint
    DELETE Inv_Locations WHERE Location_ID = pLocationID;
end if;

RETURN ParentID;

exception
WHEN cannot_delete_system_location then
  RETURN -110;
END DeleteLocation;
/
show errors;
