--Copyright Cambridgesoft corp 1999-2002 all rights reserved


spool import_chemreg.txt

@@parameters.sql
@@prompts.sql
@@Disable_Triggers_ora.sql
Connect &&InstallUser/&&sysPass@&&serverName 
HOST imp.exe userid=&&InstallUser/&&sysPass@&&serverName  FILE=biosar_chemreg.dmp FULL=Y IGNORE=Y LOG=biosar_chemreg.log

@@RemoveBadLookup.sql

@@Enable_Triggers_ora.sql

spool off
exit


	
