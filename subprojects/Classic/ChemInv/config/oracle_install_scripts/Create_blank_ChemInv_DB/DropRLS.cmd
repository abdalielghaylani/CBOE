ECHO OFF
TITLE Drop RLS from Inventory Enterprise
CLS
ECHO This script drops the RLS policies being applied to Inventory Enterprise.
ECHO To abort close this window or press Ctrl-C.
Pause
CD sql
sqlplus /NOLOG @Update_Scripts\RLS\DropRLS.sql
notepad Logs\LOG_DropRLS.txt
