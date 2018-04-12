-- Copyright Cambridgesoft Corp 2001-2006 all rights reserved

--#########################################################
-- Insert Inventory application data
--######################################################### 

Connect &&schemaName/&&schemaPass@&&serverName

prompt '#########################################################'
prompt 'Inserting application data...'
prompt '#########################################################'

alter table inv_locations disable constraint INV_LOC_LOC_FK;
@@Data\TableData\Globals.sql
@@Data\TableData\Db_FormGroup.sql
@@Data\TableData\Inv_Graphic_Types.sql
@@Data\TableData\Inv_Graphics.sql
@@Data\TableData\Inv_Location_Types.sql
@@Data\TableData\Inv_Locations.sql
@@Data\TableData\Inv_Compounds.sql
@@Data\TableData\Inv_API_Errors.sql
@@Data\TableData\Inv_Barcode_Desc.sql
@@Data\TableData\Inv_Batch_Status.sql
@@Data\TableData\Inv_Container_Order_Reason.sql
@@Data\TableData\Inv_Container_Status.sql
@@Data\TableData\Inv_Container_Types.sql
@@Data\TableData\Inv_Country.sql
@@Data\TableData\Inv_Eset_Type.sql
@@Data\TableData\Inv_Enumeration_Set.sql
@@Data\TableData\Inv_Enumeration.sql
@@Data\TableData\Inv_Data_Maps.sql
@@Data\TableData\Inv_Map_Fields.sql
@@Data\TableData\Inv_Data_Mappings.sql
@@Data\TableData\Inv_Order_Status.sql
@@Data\TableData\Inv_Owners.sql
@@Data\TableData\Inv_Plate_Actions.sql
@@Data\TableData\Inv_Plate_Types.sql
@@Data\TableData\Inv_Project_Job_Info.sql
@@Data\TableData\Inv_ReportFormats.sql
@@Data\TableData\Inv_ReportTypes.sql
@@Data\TableData\Inv_Request_Status.sql
@@Data\TableData\Inv_Request_Types.sql
@@Data\TableData\Inv_Reservation_Types.sql
@@Data\TableData\Inv_States.sql
@@Data\TableData\Inv_Suppliers.sql
@@Data\TableData\Inv_Unit_Types.sql
@@Data\TableData\Inv_Units.sql
@@Data\TableData\Inv_Grid_Format.sql
@@Data\TableData\Inv_Grid_Position.sql
@@Data\TableData\Inv_Physical_Plate.sql
@@Data\TableData\Inv_Plate_Format.sql
@@Data\TableData\Inv_Wells.sql
@@Data\TableData\Inv_Unit_Conversion.sql
@@Data\TableData\Inv_XmlDoc_Types.sql
@@Data\TableData\Inv_Doc_Types.sql
@@Data\TableData\Inv_Reports.sql
@@Data\TableData\Inv_Xmldocs.sql
@@Data\TableData\Inv_Xslts.sql
@@Data\TableData\Inv_Picklist_Types.sql
@@Data\TableData\Inv_Picklists.sql
alter table inv_locations enable constraint INV_LOC_LOC_FK;

-- Oracle 10.2.0.3 throws an error while importing into an existing table with lobs
-- Drop the tables and let imp recreate them
--drop table inv_xmldocs;
--drop table inv_xslts;
--drop table inv_reports;

--' use dump file for inv_reports, inv_xmldocs, inv_xslts because they have clob fields
--host imp &&schemaName/&&schemaPass@&&serverName file=Data\TableData\Clob_Table_Data.dmp ignore=yes full=yes log=Logs\LOG_Load_ChemInvDB_AppData.txt


				 						 						 


      
