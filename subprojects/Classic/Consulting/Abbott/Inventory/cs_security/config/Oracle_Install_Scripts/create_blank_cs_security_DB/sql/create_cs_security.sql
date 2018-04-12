--Copyright Cambridgesoft corp 1999-2003 all rights reserved

spool ON
spool sql\log_create_cs_security.txt

@@parameters.sql
@@prompts.sql

Connect &&InstallUser/&&sysPass@&&serverName
@@drops.sql
@@tablespaces.sql
@@createTempTableSpace_&&OraVersionNumber
@@users.sql  
Connect &&schemaName/&&schemaPass@&&serverName
@@tables.sql
@@Create_Audit_Tables.sql
@@alter_cs_security_71_to_72.sql
@@alter_cs_security_72_to_80.sql
@@inserts.sql
Connect &&InstallUser/&&sysPass@&&serverName
@@synonyms.sql

prompt logged session to: sql/log_create_cs_security_ora.txt
spool off

exit

	