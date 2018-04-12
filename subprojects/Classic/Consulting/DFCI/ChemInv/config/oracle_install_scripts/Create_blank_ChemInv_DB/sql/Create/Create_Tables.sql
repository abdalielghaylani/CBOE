-- Copyright Cambridgesoft Corp 2001-2006 all rights reserved

--#########################################################
-- Create schema tables
--######################################################### 

Connect &&schemaName/&&schemaPass@&&serverName

prompt '#########################################################'
prompt 'Creating schema tables...'
prompt '#########################################################'


--' Create Global temporary tables used by stored procedures
@@Create\Tables\TempIDs.sql;
@@Create\Tables\StoredProcedureErrors.sql;
@@Create\Tables\TempBarcodes.sql;
@@Create\Tables\Clobs.sql;
@@Create\Tables\Db_FormGroup.sql
@@Create\Tables\Globals.sql

@@Create\Tables\Audit_Row.sql;
@@Create\Tables\Audit_Column.sql;
@@Create\Tables\Audit_Delete.sql;
@@Create\Tables\Audit_Compound.sql;
@@Create\Tables\Inv_Api_Errors.sql;
@@Create\Tables\Inv_Unit_Types.sql;
@@Create\Tables\Inv_Picklist_Types.sql;
@@Create\Tables\Inv_PickLists.sql;
@@Create\Tables\Inv_Units.sql;
@@Create\Tables\Inv_Owners.sql;
@@Create\Tables\Inv_Physical_State.sql;
@@Create\Tables\Inv_Country.sql;
@@Create\Tables\Inv_States.sql;
@@Create\Tables\Inv_Address.sql;
@@Create\Tables\Inv_Suppliers.sql;
@@Create\Tables\Inv_Graphic_Types.sql
@@Create\Tables\Inv_Graphics.sql
@@Create\Tables\Inv_Location_Types.sql;
@@Create\Tables\Inv_Locations.sql;
@@Create\Tables\Inv_Container_Types.sql;
@@Create\Tables\Inv_Container_Status.sql;
@@Create\Tables\Inv_Compounds.sql
@@Create\Tables\Inv_Solvents.sql
@@Create\Tables\Inv_Synonyms.sql
@@Create\Tables\Inv_Container_Batch_Fields.sql
@@Create\Tables\Inv_Batch_Status.sql
@@Create\Tables\Inv_Container_Batches.sql
@@Create\Tables\Inv_Containers.sql
@@Create\Tables\Inv_Container_Checkin_Details.sql
@@Create\Tables\Inv_Reservation_Types.sql
@@Create\Tables\Inv_Reservations.sql
@@Create\Tables\Inv_Exclude_Container_Types.sql
@@Create\Tables\Inv_Url.sql
@@Create\Tables\Inv_Allowed_Ctypes.sql
@@Create\Tables\Inv_Plate_Types.sql
@@Create\Tables\Inv_Allowed_Ltypes.sql
@@Create\Tables\Inv_Allowed_Ptypes.sql
@@Create\Tables\Inv_Barcode_Desc.sql
@@Create\Tables\Inv_Eset_Type.sql
@@Create\Tables\Inv_Enumeration_Set.sql
@@Create\Tables\Inv_Enumeration.sql
@@Create\Tables\Inv_Enum_Values.sql
@@Create\Tables\Inv_Grid_Format.sql
@@Create\Tables\Inv_Grid_Fill_Template.sql
@@Create\Tables\Inv_Grid_Position.sql
@@Create\Tables\Inv_Physical_Plate.sql
@@Create\Tables\Inv_Plate_Format.sql
@@Create\Tables\Inv_Plates.sql
@@Create\Tables\Inv_Grid_Storage.sql
@@Create\Tables\Inv_Plate_Actions.sql
@@Create\Tables\Inv_Plate_History.sql
@@Create\Tables\Inv_Wells.sql
@@Create\Tables\Inv_Well_Compounds.sql
@@Create\Tables\Inv_Grid_Element.sql
@@Create\Tables\Inv_Request_Types.sql
@@Create\Tables\Inv_Request_Status.sql
@@Create\Tables\Inv_Requests.sql
@@Create\Tables\Inv_Request_Samples.sql
@@Create\Tables\Inv_Order_Status.sql
@@Create\Tables\Inv_Orders.sql
@@Create\Tables\Inv_Order_Containers.sql
@@Create\Tables\Inv_ReportTypes.sql
@@Create\Tables\Inv_Reports.sql
@@Create\Tables\Inv_ReportFormats.sql
@@Create\Tables\Inv_ReportParams.sql
@@Create\Tables\Inv_User_Properties.sql
@@Create\Tables\Inv_EHS_CAS_Substance.sql
@@Create\Tables\Inv_EHS_CatNum_Substance.sql
@@Create\Tables\Inv_EHS_Substances.sql
@@Create\Tables\Inv_Unit_Conversion.sql
@@Create\Tables\Inv_Container_Order.sql
@@Create\Tables\Inv_Container_Order_Reason.sql
@@Create\Tables\Inv_Project_Job_Info.sql
@@Create\Tables\Inv_Xslts.sql
@@Create\Tables\Inv_XmlDoc_Types.sql
@@Create\Tables\Inv_XmlDocs.sql
@@Create\Tables\Inv_Plate_Parent.sql
@@Create\Tables\Inv_Well_Parent.sql
@@Create\Tables\Inv_Data_Maps.sql
@@Create\Tables\Inv_Map_Fields.sql
@@Create\Tables\Inv_Data_Mappings.sql
@@Create\Tables\Inv_Doc_Types.sql
@@Create\Tables\Inv_Docs.sql
@@Create\Tables\Inv_Unit_Conversion_Formula.sql
@@Create\Tables\Inv_Org_Unit.sql
@@Create\Tables\Inv_Org_Roles.sql
@@Create\Tables\Inv_Org_Users.sql
@@Create\Tables\Inv_Custom_Field_Groups.sql
@@Create\Tables\Inv_Custom_Fields.sql
@@Create\Tables\Inv_Custom_Cpd_Field_Values.sql

@@Create\Tables\Custom_Chem_Order.sql
@@Create\Tables\Custom_ACX_ST_Vendors.sql

