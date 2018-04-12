ECHO OFF
TITLE Create DrugDeg tables and Users in DrugDeg Schema
CLS
ECHO You are about to destroy any previous contents of 
ECHO DrugDeg tables
ECHO To abort close this window or press Ctrl-C.
Pause

ECHO ************************************  
ECHO Creating DrugDeg Tables
ECHO ************************************    

CD sql 
   
sqlplus /NOLOG  @create_DrugDeg.sql
log_create_drugdeg.txt



