--Copyright Cambridgesoft corp 1999-2003 all rights reserved

@..\..\sql\parameters.sql
@@prompts.sql


spool  LOG_ImportBioSARDump.txt


Connect &&InstallUser/&&sysPass@&&serverName
@..\..\sql\drops.sql
@..\..\sql\tablespaces.sql
@..\..\sql\users.sql  

host imp.exe userid=&&InstallUser/&&sysPass@&&serverName file=&&dumpFileName FULL=Y GRANTS=N IGNORE=Y LOG=import.log


@..\..\sql\ALTER_cs_security_biosar_browser.sql
@..\..\sql\UPDATE_cs_security_biosar_browser.sql
@..\..\sql\BioSar_Browser_test_users.sql
Connect &&InstallUser/&&sysPass@&&serverName


@..\..\sql\synonyms.sql

prompt recreating roles and grants
@..\exported_data\recreate_biosar_roles_and_grants.sql

spool off

exit

	