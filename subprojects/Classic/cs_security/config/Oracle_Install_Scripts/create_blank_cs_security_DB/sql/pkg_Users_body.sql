--Copyright 1999-2003 CambridgeSoft Corporation. All rights reserved

CREATE OR REPLACE  PACKAGE BODY "CS_SECURITY"."MANAGE_USERS"
AS

	PROCEDURE GETUSERS(
		pPrivTableName IN privilege_tables.privilege_table_name%type,
		O_RS OUT CURSOR_TYPE) AS

	BEGIN
		OPEN O_RS FOR
			Select Distinct grantee as UserName, First_Name As FirstName, Last_Name AS LastName from dba_role_privs, people  
			WHERE  upper(dba_role_privs.grantee) = upper(people.user_id)
			AND Upper(dba_role_privs.granted_role) 
			IN(SELECT Upper(role_name) from security_roles s, privilege_tables p 
				WHERE s.privilege_table_int_id = p.privilege_table_id 
				AND p.privilege_table_name LIKE NVL(pPrivTableName, '%')
				) order by UserName;
	END GETUSERS;

	PROCEDURE GETALLUSERS(
		O_RS OUT CURSOR_TYPE) AS
	BEGIN
		OPEN O_RS FOR
		Select upper(username) as UserName from dba_users
		where username NOT IN ('SYSTEM','SYS','SYSMAN') 
		MINUS
		Select upper(user_id) as UserName from people
		order by 1 asc;
	END GETALLUSERS;


	PROCEDURE GETUSER(
		pUserName IN people.User_ID%type,
		O_RS OUT CURSOR_TYPE) AS
	
		my_sql VARCHAR2(2000);
		pw varchar2(30);
	BEGIN
		SELECT Password into pw FROM dba_users WHERE Upper(username) = Upper(pUserName);
		my_sql :='SELECT ' ||
				'Person_ID AS PersonID, ' || 
				'User_ID AS UserName, ' ||
				'''' || pw || ''' AS Password, ' ||
				'First_Name AS FirstName, ' ||
				'Middle_Name AS MiddleName, ' ||
				'Last_Name AS LastName, ' ||
				'Email AS Email, ' ||
				'Telephone AS Telephone, ' ||
				'Int_Address AS Address, ' ||
				'User_Code AS UserCode, ' ||
				'Supervisor_internal_id AS SuperVisorID, ' ||
				'Site_id AS SiteID, ' ||
				'Active AS isActive ' ||
				'FROM People ' ||
				'WHERE upper(User_ID) = ''' || upper(pUserName) || '''';
		OPEN O_RS FOR 
			my_Sql;
	END GETUSER;

	PROCEDURE GETROLEGRANTEES(
		pRoleName IN security_roles.role_name%type,
		O_RS OUT CURSOR_TYPE) AS

	BEGIN
		OPEN O_RS FOR
			Select Distinct grantee  from dba_role_privs 
			WHERE Upper(granted_role) = Upper(pRoleName);
	END GETROLEGRANTEES;
	  
	PROCEDURE GETROLEAVAILABLEUSERS(
		pPrivTableName IN privilege_tables.privilege_table_name%type,
		pRoleName IN security_roles.role_name%type,
		O_RS OUT CURSOR_TYPE) AS

	BEGIN
		OPEN O_RS FOR
			Select Distinct grantee as UserName from dba_role_privs, people  
			WHERE  upper(dba_role_privs.grantee) = upper(people.user_id)
			AND Upper(dba_role_privs.granted_role) 
			IN(SELECT Upper(role_name) from security_roles s, privilege_tables p 
				WHERE s.privilege_table_int_id = p.privilege_table_id 
				AND p.privilege_table_name LIKE NVL(pPrivTableName, '%')
				)
			AND grantee NOT IN ( 
								Select Distinct grantee  from dba_role_privs 
								WHERE Upper(granted_role) = Upper(pRoleName)
								);
	END GETROLEAVAILABLEUSERS;

END MANAGE_USERS;
/

