











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
	createSynonym('ACX_SYNONYM');
	createSynonym('SUBSTANCE');
	createSynonym('PRODUCT');
	createSynonym('PACKAGE');
	createSynonym('SUPPLIER');
	createSynonym('MSDX');
	createSynonym('PROPERTYALPHA');
	createSynonym('PROPERTYCLASSALPHA');
	createSynonym('SUPPLIERADDR');
	createSynonym('SUPPLIERPHONEID');
END;
/
