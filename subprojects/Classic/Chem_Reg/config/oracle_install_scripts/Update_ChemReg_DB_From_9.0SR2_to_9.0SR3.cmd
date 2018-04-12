ECHO OFF
TITLE Update Chemical Registration Schema From the ChemReg 2005 9.0 SR2 version to 9.0 SR3

CLS
ECHO You are about to update your REGDB schema to the latest version.
ECHO To abort close this window or press Ctrl-C.
Pause
cd %1
ECHO ************************************  
ECHO Adding new ChemReg views and granting privileges
ECHO ************************************  
ECHO 
sqlplus /NOLOG  @sql\HEADERFILE_9.0_SR2_update_script.sql
notepad sql\log_update_chemreg_db_from_9.0_SR2_to_9.0_SR3.txt



