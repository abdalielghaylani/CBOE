--#########################################################
--CREATES VW_DRUGDEG_COMPOUNDS View
--#########################################################

prompt starting VW_DRUGDEG_COMPOUNDS.sql

CREATE OR REPLACE VIEW &&schemaName..VW_DRUGDEG_COMPOUNDS ( CMPD_KEY,
	COMPOUND_NUMBER
 ) AS
 SELECT
	DRUGDEG_PARENTS.PARENT_CMPD_KEY AS CMPD_KEY,
	DRUGDEG_PARENTS.COMPOUND_NUMBER AS COMPOUND_NUMBER
 FROM DRUGDEG_PARENTS
 UNION
 SELECT	DRUGDEG_PARENTS.PARENT_CMPD_KEY AS CMPD_KEY,
	DRUGDEG_DEGS.COMPOUND_NUMBER AS COMPOUND_NUMBER
 FROM DRUGDEG_DEGS, DRUGDEG_EXPTS, DRUGDEG_PARENTS
 WHERE DEG_EXPT_FK = EXPT_KEY
 	AND PARENT_CMPD_FK = PARENT_CMPD_KEY
 ORDER BY CMPD_KEY;
commit;

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
	createSynonym('VW_DRUGDEG_COMPOUNDS');
END;
/
Connect &&securitySchemaName/&&securitySchemaPass@&&serverName

DELETE FROM &&securitySchemaName..OBJECT_PRIVILEGES WHERE Schema = '&&SchemaName' AND OBJECT_NAME = 'VW_DRUGDEG_COMPOUNDS';
INSERT INTO &&securitySchemaName..OBJECT_PRIVILEGES VALUES ('DD_SEARCH', 'SELECT', '&&SchemaName', 'VW_DRUGDEG_COMPOUNDS');
commit;

connect &&schemaName/&&schemaPass@&&serverName;
GRANT SELECT ON &&schemaName..VW_DRUGDEG_COMPOUNDS TO CS_SECURITY WITH GRANT OPTION;
GRANT SELECT ON &&schemaName..VW_DRUGDEG_COMPOUNDS TO DRUGDEG_BROWSER;
GRANT SELECT ON &&schemaName..VW_DRUGDEG_COMPOUNDS TO DRUGDEG_SUBMITTER;
GRANT SELECT ON &&schemaName..VW_DRUGDEG_COMPOUNDS TO DRUGDEG_ADMINISTRATOR;
commit;
