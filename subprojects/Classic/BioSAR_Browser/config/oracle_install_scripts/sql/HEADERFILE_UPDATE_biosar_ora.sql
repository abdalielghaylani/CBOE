--Copyright Cambridgesoft corp 1999-2002 all rights reserved




@@parameters.sql
@@prompts.sql

-- first spool some grants we will need later
Connect &&securitySchemaName/&&securitySchemaPass@&&serverName

set echo off
@@spoolgrants.sql

set feedback on

spool log_update_schema.txt


Connect &&InstallUser/&&sysPass@&&serverName
@@drops_update.sql
@@tablespaces.sql
@@users.sql 
Connect &&schemaName/&&schemaPass@&&serverName
@@Create_BioSar_Browser_Ora_80.sql
Connect &&InstallUser/&&sysPass@&&serverName
@@synonyms
Connect &&schemaName/&&schemaPass@&&serverName
@@Disable_Triggers_ora.sql
@@Transfer_Data.sql
@@Enable_Triggers2_ora.sql
@@Transfer_Grants.sql
@@Cleanup_72_CS_SECURITY.sql
@@UPDATE_cs_security_biosar_browser.sql
@@update_only_BioSar_Browser.sql
@@update_BioSar_from_8_0_114_to_9.sql
@@update9_toSR1.sql
@@update9_toSR2.sql
@@update9_toSR3.sql
--
--  grant unlimited on biosar tablespaces to cs_security
--
alter user BIOSARDB quota unlimited on &&tableSpaceName;
alter user BIOSARDB quota unlimited on &&indexTableSpaceName;

spool off
exit


	
