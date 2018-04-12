--spool ON
--spool ..\Logs\LOG_Update_Inventory_9SR2_to_9SR3.txt

prompt '#########################################################'
prompt 'Creating schema pre-requisites...'
prompt '#########################################################'
Connect &&schemaName/&&schemaPass@&&serverName
@@..\Update_Scripts\9SR2_to_9SR3\PLSQL\Packages\pkg_Constants_Def.sql;

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
@@..\Update_Scripts\9SR2_to_9SR3\Create\Tables\Inv_Graphic_Types.sql
------------------------------
--  New table inv_graphics  --
------------------------------
@@..\Update_Scripts\9SR2_to_9SR3\Create\Tables\Inv_Graphics.sql
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
@@..\Update_Scripts\9SR2_to_9SR3\Create\Tables\Inv_Batch_Status.sql
---------------------------------------
--  New table inv_container_batches  --
---------------------------------------
@@..\Update_Scripts\9SR2_to_9SR3\Create\Tables\Inv_Container_Batches.sql
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
@@..\Update_Scripts\9SR2_to_9SR3\Create\Tables\Inv_Container_Batch_Fields.sql
-----------------------------------------
--  Changed table inv_enumeration_set  --
-----------------------------------------
-- Drop indexes 
drop index ESET_TYPE_ID_FK_IDX;
-------------------------------
--  New table inv_doc_types  --
-------------------------------
@@..\Update_Scripts\9SR2_to_9SR3\Create\Tables\Inv_Doc_Types.sql
--------------------------
--  New table inv_docs  --
--------------------------
@@..\Update_Scripts\9SR2_to_9SR3\Create\Tables\Inv_Docs.sql
-------------------------------------
--  Changed table inv_grid_format  --
-------------------------------------
-- Add/modify columns 
alter table INV_GRID_FORMAT add CELL_NAMING NUMBER(1);
alter table INV_GRID_FORMAT add NAME_DELIMETER VARCHAR2(50);
------------------------------
--  New table inv_org_unit  --
------------------------------
@@..\Update_Scripts\9SR2_to_9SR3\Create\Tables\Inv_Org_Unit.sql
------------------------------
--  New table inv_org_roles  --
-------------------------------
@@..\Update_Scripts\9SR2_to_9SR3\Create\Tables\Inv_Org_Roles.sql
-------------------------------
--  New table inv_org_users  --
-------------------------------
@@..\Update_Scripts\9SR2_to_9SR3\Create\Tables\Inv_Org_Users.sql
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
@@..\Update_Scripts\9SR2_to_9SR3\Create\Tables\Inv_Unit_Conversion_Formula.sql

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

@@..\Update_Scripts\9SR2_to_9SR3\Create\Views\Inv_VW_Physical_Plate.sql
@@..\Update_Scripts\9SR2_to_9SR3\Create\Views\Inv_VW_Plate_Format.sql
@@..\Update_Scripts\9SR2_to_9SR3\Create\Views\Inv_VW_Plate.sql
@@..\Update_Scripts\9SR2_to_9SR3\Create\Views\Inv_VW_Well_Format.sql
@@..\Update_Scripts\9SR2_to_9SR3\Create\Views\Inv_VW_Well.sql
@@..\Update_Scripts\9SR2_to_9SR3\Create\Views\Inv_VW_Well_Flat.sql
@@..\Update_Scripts\9SR2_to_9SR3\Create\Views\Inv_VW_Grid_Location.sql
@@..\Update_Scripts\9SR2_to_9SR3\Create\Views\Inv_VW_NonGrid_Locations.sql
@@..\Update_Scripts\9SR2_to_9SR3\Create\Views\INV_VW_Grid_Location_Parent.sql
@@..\Update_Scripts\9SR2_to_9SR3\Create\Views\Inv_VW_Plate_Grid_Locations.sql
@@..\Update_Scripts\9SR2_to_9SR3\Create\Views\Inv_VW_Plate_Locations.sql
@@..\Update_Scripts\9SR2_to_9SR3\Create\Views\Inv_VW_Enumerated_Values.sql
@@..\Update_Scripts\9SR2_to_9SR3\Create\Views\Inv_VW_Plate_History.sql
@@..\Update_Scripts\9SR2_to_9SR3\Create\Views\Inv_VW_Grid_Location_Lite.sql
@@..\Update_Scripts\9SR2_to_9SR3\Create\Views\Inv_VW_Plate_Locations_All.sql
@@..\Update_Scripts\9SR2_to_9SR3\Create\Views\Inv_VW_Audit_Column_Disp.sql
@@..\Update_Scripts\9SR2_to_9SR3\Create\Views\Inv_VW_Audit_Aggregate.sql;
@@..\Update_Scripts\9SR2_to_9SR3\Create\Views\INV_VW_Reg_Structures.sql
@@..\Update_Scripts\9SR2_to_9SR3\Create\Views\INV_VW_Reg_Batches.sql
@@..\Update_Scripts\9SR2_to_9SR3\Create\Views\INV_VW_Reg_AltIDs.sql

prompt '#########################################################'
prompt 'Updating schema tables...'
prompt '#########################################################'
----------------------------------
--  recreate all the pl/sql  --
----------------------------------
prompt '#########################################################'
prompt 'Updating Packages...'
prompt '#########################################################'

@@..\Update_Scripts\9SR2_to_9SR3\PLSQL\Packages\pkg_Stringutils_Def.sql;
@@..\Update_Scripts\9SR2_to_9SR3\PLSQL\Packages\pkg_Stringutils_Body.sql;
@@..\Update_Scripts\9SR2_to_9SR3\PLSQL\Packages\pkg_Barcodes_Def.sql;
@@..\Update_Scripts\9SR2_to_9SR3\PLSQL\Packages\pkg_Barcodes_Body.sql;
@@..\Update_Scripts\9SR2_to_9SR3\PLSQL\Packages\pkg_Reservations_Def.sql;
@@..\Update_Scripts\9SR2_to_9SR3\PLSQL\Packages\pkg_Reservations_Body.sql;
@@..\Update_Scripts\9SR2_to_9SR3\PLSQL\Packages\pkg_Xmlutils_Def.sql;
@@..\Update_Scripts\9SR2_to_9SR3\PLSQL\Packages\pkg_Xmlutils_Body.sql;
@@..\Update_Scripts\9SR2_to_9SR3\PLSQL\Packages\pkg_Chemcalcs_Def.sql;
@@..\Update_Scripts\9SR2_to_9SR3\PLSQL\Packages\pkg_Chemcalcs_Body.sql;
@@..\Update_Scripts\9SR2_to_9SR3\PLSQL\Packages\pkg_Platechem_Def.sql;
@@..\Update_Scripts\9SR2_to_9SR3\PLSQL\Packages\pkg_Platechem_Body.sql;
@@..\Update_Scripts\9SR2_to_9SR3\PLSQL\Packages\pkg_Approvals_Def.sql
@@..\Update_Scripts\9SR2_to_9SR3\PLSQL\Packages\pkg_Approvals_Body.sql
@@..\Update_Scripts\9SR2_to_9SR3\PLSQL\Packages\pkg_Guiutils_Def.sql;
@@..\Update_Scripts\9SR2_to_9SR3\PLSQL\Packages\pkg_Guiutils_Body.sql;
@@..\Update_Scripts\9SR2_to_9SR3\PLSQL\Packages\pkg_Racks_def.sql;
@@..\Update_Scripts\9SR2_to_9SR3\PLSQL\Packages\pkg_Racks_Body.sql;
@@..\Update_Scripts\9SR2_to_9SR3\PLSQL\Packages\pkg_Docs_def.sql;
@@..\Update_Scripts\9SR2_to_9SR3\PLSQL\Packages\pkg_Docs_Body.sql;
@@..\Update_Scripts\9SR2_to_9SR3\PLSQL\Packages\pkg_Requests_Def.sql;
@@..\Update_Scripts\9SR2_to_9SR3\PLSQL\Packages\pkg_Requests_Body.sql;
@@..\Update_Scripts\9SR2_to_9SR3\PLSQL\Packages\pkg_Compounds_Def.sql;
@@..\Update_Scripts\9SR2_to_9SR3\PLSQL\Packages\pkg_Compounds_Body.sql;
@@..\Update_Scripts\9SR2_to_9SR3\PLSQL\Packages\pkg_Datamaps_Def.sql
@@..\Update_Scripts\9SR2_to_9SR3\PLSQL\Packages\pkg_Datamaps_Body.sql
@@..\Update_Scripts\9SR2_to_9SR3\PLSQL\Packages\pkg_Links_Def.sql;
@@..\Update_Scripts\9SR2_to_9SR3\PLSQL\Packages\pkg_Links_Body.sql;
@@..\Update_Scripts\9SR2_to_9SR3\PLSQL\Packages\pkg_Reformat_Def.sql;
@@..\Update_Scripts\9SR2_to_9SR3\PLSQL\Packages\pkg_Reformat_Body.sql;
@@..\Update_Scripts\9SR2_to_9SR3\PLSQL\Packages\pkg_Platesettings_Def.sql;
@@..\Update_Scripts\9SR2_to_9SR3\PLSQL\Packages\pkg_Platesettings_Body.sql;
@@..\Update_Scripts\9SR2_to_9SR3\PLSQL\Packages\pkg_Reports_Def.sql
@@..\Update_Scripts\9SR2_to_9SR3\PLSQL\Packages\pkg_Reports_Body.sql
@@..\Update_Scripts\9SR2_to_9SR3\PLSQL\Packages\pkg_Reportparams_Def.sql
@@..\Update_Scripts\9SR2_to_9SR3\PLSQL\Packages\pkg_Reportparams_Body.sql
@@..\Update_Scripts\9SR2_to_9SR3\PLSQL\Packages\pkg_Audit_trail_def.sql;
@@..\Update_Scripts\9SR2_to_9SR3\PLSQL\Packages\pkg_Audit_trail_Body.sql;
@@..\Update_Scripts\9SR2_to_9SR3\PLSQL\Packages\pkg_Organization_def.sql;
@@..\Update_Scripts\9SR2_to_9SR3\PLSQL\Packages\pkg_Organization_Body.sql;
--' this procedure is used in the Batch package
@@..\Update_Scripts\9SR2_to_9SR3\PLSQL\Procedures\proc_UpdateContainerBatches.sql
@@..\Update_Scripts\9SR2_to_9SR3\PLSQL\Packages\pkg_Batch_def.sql;
@@..\Update_Scripts\9SR2_to_9SR3\PLSQL\Packages\pkg_Batch_Body.sql;
@@..\Update_Scripts\9SR2_to_9SR3\PLSQL\Packages\pkg_CTX_Cheminv_Mgr_def.SQL;
@@..\Update_Scripts\9SR2_to_9SR3\PLSQL\Packages\pkg_CTX_Cheminv_Mgr_Body.SQL;


prompt '#########################################################'
prompt 'Updating functions...'
prompt '#########################################################'

@@..\Update_Scripts\9SR2_to_9SR3\PLSQL\Functions\f_SolvatePlates.sql
@@..\Update_Scripts\9SR2_to_9SR3\PLSQL\Functions\f_IsContainerTypeAllowed.sql
@@..\Update_Scripts\9SR2_to_9SR3\PLSQL\Functions\f_ExcludeContainerTypes.sql
@@..\Update_Scripts\9SR2_to_9SR3\PLSQL\Functions\f_GetCompoundID.sql
@@..\Update_Scripts\9SR2_to_9SR3\PLSQL\Functions\f_IsDuplicateCompound.sql
@@..\Update_Scripts\9SR2_to_9SR3\PLSQL\Functions\f_UpdateContainerStatus.sql
@@..\Update_Scripts\9SR2_to_9SR3\PLSQL\Functions\f_EnableGridForLocation.sql
@@..\Update_Scripts\9SR2_to_9SR3\PLSQL\Functions\f_CheckoutContainer.sql
@@..\Update_Scripts\9SR2_to_9SR3\PLSQL\Functions\f_AssignPlateTypesToLocation.sql
@@..\Update_Scripts\9SR2_to_9SR3\PLSQL\Functions\f_RetireContainer.sql
@@..\Update_Scripts\9SR2_to_9SR3\PLSQL\Functions\f_GetLocationPath.sql
@@..\Update_Scripts\9SR2_to_9SR3\PLSQL\Functions\f_CreateContainer.sql
@@..\Update_Scripts\9SR2_to_9SR3\PLSQL\Functions\f_CopyContainer.sql
@@..\Update_Scripts\9SR2_to_9SR3\PLSQL\Functions\f_CopyPlate.sql
@@..\Update_Scripts\9SR2_to_9SR3\PLSQL\Functions\f_UpdateContainer.sql
@@..\Update_Scripts\9SR2_to_9SR3\PLSQL\Functions\f_CreateLocation.sql
@@..\Update_Scripts\9SR2_to_9SR3\PLSQL\Functions\f_DeleteContainer.sql
@@..\Update_Scripts\9SR2_to_9SR3\PLSQL\Functions\f_GetGridID.sql
@@..\Update_Scripts\9SR2_to_9SR3\PLSQL\Functions\f_GetGridFormatID.sql
@@..\Update_Scripts\9SR2_to_9SR3\PLSQL\Functions\f_DeleteLocationGrid.sql
@@..\Update_Scripts\9SR2_to_9SR3\PLSQL\Functions\f_DeleteLocation.sql
@@..\Update_Scripts\9SR2_to_9SR3\PLSQL\Functions\f_GetCompoundIDFromName.sql
@@..\Update_Scripts\9SR2_to_9SR3\PLSQL\Functions\f_MoveContainer.sql
@@..\Update_Scripts\9SR2_to_9SR3\PLSQL\Functions\f_MoveLocation.sql
@@..\Update_Scripts\9SR2_to_9SR3\PLSQL\Functions\f_SubstanceNameExists.sql
@@..\Update_Scripts\9SR2_to_9SR3\PLSQL\Functions\f_UpdateAllContainerFields.sql
@@..\Update_Scripts\9SR2_to_9SR3\PLSQL\Functions\f_UpdateContainerQtyRemaining.sql
@@..\Update_Scripts\9SR2_to_9SR3\PLSQL\Functions\f_GetGridStorageID.sql
@@..\Update_Scripts\9SR2_to_9SR3\PLSQL\Functions\f_UpdateLocation.sql
@@..\Update_Scripts\9SR2_to_9SR3\PLSQL\Functions\f_OrderContainer.sql
@@..\Update_Scripts\9SR2_to_9SR3\PLSQL\Functions\f_ReorderContainer.sql
@@..\Update_Scripts\9SR2_to_9SR3\PLSQL\Functions\f_SelectHazmatData.sql
@@..\Update_Scripts\9SR2_to_9SR3\PLSQL\Functions\f_SelectSubstanceHazmatData.sql
@@..\Update_Scripts\9SR2_to_9SR3\PLSQL\Functions\f_CreateTableRow.SQL
@@..\Update_Scripts\9SR2_to_9SR3\PLSQL\Functions\f_DeleteTableRow.sql
@@..\Update_Scripts\9SR2_to_9SR3\PLSQL\Functions\f_DeletePlates.sql
@@..\Update_Scripts\9SR2_to_9SR3\PLSQL\Functions\f_GetPKColumn.sql
@@..\Update_Scripts\9SR2_to_9SR3\PLSQL\Functions\f_SetPlateFTCycle.sql
@@..\Update_Scripts\9SR2_to_9SR3\PLSQL\Functions\f_MovePlates.sql
@@..\Update_Scripts\9SR2_to_9SR3\PLSQL\Functions\f_RetirePlate.sql
@@..\Update_Scripts\9SR2_to_9SR3\PLSQL\Functions\f_UpdatePlateAttributes.sql
@@..\Update_Scripts\9SR2_to_9SR3\PLSQL\Functions\f_UpdateTable.sql
@@..\Update_Scripts\9SR2_to_9SR3\PLSQL\Functions\f_UpdateWell.sql

--@@..\Update_Scripts\9SR2_to_9SR3\Procedures\proc_InsertXMLDoc.sql
--@@..\Update_Scripts\9SR2_to_9SR3\Procedures\proc_InsertXSTL.sql

@@..\Update_Scripts\9SR2_to_9SR3\PLSQL\Functions\f_CreatePlateXML.sql
@@..\Update_Scripts\9SR2_to_9SR3\PLSQL\Functions\f_CreatePlateMap.sql
@@..\Update_Scripts\9SR2_to_9SR3\PLSQL\Functions\f_IsPlateTypeAllowed.sql   
@@..\Update_Scripts\9SR2_to_9SR3\PLSQL\Functions\f_IsTrashLocation.sql
@@..\Update_Scripts\9SR2_to_9SR3\PLSQL\Functions\f_EmptyTrash.sql
@@..\Update_Scripts\9SR2_to_9SR3\PLSQL\Functions\f_LookUpValue.sql
@@..\Update_Scripts\9SR2_to_9SR3\PLSQL\Functions\f_UpdateAddress.sql
@@..\Update_Scripts\9SR2_to_9SR3\PLSQL\Functions\f_CertifyContainer.sql
@@..\Update_Scripts\9SR2_to_9SR3\PLSQL\Functions\f_InsertCheckInDetails.sql
@@..\Update_Scripts\9SR2_to_9SR3\PLSQL\Functions\f_CheckInDefaultLocation.SQL

-- procedures from Alter_Cheminv_Plate_Procs.sql

@@..\Update_Scripts\9SR2_to_9SR3\PLSQL\Functions\f_CreateGridFormat.sql
@@..\Update_Scripts\9SR2_to_9SR3\PLSQL\Functions\f_CreatePhysPlateType.sql
@@..\Update_Scripts\9SR2_to_9SR3\PLSQL\Functions\f_DeletePhysPlateType.sql
@@..\Update_Scripts\9SR2_to_9SR3\PLSQL\Functions\f_UpdatePhysPlateType.sql
@@..\Update_Scripts\9SR2_to_9SR3\PLSQL\Functions\f_UpdateWellFormat.sql
@@..\Update_Scripts\9SR2_to_9SR3\PLSQL\Functions\f_DeletePlateFormat.sql
@@..\Update_Scripts\9SR2_to_9SR3\PLSQL\Functions\f_UpdatePlateFormat.sql
@@..\Update_Scripts\9SR2_to_9SR3\PLSQL\Functions\f_CreatePlateFormat.sql
@@..\Update_Scripts\9SR2_to_9SR3\PLSQL\Functions\f_DeletePlate.sql
@@..\Update_Scripts\9SR2_to_9SR3\PLSQL\Functions\f_AssignLocationToGrid.sql
@@..\Update_Scripts\9SR2_to_9SR3\PLSQL\Functions\f_GetNumberOfCompoundWells.sql
@@..\Update_Scripts\9SR2_to_9SR3\PLSQL\Functions\f_IsGridLocation.sql
@@..\Update_Scripts\9SR2_to_9SR3\PLSQL\Functions\f_IsFrozenLocation.sql
@@..\Update_Scripts\9SR2_to_9SR3\PLSQL\Functions\f_MovePlate.sql        

prompt '#########################################################'
prompt 'Updating procedures...'
prompt '#########################################################'

@@..\Update_Scripts\9SR2_to_9SR3\PLSQL\Procedures\proc_InsertIntoCustomChemOrder.sql
@@..\Update_Scripts\9SR2_to_9SR3\PLSQL\Procedures\proc_InsertHazmatData.sql
@@..\Update_Scripts\9SR2_to_9SR3\PLSQL\Procedures\proc_UpdateHazmatData.sql
@@..\Update_Scripts\9SR2_to_9SR3\PLSQL\Procedures\proc_InsertSubstanceHazmatData.sql
@@..\Update_Scripts\9SR2_to_9SR3\PLSQL\Procedures\proc_UpdateSubstanceHazmatData.sql
@@..\Update_Scripts\9SR2_to_9SR3\PLSQL\Procedures\proc_UpdateBatchStatus.sql

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

END;
/                                           

--' Insert application data
Connect &&schemaName/&&schemaPass@&&serverName
@@..\Update_Scripts\9SR2_to_9SR3\Data\TableData\Globals.sql
@@..\Update_Scripts\9SR2_to_9SR3\Data\TableData\Inv_Graphic_Types.sql
@@..\Update_Scripts\9SR2_to_9SR3\Data\TableData\Inv_Graphics.sql
@@..\Update_Scripts\9SR2_to_9SR3\Data\TableData\Inv_Batch_Status.sql
@@..\Update_Scripts\9SR2_to_9SR3\Data\TableData\Inv_Doc_Types.sql
@@..\Update_Scripts\9SR2_to_9SR3\Data\TableData\Inv_Country.sql
@@..\Update_Scripts\9SR2_to_9SR3\Data\TableData\Inv_States.sql
@@..\Update_Scripts\9SR2_to_9SR3\Data\TableData\Inv_Xslts.sql
@@..\Update_Scripts\9SR2_to_9SR3\Data\TableData\Inv_Location_Types.sql

@@..\Update_Scripts\9SR2_to_9SR3\Data\IncrementSequences.sql

--update db version
UPDATE GLOBALS SET value = '4.3' WHERE ID = 'VERSION_SCHEMA'; 

--' Recompile pl/sql 
Alter Function MovePlates Compile;
@@..\Update_Scripts\9SR2_to_9SR3\PLSQL\RecompilePLSQL.sql

prompt '#########################################################'
prompt 'Logged session to: Logs\LOG_Update_Inventory_9SR2_to_9SR3.txt'
prompt '#########################################################'

--spool off
