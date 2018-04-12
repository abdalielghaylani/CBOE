-- Copyright Cambridgesoft Corp 2001-2007 all rights reserved

--#########################################################
-- Alter cs_security schema for Millennium
--######################################################### 

prompt '#########################################################'
prompt 'Altering the cs_security schema...'
prompt '#########################################################'

--' Grant all cheminvdb2 object permissions to CS_SECURITY   
Connect &&schemaName/&&schemaPass@&&serverName
-- new view
GRANT SELECT,INSERT,UPDATE,DELETE ON "&&SchemaName".VIEW_INV_CUSTOM_FIELDS TO CS_SECURITY WITH GRANT OPTION;

--' Create cs_security objects and data for Inventory 
connect &&securitySchemaName/&&securitySchemaPass@&&serverName;

--#########################################################
-- Object_Privileges
--######################################################### 
--' insert inventory entries
--' INV_BROWSE_ALL PRIVS
INSERT INTO CS_SECURITY.OBJECT_PRIVILEGES VALUES ('INV_BROWSE_ALL', 'SELECT', '&&SchemaName', 'VIEW_INV_CUSTOM_FIELDS');
-- INV_MANAGE_CUSTOM_FIELDS PRIVS
INSERT INTO CS_SECURITY.OBJECT_PRIVILEGES VALUES ('INV_MANAGE_CUSTOM_FIELDS', 'INSERT', '&&SchemaName', 'VIEW_INV_CUSTOM_FIELDS');
INSERT INTO CS_SECURITY.OBJECT_PRIVILEGES VALUES ('INV_MANAGE_CUSTOM_FIELDS', 'UPDATE', '&&SchemaName', 'VIEW_INV_CUSTOM_FIELDS');
INSERT INTO CS_SECURITY.OBJECT_PRIVILEGES VALUES ('INV_MANAGE_CUSTOM_FIELDS', 'DELETE', '&&SchemaName', 'VIEW_INV_CUSTOM_FIELDS');
commit;

--' Create default roles for Inventory 
connect &&securitySchemaName/&&securitySchemaPass@&&serverName;
UPDATE CS_SECURITY.CHEMINV_PRIVILEGES SET INV_MANAGE_CUSTOM_FIELDS=0 WHERE INV_MANAGE_CUSTOM_FIELDS IS NULL;
commit;
UPDATE CS_SECURITY.CHEMINV_PRIVILEGES SET INV_MANAGE_CUSTOM_FIELDS=1 WHERE ROLE_INTERNAL_ID = (SELECT ROLE_ID FROM CS_SECURITY.SECURITY_ROLES WHERE ROLE_NAME = 'INV_ADMIN');
commit;

@@..\..\Alter\Cs_Security\GrantPrivsToRole.sql
commit;

--' grant roles based on privs
begin
	FOR role_rec IN (SELECT role_name FROM security_roles WHERE privilege_table_int_id = (SELECT privilege_table_id FROM privilege_tables WHERE privilege_table_name = '&&privTableName'))
	LOOP
		GrantPrivsToRole(role_rec.role_name);
	END LOOP;
end;
/
