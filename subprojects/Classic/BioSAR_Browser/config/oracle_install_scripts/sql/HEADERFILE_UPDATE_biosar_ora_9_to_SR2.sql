--Copyright Cambridgesoft corp 1999-2005 all rights reserved



spool log_update_schema.txt
@@parameters.sql
@@prompts.sql

-- first spool some grants we will need later


set echo off
@@update9_toSR1.sql
@@update9_toSR2.sql
@@pkg_tree_def.sql;
@@pkg_tree.sql;

spool off
exit


	
