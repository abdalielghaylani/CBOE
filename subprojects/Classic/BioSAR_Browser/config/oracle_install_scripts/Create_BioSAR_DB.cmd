ECHO OFF
TITLE Create verion 9 BioSARDB Schema and tables
CLS
ECHO You are about to destroy any previous contents of 
ECHO the BIOSARDB Schema if it exists.
ECHO To abort close this window or press Ctrl-C.
Pause
cd %1
CD sql
ECHO ************************************  
ECHO Creating BioSar Schema, Tables, Adding BioSAR privileges and roles, Creating Test users
ECHO ************************************     
   

sqlplus /NOLOG  @HEADERFILE_CREATE_biosar_ora.sql
notepad log_Create_BioSAR_DB.txt



