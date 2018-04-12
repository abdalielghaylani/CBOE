connect &&InstallUser/&&sysPass@&&serverName

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
	createSynonym('INV_LABEL_PRINTERS');
	createSynonym('INV_LPR_DEFINITION');
	createSynonym('INV_VW_COMPOUNDS');
END;
/                                           
