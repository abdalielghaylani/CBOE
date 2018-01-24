--Copyright 1999-2010 CambridgeSoft Corporation. All rights reserved


Connect &&InstallUser/&&sysPass@&&serverName &&AsSysDBA
        @"sql\Patches\Patch &&currentPatch\sql\cssecurity\Alter_CsSecurity_ora.sql"
	@sql\Parameters.sql 

--********************************************************************

UPDATE &&schemaName..CoeGlobals
	SET Value = '&&currentPatch' 
	WHERE UPPER(ID) = 'SCHEMAVERSION';

UPDATE &&schemaName..CoeGlobals
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





