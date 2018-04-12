--Copyright 1999-2003 CambridgeSoft Corporation. All rights reserved


-- first spool some grants we will need later
Connect &&schemaName/&&schemaPass@&&serverName
set echo off
@@spoolgrants.sql
@@spooltablemove.sql

set feedback on

spool sql\log_update_chemreg_db_from_d25.txt

Connect &&InstallUser/&&sysPass@&&serverName
@@tablespaces.sql
Connect &&schemaName/&&schemaPass@&&serverName
@@2004_update_to_CLOB.sql
Connect &&InstallUser/&&sysPass@&&serverName
@@2000bd25_update_script.sql
@@2000ba67_update_script.sql
@@2002_7.1.152_update_script.sql
@@2004_update_7.2.182_script.sql
Connect &&schemaName/&&schemaPass@&&serverName
@@moveToNewTablespaces.sql
@@indexClobFields&&OraVersionNumber
@@alterusers.sql
spool off
exit


	