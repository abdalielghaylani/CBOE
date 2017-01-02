cd %1
ECHO OFF
TITLE Upgrade COEDB 9SR4 To 11.0.0 
CLS
ECHO You are about to upgrade the COEDB Schema.
ECHO To abort close this window or press Ctrl-C.
Pause

ECHO. 
ECHO *******************************************************************************
ECHO   Patching COEDB
ECHO *******************************************************************************

cd sql   
sqlplus /NOLOG  @Upgrade_COEDB_9SR4_To_11.0.0.sql
notepad Upgrade_COEDB_9SR4_To_11.0.0.txt


