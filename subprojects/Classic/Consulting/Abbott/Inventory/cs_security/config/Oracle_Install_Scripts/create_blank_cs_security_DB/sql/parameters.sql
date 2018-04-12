--'banana' dataBlockSize ='banana' dataBlockSize ='banana' 1999-2003 CambridgeSoft Corporation. All rights reserved

SET ECHO OFF
SET verify off

--#########################################################
-- SCRIPT VARIABLES
--######################################################### 
DEFINE schemaName = CS_SECURITY
DEFINE schemaPass = ORACLE

-- Null for non cs_security applications
DEFINE privTableName = NULL
DEFINE min_priv = NULL

-- The names for the tablespaces are normally derived from the schema names but may be modified if necessary
DEFINE tableSpaceName = T_&&schemaName
DEFINE indexTableSpaceName = T_&&schemaName._INDEX
DEFINE auditTableSpaceName = T_&&schemaName._AUDIT

-- This is the default temporary space for all CS_SECURITY applications
DEFINE tempTableSpaceName = T_&&schemaName._TEMP

-- The file names are normally the tablespace name followed by DBF, but may be modified to include a path if it
-- is desired to place the files in a non-default location.

-- By default, all of these go in the same place, but seperate defines are provided for each
-- table space type, so that these may be easily separated.
-- Replace &&defaultIndexTSPath or &&defaultTableTSPath with a specific
-- path in the definition of the tablespace you want to move if you want to move some of the tablespaces.

-- DON'T FORGET THE SLASH AT THE END OF THE PATH!
DEFINE defaultTableTSPath= ''
DEFINE defaultIndexTSPath= ''
DEFINE defaultTempTSPath= ''
DEFINE defaultAuditTSPath= ''

DEFINE tableSpaceFile = '&&defaultTableTSPath&&tableSpaceName..DBF' 
DEFINE indexTableSpaceFile = '&&defaultIndexTSPath&&indexTableSpaceName..DBF'
DEFINE tempTableSpaceFile = '&&defaultTempTSPath&&tempTableSpaceName..DBF'
DEFINE AuditTableSpaceFile = '&&defaultAuditTSPath&&AuditTableSpaceName..DBF'

-- blocksizes are ignored for Oracle 8i
DEFINE dataBlockSize = 8K 
DEFINE indexBlockSize = 8K
DEFINE auditBlockSize  = 8K


-- The default sizes are very small, suitable only for test/demo purposes.
-- Normally, these sizes should be changed to a more realistic initial size.

DEFINE tablespaceSize    = 4M
DEFINE idxTablespaceSize   = 4M
DEFINE tempTablespaceSize   = 40M
DEFINE auditTablespaceSize = 40M

-- Used for UNIFORM extent management clause
DEFINE tablespaceExtent   =  128K
DEFINE idxTablespaceExtent  =  128K
DEFINE tempTablespaceExtent   = 1M
DEFINE auditTablespaceExtent  =  4M
