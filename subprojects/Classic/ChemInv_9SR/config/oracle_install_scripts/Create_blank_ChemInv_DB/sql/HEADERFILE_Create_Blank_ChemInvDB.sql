-- Copyright Cambridgesoft Corp 2001-2005 all rights reserved

spool ON
spool Logs\LOG_Create_blank_CheminvDB.txt

@@parameters.sql
@@prompts.sql

Connect &&InstallUser/&&sysPass@&&serverName
@@drops.sql
@@tablespaces.sql
@@users.sql
alter user cscartridge quota unlimited on &&cscartTableSpaceName;  
Connect &&schemaName/&&schemaPass@&&serverName
@@packages\pkg_Constants_def.sql;
--Inv user needs privileges to add foreign key constraints to the people table
connect &&securitySchemaName/&&securitySchemaPass@&&serverName;
GRANT REFERENCES ON people TO &&schemaName;
Connect &&schemaName/&&schemaPass@&&serverName
@@Create_ChemInvDB.sql;
@@indexClobFields&&OraVersionNumber
@@Create_Audit_Tables.sql;
@@Create_ChemInvDB_views.sql;
@@Create_ChemInvDB_Packages_Procedures.sql
@@Alter_CS_security_for_ChemInv.sql;
@@Create_ChemInvDB_Test_Users.sql;         
--@@packages\pkg_FastIndexAccess.sql --this connects as cscartridge
Connect &&InstallUser/&&sysPass@&&serverName
@@synonyms.sql
Connect &&schemaName/&&schemaPass@&&serverName
alter table inv_locations disable constraint INV_LOC_LOC_FK;
host imp &&schemaName/&&schemaPass@&&serverName file=Dump_Files\Fill_ChemInvDb_Picklists.dmp ignore=yes full=yes log=Logs\LOG_Fill_ChemInvDB_Picklists.txt
alter table inv_locations enable constraint INV_LOC_LOC_FK;
@@inserts.sql
@@incrementSequences.sql
@@RecompilePLSQL.sql
prompt logged session to: Logs\LOG_Create_blank_CheminvDB.txt
spool off

exit


	