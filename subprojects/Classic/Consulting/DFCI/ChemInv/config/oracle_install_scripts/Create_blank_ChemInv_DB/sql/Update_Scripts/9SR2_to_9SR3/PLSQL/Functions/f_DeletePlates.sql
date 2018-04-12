CREATE OR REPLACE FUNCTION  "&&SchemaName"."DELETEPLATES"
(pPlateID in varchar2)
return integer
IS
my_sql varchar2(2000);
BEGIN
  my_sql := 'UPDATE inv_plates set location_id_fk = ' || Constants.cTrashCanLoc || ', status_id_fk = ' || Constants.cDestroyed_pl || ' WHERE plate_id IN(' || pPlateID || ')';
  EXECUTE IMMEDIATE	my_sql;
  RETURN 0;
END DELETEPLATES;
/
show errors;

