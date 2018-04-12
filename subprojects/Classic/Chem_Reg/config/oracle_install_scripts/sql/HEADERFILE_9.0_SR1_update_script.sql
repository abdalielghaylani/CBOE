--Copyright 1999-2004 CambridgeSoft Corporation. All rights reserved



@@parameters.sql
@@prompts.sql

set feedback on

spool sql\log_update_chemreg_db_from_9.0_SR1_to_9.0_SR2.txt

Connect &&schemaName/&&schemaPass@&&serverName
@@update_9.0SR1_to_9.0SR2.sql

spool off
exit

	