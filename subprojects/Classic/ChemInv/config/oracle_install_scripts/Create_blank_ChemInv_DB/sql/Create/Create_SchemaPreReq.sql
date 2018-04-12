-- Copyright Cambridgesoft Corp 2001-2006 all rights reserved

--#########################################################
-- Create schema pre-requisites
--######################################################### 

Connect &&schemaName/&&schemaPass@&&serverName

prompt '#########################################################'
prompt 'Creating schema pre-requisites...'
prompt '#########################################################'

@@PLSQL\Packages\pkg_Constants_Def.sql;

--' Inv user needs privileges to add foreign key constraints to the people table
connect &&securitySchemaName/&&securitySchemaPass@&&serverName;
GRANT REFERENCES ON people TO &&schemaName;
GRANT REFERENCES ON security_roles TO &&schemaName;