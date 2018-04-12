--Copyright 1999-2003 CambridgeSoft Corporation. All rights reserved

CREATE OR REPLACE  FUNCTION CREATEROLE(
	pRoleName IN security_roles.role_name%type,
	pPrivTableName IN privilege_tables.privilege_table_name%type,
	pIsAlreadyInOracle IN integer,
	pPrivValueList IN varchar2)RETURN varchar2 AS

	PrivTableID integer;
	RoleID integer;
BEGIN
	dbms_output.put_line('hello');
	if pIsAlreadyInOracle = 0 then
		EXECUTE IMMEDIATE 'CREATE ROLE ' || pRoleName || ' NOT IDENTIFIED';
		EXECUTE IMMEDIATE 'GRANT CSS_USER TO ' || pRoleName; 
		EXECUTE IMMEDIATE 'REVOKE ' || pRoleName || ' FROM CS_SECURITY';

		SELECT Privilege_Table_ID into PrivTableID FROM Privilege_Tables WHERE Upper(Privilege_Table_Name) =  Upper(pPrivTableName);
		INSERT INTO Security_Roles
			(Privilege_Table_Int_ID,
			Role_Name)
		VALUES
			(PrivTableID,
			Upper(pRoleName)) RETURNING ROLE_ID into RoleID;
		EXECUTE IMMEDIATE 'INSERT INTO ' || pPrivTableName || ' VALUES ( ' || RoleID || ', ' ||  pPrivValueList || ')';
	else
		INSERT INTO Security_Roles
			(Privilege_Table_Int_ID,
			Role_Name)
		VALUES
			(NULL,
			Upper(pRoleName)) RETURNING ROLE_ID into RoleID;
	end if;
	RETURN '1';
END CREATEROLE;
/

CREATE OR REPLACE  FUNCTION CREATEUSER(
	pUserName IN people.User_ID%type,
	pIsAlreadyInOracle IN integer:=0,
	pPassword IN varchar2,
	pRolesGranted IN varchar2,
	pFirstName IN people.first_name%type:=NULL,
	pMiddleName IN people.middle_name%type:=NULL,
	pLastName IN people.last_name%type:=NULL,
	pTelephone IN people.telephone%type:=NULL,
	pEmail IN people.email%type:=NULL,
	pAddress IN people.int_address%type:=NULL,
	pUserCode IN people.user_code%type:=NULL,
	pSupervisorID IN people.supervisor_internal_id%type:=NULL,
	pSiteID IN people.site_id%type:=NULL,
	pIsActive IN people.Active%type:=1) RETURN varchar2 AS

	source_cursor integer;
	rows_processed integer;
	userCode people.user_code%type;
	user_or_role_name_conflict exception;
	unique_constraint_violated exception;
	role_not_found exception;
	pragma exception_init (unique_constraint_violated, -1);
	pragma exception_init (user_or_role_name_conflict, -1920);
	pragma exception_init (role_not_found, -1919);

BEGIN
	source_cursor := dbms_sql.open_cursor;
	if pIsAlreadyInOracle = 0 then
		dbms_sql.parse(source_cursor,'CREATE USER ' || pUserName || ' IDENTIFIED BY ' || pPassword || ' DEFAULT TABLESPACE USERS TEMPORARY TABLESPACE TEMP'  , dbms_sql.NATIVE);
		rows_processed := dbms_sql.execute(source_cursor);
	end if;
	dbms_sql.parse (source_cursor, 'GRANT CONNECT TO ' || pUserName, dbms_sql.NATIVE);
	rows_processed := dbms_sql.execute(source_cursor);
	dbms_sql.parse (source_cursor, 'GRANT ' || pRolesGranted || ' TO ' || pUserName, dbms_sql.NATIVE);
	rows_processed := dbms_sql.execute(source_cursor);
	dbms_sql.parse (source_cursor, 'ALTER USER ' || pUserName || ' DEFAULT ROLE ALL', dbms_sql.NATIVE);
	rows_processed := dbms_sql.execute(source_cursor);
	dbms_sql.parse (source_cursor, 'ALTER USER ' || pUserName || ' PROFILE csuserprofile', dbms_sql.NATIVE);
	rows_processed := dbms_sql.execute(source_cursor);
	if pUserCode IS NULL then
		userCode := pUserName;
	else
		userCode := pUserCode;
	end if;

	INSERT INTO people (
		User_ID,
		First_Name,
		Middle_Name,
		Last_Name,
		Email,
		Telephone,
		Int_Address,
		User_Code,
		Supervisor_internal_id,
		Site_id,
		Active)
		VALUES (
		pUserName,
		pFirstName,
		pMiddleName,
		pLastName,
		pEmail,
		pTelephone,
		pAddress,
		userCode,
		pSupervisorID,
		pSiteID,
		pIsActive);
	RETURN '1';
EXCEPTION
	WHEN user_or_role_name_conflict then
		RETURN 'user name ' || pUserName || ' conflicts with another user or role name ';
	WHEN unique_constraint_violated then
		dbms_sql.parse (source_cursor, 'DROP USER ' || pUserName , dbms_sql.NATIVE);
		rows_processed := dbms_sql.execute(source_cursor);
		RETURN 'User Code ' || UserCode || ' is or has been already taken by another user';
	WHEN role_not_found then
		dbms_sql.parse (source_cursor, 'DROP USER ' || pUserName , dbms_sql.NATIVE);
		rows_processed := dbms_sql.execute(source_cursor);
		RETURN 'Failed to find one or more of the roles to be granted';
END CREATEUSER;
/

CREATE OR REPLACE FUNCTION DELETEROLE(
	pRoleName IN security_roles.role_name%type,
	pPrivTableName IN privilege_tables.privilege_table_name%type)RETURN varchar2 AS

	RoleID integer;
	numGrantees integer;
BEGIN

	Select count(*) into numGrantees  from dba_role_privs
	WHERE Upper(granted_role) = Upper(pRoleName);
	
	if numGrantees > 0 then
		raise_application_error(-20000, 'Cannot delete '|| pRoleName || ' because it is assigned to existing users.' );
	end if;
	
	SELECT Role_ID into RoleID FROM Security_Roles WHERE Upper(Role_Name) =  Upper(pRoleName);
	EXECUTE IMMEDIATE 'DELETE FROM ' || pPrivTableName || ' WHERE ROLE_INTERNAL_ID= ' || RoleID;
	EXECUTE IMMEDIATE 'DELETE FROM CS_SECURITY_PRIVILEGES WHERE ROLE_INTERNAL_ID= ' || RoleID;
	EXECUTE IMMEDIATE 'DELETE FROM Security_Roles WHERE ROLE_ID = ' || RoleID;
	EXECUTE IMMEDIATE 'DROP ROLE ' || pRoleName;
	RETURN '1';
END DELETEROLE;
/

CREATE OR REPLACE FUNCTION DELETEUSER(
	pUserName IN people.user_id%type) RETURN varchar2 AS
    cannot_drop_connected_user exception;
	pragma exception_init (cannot_drop_connected_user, -1940);
BEGIN
	EXECUTE IMMEDIATE 'DROP USER ' || pUserName || ' CASCADE';
	RETURN '1';
EXCEPTION
	WHEN cannot_drop_connected_user then
		RETURN 'Cannot drop user ' || pUserName || ' because it is currently connected to Oracle';
END DELETEUSER;
/

CREATE OR REPLACE FUNCTION "CS_SECURITY"."UPDATEROLE"(
	pRoleName IN security_roles.role_name%type,
	pPrivTableName IN privilege_tables.privilege_table_name%type,
	pPrivValueList IN varchar2) RETURN varchar2 AS

	RoleID integer;
BEGIN
	SELECT Role_ID into RoleID FROM Security_Roles WHERE Upper(Role_Name) =  Upper(pRoleName);
	EXECUTE IMMEDIATE 'DELETE FROM ' || pPrivTableName || ' WHERE ROLE_INTERNAL_ID= ' || RoleID;
	EXECUTE IMMEDIATE 'INSERT INTO ' || pPrivTableName || ' VALUES ( ' || RoleID || ', ' ||  pPrivValueList || ')';
	RETURN '1';
END UPDATEROLE;
/

CREATE OR REPLACE FUNCTION UPDATEUSER(
	pUserName IN people.user_id%type,
	pPassword IN varchar2:=NULL,
	pRolesGranted IN varchar2:=NULL,
	pRolesRevoked IN varchar2:=NULL,
	pFirstName IN people.first_name%type:=NULL,
	pMiddleName IN people.middle_name%type:=NULL,
	pLastName IN people.last_name%type:=NULL,
	pTelephone IN people.telephone%type:=NULL,
	pEmail IN people.email%type:=NULL,
	pAddress IN people.int_address%type:=NULL,
	pUserCode IN people.user_code%type:=NULL,
	pSupervisorID IN people.supervisor_internal_id%type:=NULL,
	pSiteID IN people.site_id%type:=NULL,
	pIsActive IN people.Active%type:=1) RETURN varchar2 AS

	source_cursor integer;
	rows_processed integer;
BEGIN
	UPDATE people
	SET
		First_Name = pFirstName,
		Middle_Name = pMiddleName,
		Last_Name = pLastName,
		Email = pEmail,
		Telephone = pTelephone,
		Int_Address = pAddress,
		User_Code = pUserCode,
		Supervisor_internal_id = pSupervisorID,
		Site_id = pSiteID,
		Active = pIsActive
	WHERE
		Upper(people.user_id) = upper(pUserName);
	if (pPassword is not NULL) OR (pRolesGranted is not NULL) OR (pRolesRevoked is not NULL) then
		source_cursor := dbms_sql.open_cursor;
		if (pPassword is not NULL) then
			dbms_sql.parse(source_cursor,'ALTER USER ' || pUserName || ' IDENTIFIED BY ' || pPassword , dbms_sql.NATIVE);
			rows_processed := dbms_sql.execute(source_cursor);
		end if;

		if (pRolesRevoked is not NULL) then
			dbms_sql.parse(source_cursor,'REVOKE ' || pRolesRevoked || ' FROM ' || pUserName , dbms_sql.NATIVE);
			rows_processed := dbms_sql.execute(source_cursor);
		end if;

		if (pRolesGranted is not NULL) then
			dbms_sql.parse(source_cursor,'GRANT ' || pRolesGranted || ' TO ' || pUserName   , dbms_sql.NATIVE);
			rows_processed := dbms_sql.execute(source_cursor);
		end if;
	end if;
	RETURN '1';
END UPDATEUSER;
/

CREATE OR REPLACE PROCEDURE MAPPRIVSTOROLE(
	pRoleName IN security_roles.role_name%type,
	pPrivName IN varchar2,
	pAction IN varchar2) IS
    cannot_revoke exception;
    pragma exception_init (cannot_revoke, -1927);
	CURSOR privs_cur(PrivName_in IN varchar2) IS
		SELECT Privilege, Schema, Object_Name from cs_security.Object_Privileges
		WHERE Privilege_Name = PrivName_in AND Schema IS NOT NULL ORDER BY Schema, Privilege;
	thePrivilege varchar(30);
	theSchema varchar(30);
	theObjectName  varchar(30);
	mySQL varchar2(2000);
	keyword varchar2(10);
BEGIN
	if pAction = 'GRANT' then
		keyword := ' TO ';
	else
		keyword := ' FROM ';
	end if;
	OPEN privs_cur(pPrivName);
		LOOP
			FETCH privs_cur INTO thePrivilege, theSchema, theObjectName;
			EXIT WHEN privs_cur%NOTFOUND;
			mySQL := pAction || ' ' || thePrivilege || ' ON ' || theSchema || '.' || theObjectName || keyword || pRoleName;
			Execute IMMEDIATE
				mySQL;
		END LOOP;
	CLOSE privs_cur;
EXCEPTION
	WHEN cannot_revoke then
	 	return;
	WHEN Others then
	  	raise_application_error(-20000, 'CS_SECURITY does not have sufficient privileges to '|| mySQL ) ;
END MAPPRIVSTOROLE;
/

CREATE OR REPLACE FUNCTION CHANGEPWD(
	pUserName IN people.user_id%type,
	pPassword IN varchar2:=NULL,
	pNewPassword IN varchar2:=NULL) RETURN varchar2 AS
    invalid_user_or_pass exception;
	--source_cursor integer;
	--rows_processed integer;
	pw varchar2(30);
	cannot_reuse_pwd exception;
	pragma exception_init (cannot_reuse_pwd, -28007);
BEGIN
		SELECT Password into pw FROM dba_users WHERE Upper(username) = Upper(pUserName);
		if pPassword <> pw then
		   RAISE invalid_user_or_pass;
		end if;
		--source_cursor := dbms_sql.open_cursor;
		if (pNewPassword is not NULL) then
			--dbms_sql.parse(source_cursor,'ALTER USER ' || pUserName || ' IDENTIFIED BY ' || pNewPassword , dbms_sql.NATIVE);
			--rows_processed := dbms_sql.execute(source_cursor);
			execute immediate 'ALTER USER ' || pUserName || ' IDENTIFIED BY ' || pNewPassword;
		end if;
	RETURN '1';
EXCEPTION
	WHEN invalid_user_or_pass then
		RETURN 'Invalid User Name or Password';
	WHEN cannot_reuse_pwd then
		RETURN 'Cannot reuse previously used password.';	
END ChangePWD;
/

Create or Replace Procedure GrantOnCoreTableToAllRoles(
		pTableName IN varchar2,
		pSchemaName IN object_privileges.schema%type,
		pMinimumPrivilege IN varchar2,
		pPrivTableName IN privilege_tables.privilege_table_name%type)
IS
	Type CURSOR_TYPE IS REF CURSOR;
 	roles_cur CURSOR_TYPE;
 	theRole varchar2(30);
BEGIN

  	-- Insert object_privs
  	DELETE FROM CS_SECURITY.OBJECT_PRIVILEGES WHERE SCHEMA = Upper(pSchemaName) AND OBJECT_NAME = Upper(pTableName);
	INSERT INTO CS_SECURITY.OBJECT_PRIVILEGES VALUES (pMinimumPrivilege, 'SELECT', Upper(pSchemaName), Upper(pTableName));
	INSERT INTO CS_SECURITY.OBJECT_PRIVILEGES VALUES (pMinimumPrivilege, 'INSERT', Upper(pSchemaName), Upper(pTableName));
	INSERT INTO CS_SECURITY.OBJECT_PRIVILEGES VALUES (pMinimumPrivilege, 'DELETE', Upper(pSchemaName), Upper(pTableName));
	INSERT INTO CS_SECURITY.OBJECT_PRIVILEGES VALUES (pMinimumPrivilege, 'UPDATE', Upper(pSchemaName), Upper(pTableName));
  	
  	-- Loop over all roles for this priv table
  	manage_roles.GetRoles(pPrivTableName, roles_cur);	
  	LOOP
		FETCH roles_cur INTO theRole;
		EXIT WHEN roles_cur%NOTFOUND;
		-- grant to role
		EXECUTE IMMEDIATE 'GRANT SELECT, INSERT, DELETE, UPDATE ON ' || pSchemaname || '.' || pTableName || ' TO ' || theRole; 
	END LOOP;	
END GrantOnCoreTableToAllRoles;
/
