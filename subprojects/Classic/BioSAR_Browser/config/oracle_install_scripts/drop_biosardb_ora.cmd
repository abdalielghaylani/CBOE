ECHO OFF
TITLE Drop tables and Users in biosardb Schema
CLS
ECHO You are about to destroy any previous contents of 
ECHO biosar database tables
ECHO To abort close this window or press Ctrl-C.
Pause
cd %1
ECHO ************************************  
ECHO Droping biosardb Schema
ECHO ************************************     
   
sqlplus /NOLOG  @sql\drop_biosardb_ora.sql
sql\log_drop_biosardb_ora.txt



