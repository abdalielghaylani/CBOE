

set feedback on
--spool ..\Logs\LOG_Update_Inventory_9_to_9SR1.txt

--update tables          
-- Add/modify columns 
alter table INV_SUPPLIERS modify IS_OFFICIAL_SUPPLIER default 0;
alter table INV_SUPPLIERS
  add constraint inv_suppliers_official_chk
  check (IS_OFFICIAL_SUPPLIER IN (0,1));

ALTER TABLE inv_plates ADD ("SOLUTION_VOLUME" NUMBER);
CREATE INDEX PLATE_STATUS_ID_FK_IDX ON INV_PLATES(STATUS_ID_FK) TABLESPACE &&indexTableSpaceName;

ALTER TABLE inv_wells ADD ("SOLUTION_VOLUME" NUMBER);
CREATE INDEX WELLS_WELL_FORMAT_ID_FK_IDX ON INV_WELLS (WELL_FORMAT_ID_FK) TABLESPACE &&indexTableSpaceName;

CREATE TABLE "INV_DATA_MAPS" (
	"DATA_MAP_ID" NUMBER(16),
  "DATA_MAP_NAME" VARCHAR2(50) NOT NULL,
  "DATA_MAP_TYPE_ID_FK" NUMBER(16) NOT NULL,
  "DATA_MAP_COMMENTS" VARCHAR2(2000) NOT NULL,
  "NUM_HEADER_ROWS" NUMBER(4),
  "NUM_COLUMNS" NUMBER(4) NOT NULL,
  "COLUMN_DELIMITER" VARCHAR2(3) NOT NULL,
	"USE_WELL_COORDINATES" NUMBER(1) DEFAULT 0 NOT NULL,  
  CONSTRAINT "INV_DATA_MAPS_TYPEID_FK" 
		FOREIGN KEY("DATA_MAP_TYPE_ID_FK") 
		REFERENCES "INV_ENUMERATION"("ENUM_ID"),
  CONSTRAINT INV_DATA_MAPS_UQ UNIQUE (DATA_MAP_NAME),
	PRIMARY KEY("DATA_MAP_ID") USING INDEX TABLESPACE &&indexTableSpaceName
	)
;
CREATE INDEX INV_DATA_MAPS_TYPEIDFK_IDX ON INV_DATA_MAPS(DATA_MAP_TYPE_ID_FK) TABLESPACE &&indexTableSpaceName;
  
CREATE TABLE "INV_MAP_FIELDS" (
		"MAP_FIELD_ID" NUMBER(16),
    "DISPLAY_NAME" VARCHAR2(50) NOT NULL,
    "TABLE_NAME" VARCHAR2(30),
    "COLUMN_NAME" VARCHAR2(30),
    PRIMARY KEY("MAP_FIELD_ID") USING INDEX TABLESPACE &&indexTableSpaceName
	)
;

CREATE TABLE "INV_DATA_MAPPINGS" (
  "DATA_MAP_ID_FK" NUMBER(16),
  "MAP_FIELD_ID_FK" NUMBER(16),
  "COLUMN_NUMBER" NUMBER(4),
	PRIMARY KEY("DATA_MAP_ID_FK", "MAP_FIELD_ID_FK"),
	CONSTRAINT "INV_DATA_MAPPINGS_DATAMAPID_FK"
		FOREIGN KEY("DATA_MAP_ID_FK")
	    REFERENCES "INV_DATA_MAPS"("DATA_MAP_ID")
    	ON DELETE CASCADE,
	CONSTRAINT "INV_DATA_MAPPINGS_MFIELDID_FK"
		FOREIGN KEY("MAP_FIELD_ID_FK")
	    REFERENCES "INV_MAP_FIELDS"("MAP_FIELD_ID")
    	ON DELETE CASCADE
 	) ORGANIZATION INDEX
;

-- sequences
CREATE SEQUENCE SEQ_INV_DATA_MAPS INCREMENT BY 1 START WITH 1000 MAXVALUE 999999 MINVALUE 1 NOCYCLE NOCACHE ORDER;
CREATE SEQUENCE SEQ_INV_MAP_FIELDS INCREMENT BY 1 START WITH 1000 MAXVALUE 999999 MINVALUE 1 NOCYCLE NOCACHE ORDER;
COMMIT;

--views

--triggers
-- drop old triggers
DROP TRIGGER trg_molar_calcs;
DROP TRIGGER trg_molar_conc;
-- add/replace new triggers
@@..\Update_Scripts\9_to_9SR1\triggers\trg_inv_well_cmpds_molar_calcs.sql
@@..\Update_Scripts\9_to_9SR1\triggers\trg_inv_wells_molar_calcs.sql
@@..\Update_Scripts\9_to_9SR1\triggers\trg_inv_plates_solution_calc.sql

-- Create table level triggers for table INV_DATA_MAPS
CREATE OR REPLACE TRIGGER "TRG_INV_DATA_MAPS" 
    BEFORE INSERT 
    ON "INV_DATA_MAPS" 
    FOR EACH ROW 
    begin
		if :new.DATA_MAP_ID is null then
			select SEQ_INV_DATA_MAPS.nextval INTO :new.DATA_MAP_ID from dual;
		end if;
end;
/
-- Create table level triggers for table INV_MAP_FIELDS
CREATE OR REPLACE TRIGGER "TRG_INV_MAP_FIELDS" 
    BEFORE INSERT 
    ON "INV_MAP_FIELDS" 
    FOR EACH ROW 
    begin
		if :new.MAP_FIELD_ID is null then
			select SEQ_INV_MAP_FIELDS.nextval INTO :new.MAP_FIELD_ID from dual;
		end if;
end;
/

    	
--plsql
@@..\Update_Scripts\9_to_9SR1\FUNCTIONS\f_CheckInDefaultLocation.SQL
@@..\Update_Scripts\9_to_9SR1\FUNCTIONS\f_CopyPlate.sql
@@..\Update_Scripts\9_to_9SR1\FUNCTIONS\f_CreateContainer.sql
@@..\Update_Scripts\9_to_9SR1\FUNCTIONS\f_CreateLocation.sql
@@..\Update_Scripts\9_to_9SR1\FUNCTIONS\f_DeleteTableRow.sql
@@..\Update_Scripts\9_to_9SR1\FUNCTIONS\f_GetPKColumn.sql
@@..\Update_Scripts\9_to_9SR1\FUNCTIONS\f_OrderContainer.sql
@@..\Update_Scripts\9_to_9SR1\FUNCTIONS\f_UpdateAllContainerFields.sql
@@..\Update_Scripts\9_to_9SR1\FUNCTIONS\f_UpdateTable.sql
@@..\Update_Scripts\9_to_9SR1\PACKAGES\pkg_DataMaps_def.SQL
@@..\Update_Scripts\9_to_9SR1\PACKAGES\pkg_DataMaps_body.SQL
@@..\Update_Scripts\9_to_9SR1\PaCKAGES\pkg_PlateChem_def.sql
@@..\Update_Scripts\9_to_9SR1\PaCKAGES\pkg_PlateChem_body.sql
@@..\Update_Scripts\9_to_9SR1\PaCKAGES\pkg_Reformat_def.sql
@@..\Update_Scripts\9_to_9SR1\PaCKAGES\pkg_Reformat_body.sql

--inv roles grants
GRANT SELECT ON INV_DATA_MAPS TO INV_BROWSER;
GRANT SELECT ON INV_DATA_MAPPINGS TO INV_BROWSER;
GRANT SELECT ON INV_MAP_FIELDS TO INV_BROWSER;

GRANT INSERT,UPDATE,DELETE ON INV_DATA_MAPS TO INV_ADMIN;
GRANT INSERT,UPDATE,DELETE ON INV_DATA_MAPPINGS TO INV_ADMIN;
GRANT INSERT,UPDATE,DELETE ON INV_MAP_FIELDS TO INV_ADMIN;
GRANT EXECUTE ON DATAMAPS TO INV_ADMIN;

--cs_security grants
GRANT SELECT ON INV_DATA_MAPS TO CS_SECURITY WITH GRANT OPTION;
GRANT SELECT ON INV_DATA_MAPPINGS TO CS_SECURITY WITH GRANT OPTION;
GRANT SELECT ON INV_MAP_FIELDS TO CS_SECURITY WITH GRANT OPTION;
GRANT EXECUTE ON DATAMAPS TO CS_SECURITY WITH GRANT OPTION;
GRANT EXECUTE ON "&&SchemaName".CHECKINDEFAULTLOCATION to CS_SECURITY WITH GRANT OPTION;

--' update formgroup data
UPDATE db_formgroup SET formgroup_name = upper(formgroup_name);
commit;

-- insert picklist data
INSERT INTO inv_enumeration_set VALUES (8, 'Data Map Type', 1);
INSERT INTO inv_enumeration (enum_id, enum_value, eset_id_fk) VALUES (19, 'Plate Import Template', 8);

INSERT INTO inv_map_fields VALUES (1, 'Source Plate Barcode', 'inv_plates', 'plate_barcode');
INSERT INTO inv_map_fields VALUES (2, 'Source Well', NULL, NULL);
INSERT INTO inv_map_fields VALUES (3, 'Target Plate Barcode', 'inv_plates', 'plate_barcode');
INSERT INTO inv_map_fields VALUES (4, 'Target Well', NULL, NULL);
INSERT INTO inv_map_fields VALUES (5, 'Initial Quantity', 'inv_wells', 'qty_initial');
INSERT INTO inv_map_fields VALUES (6, 'Quantity Unit ID', 'inv_wells', 'qty_unit_fk');
INSERT INTO inv_map_fields VALUES (7, 'Concentration', 'inv_wells', 'concentration');
INSERT INTO inv_map_fields VALUES (8, 'Concentration Untit ID', 'inv_wells', 'conc_unit_fk');
INSERT INTO inv_map_fields VALUES (9, 'Solvent ID', 'inv_wells', 'solvent_id_fk');
INSERT INTO inv_map_fields VALUES (10, 'Plate Field 1', 'inv_plates', 'field_1');
INSERT INTO inv_map_fields VALUES (11, 'Plate Field 2', 'inv_plates', 'field_2');
INSERT INTO inv_map_fields VALUES (12, 'Plate Field 3', 'inv_plates', 'field_3');
INSERT INTO inv_map_fields VALUES (13, 'Plate Field 4', 'inv_plates', 'field_4');
INSERT INTO inv_map_fields VALUES (14, 'Plate Field 5', 'inv_plates', 'field_5');
INSERT INTO inv_map_fields VALUES (15, 'Plate Date 1', 'inv_plates', 'date_1');
INSERT INTO inv_map_fields VALUES (16, 'Plate Date 2', 'inv_plates', 'date_2');
INSERT INTO inv_map_fields VALUES (17, 'Well Field 1', 'inv_wells', 'field_1');
INSERT INTO inv_map_fields VALUES (18, 'Well Field 2', 'inv_wells', 'field_2');
INSERT INTO inv_map_fields VALUES (19, 'Well Field 3', 'inv_wells', 'field_3');
INSERT INTO inv_map_fields VALUES (20, 'Well Field 4', 'inv_wells', 'field_4');
INSERT INTO inv_map_fields VALUES (21, 'Well Field 5', 'inv_wells', 'field_5');
INSERT INTO inv_map_fields VALUES (22, 'Well Date 1', 'inv_wells', 'date_1');
INSERT INTO inv_map_fields VALUES (23, 'Well Date 2', 'inv_wells', 'date_2');
INSERT INTO inv_map_fields VALUES (24, 'Solvent Volume', 'inv_wells', 'solvent_volume');
INSERT INTO inv_map_fields VALUES (25, 'Solvent Volume Unit ID', 'inv_wells', 'solvent_volume_unit_id_fk');
INSERT INTO inv_map_fields VALUES (26, 'Solution Volume', 'inv_wells', 'solution_volume');
INSERT INTO inv_map_fields VALUES (27, 'Compound ID', 'inv_wells', 'compound_id_fk');
INSERT INTO inv_map_fields VALUES (28, 'Reg ID', 'inv_wells', 'reg_id_fk');
INSERT INTO inv_map_fields VALUES (29, 'Reg Number', null, null);
INSERT INTO inv_map_fields VALUES (30, 'Batch Number', 'inv_wells', 'batch_number_fk');
commit;


INSERT INTO INV_DATA_MAPS (DATA_MAP_ID, DATA_MAP_NAME, DATA_MAP_TYPE_ID_FK, DATA_MAP_COMMENTS, NUM_HEADER_ROWS, NUM_COLUMNS, COLUMN_DELIMITER, USE_WELL_COORDINATES)
	VALUES (1, 'CS Default', 19, 'This is the default CambridgeSoft template.', 0, 23, ',', 0);
INSERT INTO INV_DATA_MAPPINGS (DATA_MAP_ID_FK, MAP_FIELD_ID_FK, COLUMN_NUMBER) VALUES (1, 1, 1);
INSERT INTO INV_DATA_MAPPINGS (DATA_MAP_ID_FK, MAP_FIELD_ID_FK, COLUMN_NUMBER) VALUES (1, 2, 2);
INSERT INTO INV_DATA_MAPPINGS (DATA_MAP_ID_FK, MAP_FIELD_ID_FK, COLUMN_NUMBER) VALUES (1, 3, 3);
INSERT INTO INV_DATA_MAPPINGS (DATA_MAP_ID_FK, MAP_FIELD_ID_FK, COLUMN_NUMBER) VALUES (1, 4, 4);
INSERT INTO INV_DATA_MAPPINGS (DATA_MAP_ID_FK, MAP_FIELD_ID_FK, COLUMN_NUMBER) VALUES (1, 5, 5);
INSERT INTO INV_DATA_MAPPINGS (DATA_MAP_ID_FK, MAP_FIELD_ID_FK, COLUMN_NUMBER) VALUES (1, 6, 6);
INSERT INTO INV_DATA_MAPPINGS (DATA_MAP_ID_FK, MAP_FIELD_ID_FK, COLUMN_NUMBER) VALUES (1, 7, 7);
INSERT INTO INV_DATA_MAPPINGS (DATA_MAP_ID_FK, MAP_FIELD_ID_FK, COLUMN_NUMBER) VALUES (1, 8, 8);
INSERT INTO INV_DATA_MAPPINGS (DATA_MAP_ID_FK, MAP_FIELD_ID_FK, COLUMN_NUMBER) VALUES (1, 9, 9);
INSERT INTO INV_DATA_MAPPINGS (DATA_MAP_ID_FK, MAP_FIELD_ID_FK, COLUMN_NUMBER) VALUES (1, 10, 10);
INSERT INTO INV_DATA_MAPPINGS (DATA_MAP_ID_FK, MAP_FIELD_ID_FK, COLUMN_NUMBER) VALUES (1, 11, 11);
INSERT INTO INV_DATA_MAPPINGS (DATA_MAP_ID_FK, MAP_FIELD_ID_FK, COLUMN_NUMBER) VALUES (1, 12, 12);
INSERT INTO INV_DATA_MAPPINGS (DATA_MAP_ID_FK, MAP_FIELD_ID_FK, COLUMN_NUMBER) VALUES (1, 13, 13);
INSERT INTO INV_DATA_MAPPINGS (DATA_MAP_ID_FK, MAP_FIELD_ID_FK, COLUMN_NUMBER) VALUES (1, 14, 14);
INSERT INTO INV_DATA_MAPPINGS (DATA_MAP_ID_FK, MAP_FIELD_ID_FK, COLUMN_NUMBER) VALUES (1, 15, 15);
INSERT INTO INV_DATA_MAPPINGS (DATA_MAP_ID_FK, MAP_FIELD_ID_FK, COLUMN_NUMBER) VALUES (1, 16, 16);
INSERT INTO INV_DATA_MAPPINGS (DATA_MAP_ID_FK, MAP_FIELD_ID_FK, COLUMN_NUMBER) VALUES (1, 17, 17);
INSERT INTO INV_DATA_MAPPINGS (DATA_MAP_ID_FK, MAP_FIELD_ID_FK, COLUMN_NUMBER) VALUES (1, 18, 18);
INSERT INTO INV_DATA_MAPPINGS (DATA_MAP_ID_FK, MAP_FIELD_ID_FK, COLUMN_NUMBER) VALUES (1, 19, 19);
INSERT INTO INV_DATA_MAPPINGS (DATA_MAP_ID_FK, MAP_FIELD_ID_FK, COLUMN_NUMBER) VALUES (1, 20, 20);
INSERT INTO INV_DATA_MAPPINGS (DATA_MAP_ID_FK, MAP_FIELD_ID_FK, COLUMN_NUMBER) VALUES (1, 21, 21);
INSERT INTO INV_DATA_MAPPINGS (DATA_MAP_ID_FK, MAP_FIELD_ID_FK, COLUMN_NUMBER) VALUES (1, 22, 22);
INSERT INTO INV_DATA_MAPPINGS (DATA_MAP_ID_FK, MAP_FIELD_ID_FK, COLUMN_NUMBER) VALUES (1, 23, 23);
INSERT INTO INV_DATA_MAPPINGS (DATA_MAP_ID_FK, MAP_FIELD_ID_FK, COLUMN_NUMBER) VALUES (1, 24, 24);
INSERT INTO INV_DATA_MAPPINGS (DATA_MAP_ID_FK, MAP_FIELD_ID_FK, COLUMN_NUMBER) VALUES (1, 25, 25);
INSERT INTO INV_DATA_MAPPINGS (DATA_MAP_ID_FK, MAP_FIELD_ID_FK, COLUMN_NUMBER) VALUES (1, 26, 26);
INSERT INTO INV_DATA_MAPPINGS (DATA_MAP_ID_FK, MAP_FIELD_ID_FK, COLUMN_NUMBER) VALUES (1, 27, 27);
INSERT INTO INV_DATA_MAPPINGS (DATA_MAP_ID_FK, MAP_FIELD_ID_FK, COLUMN_NUMBER) VALUES (1, 28, 28);
INSERT INTO INV_DATA_MAPPINGS (DATA_MAP_ID_FK, MAP_FIELD_ID_FK, COLUMN_NUMBER) VALUES (1, 29, 29);
INSERT INTO INV_DATA_MAPPINGS (DATA_MAP_ID_FK, MAP_FIELD_ID_FK, COLUMN_NUMBER) VALUES (1, 30, 30);

COMMIT;

--update db version
UPDATE GLOBALS SET value = '4.1' WHERE ID = 'VERSION_SCHEMA'; 
commit;

-- update cs_security
connect &&securitySchemaName/&&securitySchemaPass@&&serverName;
INSERT INTO CS_SECURITY.OBJECT_PRIVILEGES VALUES ('INV_BROWSE_ALL', 'SELECT', '&&SchemaName', 'INV_DATA_MAPS');
INSERT INTO CS_SECURITY.OBJECT_PRIVILEGES VALUES ('INV_BROWSE_ALL', 'SELECT', '&&SchemaName', 'INV_DATA_MAPPINGS');
INSERT INTO CS_SECURITY.OBJECT_PRIVILEGES VALUES ('INV_BROWSE_ALL', 'SELECT', '&&SchemaName', 'INV_MAP_FIELDS');

INSERT INTO CS_SECURITY.OBJECT_PRIVILEGES VALUES ('INV_CHECKIN_CONTAINER', 'EXECUTE', '&&SchemaName', 'CHECKINDEFAULTLOCATION');

INSERT INTO CS_SECURITY.OBJECT_PRIVILEGES VALUES ('INV_CREATE_PLATE', 'EXECUTE', '&&SchemaName', 'DATAMAPS');
COMMIT;

-- re-grant these privileges
BEGIN
  FOR l_Role_rec IN (SELECT role_name FROM security_roles, cheminv_privileges WHERE role_id = role_internal_id AND inv_checkin_container = 1)
  LOOP
  	mapprivstorole(l_Role_rec.Role_Name,'INV_CHECKIN_CONTAINER','GRANT');
  END LOOP;

  FOR l_Role_rec IN (SELECT role_name FROM security_roles, cheminv_privileges WHERE role_id = role_internal_id AND inv_create_plate = 1)
  LOOP
  	mapprivstorole(l_Role_rec.Role_Name,'INV_CREATE_PLATE','GRANT');
  END LOOP;

  FOR l_Role_rec IN (SELECT role_name FROM security_roles, cheminv_privileges WHERE role_id = role_internal_id AND inv_browse_all = 1)
  LOOP
  	mapprivstorole(l_Role_rec.Role_Name,'INV_BROWSE_ALL','GRANT');
  END LOOP;

END;
/

-- create public synonyms
Connect &&InstallUser/&&sysPass@&&serverName
DECLARE
	PROCEDURE createSynonym(synName IN varchar2) IS
			n NUMBER;
		BEGIN
			select count(*) INTO n from dba_synonyms where Upper(synonym_name) = synName;
			if n = 0 then
				execute immediate 'CREATE PUBLIC SYNONYM ' || synName || ' FOR &&schemaName..' || synName;
			end if;
		END createSynonym;
BEGIN
	createSynonym('INV_DATA_MAPS');
	createSynonym('INV_DATA_MAPPINGS');
	createSynonym('INV_MAP_FIELDS');
END;
/
Connect &&schemaName/&&schemaPass@&&serverName
@@RecompilePLSQL.sql

--spool off