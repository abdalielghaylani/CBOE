-- Copyright Cambridgesoft Corp 2001-2007 all rights reserved

--################################################################
-- Headerfile for adding Millennium data to 9SR4
--################################################################ 

spool ..\..\Logs\LOG_Update_Inventory_9SR4_to_Millennium.txt

--' Intialize variables
@@..\..\Parameters.sql
@@..\..\Prompts.sql
Connect &&schemaName/&&schemaPass@&&serverName

---------------------------------------
--  New view view_inv_custom_fields  --
---------------------------------------
@@Create\Views\View_Inv_Custom_Fields.sql

-----------------------------------------------------
--  Alter cs_security for New privilege and tables --
-----------------------------------------------------
@@Alter\Alter_Cs_Security.sql

-- Reconnect as the proper user
Connect &&schemaName/&&schemaPass@&&serverName

--' Insert application data
@@Data\TableData\Inv_Picklist_Types.sql
@@Data\TableData\Inv_Picklists.sql
@@Data\TableData\Inv_Custom_Field_Groups.sql
@@Data\TableData\Inv_Custom_Fields.sql
@@Data\TableData\Inv_reporttypes.sql
@@Data\TableData\Inv_reports.sql

prompt '#########################################################'
prompt 'Logged session to: Logs\LOG_Update_Inventory_9SR4_to_Millennium.txt
prompt '#########################################################'

prompt 
spool off

exit
