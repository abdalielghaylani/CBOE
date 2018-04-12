cd %1
ECHO OFF
TITLE Export BioSARDB Data
CLS
ECHO ************************************************************
ECHO You will be prompted for server and user information.  
ECHO ************************************************************
ECHO To abort close this window or press Ctrl-C.
Pause

cd sql\
sqlplus /NOLOG @ExportBioSAR.sql
notepad LOG_ExportBioSAR.txt
