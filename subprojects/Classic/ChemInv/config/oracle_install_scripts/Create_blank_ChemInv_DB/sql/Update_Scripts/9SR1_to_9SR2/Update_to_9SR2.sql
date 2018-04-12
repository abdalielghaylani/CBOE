set feedback on
--spool ..\Logs\LOG_Update_Inventory_9SR1_to_9SR2.txt

--update tables          

    	
--plsql
@@..\Update_Scripts\9SR1_to_9SR2\FUNCTIONS\f_CheckoutContainer.sql
@@..\Update_Scripts\9SR1_to_9SR2\FUNCTIONS\f_CreatePlateMap.sql
@@..\Update_Scripts\9SR1_to_9SR2\FUNCTIONS\f_DeleteTableRow.sql
@@..\Update_Scripts\9SR1_to_9SR2\Packages\pkg_Barcodes_def.sql
@@..\Update_Scripts\9SR1_to_9SR2\Packages\pkg_Barcodes_body.sql
@@..\Update_Scripts\9SR1_to_9SR2\Packages\pkg_ChemCalcs_body.sql
@@..\Update_Scripts\9SR1_to_9SR2\PACKAGES\pkg_Compounds_body.SQL
@@..\Update_Scripts\9SR1_to_9SR2\PACKAGES\pkg_DataMaps_body.SQL
@@..\Update_Scripts\9SR1_to_9SR2\PACKAGES\pkg_GUIUtils_def.sql
@@..\Update_Scripts\9SR1_to_9SR2\PACKAGES\pkg_GUIUtils_body.sql
@@..\Update_Scripts\9SR1_to_9SR2\PACKAGES\pkg_PlateChem_def.sql
@@..\Update_Scripts\9SR1_to_9SR2\PACKAGES\pkg_PlateChem_body.sql
@@..\Update_Scripts\9SR1_to_9SR2\PACKAGES\pkg_Reformat_def.sql
@@..\Update_Scripts\9SR1_to_9SR2\PACKAGES\pkg_Reformat_body.sql
@@..\Update_Scripts\9SR1_to_9SR2\PACKAGES\pkg_ReportParams_def.sql
@@..\Update_Scripts\9SR1_to_9SR2\PACKAGES\pkg_ReportParams_body.sql
@@..\Update_Scripts\9SR1_to_9SR2\PACKAGES\pkg_Reports_def.sql
@@..\Update_Scripts\9SR1_to_9SR2\PACKAGES\pkg_Reports_body.sql
@@..\Update_Scripts\9SR1_to_9SR2\PACKAGES\pkg_Requests_body.sql
@@..\Update_Scripts\9SR1_to_9SR2\PROCEDURES\proc_UpdateSubstanceHazmatData.sql
@@..\Update_Scripts\9SR1_to_9SR2\TRIGGERS\trg_inv_plates_solution_calc.sql
@@..\Update_Scripts\9SR1_to_9SR2\FUNCTIONS\f_CreatePlateXML.sql
@@..\Update_Scripts\9SR1_to_9SR2\FUNCTIONS\f_CreateContainer.sql

--inv roles grants

--cs_security grants

--' update formgroup data

--' insert picklist data
DECLARE
l_count INTEGER := 0;
BEGIN
SELECT COUNT(*) INTO l_count FROM inv_map_fields WHERE map_field_id = 31;
IF l_count = 0 THEN
			INSERT INTO inv_map_fields VALUES (31, 'N/A', NULL, NULL);
END IF;
END;
/
--update db version
UPDATE GLOBALS SET value = '4.2.1' WHERE ID = 'VERSION_SCHEMA'; 

--' update picklist data
UPDATE inv_data_maps SET num_columns = 30 WHERE data_map_id = 1;
UPDATE inv_map_fields SET display_name = 'Concentration Unit ID' WHERE map_field_id = 8;

commit;
/

-- update cs_security
connect &&securitySchemaName/&&securitySchemaPass@&&serverName;
INSERT INTO &&securitySchemaName..OBJECT_PRIVILEGES VALUES ('INV_CREATE_PLATE', 'EXECUTE', '&&SchemaName', 'REFORMAT');
INSERT INTO &&securitySchemaName..OBJECT_PRIVILEGES VALUES ('INV_CREATE_PLATE', 'EXECUTE', '&&SchemaName', 'UPDATEWELL');
commit;
/
-- create public synonyms

-- import new lookup data
Connect &&schemaName/&&schemaPass@&&serverName
DELETE inv_xslts;
commit;
/
host imp &&schemaName/&&schemaPass@&&serverName parfile= Dump_Files\update_chemInvDB2_picklists_to_9sr2.inp


--' recompile pl/sql
Connect &&schemaName/&&schemaPass@&&serverName
@@RecompilePLSQL.sql

--spool off