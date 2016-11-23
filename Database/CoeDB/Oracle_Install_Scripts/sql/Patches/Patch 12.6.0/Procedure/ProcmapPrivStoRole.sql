create or replace
PROCEDURE mapPrivStoRole (
   pRoleName   IN   security_roles.role_name%TYPE,
   pPrivName   IN   VARCHAR2,
   pAction     IN   VARCHAR2)
IS
   cannot_revoke   EXCEPTION;
   PRAGMA EXCEPTION_INIT (cannot_revoke, -1927);

   CURSOR privs_cur (privname_in IN VARCHAR2) IS
      SELECT   PRIVILEGE, SCHEMA, object_name
          FROM COEDB.object_privileges
         WHERE privilege_name = privname_in AND SCHEMA IS NOT NULL
      ORDER BY SCHEMA, PRIVILEGE;

   thePrivilege    VARCHAR (30);
   theSchema       VARCHAR (30);
   theObjectName   VARCHAR (30);
   mySql           VARCHAR2 (2000);
   keyword         VARCHAR2 (10);
   ObjectCount     INTEGER;
BEGIN
   IF pAction = 'GRANT' THEN
      keyword := ' TO ';
   ELSE
      keyword := ' FROM ';
   END IF;

   OPEN privs_cur (pPrivName);
   LOOP FETCH privs_cur INTO thePrivilege, theSchema, theObjectName;
   EXIT WHEN privs_cur%NOTFOUND;
    SELECT count(*) into ObjectCount FROM DBA_OBJECTS  where owner = theSchema and object_name =UPPER(theObjectName);
      IF ObjectCount > 0 THEN
        mySql := pAction || ' ' || thePrivilege || ' ON ' || theSchema || '.' || theObjectName || keyword || pRoleName;
        EXECUTE IMMEDIATE mySql;
       END IF; 
   END LOOP;

   CLOSE privs_cur;
EXCEPTION
   WHEN cannot_revoke THEN
      RETURN;
   WHEN OTHERS THEN
      raise_application_error (-20000, 'COEDB  does not have sufficient privileges to ' || mySql);
END mapPrivStoRole;
/