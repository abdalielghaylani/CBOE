--Copyright Cambridgesoft corp 1999-2003 all rights reserved

SPOOL ON
spool log_create_sample_ora.txt

@@parameters.sql
@@prompts.sql

Connect &&InstallUser/&&sysPass@&&serverName
@@drops.sql
@@tablespaces.sql
@@users.sql  
Connect &&schemaName/&&schemaPass@&&serverName
@@tables.sql

-- Oracle 10.2.0.3 imp throws an error while importing into an existing table with clob
-- column.  We drop the table and let the imp recreated it
drop table moltable;
Connect &&InstallUser/&&sysPass@&&serverName
host imp.exe  userid=&&InstallUser/&&sysPass@&&serverName file=&&dumpFileName FULL=Y IGNORE=Y LOG=import.log


spool off

exit

	