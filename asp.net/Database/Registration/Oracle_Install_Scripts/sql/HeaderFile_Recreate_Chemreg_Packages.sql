

spool sql\log_recreate_chemreg_packages.txt

	@@PackagePrompts.sql

Connect &&schemaName/&&schemaPass@&&serverName

@@CREATE_Packages.sql

prompt logged session to: sql/log_recreate_chemreg_packages.txt
spool off

exit