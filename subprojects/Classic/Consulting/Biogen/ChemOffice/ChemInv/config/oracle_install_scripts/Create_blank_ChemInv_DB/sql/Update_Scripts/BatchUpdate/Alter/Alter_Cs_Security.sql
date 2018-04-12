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

GRANT EXECUTE ON "&&SchemaName".UPDATEREQNUM to CS_SECURITY WITH GRANT OPTION;

--' Create cs_security objects and data for Inventory 
connect &&securitySchemaName/&&securitySchemaPass@&&serverName;

--#########################################################
-- Object_Privileges
--######################################################### 
--' insert inventory entries

INSERT INTO CS_SECURITY.OBJECT_PRIVILEGES VALUES ('INV_EDIT_CONTAINER', 'EXECUTE', '&&SchemaName', 'UPDATEREQNUM');

commit;


--' grant roles based on privs
begin
	FOR role_rec IN (SELECT role_name FROM security_roles WHERE privilege_table_int_id = (SELECT privilege_table_id FROM privilege_tables WHERE privilege_table_name = '&&privTableName'))
	LOOP
		GrantPrivsToRole(role_rec.role_name);
	END LOOP;
end;
/
