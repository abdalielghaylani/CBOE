-- Copyright 1998-2014 CambridgeSoft Corporation, an indirect, wholly-owned subsidiary of PerkinElmer, Inc. All rights reserved.
-- This script creates roles and privileges for Datalytix

-- NOTE THIS SCRIPT MUST BE RUN FROM THE COMMAND LINE VERSION OF SQLPLUS
-- This script will not run from SQLPlus Worksheet

Connect &&securitySchemaName/&&securitySchemaPass@&&serverName

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
	DL_CAN_MANAGE_ALL_QUERIES NUMBER(1,0) null,
	constraint &&privTableName._PK primary key (ROLE_INTERNAL_ID) ) 
;
	
--#########################################################
--CREATE ROLES
--#########################################################


Connect &&InstallUser/&&sysPass@&&serverName

--CREATE_MASTER_ROLES
--&&UserRole

DECLARE
	n NUMBER;
BEGIN
	select count(*) into n from dba_roles where role = Upper('&&UserRole');
	if n = 1 then
		execute immediate '
			DROP ROLE &&UserRole';
	end if;
END;				
/

	--CREATE ROLE &&UserRole NOT IDENTIFIED;
	--GRANT CSS_USER TO "&&UserRole";
	--GRANT "CONNECT" TO "&&UserRole";
	

--&&AdminRole
DECLARE
	n NUMBER;
BEGIN
	select count(*) into n from dba_roles where role = Upper('&&AdminRole');
	if n = 1 then
		execute immediate '
			DROP ROLE &&AdminRole';
	end if;
END;
/

	CREATE ROLE &&AdminRole NOT IDENTIFIED;
	GRANT CSS_USER TO "&&AdminRole";
	GRANT "CONNECT" TO "&&AdminRole";

	
Connect &&securitySchemaName/&&securitySchemaPass@&&serverName

delete from security_roles where ROLE_NAME Like '&&UserRole';
delete from security_roles where ROLE_NAME Like '&&AdminRole';
commit;

--PRIVELEGE_TABLES
delete  from privilege_tables where APP_NAME = '&&AppName';
commit;

INSERT INTO PRIVILEGE_TABLES (PRIVILEGE_TABLE_NAME,APP_NAME,TABLE_SPACE) values('&&privTableName','&&AppName','NONE');

--&&UserRole
--INSERT INTO SECURITY_ROLES (ROLE_ID,PRIVILEGE_TABLE_INT_ID,ROLE_NAME, COEIDENTIFIER)values(SECURITY_ROLES_SEQ.NEXTVAL,PRIVILEGE_TABLES_seq.CURRVAL,'&&UserRole', '&&COEIdentifier');
--INSERT INTO &&privTableName(ROLE_INTERNAL_ID,DL_CAN_MANAGE_ALL_QUERIES) VALUES (SECURITY_ROLES_SEQ.CURRVAL, '0');

--&&AdminRole
INSERT INTO SECURITY_ROLES (ROLE_ID,PRIVILEGE_TABLE_INT_ID,ROLE_NAME, COEIDENTIFIER)values(SECURITY_ROLES_SEQ.NEXTVAL,PRIVILEGE_TABLES_seq.CURRVAL,'&&AdminRole', '&&COEIdentifier');
INSERT INTO &&privTableName(ROLE_INTERNAL_ID,DL_CAN_MANAGE_ALL_QUERIES) VALUES (SECURITY_ROLES_SEQ.CURRVAL, '1');
Commit;


-- No Object privileges required for ChemVioBiz
--DELETE FROM &&securitySchemaName..OBJECT_PRIVILEGES WHERE Schema = Upper('&&SchemaName');
-- BROWSE_ACX PRIVS
--INSERT INTO CS_SECURITY.OBJECT_PRIVILEGES VALUES ('BROWSE_ACX', 'SELECT', '&&SchemaName', 'SUBSTANCE');
--Connect &&schemaName/&&schemaPass@&&serverName
-- Grant all object permissions to CS_SECURITY
--GRANT SELECT ON SUBSTANCE TO CS_SECURITY WITH GRANT OPTION;



CONNECT &&InstallUser/&&syspass@&&ServerName


prompt 'dropping test users...'

DECLARE
	PROCEDURE dropUser(uName IN varchar2) IS
			n NUMBER;
		BEGIN
			select count(*) into n from dba_users where Upper(username) = uName;
			if n > 0 then
				execute immediate 'DROP USER ' || uName;
			end if;
		END dropUser;
BEGIN
	dropUser('&&AdminTestUSer');
	dropUser('&&UserTestUser');
end;
/

--CREATE USER &&UserTestUser IDENTIFIED BY &&UserTestUser DEFAULT TABLESPACE &&securityTableSpaceName temporary tablespace &&tempTableSpaceName PROFILE csuserprofile ACCOUNT UNLOCK;
--GRANT &&UserRole TO &&UserTestUser ;
--ALTER USER &&UserTestUser DEFAULT ROLE ALL;

CREATE USER &&AdminTestUSer IDENTIFIED BY &&AdminTestUSer DEFAULT TABLESPACE &&securityTableSpaceName temporary tablespace &&tempTableSpaceName PROFILE csuserprofile ACCOUNT UNLOCK;
GRANT &&AdminRole TO &&AdminTestUSer;
ALTER USER &&AdminTestUSer DEFAULT ROLE ALL;

Connect &&securitySchemaName/&&securitySchemaPass@&&serverName

Delete from people where user_id = '&&UserTestUser';
Delete from people where user_id = '&&AdminTestUSer';
--Insert into people(person_id, user_id, user_code, supervisor_internal_id, last_name,site_id,active)values(PEOPLE_SEQ.NEXTVAL,'&&UserTestUser','&&UserTestUser',Null,'&&UserTestUser',Null,'1');
Insert into people(person_id, user_id, user_code, supervisor_internal_id, last_name,site_id,active)values(PEOPLE_SEQ.NEXTVAL,'&&AdminTestUSer','&&AdminTestUSer',Null,'&&AdminTestUSer',Null,'1');
commit;

