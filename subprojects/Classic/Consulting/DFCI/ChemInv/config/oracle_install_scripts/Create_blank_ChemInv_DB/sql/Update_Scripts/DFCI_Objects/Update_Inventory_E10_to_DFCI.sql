-- Copyright Cambridgesoft Corp 2001-2007 all rights reserved

--################################################################
-- Headerfile for adding DFCI data to E10
--################################################################ 

spool ..\..\Logs\LOG_Update_Inventory_E10_to_DFCI.txt

--' Intialize variables
@@..\..\Parameters.sql
@@..\..\Prompts.sql

-- Reconnect as the proper user
Connect &&schemaName/&&schemaPass@&&serverName

prompt '#########################################################'
prompt 'Inserting data...'
prompt '#########################################################'
@@Data\TableData\Inv_Picklist_Types.sql
@@Data\TableData\Inv_Picklists.sql
@@Data\TableData\Inv_Custom_Field_Groups.sql
@@Data\TableData\Inv_Custom_Fields.sql
@@Data\TableData\Inv_location_types.sql


prompt '#########################################################'
prompt 'Creating tables...'
prompt '#########################################################'
@@Create\Tables\inv_label_printers.sql
@@Create\Tables\Inv_Forecast.sql
@@Create\Tables\Inv_protocol.sql
@@Create\Tables\Inv_protocol_pi.sql
@@Create\Tables\TRANSACTION_RECORD.sql
@@ALTER\Alter_Inv_Forecast.sql
@@Alter\Alter_Inv_Compounds.sql

DEFINE acxSchemaName = CHEMACXDB
DEFINE acxSchemaPass = ORACLE 

-- Chemacx table
Connect &&acxschemaName/&&acxschemaPass@&&serverName
GRANT SELECT,INSERT,UPDATE,DELETE ON "&&acxSchemaName".PACKAGESIZECONVERSION TO CS_SECURITY WITH GRANT OPTION;
GRANT SELECT,INSERT,UPDATE,DELETE ON "&&acxSchemaName".PACKAGE TO CS_SECURITY WITH GRANT OPTION;

Connect &&schemaName/&&schemaPass@&&serverName

prompt '#########################################################'
prompt 'Updating Procedures...'
prompt '#########################################################'

@@PLSQL\Procedures\sp_dfci_inventoryupdate.sql
@@PLSQL\Procedures\sp_dfci_inventoryupdate_ids.sql

prompt '#########################################################'
prompt 'Updating Packages...'
prompt '#########################################################'

@@PLSQL\Packages\pkg_GUIUtils_def.sql
@@PLSQL\Packages\pkg_GUIUtils_Body.sql
@@PLSQL\Packages\pkg_requests_def.sql
@@PLSQL\Packages\pkg_requests_Body.sql
@@PLSQL\Packages\pkg_Compounds_def.sql
@@PLSQL\Packages\pkg_compounds_body.sql

prompt '#########################################################'
prompt 'Updating Functions...'
prompt '#########################################################'

@@PLSQL\Functions\f_genABCFeed.sql
@@PLSQL\Functions\f_createorupdateprotocol.sql
@@PLSQL\Functions\f_createorupdateprotocolpi.sql
@@PLSQL\Functions\f_dfci_cyclecountupdate.sql
@@PLSQL\Functions\f_dfci_getlocationatdate.sql
@@PLSQL\Functions\f_dfci_getpiatdate.sql
@@PLSQL\Functions\f_dfci_getqtyremainingatdate.sql


prompt '#########################################################'
prompt 'Updating Triggers...'
prompt '#########################################################'
@@PLSQL\Triggers\audit_inv_protocol_bi0.trg
@@PLSQL\Triggers\audit_inv_protocol_pi_bi0.trg
@@PLSQL\Triggers\audit_inv_protocol_pi_au0.trg
@@PLSQL\Triggers\audit_inv_protocol_au0.trg
@@PLSQL\Triggers\audit_inv_protocol_pi_ad0.trg
@@PLSQL\Triggers\audit_inv_protocol_ad0.trg


@@Alter\Alter_Cs_Security.sql

prompt '#########################################################'
prompt 'Recompiling pl/sql...'
prompt '#########################################################'

--' Recompile pl/sql 
@@PLSQL\RecompilePLSQL.sql

prompt '#########################################################'
prompt 'Logged session to: Logs\LOG_Update_Inventory_E10_to_DFCI.txt
prompt '#########################################################'

prompt 
spool off

exit
