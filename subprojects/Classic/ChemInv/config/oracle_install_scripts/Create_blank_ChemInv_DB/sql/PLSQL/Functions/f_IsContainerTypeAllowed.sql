CREATE OR REPLACE FUNCTION "IS_CONTAINER_TYPE_ALLOWED"
(pContainerID in
 inv_containers.container_id%type:=NULL, 
	pLocationID IN VARCHAR2)
	return integer
is
rec_count INTEGER :=0 ;
l_locationIds_t Stringutils.t_char;
tempCount INT;
begin
-- Current implementation only checks for locations where all container types are disallowed.
-- Future implementation will check for specific container type exclusions.
-- TODO: get container type ID from container ID and check against specific container type exclusions.
l_locationIds_t := stringutils.split(pLocationID,',');

FOR i IN l_locationIds_t.FIRST..l_locationIds_t.LAST
LOOP
					select count(*) into tempCount from inv_exclude_container_types where location_id_fk = l_locationIds_t(i) ;
					rec_count := rec_count + tempCount;
	END LOOP;
	if rec_count = 1 then
		return 0;
	else
		return 1;
	end if;
end Is_Container_Type_Allowed;
/
show errors;
