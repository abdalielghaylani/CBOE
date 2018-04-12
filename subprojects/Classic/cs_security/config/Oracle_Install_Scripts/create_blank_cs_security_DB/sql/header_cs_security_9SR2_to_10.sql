--Copyright Cambridgesoft corp 1999-2005 all rights reserved

spool ON
spool sql\log_upgrade_cs_security_9SR2_to_10.txt

@@parameters.sql
@@prompts.sql

Connect &&schemaName/&&schemaPass@&&serverName
--@@alter_cs_security_9SR2_to_10.sql
-- Use the cs_security tablespace and tem tables space
@@pkg_users_body.sql;

@@globals.sql

prompt logged session to: sql/log_upgrade_cs_security_9SR2_to_10.txt
spool off

exit

	