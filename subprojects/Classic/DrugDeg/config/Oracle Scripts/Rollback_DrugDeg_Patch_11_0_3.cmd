cd %1
ECHO OFF
TITLE Rollbacking Patch 11.0.3 from DrugDeg Schema
CLS
ECHO You are about to rollback the patch 11.0.3 from DrugDeg Schema
ECHO To abort close this window or press Ctrl-C.
Pause

ECHO ************************************  
ECHO Updating DrugDeg Schema
ECHO ************************************    

sqlplus /NOLOG  @sql\Patches\HeaderFile_Rollback_Patch_11_0_3.sql
notepad sql\Patches\log_Rollback_DrugDeg_Patch_11_0_3.txt



