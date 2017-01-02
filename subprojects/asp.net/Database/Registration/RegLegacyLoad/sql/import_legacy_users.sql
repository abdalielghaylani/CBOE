PROMPT ===> Starting file import_legacy_users.sql

PROMPT Dropping legacy_users Table
DROP TABLE legacy_users;

PROMPT Creating legacy_users table
CREATE TABLE legacy_users(
    PersonID		INT,
    UserName        VARCHAR2(30),
	UserCode        VARCHAR2(10),
	Active			VARCHAR2(1),
    Role	        VARCHAR2(30),
    FirstName       VARCHAR2(50),
    MiddleName		VARCHAR2(50),
    LastName		VARCHAR2(50),
	SupervisorID    INT,
    Title			VARCHAR2(50),
	Department		VARCHAR2(50),
	SiteID          INT,   
	Address			VARCHAR2(50),
	Telephone		VARCHAR2(50),
	Email			VARCHAR2(50)	
);

-- SQLLoader users.txt 
PROMPT Importing user information using control file and sqlldr
PROMPT host sqlldr userid=&&schemaName/&&schemaPass@&&serverName control=import_users.ctl log=&&tmplog
HOST sqlldr userid=&&schemaName/&&schemaPass@&&serverName control=import_users.ctl log=&&tmplog

SPOOL off
HOST type &&tmplog >> &&scriptlog
SPOOL &&scriptlog append


CONNECT &&securitySchemaName/&&securitySchemaPass@&&serverName

-- REGDB needs these grants in order to transfer data to the COEDB hitlist tables
GRANT select on coedb.coehitlistid_seq to regdb;
GRANT insert on coedb.coesavedhitlistid to regdb;
GRANT insert on coedb.coesavedhitlist to regdb;

PROMPT Create users in the DataBase and add to People table
DECLARE
   lerror       VARCHAR2 (2500);
   lexception   EXCEPTION;

   CURSOR c_pp IS
      SELECT x.LastName, x.FirstName, x.UserName
        FROM (SELECT 'UNKNOWN' LastName, 'UNKNOWN' FirstName,
                     'UNKNOWN' UserName
                FROM DUAL
              UNION ALL
              SELECT DECODE (LastName, '0', NULL, LastName) LastName,
                     DECODE (FirstName, '0', NULL, FirstName) FirstName,
                     UserName
                FROM &&schemaName..legacy_users) x
       WHERE NOT EXISTS (SELECT 1
                           FROM coedb.people pp
                          WHERE pp.user_id = upper(trim(x.UserName)));

   FUNCTION generatepwd (ausername VARCHAR2) RETURN VARCHAR2 IS
      lreverseusername   VARCHAR2 (50);
   BEGIN
      SELECT REVERSE (ausername)
        INTO lreverseusername
        FROM DUAL;

      RETURN '7' || lreverseusername || '11C';
   EXCEPTION
      WHEN OTHERS THEN
         RAISE;
   END generatepwd;
BEGIN
   FOR r_pp IN c_pp LOOP
      BEGIN
         lerror :=
            cs_security.createuser (upper(trim(r_pp.UserName)),
                                    0,
                                    '"' || generatepwd (trim(r_pp.UserName)) || '"',
                                    'CSS_USER',
                                    trim(r_pp.FirstName),
                                    NULL,
                                    NVL (trim(r_pp.LastName), trim(r_pp.UserName)),
                                    NULL,
                                    NULL,
                                    NULL,
                                    NULL,
                                    NULL,
                                    NULL,
                                    0);

         IF lerror <> '1' THEN
            RAISE lexception;
         END IF;
      EXCEPTION
         WHEN lexception THEN
            raise_application_error (-20000, lerror);
         WHEN OTHERS THEN
            RAISE;
      END;
   END LOOP;
   COMMIT;
END;
/

CONNECT &&schemaName/&&schemaPass@&&serverName
