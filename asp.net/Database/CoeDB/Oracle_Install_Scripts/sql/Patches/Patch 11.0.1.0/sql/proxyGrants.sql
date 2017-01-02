--Copyright 1999-2010 CambridgeSoft Corporation. All rights reserved

PROMPT Starting proxyGrants.sql

DECLARE
   lSql   VARCHAR2 (2000);

   CURSOR c_alterUser IS
      SELECT   'alter user ' || userName || CHR (10) || 'grant connect through coeuser' alterSql
          FROM dba_users u, &&schemaName..people p
         WHERE u.userName = UPPER (p.user_id)
      ORDER BY userName;
BEGIN
   FOR r_alterUser IN c_alterUser LOOP
      EXECUTE IMMEDIATE r_alterUser.alterSql;
   END LOOP;
END;
/


--cs_security
DECLARE
  FlagSchema NUmber;
BEGIN
    SELECT count(1) 
        INTO FlagSchema
        FROM DBA_USERS
        WHERE USERNAME='CS_SECURITY';
    IF FlagSchema>0 THEN 
        EXECUTE IMMEDIATE 'ALTER USER cs_security GRANT CONNECT THROUGH coeuser';
    END IF;
END;
/


ALTER USER coedb GRANT CONNECT THROUGH coeuser;

DECLARE
   n   NUMBER;
BEGIN
   SELECT COUNT (*) INTO n
     FROM dba_users du1, dba_users du2
    WHERE du1.userName = UPPER ('SAMPLE') AND du2.userName = UPPER ('COEUSER');

   IF n = 1 THEN
      EXECUTE IMMEDIATE 'alter user sample grant connect through coeuser';
   END IF;
END;
/