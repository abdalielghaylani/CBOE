
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
	dropSynonym('SECURITY_ROLES');
	dropSynonym('ALT_IDS');
	dropSynonym('BATCHES');
	dropSynonym('COMMIT_TYPES');
	dropSynonym('COMPOUND_MOLECULE');
	dropSynonym('COMPOUND_PROJECT');
	dropSynonym('COMPOUND_SALT');
	dropSynonym('COMPOUND_TYPE');
	dropSynonym('DUPLICATES');
	dropSynonym('IDENTIFIERS');
	dropSynonym('MIXTURE_SAMPLES');
	dropSynonym('MIXTURES');
	dropSynonym('NOTEBOOKS');
	dropSynonym('PROJECTS');
	dropSynonym('SALTS');
	dropSynonym('REG_APPROVED');
	dropSynonym('REG_NUMBERS');
	dropSynonym('REG_QUALITY_CHECKED');
	dropSynonym('SEQUENCE');
	dropSynonym('SPECTRA');
	dropSynonym('STRUCTURE_MIXTURE');
	dropSynonym('STRUCTURES');
	dropSynonym('SOLVATES');
	dropSynonym('MOLFILES');
	dropSynonym('TEMPORARY_STRUCTURES');
	dropSynonym('TEST_SAMPLES');
	dropSynonym('EXPERIMENTTYPERESULTS');
	dropSynonym('EXPERIMENTTYPEPARAMETERS');
	dropSynonym('EXPERIMENTTYPE');
	dropSynonym('EXPERIMENTS');
	dropSynonym('RESULTTYPE');
	dropSynonym('RESULTS');
	dropSynonym('PARAMETERS');
	dropSynonym('PARAMETERTYPE');
	dropSynonym('BATCH_PROJECTS');
	dropSynonym('BATCH_PROJ_UTILIZATIONS');
	dropSynonym('UTILIZATIONS');
	dropSynonym('CMPD_MOL_UTILIZATIONS');
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
	dropUser('T1_84');
	dropUser('T2_84');
	dropUser('T3_84');
	dropUser('T4_84');
	dropUser('T5_84');
	dropUser('T6_84');
	dropUser('T1_85');
	dropUser('T2_85');
	dropUser('T3_85');
	dropUser('T4_85');
	dropUser('T5_85');
	dropUser('T6_85');
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
	dropRole('BROWSER');
	dropRole('SUBMITTER');
	dropRole('SUPERVISING_SCIENTIST');
	dropRole('CHEMICAL_ADMINISTRATOR');
	dropRole('SUPERVISING_CHEMICAL_ADMIN');
	dropRole('PERFUME_CHEMIST');
END;
/

prompt 'Finished dropping &&schemaName.'

