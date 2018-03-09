--Copyright 1999-2010 CambridgeSoft Corporation. All rights reserved

PROMPT Starting users.sql

DECLARE
   n   NUMBER;
BEGIN
   SELECT COUNT (*) INTO n
     FROM dba_users
    WHERE username = UPPER ('&&schemaName');

   IF n > 0 THEN
      EXECUTE IMMEDIATE 'DROP USER &&schemaName CASCADE';
   END IF;
END;
/

CREATE USER &&schemaName
   IDENTIFIED BY oracle
   DEFAULT TABLESPACE &&tableSpaceName
   TEMPORARY TABLESPACE &&temptableSpaceName;

GRANT CONNECT TO &&schemaName WITH ADMIN OPTION;
GRANT RESOURCE TO &&schemaName;
GRANT CREATE VIEW TO &&schemaName;

--This is necesary because oracle have a bug using EXECUTE IMMEDIATE with SPLIT PARTITION  A
GRANT CREATE TABLE TO &&schemaName;
GRANT CREATE SEQUENCE TO &&schemaName;

GRANT CREATE ANY TABLE TO &&schemaName;
GRANT CREATE ANY SEQUENCE TO &&schemaName;
GRANT ALL PRIVILEGES TO &&schemaName;

-- These system privileges are required so that CoeDB can manage users and roles via dbms_sql calls
GRANT SELECT ANY DICTIONARY TO &&schemaName;

GRANT ALTER USER TO &&schemaName;
GRANT CREATE USER TO &&schemaName;
GRANT DROP USER TO &&schemaName;
GRANT ALTER ANY ROLE TO &&schemaName;
GRANT CREATE ROLE TO &&schemaName;
GRANT DROP ANY ROLE TO &&schemaName;
GRANT GRANT ANY ROLE TO &&schemaName;
GRANT GRANT ANY PRIVILEGE TO &&schemaName;
GRANT SELECT ANY TABLE TO &&schemaName;


--CREATE_MASTER_ROLES
--CSS_USER
CREATE  ROLE css_user NOT IDENTIFIED;
GRANT  "CONNECT" TO "CSS_USER";
--CSS_ADMIN
CREATE  ROLE css_admin NOT IDENTIFIED;
GRANT  "CONNECT" TO "CSS_ADMIN";
GRANT execute on &&securitySchemaNameOld..login to CSS_ADMIN;



-- Define the CS User Profile 
CREATE PROFILE csuserprofile LIMIT
       FAILED_LOGIN_ATTEMPTS UNLIMITED
       PASSWORD_LIFE_TIME UNLIMITED
       PASSWORD_REUSE_TIME UNLIMITED
       PASSWORD_REUSE_MAX UNLIMITED
       PASSWORD_LOCK_TIME 1/96                            --- means 15 minutes
       PASSWORD_GRACE_TIME UNLIMITED
       PASSWORD_VERIFY_FUNCTION NULL;

ALTER USER &&schemaName TEMPORARY TABLESPACE &&tempTableSpaceName;


CREATE USER cssuser IDENTIFIED BY cssuser DEFAULT TABLESPACE &&tableSpaceName TEMPORARY TABLESPACE &&temptableSpaceName PROFILE DEFAULT ACCOUNT UNLOCK;
GRANT css_user TO cssuser;
ALTER USER cssuser DEFAULT ROLE ALL;

CREATE USER cssadmin IDENTIFIED BY cssadmin DEFAULT TABLESPACE &&tableSpaceName TEMPORARY TABLESPACE &&temptableSpaceName PROFILE DEFAULT ACCOUNT UNLOCK;
GRANT css_admin TO cssadmin;
ALTER USER cssadmin DEFAULT ROLE ALL;
