-- Create procedure/function UPDATEALLCONTAINERFIELDS.

CREATE OR REPLACE FUNCTION "UPDATEALLCONTAINERFIELDS"
(pContainerID in Inv_Containers.Container_ID%Type,
  pBarcode in Inv_Containers.Barcode%Type,
  pLocationID in Inv_Containers.Location_ID_FK%Type,
  pUOMID in Inv_Containers.Unit_Of_Meas_ID_FK%Type,
  pContainerTypeID in Inv_Containers.Container_Type_ID_FK%Type,
  pContainerStatusID in Inv_Containers.Container_Status_ID_FK%Type:=1,
  pRegID in Inv_Containers.Reg_ID_FK%Type,
  pBatchNumber in Inv_Containers.Batch_Number_FK%Type,
  pMaxQty in Inv_Containers.Qty_Max%Type,
  pQtyRemaining in Inv_Containers.Qty_Remaining%Type,
  pMinStockQty in Inv_Containers.Qty_MinStock%Type,
  pMaxStockQty in Inv_Containers.Qty_MaxStock%Type,
  pExpDate in Inv_Containers.Date_Expires%Type,
  pCompoundID in Inv_Containers.Compound_ID_FK%Type,
  pContainerName in Inv_Containers.Container_Name%Type,
  pContainerDesc in Inv_Containers.Container_Description%Type,
  pTareWeight in Inv_Containers.Tare_Weight%Type,
  pNetWeight in Inv_Containers.Net_Wght%Type,
  pFinalWeight in Inv_Containers.Final_Wght%Type,
  pUOWID in Inv_Containers.Unit_of_WGHT_ID_FK%Type,
  pPurity in Inv_Containers.Purity%Type,
  pUOPID in Inv_Containers.Unit_of_Purity_ID_FK%Type,
  pConcentration in Inv_Containers.Concentration%Type,
  pDensity in Inv_Containers.Density%Type,
  pUOCID in Inv_Containers.Unit_of_CONC_ID_FK%Type,
  pUODID in Inv_Containers.Unit_of_Density_ID_FK%Type,
  pSolventIDFK in Inv_Containers.Solvent_ID_FK%Type,
  pGrade in Inv_Containers.Grade%Type,
  pComments in Inv_Containers.Container_Comments%Type,
  pStorageConditions IN inv_containers.storage_conditions%TYPE,
  pHandlingProcedures IN inv_containers.handling_procedures%TYPE,
  pSupplierID in Inv_Containers.Supplier_ID_FK%Type,
  pSupplierCatNum in Inv_Containers.Supplier_CatNum%Type,
  pLotNum in Inv_Containers.Lot_Num%Type,
  pDateProduced in Inv_Containers.Date_Produced%Type,
  pDateOrdered in Inv_Containers.Date_Ordered%Type,
  pDateReceived in Inv_Containers.Date_Received%Type,
  pDateCertified in Inv_Containers.Date_Certified%Type,
  pDateApproved in Inv_Containers.Date_Approved%Type,
  pContainerCost in Inv_Containers.Container_Cost%Type,
  pPONumber in Inv_Containers.PO_Number%Type,
  pPOLineNumber in Inv_Containers.PO_Line_Number%Type,
  pReqNumber in Inv_Containers.Req_Number%Type,
  pOwnerID in Inv_Containers.Owner_ID_FK%Type,
  pCurrentUserID in Inv_Containers.Current_User_ID_FK%Type,
  pField_1 in inv_containers.Field_1%Type,
  pField_2 in inv_containers.Field_2%Type,
  pField_3 in inv_containers.Field_3%Type,
  pField_4 in inv_containers.Field_4%Type,
  pField_5 in inv_containers.Field_5%Type,
  pField_6 in inv_containers.Field_6%Type,
  pField_7 in inv_containers.Field_7%Type,
  pField_8 in inv_containers.Field_8%Type,
  pField_9 in inv_containers.Field_9%Type,
  pField_10 in inv_containers.Field_10%Type,
  pDate_1 in inv_containers.Date_1%Type,
  pDate_2 in inv_containers.Date_2%Type,
  pDate_3 in inv_containers.Date_3%Type,
  pDate_4 in inv_containers.Date_4%Type,
  pDate_5 in inv_containers.Date_5%Type
  )
RETURN Inv_Containers.Location_ID_FK%Type is
duplicate_barcode exception;
container_type_not_allowed exception;
CURSOR dupBarcode_cur(Barcode_in in Inv_Containers.Barcode%Type) IS
  SELECT Container_ID FROM Inv_Containers WHERE inv_Containers.Barcode = Barcode_in;
dupBarcode_id Inv_Containers.Container_ID%Type;
BEGIN
if is_container_type_allowed(pContainerID, pLocationID) = 0 then
  RAISE container_type_not_allowed;
end if;
-- Check for duplicate barcode
if pBarcode is not null then
  OPEN dupBarcode_cur(pBarcode);
  FETCH dupBarcode_cur into dupBarcode_id;
  if (dupBarcode_cur%FOUND) AND (NOT dupBarcode_id = pContainerID) then
    RAISE duplicate_barcode;
  end if;
  CLOSE dupBarcode_cur;
end if;

UPDATE Inv_Containers
SET
  Container_Status_ID_FK = pContainerStatusID,
  Barcode = pBarcode,
  Location_ID_FK = pLocationID,
  UNIT_OF_MEAS_ID_FK = pUOMID,
  Container_Type_ID_FK = pContainerTypeID,
  Reg_ID_FK = pRegID,
  Batch_Number_FK = pBatchNumber,
  Qty_Max = pMaxQty,
  Qty_Remaining = pQtyRemaining,
  Qty_MinStock = pMinStockQty,
  Qty_MaxStock = pMaxStockQty,
  Date_Expires = pExpDate,
  Compound_ID_FK = pCompoundID,
  Container_Name = pContainerName,
  Container_Description = pContainerDesc,
  Tare_Weight = pTareWeight,
  Net_Wght = pNetWeight,
  Final_Wght = pFinalWeight,
  Unit_of_WGHT_ID_FK = pUOWID,
  Purity = pPurity,
  Unit_of_Purity_ID_FK = pUOPID,
  Concentration = pConcentration,
  Density = pDensity,
  Unit_of_CONC_ID_FK = pUOCID,
  Unit_of_Density_ID_FK = pUODID,
  Solvent_ID_FK = pSolventIDFK,
  Grade = pGrade,
  Container_Comments = pComments,
  Storage_Conditions = pStorageConditions,
  Handling_Procedures = pHandlingProcedures,
  Lot_Num = pLotNum,
  Supplier_CatNum = pSupplierCatNum,
  Supplier_ID_FK = pSupplierID,
  Date_Produced =  pDateProduced,
  Date_Ordered =  pDateOrdered,
  Date_Received =  pDateReceived,
  Date_Certified = pDateCertified,
  Date_Approved = pDateApproved,
  Container_Cost = pContainerCost,
  PO_Number = pPONumber,
  PO_Line_Number = pPOLineNumber,
  Req_Number = pReqNumber,
  Owner_ID_FK = pOwnerID,
  Current_User_ID_FK = pCurrentUserID,
  Field_1 = pField_1,
  Field_2 = pField_2,
  Field_3 = pField_3,
  Field_4 = pField_4,
  Field_5 = pField_5,
  Field_6 = pField_6,
  Field_7 = pField_7,
  Field_8 = pField_8,
  Field_9 = pField_9,
  Field_10 = pField_10,
  Date_1 = pDate_1,
  Date_2 = pDate_2,
  Date_3 = pDate_3,
  Date_4 = pDate_4,
  Date_5 = pDate_5
WHERE
  Container_ID = pContainerID;
  if sql%rowcount = 1 then
    Reservations.ReconcileQtyAvailable(pContainerID);
    UpdateContainerBatches(pContainerID);
    RETURN pContainerID;
  Else
    --RETURN 'Container to update could not be found';
    RETURN -119;
  End if;

UpdateContainerBatches(pContainerID);

exception
WHEN duplicate_barcode then
	--RETURN 'A container with same barcode ID already exists'
  RETURN -102;
WHEN container_type_not_allowed then
  RETURN -128;
END UpdateAllContainerFields;


/
show errors;
