--Copyright Cambridgesoft corp 1999-2003 all rights reserved

SPOOL ON
spool log_create_DrugDeg.txt

@@parameters.sql
@@prompts.sql

Connect &&InstallUser/&&sysPass@&&serverName
@@drops.sql
@@tablespaces.sql
@@users.sql  
alter user cscartridge quota unlimited on &&cscartTableSpaceName;    

Connect &&schemaName/&&schemaPass@&&serverName
@@tables.sql

Connect &&InstallUser/&&sysPass@&&serverName
@@synonyms.sql

Connect &&schemaName/&&schemaPass@&&serverName
@@indexes.sql
@@triggers.sql
@@update_cs_security_for_drugdeg.sql


Connect &&InstallUser/&&sysPass@&&serverName
@@testusers.sql

Connect &&schemaName/&&schemaPass@&&serverName
@@statuses.sql


Connect &&schemaName/&&schemaPass@&&serverName
@@globals.sql


@@cartindex.sql

@@update_10_to_1101.sql

-- ' Applying the latest patch
@@"Patches\Parameters.sql"
@@"Patches\Patch &&schemaVersion\Parameters.sql"
@@"Patches\Patch &&nextPatch\patch.sql"

spool off

exit

	