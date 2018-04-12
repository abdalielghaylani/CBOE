--Copyright Cambridgesoft corp 1999-2005 all rights reserved

spool ON
spool sql\log_upgrade_cs_security_9_to_9SR2.txt

@@parameters.sql
@@prompts.sql

Connect &&schemaName/&&schemaPass@&&serverName
@@alter_cs_security_9_to_9SR2.sql

prompt logged session to: sql/log_upgrade_cs_security_9_to_9SR2.txt
spool off

exit

	