cd %1
ECHO OFF
TITLE Add RLS Policy
CLS
ECHO You are about to add RLS Policy for Registration Integration. 
ECHO To abort or close this window press Ctrl-C.
Pause
cd ..
sqlplus /NOLOG @Update_Scripts\RegistrationIntegration\AddRLSPolicy.sql
notepad Logs\LOG_AddRLSPolicy.txt
