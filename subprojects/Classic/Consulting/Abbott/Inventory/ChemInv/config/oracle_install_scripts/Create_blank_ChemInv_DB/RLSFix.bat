ECHO OFF
TITLE RLS Fix
CLS
ECHO To abort close this window or press Ctrl-C.
Pause
CD sql
sqlplus /NOLOG @RLSFix.sql
