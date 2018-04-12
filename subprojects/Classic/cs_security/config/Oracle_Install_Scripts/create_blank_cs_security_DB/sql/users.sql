--Copyright 1999-2003 CambridgeSoft Corporation. All rights reserved


prompt starting users.sql

DECLARE
	n NUMBER;
BEGIN
	select count(*) into n from dba_users where username = Upper('&&schemaName');
	if n > 0 then
		execute immediate 'DROP USER &&schemaName CASCADE';
	end if;
END;
/

CREATE USER &&schemaName
	IDENTIFIED BY ORACLE
	DEFAULT TABLESPACE &&tableSpaceName
	TEMPORARY TABLESPACE &&tempTableSpaceName;

GRANT CONNECT, RESOURCE TO  &&schemaName;




 



