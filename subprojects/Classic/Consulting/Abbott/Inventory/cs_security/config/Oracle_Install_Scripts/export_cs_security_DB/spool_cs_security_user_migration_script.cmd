
ECHO OFF
TITLE Migrate CS_Security Users
CLS
ECHO This script spools a sql script which can be used to migrate   
ECHO cs_security users to a new Oracle database.

ECHO Executing this script will take no action on the source database.  
ECHO It will simply create a sql script which must be executed against
ECHO the new (target) database in order to create new users.
 
ECHO The spooled scrript is called: migrate_cs_security_users.sql
ECHO To abort close this window or press Ctrl-C.
Pause

sqlplus /NOLOG @sql\spool_cs_security_user_migration_script.sql
notepad migrate_cs_security_users.sql