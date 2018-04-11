--Copyright 1999-2003 CambridgeSoft Corporation. All rights reserved

SET ECHO off
SET verify off

--#########################################################
-- PROMPT THE USER FOR SCRIPT SUBSTITUTION VARIABLES
--######################################################### 

ACCEPT serverName CHAR DEFAULT '' PROMPT 'Enter the target Oracle service name:'
ACCEPT InstallUser CHAR DEFAULT 'system' PROMPT 'Enter the name of an Oracle account with system privileges (system):'
ACCEPT sysPass char DEFAULT 'manager2' PROMPT 'Enter the above oracle account password (manager2):' hide
ACCEPT regCompoundsSdfName CHAR DEFAULT 'regcompounds.sdf' PROMPT 'Enter name of the legacy compound data sd-file  (regcompounds.sdf):'
ACCEPT regPrefix CHAR DEFAULT 'IWD' PROMPT 'Enter prefix for your legacy registry numbers (IWD):'
ACCEPT regPrefixDelimeter CHAR DEFAULT '-' PROMPT 'Enter prefix delimiter for your legacy registry numbers (-):'
ACCEPT regBatchDelimeter CHAR DEFAULT '-' PROMPT 'Enter batch delimiter for your legacy registry numbers (-):'