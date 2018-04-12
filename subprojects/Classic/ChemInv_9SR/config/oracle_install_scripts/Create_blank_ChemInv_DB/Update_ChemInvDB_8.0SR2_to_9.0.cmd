cd %1
ECHO OFF
TITLE Update Inventory database from 8.0SR2 to 9.0
CLS
ECHO This script updates the Inventory database from version 8.0SR2 to 9.0.
ECHO.    
ECHO To abort close this window or press Ctrl-C.
Pause
CD sql\update_scripts\8.0SR2_to_9.0
sqlplus /NOLOG @HEADERFILE_Update_ChemInvDB_8.0SR2_to_9.0.sql
notepad ..\..\Logs\LOG_Update_ChemInvDB_8.0SR2_to_9.0.txt
			
