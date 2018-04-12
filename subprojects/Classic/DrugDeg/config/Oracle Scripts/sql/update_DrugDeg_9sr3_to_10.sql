--Copyright Cambridgesoft corp 1999-2003 all rights reserved

SPOOL ON
spool log_update_DrugDeg.txt

@@parameters.sql
@@prompts.sql



Connect &&schemaName/&&schemaPass@&&serverName
@@globals.sql



spool off

exit

	