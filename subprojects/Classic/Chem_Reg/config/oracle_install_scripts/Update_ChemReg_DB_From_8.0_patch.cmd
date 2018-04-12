ECHO OFF
TITLE Update Chemical Registration Schema From the ChemReg 2004_8.0 release version (patch).

CLS
ECHO Run this after running Update_ChemReg_DB_From_8.0.cmd. This is a patch script adding four fields to temporary_structures.
ECHO To abort close this window or press Ctrl-C.
Pause
cd %1
ECHO ************************************  
ECHO Adding four fields to temporary_structures
ECHO ************************************  
ECHO 
sqlplus /NOLOG  @sql\HEADERFILE_8.0_update_script_patch.sql
notepad sql\log_update_chemreg_db_from_8.0_patch.txt



