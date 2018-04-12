CREATE OR REPLACE
FUNCTION	"ISPLATETYPEALLOWED"(
	pLocationID in inv_locations.location_id%Type,
	pPlateTypeID in inv_plate_types.plate_type_id%Type,
	pIsPlateMap in integer)
	return integer
is
rec_count integer;
BEGIN
-- Current implementation only checks for locations plate maps are allowed or not allowed.
-- Future implementation will check for specific plate type exclusions.
	IF pIsPlateMap = 1 THEN
		SELECT count(*) INTO rec_count 
			FROM inv_locations, inv_location_types
			WHERE   
				location_id = pLocationID
				AND location_type_id_fk = location_type_id 
				AND location_type_name = 'Plate Map';				
	END IF;
	if rec_count = 1 then
		return 0;
	else
		return 1;
	end if;
END;
/
