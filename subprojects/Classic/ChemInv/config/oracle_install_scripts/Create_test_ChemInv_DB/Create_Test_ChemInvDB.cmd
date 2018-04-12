cd %1
ECHO OFF
TITLE Create Inventory Enterprise Test DB
CLS
ECHO You are about to destroy any previous contents of the 
ECHO Inventory Enterprise database.  
ECHO ************************************************************
ECHO You will be prompted for server and user information.  
ECHO ************************************************************
ECHO To abort close this window or press Ctrl-C.
Pause

cd ..\create_blank_cheminv_db\sql\
sqlplus /NOLOG @HEADERFILE_Create_test_ChemInvDB.sql
notepad Logs\LOG_Create_Test_ChemInvDB.txt
