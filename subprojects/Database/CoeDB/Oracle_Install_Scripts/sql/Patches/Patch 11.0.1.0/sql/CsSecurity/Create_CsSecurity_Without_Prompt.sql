--Copyright 1999-2010 CambridgeSoft Corporation. All rights reserved

@"sql\Patches\Patch 11.0.1.0\sql\CsSecurity\parameters.sql"
@&&InstallUser

Connect &&InstallUser/&&sysPass@&&serverName &&AsSysDBA

@"sql\Patches\Patch 11.0.1.0\sql\CsSecurity\drop_CsSecurity.sql"
@"sql\Patches\Patch 11.0.1.0\sql\CsSecurity\CREATE_CsSecurity_ora.sql"

