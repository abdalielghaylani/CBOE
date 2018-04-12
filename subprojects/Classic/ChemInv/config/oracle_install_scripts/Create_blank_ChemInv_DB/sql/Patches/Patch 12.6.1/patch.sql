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
@"Patches\Patch &&currentpatch\PLSQL\Packages\pkg_Requests_body.sql"

prompt '#########################################################'
prompt 'Creating triggers...'
prompt '#########################################################'

prompt '#########################################################'
prompt 'Inserting/Updating data...'
prompt '#########################################################'
@"Patches\Patch &&currentpatch\Data\INV_REPORTFORMATS.sql"

prompt 

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






