ECHO OFF
TITLE Update Chemical Registration Schema From  d25 version.
CLS
ECHO You are about to update your REGDB schema to the lates release
ECHO To abort close this window or press Ctrl-C.
Pause
cd %1
ECHO ************************************  
ECHO Adding new ChemREg Tables and fields, Adding new ChemReg privileges and roles
ECHO  You must also run Update_LONG_to_CLOB.cmd  which will update structure fields to CLOB and index them using the CS Cartridge
ECHO ************************************     
   

sqlplus /NOLOG  @sql\HEADERFILE_2000bd67_update_script.sql
notepad sql\log_update_chemreg_db_from_d67.txt



