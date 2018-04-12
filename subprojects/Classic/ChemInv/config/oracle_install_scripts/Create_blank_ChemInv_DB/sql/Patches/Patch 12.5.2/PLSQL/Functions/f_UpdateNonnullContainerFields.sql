-- Create procedure/function UPDATENONNULLCONTAINERFIELDS.
CREATE OR REPLACE  FUNCTION "&&SchemaName"."UPDATENONNULLCONTAINERFIELDS"                            
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
  pUOCostID IN inv_containers.unit_of_cost_id_fk%TYPE,
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
  pDate_5 in inv_containers.Date_5%Type,
  pPrincipalID IN inv_containers.PRINCIPAL_ID_FK%TYPE:=Null,
  PLocationTypeID IN inv_containers.Location_Type_ID_FK%TYPE:=Null
  )
RETURN Inv_Containers.Location_ID_FK%Type is
duplicate_barcode exception;
container_type_not_allowed exception;
CURSOR dupBarcode_cur(Barcode_in in Inv_Containers.Barcode%Type) IS
  SELECT Container_ID FROM Inv_Containers WHERE inv_Containers.Barcode = Barcode_in;
dupBarcode_id Inv_Containers.Container_ID%Type;
l_compound_id inv_compounds.compound_id%TYPE;
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

-- Get the new compound_id
if pRegID is not null and pBatchNumber is not null then
	l_compound_id := CREATEREGCOMPOUND( pRegID, pBatchNumber );
else
	l_compound_id := NVL(pCompoundID,l_compound_id);
end if;

UPDATE Inv_Containers
SET
  Container_Status_ID_FK = NVL(pContainerStatusID,Container_Status_ID_FK),
  Barcode = NVL(pBarcode,Barcode),
  Location_ID_FK = NVL(pLocationID,Location_ID_FK),
  UNIT_OF_MEAS_ID_FK = NVL(pUOMID,UNIT_OF_MEAS_ID_FK),
  Container_Type_ID_FK = NVL(pContainerTypeID,Container_Type_ID_FK),
  Reg_ID_FK = NVL(pRegID,Reg_ID_FK),
  Batch_Number_FK = NVL(pBatchNumber,Batch_Number_FK),
  Qty_Max = NVL(pMaxQty,Qty_Max),
  Qty_Remaining = NVL(pQtyRemaining,Qty_Remaining),
  Qty_MinStock = NVL(pMinStockQty,Qty_MinStock),
  Qty_MaxStock = NVL(pMaxStockQty,Qty_MaxStock),
  Date_Expires = NVL(pExpDate,Date_Expires),
  Compound_ID_FK = NVL(l_compound_id,Compound_ID_FK),
  Container_Name = NVL(pContainerName,Container_Name),
  Container_Description = NVL(pContainerDesc,Container_Description),
  Tare_Weight = NVL(pTareWeight,Tare_Weight),
  Net_Wght = NVL(pNetWeight,Net_Wght),
  Final_Wght = NVL(pFinalWeight,Final_Wght),
  Unit_of_WGHT_ID_FK = NVL(pUOWID,Unit_of_WGHT_ID_FK),
  Purity = NVL(pPurity,Purity),
  Unit_of_Purity_ID_FK = NVL(pUOPID,Unit_of_Purity_ID_FK),
  Concentration = NVL(pConcentration,Concentration),
  Density = NVL(pDensity,Density),
  Unit_of_CONC_ID_FK = NVL(pUOCID,Unit_of_CONC_ID_FK),
  Unit_of_Density_ID_FK = NVL(pUODID,Unit_of_Density_ID_FK),
  Solvent_ID_FK = NVL(pSolventIDFK,Solvent_ID_FK),
  Grade = NVL(pGrade,Grade),
  Container_Comments = NVL(pComments,Container_Comments),
  Storage_Conditions = NVL(pStorageConditions,Storage_Conditions),
  Handling_Procedures = NVL(pHandlingProcedures,Handling_Procedures),
  Lot_Num = NVL(pLotNum,Lot_Num),
  Supplier_CatNum = NVL(pSupplierCatNum,Supplier_CatNum),
  Supplier_ID_FK = NVL(pSupplierID,Supplier_ID_FK),
  Date_Produced = NVL( pDateProduced,Date_Produced),
  Date_Ordered = NVL( pDateOrdered,Date_Ordered),
  Date_Received = NVL( pDateReceived,Date_Received),
  Date_Certified = NVL(pDateCertified,Date_Certified),
  Date_Approved = NVL(pDateApproved,Date_Approved),
  Container_Cost = NVL(pContainerCost,Container_Cost),
  Unit_Of_Cost_ID_FK = NVL(pUOCostID,Unit_Of_Cost_ID_FK),
  PO_Number = NVL(pPONumber,PO_Number),
  PO_Line_Number = NVL(pPOLineNumber,PO_Line_Number),
  Req_Number = NVL(pReqNumber,Req_Number),
  Owner_ID_FK = NVL(pOwnerID,Owner_ID_FK),
  Current_User_ID_FK = NVL(pCurrentUserID,Current_User_ID_FK),
  Field_1 = NVL(pField_1,Field_1),
  Field_2 = NVL(pField_2,Field_2),
  Field_3 = NVL(pField_3,Field_3),
  Field_4 = NVL(pField_4,Field_4),
  Field_5 = NVL(pField_5,Field_5),
  Field_6 = NVL(pField_6,Field_6),
  Field_7 = NVL(pField_7,Field_7),
  Field_8 = NVL(pField_8,Field_8),
  Field_9 = NVL(pField_9,Field_9),
  Field_10 = NVL(pField_10,Field_10),
  Date_1 = NVL(pDate_1,Date_1),
  Date_2 = NVL(pDate_2,Date_2),
  Date_3 = NVL(pDate_3,Date_3),
  Date_4 = NVL(pDate_4,Date_4),
  Date_5 = NVL(pDate_5,Date_5),
  PRINCIPAL_ID_FK = NVL(pPrincipalID,PRINCIPAL_ID_FK),
  Location_Type_ID_FK = NVL(PLocationTypeID,Location_Type_ID_FK)
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

exception
WHEN duplicate_barcode then
	--RETURN 'A container with same barcode ID already exists'
  RETURN -102;
WHEN container_type_not_allowed then
  RETURN -128;
END UpdateNonnullContainerFields;
/

show errors;
