ECHO OFF
TITLE Update DrugDeg tables and Users in DrugDeg Schema
CLS
ECHO You are about to modify the DrugDeg Schema
ECHO To abort close this window or press Ctrl-C.
Pause

ECHO ************************************  
ECHO Updating DrugDeg Schema
ECHO ************************************    

CD sql 
   
sqlplus /NOLOG  @update_DrugDeg_9sr3_to_10.sql
log_update_drugdeg.txt



