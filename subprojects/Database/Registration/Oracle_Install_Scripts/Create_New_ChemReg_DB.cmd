ECHO OFF
TITLE Create Chemical Registration tables and Users
CLS
ECHO You are about to create the REGDB Schema.
ECHO Any existing REGDB schema will be destroyed and all data lost.
ECHO To abort close this window or press Ctrl-C.
Pause

ECHO *******************************************************************************
ECHO Creating REGDB Schema and Tables, Adding ChemReg privileges and roles, Creating ChemReg test users
ECHO *******************************************************************************

   
sqlplus /NOLOG  @sql\HeaderFile_Create_ChemReg_ora.sql
notepad sql\log_create_chemreg_ora.txt

