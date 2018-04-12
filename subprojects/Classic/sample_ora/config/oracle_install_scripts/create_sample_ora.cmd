ECHO OFF
TITLE Create SAMPLE_ORA tables and Users in Sample Schema and Import Sample Data
CLS
ECHO You are about to destroy any previous contents of 
ECHO Sample tables
ECHO To abort close this window or press Ctrl-C.
Pause

ECHO ************************************  
ECHO Creating SAMPLE Tables
ECHO ************************************    

CD sql 
   
sqlplus /NOLOG  @create_sample_ora.sql
log_create_sample_ora.txt



