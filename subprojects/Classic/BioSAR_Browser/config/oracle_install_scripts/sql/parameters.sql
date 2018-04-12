--1999-2003 CambridgeSoft Corporation. All rights reserved

SET ECHO OFF
SET verify off

--#########################################################
-- SCRIPT VARIABLES
--######################################################### 
DEFINE schemaName = BIOSARDB
DEFINE schemaPass = ORACLE
DEFINE schemaVersion = 11.0.1
DEFINE appVersion = 11.0.1
DEFINE securitySchemaName = COEDB
DEFINE securitySchemaPass = ORACLE
DEFINE cartSchemaName = CsCartridge
DEFINE cartSchemaPass = CsCartridge 

-- Null for non cs_security applications
DEFINE privTableName = BIOSAR_BROWSER_PRIVILEGES
DEFINE min_priv = SEARCH_USING_FORMGROUP

-- The names for the tablespaces are normally derived from the schema names but may be modified if necessary
DEFINE tableSpaceName = T_&&schemaName
DEFINE indexTableSpaceName = T_&&schemaName._INDEX
DEFINE tempTableSpaceName = T_&&securitySchemaName._TEMP
DEFINE securityTableSpaceName = T_&&securitySchemaName

-- The file names are normally the tablespace name followed by DBF, but may be modified to include a path if it
-- is desired to place the files in a non-default location.

-- By default, all of these go in the same place, but seperate defines are provided for index
-- tablespaces, lob tablespaces, and table tablespaces, so that these may be easily separated.
-- Replace &&defaultIndexTSPath, &&defaultLobTSPath, or &&defaultTableTSPath with a specific
-- path in the definition of the tablespace you want to move if you want to move some of the tablespaces.

-- DON'T FORGET THE SLASH AT THE END OF THE PATH!
DEFINE defaultTableTSPath= ''
DEFINE defaultIndexTSPath= ''

DEFINE tableSpaceFile = '&&defaultTableTSPath&&tableSpaceName..DBF' 
DEFINE indexTableSpaceFile = '&&defaultIndexTSPath&&indexTableSpaceName..DBF'

DEFINE dataBlockSize = 8K 
DEFINE indexBlockSize = 8K


-- The default sizes are very small, suitable only for demo purposes.
-- Normally, these sizes should be changed to a more realistic initial size.
-- For the Table and IDX tablespaces, approximately 10M/user/year should be allocated,
-- LOB and CTX are very dependant on the number and types of documents and spectra
-- which will be stored, but 20M/user/year is a reasonable starting point.

DEFINE tablespaceSize    = 8M
DEFINE idxTablespaceSize   = 4M

--DEFINE tablespaceExtent   =  128K
--DEFINE idxTablespaceExtent  =  128K

DEFINE tablespaceExtent   =  'AUTO'
DEFINE idxTablespaceExtent  =  'AUTO'

