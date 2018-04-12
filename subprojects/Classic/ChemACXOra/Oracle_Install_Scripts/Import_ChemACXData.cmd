ECHO OFF
TITLE Import ChemACX data into blank database
CLS
ECHO You are about to import the contents of the test data into 
ECHO your Chemical Inventory database.  
ECHO To abort close this window or press Ctrl-C.
Pause
CD sql
imp.exe parfile= ChemACX.imp
notepad Import_ChemACX.log