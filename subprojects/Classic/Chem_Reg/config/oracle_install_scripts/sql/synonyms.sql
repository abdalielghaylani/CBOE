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
	createSynonym('ALT_IDS');
	createSynonym('BATCHES');
	createSynonym('COMMIT_TYPES');
	createSynonym('COMPOUND_MOLECULE');
	createSynonym('COMPOUND_PROJECT');
	createSynonym('COMPOUND_SALT');
	createSynonym('COMPOUND_TYPE');
	createSynonym('DUPLICATES');
	createSynonym('IDENTIFIERS');
	createSynonym('MIXTURE_SAMPLES');
	createSynonym('MIXTURES');
	createSynonym('NOTEBOOKS');
	createSynonym('PROJECTS');
	createSynonym('SALTS');
	createSynonym('REG_APPROVED');
	createSynonym('REG_NUMBERS');
	createSynonym('REG_QUALITY_CHECKED');
	createSynonym('SEQUENCE');
	createSynonym('SPECTRA');
	createSynonym('STRUCTURE_MIXTURE');
	createSynonym('STRUCTURES');
	createSynonym('SOLVATES');
	createSynonym('MOLFILES');
	createSynonym('TEMPORARY_STRUCTURES');
	createSynonym('TEST_SAMPLES');
	createSynonym('EXPERIMENTTYPERESULTS');
	createSynonym('EXPERIMENTTYPEPARAMETERS');
	createSynonym('EXPERIMENTTYPE');
	createSynonym('EXPERIMENTS');
	createSynonym('RESULTTYPE');
	createSynonym('RESULTS');
	createSynonym('PARAMETERS');
	createSynonym('PARAMETERTYPE');
	createSynonym('BATCH_PROJECTS');
	createSynonym('BATCH_PROJ_UTILIZATIONS');
	createSynonym('UTILIZATIONS');
	createSynonym('CMPD_MOL_UTILIZATIONS');
END;
/

