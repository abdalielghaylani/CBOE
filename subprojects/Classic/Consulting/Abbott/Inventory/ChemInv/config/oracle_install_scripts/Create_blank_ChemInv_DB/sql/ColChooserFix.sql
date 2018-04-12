--Copyright 1999-2005 CambridgeSoft Corporation. All rights reserved
set echo off

@@parameters.sql
@@prompts.sql
-- Copyright Cambridgesoft Corp 2001-2005 all rights reserved

SET ECHO OFF
SET verify off


Connect &&schemaName/&&schemaPass@&&serverName

--plsql
@@PACKAGES\pkg_Requests_body.sql

exit
