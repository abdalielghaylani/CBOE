ECHO OFF
TITLE Update BioSAR schema from 9.0SR3 to 10

CLS

ECHO ------------------------------------------------------------  
ECHO This script must be run in order to update BioSAR schema 
ECHO from 9.0 SR3 to 10. 
ECHO
ECHO To abort close this window or press Ctrl-C.
ECHO ------------------------------------------------------------  
Pause
cd %1
ECHO ************************************  
ECHO Update BioSAR Schema.
ECHO ************************************     
ECHO.
CD sql  
sqlplus /NOLOG  @HEADERFILE_UPDATE_biosar_ora_9SR3_to_9SR4.sql

NotePad log_update_schema.txt





