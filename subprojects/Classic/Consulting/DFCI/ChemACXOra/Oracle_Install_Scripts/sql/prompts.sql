--Copyright 1999-2003 CambridgeSoft Corporation. All rights reserved

SET ECHO OFF
SET verify off

--#########################################################
-- PROMPT THE USER FOR SCRIPT SUBSTITUTION VARIABLES
--######################################################### 

ACCEPT serverName CHAR DEFAULT '' PROMPT 'Enter the target Oracle service name:'
ACCEPT OraVersionNumber CHAR DEFAULT '9' PROMPT 'Enter the Oracle major version number (9):'
ACCEPT InstallUser CHAR DEFAULT 'system' PROMPT 'Enter the name of an Oracle account with system privileges (system):'
ACCEPT sysPass CHAR DEFAULT 'manager2' PROMPT 'Enter the above oracle account password (manager2):'
ACCEPT importdata CHAR DEFAULT 'Y' PROMPT 'Would you like to import the ChemACX data? (Y):'
ACCEPT alter_qre CHAR DEFAULT 'Y' PROMPT 'Would you like to alter QUERY_REWRITE_ENABLED ? (Y):'
ACCEPT alter_sec CHAR DEFAULT 'Y' PROMPT 'Do you need to update CS_SECURITY schema ? (Y):'

PROMPT 
PROMPT ==============================================================================
PROMPT = Please wait for couple minutes for next prompt to enter DUMP file location =
PROMPT = After that script can be left running for several hours unattended         =
PROMPT ==============================================================================