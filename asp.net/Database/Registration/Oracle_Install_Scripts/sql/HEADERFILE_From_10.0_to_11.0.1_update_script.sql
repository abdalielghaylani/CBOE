--Copyright 1999-2003 CambridgeSoft Corporation. All rights reserved


spool sql\log_update_chemreg_db_from_10.0_to_11.0.1.txt

	@@Parameters.sql 
	@@Prompts.sql

Connect &&InstallUser/&&sysPass@&&serverName &&AsSysDBA;

	VAR Begin VARCHAR2(20);
	VAR End VARCHAR2(20);
	VAR Elapsed VARCHAR2(100);
	exec :Begin:=to_char(systimestamp,'HH:MI:SS.FF4');


	--@@Drops.sql
	--@@Tablespaces.sql
	--@@Users.sql

Connect &&securitySchemaName/&&securitySchemaPass@&&serverName

	@@ALTER_CoeDB_for_chemreg_ora.sql

Connect &&schemaName/&&schemaPass@&&serverName

	@@Update_10.0_to_11.0.1.sql

Connect &&InstallUser/&&sysPass@&&serverName &&AsSysDBA;

	@@CREATE_chemreg_test_users.sql
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

prompt logged session to: sql/log_update_chemreg_db_from_10.0_to_11.0.1.txt
spool off

exit


	