CREATE OR REPLACE FUNCTION "CREATECONTAINER"(pBarcode IN Inv_Containers.Barcode%TYPE,
									pBarcodeDescID IN Inv_Barcode_Desc.Barcode_Desc_ID%TYPE,
									--pLocationID IN Inv_Containers.Location_ID_FK%TYPE,
									pLocationID IN VARCHAR2,
									pUOMID IN Inv_Containers.Unit_Of_Meas_ID_FK%TYPE,
									pContainerTypeID IN Inv_Containers.Container_Type_ID_FK%TYPE,
									pContainerStatusID IN Inv_Containers.Container_Status_ID_FK%TYPE := 1,
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
									pNetWeight IN Inv_Containers.Net_Wght%TYPE,
									pFinalWeight IN Inv_Containers.Final_Wght%TYPE,
									pUOWID IN Inv_Containers.Unit_of_WGHT_ID_FK%TYPE,
									pPurity IN Inv_Containers.Purity%TYPE,
									pUOPID IN Inv_Containers.Unit_of_Purity_ID_FK%TYPE,
									pConcentration IN Inv_Containers.Concentration%TYPE,
									pDensity IN Inv_Containers.Density%TYPE,
									pUOCID IN Inv_Containers.Unit_of_CONC_ID_FK%TYPE,
									pUODID IN Inv_Containers.Unit_of_DENSITY_ID_FK%TYPE,
									pSolventIDFK IN Inv_Containers.Solvent_ID_FK%TYPE,
									pGrade IN Inv_Containers.Grade%TYPE,
									pComments IN Inv_Containers.Container_Comments%TYPE,
									pStorageConditions IN inv_containers.storage_conditions%TYPE,
									pHandlingProcedures IN inv_containers.handling_procedures%TYPE,
									pSupplierID IN Inv_Containers.Supplier_ID_FK%TYPE,
									pSupplierCatNum IN Inv_Containers.Supplier_CatNum%TYPE,
									pLotNum IN Inv_Containers.Lot_Num%TYPE,
									pDateProduced IN Inv_Containers.Date_Produced%TYPE,
									pDateOrdered IN Inv_Containers.Date_Ordered%TYPE,
									pDateReceived IN Inv_Containers.Date_Received%TYPE,
									pContainerCost IN Inv_Containers.Container_Cost%TYPE,
									pUOCostID IN inv_containers.unit_of_cost_id_fk%TYPE,
									pPONumber IN Inv_Containers.PO_Number%TYPE,
									pPOLineNumber IN Inv_Containers.PO_Line_Number%TYPE,
									pReqNumber IN Inv_Containers.Req_Number%TYPE,
									pOwnerID IN Inv_Containers.Owner_ID_FK%TYPE,
									pCurrentUserID IN Inv_Containers.Current_User_ID_FK%TYPE,
									pNumCopies IN INT := 1,
									pNewIDs OUT VARCHAR2,
									pField_1 IN inv_containers.Field_1%TYPE,
									pField_2 IN inv_containers.Field_2%TYPE,
									pField_3 IN inv_containers.Field_3%TYPE,
									pField_4 IN inv_containers.Field_4%TYPE,
									pField_5 IN inv_containers.Field_5%TYPE,
									pField_6 IN inv_containers.Field_6%TYPE,
									pField_7 IN inv_containers.Field_7%TYPE,
									pField_8 IN inv_containers.Field_8%TYPE,
									pField_9 IN inv_containers.Field_9%TYPE,
									pField_10 IN inv_containers.Field_10%TYPE,
									pDate_1 IN inv_containers.Date_1%TYPE,
									pDate_2 IN inv_containers.Date_2%TYPE,
									pDate_3 IN inv_containers.Date_3%TYPE,
									pDate_4 IN inv_containers.Date_4%TYPE,
									pDate_5 IN inv_containers.Date_5%TYPE)

 RETURN Inv_Containers.Container_ID%TYPE IS
	NewContainerID Inv_Containers.Container_ID%TYPE;
	duplicate_barcode EXCEPTION;
	excess_contents EXCEPTION;
	container_type_not_allowed EXCEPTION;
	CURSOR dupBarcode_cur(Barcode_in IN Inv_Containers.Barcode%TYPE) IS
		SELECT Container_ID
		FROM Inv_Containers
		WHERE inv_Containers.Barcode = Barcode_in;
	dupBarcode_id   Inv_Containers.Container_ID%TYPE;
	v_barcode_i     INT;
	v_barcode_c     Inv_Containers.Barcode%TYPE;
	v_barcodedescid Inv_Barcode_Desc.Barcode_Desc_ID%TYPE;
	l_locationId inv_containers.location_id_fk%TYPE;
BEGIN

	IF is_container_type_allowed(NULL, pLocationID) = 0 THEN
		RAISE container_type_not_allowed;
	END IF;
	IF pInitialQty > pMaxQty THEN
		RAISE excess_contents;
	END IF;

	-- Loop over pNumCopies
	FOR v_Counter IN 1 .. pNumCopies
	LOOP
		IF pBarcode IS NOT NULL THEN
			--' if there are multiple copies and a barcode is specified either increment the barcode or append a number to the string
			IF pNumCopies > 1 THEN
				IF guiutils.IS_NUMBER(pBarcode) = 1 THEN
					v_barcode_i := TO_NUMBER(pBarcode) + (v_Counter - 1);
					v_barcode_c := TO_CHAR(v_barcode_i);
				ELSE
					v_barcode_c := pBarcode || v_Counter;
				END IF;
				-- Check for duplicate barcode
				IF v_barcode_c IS NOT NULL THEN
					OPEN dupBarcode_cur(v_barcode_c);
					FETCH dupBarcode_cur
						INTO dupBarcode_id;
					IF dupBarcode_cur%ROWCOUNT = 1 THEN
						RAISE duplicate_barcode;
					END IF;
					CLOSE dupBarcode_cur;
				END IF;
			ELSE
				v_barcode_c := pBarcode;
			END IF;

		ELSE
			IF pBarcodeDescID IS NULL THEN
				v_barcodedescid := constants.cContainerBarcodeDesc;
			ELSE
				v_barcodedescid := pBarcodeDescID;
			END IF;
			v_barcode_c := barcodes.GetNextBarcode(v_barcodedescid);
		END IF;

		l_locationId := guiutils.GetLocationId(pLocationID, NULL, NULL, NULL);

		INSERT INTO Inv_Containers
			(Container_Status_ID_FK,
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
			 Date_5)
		VALUES
			(pContainerStatusID,
			 v_barcode_c,
			 l_locationId,
			 l_locationId,
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
			 trunc(sysdate),
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
			 pDate_5)
		RETURNING Container_ID INTO NewContainerID;
		UpdateContainerBatches(NewContainerID);
		pNewIDs := pNewIDs || TO_CHAR(NewContainerID) || '|';
	END LOOP;
	pNewIDs := SUBSTR(pNewIDs, 0, LENGTH(pNewIDs) - 1);
	RETURN NewContainerID;

EXCEPTION
	WHEN VALUE_ERROR THEN
		RETURN - 123;
	WHEN duplicate_barcode THEN
		--RETURN 'A container with same barcode ID already exists:' || to_Char(dupBarcode_id);
		RETURN - 102;
	WHEN excess_contents THEN
		--RETURN 'Amount cannot exceed contianer size.';
		RETURN - 103;
	WHEN container_type_not_allowed THEN
		RETURN - 128;

END CreateContainer;
/
show errors;
