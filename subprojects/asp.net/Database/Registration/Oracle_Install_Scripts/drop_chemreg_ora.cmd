ECHO OFF
TITLE Drop tables and Users in regdb Schema
CLS
ECHO You are about to destroy any previous contents of 
ECHO registration database tables
ECHO To abort close this window or press Ctrl-C.
Pause
cd %1
ECHO ************************************  
ECHO Droping regdb Schema
ECHO ************************************     
   
sqlplus /NOLOG  @sql\drop_chemreg_ora.sql
sql\log_drop_chemreg_ora.txt



