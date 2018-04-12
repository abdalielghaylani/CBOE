--Copyright Cambridgesoft corp 1999-2003 all rights reserved

spool ON
spool sql\log_upgrade_cs_security_71_to_80.txt

@@parameters.sql
@@prompts.sql

Connect &&InstallUser/&&sysPass@&&serverName
@@tablespaces.sql
@@createTempTableSpace_&&OraVersionNumber
Connect &&schemaName/&&schemaPass@&&serverName
@@alter_cs_security_71_to_72.sql
@@alter_cs_security_72_to_80.sql
prompt logged session to: sql/log_upgrage_cs_security_71_to_80.txt
spool off

exit

	