-- Copyright 1999-2010 CambridgeSoft Corporation. All rights reserved
PROMPT ====>  Starting file HEADERFILE_Import_LegacyData.sql
PROMPT
PROMPT
PROMPT
DEFINE scriptlog = log\log_Import_LegacyData.txt
SPOOL &&scriptlog 
@@prompts.sql
@@parameters.sql

CONNECT &&schemaName/&&schemaPass@&&serverName
SET serveroutput on

VAR Begin VARCHAR2(20);
VAR End VARCHAR2(20);
VAR Elapsed VARCHAR2(100);
exec :Begin:=to_char(systimestamp,'HH:MI:SS.FF4');

@@import_regcompounds_sdf.sql
@@import_fragments_sdf.sql
@@import_legacy_users.sql
@@load_to_temp_tables.sql
@@register_from_temp_tables.sql

SET serveroutput on
BEGIN
	:End:=to_char(systimestamp,'HH:MI:SS.FF4');
	:Elapsed:=to_timestamp(:End,'HH:MI:SS.FF4')-to_timestamp(:Begin,'HH:MI:SS.FF4');
	dbms_output.put_line('');
	dbms_output.put_line('-----------------------------------');
	dbms_output.put_line('Legacy Data Import Script');
	dbms_output.put_line('Begin: '||:Begin);
	dbms_output.put_line('End: '||:End);
	dbms_output.put_line('Elapsed: '||substr(:Elapsed,instr(:Elapsed,' ')+1,13));
END;
/


PROMPT Logged session to: &&scriptlog 
SPOOL off

EXIT
