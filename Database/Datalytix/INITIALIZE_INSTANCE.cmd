ECHO OFF
TITLE Create CoeDB tables and Users
CLS
ECHO You are about to create a Global Schema(COEDB by default).
ECHO To abort close this window or press Ctrl-C.
Pause
cd %1
ECHO ************************************  
ECHO Creating Global Schema and Tables
ECHO ************************************     
   
sqlplus /NOLOG  @sql\Instance_HEADERFILE_CREATE_GLOBAL_SCHEMA_ora.sql
notepad sql\log_create_globalschema_ora.txt