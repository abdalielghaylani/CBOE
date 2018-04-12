cd %1
ECHO OFF
TITLE Update Inventory Database from 9SR1 to 9SR2
CLS
ECHO You are about to update your Inventory 9SR1 database to 9SR2.
ECHO To abort close this window or press Ctrl-C.
Pause

cd 9SR1_to_9SR2
sqlplus /NOLOG @Update_Inventory_9SR1_to_9SR2.sql
notepad ..\..\Logs\LOG_Update_Inventory_9SR1_to_9SR2.txt
