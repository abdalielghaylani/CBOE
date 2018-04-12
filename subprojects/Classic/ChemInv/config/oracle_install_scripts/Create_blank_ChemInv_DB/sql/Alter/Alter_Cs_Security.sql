-- Copyright Cambridgesoft Corp 2001-2006 all rights reserved

--#########################################################
-- Alter cs_security schema for Inventory
-- Creates and grants to default ChemInv Roles and populates CS_SECURITY tables
--######################################################### 

prompt '#########################################################'
prompt 'Altering the cs_security schema...'
prompt '#########################################################'

--' Grant all cheminvdb2 object permissions to CS_SECURITY   
Connect &&schemaName/&&schemaPass@&&serverName
@@Alter\Cs_Security\Grant_ChemInvDB2Privs.sql

--' Create cs_security objects and data for Inventory 
connect &&securitySchemaName/&&securitySchemaPass@&&serverName;
@@Alter\Cs_Security\Create_PrivilegeTable.sql
@@Alter\Cs_Security\Insert_TableData.sql

--#########################################################
--' remove this entry once it is added to the cs_security schema in 10
--#########################################################
@@Alter\Cs_Security\Create_Procedure.sql

--' Create default roles for Inventory 
CONNECT &&InstallUser/&&sysPass@&&serverName
@@Alter\Cs_Security\Create_DefaultRoles.sql
