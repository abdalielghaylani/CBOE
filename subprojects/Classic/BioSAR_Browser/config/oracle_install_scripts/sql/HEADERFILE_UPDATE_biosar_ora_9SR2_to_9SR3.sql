--Copyright Cambridgesoft corp 1999-2005 all rights reserved



spool log_update_schema.txt
@@parameters.sql
@@prompts.sql




set echo off

@@update9_toSR3.sql

spool off
exit


	
