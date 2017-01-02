ECHO OFF
TITLE Drop tables and Users in Cs-Security Schema
CLS
ECHO You are about to destroy any previous contents of 
ECHO Cs-Security database tables
ECHO To abort close this window or press Ctrl-C.
Pause
cd %1
ECHO ************************************  
ECHO Droping Cs-Security Schema
ECHO ************************************     
   
sqlplus /NOLOG  @"sql\Patches\Patch 11.0.1.0\sql\CsSecurity\drop_CsSecurity_ora.sql"
notepad sql\Patches\Patch 11.0.1.0\sql\CsSecurity\log_drop_CsSecurity_ora.txt



