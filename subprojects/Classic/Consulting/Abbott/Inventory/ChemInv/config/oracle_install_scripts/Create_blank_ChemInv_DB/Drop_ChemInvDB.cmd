cd %1
ECHO OFF
TITLE Drop Inventory Enterprise Database
CLS
ECHO You are about to destroy any previous contents of the 
ECHO Inventory Enterprise database.  
ECHO To abort close this window or press Ctrl-C.
Pause

sqlplus /NOLOG @sql\drop_cheminv_ora.sql
Notepad sql\Logs\LOG_drop_ChemInvDB.txt