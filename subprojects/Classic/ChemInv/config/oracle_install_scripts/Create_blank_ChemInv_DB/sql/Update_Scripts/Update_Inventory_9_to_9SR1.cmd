cd %1
ECHO OFF
TITLE Update Inventory Database from 9 to 9SR1
CLS
ECHO You are about to update your Inventory 9 database to 9SR1.
ECHO To abort close this window or press Ctrl-C.
Pause

cd 9_to_9SR1
sqlplus /NOLOG @Update_Inventory_9_to_9SR1.sql
notepad ..\..\Logs\LOG_Update_Inventory_9_to_9SR1.txt

