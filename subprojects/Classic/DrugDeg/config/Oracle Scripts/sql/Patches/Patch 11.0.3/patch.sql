--Copyright 1999-2010 CambridgeSoft Corporation. All rights reserved
--#######################################################################
-- Headerfile for updating the drugdeg schema to the current patch
--####################################################################### 

@"Patches\Patch &&nextPatch\parameters.sql"

prompt *******************************
prompt **** Applying Patch &&CurrentPatch ****
prompt *******************************
prompt Starting "patch.sql"...
prompt 

--#########################################################
--VIEWS
--#########################################################
Connect &&InstallUser/&&sysPass@&&serverName
@"Patches\Patch &&currentpatch\Create\Views\VW_DRUGDEG_NAME.sql"
prompt *******************************
Connect &&InstallUser/&&sysPass@&&serverName
@"Patches\Patch &&currentpatch\Create\Views\VW_DRUGDEG_COMPOUNDS.sql"
prompt *******************************

prompt ####################################################################
prompt Logged session to: Patches\log_Patches_DRUGDEG.txt
prompt ####################################################################

prompt 
spool off


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






