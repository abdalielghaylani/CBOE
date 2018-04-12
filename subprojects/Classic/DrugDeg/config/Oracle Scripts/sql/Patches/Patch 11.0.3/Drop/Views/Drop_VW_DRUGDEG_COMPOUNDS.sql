--#########################################################
--Drops VW_DRUGDEG_COMPOUNDS View
--#########################################################

prompt starting Drop_VW_DRUGDEG_COMPOUNDS.sql

prompt 'dropping public synonym for view VW_DRUGDEG_COMPOUNDS' 

DECLARE
	PROCEDURE dropSynonym(synName IN varchar2) IS
			n NUMBER;
		BEGIN
			select count(*) into n from dba_synonyms where Upper(synonym_name) = synName;
			if n = 1 then
				execute immediate 'DROP PUBLIC SYNONYM ' || synName;
			end if;
		END dropSynonym;
BEGIN
	dropSynonym('VW_DRUGDEG_COMPOUNDS');
END;
/

prompt 'Dropped synonym for view VW_DRUGDEG_COMPOUNDS'

prompt 'Dropping view VW_DRUGDEG_COMPOUNDS'

DROP VIEW &&schemaName..VW_DRUGDEG_COMPOUNDS;

prompt 'Dropped view VW_DRUGDEG_COMPOUNDS'

Connect &&securitySchemaName/&&securitySchemaPass@&&serverName

DELETE FROM &&securitySchemaName..OBJECT_PRIVILEGES WHERE Schema = '&&SchemaName' AND OBJECT_NAME = 'VW_DRUGDEG_COMPOUNDS';
commit;
