CREATE OR REPLACE  FUNCTION "&&SchemaName"."GETGRIDID"      (
    pLocationid in inv_locations.location_id%type:=Null,
    pParentID in inv_locations.location_id%type:=Null
   )
	return inv_grid_element.grid_storage_id_fk%type
IS
GridID inv_grid_element.grid_storage_id_fk%type;
BEGIN
  
  If pLocationID IS NOT NULL  then
    select grid_storage_id_fk into GridID from inv_grid_element where location_id_fk = pLocationID;	
  Else
    select grid_storage_id into GridID from inv_grid_Storage where location_ID_FK = pParentID;
  End if;
  
  RETURN GridID;
exception
  WHEN NO_DATA_FOUND THEN
  RETURN 0;
end GetGridID;
/
show errors;
