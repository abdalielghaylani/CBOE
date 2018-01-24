--Copyright 1999-2010 CambridgeSoft Corporation. All rights reserved

prompt 
prompt Starting "Alter_CoeDB_for_ChemReg_ora.sql"...
prompt 

--#########################################################
--CREATE TABLES
--#########################################################

DECLARE
	n NUMBER;
BEGIN
	select count(*) into n from user_tables where table_name = Upper('&&privTableName');
	if n = 1 then
		execute immediate '
		DROP TABLE &&privTableName CASCADE CONSTRAINTS';
	end if;
END;
/

CREATE TABLE  &&privTableName (
	ROLE_INTERNAL_ID NUMBER(8,0) not null,
	SEARCH_TEMP NUMBER(1,0) null,
	SEARCH_REG NUMBER(1,0) null,
	ADD_COMPOUND_TEMP NUMBER(1,0) null,
	EDIT_COMPOUND_TEMP NUMBER(1,0) null,
	DELETE_TEMP NUMBER(1,0) null,
	REGISTER_TEMP NUMBER(1,0) null,
	EDIT_COMPOUND_REG NUMBER(1,0) null,
	DELETE_REG NUMBER(1,0) null,
	DELETE_BATCH_REG NUMBER(1,0) null,
	EDIT_SCOPE_SUPERVISOR NUMBER(1,0) null,
	EDIT_SCOPE_ALL NUMBER(1,0) null,
	SET_APPROVED_FLAG NUMBER(1,0) null,
	TOGGLE_APPROVED_FLAG NUMBER(1,0) null,
	EDIT_SALT_TABLE NUMBER(1,0) null,
	ADD_SALT_TABLE NUMBER(1,0) null,
	DELETE_SALT_TABLE NUMBER(1,0) null,
	EDIT_SEQUENCES_TABLE NUMBER(1,0) null,
	ADD_SEQUENCES_TABLE NUMBER(1,0) null,
	DELETE_SEQUENCES_TABLE NUMBER(1,0) null,
	EDIT_NOTEBOOKS_TABLE NUMBER(1,0) null,
	ADD_NOTEBOOKS_TABLE NUMBER(1,0) null,
	DELETE_NOTEBOOKS_TABLE NUMBER(1,0) null,
	EDIT_PROJECTS_TABLE NUMBER(1,0) null,
	ADD_PROJECTS_TABLE NUMBER(1,0) null,
	DELETE_PROJECTS_TABLE NUMBER(1,0) null,
	EDIT_SOLVATES_TABLE NUMBER(1,0) null,
	ADD_SOLVATES_TABLE NUMBER(1,0) null,
	DELETE_SOLVATES_TABLE NUMBER(1,0) null,
	EDIT_COMPOUND_TYPE_TABLE NUMBER(1,0) null,
	ADD_COMPOUND_TYPE_TABLE NUMBER(1,0) null,
	DELETE_COMPOUND_TYPE_TABLE NUMBER(1,0) null,
	EDIT_UTILIZATIONS_TABLE NUMBER(1,0) null,
	ADD_UTILIZATIONS_TABLE NUMBER(1,0) null,
	DELETE_UTILIZATIONS_TABLE NUMBER(1,0) null,
	EDIT_BATCH_PROJECTS_TABLE NUMBER(1,0) null,
	ADD_BATCH_PROJECTS_TABLE NUMBER(1,0) null,
	DELETE_BATCH_PROJECTS_TABLE NUMBER(1,0) null,
	EDIT_SITES_TABLE NUMBER(1,0) null, 
	DELETE_SITES_TABLE NUMBER(1,0) null,
	ADD_SITES_TABLE NUMBER(1,0) null,
	SITE_ACCESS_ALL NUMBER(1,0) null,
	MANAGE_PEOPLE_PROJECT NUMBER(1,0), 
	MANAGE_SYSTEM_DUPLICATES NUMBER(1,0),
	REGISTER_DIRECT NUMBER(1,0),
	CONFIG_REG NUMBER(1,0),
	ADD_COMPONENT NUMBER(1,0),
	ADD_PICKLIST_TABLE NUMBER(1,0),
	EDIT_PICKLIST_TABLE NUMBER(1,0),
	DELETE_PICKLIST_TABLE NUMBER(1,0),
	ADD_IDENTIFIER_TYPE_TABLE NUMBER(1,0),
	EDIT_IDENTIFIER_TYPE_TABLE NUMBER(1,0),
	DELETE_IDENTIFIER_TYPE_TABLE NUMBER(1,0),
	ADD_BATCH_PERM NUMBER(1,0), --11.0.1
	LOAD_SAVE_RECORD NUMBER(1,0), --11.0.1
	constraint PK_CHEMREG_PRIVS 
		primary key (ROLE_INTERNAL_ID)
	) TABLESPACE &&securityTableSpaceName
;

		
--#########################################################
--CREATE ROLES
--#########################################################


DECLARE
    PROCEDURE CreateRole(ARole VARCHAR2) AS
         LExist Number;
    BEGIN
        SELECT Count(1) INTO LExist FROM DBA_ROLES WHERE Role=UPPER(ARole);
        IF LExist=0 THEN
            EXECUTE IMMEDIATE 'CREATE ROLE '||ARole||' NOT IDENTIFIED';
        END IF;
    END;
BEGIN
    CreateRole('BROWSER');
    CreateRole('SUBMITTER');
    CreateRole('SUPERVISING_SCIENTIST');
    CreateRole('CHEMICAL_ADMINISTRATOR');
    CreateRole('SUPERVISING_CHEMICAL_ADMIN');
    CreateRole('PERFUME_CHEMIST');
END;
/

GRANT CONNECT TO SUPERVISING_CHEMICAL_ADMIN;
GRANT CONNECT TO CHEMICAL_ADMINISTRATOR;
GRANT CONNECT TO SUPERVISING_SCIENTIST;
GRANT CONNECT TO BROWSER;
GRANT CONNECT TO SUBMITTER;
GRANT CONNECT TO PERFUME_CHEMIST;


-- System Privileges

GRANT CSS_ADMIN TO SUPERVISING_CHEMICAL_ADMIN;
GRANT CSS_USER TO CHEMICAL_ADMINISTRATOR;
GRANT CSS_USER TO SUPERVISING_SCIENTIST;
GRANT CSS_USER TO SUBMITTER;
GRANT CSS_USER TO BROWSER;
Grant CSS_USER to PERFUME_CHEMIST;

GRANT EXECUTE ON GrantOnCoreTableToAllRoles to &&schemaName;

-- BROWSER 
	GRANT SELECT ON SECURITY_ROLES TO BROWSER;
	GRANT SELECT ON PRIVILEGE_TABLES TO BROWSER;
	GRANT SELECT ON CHEM_REG_PRIVILEGES TO BROWSER;
-- SUBMITTER
	GRANT SELECT ON SECURITY_ROLES TO SUBMITTER;
	GRANT SELECT ON PRIVILEGE_TABLES TO SUBMITTER;
	GRANT SELECT ON CHEM_REG_PRIVILEGES TO SUBMITTER;
-- SUPERVISING_SCIENTIST
	GRANT UPDATE ON PEOPLE TO SUPERVISING_SCIENTIST;
	GRANT SELECT ON SECURITY_ROLES TO SUPERVISING_SCIENTIST;
	GRANT SELECT ON PRIVILEGE_TABLES TO SUPERVISING_SCIENTIST;
	GRANT SELECT ON CHEM_REG_PRIVILEGES TO SUPERVISING_SCIENTIST;
-- CHEMICAL_ADMINISTRATOR
	GRANT SELECT ON SECURITY_ROLES TO CHEMICAL_ADMINISTRATOR;
	GRANT SELECT ON PRIVILEGE_TABLES TO CHEMICAL_ADMINISTRATOR;
	GRANT SELECT ON CHEM_REG_PRIVILEGES TO CHEMICAL_ADMINISTRATOR;
	GRANT INSERT,UPDATE  ON PEOPLE TO CHEMICAL_ADMINISTRATOR;
	GRANT INSERT, UPDATE ON SITES TO CHEMICAL_ADMINISTRATOR;
-- SUPERVISING_CHEMICAL_ADMIN
	GRANT INSERT,UPDATE,DELETE ON SITES TO SUPERVISING_CHEMICAL_ADMIN;
	GRANT INSERT,UPDATE,DELETE ON PEOPLE TO SUPERVISING_CHEMICAL_ADMIN;
	GRANT SELECT,INSERT,UPDATE,DELETE ON SECURITY_ROLES TO SUPERVISING_CHEMICAL_ADMIN;
	GRANT SELECT,INSERT,UPDATE,DELETE ON CHEM_REG_PRIVILEGES TO SUPERVISING_CHEMICAL_ADMIN;
	GRANT SELECT ON PRIVILEGE_TABLES TO SUPERVISING_CHEMICAL_ADMIN;
-- PERFUME_CHEMIST
	GRANT SELECT ON SECURITY_ROLES TO PERFUME_CHEMIST;
	GRANT SELECT ON PRIVILEGE_TABLES TO PERFUME_CHEMIST;
	GRANT SELECT ON CHEM_REG_PRIVILEGES TO PERFUME_CHEMIST;


--#########################################################
--DATA INSERTS
--#########################################################


delete from security_roles where Privilege_Table_Int_ID IN(select privilege_table_id from privilege_tables where TABLE_SPACE = 'T_&&schemaName');
delete from privilege_tables where TABLE_SPACE = 'T_&&schemaName';
commit;

delete from security_roles where ROLE_NAME Like 'BROWSER';
delete from security_roles where ROLE_NAME Like 'SUBMITTER';
delete from security_roles where ROLE_NAME Like 'SUPERVISING_SCIENTIST';
delete from security_roles where ROLE_NAME Like 'CHEMICAL_ADMINISTRATOR';
delete from security_roles where ROLE_NAME Like 'SUPERVISING_CHEMICAL_ADMIN';
delete from security_roles where ROLE_NAME Like 'PERFUME_CHEMIST';
commit;

INSERT INTO PRIVILEGE_TABLES (PRIVILEGE_TABLE_ID,APP_NAME, APP_URL,PRIVILEGE_TABLE_NAME,TABLE_SPACE) values(PRIVILEGE_TABLES_seq.NextVal,'Chemical Registration', '', 'CHEM_REG_PRIVILEGES', 'T_&&schemaName');

--Browser
INSERT INTO SECURITY_ROLES (ROLE_ID,PRIVILEGE_TABLE_INT_ID,ROLE_NAME, COEIDENTIFIER)values(SECURITY_ROLES_SEQ.NEXTVAL,PRIVILEGE_TABLES_seq.CURRVAL,'BROWSER', 'REGISTRATION');
INSERT INTO CHEM_REG_PRIVILEGES(ROLE_INTERNAL_ID,SEARCH_TEMP,SEARCH_REG,ADD_COMPOUND_TEMP,EDIT_COMPOUND_TEMP,DELETE_TEMP,REGISTER_TEMP,EDIT_COMPOUND_REG,DELETE_REG,DELETE_BATCH_REG,EDIT_SCOPE_SUPERVISOR,EDIT_SCOPE_ALL,SET_APPROVED_FLAG,TOGGLE_APPROVED_FLAG,EDIT_SALT_TABLE,ADD_SALT_TABLE,DELETE_SALT_TABLE,EDIT_SEQUENCES_TABLE,ADD_SEQUENCES_TABLE,DELETE_SEQUENCES_TABLE,EDIT_NOTEBOOKS_TABLE,ADD_NOTEBOOKS_TABLE,DELETE_NOTEBOOKS_TABLE,EDIT_PROJECTS_TABLE,ADD_PROJECTS_TABLE,DELETE_PROJECTS_TABLE,EDIT_SOLVATES_TABLE,ADD_SOLVATES_TABLE,DELETE_SOLVATES_TABLE,EDIT_COMPOUND_TYPE_TABLE,ADD_COMPOUND_TYPE_TABLE,DELETE_COMPOUND_TYPE_TABLE,EDIT_UTILIZATIONS_TABLE,ADD_UTILIZATIONS_TABLE,DELETE_UTILIZATIONS_TABLE,EDIT_BATCH_PROJECTS_TABLE,ADD_BATCH_PROJECTS_TABLE,DELETE_BATCH_PROJECTS_TABLE,EDIT_SITES_TABLE,DELETE_SITES_TABLE,ADD_SITES_TABLE,SITE_ACCESS_ALL,REGISTER_DIRECT,CONFIG_REG,ADD_COMPONENT,ADD_BATCH_PERM,LOAD_SAVE_RECORD)values(SECURITY_ROLES_SEQ.CURRVAL,'0','1','0','0','0','0','0','0','0','0','0','0','0','0','0','0','0','0','0','0','0','0','0','0','0','0','0','0','0','0','0','0','0','0','0','0','0','0','0','0','0','0','0','0','0','1');

--Submitter
INSERT INTO SECURITY_ROLES (ROLE_ID,PRIVILEGE_TABLE_INT_ID,ROLE_NAME,COEIDENTIFIER)values(SECURITY_ROLES_SEQ.NEXTVAL,PRIVILEGE_TABLES_seq.CURRVAL,'SUBMITTER', 'REGISTRATION');
INSERT INTO CHEM_REG_PRIVILEGES(ROLE_INTERNAL_ID,SEARCH_TEMP,SEARCH_REG,ADD_COMPOUND_TEMP,EDIT_COMPOUND_TEMP,DELETE_TEMP,REGISTER_TEMP,EDIT_COMPOUND_REG,DELETE_REG,DELETE_BATCH_REG,EDIT_SCOPE_SUPERVISOR,EDIT_SCOPE_ALL,SET_APPROVED_FLAG,TOGGLE_APPROVED_FLAG,EDIT_SALT_TABLE,ADD_SALT_TABLE,DELETE_SALT_TABLE,EDIT_SEQUENCES_TABLE,ADD_SEQUENCES_TABLE,DELETE_SEQUENCES_TABLE,EDIT_NOTEBOOKS_TABLE,ADD_NOTEBOOKS_TABLE,DELETE_NOTEBOOKS_TABLE,EDIT_PROJECTS_TABLE,ADD_PROJECTS_TABLE,DELETE_PROJECTS_TABLE,EDIT_SOLVATES_TABLE,ADD_SOLVATES_TABLE,DELETE_SOLVATES_TABLE,EDIT_COMPOUND_TYPE_TABLE,ADD_COMPOUND_TYPE_TABLE,DELETE_COMPOUND_TYPE_TABLE,EDIT_UTILIZATIONS_TABLE,ADD_UTILIZATIONS_TABLE,DELETE_UTILIZATIONS_TABLE,EDIT_BATCH_PROJECTS_TABLE,ADD_BATCH_PROJECTS_TABLE,DELETE_BATCH_PROJECTS_TABLE,EDIT_SITES_TABLE,DELETE_SITES_TABLE,ADD_SITES_TABLE,SITE_ACCESS_ALL,REGISTER_DIRECT,CONFIG_REG,ADD_COMPONENT,ADD_BATCH_PERM,LOAD_SAVE_RECORD)values(SECURITY_ROLES_SEQ.CURRVAL,'1','1','1','1','0','1','1','0','0','0','0','0','0','0','0','0','0','0','0','0','0','0','0','0','0','0','0','0','0','0','0','0','0','0','0','0','0','0','0','0','0','0','0','0','0','1');

--Supervising Scientist
INSERT INTO SECURITY_ROLES (ROLE_ID,PRIVILEGE_TABLE_INT_ID,ROLE_NAME,COEIDENTIFIER)values(SECURITY_ROLES_SEQ.NEXTVAL,PRIVILEGE_TABLES_seq.CURRVAL,'SUPERVISING_SCIENTIST','REGISTRATION');
INSERT INTO CHEM_REG_PRIVILEGES(ROLE_INTERNAL_ID,SEARCH_TEMP,SEARCH_REG,ADD_COMPOUND_TEMP,EDIT_COMPOUND_TEMP,DELETE_TEMP,REGISTER_TEMP,EDIT_COMPOUND_REG,DELETE_REG,DELETE_BATCH_REG,EDIT_SCOPE_SUPERVISOR,EDIT_SCOPE_ALL,SET_APPROVED_FLAG,TOGGLE_APPROVED_FLAG,EDIT_SALT_TABLE,ADD_SALT_TABLE,DELETE_SALT_TABLE,EDIT_SEQUENCES_TABLE,ADD_SEQUENCES_TABLE,DELETE_SEQUENCES_TABLE,EDIT_NOTEBOOKS_TABLE,ADD_NOTEBOOKS_TABLE,DELETE_NOTEBOOKS_TABLE,EDIT_PROJECTS_TABLE,ADD_PROJECTS_TABLE,DELETE_PROJECTS_TABLE,EDIT_SOLVATES_TABLE,ADD_SOLVATES_TABLE,DELETE_SOLVATES_TABLE,EDIT_COMPOUND_TYPE_TABLE,ADD_COMPOUND_TYPE_TABLE,DELETE_COMPOUND_TYPE_TABLE,EDIT_UTILIZATIONS_TABLE,ADD_UTILIZATIONS_TABLE,DELETE_UTILIZATIONS_TABLE,EDIT_BATCH_PROJECTS_TABLE,ADD_BATCH_PROJECTS_TABLE,DELETE_BATCH_PROJECTS_TABLE,EDIT_SITES_TABLE,DELETE_SITES_TABLE,ADD_SITES_TABLE,SITE_ACCESS_ALL,REGISTER_DIRECT,CONFIG_REG,ADD_COMPONENT,ADD_BATCH_PERM,LOAD_SAVE_RECORD)values(SECURITY_ROLES_SEQ.CURRVAL,'1','1','1','1','1','1','1','0','0','1','0','1','0','0','0','0','0','0','0','0','0','0','0','0','0','0','0','0','0','0','0','0','0','0','0','0','0','0','0','0','0','1','0','0','1','1');

--Chemical Admin
INSERT INTO SECURITY_ROLES (ROLE_ID,PRIVILEGE_TABLE_INT_ID,ROLE_NAME, COEIDENTIFIER)values(SECURITY_ROLES_SEQ.NEXTVAL,PRIVILEGE_TABLES_seq.CURRVAL,'CHEMICAL_ADMINISTRATOR', 'REGISTRATION');
INSERT INTO CHEM_REG_PRIVILEGES(ROLE_INTERNAL_ID,SEARCH_TEMP,SEARCH_REG,ADD_COMPOUND_TEMP,EDIT_COMPOUND_TEMP,DELETE_TEMP,REGISTER_TEMP,EDIT_COMPOUND_REG,DELETE_REG,DELETE_BATCH_REG,EDIT_SCOPE_SUPERVISOR,EDIT_SCOPE_ALL,SET_APPROVED_FLAG,TOGGLE_APPROVED_FLAG,EDIT_SALT_TABLE,ADD_SALT_TABLE,DELETE_SALT_TABLE,EDIT_SEQUENCES_TABLE,ADD_SEQUENCES_TABLE,DELETE_SEQUENCES_TABLE,EDIT_NOTEBOOKS_TABLE,ADD_NOTEBOOKS_TABLE,DELETE_NOTEBOOKS_TABLE,EDIT_PROJECTS_TABLE,ADD_PROJECTS_TABLE,DELETE_PROJECTS_TABLE,EDIT_SOLVATES_TABLE,ADD_SOLVATES_TABLE,DELETE_SOLVATES_TABLE,EDIT_COMPOUND_TYPE_TABLE,ADD_COMPOUND_TYPE_TABLE,DELETE_COMPOUND_TYPE_TABLE,EDIT_UTILIZATIONS_TABLE,ADD_UTILIZATIONS_TABLE,DELETE_UTILIZATIONS_TABLE,EDIT_BATCH_PROJECTS_TABLE,ADD_BATCH_PROJECTS_TABLE,DELETE_BATCH_PROJECTS_TABLE,EDIT_SITES_TABLE,DELETE_SITES_TABLE,ADD_SITES_TABLE,SITE_ACCESS_ALL,REGISTER_DIRECT,CONFIG_REG,ADD_COMPONENT,ADD_PICKLIST_TABLE,EDIT_PICKLIST_TABLE,DELETE_PICKLIST_TABLE,ADD_IDENTIFIER_TYPE_TABLE,EDIT_IDENTIFIER_TYPE_TABLE,DELETE_IDENTIFIER_TYPE_TABLE,ADD_BATCH_PERM,LOAD_SAVE_RECORD)values(SECURITY_ROLES_SEQ.CURRVAL,'1','1','1','1','1','1','1','0','0','1','1','1','1','1','1','0','1','0','0','1','1','0','1','1','0','1','1','0','1','1','0','1','1','0','1','1','0','1','0','1','0','1','0','1','1','1','1','1','1','1','1','1');

--Supervising Chemical Admin
INSERT INTO SECURITY_ROLES (ROLE_ID,PRIVILEGE_TABLE_INT_ID,ROLE_NAME, COEIDENTIFIER)values(SECURITY_ROLES_SEQ.NEXTVAL,PRIVILEGE_TABLES_seq.CURRVAL,'SUPERVISING_CHEMICAL_ADMIN', 'REGISTRATION');
INSERT INTO CHEM_REG_PRIVILEGES(ROLE_INTERNAL_ID,SEARCH_TEMP,SEARCH_REG,ADD_COMPOUND_TEMP,EDIT_COMPOUND_TEMP,DELETE_TEMP,REGISTER_TEMP,EDIT_COMPOUND_REG,DELETE_REG,DELETE_BATCH_REG,EDIT_SCOPE_SUPERVISOR,EDIT_SCOPE_ALL,SET_APPROVED_FLAG,TOGGLE_APPROVED_FLAG,EDIT_SALT_TABLE,ADD_SALT_TABLE,DELETE_SALT_TABLE,EDIT_SEQUENCES_TABLE,ADD_SEQUENCES_TABLE,DELETE_SEQUENCES_TABLE,EDIT_NOTEBOOKS_TABLE,ADD_NOTEBOOKS_TABLE,DELETE_NOTEBOOKS_TABLE,EDIT_PROJECTS_TABLE,ADD_PROJECTS_TABLE,DELETE_PROJECTS_TABLE,EDIT_SOLVATES_TABLE,ADD_SOLVATES_TABLE,DELETE_SOLVATES_TABLE,EDIT_COMPOUND_TYPE_TABLE,ADD_COMPOUND_TYPE_TABLE,DELETE_COMPOUND_TYPE_TABLE,EDIT_UTILIZATIONS_TABLE,ADD_UTILIZATIONS_TABLE,DELETE_UTILIZATIONS_TABLE,EDIT_BATCH_PROJECTS_TABLE,ADD_BATCH_PROJECTS_TABLE,DELETE_BATCH_PROJECTS_TABLE,EDIT_SITES_TABLE,DELETE_SITES_TABLE,ADD_SITES_TABLE,SITE_ACCESS_ALL,REGISTER_DIRECT,CONFIG_REG,ADD_COMPONENT,ADD_PICKLIST_TABLE,EDIT_PICKLIST_TABLE,DELETE_PICKLIST_TABLE,ADD_IDENTIFIER_TYPE_TABLE,EDIT_IDENTIFIER_TYPE_TABLE,DELETE_IDENTIFIER_TYPE_TABLE,ADD_BATCH_PERM,LOAD_SAVE_RECORD)values(SECURITY_ROLES_SEQ.CURRVAL,'1','1','1','1','1','1','1','1','1','1','1','1','1','1','1','1','1','1','1','1','1','1','1','1','1','1','1','1','1','1','1','1','1','1','1','1','1','1','1','1','1','1','1','1','1','1','1','1','1','1','1','1');

--Perfume Chemist
INSERT INTO SECURITY_ROLES (ROLE_ID,PRIVILEGE_TABLE_INT_ID,ROLE_NAME, COEIDENTIFIER)values(SECURITY_ROLES_SEQ.NEXTVAL,PRIVILEGE_TABLES_seq.CURRVAL,'PERFUME_CHEMIST', 'REGISTRATION');
INSERT INTO CHEM_REG_PRIVILEGES(ROLE_INTERNAL_ID,SEARCH_TEMP,SEARCH_REG,ADD_COMPOUND_TEMP,EDIT_COMPOUND_TEMP,DELETE_TEMP,REGISTER_TEMP,EDIT_COMPOUND_REG,DELETE_REG,DELETE_BATCH_REG,EDIT_SCOPE_SUPERVISOR,EDIT_SCOPE_ALL,SET_APPROVED_FLAG,TOGGLE_APPROVED_FLAG,EDIT_SALT_TABLE,ADD_SALT_TABLE,DELETE_SALT_TABLE,EDIT_SEQUENCES_TABLE,ADD_SEQUENCES_TABLE,DELETE_SEQUENCES_TABLE,EDIT_NOTEBOOKS_TABLE,ADD_NOTEBOOKS_TABLE,DELETE_NOTEBOOKS_TABLE,EDIT_PROJECTS_TABLE,ADD_PROJECTS_TABLE,DELETE_PROJECTS_TABLE,EDIT_SOLVATES_TABLE,ADD_SOLVATES_TABLE,DELETE_SOLVATES_TABLE,EDIT_COMPOUND_TYPE_TABLE,ADD_COMPOUND_TYPE_TABLE,DELETE_COMPOUND_TYPE_TABLE,EDIT_UTILIZATIONS_TABLE,ADD_UTILIZATIONS_TABLE,DELETE_UTILIZATIONS_TABLE,EDIT_BATCH_PROJECTS_TABLE,ADD_BATCH_PROJECTS_TABLE,DELETE_BATCH_PROJECTS_TABLE,EDIT_SITES_TABLE,DELETE_SITES_TABLE,ADD_SITES_TABLE,SITE_ACCESS_ALL,REGISTER_DIRECT,CONFIG_REG,ADD_COMPONENT,ADD_BATCH_PERM,LOAD_SAVE_RECORD)values(SECURITY_ROLES_SEQ.CURRVAL,'0','1','0','0','0','0','0','0','0','0','0','0','0','0','0','0','0','0','0','0','0','0','0','0','0','0','0','0','0','0','0','0','0','0','0','0','0','0','0','1','0','0','0','0','1','0');

Commit;


--update Browser
UPDATE chem_reg_privileges SET MANAGE_PEOPLE_PROJECT='0' Where role_internal_id = (Select role_id from security_roles where upper(role_name) = 'BROWSER');
UPDATE chem_reg_privileges SET MANAGE_SYSTEM_DUPLICATES='0' Where role_internal_id = (Select role_id from security_roles where upper(role_name) = 'BROWSER');

--update submitter
UPDATE chem_reg_privileges SET MANAGE_PEOPLE_PROJECT='0' Where role_internal_id = (Select role_id from security_roles where upper(role_name) = 'SUBMITTER');
UPDATE chem_reg_privileges SET MANAGE_SYSTEM_DUPLICATES='0' Where role_internal_id = (Select role_id from security_roles where upper(role_name) = 'SUBMITTER');

--update supervising scientist
UPDATE chem_reg_privileges SET MANAGE_PEOPLE_PROJECT='0' Where role_internal_id = (Select role_id from security_roles where upper(role_name) = 'SUPERVISING_SCIENTIST');
UPDATE chem_reg_privileges SET MANAGE_SYSTEM_DUPLICATES='0' Where role_internal_id = (Select role_id from security_roles where upper(role_name) = 'SUPERVISING_SCIENTIST');

--update chemical_administrator
UPDATE chem_reg_privileges SET MANAGE_PEOPLE_PROJECT='1' Where role_internal_id = (Select role_id from security_roles where upper(role_name) = 'CHEMICAL_ADMINISTRATOR');
UPDATE chem_reg_privileges SET MANAGE_SYSTEM_DUPLICATES='0' Where role_internal_id = (Select role_id from security_roles where upper(role_name) = 'CHEMICAL_ADMINISTRATOR');

--update supervising_chemical_admin
UPDATE chem_reg_privileges SET MANAGE_PEOPLE_PROJECT='1' Where role_internal_id = (Select role_id from security_roles where upper(role_name) = 'SUPERVISING_CHEMICAL_ADMIN');
UPDATE chem_reg_privileges SET MANAGE_SYSTEM_DUPLICATES='1' Where role_internal_id = (Select role_id from security_roles where upper(role_name) = 'SUPERVISING_CHEMICAL_ADMIN');

--update perfume chemixt
UPDATE chem_reg_privileges SET MANAGE_PEOPLE_PROJECT='0' Where role_internal_id = (Select role_id from security_roles where upper(role_name) = 'PERFUME_CHEMIST');
UPDATE chem_reg_privileges SET MANAGE_SYSTEM_DUPLICATES='0' Where role_internal_id = (Select role_id from security_roles where upper(role_name) = 'PERFUME_CHEMIST');


-- DELETE OBJECT PRIVILEGES
DELETE FROM CS_SECURITY.OBJECT_PRIVILEGES WHERE Schema = Upper('&&schemaName');

--- SEARCH_TEMP PRIVS
INSERT INTO CS_SECURITY.OBJECT_PRIVILEGES VALUES ('SEARCH_TEMP', 'EXECUTE', '&&schemaName', 'COMPOUNDREGISTRY');

--- SEARCH_REG PRIVS
INSERT INTO CS_SECURITY.OBJECT_PRIVILEGES VALUES ('SEARCH_REG', 'EXECUTE', '&&schemaName', 'COMPOUNDREGISTRY');

--- ADD_COMPOUND_TEMP PRIVS
INSERT INTO CS_SECURITY.OBJECT_PRIVILEGES VALUES ('ADD_COMPOUND_TEMP', 'EXECUTE', '&&schemaName', 'COMPOUNDREGISTRY');

--- EDIT_COMPOUND_TEMP PRIVS
INSERT INTO CS_SECURITY.OBJECT_PRIVILEGES VALUES ('EDIT_COMPOUND_TEMP', 'EXECUTE', '&&schemaName', 'COMPOUNDREGISTRY');

--- DELETE_TEMP PRIVS
INSERT INTO CS_SECURITY.OBJECT_PRIVILEGES VALUES ('DELETE_TEMP', 'EXECUTE', '&&schemaName', 'COMPOUNDREGISTRY');

--- REGISTER_TEMP PRIVS
INSERT INTO CS_SECURITY.OBJECT_PRIVILEGES VALUES ('REGISTER_TEMP', 'EXECUTE', '&&schemaName', 'COMPOUNDREGISTRY');

--- EDIT_COMPOUND_REG PRIVS
INSERT INTO CS_SECURITY.OBJECT_PRIVILEGES VALUES ('EDIT_COMPOUND_REG', 'EXECUTE', '&&schemaName', 'COMPOUNDREGISTRY');

--- DELETE_REG PRIVS
INSERT INTO CS_SECURITY.OBJECT_PRIVILEGES VALUES ('DELETE_REG', 'EXECUTE', '&&schemaName', 'COMPOUNDREGISTRY');

--- SET_APPROVED_FLAG PRIVS
INSERT INTO CS_SECURITY.OBJECT_PRIVILEGES VALUES ('SET_APPROVED_FLAG', 'EXECUTE', '&&schemaName', 'COMPOUNDREGISTRY');

--- TOGGLE_APPROVED_FLAG PRIVS
INSERT INTO CS_SECURITY.OBJECT_PRIVILEGES VALUES ('TOGGLE_APPROVED_FLAG', 'EXECUTE', '&&schemaName', 'COMPOUNDREGISTRY');

--- EDIT_SALT_TABLE PRIVS
INSERT INTO CS_SECURITY.OBJECT_PRIVILEGES VALUES ('EDIT_SALT_TABLE', 'EXECUTE', '&&schemaName', 'COMPOUNDREGISTRY');

--- ADD_SALT_TABLE PRIVS
INSERT INTO CS_SECURITY.OBJECT_PRIVILEGES VALUES ('ADD_SALT_TABLE', 'EXECUTE', '&&schemaName', 'COMPOUNDREGISTRY');

--- DELETE_SALT_TABLE PRIVS
INSERT INTO CS_SECURITY.OBJECT_PRIVILEGES VALUES ('DELETE_SALT_TABLE', 'EXECUTE', '&&schemaName', 'COMPOUNDREGISTRY');

--- EDIT_SEQUENCES_TABLE PRIVS
INSERT INTO CS_SECURITY.OBJECT_PRIVILEGES VALUES ('EDIT_SEQUENCES_TABLE', 'EXECUTE', '&&schemaName', 'COMPOUNDREGISTRY');

--- ADD_SEQUENCES_TABLE PRIVS
INSERT INTO CS_SECURITY.OBJECT_PRIVILEGES VALUES ('ADD_SEQUENCES_TABLE', 'EXECUTE', '&&schemaName', 'COMPOUNDREGISTRY');

--- DELETE_SEQUENCES_TABLE PRIVS
INSERT INTO CS_SECURITY.OBJECT_PRIVILEGES VALUES ('DELETE_SEQUENCES_TABLE', 'EXECUTE', '&&schemaName', 'COMPOUNDREGISTRY');

--- EDIT_NOTEBOOKS_TABLE PRIVS
INSERT INTO CS_SECURITY.OBJECT_PRIVILEGES VALUES ('EDIT_NOTEBOOKS_TABLE', 'EXECUTE', '&&schemaName', 'COMPOUNDREGISTRY');

--- ADD_NOTEBOOKS_TABLE PRIVS
INSERT INTO CS_SECURITY.OBJECT_PRIVILEGES VALUES ('ADD_NOTEBOOKS_TABLE', 'EXECUTE', '&&schemaName', 'COMPOUNDREGISTRY');

--- DELETE_NOTEBOOKS_TABLE PRIVS
INSERT INTO CS_SECURITY.OBJECT_PRIVILEGES VALUES ('DELETE_NOTEBOOKS_TABLE', 'EXECUTE', '&&schemaName', 'COMPOUNDREGISTRY');

--- EDIT_PROJECTS_TABLE PRIVS
INSERT INTO CS_SECURITY.OBJECT_PRIVILEGES VALUES ('EDIT_PROJECTS_TABLE', 'EXECUTE', '&&schemaName', 'COMPOUNDREGISTRY');

--- ADD_PROJECTS_TABLE PRIVS
INSERT INTO CS_SECURITY.OBJECT_PRIVILEGES VALUES ('ADD_PROJECTS_TABLE', 'EXECUTE', '&&schemaName', 'COMPOUNDREGISTRY');

--- DELETE_PROJECTS_TABLE PRIVS
INSERT INTO CS_SECURITY.OBJECT_PRIVILEGES VALUES ('DELETE_PROJECTS_TABLE', 'EXECUTE', '&&schemaName', 'COMPOUNDREGISTRY');

--- EDIT_SOLVATES_TABLE PRIVS
INSERT INTO CS_SECURITY.OBJECT_PRIVILEGES VALUES ('EDIT_SOLVATES_TABLE', 'EXECUTE', '&&schemaName', 'COMPOUNDREGISTRY');

--- ADD_SOLVATES_TABLE PRIVS
INSERT INTO CS_SECURITY.OBJECT_PRIVILEGES VALUES ('ADD_SOLVATES_TABLE', 'EXECUTE', '&&schemaName', 'COMPOUNDREGISTRY');

--- DELETE_SOLVATES_TABLE PRIVS
INSERT INTO CS_SECURITY.OBJECT_PRIVILEGES VALUES ('DELETE_SOLVATES_TABLE', 'EXECUTE', '&&schemaName', 'COMPOUNDREGISTRY');

--- EDIT_COMPOUND_TYPE_TABLE PRIVS
INSERT INTO CS_SECURITY.OBJECT_PRIVILEGES VALUES ('EDIT_COMPOUND_TYPE_TABLE', 'EXECUTE', '&&schemaName', 'COMPOUNDREGISTRY');

--- ADD_COMPOUND_TYPE_TABLE PRIVS
INSERT INTO CS_SECURITY.OBJECT_PRIVILEGES VALUES ('ADD_COMPOUND_TYPE_TABLE', 'EXECUTE', '&&schemaName', 'COMPOUNDREGISTRY');

--- DELETE_COMPOUND_TYPE_TABLE PRIVS
INSERT INTO CS_SECURITY.OBJECT_PRIVILEGES VALUES ('DELETE_COMPOUND_TYPE_TABLE', 'EXECUTE', '&&schemaName', 'COMPOUNDREGISTRY');

--- EDIT_UTILIZATIONS_TABLE PRIVS
INSERT INTO CS_SECURITY.OBJECT_PRIVILEGES VALUES ('EDIT_UTILIZATIONS_TABLE', 'EXECUTE', '&&schemaName', 'COMPOUNDREGISTRY');

--- ADD_UTILIZATIONS_TABLE PRIVS
INSERT INTO CS_SECURITY.OBJECT_PRIVILEGES VALUES ('ADD_UTILIZATIONS_TABLE', 'EXECUTE', '&&schemaName', 'COMPOUNDREGISTRY');

--- DELETE_UTILIZATIONS_TABLE PRIVS
INSERT INTO CS_SECURITY.OBJECT_PRIVILEGES VALUES ('DELETE_UTILIZATIONS_TABLE', 'EXECUTE', '&&schemaName', 'COMPOUNDREGISTRY');

--- EDIT_BATCH_PROJECTS_TABLE PRIVS
INSERT INTO CS_SECURITY.OBJECT_PRIVILEGES VALUES ('EDIT_BATCH_PROJECTS_TABLE', 'EXECUTE', '&&schemaName', 'COMPOUNDREGISTRY');

--- ADD_BATCH_PROJECTS_TABLE PRIVS
INSERT INTO CS_SECURITY.OBJECT_PRIVILEGES VALUES ('ADD_BATCH_PROJECTS_TABLE', 'EXECUTE', '&&schemaName', 'COMPOUNDREGISTRY');

--- DELETE_BATCH_PROJECTS_TABLE PRIVS
INSERT INTO CS_SECURITY.OBJECT_PRIVILEGES VALUES ('DELETE_BATCH_PROJECTS_TABLE', 'EXECUTE', '&&schemaName', 'COMPOUNDREGISTRY');

--- SEARCH_EVAL_DATA PRIVS
INSERT INTO CS_SECURITY.OBJECT_PRIVILEGES VALUES ('SEARCH_EVAL_DATA', 'EXECUTE', '&&schemaName', 'COMPOUNDREGISTRY');

--- EDIT_SITES_TABLE PRIVS
INSERT INTO CS_SECURITY.OBJECT_PRIVILEGES VALUES ('EDIT_SITES_TABLE', 'EXECUTE', '&&schemaName', 'COMPOUNDREGISTRY');

--- DELETE_SITES_TABLE PRIVS
INSERT INTO CS_SECURITY.OBJECT_PRIVILEGES VALUES ('DELETE_SITES_TABLE', 'EXECUTE', '&&schemaName', 'COMPOUNDREGISTRY');

--- ADD_SITES_TABLE PRIVS
INSERT INTO CS_SECURITY.OBJECT_PRIVILEGES VALUES ('ADD_SITES_TABLE', 'EXECUTE', '&&schemaName', 'COMPOUNDREGISTRY');

COMMIT;

-- CS_SECURITY object privileges
-- This procedure inserts into objects privilege table w/o complaining about duplicate constraint violations
DECLARE
	PROCEDURE addObjectPriv(appPriv in varchar2, priv in varchar2, schemaName in varchar2, objectName in varchar2) IS
	BEGIN
		execute immediate 'INSERT INTO CS_SECURITY.OBJECT_PRIVILEGES VALUES ('''||appPriv||''','''||priv||''','''||schemaName||''','''||objectName||''')';
		COMMIT;
	EXCEPTION
  		when DUP_VAL_ON_INDEX then
      		rollback;
 	END addObjectPriv;
BEGIN
	-- SEARCH_REG
	addObjectPriv('SEARCH_REG', 'SELECT', 'CS_SECURITY', 'CHEM_REG_PRIVILEGES');
	addObjectPriv('SEARCH_REG', 'SELECT', 'CS_SECURITY', 'PRIVILEGE_TABLES');
	addObjectPriv('SEARCH_REG', 'SELECT', 'CS_SECURITY', 'SECURITY_ROLES');
	-- EDIT_SITES_TABLE
	addObjectPriv('EDIT_SITES_TABLE', 'UPDATE', 'CS_SECURITY', 'SITES');
	-- DELETE_SITES_TABLE
	addObjectPriv('DELETE_SITES_TABLE', 'DELETE', 'CS_SECURITY', 'SITES');
	-- ADD_SITES_TABLE
	addObjectPriv('ADD_SITES_TABLE', 'INSERT', 'CS_SECURITY', 'SITES');
	COMMIT;
END;
/

--######################
-- Page Control Settings required XMLs
--######################

DELETE FROM COEPAGECONTROL WHERE APPLICATION IN ('REGISTRATION');

DECLARE
 L_MasterXml_1 CLOB:= '<?xml version="1.0" encoding="utf-8"?>
<COEPageControlSettings type="Master">
    <Application>REGISTRATION</Application>
    <ID>COEPageControlSettings_Registration_Master</ID>
    <Pages>
        <Page>
            <ID>ASP.FORMS_SUBMITRECORD_CONTENTAREA_SUBMITMIXTURE_ASPX</ID>
            <Description>Page to submit registries (temporary registries until registration)</Description>
            <FriendlyName>Submit New Record </FriendlyName>
            <Controls>
                <Control>
                    <ID>ASP.FORMS_SUBMITRECORD_CONTENTAREA_SUBMITMIXTURE_ASPX</ID>
                    <Description>Page to submit registries (temporary registries until registration)</Description>
                    <FriendlyName>Submit Registries</FriendlyName>
                    <PlaceHolderID/>
                    <TypeOfControl>Page</TypeOfControl>
                </Control>
                <Control>
                    <ID>SearchComponentButton</ID>
                    <Description>Search components to add to the current registry</Description>
                    <FriendlyName>Search Components button</FriendlyName>
                    <PlaceHolderID/>
                    <TypeOfControl>Control</TypeOfControl>
                </Control>
                <Control>
                    <ID>AddComponentButton</ID>
                    <Description>Add components to the current registry</Description>
                    <FriendlyName>Add Components button</FriendlyName>
                    <PlaceHolderID/>
                    <TypeOfControl>Control</TypeOfControl>
                </Control>
                <Control>
                    <ID>DoneAddingButton</ID>
                    <Description>Finish adding components to the current registry</Description>
                    <FriendlyName>Finish Adding Components button</FriendlyName>
                    <PlaceHolderID/>
                    <TypeOfControl>Control</TypeOfControl>
                </Control>
                <Control>
                    <ID>SubmitButton</ID>
                    <Description>Submit the current registry</Description>
                    <FriendlyName>Submit button</FriendlyName>
                    <PlaceHolderID/>
                    <TypeOfControl>Control</TypeOfControl>
                </Control>
                <Control>
                    <ID>RegisterButton</ID>
                    <Description>Permanently Register skipping Temp</Description>
                    <FriendlyName>Register Button</FriendlyName>
                    <PlaceHolderID/>
                    <TypeOfControl>Control</TypeOfControl>
                </Control>
                <Control>
                    <ID>SaveButton</ID>
                    <Description>Save the current registry</Description>
                    <FriendlyName>Save button</FriendlyName>
                    <PlaceHolderID/>
                    <TypeOfControl>Control</TypeOfControl>
                </Control>
                <Control>
                    <ID>1</ID>
                    <Description>Details form Mixture</Description>
                    <FriendlyName>This shows the properties of a registry (View Mode)</FriendlyName>
                    <PlaceHolderID>mixtureInformationHolder</PlaceHolderID>
                    <TypeOfControl>COEForm</TypeOfControl>
                </Control>
                <Control>
                    <ID>FormGeneratorDebuggingInfo</ID>
                    <TypeOfControl>Control</TypeOfControl>
                    <PlaceHolderID>mixtureInformationHolder</PlaceHolderID>
                    <COEFormID>1</COEFormID>
                </Control>
                <Control>
                    <ID>FormGeneratorDebuggingInfo</ID>
                    <TypeOfControl>Control</TypeOfControl>
                    <PlaceHolderID>mixtureInformationHolder</PlaceHolderID>
                    <COEFormID>5</COEFormID>
                </Control>
                <Control>
                    <ID>FormGeneratorDebuggingInfo</ID>
                    <TypeOfControl>Control</TypeOfControl>
                    <PlaceHolderID>mixtureInformationHolder</PlaceHolderID>
                    <COEFormID>1001</COEFormID>
                </Control>
                <Control>
                    <ID>FormGeneratorDebuggingInfo</ID>
                    <TypeOfControl>Control</TypeOfControl>
                    <PlaceHolderID>mixtureInformationHolder</PlaceHolderID>
                    <COEFormID>1002</COEFormID>
                </Control>
                <Control>
                    <ID>FormGroupDebuggingInfo</ID>
                    <TypeOfControl>Control</TypeOfControl>
                    <PlaceHolderID>mixtureInformationHolder</PlaceHolderID>
                    <COEFormID>-1</COEFormID>
                </Control>
                <Control>
                    <ID>5</ID>
                    <Description>Fragments COEForm</Description>
                    <FriendlyName>Fragments COEForm</FriendlyName>
                    <PlaceHolderID>mixtureInformationHolder</PlaceHolderID>
                    <TypeOfControl>COEForm</TypeOfControl>
                </Control>
                <Control>
                    <ID>1001</ID>
                    <Description>Compound Properties COEForm</Description>
                    <FriendlyName>Compound Properties COEForm</FriendlyName>
                    <PlaceHolderID>mixtureInformationHolder</PlaceHolderID>
                    <TypeOfControl>COEForm</TypeOfControl>
                </Control>
                <Control>
                    <ID>1002</ID>
                    <Description>Batch Properties COEForm</Description>
                    <FriendlyName>Batch Properties COEForm</FriendlyName>
                    <PlaceHolderID>mixtureInformationHolder</PlaceHolderID>
                    <TypeOfControl>COEForm</TypeOfControl>
                </Control>
                <Control>
                    <ID>4</ID>
                    <Description>Batch COEForm</Description>
                    <FriendlyName>Batch COEForm</FriendlyName>
                    <PlaceHolderID>mixtureInformationHolder</PlaceHolderID>
                    <TypeOfControl>COEForm</TypeOfControl>
                </Control>
                <Control>
                    <ID>1003</ID>
                    <Description>Batch Component COEForm</Description>
                    <FriendlyName>Batch Component COEForm</FriendlyName>
                    <PlaceHolderID>mixtureInformationHolder</PlaceHolderID>
                    <TypeOfControl>COEForm</TypeOfControl>
                </Control>
                <Control>
                    <ID>VCompound_IdentifiersUltraGrid</ID>
                    <Description>Identifier grid</Description>
                    <FriendlyName>Identifier grid</FriendlyName>
                    <PlaceHolderID>mixtureInformationHolder</PlaceHolderID>
                    <TypeOfControl>COEGenerableControl</TypeOfControl>
                    <COEFormID>1</COEFormID>
                </Control>
                <Control>
                    <ID>AMix_IdentifiersUltraGrid</ID>
                    <Description>Identifiers grid - Add Mode</Description>
                    <FriendlyName>Identifiers grid - Add Mode</FriendlyName>
                    <PlaceHolderID>mixtureInformationHolder</PlaceHolderID>
                    <TypeOfControl>COEGenerableControl</TypeOfControl>
                    <COEFormID>0</COEFormID>
                </Control>
                <Control>
                    <ID>EMix_ProjectsUltraGrid</ID>
                    <Description>Projects grid - Edit Mode</Description>
                    <FriendlyName>Projects grid - Edit Mode</FriendlyName>
                    <PlaceHolderID>mixtureInformationHolder</PlaceHolderID>
                    <TypeOfControl>COEGenerableControl</TypeOfControl>
                    <COEFormID>0</COEFormID>
                </Control>
                <Control>
                    <ID>VCompound_IdentifiersUltraGrid</ID>
                    <Description>Identifiers grid</Description>
                    <FriendlyName>Identifiers grid</FriendlyName>
                    <PlaceHolderID>mixtureInformationHolder</PlaceHolderID>
                    <TypeOfControl>COEGenerableControl</TypeOfControl>
                    <COEFormID>1</COEFormID>
                </Control>
                <Control>
                    <ID>SequenceDropDownList</ID>
                    <Description>Component Sequence</Description>
                    <FriendlyName>Component Sequence</FriendlyName>
                    <PlaceHolderID>mixtureInformationHolder</PlaceHolderID>
                    <TypeOfControl>COEGenerableControl</TypeOfControl>
                    <COEFormID>1</COEFormID>
                </Control>
                <Control>
                    <ID>SequenceDropDownList</ID>
                    <Description>Registry Sequence</Description>
                    <FriendlyName>Registry Sequence</FriendlyName>
                    <PlaceHolderID>mixtureInformationHolder</PlaceHolderID>
                    <TypeOfControl>COEGenerableControl</TypeOfControl>
                    <COEFormID>0</COEFormID>
                </Control>
                <Control>
                    <ID>AMix_ProjectsUltraGrid</ID>
                    <Description>Projects grid - Add</Description>
                    <FriendlyName>Projects grid - Add</FriendlyName>
                    <PlaceHolderID>mixtureInformationHolder</PlaceHolderID>
                    <TypeOfControl>COEGenerableControl</TypeOfControl>
                    <COEFormID>0</COEFormID>
                </Control>
                <Control>
                    <ID>IdentifiersUltraGrid</ID>
                    <Description>Identifiers grid</Description>
                    <FriendlyName>Identifiers grid</FriendlyName>
                    <PlaceHolderID>mixtureInformationHolder</PlaceHolderID>
                    <TypeOfControl>COEGenerableControl</TypeOfControl>
                    <COEFormID>0</COEFormID>
                </Control>
            </Controls>
        </Page>
		<Page>
            <ID>ASP.FORMS_SUBMITRECORD_SUBMITMIXTURE_ASPX</ID>
            <Description>Here goes the page description text</Description>
            <FriendlyName>SUBMITMIXTURE_ASPX</FriendlyName>
            <Controls>
                <Control>
                    <ID>ASP.FORMS_SUBMITRECORD_SUBMITMIXTURE_ASPX</ID>
                    <Description>Control description goes here!</Description>
                    <FriendlyName>Control friendly name</FriendlyName>
                    <PlaceHolderID/>
                    <TypeOfControl>Page</TypeOfControl>
                </Control>
            </Controls>
        </Page>
			<Page>
            <ID>ASP.FORMS_SUBMITRECORD_SEARCHCOMPOUND_ASPX</ID>
            <Description>Here goes the page description text</Description>
            <FriendlyName>Submit Search Component</FriendlyName>
            <Controls>
                <Control>
                    <ID>ASP.FORMS_SUBMITRECORD_SEARCHCOMPOUND_ASPX</ID>
                    <Description>Control description goes here!</Description>
                    <FriendlyName>Control friendly name</FriendlyName>
                    <PlaceHolderID/>
                    <TypeOfControl>Page</TypeOfControl>
                </Control>
            </Controls>
        </Page>
		<Page>
            <ID>ASP.FORMS_SUBMITRECORD_LOADMIXTUREFORM_ASPX</ID>
            <Description>Here goes the page description text</Description>
            <FriendlyName>Load Template</FriendlyName>
            <Controls>
                <Control>
                    <ID>ASP.FORMS_SUBMITRECORD_LOADMIXTUREFORM_ASPX</ID>
                    <Description>Control description goes here!</Description>
                    <FriendlyName>Control friendly name</FriendlyName>
                    <PlaceHolderID/>
                    <TypeOfControl>Page</TypeOfControl>
                </Control>
            </Controls>
        </Page>
        <Page>
            <ID>ASP.FORMS_SUBMITRECORD_SAVEMIXTUREFORM_ASPX</ID>
            <Description>Here goes the page description text</Description>
            <FriendlyName>Save Template</FriendlyName>
            <Controls>
                <Control>
                    <ID>ASP.FORMS_SUBMITRECORD_SAVEMIXTUREFORM_ASPX</ID>
                    <Description>Control description goes here!</Description>
                    <FriendlyName>Control friendly name</FriendlyName>
                    <PlaceHolderID/>
                    <TypeOfControl>Page</TypeOfControl>
                </Control>
            </Controls>
        </Page>
       
        <Page>
            <ID>ASP.FORMS_REVIEWREGISTER_CONTENTAREA_REVIEWREGISTERMIXTURE_ASPX</ID>
            <Description>Page to submit registries (temporary registries until registration)</Description>
            <FriendlyName>Review Register</FriendlyName>
            <Controls>
                <Control>
                    <ID>ASP.FORMS_REVIEWREGISTER_CONTENTAREA_REVIEWREGISTERMIXTURE_ASPX</ID>
                    <Description>Page to submit registries (temporary registries until registration)</Description>
                    <FriendlyName>Submit Registries</FriendlyName>
                    <PlaceHolderID/>
                    <TypeOfControl>Page</TypeOfControl>
                </Control>
                <Control>
                    <ID>SearchComponentButton</ID>
                    <Description>Search components to add to the current registry</Description>
                    <FriendlyName>Search Components button</FriendlyName>
                    <PlaceHolderID/>
                    <TypeOfControl>Control</TypeOfControl>
                </Control>
                <Control>
                    <ID>EditButton</ID>
                    <Description>Edit record button allows you to go into Edit mode</Description>
                    <FriendlyName>Edit Record Button</FriendlyName>
                    <PlaceHolderID/>
                    <TypeOfControl>Control</TypeOfControl>
                </Control>
                <Control>
                    <ID>RegisterButton</ID>
                    <Description>Register the temporary record to permanent registry</Description>
                    <FriendlyName>Register Record Button</FriendlyName>
                    <PlaceHolderID/>
                    <TypeOfControl>Control</TypeOfControl>
                </Control>
                <Control>
                    <ID>AddComponentButton</ID>
                    <Description>Add components to the current registry</Description>
                    <FriendlyName>Add Components button</FriendlyName>
                    <PlaceHolderID/>
                    <TypeOfControl>Control</TypeOfControl>
                </Control>
                <Control>
                    <ID>DoneAddingButton</ID>
                    <Description>Finish adding components to the current registry</Description>
                    <FriendlyName>Finish Adding Components button</FriendlyName>
                    <PlaceHolderID/>
                    <TypeOfControl>Control</TypeOfControl>
                </Control>
                <Control>
                    <ID>SubmitButton</ID>
                    <Description>Submit the current registry</Description>
                    <FriendlyName>Submit button</FriendlyName>
                    <PlaceHolderID/>
                    <TypeOfControl>Control</TypeOfControl>
                </Control>
                <Control>
                    <ID>SaveButton</ID>
                    <Description>Save the current registry</Description>
                    <FriendlyName>Save button</FriendlyName>
                    <PlaceHolderID/>
                    <TypeOfControl>Control</TypeOfControl>
                </Control>
                <Control>
                    <ID>1</ID>
                    <Description>Details form Mixture</Description>
                    <FriendlyName>This shows the properties of a registry (View Mode)</FriendlyName>
                    <PlaceHolderID>mixtureInformationHolder</PlaceHolderID>
                    <TypeOfControl>COEForm</TypeOfControl>
                </Control>
                <Control>
                    <ID>5</ID>
                    <Description>Fragments COEForm</Description>
                    <FriendlyName>Fragments COEForm</FriendlyName>
                    <PlaceHolderID>mixtureInformationHolder</PlaceHolderID>
                    <TypeOfControl>COEForm</TypeOfControl>
                </Control>
                <Control>
                    <ID>1001</ID>
                    <Description>Compound Properties COEForm</Description>
                    <FriendlyName>Compound Properties COEForm</FriendlyName>
                    <PlaceHolderID>mixtureInformationHolder</PlaceHolderID>
                    <TypeOfControl>COEForm</TypeOfControl>
                </Control>
                <Control>
                    <ID>1002</ID>
                    <Description>Batch Properties COEForm</Description>
                    <FriendlyName>Batch Properties COEForm</FriendlyName>
                    <PlaceHolderID>mixtureInformationHolder</PlaceHolderID>
                    <TypeOfControl>COEForm</TypeOfControl>
                </Control>
                <Control>
                    <ID>4</ID>
                    <Description>Batch COEForm</Description>
                    <FriendlyName>Batch COEForm</FriendlyName>
                    <PlaceHolderID>mixtureInformationHolder</PlaceHolderID>
                    <TypeOfControl>COEForm</TypeOfControl>
                </Control>
                <Control>
                    <ID>1003</ID>
                    <Description>Batch Component COEForm</Description>
                    <FriendlyName>Batch Component COEForm</FriendlyName>
                    <PlaceHolderID>mixtureInformationHolder</PlaceHolderID>
                    <TypeOfControl>COEForm</TypeOfControl>
                </Control>
                <Control>
                    <ID>ACompoundIdentifiersUltraGrid</ID>
                    <Description>Identifier grid</Description>
                    <FriendlyName>Identifier grid</FriendlyName>
                    <PlaceHolderID>mixtureInformationHolder</PlaceHolderID>
                    <TypeOfControl>COEGenerableControl</TypeOfControl>
                    <COEFormID>1</COEFormID>
                </Control>
                <Control>
                    <ID>EMixIdentifiersUltraGrid</ID>
                    <Description>Identifiers grid</Description>
                    <FriendlyName>Identifiers grid</FriendlyName>
                    <PlaceHolderID>mixtureInformationHolder</PlaceHolderID>
                    <TypeOfControl>COEGenerableControl</TypeOfControl>
                    <COEFormID>0</COEFormID>
                </Control>
                <Control>
                    <ID>EMix_ProjectsUltraGrid</ID>
                    <Description>Projects grid - Edit Mode</Description>
                    <FriendlyName>Projects grid - Edit Mode</FriendlyName>
                    <PlaceHolderID>mixtureInformationHolder</PlaceHolderID>
                    <TypeOfControl>COEGenerableControl</TypeOfControl>
                    <COEFormID>0</COEFormID>
                </Control>
                <Control>
                    <ID>SequenceDropDownList</ID>
                    <Description>Component Sequence</Description>
                    <FriendlyName>Component Sequence</FriendlyName>
                    <PlaceHolderID>mixtureInformationHolder</PlaceHolderID>
                    <TypeOfControl>COEGenerableControl</TypeOfControl>
                    <COEFormID>1</COEFormID>
                </Control>
                <Control>
                    <ID>SequenceDropDownList</ID>
                    <Description>Registry Sequence</Description>
                    <FriendlyName>Registry Sequence</FriendlyName>
                    <PlaceHolderID>mixtureInformationHolder</PlaceHolderID>
                    <TypeOfControl>COEGenerableControl</TypeOfControl>
                    <COEFormID>0</COEFormID>
                </Control>
            </Controls>
        </Page>
		<Page>
            <ID>ASP.FORMS_REVIEWREGISTER_CONTENTAREA_REVIEWREGISTERSEARCH_ASPX</ID>
            <Description>Here goes the page description text</Description>
            <FriendlyName>Search Temporary Records</FriendlyName>
            <Controls>
                <Control>
                    <ID>ASP.FORMS_REVIEWREGISTER_CONTENTAREA_REVIEWREGISTERSEARCH_ASPX</ID>
                    <Description>Control description goes here!</Description>
                    <FriendlyName>Control friendly name</FriendlyName>
                    <PlaceHolderID/>
                    <TypeOfControl>Page</TypeOfControl>
                </Control>
                <Control>
                    <ID>FormGeneratorDebuggingInfo</ID>
                    <TypeOfControl>Control</TypeOfControl>
                    <PlaceHolderID>SearchTempFrame</PlaceHolderID>
                    <COEFormID>0</COEFormID>
                </Control>
                <Control>
                    <ID>FormGeneratorDebuggingInfo</ID>
                    <TypeOfControl>Control</TypeOfControl>
                    <PlaceHolderID>SearchTempFrame</PlaceHolderID>
                    <COEFormID>1</COEFormID>
                </Control>
            </Controls>
        </Page>
		 <Page>
            <ID>ASP.FORMS_BULKREGISTERMARKED_CONTENTAREA_DELETEMARKED_ASPX</ID>
            <Description>Here goes the page description text</Description>
            <FriendlyName>Delete Marked</FriendlyName>
            <Controls>
                <Control>
                    <ID>ASP.FORMS_BULKREGISTERMARKED_CONTENTAREA_DELETEMARKED_ASPX</ID>
                    <Description>Control description goes here!</Description>
                    <FriendlyName>Control friendly name</FriendlyName>
                    <PlaceHolderID/>
                    <TypeOfControl>Page</TypeOfControl>
                </Control>
            </Controls>
        </Page>
        <Page>
            <ID>ASP.FORMS_BULKREGISTERMARKED_CONTENTAREA_REGISTERMARKED_ASPX</ID>
            <Description>Here goes the page description text</Description>
            <FriendlyName>Register Marked</FriendlyName>
            <Controls>
                <Control>
                    <ID>ASP.FORMS_BULKREGISTERMARKED_CONTENTAREA_REGISTERMARKED_ASPX</ID>
                    <Description>Control description goes here!</Description>
                    <FriendlyName>Control friendly name</FriendlyName>
                    <PlaceHolderID/>
                    <TypeOfControl>Page</TypeOfControl>
                </Control>
            </Controls>
        </Page>
		     <Page>
            <ID>ASP.FORMS_REGISTRYDUPLICATES_CONTENTAREA_REGISTRYDUPLICATES_ASPX</ID>
            <Description>Here goes the page description text</Description>
            <FriendlyName>Regisrty Duplicate Resolution</FriendlyName>
            <Controls>
                <Control>
                    <ID>ASP.FORMS_REGISTRYDUPLICATES_CONTENTAREA_REGISTRYDUPLICATES_ASPX</ID>
                    <Description>Control description goes here!</Description>
                    <FriendlyName>Control friendly name</FriendlyName>
                    <PlaceHolderID/>
                    <TypeOfControl>Page</TypeOfControl>
                </Control>
            </Controls>
        </Page>
		 <Page>
            <ID>ASP.FORMS_COMPONENTDUPLICATES_CONTENTAREA_COMPONENTDUPLICATES_ASPX</ID>
            <Description>Here goes the page description text</Description>
            <FriendlyName>Component Duplicate Resolution</FriendlyName>
            <Controls>
                <Control>
                    <ID>ASP.FORMS_COMPONENTDUPLICATES_CONTENTAREA_COMPONENTDUPLICATES_ASPX</ID>
                    <Description>Control description goes here!</Description>
                    <FriendlyName>Control friendly name</FriendlyName>
                    <PlaceHolderID/>
                    <TypeOfControl>Page</TypeOfControl>
                </Control>
            </Controls>
        </Page>
';

 L_MasterXml_2 CLOB:= '
		<Page>
            <ID>ASP.FORMS_VIEWMIXTURE_CONTENTAREA_VIEWMIXTURE_ASPX</ID>
            <Description>Here goes the page description text</Description>
            <FriendlyName>View Mixture</FriendlyName>
            <Controls>
                <Control>
                    <ID>ASP.FORMS_VIEWMIXTURE_CONTENTAREA_VIEWMIXTURE_ASPX</ID>
                    <Description>Control description goes here!</Description>
                    <FriendlyName>Control friendly name</FriendlyName>
                    <PlaceHolderID/>
                    <TypeOfControl>Page</TypeOfControl>
                </Control>
                <Control>
                    <ID>EditButton</ID>
                    <Description>Edit record button allows you to go into Edit mode</Description>
                    <FriendlyName>Edit Record Button</FriendlyName>
                    <PlaceHolderID/>
                    <TypeOfControl>Control</TypeOfControl>
                </Control>
                <Control>
                    <ID>DeleteRegButton</ID>
                    <Description>Delete record button allows you to go into Delete mode</Description>
                    <FriendlyName>Delete Record Button</FriendlyName>
                    <PlaceHolderID/>
                    <TypeOfControl>Control</TypeOfControl>
                </Control>
                <Control>
                    <ID>DeleteBatchButton</ID>
                    <Description>Delete batch button allows you to go into Delete  batch mode</Description>
                    <FriendlyName>Delete Batch Button</FriendlyName>
                    <PlaceHolderID/>
                    <TypeOfControl>Control</TypeOfControl>
                </Control>
                <Control>
                    <ID>SendToInventory</ID>
                    <Description>Send Permanent Record to Inventory</Description>
                    <FriendlyName>Send To Inventory Button</FriendlyName>
                    <PlaceHolderID/>
                    <TypeOfControl>Control</TypeOfControl>
                </Control>
                <Control>
                    <ID>AddComponentButton</ID>
                    <Description>Add components to the current registry</Description>
                    <FriendlyName>Add Components button</FriendlyName>
                    <PlaceHolderID/>
                    <TypeOfControl>Control</TypeOfControl>
                </Control>
                <Control>
                    <ID>MoveBatchButton</ID>
                    <Description>Move batch button allows you to go Move a batch to another registry record</Description>
                    <FriendlyName>Move Batch Button</FriendlyName>
                    <PlaceHolderID/>
                    <TypeOfControl>Control</TypeOfControl>
                </Control>
                <Control>
                    <ID>AddBatchButton</ID>
                    <Description>Add batch button allows you to go add a batch to another registry record</Description>
                    <FriendlyName>Add Batch Button</FriendlyName>
                    <PlaceHolderID/>
                    <TypeOfControl>Control</TypeOfControl>
                </Control>
				<Control>
					<ID>0</ID>
					<Description>Details form Mixture</Description>
					<FriendlyName>This shows the properties of a registry (View Mode)</FriendlyName>
					<PlaceHolderID>mixtureInformationHolder</PlaceHolderID>
					<TypeOfControl>COEForm</TypeOfControl>
				</Control>
						<Control>
					<ID>1000</ID>
					<Description>Custom properties Details form Mixture</Description>
					<FriendlyName>This shows the custom properties of a registry (View Mode)</FriendlyName>
					<PlaceHolderID>mixtureInformationHolder</PlaceHolderID>
					<TypeOfControl>COEForm</TypeOfControl>
				</Control>
                <Control>
                    <ID>1</ID>
                    <Description>Details form Mixture</Description>
                    <FriendlyName>This shows the properties of a registry (View Mode)</FriendlyName>
                    <PlaceHolderID>mixtureInformationHolder</PlaceHolderID>
                    <TypeOfControl>COEForm</TypeOfControl>
                </Control>
                <Control>
                    <ID>6</ID>
                    <Description>Fragments COEForm - Edit Mode</Description>
                    <FriendlyName>Fragments COEForm - Edit Mode</FriendlyName>
                    <PlaceHolderID>mixtureInformationHolder</PlaceHolderID>
                    <TypeOfControl>COEForm</TypeOfControl>
                </Control>
                <Control>
                    <ID>8</ID>
                    <Description>Fragments COEForm - View Mode</Description>
                    <FriendlyName>Fragments COEForm - View Mode</FriendlyName>
                    <PlaceHolderID>mixtureInformationHolder</PlaceHolderID>
                    <TypeOfControl>COEForm</TypeOfControl>
                </Control>
                <Control>
                    <ID>1001</ID>
                    <Description>Compound Properties COEForm</Description>
                    <FriendlyName>Compound Properties COEForm</FriendlyName>
                    <PlaceHolderID>mixtureInformationHolder</PlaceHolderID>
                    <TypeOfControl>COEForm</TypeOfControl>
                </Control>
                <Control>
                    <ID>1002</ID>
                    <Description>Batch Properties COEForm</Description>
                    <FriendlyName>Batch Properties COEForm</FriendlyName>
                    <PlaceHolderID>mixtureInformationHolder</PlaceHolderID>
                    <TypeOfControl>COEForm</TypeOfControl>
                </Control>
                <Control>
                    <ID>4</ID>
                    <Description>Batch COEForm</Description>
                    <FriendlyName>Batch COEForm</FriendlyName>
                    <PlaceHolderID>mixtureInformationHolder</PlaceHolderID>
                    <TypeOfControl>COEForm</TypeOfControl>
                </Control>
                <Control>
                    <ID>1003</ID>
                    <Description>Batch Component COEForm</Description>
                    <FriendlyName>Batch Component COEForm</FriendlyName>
                    <PlaceHolderID>mixtureInformationHolder</PlaceHolderID>
                    <TypeOfControl>COEForm</TypeOfControl>
                </Control>
				<Control>
				  <ID>APPROVEDProperty</ID>
				  <Description>Approved or not flag</Description>
				  <FriendlyName>Approved or not flag</FriendlyName>
				  <PlaceHolderID>mixtureInformationHolder</PlaceHolderID>
				  <TypeOfControl>COEGenerableControl</TypeOfControl>
				  <COEFormID>0</COEFormID>
				</Control>
                <Control>
                    <ID>EMix_ProjectsUltraGrid</ID>
                    <Description>Registry Identifier grid - Edit</Description>
                    <FriendlyName>RegistryIdentifier grid - Edit</FriendlyName>
                    <PlaceHolderID>mixtureInformationHolder</PlaceHolderID>
                    <TypeOfControl>COEGenerableControl</TypeOfControl>
                    <COEFormID>0</COEFormID>
                </Control>
                <Control>
                    <ID>ACompound_IdentifiersUltraGrid</ID>
                    <Description>Compound Identifiers grid - Add</Description>
                    <FriendlyName>Compound Identifiers grid - Add</FriendlyName>
                    <PlaceHolderID>mixtureInformationHolder</PlaceHolderID>
                    <TypeOfControl>COEGenerableControl</TypeOfControl>
                    <COEFormID>1</COEFormID>
                </Control>
                <Control>
                    <ID>ECompound_IdentifiersUltraGrid</ID>
                    <Description>Compound Identifers grid - Edit</Description>
                    <FriendlyName>Compound Identifiers grid - Edit</FriendlyName>
                    <PlaceHolderID>mixtureInformationHolder</PlaceHolderID>
                    <TypeOfControl>COEGenerableControl</TypeOfControl>
                    <COEFormID>1</COEFormID>
                </Control>
                <Control>
                    <ID>SequenceDropDownList</ID>
                    <Description>Component Sequence</Description>
                    <FriendlyName>Component Sequence</FriendlyName>
                    <PlaceHolderID>mixtureInformationHolder</PlaceHolderID>
                    <TypeOfControl>COEGenerableControl</TypeOfControl>
                    <COEFormID>1</COEFormID>
                </Control>
                <Control>
                    <ID>SequenceDropDownList</ID>
                    <Description>Registry Sequence</Description>
                    <FriendlyName>Registry Sequence</FriendlyName>
                    <PlaceHolderID>mixtureInformationHolder</PlaceHolderID>
                    <TypeOfControl>COEGenerableControl</TypeOfControl>
                    <COEFormID>0</COEFormID>
                </Control>
                <Control>
                    <ID>EMix_ProjectsUltraGrid</ID>
                    <Description>Registry Projects</Description>
                    <FriendlyName>Registry Projects</FriendlyName>
                    <PlaceHolderID>mixtureInformationHolder</PlaceHolderID>
                    <TypeOfControl>COEGenerableControl</TypeOfControl>
                    <COEFormID>0</COEFormID>
                </Control>
            </Controls>
        </Page>
		 <Page>
            <ID>ASP.FORMS_VIEWMIXTURE_CONTENTAREA_MOVEDELETE_ASPX</ID>
            <Description>Here goes the page description text</Description>
            <FriendlyName>Move Delete Batch</FriendlyName>
            <Controls>
                <Control>
                    <ID>ASP.FORMS_VIEWMIXTURE_CONTENTAREA_MOVEDELETE_ASPX</ID>
                    <Description>Control description goes here!</Description>
                    <FriendlyName>Control friendly name</FriendlyName>
                    <PlaceHolderID/>
                    <TypeOfControl>Page</TypeOfControl>
                </Control>
            </Controls>
        </Page>
		 <Page>
            <ID>ASP.FORMS_VIEWMIXTURE_CONTENTAREA_VIEWMIXTURESEARCH_ASPX</ID>
            <Description>Here goes the page description text</Description>
            <FriendlyName>Search Permanent Records</FriendlyName>
            <Controls>
                <Control>
                    <ID>ASP.FORMS_VIEWMIXTURE_CONTENTAREA_VIEWMIXTURESEARCH_ASPX</ID>
                    <Description>Control description goes here!</Description>
                    <FriendlyName>Control friendly name</FriendlyName>
                    <PlaceHolderID/>
                    <TypeOfControl>Page</TypeOfControl>
                </Control>
            </Controls>
        </Page>
		<Page>
            <ID>ASP.FORMS_REGISTRATIONADMIN_CONTENTAREA_DEFAULT_ASPX</ID>
            <Description>Here goes the page description text</Description>
            <FriendlyName>Reg Admin</FriendlyName>
            <Controls>
                <Control>
                    <ID>ASP.FORMS_REGISTRATIONADMIN_CONTENTAREA_DEFAULT_ASPX</ID>
                    <Description>Control description goes here!</Description>
                    <FriendlyName>Control friendly name</FriendlyName>
                    <PlaceHolderID/>
                    <TypeOfControl>Page</TypeOfControl>
                </Control>
            </Controls>
        </Page>
        <Page>
            <ID>ASP.FORMS_TABLEEDITOR_CONTENTAREA_TABLEEDITOR_ASPX</ID>
            <Description>Page to submit registries (temporary registries until registration)</Description>
            <FriendlyName>Table Editor</FriendlyName>
            <Controls>
                <Control>
                    <ID>ASP.FORMS_TABLEEDITOR_CONTENTAREA_TABLEEDITOR_ASPX</ID>
                    <Description>Page to submit registries (temporary registries until registration)</Description>
                    <FriendlyName>Submit Registries</FriendlyName>
                    <PlaceHolderID/>
                    <TypeOfControl>Page</TypeOfControl>
                </Control>
                <Control>
                    <ID>AddTableButton</ID>
                    <Description/>
                    <FriendlyName>Add Table button</FriendlyName>
                    <PlaceHolderID/>
                    <TypeOfControl>Control</TypeOfControl>
                </Control>
                <Control>
                    <ID>AddRecordButton</ID>
                    <Description>Add components to the current registry</Description>
                    <FriendlyName>Add Record button</FriendlyName>
                    <PlaceHolderID/>
                    <TypeOfControl>Control</TypeOfControl>
                </Control>
                <Control>
                    <ID>EditRecordButton</ID>
                    <Description>Finish adding components to the current registry</Description>
                    <FriendlyName>Edit Record button</FriendlyName>
                    <PlaceHolderID/>
                    <TypeOfControl>Control</TypeOfControl>
                </Control>
                <Control>
                    <ID>DeleteRecordButton</ID>
                    <Description>Submit the current registry</Description>
                    <FriendlyName>Delete Record button</FriendlyName>
                    <PlaceHolderID/>
                    <TypeOfControl>Control</TypeOfControl>
                </Control>
            </Controls>
        </Page>
		<Page>
            <ID>ASP.FORMS_TABLEEDITOR_CONTENTAREA_TABLEEDITOR_ASPX</ID>
            <Description>Here goes the page description text</Description>
            <FriendlyName>TABLEEDITOR_ASPX</FriendlyName>
            <Controls>
                <Control>
                    <ID>ASP.FORMS_TABLEEDITOR_CONTENTAREA_TABLEEDITOR_ASPX</ID>
                    <Description>Control description goes here!</Description>
                    <FriendlyName>Control friendly name</FriendlyName>
                    <PlaceHolderID/>
                    <TypeOfControl>Page</TypeOfControl>
                </Control>
            </Controls>
        </Page>
		  <Page>
            <ID>ASP.FORMS_REGISTRATIONADMIN_CONTENTAREA_ROOTPROPERTYLIST_ASPX</ID>
            <Description>Here goes the page description text</Description>
            <FriendlyName>Manage Registry Properties</FriendlyName>
            <Controls>
                <Control>
                    <ID>ASP.FORMS_REGISTRATIONADMIN_CONTENTAREA_ROOTPROPERTYLIST_ASPX</ID>
                    <Description>Control description goes here!</Description>
                    <FriendlyName>Control friendly name</FriendlyName>
                    <PlaceHolderID/>
                    <TypeOfControl>Page</TypeOfControl>
                </Control>
            </Controls>
        </Page>
		 <Page>
            <ID>ASP.FORMS_REGISTRATIONADMIN_CONTENTAREA_COMPOUND_ASPX</ID>
            <Description>Here goes the page description text</Description>
            <FriendlyName>Manage Component Properties</FriendlyName>
            <Controls>
                <Control>
                    <ID>ASP.FORMS_REGISTRATIONADMIN_CONTENTAREA_COMPOUND_ASPX</ID>
                    <Description>Control description goes here!</Description>
                    <FriendlyName>Control friendly name</FriendlyName>
                    <PlaceHolderID/>
                    <TypeOfControl>Page</TypeOfControl>
                </Control>
            </Controls>
        </Page>
		 <Page>
            <ID>ASP.FORMS_REGISTRATIONADMIN_CONTENTAREA_BATCH_ASPX</ID>
            <Description>Here goes the page description text</Description>
            <FriendlyName>Manage Batch Properties</FriendlyName>
            <Controls>
                <Control>
                    <ID>ASP.FORMS_REGISTRATIONADMIN_CONTENTAREA_BATCH_ASPX</ID>
                    <Description>Control description goes here!</Description>
                    <FriendlyName>Control friendly name</FriendlyName>
                    <PlaceHolderID/>
                    <TypeOfControl>Page</TypeOfControl>
                </Control>
            </Controls>
        </Page>
        <Page>
            <ID>ASP.FORMS_REGISTRATIONADMIN_CONTENTAREA_BATCHCOMPONENT_ASPX</ID>
            <Description>Here goes the page description text</Description>
            <FriendlyName>Manage Batch Component Properties</FriendlyName>
            <Controls>
                <Control>
                    <ID>ASP.FORMS_REGISTRATIONADMIN_CONTENTAREA_BATCHCOMPONENT_ASPX</ID>
                    <Description>Control description goes here!</Description>
                    <FriendlyName>Control friendly name</FriendlyName>
                    <PlaceHolderID/>
                    <TypeOfControl>Page</TypeOfControl>
                </Control>
            </Controls>
        </Page> 
       <Page>
            <ID>ASP.FORMS_REGISTRATIONADMIN_CONTENTAREA_PARAMEDIT_ASPX</ID>
            <Description>Here goes the page description text</Description>
            <FriendlyName>Edit Parameter</FriendlyName>
            <Controls>
                <Control>
                    <ID>ASP.FORMS_REGISTRATIONADMIN_CONTENTAREA_PARAMEDIT_ASPX</ID>
                    <Description>Control description goes here!</Description>
                    <FriendlyName>Control friendly name</FriendlyName>
                    <PlaceHolderID/>
                    <TypeOfControl>Page</TypeOfControl>
                </Control>
            </Controls>
        </Page>
      
        <Page>
            <ID>ASP.FORMS_REGISTRATIONADMIN_CONTENTAREA_VALIDATIONRULES_ASPX</ID>
            <Description>Here goes the page description text</Description>
            <FriendlyName>Edit Validation Rule</FriendlyName>
            <Controls>
                <Control>
                    <ID>ASP.FORMS_REGISTRATIONADMIN_CONTENTAREA_VALIDATIONRULES_ASPX</ID>
                    <Description>Control description goes here!</Description>
                    <FriendlyName>Control friendly name</FriendlyName>
                    <PlaceHolderID/>
                    <TypeOfControl>Page</TypeOfControl>
                </Control>
            </Controls>
        </Page> 		
		<Page>
            <ID>ASP.FORMS_REGISTRATIONADMIN_CONTENTAREA_ADDINS_ASPX</ID>
            <Description>Here goes the page description text</Description>
            <FriendlyName>Manage Addins</FriendlyName>
            <Controls>
                <Control>
                    <ID>ASP.FORMS_REGISTRATIONADMIN_CONTENTAREA_ADDINS_ASPX</ID>
                    <Description>Control description goes here!</Description>
                    <FriendlyName>Control friendly name</FriendlyName>
                    <PlaceHolderID/>
                    <TypeOfControl>Page</TypeOfControl>
                </Control>
            </Controls>
        </Page>
		<Page>
            <ID>ASP.FORMS_REGISTRATIONADMIN_CONTENTAREA_ADDINSEDIT_ASPX</ID>
            <Description>Here goes the page description text</Description>
            <FriendlyName>Edit Addin</FriendlyName>
            <Controls>
                <Control>
                    <ID>ASP.FORMS_REGISTRATIONADMIN_CONTENTAREA_ADDINSEDIT_ASPX</ID>
                    <Description>Control description goes here!</Description>
                    <FriendlyName>Control friendly name</FriendlyName>
                    <PlaceHolderID/>
                    <TypeOfControl>Page</TypeOfControl>
                </Control>
            </Controls>
        </Page>
		<Page>
            <ID>ASP.FORMS_REGISTRATIONADMIN_CONTENTAREA_EDITFORMSXML_ASPX</ID>
            <Description>Here goes the page description text</Description>
            <FriendlyName>Edit Form XML</FriendlyName>
            <Controls>
                <Control>
                    <ID>ASP.FORMS_REGISTRATIONADMIN_CONTENTAREA_EDITFORMSXML_ASPX</ID>
                    <Description>Control description goes here!</Description>
                    <FriendlyName>Control friendly name</FriendlyName>
                    <PlaceHolderID/>
                    <TypeOfControl>Page</TypeOfControl>
                </Control>
            </Controls>
        </Page>
        <Page>
            <ID>ASP.FORMS_REGISTRATIONADMIN_CONTENTAREA_IMPORTEXPORTCUSTOM_ASPX</ID>
            <Description>Here goes the page description text</Description>
            <FriendlyName>Import Export Configuration</FriendlyName>
            <Controls>
                <Control>
                    <ID>ASP.FORMS_REGISTRATIONADMIN_CONTENTAREA_IMPORTEXPORTCUSTOM_ASPX</ID>
                    <Description>Control description goes here!</Description>
                    <FriendlyName>Control friendly name</FriendlyName>
                    <PlaceHolderID/>
                    <TypeOfControl>Page</TypeOfControl>
                </Control>
            </Controls>
        </Page>	  
			<Page>
            <ID>ASP.FORMS_REGISTRATIONADMIN_CONTENTAREA_CONFIGSETTINGS_ASPX</ID>
            <Description>Here goes the page description text</Description>
            <FriendlyName>Config Settings</FriendlyName>
            <Controls>
                <Control>
                    <ID>ASP.FORMS_REGISTRATIONADMIN_CONTENTAREA_CONFIGSETTINGS_ASPX</ID>
                    <Description>Control description goes here!</Description>
                    <FriendlyName>Control friendly name</FriendlyName>
                    <PlaceHolderID/>
                    <TypeOfControl>Page</TypeOfControl>
                </Control>
            </Controls>
        </Page>
        <Page>
            <ID>ASP.FORMS_PAGECONTROLSETTING_CONTENTAREA_PAGECONTROLSETTING_ASPX</ID>
            <Description>Here goes the page description text</Description>
            <FriendlyName>Page Control Setting</FriendlyName>
            <Controls>
                <Control>
                    <ID>ASP.FORMS_PAGECONTROLSETTING_CONTENTAREA_PAGECONTROLSETTING_ASPX</ID>
                    <Description>Control description goes here!</Description>
                    <FriendlyName>Control friendly name</FriendlyName>
                    <PlaceHolderID/>
                    <TypeOfControl>Page</TypeOfControl>
                </Control>
            </Controls>
		</Page>
         <Page>
            <ID>ASP.FORMS_PUBLIC_CONTENTAREA_HELP_ASPX</ID>
            <Description>Here goes the page description text</Description>
            <FriendlyName>Help Page</FriendlyName>
            <Controls>
                <Control>
                    <ID>ASP.FORMS_PUBLIC_CONTENTAREA_HELP_ASPX</ID>
                    <Description>Control description goes here!</Description>
                    <FriendlyName>Control friendly name</FriendlyName>
                    <PlaceHolderID/>
                    <TypeOfControl>Page</TypeOfControl>
                </Control>
            </Controls>
        </Page>
        <Page>
            <ID>ASP.FORMS_PUBLIC_CONTENTAREA_ABOUT_ASPX</ID>
            <Description>Here goes the page description text</Description>
            <FriendlyName>About Page</FriendlyName>
            <Controls>
                <Control>
                    <ID>ASP.FORMS_PUBLIC_CONTENTAREA_ABOUT_ASPX</ID>
                    <Description>Control description goes here!</Description>
                    <FriendlyName>Control friendly name</FriendlyName>
                    <PlaceHolderID/>
                    <TypeOfControl>Page</TypeOfControl>
                </Control>
            </Controls>
        </Page>

        <Page>
            <ID>ASP.FORMS_PUBLIC_CONTENTAREA_HOME_ASPX</ID>
            <Description>Here goes the page description text</Description>
            <FriendlyName>Home</FriendlyName>
            <Controls>
                <Control>
                    <ID>ASP.FORMS_PUBLIC_CONTENTAREA_HOME_ASPX</ID>
                    <Description>Control description goes here!</Description>
                    <FriendlyName>Control friendly name</FriendlyName>
                    <PlaceHolderID/>
                    <TypeOfControl>Page</TypeOfControl>
                </Control>
            </Controls>
        </Page>
        <Page>
            <ID>ASP.FORMS_PUBLIC_CONTENTAREA_MESSAGES_ASPX</ID>
            <Description>Here goes the page description text</Description>
            <FriendlyName>MESSAGES_ASPX</FriendlyName>
            <Controls>
                <Control>
                    <ID>ASP.FORMS_PUBLIC_CONTENTAREA.MESSAGES_ASPX</ID>
                    <Description>Control description goes here!</Description>
                    <FriendlyName>Control friendly name</FriendlyName>
                    <PlaceHolderID/>
                    <TypeOfControl>Page</TypeOfControl>
                </Control>
            </Controls>
        </Page>
    </Pages>
    <AppSettings>
        <AppSetting>
            <ID>SearchEngineURL</ID>
            <Key>Registration/CBV/SearchEngineURL</Key>
            <Value>/ChemBioViz/Forms/Search/ContentArea/ChemBioVizSearch.aspx</Value>
            <Type>COEConfiguration</Type>
        </AppSetting>
        <AppSetting>
            <ID>DataLoaderFormGroupId</ID>
            <Key>Registration/CBV/ReviewRegisterRegistryFormGroupId</Key>
            <Value>4011</Value>
            <Type>COEConfiguration</Type>
        </AppSetting>
        <AppSetting>
            <ID>SessionUserID</ID>
            <Key>Session User ID</Key>
            <Value>15</Value>
            <Type>Session</Type>
        </AppSetting>
        <AppSetting>
            <ID>MultipleVariable</ID>
            <Key>MultipleVariable</Key>
            <Value>MultipleVariableValue</Value>
            <Type>All</Type>
        </AppSetting>
        <AppSetting>
            <ID>ApprovalsEnabled</ID>
            <Key>Registration/REGADMIN/ApprovalsEnabled</Key>
            <Value>False</Value>
            <Type>COEConfiguration</Type>
        </AppSetting>
        <AppSetting>
            <ID>IsRegistryApproved</ID>
            <Key>IsRegistryApproved</Key>
            <Value>False</Value>
            <Type>Session</Type>
        </AppSetting>
    </AppSettings>
</COEPageControlSettings>';

 L_CustomXml_1 CLOB:= '<?xml version="1.0" encoding="utf-8"?>
<COEPageControlSettings>
    <Type>Custom</Type>
    <Application>REGISTRATION</Application>
    <Pages>
        <Page>
            <ID>ASP.FORMS_SUBMITRECORD_CONTENTAREA_SUBMITMIXTURE_ASPX</ID>
            <ControlSettings>
               <ControlSetting>
                    <ID>SUBMITRECORD</ID>
                    <Privileges>
                        <Operator>OR</Operator>
                        <Privilege>
                            <ID>ADD_COMPOUND_TEMP</ID>
                        </Privilege>
                        <Privilege>
                            <ID>REGISTER_DIRECT</ID>
                        </Privilege>
                    </Privileges>
                    <Controls>
                        <Control>
                            <ID>RegisterButton</ID>
                            <TypeOfControl>Control</TypeOfControl>
                            <PlaceHolderID>
                            </PlaceHolderID>
                            <COEFormID>-1</COEFormID>
                        </Control>
                        <Control>
                            <ID>ASP.FORMS_SUBMITRECORD_CONTENTAREA_SUBMITMIXTURE_ASPX</ID>
                            <TypeOfControl>Page</TypeOfControl>
                            <PlaceHolderID>
                            </PlaceHolderID>
                            <COEFormID>-1</COEFormID>
                        </Control>
                    </Controls>
                    <AppSettings/>
                </ControlSetting>
            </ControlSettings>
        </Page>
        <Page>
            <ID>ASP.FORMS_REVIEWREGISTER_CONTENTAREA_REVIEWREGISTERMIXTURE_ASPX</ID>
            <ControlSettings>
                <ControlSetting>
                    <ID>EditCompoundTemp</ID>
                    <Privileges>
                        <Operator>OR</Operator>
                        <ID>EditCompoundTemp</ID>
                        <Privilege>
                            <ID>EDIT_COMPOUND_TEMP</ID>
                        </Privilege>
                    </Privileges>
                    <AppSettings>
                    </AppSettings>
                    <Controls>
                        <Control>
                            <ID>EditButton</ID>
                            <TypeOfControl>Control</TypeOfControl>
                            <PlaceHolderID>
                            </PlaceHolderID>
                            <COEFormID>-1</COEFormID>
                        </Control>
                    </Controls>
                </ControlSetting>
                <ControlSetting>
                    <ID>RegisterFromTemp</ID>
                    <Privileges>
                        <Operator>OR</Operator>
                        <ID>RegisterFromTemp</ID>
                        <Privilege>
                            <ID>REGISTER_TEMP</ID>
                        </Privilege>
                        <Controls>
                            <Control>
                                <ID>RegisterButton</ID>
                                <TypeOfControl>Control</TypeOfControl>
                                <PlaceHolderID>
                                </PlaceHolderID>
                                <COEFormID>-1</COEFormID>
                            </Control>
                        </Controls>
                    </Privileges>
                    <AppSettings>
                    </AppSettings>
                    <Controls>
                        <Control>
                            <ID>RegisterButton</ID>
                            <TypeOfControl>Control</TypeOfControl>
                            <PlaceHolderID>
                                </PlaceHolderID>
                            <COEFormID>-1</COEFormID>
                        </Control>
                    </Controls>
                </ControlSetting>
            </ControlSettings>
        </Page>
        <Page>
            <ID>ASP.FORMS_BULKREGISTERMARKED_CONTENTAREA_REGISTERMARKED_ASPX</ID>
            <ControlSettings>
                <ControlSetting>
                    <ID>RegisterMarkedPageAccess</ID>
                    <Privileges>
                        <Operator>OR</Operator>
                        <ID>RegisterMarkedPageAccess</ID>
                        <Privilege>
                            <ID>REGISTER_TEMP</ID>
                        </Privilege>
                    </Privileges>
                    <AppSettings>
                    </AppSettings>
                    <Controls>
                        <Control>
                            <ID>ASP.FORMS_BULKREGISTERMARKED_CONTENTAREA_REGISTERMARKED_ASPX</ID>
                            <TypeOfControl>Page</TypeOfControl>
                            <PlaceHolderID>
                            </PlaceHolderID>
                            <COEFormID>-1</COEFormID>
                        </Control>
                    </Controls>
                </ControlSetting>
            </ControlSettings>
        </Page>
        <Page>
            <ID>ASP.FORMS_BULKREGISTERMARKED_CONTENTAREA_REGISTERMARKED_ASPX</ID>
            <ControlSettings>
                <ControlSetting>
                    <ID>RegisterMarkedPageAccess</ID>
                    <Privileges>
                        <Operator>OR</Operator>
                        <ID>RegisterMarkedPageAccess</ID>
                        <Privilege>
                            <ID>REGISTER_TEMP</ID>
                        </Privilege>
                    </Privileges>
                    <AppSettings>
                    </AppSettings>
                    <Controls>
                        <Control>
                            <ID>ASP.FORMS_BULKREGISTERMARKED_CONTENTAREA_REGISTERMARKED_ASPX</ID>
                            <TypeOfControl>Page</TypeOfControl>
                            <PlaceHolderID>
                            </PlaceHolderID>
                            <COEFormID>-1</COEFormID>
                        </Control>
                    </Controls>
                </ControlSetting>
            </ControlSettings>
        </Page>
        <Page>
            <ID>ASP.FORMS_VIEWMIXTURE_CONTENTAREA_MOVEDELETE_ASPX</ID>
            <ControlSettings>
                <ControlSetting>
                    <ID>MoveDeletePageAccess</ID>
                    <Privileges>
                        <Operator>OR</Operator>
                        <ID>MoveDeletePageAccess</ID>
                        <Privilege>
                            <ID>DELETE_BATCH_REG</ID>
                        </Privilege>
                    </Privileges>
                    <Controls>
                        <Control>
                            <ID>ASP.FORMS_VIEWMIXTURE_CONTENTAREA_MOVEDELETE_ASPX</ID>
                            <TypeOfControl>Page</TypeOfControl>
                            <PlaceHolderID>
                            </PlaceHolderID>
                            <COEFormID>-1</COEFormID>
                        </Control>
                    </Controls>
                    <AppSettings>
                    </AppSettings>
                </ControlSetting>
            </ControlSettings>
        </Page>
';	

 L_CustomXml_2 CLOB:= '		
 <Page>
            <ID>ASP.FORMS_VIEWMIXTURE_CONTENTAREA_VIEWMIXTURE_ASPX</ID>
            <ControlSettings>
                <ControlSetting>
                    <ID>EditRegisteredRecord</ID>
                    <Privileges>
                        <Operator>OR</Operator>
                        <ID>EditRegisteredRecord</ID>
                        <Privilege>
                            <ID>EDIT_COMPOUND_REG</ID>
                        </Privilege>
                    </Privileges>
                    <Controls>
                        <Control>
                            <ID>EditButton</ID>
                            <TypeOfControl>Control</TypeOfControl>
                            <PlaceHolderID>
                                </PlaceHolderID>
                            <COEFormID>-1</COEFormID>
                        </Control>
                    </Controls>
                    <AppSettings>
                    </AppSettings>
                </ControlSetting>
                <ControlSetting>
                    <ID>DeleteRegisteredRecord</ID>
                    <Privileges>
                        <Operator>OR</Operator>
                        <ID>DeleteRegisteredRecord</ID>
                        <Privilege>
                            <ID>DELETE_REG</ID>
                        </Privilege>
                    </Privileges>
                    <Controls>
                        <Control>
                            <ID>DeleteRegButton</ID>
                            <TypeOfControl>Control</TypeOfControl>
                            <PlaceHolderID>
                            </PlaceHolderID>
                            <COEFormID>-1</COEFormID>
                        </Control>
                    </Controls>
                    <AppSettings>
                    </AppSettings>
                </ControlSetting>
                <ControlSetting>
                    <ID>DeleteMoveBatchFromPerm</ID>
                    <Privileges>
                        <Operator>OR</Operator>
                        <ID>DeleteMoveBatchFromPerm</ID>
                        <Privilege>
                            <ID>DELETE_BATCH_REG</ID>
                        </Privilege>
                        <Controls>
                            <Control>
                                <ID>DeleteBatchButton</ID>
                                <TypeOfControl>Control</TypeOfControl>
                                <PlaceHolderID>
                                </PlaceHolderID>
                                <COEFormID>-1</COEFormID>
                            </Control>
                            <Control>
                                <ID>MoveBatchButton</ID>
                                <TypeOfControl>Control</TypeOfControl>
                                <PlaceHolderID>
                                </PlaceHolderID>
                                <COEFormID>-1</COEFormID>
                            </Control>
                        </Controls>
                    </Privileges>
                    <Controls>
                        <Control>
                            <ID>DeleteBatchButton</ID>
                            <TypeOfControl>Control</TypeOfControl>
                            <PlaceHolderID>
                            </PlaceHolderID>
                            <COEFormID>-1</COEFormID>
                        </Control>
                        <Control>
                            <ID>MoveBatchButton</ID>
                            <TypeOfControl>Control</TypeOfControl>
                            <PlaceHolderID>
                            </PlaceHolderID>
                            <COEFormID>-1</COEFormID>
                        </Control>
                    </Controls>
                    <AppSettings>
                    </AppSettings>
                </ControlSetting>
                <ControlSetting>
                    <ID>AddComponentPerm</ID>
                    <Privileges>
                        <Operator>OR</Operator>
                        <ID>AddComponentPerm</ID>
                        <Privilege>
                            <ID>ADD_COMPONENT</ID>
                        </Privilege>
                    </Privileges>
                    <Controls>
                        <Control>
                            <ID>AddComponentButton</ID>
                            <TypeOfControl>Control</TypeOfControl>
                            <PlaceHolderID>
                            </PlaceHolderID>
                            <COEFormID>-1</COEFormID>
                        </Control>
                    </Controls>
                    <AppSettings>
                    </AppSettings>
                </ControlSetting>
				<ControlSetting>
                    <ID>AddBatchPerm</ID>
                    <Privileges>
                        <Operator>OR</Operator>
                        <ID>AddBatchPerm</ID>
                        <Privilege>
                            <ID>ADD_BATCH_PERM</ID>
                        </Privilege>
                    </Privileges>
                    <Controls>
                        <Control>
                            <ID>AddBatchButton</ID>
                            <TypeOfControl>Control</TypeOfControl>
                            <PlaceHolderID>
                            </PlaceHolderID>
                            <COEFormID>-1</COEFormID>
                        </Control>
                    </Controls>
                    <AppSettings>
                    </AppSettings>
                </ControlSetting>
                <ControlSetting>
                    <ID>SendToInventory</ID>
                    <Privileges>
                        <Operator>OR</Operator>
                        <ID>SendToInventory</ID>
                        <Privilege>
                            <ID>INV_CREATE_CONTAINER</ID>
                        </Privilege>
                    </Privileges>
                    <Controls>
                        <Control>
                            <ID>SendToInventory</ID>
                            <TypeOfControl>Control</TypeOfControl>
                            <PlaceHolderID>
                            </PlaceHolderID>
                            <COEFormID>-1</COEFormID>
                        </Control>
                    </Controls>
                    <AppSettings>
                    </AppSettings>
                </ControlSetting>
                <ControlSetting>
                    <ID>EditApprovedRecord</ID>
                    <Privileges>
                        <Operator>OR</Operator>
                        <ID/>
                        <Privilege>
                            <ID>TOGGLE_APPROVED_FLAG</ID>
                        </Privilege>
                    </Privileges>
                    <Controls>
                        <Control>
                            <ID>EditButton</ID>
                            <TypeOfControl>Control</TypeOfControl>
                            <PlaceHolderID/>
                            <COEFormID>-1</COEFormID>
                        </Control>
                        <Control>
						  <ID>0</ID>
						  <Description>Details form Mixture</Description>
						  <FriendlyName>This shows the properties of a registry (View Mode)</FriendlyName>
						  <PlaceHolderID>mixtureInformationHolder</PlaceHolderID>
						  <TypeOfControl>COEForm</TypeOfControl>
						</Control>
						<Control>
						  <ID>1000</ID>
						  <Description>Custom properties Details form Mixture</Description>
						  <FriendlyName>This shows the custom properties of a registry (View Mode)</FriendlyName>
						  <PlaceHolderID>mixtureInformationHolder</PlaceHolderID>
						  <TypeOfControl>COEForm</TypeOfControl>
						</Control>
						<Control>
						  <ID>1</ID>
						  <Description>Details form Mixture</Description>
						  <FriendlyName>This shows the properties of a registry (View Mode)</FriendlyName>
						  <PlaceHolderID>mixtureInformationHolder</PlaceHolderID>
						  <TypeOfControl>COEForm</TypeOfControl>
						</Control>
						<Control>
						  <ID>1001</ID>
						  <Description>Compound Properties COEForm</Description>
						  <FriendlyName>Compound Properties COEForm</FriendlyName>
						  <PlaceHolderID>mixtureInformationHolder</PlaceHolderID>
						  <TypeOfControl>COEForm</TypeOfControl>
						</Control>                                    
                    </Controls>
                    <AppSettings>
                        <Operator>OR</Operator>
                        <ID/>
                        <AppSetting>
                            <ID>ApprovalsEnabled</ID>
                            <Key>Registration/REGADMIN/ApprovalsEnabled</Key>
                            <Value>False</Value>
                            <Type>COEConfiguration</Type>
                        </AppSetting>
                        <AppSetting>
                            <ID>IsRegistryApproved</ID>
                            <Key>IsRegistryApproved</Key>
                            <Value>False</Value>
                            <Type>Session</Type>
                        </AppSetting>
                    </AppSettings>
                </ControlSetting>
				<ControlSetting>
				  <ID>ApprovedEnabled</ID>
				  <Privileges>
					<Operator>OR</Operator>
					<ID></ID>
					<Privilege>
					  <ID>SET_APPROVED_FLAG</ID>
					</Privilege>
				  </Privileges>
				  <Controls>
					<Control>
					  <ID>APPROVEDProperty</ID>
					  <TypeOfControl>COEGenerableControl</TypeOfControl>
					  <PlaceHolderID>mixtureInformationHolder</PlaceHolderID>
					  <COEFormID>0</COEFormID>
					</Control>
				  </Controls>
				  <AppSettings>
					<Operator>AND</Operator>
					<ID></ID>
					<AppSetting>
					  <ID>ApprovalsEnabled</ID>
					  <Key>Registration/REGADMIN/ApprovalsEnabled</Key>
					  <Value>True</Value>
					  <Type>COEConfiguration</Type>
					</AppSetting>
				  </AppSettings>
				</ControlSetting>
            </ControlSettings>
        </Page>
        <Page>
            <ID>ASP.FORMS_VIEWMIXTURE_CONTENTAREA_VIEWMIXTURESEARCH_ASPX</ID>
            <ControlSettings>
                <ControlSetting>
                    <ID>SearchRegistry</ID>
                    <Privileges>
                        <Operator>OR</Operator>
                        <ID>SearchRegistry</ID>
                        <Privilege>
                            <ID>SEARCH_REG</ID>
                        </Privilege>
                    </Privileges>
                    <Controls>
                        <Control>
                            <ID>ASP.FORMS_VIEWMIXTURE_CONTENTAREA_VIEWMIXTURESEARCH_ASPX</ID>
                            <TypeOfControl>Page</TypeOfControl>
                            <PlaceHolderID>
                            </PlaceHolderID>
                            <COEFormID>-1</COEFormID>
                        </Control>
                    </Controls>
                    <AppSettings>
                    </AppSettings>
                </ControlSetting>
            </ControlSettings>
        </Page>
        <Page>
            <ID>ASP.FORMS_TABLEEDITOR_CONTENTAREA_TABLEEDITOR_ASPX</ID>
            <ControlSettings>
                <ControlSetting>
                    <ID>TableEditorPage</ID>
                    <Privileges>
                        <Operator>OR</Operator>
                        <ID>TableEditorPage</ID>
                        <Privilege>
                            <ID>CONFIG_REG</ID>
                        </Privilege>
                    </Privileges>
                    <Controls>
                        <Control>
                            <ID>ASP.FORMS_TABLEEDITOR_CONTENTAREA_TABLEEDITOR_ASPX</ID>
                            <TypeOfControl>Page</TypeOfControl>
                            <PlaceHolderID>
                            </PlaceHolderID>
                            <COEFormID>-1</COEFormID>
                        </Control>
                    </Controls>
                    <AppSettings>
                    </AppSettings>
                </ControlSetting>
            </ControlSettings>
        </Page>
        <Page>
            <ID>ASP.FORMS_REGISTRATIONADMIN_CONTENTAREA_ADDINS_ASPX</ID>
            <ControlSettings>
                <ControlSetting>
                    <ID>RegAdmin_AddInsPage</ID>
                    <Privileges>
                        <Operator>OR</Operator>
                        <ID>RegAdmin_AddInsPage</ID>
                        <Privilege>
                            <ID>CONFIG_REG</ID>
                        </Privilege>
                    </Privileges>
                    <Controls>
                        <Control>
                            <ID>ASP.FORMS_REGISTRATIONADMIN_CONTENTAREA_ADDINS_ASPX</ID>
                            <TypeOfControl>Page</TypeOfControl>
                            <PlaceHolderID>
                            </PlaceHolderID>
                            <COEFormID>-1</COEFormID>
                        </Control>
                    </Controls>
                    <AppSettings>
                    </AppSettings>
                </ControlSetting>
            </ControlSettings>
        </Page>
        <Page>
            <ID>ASP.FORMS_REGISTRATIONADMIN_CONTENTAREA_ADDINSEDIT_ASPX</ID>
            <ControlSettings>
                <ControlSetting>
                    <ID>RegAdmin_AddInsEditPage</ID>
                    <Privileges>
                        <Operator>OR</Operator>
                        <ID>RegAdmin_AddInsEditPage</ID>
                        <Privilege>
                            <ID>CONFIG_REG</ID>
                        </Privilege>
                    </Privileges>
                    <Controls>
                        <Control>
                            <ID>ASP.FORMS_REGISTRATIONADMIN_CONTENTAREA_ADDINSEDIT_ASPX</ID>
                            <TypeOfControl>Page</TypeOfControl>
                            <PlaceHolderID>
                            </PlaceHolderID>
                            <COEFormID>-1</COEFormID>
                        </Control>
                    </Controls>
                    <AppSettings>
                    </AppSettings>
                </ControlSetting>
            </ControlSettings>
        </Page>
        <Page>
            <ID>ASP.FORMS_REGISTRATIONADMIN_CONTENTAREA_BATCH_ASPX</ID>
            <ControlSettings>
                <ControlSetting>
                    <ID>RegAdmin_BatchPage</ID>
                    <Privileges>
                        <Operator>OR</Operator>
                        <ID>RegAdmin_BatchPage</ID>
                        <Privilege>
                            <ID>CONFIG_REG</ID>
                        </Privilege>
                        <Controls>
                            <Control>
                                <ID>ASP.FORMS_REGISTRATIONADMIN_CONTENTAREA_BATCH_ASPX</ID>
                                <TypeOfControl>Page</TypeOfControl>
                                <PlaceHolderID>
                                </PlaceHolderID>
                                <COEFormID>-1</COEFormID>
                            </Control>
                        </Controls>
                    </Privileges>
                    <Controls>
                        <Control>
                            <ID>ASP.FORMS_REGISTRATIONADMIN_CONTENTAREA_BATCH_ASPX</ID>
                            <TypeOfControl>Page</TypeOfControl>
                            <PlaceHolderID>
                            </PlaceHolderID>
                            <COEFormID>-1</COEFormID>
                        </Control>
                    </Controls>
                    <AppSettings>
                    </AppSettings>
                </ControlSetting>
            </ControlSettings>
        </Page>
        <Page>
            <ID>ASP.FORMS_REGISTRATIONADMIN_CONTENTAREA_COMPOUND_ASPX</ID>
            <ControlSettings>
                <ControlSetting>
                    <ID>RegAdmin_Compound</ID>
                    <Privileges>
                        <Operator>OR</Operator>
                        <ID>RegAdmin_Compound</ID>
                        <Privilege>
                            <ID>CONFIG_REG</ID>
                        </Privilege>
                    </Privileges>
                    <Controls>
                        <Control>
                            <ID>ASP.FORMS_REGISTRATIONADMIN_CONTENTAREA_COMPOUND_ASPX</ID>
                            <TypeOfControl>Page</TypeOfControl>
                            <PlaceHolderID>
                            </PlaceHolderID>
                            <COEFormID>-1</COEFormID>
                        </Control>
                    </Controls>
                    <AppSettings>
                    </AppSettings>
                </ControlSetting>
            </ControlSettings>
        </Page>
        <Page>
            <ID>ASP.FORMS_REGISTRATIONADMIN_CONTENTAREA_CONFIGSETTINGS_ASPX</ID>
            <ControlSettings>
                <ControlSetting>
                    <ID>RegAdmin_ConfigSet</ID>
                    <Privileges>
                        <Operator>OR</Operator>
                        <ID>RegAdmin_ConfigSet</ID>
                        <Privilege>
                            <ID>CONFIG_REG</ID>
                        </Privilege>
                    </Privileges>
                    <Controls>
                        <Control>
                            <ID>ASP.FORMS_REGISTRATIONADMIN_CONTENTAREA_CONFIGSETTINGS_ASPX</ID>
                            <TypeOfControl>Page</TypeOfControl>
                            <PlaceHolderID>
                            </PlaceHolderID>
                            <COEFormID>-1</COEFormID>
                        </Control>
                    </Controls>
                    <AppSettings>
                    </AppSettings>
                </ControlSetting>
            </ControlSettings>
        </Page>
        <Page>
            <ID>ASP.FORMS_REGISTRATIONADMIN_CONTENTAREA_CUSTOMIZEFORMS_ASPX</ID>
            <ControlSettings>
                <ControlSetting>
                    <ID>RegAdmin_CustomizeForms</ID>
                    <Privileges>
                        <Operator>OR</Operator>
                        <ID>RegAdmin_CustomizeForms</ID>
                        <Privilege>
                            <ID>CONFIG_REG</ID>
                        </Privilege>
                    </Privileges>
                    <Controls>
                        <Control>
                            <ID>ASP.FORMS_REGISTRATIONADMIN_CONTENTAREA_CUSTOMIZEFORMS_ASPX</ID>
                            <TypeOfControl>Page</TypeOfControl>
                            <PlaceHolderID>
                            </PlaceHolderID>
                            <COEFormID>-1</COEFormID>
                        </Control>
                    </Controls>
                    <AppSettings>
                    </AppSettings>
                </ControlSetting>
            </ControlSettings>
        </Page>
 ';
L_CustomXml_3 CLOB:= '
<Page>
            <ID>ASP.FORMS_REGISTRATIONADMIN_CONTENTAREA_DEFAULT_ASPX</ID>
            <ControlSettings>
                <ControlSetting>
                    <ID>RegAdmin_Default</ID>
                    <Privileges>
                        <Operator>OR</Operator>
                        <ID>RegAdmin_Default</ID>
                        <Privilege>
                            <ID>CONFIG_REG</ID>
                        </Privilege>
                    </Privileges>
                    <Controls>
                        <Control>
                            <ID>ASP.FORMS_REGISTRATIONADMIN_CONTENTAREA_DEFAULT_ASPX</ID>
                            <TypeOfControl>Page</TypeOfControl>
                            <PlaceHolderID>
                            </PlaceHolderID>
                            <COEFormID>-1</COEFormID>
                        </Control>
                    </Controls>
                    <AppSettings>
                    </AppSettings>
                </ControlSetting>
            </ControlSettings>
        </Page>
        <Page>
            <ID>ASP.FORMS_REGISTRATIONADMIN_CONTENTAREA_EDITFORMXML_ASPX</ID>
            <ControlSettings>
                <ControlSetting>
                    <ID>RegAdmin_EditForm</ID>
                    <Privileges>
                        <Operator>OR</Operator>
                        <ID>RegAdmin_EditForm</ID>
                        <Privilege>
                            <ID>CONFIG_REG</ID>
                        </Privilege>
                    </Privileges>
                    <Controls>
                        <Control>
                            <ID>ASP.FORMS_REGISTRATIONADMIN_CONTENTAREA_EDITFORMXML_ASPX</ID>
                            <TypeOfControl>Page</TypeOfControl>
                            <PlaceHolderID>
                            </PlaceHolderID>
                            <COEFormID>-1</COEFormID>
                        </Control>
                    </Controls>
                    <AppSettings>
                    </AppSettings>
                </ControlSetting>
            </ControlSettings>
        </Page>
        <Page>
            <ID>ASP.FORMS_REGISTRATIONADMIN_CONTENTAREA_IMPORTEXPORTCUSTOM_ASPX</ID>
            <ControlSettings>
                <ControlSetting>
                    <ID>RegAdmin_ImpExp</ID>
                    <Privileges>
                        <Operator>OR</Operator>
                        <ID>RegAdmin_ImpExp</ID>
                        <Privilege>
                            <ID>CONFIG_REG</ID>
                        </Privilege>
                    </Privileges>
                    <Controls>
                        <Control>
                            <ID>ASP.FORMS_REGISTRATIONADMIN_CONTENTAREA_IMPORTEXPORTCUSTOM_ASPX</ID>
                            <TypeOfControl>Page</TypeOfControl>
                            <PlaceHolderID>
                            </PlaceHolderID>
                            <COEFormID>-1</COEFormID>
                        </Control>
                    </Controls>
                    <AppSettings>
                    </AppSettings>
                </ControlSetting>
            </ControlSettings>
        </Page>
        <Page>
            <ID>ASP.FORMS_REGISTRATIONADMIN_CONTENTAREA_PARAMEDIT_ASPX</ID>
            <ControlSettings>
                <ControlSetting>
                    <ID>RegAdmin_ParamEdit</ID>
                    <Privileges>
                        <Operator>OR</Operator>
                        <ID>RegAdmin_ParamEdit</ID>
                        <Privilege>
                            <ID>CONFIG_REG</ID>
                        </Privilege>
                    </Privileges>
                    <Controls>
                        <Control>
                            <ID>ASP.FORMS_REGISTRATIONADMIN_CONTENTAREA_PARAMEDIT_ASPX</ID>
                            <TypeOfControl>Page</TypeOfControl>
                            <PlaceHolderID>
                            </PlaceHolderID>
                            <COEFormID>-1</COEFormID>
                        </Control>
                    </Controls>
                    <AppSettings>
                    </AppSettings>
                </ControlSetting>
            </ControlSettings>
        </Page>
        <Page>
            <ID>ASP.FORMS_REGISTRATIONADMIN_CONTENTAREA_PROPERTYEDIT_ASPX</ID>
            <ControlSettings>
                <ControlSetting>
                    <ID>RegAdmin_PropEdit</ID>
                    <Privileges>
                        <Operator>OR</Operator>
                        <ID>RegAdmin_PropEdit</ID>
                        <Privilege>
                            <ID>CONFIG_REG</ID>
                        </Privilege>
                    </Privileges>
                    <Controls>
                        <Control>
                            <ID>ASP.FORMS_REGISTRATIONADMIN_CONTENTAREA_PROPERTYEDIT_ASPX</ID>
                            <TypeOfControl>Page</TypeOfControl>
                            <PlaceHolderID>
                            </PlaceHolderID>
                            <COEFormID>-1</COEFormID>
                        </Control>
                    </Controls>
                    <AppSettings>
                    </AppSettings>
                </ControlSetting>
            </ControlSettings>
        </Page>
        <Page>
            <ID>ASP.FORMS_REGISTRATIONADMIN_CONTENTAREA_ROOTPROPERTYLIST_ASPX</ID>
            <ControlSettings>
                <ControlSetting>
                    <ID>RegAdmin_RootPropertyList</ID>
                    <Privileges>
                        <Operator>OR</Operator>
                        <ID>RegAdmin_RootPropertyList</ID>
                        <Privilege>
                            <ID>CONFIG_REG</ID>
                        </Privilege>
                    </Privileges>
                    <Controls>
                        <Control>
                            <ID>ASP.FORMS_REGISTRATIONADMIN_CONTENTAREA_ROOTPROPERTYLIST_ASPX</ID>
                            <TypeOfControl>Page</TypeOfControl>
                            <PlaceHolderID>
                            </PlaceHolderID>
                            <COEFormID>-1</COEFormID>
                        </Control>
                    </Controls>
                    <AppSettings>
                    </AppSettings>
                </ControlSetting>
            </ControlSettings>
        </Page>
        <Page>
            <ID>ASP.FORMS_REGISTRATIONADMIN_CONTENTAREA_VALIDATIONRULES_ASPX</ID>
            <ControlSettings>
                <ControlSetting>
                    <ID>RegAdmin_ValRules</ID>
                    <Privileges>
                        <Operator>OR</Operator>
                        <ID>RegAdmin_ValRules</ID>
                        <Privilege>
                            <ID>CONFIG_REG</ID>
                        </Privilege>
                    </Privileges>
                    <Controls>
                        <Control>
                            <ID>ASP.FORMS_REGISTRATIONADMIN_CONTENTAREA_VALIDATIONRULES_ASPX</ID>
                            <TypeOfControl>Page</TypeOfControl>
                            <PlaceHolderID>
                            </PlaceHolderID>
                            <COEFormID>-1</COEFormID>
                        </Control>
                    </Controls>
                    <AppSettings>
                    </AppSettings>
                </ControlSetting>
            </ControlSettings>
        </Page>
        <Page>
            <ID>ASP.FORMS_COMPONENTDUPLICATES_CONTENTAREA_COMPONENTDUPLICATES_ASPX</ID>
            <ControlSettings>
            </ControlSettings>
        </Page>
    </Pages>
</COEPageControlSettings>';
 
 L_PrivilegesXml CLOB:= 'CHEM_REG_PRIVILEGES';

 L_COEPageControl_Seq NUmber(8);

BEGIN
	INSERT INTO COEPAGECONTROL (APPLICATION, TYPE, CONFIGURATIONXML) VALUES ('REGISTRATION','MASTER', L_MasterXml_1);
	SELECT COEPageControl_seq.CURRVAL INTO L_COEPageControl_Seq FROM DUAL;
	UPDATE COEPAGECONTROL SET CONFIGURATIONXML=CONFIGURATIONXML||L_MasterXml_2 WHERE ID=L_COEPageControl_Seq;

	INSERT INTO COEPAGECONTROL (APPLICATION, TYPE, CONFIGURATIONXML) VALUES ('REGISTRATION','CUSTOM', L_CustomXml_1);	
	SELECT COEPageControl_seq.CURRVAL INTO L_COEPageControl_Seq FROM DUAL;
	UPDATE COEPAGECONTROL SET CONFIGURATIONXML=CONFIGURATIONXML||L_CustomXml_2||L_CustomXml_3 WHERE ID=L_COEPageControl_Seq;
	
	INSERT INTO COEPAGECONTROL (APPLICATION, TYPE, CONFIGURATIONXML) VALUES ('REGISTRATION','PRIVILEGES', L_PrivilegesXml);
	COMMIT;    
END;
/


--#########################################################
--GRANTS
--#########################################################
 

GRANT SELECT ON People TO &&schemaName;
GRANT SELECT ON Chem_reg_privileges TO &&schemaName;
GRANT SELECT ON Privilege_tables TO &&schemaName;
GRANT SELECT ON Security_roles TO &&schemaName;

