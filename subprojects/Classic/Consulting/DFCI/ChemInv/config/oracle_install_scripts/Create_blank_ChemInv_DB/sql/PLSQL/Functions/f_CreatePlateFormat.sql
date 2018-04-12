CREATE OR REPLACE FUNCTION
 "CREATEPLATEFORMAT"(pPlateFormatName IN inv_plate_format.plate_format_name%TYPE,
										 pPhysPlateIdFk   IN inv_plate_format.phys_plate_id_fk%TYPE)
 RETURN inv_plate_format.plate_format_id%TYPE

 IS

	NewPlateFormatId inv_plate_format.plate_format_id%TYPE;
	GridFormatId     inv_grid_format.grid_format_id%TYPE;
	CURSOR grid_positions IS
		SELECT * FROM inv_grid_position WHERE grid_format_id_fk = GridFormatId;
	l_XmlDocID inv_xmldocs.xmldoc_id%TYPE;

BEGIN

	-- insert new values into inv_plate_format
	INSERT INTO inv_plate_format
		(plate_format_name, phys_plate_id_fk)
	VALUES
		(pPlateFormatName, pPhysPlateIdFk)
	RETURNING plate_format_id INTO NewPlateFormatId;

	-- create as many new wells as necessary
	-- get grid format id
	SELECT grid_format_id_fk
		INTO GridFormatId
		FROM inv_physical_plate
	 WHERE phys_plate_id = pPhysPlateIdFK;

	-- insert into inv_wells
	-- 1 is the value for a compound well
	FOR grid_positions_cur IN grid_positions
	LOOP
		INSERT INTO inv_wells
			(plate_format_id_fk, grid_position_id_fk, well_format_id_fk)
		VALUES
			(NewPlateFormatID, grid_positions_cur.grid_position_id, 1);
	END LOOP;

	--create the daughter reformat map
	l_XmlDocID := Reformat.CreateDaughteringMap(NewPlateFormatId);

	RETURN NewPlateFormatId;

END CreatePlateFormat;
/
show errors;