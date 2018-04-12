cd %1
ECHO OFF
TITLE Update Inventory Database with DFCI Custom Field Data
CLS
ECHO You are about to add DFCI custom database objects to your 
ECHO Inventory E10 schema.  To abort close this window or press Ctrl-C.
Pause

cd DFCI_Objects
sqlplus /NOLOG @Update_Inventory_E10_to_DFCI.sql
notepad ..\..\Logs\LOG_Update_Inventory_E10_to_DFCI.txt
