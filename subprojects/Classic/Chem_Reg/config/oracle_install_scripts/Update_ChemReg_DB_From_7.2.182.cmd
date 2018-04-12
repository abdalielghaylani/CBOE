ECHO OFF
TITLE Update Chemical Registration Schema From the ChemReg 2002_7.2.182 release version.

CLS
ECHO You are about to update your REGDB schema to the latest version.
ECHO To abort close this window or press Ctrl-C.
Pause
cd %1
ECHO ************************************  
ECHO Adding new ChemReg Tables and fields, Adding new ChemReg privileges and roles
ECHO  You must also run Update_LONG_to_CLOB.cmd  which will update structure fields to CLOB and index them using the CS Cartridge
ECHO ************************************  
ECHO 
sqlplus /NOLOG  @sql\HEADERFILE_7.2.182_update_script.sql
notepad sql\log_update_chemreg_db_from_7.2.182.txt



