cd sql
ECHO OFF
TITLE Apply BioSAR Patches 
CLS
ECHO You are about to upgrade the BioSAR Schema.
ECHO To abort close this window or press Ctrl-C.
Pause

ECHO. 
ECHO *******************************************************************************
ECHO   Patching BioSAR Database
ECHO *******************************************************************************

   
sqlplus /NOLOG  @Patches\HEADERFILE_PATCH_BIOSARDB.sql
notepad Patches\log_Patches_BioSARDB.txt


