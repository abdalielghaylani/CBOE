--Copyright 1999-2003 CambridgeSoft Corporation. All rights reserved


spool COERegistrationDBPatch08\log_create_chemregFrom_11_to_11.0.1.txt

	@@Parameters.sql 
	@@Prompts.sql

Connect &&InstallUser/&&sysPass@&&serverName &&AsSysDBA;

	VAR Begin VARCHAR2(20);
	VAR End VARCHAR2(20);
	VAR Elapsed VARCHAR2(100);
	exec :Begin:=to_char(systimestamp,'HH:MI:SS.FF4');

	@@Users.sql


Connect &&schemaName/&&schemaPass@&&serverName

	@@Update_ChemReg_DB_From_11_to_11.0.1.SQL


Connect &&securitySchemaName/&&securitySchemaPass@&&serverName

	@@ALTER_CoeDB_for_chemreg_ora.sql


Connect &&InstallUser/&&sysPass@&&serverName &&AsSysDBA;

	@@Grants.sql


	SET serveroutput on
	BEGIN
		:End:=to_char(systimestamp,'HH:MI:SS.FF4');
		:Elapsed:=to_timestamp(:End,'HH:MI:SS.FF4')-to_timestamp(:Begin,'HH:MI:SS.FF4');
		
		dbms_output.put_line('Begin: '||:Begin);
		dbms_output.put_line('End: '||:End);
		dbms_output.put_line('Elapsed: '||substr(:Elapsed,instr(:Elapsed,' ')+1,13));
	END;
	/

prompt logged session to: COERegistrationDBPatch08\log_create_chemregFrom_11_to_11.0.1.txt
spool off

exit


	