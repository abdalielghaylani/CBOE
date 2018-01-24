ECHO OFF
TITLE Create CS-Security Synonyms Schema
CLS
ECHO You are about to create the CS-Security Synonyms Schema.
ECHO Any existing CS-Security schema will be destroyed and all data lost.
ECHO To abort close this window or press Ctrl-C.
Pause
cd %1
ECHO ************************************  
ECHO Creating CS-Security Synonyms Schema
ECHO ************************************     
   
sqlplus /NOLOG  @"sql\Patches\Patch 11.0.1.0\sql\CsSecurity\HEADERFILE_CREATE_CsSecurity_ora.sql"
notepad "sql\Patches\Patch 11.0.1.0\sql\CsSecurity\log_create_CSSecurity_ora.txt"

