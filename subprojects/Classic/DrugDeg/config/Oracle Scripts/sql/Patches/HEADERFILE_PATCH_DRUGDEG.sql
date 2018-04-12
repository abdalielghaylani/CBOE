--Copyright 1999-2010 CambridgeSoft Corporation. All rights reserved


spool Patches\log_Patches_DRUGDEG.txt

	@Parameters.sql 
	@Patches\Prompts.sql

Connect &&schemaName/&&schemaPass@&&serverName

	@Patches\Parameters.sql
        @Patches\PromptsVersion.sql

	VAR Begin VARCHAR2(20);
	VAR End VARCHAR2(20);
	VAR Elapsed VARCHAR2(100);
	exec :Begin:=to_char(systimestamp,'HH:MI:SS.FF4');

	@"Patches\Patch &&fromVersion\parameters.sql"
	@"Patches\Patch &&nextPatch\patch.sql"


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
		dbms_output.put_line('logged session to: Patches\log_Patches_DRUGDEG.txt');
                dbms_output.put_line('**********************************************************');
	END;
	/
	

spool off

exit


	