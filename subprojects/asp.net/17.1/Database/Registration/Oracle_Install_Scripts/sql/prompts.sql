--Copyright 1999-2003 CambridgeSoft Corporation. All rights reserved

SET ECHO OFF
SET VERIFY OFF

--#########################################################
-- PROMPT THE USER FOR SCRIPT SUBSTITUTION VARIABLES
--######################################################### 

ACCEPT serverName CHAR PROMPT 'Enter the target Oracle service name:'
DEFINE OraVersionNumber = 9
ACCEPT InstallUser CHAR DEFAULT 'system' PROMPT 'Enter the name of an Oracle account with system privileges (system):'
ACCEPT sysPass CHAR DEFAULT 'manager2' PROMPT 'Enter the above oracle account password (manager2):' hide

--RLS is activated from the application at least for now
DEFINE ActivateRLS  = 'N'
--ACCEPT ActivateRLS CHAR DEFAULT 'N' PROMPT 'Do you want to activate Row-Level security? (N):'

@@&&InstallUser
