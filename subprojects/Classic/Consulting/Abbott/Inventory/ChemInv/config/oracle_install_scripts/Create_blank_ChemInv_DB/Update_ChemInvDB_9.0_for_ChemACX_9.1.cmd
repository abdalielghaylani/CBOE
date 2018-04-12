cd %1
ECHO OFF
TITLE Update Inventory database 9.0 for use with ChemACX 9.1
CLS
ECHO This script updates the Inventory 9.0 database for use with ChemACX 9.1.
ECHO.    
ECHO To abort close this window or press Ctrl-C.
Pause
CD sql\update_scripts\ACX_9.1
sqlplus /NOLOG @Update_Units.sql
notepad ..\..\Logs\LOG_Update_ChemInvDB_9.0_for_ChemACX_9.1.txt
			
