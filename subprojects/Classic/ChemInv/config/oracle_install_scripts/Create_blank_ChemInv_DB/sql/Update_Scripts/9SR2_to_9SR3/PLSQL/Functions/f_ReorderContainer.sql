CREATE OR REPLACE FUNCTION "REORDERCONTAINER"
 (pOldContainerID IN Inv_Containers.Container_ID%TYPE,
  pContainerName IN Inv_Containers.Container_Name%TYPE,
  pComments IN Inv_Containers.Container_Comments%TYPE,
  pOwnerID IN Inv_Containers.Owner_ID_FK%TYPE,
  pCurrentUserID IN Inv_Containers.Current_User_ID_FK%TYPE,
  pNumCopies IN int := 1,
  --New parameters for OrderSubstance
  pDueDate IN inv_Container_Order.due_date%TYPE,
  pProjectNo IN inv_Container_Order.project_no%TYPE,
  pJobNo IN inv_Container_Order.job_no%TYPE,
  pIsRushOrder IN inv_Container_Order.isRushOrder%TYPE,
  pDeliveryLocationID IN inv_Container_Order.delivery_Location_ID_FK%TYPE,
  pOrderReasonID IN inv_Container_Order.container_order_reason_id_fk%TYPE,
  pOrderReasonIfOtherText IN inv_Container_Order.reason_if_other%TYPE,
  pNewIDs OUT VARCHAR2)
RETURN Inv_Containers.Container_ID%TYPE
IS
  NewContainerID Inv_Containers.Container_ID%TYPE;
BEGIN
  -- Loop over pNumCopies
  FOR v_Counter IN 1..pNumCopies LOOP

    SELECT seq_inv_containers.NEXTVAL
    INTO   NewContainerID
    FROM   dual;

    INSERT INTO Inv_Containers
     (Container_Status_ID_FK, Location_ID_FK, Def_Location_ID_FK,
      UNIT_OF_MEAS_ID_FK, Container_Type_ID_FK, Reg_ID_FK, Batch_Number_FK,
      Qty_Max, Qty_Initial, Qty_Remaining, Qty_Available, Qty_MinStock,
      Qty_MaxStock, Date_Expires, Date_Created, Compound_ID_FK, Container_Name,
      Container_Description, Tare_Weight, Unit_of_WGHT_ID_FK, Purity,
      Unit_of_Purity_ID_FK, Concentration, Unit_of_Conc_ID_FK, Solvent_ID_FK,
      Grade, Container_Comments, Lot_Num, Supplier_CatNum, Supplier_ID_FK,
      Ordered_by_ID_FK,
      Date_Produced, Date_Ordered, Date_Received, Container_Cost, PO_Number,
      Req_Number, Owner_ID_FK, Current_User_ID_FK, Container_ID,
      Field_1,Field_2,Field_3,Field_4,Field_5,Field_6,Field_7,Field_8,Field_9,Field_10,
      Date_1,Date_2,Date_3,Date_4,Date_5)
    SELECT Constants.cOrderPending, Constants.cOnOrderLoc, Constants.cOnOrderLoc,
           UNIT_OF_MEAS_ID_FK, Container_Type_ID_FK, Reg_ID_FK, Batch_Number_FK,
           Qty_Initial, Qty_Initial, Qty_Initial, Qty_Initial, Qty_MinStock,
           Qty_MaxStock, Date_Expires, SYSDATE, Compound_ID_FK, pContainerName,
           Container_Description, Tare_Weight, Unit_of_WGHT_ID_FK, Purity,
           Unit_of_Purity_ID_FK, Concentration, Unit_of_Conc_ID_FK, Solvent_ID_FK,
           Grade, pComments, Lot_Num, Supplier_CatNum, Supplier_ID_FK,
           pCurrentUserID,
           NULL, SYSDATE, NULL, Container_Cost, PO_Number,
           NULL, pOwnerID, pCurrentUserID, NewContainerID,
           Field_1,Field_2,Field_3,Field_4,Field_5,Field_6,Field_7,Field_8,Field_9,Field_10,
           Date_1,Date_2,Date_3,Date_4,Date_5
    FROM   Inv_Containers
    WHERE  Container_ID = pOldContainerID;
	UpdateContainerBatches(NewContainerID);

    --New container order table.
    INSERT INTO inv_Container_Order
      (container_id, due_Date, project_no, job_no, isRushOrder,
       delivery_location_ID_FK,
       new_supplier_name, new_supplier_contact,
       new_supplier_phone, new_supplier_fax, order_source,
       container_order_reason_id_fk, reason_if_other)
    SELECT NewContainerID, pDueDate, pProjectNo, pJobNo, pIsRushOrder,
           pDeliveryLocationID,
           new_supplier_name, new_supplier_contact,
           new_supplier_phone, new_supplier_fax, Constants.cOrderSourceReorder,
           pOrderReasonID, pOrderReasonIfOtherText
    FROM   dual, (SELECT new_supplier_name, new_supplier_contact,
                         new_supplier_phone, new_supplier_fax
                  FROM   inv_Container_Order
                  WHERE  Container_ID = pOldContainerID)
    WHERE  new_supplier_name (+) = dummy;

    -- Insert the ID into the global temporary table
    -- so that the container can be associated with an order line.
    INSERT INTO TempIDs
      (ID)
    VALUES
      (NewContainerID);

    pNewIDs := pNewIDs || TO_CHAR(NewContainerID) ||'|';
  END LOOP;

  pNewIDs := SUBSTR(pNewIDs, 0, LENGTH(pNewIDs)-1);

  -- add the item to abp_chem_order
  --InsertIntoCustomChemOrder(NewContainerID, pNumCopies);

  RETURN NewContainerID;
EXCEPTION
  WHEN VALUE_ERROR THEN
    RETURN -123;
END REORDERCONTAINER;
/
show errors;
