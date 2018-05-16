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

-- ' Applying the latest patch
@@"Patches\Parameters.sql"
@@"Patches\Patch &&schemaVersion\Parameters.sql"
@@"Patches\Patch &&nextPatch\patch.sql"

--When 17.1.0 is reached, we end the script execution and continue the remaining patch scripts as a fresh nested script to avoid problem.
--This is to avoid the oracle error SP2-0309: SQL*Plus command procedures may only be nested to a depth of 20.
--This is caused due to the nested script execution has reached the depth limit of 20 starting from 11.0.1
	SELECT	CASE
		WHEN  '&&currentPatch' = '17.1.0'
		THEN  '"sql\Patches\Patch &&nextPatch\patch.sql"'
		ELSE  'sql\Patches\stop.sql'
	END	AS setNextPatch 
	FROM	DUAL;

	@&&setNextPatch 

--' Recompile pl/sql 
@@PLSQL\RecompilePLSQL.sql

prompt '#########################################################'
prompt 'Logged session to: Logs\LOG_Create_blank_CheminvDB.txt'
prompt '#########################################################'

prompt 
spool off

exit


	