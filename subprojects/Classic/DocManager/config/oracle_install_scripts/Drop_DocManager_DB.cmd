ECHO OFF
TITLE Drop Document Manager tables and Users
CLS
ECHO You are about to drop the DOCMGR Schema.
ECHO Any existing DOCMANAGER schema will be destroyed and all data lost.
ECHO To abort close this window or press Ctrl-C.
Pause
cd %1
ECHO ************************************  
ECHO Droping DOCMANAGER Schema and Tables
ECHO ************************************     
   
sqlplus /NOLOG  @scripts\drop_docmanager.sql
notepad scripts\log_drop_DocManager.txt

