--
--this script sets up test users for BioSar Browser. If assumes that CHEM_REG, MIX_REG, BIOAssay Manager and ChemINV are isntalled. 
--If they are not installed you will see errors in the script indicating roles do not exist. This is OKAY.
--Do not be concerned about these script erros.
--
Connect &&InstallUser/&&sysPass@&&serverName

prompt 'dropping test users...'

DECLARE
	PROCEDURE dropUser(uName IN varchar2) IS
			n NUMBER;
		BEGIN
			select count(*) into n from dba_users where username = uName;
			if n =1 then
				execute immediate 'DROP USER ' || uName;
			end if;
		END dropUser;
BEGIN
	dropUser('BIOSAR_ADMIN');
	dropUser('BIOSAR_USER_ADMIN');
	dropUser('BIOSAR_USER');
	dropUser('BIOSAR_USER_BROWSER');
end;
/

prompt 'creating test users...'


--This user has rights to to BioSar Browser and ChemReg in addition to the BioSar Browser roles


CREATE USER BIOSAR_ADMIN IDENTIFIED BY BIOSAR_ADMIN default tablespace &&tableSpaceName temporary tablespace &&tempTableSpaceName;
Grant BIOSAR_BROWSER_ADMIN to BIOSAR_ADMIN;
Grant BROWSER to BIOSAR_ADMIN;
Grant INV_Browser to BIOSAR_ADMIN;
--Grant HTS_ADMIN to BIOSAR_ADMIN;
ALTER USER BIOSAR_ADMIN DEFAULT ROLE ALL;


CREATE USER BIOSAR_USER_ADMIN IDENTIFIED BY BIOSAR_USER_ADMIN default tablespace &&tableSpaceName temporary tablespace &&tempTableSpaceName;
GRANT BIOSAR_BROWSER_USER_ADMIN TO BIOSAR_USER_ADMIN;
GRANT BROWSER TO BIOSAR_USER_ADMIN;
Grant INV_Browser to BIOSAR_USER_ADMIN;
--Grant HTS_BROWSER to BIOSAR_USER_ADMIN;
ALTER USER BIOSAR_USER_ADMIN DEFAULT ROLE ALL;


CREATE USER BIOSAR_USER IDENTIFIED BY BIOSAR_USER default tablespace &&tableSpaceName temporary tablespace &&tempTableSpaceName;
GRANT BIOSAR_BROWSER_USER TO BIOSAR_USER;
GRANT BROWSER TO BIOSAR_USER;
--Grant HTS_BROWSER to BIOSAR_USER;
ALTER USER BIOSAR_USER DEFAULT ROLE ALL;
 

CREATE USER BIOSAR_USER_BROWSER IDENTIFIED BY BIOSAR_USER_BROWSER default tablespace &&tableSpaceName temporary tablespace &&tempTableSpaceName;
GRANT BIOSAR_BROWSER_USER_BROWSER TO BIOSAR_USER_BROWSER;
GRANT BROWSER TO BIOSAR_USER_BROWSER;
--Grant HTS_BROWSER to BIOSAR_USER_BROWSER;
ALTER USER BIOSAR_USER_BROWSER DEFAULT ROLE ALL;

Connect &&securitySchemaName/&&securitySchemaPass@&&serverName

Delete from people where user_id = 'BIOSAR_ADMIN';
Delete from people where user_id = 'BIOSAR_USER_ADMIN';
Delete from people where user_id = 'BIOSAR_USER';
Delete from people where user_id = 'BIOSAR_USER_BROWSER';
Insert into people
  (person_id, 
   user_id, 
   user_code, 
   supervisor_internal_id, 
   last_name,
   site_id,
   active,
   email)
values
  (PEOPLE_SEQ.NEXTVAL,
   'BIOSAR_ADMIN',
   '',
   '4',
   'BIOSAR_ADMIN',
   '1',
   '1',
   '');

Insert into people
  (person_id, 
   user_id, 
   user_code, 
   supervisor_internal_id, 
   last_name,
   site_id,
   active,
   email)
values
  (PEOPLE_SEQ.NEXTVAL,
   'BIOSAR_USER_ADMIN',
   '',
   '4',
   'BIOSAR_USER_ADMIN',
   '1',
   '1',
   '');

Insert into people
  (person_id, 
   user_id, 
   user_code, 
   supervisor_internal_id, 
   last_name,
   site_id,
   active,
   email)
values
  (PEOPLE_SEQ.NEXTVAL,
   'BIOSAR_USER',
   '',
   '4',
   'BIOSAR_USER',
   '1',
   '1',
   '');

Insert into people
  (person_id, 
   user_id, 
   user_code, 
   supervisor_internal_id, 
   last_name,
   site_id,
   active,
   email)
values
  (PEOPLE_SEQ.NEXTVAL,
   'BIOSAR_USER_BROWSER',
   '',
   '4',
   'BIOSAR_USER_BROWSER',
   '1',
   '1',
   '');

 
commit;
