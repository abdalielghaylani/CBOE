-- Copyright Cambridgesoft Corp 2001-2007 all rights reserved

prompt '#########################################################'
prompt 'Granting privileges to cs_security...'
prompt '#########################################################'

--' Grant all cheminvdb2 object permissions to CS_SECURITY   
Connect &&schemaName/&&schemaPass@&&serverName

-- New view
GRANT SELECT,INSERT,UPDATE,DELETE ON "&&SchemaName".INV_VW_COMPOUNDS TO &&securitySchemaName WITH GRANT OPTION;

-- Create cs_security objects and data for Inventory 
connect &&securitySchemaName/&&securitySchemaPass@&&serverName;
INSERT INTO &&securitySchemaName..OBJECT_PRIVILEGES VALUES ('INV_BROWSE_ALL', 'SELECT', '&&SchemaName', 'INV_VW_COMPOUNDS');

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
