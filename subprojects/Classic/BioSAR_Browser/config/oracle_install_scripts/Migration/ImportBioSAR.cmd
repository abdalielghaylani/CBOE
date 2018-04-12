cd %1
ECHO OFF
TITLE Import BioSARDB Data from dump file
CLS
ECHO You are about to destroy any previous contents of the 
ECHO BioSAR database.  
ECHO ************************************************************
ECHO You will be prompted for server and user information.  
ECHO ************************************************************
ECHO To abort close this window or press Ctrl-C.
Pause

cd sql\
sqlplus /NOLOG @ImportBioSARDump.sql
notepad LOG_ImportBioSARDump.txt
