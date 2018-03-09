--Copyright 1999-2010 CambridgeSoft Corporation. All rights reserved

--#########################################################
--CREATES TABLESPACE AND USER
--#########################################################

PROMPT Creating tablespace and User...

DECLARE
   PROCEDURE createTableSpace (
      pName         IN   VARCHAR2,
      pDataFile     IN   VARCHAR2,
      pSize         IN   VARCHAR2,
      pBlockSize    IN   VARCHAR2,
      pExtentSize   IN   VARCHAR2)
   IS
      n                    NUMBER;
      blockSizeClause      VARCHAR2 (50);
      extentSizeClause     VARCHAR2 (50);
      segmentSpaceClause   VARCHAR2 (50);
      storageClause        VARCHAR2 (100);
      mySql                VARCHAR2 (300);
	  sysTSPath varchar2(2000);
      vDataFile varchar2(2000);
   BEGIN
	  select  nvl(substr(file_name, 1, instr(file_name, '/',-1)),substr(file_name, 1, instr(file_name, '\',-1))) into sysTSPath
	from dba_data_files
	 where TABLESPACE_NAME = 'SYSTEM' and rownum<2;
	   
      vDataFile := pDataFile;
      if (instr(vDataFile , '/',1) + instr(vDataFile , '\',1)) = 0  then
	vDataFile := sysTSPath || vDataFile; 
      end if;
	  
      SELECT COUNT (*) INTO n
        FROM dba_tablespaces
       WHERE tablespace_name = UPPER (pname);

      IF n = 0 THEN
         IF &&oraVersionNumber = 8 THEN
            blockSizeClause := '';
            segmentSpaceClause := '';
            storageClause := '';
         ELSE
            blockSizeClause := 'BLOCKSIZE ' || pBlockSize;
            segmentSpaceClause := ' SEGMENT SPACE MANAGEMENT AUTO ';
            storageClause := '';
         END IF;

         IF pExtentSize = 'AUTO' THEN
            extentSizeClause := 'AUTOALLOCATE ';
         ELSE
            extentSizeClause := 'UNIFORM SIZE ' || pExtentSize;
         END IF;

         mySql := 'CREATE TABLESPACE ' || pName
              || ' DATAFILE ''' || vDataFile || ''''
              || ' SIZE ' || pSize
              || ' REUSE AUTOEXTEND ON MAXSIZE UNLIMITED ' || storageClause || segmentSpaceClause || blockSizeClause
              || ' EXTENT MANAGEMENT LOCAL ' || extentSizeClause;

         EXECUTE IMMEDIATE mySql;
      END IF;
   END createTableSpace;
BEGIN
	createTableSpace ('&&securityTableSpaceName', '&&securityTableSpaceFile', '&&securityTableSpaceSize', '&&securityBlockSize', '&&securityTablespaceExtent');
END;
/

DECLARE
    LExist NUMBER;
	sysTSPath varchar2(2000);
	vDataFile varchar2(2000);
BEGIN
	select  nvl(substr(file_name, 1, instr(file_name, '/',-1)),substr(file_name, 1, instr(file_name, '\',-1))) into sysTSPath
	from dba_data_files
	 where TABLESPACE_NAME = 'SYSTEM' and rownum<2;
	   
      vDataFile := '&&tempTableSpaceFile';
      if (instr(vDataFile , '/',1) + instr(vDataFile , '\',1)) = 0  then
	vDataFile := sysTSPath || vDataFile; 
      end if;
	  
    SELECT count(*) INTO LExist FROM DBA_Tablespaces WHERE Tablespace_Name = Upper('&&tempTableSpaceName');    

    IF LExist = 0 THEN
        EXECUTE IMMEDIATE 
            'CREATE TEMPORARY TABLESPACE &&tempTableSpaceName
                TEMPFILE ''' || vDataFile || ''' SIZE &&tempTablespaceSize REUSE
                AUTOEXTEND ON MAXSIZE UNLIMITED
                    EXTENT MANAGEMENT LOCAL UNIFORM SIZE &&tempTablespaceExtent';
    END IF;
END;
/
  
DECLARE
   n   NUMBER;
BEGIN
   SELECT COUNT (*) INTO n
     FROM dba_users
    WHERE username = UPPER ('&&securitySchemaName');

   IF n > 0 THEN
      EXECUTE IMMEDIATE 'DROP USER &&securitySchemaName CASCADE';
   END IF;
END;
/

CREATE USER &&securitySchemaName
   IDENTIFIED BY &&securitySchemaPass
   DEFAULT TABLESPACE &&securityTableSpaceName
   TEMPORARY TABLESPACE &&temptablespacename;

GRANT CONNECT, RESOURCE TO &&securitySchemaName;


--#########################################################
--CREATE Synonyms for CS-Security
--#########################################################

PROMPT Creating &&schemaName Synonyms for CS-Security...

DECLARE
   CURSOR cTables IS
      SELECT privilege_table_name tableName
        FROM &&schemaName..privilege_tables
       WHERE NOT privilege_table_name = 'CS_SECURITY_PRIVILEGES';

   PROCEDURE createSynonym (synName IN VARCHAR2, objectType IN VARCHAR2) IS
      n   NUMBER;
   BEGIN
       insert into coedb.log(LOGCOMMENT) values(synName||' '||objectType );
commit;

      SELECT COUNT (*) INTO n
        FROM dba_synonyms
       WHERE UPPER (synonym_name) = synName
         AND UPPER (table_owner) = '&&securitySchemaName';

      IF n = 0 THEN
         CASE (objectType)
	    WHEN 'SYNONYM' THEN
               EXECUTE IMMEDIATE 'CREATE PUBLIC SYNONYM ' || synName || ' FOR &&securitySchemaName..' || synName;
            ELSE
               EXECUTE IMMEDIATE 'CREATE SYNONYM &&securitySchemaName..' || synName || ' FOR &&schemaName..' || synName;

               CASE (objectType)
                  WHEN 'TABLE' THEN
                     EXECUTE IMMEDIATE 'GRANT REFERENCES, INSERT, UPDATE, DELETE, SELECT ON &&schemaName..' || synName || ' TO &&securitySchemaName WITH GRANT OPTION';
                  WHEN 'SEQUENCE' THEN
                     EXECUTE IMMEDIATE 'GRANT SELECT, ALTER ON &&schemaName..' || synName || ' TO &&securitySchemaName WITH GRANT OPTION';
                  WHEN 'ROLE' THEN
                     EXECUTE IMMEDIATE 'GRANT ' || synName || ' TO &&securitySchemaName WITH ADMIN OPTION';
                  ELSE
                     EXECUTE IMMEDIATE 'GRANT EXECUTE ON &&schemaName..' || synName || ' TO &&securitySchemaName WITH GRANT OPTION';
               END CASE;
         END CASE;
      END IF;
	EXCEPTION
WHEN OTHERS THEN 
  insert into coedb.log(LOGCOMMENT) values(' ERROR '||synName||' '||objectType );
commit;

   END createSynonym;
BEGIN
    --tables
    createSynonym ('AUDIT_COLUMN', 'TABLE');
    createSynonym ('AUDIT_DELETE', 'TABLE');
    createSynonym ('AUDIT_ROW', 'TABLE');
    createSynonym ('CS_SECURITY_PRIVILEGES', 'TABLE');
    createSynonym ('OBJECT_PRIVILEGES', 'TABLE');
    createSynonym ('PEOPLE', 'TABLE');
    createSynonym ('PRIVILEGE_TABLES', 'TABLE');
    createSynonym ('SECURITY_ROLES', 'TABLE');
    createSynonym ('SITES', 'TABLE');
    --functions
    createSynonym ('CHANGEPWD', 'FUNCTION');
    createSynonym ('CREATEROLE', 'FUNCTION');
    createSynonym ('CREATEUSER', 'FUNCTION');
    createSynonym ('DELETEROLE', 'FUNCTION');
    createSynonym ('DELETEUSER', 'FUNCTION');
    createSynonym ('UPDATEROLE', 'FUNCTION');
    createSynonym ('UPDATEUSER', 'FUNCTION');
    --procedures
    createSynonym ('GRANTONCORETABLETOALLROLES', 'PROCEDURE');
    createSynonym ('MAPPRIVSTOROLE', 'PROCEDURE');
    --packages
    createSynonym ('AUDIT_TRAIL', 'PACKAGE');
    createSynonym ('LOGIN', 'PACKAGE');
    createSynonym ('MANAGE_ROLES', 'PACKAGE');
    createSynonym ('MANAGE_USERS', 'PACKAGE');
    --sequences
    createSynonym ('SITES_SEQ', 'SEQUENCE');
    createSynonym ('PEOPLE_SEQ', 'SEQUENCE');
    createSynonym ('SECURITY_ROLES_SEQ', 'SEQUENCE');
    createSynonym ('PRIVILEGE_TABLES_SEQ', 'SEQUENCE');
    createSynonym ('SEQ_RID', 'SEQUENCE');
    createSynonym ('SEQ_AUDIT', 'SEQUENCE');
    --synonym
    createSynonym ('SECURITY_ROLES', 'SYNONYM');
    createSynonym ('PEOPLE', 'SYNONYM');
    createSynonym ('PRIVILEGE_TABLES', 'SYNONYM');
    createSynonym ('SITES', 'SYNONYM');
    --roles
    createSynonym ('CSS_USER', 'ROLE');
    createSynonym ('CSS_ADMIN', 'ROLE');
    --dynamic tables
    FOR rTables IN cTables LOOP
        createSynonym (rTables.tableName, 'TABLE');
    END LOOP;
END;
/