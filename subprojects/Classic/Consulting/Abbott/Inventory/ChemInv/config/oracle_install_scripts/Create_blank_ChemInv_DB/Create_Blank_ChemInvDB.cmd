cd %1
ECHO OFF
TITLE Create Blank Inventory Enterprise Database
CLS
ECHO You are about to create a blank Inventory Enterprise database.  
ECHO To abort close this window or press Ctrl-C.
Pause
cd sql
sqlplus /NOLOG @HEADERFILE_Create_Blank_ChemInvDB.sql
notepad Logs\LOG_Create_blank_CheminvDB.txt