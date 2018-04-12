-- Copyright Cambridgesoft Corp 2001-2006 all rights reserved

-- Drop inventory database.
-- This script drops the table spaces and schema owner for the inventory database.

-- NOTE THIS SCRIPT MUST BE RUN FROM THE COMMAND LINE VERSION OF SQLPLUS
-- This script will not run from SQLPlus Worksheet!

SPOOL ON
spool Logs\LOG_Drop_Inventory_Schema.txt

@@parameters.sql
@@prompts.sql

@@Drop\Drop_Synonyms.sql
@@Drop\Drop_UsersRoles.sql
@@Drop\Drop_Tablespaces.sql
@@Drop\Drop_PrivilegeTable.sql
@@Data\Delete_CsSecurityData.sql

spool off;
exit

