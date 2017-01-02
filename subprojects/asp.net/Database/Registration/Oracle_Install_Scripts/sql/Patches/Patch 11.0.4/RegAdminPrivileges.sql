Connect &&securitySchemaName/&&securitySchemaPass@&&serverName


Alter Table chem_reg_privileges add(
IMPORT_CONFIG Number(1,0) default 0, 
MANAGE_TABLES Number(1,0) default 0, 
MANAGE_PROPERTIES Number(1,0) default 0, 
CUSTOMIZE_FORMS Number(1,0) default 0, 
MANAGE_ADDINS Number(1,0) default 0, 
EDIT_FORM_XML Number(1,0) default 0, 
MANAGE_SYSTEM_SETTINGS Number(1,0) default 0
);



UPDATE chem_reg_privileges SET IMPORT_CONFIG='1' Where role_internal_id = (Select role_id from security_roles where upper(role_name) = 'SUPERVISING_CHEMICAL_ADMIN');
UPDATE chem_reg_privileges SET MANAGE_TABLES='1' Where role_internal_id = (Select role_id from security_roles where upper(role_name) = 'SUPERVISING_CHEMICAL_ADMIN');
UPDATE chem_reg_privileges SET MANAGE_PROPERTIES='1' Where role_internal_id = (Select role_id from security_roles where upper(role_name) = 'SUPERVISING_CHEMICAL_ADMIN');
UPDATE chem_reg_privileges SET CUSTOMIZE_FORMS='1' Where role_internal_id = (Select role_id from security_roles where upper(role_name) = 'SUPERVISING_CHEMICAL_ADMIN');
UPDATE chem_reg_privileges SET MANAGE_ADDINS='1' Where role_internal_id = (Select role_id from security_roles where upper(role_name) = 'SUPERVISING_CHEMICAL_ADMIN');
UPDATE chem_reg_privileges SET EDIT_FORM_XML='1' Where role_internal_id = (Select role_id from security_roles where upper(role_name) = 'SUPERVISING_CHEMICAL_ADMIN');
UPDATE chem_reg_privileges SET MANAGE_SYSTEM_SETTINGS='1' Where role_internal_id = (Select role_id from security_roles where upper(role_name) = 'SUPERVISING_CHEMICAL_ADMIN');







