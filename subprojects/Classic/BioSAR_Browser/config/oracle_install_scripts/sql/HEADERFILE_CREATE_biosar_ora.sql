--Copyright Cambridgesoft corp 1999-2002 all rights reserved


spool log_Create_BioSAR_DB.txt

@@parameters.sql
@@prompts.sql

Connect &&InstallUser/&&sysPass@&&serverName
@@drops.sql
@@tablespaces.sql
@@users.sql  
Connect &&schemaName/&&schemaPass@&&serverName
@@Create_BioSar_Browser_Ora_80.sql
@@ALTER_cs_security_biosar_browser.sql
@@UPDATE_cs_security_biosar_browser.sql
@@BioSar_Browser_test_users.sql
Connect &&InstallUser/&&sysPass@&&serverName
@@synonyms

--  grant unlimited on biosar tablespaces to cs_security
--
alter user BIOSARDB quota unlimited on &&tableSpaceName;
alter user BIOSARDB quota unlimited on &&indexTableSpaceName;

@@update_BioSar_from_8_0_114_to_9.sql
@@update9_toSR1.sql
@@update9_toSR2.sql
@@update9_toSR3.sql
@@update9_toSR4.sql
@@update_10_to_1101.sql
--

-- ' Applying the latest patch
@@"Patches\Parameters.sql"
@@"Patches\Patch &&schemaVersion\Parameters.sql"
@@"Patches\Patch &&nextPatch\patch.sql"

spool off
exit


	
