--#########################################################
--ADD TESTING USERS
--#########################################################

Connect &&InstallUser/&&sysPass@&&serverName

prompt Dropping test users...

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
	dropUser('DOC_BROWSER');
	dropUser('DOC_SUBMITTER');
	dropUser('DOC_ADMIN');
end;
/


CREATE USER DOC_BROWSER IDENTIFIED BY DOC_BROWSER DEFAULT TABLESPACE &&tableSpaceName TEMPORARY TABLESPACE &&tempTableSpaceName PROFILE DEFAULT ACCOUNT UNLOCK;
GRANT DOCMGR_BROWSER TO DOC_BROWSER;
ALTER USER DOC_BROWSER DEFAULT ROLE ALL;


CREATE USER DOC_SUBMITTER IDENTIFIED BY DOC_SUBMITTER DEFAULT TABLESPACE &&tableSpaceName TEMPORARY TABLESPACE &&tempTableSpaceName PROFILE DEFAULT ACCOUNT UNLOCK;
GRANT DOCMGR_SUBMITTER TO DOC_SUBMITTER;
ALTER USER DOC_SUBMITTER DEFAULT ROLE ALL;

CREATE USER DOC_ADMIN IDENTIFIED BY DOC_ADMIN DEFAULT TABLESPACE &&tableSpaceName TEMPORARY TABLESPACE &&tempTableSpaceName PROFILE DEFAULT ACCOUNT UNLOCK;
GRANT DOCMGR_ADMINISTRATOR  TO DOC_ADMIN;
ALTER USER DOC_ADMIN DEFAULT ROLE ALL;


Connect &&securitySchemaName/&&securitySchemaPass@&&serverName

DELETE FROM PEOPLE WHERE USER_ID = 'DOC_BROWSER';
DELETE FROM PEOPLE WHERE USER_ID = 'DOC_SUBMITTER';
DELETE FROM PEOPLE WHERE USER_ID = 'DOC_ADMIN';

INSERT INTO PEOPLE(user_id, user_code, supervisor_internal_id, first_name, last_name, email, site_id, active) VALUES('DOC_BROWSER', '', '5', 'Browser', 'DOC_BROWSER', '', '1', '1');
INSERT INTO PEOPLE(user_id, user_code, supervisor_internal_id, first_name, last_name, email, site_id, active) VALUES('DOC_SUBMITTER', '', '5', 'Submitter', 'DOC_SUBMITTER', '', '1', '1');
INSERT INTO PEOPLE(user_id, user_code, supervisor_internal_id, first_name, last_name, email, site_id, active) VALUES('DOC_ADMIN', '', '5', 'Administrator', 'DOC_ADMIN', '', '1', '1');
