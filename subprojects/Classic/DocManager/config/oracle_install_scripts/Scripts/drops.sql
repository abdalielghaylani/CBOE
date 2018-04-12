
prompt Dropping public synonyms...

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
	dropSynonym('DOCMGR_DOCUMENTS');
	dropSynonym('DOCMGR_STRUCTURES');
	dropSynonym('CTX_USER_JOB_LIST');
	dropSynonym('CTX_SCHEDULE_DOCMANAGER');
END;
/

prompt Dropping indexes...

DECLARE
	PROCEDURE dropIndex(tableOwner IN varchar2, indexName IN varchar2) IS
			n NUMBER;
		BEGIN
			select count(*) into n from dba_indexes where Upper(index_name) = indexName and Upper(table_owner) = tableOwner;
			if n = 1 then
				execute immediate 'DROP INDEX ' || tableOwner || '.' || indexName;
			end if;
		END dropIndex;
BEGIN
	dropIndex('&&schemaName', 'INDEX_DOCMGR_DOCUMENTS');
	dropIndex('&&schemaName', 'MX');
END;
/

prompt Dropping ctx tables...

DECLARE
	PROCEDURE dropTable(tableOwner IN varchar2, tableName IN varchar2) IS
			n NUMBER;
		BEGIN
			select count(*) into n from dba_tables where Upper(table_name) = tableName and Upper(owner) = tableOwner;
			if n = 1 then
				execute immediate 'DROP TABLE ' || tableOwner || '.' || tableName;
			end if;
		END dropTable;
BEGIN
	dropTable('CTXSYS', 'DR$JOB_LIST');
END;
/

prompt Dropping ctx views...

DECLARE
	PROCEDURE dropView(viewOwner IN varchar2, viewName IN varchar2) IS
			n NUMBER;
		BEGIN
			select count(*) into n from dba_views where Upper(view_name) = viewName and Upper(owner) = viewOwner;
			if n = 1 then
				execute immediate 'DROP VIEW ' || viewOwner || '.' || viewName;
			end if;
		END dropView;
BEGIN
	dropView('CTXSYS', 'CTX_JOB_LIST');
	dropView('CTXSYS', 'CTX_USER_JOB_LIST');
END;
/


prompt Dropping tablespaces...

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
end;
/

prompt Dropping user...

DECLARE
	n NUMBER;
BEGIN
	select count(*) into n from dba_users where username = '&&schemaName';
	if n = 1 then
		execute immediate 'DROP USER &&schemaName CASCADE';
	end if;
END;
/
prompt Dropping test users...

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
	dropUser('DOC_BROWSER');
	dropUser('DOC_SUBMITTER');
	dropUser('DOC_ADMIN');
end;
/

prompt Dropping test roles...

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
	dropRole('DOCMGR_BROWSER');
	dropRole('DOCMGR_SUBMITTER');
	dropRole('DOCMGR_ADMINISTRATOR');
	dropRole('DOCMGR_EXTERNAL');
END;
/

prompt Finished dropping &&schemaName.
prompt 

