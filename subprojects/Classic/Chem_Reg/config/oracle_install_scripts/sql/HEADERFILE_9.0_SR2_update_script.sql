--Copyright 1999-2004 CambridgeSoft Corporation. All rights reserved



@@parameters.sql
@@prompts.sql

set feedback on

spool sql\log_update_chemreg_db_from_9.0_SR2_to_9.0_SR3.txt

Connect &&schemaName/&&schemaPass@&&serverName
@@update_9.0SR2_to_9.0SR3.sql

spool off
exit

	