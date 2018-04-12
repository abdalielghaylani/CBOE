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

-- Define the CS User Profile 
CREATE PROFILE csuserprofile LIMIT
       FAILED_LOGIN_ATTEMPTS UNLIMITED
       PASSWORD_LIFE_TIME UNLIMITED
       PASSWORD_REUSE_TIME UNLIMITED 
       PASSWORD_REUSE_MAX UNLIMITED
       PASSWORD_LOCK_TIME 1/96    --- means 15 minutes
       PASSWORD_GRACE_TIME UNLIMITED
       PASSWORD_VERIFY_FUNCTION NULL
;        

alter user &&schemaName temporary tablespace &&tempTableSpaceName;
Connect &&schemaName/&&schemaPass@&&serverName

ALTER TABLE PEOPLE
ADD CONSTRAINT PEOPLE_USER_ID_UK
UNIQUE(USER_ID);

GRANT execute on changepwd to CSS_ADMIN;
UPDATE cs_security_privileges set CSS_CHANGE_PASSWORD = 1 where role_internal_id = 
	(select role_id from security_roles where role_name = 'CSS_ADMIN');
commit;	


-- Create user and role management packages is cs_Security
@@pkg_users_def.sql;
@@pkg_users_body.sql;
@@pkg_roles_def.sql;
@@pkg_roles_body.sql;
@@pkg_login_def.sql;
@@pkg_login_body.sql;

-- Create Stored Procedures and Functions
@@create_cs_security_stored_procedures.sql




