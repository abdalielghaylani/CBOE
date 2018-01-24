--Copyright 1999-2010 CambridgeSoft Corporation. All rights reserved

@"sql\Patches\Patch 11.0.0\parameters.sql"

prompt *******************************
prompt **** Applying Patch &&CurrentPatch ****
prompt *******************************
prompt Starting "patch.sql"...
prompt 

Connect &&InstallUser/&&sysPass@&&serverName

	VAR Begin VARCHAR2(20);
	VAR End VARCHAR2(20);
	VAR Elapsed VARCHAR2(100);
	exec :Begin:=to_char(systimestamp,'HH:MI:SS.FF4');

	@"sql\Patches\Patch 11.0.0\Users.sql"


Connect &&schemaName/&&schemaPass@&&serverName

	@"sql\Patches\Patch 11.0.0\Update_ChemReg_DB_From_11_to_11.0.1.SQL"


Connect &&securitySchemaName/&&securitySchemaPass@&&serverName

	@"sql\Patches\Patch 11.0.0\ALTER_CoeDB_for_chemreg_ora.sql"


Connect &&InstallUser/&&sysPass@&&serverName

	@"sql\Patches\Patch 11.0.0\Grants.sql"


	SET serveroutput on
	BEGIN
		:End:=to_char(systimestamp,'HH:MI:SS.FF4');
		:Elapsed:=to_timestamp(:End,'HH:MI:SS.FF4')-to_timestamp(:Begin,'HH:MI:SS.FF4');
		
		dbms_output.put_line('Begin: '||:Begin);
		dbms_output.put_line('End: '||:End);
		dbms_output.put_line('Elapsed: '||substr(:Elapsed,instr(:Elapsed,' ')+1,13));
	END;
	/


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






