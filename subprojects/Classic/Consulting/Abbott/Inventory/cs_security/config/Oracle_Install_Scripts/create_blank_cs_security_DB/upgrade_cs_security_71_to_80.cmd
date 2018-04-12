cd %1
ECHO OFF
TITLE Upgrage CS_Security Database 7.1 to 8.0
CLS
ECHO You are about to upgrade the CS Security database.  
ECHO Please ensure your current cs_security schema corresponds to version 7.1
ECHO To abort close this window or press Ctrl-C.
Pause

sqlplus /NOLOG @sql\header_cs_security_71_to_80.sql
notepad sql\log_upgrade_cs_security_71_to_80.txt
