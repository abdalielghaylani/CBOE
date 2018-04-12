--1999-2003 CambridgeSoft Corporation. All rights reserved


--#########################################################
-- SCRIPT VARIABLES
--######################################################### 
DEFINE schemaName = CHEMACXDB
DEFINE schemaPass = ORACLE
DEFINE schemaVersion = 9.2
DEFINE appVersion = 9.0.4
DEFINE securitySchemaName = CS_SECURITY
DEFINE securitySchemaPass = ORACLE

-- The names for the tablespaces are normally derived from the schema names but may be modified if necessary
DEFINE tableSpaceName = T_&&schemaName
DEFINE indexTableSpaceName = T_&&schemaName._INDEX
DEFINE lobsTableSpaceName = T_&&schemaName._LOBS
DEFINE cscartTableSpaceName = T_&&schemaName._CSCART
DEFINE msdxLobsTableSpaceName = T_&&schemaName._MSDXLOBS
DEFINE tempTableSpaceName = T_&&securitySchemaName._TEMP
DEFINE securityTableSpaceName = T_&&securitySchemaName

-- Null for non cs_security applications
DEFINE privTableName = CHEMACX_PRIVILEGES
DEFINE min_priv = BROWSE_ACX

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
DEFINE defaultcscartTSPath= ''
DEFINE defaultMsdxLobTSPath= ''

DEFINE tableSpaceFile = '&&defaultTableTSPath&&tableSpaceName..DBF' 
DEFINE indexTableSpaceFile = '&&defaultIndexTSPath&&indexTableSpaceName..DBF'
DEFINE lobsTableSpaceFile = '&&defaultLobTSPath&&lobsTableSpaceName..DBF'
DEFINE msdxLobsTableSpaceFile = '&&defaultMsdxLobTSPath&&msdxLobsTableSpaceName..DBF'
DEFINE cscartTableSpaceFile = '&&defaultcscartTSPath&&cscartTableSpaceName..DBF'


DEFINE dataBlockSize = 8K 
DEFINE indexBlockSize = 16K
DEFINE lobBlockSize  = 2K 
DEFINE msdxLobBlockSize  = 2K
DEFINE cscartBlockSize  = 8K


-- The default sizes are very small, suitable only for demo purposes.
-- Normally, these sizes should be changed to a more realistic initial size.
-- For the Table and IDX tablespaces, approximately 10M/user/year should be allocated,
-- LOB and CTX are very dependant on the number and types of documents and spectra
-- which will be stored, but 20M/user/year is a reasonable starting point.

DEFINE tablespaceSize    = 300M
DEFINE idxTablespaceSize   = 300M
DEFINE lobTablespaceSize = 1000M
DEFINE msdxLobTablespaceSize = 1000M
DEFINE cscartTablespaceSize = 600M

DEFINE tablespaceExtent   =  4M
DEFINE idxTablespaceExtent  =  128K
DEFINE lobTablespaceExtent  =  4M
DEFINE msdxLobTablespaceExtent  =  4M
DEFINE cscartTablespaceExtent  =  128K

-- The storage size for base64cdx data in chemical structures table
DEFINE lobB64cdx = 4M
DEFINE msdxLob = 4M

