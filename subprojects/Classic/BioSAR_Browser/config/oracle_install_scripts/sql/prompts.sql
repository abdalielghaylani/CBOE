--Copyright 1999-2003 CambridgeSoft Corporation. All rights reserved

SET ECHO OFF
SET verify off

--#########################################################
-- PROMPT THE USER FOR SCRIPT SUBSTITUTION VARIABLES
--######################################################### 

ACCEPT serverName CHAR DEFAULT '' PROMPT 'Enter the target Oracle service name:'
--We no longer support Oracle 8 so we can default to use 9 as major version
-- 9 and 10 are compatible so 10 is covered by 9
ACCEPT InstallUser CHAR DEFAULT 'system' PROMPT 'Enter the name of an Oracle account with system privileges (system):'
DEFINE OraVersionNumber= '9'
--ACCEPT OraVersionNumber CHAR DEFAULT '9' PROMPT 'Enter the Oracle major version number (9):'ACCEPT InstallUser CHAR DEFAULT 'system' PROMPT 'Enter the name of an Oracle account with system privileges (system):'
ACCEPT sysPass CHAR DEFAULT 'manager2' PROMPT 'Enter the above oracle account password (manager2):' HIDE

