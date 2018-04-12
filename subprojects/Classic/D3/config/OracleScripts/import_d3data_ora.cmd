ECHO OFF
TITLE Import D3Data tables and D3Data Schema
CLS
ECHO You are about to destroy any previous contents of 
ECHO d3data tables
ECHO To abort close this window or press Ctrl-C.
Pause

ECHO ************************************  
ECHO Importing D3Data Tables
ECHO ************************************    

CD sql 
   
sqlplus /NOLOG  @import_d3data.sql
log_import_d3data.txt



