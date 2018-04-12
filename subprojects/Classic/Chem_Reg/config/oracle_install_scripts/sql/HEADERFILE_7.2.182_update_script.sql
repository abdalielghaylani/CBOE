--Copyright 1999-2004 CambridgeSoft Corporation. All rights reserved



@@parameters.sql
@@prompts.sql

-- first spool some grants we will need later
Connect &&schemaName/&&schemaPass@&&serverName
set echo off
@@spoolgrants.sql
@@spooltablemove.sql

set feedback on

spool sql\log_update_chemreg_db_from_7.2.182.txt

Connect &&InstallUser/&&sysPass@&&serverName
@@tablespaces.sql
Connect &&schemaName/&&schemaPass@&&serverName
@@2004_update_to_CLOB.sql
Connect &&InstallUser/&&sysPass@&&serverName
@@2004_update_7.2.182_script.sql
Connect &&schemaName/&&schemaPass@&&serverName
--if moving from a different machine this is a good thing to run
@@synonyms.sql
@@moveToNewTablespaces.sql
@@indexClobFields&&OraVersionNumber
@@alterusers.sql
spool off
exit

	