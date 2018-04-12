--Copyright Cambridgesoft corp 1999-2002 all rights reserved


spool sql\log_update_chemreg_db_from_7.1.152.txt
SET ECHO OFF
SET verify off


@@parameters.sql
@@prompts.sql
Connect &&InstallUser/&&sysPass@&&serverName
@@tablespaces.sql
@@2002_7.1.152_update_script.sql
@@2004_update_7.2.182_script.sql
Connect &&schemaName/&&schemaPass@&&serverName
@@2004_update_to_CLOB.sql
@@indexClobFields&&OraVersionNumber
spool off
exit
	