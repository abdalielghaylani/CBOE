-- Copyright 1998-2014 CambridgeSoft Corporation, an indirect, wholly-owned subsidiary of PerkinElmer, Inc. All rights reserved.

PROMPT Starting Instance_users.sql

CONNECT &&InstallUser/&&sysPass@&&serverName &&AsSysDBA

SET serveroutput on

DECLARE
	n		NUMBER;
	mySql   VARCHAR2 (300);
BEGIN
	select count(*) into n from dba_users where username = Upper('&&globalSchemaName');
	IF n > 0 THEN
		DBMS_OUTPUT.PUT_LINE('global user already exists, skip the creation');
                execute immediate 'alter user &&globalSchemaName identified by &&globalSchemaPass';
	ELSE
		mySql := 'CREATE USER '|| '&&globalSchemaName' || ' IDENTIFIED BY ' || '&&globalSchemaPass' ||
			' DEFAULT TABLESPACE ' || '&&tableSpaceName' || ' TEMPORARY TABLESPACE ' || '&&temptableSpaceName';
		DBMS_OUTPUT.PUT_LINE('executing:' || mySql);		
		execute immediate mySql;		
	END IF;
END;
/
