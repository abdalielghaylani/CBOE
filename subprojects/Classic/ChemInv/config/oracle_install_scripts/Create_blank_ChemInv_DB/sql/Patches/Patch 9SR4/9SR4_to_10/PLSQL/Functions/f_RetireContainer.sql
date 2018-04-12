CREATE OR REPLACE FUNCTION "RETIRECONTAINER"
(
    pContainerID in varchar2,
    pQtyRemaining in varchar2,
    pContainerStatusID in Inv_Containers.Container_Status_ID_FK%type,
    pLocationID in Inv_Containers.Location_Id_FK%type
)
return inv_containers.Container_ID%Type
IS
source_cursor integer;
rows_processed integer;
containers_t STRINGUTILS.t_char;
qtyremaining_t STRINGUTILS.t_char;
excess_contents exception;
container_type_not_allowed exception;
l_locationId inv_locations.location_id%TYPE;
pragma exception_init (excess_contents, -2290);
BEGIN
if is_container_type_allowed(NULL, pLocationID) = 0 then
  RAISE container_type_not_allowed;
end if;

containers_t := STRINGUTILS.split(pContainerID,',');
qtyremaining_t := STRINGUTILS.split(pQtyRemaining,',');

IF guiutils.UseGetLocation(pLocationID) = 0 THEN
  FOR i in containers_t.First..containers_t.Last
  LOOP
	UPDATE inv_containers SET qty_remaining = qtyremaining_t(i), container_status_id_fk = pContainerStatusID, location_id_fk = pLocationID WHERE container_id = containers_t(i);
  END LOOP;
ELSE
	FOR i in containers_t.First..containers_t.Last
	LOOP
  	l_locationId := guiutils.GetLocationId(pLocationID, containers_t(i), NULL, NULL);
		UPDATE inv_containers SET qty_remaining = qtyremaining_t(i), container_status_id_fk = pContainerStatusID, location_id_fk = l_locationId WHERE container_id = containers_t(i);
  END LOOP;
END IF;


FOR i in containers_t.First..containers_t.Last
Loop
      Reservations.ReconcileQtyAvailable(containers_t(i));
End loop;
RETURN 1;

exception
WHEN excess_contents then
  RETURN -103;
WHEN container_type_not_allowed then
  RETURN -128;
END RetireContainer;
/
show errors;
