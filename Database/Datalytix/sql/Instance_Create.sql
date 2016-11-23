-- Copyright 1998-2014 CambridgeSoft Corporation, an indirect, wholly-owned subsidiary of PerkinElmer, Inc. All rights reserved.

--Creation script for Instance_Create.sql

--#########################################################
--CREATE Tables
--#########################################################

prompt 
prompt Starting "Instance_Create.sql"...
prompt

-- The names for the tablespaces are normally derived from the schema names but may be modified if necessary
DEFINE tableSpaceName = T_&&globalSchemaName._TABL
DEFINE indexTableSpaceName = T_&&globalSchemaName._INDEX
DEFINE tempTableSpaceName = T_&&globalSchemaName._TEMP

-- The file names are normally the tablespace name followed by DBF, but may be modified to include a path if it
-- is desired to place the files in a non-default location.

-- By default, all of these go in the same place, but seperate defines are provided for index
-- tablespaces, lob tablespaces, and table tablespaces, so that these may be easily separated.
-- Replace &&defaultIndexTSPath, &&defaultLobTSPath, or &&defaultTableTSPath with a specific
-- path in the definition of the tablespace you want to move if you want to move some of the tablespaces.

-- DON'T FORGET THE SLASH AT THE END OF THE PATH!
DEFINE defaultTableTSPath= ''
DEFINE defaultIndexTSPath= ''
DEFINE defaultTempTSPath= ''

DEFINE tableSpaceFile = '&&defaultTableTSPath&&tableSpaceName..DBF'
DEFINE indexTableSpaceFile = '&&defaultIndexTSPath&&indexTableSpaceName..DBF'
DEFINE tempTableSpaceFile = '&&defaultTempTSPath&&tempTableSpaceName..DBF'

DEFINE dataBlockSize = 8K
DEFINE indexBlockSize = 8K
DEFINE tempBlockSize = 8K

-- The default sizes are very small, suitable only for demo purposes.
-- Normally, these sizes should be changed to a more realistic initial size.
-- For the Table and IDX tablespaces, approximately 10M/user/year should be allocated,
-- LOB and CTX are very dependant on the number and types of documents and spectra
-- which will be stored, but 20M/user/year is a reasonable starting point.

DEFINE tablespaceSize = 4M
DEFINE idxTablespaceSize = 4M
DEFINE tempTablespaceSize = 40M


DEFINE tablespaceExtent = 'AUTO'
DEFINE idxTablespaceExtent = 'AUTO'
DEFINE tempTablespaceExtent = 1M

DEFINE AsSysDBA = ''
DEFINE OraVersionNumber = 9
DEFINE UpgradeCsSecurity = N

whenever sqlerror exit SQL.SQLCODE rollback

CONNECT &&InstallUser/&&sysPass@&&serverName &&AsSysDBA;

VARIABLE script_name VARCHAR2(50)
COLUMN :script_name NEW_VALUE script_file NOPRINT; 

DECLARE
   n   NUMBER;
BEGIN
   SELECT COUNT (*) INTO n
   FROM dba_users
   WHERE username = UPPER ('&&globalSchemaName');
   
   IF n > 0 THEN
     :script_name:='Instance_CreateTablesAndPermission.sql';
   ELSE
	 :script_name:='Instance_CreateAll.sql';
   END IF;
END;
/

SELECT :script_name FROM DUAL;
@@&script_file


CONNECT &&InstallUser/&&sysPass@&&serverName &&AsSysDBA

VAR Begin VARCHAR2(20);
VAR End VARCHAR2(20);
VAR Elapsed VARCHAR2(100);
exec :Begin:=to_char(systimestamp,'HH:MI:SS.FF4');

SET serveroutput on
BEGIN
	:End:=to_char(systimestamp,'HH:MI:SS.FF4');
	:Elapsed:=to_timestamp(:End,'HH:MI:SS.FF4')-to_timestamp(:Begin,'HH:MI:SS.FF4');

	dbms_output.put_line('Begin: '||:Begin);
	dbms_output.put_line('End: '||:End);
	dbms_output.put_line('Elapsed: '||substr(:Elapsed,instr(:Elapsed,' ')+1,13));
END;
/