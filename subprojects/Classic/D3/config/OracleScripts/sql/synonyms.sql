
--Copyright 1999-2003 CambridgeSoft Corporation. All rights reserved

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
	createSynonym('D3_BASE64','DRUGDEG_BASE64');
	createSynonym('D3_CONDS','DRUGDEG_CONDS');
	createSynonym('D3_DEGS','DRUGDEG_DEGS');
	createSynonym('D3_DEGSFGROUP','DRUGDEG_DEGSFGROUP');
	createSynonym('D3_EXPTS','DRUGDEG_EXPTS');
	createSynonym('D3_FGROUPS','DRUGDEG_FGROUPS');
	createSynonym('D3_MECHS','DRUGDEG_MECHS');
	createSynonym('D3_PARENTS','DRUGDEG_PARENTS');
	createSynonym('D3_SALTS','DRUGDEG_SALTS');
	createSynonym('D3_STATUSES','DRUGDEG_STATUSES');
	createSynonym('D3_DEG_BASE64', 'DRUGDEG_BASE64');
END;
/


