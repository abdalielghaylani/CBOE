ECHO OFF
TITLE Enable Row Level Security. 
CLS
ECHO You are about to Enable Row Level Secruity for Chemical Registration.
ECHO You must have run either Update_ChemReg_DB_From_7.1.152.cmd
ECHO or Create_New_ChemReg_DB.cmd prior to running this script.
ECHO To abort close this window or press Ctrl-C.
Pause
cd %1
CD sql
ECHO ************************************  
ECHO Adding Row Level Secruity to REGDB
ECHO ************************************     
   

sqlplus /NOLOG @CREATE_FGAC_FOR_PROJECT_RLS.sql
notepad log_enable_rls.txt


