ECHO OFF
TITLE Import Legacy Registration Data into REGDB
CLS
ECHO You are about to Import Legacy Registration Data
ECHO This will import the data and append it to the REGDB schema
ECHO To abort close this window or press Ctrl-C.
Pause
cd %1
ECHO ************************************  
ECHO Importing Legacy Registration Data
ECHO ************************************     
   
sqlplus /NOLOG  @sql\HEADERFILE_Import_LegacyData.sql
notepad log\log_Import_LegacyData.txt

