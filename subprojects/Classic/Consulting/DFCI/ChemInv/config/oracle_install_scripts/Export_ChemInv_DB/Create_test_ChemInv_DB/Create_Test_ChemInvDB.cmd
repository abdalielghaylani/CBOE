cd %1
ECHO OFF
TITLE Create Test Inventory Enterprise DB
CLS
ECHO You are about to destroy any previous contents of the 
ECHO Inventory Enterprise database.  
ECHO To abort close this window or press Ctrl-C.
Pause
cd ..\create_blank_cheminv_db\sql\
sqlplus /NOLOG @HEADERFILE_Create_Test_ChemInvDB.sql
notepad Logs\LOG_Create_Test_ChemInvDB.txt
