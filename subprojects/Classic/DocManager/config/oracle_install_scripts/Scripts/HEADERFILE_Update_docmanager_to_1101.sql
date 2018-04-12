--Copyright 1999-2003 CambridgeSoft Corporation. All rights reserved

spool ON
spool log_Update_DocManager_to_1101.txt

@@parameters.sql
@@prompts.sql


Connect &&schemaName/&&schemaPass@&&serverName
@@updateTo1101.sql

prompt logged session to: log_Update_DocManager_to_1101.txt
spool off

exit


	