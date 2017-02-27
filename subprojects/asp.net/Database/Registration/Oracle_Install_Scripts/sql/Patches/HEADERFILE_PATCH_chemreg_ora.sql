--Copyright 1999-2010 CambridgeSoft Corporation. All rights reserved


spool sql\Patches\log_Patches_chemreg_ora.txt

	@sql\Parameters.sql 
	@sql\Patches\Prompts.sql

Connect &&InstallUser/&&sysPass@&&serverName &&AsSysDBA;

	@sql\Patches\Parameters.sql
        @sql\Patches\PromptsVersion.sql

	VAR Begin VARCHAR2(20);
	VAR End VARCHAR2(20);
	VAR Elapsed VARCHAR2(100);
	exec :Begin:=to_char(systimestamp,'HH:MI:SS.FF4');
--Altering the schema user to support oracle 12c
	@sql\alter_schemauser_for_oracle12c.sql
--Additional grants to the schema user to support oracle 12c
	@sql\Grant_schemauser_for_oracle12c.sql

	@"sql\Patches\Patch &&fromVersion\parameters.sql"
	@"sql\Patches\Patch &&nextPatch\patch.sql"

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
	VAR toVersion VARCHAR2(100); 			
	BEGIN
		SELECT Value INTO :toVersion FROM &&schemaName..Globals WHERE UPPER(ID) = 'VERSION_SCHEMA';
		:End:=to_char(systimestamp,'HH:MI:SS.FF4');
		:Elapsed:=to_timestamp(:End,'HH:MI:SS.FF4')-to_timestamp(:Begin,'HH:MI:SS.FF4');
	        dbms_output.put_line('************************  Summary ************************');
		dbms_output.put_line('Patches Applied'||CHR(10));
                dbms_output.put_line('From Version: &&fromVersion');
                dbms_output.put_line('To Version  : '||:toVersion||CHR(10));
		dbms_output.put_line('Begin       : '||:Begin);
		dbms_output.put_line('End         : '||:End);
		dbms_output.put_line('Elapsed     : '||substr(:Elapsed,instr(:Elapsed,' ')+1,13)||CHR(10));
		dbms_output.put_line('logged session to: sql\Patches\log_Patches_chemreg_ora.txt');
                dbms_output.put_line('**********************************************************');
	END;
	/
	

spool off

exit


	