cd %1
ECHO OFF
TITLE Apply Chemical Registration Patches 
CLS
ECHO You are about to upgrade the REGDB Schema.
ECHO To abort close this window or press Ctrl-C.
Pause

ECHO. 
ECHO *******************************************************************************
ECHO   Patching Registration
ECHO *******************************************************************************

   
sqlplus /NOLOG  @sql\Patches\HEADERFILE_PATCH_chemreg_ora.sql
notepad sql\Patches\log_Patches_chemreg_ora.txt


