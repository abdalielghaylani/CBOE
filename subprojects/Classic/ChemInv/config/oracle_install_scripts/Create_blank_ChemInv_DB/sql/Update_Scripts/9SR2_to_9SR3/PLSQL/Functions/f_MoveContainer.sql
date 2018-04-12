CREATE OR REPLACE FUNCTION "MOVECONTAINER"
(pLocationID IN VARCHAR2, pContainerId IN varchar2)
return VARCHAR2
IS
l_locationId inv_locations.location_id%TYPE;
source_cursor integer;
rows_processed integer;
container_type_not_allowed exception;
l_containerIds_t stringutils.t_char;

BEGIN
/*
if is_container_type_allowed(NULL, pLocationID) = 0 then
  RAISE container_type_not_allowed;
end if;
*/
source_cursor := dbms_sql.open_cursor;

IF instr(pLocationID,',') > 0 OR racks.isRackLocation(pLocationID) = 1 THEN
	l_containerIds_t := stringutils.split(pContainerId,',');
  FOR i IN l_containerIds_t.FIRST..l_containerIds_t.LAST
  LOOP
  	--l_locationId := racks.multiGetNextOpenPosition(pLocationID);
			l_locationId := guiutils.GetLocationId(pLocationID, l_containerIds_t(i),NULL,NULL);
  	UPDATE inv_containers SET location_id_fk = l_locationId WHERE container_id = l_containerIds_t(i);
  END LOOP;
ELSE
	dbms_sql.parse(source_cursor,'UPDATE Inv_Containers SET Location_ID_FK = ' || pLocationID || ' WHERE Container_ID IN (' || pContainerID || ')' , dbms_sql.NATIVE);
	rows_processed := dbms_sql.execute(source_cursor);
END IF;


RETURN pLocationID;
Exception
WHEN container_type_not_allowed then
  RETURN -128;
END MoveContainer;
/
show errors;
