cd %1
ECHO OFF
TITLE Upgrage CS_Security Database 9.0SR2 to 10
CLS
ECHO You are about to upgrade the CS Security database.  
ECHO This upgrade can be applied to versions 9.0 or 9.0 SR1
ECHO To abort close this window or press Ctrl-C.
Pause

sqlplus /NOLOG @sql\header_cs_security_9SR2_to_10.sql
notepad sql\log_upgrade_cs_security_9SR2_to_10.txt
