CREATE OR REPLACE FUNCTION "CHECKINDEFAULTLOCATION"
(pLocationID IN inv_Containers.Location_ID_FK%Type,
 pContainerId IN varchar2,
 pCurrentUserID IN inv_Containers.Current_User_ID_FK%Type:=NULL,
 pOwnerID IN inv_containers.Owner_ID_FK%Type:=NULL,
 pContainerStatusID IN inv_Containers.Container_status_id_fk%type:=NULL)
return inv_Containers.Location_ID_FK%Type
IS
destination_not_found exception;
container_type_not_allowed exception;
pragma exception_init (destination_not_found, -2291);
my_sql varchar2(4000);

BEGIN
if is_container_type_allowed(Null, pLocationID) = 0 then
  RAISE container_type_not_allowed;
end if;

  my_sql := 'UPDATE Inv_Containers SET Location_ID_FK = Def_Location_ID_FK ';
if pCurrentUserID is NOT NULL then
  my_sql := my_sql || ', Current_User_ID_FK = ''' || pCurrentUserID || '''';
end if;
if pContainerStatusID is NOT Null then
  my_sql := my_sql || ', Container_Status_ID_FK = ' || pContainerStatusID;
end if;
if pOwnerID is NOT Null then
  my_sql := my_sql || ', Owner_ID_FK = ''' || pOwnerID || '''';
End if;
  my_sql := my_sql || ' WHERE Container_ID IN (' || pContainerID || ')';
dbms_output.put_line(my_Sql);
EXECUTE IMMEDIATE
  my_sql;
RETURN pLocationID;

exception
WHEN destination_not_found then
--RETURN 'Could not find the destination location.';
  RETURN -104;
WHEN container_type_not_allowed then
  RETURN -128;
END CheckInDefaultLocation;
/
show errors;