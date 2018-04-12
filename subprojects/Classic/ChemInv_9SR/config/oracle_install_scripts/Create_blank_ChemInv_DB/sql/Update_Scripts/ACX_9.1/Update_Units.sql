-- Copyright Cambridgesoft Corp 2001-2005 all rights reserved.

-- Update ChemInvDB from 8.0 to 9.0, which is also schemaversion 3.0 to 4.0.

spool ON
spool ..\..\Logs\LOG_Update_ChemInvDB_9.0_for_ChemACX_9.1.txt

@@..\..\parameters.sql
@@..\..\prompts.sql

Connect &&schemaName/&&schemaPass@&&serverName

--update existing units
--UPDATE inv_units SET unit_name = 'bag', unit_abreviation = 'bag' WHERE unit_id = 28;
--UPDATE inv_units SET unit_name = 'box', unit_abreviation = 'box' WHERE unit_id = 31;
--UPDATE inv_units SET unit_name = 'case', unit_abreviation = 'case' WHERE unit_id = 33;
--UPDATE inv_units SET unit_name = 'cylinder', unit_abreviation = 'cylinder' WHERE unit_id = 38;
--UPDATE inv_units SET unit_name = 'pkg', unit_abreviation = 'pkg' WHERE unit_id = 57;
--UPDATE inv_units SET unit_name = 'roll', unit_abreviation = 'roll' WHERE unit_id = 61;
--UPDATE inv_units SET unit_name = 'sheet', unit_abreviation = 'sheet' WHERE unit_id = 64;
--UPDATE inv_units SET unit_name = 'slug', unit_abreviation = 'slug' WHERE unit_id = 67;
--UPDATE inv_units SET unit_name = 'unit', unit_abreviation = 'unit' WHERE unit_id = 76;
--UPDATE inv_units SET unit_name = 'capsule', unit_abreviation = 'capsule' WHERE unit_id = 82;

--insert new units
INSERT INTO inv_units (unit_id, unit_name, unit_abreviation, unit_type_id_fk) VALUES (92,'can','can',4);
INSERT INTO inv_units (unit_id, unit_name, unit_abreviation, unit_type_id_fk) VALUES (93,'pouch','pouch',4);
INSERT INTO inv_units (unit_id, unit_name, unit_abreviation, unit_type_id_fk) VALUES (94,'KIU','KIU',4);
INSERT INTO inv_units (unit_id, unit_name, unit_abreviation, unit_type_id_fk) VALUES (95,'mt','mt',2);
INSERT INTO inv_units (unit_id, unit_name, unit_abreviation, unit_type_id_fk) VALUES (96,'sleeve','sleeve',4);
INSERT INTO inv_units (unit_id, unit_name, unit_abreviation, unit_type_id_fk) VALUES (97,'syringe','syringe',4);
INSERT INTO inv_units (unit_id, unit_name, unit_abreviation, unit_type_id_fk) VALUES (98,'ton','ton',2);
INSERT INTO inv_units (unit_id, unit_name, unit_abreviation, unit_type_id_fk) VALUES (99,'reel','reel',4);

--insert new units for the units being overwritten
INSERT INTO inv_units (unit_id, unit_name, unit_abreviation, unit_type_id_fk) VALUES (100,'each','ea',4);
INSERT INTO inv_units (unit_id, unit_name, unit_abreviation, unit_type_id_fk) VALUES (101,'micromolar','uM',3);
--update inv_unit_conversion
UPDATE inv_unit_conversion SET from_unit_id_fk = 101 WHERE from_unit_id_fk = 91;
UPDATE inv_unit_conversion SET to_unit_id_fk = 101 WHERE to_unit_id_fk = 91;

--update unit references
UPDATE inv_containers SET UNIT_OF_CONC_ID_FK = 100 WHERE UNIT_OF_CONC_ID_FK = 21;
UPDATE inv_containers SET UNIT_OF_CONC_ID_FK = 101 WHERE UNIT_OF_CONC_ID_FK = 91;
UPDATE inv_containers SET UNIT_OF_MEAS_ID_FK = 100 WHERE UNIT_OF_MEAS_ID_FK = 21;
UPDATE inv_containers SET UNIT_OF_MEAS_ID_FK = 101 WHERE UNIT_OF_MEAS_ID_FK = 91;
UPDATE inv_containers SET UNIT_OF_PURITY_ID_FK = 100 WHERE UNIT_OF_PURITY_ID_FK = 21;
UPDATE inv_containers SET UNIT_OF_PURITY_ID_FK = 101 WHERE UNIT_OF_PURITY_ID_FK = 91;
UPDATE inv_containers SET UNIT_OF_WGHT_ID_FK = 100 WHERE UNIT_OF_WGHT_ID_FK = 21;
UPDATE inv_containers SET UNIT_OF_WGHT_ID_FK = 101 WHERE UNIT_OF_WGHT_ID_FK = 91;
UPDATE inv_physical_plate SET CAPACITY_UNIT_ID_FK = 100 WHERE CAPACITY_UNIT_ID_FK = 21;
UPDATE inv_physical_plate SET CAPACITY_UNIT_ID_FK = 101 WHERE CAPACITY_UNIT_ID_FK = 91;
UPDATE inv_plates SET CONC_UNIT_FK = 100 WHERE CONC_UNIT_FK = 21;
UPDATE inv_plates SET CONC_UNIT_FK = 101 WHERE CONC_UNIT_FK = 91;
UPDATE inv_plates SET WEIGHT_UNIT_FK = 100 WHERE WEIGHT_UNIT_FK = 21;
UPDATE inv_plates SET WEIGHT_UNIT_FK = 101 WHERE WEIGHT_UNIT_FK = 91;
UPDATE inv_plates SET MOLAR_UNIT_FK = 100 WHERE MOLAR_UNIT_FK = 21;
UPDATE inv_plates SET MOLAR_UNIT_FK = 101 WHERE MOLAR_UNIT_FK = 91;
UPDATE inv_plates SET QTY_UNIT_FK = 100 WHERE QTY_UNIT_FK = 21;
UPDATE inv_plates SET QTY_UNIT_FK = 101 WHERE QTY_UNIT_FK = 91;
UPDATE inv_plates SET SOLVENT_VOLUME_UNIT_ID_FK = 100 WHERE SOLVENT_VOLUME_UNIT_ID_FK = 21;
UPDATE inv_plates SET SOLVENT_VOLUME_UNIT_ID_FK = 101 WHERE SOLVENT_VOLUME_UNIT_ID_FK = 91;
UPDATE inv_plates SET WELL_CAPACITY_UNIT_ID_FK = 100 WHERE WELL_CAPACITY_UNIT_ID_FK = 21;
UPDATE inv_plates SET WELL_CAPACITY_UNIT_ID_FK = 101 WHERE WELL_CAPACITY_UNIT_ID_FK = 91;
UPDATE inv_unit_conversion SET FROM_UNIT_ID_FK = 100 WHERE FROM_UNIT_ID_FK = 21;
UPDATE inv_unit_conversion SET FROM_UNIT_ID_FK = 101 WHERE FROM_UNIT_ID_FK = 91;
UPDATE inv_unit_conversion SET TO_UNIT_ID_FK = 100 WHERE TO_UNIT_ID_FK = 21;
UPDATE inv_unit_conversion SET TO_UNIT_ID_FK = 101 WHERE TO_UNIT_ID_FK = 91;
UPDATE inv_wells SET CONC_UNIT_FK = 100 WHERE CONC_UNIT_FK = 21;
UPDATE inv_wells SET CONC_UNIT_FK = 101 WHERE CONC_UNIT_FK = 91;
UPDATE inv_wells SET MOLAR_UNIT_FK = 100 WHERE MOLAR_UNIT_FK = 21;
UPDATE inv_wells SET MOLAR_UNIT_FK = 101 WHERE MOLAR_UNIT_FK = 91;
UPDATE inv_wells SET QTY_UNIT_FK = 100 WHERE QTY_UNIT_FK = 21;
UPDATE inv_wells SET QTY_UNIT_FK = 101 WHERE QTY_UNIT_FK = 91;
UPDATE inv_wells SET SOLVENT_VOLUME_UNIT_ID_FK = 100 WHERE SOLVENT_VOLUME_UNIT_ID_FK = 21;
UPDATE inv_wells SET SOLVENT_VOLUME_UNIT_ID_FK = 101 WHERE SOLVENT_VOLUME_UNIT_ID_FK = 91;
UPDATE inv_wells SET WEIGHT_UNIT_FK = 100 WHERE WEIGHT_UNIT_FK = 21;
UPDATE inv_wells SET WEIGHT_UNIT_FK = 101 WHERE WEIGHT_UNIT_FK = 91;

--delete overwritten units
DELETE inv_units WHERE unit_id IN (21,91);
--insert overwriting units
INSERT INTO inv_units (unit_id, unit_name, unit_abreviation, unit_type_id_fk) VALUES (21,'kit','kit',4);
INSERT INTO inv_units (unit_id, unit_name, unit_abreviation, unit_type_id_fk) VALUES (91,'rack','rack',4);

COMMIT;

spool off
exit
