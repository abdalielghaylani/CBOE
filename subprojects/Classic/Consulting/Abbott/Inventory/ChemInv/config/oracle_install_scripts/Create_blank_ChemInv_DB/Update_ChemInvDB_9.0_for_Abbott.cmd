cd %1
ECHO OFF
TITLE Update Inventory database from 9.0 to Abbott
CLS
ECHO This script updates the Inventory database from version 9.0 to Abbott Configuration.
ECHO.    
ECHO To abort close this window or press Ctrl-C.
Pause
CD sql\update_scripts\9.0_to_Abbott
sqlplus /NOLOG @HEADERFILE_Update_ChemInvDB_9.0_to_Abbott.sql
notepad ..\..\Logs\LOG_Update_ChemInvDB_9.0_to_Abbott.txt
			
