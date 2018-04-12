cd %1
ECHO OFF
TITLE Update Inventory Database from 10 to 11.0.1
CLS
ECHO You are about to update your Inventory 10 database to 11.0.1 (Classic).
ECHO To abort close this window or press Ctrl-C.
Pause

cd 10_to_11.0.1
sqlplus /NOLOG @Update_Inventory_10_to_11.0.1.sql
notepad ..\..\Logs\LOG_Update_Inventory_10_to_11.0.1.txt
cd..