ECHO OFF
TITLE Import Chemical Registration  and ChemInventory Schema Information For BioSAR
CLS
ECHO You are about to import the REGDB and CHEMINV2 Schema Information to BioSar
ECHO To abort close this window or press Ctrl-C.
Pause
cd %1
CD sql


sqlplus /NOLOG @HEADERFILE_import_chemreg_cheminv.sql

notepad import_chemreg_cheminv.txt