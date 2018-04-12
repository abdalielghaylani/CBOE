-- Create procedure/function DELETECONTAINER.
CREATE OR REPLACE  FUNCTION "&&SchemaName"."DELETECONTAINER"        
(pContainerID in varchar2)
return integer 
IS
my_sql varchar2(2000);
currentLocation integer;
BEGIN
  my_sql := 'UPDATE inv_containers set location_id_fk = ' || Constants.cTrashCanLoc || ' WHERE container_id IN(' || pContainerID || ')'; 
  EXECUTE IMMEDIATE
	my_sql;	
    RETURN 1;
END DeleteContainer;
/
show errors;
