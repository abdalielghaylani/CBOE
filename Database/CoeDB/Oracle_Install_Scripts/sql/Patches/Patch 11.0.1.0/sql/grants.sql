--Copyright 1999-2010 CambridgeSoft Corporation. All rights reserved

PROMPT Starting grants.sql

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

--Grant from COEDB inherited of cs_security
SET serveroutput on
DECLARE
    CURSOR C_Grants IS
        SELECT  'GRANT '||Privilege|| ' ON &&schemaName..'||Table_Name||' TO '||Grantee||Decode(Grantable,'YES',' WITH GRANT OPTION',NULL) StringGrant
          FROM   DBA_Tab_Privs
         WHERE   UPPER(Owner) = UPPER('&&securitySchemaName') AND TABLE_NAME IN (SELECT Table_Name FROM DBA_Tables  WHERE UPPER(Owner)=UPPER('&&schemaName'))
        UNION ALL
        SELECT   'GRANT '||Privilege||' ('||COLUMN_NAME||') ON &&schemaName..'||Table_Name||' TO '||Grantee||Decode(Grantable,'YES',' WITH GRANT OPTION',NULL)
          FROM   DBA_Col_Privs
         WHERE    UPPER(Owner) = UPPER('&&securitySchemaName') AND TABLE_NAME IN (SELECT Table_Name FROM DBA_Tables  WHERE UPPER(Owner)=UPPER('&&schemaName'));
BEGIN
    FOR R_Grants IN C_Grants LOOP
	BEGIN
       		EXECUTE IMMEDIATE R_Grants.StringGrant; 
	EXCEPTION
		WHEN OTHERS THEN
          		dbms_output.put_line('Error: '||SQLERRM||' in line '||R_Grants.StringGrant);
	END;
    END LOOP;
END;
/

--Grant to COEDB inherited of cs_security
DECLARE
    CURSOR C_Grants IS
        SELECT StringGrant
    FROM   DBA_Objects O, (SELECT  'GRANT '||Privilege|| ' ON '||Owner||'.'||Table_Name||' TO &&schemaName '||Decode(Grantable,'YES',' WITH GRANT OPTION',NULL) StringGrant,P.Grantee,P.Table_Name,P.Owner
                             FROM   DBA_Tab_Privs P, DBA_Users U
                            WHERE   P.Grantee = U.UserName
                           UNION ALL
                           SELECT   'GRANT '||Privilege||'('||COLUMN_NAME||') ON '||Owner||'.'||Table_Name||' TO &&schemaName '||Decode(Grantable,'YES',' WITH GRANT OPTION',NULL) ,P.Grantee,P.Table_Name,P.Owner
                             FROM   DBA_Col_Privs P, DBA_Users U
                            WHERE   P.Grantee = U.UserName) Grants
   WHERE   O.Owner = Grants.Owner AND O.Object_Name = Grants.Table_Name
           AND O.Object_Type IN
                    ('TABLE','VIEW','SEQUENCE','PACKAGE','PROCEDURE','FUNCTION','SNAPSHOT','MATERIALIZED VIEW','DIRECTORY','LIBRARY',
                     'TYPE','OPERATOR','INDEXTYPE')
            AND UPPER(Grants.Grantee) = UPPER('&&securitySchemaName');
BEGIN
    FOR R_Grants IN C_Grants LOOP
    	BEGIN
               EXECUTE IMMEDIATE R_Grants.StringGrant; 
    	EXCEPTION
        	WHEN OTHERS THEN
                	dbms_output.put_line('Error: '||SQLERRM||' in line '||R_Grants.StringGrant);
    	END;
    END LOOP;
END;
/  