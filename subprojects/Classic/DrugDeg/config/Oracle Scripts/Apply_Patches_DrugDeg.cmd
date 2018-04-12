cd %1
ECHO OFF
TITLE Apply DrugDeg Patches 
CLS
ECHO You are about to upgrade the DRUGDEG Schema.
ECHO To abort close this window or press Ctrl-C.
Pause

ECHO. 
ECHO *******************************************************************************
ECHO   Patching DRUGDEG Database
ECHO *******************************************************************************

cd sql   
sqlplus /NOLOG  @Patches\HEADERFILE_PATCH_DRUGDEG.sql
notepad Patches\log_Patches_DRUGDEG.txt


