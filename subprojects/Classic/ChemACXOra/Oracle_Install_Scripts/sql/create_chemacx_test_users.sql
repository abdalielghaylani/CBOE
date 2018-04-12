--create test users Copyright Cambridgesoft corp 1999-2000 all rights reserved

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
	dropUser('ACXBUYER');
	dropUser('ACXBROWSER');
end;
/

CREATE USER ACXBROWSER IDENTIFIED BY ACXBROWSER DEFAULT TABLESPACE &&tableSpaceName PROFILE DEFAULT ACCOUNT UNLOCK;
GRANT ACX_BROWSER TO ACXBROWSER ;
ALTER USER ACXBROWSER DEFAULT ROLE ALL;

CREATE USER ACXBUYER IDENTIFIED BY ACXBUYER DEFAULT TABLESPACE &&tableSpaceName PROFILE DEFAULT ACCOUNT UNLOCK;
GRANT ACX_BUYER TO ACXBUYER;
ALTER USER ACXBUYER DEFAULT ROLE ALL;


Connect cs_security/oracle@&&ServerName
Delete from people where user_id = 'ACXBROWSER';
Delete from people where user_id = 'ACXBUYER';
Insert into people(person_id, user_id, user_code, supervisor_internal_id, last_name,site_id,active)values(PEOPLE_SEQ.NEXTVAL,'ACXBROWSER','BRW','1','ACXBROWSER','1','1');
Insert into people(person_id, user_id, user_code, supervisor_internal_id, last_name,site_id,active)values(PEOPLE_SEQ.NEXTVAL,'ACXBUYER','BUY','1','ACXBROWSER','1','1');
commit;
