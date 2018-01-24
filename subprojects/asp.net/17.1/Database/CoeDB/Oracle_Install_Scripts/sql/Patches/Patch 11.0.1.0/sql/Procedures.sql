--Copyright 1999-2010 CambridgeSoft Corporation. All rights reserved

CREATE OR REPLACE PROCEDURE COEDB.createServiceTables (
   aServiceName           IN   VARCHAR2,
   aSchemaName            IN   VARCHAR2 := NULL,
   aIndexTableSpaceName   IN   VARCHAR2 := NULL)
IS
   lSqlDdl       VARCHAR2 (4000);
   lSchemaName   VARCHAR2 (30);
   lExist        INTEGER;

   PROCEDURE createSearchTables IS
   BEGIN
      SELECT COUNT (1) INTO lExist
        FROM dba_tables
       WHERE UPPER (table_name) = 'COESAVEDSEARCHCRITERIA'
         AND UPPER (owner) = UPPER (lSchemaName);

      IF lExist = 0 THEN
         lSqlDdl := 'CREATE TABLE ' || lSchemaName || '.COESAVEDSEARCHCRITERIA
                (
                  ID                 NUMBER(9)                 NOT NULL,
                  NAME               VARCHAR2(255 BYTE)         NOT NULL,
                  DESCRIPTION        VARCHAR2(255 BYTE),
                  USER_ID            VARCHAR2(30 BYTE),
                  NUM_HITS           NUMBER(9),
                  IS_PUBLIC          VARCHAR2(1 BYTE)           NOT NULL,
                  FORMGROUP          NUMBER(9),
                  DATE_CREATED       DATE                       NOT NULL,
                  COESEARCHCRITERIA  CLOB                       NOT NULL,
                  DATABASE           VARCHAR2(30 BYTE)          NOT NULL,
                  DATAVIEW_ID        NUMBER(9,0),
                  CONSTRAINT PK_' || lSchemaName || '_COESAVEDSEARCH PRIMARY KEY (ID) USING INDEX TABLESPACE ' || aIndexTableSpaceName || '
                )';

         EXECUTE IMMEDIATE lSqlDdl;
         EXECUTE IMMEDIATE 'CREATE INDEX ' || lSchemaName || '.INDEX_COESAVEDSEARCH_USER_ID ON '   || lSchemaName || '.COESAVEDSEARCHCRITERIA (USER_ID ASC)  TABLESPACE '      || aIndexTableSpaceName;
         EXECUTE IMMEDIATE 'CREATE INDEX ' || lSchemaName || '.INDEX_COESAVEDSEARCH_IS_PUBLIC ON ' || lSchemaName || '.COESAVEDSEARCHCRITERIA (IS_PUBLIC ASC)  TABLESPACE '    || aIndexTableSpaceName;
         EXECUTE IMMEDIATE 'CREATE INDEX ' || lSchemaName || '.INDEX_COESAVEDSEARCH_FORMGROUP ON ' || lSchemaName || '.COESAVEDSEARCHCRITERIA (FORMGROUP ASC)  TABLESPACE '    || aIndexTableSpaceName;
         EXECUTE IMMEDIATE 'CREATE INDEX ' || lSchemaName || '.INDEX_COESAVEDSEARCH_DATE_CREA ON ' || lSchemaName || '.COESAVEDSEARCHCRITERIA (DATE_CREATED ASC)  TABLESPACE ' || aIndexTableSpaceName;
         EXECUTE IMMEDIATE 'CREATE INDEX ' || lSchemaName || '.INDEX_COESAVEDSEARCH_DATABASE ON '  || lSchemaName || '.COESAVEDSEARCHCRITERIA (DATABASE ASC)  TABLESPACE '     || aIndexTableSpaceName;
      END IF;

      SELECT COUNT (1) INTO lExist
        FROM dba_tables
       WHERE UPPER (table_name) = 'COESEARCHCRITERIA'
         AND UPPER (owner) = UPPER (lSchemaName);

      IF lExist = 0 THEN
         lSqlDdl := 'CREATE TABLE ' || lSchemaName || '.COESEARCHCRITERIA
                (
                  ID                 NUMBER(9)                 NOT NULL,
                  NAME               VARCHAR2(255 BYTE),
                  DESCRIPTION        VARCHAR2(255 BYTE),
                  USER_ID            VARCHAR2(30 BYTE),
                  NUM_HITS           NUMBER(9),
                  IS_PUBLIC          VARCHAR2(1 BYTE)           NOT NULL,
                  FORMGROUP          NUMBER(9),
                  DATE_CREATED       DATE                       NOT NULL,
                  COESEARCHCRITERIA  CLOB                       NOT NULL,
                  DATABASE           VARCHAR2(30 BYTE)               NOT NULL,
                  DATAVIEW_ID        NUMBER(9,0),
                  CONSTRAINT PK_' || lSchemaName || '_COESEARCHCRITE PRIMARY KEY (ID) USING INDEX TABLESPACE ' || aIndexTableSpaceName || '
                )';

         EXECUTE IMMEDIATE lSqlDdl;
         EXECUTE IMMEDIATE 'CREATE INDEX ' || lSchemaName || '.INDEX_COESEARCH_USER_ID ON '       || lSchemaName || '.COESEARCHCRITERIA (USER_ID ASC)  TABLESPACE '      || aIndexTableSpaceName;
         EXECUTE IMMEDIATE 'CREATE INDEX ' || lSchemaName || '.INDEX_COESEARCH_IS_PUBLIC ON '     || lSchemaName || '.COESEARCHCRITERIA (IS_PUBLIC ASC)  TABLESPACE '    || aIndexTableSpaceName;
         EXECUTE IMMEDIATE 'CREATE INDEX ' || lSchemaName || '.INDEX_CCOESEARCH_FORMGROUP ON '    || lSchemaName || '.COESEARCHCRITERIA (FORMGROUP ASC)  TABLESPACE '    || aIndexTableSpaceName;
         EXECUTE IMMEDIATE 'CREATE INDEX ' || lSchemaName || '.INDEX_CCOESEARCH_DATE_CREATED ON ' || lSchemaName || '.COESEARCHCRITERIA (DATE_CREATED ASC)  TABLESPACE ' || aIndexTableSpaceName;
         EXECUTE IMMEDIATE 'CREATE INDEX ' || lSchemaName || '.INDEX_CCOESEARCH_DATABASE ON '     || lSchemaName || '.COESEARCHCRITERIA (DATABASE ASC)  TABLESPACE '     || aIndexTableSpaceName;

         SELECT COUNT (1) INTO lExist
           FROM dba_sequences
          WHERE UPPER (sequence_name) = 'COESEARCHCRITERIA_SEQ'
            AND UPPER (sequence_owner) = UPPER (lSchemaName);

         IF lExist = 1 THEN
            EXECUTE IMMEDIATE 'DROP SEQUENCE ' || lSchemaName || '.COESEARCHCRITERIA_SEQ';
         END IF;

         EXECUTE IMMEDIATE 'CREATE SEQUENCE ' || lSchemaName || '.COESEARCHCRITERIA_SEQ INCREMENT BY 1 START WITH 1 MAXVALUE 1.0E27 MINVALUE 1 NOCYCLE CACHE 20 NOORDER';
      END IF;
   END;

   PROCEDURE createHitListTables IS
      lExist   INTEGER;
   BEGIN
        SELECT COUNT (1) INTO lExist
          FROM dba_tables
         WHERE UPPER (table_name) = 'COESAVEDHITLIST'
           AND UPPER (owner) = UPPER (lSchemaName);

        IF lExist = 0 THEN
           lSqlDdl := 'CREATE TABLE ' || lSchemaName || '.COESAVEDHITLIST
               (
                 HITLISTID     NUMBER(9)                         NOT NULL,
                 ID            NUMBER(9)                         NOT NULL,
                 DATESTAMP     DATE               DEFAULT SYSDATE NOT NULL,
                 SORTORDER     FLOAT,
                 CONSTRAINT PK_' || lSchemaName || '_COESAVEDHITLIST PRIMARY KEY (HITLISTID,ID)
               ) ORGANIZATION INDEX';

           EXECUTE IMMEDIATE lSqlDdl;
        END IF;

        SELECT COUNT (1) INTO lExist
          FROM dba_tables
         WHERE UPPER (table_name) = 'COETEMPHITLIST'
           AND UPPER (owner) = UPPER (lSchemaName);

        IF lExist = 0 THEN
           lSqlDdl := 'CREATE TABLE ' || lSchemaName || '.COETEMPHITLIST
                   (
                     HITLISTID  NUMBER(9)                            NOT NULL,
                     ID         NUMBER(9)                     NOT NULL,
                     DATESTAMP  DATE                  DEFAULT SYSDATE NOT NULL,
                     SORTORDER  FLOAT,
                     CONSTRAINT PK_' || lSchemaName || '_COETEMPHITLIST PRIMARY KEY (HITLISTID,ID)
                   ) ORGANIZATION INDEX';

           EXECUTE IMMEDIATE lSqlDdl;
        END IF;

        SELECT COUNT (1) INTO lExist
          FROM dba_tables
         WHERE UPPER (table_name) = 'COESAVEDHITLISTID'
           AND UPPER (owner) = UPPER (lSchemaName);

        IF lExist = 0 THEN
           lSqlDdl := 'CREATE TABLE ' || lSchemaName || '.COESAVEDHITLISTID
                  (
                    ID            NUMBER(9)                      NOT NULL,
                    NAME          VARCHAR2(255 BYTE)              NOT NULL,
                    DESCRIPTION   VARCHAR2(255 BYTE),
                    USER_ID       VARCHAR2(30 BYTE),
                    NUMBER_HITS   NUMBER(9),
                    IS_PUBLIC     VARCHAR2(1 BYTE)                NOT NULL,
                    DATE_CREATED  DATE                            NOT NULL,
                    DATABASE      VARCHAR2(30 BYTE)               NOT NULL,
                    TYPE          VARCHAR2(10 BYTE),
                    DATAVIEW_ID   NUMBER(9),
                    PARENT_HITLIST_ID NUMBER(9),
            HITLISTID     NUMBER(9),
            PARENT_HITLIST_TYPE VARCHAR2(10),
            SEARCH_CRITERIA_ID NUMBER(9),
                SEARCH_CRITERIA_TYPE VARCHAR2(10),
                    CONSTRAINT PK_' || lSchemaName || '_COESAVEDHITID PRIMARY KEY (ID) USING INDEX TABLESPACE  ' || aIndexTableSpaceName || '
                  )';

           EXECUTE IMMEDIATE lSqlDdl;
           EXECUTE IMMEDIATE 'CREATE INDEX ' || lSchemaName || '.INDEX_COESAVHITLISID_USER_ID ON '   || lSchemaName || '.COESAVEDHITLISTID (USER_ID ASC)  TABLESPACE '      || aIndexTableSpaceName;
           EXECUTE IMMEDIATE 'CREATE INDEX ' || lSchemaName || '.INDEX_COESAVHITLISID_IS_PUBLIC ON ' || lSchemaName || '.COESAVEDHITLISTID (IS_PUBLIC ASC)  TABLESPACE '    || aIndexTableSpaceName;
           EXECUTE IMMEDIATE 'CREATE INDEX ' || lSchemaName || '.INDEX_COESAVHITLISID_DATE_CRE ON '  || lSchemaName || '.COESAVEDHITLISTID (DATE_CREATED ASC)  TABLESPACE ' || aIndexTableSpaceName;
           EXECUTE IMMEDIATE 'CREATE INDEX ' || lSchemaName || '.INDEX_COESAVHITLISID_DATABASE ON '  || lSchemaName || '.COESAVEDHITLISTID (DATABASE ASC)  TABLESPACE '     || aIndexTableSpaceName;
           EXECUTE IMMEDIATE 'CREATE INDEX ' || lSchemaName || '.INDEX_COESAVHITLISID_SCRITERID ON ' || lSchemaName || '.COESAVEDHITLISTID (SEARCH_CRITERIA_ID ASC)  TABLESPACE ' || aIndexTableSpaceName;
           EXECUTE IMMEDIATE 'CREATE INDEX ' || lSchemaName || '.INDEX_COESAVHITLISID_CRITERITY ON ' || lSchemaName || '.COESAVEDHITLISTID (SEARCH_CRITERIA_TYPE ASC)  TABLESPACE ' || aIndexTableSpaceName;
        END IF;

      -- Partition Section
        BEGIN
            partitionManagment.validatePartitionSupport;

            SELECT COUNT (1) INTO lExist
              FROM dba_tables
             WHERE UPPER (table_name) = 'COETEMPHITLISTID'
               AND UPPER (owner) = UPPER (lSchemaName);

            IF lExist = 0 THEN
                EXECUTE IMMEDIATE 'CREATE TABLE ' || lSchemaName || '.COETEMPHITLISTID
                    (
                     ID            NUMBER(9)                      NOT NULL,
                     NAME          VARCHAR2(255 BYTE)              NOT NULL,
                     DESCRIPTION   VARCHAR2(255 BYTE),
                     USER_ID       VARCHAR2(30 BYTE),
                     NUMBER_HITS   NUMBER(9),
                     IS_PUBLIC     VARCHAR2(1 BYTE)                NOT NULL,
                     DATE_CREATED  DATE                            NOT NULL,
                     DATABASE      VARCHAR2(30 BYTE)               NOT NULL,
                     TYPE          VARCHAR2(10 BYTE),
                     DATAVIEW_ID   NUMBER(9),
                     PARENT_HITLIST_ID NUMBER(9),
                     HITLISTID     NUMBER(9),
                     PARENT_HITLIST_TYPE NUMBER(9),
                     SEARCH_CRITERIA_ID NUMBER(9),
                     SEARCH_CRITERIA_TYPE VARCHAR2(10),
                     CONSTRAINT PK_' || lSchemaName || '_COETEMPHITLISTID PRIMARY KEY (ID) 
                    ) ORGANIZATION INDEX
                      PARTITION BY RANGE (ID) (PARTITION COETEMPHITLISTIDMAXVALUE VALUES LESS THAN(MAXVALUE))';
            END IF;

            PartitionManagment.UpdatePartitionJob ('COETEMPHITLISTID', SYSDATE, lSchemaName);

        EXCEPTION
            WHEN OTHERS THEN
               SELECT COUNT (1) INTO lExist
                 FROM dba_tables
                WHERE UPPER (table_name) = 'COETEMPHITLISTID'
                  AND UPPER (owner) = UPPER (lSchemaname);

               IF lExist = 0 THEN
                 EXECUTE IMMEDIATE 'CREATE TABLE ' || lSchemaName || '.COETEMPHITLISTID
                   (
                     ID            NUMBER(9)                      NOT NULL,
                     NAME          VARCHAR2(255 BYTE)              NOT NULL,
                     DESCRIPTION   VARCHAR2(255 BYTE),
                     USER_ID       VARCHAR2(30 BYTE),
                     NUMBER_HITS   NUMBER(9),
                     IS_PUBLIC     VARCHAR2(1 BYTE)                NOT NULL,
                     DATE_CREATED  DATE                            NOT NULL,
                     DATABASE      VARCHAR2(30 BYTE)               NOT NULL,
                     TYPE          VARCHAR2(10 BYTE),
                     DATAVIEW_ID   NUMBER(9),
                     PARENT_HITLIST_ID NUMBER(9),
                     HITLISTID     NUMBER(9),
                     PARENT_HITLIST_TYPE NUMBER(9),
                     SEARCH_CRITERIA_ID NUMBER(9),
                     SEARCH_CRITERIA_TYPE VARCHAR2(10),
                     CONSTRAINT PK_' || lSchemaName || '_COETEMPHITLISTID PRIMARY KEY (ID) USING INDEX TABLESPACE ' || aIndexTableSpaceName || '
                   )';
               END IF;
        END;


        EXECUTE IMMEDIATE 'CREATE INDEX ' || lSchemaName || '.INDEX_COEHITLISTID_USER_ID ON ' || lSchemaName || '.COETEMPHITLISTID (USER_ID ASC)  TABLESPACE ' || aIndexTableSpaceName;
        EXECUTE IMMEDIATE 'CREATE INDEX ' || lSchemaName || '.INDEX_COEHITLISTID_IS_PUBLIC ON ' || lSchemaName || '.COETEMPHITLISTID (IS_PUBLIC ASC)  TABLESPACE ' || aIndexTableSpaceName;
        EXECUTE IMMEDIATE 'CREATE INDEX ' || lSchemaName || '.INDEX_COEHITLISTID_DATE_CREATE ON ' || lSchemaName || '.COETEMPHITLISTID (DATE_CREATED ASC)  TABLESPACE ' || aIndexTableSpaceName;
        EXECUTE IMMEDIATE 'CREATE INDEX ' || lSchemaName || '.INDEX_COEHITLISTID_DATABASE ON ' || lSchemaName || '.COETEMPHITLISTID (DATABASE ASC)  TABLESPACE ' || aIndexTableSpaceName;
        EXECUTE IMMEDIATE 'CREATE INDEX ' || lSchemaName || '.INDEX_COEHITLISTID_SCRITERIAID ON ' || lSchemaName || '.COETEMPHITLISTID (SEARCH_CRITERIA_ID ASC)  TABLESPACE ' || aIndexTableSpaceName;
        EXECUTE IMMEDIATE 'CREATE INDEX ' || lSchemaName || '.INDEX_COEHITLISTID_SCRITERIATY ON ' || lSchemaName || '.COETEMPHITLISTID (SEARCH_CRITERIA_TYPE ASC)  TABLESPACE ' || aIndexTableSpaceName;


        SELECT COUNT (1) INTO lExist
           FROM dba_sequences
          WHERE UPPER (sequence_name) = 'COEHITLISTID_SEQ'
            AND UPPER (sequence_owner) = UPPER (lSchemaName);

         IF lExist = 1 THEN
            EXECUTE IMMEDIATE 'DROP SEQUENCE ' || lSchemaName || '.COEHITLISTID_SEQ';
         END IF;

         EXECUTE IMMEDIATE 'CREATE SEQUENCE ' || lSchemaName || '.COEHITLISTID_SEQ INCREMENT BY 1 START WITH 1 MAXVALUE 1.0E27 MINVALUE 1 NOCYCLE CACHE 20 NOORDER';
   END;
BEGIN
   IF aSchemaName IS NOT NULL THEN
      lSchemaName := aSchemaName;
   ELSE
      lSchemaName := USER;
   END IF;

   CASE UPPER (aServiceName)
      WHEN 'SEARCH' THEN
         BEGIN
            createSearchTables;
         END;
      WHEN 'HITLIST' THEN
         BEGIN
            createHitListTables;
         END;
      WHEN 'ALL' THEN
         BEGIN
            createSearchTables;
            createHitListTables;
         END;
   END CASE;
EXCEPTION
   WHEN OTHERS THEN
      BEGIN
         Raise_Application_Error (-20000, DBMS_UTILITY.Format_Error_Stack);
      END;
END;
/

--CS-Security

CREATE OR REPLACE FUNCTION createRole (
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
      EXECUTE IMMEDIATE 'GRANT CSS_USER TO ' || pRoleName;
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


CREATE OR REPLACE FUNCTION createCOERole (
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
      EXECUTE IMMEDIATE 'GRANT CSS_USER TO ' || pRoleName;
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

CREATE OR REPLACE FUNCTION createUser (
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
   PRAGMA EXCEPTION_INIT (unique_constraint_violated, -1);
   PRAGMA EXCEPTION_INIT (user_or_role_name_conflict, -1920);
   PRAGMA EXCEPTION_INIT (role_not_found, -1919);
BEGIN
   source_cursor := DBMS_SQL.open_cursor;

   IF pIsAlreadyInOracle = 0 THEN
      DBMS_SQL.parse (source_cursor, 'CREATE USER ' || pUserName || ' IDENTIFIED BY ' || pPassword || ' DEFAULT TABLESPACE &&securityTableSpaceName TEMPORARY TABLESPACE &&tempTableSpaceName', DBMS_SQL.native);
      rows_processed := DBMS_SQL.EXECUTE (source_cursor);
   END IF;

   DBMS_SQL.parse (source_cursor, 'GRANT CONNECT TO ' || pUserName, DBMS_SQL.native);
   rows_processed := DBMS_SQL.EXECUTE (source_cursor);
   DBMS_SQL.parse (source_cursor, 'ALTER USER ' || pUserName || ' GRANT CONNECT THROUGH COEUSER', DBMS_SQL.native);
   rows_processed := DBMS_SQL.EXECUTE (source_cursor);
   DBMS_SQL.parse (source_cursor, 'GRANT ' || pRolesGranted || ' TO ' || pUserName, DBMS_SQL.native);
   rows_processed := DBMS_SQL.EXECUTE (source_cursor);
   DBMS_SQL.parse (source_cursor, 'ALTER USER ' || pUserName || ' DEFAULT ROLE ALL', DBMS_SQL.native);
   rows_processed := DBMS_SQL.EXECUTE (source_cursor);
   DBMS_SQL.parse (source_cursor, 'ALTER USER ' || pUserName || ' PROFILE csuserprofile', DBMS_SQL.native);
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
        DBMS_SQL.parse (source_cursor, 'DROP USER ' || pUserName, DBMS_SQL.native);
        rows_processed := DBMS_SQL.EXECUTE (source_cursor);
      END IF;
      RETURN 'User Code ' || usercode || ' is or has been already taken by another user';
   WHEN role_not_found THEN
	  IF pIsAlreadyInOracle = 0 THEN
	    DBMS_SQL.parse (source_cursor, 'DROP USER ' || pUserName, DBMS_SQL.native);
        rows_processed := DBMS_SQL.EXECUTE (source_cursor);
      END IF;
      RETURN 'Failed to find one or more of the roles to be granted';
END createUser;
/

CREATE OR REPLACE FUNCTION createUserFromLDAP (
   pUserName            IN   people.user_id%TYPE,
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
   pIsActive            IN   people.active%TYPE := 1)
   RETURN VARCHAR2
AS
   source_cursor                INTEGER;
   rows_processed               INTEGER;
   userCode                     people.user_code%TYPE;
   user_or_role_name_conflict   EXCEPTION;
   unique_constraint_violated   EXCEPTION;
   role_not_found               EXCEPTION;
   PRAGMA EXCEPTION_INIT (unique_constraint_violated, -1);
   PRAGMA EXCEPTION_INIT (user_or_role_name_conflict, -1920);
   PRAGMA EXCEPTION_INIT (role_not_found, -1919);
BEGIN
   source_cursor := DBMS_SQL.open_cursor;

  
      DBMS_SQL.parse (source_cursor, 'CREATE USER ' || pUserName || ' IDENTIFIED EXTERNALLY DEFAULT TABLESPACE &&securityTableSpaceName TEMPORARY TABLESPACE &&tempTableSpaceName', DBMS_SQL.native);
      rows_processed := DBMS_SQL.EXECUTE (source_cursor);


   DBMS_SQL.parse (source_cursor, 'GRANT CREATE SESSION TO ' || pUserName, DBMS_SQL.native);
   rows_processed := DBMS_SQL.EXECUTE (source_cursor);
   DBMS_SQL.parse (source_cursor, 'ALTER USER ' || pUserName || ' GRANT CONNECT THROUGH COEUSER', DBMS_SQL.native);
   rows_processed := DBMS_SQL.EXECUTE (source_cursor);
   DBMS_SQL.parse (source_cursor, 'GRANT ' || pRolesGranted || ' TO ' || pUserName, DBMS_SQL.native);
   rows_processed := DBMS_SQL.EXECUTE (source_cursor);
   DBMS_SQL.parse (source_cursor, 'ALTER USER ' || pUserName || ' DEFAULT ROLE ALL', DBMS_SQL.native);
   rows_processed := DBMS_SQL.EXECUTE (source_cursor);
   DBMS_SQL.parse (source_cursor, 'ALTER USER ' || pUserName || ' PROFILE csuserprofile', DBMS_SQL.native);
   rows_processed := DBMS_SQL.EXECUTE (source_cursor);

   IF pUserCode IS NULL THEN
      userCode := pUserName;
   ELSE
      userCode := pUserCode;
   END IF;

   INSERT INTO people (user_id, first_name, middle_name, last_name, email, telephone, int_address, user_code, supervisor_internal_id, site_id, active)
        VALUES (pUserName, pFirstName, pMiddleName, pLastName, pEmail, pTelephone, pAddress, userCode, pSupervisorId, pSiteId, pIsActive);

   RETURN '1';
EXCEPTION
   WHEN user_or_role_name_conflict THEN
      RETURN 'user name ' || pUserName || ' conflicts with another user or role name ';
   WHEN unique_constraint_violated THEN
      DBMS_SQL.parse (source_cursor, 'DROP USER ' || pUserName, DBMS_SQL.native);
      rows_processed := DBMS_SQL.EXECUTE (source_cursor);
      RETURN 'User Code ' || usercode || ' is or has been already taken by another user';
   WHEN role_not_found THEN
      DBMS_SQL.parse (source_cursor, 'DROP USER ' || pUserName, DBMS_SQL.native);
      rows_processed := DBMS_SQL.EXECUTE (source_cursor);
      RETURN 'Failed to find one or more of the roles to be granted';
END createUserFromLDAP;
/


CREATE OR REPLACE FUNCTION deleteRole (
   pRoleName        IN   security_roles.role_name%TYPE,
   pPrivTableName   IN   privilege_tables.privilege_table_name%TYPE)
   RETURN VARCHAR2
AS
   roleId        INTEGER;
   numGrantees   INTEGER;
BEGIN
   SELECT COUNT (*) INTO numGrantees
     FROM dba_role_privs
    WHERE UPPER (granted_role) = UPPER (pRoleName);

   IF numGrantees > 0 THEN
      raise_application_error (-20000, 'Cannot delete ' || pRoleName || ' because it is assigned to existing users.');
   END IF;

   SELECT role_id INTO roleId
     FROM security_roles
    WHERE UPPER (role_name) = UPPER (pRoleName);

   EXECUTE IMMEDIATE 'DELETE FROM ' || pPrivTableName || ' WHERE ROLE_INTERNAL_ID= ' || roleId;
   EXECUTE IMMEDIATE 'DELETE FROM Security_Roles WHERE ROLE_ID = ' || roleId;
   EXECUTE IMMEDIATE 'DROP ROLE ' || pRoleName;

   RETURN '1';
END deleteRole;
/

CREATE OR REPLACE FUNCTION deleteUser (pUserName IN people.user_id%TYPE) RETURN VARCHAR2 AS
   cannot_drop_connected_user   EXCEPTION;
   PRAGMA EXCEPTION_INIT (cannot_drop_connected_user, -1940);
BEGIN
   EXECUTE IMMEDIATE 'DROP USER ' || pUserName || ' CASCADE';

   UPDATE people
      SET active = 0
    WHERE user_id = pUserName;

   RETURN '1';
EXCEPTION
   WHEN cannot_drop_connected_user THEN
      RETURN 'Cannot drop user ' || pUserName || ' because it is currently connected to Oracle';
END deleteUser;
/

CREATE OR REPLACE FUNCTION &&schemaname.."UPDATEROLE" (
   pRoleName        IN   security_roles.role_name%TYPE,
   pPrivTableName   IN   privilege_tables.privilege_table_name%TYPE,
   pPrivValueList   IN   VARCHAR2)
   RETURN VARCHAR2
AS
   roleId   INTEGER;
BEGIN
   SELECT role_id INTO roleId
     FROM security_roles
    WHERE UPPER (role_name) = UPPER (pRoleName);

   EXECUTE IMMEDIATE 'DELETE FROM ' || pPrivTableName || ' WHERE ROLE_INTERNAL_ID= ' || roleId;
   EXECUTE IMMEDIATE 'INSERT INTO ' || pPrivTableName || ' VALUES ( ' || roleId || ', ' || pPrivValueList || ')';

   RETURN '1';
END updaterole;
/

CREATE OR REPLACE FUNCTION updateUser (
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
   END IF;

   RETURN '1';
END updateUser;
/

CREATE OR REPLACE PROCEDURE mapPrivStoRole (
   pRoleName   IN   security_roles.role_name%TYPE,
   pPrivName   IN   VARCHAR2,
   pAction     IN   VARCHAR2)
IS
   cannot_revoke   EXCEPTION;
   PRAGMA EXCEPTION_INIT (cannot_revoke, -1927);

   CURSOR privs_cur (privname_in IN VARCHAR2) IS
      SELECT   PRIVILEGE, SCHEMA, object_name
          FROM &&schemaname..object_privileges
         WHERE privilege_name = privname_in AND SCHEMA IS NOT NULL
      ORDER BY SCHEMA, PRIVILEGE;

   thePrivilege    VARCHAR (30);
   theSchema       VARCHAR (30);
   theObjectName   VARCHAR (30);
   mySql           VARCHAR2 (2000);
   keyword         VARCHAR2 (10);
BEGIN
   IF pAction = 'GRANT' THEN
      keyword := ' TO ';
   ELSE
      keyword := ' FROM ';
   END IF;

   OPEN privs_cur (pPrivName);
   LOOP FETCH privs_cur INTO thePrivilege, theSchema, theObjectName;
   EXIT WHEN privs_cur%NOTFOUND;
      mySql := pAction || ' ' || thePrivilege || ' ON ' || theSchema || '.' || theObjectName || keyword || pRoleName;
      EXECUTE IMMEDIATE mySql;
   END LOOP;

   CLOSE privs_cur;
EXCEPTION
   WHEN cannot_revoke THEN
      RETURN;
   WHEN OTHERS THEN
      raise_application_error (-20000, '&&securitySchemaName  does not have sufficient privileges to ' || mySql);
END mapPrivStoRole;
/

CREATE OR REPLACE FUNCTION changePwd (
   pUserName      IN   people.user_id%TYPE,
   pPassword      IN   VARCHAR2 := NULL,
   pNewPassword   IN   VARCHAR2 := NULL)
   RETURN VARCHAR2
AS
   invalid_user_or_pass   EXCEPTION;
   --source_cursor integer;
   --rows_processed integer;
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

   --source_cursor := dbms_sql.open_cursor;
   IF (pNewPassword IS NOT NULL) THEN
      --dbms_sql.parse (source_cursor, 'ALTER USER ' || pUserName || ' IDENTIFIED BY ' || pNewPassword, dbms_sql.NATIVE);
      --rows_processed := dbms_sql.execute (source_cursor);
      EXECUTE IMMEDIATE 'ALTER USER ' || pUserName || ' IDENTIFIED BY ' || pNewPassword;
   END IF;

   RETURN '1';
EXCEPTION
   WHEN invalid_user_or_pass THEN
      RETURN 'Invalid User Name or Password';
   WHEN cannot_reuse_pwd THEN
      RETURN 'Cannot reuse previously used password.';
END changePwd;
/

CREATE OR REPLACE PROCEDURE grantOnCoreTableToAllRoles (
   pTableName          IN   VARCHAR2,
   pSchemaName         IN   object_privileges.SCHEMA%TYPE,
   pMinimumPrivilege   IN   VARCHAR2,
   pPrivTableName      IN   privilege_tables.privilege_table_name%TYPE)
IS
   TYPE cursor_type IS REF CURSOR;

   roles_cur   cursor_type;
   theRole     VARCHAR2 (30);
BEGIN
   -- Insert object_privs
   DELETE FROM &&schemaname..object_privileges
         WHERE SCHEMA = UPPER (pSchemaName)
           AND object_name = UPPER (pTableName);

   INSERT INTO &&schemaname..object_privileges VALUES (pMinimumPrivilege, 'SELECT', UPPER (pSchemaName), UPPER (pTableName));
   INSERT INTO &&schemaname..object_privileges VALUES (pMinimumPrivilege, 'INSERT', UPPER (pSchemaName), UPPER (pTableName));
   INSERT INTO &&schemaname..object_privileges VALUES (pMinimumPrivilege, 'DELETE', UPPER (pSchemaName), UPPER (pTableName));
   INSERT INTO &&schemaname..object_privileges VALUES (pMinimumPrivilege, 'UPDATE', UPPER (pSchemaName), UPPER (pTableName));

   -- Loop over all roles for this priv table
   manage_roles.getRoles (pPrivTableName, roles_cur);
   LOOP FETCH roles_cur INTO theRole;
   EXIT WHEN roles_cur%NOTFOUND;
      -- grant to role
      EXECUTE IMMEDIATE 'GRANT SELECT, INSERT, DELETE, UPDATE ON ' || pSchemaName || '.' || pTableName || ' TO ' || theRole;
   END LOOP;
END grantOnCoreTableToAllRoles;
/
