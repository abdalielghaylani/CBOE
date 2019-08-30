-- Copyright Cambridgesoft corp 2001 all rights reserved.

-- Drop regdb database.
-- This script drops the table spaces and database owner for the cs_security database.

-- NOTE THIS SCRIPT MUST BE RUN FROM THE COMMAND LINE VERSION OF SQLPLUS
-- This script will not run from SQLPlus Worksheet!


spool sql\log_drop_coedb_ora.txt

@@Parameters.sql
@@PromptsDrop.sql

CONNECT &&InstallUser/&&sysPass@&&serverName &&AsSysDBA

@@Drops.sql
@@Drops_N.sql

@"sql\Patches\Patch 11.0.1.0\sql\CsSecurity\Parameters.sql"
@"sql\Patches\Patch 11.0.1.0\sql\CsSecurity\Drop_CsSecurity.sql"

spool off;
exit


