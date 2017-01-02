ECHO OFF
TITLE VALIDATE DB OBJECTS 
CLS
ECHO You are about to recompile all invalid objects in COEDB, REGDB, CHEMINVDB2
ECHO To abort close this window or press Ctrl-C.
Pause
cd%1
ECHO. 
ECHO *******************************************************************************
ECHO   Recompie DB Objects
ECHO *******************************************************************************

   
sqlplus /NOLOG  @sql\HEADERFILE_FIX_DB_ora.sql
notepad sql\log_FIX_DB_ora.txt