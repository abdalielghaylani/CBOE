-- Copyright Cambridgesoft Corp 2001-2007 all rights reserved

--################################################################
-- Headerfile for updating the cheminvdb2 schema from 9SR3 to 10
--################################################################ 

spool ..\..\Logs\LOG_Update_Inventory_9SR3_to_10.txt

-- Intialize variables
@@..\..\Parameters.sql
@@..\..\Prompts.sql

Connect &&InstallUser/&&sysPass@&&serverName

GRANT CREATE ANY INDEX TO &&schemaName;
GRANT CREATE ANY SNAPSHOT TO &&schemaName;
GRANT CREATE ANY VIEW TO &&schemaName;

Connect &&schemaName/&&schemaPass@&&serverName

prompt '#########################################################'
prompt 'Updating Tables...'
prompt '#########################################################'

@@Create\Tables\Inv_Picklist_Types.sql
@@Alter\Alter_Inv_Picklists.sql
@@Alter\Alter_Inv_Requests.sql
@@Alter\Alter_Inv_Compounds.sql
@@Create\Tables\Inv_Custom_Field_Groups.sql
@@Create\Tables\Inv_Custom_Fields.sql
@@Create\Tables\Inv_Custom_Cpd_Field_Values.sql
@@Create\Tables\inv_lpr_definition.sql
@@Create\Tables\inv_label_printers.sql

prompt '#########################################################'
prompt 'Creating views...'
prompt '#########################################################'

-- Use this CompoundsViewScript variable to execute one version of the INV_VW_COMPOUNDS script 
-- or another, depending on whether the system has Reg Integration or not.  For Reg Integration, 
-- the view is actually  created through the UpdateRegIntegration_to_10.sql script.  We don't
-- want to have to go through the process of creating that view here only to drop and recreate it later;
-- rather, we'll just skip that part of the script for now.  For systems without Reg Integration,
-- we need to create the full view at this time.
DEFINE CompoundsViewScript = Inv_VW_Compounds
col comments_col for a30 new_value CompoundsViewScript

@@RegistrationIntegration\DetectRegIntegration.sql;
-- Run whichever script is stored in the variable... either 'Blank' or 'Inv_VW_Compounds'
@@Create\Views\&CompoundsViewScript..sql
@@Create\Views\Inv_VW_Well.sql
@@Create\Views\Inv_VW_Well_Flat.sql
@@Create\Views\Inv_VW_Plate.sql

prompt '#########################################################'
prompt 'Creating Public Synonyms...'
prompt '#########################################################'

@@Create\Synonyms\Create_Synonyms.sql;

prompt '#########################################################'
prompt 'Creating Indexes...'
prompt '#########################################################'

----------------------------------------
--  Create new indexes for efficiency --
----------------------------------------
Create index fidx_inv_comp_up On INV_Compounds(UPPER(substance_name)) TABLESPACE &&indexTableSpaceName;
Create index fidx_inv_syn_up On Inv_SYNONYMS(UPPER(substance_name)) TABLESPACE &&indexTableSpaceName;
Create index fidx_inv_comp_low On INV_Compounds(LOWER(substance_name)) TABLESPACE &&indexTableSpaceName;
Create index fidx_inv_syn_low On Inv_SYNONYMS(LOWER(substance_name)) TABLESPACE &&indexTableSpaceName;

Connect &&schemaName/&&schemaPass@&&serverName

prompt '#########################################################'
prompt 'Updating Functions and Packages...'
prompt '#########################################################'

@@PLSQL\Packages\pkg_XMLUtils_def.sql;
@@PLSQL\Packages\pkg_Reformat_def.sql;
@@PLSQL\Packages\pkg_GUIUtils_def.sql;
@@PLSQL\Packages\pkg_Compounds_def.sql;
@@PLSQL\Packages\pkg_PlateSettings_def.sql;
@@PLSQL\Packages\pkg_ChemCalcs_def.sql;
@@PLSQL\Packages\pkg_Racks_def.sql;
@@PLSQL\Packages\pkg_Requests_def.sql;
@@PLSQL\Packages\pkg_Approvals_def.sql;
@@PLSQL\Packages\pkg_Audit_trail_def.sql;
@@PLSQL\Packages\pkg_Batch_def.sql;
@@PLSQL\Packages\pkg_Organization_def.sql;
@@PLSQL\Packages\pkg_PlateChem_def.sql;

-- Compile XMLUtils first to fix CSBR-94151
@@PLSQL\Packages\pkg_XMLUtils_Body.sql;

@@PLSQL\Functions\f_IsPlateTypeAllowed.sql;
@@PLSQL\Functions\f_CreateRegCompound.sql;
@@PLSQL\Functions\f_UpdateRegCompound.sql;
@@PLSQL\Functions\f_CreatePlateXML.sql;
@@PLSQL\Functions\f_LookUpValue.sql;
@@PLSQL\Functions\f_UpdateLocation.sql;
@@PLSQL\Functions\f_CopyPlate.sql;
@@PLSQL\Functions\f_GetPrimaryKeyIDs.sql;
@@PLSQL\Functions\f_CheckoutContainer.sql;
@@PLSQL\Functions\f_RetireContainer.sql;
@@PLSQL\Functions\f_CheckOpenPositions.sql;
@@PLSQL\Functions\f_CreateContainer.sql;
@@PLSQL\Functions\f_CertifyContainer.sql;
@@PLSQL\Functions\f_CopyContainer.sql;
@@PLSQL\Functions\f_OrderContainer.sql;
@@PLSQL\Functions\f_ReorderContainer.sql;
@@PLSQL\Functions\f_DeleteLocation.sql;
@@PLSQL\Functions\f_SelectSubstanceHazmatData.sql
@@PLSQL\Functions\f_SelectHazmatData.sql;
@@PLSQL\Functions\f_CreateTableRow.sql;
@@PLSQL\Functions\f_DeleteContainer.sql;
@@PLSQL\Functions\f_DeletePlates.sql;
@@PLSQL\Functions\f_UpdateAllContainerFields.sql;
@@PLSQL\Functions\f_UpdateContainer.sql;
@@PLSQL\Functions\f_UpdateContainerStatus.sql;
@@PLSQL\Functions\f_UpdatePlateAttributes.sql;
@@PLSQL\Functions\f_UpdateWell.sql;
@@PLSQL\Functions\f_InsertCheckInDetails.sql;

@@PLSQL\Packages\pkg_Reformat_body.sql;
@@PLSQL\Packages\pkg_GUIUtils_body.sql;
@@PLSQL\Packages\pkg_Compounds_body.sql;
@@PLSQL\Packages\pkg_PlateSettings_Body.sql;
@@PLSQL\Packages\pkg_ChemCalcs_body.sql;
@@PLSQL\Packages\pkg_Racks_Body.sql;
@@PLSQL\Packages\pkg_Requests_body.sql;
@@PLSQL\Packages\pkg_Approvals_Body.sql;
@@PLSQL\Packages\pkg_Audit_trail_Body.sql;
@@PLSQL\Packages\pkg_Batch_Body.sql;
@@PLSQL\Packages\pkg_Organization_Body.sql;
@@PLSQL\Packages\pkg_PlateChem_body.sql;

prompt '#########################################################'
prompt 'Updating triggers...'
prompt '#########################################################'

@@PLSQL\Triggers\inv_containers_au0.trg;

prompt '#########################################################'
prompt 'Updating procedures...'
prompt '#########################################################'

@@PLSQL\Procedures\proc_InsertIntoCustomChemOrder.sql;
@@PLSQL\Procedures\proc_UpdateContainerBatches.sql;

prompt '#########################################################'
prompt 'Inserting data...'
prompt '#########################################################'

--  Alter cs_security for New privilege and tables --
@@Alter\Alter_Cs_Security.sql

-- Insert application data
Connect &&schemaName/&&schemaPass@&&serverName
@@Data\TableData\Inv_Picklist_Types.sql
@@Data\TableData\Inv_Picklists.sql
@@Data\TableData\Inv_API_Errors.sql
@@Data\TableData\Inv_Reports.sql
@@Data\TableData\LocationReport.sql
@@Data\TableData\SolventDilutionVolume.sql
@@Data\TableData\Update_Units.sql
@@Data\TableData\Inv_Xmldocs.sql
@@Data\TableData\ShippingReport.sql

-- Update DB version
UPDATE GLOBALS SET value = '10.0' WHERE ID = 'VERSION_SCHEMA';
UPDATE GLOBALS SET value = '10.0' WHERE ID = 'VERSION_APP';

-- Fixing CSBR-82346, compilation errors in the CTX_CHEMINV_MGR package
Declare 
RLSvalue integer := 0;
PackageExists integer :=0;

begin

	SELECT value into RLSvalue from GLOBALS WHERE ID = 'RLS_ENABLED';
	select count(*) into PackageExists from all_PROCEDURES where upper(object_name) like '%CTX_CHEMINV_MGR%';

	if RLSvalue = 0 AND PackageExists =1 THEN
		execute immediate 'drop package CTX_CHEMINV_MGR';
	end if;
end;
/


-- Fix the search by date_created problem (CSBR-89814)
update inv_plates set date_created = trunc(date_created);
update inv_containers set date_created = trunc(date_created);

commit;

prompt '#########################################################'
prompt 'Recompiling pl/sql...'
prompt '#########################################################'

--@@PLSQL\RecompilePLSQL.sql


prompt #############################################################
prompt Logged session to: Logs\LOG_Update_Inventory_9SR3_to_10.txt
prompt #############################################################

prompt 
spool off

exit
