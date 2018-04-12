cd %1
ECHO OFF
TITLE Apply CHEMINVDB2 Patches 
CLS
ECHO You are about to upgrade the CHEMINVDB2 Schema.
ECHO To abort close this window or press Ctrl-C.
Pause

ECHO. 
ECHO *******************************************************************************
ECHO   Patching CHEMINVDB2 Database
ECHO *******************************************************************************

cd sql 
   
sqlplus /NOLOG  @Patches\HEADERFILE_PATCH_CHEMINVDB2.sql
notepad Patches\log_Patches_CHEMINVDB2.txt


