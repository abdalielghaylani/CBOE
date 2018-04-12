--Copyright 1999-2003 CambridgeSoft Corporation. All rights reserved
-- NOTE THIS SCRIPT MUST BE RUN FROM THE COMMAND LINE VERSION OF SQLPLUS
-- This script will not run from SQLPlus Worksheet!


-- Assumes that CS_SECURITY schema exists.
-- These system privileges are required so that CS_SECURITY can manage users and roles via dbms_sql calls
Connect &&InstallUser/&&sysPass@&&serverName


BEGIN
   IF &&OraVersionNumber = 9 THEN
		EXECUTE IMMEDIATE 'GRANT SELECT ANY DICTIONARY TO &&schemaName';
   END IF;
END;   		
/

grant alter user to cs_security;
grant create user to cs_security;
grant drop user to cs_security;
grant alter any role to cs_security;
grant create role to cs_security;
grant drop any role to cs_security;
grant grant any role to cs_security;
grant grant any privilege to cs_security;
grant connect to cs_security with admin option;
grant select any table to cs_security;


--CREATE_MASTER_ROLES
--CSS_USER
	CREATE ROLE CSS_USER NOT IDENTIFIED;
	REVOKE "CSS_USER" FROM "SYSTEM";
	GRANT "CONNECT" TO "CSS_USER";
--CSS_ADMIN
	CREATE ROLE CSS_ADMIN NOT IDENTIFIED;
	REVOKE "CSS_ADMIN" FROM "SYSTEM";
	GRANT "CONNECT" TO "CSS_ADMIN";


Connect &&schemaName/&&schemaPass@&&serverName

ALTER TABLE PRIVILEGE_TABLES
ADD CONSTRAINT APP_NAME_U
UNIQUE(APP_NAME);

-- Correct active flag in legacy people table
UPDATE people SET Active = -1 WHERE Active = 0;
UPDATE people SET Active = 1 WHERE Active = -1;
UPDATE people SET Active = -1 WHERE Active IS NULL;


CREATE TABLE CS_SECURITY_PRIVILEGES(
	ROLE_INTERNAL_ID NUMBER(8) NOT NULL,
	CSS_LOGIN NUMBER(1), 
	CSS_CREATE_USER NUMBER(1), 
	CSS_EDIT_USER NUMBER(1), 
	CSS_DELETE_USER NUMBER(1),
	CSS_CHANGE_PASSWORD NUMBER(1), 
	CSS_CREATE_ROLE NUMBER(1), 
	CSS_EDIT_ROLE NUMBER(1), 
	CSS_DELETE_ROLE NUMBER(1), 
	CSS_CREATE_WORKGRP NUMBER(1), 
	CSS_EDIT_WORKGRP NUMBER(1), 
	CSS_DELETE_WORKGRP NUMBER(1),
	"RID" NUMBER(10) NOT NULL, 
	"CREATOR" VARCHAR2(30) DEFAULT RTRIM(user) NOT NULL, 
	"TIMESTAMP" DATE DEFAULT sysdate NOT NULL,   
	CONSTRAINT CS_SECURITY_PRIVILEGES_PK 
		primary key (ROLE_INTERNAL_ID) USING INDEX TABLESPACE &&indexTableSpaceName 
	)
; 

-- Audit Trail Triggers
@@sql\triggers\cs_security_privileges_ad0.trg;
@@sql\triggers\cs_security_privileges_au0.trg;
@@sql\triggers\cs_security_privileges_bi0.trg;

CREATE TABLE OBJECT_PRIVILEGES(
	PRIVILEGE_NAME VARCHAR2(30) NOT NULL, 
	PRIVILEGE VARCHAR2(10) NOT NULL, 
	SCHEMA VARCHAR2(30) NOT NULL, 
	OBJECT_NAME VARCHAR2(30) NOT NULL
	)
; 

ALTER TABLE OBJECT_PRIVILEGES 
	ADD CONSTRAINT OBJECT_PRIV_U
	UNIQUE(PRIVILEGE_NAME, PRIVILEGE, SCHEMA, OBJECT_NAME);

delete from privilege_tables where TABLE_SPACE = 'T_CS_SECURITY';

INSERT INTO PRIVILEGE_TABLES (PRIVILEGE_TABLE_NAME,APP_NAME,TABLE_SPACE) values('CS_SECURITY_PRIVILEGES','CS Security','&&tableSpaceName');

--CSS_ADMIN
INSERT INTO SECURITY_ROLES (PRIVILEGE_TABLE_INT_ID,ROLE_NAME)values(PRIVILEGE_TABLES_seq.CURRVAL,'CSS_ADMIN');

INSERT INTO CS_SECURITY_PRIVILEGES (ROLE_INTERNAL_ID			,CSS_LOGIN	,CSS_CREATE_USER	,CSS_EDIT_USER	,CSS_DELETE_USER	,CSS_CHANGE_PASSWORD	,CSS_CREATE_ROLE	,CSS_EDIT_ROLE	,CSS_DELETE_ROLE	,CSS_CREATE_WORKGRP		,CSS_EDIT_WORKGRP	,CSS_DELETE_WORKGRP	)
							VALUES (SECURITY_ROLES_SEQ.CURRVAL	,1			,1					,1				,1					,0						,1					,1				,1					,1						,1					,1					);
--CSS_USER
INSERT INTO SECURITY_ROLES (PRIVILEGE_TABLE_INT_ID,ROLE_NAME)values(PRIVILEGE_TABLES_seq.CURRVAL,'CSS_USER');

INSERT INTO CS_SECURITY_PRIVILEGES (ROLE_INTERNAL_ID			,CSS_LOGIN	,CSS_CREATE_USER	,CSS_EDIT_USER	,CSS_DELETE_USER	,CSS_CHANGE_PASSWORD	,CSS_CREATE_ROLE	,CSS_EDIT_ROLE	,CSS_DELETE_ROLE	,CSS_CREATE_WORKGRP		,CSS_EDIT_WORKGRP	,CSS_DELETE_WORKGRP	)
							VALUES (SECURITY_ROLES_SEQ.CURRVAL	,1			,0					,0				,0					,1						,0					,0				,0					,0						,0					,0					);
commit;


-- Create user and role management packages is cs_Security
@@pkg_Users_def.sql;
@@pkg_Users_body.sql;
@@pkg_Roles_def.sql;
@@pkg_Roles_body.sql;
@@pkg_Login_def.sql;
@@pkg_Login_body.sql;

-- Create Stored Procedures and Functions
@@CREATE_cs_security_Stored_Procedures.sql

@@Map_CSPrivilge_To_Oracle_Privilege_CS_Security.sql

-- 
GRANT execute on MapPrivsToRole to Public;


--Grant to CSS_ADMIN
GRANT execute on login to CSS_ADMIN;
GRANT execute on Manage_Users to CSS_ADMIN;
GRANT execute on Manage_Roles to CSS_ADMIN;
GRANT select on privilege_tables to CSS_ADMIN;
GRANT select on people to CSS_ADMIN;
GRANT select on sites to CSS_ADMIN;
grant execute on createuser to CSS_ADMIN;
grant execute on updateuser to CSS_ADMIN;
grant execute on deleteuser to CSS_ADMIN;
grant execute on createrole to CSS_ADMIN;
grant execute on updaterole to CSS_ADMIN;
grant execute on deleterole to CSS_ADMIN;

--Grant to CSS_USER
GRANT execute on login to CSS_USER;
GRANT execute on Manage_Roles to CSS_USER;
GRANT execute on Manage_Users to CSS_USER;
GRANT execute on changepwd to CSS_USER;
GRANT select on privilege_tables to CSS_USER;
GRANT select on people to CSS_USER;
GRANT select on sites to CSS_USER;


CREATE USER CSSUSER IDENTIFIED BY CSSUSER DEFAULT TABLESPACE &&tableSpaceName TEMPORARY TABLESPACE &&tempTableSpaceName PROFILE DEFAULT ACCOUNT UNLOCK;
GRANT CSS_USER TO CSSUSER;
ALTER USER CSSUSER DEFAULT ROLE ALL;
Delete from people where user_id = 'CSSUSER';
Insert into people(user_id, user_code, supervisor_internal_id, last_name,site_id,active)values('CSSUSER','CSSUSER','1','CSSUSER','1','1');


CREATE USER CSSADMIN IDENTIFIED BY CSSADMIN DEFAULT TABLESPACE &&tableSpaceName TEMPORARY TABLESPACE &&tempTableSpaceName PROFILE DEFAULT ACCOUNT UNLOCK;
GRANT CSS_ADMIN TO CSSADMIN;
ALTER USER CSSADMIN DEFAULT ROLE ALL;
Delete from people where user_id = 'CSSADMIN';
Insert into people(user_id, user_code, supervisor_internal_id, last_name,site_id,active)values('CSSADMIN','CSSADMIN','1','CSSADMIN','1','1');

commit;


