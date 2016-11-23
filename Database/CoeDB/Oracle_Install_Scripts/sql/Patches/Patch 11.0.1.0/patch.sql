--Copyright 1999-2010 CambridgeSoft Corporation. All rights reserved

@"sql\Patches\Patch &&nextPatch\parameters.sql"

spool sql\Patches\log_Patches_CoeDB_11.0.1.0_ora.txt

prompt *********************************
prompt **** Applying Patch &&CurrentPatch ****
prompt *********************************
prompt Starting "patch.sql"...
prompt 


--********************************************************************

Connect &&InstallUser/&&sysPass@&&serverName &&AsSysDBA;

	@"sql\Patches\Patch &&currentPatch\sql\drops.sql"
	@"sql\Patches\Patch &&currentPatch\sql\tablespaces.sql"
	@"sql\Patches\Patch &&currentPatch\sql\CsSecurity\createTempTableSpace_&&OraVersionNumber"
	@"sql\Patches\Patch &&currentPatch\sql\users.sql"

Connect &&schemaName/&&schemaPass@&&serverName
	@"sql\Patches\Patch &&currentPatch\sql\CREATE_COEDB_ora.sql"
	@"sql\Patches\Patch &&currentPatch\sql\cssecurity\Upgrade_CsSecurity_to_CoeDB_ora.sql"

Connect &&InstallUser/&&sysPass@&&serverName &&AsSysDBA;
	@"sql\Patches\Patch &&currentPatch\sql\synonyms.sql"
	@"sql\Patches\Patch &&currentPatch\sql\coeuser.sql"

Connect &&schemaName/&&schemaPass@&&serverName
	@"sql\Patches\Patch &&currentPatch\sql\inserts.sql"
	@"sql\Patches\Patch &&currentPatch\sql\grants.sql"
	@"sql\Patches\Patch &&currentPatch\sql\proxyGrants.sql"
	@"sql\Patches\Patch &&currentPatch\sql\roles.sql"

prompt ********************************
prompt **** Applied patch &&CurrentPatch ****
prompt ********************************
prompt 
spool off
spool sql\Patches\log_Patches_CoeDB_ora.txt
prompt 
prompt ****************************************************************************
prompt WARNING: You are about to rename the CS_SECURITY schema table.
prompt Please check if there are errors on the following file log before continuing.
prompt "sql\Patches\log_Patches_CoeDB_11.0.1.0_ora.txt"
ACCEPT VerifyError CHAR DEFAULT 'N' PROMPT 'Do you want continue? (N):'


@"sql\Patches\Patch &&currentPatch\patch_&&VerifyError"
