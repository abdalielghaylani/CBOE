-- Copyright 1998-2014 CambridgeSoft Corporation, an indirect, wholly-owned subsidiary of PerkinElmer, Inc. All rights reserved.

prompt 
prompt Starting "doEnableGlobalUserForPrimaryDatasource.sql"...
prompt

SET ECHO OFF
SET verify off

--#########################################################
-- SCRIPT VARIABLES
--#########################################################

CONNECT &&InstallUser/&&sysPass@&&serverName &&AsSysDBA
SET serveroutput on

@@Instance_grants.sql
@@Instance_CREATE_GLOBAL_SCHEMA_ora.sql
@@Instance_CreateFunction.sql
@@Instance_CreateJob.sql
@@Instance_enableProxyForInstanceGlobalUser.sql
