-- Create procedure/function CREATECONTAINER.
CREATE OR REPLACE  FUNCTION "&&SchemaName"."CREATECONTAINER"                                 
(
  pBarcode in Inv_Containers.Barcode%Type,
  pBarcodeDescID in Inv_Barcode_Desc.Barcode_Desc_ID%TYPE,
  pLocationID in Inv_Containers.Location_ID_FK%Type,
  pUOMID in Inv_Containers.Unit_Of_Meas_ID_FK%Type,
  pContainerTypeID in Inv_Containers.Container_Type_ID_FK%Type,
  pContainerStatusID in Inv_Containers.Container_Status_ID_FK%Type:=1,
  pMaxQty in Inv_Containers.Qty_Max%Type,
  pRegID in Inv_Containers.Reg_ID_FK%Type,
  pBatchNumber in Inv_Containers.Batch_Number_FK%Type,
  pInitialQty in Inv_Containers.Qty_Initial%Type,
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
  pUODID in Inv_Containers.Unit_of_DENSITY_ID_FK%Type,
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
  pContainerCost in Inv_Containers.Container_Cost%Type,
  pUOCostID IN inv_containers.unit_of_cost_id_fk%TYPE,
  pPONumber in Inv_Containers.PO_Number%Type,
  pPOLineNumber in Inv_Containers.PO_Line_Number%Type,
  pReqNumber in Inv_Containers.Req_Number%Type,
  pOwnerID in Inv_Containers.Owner_ID_FK%Type,
  pCurrentUserID in Inv_Containers.Current_User_ID_FK%Type,
  pNumCopies in int:=1,
  pNewIDs out varchar2,
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

return Inv_Containers.Container_ID%Type
is
NewContainerID Inv_Containers.Container_ID%Type;
duplicate_barcode exception;
excess_contents exception;
container_type_not_allowed exception;
CURSOR dupBarcode_cur(Barcode_in in Inv_Containers.Barcode%Type) IS
  SELECT Container_ID FROM Inv_Containers WHERE inv_Containers.Barcode = Barcode_in;
dupBarcode_id Inv_Containers.Container_ID%Type;
v_barcode_i int;
v_barcode_c Inv_Containers.Barcode%Type;
v_barcodedescid Inv_Barcode_Desc.Barcode_Desc_ID%TYPE;
BEGIN

if is_container_type_allowed(NULL, pLocationID) = 0 then
  RAISE container_type_not_allowed;
end if;
if pInitialQty > pMaxQty then
  RAISE excess_contents;
end if;

-- Loop over pNumCopies
FOR v_Counter IN 1..pNumCopies LOOP

/*
  if pNumCopies > 1 then
    v_barcode_i := TO_NUMBER(pBarcode) + (v_Counter -1);
    v_barcode_c := TO_CHAR(v_barcode_i);
  else
    v_barcode_c := pBarcode;
  end if;

  -- Check for duplicate barcode
  if v_barcode_c is not null then
    OPEN dupBarcode_cur(v_barcode_c);
    FETCH dupBarcode_cur into dupBarcode_id;
    if dupBarcode_cur%ROWCOUNT = 1 then
      RAISE duplicate_barcode;
    end if;
    CLOSE dupBarcode_cur;
  end if;
*/
  IF pBarcode IS NOT NULL THEN
	v_barcode_c := pBarcode;
  ELSE
  	IF pBarcodeDescID IS NULL THEN
		v_barcodedescid := constants.cContainerBarcodeDesc;
  	ELSE
  		v_barcodedescid := pBarcodeDescID;
  	END IF;
	v_barcode_c := barcodes.GetNextBarcode(v_barcodedescid);
  END IF;

  INSERT INTO Inv_Containers (
    Container_Status_ID_FK,
    Barcode,
    Location_ID_FK,
    Def_Location_ID_FK,
    UNIT_OF_MEAS_ID_FK,
    Container_Type_ID_FK,
    Reg_ID_FK,
    Batch_Number_FK,
    Qty_Max,
    Qty_Initial,
    Qty_Remaining,
    Qty_Available,
    Qty_MinStock,
    Qty_MaxStock,
    Date_Expires,
    Date_Created,
    Compound_ID_FK,
    Container_Name,
    Container_Description,
    Tare_Weight,
    Net_Wght,
    Final_wght,
    Unit_of_WGHT_ID_FK,
    Purity,
    Unit_of_Purity_ID_FK,
    Concentration,
    Density,
    Unit_of_Conc_ID_FK,
    Unit_of_Density_ID_FK,
    Solvent_ID_FK,
    Grade,
    Container_Comments,
    Storage_Conditions,
    Handling_Procedures,
    Lot_Num,
    Supplier_CatNum,
    Supplier_ID_FK,
    Date_Produced,
    Date_Ordered,
    Date_Received,
    Container_Cost,
    Unit_of_Cost_ID_FK,
    PO_Number,
    PO_Line_Number,
    Req_Number,
    Owner_ID_FK,
    Current_User_ID_FK,
    Field_1,
    Field_2,
    Field_3,
    Field_4,
    Field_5,
    Field_6,
    Field_7,
    Field_8,
    Field_9,
    Field_10,
    Date_1,
    Date_2,
    Date_3,
    Date_4,
    Date_5
    )
  VALUES (
    pContainerStatusID,
    v_barcode_c,
    pLocationID,
    pLocationID,
    pUOMID,
    pContainerTypeID,
    pRegID,
    pBatchNumber,
    pMaxQty,
    pInitialQty,
    pInitialQty,
    pInitialQty,
    pMinStockQty,
    pMaxStockQty,
    pExpDate,
    sysdate,
    pCompoundID,
    pContainerName,
    pContainerDesc,
    pTareWeight,
    pNetWeight,
    pFinalWeight,
    pUOWID,
    pPurity,
    pUOPID,
    pConcentration,
    pDensity,
    pUOCID,
    pUODID,
    pSolventIDFK,
    pGrade,
    pComments,
    pStorageConditions,
    pHandlingProcedures,
    pLotNum,
    pSupplierCatNum,
    pSupplierID,
    pDateProduced,
    pDateOrdered,
    pDateReceived,
    pContainerCost,
    pUOCostID,
    pPONumber,
    pPOLineNumber,
    pReqNumber,
    pOwnerID,
    pCurrentUserID,
    pField_1,
    pField_2,
    pField_3,
    pField_4,
    pField_5,
    pField_6,
    pField_7,
    pField_8,
    pField_9,
    pField_10,
    pDate_1,
    pDate_2,
    pDate_3,
    pDate_4,
    pDate_5
   )
    RETURNING Container_ID into NewContainerID;
    pNewIDs := pNewIDs || TO_CHAR(NewContainerID) ||'|';
END LOOP;
    pNewIDs := SUBSTR(pNewIDs, 0, LENGTH(pNewIDs)-1);
RETURN NewContainerID;

exception
WHEN VALUE_ERROR then
  RETURN -123;
WHEN duplicate_barcode then
	--RETURN 'A container with same barcode ID already exists:' || to_Char(dupBarcode_id);
    RETURN -102;
WHEN excess_contents then
	--RETURN 'Amount cannot exceed contianer size.';
  RETURN -103;
WHEN container_type_not_allowed then
  RETURN -128;

END CreateContainer;
/
show errors;
