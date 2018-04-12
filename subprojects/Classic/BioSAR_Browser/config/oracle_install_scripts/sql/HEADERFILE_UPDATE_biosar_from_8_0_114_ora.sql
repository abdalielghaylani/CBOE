--Copyright Cambridgesoft corp 1999-2002 all rights reserved



spool log_update_schema.txt
@@parameters.sql
@@prompts.sql

-- first spool some grants we will need later


set echo off
@@upgradedrops.sql
@@update_BioSar_from_8_0_114_to_9.sql
@@update9_toSR1.sql
@@update9_toSR2.sql
spool off
exit


	
