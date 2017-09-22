COLUMN X NOPRINT NEW_VALUE BioSar_r
SELECT case when COUNT(*)=0 then 'TRUE' else 'FALSE' end as X
        FROM dba_tables
       WHERE table_name = UPPER ('BIOSAR_BROWSER_PRIVILEGES');

CREATE OR REPLACE PACKAGE &&schemaName..Constant_of_Manage_Roles AS	 
	--  !!!!!!!!!!!!this package must be created before Manage_Roles package!!!!!!!!!!
	-- this package is used for correct working Manage_Roles package,  BioSar_roles depends on customer has BioSar/Datalytix or not. 
	-- Please, don't delete this package!!!
   -- if we have biosar_browser_privileges table BioSar_roles must be FALSE 
   -- if we don't have  biosar_browser_privileges table BioSar_roles must be TRUE
   
   BioSar_roles constant boolean := &&BioSar_r;
end;
/	   
show errors

CREATE OR REPLACE PACKAGE &&schemaName..Manage_Roles AS
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
show errors

CREATE OR REPLACE PACKAGE BODY &&schemaName..Manage_Roles AS
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

  FUNCTION excludeFGRolesSql_int RETURN integer AS
      n   NUMBER;
   BEGIN
      SELECT COUNT (*)
        INTO n
        FROM dba_tables
       WHERE table_name = UPPER ('BIOSAR_BROWSER_PRIVILEGES');

      RETURN n;
   END;

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
   BEGIN
   $if Constant_of_Manage_Roles.BioSar_roles $then 
      IF pPrivTableName IS NOT NULL THEN
       OPEN o_rs FOR
       SELECT Distinct ROLE_NAME
					 FROM security_roles s, privilege_tables p
					WHERE p.privilege_table_id = s.privilege_table_int_id
					AND  Upper(p.privilege_table_name)  = Upper(pPrivTableName)
					ORDER BY ROLE_NAME ASC;
	  ELSE
       OPEN o_rs FOR
       SELECT Distinct ROLE_NAME
					 FROM security_roles s
					ORDER BY ROLE_NAME ASC;
	  END IF;
   $ELSE

      IF pPrivTableName IS NOT NULL THEN
       OPEN o_rs FOR
       SELECT Distinct ROLE_NAME
					 FROM security_roles s, privilege_tables p
					WHERE p.privilege_table_id = s.privilege_table_int_id
					AND Upper(p.privilege_table_name) = Upper(pPrivTableName)
					and role_id NOT IN (select role_internal_id from biosar_browser_privileges where is_formgroup_role = 1)
					ORDER BY ROLE_NAME ASC;
	  ELSE
       OPEN o_rs FOR
       SELECT Distinct ROLE_NAME
					 FROM security_roles s where role_id NOT IN (select role_internal_id from biosar_browser_privileges where is_formgroup_role = 1)
					ORDER BY ROLE_NAME ASC;
	  END IF;
	$end

   END getRoles;

PROCEDURE getCOERoles (
      pCOEIdentifier   IN       security_roles.coeidentifier%TYPE,
      o_rs             OUT      cursor_type)
   AS
   BEGIN
   $if Constant_of_Manage_Roles.BioSar_roles $then 

         IF pCOEIdentifier IS NOT NULL THEN
           OPEN o_rs FOR
           SELECT Distinct ROLE_NAME
                         FROM security_roles s, privilege_tables p
                        WHERE p.privilege_table_id = s.privilege_table_int_id
                        AND  Upper(s.coeidentifier)  = Upper(pCOEIdentifier)
                         ORDER BY ROLE_NAME;
         ELSE
           OPEN o_rs FOR
            SELECT Distinct ROLE_NAME
                         FROM security_roles 
                         ORDER BY ROLE_NAME;
        
         end if;               
       $ELSE
         IF pCOEIdentifier IS NOT NULL THEN
           OPEN o_rs FOR
           SELECT Distinct ROLE_NAME
                         FROM security_roles s, privilege_tables p
                        WHERE p.privilege_table_id = s.privilege_table_int_id
                        AND Upper(s.coeidentifier) = Upper(pCOEIdentifier)
                        and role_id NOT IN (select role_internal_id from biosar_browser_privileges where is_formgroup_role = 1)
                         ORDER BY ROLE_NAME;
         ELSE
           OPEN o_rs FOR
            SELECT Distinct ROLE_NAME
                         FROM security_roles where role_id NOT IN (select role_internal_id from biosar_browser_privileges where is_formgroup_role = 1)
                         ORDER BY ROLE_NAME;

         end if;               
       $END

   END getCOERoles;



   PROCEDURE getAvailableRoles (
      pUserName        IN       people.user_id%TYPE,
      pPrivTableName   IN       privilege_tables.privilege_table_name%TYPE := NULL,
      o_rs             OUT      cursor_type)
   AS
      l_pprivtablename privilege_tables.privilege_table_name%TYPE := case when pprivtablename is null or pprivtablename = '' then '%' else pprivtablename end;
   BEGIN
   $if Constant_of_Manage_Roles.BioSar_roles $then 
        OPEN o_rs FOR
        select DISTINCT o.ROLE_NAME from 
                    (SELECT ROLE_NAME
                      FROM   security_roles s
                      MINUS
                    SELECT ROLE_NAME
          FROM   security_roles s, dba_role_privs rp, privilege_tables p 
          WHERE  s.role_name = rp.granted_role
               and Upper(rp.grantee) = Upper(pusername)
                     and s.privilege_table_int_id = p.privilege_table_id AND p.privilege_table_name LIKE l_pprivtablename) o
        ORDER BY 1 ASC;
       $else
        OPEN o_rs FOR
        select DISTINCT o.ROLE_NAME from 
                    (SELECT ROLE_NAME
                      FROM   security_roles s
                      where role_id NOT IN (select role_internal_id from biosar_browser_privileges where is_formgroup_role = 1)
                      MINUS
                    SELECT ROLE_NAME
          FROM   security_roles s, dba_role_privs rp, privilege_tables p 
          WHERE  s.role_name = rp.granted_role
               and Upper(rp.grantee) = Upper(pusername)
                     and s.privilege_table_int_id = p.privilege_table_id AND p.privilege_table_name LIKE l_pprivtablename) o
        ORDER BY 1 ASC;
       $end 
   END getAvailableRoles;


   -- CBOE-1793 modified queries to get distint roles. Also roles must present in dba_roles table. ASV 12SEP13
   PROCEDURE getCOEAvailableRoles (
      pUserName        IN       people.user_id%TYPE,
      pCOEIdentifier   IN       security_roles.coeidentifier%TYPE := NULL,
      o_rs             OUT      cursor_type)
   AS
      l_pCOEIdentifier security_roles.coeidentifier%TYPE := case when pCOEIdentifier = '' or pCOEIdentifier IS NULL then '%' else pCOEIdentifier end;
   BEGIN
   $if Constant_of_Manage_Roles.BioSar_roles $then 
        OPEN o_rs FOR
        select DISTINCT o.ROLE_NAME from 
                (SELECT ROLE_NAME
				  FROM   security_roles s
				  WHERE  s.PRIVILEGE_TABLE_INT_ID IS NOT NULL 
			    MINUS
                select 
                  s.ROLE_NAME from 
                 security_roles  s,
                 (select --+ no_query_transformation 
                 drp.granted_role,drp.grantee from dba_role_privs drp, security_roles s1 
                 where  Upper(drp.granted_role) = s1.ROLE_NAME and s1.coeidentifier LIKE l_pCOEIdentifier) drp
                             where s.role_name = drp.granted_role and Upper(drp.grantee) = UPPER(pUserName)
                                        and  s.PRIVILEGE_TABLE_INT_ID IS NOT NULL
                ) o, DBA_ROLES dr where o.ROLE_NAME = dr.ROLE  
                 ORDER BY 1 ASC;

    $else
        OPEN o_rs FOR
        select DISTINCT o.ROLE_NAME from 
                (SELECT ROLE_NAME
				  FROM   security_roles s
				  WHERE  s.PRIVILEGE_TABLE_INT_ID IS NOT NULL AND role_id NOT IN (select role_internal_id from biosar_browser_privileges where is_formgroup_role = 1)
			    MINUS
                select 
                  s.ROLE_NAME from 
                 security_roles  s,
                 (select --+ no_query_transformation 
                 drp.granted_role,drp.grantee from dba_role_privs drp, security_roles s1 
                 where  Upper(drp.granted_role) = s1.ROLE_NAME and s1.coeidentifier LIKE l_pCOEIdentifier) drp
                             where s.role_name = drp.granted_role and Upper(drp.grantee) = UPPER(pUserName)
                                        and  s.PRIVILEGE_TABLE_INT_ID IS NOT NULL
                ) o, DBA_ROLES dr where o.ROLE_NAME = dr.ROLE  
                 ORDER BY 1 ASC;
    $end 
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
         SELECT --+ no_query_transformation 
           granted_role AS role_name
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
      l_pPrivTableName privilege_tables.privilege_table_name%TYPE := NVL(Upper(pPrivTableName), '%');
   BEGIN
   $if Constant_of_Manage_Roles.BioSar_roles $then 
        OPEN o_rs FOR
        Select Distinct ROLE_NAME
					FROM security_roles s, privilege_tables p
					WHERE  s.privilege_table_int_id = p.privilege_table_id
					AND Upper(p.privilege_table_name) LIKE l_pPrivTableName
					AND role_name <> pRoleName
					AND role_name NOT IN (Select granted_role AS ROLE_NAME from dba_role_privs
										  WHERE Upper(grantee) = Upper(pRoleName))
					ORDER BY ROLE_NAME ASC;
       $else
        OPEN o_rs FOR
        Select Distinct ROLE_NAME
					FROM security_roles s, privilege_tables p
					WHERE  s.privilege_table_int_id = p.privilege_table_id
					AND Upper(p.privilege_table_name) LIKE l_pPrivTableName
					AND role_name <> pRoleName
					AND role_name NOT IN (Select granted_role AS ROLE_NAME from dba_role_privs
										  WHERE Upper(grantee) = Upper(pRoleName))
					AND role_id NOT IN (select role_internal_id from biosar_browser_privileges where is_formgroup_role = 1) ORDER BY ROLE_NAME ASC;

       $end 

   END getRoleAvailableRoles;


PROCEDURE getCOERoleAvailableRoles (
      pCOEIdentifier   IN       security_roles.coeidentifier%TYPE,
      pRoleName        IN       security_roles.role_name%TYPE,
      o_rs             OUT      cursor_type
   )
   AS
      l_pCOEIdentifier  security_roles.coeidentifier%TYPE := NVL(Upper(pCOEIdentifier), '%');
   BEGIN

   $if Constant_of_Manage_Roles.BioSar_roles $then 
        OPEN o_rs FOR
        Select Distinct ROLE_NAME
					FROM security_roles s
					WHERE Upper(s.coeidentifier) LIKE l_pCOEIdentifier
					AND role_name <> pRoleName
					AND role_name NOT IN (Select granted_role AS ROLE_NAME from dba_role_privs
										  WHERE Upper(grantee) = Upper(pRoleName)
										  )
					ORDER BY ROLE_NAME ASC;
	   $else
        OPEN o_rs FOR
        Select Distinct ROLE_NAME
					FROM security_roles s
					WHERE Upper(s.coeidentifier) LIKE l_pCOEIdentifier
					AND role_name <> pRoleName
					AND role_name NOT IN (Select granted_role AS ROLE_NAME from dba_role_privs
										  WHERE Upper(grantee) = Upper(pRoleName)
										  )
					AND role_id NOT IN (select role_internal_id from biosar_browser_privileges where is_formgroup_role = 1) ORDER BY ROLE_NAME ASC;
	  
	   $end 

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
show errors
