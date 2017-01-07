--Copyright 1999-2003 CambridgeSoft Corporation. All rights reserved


spool sql\log_create_chemreg_ora.txt

	@@Parameters.sql
	@@Prompts.sql

Connect &&InstallUser/&&sysPass@&&serverName &&AsSysDBA;

	VAR Begin VARCHAR2(20);
	VAR End VARCHAR2(20);
	VAR Elapsed VARCHAR2(100);
	exec :Begin:=to_char(systimestamp,'HH:MI:SS.FF4');


	@@Drops.sql
	@@Tablespaces.sql
	@@Users.sql

Connect &&securitySchemaName/&&securitySchemaPass@&&serverName

	@@CleanCoeDB.sql
	
Connect &&schemaName/&&schemaPass@&&serverName

	@@CREATE_chemreg_ora.sql


Connect &&securitySchemaName/&&securitySchemaPass@&&serverName

	@@ALTER_CoeDB_for_chemreg_ora.sql

--*** Starts required configuration settings for Registration App 11.0.1 ***
--    Here we insert all of those settings that we need to have for a RegSchema ready to use.
--    All of these settings must be inserted with external tools as RegAdmin. (this will be done in the near future)

Connect &&schemaName/&&schemaPass@&&serverName

	@@CUSTOMIZE_chemreg_base.sql

--*** Ends required configuration settings for Registration App 11.0.1 ***

Connect &&InstallUser/&&sysPass@&&serverName &&AsSysDBA;

	@@CREATE_chemreg_test_users.sql
	@@Grants.sql

	@@sql\Patches\Parameters.sql
	@@"sql\Patches\Patch 11.0.1\Parameters.sql"
	@@"sql\Patches\Patch 11.0.2\patch.sql"

	SET serveroutput on
	BEGIN
		:End:=to_char(systimestamp,'HH:MI:SS.FF4');
		:Elapsed:=to_timestamp(:End,'HH:MI:SS.FF4')-to_timestamp(:Begin,'HH:MI:SS.FF4');
		
		dbms_output.put_line('Begin: '||:Begin);
		dbms_output.put_line('End: '||:End);
		dbms_output.put_line('Elapsed: '||substr(:Elapsed,instr(:Elapsed,' ')+1,13));
	END;
	/
	


prompt logged session to: sql/log_create_chemreg_ora.txt
spool off

exit


	