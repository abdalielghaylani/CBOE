--Copyright 1999-2010 CambridgeSoft Corporation. All rights reserved

spool ON
spool "sql\Patches\Patch 11.0.1.0\sql\cssecurity\log_Upgrade_cssecurity_to_coedb_ora.txt"

@"sql\Patches\Patch 11.0.1.0\sql\cssecurity\parameters.sql"
@"sql\Patches\Patch 11.0.1.0\sql\cssecurity\prompts.sql"

Connect &&InstallUser/&&sysPass@&&serverName &&AsSysDBA

VAR Begin VARCHAR2(20);
VAR End VARCHAR2(20);
VAR Elapsed VARCHAR2(100);
exec :Begin:=to_char(systimestamp,'HH:MI:SS.FF4');

@"sql\Patches\Patch 11.0.1.0\sql\drops.sql"
@"sql\Patches\Patch 11.0.1.0\sql\tablespaces.sql"
@"sql\Patches\Patch 11.0.1.0\sql\\CsSecurity\createTempTableSpace_&&OraVersionNumber"
@"sql\Patches\Patch 11.0.1.0\sql\users.sql"
@"sql\Patches\Patch 11.0.1.0\sql\CREATE_COEDB_ora.sql"
@"sql\Patches\Patch 11.0.1.0\sql\cssecurity\Upgrade_CsSecurity_to_CoeDB_ora.sql"
@"sql\Patches\Patch 11.0.1.0\sql\synonyms.sql"
@"sql\Patches\Patch 11.0.1.0\sql\coeuser.sql"
@"sql\Patches\Patch 11.0.1.0\sql\inserts.sql"
@"sql\Patches\Patch 11.0.1.0\sql\grants.sql"
@"sql\Patches\Patch 11.0.1.0\sql\proxyGrants.sql"
@"sql\Patches\Patch 11.0.1.0\sql\roles.sql"

Connect &&InstallUser/&&sysPass@&&serverName &&AsSysDBA
@drop_CsSecurity.sql
@CREATE_CsSecurity_ora.sql

SET serveroutput on
BEGIN
	:End:=to_char(systimestamp,'HH:MI:SS.FF4');
	:Elapsed:=to_timestamp(:End,'HH:MI:SS.FF4')-to_timestamp(:Begin,'HH:MI:SS.FF4');

	dbms_output.put_line('Begin: '||:Begin);
	dbms_output.put_line('End: '||:End);
	dbms_output.put_line('Elapsed: '||substr(:Elapsed,instr(:Elapsed,' ')+1,13));
END;
/

prompt logged session to: sql\Patches\Patch 11.0.1.0\sql\cssecurity\log_Upgrade_cssecurity_to_coedb_ora.txt
spool off

exit
