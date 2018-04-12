CREATE OR REPLACE  FUNCTION "&&SchemaName"."GETGRIDFORMATID"          (
    pLocationid in inv_locations.location_id%type)
	return inv_grid_format.grid_format_id%type
IS
GridFormatID inv_grid_format.grid_format_id%type;
BEGIN
  select grid_format_id_fk into GridFormatID from inv_grid_Storage where location_ID_FK = pLocationID;
  RETURN GridFormatID;
exception
  WHEN NO_DATA_FOUND THEN
  RETURN 0;
end GetGridFormatID;
/
show errors;
