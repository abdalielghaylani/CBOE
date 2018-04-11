--Copyright 1999-2003 CambridgeSoft Corporation. All rights reserved

prompt 
prompt Starting "parameters.sql"...
prompt

SET ECHO OFF
SET verify off

--#########################################################
-- SCRIPT VARIABLES
--#########################################################
DEFINE schemaName = COEDB
DEFINE schemaPass = ORACLE
DEFINE schemaVersion = 11.0.1.0
DEFINE appVersion = 8.0.269
DEFINE securitySchemaName = COEDB
DEFINE securitySchemaNameOld = CS_SECURITY  --Temporary
DEFINE securitySchemaPass = ORACLE
DEFINE cartSchemaName = CSCARTRIDGE
DEFINE cartSchemaPass = CSCARTRIDGE

-- Null for non cs_security applications
DEFINE privTableName = CHEM_REG_PRIVILEGES
DEFINE min_priv = SEARCH_REG

-- The names for the tablespaces are normally derived from the schema names but may be modified if necessary
DEFINE tableSpaceName = T_&&schemaName._TABL
DEFINE indexTableSpaceName = T_&&schemaName._INDEX
DEFINE lobsTableSpaceName = T_&&schemaName._LOBS
DEFINE cscartTableSpaceName = T_&&schemaName._CSCART
DEFINE auditTableSpaceName = T_&&schemaName._AUDIT
DEFINE securityTableSpaceName = T_&&securitySchemaName
DEFINE tempTableSpaceName = T_&&securitySchemaName._TEMP

-- The file names are normally the tablespace name followed by DBF, but may be modified to include a path if it
-- is desired to place the files in a non-default location.

-- By default, all of these go in the same place, but seperate defines are provided for index
-- tablespaces, lob tablespaces, and table tablespaces, so that these may be easily separated.
-- Replace &&defaultIndexTSPath, &&defaultLobTSPath, or &&defaultTableTSPath with a specific
-- path in the definition of the tablespace you want to move if you want to move some of the tablespaces.

-- DON'T FORGET THE SLASH AT THE END OF THE PATH!
DEFINE defaultTableTSPath= ''
DEFINE defaultIndexTSPath= ''
DEFINE defaultLobTSPath= ''
DEFINE defaultCscartTSPath= ''
DEFINE defaultAuditTSPath= ''
DEFINE defaultTempTSPath= ''
DEFINE defaultSecurityTSPath= '&&defaultTableTSPath'

DEFINE tableSpaceFile = '&&defaultTableTSPath&&tableSpaceName..DBF'
DEFINE indexTableSpaceFile = '&&defaultIndexTSPath&&indexTableSpaceName..DBF'
DEFINE lobsTableSpaceFile = '&&defaultLobTSPath&&lobsTableSpaceName..DBF'
DEFINE cscartTableSpaceFile = '&&defaultCscartTSPath&&cscartTableSpaceName..DBF'
DEFINE auditTableSpaceFile = '&&defaultAuditTSPath&&auditTableSpaceName..DBF'
DEFINE tempTableSpaceFile = '&&defaultTempTSPath&&tempTableSpaceName..DBF'
DEFINE securityTableSpaceFile = '&&defaultSecurityTSPath&&securityTableSpaceName..DBF'

DEFINE dataBlockSize = 8K
DEFINE indexBlockSize = 8K
DEFINE lobBlockSize = 8K
DEFINE cscartBlockSize = 8K
DEFINE auditBlockSize = 8K
DEFINE securityBlockSize = 8K
DEFINE tempBlockSize = 8K

-- The default sizes are very small, suitable only for demo purposes.
-- Normally, these sizes should be changed to a more realistic initial size.
-- For the Table and IDX tablespaces, approximately 10M/user/year should be allocated,
-- LOB and CTX are very dependant on the number and types of documents and spectra
-- which will be stored, but 20M/user/year is a reasonable starting point.

DEFINE tablespaceSize = 4M
DEFINE idxTablespaceSize = 4M
DEFINE lobTablespaceSize = 4M
DEFINE cscartTablespaceSize = 4M
DEFINE auditTablespaceSize = 40M
DEFINE tempTablespaceSize = 40M
DEFINE securityTablespaceSize = 4M

DEFINE tablespaceExtent = 'AUTO'
DEFINE idxTablespaceExtent = 'AUTO'
DEFINE lobTablespaceExtent = 'AUTO'
DEFINE cscartTablespaceExtent = 'AUTO'
DEFINE auditTablespaceExtent = 4M
DEFINE tempTablespaceExtent = 1M
DEFINE securityTablespaceExtent = 128K

-- The storage size for base64cdx data in chemical structures table
DEFINE lobB64cdx = 2K

DEFINE AsSysDBA = ''
DEFINE OraVersionNumber = 9
DEFINE UpgradeCsSecurity = N
