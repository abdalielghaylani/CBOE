CREATE OR REPLACE FUNCTION "ORDERCONTAINER"
 (
  pBarcode IN Inv_Containers.Barcode%TYPE,
  pLocationID IN Inv_Containers.Location_ID_FK%TYPE,
  pUOMID IN Inv_Containers.Unit_Of_Meas_ID_FK%TYPE,
  pContainerTypeID IN Inv_Containers.Container_Type_ID_FK%TYPE,
  pContainerStatusID IN Inv_Containers.Container_Status_ID_FK%Type:=1,
  pMaxQty IN Inv_Containers.Qty_Max%TYPE,
  pRegID IN Inv_Containers.Reg_ID_FK%TYPE,
  pBatchNumber IN Inv_Containers.Batch_Number_FK%TYPE,
  pInitialQty IN Inv_Containers.Qty_Initial%TYPE,
  pQtyRemaining IN Inv_Containers.Qty_Remaining%TYPE,
  pMinStockQty IN Inv_Containers.Qty_MinStock%TYPE,
  pMaxStockQty IN Inv_Containers.Qty_MaxStock%TYPE,
  pExpDate IN Inv_Containers.Date_Expires%TYPE,
  pCompoundID IN Inv_Containers.Compound_ID_FK%TYPE,
  pContainerName IN Inv_Containers.Container_Name%TYPE,
  pContainerDesc IN Inv_Containers.Container_Description%TYPE,
  pTareWeight IN Inv_Containers.Tare_Weight%TYPE,
  pUOWID IN Inv_Containers.Unit_of_WGHT_ID_FK%TYPE,
  pPurity IN Inv_Containers.Purity%TYPE,
  pUOPID IN Inv_Containers.Unit_of_Purity_ID_FK%TYPE,
  pConcentration IN Inv_Containers.Concentration%TYPE,
  pUOCID IN Inv_Containers.Unit_of_CONC_ID_FK%TYPE,
  pSolventIDFK IN Inv_Containers.Solvent_ID_FK%TYPE,
  pGrade IN Inv_Containers.Grade%TYPE,
  pComments IN Inv_Containers.Container_Comments%TYPE,
  pSupplierID IN Inv_Containers.Supplier_ID_FK%TYPE,
  pSupplierCatNum IN Inv_Containers.Supplier_CatNum%TYPE,
  pLotNum IN Inv_Containers.Lot_Num%TYPE,
  pDateProduced IN Inv_Containers.Date_Produced%TYPE,
  pDateOrdered IN Inv_Containers.Date_Ordered%TYPE,
  pDateReceived IN Inv_Containers.Date_Received%TYPE,
  pContainerCost IN Inv_Containers.Container_Cost%TYPE,
  pPONumber IN Inv_Containers.PO_Number%TYPE,
  pReqNumber IN Inv_Containers.Req_Number%TYPE,
  pOwnerID IN Inv_Containers.Owner_ID_FK%TYPE,
  pCurrentUserID IN Inv_Containers.Current_User_ID_FK%TYPE,
  pNumCopies IN int := 1,
  pField_1 in inv_containers.Field_1%Type,
  pField_2 in inv_containers.Field_2%Type,
  pField_3 in inv_containers.Field_3%Type,
  pField_4 in inv_containers.Field_4%Type,
  pField_5 in inv_containers.Field_5%Type,
  pField_6 in inv_containers.Field_1%Type,
  pField_7 in inv_containers.Field_2%Type,
  pField_8 in inv_containers.Field_3%Type,
  pField_9 in inv_containers.Field_4%Type,
  pField_10 in inv_containers.Field_5%Type,
  pDate_1 in inv_containers.Date_1%Type,
  pDate_2 in inv_containers.Date_2%Type,
  pDate_3 in inv_containers.Date_3%Type,
  pDate_4 in inv_containers.Date_4%Type,
  pDate_5 in inv_containers.Date_5%Type,
  --New paramters for OrderSubstance
  pDueDate IN inv_Container_Order.due_date%TYPE,
  pProjectNo IN inv_Container_Order.project_no%TYPE,
  pJobNo IN inv_Container_Order.job_no%TYPE,
  pIsRushOrder IN inv_Container_Order.isRushOrder%TYPE,
  pDeliveryLocationID IN inv_Container_Order.delivery_Location_ID_FK%TYPE,
  pNewSupplierName IN inv_Container_Order.new_supplier_name%TYPE,
  pNewSupplierContact IN inv_Container_Order.new_supplier_contact%TYPE,
  pNewSupplierPhone IN inv_Container_Order.new_supplier_phone%TYPE,
  pNewSupplierFax IN inv_Container_Order.new_supplier_fax%TYPE,
  pOrderReasonID IN inv_Container_Order.container_order_reason_id_fk%TYPE,
  pOrderReasonIfOtherText IN inv_Container_Order.reason_if_other%TYPE,
  pNewIDs OUT VARCHAR2
  )
RETURN Inv_Containers.Container_ID%Type
IS
NewContainerID Inv_Containers.Container_ID%Type;
duplicate_barcode exception;
excess_contents exception;
CURSOR dupBarcode_cur(Barcode_in in Inv_Containers.Barcode%Type) IS
  SELECT Container_ID FROM Inv_Containers WHERE inv_Containers.Barcode = Barcode_in;
dupBarcode_id Inv_Containers.Container_ID%Type;
v_barcode_i int;
v_barcode_c Inv_Containers.Barcode%Type;
v_barcodedescid Inv_Barcode_Desc.Barcode_Desc_ID%TYPE;
vNumCopies int;
BEGIN
if pInitialQty > pMaxQty then
  RAISE excess_contents;
end if;
if pNumCopies is null then
	vNumCopies := 1;
else 
	vNumCopies := pNumCopies;
end if;
-- Loop over pNumCopies
FOR v_Counter IN 1..vNumCopies LOOP

  IF pBarcode IS NOT NULL THEN
	 	return -1;
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
  ELSE
	v_barcodedescid := constants.cContainerBarcodeDesc;
	v_barcode_c := barcodes.GetNextBarcode(v_barcodedescid);
  END IF;
/*
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
*/    
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
    Unit_of_WGHT_ID_FK,
    Purity,
    Unit_of_Purity_ID_FK,
    Concentration,
    Unit_of_Conc_ID_FK,
    Solvent_ID_FK,
    Grade,
    Container_Comments,
    Lot_Num,
    Supplier_CatNum,
    Supplier_ID_FK,
    Date_Produced,
    Date_Ordered,
    Date_Received,
    Container_Cost,
    PO_Number,
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
    pQtyRemaining,
    pQtyRemaining,
    pMinStockQty,
    pMaxStockQty,
    pExpDate,
    sysdate,
    pCompoundID,
    pContainerName,
    pContainerDesc,
    pTareWeight,
    pUOWID,
    pPurity,
    pUOPID,
    pConcentration,
    pUOCID,
    pSolventIDFK,
    pGrade,
    pComments,
    pLotNum,
    pSupplierCatNum,
    pSupplierID,
    pDateProduced,
    pDateOrdered,
    pDateReceived,
    pContainerCost,
    pPONumber,
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
    RETURNING Container_ID INTO NewContainerID;
    UpdateContainerBatches(NewContainerID);
    pNewIDs := pNewIDs || TO_CHAR(NewContainerID) ||'|';

  --New container order table.
  INSERT INTO inv_Container_Order
    (container_id, due_Date, project_no, job_no, isRushOrder,
     delivery_location_ID_FK,
     new_supplier_name, new_supplier_contact,
     new_supplier_phone, new_supplier_fax, order_source,
     container_order_reason_id_fk, reason_if_other)
  VALUES
    (NewContainerID, pDueDate, pProjectNo, pJobNo, pIsRushOrder,
     pDeliveryLocationID,
     pNewSupplierName, pNewSupplierContact,
     pNewSupplierPhone, pNewSupplierFax, Constants.cOrderSourceNewSubstance,
     pOrderReasonID, pOrderReasonIfOtherText);

  --Insert the ID into the global temporary table
  -- so that the container can be associated with an order line.
  INSERT INTO TempIDs
    (ID)
  VALUES
    (NewContainerID);

END LOOP;
    pNewIDs := SUBSTR(pNewIDs, 0, LENGTH(pNewIDs)-1);

    -- add the item to custom_chem_order
    --InsertIntoCustomChemOrder(NewContainerID, pNumCopies);

RETURN NewContainerID;

EXCEPTION
WHEN VALUE_ERROR then
  RETURN -123;
WHEN duplicate_barcode then
	--RETURN 'A container with same barcode ID already exists:' || to_Char(dupBarcode_id);
    RETURN -102;
WHEN excess_contents then
	--RETURN 'Amount cannot exceed contianer size.';
  RETURN -103;
END OrderContainer;
/
show errors;
