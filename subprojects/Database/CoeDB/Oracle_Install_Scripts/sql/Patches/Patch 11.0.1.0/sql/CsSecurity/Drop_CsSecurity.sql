--Copyright 1999-2010 CambridgeSoft Corporation. All rights reserved

prompt 'dropping public synonyms' 

DECLARE
	PROCEDURE dropSynonym(synName IN varchar2) IS
			n NUMBER;
		BEGIN
			select count(*) into n from dba_synonyms where Upper(synonym_name) = synName
			and owner = 'PUBLIC';
			if n > 0 then
				execute immediate 'DROP PUBLIC SYNONYM ' || synName;
			end if;
		END dropSynonym;
BEGIN
	dropSynonym('SECURITY_ROLES');
	dropSynonym('PEOPLE');
	dropSynonym('PRIVILEGE_TABLES');
	dropSynonym('SITES');
	dropSynonym('CHEM_REG_PRIVILEGES');
END;
/

prompt 'dropping user...'

DECLARE
	n NUMBER;
BEGIN
	select count(*) into n from dba_users where username = '&&securitySchemaNameOld';
	if n > 0 then
		execute immediate 'DROP USER &&securitySchemaNameOld CASCADE';
	end if;
END;
/

prompt 'dropping tablespaces...'

DECLARE
	n NUMBER;
	dataFileClause varchar2(20);
BEGIN
	if &&OraVersionNumber =  8 then 
		dataFileClause := '';
	else 
		dataFileClause := 'AND DATAFILES';	
	end if;

	select count(*) into n from dba_tablespaces where tablespace_name = '&&securityTableSpaceName';
	if n > 0 then
		execute immediate '
		DROP TABLESPACE &&securityTableSpaceName INCLUDING CONTENTS '||dataFileClause||' CASCADE CONSTRAINTS';
	end if;

	select count(*) into n from dba_tablespaces where tablespace_name = '&&tempTableSpaceName';
	if n > 0 then
		execute immediate '
		DROP TABLESPACE &&tempTableSpaceName INCLUDING CONTENTS '||dataFileClause||' CASCADE CONSTRAINTS';
	end if;	
end;
/

prompt 'Finished dropping cs_security.'

