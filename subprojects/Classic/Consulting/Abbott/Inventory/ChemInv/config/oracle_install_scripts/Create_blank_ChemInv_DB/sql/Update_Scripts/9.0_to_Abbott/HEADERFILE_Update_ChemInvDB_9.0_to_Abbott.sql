-- Copyright Cambridgesoft Corp 2001-2005 all rights reserved.

-- Update ChemInvDB from 9.0 to Abbott

spool ON
spool ..\..\Logs\LOG_Update_ChemInvDB_9.0_to_Abbott.txt

@@parameters.sql
@@..\..\prompts.sql

--Inv user needs privileges to add foreign key constraints to the people table
connect &&securitySchemaName/&&securitySchemaPass@&&serverName;
GRANT REFERENCES ON people TO &&schemaName;

Connect &&schemaName/&&schemaPass@&&serverName
@@..\..\globals.sql
@@Update_Tables.SQL
@@Update_TableData.SQL
@@Update_PLSQL.SQL
--@@Update_Views.sql
@@Update_Grants.sql;
@@Update_CSSecurity.sql

Connect &&InstallUser/&&sysPass@&&serverName
@@Update_Synonyms.sql
@@..\..\RecompilePLSQL.sql

spool off
exit


 


