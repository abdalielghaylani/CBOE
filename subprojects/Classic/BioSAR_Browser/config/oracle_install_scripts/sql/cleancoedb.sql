prompt dropping privilege table...

DECLARE
	n NUMBER;
BEGIN
	select count(*) into n from user_tables where table_name = Upper('&&privTableName');
	if n = 1 then
		execute immediate '
		DROP TABLE &&privTableName CASCADE CONSTRAINTS';
	end if;

	select count(*) into n from all_synonyms where table_name = Upper('&&privTableName') and owner = '&&securitySchemaName';
	if n = 1 then
		execute immediate '
		DROP SYNONYM &&securitySchemaName..&&privTableName';
	end if;	
END;
/


--PRIVELEGE_TABLES
delete from security_roles where Privilege_Table_Int_ID IN(select privilege_table_id from privilege_tables where upper(APP_NAME) LIKE upper('BIOSAR%'));
delete from privilege_tables where upper(APP_NAME) LIKE upper('BIOSAR%');
commit;
/


Delete from people where user_id = 'BIOSAR_ADMIN';
Delete from people where user_id = 'BIOSAR_USER_ADMIN';
Delete from people where user_id = 'BIOSAR_USER';
Delete from people where user_id = 'BIOSAR_USER_BROWSER';

commit;



delete from OBJECT_PRIVILEGES where schema = '&&schemaName';
commit;


prompt finished cleancoedb.sql...