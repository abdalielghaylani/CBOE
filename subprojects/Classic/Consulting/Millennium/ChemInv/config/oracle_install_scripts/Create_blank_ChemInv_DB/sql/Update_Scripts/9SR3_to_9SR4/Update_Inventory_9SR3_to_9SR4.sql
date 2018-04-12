-- Copyright Cambridgesoft Corp 2001-2007 all rights reserved

--################################################################
-- Headerfile for updating the cheminvdb2 schema from 9SR3 to 9SR4
--################################################################ 

spool ..\..\Logs\LOG_Update_Inventory_9SR3_to_9SR4.txt

-- Intialize variables
@@..\..\Parameters.sql
@@..\..\Prompts.sql
Connect &&schemaName/&&schemaPass@&&serverName

------------------------------------
--  New table inv_picklist_types  --
------------------------------------
@@Create\Tables\Inv_Picklist_Types.sql

---------------------------------
--  Alter table inv_picklists  --
---------------------------------
@@Alter\Alter_Inv_Picklists.sql

----------------------------------------
--  New table inv_custom_field_groups --
----------------------------------------
@@Create\Tables\Inv_Custom_Field_Groups.sql

----------------------------------
--  New table inv_custom_fields --
----------------------------------
@@Create\Tables\Inv_Custom_Fields.sql

--------------------------------------------
--  New table inv_custom_cpd_field_values --
--------------------------------------------
@@Create\Tables\Inv_Custom_Cpd_Field_Values.sql

--------------------------------------------
--  Updated functions
--------------------------------------------

@@PLSQL\Functions\f_CheckoutContainer.sql

----------------------------------------
--  Create new indexes for efficiency --
----------------------------------------
Create index fidx_inv_comp_up On INV_Compounds(UPPER(substance_name)) TABLESPACE &&indexTableSpaceName;
Create index fidx_inv_syn_up On Inv_SYNONYMS(UPPER(substance_name)) TABLESPACE &&indexTableSpaceName;
Create index fidx_inv_comp_low On INV_Compounds(LOWER(substance_name)) TABLESPACE &&indexTableSpaceName;
Create index fidx_inv_syn_low On Inv_SYNONYMS(LOWER(substance_name)) TABLESPACE &&indexTableSpaceName;

-----------------------------------------------------
--  Alter cs_security for New privilege and tables --
-----------------------------------------------------
@@Alter\Alter_Cs_Security.sql

-- Note: no new synonyms, not best practices

-- Insert application data
Connect &&schemaName/&&schemaPass@&&serverName
@@Data\TableData\Inv_Picklist_Types.sql
@@Data\TableData\Inv_Picklists.sql

-- Update DB version
UPDATE GLOBALS SET value = '4.4' WHERE ID = 'VERSION_SCHEMA'; 

prompt #############################################################
prompt Logged session to: Logs\LOG_Update_Inventory_9SR3_to_9SR4.txt
prompt #############################################################

prompt 
spool off

exit
