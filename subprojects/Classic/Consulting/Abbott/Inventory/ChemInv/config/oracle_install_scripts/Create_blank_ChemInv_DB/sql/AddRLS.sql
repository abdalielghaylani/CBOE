--Copyright 1999-2005 CambridgeSoft Corporation. All rights reserved
set echo off

@@parameters.sql
--@@prompts.sql
-- Copyright Cambridgesoft Corp 2001-2005 all rights reserved

SET ECHO OFF
SET verify off

--#########################################################
-- PROMPT THE USER FOR SCRIPT SUBSTITUTION VARIABLES
--######################################################### 

ACCEPT serverName CHAR DEFAULT '' PROMPT 'Enter the target Oracle service name:'
ACCEPT OraVersionNumber CHAR DEFAULT '9' PROMPT 'Enter the Oracle major version number (9):'
ACCEPT InstallUser CHAR DEFAULT 'sys' PROMPT 'Enter the name of an Oracle account that can login as sysdba(sys):'
ACCEPT sysPass CHAR DEFAULT 'manager2' PROMPT 'Enter the above oracle account password (manager2):'


set feedback on
spool Logs/LOG_AddRLS.txt

Connect &&InstallUser/&&sysPass@&&serverName AS SYSDBA;
--' Privs for implementing RLS in the cheminvdb2 schema
GRANT EXECUTE ON DBMS_RLS TO &&schemaName;
GRANT CREATE ANY CONTEXT TO &&schemaName;
GRANT CREATE ANY TRIGGER TO &&schemaName;
GRANT ADMINISTER DATABASE TRIGGER TO &&schemaName;

--' privs for determining the predicate
GRANT SELECT ANY DICTIONARY TO &&schemaName;
GRANT REFERENCES ON cs_security.security_roles TO &&schemaName;

Connect &&schemaName/&&schemaPass@&&serverName

--' Create tables
CREATE TABLE inv_role_locations (
	role_id_fk NUMBER(8),
  location_id_fk NUMBER(8),
	PRIMARY KEY(role_id_fk, location_id_fk),
  CONSTRAINT "INV_ROLELOC_ROLEID_FK"
		FOREIGN KEY("ROLE_ID_FK")
    REFERENCES CS_SECURITY.SECURITY_ROLES("ROLE_ID")	
    ON DELETE CASCADE,
	CONSTRAINT "INV_USERLOC_LOCATIONID_FK"
		FOREIGN KEY("LOCATION_ID_FK")
	  REFERENCES "INV_LOCATIONS"("LOCATION_ID")
  	ON DELETE CASCADE)
  ORGANIZATION INDEX;

--' Create application context
CREATE CONTEXT ctx_cheminv USING cheminvdb2.ctx_cheminv_mgr;

--plsql
@@PACKAGES\pkg_RLS_def.sql
@@PACKAGES\pkg_RLS_Body.sql
@@PACKAGES\pkg_ManageRLS_def.sql
@@PACKAGES\pkg_ManageRLS_Body.SQL
@@PACKAGES\pkg_CTX_Cheminv_Mgr_def.SQL
@@PACKAGES\pkg_CTX_Cheminv_Mgr_Body.SQL

--triggers
@@TRIGGERS\trg_set_user_roleIDs.sql

--' add policies to table
BEGIN
  DBMS_RLS.ADD_POLICY( 'CHEMINVDB2','INV_LOCATIONS','LOCATION_VPD_P1','CHEMINVDB2','RLS.SETPREDICATE');
  DBMS_RLS.ADD_POLICY( 'CHEMINVDB2','INV_CONTAINERS','LOCATION_VPD_P1','CHEMINVDB2','RLS.SETPREDICATE');
  DBMS_RLS.ADD_POLICY( 'CHEMINVDB2','INV_PLATES','LOCATION_VPD_P1','CHEMINVDB2','RLS.SETPREDICATE');
END; 
/

--inv roles grants
GRANT SELECT ON INV_ROLE_LOCATIONS TO INV_BROWSER;

GRANT INSERT,UPDATE,DELETE ON INV_ROLE_LOCATIONS TO INV_ADMIN;
GRANT EXECUTE ON ManageRLS TO INV_ADMIN;

--cs_security grants
--GRANT SELECT ON INV_ROLE_LOCATIONS TO CS_SECURITY WITH GRANT OPTION;
GRANT EXECUTE ON ManageRLS TO CS_SECURITY WITH GRANT OPTION;

-- update cs_security
connect &&securitySchemaName/&&securitySchemaPass@&&serverName;

--' privs on cs_security needed to set the RLS predicate in cheminvdb2
GRANT SELECT ON security_roles TO cheminvdb2;
GRANT SELECT ON privilege_tables TO cheminvdb2;

INSERT INTO CS_SECURITY.OBJECT_PRIVILEGES VALUES ('CSS_CREATE_ROLE', 'SELECT', '&&SchemaName', 'INV_ROLE_LOCATIONS');
INSERT INTO CS_SECURITY.OBJECT_PRIVILEGES VALUES ('CSS_CREATE_ROLE', 'EXECUTE', '&&SchemaName', 'MANAGERLS');
COMMIT;

-- create public synonyms
Connect &&InstallUser/&&sysPass@&&serverName as sysdba
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


spool off
exit
