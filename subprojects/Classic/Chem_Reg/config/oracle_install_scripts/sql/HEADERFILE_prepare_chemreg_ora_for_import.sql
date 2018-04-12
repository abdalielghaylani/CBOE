--Copyright 1999-2004 CambridgeSoft Corporation. All rights reserved

-- Prepares regdb for importing a data dump
-- Creates the empty regdb and sets up cs_security and then drops and recreates the regdb user

spool ON
spool sql\log_prepare_chemreg_for_import.txt

@@parameters.sql
@@prompts.sql

Connect &&InstallUser/&&sysPass@&&serverName
@@drops.sql
@@tablespaces.sql
@@users.sql  
Connect &&schemaName/&&schemaPass@&&serverName
@@CREATE_chemreg_ora.sql
@@ALTER_cs_security_for_chemreg_ora.sql
@@CREATE_chemreg_test_users.sql
@@2002_7.1.152_update_script.sql
@@2004_update_7.2.182_script.sql
@@indexClobFields&&OraVersionNumber.sql
Connect &&InstallUser/&&sysPass@&&serverName
@@synonyms.sql
prompt 'dropping user...'
DECLARE
	n NUMBER;
BEGIN
	select count(*) into n from dba_users where username = '&&schemaName';
	if n = 1 then
		execute immediate 'DROP USER &&schemaName CASCADE';
	end if;
END;
/
@@users.sql

prompt logged session to: sql/log_prepare_chemreg_for_import.txt
spool off

exit


	