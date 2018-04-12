cd %1
ECHO OFF
TITLE Create Structure Index using CS Cartridge 9.0
CLS
ECHO You are about to create a chemical structure index using  
ECHO CambridgeSoft's Cartridge version 9.0.
ECHO This process may take serveral hours.
Pause

sqlplus /NOLOG @sql\CSCartridge9Index.sql
sql\log_create_cartridge_idx_9.0.txt