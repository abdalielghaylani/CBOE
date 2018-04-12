--Copyright 1999-2006 CambridgeSoft Corporation. All rights reserved
-- COE11  15-Sep-2009  kfd

--#########################################################
-- Update file for activating RLS in Inventory Enterprise
--######################################################### 

-- Intialize variables
@parameters.sql
@prompts.sql

SET verify off
set feedback on
spool Logs/LOG_AddRLS.txt

Connect &&InstallUser/&&sysPass@&&serverName
--' Privs for implementing RLS in the cheminvdb2 schema
-- GRANT EXECUTE ON DBMS_RLS TO &&schemaName;
GRANT CREATE ANY CONTEXT TO &&schemaName;
GRANT CREATE ANY TRIGGER TO &&schemaName;
GRANT ADMINISTER DATABASE TRIGGER TO &&schemaName;

--' privs for determining the predicate
GRANT SELECT ANY DICTIONARY TO &&schemaName;
GRANT REFERENCES ON &&securitySchemaName..security_roles TO &&schemaName;
GRANT SELECT ON &&securitySchemaName..security_roles TO &&schemaName;
GRANT SELECT ON &&securitySchemaName..privilege_tables TO &&schemaName;

Connect &&schemaName/&&schemaPass@&&serverName

--' Create tables
CREATE TABLE inv_role_locations (
	role_id_fk NUMBER(8),
  location_id_fk NUMBER(8),
	PRIMARY KEY(role_id_fk, location_id_fk),
  CONSTRAINT "INV_ROLELOC_ROLEID_FK"
		FOREIGN KEY("ROLE_ID_FK")
    REFERENCES &&securitySchemaName..SECURITY_ROLES("ROLE_ID")	
    ON DELETE CASCADE,
	CONSTRAINT "INV_USERLOC_LOCATIONID_FK"
		FOREIGN KEY("LOCATION_ID_FK")
	  REFERENCES "INV_LOCATIONS"("LOCATION_ID")
  	ON DELETE CASCADE)
  ORGANIZATION INDEX;

--' Create application context
CREATE CONTEXT ctx_cheminv USING &&schemaName..ctx_cheminv_mgr;

--plsql
@@Update_Scripts\RLS\PACKAGES\pkg_RLS_def.sql
@@Update_Scripts\RLS\PACKAGES\pkg_RLS_Body.sql
@@Update_Scripts\RLS\PACKAGES\pkg_ManageRLS_def.sql
@@Update_Scripts\RLS\PACKAGES\pkg_ManageRLS_Body.SQL
@@Update_Scripts\RLS\PACKAGES\pkg_CTX_Cheminv_Mgr_def.SQL
@@Update_Scripts\RLS\PACKAGES\pkg_CTX_Cheminv_Mgr_Body.SQL

--triggers
@@Update_Scripts\RLS\TRIGGERS\trg_set_user_roleIDs.sql

Connect &&InstallUser/&&sysPass@&&serverName

--' add policies to table
BEGIN
  DBMS_RLS.ADD_POLICY( '&&schemaName','INV_LOCATIONS','LOCATION_VPD_P1','&&schemaName','RLS.SETPREDICATE');
  DBMS_RLS.ADD_POLICY( '&&schemaName','INV_CONTAINERS','LOCATION_VPD_P1','&&schemaName','RLS.SETPREDICATE');
  DBMS_RLS.ADD_POLICY( '&&schemaName','INV_PLATES','LOCATION_VPD_P1','&&schemaName','RLS.SETPREDICATE');
END; 
/

Connect &&schemaName/&&schemaPass@&&serverName

--inv roles grants
GRANT SELECT ON INV_ROLE_LOCATIONS TO INV_BROWSER;

GRANT INSERT,UPDATE,DELETE ON INV_ROLE_LOCATIONS TO INV_ADMIN;
GRANT EXECUTE ON ManageRLS TO INV_ADMIN;
GRANT EXECUTE ON ManageRLS TO CSS_ADMIN;


--cs_security grants
--GRANT SELECT ON INV_ROLE_LOCATIONS TO &&securitySchemaName WITH GRANT OPTION;
GRANT EXECUTE ON ManageRLS TO &&securitySchemaName WITH GRANT OPTION;

-- update cs_security
connect &&securitySchemaName/&&securitySchemaPass@&&serverName;

--' privs on cs_security needed to set the RLS predicate in cheminvdb2
GRANT SELECT ON security_roles TO &&schemaName;
GRANT SELECT ON privilege_tables TO &&schemaName;
GRANT SELECT ON security_roles to css_admin;

INSERT INTO &&securitySchemaName..OBJECT_PRIVILEGES VALUES ('CSS_CREATE_ROLE', 'SELECT', '&&SchemaName', 'SECURITY_ROLES');
INSERT INTO &&securitySchemaName..OBJECT_PRIVILEGES VALUES ('CSS_CREATE_ROLE', 'SELECT', '&&SchemaName', 'INV_ROLE_LOCATIONS');
INSERT INTO &&securitySchemaName..OBJECT_PRIVILEGES VALUES ('CSS_CREATE_ROLE', 'EXECUTE', '&&SchemaName', 'MANAGERLS');
COMMIT;

-- create public synonyms
Connect &&InstallUser/&&sysPass@&&serverName
DECLARE
	PROCEDURE createSynonym(synName IN varchar2) IS
			n NUMBER;
		BEGIN
			select count(*) INTO n from dba_synonyms where Upper(synonym_name) = synName;
			if n = 0 then
				execute immediate 'CREATE PUBLIC SYNONYM ' || synName || ' FOR &&schemaName..' || synName;
			end if;
		END createSynonym;
BEGIN
	createSynonym('INV_ROLE_LOCATIONS');
END;
/

Connect &&schemaName/&&schemaPass@&&serverName

--update Globals table
UPDATE globals SET VALUE='1' WHERE ID = 'RLS_ENABLED';
commit;

spool off
exit
