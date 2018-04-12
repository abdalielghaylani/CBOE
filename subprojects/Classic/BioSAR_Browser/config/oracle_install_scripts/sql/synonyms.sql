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
	createSynonym('DB_COLUMN');
	createSynonym('DB_TABLE');
	createSynonym('DB_RELATIONSHIP');
	createSynonym('DB_FORMTYPE');
	createSynonym('DB_FORM_ITEM');
	createSynonym('DB_FORM');
	createSynonym('DB_FORMGROUP');
	createSynonym('DB_SCHEMA');
	createSynonym('DB_HTTP_CONTENT_TYPE');
	createSynonym('DB_INDEX_TYPE');
	createSynonym('DB_XML_TEMPL_DEF');
END;
/

