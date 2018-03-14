--Copyright 1999-2003 CambridgeSoft Corporation. All rights reserved

SET ECHO off
SET verify off

--#########################################################
-- PROMPT THE USER FOR SCRIPT SUBSTITUTION VARIABLES
--######################################################### 

ACCEPT serverName CHAR DEFAULT '' PROMPT 'Enter the target Oracle service name:'
-- OraVersionNumber  was needed for Oracle 8.  Any value above 8 will be ok for now.  No need to ask anylonger
--ACCEPT OraVersionNumber CHAR DEFAULT '9' PROMPT 'Enter the Oracle major version number (9):'
DEFINE OraVersionNumber = 9
ACCEPT InstallUser CHAR DEFAULT 'sys' PROMPT 'Enter the name of an Oracle account with system privileges (sys):'
ACCEPT sysPass CHAR DEFAULT 'manager2' PROMPT 'Enter the above oracle account password (manager2):' hide


@@&&InstallUser

