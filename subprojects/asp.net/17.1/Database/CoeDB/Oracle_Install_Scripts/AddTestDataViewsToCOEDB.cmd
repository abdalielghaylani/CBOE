ECHO OFF
TITLE Create CoeDB tables and Users
CLS
ECHO You are about Add test dataviews to COEDB.COEDataviews.
ECHO This assumes you have the following data - CHEMACXDB, CHEMINVDB2, SAMPLE, REGDB.
ECHO If you do not have all of this data you should remove the dataviews that do not have data.
ECHO To abort close this window or press Ctrl-C.
Pause
cd %1
ECHO ************************************  
ECHO Creating test dataviews
ECHO ************************************     
   
sqlplus /NOLOG  @sql\HEADERFILE_testDataViews.sql
notepad sql\log_testdataviews.txt
