ECHO OFF
TITLE Import test data into ChemInv
CLS
ECHO You are about to import the contents of the test data into 
ECHO your Chemical Inventory database.  
ECHO To abort close this window or press Ctrl-C.
Pause
CD sql
imp.exe parfile= Test_ChemInvDB.inp
sqlplus /NOLOG @indexingworkaround.sql
notepad Import_Test_CheminvDB.log
   
