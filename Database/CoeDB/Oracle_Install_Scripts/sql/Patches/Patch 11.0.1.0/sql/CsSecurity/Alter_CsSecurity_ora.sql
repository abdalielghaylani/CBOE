--Copyright 1999-2010 CambridgeSoft Corporation. All rights reserved

--#########################################################
--VERIFY TABLESPACE AND USER
--#########################################################

PROMPT Verifying tablespace and User...

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
   BEGIN
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
              || ' DATAFILE ''' || pDataFile || ''''
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
	   
      vDataFile := '&&tempTableSpaceName';
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
 n NUMBER;
BEGIN
   SELECT COUNT (*) INTO N
     FROM dba_users
    WHERE username = UPPER ('&&securitySchemaName');

   IF n = 0 THEN
      EXECUTE IMMEDIATE 'CREATE USER &&securitySchemaName
	IDENTIFIED BY &&securitySchemaPass
	DEFAULT TABLESPACE &&securityTableSpaceName
	TEMPORARY TABLESPACE &&temptablespacename';
   END IF;
END;
/


PROMPT Verifying and renaming tables...
PROMPT 

SET serveroutput on
DECLARE

    
    CURSOR C_TableToRename IS
        SELECT Table_Name
            FROM DBA_Tables 
            WHERE UPPER(Owner)=UPPER('&&securitySchemaName');  

    PROCEDURE VerifyMatchBetweenTables(ATable Varchar2) AS
        LExist   NUMBER;
    BEGIN
       
        EXECUTE IMMEDIATE 
            'SELECT Count(1)
                FROM  DBA_TABLES
                WHERE  TABLE_NAME=UPPER('''||ATable||''') AND OWNER=UPPER(''&&securitySchemaName'')' INTO LExist;
                      
        IF LExist = 1 THEN  
            EXECUTE IMMEDIATE 
                'SELECT Count(1)
                      FROM DUAL
                      WHERE (SELECT Count(1) FROM &&securitySchemaName..'||ATable||') = (SELECT Count(1) FROM &&schemaName..'||ATable||')' INTO LExist;
            IF LExist <> 1 THEN
                DBMS_Output.Put_Line('WARNING Table &&securitySchemaName..'||ATable||': Number of rows not match between source and target.');
            END IF; 
        END IF;
                
    EXCEPTION
        WHEN OTHERS THEN
            DBMS_Output.Put_Line('ERROR verifying number of rows between source and target &&securitySchemaName..'||ATable||' - '||SQLERRM); 
    END;     
    
    PROCEDURE RenameTable(ATable Varchar2) AS
        LExist   NUMBER;
    BEGIN

        EXECUTE IMMEDIATE 'ALTER TABLE &&securitySchemaName..'||ATable||' RENAME TO '||ATable||'_OLD';
        
    EXCEPTION
        WHEN OTHERS THEN 
            DBMS_Output.Put_Line('ERROR renaming &&securitySchemaName..'||ATable||' - The table was not renamed. - '||SQLERRM);
    END;
      
BEGIN
    VerifyMatchBetweenTables('AUDIT_COLUMN');
    VerifyMatchBetweenTables('AUDIT_DELETE');
    VerifyMatchBetweenTables('AUDIT_ROW');
    VerifyMatchBetweenTables('CS_SECURITY_PRIVILEGES');
    VerifyMatchBetweenTables('OBJECT_PRIVILEGES');
    VerifyMatchBetweenTables('PEOPLE');
    VerifyMatchBetweenTables('PRIVILEGE_TABLES');
    VerifyMatchBetweenTables('SECURITY_ROLES');
    VerifyMatchBetweenTables('SITES');
    
    FOR R_TableToRename IN C_TableToRename LOOP
        RenameTable(R_TableToRename.Table_Name);
    END LOOP;
END;
/

GRANT CONNECT, RESOURCE TO &&securitySchemaName;


--#########################################################
--CREATE Synonyms for CS-Security
--#########################################################

PROMPT Creating &&schemaName Synonyms for CS-Security...
PROMPT 

DECLARE
   CURSOR cTables IS
      SELECT privilege_table_name tableName
        FROM &&schemaName..privilege_tables
       WHERE NOT privilege_table_name = 'CS_SECURITY_PRIVILEGES';

   CURSOR cTriggers IS
      SELECT Object_Name
        FROM DBA_Objects
       WHERE UPPER (Owner) = UPPER('&&securitySchemaName') AND Object_Type='TRIGGER';

   PROCEDURE createSynonym (synName IN VARCHAR2, objectType IN VARCHAR2) IS
      n   NUMBER;
      LObject_Type   Varchar2(30):=NULL;
   BEGIN

      BEGIN
          IF  objectType = 'SYNONYM' THEN
               SELECT Object_Type
                       INTO LObject_Type
                       FROM DBA_Objects
                       WHERE UPPER (Owner) = 'PUBLIC' AND UPPER (Object_name)=UPPER(synName);
         	 
               IF LObject_Type = objectType THEN
                   EXECUTE IMMEDIATE 'DROP PUBLIC '||LObject_Type||' '||synName;
               END IF;
           ELSIF objectType = 'TABLE' THEN
               SELECT Count(1)
                       INTO n
                       FROM DBA_Objects
                       WHERE UPPER (Owner) = UPPER('&&securitySchemaName') AND UPPER (Object_name)=UPPER(synName) AND Object_TypE='SYNONYM';
         	 
               IF n<>0 THEN
                   EXECUTE IMMEDIATE 'DROP SYNONYM &&securitySchemaName..'||synName;
               END IF;
           ELSE 
               SELECT Object_Type
                       INTO LObject_Type
                       FROM DBA_Objects
                       WHERE UPPER (Owner) = UPPER('&&securitySchemaName') AND UPPER (Object_name)=UPPER(synName) AND Object_Type<>'PACKAGE BODY' and Object_Type<>'TABLE';
         	 
               IF LObject_Type = objectType THEN
                   EXECUTE IMMEDIATE 'DROP '||LObject_Type||' &&securitySchemaName..'||synName;
               END IF;
           END IF;  
      EXCEPTION
          WHEN NO_DATA_FOUND THEN NULL;  --nothing to do
          WHEN OTHERS THEN DBMS_Output.Put_Line('ERROR processing '||synName||' - '||DBMS_UTILITY.FORMAT_ERROR_STACK);
      END;

      CASE (objectType)
	WHEN 'SYNONYM' THEN
           EXECUTE IMMEDIATE 'CREATE PUBLIC SYNONYM ' || synName || ' FOR &&schemaName..' || synName;
        ELSE
            SELECT Count(1)
                  INTO n
                  FROM DBA_Objects
                  WHERE UPPER (Owner) = '&&schemaName' AND UPPER(Object_name)=UPPER(synName);
            IF n <> 0 THEN
                                         
                EXECUTE IMMEDIATE 'CREATE SYNONYM &&securitySchemaName..' || synName || ' FOR &&schemaName..' || synName;

               CASE (objectType)
                  WHEN 'TABLE' THEN
                     EXECUTE IMMEDIATE 'GRANT REFERENCES, INSERT, UPDATE, DELETE, SELECT ON &&schemaName..' || synName || ' TO &&securitySchemaName WITH GRANT OPTION';
                  WHEN 'SEQUENCE' THEN
                     EXECUTE IMMEDIATE 'GRANT SELECT, ALTER ON &&schemaName..' || synName || ' TO &&securitySchemaName WITH GRANT OPTION';
                  WHEN 'ROLE' THEN
                     EXECUTE IMMEDIATE 'GRANT ' || synName || ' TO &&securitySchemaName WITH ADMIN OPTION';
                  WHEN 'SYNONYM' THEN
                     EXECUTE IMMEDIATE 'CREATE SYNONYM &&securitySchemaName..' || synName || ' FOR &&schemaName..' || synName;
                  ELSE
                     EXECUTE IMMEDIATE 'GRANT EXECUTE ON &&schemaName..' || synName || ' TO &&securitySchemaName WITH GRANT OPTION';
               END CASE;
            END IF;
      END CASE;
   EXCEPTION
          WHEN OTHERS THEN 
	     DBMS_Output.Put_Line('ERROR creating synonym to '||synName||' '||objectType||' - '||DBMS_UTILITY.FORMAT_ERROR_STACK);
   END createSynonym;

  PROCEDURE dropTriggers (triggerName IN VARCHAR2) IS
  BEGIN
         
    EXECUTE IMMEDIATE 'DROP TRIGGER &&securitySchemaName..'||triggerName;
                
                   
  EXCEPTION
       WHEN OTHERS THEN DBMS_Output.Put_Line('ERROR dropping '||triggerName||' TRIGGER - '||DBMS_UTILITY.FORMAT_ERROR_STACK);
  END dropTriggers ;	
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
    --dynamic triggers
    FOR rTriggers IN cTriggers LOOP
        dropTriggers (rTriggers.Object_Name);
    END LOOP;
END;
/