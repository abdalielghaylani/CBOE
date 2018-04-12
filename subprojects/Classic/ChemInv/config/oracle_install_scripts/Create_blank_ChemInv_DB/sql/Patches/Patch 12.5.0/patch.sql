--Copyright 1999-2010 CambridgeSoft Corporation. All rights reserved

@"Patches\Patch &&nextPatch\parameters.sql"

prompt *******************************
prompt **** Applying Patch &&CurrentPatch ****
prompt *******************************
prompt Starting "patch.sql"...
prompt 

Connect &&schemaName/&&schemaPass@&&serverName

prompt '#########################################################'
prompt 'Create Tables...'
prompt '#########################################################'
@"Patches\Patch &&currentpatch\Create\Inv_Custom_Field_Group_Types.sql"

prompt '#########################################################'
prompt 'Updating Privileges...'
prompt '#########################################################'

--Connect as the security schema user to granting roles
Connect &&securitySchemaName/&&securitySchemaPass@&&serverName

--Grant roles based on privileges to INV_CUSTOM_FIELD_GROUP_TYPES
begin
 FOR role_rec IN (SELECT role_name FROM security_roles WHERE privilege_table_int_id = (SELECT privilege_table_id FROM privilege_tables WHERE privilege_table_name = 'CHEMINV_PRIVILEGES'))
 LOOP
  EXECUTE IMMEDIATE 'GRANT SELECT ON &&SchemaName..INV_CUSTOM_FIELD_GROUP_TYPES' || ' TO ' || role_rec.role_name;
 END LOOP;
end;
/

@"Patches\Patch &&currentpatch\Alter\Alter_PrivilegeTable.sql"

--'Connect it back as the schema user
Connect &&schemaName/&&schemaPass@&&serverName

prompt '#########################################################'
prompt 'Alter Tables...'
prompt '#########################################################'
@"Patches\Patch &&currentpatch\Alter\Alter_Inv_Custom_Field_Groups.sql"

prompt '#########################################################'
prompt 'Updating Functions, Procedures and Packages...'
prompt '#########################################################'

@"Patches\Patch &&currentpatch\PLSQL\Function\UpdateLocation.sql"
@"Patches\Patch &&currentpatch\PLSQL\Function\f_UpdateAllContainerFields.sql"
@"Patches\Patch &&currentpatch\PLSQL\Packages\pkg_authority.sql"
@"Patches\Patch &&currentpatch\PLSQL\Function\f_UpdateNonnullContainerFields.sql"

Connect &&schemaName/&&schemaPass@&&serverName
@"Patches\Patch &&currentpatch\PLSQL\Procedures\proc_UpdateContainerBatches.sql"

prompt '#########################################################'
prompt 'Creating triggers...'
prompt '#########################################################'
@"Patches\Patch &&currentpatch\PLSQL\Triggers\TRG_AUDIT_INV_CSTM_FLDVAL_AI0.sql"
@"Patches\Patch &&currentpatch\PLSQL\Triggers\TRG_AUDIT_INV_CSTM_FLD_VAL_AD0.sql"

prompt '#########################################################'
prompt 'Inserting/Updating data...'
prompt '#########################################################'

@"Patches\Patch &&currentpatch\Data\Inv_Custom_Field_Group_Types_Data.sql"
@"Patches\Patch &&currentpatch\Data\Hazards.sql"
@"Patches\Patch &&currentpatch\Data\GHS_Hazards.sql"
@"Patches\Patch &&currentpatch\Data\pictogram_disclaimer.sql"
@"Patches\Patch &&currentpatch\Data\inv_reports_pictogram.sql"
@"Patches\Patch &&currentpatch\Data\Update_User_Plate_Preferences.sql"

prompt 
set define on

UPDATE &&schemaName..Globals
	SET Value = '&&CurrentPatch' 
	WHERE UPPER(ID) = 'VERSION_SCHEMA';
UPDATE &&schemaName..Globals
	SET Value = '&&CurrentPatch' 
	WHERE UPPER(ID) = 'VERSION_APP';
COMMIT;

prompt **** Patch &&CurrentPatch Applied ****

COL setNextPatch NEW_VALUE setNextPatch NOPRINT
SELECT	CASE
		WHEN  '&&toVersion'='&&CurrentPatch'
		THEN  'Patches\stop.sql'
		ELSE  '"Patches\Patch &&nextPatch\patch.sql"'
	END	AS setNextPatch 
FROM	DUAL;
 
prompt ****&&setNextPatch ***
@&&setNextPatch 






