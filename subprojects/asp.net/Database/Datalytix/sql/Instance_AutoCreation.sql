-- Copyright 1998-2014 CambridgeSoft Corporation, an indirect, wholly-owned subsidiary of PerkinElmer, Inc. All rights reserved.

spool ON
spool log_auto_create_globalschema_ora.txt

@@"..\..\COEDB\Oracle_Install_Scripts\sql\parameters.sql"
@@"..\..\COEDB\Oracle_Install_Scripts\sql\Patches\Patch 12.6.2\parameters.sql"

DEFINE serverName=&1
DEFINE InstallUser=&2
DEFINE sysPass=&3
DEFINE globalSchemaName=&4 
DEFINE globalSchemaPass=&5

@@Instance_CreateAll.sql

prompt logged session to: log_auto_create_globalschema_ora.txt
spool off

exit
