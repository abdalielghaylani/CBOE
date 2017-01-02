ECHO OFF
TITLE Drop tables and Users in coedb Schema
CLS
ECHO You are about to destroy any previous contents of 
ECHO coedb database tables
ECHO To abort close this window or press Ctrl-C.
Pause
cd %1
ECHO ************************************  
ECHO Droping coedb Schema
ECHO ************************************     
   
sqlplus /NOLOG  @sql\drop_coedb_ora.sql
notepad sql\log_drop_coedb_ora.txt



