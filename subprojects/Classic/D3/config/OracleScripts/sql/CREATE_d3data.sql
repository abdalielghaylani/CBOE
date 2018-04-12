--Copyright Cambridgesoft corp 1999-2003 all rights reserved

SPOOL ON
spool log_create_d3data.txt

@@parameters.sql
@@prompts.sql

Connect &&InstallUser/&&sysPass@&&serverName
@@drops.sql
@@tablespaces.sql
@@users.sql  
alter user cscartridge quota unlimited on &&cscartTableSpaceName;  

Connect &&schemaName/&&schemaPass@&&serverName
@@tables.sql
@@indexes.sql

Connect &&InstallUser/&&sysPass@&&serverName
@@synonyms.sql


Connect &&schemaName/&&schemaPass@&&serverName
@update_cs_security_for_drugdeg.sql


Connect &&InstallUser/&&sysPass@&&serverName
@@testusers.sql

@@update_10_to_1101.sql


@@import_script.sql

spool off

exit

	