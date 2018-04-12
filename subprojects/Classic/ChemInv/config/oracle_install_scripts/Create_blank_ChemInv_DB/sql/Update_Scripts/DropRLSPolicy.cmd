cd %1
ECHO OFF
TITLE Drop RLS Policy
CLS
ECHO You are about to Drop RLS Policy for Registration Integration. 
ECHO To abort or close this window press Ctrl-C.
Pause
cd ..
sqlplus /NOLOG @Update_Scripts\RegistrationIntegration\DropRLSPolicy.sql
notepad Logs\LOG_DropRLSPolicy.txt
