-- Create procedure/function MOVELOCATION.
CREATE OR REPLACE  FUNCTION "&&SchemaName"."MOVELOCATION"         
 (pLocationID IN inv_Locations.Location_ID%Type, pParentId IN inv_Locations.Location_ID%Type)
return inv_Locations.Location_ID%Type
IS
parentid_not_found exception;
pragma exception_init (parentid_not_found, -2291);
cannot_move_root exception;
cannot_move_system_location exception;
cannot_move_onto_self exception;
duplicate_name exception;
CURSOR dupName_cur(LocationName_in in Inv_Locations.Location_Name%Type) IS
  SELECT Location_ID FROM Inv_Locations WHERE Location_Name = LocationName_in 
  AND Parent_ID = pParentID;
CURSOR isSubLocation_cur(SourceLocationID in inv_Locations.Location_ID%Type, DestLocationID in inv_Locations.Location_ID%Type) IS
  SELECT Location_Id FROM inv_locations  WHERE  Location_ID = DestLocationID AND 
  Location_ID IN (SELECT Location_id FROM inv_Locations WHERE inv_Locations.Parent_ID IS NOT NULL CONNECT BY prior inv_Locations.Location_ID = inv_Locations.Parent_ID START WITH inv_Locations.Location_ID= SourceLocationID);
isSublocationID Inv_Locations.Location_ID%Type;
dupName_id Inv_Locations.Location_ID%Type;
LocationName Inv_Locations.Location_Name%Type;

BEGIN

if pLocationID = 0 then
  RAISE cannot_move_root;
end if;
if pLocationID < 500 then
  RAISE cannot_move_system_location;
end if;

-- Cannot move onto a sublocation
OPEN isSublocation_cur(pLocationID, pParentID);
FETCH isSublocation_cur into isSublocationID;
if (isSublocation_cur%FOUND) then
  RAISE cannot_move_onto_self;
end if;

-- Check for duplicate name within the new location
SELECT Location_Name INTO LocationName FROM inv_Locations WHERE Location_ID = pLocationID; 
OPEN dupName_cur(LocationName);
FETCH dupName_cur into dupName_id;
if (dupName_cur%FOUND) then 
  RAISE duplicate_name;
end if;
CLOSE dupName_cur;


UPDATE Inv_Locations SET Parent_ID= pParentID WHERE Location_ID= pLocationID;
if sql%notfound then
   --RETURN 'Could not find the location to be moved.';
   RETURN -111;
end if;

RETURN pParentID;

exception
WHEN parentid_not_found then
  --RETURN 'Could not find the destination location.';
  RETURN -104;
WHEN cannot_move_root then
  --RETURN 'The root location cannot be moved.';
  RETURN -113;
WHEN cannot_move_onto_self then
  --RETURN 'Location cannot be moved onto itself or onto one of its sublocations.';
  RETURN -114;
WHEN cannot_move_system_location then
  --RETURN 'This is a system location that cannot be moved.';
  RETURN -115;
WHEN duplicate_name then
	-- RETURN 'A location with same name already exists at this location.';
  RETURN -106;
END MoveLocation;
/
show errors;
