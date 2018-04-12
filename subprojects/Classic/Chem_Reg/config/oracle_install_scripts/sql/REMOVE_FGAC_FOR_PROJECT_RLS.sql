--Copyright 1999-2003 CambridgeSoft Corporation. All rights reserved

spool on
spool log_disable_rls.txt

--Run the Script to Remove a Project Row Level Locking predicate function and apply the function as policies to particular tables.
--see the document PROJECT_RLS_INSTRUCTIONS.txt in this directory for complete information about the effects of this feature.
SET verify off
ACCEPT serverName Char DEFAULT '' PROMPT 'Enter the Oracle service name:'
ACCEPT sysPass Char DEFAULT 'manager2' PROMPT 'Enter the system account password (manager2):' hide
ACCEPT securitySchemaName Char DEFAULT 'cs_security' PROMPT 'Enter the cs_security schema name(cs_security):'
ACCEPT securitySchemaPass Char DEFAULT 'oracle' PROMPT 'Enter cs_security account password (oracle):' hide


Connect &&securitySchemaName/&&securitySchemaPass@&&serverName;

UPDATE chem_reg_privileges SET DELETE_PROJECTS_TABLE='1' Where role_internal_id = (Select role_id from security_roles where upper(role_name) = 'CHEMICAL_ADMINISTRATOR');
UPDATE chem_reg_privileges SET EDIT_PROJECTS_TABLE='1' Where role_internal_id = (Select role_id from security_roles where upper(role_name) = 'CHEMICAL_ADMINISTRATOR');


UPDATE chem_reg_privileges SET DELETE_PROJECTS_TABLE='1' Where role_internal_id = (Select role_id from security_roles where upper(role_name) = 'SUPERVISING_CHEMICAL_ADMIN');
UPDATE chem_reg_privileges SET EDIT_PROJECTS_TABLE='1' Where role_internal_id = (Select role_id from security_roles where upper(role_name) = 'SUPERVISING_CHEMICAL_ADMIN');
UPDATE chem_reg_privileges SET ADD_PROJECTS_TABLE='1' Where role_internal_id = (Select role_id from security_roles where upper(role_name) = 'SUPERVISING_CHEMICAL_ADMIN');

CONNECT System/&&sysPass@&&serverName;

exec dbms_rls.drop_policy('REGDB','COMPOUND_MOLECULE','CMPD_MOL_POLICY');
exec dbms_rls.drop_policy('REGDB','REG_NUMBERS','REG_NUM_POLICY');
exec dbms_rls.drop_policy('REGDB','STRUCTURES','STRUCTURES_POLICY');
exec dbms_rls.drop_policy('REGDB','BATCHES','BATCHES_POLICY');
exec dbms_rls.drop_policy('REGDB','TEMPORARY_STRUCTURES','TEMP_CMPD_POLICY');
exec dbms_rls.drop_policy('REGDB','COMPOUND_SALT','CMPD_SALT_POLICY');
exec dbms_rls.drop_policy('REGDB','PROJECTS','PROJECTS_POLICY');

Drop Function  REGDB.PeopleProject_RLL_Function;

commit;

quit;

spool off
	
	