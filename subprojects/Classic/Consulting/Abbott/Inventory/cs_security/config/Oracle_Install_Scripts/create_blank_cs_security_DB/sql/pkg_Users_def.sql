--Copyright 1999-2003 CambridgeSoft Corporation. All rights reserved

CREATE OR REPLACE  PACKAGE "CS_SECURITY"."MANAGE_USERS"                        
AS
	TYPE  CURSOR_TYPE IS REF CURSOR;

	--Returns all Oracle users
	PROCEDURE GETALLUSERS(
		O_RS OUT CURSOR_TYPE);
	
	--Returns all CSS users for a given PrivTable.
	--Use PrivTableName = Null to get all CSS users 
	PROCEDURE GETUSERS(
		pPrivTableName IN privilege_tables.privilege_table_name%type,
		O_RS OUT CURSOR_TYPE);
	
	--Returns all attributes of a single user           
	PROCEDURE GETUSER(
		pUserName IN people.User_ID%type,
		O_RS OUT CURSOR_TYPE);

	--Gets grantees of a given role
	PROCEDURE GETROLEGRANTEES(
		pRoleName IN security_roles.role_name%type,
		O_RS OUT CURSOR_TYPE);
	
	-- Gets users that are not yet grantees to a role
	PROCEDURE GETROLEAVAILABLEUSERS(
		pPrivTableName IN privilege_tables.privilege_table_name%type,
		pRoleName IN security_roles.role_name%type,
		O_RS OUT CURSOR_TYPE);

END MANAGE_USERS;
/

