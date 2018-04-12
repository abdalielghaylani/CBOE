--Copyright 1999-2010 CambridgeSoft Corporation. All rights reserved

SET ECHO OFF
SET VERIFY OFF

--#########################################################
-- PROMPT THE USER FOR SCRIPT SUBSTITUTION VARIABLES
--######################################################### 


ACCEPT serverName CHAR PROMPT 'Enter the target Oracle service name:'
ACCEPT InstallUser CHAR DEFAULT 'system' PROMPT 'Enter the name of an Oracle account with system privileges (system):'
ACCEPT sysPass CHAR DEFAULT 'manager2' PROMPT 'Enter the above oracle account password (manager2):' hide
ACCEPT schemaName CHAR DEFAULT '&schemaName' PROMPT 'Enter the name of target schema owner (&schemaName):'
ACCEPT schemaPass CHAR DEFAULT '&schemaPass' PROMPT 'Enter the above oracle account password (&schemaPass):' hide










