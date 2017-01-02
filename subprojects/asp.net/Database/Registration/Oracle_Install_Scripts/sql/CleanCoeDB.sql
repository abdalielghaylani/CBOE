prompt 
prompt Starting "CleanCoeDB.sql"...
prompt 

prompt Dropping privilege table...

DECLARE
	n NUMBER;
BEGIN
	select count(*) into n from user_tables where table_name = Upper('&&privTableName');
	if n = 1 then
		execute immediate '
		DROP TABLE &&privTableName CASCADE CONSTRAINTS';
	end if;
END;
/


--PRIVELEGE_TABLES
delete from security_roles where Privilege_Table_Int_ID IN(select privilege_table_id from privilege_tables where TABLE_SPACE = 'T_REGDB');
delete from privilege_tables where TABLE_SPACE = 'T_REGDB';
commit;
/


Delete from people where user_id = 'T1_84';
Delete from people where user_id = 'T2_84';
Delete from people where user_id = 'T3_84';
Delete from people where user_id = 'T4_84';
Delete from people where user_id = 'T5_84';
Delete from people where user_id = 'T6_84';
Delete from people where user_id = 'T1_85';
Delete from people where user_id = 'T2_85';
Delete from people where user_id = 'T3_85';
Delete from people where user_id = 'T4_85';
Delete from people where user_id = 'T5_85';
Delete from people where user_id = 'T6_85';
commit;



delete from OBJECT_PRIVILEGES where schema = '&&schemaName';
commit;

DELETE FROM COEFORM WHERE upper(APPLICATION)='REGISTRATION';
COMMIT;

DELETE FROM COEDATAVIEW WHERE upper(APPLICATION)='REGISTRATION';
COMMIT;

DELETE FROM COECONFIGURATION WHERE upper(DESCRIPTION)='REGISTRATION';
COMMIT;
