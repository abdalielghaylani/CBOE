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
--Altering schema user to support oracle 12c
	@@alter_schemauser_for_oracle12c.sql

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
--Additional grants to the schema user to support oracle 12c
	@@Grant_schemauser_for_oracle12c.sql
	@@Grants.sql

	@@sql\Patches\Parameters.sql
	@@"sql\Patches\Patch 11.0.1\Parameters.sql"
	@@"sql\Patches\Patch 11.0.2\patch.sql"

--When 12.6.2 is reached, we end the script execution and continue the remaining patch scripts as a fresh nested script to avoid.
--This is to avoid the oracle error SP2-0309: SQL*Plus command procedures may only be nested to a depth of 20.
--This is caused due to the nested script execution has reached the depth limit of 20 starting from 11.0.1
	SELECT	CASE
		WHEN  '&&currentPatch' = '12.6.2'
		THEN  '"sql\Patches\Patch &&nextPatch\patch.sql"'
		ELSE  'sql\Patches\stop.sql'
	END	AS setNextPatch 
	FROM	DUAL;

	@&&setNextPatch 

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


	