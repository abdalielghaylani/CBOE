-- Check for container type exclusions
CREATE OR REPLACE  FUNCTION "&&SchemaName"."IS_CONTAINER_TYPE_ALLOWED"    
(pContainerID in 
 inv_containers.container_id%type:=NULL, pLocationID in 
 inv_locations.location_id%type)
	return integer
is
rec_count integer;
begin
-- Current implementation only checks for locations where all container types are disallowed.
-- Future implementation will check for specific container type exclusions.
-- TODO: get container type ID from container ID and check against specific container type exclusions.
	select count(*) into rec_count from inv_exclude_container_types where location_id_fk = pLocationID ;
	if rec_count = 1 then
		return 0;
	else
		return 1;
	end if;
end Is_Container_Type_Allowed;  
/
show errors;
