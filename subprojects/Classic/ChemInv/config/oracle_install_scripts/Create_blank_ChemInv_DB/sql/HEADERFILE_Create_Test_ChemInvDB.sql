-- Copyright Cambridgesoft Corp 2001-2006 all rights reserved

--#########################################################
-- Headerfile for creating test cheminvdb2 schema
--######################################################### 

spool ON
spool Logs\LOG_Create_test_CheminvDB.txt

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

--' Alter schema user for tablespace usage to support Oracle 12c for fresh installation
@@Alter\Alter_ChemInvUser.sql

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

@@..\..\Create_test_ChemInv_DB\Insert_Test_Data.sql

--' Create packages and procedures
@@PLSQL\Create_PLSQL.sql


-- ' Applying the latest patch
@@"Patches\Parameters.sql"
@"Patches\Patch &&schemaVersion\Parameters.sql"
@"Patches\Patch &&nextPatch\patch.sql"

--11.0.3
@&&setNextPatch 
--11.0.4
@&&setNextPatch 
--12.1.0
@&&setNextPatch 
--12.1.1
@&&setNextPatch 
--12.1.3
@&&setNextPatch 
--12.3.0
@&&setNextPatch 
--12.3.1
@&&setNextPatch 
--12.3.2
@&&setNextPatch 
--12.5.0
@&&setNextPatch 
--12.5.1
@&&setNextPatch 
--12.5.2
@&&setNextPatch 
--12.5.3
@&&setNextPatch 
--12.6.0
@&&setNextPatch 
--12.6.1
@&&setNextPatch 
--12.6.2
@&&setNextPatch 
--12.6.3
@&&setNextPatch 
--17.1.0
@&&setNextPatch 
--18.1.0
@&&setNextPatch 
--18.1.1
@&&setNextPatch 
--19.1.0
@&&setNextPatch 


--' Recompile pl/sql 
@@PLSQL\RecompilePLSQL.sql

prompt '#########################################################'
prompt 'Logged session to: Logs\LOG_Create_test_CheminvDB.txt'
prompt '#########################################################'

prompt 
spool off

exit


	