--Copyright 1999-2003 CambridgeSoft Corporation. All rights reserved

@@parameters.sql
@@prompts.sql

-- first spool some grants we will need later
Connect &&schemaName/&&schemaPass@&&serverName
set echo off
@@spoolgrants.sql
@@spooltablemove.sql

set feedback on

spool sql\log_update_chemreg_db_from_d67.txt

Connect &&InstallUser/&&sysPass@&&serverName
@@tablespaces.sql
Connect &&schemaName/&&schemaPass@&&serverName
@@2004_update_to_CLOB.sql
Connect &&InstallUser/&&sysPass@&&serverName
@@2000ba67_update_script.sql
@@2002_7.1.152_update_script.sql
@@2004_update_7.2.182_script.sql
Connect &&schemaName/&&schemaPass@&&serverName
@@moveToNewTablespaces.sql
@@alterusers.sql
--if moving from a different machine this is a good thing to run
@@synonyms.sql
@@indexClobFields&&OraVersionNumber
@@update_mw2_formula2.sql
spool off
exit