-- Copyright Cambridgesoft Corp 2001-2006 all rights reserved

--#########################################################
-- Drop all schema owner, test users, and test roles
--######################################################### 

Connect &&InstallUser/&&sysPass@&&serverName

prompt '#########################################################'
prompt 'dropping schema owner...'
prompt '#########################################################'


DECLARE
	n NUMBER;
BEGIN
	select count(*) into n from dba_users where username = '&&schemaName';
	if n = 1 then
		execute immediate 'DROP USER &&schemaName CASCADE';
	end if;
END;
/

prompt '#########################################################'
prompt 'dropping default users...'
prompt '#########################################################'

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
	dropUser('INVREGISTRAR');
	dropUser('INVRECEIVING');
	dropUser('INVBROWSER');
	dropUser('INVFINANCE');
end;
/

prompt '#########################################################'
prompt 'dropping default roles...'
prompt '#########################################################'

DECLARE
	PROCEDURE dropRole(roleName IN varchar2) IS
			n NUMBER;
		BEGIN
			select count(*) into n from dba_roles where Upper(role) = roleName;
			if n > 0 then
				execute immediate 'DROP ROLE ' || roleName;
			end if;
		END dropRole;
BEGIN
	dropRole('INV_ADMIN');
	dropRole('INV_CHEMIST');
	dropRole('INV_REGISTRAR');
	dropRole('INV_RECEIVING');
	dropRole('INV_BROWSER');
	dropRole('INV_FINANCE');
END;
/
show errors;

