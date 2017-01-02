-- Copyright 1998-2014 CambridgeSoft Corporation, an indirect, wholly-owned subsidiary of PerkinElmer, Inc. All rights reserved.

spool ON
spool log_create_globalschema_ora.txt

@@Instance_parameters.sql
@@Instance_prompts.sql
@@Instance_CreateAll.sql

prompt logged session to: log_create_globalschema_ora.txt
spool off

exit