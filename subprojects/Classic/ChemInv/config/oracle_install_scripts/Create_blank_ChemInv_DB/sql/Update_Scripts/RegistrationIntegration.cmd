cd %1
ECHO OFF
TITLE Integrate Inventory Enterprise with Registration Enterprise	
CLS
ECHO You are about to integrate Inventory Enterprise with Registration Enterprise. 
ECHO To abort or close this window press Ctrl-C.
Pause
cd ..
sqlplus /NOLOG @Update_Scripts\RegistrationIntegration\RegistrationEnterprise.sql
notepad Logs\LOG_RegistrationIntegration.txt
