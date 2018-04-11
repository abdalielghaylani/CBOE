--Copyright 1999-2010 CambridgeSoft Corporation. All rights reserved

SET ECHO OFF
SET VERIFY OFF
SET TERM OFF
-- Check that GLOBALS exist
-- If not, then create it and set the schema_version from parameters.sql
DECLARE
	n NUMBER;
	PROCEDURE writeGlobalValue(pID in varchar2, pValue in varchar2) IS
		BEGIN
			execute immediate 'INSERT INTO GLOBALS (ID, VALUE) VALUES ('''||pID||''','''||pValue||''')';
		EXCEPTION
  			when DUP_VAL_ON_INDEX then
      			execute immediate 'UPDATE GLOBALS SET value = '''||pValue||''' WHERE ID = '''||pID||'''';
 		END writeGlobalValue;
BEGIN
	select count(*) into n from user_tables where table_name = 'GLOBALS';
	if n = 0 then
		execute immediate '
			CREATE TABLE GLOBALS (
				ID VARCHAR2(250) not null,
				VALUE  VARCHAR2(250) null,
				constraint PK_GLOBALS_ID primary key (ID)
			)';
		execute immediate 'GRANT select on  GLOBALS to PUBLIC';
		writeGlobalValue('VERSION_SCHEMA','11.0.1');

	end if; 	 	
END;
/

COL setSchemaName NEW_VALUE setSchemaName NOPRINT
SELECT	CASE
		WHEN  (SELECT 1 FROM DBA_TABLES WHERE TABLE_NAME='GLOBALS' AND Owner='CS_SECURITY')=1
		THEN  'CS_SECURITY.Globals'
		ELSE  '&&SchemaName..CoeGlobals'
	END	AS setSchemaName 
FROM	DUAL;

COL setSchemaVersionID NEW_VALUE setSchemaVersionID NOPRINT
SELECT	CASE
		WHEN  (SELECT 1 FROM DBA_TABLES WHERE TABLE_NAME='GLOBALS' AND Owner='CS_SECURITY')=1
		THEN  'VERSION_SCHEMA'
		ELSE  'SCHEMAVERSION'
	END	AS setSchemaVersionID 
FROM	DUAL;

COL fromVersion new_value fromVersion NOPRINT
SELECT Value AS fromVersion FROM &&setSchemaName WHERE UPPER(ID) = '&&setSchemaVersionID';

SET TERM ON
PROMPT 
PROMPT The current version is &&fromVersion

PROMPT The last version available is &&LastPatch

SET serveroutput on

BEGIN
	IF '&&fromVersion'='&&LastPatch' THEN
		dbms_output.put_line(CHR(10)||'WARNING The current version is equal to latest version available. Should not be necessary to run a patch.');
	END IF;
END;
/

ACCEPT toVersion CHAR DEFAULT &&toVersion PROMPT 'Which version do you want to upgrade to? (&&toVersion):'







