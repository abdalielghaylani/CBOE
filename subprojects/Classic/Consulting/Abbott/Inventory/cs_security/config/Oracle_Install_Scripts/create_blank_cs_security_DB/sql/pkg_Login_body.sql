--Copyright 1999-2003 CambridgeSoft Corporation. All rights reserved

CREATE OR REPLACE PACKAGE BODY "CS_SECURITY"."LOGIN"
IS

	PROCEDURE GETPRIVSBYROLEID(
		pRoleIDList IN varchar2,
		pPrivTableName IN privilege_tables.privilege_table_name%type,
		O_RS OUT CURSOR_TYPE) AS
		my_sql varchar2(2000);
	
	BEGIN
		my_sql := 'SELECT * FROM ' || pPrivTableName || ' WHERE ROLE_INTERNAL_ID IN ( ' || pRoleIDList || ')';
		OPEN O_RS FOR
		my_sql;
	END GETPRIVSBYROLEID;

	PROCEDURE GETPRIVS(
		pPrivTableName IN privilege_tables.privilege_table_name%type,
		O_RS OUT CURSOR_TYPE) AS

		my_sql varchar2(2000);

	BEGIN
		my_sql := 'SELECT * FROM ' || pPrivTableName || ' WHERE ROLE_INTERNAL_ID IS NULL';
		OPEN O_RS FOR
			my_sql;
	END GETPRIVS;

	 PROCEDURE GETUSERROLEIDS(
		pUserName IN people.User_id%type,
		pPrivTableName IN privilege_tables.privilege_table_name%type,
		O_RS OUT CURSOR_TYPE) AS

	BEGIN
		OPEN O_RS FOR
			SELECT distinct s.role_id
						FROM security_roles s, privilege_tables p, dba_role_privs rp		  
						WHERE s.privilege_table_int_id = p.privilege_table_id 
						AND s.role_name = rp.granted_role
						AND Upper(p.privilege_table_name) = pPrivTableName 
						AND rp.granted_role IN (select granted_role 
												from dba_role_privs
												WHERE  grantee = pUserName
												UNION
												Select granted_role
												from dba_role_privs
												WHERE grantee IN (select granted_role 
																	from dba_role_privs
																	WHERE  grantee = pUserName)
												); 
	END GETUSERROLEIDS;   

	FUNCTION GETPERSONID(
		pUserName IN people.User_ID%type) RETURN people.person_id%type AS
	
	PersonID people.person_id%type;
	BEGIN
		SELECT Person_ID into PersonID from people where upper(user_id) = Upper(pUserName)
		AND Active <> -1;
	RETURN PersonID;
	EXCEPTION
		WHEN NO_DATA_FOUND THEN
		RETURN 0;    
	END GETPERSONID;
	
END LOGIN;
/

