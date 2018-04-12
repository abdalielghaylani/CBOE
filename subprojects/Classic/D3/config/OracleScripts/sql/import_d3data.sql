--Copyright Cambridgesoft corp 1999-2003 all rights reserved

SPOOL ON
spool log_import_d3data.txt

@@parameters.sql
@@prompts.sql


@@import_script.sql

spool off

exit

	