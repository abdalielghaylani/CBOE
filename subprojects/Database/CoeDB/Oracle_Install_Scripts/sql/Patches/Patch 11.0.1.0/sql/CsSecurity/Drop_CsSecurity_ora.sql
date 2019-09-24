--Copyright 1999-2010 CambridgeSoft Corporation. All rights reserved

-- Drop regdb database.
-- This script drops the table spaces and database owner for the cs_security database.

-- NOTE THIS SCRIPT MUST BE RUN FROM THE COMMAND LINE VERSION OF SQLPLUS
-- This script will not run from SQLPlus Worksheet!

SPOOL ON
spool "sql\Patches\Patch 11.0.1.0\sql\CsSecurity\log_drop_CSSecurity_ora.txt"

@sql\Parameters.sql
@sql\Prompts.sql

CONNECT &&InstallUser/&&sysPass@&&serverName &&AsSysDBA

@"sql\Patches\Patch 11.0.1.0\sql\CsSecurity\drop_CsSecurity.sql"

spool off;
exit