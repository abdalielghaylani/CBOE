SET ECHO OFF
SET verify off

--#########################################################
-- PROMPT THE USER FOR SCRIPT SUBSTITUTION VARIABLES
--######################################################### 

ACCEPT oraServiceName CHAR DEFAULT '' PROMPT 'Enter the target service name:'
ACCEPT COESchema CHAR DEFAULT 'COEDB' PROMPT 'Enter the name of the COE Schema Owner (COEDB):'
ACCEPT COEPass CHAR DEFAULT 'ORACLE' PROMPT 'Enter the name of the COE Scehma Password (ORACLE):'

DEFINE importTableName = IMPORTED_USERS
DEFINE tmplog = 'import.txt'
DEFINE scriptlog = 'log_create_COEUsers.txt'

SPOOL ON
spool &&scriptlog


CONNECT &&COESchema/&&COEPass@&&oraServiceName

PROMPT Dropping &&importTableName table if there is one.
DROP TABLE &&importTableName;

PROMPT Creating &&importTableName table

CREATE TABLE &&importTableName(
    UserName		VARCHAR2(30),
    LastName		VARCHAR2(50),
		FirstName		VARCHAR2(50),
		Department	VARCHAR2(50),
    Supervisor	VARCHAR2(100),
    Site				VARCHAR2(100),
    Active			VARCHAR2(1),
    Roles				VARCHAR2(300), 
		Address			VARCHAR2(50),
		Telephone		VARCHAR2(50),
		Email				VARCHAR2(50),
		UserCode 		VARCHAR2(100)	
);



-- SQLLoader users.txt 
PROMPT Importing user information using control file and sqlldr
PROMPT host sqlldr userid=&&COESchema/&&COEPass@&&oraServiceName control=import_users.ctl log=&&tmplog
HOST sqlldr userid=&&COESchema/&&COEPass@&&oraServiceName control=import_users.ctl log=&&tmplog

SPOOL off
HOST type &&tmplog >> &&scriptlog
SPOOL &&scriptlog append

PROMPT Set Active=1 where it is null
update &&importTableName set active = '1' where active is null;
commit;


PROMPT Creating users


PROMPT Create users in the DataBase and add to People table
DECLARE
   lerror       VARCHAR2 (2500);
   lexception   EXCEPTION;
   lroles				VARCHAR2(2500);
   lsupervisorusername VARCHAR2 (100);

   CURSOR c_pp IS
      SELECT x.UserName, x.LastName, x.FirstName, x.Department, x.Supervisor, x.Site, x.Active, x.Roles, x.Address, x.Telephone, x.Email
        FROM (SELECT 
	                'UNKNOWN' UserName, 'UNKNOWN' LastName, 'UNKNOWN' FirstName, 'UNKNOWN' Department, NULL as Supervisor, NULL as Site, NULL as Active, NULL as Roles, NULL as Address, NULL as Telephone, Null as Email, Null as UserCode     
                FROM DUAL
              UNION ALL
  	            SELECT  DISTINCT UserName, LastName, FirstName, Department, Supervisor, Site, Active, Roles, Address, Telephone, Email, UserCode
                FROM &&importTableName) x                
       WHERE NOT EXISTS (SELECT 1
                           FROM people pp
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
      	 lroles := 'CSS_USER';
      	 if r_pp.Roles is not null then
      	 	lroles := lroles || ',' || r_pp.Roles;
      	 end if;
         lerror :=
            &&COESchema..createuser (upper(trim(r_pp.UserName)), --username
                                    0,  --already in Oracle
                                    '"' || generatepwd (upper(trim(r_pp.UserName))) || '"', --password
                                    lroles, --roles granted
                                    trim(r_pp.FirstName),  --firstname
                                    NULL, --middle name
                                    NVL (trim(r_pp.LastName), trim(r_pp.UserName)),  --last name uses username if null
                                    r_pp.Telephone, --telephone
                                    r_pp.Email, --Email
                                    r_pp.Address, --Address
                                    NULL, --UserCode
                                    0, --SupervisorId
                                    1, --SiteId
                                    r_pp.Active --IsActive
                                    );
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

PROMPT Updating Supervisors

MERGE INTO COEDB.PEOPLE
 USING (SELECT p.person_id, i.username
          FROM people p, imported_users i
          where p.USER_ID = Upper(i.SUPERVISOR)) supervisorlist
    ON (PEOPLE.USER_ID = upper(supervisorlist.UserName))
WHEN matched THEN
UPDATE
   SET PEOPLE.SUPERVISOR_INTERNAL_ID = supervisorlist.person_id;

commit;								

PROMPT Adding new sites to the system
		
insert into COEDB.sites(site_code, site_name, Active)
select distinct SITE, SITE, 1
from imported_users, sites
where site not in (select site_name from sites);

commit;		
		
PROMPT Updating users with site id

MERGE INTO COEDB.PEOPLE
 USING (SELECT s.SITE_ID, i.username
          FROM SITES s, imported_users i
          where s.SITE_NAME = i.SITE) SiteNames
    ON (PEOPLE.USER_ID = upper(SiteNames.UserName))
WHEN matched THEN
UPDATE
   SET PEOPLE.SITE_ID = SiteNames.SITE_ID;


exit

	