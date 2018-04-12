cd %1
ECHO OFF
TITLE Create Blank ChemACX Database
CLS
ECHO You are about to create a blank ChemACX database.  
ECHO To abort close this window or press Ctrl-C.
Pause

sqlplus /NOLOG @sql\create_chemacx.sql
sql\log_create_chemacx.txt