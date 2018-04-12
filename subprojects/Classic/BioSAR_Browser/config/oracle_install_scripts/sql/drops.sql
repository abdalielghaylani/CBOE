
prompt 'dropping public synonyms...' 

DECLARE
	PROCEDURE dropSynonym(synName IN varchar2) IS
			n NUMBER;
		BEGIN
			select count(*) into n from all_synonyms where Upper(synonym_name) = synName;
			if n > 0 then
				execute immediate 'DROP PUBLIC SYNONYM ' || synName;
			end if;
		END dropSynonym;
BEGIN	
	dropSynonym('DB_COLUMN');
	dropSynonym('DB_TABLE');
	dropSynonym('DB_RELATIONSHIP');
	dropSynonym('DB_FORMTYPE');
	dropSynonym('DB_FORM_ITEM');
	dropSynonym('DB_FORM');
	dropSynonym('DB_FORMGROUP');
	dropSynonym('DB_SCHEMA');
	dropSynonym('DB_HTTP_CONTENT_TYPE');
	dropSynonym('DB_INDEX_TYPE');
	dropSynonym('DB_XML_TEMPL_DEF');
	dropSynonym('TREE_ITEM');
	dropSynonym('TREE_ITEM_TYPE');
	dropSynonym('TREE_NODE');
	dropSynonym('TREE_TYPE');
	dropSynonym('&&privTableName');	
END;
/

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
	dropUser('BIOSAR_ADMIN');
	dropUser('BIOSAR_USER_ADMIN');
	dropUser('BIOSAR_USER');
	dropUser('BIOSAR_USER_BROWSER');
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
	dropRole('BIOSAR_BROWSER_ADMIN');
	dropRole('BIOSAR_BROWSER_USER_ADMIN');
	dropRole('BIOSAR_BROWSER_USER');
	dropRole('BIOSAR_BROWSER_USER_BROWSER');
END;
/

prompt 'Finished dropping &&schemaName.'

