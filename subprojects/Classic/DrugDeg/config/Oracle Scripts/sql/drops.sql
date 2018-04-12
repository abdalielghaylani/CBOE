

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

	dropUser('DRUGDEGADMIN');
	dropUser('SCIENTIST');
	dropUser('DRUGDEGBROWSER');
	dropUser('DRUGDEGSUBMITTER');
	dropUser('DRUGDEG_T1');
	dropUser('DRUGDEG_T2');
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
	dropRole('DRUGDEG_BROWSER');
	dropRole('DRUGDEG_ADMINISTRATOR');
	dropRole('DRUGDEG_SUBMITTER');
	
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
	dropSynonym('DRUGDEG_PARENTS');
	dropSynonym('DRUGDEG_EXPTS');
	dropSynonym('DRUGDEG_DEGS');
	dropSynonym('DRUGDEG_MECHS');
	dropSynonym('DRUGDEG_SALTS');
	dropSynonym('DRUGDEG_BASE64');	
	dropSynonym('DRUGDEG_FGROUPS');	
	dropSynonym('DRUGDEG_DEGSFGROUP');	
	dropSynonym('DRUGDEG_STATUSES');	
	dropSynonym('DRUGDEG_DEG_BASE64');
	dropSynonym('DRUGDEG_CONDS');
	
	
END;
/

prompt 'Finished dropping &&schemaName.'

