-- Create procedure/function DELETECONTAINER.
create or replace
FUNCTION "&&SchemaName"."DELETECONTAINER"(pContainerID in varchar2)
return integer
IS
my_sql varchar2(4000);
currentLocation integer;
BEGIN
  my_sql := 'UPDATE inv_containers set location_id_fk = :ConstantValue WHERE container_id IN(' || pContainerID || ')';
  EXECUTE IMMEDIATE
	my_sql
	USING Constants.cTrashCanLoc;
    RETURN 1;
END DeleteContainer;
/
show errors;