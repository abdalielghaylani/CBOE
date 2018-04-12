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


 	writeGlobalValue('VERSION_SCHEMA','&&schemaVersion');
 	writeGlobalValue('VERSION_APP','&&appVersion');
 	writeGlobalValue('RLS_ENABLED','0');
END;
/