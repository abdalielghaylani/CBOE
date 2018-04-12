cd %1
ECHO OFF
TITLE Integrate Inventory Enterprise with Registration Enterprise	
CLS
ECHO To abort close this window or press Ctrl-C.
Pause
cd ..
sqlplus /NOLOG @Update_Scripts\RegistrationIntegration\RegistrationEnterprise.sql
notepad Logs\LOG_RegistrationIntegration.txt