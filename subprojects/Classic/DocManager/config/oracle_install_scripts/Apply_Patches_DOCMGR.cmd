cd %1
ECHO OFF
TITLE Apply DOCMGR Patches 
CLS
ECHO You are about to upgrade the DOCMGR Schema.
ECHO To abort close this window or press Ctrl-C.
Pause

ECHO. 
ECHO *******************************************************************************
ECHO   Patching DOCMGR Database
ECHO *******************************************************************************

cd scripts   
sqlplus /NOLOG  @Patches\HEADERFILE_PATCH_DOCMGR.sql
notepad Patches\log_Patches_DOCMGR.txt


