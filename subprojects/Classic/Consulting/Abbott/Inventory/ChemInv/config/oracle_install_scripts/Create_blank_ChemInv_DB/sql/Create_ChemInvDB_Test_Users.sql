-- Copyright Cambridgesoft Corp 2001-2005 all rights reserved

-- NOTE THIS SCRIPT MUST BE RUN FROM THE COMMAND LINE VERSION OF SQLPLUS
-- This script will not run from SQLPlus Worksheet!

prompt 'dropping test users...'

connect &&InstallUser/&&sysPass@&&serverName

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
	dropUser('INVADMIN');
	dropUser('INVCHEMIST');
	dropUser('INVBROWSER');
	dropUser('INVFINANCE');
	dropUser('INVREGISTRAR');
	dropUser('INVRECEIVING');
END;
/


prompt 'creating test users...'

CREATE USER INVBROWSER IDENTIFIED BY INVBROWSER DEFAULT TABLESPACE &&tableSpaceName TEMPORARY TABLESPACE &&tempTableSpaceName PROFILE DEFAULT ACCOUNT UNLOCK;
GRANT INV_BROWSER TO INVBROWSER ;
ALTER USER INVBROWSER DEFAULT ROLE ALL;

CREATE USER INVCHEMIST IDENTIFIED BY INVCHEMIST DEFAULT TABLESPACE &&tableSpaceName TEMPORARY TABLESPACE &&tempTableSpaceName PROFILE DEFAULT ACCOUNT UNLOCK;
GRANT INV_CHEMIST TO INVCHEMIST ;
ALTER USER INVCHEMIST DEFAULT ROLE ALL;

CREATE USER INVRECEIVING IDENTIFIED BY INVRECEIVING DEFAULT TABLESPACE &&tableSpaceName TEMPORARY TABLESPACE &&tempTableSpaceName PROFILE DEFAULT ACCOUNT UNLOCK;
GRANT INV_RECEIVING TO INVRECEIVING ;
ALTER USER INVRECEIVING DEFAULT ROLE ALL;

CREATE USER INVFINANCE IDENTIFIED BY INVFINANCE DEFAULT TABLESPACE &&tableSpaceName TEMPORARY TABLESPACE &&tempTableSpaceName PROFILE DEFAULT ACCOUNT UNLOCK;
GRANT INV_FINANCE TO INVFINANCE ;
ALTER USER INVFINANCE DEFAULT ROLE ALL;

CREATE USER INVREGISTRAR IDENTIFIED BY INVREGISTRAR DEFAULT TABLESPACE &&tableSpaceName TEMPORARY TABLESPACE &&tempTableSpaceName PROFILE DEFAULT ACCOUNT UNLOCK;
GRANT INV_REGISTRAR TO INVREGISTRAR ;
ALTER USER INVREGISTRAR DEFAULT ROLE ALL;

CREATE USER INVADMIN IDENTIFIED BY INVADMIN DEFAULT TABLESPACE &&tableSpaceName TEMPORARY TABLESPACE &&tempTableSpaceName PROFILE DEFAULT ACCOUNT UNLOCK;
GRANT INV_ADMIN TO INVADMIN;
ALTER USER INVADMIN DEFAULT ROLE ALL;


connect &&securitySchemaName/&&securitySchemaPass@&&serverName;
Delete from people where user_id = 'INVBROWSER';
Delete from people where user_id = 'INVCHEMIST';
Delete from people where user_id = 'INVRECEIVING';
Delete from people where user_id = 'INVFINANCE';
Delete from people where user_id = 'INVREGISTRAR';
Delete from people where user_id = 'INVADMIN';
DELETE FROM people where user_id = 'UNKNOWN';
Insert into people(person_id, user_id, user_code, supervisor_internal_id, last_name,site_id,active)values(PEOPLE_SEQ.NEXTVAL,'INVBROWSER','INVBRW','1','INVBROWSER','1','1');
Insert into people(person_id, user_id, user_code, supervisor_internal_id, last_name,site_id,active)values(PEOPLE_SEQ.NEXTVAL,'INVCHEMIST','INVCHM','1','INVCHEMIST','1','1');
Insert into people(person_id, user_id, user_code, supervisor_internal_id, last_name,site_id,active)values(PEOPLE_SEQ.NEXTVAL,'INVRECEIVING','INVRCV','1','INVRECEIVING','1','1');
Insert into people(person_id, user_id, user_code, supervisor_internal_id, last_name,site_id,active)values(PEOPLE_SEQ.NEXTVAL,'INVFINANCE','INVFIN','1','INVFINANCE','1','1');
Insert into people(person_id, user_id, user_code, supervisor_internal_id, last_name,site_id,active)values(PEOPLE_SEQ.NEXTVAL,'INVREGISTRAR','INVREG','1','INVREGISTRAR','1','1');
Insert into people(person_id, user_id, user_code, supervisor_internal_id, last_name,site_id,active)values(PEOPLE_SEQ.NEXTVAL,'INVADMIN','INVADMN','1','INVADMIN','1','1');
--this row is for the application and is not a test user
INSERT INTO people(person_id, user_id, user_code, supervisor_internal_id, last_name, site_id, active) VALUES (PEOPLE_SEQ.NEXTVAL,'UNKNOWN','UNKNOWN','1','UNKNOWN','1','0');
commit;
