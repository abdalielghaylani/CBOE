--Copyright 1999-2012 PerkinElmer Inc. All rights reserved

spool Upgrade_COEDB_9SR4_To_11.0.0.txt

@parameters.sql
@prompts.sql

CONNECT &&InstallUser/&&sysPass@&&serverName

VAR Begin VARCHAR2(20);
VAR End VARCHAR2(20);
VAR Elapsed VARCHAR2(100);
exec :Begin:=to_char(systimestamp,'HH:MI:SS.FF4');

@drops.sql
@tablespaces.sql
@CreateTempTablespace_9.sql
@users.sql

host imp &&InstallUser/&&sysPass@&&serverName FILE="CoeDB.dmp" LOG="log_ImpCoeDB.txt" fromuser=COEDB touser=COEDB TABLES=COEGLOBALS IGNORE=YES

SET serveroutput on
BEGIN
	:End:=to_char(systimestamp,'HH:MI:SS.FF4');
	:Elapsed:=to_timestamp(:End,'HH:MI:SS.FF4')-to_timestamp(:Begin,'HH:MI:SS.FF4');

	dbms_output.put_line('Begin: '||:Begin);
	dbms_output.put_line('End: '||:End);
	dbms_output.put_line('Elapsed: '||substr(:Elapsed,instr(:Elapsed,' ')+1,13));
END;
/

prompt logged session to: sql/Upgrade_COEDB_9SR4_To_11.0.0.txt
spool off

exit