--Copyright Cambridgesoft corp 1999-2003 all rights reserved

spool sql\log_create_chemacx.txt

@@parameters.sql
@@prompts.sql

Connect &&InstallUser/&&sysPass@&&serverName
@@drops.sql
@@tablespaces.sql
@@users.sql
alter user cscartridge quota unlimited on &&cscartTableSpaceName;    
Connect &&schemaName/&&schemaPass@&&serverName
@@globals.sql
@@tables.sql
Connect &&InstallUser/&&sysPass@&&serverName
@@synonyms.sql
Connect &&securitySchemaName/&&securitySchemaPass@&&serverName
@@alter_cs_security_for_chemacx_ora.sql
@@create_chemacx_test_users.sql;
spool off
Connect &&InstallUser/&&sysPass@&&serverName
prompt foo
@@importdata&&importdata


exit

	