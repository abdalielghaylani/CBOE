-- Copyright Cambridgesoft Corp 2001-2006 all rights reserved

--#########################################################
-- Create schema views
--######################################################### 

Connect &&schemaName/&&schemaPass@&&serverName

prompt '#########################################################'
prompt 'Creating schema views...'
prompt '#########################################################'


@@Create\Views\Inv_VW_Physical_Plate.sql
@@Create\Views\Inv_VW_Plate_Format.sql
@@Create\Views\Inv_VW_Plate.sql
@@Create\Views\Inv_VW_Well_Format.sql
@@Create\Views\Inv_VW_Well.sql
@@Create\Views\Inv_VW_Well_Flat.sql
@@Create\Views\Inv_VW_Grid_Location.sql
@@Create\Views\Inv_VW_NonGrid_Locations.sql
@@Create\Views\INV_VW_Grid_Location_Parent.sql
@@Create\Views\Inv_VW_Plate_Grid_Locations.sql
@@Create\Views\Inv_VW_Plate_Locations.sql
@@Create\Views\Inv_VW_Enumerated_Values.sql
@@Create\Views\Inv_VW_Plate_History.sql
@@Create\Views\Inv_VW_Grid_Location_Lite.sql
@@Create\Views\Inv_VW_Plate_Locations_All.sql
@@Create\Views\Inv_VW_Audit_Column_Disp.sql
@@Create\Views\Inv_VW_Audit_Aggregate.sql;
@@Create\Views\INV_VW_Reg_Structures.sql
@@Create\Views\INV_VW_Reg_Batches.sql
@@Create\Views\INV_VW_Reg_AltIDs.sql


