--Copyright Cambridgesoft corp 1999-2003 all rights reserved

@..\..\sql\parameters.sql
@@prompts.sql





Connect &&InstallUser/&&sysPass@&&serverName


host exp.exe userid=&&InstallUser/&&sysPass@&&serverName file=&&dumpFileName parfile=biosar.exp


@spool_biosar_permissions.sql

spool LOG_ExportBioSAR.txt append

prompt
prompt
prompt script for recreating roles and grants has been spooled to ..\exported_data\recreate_biosar_roles_and_grants.sql

spool off

exit

	