-- Copyright Cambridgesoft Corp 2001-2007 all rights reserved

--################################################################
-- Headerfile for updating the cheminvdb2 schema from 9SR3 to 10
--################################################################ 

spool ..\..\Logs\LOG_Update_Inventory_9SR3_to_10.txt

-- Intialize variables
@@..\..\Parameters.sql
@@..\..\Prompts.sql
Connect &&schemaName/&&schemaPass@&&serverName

prompt '#########################################################'
prompt 'Updating Tables...'
prompt '#########################################################'

------------------------------------
--  New table inv_picklist_types  --
------------------------------------
@@Create\Tables\Inv_Picklist_Types.sql

---------------------------------
--  Alter table inv_picklists  --
---------------------------------
@@Alter\Alter_Inv_Picklists.sql

---------------------------------
--  Alter table Inv_Requests  --
---------------------------------
@@Alter\Alter_Inv_Requests.sql

---------------------------------
--  Alter table Inv_Compounds --
---------------------------------
@@Alter\Alter_Inv_Compounds.sql

----------------------------------------
--  New table inv_custom_field_groups --
----------------------------------------
@@Create\Tables\Inv_Custom_Field_Groups.sql

----------------------------------
--  New table inv_custom_fields --
----------------------------------
@@Create\Tables\Inv_Custom_Fields.sql

--------------------------------------------
--  New table inv_custom_cpd_field_values --
--------------------------------------------
@@Create\Tables\Inv_Custom_Cpd_Field_Values.sql


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

-----------------------------------------------------
--  Alter cs_security for New privilege and tables --
-----------------------------------------------------
@@Alter\Alter_Cs_Security.sql

----------------------------------
--  Recreate all the pl/sql  --
----------------------------------
Connect &&schemaName/&&schemaPass@&&serverName
prompt '#########################################################'
prompt 'Updating Packages...'
prompt '#########################################################'

@@PLSQL\Packages\pkg_Requests_def.sql;
@@PLSQL\Packages\pkg_Requests_body.sql;
@@PLSQL\Packages\pkg_Reformat_def.sql;
@@PLSQL\Packages\pkg_Reformat_body.sql;
@@PLSQL\Packages\pkg_GUIUtils_def.sql;
@@PLSQL\Packages\pkg_GUIUtils_body.sql;
@@PLSQL\Packages\pkg_Compounds_def.sql;
@@PLSQL\Packages\pkg_Compounds_body.sql;
@@PLSQL\Packages\pkg_PlateSettings_def.sql;
@@PLSQL\Packages\pkg_PlateSettings_Body.sql;
@@PLSQL\Packages\pkg_XMLUtils_def.sql;
@@PLSQL\Packages\pkg_XMLUtils_Body.sql;
@@PLSQL\Packages\pkg_ChemCalcs_def.sql;
@@PLSQL\Packages\pkg_ChemCalcs_body.sql;

prompt '#########################################################'
prompt 'Updating Functions...'
prompt '#########################################################'

@@PLSQL\Functions\f_IsPlateTypeAllowed.sql;
@@PLSQL\Functions\f_CreatePlateXML.sql;
@@PLSQL\Functions\f_LookUpValue.sql;
@@PLSQL\Functions\f_UpdateLocation.sql;
@@PLSQL\Functions\f_CopyPlate.sql;
@@PLSQL\Functions\f_GetPrimaryKeyIDs.sql;

prompt '#########################################################'
prompt 'Recompiling pl/sql...'
prompt '#########################################################'

--' Recompile pl/sql 
@@PLSQL\RecompilePLSQL.sql

-- Insert application data
@@Data\TableData\Inv_Picklist_Types.sql
@@Data\TableData\Inv_Picklists.sql
@@Data\TableData\Inv_Map_Fields.sql
@@Data\TableData\Inv_Data_Mappings.sql
@@Data\TableData\Inv_API_Errors.sql

--This is only for QA persons. Needs to be removed before release.
@@Data\TableData\Inv_Reports.sql
@@Data\TableData\Inv_Reportparams.sql

--' Fixing CSBR-82346
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

-- Update DB version
UPDATE GLOBALS SET value = '4.4' WHERE ID = 'VERSION_SCHEMA'; 

prompt #############################################################
prompt Logged session to: Logs\LOG_Update_Inventory_9SR3_to_10.txt
prompt #############################################################

prompt 
spool off

exit
