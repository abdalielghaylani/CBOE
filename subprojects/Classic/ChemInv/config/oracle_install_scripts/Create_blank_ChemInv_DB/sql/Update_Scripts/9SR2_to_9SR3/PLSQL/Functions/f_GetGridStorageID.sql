CREATE OR REPLACE  FUNCTION "&&SchemaName"."GETGRIDSTORAGEID"        (
    pLocationid in inv_locations.location_id%type:=Null,
    pParentID in inv_locations.location_id%type:=Null
   )
	return inv_grid_element.grid_storage_id_fk%type
IS
GridStorageID inv_grid_element.grid_storage_id_fk%type;
BEGIN
  
  If pLocationID IS NOT NULL  then
    select grid_storage_id_fk into GridStorageID from inv_grid_element where location_id_fk = pLocationID;	
  Else
    select grid_storage_id into GridStorageID from inv_grid_Storage where location_ID_FK = pParentID;
  End if;
  
  RETURN GridStorageID;
exception
  WHEN NO_DATA_FOUND THEN
  RETURN 0;
end GetGridStorageID;
/
show errors;
