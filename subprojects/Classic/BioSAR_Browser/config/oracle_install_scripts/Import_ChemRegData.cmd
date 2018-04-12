ECHO OFF
TITLE Import Chemical Registration Schema Information For BioSAR
CLS
ECHO You are about to import the RegDB Schema Information to BioSar
ECHO To abort close this window or press Ctrl-C.
Pause
cd %1
CD sql

sqlplus /NOLOG @HEADERFILE_import_chemreg.sql

notepad import_chemreg.txt