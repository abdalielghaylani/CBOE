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
create index INV_REG_ID_FK on INV_COMPOUNDS(REG_ID_FK);

prompt '#########################################################'
prompt 'Updating Privileges...'
prompt '#########################################################'

prompt '#########################################################'
prompt 'Alter Tables...'
prompt '#########################################################'

prompt '#########################################################'
prompt 'Updating Functions, Procedures and Packages...'
prompt '#########################################################'
@"Patches\Patch &&currentpatch\PLSQL\Functions\f_CreatePlateXML.sql"
prompt '#########################################################'
prompt 'Creating triggers...'
prompt '#########################################################'

prompt '#########################################################'
prompt 'Inserting/Updating data...'
prompt '#########################################################'

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






