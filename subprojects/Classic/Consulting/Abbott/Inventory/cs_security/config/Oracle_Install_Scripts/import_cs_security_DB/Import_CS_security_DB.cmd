ECHO OFF
TITLE Import test data into ChemInv
CLS
ECHO You are about to import the contents of the CS_SECURITY dump file into 
ECHO your CS_SECURITY database.  
ECHO To abort close this window or press Ctrl-C.
Pause
CD sql
imp.exe parfile= CS_SecurityDB.inp
notepad Import_CS_SecurityDB.log