--#########################################################
--Drops VW_DRUGDEG_NAME View
--#########################################################

prompt starting Drop_VW_DRUGDEG_NAME.sql

prompt 'dropping public synonym for view VW_DRUGDEG_NAME' 

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
	dropSynonym('VW_DRUGDEG_NAME');
END;
/

prompt 'Dropped synonym for view VW_DRUGDEG_NAME'

prompt 'Dropping view VW_DRUGDEG_NAME'

DROP VIEW &&schemaName..VW_DRUGDEG_NAME;

prompt 'Dropped view VW_DRUGDEG_NAME'

Connect &&securitySchemaName/&&securitySchemaPass@&&serverName

DELETE FROM &&securitySchemaName..OBJECT_PRIVILEGES WHERE Schema = '&&SchemaName' AND OBJECT_NAME = 'VW_DRUGDEG_NAME';
commit;
