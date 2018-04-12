-- Copyright Cambridgesoft Corp 2001-2005 all rights reserved

SET ECHO OFF
SET verify off

--#########################################################
-- PROMPT THE USER FOR SCRIPT SUBSTITUTION VARIABLES
--######################################################### 

ACCEPT serverName CHAR DEFAULT '' PROMPT 'Enter the target Oracle service name:'
ACCEPT InstallUser CHAR DEFAULT 'system' PROMPT 'Enter the name of an Oracle account with system privileges (system):'
ACCEPT sysPass CHAR DEFAULT 'manager2' PROMPT 'Enter the above oracle account password (manager2):'
ACCEPT dumpfileName CHAR DEFAULT '..\exported_data\biosardb.dmp' PROMPT 'Enter the path to the dump file (..\exported_data\biosardb.dmp):'

DEFINE oraversionnumber=10