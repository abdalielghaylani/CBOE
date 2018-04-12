--Copyright 1999-2003 CambridgeSoft Corporation. All rights reserved

CREATE OR REPLACE  PACKAGE BODY "CS_SECURITY"."MANAGE_ROLES"                                                       
AS

	FUNCTION excludeFGrolesSQL
	return varchar2
	AS
	  n number;
	BEGIN
		select count(*) into n from dba_tables where table_name = Upper('BIOSAR_BROWSER_PRIVILEGES');
		if n = 1 then
			return 'role_id NOT IN (select role_internal_id from biosar_browser_privileges where is_formgroup_role = 1)';
		else
			return '1=1';	
		end if;
	END excludeFGrolesSQL;
	PROCEDURE GETALLROLES(
		O_RS OUT CURSOR_TYPE) AS
	BEGIN
		OPEN O_RS FOR
			SELECT ROLE AS ROLE_NAME FROM dba_roles
			MINUS
			SELECT ROLE_NAME FROM security_roles
			ORDER BY 1 ASC;
	END GETALLROLES;

	PROCEDURE GETROLES(
		pPrivTableName IN privilege_tables.privilege_table_name%type,
		O_RS OUT CURSOR_TYPE) AS

		mySQL varchar(2000);
	BEGIN
		if pPrivTableName IS NOT NULL then
			mySQL := 'SELECT Distinct ROLE_NAME
						FROM security_roles s, privilege_tables p
						WHERE p.privilege_table_id = s.privilege_table_int_id
						AND Upper(p.privilege_table_name) = Upper(''' || pPrivTableName || ''')
						AND '||excludeFGrolesSQL|| 
						' ORDER BY ROLE_NAME ASC';
		else
			mySQL := 'SELECT Distinct ROLE_NAME FROM security_roles  
			WHERE '||excludeFGrolesSQL|| 
			' ORDER BY ROLE_NAME';
		end if;
		OPEN O_RS FOR
			mySQL;
	END GETROLES;

	PROCEDURE GETAVAILABLEROLES(
		pUserName IN people.User_id%type,
		pPrivTableName IN privilege_tables.privilege_table_name%type:=NULL,
		O_RS OUT CURSOR_TYPE) AS
        mySQL varchar(2000);
	BEGIN
		mySQL := 'SELECT DISTINCT ROLE_NAME
				  FROM   security_roles s
				  WHERE '||excludeFGrolesSQL||'
				 MINUS
				  SELECT ROLE_NAME
				  FROM   security_roles s
				  WHERE  s.role_name IN (SELECT granted_role 
										 FROM   dba_role_privs
										 WHERE	Upper(grantee) = Upper('''||pUserName||''')
										   AND  Upper(dba_role_privs.granted_role) IN ( SELECT DISTINCT ROLE_NAME
																						FROM   security_roles s, privilege_tables p
																						WHERE  s.privilege_table_int_id = p.privilege_table_id
																						  AND  p.privilege_table_name LIKE NVL('''||pPrivTableName||''', ''%'')))
				  ORDER BY 1 ASC';
		OPEN O_RS FOR
			mySQL;
	END GETAVAILABLEROLES;

	PROCEDURE GETUSERROLES(
		pUserName IN people.User_id%type,
		pPrivTableName IN privilege_tables.privilege_table_name%type:=NULL,
		O_RS OUT CURSOR_TYPE) AS

	BEGIN
		OPEN O_RS FOR
			Select granted_role AS ROLE_NAME from dba_role_privs
			WHERE Upper(grantee) = Upper(pUserName)
			AND  Upper(dba_role_privs.granted_role)
			IN(Select Distinct ROLE_NAME
				FROM security_roles s, privilege_tables p
				WHERE  s.privilege_table_int_id = p.privilege_table_id
				AND p.privilege_table_name LIKE NVL(pPrivTableName, '%'))
			ORDER BY ROLE_NAME ASC;
	END GETUSERROLES;

	PROCEDURE GETROLEROLES(
		pRoleName IN security_roles.Role_name%type,
		O_RS OUT CURSOR_TYPE) AS

	BEGIN
		OPEN O_RS FOR
			Select granted_role AS ROLE_NAME from dba_role_privs
			WHERE Upper(grantee) = Upper(pRoleName)
			ORDER BY ROLE_NAME ASC;
	END GETROLEROLES;

	PROCEDURE GETROLEAVAILABLEROLES(
		pPrivTableName IN privilege_tables.privilege_table_name%type,
		pRoleName IN security_roles.Role_name%type,
		O_RS OUT CURSOR_TYPE) AS
        mySQL varchar(2000);
	BEGIN                   
		mySQL := 'Select Distinct ROLE_NAME
					FROM security_roles s, privilege_tables p
					WHERE  s.privilege_table_int_id = p.privilege_table_id
					AND Upper(p.privilege_table_name) LIKE NVL(Upper('''||pPrivTableName||'''), ''%'')
					AND role_name <> '''||pRoleName||'''
					AND role_name NOT IN (Select granted_role AS ROLE_NAME from dba_role_privs
										  WHERE Upper(grantee) = Upper('''||pRoleName||''')
										  ) 
					AND '||excludeFGrolesSQL||
					' ORDER BY ROLE_NAME ASC';
		OPEN O_RS FOR
			mySQL;
	END GETROLEAVAILABLEROLES;

	PROCEDURE GETROLEPRIVS(
		pRoleName IN Security_Roles.Role_Name%type,
		O_RS OUT CURSOR_TYPE) AS

	RoleID integer;
	PrivTableName varchar2(30);
	my_sql varchar2(2000);
	BEGIN
		SELECT s.ROLE_ID , p.PRIVILEGE_TABLE_NAME INTO RoleID, PrivTableName
		FROM Security_Roles s, Privilege_Tables p
		WHERE s.Privilege_Table_int_ID = p.Privilege_Table_ID
		AND Upper(s.Role_Name) = Upper(pRoleName);
		my_sql := 'SELECT * FROM ' || PrivTableName || ' WHERE ROLE_INTERNAL_ID = ' || To_Char(RoleID);
		OPEN O_RS FOR
			my_sql;
	END GETROLEPRIVS;

	FUNCTION UPDATEROLESGRANTEDTOROLE(
		pRoleName IN security_roles.role_name%type,
		pRolesGranted IN varchar2:=NULL,
		pRolesRevoked IN varchar2:=NULL)RETURN varchar2 AS

	source_cursor integer;
	rows_processed integer;
	BEGIN
		source_cursor := dbms_sql.open_cursor;
		if (pRolesRevoked is not NULL) then
			dbms_sql.parse(source_cursor,'REVOKE ' || pRolesRevoked || ' FROM ' || pRoleName , dbms_sql.NATIVE);
			rows_processed := dbms_sql.execute(source_cursor);
		end if;
		if (pRolesGranted is not NULL) then
			dbms_sql.parse(source_cursor,'GRANT ' || pRolesGranted || ' TO ' || pRoleName   , dbms_sql.NATIVE);
			rows_processed := dbms_sql.execute(source_cursor);
		end if;
		RETURN '1';
	END UPDATEROLESGRANTEDTOROLE;

	FUNCTION UPDATEUSERSGRANTEDAROLE(
		pRoleName IN security_roles.role_name%type,
		pUsersGranted IN varchar2:=NULL,
		pUsersRevoked IN varchar2:=NULL) RETURN varchar2 AS

		source_cursor integer;
		rows_processed integer;
	BEGIN
		source_cursor := dbms_sql.open_cursor;
		if (pUsersRevoked is not NULL) then
			dbms_sql.parse(source_cursor,'REVOKE ' || pRoleName || ' FROM ' || pUsersRevoked , dbms_sql.NATIVE);
			rows_processed := dbms_sql.execute(source_cursor);
		end if;
		if (pUsersGranted is not NULL) then
			dbms_sql.parse(source_cursor,'GRANT ' || pRoleName || ' TO ' || pUsersGranted , dbms_sql.NATIVE);
			rows_processed := dbms_sql.execute(source_cursor);
		end if;
		RETURN '1';
	END UPDATEUSERSGRANTEDAROLE;

END MANAGE_ROLES;
/

