-- Copyright Cambridgesoft Corp 2001-2007 all rights reserved

--################################################################
-- Headerfile for updating the cheminvdb2 schema from 9SR2 to 9SR3
--################################################################ 

spool ON
spool ..\..\Logs\LOG_Update_Inventory_9SR2_to_9SR3.txt

--' Intialize variables
@@..\..\Parameters.sql
@@..\..\Prompts.sql
prompt '#########################################################'
prompt 'Creating schema pre-requisites...'
prompt '#########################################################'
Connect &&schemaName/&&schemaPass@&&serverName
@@PLSQL\Packages\pkg_Constants_Def.sql;

--' Inv user needs privileges to add foreign key constraints to the security_roles table
connect &&securitySchemaName/&&securitySchemaPass@&&serverName;
GRANT REFERENCES ON security_roles TO &&schemaName;

prompt '#########################################################'
prompt 'Updating schema tables...'
prompt '#########################################################'
Connect &&schemaName/&&schemaPass@&&serverName
--------------------------------
--  Changed table inv_states  --
--------------------------------
update inv_states set country_id_fk=1000 where country_id_fk=1;
alter table INV_STATES
  add constraint INV_STATES_COUNTRY_ID_FK foreign key (COUNTRY_ID_FK)
  references INV_COUNTRY (COUNTRY_ID);
-----------------------------------
--  New table inv_graphic_types  --
-----------------------------------
@@Create\Tables\Inv_Graphic_Types.sql
------------------------------
--  New table inv_graphics  --
------------------------------
@@Create\Tables\Inv_Graphics.sql
----------------------------------------
--  Changed table inv_location_types  --
----------------------------------------
-- Add/modify columns 
alter table INV_LOCATION_TYPES add GRAPHIC_ID_FK NUMBER(4);
-- Create/Recreate primary, unique and foreign key constraints 
alter table INV_LOCATION_TYPES
  add constraint LOCATION_TYPES_GRAPHIC_ID_FK foreign key (GRAPHIC_ID_FK)
  references INV_GRAPHICS (GRAPHIC_ID);
-----------------------------------
--  Changed table inv_locations  --
-----------------------------------
-- Add/modify columns 
alter table INV_LOCATIONS add COLLAPSE_CHILD_NODES NUMBER(1);
----------------------------------
--  New table inv_batch_status  --
----------------------------------
@@Create\Tables\Inv_Batch_Status.sql
---------------------------------------
--  New table inv_container_batches  --
---------------------------------------
@@Create\Tables\Inv_Container_Batches.sql
------------------------------------
--  Changed table inv_containers  --
------------------------------------
-- Add/modify columns 
alter table INV_CONTAINERS add BATCH_ID_FK NUMBER(4);
-- Create/Recreate primary, unique and foreign key constraints 
alter table INV_CONTAINERS
  add constraint INV_CONT_BATCH_ID_FK foreign key (BATCH_ID_FK)
  references INV_CONTAINER_BATCHES (BATCH_ID);
-- Create/Recreate indexes 
CREATE INDEX CONTAINER_BATCH_ID_FK_IDX ON INV_CONTAINERS(BATCH_ID_FK) TABLESPACE &&indexTableSpaceName;
CREATE INDEX CONTAINER_REG_ID_FK_IDX ON INV_CONTAINERS(REG_ID_FK) TABLESPACE &&indexTableSpaceName;
CREATE INDEX CONTAINER_BATCH_NUMBER_FK_IDX ON INV_CONTAINERS(BATCH_NUMBER_FK) TABLESPACE &&indexTableSpaceName;
--------------------------------------------
--  New table inv_container_batch_fields  --
--------------------------------------------
@@Create\Tables\Inv_Container_Batch_Fields.sql
-----------------------------------------
--  Changed table inv_enumeration_set  --
-----------------------------------------
-- Drop indexes 
drop index ESET_TYPE_ID_FK_IDX;
-------------------------------
--  New table inv_doc_types  --
-------------------------------
@@Create\Tables\Inv_Doc_Types.sql
--------------------------
--  New table inv_docs  --
--------------------------
@@Create\Tables\Inv_Docs.sql
-------------------------------------
--  Changed table inv_grid_format  --
-------------------------------------
-- Add/modify columns 
alter table INV_GRID_FORMAT add CELL_NAMING NUMBER(1);
alter table INV_GRID_FORMAT add NAME_DELIMETER VARCHAR2(50);
------------------------------
--  New table inv_org_unit  --
------------------------------
@@Create\Tables\Inv_Org_Unit.sql
------------------------------
--  New table inv_org_roles  --
-------------------------------
@@Create\Tables\Inv_Org_Roles.sql
-------------------------------
--  New table inv_org_users  --
-------------------------------
@@Create\Tables\Inv_Org_Users.sql
----------------------------------
--  Changed table inv_requests  --
----------------------------------
-- Add/modify columns 
alter table INV_REQUESTS modify CONTAINER_ID_FK null;
alter table INV_REQUESTS modify USER_ID_FK null;
alter table INV_REQUESTS add BATCH_ID_FK NUMBER(4);
alter table INV_REQUESTS add ORG_UNIT_ID_FK NUMBER(4);
alter table INV_REQUESTS add ASSIGNED_USER_ID_FK VARCHAR2(100);
alter table INV_REQUESTS add QTY_DELIVERED NUMBER;
alter table INV_REQUESTS add FIELD_1 VARCHAR2(2000);
alter table INV_REQUESTS add FIELD_2 VARCHAR2(2000);
alter table INV_REQUESTS add FIELD_3 VARCHAR2(2000);
alter table INV_REQUESTS add FIELD_4 VARCHAR2(2000);
alter table INV_REQUESTS add FIELD_5 VARCHAR2(2000);
alter table INV_REQUESTS add DATE_1 DATE;
alter table INV_REQUESTS add DATE_2 DATE;
-- Drop primary, unique and foreign key constraints 
alter table INV_REQUESTS
  drop constraint INV_REQUESTS_USERID_FK;
-- Create/Recreate primary, unique and foreign key constraints 
alter table INV_REQUESTS
  add constraint INV_REQUESTS_BATCH_ID_FK foreign key (BATCH_ID_FK)
  references INV_CONTAINER_BATCHES (BATCH_ID) on delete cascade;
alter table INV_REQUESTS
  add constraint INV_REQ_ASSIGNED_USER_ID_FK foreign key (ASSIGNED_USER_ID_FK)
  references CS_SECURITY.PEOPLE (USER_ID);
-- Create/Recreate indexes 
CREATE INDEX REQUESTS_BATCH_ID_FK_IDX ON INV_REQUESTS(BATCH_ID_FK) TABLESPACE &&indexTableSpaceName;
---------------------------------------------
--  New table inv_unit_conversion_formula  --
---------------------------------------------
@@Create\Tables\Inv_Unit_Conversion_Formula.sql

------------------------------------------
--  New sequence seq_inv_reportformats  --
------------------------------------------
CREATE SEQUENCE SEQ_INV_REPORTFORMATS INCREMENT BY 1 START WITH 1000;
------------------------------------------------
--  Changed view inv_vw_plate_grid_locations  --
------------------------------------------------
create or replace view inv_vw_plate_grid_locations
(location_id, location_name, location_description, location_barcode, row_count, col_count, plate_type_id, plate_type_name)
as
select
	inv_vw_grid_location_parent."LOCATION_ID",inv_vw_grid_location_parent."LOCATION_NAME",inv_vw_grid_location_parent."LOCATION_DESCRIPTION",inv_vw_grid_location_parent."LOCATION_BARCODE",inv_vw_grid_location_parent."ROW_COUNT",inv_vw_grid_location_parent."COL_COUNT",
	inv_allowed_ptypes.plate_type_id_fk,
	inv_plate_types.plate_type_name
from inv_vw_grid_location_parent, inv_allowed_ptypes, inv_plate_types
where inv_allowed_ptypes.location_id_fk = inv_vw_grid_location_parent.location_id
and inv_allowed_ptypes.plate_type_id_fk = inv_plate_types.plate_type_id
/

----------------------------------
--  recreate all the views  --
----------------------------------
prompt '#########################################################'
prompt 'Creating schema views...'
prompt '#########################################################'

@@Create\Views\Inv_VW_Physical_Plate.sql
@@Create\Views\Inv_VW_Plate_Format.sql
@@Create\Views\Inv_VW_Plate.sql
@@Create\Views\Inv_VW_Well_Format.sql
@@Create\Views\Inv_VW_Well.sql
@@Create\Views\Inv_VW_Well_Flat.sql
@@Create\Views\Inv_VW_Grid_Location.sql
@@Create\Views\Inv_VW_NonGrid_Locations.sql
@@Create\Views\INV_VW_Grid_Location_Parent.sql
@@Create\Views\Inv_VW_Plate_Grid_Locations.sql
@@Create\Views\Inv_VW_Plate_Locations.sql
@@Create\Views\Inv_VW_Enumerated_Values.sql
@@Create\Views\Inv_VW_Plate_History.sql
@@Create\Views\Inv_VW_Grid_Location_Lite.sql
@@Create\Views\Inv_VW_Plate_Locations_All.sql
@@Create\Views\Inv_VW_Audit_Column_Disp.sql
@@Create\Views\Inv_VW_Audit_Aggregate.sql;
@@Create\Views\INV_VW_Reg_Structures.sql
@@Create\Views\INV_VW_Reg_Batches.sql
@@Create\Views\INV_VW_Reg_AltIDs.sql

prompt '#########################################################'
prompt 'Updating schema tables...'
prompt '#########################################################'
----------------------------------
--  recreate all the pl/sql  --
----------------------------------
prompt '#########################################################'
prompt 'Updating Packages...'
prompt '#########################################################'

@@PLSQL\Packages\pkg_Stringutils_Def.sql;
@@PLSQL\Packages\pkg_Stringutils_Body.sql;
@@PLSQL\Packages\pkg_Barcodes_Def.sql;
@@PLSQL\Packages\pkg_Barcodes_Body.sql;
@@PLSQL\Packages\pkg_Reservations_Def.sql;
@@PLSQL\Packages\pkg_Reservations_Body.sql;
@@PLSQL\Packages\pkg_Xmlutils_Def.sql;
@@PLSQL\Packages\pkg_Xmlutils_Body.sql;
@@PLSQL\Packages\pkg_Chemcalcs_Def.sql;
@@PLSQL\Packages\pkg_Chemcalcs_Body.sql;
@@PLSQL\Packages\pkg_Platechem_Def.sql;
@@PLSQL\Packages\pkg_Platechem_Body.sql;
@@PLSQL\Packages\pkg_Approvals_Def.sql
@@PLSQL\Packages\pkg_Approvals_Body.sql
@@PLSQL\Packages\pkg_Guiutils_Def.sql;
@@PLSQL\Packages\pkg_Guiutils_Body.sql;
@@PLSQL\Packages\pkg_Racks_def.sql;
@@PLSQL\Packages\pkg_Racks_Body.sql;
@@PLSQL\Packages\pkg_Docs_def.sql;
@@PLSQL\Packages\pkg_Docs_Body.sql;
@@PLSQL\Packages\pkg_Requests_Def.sql;
@@PLSQL\Packages\pkg_Requests_Body.sql;
@@PLSQL\Packages\pkg_Compounds_Def.sql;
@@PLSQL\Packages\pkg_Compounds_Body.sql;
@@PLSQL\Packages\pkg_Datamaps_Def.sql
@@PLSQL\Packages\pkg_Datamaps_Body.sql
@@PLSQL\Packages\pkg_Links_Def.sql;
@@PLSQL\Packages\pkg_Links_Body.sql;
@@PLSQL\Packages\pkg_Reformat_Def.sql;
@@PLSQL\Packages\pkg_Reformat_Body.sql;
@@PLSQL\Packages\pkg_Platesettings_Def.sql;
@@PLSQL\Packages\pkg_Platesettings_Body.sql;
@@PLSQL\Packages\pkg_Reports_Def.sql
@@PLSQL\Packages\pkg_Reports_Body.sql
@@PLSQL\Packages\pkg_Reportparams_Def.sql
@@PLSQL\Packages\pkg_Reportparams_Body.sql
@@PLSQL\Packages\pkg_Audit_trail_def.sql;
@@PLSQL\Packages\pkg_Audit_trail_Body.sql;
@@PLSQL\Packages\pkg_Organization_def.sql;
@@PLSQL\Packages\pkg_Organization_Body.sql;
--' this procedure is used in the Batch package
@@PLSQL\Procedures\proc_UpdateContainerBatches.sql
@@PLSQL\Packages\pkg_Batch_def.sql;
@@PLSQL\Packages\pkg_Batch_Body.sql;
@@PLSQL\Packages\pkg_CTX_Cheminv_Mgr_def.SQL;
@@PLSQL\Packages\pkg_CTX_Cheminv_Mgr_Body.SQL;


prompt '#########################################################'
prompt 'Updating functions...'
prompt '#########################################################'

@@PLSQL\Functions\f_SolvatePlates.sql
@@PLSQL\Functions\f_IsContainerTypeAllowed.sql
@@PLSQL\Functions\f_ExcludeContainerTypes.sql
@@PLSQL\Functions\f_GetCompoundID.sql
@@PLSQL\Functions\f_IsDuplicateCompound.sql
@@PLSQL\Functions\f_UpdateContainerStatus.sql
@@PLSQL\Functions\f_EnableGridForLocation.sql
@@PLSQL\Functions\f_CheckoutContainer.sql
@@PLSQL\Functions\f_AssignPlateTypesToLocation.sql
@@PLSQL\Functions\f_RetireContainer.sql
@@PLSQL\Functions\f_GetLocationPath.sql
@@PLSQL\Functions\f_CreateContainer.sql
@@PLSQL\Functions\f_CopyContainer.sql
@@PLSQL\Functions\f_CopyPlate.sql
@@PLSQL\Functions\f_UpdateContainer.sql
@@PLSQL\Functions\f_CreateLocation.sql
@@PLSQL\Functions\f_DeleteContainer.sql
@@PLSQL\Functions\f_GetGridID.sql
@@PLSQL\Functions\f_GetGridFormatID.sql
@@PLSQL\Functions\f_DeleteLocationGrid.sql
@@PLSQL\Functions\f_DeleteLocation.sql
@@PLSQL\Functions\f_GetCompoundIDFromName.sql
@@PLSQL\Functions\f_MoveContainer.sql
@@PLSQL\Functions\f_MoveLocation.sql
@@PLSQL\Functions\f_SubstanceNameExists.sql
@@PLSQL\Functions\f_UpdateAllContainerFields.sql
@@PLSQL\Functions\f_UpdateContainerQtyRemaining.sql
@@PLSQL\Functions\f_GetGridStorageID.sql
@@PLSQL\Functions\f_UpdateLocation.sql
@@PLSQL\Functions\f_OrderContainer.sql
@@PLSQL\Functions\f_ReorderContainer.sql
@@PLSQL\Functions\f_SelectHazmatData.sql
@@PLSQL\Functions\f_SelectSubstanceHazmatData.sql
@@PLSQL\Functions\f_CreateTableRow.SQL
@@PLSQL\Functions\f_DeleteTableRow.sql
@@PLSQL\Functions\f_DeletePlates.sql
@@PLSQL\Functions\f_GetPKColumn.sql
@@PLSQL\Functions\f_SetPlateFTCycle.sql
@@PLSQL\Functions\f_MovePlates.sql
@@PLSQL\Functions\f_RetirePlate.sql
@@PLSQL\Functions\f_UpdatePlateAttributes.sql
@@PLSQL\Functions\f_UpdateTable.sql
@@PLSQL\Functions\f_UpdateWell.sql

--@@Procedures\proc_InsertXMLDoc.sql
--@@Procedures\proc_InsertXSTL.sql

@@PLSQL\Functions\f_CreatePlateXML.sql
@@PLSQL\Functions\f_CreatePlateMap.sql
@@PLSQL\Functions\f_IsPlateTypeAllowed.sql   
@@PLSQL\Functions\f_IsTrashLocation.sql
@@PLSQL\Functions\f_EmptyTrash.sql
@@PLSQL\Functions\f_LookUpValue.sql
@@PLSQL\Functions\f_UpdateAddress.sql
@@PLSQL\Functions\f_CertifyContainer.sql
@@PLSQL\Functions\f_InsertCheckInDetails.sql
@@PLSQL\Functions\f_CheckInDefaultLocation.SQL

-- procedures from Alter_Cheminv_Plate_Procs.sql

@@PLSQL\Functions\f_CreateGridFormat.sql
@@PLSQL\Functions\f_CreatePhysPlateType.sql
@@PLSQL\Functions\f_DeletePhysPlateType.sql
@@PLSQL\Functions\f_UpdatePhysPlateType.sql
@@PLSQL\Functions\f_UpdateWellFormat.sql
@@PLSQL\Functions\f_DeletePlateFormat.sql
@@PLSQL\Functions\f_UpdatePlateFormat.sql
@@PLSQL\Functions\f_CreatePlateFormat.sql
@@PLSQL\Functions\f_DeletePlate.sql
@@PLSQL\Functions\f_AssignLocationToGrid.sql
@@PLSQL\Functions\f_GetNumberOfCompoundWells.sql
@@PLSQL\Functions\f_IsGridLocation.sql
@@PLSQL\Functions\f_IsFrozenLocation.sql
@@PLSQL\Functions\f_MovePlate.sql        

prompt '#########################################################'
prompt 'Updating procedures...'
prompt '#########################################################'

@@PLSQL\Procedures\proc_InsertIntoCustomChemOrder.sql
@@PLSQL\Procedures\proc_InsertHazmatData.sql
@@PLSQL\Procedures\proc_UpdateHazmatData.sql
@@PLSQL\Procedures\proc_InsertSubstanceHazmatData.sql
@@PLSQL\Procedures\proc_UpdateSubstanceHazmatData.sql
@@PLSQL\Procedures\proc_UpdateBatchStatus.sql

-----------------------------------------
--  New trigger trg_inv_reportformats  --
-----------------------------------------
CREATE OR REPLACE TRIGGER "TRG_INV_REPORTFORMATS" 
    BEFORE INSERT 
    ON "INV_REPORTFORMATS" 
    FOR EACH ROW WHEN (new.id is null or new.id = 0) begin
			select SEQ_INV_REPORTFORMATS .nextval into :new.ID from dual;
end;
/

--' Alter cs_security to integrate the Inventory application
prompt '#########################################################'
prompt 'Altering the cs_security schema...'
prompt '#########################################################'

--' Grant all cheminvdb2 object permissions to CS_SECURITY   
Connect &&schemaName/&&schemaPass@&&serverName
GRANT SELECT ON "&&SchemaName".INV_CONTAINER_BATCHES TO CS_SECURITY WITH GRANT OPTION;
GRANT SELECT ON "&&SchemaName".INV_BATCH_STATUS TO CS_SECURITY WITH GRANT OPTION;
GRANT SELECT ON "&&SchemaName".INV_UNIT_CONVERSION_FORMULA TO CS_SECURITY WITH GRANT OPTION;
GRANT SELECT ON "&&SchemaName".INV_GRAPHICS TO CS_SECURITY WITH GRANT OPTION;
GRANT SELECT ON "&&SchemaName".INV_GRAPHIC_TYPES TO CS_SECURITY WITH GRANT OPTION;
GRANT SELECT ON "&&SchemaName".INV_DOCS TO CS_SECURITY WITH GRANT OPTION;
GRANT SELECT ON "&&SchemaName".INV_DOC_TYPES TO CS_SECURITY WITH GRANT OPTION;
GRANT SELECT ON "&&SchemaName".INV_ORG_UNIT TO CS_SECURITY WITH GRANT OPTION;
GRANT SELECT ON "&&SchemaName".INV_ORG_ROLES TO CS_SECURITY WITH GRANT OPTION;
GRANT SELECT ON "&&SchemaName".INV_ORG_USERS TO CS_SECURITY WITH GRANT OPTION;
GRANT SELECT ON "&&SchemaName".INV_VW_GRID_LOCATION_LITE TO CS_SECURITY WITH GRANT OPTION;
GRANT EXECUTE ON "&&SchemaName".RACKS TO CS_SECURITY WITH GRANT OPTION;
GRANT EXECUTE ON "&&SchemaName".BATCH TO CS_SECURITY WITH GRANT OPTION;
GRANT EXECUTE ON "&&SchemaName".DOCS TO CS_SECURITY WITH GRANT OPTION;
GRANT EXECUTE ON "&&SchemaName".ORGANIZATION TO CS_SECURITY WITH GRANT OPTION;
GRANT SELECT ON "&&SchemaName".INV_VW_REG_ALTIDS TO CS_SECURITY WITH GRANT OPTION;
GRANT SELECT ON "&&SchemaName".INV_VW_REG_BATCHES TO CS_SECURITY WITH GRANT OPTION;
GRANT SELECT ON "&&SchemaName".INV_VW_REG_STRUCTURES TO CS_SECURITY WITH GRANT OPTION;


--inv roles grants
grant SELECT on "&&SchemaName".INV_CONTAINER_BATCHES TO Inv_Browser;
grant SELECT on "&&SchemaName".INV_BATCH_STATUS TO Inv_Browser;
grant SELECT on "&&SchemaName".INV_UNIT_CONVERSION_FORMULA TO Inv_Browser;
grant SELECT on "&&SchemaName".INV_GRAPHICS TO Inv_Browser;
grant SELECT on "&&SchemaName".INV_GRAPHIC_TYPES TO Inv_Browser;
grant SELECT on "&&SchemaName".INV_DOCS TO Inv_Browser;
grant SELECT on "&&SchemaName".INV_DOC_TYPES TO Inv_Browser;
grant SELECT on "&&SchemaName".INV_ORG_UNIT TO Inv_Browser;
grant SELECT on "&&SchemaName".INV_ORG_ROLES TO Inv_Browser;
grant SELECT on "&&SchemaName".INV_ORG_USERS TO Inv_Browser;
grant EXECUTE on "&&SchemaName".DOCS TO Inv_Browser;
grant EXECUTE on "&&SchemaName".RACKS TO Inv_Browser;
grant EXECUTE on "&&SchemaName".ORGANIZATION TO Inv_Browser;
grant EXECUTE on "&&SchemaName".BATCH TO Inv_Browser;
grant SELECT on "&&SchemaName".INV_VW_REG_BATCHES TO Inv_Browser;
grant SELECT on "&&SchemaName".INV_VW_REG_STRUCTURES TO Inv_Browser;
grant SELECT on "&&SchemaName".INV_VW_REG_ALTIDS TO Inv_Browser;
GRANT EXECUTE ON "&&SchemaName".CREATECONTAINER TO INV_REGISTRAR; 
GRANT EXECUTE ON "&&SchemaName".CreatePlateFormat TO INV_ADMIN;
GRANT EXECUTE ON "&&SchemaName".UpdatePlateFormat TO INV_ADMIN;
GRANT EXECUTE ON "&&SchemaName".DeletePlateFormat TO INV_ADMIN;
GRANT EXECUTE ON "&&SchemaName".UpdateWellFormat TO INV_ADMIN;

--' Create cs_security objects and data for Inventory 
connect &&securitySchemaName/&&securitySchemaPass@&&serverName;
INSERT INTO CS_SECURITY.OBJECT_PRIVILEGES VALUES ('INV_BROWSE_ALL', 'SELECT', '&&SchemaName', 'INV_CONTAINER_BATCHES');
INSERT INTO CS_SECURITY.OBJECT_PRIVILEGES VALUES ('INV_BROWSE_ALL', 'SELECT', '&&SchemaName', 'INV_BATCH_STATUS');
INSERT INTO CS_SECURITY.OBJECT_PRIVILEGES VALUES ('INV_BROWSE_ALL', 'SELECT', '&&SchemaName', 'INV_UNIT_CONVERSION_FORMULA');
INSERT INTO CS_SECURITY.OBJECT_PRIVILEGES VALUES ('INV_BROWSE_ALL', 'SELECT', '&&SchemaName', 'INV_GRAPHICS');
INSERT INTO CS_SECURITY.OBJECT_PRIVILEGES VALUES ('INV_BROWSE_ALL', 'SELECT', '&&SchemaName', 'INV_GRAPHIC_TYPES');
INSERT INTO CS_SECURITY.OBJECT_PRIVILEGES VALUES ('INV_BROWSE_ALL', 'SELECT', '&&SchemaName', 'INV_DOCS');
INSERT INTO CS_SECURITY.OBJECT_PRIVILEGES VALUES ('INV_BROWSE_ALL', 'SELECT', '&&SchemaName', 'INV_DOC_TYPES');
INSERT INTO CS_SECURITY.OBJECT_PRIVILEGES VALUES ('INV_BROWSE_ALL', 'SELECT', '&&SchemaName', 'INV_ORG_UNIT');
INSERT INTO CS_SECURITY.OBJECT_PRIVILEGES VALUES ('INV_BROWSE_ALL', 'SELECT', '&&SchemaName', 'INV_ORG_ROLES');
INSERT INTO CS_SECURITY.OBJECT_PRIVILEGES VALUES ('INV_BROWSE_ALL', 'SELECT', '&&SchemaName', 'INV_ORG_USERS');
INSERT INTO CS_SECURITY.OBJECT_PRIVILEGES VALUES ('INV_BROWSE_ALL', 'EXECUTE', '&&SchemaName', 'RACKS');
INSERT INTO CS_SECURITY.OBJECT_PRIVILEGES VALUES ('INV_BROWSE_ALL', 'EXECUTE', '&&SchemaName', 'DOCS');
INSERT INTO CS_SECURITY.OBJECT_PRIVILEGES VALUES ('INV_BROWSE_ALL', 'EXECUTE', '&&SchemaName', 'ORGANIZATION');
INSERT INTO CS_SECURITY.OBJECT_PRIVILEGES VALUES ('INV_BROWSE_ALL', 'EXECUTE', '&&SchemaName', 'BATCH');
INSERT INTO CS_SECURITY.OBJECT_PRIVILEGES VALUES ('INV_BROWSE_ALL', 'SELECT', '&&SchemaName', 'INV_VW_REG_BATCHES');
INSERT INTO CS_SECURITY.OBJECT_PRIVILEGES VALUES ('INV_BROWSE_ALL', 'SELECT', '&&SchemaName', 'INV_VW_REG_STRUCTURES');
INSERT INTO CS_SECURITY.OBJECT_PRIVILEGES VALUES ('INV_BROWSE_ALL', 'SELECT', '&&SchemaName', 'INV_VW_REG_ALTIDS');
INSERT INTO CS_SECURITY.OBJECT_PRIVILEGES VALUES ('INV_CREATE_PLATE', 'EXECUTE', '&&SchemaName', 'CREATEPLATEFORMAT');
commit;

connect &&InstallUser/&&sysPass@&&serverName

prompt '#########################################################'
prompt 'Creating synonyms ...'
prompt '#########################################################'

DECLARE
	PROCEDURE createSynonym(synName IN varchar2) IS
			n NUMBER;
		BEGIN
			select count(*) into n from dba_synonyms where Upper(synonym_name) = synName;
			if n = 0 then
				execute immediate 'CREATE PUBLIC SYNONYM ' || synName || ' FOR &&schemaName..' || synName;
			end if;
		END createSynonym;
BEGIN
	createSynonym('INV_GRAPHICS');
	createSynonym('INV_GRAPHIC_TYPES');
	createSynonym('INV_CONTAINER_BATCHES');
	createSynonym('INV_UNIT_CONVERSION_FORMULA');
	createSynonym('INV_DOCS');
	createSynonym('INV_DOC_TYPES');
	createSynonym('INV_ORG_UNIT');
	createSynonym('INV_ORG_ROLES');
	createSynonym('INV_ORG_USERS');
	createSynonym('INV_BATCH_STATUS');
	createSynonym('inv_vw_grid_location_lite');
	createSynonym('INV_VW_REG_BATCHES');
	createSynonym('INV_VW_REG_STRUCTURES');
	createSynonym('INV_VW_REG_ALTIDS');

END;
/                                           

--' Insert application data
Connect &&schemaName/&&schemaPass@&&serverName
@@Data\TableData\Globals.sql
@@Data\TableData\Inv_Graphic_Types.sql
@@Data\TableData\Inv_Graphics.sql
@@Data\TableData\Inv_Batch_Status.sql
@@Data\TableData\Inv_Doc_Types.sql
@@Data\TableData\Inv_Country.sql
@@Data\TableData\Inv_States.sql
@@Data\TableData\Inv_Xslts.sql
@@Data\TableData\Inv_Location_Types.sql

@@Data\IncrementSequences.sql

--update db version
UPDATE GLOBALS SET value = '4.3' WHERE ID = 'VERSION_SCHEMA'; 

--' Recompile pl/sql 
@@PLSQL\RecompilePLSQL.sql

prompt '#########################################################'
prompt 'Logged session to: Logs\LOG_Update_Inventory_9SR2_to_9SR3.txt'
prompt '#########################################################'

prompt 
spool off

exit


	