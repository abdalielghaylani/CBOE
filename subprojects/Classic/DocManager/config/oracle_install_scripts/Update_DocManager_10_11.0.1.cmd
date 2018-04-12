ECHO OFF
TITLE Update Document Manager tables and Users
CLS
ECHO You are about to update the DOCMGR Schema.
ECHO To abort close this window or press Ctrl-C.
Pause
cd %1
ECHO ************************************  
ECHO Updating DOCMANAGER Schema and Tables to version 11.0.1
ECHO ************************************     
   
sqlplus /NOLOG  @scripts\HEADERFILE_Update_docmanager_to_1101.sql
notepad scripts\log_Update_DocManager_to_1101.txt

