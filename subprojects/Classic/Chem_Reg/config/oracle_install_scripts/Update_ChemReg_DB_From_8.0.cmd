ECHO OFF
TITLE Update Chemical Registration Schema From the ChemReg 2004_8.0 release version.

CLS
ECHO You are about to update your REGDB schema to the latest version.
ECHO To abort close this window or press Ctrl-C.
Pause
cd %1
ECHO ************************************  
ECHO Adding new ChemReg views and granting privileges
ECHO ************************************  
ECHO 
sqlplus /NOLOG  @sql\HEADERFILE_8.0_update_script.sql
notepad sql\log_update_chemreg_db_from_8.0.txt



