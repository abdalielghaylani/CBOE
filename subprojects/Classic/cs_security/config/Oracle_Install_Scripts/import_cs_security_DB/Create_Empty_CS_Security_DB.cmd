ECHO OFF
TITLE Create ChemInv test database
CLS
ECHO You are about to destroy any previous contents of the 
ECHO CS_SECURITY database.  
ECHO To abort close this window or press Ctrl-C.
Pause
CD sql
sqlplus /NOLOG @Create_Empty_CS_SecurityDB.sql
