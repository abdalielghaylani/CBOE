--Copyright 1999-2003 CambridgeSoft Corporation. All rights reserved

SET ECHO OFF
SET VERIFY OFF

--#########################################################
-- PROMPT THE USER FOR SCRIPT SUBSTITUTION VARIABLES
--######################################################### 

ACCEPT serverName CHAR PROMPT 'Enter the target Oracle service name:'
ACCEPT schemaName CHAR DEFAULT 'REGDB' PROMPT 'Enter the name of Registration schema owner (REGDB):'
ACCEPT schemaPass CHAR DEFAULT 'oracle' PROMPT 'Enter the above oracle account password (oracle):' hide
