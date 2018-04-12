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
pTempParentID varchar(100);
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
  -- Checking whether the LocationId is a Grid Element of Grid or not 
  -- if it is a Grid Element then delete parent location also
  if isGridLocation(pLocationID) > 0 then   
    pTempParentID := ParentID;
    SELECT parent_id into ParentID FROM inv_Locations Where Location_ID = pTempParentID;    
  end if; 
  
  if GridID > 0 then  
    numDeleted := DeleteLocationGrid(GridID);
    if length(pTempParentID)>0 then
      DELETE Inv_Locations WHERE Location_ID = pTempParentID;
      -- Removing with Default home locations  
      DELETE FROM INV_USER_PROPERTIES WHERE PROPERTYNAME = 'INVDefLoc' AND PROPERTYVALUE = to_char(pTempParentID);
    end if;
  else
    -- Deal with orphaned default location values
    -- Deal with orphaned default location values
	if pRecursively = 0 then    
	    UPDATE inv_containers SET Def_Location_ID_FK = Location_ID_FK WHERE Def_Location_ID_FK = pLocationID;
	    -- Dealing with Default home locations  
        DELETE FROM INV_USER_PROPERTIES WHERE PROPERTYNAME = 'INVDefLoc' AND PROPERTYVALUE = to_char(pLocationID);
	else
	    UPDATE inv_containers SET Def_Location_ID_FK = Location_ID_FK WHERE Def_Location_ID_FK  in (select location_id from inv_locations start with location_id = pLocationID connect by prior location_id = parent_id);
	     -- Dealing with Default home locations  
        DELETE FROM INV_USER_PROPERTIES WHERE PROPERTYNAME = 'INVDefLoc' AND PROPERTYVALUE in (select to_char(location_id) from inv_locations start with location_id = pLocationID connect by prior location_id = parent_id);
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
