cd %1
ECHO OFF
TITLE Update ChemACX Database
CLS
ECHO You are about to upgrade the ChemACX database to version 9.2.  
ECHO To abort close this window or press Ctrl-C.
Pause
cd sql
sqlplus /NOLOG @updateChemACX_8_to_92.sql
log_update_chemacxdb.txt
