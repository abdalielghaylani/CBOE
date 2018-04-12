ECHO OFF
TITLE Import Chemical Inventory Schema Information For BioSAR
CLS
ECHO You are about to import the CHEMINV2 Schema Information to BioSar
ECHO To abort close this window or press Ctrl-C.
Pause
cd %1
CD sql


sqlplus /NOLOG @HEADERFILE_import_cheminv.sql

notepad import_cheminv.txt