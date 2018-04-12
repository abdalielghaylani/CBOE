cd %1
ECHO OFF
TITLE Update Inventory Database from 9SR4 to 10
CLS
ECHO You are about to update your Inventory 9SR4 database to 10.
ECHO To abort close this window or press Ctrl-C.
Pause

cd 9SR4_to_10
sqlplus /NOLOG @Update_Inventory_9SR4_to_10.sql
notepad ../LOG_Update_Inventory_9SR4_to_10.txt
cd..