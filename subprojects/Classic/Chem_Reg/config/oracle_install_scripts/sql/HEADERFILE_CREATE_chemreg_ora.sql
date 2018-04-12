--Copyright 1999-2003 CambridgeSoft Corporation. All rights reserved

spool ON
spool sql\log_create_chemreg_ora.txt

@@parameters.sql
@@prompts.sql

Connect &&InstallUser/&&sysPass@&&serverName
@@drops.sql
@@tablespaces.sql
@@users.sql  
Connect &&schemaName/&&schemaPass@&&serverName
@@CREATE_chemreg_ora.sql
@@ALTER_cs_security_for_chemreg_ora.sql
@@CREATE_chemreg_test_users.sql
@@2002_7.1.152_update_script.sql
@@2004_update_7.2.182_script.sql
@@update_8.0_to_9.0.sql
@@update_9.0_to_9.0SR1.sql
@@update_9.0SR1_to_9.0SR2.sql
@@update_9.0SR2_to_9.0SR3.sql
@@indexClobFields&&OraVersionNumber
@@update_9.0SR3_to_10.0.sql
Connect &&InstallUser/&&sysPass@&&serverName
@@synonyms.sql

prompt logged session to: sql/log_create_chemreg_ora.txt
spool off

exit


	