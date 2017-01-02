-- Copyright 1998-2014 CambridgeSoft Corporation, an indirect, wholly-owned subsidiary of PerkinElmer, Inc. All rights reserved.

spool log_Create_Datalytix.txt

@@parameters.sql
@@prompts.sql

@@alter_coedb_for_datalytix.sql
@@evolveSchema.sql
@@create_VALID_CARTRIDGE_Table.sql

--
spool off
exit