--Copyright 1999-2010 CambridgeSoft Corporation. All rights reserved

spool ON
spool sql\log_FIX_DB_ora.txt

@@Prompts.sql

Connect &&InstallUser/&&sysPass@&&serverName as SYSDBA;

--#########################################################
--RECOMPILING STARTED
--#########################################################

@@Recompiling.sql

prompt logged session to: sql\log_FIX_DB_ora.txt
spool off

exit


	