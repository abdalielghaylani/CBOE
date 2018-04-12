--Copyright 1999-2004 CambridgeSoft Corporation. All rights reserved



@@parameters.sql
@@prompts.sql

set feedback on

spool sql\log_update_chemreg_db_from_8.0.txt

Connect &&schemaName/&&schemaPass@&&serverName
@@update_8.0_to_9.0.sql

spool off
exit

	