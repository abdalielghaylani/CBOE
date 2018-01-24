cd %1
ECHO OFF
TITLE Apply CoeDB Patches 
CLS
ECHO You are about to upgrade the CoeDB Schema.
ECHO To abort close this window or press Ctrl-C.
Pause

ECHO. 
ECHO *******************************************************************************
ECHO   Patching CoeDB schema
ECHO *******************************************************************************

   
sqlplus /NOLOG  @sql\Patches\HEADERFILE_PATCH_CoeDB_ora.sql
notepad sql\Patches\log_Patches_CoeDB_ora.txt


