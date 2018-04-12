ECHO OFF
TITLE Prepares regdb schema for importing a dump file
CLS
ECHO You are about to drop the REGDB Schema.
ECHO Any existing REGDB schema will be destroyed and all data lost.
ECHO To abort close this window or press Ctrl-C.
Pause
cd %1
ECHO ************************************  
ECHO Creating REGDB Schema and Tables, Adding ChemReg privileges and roles, Creating ChemReg test users
ECHO ************************************     
   
sqlplus /NOLOG  @sql\HEADERFILE_prepare_chemreg_ora_for_import.sql
notepad sql\log_prepare_chemreg_for_import.txt

