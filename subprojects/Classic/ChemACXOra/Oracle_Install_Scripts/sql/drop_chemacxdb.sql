-- Copyright Cambridgesoft corp 2003-2004 all rights reserved.

-- Drop ChemACX database.
-- This script drops the table spaces and database owner for the ChemACX database.

-- NOTE THIS SCRIPT MUST BE RUN FROM THE COMMAND LINE VERSION OF SQLPLUS
-- This script will not run from SQLPlus Worksheet!

SPOOL ON
spool sql\log_drop_chemacxdb.txt

@@parameters.sql
@@prompts2.sql
CONNECT &&InstallUser/&&sysPass@&&serverName;
@@drops.sql

spool off;
exit

