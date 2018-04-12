cd %1
ECHO OFF
TITLE Drop tables and Users in cs_security Schema
CLS
ECHO You are about to destroy any previous contents of 
ECHO cs_security tables
ECHO To abort close this window or press Ctrl-C.
Pause

ECHO ************************************  
ECHO Droping cs_security Schema
ECHO ************************************     
   
sqlplus /NOLOG  @sql\drop_cs_security.sql
sql\log_drop_cs_security.txt



