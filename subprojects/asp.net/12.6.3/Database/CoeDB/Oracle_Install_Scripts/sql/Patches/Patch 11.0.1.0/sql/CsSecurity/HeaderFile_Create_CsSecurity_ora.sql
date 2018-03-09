--Copyright 1999-2010 CambridgeSoft Corporation. All rights reserved

spool ON
spool "sql\Patches\Patch 11.0.1.0\sql\cssecurity\log_create_CSSecurity_ora.txt"

@"sql\Patches\Patch 11.0.1.0\sql\cssecurity\parameters.sql"
@"sql\Patches\Patch 11.0.1.0\sql\cssecurity\prompts.sql"

Connect &&InstallUser/&&sysPass@&&serverName &&AsSysDBA

@"sql\Patches\Patch 11.0.1.0\sql\cssecurity\drop_CsSecurity.sql"
@"sql\Patches\Patch 11.0.1.0\sql\cssecurity\CREATE_CsSecurity_ora.sql"

prompt logged session to: sql\Patches\Patch  11.0.1.0\sql\cssecurity\log_create_CSSecurity_ora.txt
spool off

exit
