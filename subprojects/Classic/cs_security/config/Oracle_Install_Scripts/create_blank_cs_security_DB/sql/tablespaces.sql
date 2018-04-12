


--#########################################################
--CREATES TABLESPACES
--#########################################################

-- For consistency we use a standardized procedure for creating our permanent table spaces.
-- Creates Locally managed table spaces.  Maybe uniform or autoalocated.  
-- Uses block size and auto space management for 9i
-- Uses default space management parameters for 8i
	

prompt 'creating tablespaces.sql...'

DECLARE
	
	PROCEDURE createTableSpace(pName in varchar2, pDataFile in varchar2, pSize in varchar2, pBlockSize in varchar2, pExtentSize in varchar2)
		IS	
			n NUMBER;
			blockSizeClause varchar2(50);
			extentSizeClause varchar2(50);
			segmentSpaceClause varchar2(50);
			storageClause varchar2(100);
			mySql varchar2(300);
			sysTSPath varchar2(2000);
			vDataFile varchar2(2000);
		
		BEGIN
		   select  nvl(substr(file_name, 1, instr(file_name, '/',-1)),substr(file_name, 1, instr(file_name, '\',-1))) into sysTSPath
		   from dba_data_files
		   where TABLESPACE_NAME = 'SYSTEM';
		   
		   vDataFile := pDataFile;
		   if (instr(vDataFile , '/',1) + instr(vDataFile , '\',1)) = 0  then
			vDataFile := sysTSPath || vDataFile; 
		   end if;
     
			select count(*) into n from dba_tablespaces where tablespace_name = Upper(pName);
			if n = 0 then
				if &&OraVersionNumber =  8 then 
					blockSizeClause := '';
					segmentSpaceClause := '';
					storageClause := '';
				else
					blockSizeClause := 'BLOCKSIZE ' || pBlockSize;
					segmentSpaceClause := ' SEGMENT SPACE MANAGEMENT AUTO ';	
					storageClause := '';
				end if;
				
				
				if pExtentSize = 'AUTO' then
					extentSizeClause := 'AUTOALLOCATE ';
				else
					extentSizeClause := 'UNIFORM SIZE ' || pExtentSize;
				end if;
			
			  mySql :=	'CREATE TABLESPACE ' ||pName || 
						' DATAFILE ''' || vDataFile ||''''||
						' SIZE ' || pSize ||
						' REUSE AUTOEXTEND ON MAXSIZE UNLIMITED ' ||
						storageClause ||
						segmentSpaceClause ||
						blockSizeClause ||
						' EXTENT MANAGEMENT LOCAL ' || extentSizeClause;
				
				execute immediate mySql; 			
			 end if;
		END createTableSpace;
BEGIN
	
	createTableSpace('&&tableSpaceName','&&tableSpaceFile','&&tableSpaceSize','&&dataBlockSize','&&tablespaceExtent');
	createTableSpace('&&indexTableSpaceName','&&indexTableSpaceFile','&&idxTableSpaceSize','&&indexBlockSize','&&idxTablespaceExtent');
	createTableSpace('&&auditTableSpaceName','&&auditTableSpaceFile','&&auditTableSpaceSize','&&auditBlockSize','&&auditTablespaceExtent');
	
END;
/



