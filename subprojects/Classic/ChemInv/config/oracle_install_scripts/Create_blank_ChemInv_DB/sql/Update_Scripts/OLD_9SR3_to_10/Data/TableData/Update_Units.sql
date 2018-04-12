-- Copyright Cambridgesoft Corp 2001-2005 all rights reserved.

DECLARE
	ea_unit_id INTEGER := 0;
	um_unit_id INTEGER := 0;

BEGIN
	-- Get existing unit_id
	select unit_id into ea_unit_id from inv_units where unit_name = 'each' and unit_abreviation = 'ea';
	select unit_id into um_unit_id from inv_units where unit_name = 'micromolar' and unit_abreviation = 'uM';

	-- Insert new units
	INSERT INTO inv_units (unit_id, unit_name, unit_abreviation, unit_type_id_fk) 
	SELECT 92,'can','can',4 from dual
	WHERE (select 1 from inv_units where unit_id = 92) is null;

	INSERT INTO inv_units (unit_id, unit_name, unit_abreviation, unit_type_id_fk) 
	SELECT 93,'pouch','pouch',4 from dual
	WHERE (select 1 from inv_units where unit_id = 92) is null;

	INSERT INTO inv_units (unit_id, unit_name, unit_abreviation, unit_type_id_fk) 
	SELECT 94,'KIU','KIU',4 from dual
	WHERE (select 1 from inv_units where unit_id = 94) is null;

	INSERT INTO inv_units (unit_id, unit_name, unit_abreviation, unit_type_id_fk) 
	SELECT 95,'mt','mt',2 from dual
	WHERE (select 1 from inv_units where unit_id = 95) is null;

	INSERT INTO inv_units (unit_id, unit_name, unit_abreviation, unit_type_id_fk) 
	SELECT 96,'sleeve','sleeve',4 from dual
	WHERE (select 1 from inv_units where unit_id = 96) is null;

	INSERT INTO inv_units (unit_id, unit_name, unit_abreviation, unit_type_id_fk) 
	SELECT 97,'syringe','syringe',4 from dual
	WHERE (select 1 from inv_units where unit_id = 97) is null;

	INSERT INTO inv_units (unit_id, unit_name, unit_abreviation, unit_type_id_fk) 
	SELECT 98,'ton','ton',2 from dual
	WHERE (select 1 from inv_units where unit_id = 98) is null;

	INSERT INTO inv_units (unit_id, unit_name, unit_abreviation, unit_type_id_fk) 
	SELECT 99,'reel','reel',4 from dual
	WHERE (select 1 from inv_units where unit_id = 99) is null;

	INSERT INTO inv_units (unit_id, unit_name, unit_abreviation, unit_type_id_fk) 
	SELECT 100,'each','ea',4 from dual
	WHERE (select 1 from inv_units where unit_id = 100) is null;

	INSERT INTO inv_units (unit_id, unit_name, unit_abreviation, unit_type_id_fk) 
	SELECT 101,'micromolar','uM',3 from dual
	WHERE (select 1 from inv_units where unit_id = 101) is null;

	-- Update where appropriate.  91 used to be "uM", now it is "rack"
	if um_unit_id = 91 then
		UPDATE inv_unit_conversion SET from_unit_id_fk = 101 WHERE from_unit_id_fk = 91;
		UPDATE inv_unit_conversion SET to_unit_id_fk = 101 WHERE to_unit_id_fk = 91;
		UPDATE inv_containers SET UNIT_OF_CONC_ID_FK = 101 WHERE UNIT_OF_CONC_ID_FK = 91;
		UPDATE inv_containers SET UNIT_OF_MEAS_ID_FK = 101 WHERE UNIT_OF_MEAS_ID_FK = 91;
		UPDATE inv_containers SET UNIT_OF_PURITY_ID_FK = 101 WHERE UNIT_OF_PURITY_ID_FK = 91;
		UPDATE inv_containers SET UNIT_OF_WGHT_ID_FK = 101 WHERE UNIT_OF_WGHT_ID_FK = 91;
		UPDATE inv_physical_plate SET CAPACITY_UNIT_ID_FK = 101 WHERE CAPACITY_UNIT_ID_FK = 91;
		UPDATE inv_plates SET CONC_UNIT_FK = 101 WHERE CONC_UNIT_FK = 91;
		UPDATE inv_plates SET WEIGHT_UNIT_FK = 101 WHERE WEIGHT_UNIT_FK = 91;
		UPDATE inv_plates SET MOLAR_UNIT_FK = 101 WHERE MOLAR_UNIT_FK = 91;
		UPDATE inv_plates SET QTY_UNIT_FK = 101 WHERE QTY_UNIT_FK = 91;
		UPDATE inv_plates SET SOLVENT_VOLUME_UNIT_ID_FK = 101 WHERE SOLVENT_VOLUME_UNIT_ID_FK = 91;
		UPDATE inv_plates SET WELL_CAPACITY_UNIT_ID_FK = 101 WHERE WELL_CAPACITY_UNIT_ID_FK = 91;
		UPDATE inv_unit_conversion SET FROM_UNIT_ID_FK = 101 WHERE FROM_UNIT_ID_FK = 91;
		UPDATE inv_unit_conversion SET TO_UNIT_ID_FK = 101 WHERE TO_UNIT_ID_FK = 91;
		UPDATE inv_wells SET CONC_UNIT_FK = 101 WHERE CONC_UNIT_FK = 91;
		UPDATE inv_wells SET MOLAR_UNIT_FK = 101 WHERE MOLAR_UNIT_FK = 91;
		UPDATE inv_wells SET QTY_UNIT_FK = 101 WHERE QTY_UNIT_FK = 91;
		UPDATE inv_wells SET SOLVENT_VOLUME_UNIT_ID_FK = 101 WHERE SOLVENT_VOLUME_UNIT_ID_FK = 91;
		UPDATE inv_wells SET WEIGHT_UNIT_FK = 101 WHERE WEIGHT_UNIT_FK = 91;
	end if;

	-- Update where appropriate.  21 used to be "ea", now it is "kit"
	if ea_unit_id = 21 then
		UPDATE inv_containers SET UNIT_OF_CONC_ID_FK = 100 WHERE UNIT_OF_CONC_ID_FK = 21;
		UPDATE inv_containers SET UNIT_OF_MEAS_ID_FK = 100 WHERE UNIT_OF_MEAS_ID_FK = 21;
		UPDATE inv_containers SET UNIT_OF_PURITY_ID_FK = 100 WHERE UNIT_OF_PURITY_ID_FK = 21;
		UPDATE inv_containers SET UNIT_OF_WGHT_ID_FK = 100 WHERE UNIT_OF_WGHT_ID_FK = 21;
		UPDATE inv_physical_plate SET CAPACITY_UNIT_ID_FK = 100 WHERE CAPACITY_UNIT_ID_FK = 21;
		UPDATE inv_plates SET CONC_UNIT_FK = 100 WHERE CONC_UNIT_FK = 21;
		UPDATE inv_plates SET WEIGHT_UNIT_FK = 100 WHERE WEIGHT_UNIT_FK = 21;
		UPDATE inv_plates SET MOLAR_UNIT_FK = 100 WHERE MOLAR_UNIT_FK = 21;
		UPDATE inv_plates SET QTY_UNIT_FK = 100 WHERE QTY_UNIT_FK = 21;
		UPDATE inv_plates SET SOLVENT_VOLUME_UNIT_ID_FK = 100 WHERE SOLVENT_VOLUME_UNIT_ID_FK = 21;
		UPDATE inv_plates SET WELL_CAPACITY_UNIT_ID_FK = 100 WHERE WELL_CAPACITY_UNIT_ID_FK = 21;
		UPDATE inv_unit_conversion SET FROM_UNIT_ID_FK = 100 WHERE FROM_UNIT_ID_FK = 21;
		UPDATE inv_unit_conversion SET TO_UNIT_ID_FK = 100 WHERE TO_UNIT_ID_FK = 21;
		UPDATE inv_wells SET CONC_UNIT_FK = 100 WHERE CONC_UNIT_FK = 21;
		UPDATE inv_wells SET MOLAR_UNIT_FK = 100 WHERE MOLAR_UNIT_FK = 21;
		UPDATE inv_wells SET QTY_UNIT_FK = 100 WHERE QTY_UNIT_FK = 21;
		UPDATE inv_wells SET SOLVENT_VOLUME_UNIT_ID_FK = 100 WHERE SOLVENT_VOLUME_UNIT_ID_FK = 21;
		UPDATE inv_wells SET WEIGHT_UNIT_FK = 100 WHERE WEIGHT_UNIT_FK = 21;
	end if;
	
	-- Update overwriting units
	update inv_units set unit_name = 'kit', unit_abreviation = 'kit', unit_type_id_fk = 4 where unit_id = 21;
	update inv_units set unit_name = 'rack', unit_abreviation = 'rack', unit_type_id_fk = 4 where unit_id = 91;	

commit;
END;
/