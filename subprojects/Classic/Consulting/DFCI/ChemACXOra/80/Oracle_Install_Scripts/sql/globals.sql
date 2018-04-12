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
				constraint PK_GLOBALS_ID primary key (ID) USING INDEX TABLESPACE &&indexTableSpaceName 
			)';
		execute immediate 'GRANT select on  GLOBALS to PUBLIC';
	end if;

 	writeGlobalValue('VERSION_SCHEMA','&&schemaVersion');
 	writeGlobalValue('VERSION_APP','&&appVersion');
END;
/
