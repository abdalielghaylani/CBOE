--Copyright 1999-2004 CambridgeSoft Corporation. All rights reserved

SET ECHO OFF
SET verify off

--#########################################################
-- PROMPT THE USER FOR SCRIPT SUBSTITUTION VARIABLES
--######################################################### 

ACCEPT serverName CHAR DEFAULT '' PROMPT 'Enter the target Oracle service name:'
--ACCEPT OraVersionNumber CHAR DEFAULT '9' PROMPT 'Enter the Oracle major version number (9):'
DEFINE OraVersionNumber= '9'
ACCEPT InstallUser CHAR DEFAULT 'system' PROMPT 'Enter the name of an Oracle account with system privileges (system):'
ACCEPT sysPass CHAR DEFAULT 'manager2' PROMPT 'Enter the above oracle account password (manager2):' HIDE
ACCEPT dmpFile CHAR DEFAULT 'd3data.dmp' PROMPT 'Enter the path to the D3 Data dmp file (d3data.dmp):'

