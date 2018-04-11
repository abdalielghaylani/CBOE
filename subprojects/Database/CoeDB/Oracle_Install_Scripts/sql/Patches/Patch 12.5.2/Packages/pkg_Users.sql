--Copyright 1999-2003 CambridgeSoft Corporation. All rights reserved

CREATE OR REPLACE PACKAGE &&schemaname..Manage_Users AS
   TYPE cursor_type IS REF CURSOR;

   --Returns all Oracle users
   PROCEDURE getAllUsers (o_rs OUT cursor_type);

   --Returns all CSS users for a given PrivTable.
   --Use PrivTableName = Null to get all CSS users
   PROCEDURE getUsers (
      pprivtablename   IN       privilege_tables.privilege_table_name%TYPE,
      o_rs             OUT      cursor_type);

 --Returns all CSS users for a given coeidentifier
   --Use coeidentifier= Null to get all coe users
   PROCEDURE getCOEUsers (
      pCOEIdentifier   IN       security_roles.coeidentifier%TYPE,
      o_rs             OUT      cursor_type);

   --Returns all attributes of a single user
   PROCEDURE getUser (pusername IN people.user_id%TYPE, o_rs OUT cursor_type);

   --Gets grantees of a given role
   PROCEDURE getRoleGrantees (
      prolename   IN       security_roles.role_name%TYPE,
      o_rs        OUT      cursor_type);

   --Gets users that are not yet grantees to a role
   PROCEDURE getRoleAvailableUsers (
      pprivtablename   IN       privilege_tables.privilege_table_name%TYPE,
      prolename        IN       security_roles.role_name%TYPE,
      o_rs             OUT      cursor_type);
END Manage_Users;
/


CREATE OR REPLACE PACKAGE BODY &&schemaname..Manage_Users
AS
   PROCEDURE getUsers (
      pPrivTableName   IN       privilege_tables.privilege_table_name%TYPE,
      o_rs             OUT      cursor_type
   )
   AS
   BEGIN
      OPEN o_rs FOR
         SELECT DISTINCT grantee AS username, first_name AS firstname, last_name AS lastname
                    FROM dba_role_privs, people
                   WHERE UPPER (dba_role_privs.grantee) = UPPER (people.user_id)
                     AND UPPER (dba_role_privs.granted_role) IN (
                            SELECT UPPER (role_name)
                              FROM security_roles s, privilege_tables p
                             WHERE s.privilege_table_int_id = p.privilege_table_id
                               AND p.privilege_table_name LIKE NVL (pprivtablename, '%'));
   END getUsers;







  PROCEDURE getCOEUsers (
      pCOEIdentifier   IN       security_roles.coeidentifier%TYPE,
      o_rs             OUT      cursor_type
   )
   AS
   BEGIN
      OPEN o_rs FOR
         SELECT DISTINCT grantee AS username, first_name AS firstname, last_name AS lastname,
              Person_id, first_name || ' ' || last_name AS FULL_NAME      FROM dba_role_privs, people
                   WHERE UPPER (dba_role_privs.grantee) = UPPER (people.user_id)
                     AND UPPER (dba_role_privs.granted_role) IN (
                            SELECT UPPER (role_name)
                              FROM security_roles s
                             WHERE UPPER(s.coeidentifier) LIKE UPPER(NVL ( pCOEIdentifier, '%'))) ORDER BY username ASC;

   END getCOEUsers;


   PROCEDURE getAllUsers (o_rs OUT cursor_type)
   AS
   BEGIN
      OPEN o_rs FOR
         SELECT   UPPER (username) AS userName
             FROM dba_users
            WHERE username NOT IN ('SYSTEM', 'SYS', 'SYSMAN')
         MINUS
         SELECT   UPPER (user_id) AS userName
             FROM people
         ORDER BY 1 ASC;
   END getAllUsers;

   PROCEDURE getUser (pUserName IN people.user_id%TYPE, o_rs OUT cursor_type)
   AS
      pw       VARCHAR2 (30);
   BEGIN
      SELECT PASSWORD
        INTO pw
        FROM dba_users
       WHERE UPPER (userName) = UPPER (pUserName);

      OPEN o_rs FOR 
      	SELECT Person_ID AS PersonID, User_ID AS UserName, pw AS Password, 
      		First_Name AS FirstName, Middle_Name AS MiddleName, Last_Name AS LastName, 
      		Email AS Email, Telephone AS Telephone, Int_Address AS Address, User_Code AS UserCode, 
      		Supervisor_internal_id AS SuperVisorID, Title AS Title, Department AS Department, 
      		Site_id AS SiteID, Active AS isActive 
      	FROM People WHERE upper(User_ID) = UPPER (pUserName);
   END getUser;

   PROCEDURE getRoleGrantees (
      pRoleName   IN       security_roles.role_name%TYPE,
      o_rs        OUT      cursor_type
   )
   AS
   BEGIN
      OPEN o_rs FOR
         SELECT DISTINCT grantee as username
                    FROM dba_role_privs, people
                    WHERE UPPER(grantee) = UPPER(User_id) AND UPPER (granted_role) = UPPER (pRoleName);
   END getrolegrantees;

   PROCEDURE getRoleAvailableUsers (
      pPrivTableName   IN       privilege_tables.privilege_table_name%TYPE,
      pRoleName        IN       security_roles.role_name%TYPE,
      o_rs             OUT      cursor_type
   )
   AS
   BEGIN
      OPEN o_rs FOR
         SELECT DISTINCT grantee AS userName
                    FROM dba_role_privs, people
                   WHERE UPPER (dba_role_privs.grantee) = UPPER (people.user_id)
                     AND UPPER (dba_role_privs.granted_role) IN (
                            SELECT UPPER (role_name)
                              FROM security_roles s, privilege_tables p
                             WHERE s.privilege_table_int_id = p.privilege_table_id
                               AND p.privilege_table_name LIKE NVL (pprivtablename, '%'))
                     AND grantee NOT IN (
                                 SELECT DISTINCT grantee
                                            FROM dba_role_privs
                                           WHERE UPPER (granted_role) = UPPER (prolename))
         order by grantee;
   END getRoleAvailableUsers;
END Manage_Users;
/
