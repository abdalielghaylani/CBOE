--Copyright 1999-2010 CambridgeSoft Corporation. All rights reserved

prompt 
prompt Starting "CsSecurity/HeaderFile_Upgrade_CsSecurity_TO_CoeDB_Y.sql"...
prompt

--Upgrade CoeDb 

@"sql\Patches\Patch 11.0.1.0\sql\CsSecurity\parameters.sql"
@@&&InstallUser

Connect &&InstallUser/&&sysPass@&&serverName &&AsSysDBA
@"sql\Patches\Patch 11.0.1.0\sql\cssecurity\Upgrade_CsSecurity_to_CoeDB_ora.sql"

Connect &&InstallUser/&&sysPass@&&serverName &&AsSysDBA

@"sql\Patches\Patch 11.0.1.0\sql\cssecurity\Alter_CsSecurity_ora.sql"
