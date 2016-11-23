-- Copyright 1998-2014 CambridgeSoft Corporation, an indirect, wholly-owned subsidiary of PerkinElmer, Inc. All rights reserved.

prompt 
prompt Starting "Instance_parameters.sql"...
prompt

SET ECHO OFF
SET verify off

--#########################################################
-- SCRIPT VARIABLES
--#########################################################
DEFINE schemaName = COEDB
DEFINE globalSchemaName = COEUSER
DEFINE globalSchemaPass = ORACLE
DEFINE schemaVersion = 11.0.1.0
DEFINE appVersion = 8.0.269

-- The names for the tablespaces are normally derived from the schema names but may be modified if necessary
DEFINE tableSpaceName = T_&&schemaName._TABL
DEFINE indexTableSpaceName = T_&&schemaName._INDEX
DEFINE tempTableSpaceName = T_&&schemaName._TEMP

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
-- The flag indicates whether drop and create the tablespace.
-- Y: Scripts will skip drop the tablespace if they already in database, and skip to recreate tablespaces. 
--    But if the required tablespaces not exist, will show warning message and exist the scripts.
-- N: Scripts will drop the old tablespaces and recreate them.
DEFINE BypassTablespaceCreateAndDrop = N