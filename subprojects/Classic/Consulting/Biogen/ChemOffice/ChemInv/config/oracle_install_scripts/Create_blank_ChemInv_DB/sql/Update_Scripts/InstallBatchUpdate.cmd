cd %1
ECHO OFF
TITLE Add BatchUpdate functionality
CLS
ECHO You are about add BatchUpdate functionality.
ECHO To abort close this window or press Ctrl-C.
Pause

cd BatchUpdate
sqlplus /NOLOG @batchupdate.sql
notepad ..\..\Logs\batchupdate.txt
cd..