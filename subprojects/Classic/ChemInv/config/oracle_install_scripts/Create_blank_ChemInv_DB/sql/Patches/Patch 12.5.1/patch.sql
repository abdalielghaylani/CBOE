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


prompt '#########################################################'
prompt 'Updating Functions, Procedures and Packages...'
prompt '#########################################################'
@"Patches\Patch &&currentpatch\PLSQL\Packages\pkg_authority.sql"
@"Patches\Patch &&currentpatch\PLSQL\Packages\pkg_GUIUtils_Body.sql"
@"Patches\Patch &&currentpatch\PLSQL\Packages\pkg_StringUtils_Body.sql"
@"Patches\Patch &&currentpatch\PLSQL\Packages\pkg_PlateChem_body.sql"


prompt '#########################################################'
prompt 'Creating triggers...'
prompt '#########################################################'
@"Patches\Patch &&currentpatch\PLSQL\Triggers\TRG_AUDIT_INV_CSTM_FLDVAL_AI0.sql"
@"Patches\Patch &&currentpatch\PLSQL\Triggers\TRG_AUDIT_INV_CSTM_FLD_VAL_AD0.sql"


prompt '#########################################################'
prompt 'Inserting/Updating data...'
prompt '#########################################################'
INSERT INTO GLOBALS(ID, VALUE) VALUES ('GS_ENABLED', '0');

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






