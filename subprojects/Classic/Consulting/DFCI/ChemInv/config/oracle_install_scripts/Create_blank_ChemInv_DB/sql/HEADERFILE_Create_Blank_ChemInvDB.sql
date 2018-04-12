-- Copyright Cambridgesoft Corp 2001-2006 all rights reserved

--#########################################################
-- Headerfile for creating blank cheminvdb2 schema
--######################################################### 

spool ON
spool Logs\LOG_Create_blank_CheminvDB.txt

--' Intialize variables
@@Parameters.sql
@@Prompts.sql

--' Do db cleanup, drop schema, users, and roles
@@Drop\Drop_Synonyms.sql
@@Drop\Drop_UsersRoles.sql
@@Drop\Drop_Tablespaces.sql
@@Drop\Drop_PrivilegeTable.sql
@@Data\Delete_CsSecurityData.sql

--' Create tablespaces and SchemaOwner
@@Create\Create_Tablespaces.sql
@@Create\Create_SchemaOwner.sql

--' Alter catridge user for tablespace usage
@@Alter\Alter_CartridgeUser.sql

--' Create schema pre-req's
@@Create\Create_SchemaPreReq.sql

--' Create schema tables
@@Create\Create_Tables.sql

--' Create schema views
@@Create\Create_Views.sql

--' Create packages and procedures
@@PLSQL\Create_PLSQL.sql

--' Create business logic table triggers
@@PLSQL\Create_Triggers.sql

--' Alter cs_security to integrate the Inventory application
@@Alter\Alter_Cs_Security.sql

--' Create default users
@@Create\Create_Default_users.sql

--' Create synonyms
@@Create\Create_Synonyms.sql

--' Insert application data
@@Data\Insert_InventoryData.sql
@@Data\IncrementSequences.sql

--' Recompile pl/sql 
@@PLSQL\RecompilePLSQL.sql

prompt '#########################################################'
prompt 'Logged session to: Logs\LOG_Create_blank_CheminvDB.txt'
prompt '#########################################################'

prompt 
spool off

exit


	