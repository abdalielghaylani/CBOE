--Copyright 1999-2006 CambridgeSoft Corporation. All rights reserved
set echo off

@parameters.sql

SET ECHO OFF
SET verify off

--#########################################################
-- PROMPT THE USER FOR SCRIPT SUBSTITUTION VARIABLES
--######################################################### 

ACCEPT serverName CHAR DEFAULT '' PROMPT 'Enter the target Oracle service name:'
ACCEPT InstallUser CHAR DEFAULT 'system' PROMPT 'Enter the name of an Oracle account with system privleges(system):'
ACCEPT sysPass CHAR DEFAULT 'manager2' PROMPT 'Enter the above oracle account password (manager2):' hide


set feedback on
spool Logs/LOG_AddRLSPolicy.txt

Prompt '--#########################################################'
Prompt '-- APPLYING THE RLS POLICY' 
Prompt '--######################################################### '

@@Update_Scripts\RegistrationIntegration\Add_Reg_RLS_Policy.sql;

spool off
exit
