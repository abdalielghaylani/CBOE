--Copyright 1999-2003 CambridgeSoft Corporation. All rights reserved

DECLARE
	PROCEDURE createSynonym(synName IN varchar2) IS
			n NUMBER;
		BEGIN
			select count(*) into n from dba_synonyms where Upper(synonym_name) = synName;
			if n = 0 then
				execute immediate 'CREATE PUBLIC SYNONYM ' || synName || ' FOR &&schemaName..' || synName;
			end if;
		END createSynonym;
BEGIN
	createSynonym('SECURITY_ROLES');
	createSynonym('PEOPLE');
	createSynonym('PRIVILEGE_TABLES');
	createSynonym('SITES');
END;
/

