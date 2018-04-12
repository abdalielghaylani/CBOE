cd %1
ECHO OFF
TITLE Update Inventory Enterprise Integration with Registration Enterprise for 10
CLS
ECHO You are about to update the Inventory Enterprise integration
ECHO with Registration Enterprise for the 10 release.
ECHO To abort or close this window press Ctrl-C.
Pause

cd 9SR3_to_10
sqlplus /NOLOG @UpdateRegIntegration_to_10.sql
notepad ..\..\Logs\LOG_UpdateRegIntegration10.txt
cd..
