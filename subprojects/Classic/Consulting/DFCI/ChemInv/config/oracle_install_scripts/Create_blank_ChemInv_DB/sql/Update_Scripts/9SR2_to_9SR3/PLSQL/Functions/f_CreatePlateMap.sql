CREATE OR REPLACE FUNCTION "CREATEPLATEMAP"
(
	pPlateIDList varchar2,
	pLocationIDList varchar2
)
return varchar2
is
vPlateIDList_t STRINGUTILS.t_char;
vLocationIDList_t STRINGUTILS.t_char;
NewPlateID Inv_Plates.Plate_ID%Type;
--v_Location_ID_FK Inv_Plates.Location_ID_FK%Type;
v_Plate_Type_ID_FK Inv_Plates.Plate_Type_ID_FK%Type;
v_Plate_Format_ID_FK Inv_Plates.Plate_Format_ID_FK%Type;
--v_Parent_Plate_ID_FK Inv_Plates.Plate_ID%Type;
v_Status_ID_FK Inv_Plates.Status_ID_FK%Type;
v_Group_Name Inv_Plates.Group_Name%Type;
v_Library_ID_FK Inv_Plates.Library_ID_FK%Type;
v_FT_Cycles Inv_Plates.FT_Cycles%Type;
v_Weight_Unit_FK Inv_Plates.Weight_Unit_FK%Type;
v_Qty_Unit_FK Inv_Plates.Qty_Unit_FK%Type;
v_Solvent_ID_FK Inv_Plates.Solvent_ID_FK%Type;
v_Concentration Inv_Plates.Concentration%Type;
v_Conc_Unit_FK Inv_Plates.Conc_Unit_FK%Type;
l_supplierBarcode inv_plates.supplier_barcode%TYPE;
v_Supplier_Shipment_Code Inv_Plates.Supplier_Shipment_Code%Type;
v_Supplier_Shipment_Number Inv_Plates.Supplier_Shipment_Number%Type;
v_Supplier_Shipment_Date Inv_Plates.Supplier_Shipment_Date%Type;
v_Molar_Amount Inv_Plates.Molar_Amount%Type;
v_Molar_Unit_FK Inv_Plates.Molar_Unit_FK%Type;
v_Amounts_Differ Inv_Plates.Amounts_Differ%Type;
v_Supplier Inv_Plates.Supplier%Type;
v_Plate_Exists Inv_Plates.Plate_Exists%Type;

vNewIDs varchar2(200);

wellformats_rec inv_vw_well_format%ROWTYPE;
vNewWellID inv_wells.well_id%Type;

BEGIN
	--SELECT plate_type_id INTO v_Plate_Type_ID_FK FROM inv_plate_types WHERE plate_type_name = 'Plate Map';
	vPlateIDList_t := STRINGUTILS.split(pPlateIDList, ',');
	vLocationIDList_t := STRINGUTILS.split(pLocationIDList, ',');

	FOR i in vPlateIDList_t.First..vPlateIDList_t.Last
	Loop
		-- Get Plate Values
		SELECT
			--Location_ID_FK,
			Plate_Type_ID_FK,
			Plate_Format_ID_FK,
			--Parent_Plate_ID_FK,
			Status_ID_FK,
			Group_Name,
			Library_ID_FK,
			FT_Cycles,
			Weight_Unit_FK,
			Qty_Unit_FK,
			Solvent_ID_FK,
			Concentration,
			Conc_Unit_FK,
			Supplier_Barcode,
			Supplier_Shipment_Code,
			Supplier_Shipment_Number,
			Supplier_Shipment_Date,
			Molar_Amount,
			Molar_Unit_FK,
			Amounts_Differ,
			Supplier,
			Plate_Exists
		INTO
			--v_Location_ID_FK,
			v_Plate_Type_ID_FK,
			v_Plate_Format_ID_FK,
			--v_Parent_Plate_ID_FK,
			v_Status_ID_FK,
			v_Group_Name,
			v_Library_ID_FK,
			v_FT_Cycles,
			v_Weight_Unit_FK,
			v_Qty_Unit_FK,
			v_Solvent_ID_FK,
			v_Concentration,
			v_Conc_Unit_FK,
			l_supplierBarcode,
			v_Supplier_Shipment_Code,
			v_Supplier_Shipment_Number,
			v_Supplier_Shipment_Date,
			v_Molar_Amount,
			v_Molar_Unit_FK,
			v_Amounts_Differ,
			v_Supplier,
			v_Plate_Exists
		FROM Inv_Plates
		WHERE plate_ID = vPlateIDList_t(i);

		INSERT INTO Inv_Plates(
			Location_ID_FK,
			Plate_Type_ID_FK,
			Plate_Format_ID_FK,
			--Parent_Plate_ID_FK,
			Status_ID_FK,
			Group_Name,
			Library_ID_FK,
			FT_Cycles,
			Weight_Unit_FK,
			Qty_Unit_FK,
			Solvent_ID_FK,
			Concentration,
			Conc_Unit_FK,
			Supplier_Barcode,
			Supplier_Shipment_Code,
			Supplier_Shipment_Number,
			Supplier_Shipment_Date,
			Molar_Amount,
			Molar_Unit_FK,
			Amounts_Differ,
			Supplier,
			Plate_Exists,
			Is_Plate_Map  )
		VALUES (
			vLocationIDList_t(i),
			v_Plate_Type_ID_FK,
			v_Plate_Format_ID_FK,
			--vPlateIDList_t(i),
			v_Status_ID_FK,
			v_Group_Name,
			v_Library_ID_FK,
			v_FT_Cycles,
			v_Weight_Unit_FK,
			v_Qty_Unit_FK,
			v_Solvent_ID_FK,
			v_Concentration,
			v_Conc_Unit_FK,
			l_supplierBarcode,
			v_Supplier_Shipment_Code,
			v_Supplier_Shipment_Number,
			v_Supplier_Shipment_Date,
			v_Molar_Amount,
			v_Molar_Unit_FK,
			v_Amounts_Differ,
			v_Supplier,
			0,
			1
		)
	    RETURNING Plate_ID into NewPlateID;
	    --add parent plate relationship
	    INSERT INTO inv_plate_parent VALUES(vPlateIDList_t(i), NewPlateID);

    	vNewIDs := vNewIDs || TO_CHAR(NewPlateID) ||'|';

		FOR wellformats_rec IN (SELECT * FROM inv_vw_well_format WHERE plate_format_id_fk = v_Plate_Format_ID_FK)
		LOOP
    	-- create the wells
			INSERT INTO inv_wells
				(plate_id_fk, well_format_id_fk, grid_position_id_fk)
				VALUES
				(NewPlateID, wellformats_rec.well_format_id_fk, wellformats_rec.grid_position_id)
				returning well_id INTO vNewWellID;

      -- add the well compounds
			FOR compound_rec IN (SELECT compound_id_fk, reg_id_fk, batch_number_fk FROM inv_wells, inv_well_compounds WHERE well_id = well_id_fk(+) AND plate_id_fk = vPlateIDList_t(i) AND grid_position_id_fk = wellformats_rec.grid_position_id)
      LOOP
      	INSERT INTO inv_well_compounds
        	(well_id_fk, compound_id_fk, reg_id_fk, batch_number_fk)
          VALUES
          (vNewWellID, compound_rec.compound_id_fk, compound_rec.reg_id_fk, compound_rec.batch_number_fk);
			END LOOP;
  	END LOOP;

  	END LOOP;

    vNewIDs := SUBSTR(vNewIDs, 0, LENGTH(vNewIDs)-1);
RETURN vNewIDs;

exception
WHEN VALUE_ERROR then
  RETURN -123;

END CREATEPLATEMAP;
/
