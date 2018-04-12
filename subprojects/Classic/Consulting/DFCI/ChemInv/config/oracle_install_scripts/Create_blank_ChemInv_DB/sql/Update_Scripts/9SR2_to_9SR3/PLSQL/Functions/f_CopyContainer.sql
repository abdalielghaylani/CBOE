CREATE OR REPLACE FUNCTION "COPYCONTAINER"
(
  pContainerID in Inv_Containers.Container_ID%Type,
  pNumCopies in int:=1,
  pNewIDs out varchar2
)
return Inv_Containers.Container_ID%Type
is
NewContainerID Inv_Containers.Container_ID%Type;

v_Location_ID_FK Inv_Containers.Location_ID_FK%Type;
v_Compound_ID_FK Inv_Containers.Compound_ID_FK%Type;
v_Reg_ID_FK Inv_Containers.Reg_ID_FK%Type;
v_Batch_Number_FK Inv_Containers.Batch_Number_FK%Type;
v_Container_Name Inv_Containers.Container_Name%Type;
v_Container_Description Inv_Containers.Container_Description%Type;
v_Qty_Max Inv_Containers.Qty_Max%Type;
v_Qty_Initial Inv_Containers.Qty_Initial%Type;
v_Qty_Remaining Inv_Containers.Qty_Remaining%Type;
v_Qty_Minstock Inv_Containers.Qty_Minstock%Type;
v_Qty_Maxstock Inv_Containers.Qty_Maxstock%Type;
v_Well_Number Inv_Containers.Well_Number%Type;
v_Well_Row Inv_Containers.Well_Row%Type;
v_Well_Column Inv_Containers.Well_Column%Type;
v_Date_Expires Inv_Containers.Date_Expires%Type;
v_Date_Created Inv_Containers.Date_Created%Type;
v_Container_Type_ID_FK Inv_Containers.Container_Type_ID_FK%Type;
v_Purity Inv_Containers.Purity%Type;
v_Solvent_ID_FK Inv_Containers.Solvent_ID_FK%Type;
v_Concentration Inv_Containers.Concentration%Type;
v_Unit_Of_Meas_ID_FK Inv_Containers.Unit_Of_Meas_ID_FK%Type;
v_Unit_Of_Wght_ID_FK Inv_Containers.Unit_Of_Wght_ID_FK%Type;
v_Unit_Of_Conc_ID_FK Inv_Containers.Unit_Of_Conc_ID_FK%Type;
v_Grade Inv_Containers.Grade%Type;
v_Weight Inv_Containers.Weight%Type;
v_Unit_Of_Purity_ID_FK Inv_Containers.Unit_Of_Purity_ID_FK%Type;
v_Tare_Weight Inv_Containers.Tare_Weight%Type;
v_Owner_ID_FK Inv_Containers.Owner_ID_FK%Type;
v_Container_Comments Inv_Containers.Container_Comments%Type;
v_Storage_Conditions inv_containers.storage_conditions%TYPE;
v_Handling_Procedures inv_containers.handling_procedures%TYPE;
v_Ordered_By_ID_FK Inv_Containers.Ordered_By_ID_FK%Type;
v_Date_Ordered Inv_Containers.Date_Ordered%Type;
v_Date_Received Inv_Containers.Date_Received%Type;
v_Lot_Num Inv_Containers.Lot_Num%Type;
v_Container_Status_ID_FK Inv_Containers.Container_Status_ID_FK%Type;
v_Received_By_ID_FK Inv_Containers.Received_By_ID_FK%Type;
v_Final_Wght Inv_Containers.Final_Wght%Type;
v_Net_Wght Inv_Containers.Net_Wght%Type;
v_Qty_Available Inv_Containers.Qty_Available%Type;
v_Qty_Reserved Inv_Containers.Qty_Reserved%Type;
v_Physical_State_ID_FK Inv_Containers.Physical_State_ID_FK%Type;
v_Current_User_ID_FK Inv_Containers.Current_User_ID_FK%Type;
v_Supplier_ID_FK Inv_Containers.Supplier_ID_FK%Type;
v_Supplier_Catnum Inv_Containers.Supplier_Catnum%Type;
v_Date_Produced Inv_Containers.Date_Produced%Type;
v_Container_Cost Inv_Containers.Container_Cost%Type;
v_Unit_Of_Cost_ID_FK Inv_Containers.Unit_Of_Cost_ID_FK%Type;
v_Def_Location_ID_FK Inv_Containers.Def_Location_ID_FK%Type;
v_PO_Number Inv_Containers.PO_Number%Type;
v_Req_Number Inv_Containers.Req_Number%Type;
v_Density Inv_Containers.Density%Type;
v_Unit_Of_Density_ID_FK Inv_Containers.Unit_Of_Density_ID_FK%Type;
v_PO_Line_Number Inv_Containers.PO_Line_Number%Type;
v_Field_1 Inv_Containers.Field_1%Type;
v_Field_2 Inv_Containers.Field_2%Type;
v_Field_3 Inv_Containers.Field_3%Type;
v_Field_4 Inv_Containers.Field_4%Type;
v_Field_5 Inv_Containers.Field_5%Type;
v_Field_6 Inv_Containers.Field_6%Type;
v_Field_7 Inv_Containers.Field_7%Type;
v_Field_8 Inv_Containers.Field_8%Type;
v_Field_9 Inv_Containers.Field_9%Type;
v_Field_10 Inv_Containers.Field_10%Type;
v_Date_1 Inv_Containers.Date_1%Type;
v_Date_2 Inv_Containers.Date_2%Type;
v_Date_3 Inv_Containers.Date_3%Type;
v_Date_4 Inv_Containers.Date_4%Type;
v_Date_5 Inv_Containers.Date_5%Type;
v_Creator Inv_Containers.Creator%Type;
v_Timestamp Inv_Containers.Timestamp%Type;

BEGIN
	-- Get Container Values
	SELECT
		Location_ID_FK,
		Compound_ID_FK,
		Reg_ID_FK,
		Batch_Number_FK,
		Container_Name,
		Container_Description,
		Qty_Max,
		Qty_Initial,
		Qty_Remaining,
		Qty_Minstock,
		Qty_Maxstock,
		Well_Number,
		Well_Row,
		Well_Column,
		Date_Expires,
		sysdate,
		Container_Type_ID_FK,
		Purity,
		Solvent_ID_FK,
		Concentration,
		Unit_Of_Meas_ID_FK,
		Unit_Of_Wght_ID_FK,
		Unit_Of_Conc_ID_FK,
		Grade,
		Weight,
		Unit_Of_Purity_ID_FK,
		Tare_Weight,
		Owner_ID_FK,
		Container_Comments,
    Storage_Conditions,
    Handling_Procedures,
		Ordered_By_ID_FK,
		Date_Ordered,
		Date_Received,
		Lot_Num,
		Container_Status_ID_FK,
		Received_By_ID_FK,
		Final_Wght,
		Net_Wght,
		Qty_Available,
		Qty_Reserved,
		Physical_State_ID_FK,
		Current_User_ID_FK,
		Supplier_ID_FK,
		Supplier_Catnum,
		Date_Produced,
		Container_Cost,
		Unit_Of_Cost_ID_FK,
		Def_Location_ID_FK,
		PO_Number,
		Req_Number,
		Density,
		Unit_Of_Density_ID_FK,
		PO_Line_Number,
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
		Date_5,
		RTRIM(user),
		sysdate
	INTO
		v_Location_ID_FK,
		v_Compound_ID_FK,
		v_Reg_ID_FK,
		v_Batch_Number_FK,
		v_Container_Name,
		v_Container_Description,
		v_Qty_Max,
		v_Qty_Initial,
		v_Qty_Remaining,
		v_Qty_Minstock,
		v_Qty_Maxstock,
		v_Well_Number,
		v_Well_Row,
		v_Well_Column,
		v_Date_Expires,
		v_Date_Created,
		v_Container_Type_ID_FK,
		v_Purity,
		v_Solvent_ID_FK,
		v_Concentration,
		v_Unit_Of_Meas_ID_FK,
		v_Unit_Of_Wght_ID_FK,
		v_Unit_Of_Conc_ID_FK,
		v_Grade,
		v_Weight,
		v_Unit_Of_Purity_ID_FK,
		v_Tare_Weight,
		v_Owner_ID_FK,
		v_Container_Comments,
    v_Storage_Conditions,
    v_Handling_Procedures,
		v_Ordered_By_ID_FK,
		v_Date_Ordered,
		v_Date_Received,
		v_Lot_Num,
		v_Container_Status_ID_FK,
		v_Received_By_ID_FK,
		v_Final_Wght,
		v_Net_Wght,
		v_Qty_Available,
		v_Qty_Reserved,
		v_Physical_State_ID_FK,
		v_Current_User_ID_FK,
		v_Supplier_ID_FK,
		v_Supplier_Catnum,
		v_Date_Produced,
		v_Container_Cost,
		v_Unit_Of_Cost_ID_FK,
		v_Def_Location_ID_FK,
		v_PO_Number,
		v_Req_Number,
		v_Density,
		v_Unit_Of_Density_ID_FK,
		v_PO_Line_Number,
		v_Field_1,
		v_Field_2,
		v_Field_3,
		v_Field_4,
		v_Field_5,
		v_Field_6,
		v_Field_7,
		v_Field_8,
		v_Field_9,
		v_Field_10,
		v_Date_1,
		v_Date_2,
		v_Date_3,
		v_Date_4,
		v_Date_5,
		v_Creator,
		v_Timestamp
	FROM inv_containers
	WHERE container_ID = pContainerID;
	if racks.isRackLocation(v_Location_ID_FK)>0 then
		SELECT Parent_ID into v_Location_ID_FK FROM inv_locations WHERE Location_ID = v_Location_ID_FK;	
	end if;
	
	-- Loop over pNumCopies
	FOR v_Counter IN 1..pNumCopies LOOP

	INSERT INTO Inv_Containers(
		CONTAINER_ID,
		LOCATION_ID_FK,
		COMPOUND_ID_FK,
		PARENT_CONTAINER_ID_FK,
		REG_ID_FK,
		BATCH_NUMBER_FK,
		CONTAINER_NAME,
		CONTAINER_DESCRIPTION,
		QTY_MAX,
		QTY_INITIAL,
		QTY_REMAINING,
		QTY_MINSTOCK,
		QTY_MAXSTOCK,
		WELL_NUMBER,
		WELL_ROW,
		WELL_COLUMN,
		DATE_EXPIRES,
		DATE_CREATED,
		CONTAINER_TYPE_ID_FK,
		PURITY,
		SOLVENT_ID_FK,
		CONCENTRATION,
		UNIT_OF_MEAS_ID_FK,
		UNIT_OF_WGHT_ID_FK,
		UNIT_OF_CONC_ID_FK,
		GRADE, WEIGHT,
		UNIT_OF_PURITY_ID_FK,
		TARE_WEIGHT,
		OWNER_ID_FK,
		CONTAINER_COMMENTS,
    STORAGE_CONDITIONS,
    HANDLING_PROCEDURES,
		ORDERED_BY_ID_FK,
		DATE_ORDERED,
		DATE_RECEIVED,
		LOT_NUM,
		CONTAINER_STATUS_ID_FK,
		RECEIVED_BY_ID_FK,
		FINAL_WGHT,
		NET_WGHT,
		QTY_AVAILABLE,
		QTY_RESERVED,
		PHYSICAL_STATE_ID_FK,
		CURRENT_USER_ID_FK,
		SUPPLIER_ID_FK,
		SUPPLIER_CATNUM,
		DATE_PRODUCED,
		CONTAINER_COST,
		UNIT_OF_COST_ID_FK,
		DEF_LOCATION_ID_FK,
		BARCODE,
		PO_NUMBER,
		REQ_NUMBER,
		DENSITY,
		UNIT_OF_DENSITY_ID_FK,
		PO_LINE_NUMBER,
		FIELD_1,
		FIELD_2,
		FIELD_3,
		FIELD_4,
		FIELD_5,
		FIELD_6,
		FIELD_7,
		FIELD_8,
		FIELD_9,
		FIELD_10,
		DATE_1,
		DATE_2,
		DATE_3,
		DATE_4,
		DATE_5,
		RID,
		CREATOR,
		TIMESTAMP)
	VALUES (
		NULL,
		guiutils.GetLocationId(v_Location_ID_FK, pContainerID, NULL, NULL),
		v_Compound_ID_FK,
		NULL,
		v_Reg_ID_FK,
		v_Batch_Number_FK,
		v_Container_Name,
		v_Container_Description,
		v_Qty_Max,
		v_Qty_Initial,
		v_Qty_Remaining,
		v_Qty_Minstock,
		v_Qty_Maxstock,
		v_Well_Number,
		v_Well_Row,
		v_Well_Column,
		v_Date_Expires,
		v_Date_Created,
		v_Container_Type_ID_FK,
		v_Purity,
		v_Solvent_ID_FK,
		v_Concentration,
		v_Unit_Of_Meas_ID_FK,
		v_Unit_Of_Wght_ID_FK,
		v_Unit_Of_Conc_ID_FK,
		v_Grade,
		v_Weight,
		v_Unit_Of_Purity_ID_FK,
		v_Tare_Weight,
		v_Owner_ID_FK,
		v_Container_Comments,
    v_Storage_Conditions,
    v_Handling_Procedures,
		v_Ordered_By_ID_FK,
		v_Date_Ordered,
		v_Date_Received,
		v_Lot_Num,
		v_Container_Status_ID_FK,
		v_Received_By_ID_FK,
		v_Final_Wght,
		v_Net_Wght,
		v_Qty_Available,
		v_Qty_Reserved,
		v_Physical_State_ID_FK,
		v_Current_User_ID_FK,
		v_Supplier_ID_FK,
		v_Supplier_Catnum,
		v_Date_Produced,
		v_Container_Cost,
		v_Unit_Of_Cost_ID_FK,
		v_Def_Location_ID_FK,
		NULL,
		v_PO_Number,
		v_Req_Number,
		v_Density,
		v_Unit_Of_Density_ID_FK,
		v_PO_Line_Number,
		v_Field_1,
		v_Field_2,
		v_Field_3,
		v_Field_4,
		v_Field_5,
		v_Field_6,
		v_Field_7,
		v_Field_8,
		v_Field_9,
		v_Field_10,
		v_Date_1,
		v_Date_2,
		v_Date_3,
		v_Date_4,
		v_Date_5,
	NULL,
	v_Creator,
	v_Timestamp
	)
    RETURNING Container_ID into NewContainerID;
    UpdateContainerBatches(NewContainerID);
    pNewIDs := pNewIDs || TO_CHAR(NewContainerID) ||'|';
END LOOP;
    pNewIDs := SUBSTR(pNewIDs, 0, LENGTH(pNewIDs)-1);
RETURN NewContainerID;

exception
WHEN VALUE_ERROR then
  RETURN -123;

END COPYCONTAINER;
/
show errors;
