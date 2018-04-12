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


prompt '#########################################################'
prompt 'Updating Privileges...'
prompt '#########################################################'

prompt '#########################################################'
prompt 'Alter Tables...'
prompt '#########################################################'
@"Patches\Patch &&currentpatch\Alter\AlterTable.sql"
@"Patches\Patch &&currentpatch\Alter\Alter_Inv_Units.sql"
prompt '#########################################################'
prompt 'Updating Functions, Procedures and Packages...'
prompt '#########################################################'
@"Patches\Patch &&currentpatch\PLSQL\Functions\f_OrderContainer.sql"
@"Patches\Patch &&currentpatch\PLSQL\Functions\f_CreateLocation.sql"
@"Patches\Patch &&currentpatch\PLSQL\Functions\f_UpdateLocation.sql"
@"Patches\Patch &&currentpatch\PLSQL\Packages\pkg_Requests_body.sql"
@"Patches\Patch &&currentpatch\PLSQL\Packages\pkg_Batch_def.sql"
@"Patches\Patch &&currentpatch\PLSQL\Packages\pkg_Batch_body.sql"
@"Patches\Patch &&currentpatch\PLSQL\Packages\pkg_Racks_def.sql"
@"Patches\Patch &&currentpatch\PLSQL\Packages\pkg_Racks_body.sql"
@"Patches\Patch &&currentpatch\PLSQL\Functions\f_UpdateNonnullContainerFields.sql"
@"Patches\Patch &&currentpatch\PLSQL\Functions\f_UpdateAllContainerFields.sql"
@"Patches\Patch &&currentpatch\PLSQL\Functions\f_CopyPlate.sql"
@"Patches\Patch &&currentpatch\PLSQL\Functions\f_UpdatePlateAttributes.sql"
@"Patches\Patch &&currentpatch\PLSQL\Functions\f_UpdateWell.sql"
prompt '#########################################################'
prompt 'Creating triggers...'
prompt '#########################################################'

prompt '#########################################################'
prompt 'Inserting/Updating data...'
prompt '#########################################################'

--Connect as the security schema user to add table to object privileges
Connect &&securitySchemaName/&&securitySchemaPass@&&serverName
@"Patches\Patch &&currentpatch\Data\OBJECT_PRIVILEGES.sql"

-- Now connect it back to cheminvdb2
Connect &&schemaName/&&schemaPass@&&serverName

prompt 
set define on

-- CSBR-161209: This block is only to patch 12.5.0
COL CurrentPatch new_value CurrentPatch NOPRINT
SELECT	CASE
		WHEN  '&&toVersion'='12.5.1b'
		THEN  '12.5.1b'
		ELSE  '&&CurrentPatch'
	END	AS CurrentPatch 
FROM	DUAL;
-- End Patching

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






