--#########################################################
--CREATES TABLESPACES
--#########################################################

PROMPT Starting tablespaces...

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
              || ' DATAFILE ''' || vDataFile ||''''
              || ' SIZE ' || pSize 
              || ' REUSE AUTOEXTEND ON MAXSIZE UNLIMITED ' || storageClause || segmentSpaceClause || blockSizeClause
              || ' EXTENT MANAGEMENT LOCAL ' || extentSizeClause;

         EXECUTE IMMEDIATE mySql;
      END IF;
   END createTableSpace;
BEGIN
   createTableSpace ('&&tableSpaceName', '&&tableSpaceFile', '&&tableSpaceSize', '&&dataBlockSize', '&&tablespaceExtent');
   createTableSpace ('&&indexTableSpaceName', '&&indexTableSpaceFile', '&&idxTableSpaceSize', '&&indexBlockSize', '&&idxTablespaceExtent');
   createTableSpace ('&&lobsTableSpaceName', '&&lobsTableSpaceFile', '&&lobTableSpaceSize', '&&lobBlockSize', '&&lobTablespaceExtent');
   createTableSpace ('&&cscartTableSpaceName', '&&cscartTableSpaceFile', '&&cscartTableSpaceSize', '&&cscartBlockSize', '&&cscartTablespaceExtent');
   createTableSpace ('&&auditTableSpaceName', '&&auditTableSpaceFile', '&&auditTableSpaceSize', '&&auditBlockSize', '&&auditTablespaceExtent');
   createTableSpace ('&&securityTableSpaceName', '&&securityTableSpaceFile', '&&securityTablespaceSize', '&&securityBlockSize', '&&securityTablespaceExtent');
 
END;
/