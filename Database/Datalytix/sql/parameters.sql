-- Copyright 1998-2014 CambridgeSoft Corporation, an indirect, wholly-owned subsidiary of PerkinElmer, Inc. All rights reserved.


--#########################################################
-- SCRIPT VARIABLES
--######################################################### 
DEFINE schemaName = COEDB
DEFINE schemaPass = ORACLE
DEFINE schemaVersion ='' 
DEFINE appVersion = ''
DEFINE securitySchemaName = COEDB
DEFINE securitySchemaPass = ORACLE
DEFINE globalSchemaName = COEUSER
DEFINE globalSchemaPass = ORACLE

-- The names for the tablespaces are normally derived from the schema names but may be modified if necessary
DEFINE tableSpaceName = T_&&schemaName
DEFINE indexTableSpaceName = T_&&schemaName._INDEX
DEFINE lobsTableSpaceName = T_&&schemaName._LOBS
DEFINE cscartTableSpaceName = T_&&schemaName._CSCART
DEFINE msdxLobsTableSpaceName = T_&&schemaName._MSDXLOBS
DEFINE tempTableSpaceName = T_&&securitySchemaName._TEMP
DEFINE securityTableSpaceName = T_&&securitySchemaName

-- Null for non cs_security applications
DEFINE privTableName = DATALYTIX_PRIVILEGES
DEFINE min_priv = DL_MANAGE_ALL_QUERIES
DEFINE UserRole = DL_USER
DEFINE AdminRole = DL_ADMIN
DEFINE AppName = 'Datalytix'
DEFINE COEIdentifier = 'Datalytix'
DEFINE AdminTestUser = DLADMIN
DEFINE UserTestUser = DLUSER


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

DEFINE tablespaceSize    = 4M
DEFINE idxTablespaceSize   = 4M
DEFINE lobTablespaceSize = 4M
DEFINE msdxLobTablespaceSize = 4M
DEFINE cscartTablespaceSize = 4M

DEFINE tablespaceExtent   =  4M
DEFINE idxTablespaceExtent  =  128K
DEFINE lobTablespaceExtent  =  4M
DEFINE msdxLobTablespaceExtent  =  4M
DEFINE cscartTablespaceExtent  =  128K

-- The storage size for base64cdx data in chemical structures table
DEFINE lobB64cdx = 4M
DEFINE msdxLob = 4M

