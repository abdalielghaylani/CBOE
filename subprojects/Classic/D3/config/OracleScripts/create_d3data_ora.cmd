ECHO OFF
TITLE Create D3Data tables and Users in D3Data Schema and Import D3 Data
CLS
ECHO You are about to destroy any previous contents of 
ECHO d3data tables
ECHO To abort close this window or press Ctrl-C.
Pause

ECHO ************************************  
ECHO Creating D3Data Tables
ECHO ************************************    

CD sql 
   
sqlplus /NOLOG  @create_d3data.sql
log_create_d3data.txt



