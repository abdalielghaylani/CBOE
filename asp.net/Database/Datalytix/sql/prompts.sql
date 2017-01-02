-- Copyright 1998-2014 CambridgeSoft Corporation, an indirect, wholly-owned subsidiary of PerkinElmer, Inc. All rights reserved.

SET ECHO OFF
SET verify off

--#########################################################
-- PROMPT THE USER FOR SCRIPT SUBSTITUTION VARIABLES
--######################################################### 

ACCEPT serverName CHAR DEFAULT '' PROMPT 'Enter the target Oracle service name:'

DEFINE OraVersionNumber= '9'
ACCEPT InstallUser CHAR DEFAULT 'system' PROMPT 'Enter the name of an Oracle account with system privileges (system):'
ACCEPT sysPass CHAR DEFAULT 'manager2' PROMPT 'Enter the above Oracle account password (manager2):' HIDE

