ECHO OFF
TITLE Importing Users From Enotebook to COE
CLS
ECHO You are about to import users into your COE Database
ECHO To abort close this window or press Ctrl-C.
Pause

ECHO ************************************  
ECHO Creating COE Users
ECHO ************************************    

CD sql 
   
sqlplus /NOLOG  @create_COEUsers.sql
log_create_COEUsers.txt



