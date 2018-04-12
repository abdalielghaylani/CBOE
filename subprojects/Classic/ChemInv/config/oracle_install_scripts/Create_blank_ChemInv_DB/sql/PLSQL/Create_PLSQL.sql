-- Copyright Cambridgesoft Corp 2001-2006 all rights reserved

prompt Starting Create_PLSQL.sql

--#########################################################
-- Create tablespaces
--######################################################### 

Connect &&schemaName/&&schemaPass@&&serverName

prompt '#########################################################'
prompt 'Creating Referenced Packages...'
prompt '#########################################################'

@@PLSQL\Packages\pkg_Stringutils_Def.sql;
@@PLSQL\Packages\pkg_Stringutils_Body.sql;
@@PLSQL\Packages\pkg_Audit_trail_def.sql;
@@PLSQL\Packages\pkg_Audit_trail_Body.sql;
@@PLSQL\Packages\pkg_Chemcalcs_Def.sql;
@@PLSQL\Packages\pkg_Chemcalcs_Body.sql;

@@PLSQL\Packages\pkg_Racks_def.sql;
@@PLSQL\Packages\pkg_Reservations_Def.sql;
@@PLSQL\Packages\pkg_Guiutils_Def.sql;
@@PLSQL\Packages\pkg_Racks_Body.sql;
@@PLSQL\Packages\pkg_Reservations_Body.sql;
@@PLSQL\Packages\pkg_Guiutils_Body.sql;

@@PLSQL\Packages\pkg_Barcodes_Def.sql;
@@PLSQL\Packages\pkg_Barcodes_Body.sql;
@@PLSQL\Packages\pkg_Xmlutils_Def.sql;
@@PLSQL\Packages\pkg_Xmlutils_Body.sql;

@@PLSQL\Packages\pkg_Platechem_Def.sql;
@@PLSQL\Packages\pkg_Reformat_Def.sql;

prompt '#########################################################'
prompt 'Creating Referenced Functions...'
prompt '#########################################################'

@@PLSQL\Functions\f_CreateRegCompound.sql
@@PLSQL\Functions\f_SolvatePlates.sql
@@PLSQL\Functions\f_GetLocationPath.sql
@@PLSQL\Functions\f_IsContainerTypeAllowed.sql
@@PLSQL\Functions\f_IsGridLocation.sql
@@PLSQL\Procedures\proc_UpdateBatchStatus.sql
@@PLSQL\Functions\f_CreatePlateXML.sql

prompt '#########################################################'
prompt 'Creating Packages...'
prompt '#########################################################'

@@PLSQL\Packages\pkg_Platechem_Body.sql;
@@PLSQL\Packages\pkg_Approvals_Def.sql
@@PLSQL\Packages\pkg_Approvals_Body.sql
@@PLSQL\Packages\pkg_Docs_def.sql;
@@PLSQL\Packages\pkg_Docs_Body.sql;
@@PLSQL\Packages\pkg_Requests_Def.sql;
@@PLSQL\Packages\pkg_Requests_Body.sql;
@@PLSQL\Packages\pkg_Compounds_Def.sql;
@@PLSQL\Packages\pkg_Compounds_Body.sql;
@@PLSQL\Packages\pkg_Datamaps_Def.sql
@@PLSQL\Packages\pkg_Datamaps_Body.sql
@@PLSQL\Packages\pkg_Links_Def.sql;
@@PLSQL\Packages\pkg_Links_Body.sql;
@@PLSQL\Packages\pkg_Reformat_Body.sql;
@@PLSQL\Packages\pkg_Platesettings_Def.sql;
@@PLSQL\Packages\pkg_Platesettings_Body.sql;
@@PLSQL\Packages\pkg_Reportparams_Def.sql
@@PLSQL\Packages\pkg_Reportparams_Body.sql
@@PLSQL\Packages\pkg_Reports_Def.sql
@@PLSQL\Packages\pkg_Reports_Body.sql
@@PLSQL\Packages\pkg_Organization_def.sql;
@@PLSQL\Packages\pkg_Organization_Body.sql;
--' this procedure is used in the Batch package
@@PLSQL\Procedures\proc_UpdateContainerBatches.sql
@@PLSQL\Packages\pkg_Batch_def.sql;
@@PLSQL\Packages\pkg_Batch_Body.sql;
--@@PLSQL\Packages\pkg_CTX_Cheminv_Mgr_def.SQL;
--@@PLSQL\Packages\pkg_CTX_Cheminv_Mgr_Body.SQL;


prompt '#########################################################'
prompt 'Creating functions...'
prompt '#########################################################'

@@PLSQL\Functions\f_ExcludeContainerTypes.sql
@@PLSQL\Functions\f_GetCompoundID.sql
@@PLSQL\Functions\f_IsDuplicateCompound.sql
@@PLSQL\Functions\f_UpdateContainerStatus.sql
@@PLSQL\Functions\f_EnableGridForLocation.sql
@@PLSQL\Functions\f_CheckoutContainer.sql
@@PLSQL\Functions\f_AssignPlateTypesToLocation.sql
@@PLSQL\Functions\f_RetireContainer.sql
@@PLSQL\Functions\f_CreateContainer.sql
@@PLSQL\Functions\f_CopyContainer.sql
@@PLSQL\Functions\f_CopyPlate.sql
@@PLSQL\Functions\f_UpdateContainer.sql
@@PLSQL\Functions\f_CreateLocation.sql
@@PLSQL\Functions\f_DeleteContainer.sql
@@PLSQL\Functions\f_GetGridID.sql
@@PLSQL\Functions\f_GetGridFormatID.sql
@@PLSQL\Functions\f_DeleteLocationGrid.sql
@@PLSQL\Functions\f_DeleteLocation.sql
@@PLSQL\Functions\f_GetCompoundIDFromName.sql
@@PLSQL\Functions\f_MoveContainer.sql
@@PLSQL\Functions\f_MoveLocation.sql
@@PLSQL\Functions\f_SubstanceNameExists.sql
@@PLSQL\Functions\f_UpdateAllContainerFields.sql
@@PLSQL\Functions\f_UpdateContainerQtyRemaining.sql
@@PLSQL\Functions\f_GetGridStorageID.sql
@@PLSQL\Functions\f_UpdateLocation.sql
@@PLSQL\Functions\f_OrderContainer.sql
@@PLSQL\Functions\f_ReorderContainer.sql
@@PLSQL\Functions\f_SelectHazmatData.sql
@@PLSQL\Functions\f_SelectSubstanceHazmatData.sql
@@PLSQL\Functions\f_CreateTableRow.SQL
@@PLSQL\Functions\f_DeleteTableRow.sql
@@PLSQL\Functions\f_DeletePlates.sql
@@PLSQL\Functions\f_GetPKColumn.sql
@@PLSQL\Functions\f_IsFrozenLocation.sql
@@PLSQL\Functions\f_SetPlateFTCycle.sql
@@PLSQL\Functions\f_MovePlates.sql
@@PLSQL\Functions\f_RetirePlate.sql
@@PLSQL\Functions\f_UpdatePlateAttributes.sql
@@PLSQL\Functions\f_UpdateTable.sql
@@PLSQL\Functions\f_UpdateWell.sql

--@@Procedures\proc_InsertXMLDoc.sql
--@@Procedures\proc_InsertXSTL.sql
@@PLSQL\Functions\f_UpdateRegCompound.sql
@@PLSQL\Functions\f_CreatePlateMap.sql
@@PLSQL\Functions\f_IsPlateTypeAllowed.sql   
@@PLSQL\Functions\f_IsTrashLocation.sql
@@PLSQL\Functions\f_EmptyTrash.sql
@@PLSQL\Functions\f_LookUpValue.sql
@@PLSQL\Functions\f_UpdateAddress.sql
@@PLSQL\Functions\f_CertifyContainer.sql
@@PLSQL\Functions\f_InsertCheckInDetails.sql
@@PLSQL\Functions\f_CheckInDefaultLocation.SQL

-- procedures from Alter_Cheminv_Plate_Procs.sql

@@PLSQL\Functions\f_CreateGridFormat.sql
@@PLSQL\Functions\f_CreatePhysPlateType.sql
@@PLSQL\Functions\f_DeletePhysPlateType.sql
@@PLSQL\Functions\f_UpdatePhysPlateType.sql
@@PLSQL\Functions\f_UpdateWellFormat.sql
@@PLSQL\Functions\f_DeletePlateFormat.sql
@@PLSQL\Functions\f_UpdatePlateFormat.sql
@@PLSQL\Functions\f_CreatePlateFormat.sql
@@PLSQL\Functions\f_DeletePlate.sql
@@PLSQL\Functions\f_AssignLocationToGrid.sql
@@PLSQL\Functions\f_GetNumberOfCompoundWells.sql
@@PLSQL\Functions\f_MovePlate.sql
@@PLSQL\Functions\f_GetPrimaryKeyIDs.sql
@@PLSQL\Functions\f_CheckOpenPositions.sql
prompt '#########################################################'
prompt 'Creating procedures...'
prompt '#########################################################'

@@PLSQL\Procedures\proc_InsertIntoCustomChemOrder.sql
@@PLSQL\Procedures\proc_InsertHazmatData.sql
@@PLSQL\Procedures\proc_UpdateHazmatData.sql
@@PLSQL\Procedures\proc_InsertSubstanceHazmatData.sql
@@PLSQL\Procedures\proc_UpdateSubstanceHazmatData.sql
