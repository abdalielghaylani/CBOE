ECHO OFF
TITLE Create CoeDB tables and Users
CLS
ECHO You are about to create the COEDB Schema.
ECHO Any existing COEDB schema will be destroyed and all data lost.
ECHO To abort close this window or press Ctrl-C.
Pause
cd %1
ECHO ************************************  
ECHO Creating COEDB Schema and Tables, Adding COEUser
ECHO ************************************     
   
sqlplus /NOLOG  @sql\HeaderFile_Create_CoeDB_ora.sql
notepad sql\log_create_coedb_ora.txt
