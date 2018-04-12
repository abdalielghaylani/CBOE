--Copyright 1999-2004 CambridgeSoft Corporation. All rights reserved



@@parameters.sql
@@prompts.sql

set feedback on

spool sql\log_update_chemreg_db_from_9.0_SR3_to_10.0.txt

Connect &&schemaName/&&schemaPass@&&serverName
@@update_9.0SR3_to_10.0.sql
--@@indexClobFields&&OraVersionNumber
spool off
exit

	