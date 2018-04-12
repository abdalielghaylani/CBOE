cd %1
ECHO OFF
TITLE Create Structure Index using CS Cartridge 2.1
CLS
ECHO You are about to create a chemical structure index using  
ECHO CambridgeSoft's Cartridge version 2.1.
ECHO This process may take serveral hours.
Pause

sqlplus /NOLOG @sql\indexingworkaround.sql
sql\log_create_cartridge_idx_2.1.txt