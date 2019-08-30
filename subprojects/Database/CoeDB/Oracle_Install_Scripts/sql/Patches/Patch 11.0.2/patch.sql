--Copyright 1999-2010 CambridgeSoft Corporation. All rights reserved

@"sql\Patches\Patch &&nextPatch\parameters.sql"

prompt *******************************
prompt **** Applying Patch &&currentPatch ****
prompt *******************************
prompt Starting "patch.sql"...


--#########################################################
--TABLES
--######################################################### 

--Report

CREATE TABLE &&schemaName..COEREPORT
(
  ID               NUMBER(9)              NOT NULL,
  NAME             VARCHAR2(255 BYTE),
  DESCRIPTION      VARCHAR2(255 BYTE),
  USER_ID              VARCHAR2(30 BYTE),
  IS_PUBLIC            VARCHAR2(1 BYTE)   NOT NULL,
  DATE_CREATED         DATE NOT NULL,
  REPORT_TEMPLATE      CLOB NOT NULL,
  DATABASE             VARCHAR2(30 BYTE)  NOT NULL,
  APPLICATION          VARCHAR2(255 BYTE), 
  DATAVIEW_ID          NUMBER(9)
); 

--#########################################################
--SEQUENCES
--#########################################################

--#########################################################
--TRIGGERS
--#########################################################

--Report


CREATE SEQUENCE &&schemaName..COEREPORT_SEQ INCREMENT BY 1 START WITH 1;

--#########################################################
--INDEXES
--#########################################################

--#########################################################
--CONSTRAINTS
--#########################################################

--Report
ALTER TABLE &&schemaName..COEREPORT ADD
  CONSTRAINT PK_COEREPORT PRIMARY KEY (ID);
ALTER TABLE &&schemaName..COEREPORT ADD
  CONSTRAINT FK_COEREPORT_COEDATAVIEW FOREIGN KEY(DATAVIEW_ID) REFERENCES &&schemaName..COEDATAVIEW(ID);
ALTER TABLE &&schemaName..COEREPORT ADD
  CONSTRAINT FK_COEREPORT_PEOPLE FOREIGN KEY(USER_ID) REFERENCES &&schemaName..PEOPLE(USER_ID);


--#########################################################
--VIEWS
--#########################################################

--#########################################################
--PROCEDURES AND FUNCTIONS
--#########################################################

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
BEGIN
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
    WHERE UPPER (people.user_id) = UPPER (pUserName);

   IF (pPassword IS NOT NULL) OR (pRolesGranted IS NOT NULL) OR (pRolesRevoked IS NOT NULL) THEN
      source_cursor := DBMS_SQL.open_cursor;

      IF (pPassword IS NOT NULL) THEN
         DBMS_SQL.parse (source_cursor, 'ALTER USER ' || pUserName || ' IDENTIFIED BY ' || pPassword, DBMS_SQL.native);
		 rows_processed := DBMS_SQL.EXECUTE (source_cursor);
      END IF;

      IF (pRolesRevoked IS NOT NULL) THEN
         DBMS_SQL.parse (source_cursor, 'REVOKE ' || pRolesRevoked || ' FROM ' || pUserName, DBMS_SQL.native);
         rows_processed := DBMS_SQL.EXECUTE (source_cursor);
      END IF;

      IF (pRolesGranted IS NOT NULL) THEN
         DBMS_SQL.parse (source_cursor, 'GRANT ' || pRolesGranted || ' TO ' || pUserName, DBMS_SQL.native);
         rows_processed := DBMS_SQL.EXECUTE (source_cursor);
      END IF;
	  
	  DBMS_SQL.parse (source_cursor, 'ALTER USER ' || pUserName || ' GRANT CONNECT THROUGH COEUSER', DBMS_SQL.native);
      rows_processed := DBMS_SQL.EXECUTE (source_cursor);
   END IF;

   RETURN '1';
END updateUser;
/

CREATE OR REPLACE FUNCTION &&schemaName..createRole (
   pRoleName            IN   security_roles.role_name%TYPE,
   pPrivTableName       IN   privilege_tables.privilege_table_name%TYPE,
   pIsAlreadyInOracle   IN   INTEGER,
   pPrivValueList       IN   VARCHAR2) 
   RETURN VARCHAR2
AS
   privTableId   INTEGER;
   roleId        INTEGER;
BEGIN

   IF pIsAlreadyInOracle = 0 THEN
      EXECUTE IMMEDIATE 'CREATE ROLE ' || pRoleName || ' NOT IDENTIFIED';
      --EXECUTE IMMEDIATE 'GRANT CSS_USER TO ' || pRoleName;
	  EXECUTE IMMEDIATE 'GRANT EXECUTE ON &&schemaName..LOGIN TO ' || pRoleName;
      EXECUTE IMMEDIATE 'REVOKE ' || pRoleName || ' FROM &&securitySchemaName'; 

      SELECT privilege_table_id
        INTO privTableId
        FROM privilege_tables
       WHERE UPPER (privilege_table_name) = UPPER (pPrivTableName);

      INSERT INTO security_roles (privilege_table_int_id, role_name)
           VALUES (privTableId, UPPER (pRoleName))
        RETURNING role_id
             INTO roleId;

      EXECUTE IMMEDIATE 'INSERT INTO ' || pPrivTableName || ' VALUES ( ' || roleId || ', ' || pPrivValueList || ')';
   ELSE
      INSERT INTO security_roles (privilege_table_int_id, role_name)
           VALUES (NULL, UPPER (pRoleName))
        RETURNING role_id
             INTO roleId;
   END IF;

   RETURN '1';
END createRole;
/

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
BEGIN

   IF pIsAlreadyInOracle = 0 THEN
      SELECT privilege_table_id
        INTO privTableId
        FROM privilege_tables
       WHERE UPPER (privilege_table_name) = UPPER (pPrivTableName);

      INSERT INTO security_roles (privilege_table_int_id, role_name, coeidentifier)
           VALUES (privTableId, UPPER (pRoleName), UPPER (pCOEIdentifier))
        RETURNING role_id
             INTO roleId;

      EXECUTE IMMEDIATE 'INSERT INTO ' || pPrivTableName || ' VALUES ( ' || roleId || ', ' || pPrivValueList || ')';
      EXECUTE IMMEDIATE 'CREATE ROLE ' || pRoleName || ' NOT IDENTIFIED';
	  EXECUTE IMMEDIATE 'GRANT EXECUTE ON &&schemaName..LOGIN TO ' || pRoleName;
      --EXECUTE IMMEDIATE 'GRANT CSS_USER TO ' || pRoleName;
      EXECUTE IMMEDIATE 'REVOKE ' || pRoleName || ' FROM COEDB';
   ELSE
      INSERT INTO security_roles (privilege_table_int_id, role_name, coeidentifier)
           VALUES (NULL, UPPER (pRoleName), UPPER (pCOEIdentifier))
        RETURNING role_id
             INTO roleId;
   END IF;

   RETURN '1';
END createCOERole;
/
--#########################################################
--PACKAGES
--#########################################################
set define off

@"sql\Patches\Patch 11.0.2\Packages\pkg_ConfigurationManager.sql"

set define on

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








