CREATE OR REPLACE PACKAGE BODY RLS
AS
  FUNCTION SetPredicate (
  	p_schema VARCHAR2,
    p_name VARCHAR2) RETURN VARCHAR2
  IS
    l_username  VARCHAR2(50);
    l_predicate VARCHAR2(2000);
  BEGIN
    -- you can also get the user-defined context variables here
    l_username := sys_context('USERENV', 'SESSION_USER');
    --' get inventory role_id

    -- this is where the magic
    -- you can also use the 'ename IN ( ... )' format
    l_predicate := 'LOCATION_ID NOT IN (SELECT location_id_fk FROM inv_role_locations WHERE role_id_fk IN ';
    l_predicate := l_predicate || '(Select Distinct role_id	FROM cs_security.security_roles s, cs_security.privilege_tables p WHERE  s.privilege_table_int_id = p.privilege_table_id AND p.privilege_table_name LIKE NVL(''CHEMINV_PRIVILEGES'', ''%'') AND role_name IN (SELECT granted_role AS ROLE_NAME from dba_role_privs WHERE Upper(grantee) = Upper(''' || l_username || '''))))';      


    RETURN   l_predicate;
  END SetPredicate;
    
END RLS;
/