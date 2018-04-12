ECHO OFF
TITLE Disable Row Level Security. 
CLS
ECHO You are about to Disable Row Level Secruity for Chemical Registration.
ECHO This script does not alter the database schema. It only removes the Fine Grain Access Control. 
ECHO To abort close this window or press Ctrl-C.
Pause
cd %1
CD sql
ECHO ************************************  
ECHO Disabling Row Level Security for REGDB
ECHO ************************************     
   

sqlplus /NOLOG @REMOVE_FGAC_FOR_PROJECT_RLS.sql
notepad log_disable_rls.txt

