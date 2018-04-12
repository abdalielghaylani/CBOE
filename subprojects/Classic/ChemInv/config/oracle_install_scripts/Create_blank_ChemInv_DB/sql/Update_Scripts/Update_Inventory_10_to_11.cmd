cd %1
ECHO OFF
TITLE Update Inventory Database from 10.0 to 11
CLS
ECHO You are about to update your Inventory 10.0 database to 11
ECHO To abort close this window or press Ctrl-C.
Pause

cd 10.0_to_11
sqlplus /NOLOG @Update_Inventory_10_to_11.sql
notepad ..\..\Logs\LOG_Update_Inventory_10_to_11.txt
cd..