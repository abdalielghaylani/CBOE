-- Copyright Cambridgesoft Corp 2001-2006 all rights reserved

--#########################################################
-- Delete cs_security Inventory specific data 
--######################################################### 

connect &&securitySchemaName/&&securitySchemaPass@&&serverName;

prompt '#########################################################'
prompt 'Deleting cs_security Inventory data...'
prompt '#########################################################'

--' delete existing entry
DELETE FROM security_roles WHERE privilege_table_int_id IN (SELECT privilege_table_id FROM privilege_tables WHERE table_space = 'T_CHEMINV2');
--' delete existing entry
delete from privilege_tables where TABLE_SPACE = 'T_CHEMINV2';
commit;

--' delete existing entry
DELETE FROM CS_SECURITY.OBJECT_PRIVILEGES WHERE Schema = '&&SchemaName';
commit;

--' Delete the existing data from security_roles
delete from security_roles where (PRIVILEGE_TABLE_INT_ID) IN (SELECT PRIVILEGE_TABLE_ID FROM PRIVILEGE_TABLES WHERE PRIVILEGE_TABLE_NAME = 'CHEMINV_PRIVILEGES');
commit;

--' Delete default users from the people table
Delete from people where user_id = 'INVBROWSER';
Delete from people where user_id = 'INVCHEMIST';
Delete from people where user_id = 'INVRECEIVING';
Delete from people where user_id = 'INVFINANCE';
Delete from people where user_id = 'INVREGISTRAR';
Delete from people where user_id = 'INVADMIN';
Delete from people where user_id = 'UNKNOWN';
commit;
