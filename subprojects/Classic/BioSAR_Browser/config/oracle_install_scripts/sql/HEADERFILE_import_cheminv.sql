--Copyright Cambridgesoft corp 1999-2002 all rights reserved


spool import_cheminv.txt

@@parameters.sql
@@prompts.sql
@@Disable_Triggers_ora.sql
Connect &&InstallUser/&&sysPass@&&serverName 
HOST imp.exe userid=&&InstallUser/&&sysPass@&&serverName  FILE=biosar_cheminv2.dmp FULL=Y IGNORE=Y LOG=biosar_cheminv2.dmp.log
@@Enable_Triggers_ora.sql

spool off
exit


	
