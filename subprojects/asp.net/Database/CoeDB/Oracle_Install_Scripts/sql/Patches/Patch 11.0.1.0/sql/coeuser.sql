--Copyright 1999-2010 CambridgeSoft Corporation. All rights reserved

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

GRANT CREATE SESSION TO COEUSER;


                      




 



