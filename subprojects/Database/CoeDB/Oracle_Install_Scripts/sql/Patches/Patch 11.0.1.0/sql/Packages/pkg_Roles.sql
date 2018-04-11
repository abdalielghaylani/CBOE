--Copyright 1999-2010 CambridgeSoft Corporation. All rights reserved

CREATE OR REPLACE PACKAGE &&schemaname..Manage_Roles AS
   TYPE cursor_type IS REF CURSOR;

   ---Gets all Oracle roles
   PROCEDURE getAllRoles (o_rs OUT cursor_type);

   ---Gets all roles for a given privilege table
   ---Set pPrivTableName to NULL to get all roles in coedb
   PROCEDURE getRoles (
      pPrivTableName   IN       privilege_tables.privilege_table_name%TYPE,
      o_rs             OUT      cursor_type);

 ---Gets all roles for a given privilege table
   ---Set pPrivTableName to NULL to get all roles in coedb
   PROCEDURE getCOERoles (
      pCOEIdentifier   IN       security_roles.coeidentifier%TYPE,
      o_rs             OUT      cursor_type);

   ---Gets all roles NOT already granted to a given user within a given privilege table
   ---Set pPrivTableName to NULL to get all roles NOT already granted to a given user in all of CS_SECURITY
   PROCEDURE getAvailableRoles (
      pUserName        IN       people.user_id%TYPE,
      pPrivTableName   IN       privilege_tables.privilege_table_name%TYPE := NULL,
      o_rs             OUT      cursor_type);

 ---Gets all coe roles NOT already granted to a given user within a given privilege table
   ---Set pCOEIdentifier to NULL to get all roles NOT already granted to a given user in all of CS_SECURITY
   PROCEDURE getCOEAvailableRoles (
      pUserName        IN       people.user_id%TYPE,
      pCOEIdentifier   IN       security_roles.coeidentifier%TYPE := NULL,
      o_rs             OUT      cursor_type);



   ---Gets all roles granted to a given user within a given privilege table
   ---Set pPrivTableName to NULL to get all roles for a given user in all of CS_SECURITY
   PROCEDURE getUserRoles (
      pUserName        IN       people.user_id%TYPE,
      pPrivTableName   IN       privilege_tables.privilege_table_name%TYPE := NULL,
      o_rs             OUT      cursor_type);

---Gets all roles granted to a given user for a particular coeidentifier
   ---Set pCOEIdentifier to NULL to get all roles for a given user in all of coedb
   PROCEDURE getCOEUserRoles (
      pUserName        IN       people.user_id%TYPE,
      pCOEIdentifier   IN       security_roles.coeidentifier%TYPE := NULL,
      o_rs             OUT      cursor_type);


   ---Gets all roles granted to a given role
   PROCEDURE getRoleRoles (
      pRoleName   IN       security_roles.role_name%TYPE,
      o_rs        OUT      cursor_type);

   ---Gets all roles in a privTable available to a given role
   PROCEDURE getRoleAvailableRoles (
      pPrivTableName   IN       privilege_tables.privilege_table_name%TYPE,
      pRoleName        IN       security_roles.role_name%TYPE,
      o_rs             OUT      cursor_type);

 ---Gets all roles in a privTable available to a given role
   PROCEDURE getCOERoleAvailableRoles (
      pCOEIdentifier   IN       security_roles.coeidentifier%TYPE,
      pRoleName        IN       security_roles.role_name%TYPE,
      o_rs             OUT      cursor_type);

   ---Returns a single record whose fields contain the privilege names for that role
   PROCEDURE getRolePrivs (
      pRoleName   IN       security_roles.role_name%TYPE,
      o_rs        OUT      cursor_type);

   --return default privileges for a role with a particualr coeidentifier
   PROCEDURE getAppDefaultPrivs (
      pCOEIdentifier   IN       security_roles.coeidentifier%TYPE,
      o_rs        OUT      cursor_type);

   --return all application roles for a given user
   PROCEDURE GETALLAPPLICATIONPRIVILEGES (
      pUserName   IN       dba_role_privs.GRANTEE%TYPE,
      o_rs        OUT      cursor_type);
      
   ---Updates roles granted to a given role
   FUNCTION updateRolesGrantedToRole (
      pRoleName       IN   security_roles.role_name%TYPE,
      pRolesGranted   IN   VARCHAR2 := NULL,
      pRolesRevoked   IN   VARCHAR2 := NULL)
      RETURN VARCHAR2;

   ---Updates users granted a given role
   FUNCTION updateUsersGrantedARole (
      pRoleName       IN   security_roles.role_name%TYPE,
      pUsersGranted   IN   VARCHAR2 := NULL,
      pUsersRevoked   IN   VARCHAR2 := NULL)
      RETURN VARCHAR2;
END manage_roles;
/


CREATE OR REPLACE PACKAGE BODY &&schemaname..Manage_Roles AS
   FUNCTION excludeFGRolesSql RETURN VARCHAR2 AS
      n   NUMBER;
   BEGIN
      SELECT COUNT (*)
        INTO n
        FROM dba_tables
       WHERE table_name = UPPER ('BIOSAR_BROWSER_PRIVILEGES');

      IF n = 1 THEN
         RETURN 'role_id NOT IN (select role_internal_id from biosar_browser_privileges where is_formgroup_role = 1)';
      ELSE
         RETURN '1=1';
      END IF;
   END excludeFGRolesSql;

   PROCEDURE getAllRoles (o_rs OUT cursor_type) AS
   BEGIN
      OPEN o_rs FOR
         SELECT role AS role_name FROM dba_roles
         MINUS
         SELECT role_name FROM security_roles
         ORDER BY 1 ASC;
   END getAllRoles;

   PROCEDURE getRoles (
      pPrivTableName   IN       privilege_tables.privilege_table_name%TYPE,
      o_rs             OUT      cursor_type)
   AS
      mySql   VARCHAR (2000);
   BEGIN
      IF pPrivTableName IS NOT NULL THEN
         mySql := 'SELECT Distinct ROLE_NAME
					 FROM security_roles s, privilege_tables p
					WHERE p.privilege_table_id = s.privilege_table_int_id
					  AND Upper(p.privilege_table_name) = Upper(''' || pPrivTableName || ''')
					  AND ' || excludeFGRolesSql
               || ' ORDER BY ROLE_NAME ASC';
      ELSE
         mySql := 'SELECT Distinct ROLE_NAME FROM security_roles  
		            WHERE ' || excludeFGRolesSql
               || ' ORDER BY ROLE_NAME';
      END IF;

      OPEN o_rs FOR mySql;
   END getRoles;

PROCEDURE getCOERoles (
      pCOEIdentifier   IN       security_roles.coeidentifier%TYPE,
      o_rs             OUT      cursor_type)
   AS
      mySql   VARCHAR (2000);
   BEGIN
      IF pCOEIdentifier IS NOT NULL THEN
         mySql := 'SELECT Distinct ROLE_NAME
					 FROM security_roles s, privilege_tables p
					WHERE p.privilege_table_id = s.privilege_table_int_id
					AND  Upper(s.coeidentifier) = Upper(''' || pCOEIdentifier || ''')
					  AND ' || excludeFGRolesSql
               || ' ORDER BY ROLE_NAME ASC';
      ELSE
         mySql := 'SELECT Distinct ROLE_NAME FROM security_roles
		            WHERE ' || excludeFGRolesSql
               || ' ORDER BY ROLE_NAME';
      END IF;

      OPEN o_rs FOR mySql;
   END getCOERoles;



   PROCEDURE getAvailableRoles (
      pUserName        IN       people.user_id%TYPE,
      pPrivTableName   IN       privilege_tables.privilege_table_name%TYPE := NULL,
      o_rs             OUT      cursor_type)
   AS
      mySql   VARCHAR (2000);
   BEGIN
      mySql := 'SELECT DISTINCT ROLE_NAME
				  FROM   security_roles s
				  WHERE ' || excludeFGRolesSql || '
				 MINUS
				  SELECT ROLE_NAME
				  FROM   security_roles s
				  WHERE  s.role_name IN (SELECT granted_role 
										 FROM   dba_role_privs
										 WHERE	Upper(grantee) = Upper(''' || pusername || ''')
										   AND  Upper(dba_role_privs.granted_role) IN ( SELECT DISTINCT ROLE_NAME
																						FROM   security_roles s, privilege_tables p
																						WHERE  s.privilege_table_int_id = p.privilege_table_id
																						  AND  p.privilege_table_name LIKE NVL(''' || pprivtablename || ''', ''%'')))
				  ORDER BY 1 ASC';

      OPEN o_rs FOR mySql;
   END getAvailableRoles;



   PROCEDURE getCOEAvailableRoles (
      pUserName        IN       people.user_id%TYPE,
      pCOEIdentifier   IN       security_roles.coeidentifier%TYPE := NULL,
      o_rs             OUT      cursor_type)
   AS
      mySql   VARCHAR (2000);
   BEGIN                    
	IF (pCOEIdentifier IS NULL) OR (pCOEIdentifier = '') THEN
    mySql := 'SELECT DISTINCT ROLE_NAME
				  FROM   security_roles s
				  WHERE ' || excludeFGRolesSql || '
				 MINUS
				  SELECT ROLE_NAME
				  FROM   security_roles s
				  WHERE  s.role_name IN (SELECT granted_role 
										 FROM   dba_role_privs
										 WHERE	Upper(grantee) = Upper(''' || pusername || ''')
										   AND  Upper(dba_role_privs.granted_role) IN ( SELECT DISTINCT ROLE_NAME
																						FROM   security_roles s
																						WHERE  s.coeidentifier LIKE NVL(''' || pCOEIdentifier || ''', ''%'') OR s.coeidentifier IS NULL))
																						  
				  ORDER BY 1 ASC';
	ELSE
	mySql := 'SELECT DISTINCT ROLE_NAME
				  FROM   security_roles s
				  WHERE ' || excludeFGRolesSql || '
				 MINUS
				  SELECT ROLE_NAME
				  FROM   security_roles s
				  WHERE  s.role_name IN (SELECT granted_role 
										 FROM   dba_role_privs
										 WHERE	Upper(grantee) = Upper(''' || pusername || ''')
										   AND  Upper(dba_role_privs.granted_role) IN ( SELECT DISTINCT ROLE_NAME
																						FROM   security_roles s
																						WHERE  s.coeidentifier LIKE NVL(''' || pCOEIdentifier || ''', ''%'')))
																						  
				  ORDER BY 1 ASC';
	  END IF;

      OPEN o_rs FOR mySql;
   END getCOEAvailableRoles;


   PROCEDURE getUserRoles (
      pUserName        IN       people.user_id%TYPE,
      pPrivTableName   IN       privilege_tables.privilege_table_name%TYPE := NULL,
      o_rs             OUT      cursor_type)
   AS
   BEGIN
      OPEN o_rs FOR
         SELECT   granted_role AS role_name
             FROM dba_role_privs
            WHERE UPPER (grantee) = UPPER (pUserName)
              AND UPPER (dba_role_privs.granted_role) IN (
                     SELECT DISTINCT role_name
                                FROM security_roles s, privilege_tables p
                               WHERE s.privilege_table_int_id = p.privilege_table_id
                                 AND p.privilege_table_name LIKE NVL (pprivtablename, '%'))
         ORDER BY role_name ASC;
   END getUserRoles;


 PROCEDURE getCOEUserRoles (
      pUserName        IN       people.user_id%TYPE,
      pCOEIdentifier   IN       security_roles.coeidentifier%TYPE := NULL,
      o_rs             OUT      cursor_type)
   AS
   BEGIN
      OPEN o_rs FOR
         SELECT   granted_role AS role_name
             FROM dba_role_privs
            WHERE UPPER (grantee) = UPPER (pUserName)
              AND UPPER (dba_role_privs.granted_role) IN (
                     SELECT DISTINCT role_name
                                FROM security_roles s
                               WHERE s.coeIdentifier LIKE NVL (pCOEIdentifier, '%'))
                                
         ORDER BY role_name ASC;
   END getCOEUserRoles;



   PROCEDURE getRoleRoles (
      pRoleName   IN       security_roles.role_name%TYPE,
      o_rs        OUT      cursor_type)
   AS
   BEGIN
      OPEN o_rs FOR
         SELECT   granted_role AS role_name
             FROM dba_role_privs
            WHERE UPPER (grantee) = UPPER (pRoleName)
         ORDER BY role_name ASC;
   END getRoleRoles;



   PROCEDURE getRoleAvailableRoles (
      pPrivTableName   IN       privilege_tables.privilege_table_name%TYPE,
      pRoleName        IN       security_roles.role_name%TYPE,
      o_rs             OUT      cursor_type
   )
   AS
      mySql   VARCHAR (2000);
   BEGIN
      mySql := 'Select Distinct ROLE_NAME
					FROM security_roles s, privilege_tables p
					WHERE  s.privilege_table_int_id = p.privilege_table_id
					AND Upper(p.privilege_table_name) LIKE NVL(Upper(''' || pPrivTableName || '''), ''%'')
					AND role_name <> ''' || pRoleName || '''
					AND role_name NOT IN (Select granted_role AS ROLE_NAME from dba_role_privs
										  WHERE Upper(grantee) = Upper(''' || pRoleName || ''')
										  ) 
					AND ' || excludeFGRolesSql
            || ' ORDER BY ROLE_NAME ASC';

      OPEN o_rs FOR mySql;
   END getRoleAvailableRoles;


PROCEDURE getCOERoleAvailableRoles (
      pCOEIdentifier   IN       security_roles.coeidentifier%TYPE,
      pRoleName        IN       security_roles.role_name%TYPE,
      o_rs             OUT      cursor_type
   )
   AS
      mySql   VARCHAR (2000);
   BEGIN
      mySql := 'Select Distinct ROLE_NAME
					FROM security_roles s
					WHERE AND Upper(s.coeidentifier) LIKE NVL(Upper(''' || pCOEIdentifier || '''), ''%'')					
					AND role_name <> ''' || pRoleName || '''
					AND role_name NOT IN (Select granted_role AS ROLE_NAME from dba_role_privs
										  WHERE Upper(grantee) = Upper(''' || pRoleName || ''')
										  ) 
					AND ' || excludeFGRolesSql
            || ' ORDER BY ROLE_NAME ASC';

      OPEN o_rs FOR mySql;
   END getCOERoleAvailableRoles;


   PROCEDURE getRolePrivs (
      pRoleName   IN       security_roles.role_name%TYPE,
      o_rs        OUT      cursor_type)
   AS
      roleId          INTEGER;
      privTableName   VARCHAR2 (30);
      my_sql          VARCHAR2 (2000);
   BEGIN
      SELECT s.role_id, p.privilege_table_name
        INTO roleId, privTableName
        FROM security_roles s, privilege_tables p
       WHERE s.privilege_table_int_id = p.privilege_table_id
         AND UPPER (s.role_name) = UPPER (prolename);

      my_sql := 'SELECT * FROM ' || privTableName || ' WHERE ROLE_INTERNAL_ID = ' || TO_CHAR (roleId);

      OPEN o_rs FOR my_sql;
   END getRolePrivs;


PROCEDURE getAppDefaultPrivs (
      pCOEIdentifier   IN       security_roles.coeidentifier%TYPE,
      o_rs        OUT      cursor_type)
   AS
      roleId          INTEGER;
      privTableName   VARCHAR2 (30);
      my_sql          VARCHAR2 (2000);
   BEGIN
      SELECT distinct p.privilege_table_name, max(role_id) role_id
        INTO privTableName, roleId
        FROM security_roles s, privilege_tables p
       WHERE s.privilege_table_int_id = p.privilege_table_id
         AND UPPER (s.coeidentifier) = UPPER (pCOEIdentifier)
	GROUP BY  p.privilege_table_name;

      my_sql := 'SELECT * FROM ' || privTableName || ' WHERE ROLE_INTERNAL_ID = ' || TO_CHAR (roleId);

      OPEN o_rs FOR my_sql;
   END getAppDefaultPrivs;

  PROCEDURE GETALLAPPLICATIONPRIVILEGES (
      pUserName   IN       dba_role_privs.GRANTEE%TYPE,
      o_rs        OUT      cursor_type)
   AS
      CURSOR ROLES IS
      SELECT DISTINCT GRANTED_ROLE, ROLE_ID, COEIDENTIFIER FROM DBA_ROLE_PRIVS, SECURITY_ROLES
		WHERE GRANTED_ROLE = ROLE_NAME
		CONNECT BY PRIOR granted_role = GRANTEE
		START WITH UPPER(grantee) = UPPER(pUserName);
      TYPE SQLARRAY IS TABLE OF VARCHAR2 (4000); 
      my_sql SQLARRAY := SQLARRAY();
      finalSQL varchar2(32767);
      desctab DBMS_SQL.DESC_TAB;
	  desccnt PLS_INTEGER;
      cur number;
      n number;      
      i number := 1;
   BEGIN
	  FOR APPROLE IN ROLES LOOP
	  	FOR PRIVTABLE IN (SELECT PRIVILEGE_TABLE_NAME FROM PRIVILEGE_TABLES WHERE PRIVILEGE_TABLE_NAME <> 'OTHER_PRIVILEGES') LOOP
		  	my_sql.extend;
	  	    my_sql(i) := '';
    		desctab.DELETE;
    		cur := DBMS_SQL.OPEN_CURSOR;
    		DBMS_SQL.PARSE (cur, 'SELECT * FROM ' || PRIVTABLE.PRIVILEGE_TABLE_NAME, DBMS_SQL.NATIVE);
			n := DBMS_SQL.EXECUTE(cur);
		    DBMS_SQL.DESCRIBE_COLUMNS (cur, desccnt, desctab);
		    DBMS_SQL.CLOSE_CURSOR(cur);
			FOR J IN 1..desccnt LOOP                      
				IF(desctab(j).col_precision = 1) THEN
					IF(length(my_sql(i)) > 8) THEN
			  			my_sql(i) := my_sql(i) || ' || '','' || decode(' || desctab(j).col_name || ', 1, ''' || desctab(j).col_name || ''', '''') ';
			  		ELSE
			  			my_sql(i) := my_sql(i) || 'decode(' || desctab(j).col_name|| ', 1, ''' || desctab(j).col_name || ''', '''') ';
			  		END IF;
			    END IF;
			END LOOP;
			IF(length(my_sql(i)) > 1) THEN
		  		my_sql(i) := 'SELECT ''' || APPROLE.COEIDENTIFIER || ''', ' || my_sql(i) || ' FROM ' || PRIVTABLE.PRIVILEGE_TABLE_NAME || ' WHERE ROLE_INTERNAL_ID = ' || APPROLE.ROLE_ID;
	  		END IF;
	  		i := i + 1;
	  	END LOOP;
	  END LOOP;
	  i := 1;
	  FOR privSQL IN 1 .. my_sql.count LOOP
	  	IF(i = 1) THEN
		  	finalSQL := '(' || my_sql(i) || ')';
		ELSE   
			finalSQL := finalSQL || ' UNION (' ||  my_sql(i) || ')';
		END IF;
	  	i := i + 1;
	  END LOOP;
      OPEN o_rs FOR finalSQL;
   END GETALLAPPLICATIONPRIVILEGES;
   
   FUNCTION updateRolesGrantedToRole (
      pRoleName       IN   security_roles.role_name%TYPE,
      pRolesGranted   IN   VARCHAR2 := NULL,
      pRolesRevoked   IN   VARCHAR2 := NULL)
      RETURN VARCHAR2
   AS
      source_cursor    INTEGER;
      rows_processed   INTEGER;
   BEGIN
      source_cursor := DBMS_SQL.open_cursor;

      IF (prolesrevoked IS NOT NULL) THEN
         DBMS_SQL.parse (source_cursor, 'REVOKE ' || prolesrevoked || ' FROM ' || prolename, DBMS_SQL.native);
         rows_processed := DBMS_SQL.EXECUTE (source_cursor);
      END IF;

      IF (prolesgranted IS NOT NULL) THEN
         DBMS_SQL.parse (source_cursor, 'GRANT ' || prolesgranted || ' TO ' || prolename, DBMS_SQL.native);
         rows_processed := DBMS_SQL.EXECUTE (source_cursor);
      END IF;

      RETURN '1';
   END updateRolesGrantedToRole;

   FUNCTION updateUsersGrantedARole (
      pRoleName       IN   security_roles.role_name%TYPE,
      pUsersGranted   IN   VARCHAR2 := NULL,
      pUsersRevoked   IN   VARCHAR2 := NULL)
      RETURN VARCHAR2
   AS
      source_cursor    INTEGER;
      rows_processed   INTEGER;
   BEGIN
      source_cursor := DBMS_SQL.open_cursor;

      IF (pUsersRevoked IS NOT NULL) THEN
         DBMS_SQL.parse (source_cursor, 'REVOKE ' || pRoleName || ' FROM ' || pUsersRevoked, DBMS_SQL.native);
         rows_processed := DBMS_SQL.EXECUTE (source_cursor);
      END IF;

      IF (pUsersGranted IS NOT NULL) THEN
         DBMS_SQL.parse (source_cursor, 'GRANT ' || pRoleName || ' TO ' || pUsersGranted, DBMS_SQL.native);
         rows_processed := DBMS_SQL.EXECUTE (source_cursor);
      END IF;

      RETURN '1';
   END updateUsersGrantedARole;
END Manage_Roles;
/
