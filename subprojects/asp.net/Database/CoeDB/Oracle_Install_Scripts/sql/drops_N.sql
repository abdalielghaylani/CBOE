

PROMPT 'Dropping test users...'

DECLARE
   n   NUMBER;
BEGIN

  
   SELECT COUNT (*) INTO n
     FROM dba_users
    WHERE username = 'CSSUSER';

   IF n > 0 THEN
      EXECUTE IMMEDIATE 'DROP USER CSSUSER';
   END IF;

   SELECT COUNT (*) INTO n
     FROM dba_users
    WHERE username = 'CSSADMIN';

   IF n > 0 THEN
      EXECUTE IMMEDIATE 'DROP USER CSSADMIN';
   END IF;

	
   SELECT COUNT (*) INTO n
     FROM dba_users
    WHERE username = 'COEUSER';

   IF n > 0 THEN
      EXECUTE IMMEDIATE 'DROP USER COEUSER CASCADE';
   END IF;
 
END;
/

PROMPT 'dropping test roles...'

DECLARE
   n   NUMBER;
BEGIN

  
   SELECT COUNT (*) INTO n
     FROM dba_roles
    WHERE ROLE = 'CSS_USER';

   IF n > 0 THEN
      EXECUTE IMMEDIATE 'DROP ROLE CSS_USER';
   END IF;

   SELECT COUNT (*) INTO n
     FROM dba_roles
    WHERE ROLE = 'CSS_ADMIN';

   IF n > 0 THEN
      EXECUTE IMMEDIATE 'DROP ROLE CSS_ADMIN';
   END IF;

  
END;
/

