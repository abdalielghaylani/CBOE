--Copyright Cambridgesoft corp 1999-2003 all rights reserved

SPOOL ON
spool Patches\log_Rollback_DrugDeg_Patch_11_0_3.txt

	@Parameters.sql 
	@Prompts.sql

CONNECT &&InstallUser/&&sysPass@&&serverName
@"Patches\Patch 11.0.3\Drop\Views\Drop_VW_DRUGDEG_COMPOUNDS.sql"
CONNECT &&InstallUser/&&sysPass@&&serverName
@"Patches\Patch 11.0.3\Drop\Views\Drop_VW_DRUGDEG_NAME.sql"

Connect &&schemaName/&&schemaPass@&&serverName
UPDATE &&schemaName..Globals
	SET Value = '11.0.2' 
	WHERE UPPER(ID) = 'VERSION_SCHEMA';
UPDATE &&schemaName..Globals
	SET Value = '11.0.2' 
	WHERE UPPER(ID) = 'VERSION_APP';
COMMIT;

prompt logged session to: Patches\log_Rollback_DrugDeg_Patch_11_0_3.txt
spool off

exit

	