-- Copyright Cambridgesoft Corp 2001-2006 all rights reserved

--#########################################################
-- Create triggers
--######################################################### 

Connect &&schemaName/&&schemaPass@&&serverName

prompt '#########################################################'
prompt 'Creating triggers...'
prompt '#########################################################'


@@PLSQL\Triggers\Inv_Locations_Bi0.trg;
@@PLSQL\Triggers\Inv_Locations_Au0.trg;
@@PLSQL\Triggers\Inv_Locations_Ad0.trg;
@@PLSQL\Triggers\Inv_Containers_Bi0.trg;
@@PLSQL\Triggers\Inv_Containers_Au0.trg;
@@PLSQL\Triggers\Inv_Containers_Au1.trg;
@@PLSQL\Triggers\Inv_Containers_Ad0.trg;
@@PLSQL\Triggers\trg_inv_containers_family_bu.sql
@@PLSQL\Triggers\Inv_Compounds_Bi0.trg;
@@PLSQL\Triggers\Inv_Compounds_Au0.trg;
@@PLSQL\Triggers\Inv_Compounds_Ad0.trg;
@@PLSQL\Triggers\Inv_Url_Bi0.trg;
@@PLSQL\Triggers\Inv_Url_Au0.trg;
@@PLSQL\Triggers\Inv_Url_Ad0.trg;
@@PLSQL\Triggers\Inv_Requests_Bi0.trg;
@@PLSQL\Triggers\Inv_Requests_Au0.trg;
@@PLSQL\Triggers\Inv_Requests_Ad0.trg;
@@PLSQL\Triggers\Trg_Inv_Reports_Id.sql;
@@PLSQL\Triggers\Inv_Suppliers_Bi0.trg;
@@PLSQL\Triggers\Inv_Suppliers_Au0.trg;
@@PLSQL\Triggers\Inv_Suppliers_Ad0.trg;
@@PLSQL\Triggers\Inv_Orders_Bi0.trg;
@@PLSQL\Triggers\Inv_Orders_Au0.trg;
@@PLSQL\Triggers\Inv_Orders_Ad0.trg;
@@PLSQL\Triggers\Inv_Order_Containers_Bi0.trg;
@@PLSQL\Triggers\Inv_Order_Containers_Au0.trg;
@@PLSQL\Triggers\Inv_Order_Containers_Ad0.trg;
@@PLSQL\Triggers\Inv_Container_Checkin_Details_Bi0.trg;
@@PLSQL\Triggers\Inv_Container_Checkin_Details_Au0.trg;
@@PLSQL\Triggers\Inv_Container_Checkin_Details_Ad0.trg;
@@PLSQL\Triggers\Inv_Synonyms_Bi0.trg
@@PLSQL\Triggers\Inv_Synonyms_Au0.trg
@@PLSQL\Triggers\Inv_Synonyms_Ad0.trg
@@PLSQL\Triggers\trg_inv_plates_solution_calc.sql
@@PLSQL\Triggers\trg_inv_wells_cmpds_molar_calcs.sql
@@PLSQL\Triggers\trg_inv_wells_molar_calcs.sql
