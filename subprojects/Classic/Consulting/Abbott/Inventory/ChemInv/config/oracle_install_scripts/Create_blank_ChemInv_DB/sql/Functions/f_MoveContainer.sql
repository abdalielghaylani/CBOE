-- Create procedure/function MOVECONTAINER.
CREATE OR REPLACE  FUNCTION "&&SchemaName"."MOVECONTAINER"               
(pLocationID IN inv_Containers.Location_ID_FK%Type, pContainerId IN varchar2)
return integer
IS
Location_ID_FK number;
source_cursor integer;
rows_processed integer;
container_type_not_allowed exception;

BEGIN
if is_container_type_allowed(NULL, pLocationID) = 0 then
  RAISE container_type_not_allowed;
end if;
source_cursor := dbms_sql.open_cursor;
dbms_sql.parse(source_cursor,'UPDATE Inv_Containers SET Location_ID_FK = ' || pLocationID || ' WHERE Container_ID IN (' || pContainerID || ')' , dbms_sql.NATIVE);
rows_processed := dbms_sql.execute(source_cursor);
  
RETURN pLocationID;
Exception
WHEN container_type_not_allowed then 
  RETURN -128;
END MoveContainer;
/
show errors;
