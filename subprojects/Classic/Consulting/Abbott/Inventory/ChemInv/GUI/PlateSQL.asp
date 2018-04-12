<%
schemaName = Application("ORASCHEMANAME")
dateFormatString = Application("DATE_FORMAT_STRING")
SQL = "SELECT /*+ ORDERED */ p.PLATE_ID AS PLATE_ID, " & _
		"p.Plate_barcode as PLATE_BARCODE, " & _
		"p.GROUP_NAME, " & _
		"TO_CHAR(p.DATE_CREATED,'" & dateFormatString & "') AS DATE_CREATED, " & _
		"p.Location_ID_FK, " & _
	    "inv_Locations.Location_Name AS Location_Name,  " & vblf & _
		"inv_Locations.Location_Type_ID_FK as Location_Type_ID_FK, " & vblf & _
		"p.QTY_INITIAL, " & _
		"p.QTY_REMAINING, " & _
		"DECODE(p.Qty_Initial, NULL, ' ', round(p.Qty_Initial,2) ||' '||UOM.Unit_Abreviation) AS Amount_Initial, " & _
		"DECODE(p.Qty_Remaining, NULL, ' ', round(p.Qty_Remaining,2) ||' '||UOM.Unit_Abreviation) AS Amount_Remaining, " & _
		"p.plate_type_id_fk AS PLATE_TYPE_ID_FK, " & _
		"p.plate_format_id_fk AS PLATE_FORMAT_ID_FK, " & _
		"(SELECT plate_type_name from inv_plate_types WHERE plate_type_id = p.PLATE_TYPE_ID_FK) AS PLATE_TYPE_NAME, " & _
		"(SELECT plate_format_name from inv_plate_format WHERE plate_format_id = p.PLATE_FORMAT_ID_FK) AS PLATE_FORMAT_NAME, " & _
		"p.CONCENTRATION, " & _
		"DECODE(p.concentration, NULL, ' ', round(p.concentration, 5) ||' '||UOC.Unit_Abreviation) AS Concentration_String , " & _
		"p.SOLVENT_ID_FK, " & _
		"(SELECT solvent_name FROM inv_solvents WHERE solvent_id = p.solvent_id_fk) as SOLVENT, " & _
		"DECODE(p.solvent_volume, NULL, ' ', round(p.solvent_volume,2) || ' ' || UOSV.Unit_Abreviation) AS Solvent_Volume_String , " & _
		"p.solvent_volume as SOLVENT_VOLUME , " & _
		"p.solvent_volume_unit_id_fk as SOLVENT_VOLUME_UNIT_ID_FK , " & _
		"p.SUPPLIER_BARCODE, " & _
		"p.SUPPLIER_SHIPMENT_CODE, " & _
		"p.SUPPLIER_SHIPMENT_NUMBER, " & _
		"TO_CHAR(p.SUPPLIER_SHIPMENT_DATE,'" & dateFormatString & "') AS SUPPLIER_SHIPMENT_DATE, " & _
		"p.LIBRARY_ID_FK, " & _
		"e.enum_value AS LIBRARY_NAME, " & _
		"(SELECT enum_value from inv_enumeration WHERE enum_id =  p.LIBRARY_ID_FK) AS LIBRARY, " & _
		"p.CONTAINER_ID_FK, " & _
		"p.WEIGHT, " & _
		"p.WEIGHT_UNIT_FK, " & _
		"DECODE(p.WEIGHT, NULL, ' ', round(p.WEIGHT,2) ||' '||UOW.Unit_Abreviation) AS Weight_String , " & _
		"UOW.unit_name AS UOWName, " & _
		"UOW.unit_abreviation AS UOWAbv, " & _
		"p.QTY_UNIT_FK, " & _
		"UOM.unit_name AS UOMName, " & _
		"UOM.unit_abreviation AS UOMAbv, " & _
		"p.CONC_UNIT_FK, " & _
		"UOC.unit_name AS UOCName, " & _
		"UOC.unit_abreviation AS UOCAbv, " & _
		schemaName & ".guiUtils.GetParentPlateIDs(p.plate_id) AS PARENT_PLATE_ID_FK, " & _
		schemaName & ".guiUtils.GetParentPlateBarcodes(p.plate_id) as PARENT_PLATE_BARCODE, " & _
		schemaName & ".guiUtils.GetParentPlateLocationIDs(p.plate_id) AS PARENT_PLATE_LOCATION_ID, " & _

		"p.STATUS_ID_FK, " & _
		"(SELECT enum_value FROM inv_enumeration, inv_enumeration_set WHERE eset_name = 'Plate Status' and eset_id = eset_id_fk and enum_id = p.STATUS_ID_FK) AS PLATE_STATUS_NAME, " & _
		"p.FT_CYCLES AS FT_CYCLES, " & _
		"p.FIELD_1 AS FIELD_1, " & _
		"p.FIELD_2 AS FIELD_2, " & _
		"p.FIELD_3 AS FIELD_3, " & _
		"p.FIELD_4 AS FIELD_4, " & _
		"p.FIELD_5 AS FIELD_5, " & _
		"TO_CHAR(p.DATE_1,'" & dateFormatString & "') AS DATE_1, " & _
		"TO_CHAR(p.DATE_2,'" & dateFormatString & "') AS DATE_2, " & _
		"p.PLATE_NAME, " & _
		"p.IS_PLATE_MAP " & _
		"FROM " & schemaName & ".INV_PLATES p " & _
		", " & schemaName & ".INV_UNITS UOM " & _
		", " & schemaName & ".INV_UNITS UOC " & _
		", " & schemaName & ".INV_UNITS UOW " & _
		", " & schemaName & ".INV_UNITS UOSV " & _
		", " & schemaName & ".INV_ENUMERATION e " & _
		", " & schemaName & ".INV_LOCATIONS " & _
		"WHERE " & _
 	    "p.Location_ID_FK = inv_Locations.Location_ID  " & _
		"AND p.qty_unit_fk = UOM.unit_id(+) " & _
		"AND p.conc_unit_fk = UOC.unit_id(+) " & _
		"AND p.weight_unit_fk = UOW.unit_id(+) " & _
		"AND p.solvent_volume_unit_id_fk = UOSV.unit_id(+) " & _ 
		"and p.library_id_fk = e.ENUM_ID(+) " & _
		"AND "

		'"p.PARENT_PLATE_ID_FK," & _
		'"(SELECT p2.plate_barcode FROM inv_plates p2 WHERE plate_id = p.parent_plate_id_fk) AS PARENT_PLATE_BARCODE, " & vblf & _
		's"(SELECT p2.location_id_fk FROM inv_plates p2 WHERE plate_id = p.parent_plate_id_fk) AS PARENT_PLATE_LOCATION_ID, " & vblf & _

%>