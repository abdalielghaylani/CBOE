--#########################################################
--CREATE ROLES
--#########################################################

--ROLE creation script for ChemACX/Oracle version Copyright Cambridgesoft corp 1999-2000 all rights reserved

prompt 'Creating ACX roles...'

Connect &&InstallUser/&&sysPass@&&serverName

--CREATE_MASTER_ROLES
--ACX_BROWSER
DECLARE
	n NUMBER;
BEGIN
	select count(*) into n from dba_roles where role = Upper('ACX_BROWSER');
	if n = 1 then
		execute immediate '
			DROP ROLE ACX_BROWSER';
	end if;
END;				
/

	CREATE ROLE ACX_BROWSER NOT IDENTIFIED;
	REVOKE "ACX_BROWSER" FROM "SYSTEM";
--ACX_BUYER
DECLARE
	n NUMBER;
BEGIN
	select count(*) into n from dba_roles where role = Upper('ACX_BUYER');
	if n = 1 then
		execute immediate '
			DROP ROLE ACX_BUYER';
	end if;
END;
/

	CREATE ROLE ACX_BUYER NOT IDENTIFIED;
	REVOKE "ACX_BUYER" FROM "SYSTEM";

	GRANT "CONNECT" TO "ACX_BROWSER";
	GRANT "CONNECT" TO "ACX_BUYER";
	GRANT ACX_BROWSER TO "ACX_BUYER";


--create test users Copyright Cambridgesoft corp 1999-2000 all rights reserved

prompt 'Creating ACX users...'

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
