-- Copyright Cambridgesoft Corp 2001-2005 all rights reserved.

-- Update ChemInvDB from 8.0SR2 to 9.0, which is also schemaversion 3.0 to 4.0.

spool ON
spool ..\..\Logs\LOG_Update_ChemInvDB_8.0SR2_to_9.0.txt

@@..\..\parameters.sql
@@..\..\prompts.sql

--Inv user needs privileges to add foreign key constraints to the people table
connect &&securitySchemaName/&&securitySchemaPass@&&serverName;
GRANT REFERENCES ON people TO &&schemaName;

Connect &&schemaName/&&schemaPass@&&serverName
@@..\..\globals.sql
@@Update_Tables.SQL
@@..\..\Create_ChemInvDB_views.SQL
@@Update_PLSQL.SQL
@@Update_Grants.sql;
@@Update_CSSecurity.sql
@@Update_TableData.SQL
Connect &&InstallUser/&&sysPass@&&serverName
@@..\..\synonyms.SQL
@@..\..\RecompilePLSQL.sql

host imp &&schemaName/&&schemaPass@&&serverName file=..\..\Dump_Files\Fill_ChemInvDb_Picklists.dmp ignore=yes full=yes log=..\..\Logs\LOG_Update_ChemInvDB_Picklists.txt

spool off
exit


 


