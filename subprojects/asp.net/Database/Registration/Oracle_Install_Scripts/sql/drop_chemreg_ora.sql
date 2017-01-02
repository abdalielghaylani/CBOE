-- Copyright Cambridgesoft corp 2001 all rights reserved.

prompt 
prompt Starting "CDrop_ChemReg_ora.sql"...
prompt 

-- Drop regdb database.
-- This script drops the table spaces and database owner for the coedb schema.

-- NOTE THIS SCRIPT MUST BE RUN FROM THE COMMAND LINE VERSION OF SQLPLUS
-- This script will not run from SQLPlus Worksheet!

SPOOL ON
spool sql\log_drop_chemreg_ora.txt

	@@Parameters.sql
	@@PromptsDrop.sql

CONNECT &&InstallUser/&&sysPass@&&serverName;

	@@Drops.sql


CONNECT &&securitySchemaName/&&securitySchemaPass@&&serverName;

	@@CleanCoeDB.sql


spool off;
exit


