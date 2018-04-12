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
	createSynonym('DRUGDEG_BASE64');
	createSynonym('DRUGDEG_CONDS');
	createSynonym('DRUGDEG_DEGS');
	createSynonym('DRUGDEG_DEGSFGROUP');
	createSynonym('DRUGDEG_EXPTS');
	createSynonym('DRUGDEG_FGROUPS');
	createSynonym('DRUGDEG_MECHS');
	createSynonym('DRUGDEG_PARENTS');
	createSynonym('DRUGDEG_SALTS');
	createSynonym('DRUGDEG_STATUSES');
	
END;
/


DECLARE
	PROCEDURE createSynonym(synName IN varchar2, rsyname IN varchar2) IS
			n NUMBER;
		BEGIN
			select count(*) into n from dba_synonyms where Upper(synonym_name) = synName;
			if n = 0 then
				execute immediate 'CREATE PUBLIC SYNONYM ' || synName || ' FOR &&schemaName..' || rsyname;
			end if;
		END createSynonym;
BEGIN
	createSynonym('DRUGDEG_DEG_BASE64', 'DRUGDEG_BASE64');
END;
/


