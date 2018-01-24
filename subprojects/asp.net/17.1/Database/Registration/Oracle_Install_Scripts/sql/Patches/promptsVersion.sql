--Copyright 1999-2010 CambridgeSoft Corporation. All rights reserved

SET ECHO OFF
SET VERIFY OFF
SET TERM OFF
-- Check that GLOBALS exist
-- If not, then create it and set the schema_version from parameters.sql
DECLARE
	n NUMBER;
	is110 NUMBER;
	PROCEDURE writeGlobalValue(pID in varchar2, pValue in varchar2) IS
		BEGIN
			execute immediate 'INSERT INTO &&schemaName..GLOBALS (ID, VALUE) VALUES ('''||pID||''','''||pValue||''')';
		EXCEPTION
  			when DUP_VAL_ON_INDEX then
      			execute immediate 'UPDATE &&schemaName..GLOBALS SET value = '''||pValue||''' WHERE ID = '''||pID||'''';
 		END writeGlobalValue;
BEGIN
	select count(*) into n from all_tables where owner='&&schemaName' and table_name = 'GLOBALS';
	if n = 0 then
		execute immediate '
			CREATE TABLE &&schemaName..GLOBALS (
				ID VARCHAR2(250) not null,
				VALUE  VARCHAR2(250) null,
				constraint PK_GLOBALS_ID primary key (ID)
			)';
		execute immediate 'GRANT select on  &&schemaName..GLOBALS to PUBLIC';
		writeGlobalValue('VERSION_SCHEMA','11.0.1');
	else
		-- Special check to detect 11.0 version (Firmenich only)
		select count(*) into is110 from all_tables where owner='&&schemaName' and table_name = 'COETEMPHITLIST';
		if is110= 1 then
			update &&schemaName..GLOBALS set value = '11.0' where ID = 'VERSION_SCHEMA';
		end if;	
	end if; 	 	
END;
/

COL fromVersion new_value fromVersion NOPRINT
SELECT Value AS fromVersion FROM &&schemaName..Globals WHERE UPPER(ID) = 'VERSION_SCHEMA';

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







