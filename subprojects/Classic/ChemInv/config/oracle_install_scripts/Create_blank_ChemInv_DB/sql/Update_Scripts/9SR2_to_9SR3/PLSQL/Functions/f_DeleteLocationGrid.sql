CREATE OR REPLACE  FUNCTION "&&SchemaName"."DELETELOCATIONGRID"
    (pGridID in inv_grid_storage.grid_storage_id%type)
return integer
IS
	numLocationsDeleted integer;
BEGIN
  -- Deal with orphaned default location values
  UPDATE inv_containers SET Def_Location_ID_FK = Location_ID_FK WHERE Def_Location_ID_FK IN (SELECT Location_ID_FK FROM inv_Grid_element WHERE Grid_Storage_ID_FK = pGridID);
  DELETE FROM inv_locations WHERE Location_ID IN (SELECT Location_ID_FK FROM inv_Grid_element WHERE Grid_Storage_ID_FK = pGridID);
  numLocationsDeleted := sql%rowcount;
  DELETE FROM inv_grid_storage WHERE grid_storage_id = pGridID;
  RETURN numLocationsDeleted;
END DELETELOCATIONGRID;
/
show errors;
