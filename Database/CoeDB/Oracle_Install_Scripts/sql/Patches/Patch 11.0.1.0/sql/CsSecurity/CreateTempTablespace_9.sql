--Copyright 1999-2010 CambridgeSoft Corporation. All rights reserved

prompt 
prompt Starting "CreateTablespace_9.sql"...
prompt 

--#########################################################
--Oracle 9i Temporary tablespace syntax
--#########################################################

--- 

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

    IF LExist = 0 and '&&BypassTablespaceCreateAndDrop' = 'N' THEN
        EXECUTE IMMEDIATE 
            'CREATE TEMPORARY TABLESPACE &&tempTableSpaceName
                TEMPFILE ''' || vDataFile || ''' SIZE &&tempTablespaceSize REUSE
                AUTOEXTEND ON MAXSIZE UNLIMITED
                    EXTENT MANAGEMENT LOCAL UNIFORM SIZE &&tempTablespaceExtent';
    END IF;
END;
/

DECLARE
    LExist NUMBER;
BEGIN
    SELECT count(*) INTO LExist FROM DBA_Tablespaces WHERE Tablespace_Name = Upper('&&auditTableSpaceName');    

    IF LExist = 0 and '&&BypassTablespaceCreateAndDrop' = 'N' THEN
        EXECUTE IMMEDIATE 
            'CREATE TABLESPACE &&auditTableSpaceName
		DATAFILE ''&&auditTableSpaceFile''
        	SIZE &&auditTableSpaceSize REUSE
		AUTOEXTEND ON MAXSIZE UNLIMITED
        	EXTENT MANAGEMENT LOCAL UNIFORM SIZE &&auditTablespaceExtent';
    END IF;
END;
/











