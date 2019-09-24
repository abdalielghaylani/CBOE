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
@"sql\Patches\Patch &&currentPatch\Procedure\proc_ReconcileGrants.sql"

SET DEFINE ON

CREATE OR REPLACE FUNCTION &&schemaName..changePwd (
   pUserName      IN   people.user_id%TYPE,
   pPassword      IN   VARCHAR2 := NULL,
   pNewPassword   IN   VARCHAR2 := NULL)
   RETURN VARCHAR2
AS
   invalid_user_or_pass   EXCEPTION;
   pw                     VARCHAR2 (30);
   cannot_reuse_pwd       EXCEPTION;
   PRAGMA EXCEPTION_INIT (cannot_reuse_pwd, -28007);
BEGIN
   SELECT PASSWORD INTO pw
     FROM dba_users
    WHERE UPPER (userName) = UPPER (pUserName);

   IF pPassword <> pw THEN
      RAISE invalid_user_or_pass;
   END IF;

   IF (pNewPassword IS NOT NULL) THEN
      EXECUTE IMMEDIATE 'ALTER USER "' || pUserName || '" IDENTIFIED BY ' || pNewPassword;
   END IF;

   RETURN '1';
EXCEPTION
   WHEN invalid_user_or_pass THEN
      RETURN 'Invalid User Name or Password';
   WHEN cannot_reuse_pwd THEN
      RETURN 'Cannot reuse previously used password.';
END changePwd;
/


--#########################################################
--PACKAGES
--#########################################################
@"sql\Patches\Patch &&currentPatch\Packages\pkg_Roles.sql"
@"sql\Patches\Patch &&currentPatch\Packages\pkg_Users.sql"

--#########################################################
--DATA
--#########################################################

--COMMIT;

--#########################################################
--Adding column to COEGENERICOBJECT
--#########################################################
ALTER TABLE COEGENERICOBJECT ADD ASSOCIATEDDATAVIEWID VARCHAR2(255);
COMMIT;

--#####################################################################
-- COEManager PageControlSettings
--#####################################################################


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











