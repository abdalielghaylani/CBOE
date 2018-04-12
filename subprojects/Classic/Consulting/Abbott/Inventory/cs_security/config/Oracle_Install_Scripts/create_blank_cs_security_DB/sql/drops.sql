
prompt 'dropping public synonyms' 

DECLARE
	PROCEDURE dropSynonym(synName IN varchar2) IS
			n NUMBER;
		BEGIN
			select count(*) into n from dba_synonyms where Upper(synonym_name) = synName;
			if n > 0 then
				execute immediate 'DROP PUBLIC SYNONYM ' || synName;
			end if;
		END dropSynonym;
BEGIN
	dropSynonym('SECURITY_ROLES');
	dropSynonym('PEOPLE');
	dropSynonym('PRIVILEGE_TABLES');
	dropSynonym('SITES');
END;
/

prompt 'dropping user...'

DECLARE
	n NUMBER;
BEGIN
	select count(*) into n from dba_users where username = '&&schemaName';
	if n > 0 then
		execute immediate 'DROP USER &&schemaName CASCADE';
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
	select count(*) into n from dba_tablespaces where tablespace_name = '&&tableSpaceName';
	if n > 0 then
		execute immediate '
		DROP TABLESPACE &&tableSpaceName INCLUDING CONTENTS '||dataFileClause||' CASCADE CONSTRAINTS';
	end if;
		
	select count(*) into n from dba_tablespaces where tablespace_name = '&&indexTableSpaceName';
	if n > 0 then
		execute immediate '
		DROP TABLESPACE &&indexTableSpaceName INCLUDING CONTENTS '||dataFileClause||' CASCADE CONSTRAINTS';
	end if;
	select count(*) into n from dba_tablespaces where tablespace_name = '&&tempTableSpaceName';
	if n > 0 then
		execute immediate '
		DROP TABLESPACE &&tempTableSpaceName INCLUDING CONTENTS '||dataFileClause||' CASCADE CONSTRAINTS';
	end if;
end;
/

prompt 'dropping test users...'

DECLARE
	n NUMBER;
BEGIN
	select count(*) into n from dba_users where username = 'CSSUSER';
	if n > 0 then
		execute immediate 'DROP USER CSSUSER';
	end if;
	
	select count(*) into n from dba_users where username = 'CSSADMIN';
	if n > 0 then
		execute immediate 'DROP USER CSSADMIN';
	end if;
end;
/

prompt 'dropping test roles...'

DECLARE
	n NUMBER;
BEGIN
	select count(*) into n from dba_roles where role = 'CSS_USER';
	if n > 0 then
		execute immediate 'DROP ROLE CSS_USER';
	end if;
	
	select count(*) into n from dba_roles where role = 'CSS_ADMIN';
	if n > 0 then
		execute immediate 'DROP ROLE CSS_ADMIN';
	end if;
end;
/


prompt 'Dropping csuserprofile...'

DECLARE
	n NUMBER;
BEGIN
	select count(*) into n from dba_profiles where profile = 'CSUSERPROFILE';
	if n > 0 then
		execute immediate 'DROP PROFILE CSUSERPROFILE CASCADE';
	end if;
end;
/



prompt 'Finished dropping cs_security.'

