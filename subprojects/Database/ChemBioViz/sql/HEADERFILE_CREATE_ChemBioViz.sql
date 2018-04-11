--Copyright Cambridgesoft corp 1999-2002 all rights reserved


spool log_Create_ChemBioViz.txt

@@parameters.sql
@@prompts.sql

--Connect &&InstallUser/&&sysPass@&&serverName
--@@drops.sql
--@@tablespaces.sql
--@@users.sql  
--Connect &&schemaName/&&schemaPass@&&serverName

@@alter_coedb_for_chembioviz.sql

--
spool off
exit


	
