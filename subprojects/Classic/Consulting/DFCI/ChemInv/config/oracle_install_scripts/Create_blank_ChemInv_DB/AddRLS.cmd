ECHO OFF
TITLE ADD RLS to Inventory Enterprise
CLS
ECHO This script adds RLS to Inventory Enterprise.
ECHO To abort close this window or press Ctrl-C.
Pause
CD sql
sqlplus /NOLOG @Update_Scripts\RLS\AddRLS.sql
notepad Logs\LOG_AddRLS.txt
