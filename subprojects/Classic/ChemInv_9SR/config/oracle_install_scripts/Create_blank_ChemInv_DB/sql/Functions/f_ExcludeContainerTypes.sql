-- Exclude container types from a given location
CREATE OR REPLACE  FUNCTION "&&SchemaName"."EXCLUDE_CONTAINERTYPES"(
    pLocationID in inv_locations.location_id%type,
    pContainerTypeIDList in varchar2:=NULL,
    pClear in integer:=0
    )
	return integer
is
rec_count integer;
begin
-- Current implementation allows excluding all container types from a given location.
-- Future implementation will allow for specific container type exclusions.
-- TODO: parse containertypeidList and make one entry per container type
	
	delete from inv_exclude_container_types where location_id_fk = pLocationID;
	if pClear = 0 then
	 insert into inv_exclude_container_types (location_id_fk, container_type_id_fk) VALUES (pLocationID, NULL);
  end if;

		return 1;

end Exclude_ContainerTypes;  
/
show errors;
