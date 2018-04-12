	CREATE OR REPLACE FUNCTION "MOVEPLATES"
(pLocationID IN VARCHAR2,
 pPlateID IN varchar2,
 pDoFillGrid IN varchar2,
 pPreview IN varchar2)
return varchar
IS
CURSOR vEmptyLocations_cur IS
	SELECT location_name, location_id FROM inv_vw_grid_location
		WHERE location_id not in (SELECT location_id_fk FROM inv_plates)
		AND parent_id = pLocationID;
l_plateIds_t STRINGUTILS.t_char;
l_locationId inv_locations.location_id%TYPE;
source_cursor integer;
rows_processed integer;
container_type_not_allowed exception;
l_oldLocationId inv_locations.location_id%TYPE;
l_ftIncrement INT;

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
	l_plateIds_t := STRINGUTILS.split(pPlateID, ',');
	--get number of plates to be moved
	FOR i in l_plateIds_t.First..l_plateIds_t.Last
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
		FOR i in l_plateIds_t.First..l_plateIds_t.Last
		LOOP
			FETCH vEmptyLocations_cur INTO vEmptyLocations_rec;
			IF pPreview = 'true' THEN
				--return a list of location names
				vPreviewReturn := vPreviewReturn || '||' || vEmptyLocations_rec.location_id;
			ELSE
               	--' update f/t cycle
				SELECT location_id_fk INTO l_oldLocationId FROM inv_plates WHERE plate_id = l_plateIds_t(i);
                    l_ftIncrement := SetPlateFTCycle(l_plateIds_t(i), l_oldLocationId, vEmptyLocations_rec.location_id);
                    --' move plate
				UPDATE inv_plates SET location_id_fk = vEmptyLocations_rec.location_id WHERE plate_id = l_plateIds_t(i);
			END IF;
		END LOOP;
		CLOSE vEmptyLocations_cur;
	END IF;
ELSE

  IF instr(pLocationID,',') > 0 OR racks.isRackLocation(pLocationID) = 1 THEN
  	l_plateIds_t := stringutils.split(pPlateID,',');
    FOR i IN l_plateIds_t.FIRST..l_plateIds_t.LAST
    LOOP
	--' determine location_id
    	--l_locationId := racks.multiGetNextOpenPosition(pLocationID);
					l_locationId := guiutils.GetLocationId(pLocationID, NULL, l_plateIds_t(i), NULL);

	--' update f/t cycle
	SELECT location_id_fk INTO l_oldLocationId FROM inv_plates WHERE plate_id = l_plateIds_t(i);
     l_ftIncrement := SetPlateFTCycle(l_plateIds_t(i), l_oldLocationId, l_locationId);

     --' move plate
    	UPDATE inv_plates SET location_id_fk = l_locationId WHERE plate_id = l_plateIds_t(i);
    END LOOP;
  ELSE
	--' update f/t cycle
  	l_plateIds_t := stringutils.split(pPlateID,',');
    	FOR i IN l_plateIds_t.FIRST..l_plateIds_t.LAST
    	LOOP
	  	SELECT location_id_fk INTO l_oldLocationId FROM inv_plates WHERE plate_id = l_plateIds_t(i);
     	l_ftIncrement := SetPlateFTCycle(l_plateIds_t(i), l_oldLocationId, pLocationID);
	END LOOP;

     --put all plate in one location
  	source_cursor := dbms_sql.open_cursor;
  	dbms_sql.parse(source_cursor,'UPDATE Inv_Plates SET Location_ID_FK = ' || pLocationID || ' WHERE Plate_ID IN (' || pPlateID || ')' , dbms_sql.NATIVE);
  	rows_processed := dbms_sql.execute(source_cursor);
	END IF;
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

