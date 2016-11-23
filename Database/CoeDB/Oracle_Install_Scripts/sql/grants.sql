--Copyright 1999-2008 CambridgeSoft Corporation. All rights reserved


PROMPT Starting grants.sql

Connect &&schemaName/&&schemaPass@&&serverName



--grants to the coetemp tabels to base roles for use in inserting via the search service

GRANT INSERT,SELECT ON &&schemaName..COETEMPHITLIST TO CSS_USER;
GRANT INSERT,SELECT ON &&schemaName..COETEMPHITLISTID TO CSS_USER;
GRANT INSERT,SELECT ON &&schemaName..COESAVEDHITLIST TO CSS_USER;
GRANT INSERT,SELECT ON &&schemaName..COESAVEDHITLISTID TO CSS_USER;
GRANT INSERT,SELECT ON &&schemaName..COESEARCHCRITERIA TO CSS_USER;
GRANT EXECUTE ON &&schemaName..COEDBLIBRARY TO CSS_USER;

GRANT INSERT,SELECT ON &&schemaName..COETEMPHITLIST TO CSS_ADMIN;
GRANT INSERT,SELECT ON &&schemaName..COETEMPHITLISTID TO CSS_ADMIN;
GRANT INSERT,SELECT ON &&schemaName..COESAVEDHITLIST TO CSS_ADMIN;
GRANT INSERT,SELECT ON &&schemaName..COESAVEDHITLISTID TO CSS_ADMIN;
GRANT INSERT,SELECT ON &&schemaName..COESEARCHCRITERIA TO CSS_ADMIN;
GRANT EXECUTE ON &&schemaName..COEDBLIBRARY TO CSS_ADMIN;


--CS_Scurity
--Grant to CSS_ADMIN
GRANT EXECUTE 	ON login 		TO css_admin;
GRANT EXECUTE 	ON manage_users 	TO css_admin;
GRANT EXECUTE 	ON manage_roles 	TO css_admin;
GRANT SELECT 	ON privilege_tables     TO css_admin;
GRANT SELECT 	ON people 		TO css_admin;
GRANT SELECT 	ON sites 		TO css_admin;
GRANT EXECUTE 	ON createuser 		TO css_admin;
GRANT EXECUTE 	ON updateuser 		TO css_admin;
GRANT EXECUTE 	ON deleteuser 		TO css_admin;
GRANT EXECUTE 	ON createrole 		TO css_admin;
GRANT EXECUTE 	ON updaterole 		TO css_admin;
GRANT EXECUTE 	ON deleterole 		TO css_admin;
GRANT EXECUTE 	ON changepwd 		TO css_admin;
GRANT EXECUTE 	ON audit_trail 		TO css_admin;

--Grant to CSS_USER
GRANT EXECUTE 	ON login 		TO css_user;
GRANT EXECUTE 	ON manage_roles 	TO css_user;
GRANT EXECUTE 	ON manage_users 	TO css_user;
GRANT EXECUTE 	ON changepwd 		TO css_user;
GRANT SELECT 	ON privilege_tables     TO css_user;
GRANT SELECT 	ON people 		TO css_user;
GRANT SELECT 	ON sites 		TO css_user;
GRANT EXECUTE 	ON audit_trail 		TO css_user;


--Grant to PUBLIC
GRANT EXECUTE ON mapPrivStoRole 	TO PUBLIC;
GRANT EXECUTE ON Normalize TO Public;

Connect &&InstallUser/&&sysPass@&&serverName &&AsSysDBA

--this is a temporary grant until we fix some permissions issues.
GRANT SELECT ANY TABLE TO CSSADMIN;

/*
--cs_security
DECLARE
  FlagSchema NUmber;
BEGIN
    SELECT count(1) 
        INTO FlagSchema
        FROM DBA_USERS
        WHERE USERNAME='CS_SECURITY';
    IF FlagSchema>0 THEN 
        EXECUTE IMMEDIATE 'Grant select on cs_security.security_roles to &&schemaName';
        EXECUTE IMMEDIATE 'Grant select on cs_security.privilege_tables to &&schemaName';
        EXECUTE IMMEDIATE 'Grant select on cs_security.people to &&schemaName';
    END IF;
END;
/
*/



