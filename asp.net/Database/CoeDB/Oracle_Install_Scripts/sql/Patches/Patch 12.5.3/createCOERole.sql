CREATE OR REPLACE FUNCTION &&schemaName..createCOERole (
   pRoleName            IN   security_roles.role_name%TYPE,
   pPrivTableName       IN   privilege_tables.privilege_table_name%TYPE,
   pIsAlreadyInOracle   IN   INTEGER,
   pPrivValueList       IN   VARCHAR2,
    
   pCOEIdentifier       IN   security_roles.coeidentifier%TYPE)

   RETURN VARCHAR2
AS
   privTableId   INTEGER;
   roleId        INTEGER;
   rolenamecount NUMBER;   
   usernamecount NUMBER;
   
BEGIN

   IF pIsAlreadyInOracle = 0 THEN
      SELECT privilege_table_id
        INTO privTableId
        FROM privilege_tables
       WHERE UPPER (privilege_table_name) = UPPER (pPrivTableName);

     SELECT count(*) INTO usernamecount FROM people p WHERE UPPER(p.user_id) = Upper(pRoleName); 
     SELECT count(*) INTO rolenamecount FROM security_roles s WHERE UPPER(s.role_name) = Upper(pRoleName);
      
if rolenamecount = 0  and usernamecount = 0 then
      INSERT INTO security_roles (privilege_table_int_id, role_name, coeidentifier)
           VALUES (privTableId, UPPER (pRoleName), UPPER (pCOEIdentifier))
        RETURNING role_id
             INTO roleId;

      EXECUTE IMMEDIATE 'INSERT INTO ' || pPrivTableName || ' VALUES ( ' || roleId || ', ' || pPrivValueList || ')'; 
      EXECUTE IMMEDIATE 'CREATE ROLE ' || pRoleName || ' NOT IDENTIFIED'; 
	    EXECUTE IMMEDIATE 'GRANT EXECUTE ON COEDB.LOGIN TO ' || pRoleName;
      --EXECUTE IMMEDIATE 'GRANT CSS_USER TO ' || pRoleName;
      EXECUTE IMMEDIATE 'REVOKE ' || pRoleName || ' FROM COEDB';
      GrantPrivsToRole2(pRoleName); --CSBR 127261 : To fix the issue related to new role creation and assigning the role to a new user.
   
    else
      RETURN 'Rolename ' || pRoleName || ' conflicts with another user or role name';
       end if; 
   ELSE
      INSERT INTO security_roles (privilege_table_int_id, role_name, coeidentifier)
           VALUES (NULL, UPPER (pRoleName), UPPER (pCOEIdentifier))
        RETURNING role_id
             INTO roleId;
   END IF;

   RETURN '1';  
   
END createCOERole;
/