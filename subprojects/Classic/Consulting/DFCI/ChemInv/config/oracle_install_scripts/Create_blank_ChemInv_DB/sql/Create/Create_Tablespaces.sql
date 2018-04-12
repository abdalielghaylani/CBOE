-- Copyright Cambridgesoft Corp 2001-2006 all rights reserved

--#########################################################
-- Create tablespaces
--######################################################### 

Connect &&InstallUser/&&sysPass@&&serverName

prompt '#########################################################'
prompt 'Creating tablespaces...'
prompt '#########################################################'


DECLARE
	
	PROCEDURE createTableSpace(pName in varchar2, pDataFile in varchar2, pSize in varchar2, pBlockSize in varchar2, pExtentSize in varchar2)
		IS	
			n NUMBER;
			blockSizeClause varchar2(50);
			extentSizeClause varchar2(50);
			segmentSpaceClause varchar2(50);
			storageClause varchar2(100);
			mySql varchar2(300);
		BEGIN
			select count(*) into n from dba_tablespaces where tablespace_name = Upper(pName);
			if n = 0 then
				--if OraVersionNumber =  8 then 
				--	blockSizeClause := '';
				--	segmentSpaceClause := '';
				--	storageClause := '';
				--else
					blockSizeClause := 'BLOCKSIZE ' || pBlockSize;
					segmentSpaceClause := ' SEGMENT SPACE MANAGEMENT AUTO ';	
					storageClause := '';
				--end if;
				
				
				if pExtentSize = '' then
					extentSizeClause := 'AUTOALLOCATE ';
				else
					extentSizeClause := 'UNIFORM SIZE ' || pExtentSize;
				end if;
			
			  mySql :=	'CREATE TABLESPACE ' ||pName || 
						' DATAFILE ''' || pDataFile ||''''||
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
	createTableSpace('&&lobsTableSpaceName','&&lobsTableSpaceFile','&&lobTableSpaceSize','&&lobBlockSize','&&lobTablespaceExtent');
	createTableSpace('&&cscartTableSpaceName','&&cscartTableSpaceFile','&&cscartTableSpaceSize','&&cscartBlockSize','&&cscartTablespaceExtent');
	createTableSpace('&&auditTableSpaceName','&&auditTableSpaceFile','&&auditTableSpaceSize','&&auditBlockSize','&&auditTablespaceExtent');
END;
/

show errors;