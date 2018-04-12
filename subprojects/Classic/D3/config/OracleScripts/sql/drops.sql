

prompt 'dropping user...'

DECLARE
	n NUMBER;
BEGIN
	select count(*) into n from dba_users where username = '&&schemaName';
	if n = 1 then
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
	
	select count(*) into n from dba_tablespaces where tablespace_name = '&&lobsTableSpaceName';
	if n > 0 then
		execute immediate '
		DROP TABLESPACE &&lobsTableSpaceName INCLUDING CONTENTS '||dataFileClause||' CASCADE CONSTRAINTS';
	end if;

	select count(*) into n from dba_tablespaces where tablespace_name = '&&cscartTableSpaceName';
	if n > 0 then
		execute immediate '
		DROP TABLESPACE &&cscartTableSpaceName INCLUDING CONTENTS '||dataFileClause||' CASCADE CONSTRAINTS';
	end if;
end;
/




prompt 'dropping test users...'

DECLARE
	PROCEDURE dropUser(uName IN varchar2) IS
			n NUMBER;
		BEGIN
			select count(*) into n from dba_users where Upper(username) = uName;
			if n > 0 then
				execute immediate 'DROP USER ' || uName;
			end if;
		END dropUser;
BEGIN


	dropUser('D3BROWSER');
end;
/

prompt 'dropping test roles...'

DECLARE
	PROCEDURE dropRole(roleName IN varchar2) IS
			n NUMBER;
		BEGIN
			select count(*) into n from dba_roles where Upper(role) = roleName;
			if n > 0 then
				execute immediate 'DROP ROLE ' || roleName;
			end if;
		END dropRole;
BEGIN
	dropRole('D3_BROWSER');
	
END;
/

prompt 'dropping public synonyms...' 

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
	dropSynonym('D3_PARENTS');
	dropSynonym('D3_EXPTS');
	dropSynonym('D3_DEGS');
	dropSynonym('D3_MECHS');
	dropSynonym('D3_SALTS');
	dropSynonym('D3_BASE64');	
	dropSynonym('D3_FGROUPS');	
	dropSynonym('D3_DEGSFGROUP');	
	dropSynonym('D3_STATUSES');	
	dropSynonym('D3_DEG_BASE64');
	dropSynonym('D3_CONDS');
	
	
END;
/

prompt 'Finished dropping &&schemaName.'

