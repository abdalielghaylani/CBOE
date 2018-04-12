cd %1
ECHO OFF
TITLE Apply RegIntegration Patch 
CLS
ECHO You are about to upgrade the RegIntegration in Inventory Schema.
ECHO To abort close this window or press Ctrl-C.
Pause

ECHO. 
ECHO *******************************************************************************
ECHO   Patching RegIntegration 
ECHO *******************************************************************************

cd sql 
   
sqlplus /NOLOG  @Patches\HEADERFILE_PATCH_REGINTEGRATION_CHEMINVDB2.sql
notepad Patches\log_Patch_REGINTEGRATION.txt


