--Copyright 1999-2010 CambridgeSoft Corporation. All rights reserved

@"sql\Patches\Patch &&nextPatch\parameters.sql"

prompt *******************************
prompt **** Applying Patch &&CurrentPatch ****
prompt *******************************
prompt Starting "patch.sql"...
prompt 
Connect &&schemaName/&&schemaPass@&&serverName

--#########################################################
--TABLES
--######################################################### 

--#########################################################
--SEQUENCES
--#########################################################

--#########################################################
--TRIGGERS
--#########################################################

--#########################################################
--INDEXES
--#########################################################

--#########################################################
--CONSTRAINTS
--#########################################################

--#########################################################
--VIEWS
--#########################################################

--#########################################################
--PROCEDURES AND FUNCTIONS
--#########################################################

CREATE OR REPLACE FUNCTION  &&schemaName..createUser (
   pUserName            IN   people.user_id%TYPE,
   pIsAlreadyInOracle   IN   INTEGER := 0,
   pPassword            IN   VARCHAR2,
   pRolesGranted        IN   VARCHAR2,
   pFirstName           IN   people.first_name%TYPE := NULL,
   pMiddleName          IN   people.middle_name%TYPE := NULL,
   pLastName            IN   people.last_name%TYPE := NULL,
   pTelephone           IN   people.telephone%TYPE := NULL,
   pEmail               IN   people.email%TYPE := NULL,
   pAddress             IN   people.int_address%TYPE := NULL,
   pUserCode            IN   people.user_code%TYPE := NULL,
   pSupervisorId        IN   people.supervisor_internal_id%TYPE := NULL,
   pSiteId              IN   people.site_id%TYPE := NULL,
   pIsActive            IN   people.active%TYPE := 1,
   pActivatingUser		IN	 CHAR := '0')
   RETURN VARCHAR2
AS
   source_cursor                INTEGER;
   rows_processed               INTEGER;
   userCode                     people.user_code%TYPE;
   user_or_role_name_conflict   EXCEPTION;
   unique_constraint_violated   EXCEPTION;
   role_not_found               EXCEPTION;
   quotedUserName				people.user_id%TYPE;
   PRAGMA EXCEPTION_INIT (unique_constraint_violated, -1);
   PRAGMA EXCEPTION_INIT (user_or_role_name_conflict, -1920);
   PRAGMA EXCEPTION_INIT (role_not_found, -1919);
BEGIN
   source_cursor := DBMS_SQL.open_cursor;
   quotedUserName := UPPER('"' || pUserName || '"');

   IF pIsAlreadyInOracle = 0 THEN
      DBMS_SQL.parse (source_cursor, 'CREATE USER ' || quotedUserName || ' IDENTIFIED BY ' || pPassword || ' DEFAULT TABLESPACE &&securityTableSpaceName TEMPORARY TABLESPACE &&tempTableSpaceName', DBMS_SQL.native);
      rows_processed := DBMS_SQL.EXECUTE (source_cursor);
   END IF;

   DBMS_SQL.parse (source_cursor, 'GRANT CONNECT TO ' || quotedUserName, DBMS_SQL.native);
   rows_processed := DBMS_SQL.EXECUTE (source_cursor);
   DBMS_SQL.parse (source_cursor, 'ALTER USER ' || quotedUserName || ' GRANT CONNECT THROUGH COEUSER', DBMS_SQL.native);
   rows_processed := DBMS_SQL.EXECUTE (source_cursor);
   DBMS_SQL.parse (source_cursor, 'GRANT ' || pRolesGranted || ' TO ' || quotedUserName, DBMS_SQL.native);
   rows_processed := DBMS_SQL.EXECUTE (source_cursor);
   DBMS_SQL.parse (source_cursor, 'ALTER USER ' || quotedUserName || ' DEFAULT ROLE ALL', DBMS_SQL.native);
   rows_processed := DBMS_SQL.EXECUTE (source_cursor);
   DBMS_SQL.parse (source_cursor, 'ALTER USER ' || quotedUserName || ' PROFILE csuserprofile', DBMS_SQL.native);
   rows_processed := DBMS_SQL.EXECUTE (source_cursor);

   IF pUserCode IS NULL THEN
      userCode := pUserName;
   ELSE
      userCode := pUserCode;
   END IF;

   IF(pActivatingUser = '0') THEN
	INSERT INTO people (user_id, first_name, middle_name, last_name, email, telephone, int_address, user_code, supervisor_internal_id, site_id, active)
	VALUES (pUserName, pFirstName, pMiddleName, pLastName, pEmail, pTelephone, pAddress, userCode, pSupervisorId, pSiteId, pIsActive);
   ELSE
	UPDATE PEOPLE SET ACTIVE = '1' WHERE USER_ID = pUserName;
   END IF;

   RETURN '1';
EXCEPTION
   WHEN user_or_role_name_conflict THEN
      RETURN 'user name ' || pUserName || ' conflicts with another user or role name ';
   WHEN unique_constraint_violated THEN
	  IF pIsAlreadyInOracle = 0 THEN
        DBMS_SQL.parse (source_cursor, 'DROP USER ' || quotedUserName, DBMS_SQL.native);
        rows_processed := DBMS_SQL.EXECUTE (source_cursor);
      END IF;
      RETURN 'User Code ' || usercode || ' is or has been already taken by another user';
   WHEN role_not_found THEN
	  IF pIsAlreadyInOracle = 0 THEN
	    DBMS_SQL.parse (source_cursor, 'DROP USER ' || quotedUserName, DBMS_SQL.native);
        rows_processed := DBMS_SQL.EXECUTE (source_cursor);
      END IF;
      RETURN 'Failed to find one or more of the roles to be granted';
END createUser;
/

CREATE OR REPLACE FUNCTION &&schemaName..updateUser (
   pUserName       IN   people.user_id%TYPE,
   pPassword       IN   VARCHAR2 := NULL,
   pRolesGranted   IN   VARCHAR2 := NULL,
   pRolesRevoked   IN   VARCHAR2 := NULL,
   pFirstName      IN   people.first_name%TYPE := NULL,
   pMiddleName     IN   people.middle_name%TYPE := NULL,
   pLastName       IN   people.last_name%TYPE := NULL,
   pTelephone      IN   people.telephone%TYPE := NULL,
   pEmail          IN   people.email%TYPE := NULL,
   pAddress        IN   people.int_address%TYPE := NULL,
   pUserCode       IN   people.user_code%TYPE := NULL,
   pSupervisorId   IN   people.supervisor_internal_id%TYPE := NULL,
   pSiteId         IN   people.site_id%TYPE := NULL,
   pIsActive       IN   people.active%TYPE := 1
)
   RETURN VARCHAR2
AS
   source_cursor    INTEGER;
   rows_processed   INTEGER;
   quotedUserName	people.user_id%TYPE;
BEGIN
   quotedUserName := UPPER('"' || pUserName || '"');
   UPDATE people
      SET first_name = pFirstName,
          middle_name = pMiddleName,
          last_name = pLastName,
          email = pEmail,
          telephone = pTelephone,
          int_address = pAddress,
          user_code = pUserCode,
          supervisor_internal_id = pSupervisorId,
          site_id = pSiteId,
          active = pIsActive
    WHERE UPPER (people.user_id) = pUserName;

   IF (pPassword IS NOT NULL) OR (pRolesGranted IS NOT NULL) OR (pRolesRevoked IS NOT NULL) THEN
      source_cursor := DBMS_SQL.open_cursor;

      IF (pPassword IS NOT NULL) THEN
         DBMS_SQL.parse (source_cursor, 'ALTER USER ' || quotedUserName || ' IDENTIFIED BY ' || pPassword, DBMS_SQL.native);
		 rows_processed := DBMS_SQL.EXECUTE (source_cursor);
      END IF;

      IF (pRolesRevoked IS NOT NULL) THEN
         DBMS_SQL.parse (source_cursor, 'REVOKE ' || pRolesRevoked || ' FROM ' || quotedUserName, DBMS_SQL.native);
         rows_processed := DBMS_SQL.EXECUTE (source_cursor);
      END IF;

      IF (pRolesGranted IS NOT NULL) THEN
         DBMS_SQL.parse (source_cursor, 'GRANT ' || pRolesGranted || ' TO ' || quotedUserName, DBMS_SQL.native);
         rows_processed := DBMS_SQL.EXECUTE (source_cursor);
      END IF;
	  
	  DBMS_SQL.parse (source_cursor, 'ALTER USER ' || quotedUserName || ' GRANT CONNECT THROUGH COEUSER', DBMS_SQL.native);
      rows_processed := DBMS_SQL.EXECUTE (source_cursor);
   END IF;

   RETURN '1';
END updateUser;
/

CREATE OR REPLACE FUNCTION &&schemaName..deleteUser (pUserName IN people.user_id%TYPE) RETURN VARCHAR2 AS
   quotedUserName				people.user_id%TYPE;
   cannot_drop_connected_user   EXCEPTION;
   PRAGMA EXCEPTION_INIT (cannot_drop_connected_user, -1940);
BEGIN
   quotedUserName := UPPER('"' || pUserName || '"');
   EXECUTE IMMEDIATE 'DROP USER ' || quotedUserName || ' CASCADE';

   UPDATE people
      SET active = 0
    WHERE user_id = pUserName;

   RETURN '1';
EXCEPTION
   WHEN cannot_drop_connected_user THEN
      RETURN 'Cannot drop user ' || pUserName || ' because it is currently connected to Oracle';
END deleteUser;
/

--#########################################################
--PACKAGES
--#########################################################

set define off

@"sql\Patches\Patch 11.0.3\Packages\pkg_ConfigurationManager.sql"

set define on

--#########################################################
--DATA
--#########################################################

--COMMIT;



--#####################################################################
--Group Security Changes
--#####################################################################

@"sql\Patches\Patch &&currentPatch\sql\create_tables.sql"
@"sql\Patches\Patch &&currentPatch\sql\create_view.sql"
@"sql\Patches\Patch &&currentPatch\sql\PLSQL\proc_ReconcileGrants.sql"
@"sql\Patches\Patch &&currentPatch\sql\PLSQL\pkg_Groups.sql"
@"sql\Patches\Patch &&currentPatch\sql\PLSQL\pkg_Users.sql"
@"sql\Patches\Patch &&currentPatch\sql\Group_grants.sql"
@"sql\Patches\Patch &&currentPatch\sql\AdditionalTriggers.sql"
@"sql\Patches\Patch &&currentPatch\sql\group_data.sql"
@"sql\Patches\Patch &&currentPatch\sql\NewCOEDBPrincipalData.sql"


--#####################################################################
-- COEManager PageControlSettings
--#####################################################################

@"sql\Patches\Patch &&currentPatch\sql\UpgradeManagerPageControlSettings.sql"


UPDATE &&schemaName..CoeGlobals
	SET Value = '&&currentPatch' 
	WHERE UPPER(ID) = 'SCHEMAVERSION';

UPDATE &&schemaName..CoeGlobals
	SET Value = '&&versionApp' 
	WHERE UPPER(ID) = 'VERSION_APP';
COMMIT;

prompt **** Patch &&currentPatch Applied ****

COL setNextPatch NEW_VALUE setNextPatch NOPRINT
SELECT	CASE
		WHEN  '&&toVersion'='&&currentPatch'
		THEN  'sql\Patches\stop.sql'
		ELSE  '"sql\Patches\Patch &&nextPatch\patch.sql"'
	END	AS setNextPatch 
FROM	DUAL;











