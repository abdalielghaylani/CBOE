--Copyright 1999-2010 CambridgeSoft Corporation. All rights reserved

@"sql\Patches\Patch &&nextPatch\parameters.sql"

prompt *******************************
prompt **** Applying Patch &&CurrentPatch ****
prompt *******************************
prompt Starting "patch.sql"...
prompt 


--********************************************************************

ACCEPT ActivateRLS CHAR DEFAULT 'N' PROMPT 'Do you want to activate Row-Level security? (N):'

Connect &&securitySchemaName/&&securitySchemaPass@&&serverName

	@"sql\Patches\Patch &&currentPatch\sql\ALTER_CoeDB_for_chemreg_ora.sql"

Connect &&InstallUser/&&sysPass@&&serverName &&AsSysDBA;

	@"sql\Patches\Patch &&currentPatch\sql\GrantsUpgrade.sql"

Connect &&schemaName/&&schemaPass@&&serverName

	@"sql\Patches\Patch &&currentPatch\sql\Update_10.0_to_11.0.1.sql"

Connect &&InstallUser/&&sysPass@&&serverName &&AsSysDBA;

	@"sql\Patches\Patch &&currentPatch\sql\CREATE_chemreg_test_users.sql"
	@"sql\Patches\Patch &&currentPatch\sql\Grants.sql"

--********************************************************************


UPDATE &&schemaName..Globals
	SET Value = '&&currentPatch' 
	WHERE UPPER(ID) = 'VERSION_SCHEMA';

UPDATE &&schemaName..Globals
	SET Value = '&&versionApp' 
	WHERE UPPER(ID) = 'VERSION_APP';
COMMIT;

prompt **** Patch &&currentPatch Applied ****

COL setNextPatch NEW_VALUE setNextPatch NOPRINT
SELECT	CASE
		WHEN  '&&toVersion'='&&currentPatch'
		THEN  'sql\Patches\stop.sql'
		ELSE  '"sql\Patches\Patch &&nextPatch\patch.sql"'
	END	AS setNextPatch 
FROM	DUAL;

@&&setNextPatch 






