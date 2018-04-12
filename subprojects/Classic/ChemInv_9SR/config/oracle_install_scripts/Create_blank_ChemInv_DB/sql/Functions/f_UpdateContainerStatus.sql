CREATE OR REPLACE  FUNCTION "&&SchemaName"."UPDATECONTAINERSTATUS"       (
    pContainerID in varchar2,
     pContainerStatusID in Inv_containers.container_status_id_fk%Type)
return inv_containers.Container_ID%Type
IS
BEGIN
  Execute Immediate
  'Update inv_containers set container_status_id_fk = ' || pContainerStatusID || ' where Container_ID IN(' || pContainerID || ')';
  
  RETURN 1;
END UpdateContainerStatus;
/
show errors;
