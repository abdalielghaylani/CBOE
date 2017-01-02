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
   add_new_part_coefullpage(pUserName);

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
