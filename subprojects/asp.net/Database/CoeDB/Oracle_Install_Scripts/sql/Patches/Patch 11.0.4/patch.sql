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

ALTER TABLE COEREPORT ADD TYPE VARCHAR2(5) NOT NULL;
ALTER TABLE COEREPORT ADD CATEGORY VARCHAR2(255);

ALTER TABLE COEDATAVIEW ADD (APPLICATION VARCHAR2(50));

-- Table COE_PLAN_TABLE Creation is needed for structure list searches to identify underlying cscartridge indexes:
CREATE GLOBAL TEMPORARY TABLE COE_PLAN_TABLE (
        statement_id       varchar2(30),
        plan_id            number,
        timestamp          date,
        remarks            varchar2(4000),
        operation          varchar2(30),
        options            varchar2(255),
        object_node        varchar2(128),
        object_owner       varchar2(30),
        object_name        varchar2(30),
        object_alias       varchar2(65),
        object_instance    numeric,
        object_type        varchar2(30),
        optimizer          varchar2(255),
        search_columns     number,
        id                 numeric,
        parent_id          numeric,
        depth              numeric,
        position           numeric,
        cost               numeric,
        cardinality        numeric,
        bytes              numeric,
        other_tag          varchar2(255),
        partition_start    varchar2(255),
        partition_stop     varchar2(255),
        partition_id       numeric,
        other              long,
        distribution       varchar2(30),
        cpu_cost           numeric,
        io_cost            numeric,
        temp_space         numeric,
        access_predicates  varchar2(4000),
        filter_predicates  varchar2(4000),
        projection         varchar2(4000),
        time               numeric,
        qblock_name        varchar2(30),
        other_xml          clob
) ON COMMIT PRESERVE ROWS;

GRANT ALL ON COE_PLAN_TABLE TO CSS_USER;
GRANT ALL ON COE_PLAN_TABLE TO CSS_ADMIN;

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
create or replace procedure GrantPrivsToRole2(p_roleName security_roles.role_name%TYPE)
is
l_privilegeTable privilege_tables.privilege_table_name%TYPE;
l_roleId security_roles.role_id%TYPE;
l_privValue NUMBER;
BEGIN

					--' get priv table name and role_id
					SELECT role_id, privilege_table_name INTO l_roleId, l_privilegeTable FROM security_roles, privilege_tables WHERE privilege_table_int_id = privilege_table_id AND role_name = p_roleName;


					--' To avoid role dependencies it is best to process first the revokes and then the grants.
					-- That will lead to a consistent grant state
					--' loop over the privileges       
					FOR priv_rec IN (SELECT  * FROM user_tab_columns WHERE column_name <> 'ROLE_INTERNAL_ID' AND table_name = l_privilegeTable)
					LOOP
										--' revoke the grant for privileges that are not set.
										EXECUTE IMMEDIATE 'SELECT ' || priv_rec.column_name || ' FROM ' || l_privilegeTable || ' WHERE role_internal_id = ' || l_roleId  INTO l_privValue;
										IF l_privValue <> 1 THEN
													mapprivstorole(p_roleName, priv_rec.column_name, 'REVOKE');
										END IF;
					END LOOP;   
					
					--' loop over the privileges
					FOR priv_rec IN (SELECT  * FROM user_tab_columns WHERE column_name <> 'ROLE_INTERNAL_ID' AND table_name = l_privilegeTable)
					LOOP
										--' issue the grant if the role has the privilege
										EXECUTE IMMEDIATE 'SELECT ' || priv_rec.column_name || ' FROM ' || l_privilegeTable || ' WHERE role_internal_id = ' || l_roleId  INTO l_privValue;
										IF l_privValue = 1 THEN
													mapprivstorole(p_roleName, priv_rec.column_name, 'GRANT');
										END IF;
					END LOOP;

END GrantPrivsToRole2;
/

create or replace FUNCTION       "UPDATEROLE" (
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

   -- Recreate the role definition in the application privilege table
   EXECUTE IMMEDIATE 'DELETE FROM ' || pPrivTableName || ' WHERE ROLE_INTERNAL_ID= ' || roleId;
   EXECUTE IMMEDIATE 'INSERT INTO ' || pPrivTableName || ' VALUES ( ' || roleId || ', ' || pPrivValueList || ')';
   -- Issue the Oracle grants associated with the privileges 
   GrantPrivsToRole2(pRoleName);
   RETURN '1';
END updaterole;
/

create or replace FUNCTION       createRole (
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
	  EXECUTE IMMEDIATE 'GRANT EXECUTE ON COEDB.LOGIN TO ' || pRoleName;
      EXECUTE IMMEDIATE 'REVOKE ' || pRoleName || ' FROM CS_SECURITY';

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
   -- Issue the Oracle grants associated with the privileges 
   GrantPrivsToRole2(pRoleName);
   RETURN '1';
END createRole;
/

create or replace FUNCTION                                           createUser (
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
   pActivatingUser	IN	 CHAR := '0')
   RETURN VARCHAR2
AS
   source_cursor                INTEGER;
   rows_processed               INTEGER;
   user_count                   INTEGER; 
   people_count                 INTEGER; 
   user_oldpassword             VARCHAR(255);
   user_newpassword             VARCHAR(255);
   userCode                     people.user_code%TYPE;
   quotedUserName				people.user_id%TYPE;
   user_or_role_name_conflict   EXCEPTION;
   unique_constraint_violated   EXCEPTION;
   role_not_found               EXCEPTION;
   PRAGMA EXCEPTION_INIT (unique_constraint_violated, -1);
   PRAGMA EXCEPTION_INIT (user_or_role_name_conflict, -1920);
   PRAGMA EXCEPTION_INIT (role_not_found, -1919);
BEGIN
   source_cursor := DBMS_SQL.open_cursor;
  quotedUserName := UPPER('"' || pUserName || '"');
   IF pIsAlreadyInOracle = 0 THEN
    SELECT count(*) into user_count FROM DBA_USERS where username=pUserName;
      if user_count >0 then
          SELECT count(*) into people_count FROM people where USER_CODE=pUserName;
          if people_count >0 then
            RETURN 'user name ' || pUserName || ' conflicts with another user ';
          else
            SELECT password into user_oldpassword FROM SYS.USER$ where name=pUserName;
            DBMS_SQL.parse (source_cursor, 'ALTER USER ' || quotedUserName || ' IDENTIFIED BY ' || pPassword || ' DEFAULT TABLESPACE T_COEDB TEMPORARY TABLESPACE T_COEDB_TEMP', DBMS_SQL.native);
            rows_processed := DBMS_SQL.EXECUTE (source_cursor);
             SELECT password into user_newpassword FROM SYS.USER$ where name=pUserName;
            DBMS_SQL.parse (source_cursor, 'ALTER USER ' || quotedUserName || ' IDENTIFIED BY  VALUES ''' ||  user_oldpassword || ''' DEFAULT TABLESPACE T_COEDB TEMPORARY TABLESPACE T_COEDB_TEMP', DBMS_SQL.native);
            rows_processed := DBMS_SQL.EXECUTE (source_cursor);
             if user_oldpassword <> user_newpassword then
                RETURN 'User ' || pUserName || ' was not created for the purpose of LDAP';
              end if;
          end if ;
        else
         DBMS_SQL.parse (source_cursor, 'CREATE USER ' || quotedUserName || ' IDENTIFIED BY ' || pPassword || ' DEFAULT TABLESPACE T_COEDB TEMPORARY TABLESPACE T_COEDB_TEMP', DBMS_SQL.native);
          rows_processed := DBMS_SQL.EXECUTE (source_cursor);
        end if;
           
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

CREATE OR REPLACE FUNCTION coedb.createCOERole (
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
	    EXECUTE IMMEDIATE 'GRANT EXECUTE ON COEDB.LOGIN TO ' || pRoleName;
      --EXECUTE IMMEDIATE 'GRANT CSS_USER TO ' || pRoleName;
      EXECUTE IMMEDIATE 'REVOKE ' || pRoleName || ' FROM COEDB';
   ELSE
      INSERT INTO security_roles (privilege_table_int_id, role_name, coeidentifier)
           VALUES (NULL, UPPER (pRoleName), UPPER (pCOEIdentifier))
        RETURNING role_id
             INTO roleId;
   END IF;
   GrantPrivsToRole2(pRoleName); --CSBR 127261 : To fix the issue related to new role creation and assigning the role to a new user.
   RETURN '1';
END createCOERole;
/
--#########################################################
--PACKAGES
--#########################################################

set define off

---@"sql\Patches\Patch &&currentPatch\Packages\xxxxxxx"

set define on

--#########################################################
--DATA
--#########################################################

--COMMIT;



--#####################################################################
--Group Security Changes
--#####################################################################

--@"sql\Patches\Patch &&currentPatch\sql\xxxxxxxxx"



--#####################################################################
-- COEManager PageControlSettings
--#####################################################################

--@"sql\Patches\Patch &&currentPatch\sql\xxxxx"


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

@&&setNextPatch 









