-- Copyright Cambridgesoft Corp 2001-2007 all rights reserved

--################################################################
-- Headerfile for updating the cheminvdb2 schema from 10 to 11.0.1
--################################################################ 


Connect &&schemaName/&&schemaPass@&&serverName

prompt '#########################################################'
prompt 'Updating Tables...'
prompt '#########################################################'

@sql\Update_Scripts\10_to_11.0.1\Alter\Alter_Inv_PlateAuditing.sql

prompt '#########################################################'
prompt 'Updating Functions and Packages...'
prompt '#########################################################'
@sql\Update_Scripts\10_to_11.0.1\PLSQL\Functions\f_CreatePlateXML.sql
@sql\Update_Scripts\10_to_11.0.1\PLSQL\Functions\f_CopyPlate.sql


@sql\Update_Scripts\10_to_11.0.1\PLSQL\Packages\pkg_Reformat_def.sql
@sql\Update_Scripts\10_to_11.0.1\PLSQL\Packages\pkg_Reformat_body.sql
@sql\Update_Scripts\10_to_11.0.1\PLSQL\Packages\pkg_Compounds_Body.sql
@sql\Update_Scripts\10_to_11.0.1\PLSQL\Packages\pkg_Requests_def.sql
@sql\Update_Scripts\10_to_11.0.1\PLSQL\Packages\pkg_Requests_body.sql
@sql\Update_Scripts\10_to_11.0.1\PLSQL\Packages\pkg_Compounds_Body.sql
@sql\Update_Scripts\10_to_11.0.1\PLSQL\Packages\pkg_GUIUtils_def.sql
@sql\Update_Scripts\10_to_11.0.1\PLSQL\Packages\pkg_GUIUtils_Body.sql
prompt '#########################################################'
prompt 'Updating triggers...'
prompt '#########################################################'

@sql\Update_Scripts\10_to_11.0.1\PLSQL\Triggers\inv_plates_biu0.sql
@sql\Update_Scripts\10_to_11.0.1\PLSQL\Triggers\inv_plates_au0.sql
@sql\Update_Scripts\10_to_11.0.1\PLSQL\Triggers\inv_plates_ad0.sql
@sql\Update_Scripts\10_to_11.0.1\PLSQL\Triggers\inv_wells_biu0.sql
@sql\Update_Scripts\10_to_11.0.1\PLSQL\Triggers\inv_wells_au0.sql
@sql\Update_Scripts\10_to_11.0.1\PLSQL\Triggers\inv_wells_ad0.sql
@sql\Update_Scripts\10_to_11.0.1\PLSQL\Triggers\inv_wells_weight_es.sql

prompt '#########################################################'
prompt 'Inserting data...'
prompt '#########################################################'


@sql\Update_Scripts\10_to_11.0.1\PLSQL\RecompilePLSQL.sql

prompt #############################################################
prompt Logged session to: Logs\LOG_Update_Inventory_10_to_11.0.1.txt
prompt #############################################################

prompt 

