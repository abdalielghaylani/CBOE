SET ECHO OFF
SET verify off

--#########################################################
-- PROMPT THE USER FOR SCRIPT SUBSTITUTION VARIABLES
--######################################################### 

ACCEPT oraServiceName CHAR DEFAULT '' PROMPT 'Enter the target service name:'
ACCEPT systemUserName CHAR DEFAULT 'system' PROMPT 'Enter the name of a system account(system):'
ACCEPT systemPassword CHAR DEFAULT 'manager2' PROMPT 'Enter the system account password (manager2):'
ACCEPT enSchemaName CHAR DEFAULT 'EN1103' PROMPT 'Enter the name of the Enotebook Scehma (EN1103):'
ACCEPT COESchema CHAR DEFAULT 'COEDB' PROMPT 'Enter the name of the COE Schema Owner (COEDB):'
ACCEPT COEPass CHAR DEFAULT 'ORACLE' PROMPT 'Enter the name of the COE Scehma Password (ORACLE):'


SPOOL ON
spool log_create_COEUsers.txt

PROMPT Granting Select on &&enSchemaName..ELN_PEOPLE to &&COESchema

CONNECT &&systemUserName/&&systemPassword@&&oraServiceName

--COEDB needs permission on people table to do the select
GRANT select on &&enSchemaName..ELN_PEOPLE to &&COESchema;

PROMPT Creating users

CONNECT &&COESchema/&&COEPass@&&oraServiceName

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
              SELECT  DISTINCT 'UNKNOWN' LastName, 'UNKNOWN' FirstName,
                     UserName
                FROM &&enSchemaName..ELN_PEOPLE
                Where UserName <> 'EN1103') x                
       WHERE NOT EXISTS (SELECT 1
                           FROM &&COESchema..people pp
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
            &&COESchema..createuser (upper(trim(r_pp.UserName)),
                                    0,
                                    '"' || generatepwd (upper(trim(r_pp.UserName))) || '"',
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
                                    1);

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

PROMPT Revoking Select from &&COESchema on &&enSchemaName..ELN_PEOPLE

CONNECT &&systemUserName/&&systemPassword@&&oraServiceName

GRANT select on &&enSchemaName..ELN_PEOPLE to &&COESchema;

spool off

exit

	