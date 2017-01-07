--Copyright 1999-2008 CambridgeSoft Corporation. All rights reserved


prompt Starting coeuser.sql

DECLARE
	n NUMBER;
BEGIN
	select count(*) into n from dba_users where username = Upper('COEUSER');
	if n > 0 then
		execute immediate 'DROP USER COEUSER CASCADE';
	end if;
END;
/

CREATE USER COEUSER
	IDENTIFIED BY ORACLE
	DEFAULT TABLESPACE &&tableSpaceName
	TEMPORARY TABLESPACE &&tempTableSpaceName;

-- HitLIst table need space to insert new data
GRANT UNLIMITED TABLESPACE TO COEUSER;
GRANT ALTER USER TO COEUSER;
-- To create child hitlist SEQUENCE under instance user schema
--GRANT CREATE SEQUENCE TO COEUSER;
GRANT CREATE SESSION TO COEUSER;
-- To create hitlist tables under instance user schema
--GRANT CREATE TABLE TO COEUSER;
GRANT SELECT ANY DICTIONARY TO COEUSER;
GRANT SELECT ANY TABLE TO COEUSER;

--Below privileges are only required by COEDB
GRANT DELETE ANY TABLE TO COEUSER;
GRANT DROP ANY ROLE TO COEUSER;
GRANT DELETE ANY TABLE TO COEUSER;


                      




 



