CREATE OR REPLACE PACKAGE Login AS
   TYPE cursor_type IS REF CURSOR;

   --This package is used during the COWS login process and should be made available to every user

   --Gets role ids granted to a given user within a given privilege table
   --Set pPrivTableName to NULL to get all roles for a given user in all of CS_SECURITY
   PROCEDURE getUserRoleIds (
      pUserName        IN       people.user_id%TYPE,
      pPrivTableName   IN       privilege_tables.privilege_table_name%TYPE,
      o_rs             OUT      cursor_type);

   --Returns a single record with the boolean values of the privileges
   PROCEDURE getPrivs (
      pPrivTableName   IN       privilege_tables.privilege_table_name%TYPE,
      o_rs             OUT      cursor_type);

   --Returns a recordset with all privilege values for the given role ids
   PROCEDURE getPrivsByRoleId (
      pRoleIdList      IN       VARCHAR2,
      pPrivTableName   IN       privilege_tables.privilege_table_name%TYPE,
      o_rs             OUT      cursor_type);

   --Returns a CSV-string of privilege names associated with the given a user name
   PROCEDURE getPrivsByUser (
      pUserName IN VARCHAR2
      , o_rs OUT cursor_type);

   --Returns a person_id for a given user name
   FUNCTION getPersonId (pUserName IN people.user_id%TYPE)
      RETURN people.person_id%TYPE;
END Login;
/
CREATE OR REPLACE PACKAGE BODY Login IS
   PROCEDURE getPrivsByRoleId (
      pRoleIdList      IN       VARCHAR2,
      pPrivTableName   IN       privilege_tables.privilege_table_name%TYPE,
      o_rs             OUT      cursor_type)
   AS
      my_sql   VARCHAR2 (2000);
   BEGIN
      my_sql := 'SELECT * FROM ' || pPrivTableName || ' WHERE ROLE_INTERNAL_ID IN ( ' || pRoleIdList || ')';

      OPEN o_rs FOR my_sql;
   END getPrivsByRoleId;

   PROCEDURE getPrivs (
      pPrivTableName   IN       privilege_tables.privilege_table_name%TYPE,
      o_rs             OUT      cursor_type)
   AS
      my_sql   VARCHAR2 (2000);
   BEGIN
      my_sql := 'SELECT * FROM ' || pPrivTableName || ' WHERE ROLE_INTERNAL_ID IS NULL';
      OPEN o_rs FOR my_sql;
   END getPrivs;

   PROCEDURE getUserRoleIds (
      pUserName        IN       people.user_id%TYPE,
      pPrivTableName   IN       privilege_tables.privilege_table_name%TYPE,
      o_rs             OUT      cursor_type)
   AS
   BEGIN
      OPEN o_rs FOR
         SELECT DISTINCT s.role_id
                    FROM security_roles s,
                         privilege_tables p,
                         dba_role_privs rp
                   WHERE s.privilege_table_int_id = p.privilege_table_id
                     AND s.role_name = rp.granted_role
                     AND UPPER (p.privilege_table_name) = pPrivTableName
                     AND rp.granted_role IN (
                            SELECT granted_role
                              FROM dba_role_privs
                             WHERE grantee = pUserName
                            UNION
                            SELECT granted_role
                              FROM dba_role_privs
                             WHERE grantee IN (SELECT granted_role
                                                 FROM dba_role_privs
                                                WHERE grantee = pUserName));
   END getUserRoleIds;

  PROCEDURE getPrivsByUser (
    pUserName IN VARCHAR2
    , o_rs OUT cursor_type
  )
  IS
    user_name VARCHAR2(100) := pUserName;
    tbl_name VARCHAR(100);
    role_internal_id NUMBER;
    coe_role_scope SECURITY_ROLES.COEIDENTIFIER%TYPE;

    --these will hold the list(s) of privileges
    /* not used at this time
    priv obj_privilege;
    */
    privs obj_privilege_table;
    all_privs obj_privilege_table := obj_privilege_table();

    --we're gonna need some dynamic sql here
    dyn_sql VARCHAR2(4000);
    --and something to hold the 'permission=1' values themselves
    /* not used at this time
    perm_buf CLOB;
    */
    --and a permission indicator
    perm NUMBER;
    --and a variable to increment the master/rollup privileges collection
    counter NUMBER := 1;

    CURSOR cur_roles IS
    SELECT DISTINCT
      rp.granted_role AS role_granted
      , sr.role_id AS role_id
      , sr.coeidentifier AS coe_role_scope
      , pt.privilege_table_name
      , pt.app_name
    FROM dba_role_privs rp
      INNER JOIN security_roles sr ON sr.role_name = rp.granted_role
      LEFT OUTER JOIN privilege_tables pt ON pt.privilege_table_id = sr.privilege_table_int_id
    WHERE upper(pt.privilege_table_name) <> 'OTHER_PRIVILEGES'
    CONNECT BY PRIOR granted_role = grantee
    START WITH upper(grantee) = upper(user_name);

  BEGIN

    FOR rec IN cur_roles LOOP
      coe_role_scope := rec.coe_role_scope;
      tbl_name := rec.privilege_table_name;
      role_internal_id := rec.role_id;
      --  dbms_output.put_line(coe_role_scope || ' ' || role_internal_id || ' ' || tbl_name);

      --first, get the column names, and a default 'has privilege' value of 0
      dyn_sql := 'select obj_privilege('''
        || coe_role_scope
        || ''',column_name,0) from user_tab_columns where table_name = '''
        || tbl_name
        || ''''
        || 'and data_precision = 1';

      --pump the query results into the table
      EXECUTE IMMEDIATE dyn_sql BULK COLLECT INTO privs;
      --  dbms_output.put_line(dyn_sql);

      --let's just stuff the 'has p[ermission' value back into the collection
      FOR i IN 1..privs.count LOOP
        dyn_sql := 'select count(*) from ' || tbl_name
          || ' where role_internal_id = ' || role_internal_id
          || ' and ' || privs(i).priv_name || ' = 1';
        EXECUTE IMMEDIATE dyn_sql INTO perm;
        privs(i).priv_value := perm;

        --  dbms_output.put_line(dyn_sql);

        --pack the permission into the master/rollup permissions table
        all_privs.extend;
        all_privs(counter) := privs(i);
        counter := counter + 1;
      END LOOP;

      -- this actually concatenates the granted permissions (prov_value = 1)
      --   BUT we'll get to that later as necessary
      /*
      FOR i IN 1..all_privs.count LOOP
        priv := all_privs(i);
        dbms_output.put_line( priv.priv_name || ' = ' || to_char(priv.priv_value) );
        IF (priv.priv_value = 1) THEN
          IF (trim(perm_buf) is null) THEN
             perm_buf := priv.priv_name;
          ELSE
             perm_buf := perm_buf || ',' || priv.priv_name;
          END IF;
        END IF;
      END LOOP;
      dbms_output.put_line( perm_buf );
      */
    END LOOP;

    OPEN o_rs FOR SELECT * FROM TABLE(all_privs); 
  END;

   FUNCTION getpersonid (pusername IN people.user_id%TYPE)  RETURN people.person_id%TYPE AS
      personId   people.person_id%TYPE;
   BEGIN
      SELECT person_id
        INTO personId
        FROM people
       WHERE UPPER (user_id) = UPPER (pUserName) AND active <> -1;

      RETURN personId;
   EXCEPTION
      WHEN NO_DATA_FOUND THEN
         RETURN 0;
   END getPersonId;
END Login;
/
