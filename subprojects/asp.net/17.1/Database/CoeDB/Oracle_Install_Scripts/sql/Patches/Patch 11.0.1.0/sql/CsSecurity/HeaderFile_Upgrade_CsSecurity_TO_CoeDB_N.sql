--Copyright 1999-2010 CambridgeSoft Corporation. All rights reserved

prompt 
prompt Starting "sql\Patches\Patch 11.0.1.0\sql\cssecurity\HeaderFile_Upgrade_CsSecurity_TO_CoeDB_N.sql"...
prompt

--Create CsSecurity and Synonymous

@"sql\Patches\Patch 11.0.1.0\sql\cssecurity\parameters.sql"
DEFINE upgradecssecurity = 'N'
@"sql\Patches\Patch 11.0.1.0\sql\cssecurity\&&InstallUser"

Connect &&InstallUser/&&sysPass@&&serverName &&AsSysDBA

@"sql\Patches\Patch 11.0.1.0\sql\cssecurity\drop_CsSecurity.sql"
@"sql\Patches\Patch 11.0.1.0\sql\cssecurity\CREATE_CsSecurity_ora.sql"