-- Copyright Cambridgesoft corp 2001 all rights reserved.

-- Drop regdb database.
-- This script drops the table spaces and database owner for the cs_security database.

-- NOTE THIS SCRIPT MUST BE RUN FROM THE COMMAND LINE VERSION OF SQLPLUS
-- This script will not run from SQLPlus Worksheet!

SPOOL ON
spool log_drop_docmanager.txt

@@parameters.sql
@@prompts.sql
CONNECT &&InstallUser/&&sysPass@&&serverName;
@@drops.sql

spool off;
exit

