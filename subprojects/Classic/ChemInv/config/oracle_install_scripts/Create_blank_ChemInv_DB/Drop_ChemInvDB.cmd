cd %1
ECHO OFF
TITLE Drop Inventory Enterprise Database
CLS
ECHO You are about to destroy any previous contents of the 
ECHO Inventory Enterprise database.  
ECHO To abort close this window or press Ctrl-C.
Pause

cd sql
sqlplus /NOLOG @HEADERFILE_Drop_Inventory_Schema.sql
Notepad Logs\LOG_Drop_Inventory_Schema.txt