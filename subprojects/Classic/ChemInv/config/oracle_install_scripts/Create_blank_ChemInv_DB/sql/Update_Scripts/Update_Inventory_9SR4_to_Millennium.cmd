cd %1
ECHO OFF
TITLE Update Inventory Database with Millennium Custom Field Data
CLS
ECHO You are about to add Millennium custom fields to your Inventory 9SR4.
ECHO To abort close this window or press Ctrl-C.
Pause

cd 9SR4_to_Millennium
sqlplus /NOLOG @Update_Inventory_9SR4_to_Millennium.sql
notepad ..\..\Logs\LOG_Update_Inventory_9SR4_to_Millennium.txt
