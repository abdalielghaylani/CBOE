CREATE OR REPLACE FUNCTION "&&SchemaName"."MOVEPLATES"
(pLocationID IN inv_Containers.Location_ID_FK%Type,
 pPlateID IN varchar2,
 pDoFillGrid IN varchar2,
 pPreview IN varchar2)
return varchar
IS
CURSOR vEmptyLocations_cur IS
	SELECT location_name, location_id FROM inv_vw_grid_location	
		WHERE location_id not in (SELECT location_id_fk FROM inv_plates)
		AND parent_id = pLocationID;
vPlateIDList_t STRINGUTILS.t_char;
Location_ID_FK number;
source_cursor integer;
rows_processed integer;
container_type_not_allowed exception;

vPlateCount integer;
vLocationCount integer;
vEmptyLocations_rec vEmptyLocations_cur%ROWTYPE;
vPreviewReturn varchar2(2000);
vReturn varchar2(2000);
TooFewEmptyLocations exception;
BEGIN
if is_container_type_allowed(NULL, pLocationID) = 0 then
  RAISE container_type_not_allowed;
end if;

IF pDoFillGrid = 'true' THEN
	vPlateIDList_t := STRINGUTILS.split(pPlateID, ',');
	--get number of plates to be moved
	FOR i in vPlateIDList_t.First..vPlateIDList_t.Last
	LOOP
		vPlateCount := i;
	END LOOP;
	--get number of empty locations
	SELECT count(*) INTO vLocationCount FROM inv_locations l, inv_grid_element g
		WHERE location_id = location_id_fk
		AND location_id_fk not in (SELECT location_id_fk FROM inv_plates)
		AND parent_id = pLocationID;
	--make sure there are enough empty locations
	IF vPlateCount > vLocationCount THEN
		raise TooFewEmptyLocations;
	ELSE
	--fill the empty locations with one plate each
		OPEN vEmptyLocations_cur;
		FOR i in vPlateIDList_t.First..vPlateIDList_t.Last
		LOOP
			FETCH vEmptyLocations_cur INTO vEmptyLocations_rec;
			IF pPreview = 'true' THEN
				--return a list of location names
				vPreviewReturn := vPreviewReturn || '||' || vEmptyLocations_rec.location_id;
			ELSE
				UPDATE inv_plates SET location_id_fk = vEmptyLocations_rec.location_id WHERE plate_id = vPlateIDList_t(i);
			END IF;
		END LOOP;
		CLOSE vEmptyLocations_cur;
	END IF;
ELSE
	--put all plate in one location
	source_cursor := dbms_sql.open_cursor;
	dbms_sql.parse(source_cursor,'UPDATE Inv_Plates SET Location_ID_FK = ' || pLocationID || ' WHERE Plate_ID IN (' || pPlateID || ')' , dbms_sql.NATIVE);
	rows_processed := dbms_sql.execute(source_cursor);
END IF;

IF pPreview = 'true' THEN
	vReturn := ltrim(vPreviewReturn,'||');
ELSE
	vReturn := pLocationID;
END IF;
RETURN vReturn;
EXCEPTION
WHEN TooFewEmptyLocations THEN
	RETURN -131;
WHEN container_type_not_allowed then
  	RETURN -128;
END "MOVEPLATES";
/
show errors;

