-- Copyright Cambridgesoft Corp 2001-2007 all rights reserved

--#########################################################
-- Alter cs_security schema for Inventory 10
-- Creates and grants to default ChemInv Roles and populates CS_SECURITY tables
--######################################################### 

prompt '#########################################################'
prompt 'Altering the cs_security schema...'
prompt '#########################################################'

--' Grant all cheminvdb2 object permissions to CS_SECURITY   
Connect &&schemaName/&&schemaPass@&&serverName
-- new tables
GRANT SELECT,INSERT,UPDATE,DELETE ON "&&SchemaName".INV_PICKLIST_TYPES TO &&securitySchemaName WITH GRANT OPTION;
GRANT SELECT,INSERT,UPDATE,DELETE ON "&&SchemaName".INV_PICKLISTS TO &&securitySchemaName WITH GRANT OPTION;
GRANT SELECT,INSERT,UPDATE,DELETE ON "&&SchemaName".INV_CUSTOM_FIELD_GROUPS TO &&securitySchemaName WITH GRANT OPTION;
GRANT SELECT,INSERT,UPDATE,DELETE ON "&&SchemaName".INV_CUSTOM_FIELDS TO &&securitySchemaName WITH GRANT OPTION;
GRANT SELECT,INSERT,UPDATE,DELETE ON "&&SchemaName".INV_CUSTOM_CPD_FIELD_VALUES TO &&securitySchemaName WITH GRANT OPTION;
GRANT SELECT,INSERT,UPDATE,DELETE ON "&&SchemaName".INV_LABEL_PRINTERS TO &&securitySchemaName WITH GRANT OPTION;
GRANT SELECT,INSERT,UPDATE,DELETE ON "&&SchemaName".INV_LPR_DEFINITION TO &&securitySchemaName WITH GRANT OPTION;

GRANT SELECT ON "&&SchemaName".INV_CONTAINER_BATCH_FIELDS TO &&securitySchemaName WITH GRANT OPTION;
GRANT SELECT ON "&&SchemaName".INV_CONTAINER_BATCH_FIELDS TO INV_BROWSER;
GRANT EXECUTE ON "&&SchemaName".GETPRIMARYKEYIDS to &&securitySchemaName WITH GRANT OPTION;
GRANT EXECUTE ON "&&SchemaName".CHECKOPENPOSITIONS to &&securitySchemaName WITH GRANT OPTION;
GRANT EXECUTE ON "&&SchemaName".CREATEREGCOMPOUND TO &&securitySchemaName WITH GRANT OPTION;
GRANT EXECUTE ON "&&SchemaName".UPDATEREGCOMPOUND TO &&securitySchemaName WITH GRANT OPTION;

--' Create cs_security objects and data for Inventory 
connect &&securitySchemaName/&&securitySchemaPass@&&serverName;
-- new privilege
ALTER TABLE &&securitySchemaName..CHEMINV_PRIVILEGES ADD (
    "INV_MANAGE_CUSTOM_FIELDS" Number(1)
)
;
commit;

--#########################################################
-- Object_Privileges
--######################################################### 
--' insert inventory entries
--' INV_BROWSE_ALL PRIVS
INSERT INTO &&securitySchemaName..OBJECT_PRIVILEGES VALUES ('INV_BROWSE_ALL', 'SELECT', '&&SchemaName', 'INV_PICKLIST_TYPES');
INSERT INTO &&securitySchemaName..OBJECT_PRIVILEGES VALUES ('INV_BROWSE_ALL', 'SELECT', '&&SchemaName', 'INV_CUSTOM_FIELD_GROUPS');
INSERT INTO &&securitySchemaName..OBJECT_PRIVILEGES VALUES ('INV_BROWSE_ALL', 'SELECT', '&&SchemaName', 'INV_CUSTOM_FIELDS');
INSERT INTO &&securitySchemaName..OBJECT_PRIVILEGES VALUES ('INV_BROWSE_ALL', 'SELECT', '&&SchemaName', 'INV_CUSTOM_CPD_FIELD_VALUES');
INSERT INTO &&securitySchemaName..OBJECT_PRIVILEGES VALUES ('INV_BROWSE_ALL', 'SELECT', '&&SchemaName', 'INV_CONTAINER_BATCH_FIELDS');
-- INV_MANAGE_SUBSTANCES PRIVS
INSERT INTO &&securitySchemaName..OBJECT_PRIVILEGES VALUES ('INV_MANAGE_SUBSTANCES', 'INSERT', '&&SchemaName', 'INV_CUSTOM_CPD_FIELD_VALUES');
INSERT INTO &&securitySchemaName..OBJECT_PRIVILEGES VALUES ('INV_MANAGE_SUBSTANCES', 'UPDATE', '&&SchemaName', 'INV_CUSTOM_CPD_FIELD_VALUES');
INSERT INTO &&securitySchemaName..OBJECT_PRIVILEGES VALUES ('INV_MANAGE_SUBSTANCES', 'DELETE', '&&SchemaName', 'INV_CUSTOM_CPD_FIELD_VALUES');
-- INV_MANAGE_CUSTOM_FIELDS PRIVS
INSERT INTO &&securitySchemaName..OBJECT_PRIVILEGES VALUES ('INV_MANAGE_CUSTOM_FIELDS', 'INSERT', '&&SchemaName', 'INV_PICKLIST_TYPES');
INSERT INTO &&securitySchemaName..OBJECT_PRIVILEGES VALUES ('INV_MANAGE_CUSTOM_FIELDS', 'UPDATE', '&&SchemaName', 'INV_PICKLIST_TYPES');
INSERT INTO &&securitySchemaName..OBJECT_PRIVILEGES VALUES ('INV_MANAGE_CUSTOM_FIELDS', 'DELETE', '&&SchemaName', 'INV_PICKLIST_TYPES');
INSERT INTO &&securitySchemaName..OBJECT_PRIVILEGES VALUES ('INV_MANAGE_CUSTOM_FIELDS', 'INSERT', '&&SchemaName', 'INV_PICKLISTS');
INSERT INTO &&securitySchemaName..OBJECT_PRIVILEGES VALUES ('INV_MANAGE_CUSTOM_FIELDS', 'UPDATE', '&&SchemaName', 'INV_PICKLISTS');
INSERT INTO &&securitySchemaName..OBJECT_PRIVILEGES VALUES ('INV_MANAGE_CUSTOM_FIELDS', 'DELETE', '&&SchemaName', 'INV_PICKLISTS');
INSERT INTO &&securitySchemaName..OBJECT_PRIVILEGES VALUES ('INV_MANAGE_CUSTOM_FIELDS', 'INSERT', '&&SchemaName', 'INV_CUSTOM_FIELD_GROUPS');
INSERT INTO &&securitySchemaName..OBJECT_PRIVILEGES VALUES ('INV_MANAGE_CUSTOM_FIELDS', 'UPDATE', '&&SchemaName', 'INV_CUSTOM_FIELD_GROUPS');
INSERT INTO &&securitySchemaName..OBJECT_PRIVILEGES VALUES ('INV_MANAGE_CUSTOM_FIELDS', 'DELETE', '&&SchemaName', 'INV_CUSTOM_FIELD_GROUPS');
INSERT INTO &&securitySchemaName..OBJECT_PRIVILEGES VALUES ('INV_MANAGE_CUSTOM_FIELDS', 'INSERT', '&&SchemaName', 'INV_CUSTOM_FIELDS');
INSERT INTO &&securitySchemaName..OBJECT_PRIVILEGES VALUES ('INV_MANAGE_CUSTOM_FIELDS', 'UPDATE', '&&SchemaName', 'INV_CUSTOM_FIELDS');
INSERT INTO &&securitySchemaName..OBJECT_PRIVILEGES VALUES ('INV_MANAGE_CUSTOM_FIELDS', 'DELETE', '&&SchemaName', 'INV_CUSTOM_FIELDS');
INSERT INTO &&securitySchemaName..OBJECT_PRIVILEGES VALUES ('INV_MANAGE_CUSTOM_FIELDS', 'INSERT', '&&SchemaName', 'INV_CUSTOM_CPD_FIELD_VALUES');
INSERT INTO &&securitySchemaName..OBJECT_PRIVILEGES VALUES ('INV_MANAGE_CUSTOM_FIELDS', 'UPDATE', '&&SchemaName', 'INV_CUSTOM_CPD_FIELD_VALUES');
INSERT INTO &&securitySchemaName..OBJECT_PRIVILEGES VALUES ('INV_MANAGE_CUSTOM_FIELDS', 'DELETE', '&&SchemaName', 'INV_CUSTOM_CPD_FIELD_VALUES');
INSERT INTO &&securitySchemaName..OBJECT_PRIVILEGES VALUES ('INV_BROWSE_ALL', 'SELECT', '&&SchemaName', 'INV_LABEL_PRINTERS');
INSERT INTO &&securitySchemaName..OBJECT_PRIVILEGES VALUES ('INV_BROWSE_ALL', 'SELECT', '&&SchemaName', 'INV_LPR_DEFINITION');

INSERT INTO &&securitySchemaName..OBJECT_PRIVILEGES VALUES ('INV_CREATE_CONTAINER', 'EXECUTE', '&&SchemaName', 'CREATEREGCOMPOUND');
INSERT INTO &&securitySchemaName..OBJECT_PRIVILEGES VALUES ('INV_CREATE_CONTAINER', 'EXECUTE', '&&SchemaName', 'UPDATEREGCOMPOUND');
INSERT INTO &&securitySchemaName..OBJECT_PRIVILEGES VALUES ('INV_EDIT_CONTAINER', 'EXECUTE', '&&SchemaName', 'UPDATEREGCOMPOUND');
INSERT INTO &&securitySchemaName..OBJECT_PRIVILEGES VALUES ('INV_CREATE_PLATE', 'EXECUTE', '&&SchemaName', 'CREATEREGCOMPOUND');
INSERT INTO &&securitySchemaName..OBJECT_PRIVILEGES VALUES ('INV_CREATE_PLATE', 'EXECUTE', '&&SchemaName', 'UPDATEREGCOMPOUND');
INSERT INTO &&securitySchemaName..OBJECT_PRIVILEGES VALUES ('INV_EDIT_PLATE', 'EXECUTE', '&&SchemaName', 'UPDATEREGCOMPOUND');
INSERT INTO &&securitySchemaName..OBJECT_PRIVILEGES VALUES ('INV_BROWSE_ALL', 'EXECUTE', '&&SchemaName', 'GETPRIMARYKEYIDS');
INSERT INTO &&securitySchemaName..OBJECT_PRIVILEGES VALUES ('INV_BROWSE_ALL', 'EXECUTE', '&&SchemaName', 'CHECKOPENPOSITIONS');

commit;

--' Create default roles for Inventory 
connect &&securitySchemaName/&&securitySchemaPass@&&serverName;
UPDATE &&securitySchemaName..CHEMINV_PRIVILEGES SET INV_MANAGE_CUSTOM_FIELDS=0 WHERE INV_MANAGE_CUSTOM_FIELDS IS NULL;
commit;
UPDATE &&securitySchemaName..CHEMINV_PRIVILEGES SET INV_MANAGE_CUSTOM_FIELDS=1 WHERE ROLE_INTERNAL_ID = (SELECT ROLE_ID FROM &&securitySchemaName..SECURITY_ROLES WHERE ROLE_NAME = 'INV_ADMIN');
commit;

@@Alter\GrantPrivsToRole.sql
commit;

--' grant roles based on privs
begin
	FOR role_rec IN (SELECT role_name FROM security_roles WHERE privilege_table_int_id = (SELECT privilege_table_id FROM privilege_tables WHERE privilege_table_name = '&&privTableName'))
	LOOP
		GrantPrivsToRole(role_rec.role_name);
	END LOOP;
end;
/
