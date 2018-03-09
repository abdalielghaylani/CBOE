--Copyright 1999-2003 CambridgeSoft Corporation. All rights reserved
prompt 
prompt Starting "Users.sql"...
prompt 

SET SERVEROUT ON

WHENEVER SQLERROR EXIT

prompt Validating &&cartSchemaName....
prompt
DECLARE
        i NUMBER;
BEGIN
	select count(*) into i from dba_users where username = '&&cartSchemaName';
	if i = 0 then
		DBMS_OUTPUT.PUT_LINE('*******************************************************************************');		
		DBMS_OUTPUT.PUT_LINE('   Error: The &&cartSchemaName user don''t exist.!!!');		
		DBMS_OUTPUT.PUT_LINE('   You should run the &&cartSchemaName Scripts before to create the Registration scripts');		
		DBMS_OUTPUT.PUT_LINE('*******************************************************************************');
		RAISE PROGRAM_ERROR;
	end if;
END;
/

prompt Validating &&securitySchemaName....
prompt

DECLARE
        i NUMBER;
BEGIN
	select count(*) into i from dba_users where username = '&&securitySchemaName';
	if i = 0 then
		DBMS_OUTPUT.PUT_LINE('*******************************************************************************');		
		DBMS_OUTPUT.PUT_LINE('   Error: The &&securitySchemaName user don''t exist.!!!');		
		DBMS_OUTPUT.PUT_LINE('   You should run the &&securitySchemaName Scripts before to create the Registration scripts');		
		DBMS_OUTPUT.PUT_LINE('*******************************************************************************');
		RAISE PROGRAM_ERROR; 
	end if;

END;
/


WHENEVER SQLERROR CONTINUE



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
GRANT CREATE VIEW TO &&schemaName;
GRANT EXECUTE ON &&securitySchemaName..CONFIGURATIONMANAGER TO &&schemaName;
GRANT SELECT ANY TABLE TO &&schemaName;


ALTER USER &&schemaName GRANT CONNECT THROUGH COEUSER;
GRANT CREATE ANY INDEX TO &&schemaName;
GRANT CREATE ANY SNAPSHOT TO &&schemaName;
GRANT CREATE ANY VIEW TO &&schemaName;


ALTER USER &&cartSchemaName QUOTA UNLIMITED ON &&cscartTableSpaceName;





 



