CREATE OR REPLACE FUNCTION "CHECKOUTCONTAINER"
(pLocationID IN varchar2,
 pContainerId IN varchar2,
 pCurrentUserID IN inv_Containers.Current_User_ID_FK%Type:=NULL,
 pOwnerID IN inv_containers.Owner_ID_FK%Type:=NULL,
 pDefaultLocationID IN inv_Containers.Location_ID_FK%Type:=NULL,
 pContainerStatusID IN inv_Containers.Container_status_id_fk%type:=NULL)
return inv_Containers.Location_ID_FK%Type
IS
destination_not_found exception;
container_type_not_allowed exception;
pragma exception_init (destination_not_found, -2291);
my_sql varchar2(4000);
l_containerId_t     STRINGUTILS.t_char;
l_locationId inv_locations.location_id%TYPE;
BEGIN
if is_container_type_allowed(Null, pLocationID) = 0 then
  RAISE container_type_not_allowed;
end if;

	l_containerId_t :=stringutils.split(pContainerId,',');
  FOR i IN l_containerId_t.FIRST .. l_containerId_t.LAST
  LOOP
  	l_locationId := guiutils.GetLocationId(pLocationID, l_containerId_t(i), NULL, NULL);
    my_sql := 'UPDATE Inv_Containers SET Location_ID_FK= ' || l_locationId;
  if pCurrentUserID is NOT NULL then
    my_sql := my_sql || ', Current_User_ID_FK = ''' || pCurrentUserID || '''';
  end if;
  if pContainerStatusID is NOT Null then
    my_sql := my_sql || ', Container_Status_ID_FK = ' || pContainerStatusID;
  end if;
  if pDefaultLocationID is NOT Null then
  	IF pDefaultLocationID = -1 THEN
    	--' use the container's current location_id_fk
      my_sql := my_sql || ', def_location_id_fk = location_id_fk  ';
    ELSE
  	  my_sql := my_sql || ', Def_Location_ID_FK= ' || pDefaultLocationID;
    END IF;
  end if;
  if pOwnerID is NOT Null then
    my_sql := my_sql || ', Owner_ID_FK = ''' || pOwnerID || '''';
  End if;
    my_sql := my_sql || ' WHERE Container_ID = ' || l_containerId_t(i);
  EXECUTE IMMEDIATE
    my_sql;
	END LOOP;
RETURN l_locationId;

exception
WHEN destination_not_found then
--RETURN 'Could not find the destination location.';
  RETURN -104;
WHEN container_type_not_allowed then
  RETURN -128;
END CheckOutContainer;
/
show errors;
