--Copyright 1999-2003 CambridgeSoft Corporation. All rights reserved

CREATE OR REPLACE  PACKAGE "CS_SECURITY"."MANAGE_ROLES"                                             
AS

TYPE CURSOR_TYPE IS REF CURSOR;

	--- Gets all Oracle roles
	PROCEDURE GETALLROLES(
		O_RS OUT CURSOR_TYPE);

	---Gets all roles for a given privilege table
	---Set pPrivTableName to NULL to get all roles in CS_SECURITY
	PROCEDURE GETROLES(
		pPrivTableName IN privilege_tables.privilege_table_name%type,
		O_RS OUT CURSOR_TYPE);
	 
	---Gets all roles NOT already granted to a given user within a given privilege table
	---Set pPrivTableName to NULL to get all roles NOT already granted to a given user in all of CS_SECURITY          
	PROCEDURE GETAVAILABLEROLES (
		pUserName IN people.User_id%type,
		pPrivTableName IN privilege_tables.privilege_table_name%type:=NULL,
		O_RS OUT CURSOR_TYPE);

	---Gets all roles granted to a given user within a given privilege table
	---Set pPrivTableName to NULL to get all roles for a given user in all of CS_SECURITY      
	PROCEDURE GETUSERROLES (
		pUserName IN people.User_id%type,
		pPrivTableName IN privilege_tables.privilege_table_name%type:=NULL,
		O_RS OUT CURSOR_TYPE);
   
	--Gets all roles granted to a given role
	PROCEDURE GETROLEROLES(
		pRoleName IN security_roles.Role_name%type,
		O_RS OUT CURSOR_TYPE);	
	
	--Gets all roles in a privTable available to a given role
	PROCEDURE GETROLEAVAILABLEROLES(
		pPrivTableName IN privilege_tables.privilege_table_name%type,
		pRoleName IN security_roles.Role_name%type,
		O_RS OUT CURSOR_TYPE);

	---Returns a single record whose fields contain the privilege names for that role                        
	PROCEDURE GETROLEPRIVS(
		pRoleName IN Security_Roles.Role_Name%type,
		O_RS OUT CURSOR_TYPE);

	---Updates roles granted to a given role
	FUNCTION UPDATEROLESGRANTEDTOROLE(
		pRoleName IN security_roles.role_name%type,
		pRolesGranted IN varchar2:=NULL,
		pRolesRevoked IN varchar2:=NULL) RETURN varchar2;
	
	--Updates users granted a given role
	FUNCTION UPDATEUSERSGRANTEDAROLE(
		pRoleName IN security_roles.role_name%type,
		pUsersGranted IN varchar2:=NULL,
		pUsersRevoked IN varchar2:=NULL) RETURN varchar2;

END MANAGE_ROLES;
/


