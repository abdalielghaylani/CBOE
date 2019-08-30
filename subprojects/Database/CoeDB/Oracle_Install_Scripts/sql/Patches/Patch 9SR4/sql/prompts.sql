--Copyright 1999-2003 CambridgeSoft Corporation. All rights reserved

SET ECHO OFF
SET VERIFY OFF

--#########################################################
-- PROMPT THE USER FOR SCRIPT SUBSTITUTION VARIABLES
--######################################################### 

ACCEPT serverName CHAR DEFAULT '' PROMPT 'Enter the target Oracle service name:'
ACCEPT InstallUser CHAR DEFAULT 'system' PROMPT 'Enter the name of an Oracle account with system privileges (system):'
ACCEPT sysPass CHAR DEFAULT 'manager2' PROMPT 'Enter the above oracle account password (manager2):' hide
--ACCEPT UpgradeCsSecurity CHAR DEFAULT 'N' PROMPT 'Do you want to upgrade existing CS_SECURITY information? (N):'


@&&InstallUser





