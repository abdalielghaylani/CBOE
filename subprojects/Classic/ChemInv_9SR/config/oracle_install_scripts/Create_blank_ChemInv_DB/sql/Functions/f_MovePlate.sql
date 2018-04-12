CREATE OR REPLACE FUNCTION
"MOVEPLATE"
	(p_plate_id IN INV_PLATES.plate_id%TYPE,
	 p_new_location_id IN INV_LOCATIONS.location_id%TYPE,
	 p_date_moved IN INV_PLATE_HISTORY.plate_history_date%TYPE)
	 RETURN INV_PLATES.plate_id%TYPE
IS
ft_incremented INV_PLATE_HISTORY.is_ft_incremented%TYPE;
old_is_frozen INTEGER;
new_is_frozen INTEGER;
old_location_id INV_LOCATIONS.location_id%TYPE;
old_is_grid INTEGER;
new_is_grid INTEGER;
BEGIN
	-- get old location id
	SELECT INV_PLATES.location_id_fk INTO old_location_id
	FROM INV_PLATES WHERE plate_id = p_plate_id;

	old_is_grid := Isgridlocation(old_location_id);
	new_is_grid := Isgridlocation(p_new_location_id);
	IF old_is_grid = 1 THEN
		-- remove from grid location
		UPDATE INV_GRID_ELEMENT SET
		plate_id_fk = NULL WHERE plate_id_fk = p_plate_id;
	END IF;
	IF new_is_grid = 1 THEN
		-- add to grid location
		UPDATE INV_GRID_ELEMENT SET
		plate_id_fk = p_plate_id WHERE
		location_id_fk = p_new_location_id;
	END IF;

	-- determine if freeze/thaw should be incremented
	old_is_frozen := Isfrozenlocation(old_location_id);
	new_is_frozen := Isfrozenlocation(p_new_location_id);

	IF old_is_frozen = 1 AND new_is_frozen = 0 THEN
	   	 ft_incremented := 1;
	ELSE
		ft_incremented := 0;
	END IF;

	-- update plate
	UPDATE INV_PLATES SET
	location_id_fk = p_new_location_id,
	ft_cycles = ft_cycles + ft_incremented
	WHERE plate_id = p_plate_id;

	-- update plate history
	-- 3 = moved
	INSERT INTO INV_PLATE_HISTORY
	(plate_id_fk, plate_history_date, plate_action_id_fk, from_location_id_fk, to_location_id_fk, is_ft_incremented)
	VALUES
	(p_plate_id, p_date_moved, 3, old_location_id, p_new_location_id, ft_incremented);

	RETURN p_plate_id;

END Moveplate;
/
show errors;