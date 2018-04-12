-- Copyright Cambridgesoft Corp 2001-2005 all rights reserved

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
	createSynonym('INV_CONTAINER_BATCHES');
	createSynonym('INV_UNIT_CONVERSION_FORMULA');
	createSynonym('INV_GRAPHICS');
	createSynonym('INV_GRAPHIC_TYPES');
	createSynonym('INV_VW_GRID_LOCATION_LITE');
	createSynonym('INV_DOCS');
	createSynonym('INV_DOC_TYPES');
	createSynonym('INV_ORG_UNIT');
	createSynonym('INV_ORG_ROLES');
	createSynonym('INV_ORG_USERS');
	
     
END;
/                                           


