ECHO OFF
TITLE Column Chooser Fix
CLS
ECHO To abort close this window or press Ctrl-C.
Pause
CD sql
sqlplus /NOLOG @ColChooserFix.sql
