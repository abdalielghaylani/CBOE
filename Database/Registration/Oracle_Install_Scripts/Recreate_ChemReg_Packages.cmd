cd %1
ECHO OFF
TITLE Recreate Chemical Registration PL/SQL Packages
CLS
ECHO You are about to recreate the PL/SQL packages in the REGDB Schema.
ECHO Existing PL/SQL Packages will be overwritten with latest version in SQL folder.
ECHO To abort close this window or press Ctrl-C.
Pause

ECHO *******************************************************************************
ECHO Recreating REGDB PL/SQL Pacakges 
ECHO *******************************************************************************

   
sqlplus /NOLOG  @sql\HeaderFile_Recreate_Chemreg_Packages.sql
notepad sql\log_recreate_chemreg_packages.txt

