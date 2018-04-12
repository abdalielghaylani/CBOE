ECHO OFF
TITLE Create Document Manager tables and Users
CLS
ECHO You are about to create the DOCMGR Schema.
ECHO Any existing DOCMANAGER schema will be destroyed and all data lost.
ECHO To abort close this window or press Ctrl-C.
Pause
cd %1
ECHO ************************************  
ECHO Creating DOCMANAGER Schema and Tables, Adding DocManager privileges and roles, Creating DocManager test users
ECHO ************************************     
cd scripts   
sqlplus /NOLOG  @HEADERFILE_CREATE_docmanager.sql
notepad log_Create_DocManager.txt

