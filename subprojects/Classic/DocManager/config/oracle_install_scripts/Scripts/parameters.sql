
SET ECHO OFF
SET verify off

--#########################################################
-- SCRIPT VARIABLES
--######################################################### 
DEFINE schemaName = DOCMGR
DEFINE schemaPass = ORACLE
DEFINE schemaVersion = 11.0.1
DEFINE appVersion = 11.0.1
DEFINE securitySchemaName = COEDB
DEFINE securitySchemaPass = ORACLE
DEFINE regName = REGDB
DEFINE regPass = ORACLE

-- Null for non COEDB applications
DEFINE privTableName = DOCMANAGER_PRIVILEGES
DEFINE min_priv = SEARCH_DOCS

-- The names for the tablespaces are normally derived from the schema names but may be modified if necessary
DEFINE tableSpaceName = T_&&schemaName
DEFINE indexTableSpaceName = T_&&schemaName._INDEX
DEFINE lobsTableSpaceName = T_&&schemaName._LOBS
DEFINE ctxTableSpaceName = T_&&schemaName._CTX
DEFINE docTableSpaceName = T_&&schemaName._DOCS
DEFINE cscartTableSpaceName = T_&&schemaName._CSCART
DEFINE tempTableSpaceName = T_&&securitySchemaName._TEMP
DEFINE securityTableSpaceName = T_&&securitySchemaName
DEFINE auditTableSpaceName = T_&&schemaName._AUDIT

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
DEFINE defaultctxTSPath= ''
DEFINE defaultdocTSPath= ''
DEFINE defaultcscartTSPath= ''
DEFINE defaultauditTSPath= ''

DEFINE tableSpaceFile = '&&defaultTableTSPath&&tableSpaceName..DBF' 
DEFINE indexTableSpaceFile = '&&defaultIndexTSPath&&indexTableSpaceName..DBF'
DEFINE lobsTableSpaceFile = '&&defaultLobTSPath&&lobsTableSpaceName..DBF'
DEFINE ctxTableSpaceFile = '&&defaultctxTSPath&&ctxTableSpaceName..DBF'
DEFINE docTableSpaceFile = '&&defaultdocTSPath&&docTableSpaceName..DBF'
DEFINE cscartTableSpaceFile = '&&defaultcscartTSPath&&cscartTableSpaceName..DBF'
DEFINE auditTableSpaceFile = '&&defaultauditTSPath&&auditTableSpaceName..DBF'

DEFINE dataBlockSize = 8K 
DEFINE indexBlockSize = 8K
DEFINE lobBlockSize  = 8K 
DEFINE ctxBlockSize  = 8K
DEFINE docBlockSize  = 8K
DEFINE cscartBlockSize  = 8K
DEFINE auditBlockSize  = 8K

-- The default sizes are very small, suitable only for demo purposes.
-- Normally, these sizes should be changed to a more realistic initial size.
-- For the Table and IDX tablespaces, approximately 10M/user/year should be allocated,
-- LOB and CTX are very dependant on the number and types of documents and spectra
-- which will be stored, but 20M/user/year is a reasonable starting point.

DEFINE tablespaceSize    = 4M
DEFINE idxTablespaceSize   = 4M
DEFINE lobTablespaceSize = 4M
DEFINE ctxTablespaceSize = 4M
DEFINE docTablespaceSize = 4M
DEFINE cscartTablespaceSize = 120M
DEFINE auditTablespaceSize = 4M

DEFINE tablespaceExtent   =  'AUTO'
DEFINE idxTablespaceExtent  =  'AUTO'
DEFINE lobTablespaceExtent  =  'AUTO'
DEFINE ctxTablespaceExtent  =  'AUTO'
DEFINE docTablespaceExtent  =  'AUTO'
DEFINE cscartTablespaceExtent  =  'AUTO'
DEFINE auditTablespaceExtent  =  'AUTO'

-- The storage size for base64cdx data in chemical structures table
DEFINE lobB64cdx = 2K
DEFINE lobCtx = 2K
DEFINE lobDoc = 2K


