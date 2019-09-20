--Copyright 1999-2010 CambridgeSoft Corporation. All rights reserved


spool sql\Patches\log_Patches_CoeDB_ora.txt

	@sql\Parameters.sql 
	@sql\Patches\Prompts.sql

Connect &&InstallUser/&&sysPass@&&serverName &&AsSysDBA;

	@sql\Patches\premigration_validations.sql

	@sql\Patches\Parameters.sql
        @sql\Patches\PromptsVersion.sql

	VAR Begin VARCHAR2(20);
	VAR End VARCHAR2(20);
	VAR Elapsed VARCHAR2(100);
	exec :Begin:=to_char(systimestamp,'HH:MI:SS.FF4');
	--To support wm_concat function in Oracle12c; creating a user defined function for the same
Connect &&schemaName/&&schemaPass@&&serverName
	@"sql\t_wm_concat.sql"
CONNECT &&InstallUser/&&sysPass@&&serverName &&AsSysDBA

	@"sql\Patches\Patch &&fromVersion\parameters.sql"
	
-- There we run patches. IT works fine. For example, if we need update from 17.1 to 17.1.1, it executes only 
-- first patch. Others "setNextPatch" Variables will be = 'sql\Patches\stop.sql'
--For each new version of Security need to add new record @&&setNextPatch 
	@"sql\Patches\Patch &&nextPatch\patch.sql"

--11.0.3
@&&setNextPatch 
--11.0.4
@&&setNextPatch 
--12.1.0
@&&setNextPatch 
--12.1.3
@&&setNextPatch 
--12.3.0
@&&setNextPatch 
--12.4.0
@&&setNextPatch 
--12.5.0
@&&setNextPatch 
--12.5.2
@&&setNextPatch 
--12.5.3
@&&setNextPatch 
--12.6.0
@&&setNextPatch 
--12.6.1
@&&setNextPatch 
--12.6.2
@&&setNextPatch 
--12.6.3
@&&setNextPatch 
--17.1.0
@&&setNextPatch 
--17.1.1
@&&setNextPatch 
--18.1.0
@&&setNextPatch 
--18.1.1
@&&setNextPatch 
--19.1.0
@&&setNextPatch 




	SET serveroutput on
	VAR toVersion VARCHAR2(100); 			
	BEGIN
		SELECT Value INTO :toVersion FROM &&schemaName..CoeGlobals WHERE UPPER(ID) = 'SCHEMAVERSION';
		:End:=to_char(systimestamp,'HH:MI:SS.FF4');
		:Elapsed:=to_timestamp(:End,'HH:MI:SS.FF4')-to_timestamp(:Begin,'HH:MI:SS.FF4');
	        dbms_output.put_line('************************  Summary ************************');
		dbms_output.put_line('Patches Applied'||CHR(10));
                dbms_output.put_line('From Version: &&fromVersion');
                dbms_output.put_line('To Version  : '||:toVersion||CHR(10));
		dbms_output.put_line('Begin       : '||:Begin);
		dbms_output.put_line('End         : '||:End);
		dbms_output.put_line('Elapsed     : '||substr(:Elapsed,instr(:Elapsed,' ')+1,13)||CHR(10));
		dbms_output.put_line('logged session to: sql\Patches\log_Patches_CoeDB_ora.txt');
                dbms_output.put_line('**********************************************************');
	END;
	/
	

spool off

exit


	