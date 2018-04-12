CREATE OR REPLACE FUNCTION "RETIREPLATE"
(
    pPlateID in varchar2,
    pQtyRemaining in Inv_Plates.Qty_Remaining%Type,
    pStatusIDFK in Inv_Plates.Status_ID_FK%type,
    pLocationIDFK in Inv_Plates.Location_Id_FK%type
)
return inv_plates.Plate_ID%Type
IS
source_cursor integer;
rows_processed integer;
plates_t STRINGUTILS.t_char;
excess_contents exception;
container_type_not_allowed exception;
l_locationId inv_locations.location_id%TYPE;

pragma exception_init (excess_contents, -2290);
BEGIN
if is_container_type_allowed(NULL, pLocationIDFK) = 0 then
  RAISE container_type_not_allowed;
end if;


IF guiutils.UseGetLocation(pLocationIDFK) = 0 THEN
  source_cursor := dbms_sql.open_cursor;
  dbms_sql.parse(source_cursor,'Update inv_plates set Qty_Remaining =' || pQtyRemaining || ', Status_ID_FK =' || pStatusIDFK || ',Location_ID_FK =' || pLocationIDFK || ' WHERE Plate_ID IN (' || pPlateID || ')' , dbms_sql.NATIVE);
  rows_processed := dbms_sql.execute(source_cursor);
ELSE
	plates_t := STRINGUTILS.split(pPlateID,',');
	FOR i in plates_t.First..plates_t.Last
	LOOP
  	l_locationId := guiutils.GetLocationId(pLocationIDFK, NULL, plates_t(i), NULL);
		UPDATE inv_plates SET qty_remaining = pQtyRemaining, status_id_fk = pStatusIDFK, location_id_fk = l_locationId WHERE plate_id = plates_t(i);
  END LOOP;
END IF;


/*
plate_t := STRINGUTILS.split(pPlateID,',');

FOR i in containers_t.First..containers_t.Last
Loop
      Reservations.ReconcileQtyAvailable(containers_t(i));
End loop;
*/
RETURN 1;

exception
WHEN excess_contents then
  RETURN -103;
WHEN container_type_not_allowed then
  RETURN -128;
END "RETIREPLATE";
/
show errors;
