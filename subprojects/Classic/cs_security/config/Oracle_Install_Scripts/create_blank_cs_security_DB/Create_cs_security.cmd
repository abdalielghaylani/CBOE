cd %1
ECHO OFF
TITLE Create CS_Security Database
CLS
ECHO You are about to create the CS Security database.  
ECHO Warning: This process will delete any preexisting CS_Security users or roles.
ECHO To abort close this window or press Ctrl-C.
Pause

sqlplus /NOLOG @sql\create_cs_security.sql
notepad sql\log_create_cs_security.txt