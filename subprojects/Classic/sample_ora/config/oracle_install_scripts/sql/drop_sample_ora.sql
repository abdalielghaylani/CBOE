
SPOOL ON
spool sql\log_drop_sample_ora.txt
SET ECHO OFF
SET verify off

DEFINE schemaName = 'SAMPLE'
DEFINE tableSpaceName = 'T_&&schemaName'
DEFINE indexTableSpaceName = 'T_&&schemaName._INDEX'
DEFINE lobsTableSpaceName = 'T_&&schemaName._LOBS'

ACCEPT serverName Char DEFAULT '' PROMPT 'Enter the Oracle service name:'
ACCEPT InstallUser Char DEFAULT 'system' PROMPT 'Enter the name of an Oracle account with system privileges (system):'
ACCEPT sysPass Char DEFAULT 'manager' PROMPT 'Enter the system account password (manager):'

CONNECT &&InstallUser/&&sysPass@&&serverName;

prompt 'Droping user...'

DECLARE
	n NUMBER;
BEGIN
	select count(*) into n from dba_users where username = '&&schemaName';
	if n > 0 then
		execute immediate 'DROP USER &&schemaName CASCADE';
	end if;
END;
/

prompt 'Droping tablespaces...'

DECLARE
	n NUMBER;
BEGIN
	select count(*) into n from dba_tablespaces where tablespace_name = '&&tableSpaceName';
	if n > 0 then
		execute immediate '
		DROP TABLESPACE &&tableSpaceName INCLUDING CONTENTS CASCADE CONSTRAINTS';
	end if;
	
	select count(*) into n from dba_tablespaces where tablespace_name = 'T_SAMPLE_TEMP';
	if n > 0 then
		execute immediate '
		DROP TABLESPACE T_SAMPLE_TEMP INCLUDING CONTENTS CASCADE CONSTRAINTS';
	end if;
		
	select count(*) into n from dba_tablespaces where tablespace_name = '&&indexTableSpaceName';
	if n > 0 then
		execute immediate '
		DROP TABLESPACE &&indexTableSpaceName INCLUDING CONTENTS CASCADE CONSTRAINTS';
	end if;
	
	select count(*) into n from dba_tablespaces where tablespace_name = '&&lobsTableSpaceName';
	if n > 0 then
		execute immediate '
		DROP TABLESPACE &&lobsTableSpaceName INCLUDING CONTENTS CASCADE CONSTRAINTS';
	end if;
end;
/

spool off;
exit

