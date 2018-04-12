--Copyright 1999-2010 CambridgeSoft Corporation. All rights reserved

@"Patches\Patch &&nextPatch\parameters.sql"

prompt *******************************
prompt **** Applying Patch &&CurrentPatch ****
prompt *******************************
prompt Starting "patch.sql"...
prompt 
prompt *******************************

Connect &&schemaName/&&schemaPass@&&serverName

prompt '#########################################################'
prompt 'Alter Tables...'
prompt '#########################################################'

@"Patches\Patch &&currentpatch\Create\Views\Inv_VW_Well_Flat.sql"


prompt '#########################################################'
prompt 'Updating Functions and Packages...'
prompt '#########################################################'

@"Patches\Patch &&currentpatch\PLSQL\Functions\f_CreatePlateXML.sql"
@"Patches\Patch &&currentpatch\PLSQL\Functions\f_CopyPlate.sql"
@"Patches\Patch &&currentpatch\PLSQL\Packages\pkg_Reformat_body.sql"
@"Patches\Patch &&currentpatch\PLSQL\Packages\pkg_Requests_def.sql"
@"Patches\Patch &&currentpatch\PLSQL\Packages\pkg_Requests_body.sql"
@"Patches\Patch &&currentpatch\PLSQL\Packages\pkg_GUIUtils_Body.sql"


prompt '#########################################################'
prompt 'Updating triggers...'
prompt '#########################################################'

@"Patches\Patch &&currentpatch\PLSQL\Triggers\inv_wells_weight_es.sql"


prompt '#########################################################'
prompt 'Inserting Table Data...'
prompt '#########################################################'
		  

@"Patches\Patch &&currentpatch\PLSQL\RecompilePLSQL.sql"

prompt 
--spool off

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