--Copyright 1999-2003 CambridgeSoft Corporation. All rights reserved

spool ON
spool log_Create_DocManager.txt

@@parameters.sql
@@prompts.sql

Connect &&InstallUser/&&sysPass@&&serverName
@@drops.sql
@@tablespaces.sql
@@users.sql
alter user cscartridge quota unlimited on &&cscartTableSpaceName;    
@@ctx_schedule.sql
Connect &&schemaName/&&schemaPass@&&serverName
@@CREATE_docmgr_ora.sql
@@audit\Create_Audit_Tables.sql
@@indexClobFields.sql
@@update_cs_security_for_docmanager.sql
@@add_docmgr_users.sql
Connect &&InstallUser/&&sysPass@&&serverName
@@synonyms.sql
Connect &&schemaName/&&schemaPass@&&serverName
@@updateTo9SR4.sql
@@updateTo1101.sql

Connect &&schemaName/&&schemaPass@&&serverName
-- ' Applying the latest patch
@@"Patches\Parameters.sql"
@@"Patches\Patch &&schemaVersion\Parameters.sql"
@@"Patches\Patch &&nextPatch\patch.sql"

prompt logged session to: scripts/log_create_docmanager.txt
spool off

exit


	