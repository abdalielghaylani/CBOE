--Copyright 1999-2003 CambridgeSoft Corporation. All rights reserved

CREATE OR REPLACE  PACKAGE "CS_SECURITY"."LOGIN"
AS

	TYPE  CURSOR_TYPE IS REF CURSOR;
	--This package is used during the COWS login process and should be made available to every user

	--Gets role ids granted to a given user within a given privilege table
	--Set pPrivTableName to NULL to get all roles for a given user in all of CS_SECURITY
	PROCEDURE GETUSERROLEIDS(
		pUserName IN people.User_id%type,
		pPrivTableName IN privilege_tables.privilege_table_name%type,
		O_RS OUT CURSOR_TYPE);

	---Returns a single record with the boolean values of the privileges                                   
	PROCEDURE GETPRIVS(
		pPrivTableName IN privilege_tables.privilege_table_name%type,
		O_RS OUT CURSOR_TYPE);

	---Returns a recordset with all privilege values for the given role ids
	PROCEDURE GETPRIVSBYROLEID(
		pRoleIDList IN varchar2,
		pPrivTableName IN privilege_tables.privilege_table_name%type,
		O_RS OUT CURSOR_TYPE);

	--- Returns a person_id for a given user name
	FUNCTION GETPERSONID(
		pUserName IN people.User_ID%type) RETURN people.person_id%type;

END LOGIN;
/

