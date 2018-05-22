--Copyright 1999-2010 CambridgeSoft Corporation. All rights reserved


spool Patches\log_Patches_CHEMINVDB2.txt

	@Parameters.sql 
	@Patches\Prompts.sql

Connect &&schemaName/&&schemaPass@&&serverName

	@Patches\Parameters.sql
        @Patches\PromptsVersion.sql

	VAR Begin VARCHAR2(20);
	VAR End VARCHAR2(20);
	VAR Elapsed VARCHAR2(100);
	exec :Begin:=to_char(systimestamp,'HH:MI:SS.FF4');

-- CSBR-161209: This block is only to patch 12.5.0
COL fromVersion new_value fromVersion NOPRINT
	SELECT	CASE
			WHEN  '&&fromVersion'='12.5.1'
			THEN  '12.5.0'
			ELSE  '&&fromVersion'
		END	AS fromVersion 
	FROM	DUAL;
	SELECT	CASE
			WHEN  '&&fromVersion'='12.5.1b'
			THEN  '12.5.1'
			ELSE  '&&fromVersion'
		END	AS fromVersion 
	FROM	DUAL;
-- End Patching
--' Alter schema user for tablespace usage to support Oracle 12c for upgrade installation
@@Alter\Alter_ChemInvUser.sql
Connect &&schemaName/&&schemaPass@&&serverName
	
	@"Patches\Patch &&fromVersion\parameters.sql"
	@"Patches\Patch &&nextPatch\patch.sql"
--When 17.1.0 is reached, we end the script execution and continue the remaining patch scripts as a fresh nested script to avoid problem.
--This is to avoid the oracle error SP2-0309: SQL*Plus command procedures may only be nested to a depth of 20.
--This is caused due to the nested script execution has reached the depth limit of 20 starting from 11.0.1
	SELECT	CASE
		WHEN  '&&currentPatch' = '17.1.0'
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
		dbms_output.put_line('logged session to: Patches\log_Patches_CHEMINVDB2.txt');
                dbms_output.put_line('**********************************************************');
	END;
	/
	

spool off

exit


	