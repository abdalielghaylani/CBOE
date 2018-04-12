ECHO OFF
TITLE Drop ChemACX Database
CLS
ECHO You are about to destroy any previous contents of the 
ECHO ChemACX database.  
ECHO To abort close this window or press Ctrl-C.
Pause

sqlplus /NOLOG @sql\Drop_ChemACXDB.sql
Notepad sql\log_drop_chemacxdb.txt