cd %1
ECHO OFF
TITLE Update Inventory Database from 9SR2 to 9SR3
CLS
ECHO You are about to update your Inventory 9SR2 database to 9SR3.
ECHO To abort close this window or press Ctrl-C.
Pause

cd 9SR2_to_9SR3
sqlplus /NOLOG @Update_Inventory_9SR2_to_9SR3.sql
notepad ..\..\Logs\LOG_Update_Inventory_9SR2_to_9SR3.txt