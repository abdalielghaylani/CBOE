<%
schemaName = Application("ORASCHEMANAME")
dateFormatString = Application("DATE_FORMAT_STRING")
SQL = "SELECT w.WELL_ID AS WELL_ID, " & _
		"w.well_format_id_fk AS WELL_FORMAT_ID_FK, " & _
		"(SELECT enum_value from inv_enumeration WHERE enum_id =  w.well_format_id_fk) AS WELL_FORMAT, " & _
		"w.plate_id_fk AS PLATE_ID_FK, " & _
		"p.plate_format_id_fk AS PLATE_FORMAT_ID_FK, " & _
		"(SELECT plate_format_name FROM inv_plate_format WHERE plate_format_id = p.plate_format_id_fk) AS PLATE_FORMAT_NAME, " & _
		"w.grid_position_id_fk AS GRID_POSITION_ID_FK, " & _
		"(SELECT name FROM inv_grid_position WHERE grid_position_id = w.grid_position_id_fk) AS GRID_POSITION_NAME, " & _
		"w.weight AS WEIGHT, " & _
		"w.weight_unit_fk AS WEIGHT_UNIT_FK, " & _
		"DECODE(w.WEIGHT, NULL, ' ', w.WEIGHT||' '||UOW.Unit_Abreviation) AS WeightString , " & _
		"round(w.qty_initial,9) AS QTY_INITIAL, " & _
		"round(w.qty_remaining,9) AS QTY_REMAINING, " & _
		"w.qty_unit_fk AS QTY_UNIT_FK, " & _
		"DECODE(w.qty_remaining, NULL, ' ', round(w.qty_remaining,5)||' '||UOM.Unit_Abreviation) AS QtyRemainingString , " & _
		"DECODE(w.qty_initial, NULL, ' ', round(w.qty_initial,5)||' '||UOM.Unit_Abreviation) AS QtyInitialString , " & _
		"s.solvent_name AS SOLVENT, " & _
		"w.solvent_id_fk AS SOLVENT_ID_FK, " & _
		"round(w.concentration,9) AS CONCENTRATION, " & _
		"w.conc_unit_fk AS CONC_UNIT_FK, " & _
		"DECODE(w.concentration, NULL, ' ', round(w.concentration,5)||' '||UOC.Unit_Abreviation) AS ConcentrationString , " & _
		"DECODE(w.solvent_volume, NULL, ' ', round(w.solvent_volume,5) || ' ' || UOSV.Unit_Abreviation) AS Solvent_Volume_String , " & _
		"w.solution_volume," & _
		"DECODE(w.solution_volume, NULL, ' ', round(w.solution_volume,5) || ' ' || UOSV.Unit_Abreviation) AS Solution_Volume_String , " & _
		"round(w.solvent_volume,9) as SOLVENT_VOLUME , " & _
		"w.solvent_volume_unit_id_fk as SOLVENT_VOLUME_UNIT_ID_FK , " & _
		"to_char(w.molar_amount, '0D00EEEE') AS MOLAR_AMOUNT, " & _
		"w.molar_unit_fk AS MOLAR_UNIT_FK, " & _
		"DECODE(to_char(w.molar_amount, '0D00EEEE'), NULL, ' ', to_char(w.molar_amount, '0D00EEEE') ||' '||UOMLR.Unit_Abreviation) AS MolarAmountString , " & _
		"DECODE(to_char(w.molar_conc, '0D00EEEE'), NULL, ' ', to_char(w.molar_conc, '0D00EEEE')) AS MolarConcString , " & _
		"UOW.unit_id AS UOWID, " & _
		"UOW.unit_name AS UOWName, " & _
		"UOW.unit_abreviation AS UOWAbv, " & _
		"UOM.unit_id AS UOMID, " & _
		"UOM.unit_name AS UOMName, " & _
		"UOM.unit_abreviation AS UOMAbv, " & _
		"UOC.unit_id AS UOCID, " & _
		"UOC.unit_name AS UOCName, " & _
		"UOC.unit_abreviation AS UOCAbv, " & _
		"UOMLR.unit_id AS UOMLRID, " & _
		"UOMLR.unit_name AS UOMLRName, " & _
		"UOMLR.unit_abreviation AS UOMLRAbv, " & _
		"w.FIELD_1 AS FIELD_1, " & _
		"w.FIELD_2 AS FIELD_2, " & _
		"w.FIELD_3 AS FIELD_3, " & _
		"w.FIELD_4 AS FIELD_4, " & _
		"w.FIELD_5 AS FIELD_5, " & _
		"TO_CHAR(w.DATE_1,'" & dateFormatString & "') AS DATE_1, " & _
		"TO_CHAR(w.DATE_2,'" & dateFormatString & "') AS DATE_2, " & _
		schemaName & ".guiUtils.GETPARENTWELLIDS(w.well_id) AS PARENT_WELL_ID, " & _
		schemaName & ".guiUtils.GETPARENTWELLNAMES(w.well_id) AS PARENT_WELL_NAMES, " & _
		schemaName & ".guiUtils.GETPARENTWELLLABELS(w.well_id) AS PARENT_WELL_LABELS, " & _
		schemaName & ".guiUtils.GETPARENTPLATELOCATIONIDS2(w.well_id) AS PARENT_PLATE_LOCATION_ID_FK, " & _
		schemaName & ".guiUtils.GETPARENTPLATEIDS2(w.well_id) AS PARENT_PLATE_ID_FK, " & _
		schemaName & ".guiUtils.GETPARENTPLATEBARCODES2(w.well_id) AS PARENT_PLATE_BARCODES, " & _
		"p.plate_barcode AS PLATE_BARCODE, " & _
		"p.location_id_fk AS LOCATION_ID_FK, " & _
		"(SELECT count(*) FROM inv_well_compounds WHERE well_id_fk = w.well_id) as CompoundCount, " & _
		"(SELECT count(*) FROM inv_well_parent WHERE child_well_id_fk = w.well_id) as ParentCount " & _
		"FROM inv_wells w " & _
		", INV_UNITS UOW " & _
		", INV_UNITS UOM " & _
		", INV_UNITS UOC " & _
		", INV_UNITS UOMLR " & _
		", INV_UNITS UOSV " & _
		", INV_SOLVENTS s " & _
		", INV_PLATES p " & _
		"WHERE w.weight_unit_fk = UOW.unit_id(+) " & _
		"AND w.qty_unit_fk = UOM.unit_id(+) " & _
		"AND w.conc_unit_fk = UOC.unit_id(+) " & _
		"AND w.molar_unit_fk = UOMLR.unit_id(+) " & _
		"AND w.solvent_volume_unit_id_fk = UOSV.unit_id(+) " & _
		"AND w.solvent_id_fk = s.solvent_id(+) " & _
		"AND w.plate_id_fk = p.plate_id " & _
		"AND "

FlatWellSQL = "SELECT w.WELL_ID AS WELL_ID, " & _
		"w.well_format_id_fk AS WELL_FORMAT_ID_FK, " & _
		"(SELECT enum_value from inv_enumeration WHERE enum_id =  w.well_format_id_fk) AS WELL_FORMAT, " & _
		"w.plate_id_fk AS PLATE_ID_FK, " & _
		"p.plate_format_id_fk AS PLATE_FORMAT_ID_FK, " & _
		"(SELECT plate_format_name FROM inv_plate_format WHERE plate_format_id = p.plate_format_id_fk) AS PLATE_FORMAT_NAME, " & _
		"w.grid_position_id_fk AS GRID_POSITION_ID_FK, " & _
		"(SELECT name FROM inv_grid_position WHERE grid_position_id = w.grid_position_id_fk) AS GRID_POSITION_NAME, " & _
		"w.weight AS WEIGHT, " & _
		"w.weight_unit_fk AS WEIGHT_UNIT_FK, " & _
		"DECODE(w.WEIGHT, NULL, ' ', w.WEIGHT||' '||UOW.Unit_Abreviation) AS WeightString , " & _
		"round(w.qty_initial,9) AS QTY_INITIAL, " & _
		"round(w.qty_remaining,9) AS QTY_REMAINING, " & _
		"w.qty_unit_fk AS QTY_UNIT_FK, " & _
		"DECODE(w.qty_remaining, NULL, ' ', round(w.qty_remaining,5)||' '||UOM.Unit_Abreviation) AS QtyRemainingString , " & _
		"DECODE(w.qty_initial, NULL, ' ', round(w.qty_initial,5)||' '||UOM.Unit_Abreviation) AS QtyInitialString , " & _
		"s.solvent_name AS SOLVENT, " & _
		"w.solvent_id_fk AS SOLVENT_ID_FK, " & _
		"round(w.concentration,9) AS CONCENTRATION, " & _
		"w.conc_unit_fk AS CONC_UNIT_FK, " & _
		"DECODE(w.concentration, NULL, ' ', round(w.concentration,5)||' '||UOC.Unit_Abreviation) AS ConcentrationString , " & _
		"DECODE(w.solvent_volume, NULL, ' ', w.solvent_volume || ' ' || UOSV.Unit_Abreviation) AS Solvent_Volume_String , " & _
		"round(w.solution_volume,9) as solution_volume ," & _
		"DECODE(w.solution_volume, NULL, ' ', round(w.solution_volume,5) || ' ' || UOSV.Unit_Abreviation) AS Solution_Volume_String , " & _
		"round(w.solvent_volume,9) as SOLVENT_VOLUME , " & _
		"w.solvent_volume_unit_id_fk as SOLVENT_VOLUME_UNIT_ID_FK , " & _
		"to_char(w.molar_amount, '0D00EEEE') AS MOLAR_AMOUNT, " & _
		"w.molar_unit_fk AS MOLAR_UNIT_FK, " & _
		"DECODE(to_char(w.molar_amount, '0D00EEEE'), NULL, ' ', to_char(w.molar_amount, '0D00EEEE') ||' '||UOMLR.Unit_Abreviation) AS MolarAmountString , " & _
        "DECODE(to_char(w.molar_conc, '0D00EEEE'), NULL, ' ', to_char(w.molar_conc, '0D00EEEE')) AS MolarConcString , " & _
		"UOW.unit_id AS UOWID, " & _
		"UOW.unit_name AS UOWName, " & _
		"UOW.unit_abreviation AS UOWAbv, " & _
		"UOM.unit_id AS UOMID, " & _
		"UOM.unit_name AS UOMName, " & _
		"UOM.unit_abreviation AS UOMAbv, " & _
		"UOC.unit_id AS UOCID, " & _
		"UOC.unit_name AS UOCName, " & _
		"UOC.unit_abreviation AS UOCAbv, " & _
		"UOMLR.unit_id AS UOMLRID, " & _
		"UOMLR.unit_name AS UOMLRName, " & _
		"UOMLR.unit_abreviation AS UOMLRAbv, " & _
		"w.FIELD_1 AS FIELD_1, " & _
		"w.FIELD_2 AS FIELD_2, " & _
		"w.FIELD_3 AS FIELD_3, " & _
		"w.FIELD_4 AS FIELD_4, " & _
		"w.FIELD_5 AS FIELD_5, " & _
		"TO_CHAR(w.DATE_1,'" & dateFormatString & "') AS DATE_1, " & _
		"TO_CHAR(w.DATE_2,'" & dateFormatString & "') AS DATE_2, " & _
		schemaName & ".guiUtils.GETPARENTWELLIDS(w.well_id) AS PARENT_WELL_ID, " & _
		schemaName & ".guiUtils.GETPARENTWELLNAMES(w.well_id) AS PARENT_WELL_NAMES, " & _
		schemaName & ".guiUtils.GETPARENTWELLLABELS(w.well_id) AS PARENT_WELL_LABELS, " & _
		schemaName & ".guiUtils.GETPARENTPLATELOCATIONIDS2(w.well_id) AS PARENT_PLATE_LOCATION_ID_FK, " & _
		schemaName & ".guiUtils.GETPARENTPLATEIDS2(w.well_id) AS PARENT_PLATE_ID_FK, " & _
		schemaName & ".guiUtils.GETPARENTPLATEBARCODES2(w.well_id) AS PARENT_PLATE_BARCODES, " & _
		"p.plate_barcode AS PLATE_BARCODE, " & _
		"p.location_id_fk AS LOCATION_ID_FK, " & _
		"wc.compound_id_fk AS COMPOUND_ID_FK, " & _
	    "cpd.Substance_Name AS Substance_Name, " & vblf & _
		"cpd.CAS AS CAS, " & vblf & _
		"cpd.ACX_ID AS ACX_ID, " & vblf & vblf
		if Application("RegServerName") <> "NULL" then
			FlatWellSQL = FlatWellSQL & "DECODE(wc.Reg_ID_FK, NULL, NULL, (Select RegBatchID FROM cheminvdb2.inv_vw_reg_batches ivrg WHERE ivrg.regid = wc.reg_id_fk AND ivrg.batchnumber= wc.Batch_Number_FK)) AS Reg_Batch_ID, "
			FlatWellSQL = FlatWellSQL & "DECODE(wc.Reg_ID_FK, NULL, NULL, (Select RegNumber FROM cheminvdb2.inv_vw_reg_batches ivrg WHERE ivrg.regid = wc.reg_id_fk AND ivrg.batchnumber= wc.Batch_Number_FK)) AS Reg_Number, "
			FlatWellSQL = FlatWellSQL & "DECODE(wc.Reg_ID_FK, NULL, cpd.BASE64_CDX, (SELECT BASE64_CDX AS REG_BASE64_CDX FROM cheminvdb2.inv_vw_reg_structures ivrs WHERE ivrs.regid = wc.reg_id_fk)) AS Base64_CDX, "
		else
			FlatWellSQL = FlatWellSQL & "cpd.BASE64_CDX AS BASE64_CDX, " & vblf
	    End if
	    FlatWellSQL = FlatWellSQL & "wc.Reg_ID_FK,  " & vblf & _
		"wc.Batch_Number_FK,  " & vblf & _
		"(SELECT count(*) FROM inv_well_compounds WHERE well_id_fk = w.well_id) as CompoundCount, " & _
		"(SELECT count(*) FROM inv_well_parent WHERE child_well_id_fk = w.well_id) as ParentCount " & _
		"FROM inv_wells w " & _
		", INV_WELL_COMPOUNDS WC " & _
		", INV_COMPOUNDS cpd " & _
		", INV_UNITS UOW " & _
		", INV_UNITS UOM " & _
		", INV_UNITS UOC " & _
		", INV_UNITS UOMLR " & _
		", INV_UNITS UOSV " & _
		", INV_SOLVENTS s " & _
		", INV_PLATES p " & _
		"WHERE w.weight_unit_fk = UOW.unit_id(+) " & _
		"AND w.qty_unit_fk = UOM.unit_id(+) " & _
		"AND w.conc_unit_fk = UOC.unit_id(+) " & _
		"AND w.molar_unit_fk = UOMLR.unit_id(+) " & _
		"AND w.solvent_volume_unit_id_fk = UOSV.unit_id(+) " & _
		"AND w.solvent_id_fk = s.solvent_id(+) " & _
		"AND w.well_id = wc.well_id_fk(+) " & _
		"AND wc.compound_id_fk = cpd.compound_id(+) " & _
		"AND w.plate_id_fk = p.plate_id " & _
		"AND "


WellCompoundSQL = "SELECT " & _
		"wc.well_compound_id, " & _
		"wc.well_id_fk, " & _
		"wc.compound_id_fk AS COMPOUND_ID_FK, " & _
	    "cpd.Substance_Name AS Substance_Name, " & vblf & _
		"cpd.CAS AS CAS, " & vblf & _
		"cpd.ACX_ID AS ACX_ID, " & vblf & vblf
		if Application("RegServerName") <> "NULL" then
			WellCompoundSQL = WellCompoundSQL & "DECODE(wc.Reg_ID_FK, NULL, NULL, (Select RegBatchID FROM cheminvdb2.inv_vw_reg_batches ivrg WHERE ivrg.regid = wc.reg_id_fk AND ivrg.batchnumber= wc.Batch_Number_FK)) AS Reg_Batch_ID, "
			WellCompoundSQL = WellCompoundSQL & "DECODE(wc.Reg_ID_FK, NULL, cpd.BASE64_CDX, (SELECT BASE64_CDX AS REG_BASE64_CDX FROM cheminvdb2.inv_vw_reg_structures ivrs WHERE ivrs.regid = wc.reg_id_fk)) AS Base64_CDX, "
			WellCompoundSQL = WellCompoundSQL & "DECODE(wc.Reg_ID_FK, NULL, NULL, (Select RegBatchID FROM cheminvdb2.inv_vw_reg_batches ivrg WHERE ivrg.regid = wc.reg_id_fk AND ivrg.batchnumber= wc.Batch_Number_FK)) AS Reg_Number, "
		else
			WellCompoundSQL = WellCompoundSQL & "cpd.BASE64_CDX AS BASE64_CDX, " & vblf
	    End if

	    WellCompoundSQL = WellCompoundSQL & "wc.Reg_ID_FK,  " & vblf & _
		"wc.Batch_Number_FK  " & vblf & _
		"FROM " & _
		" INV_WELL_COMPOUNDS WC " & _
		", INV_COMPOUNDS cpd " & _
		" WHERE " & _
		" wc.compound_id_fk = cpd.compound_id(+) " & _
		"AND "
		'"AND w.well_id = wc.well_id_fk(+) " & _

WellCompoundSQL2 = "SELECT " & _
		"wc.well_compound_id, " & _
		"wc.well_id_fk, " & _
		"wc.compound_id_fk AS COMPOUND_ID_FK, " & _
	    "cpd.Substance_Name AS Substance_Name, " & vblf & _
		"cpd.CAS AS CAS, " & vblf & _
		"cpd.ACX_ID AS ACX_ID, " & vblf & vblf
		if Application("RegServerName") <> "NULL" then
			WellCompoundSQL2 = WellCompoundSQL2 & "DECODE(wc.Reg_ID_FK, NULL, NULL, (Select RegBatchID From inv_vw_reg_batches ivrb WHERE ivrb.regid = wc.reg_id_fk AND ivrb.batchnumber= wc.Batch_Number_FK)) AS Reg_Batch_ID, "
	    End if
	    WellCompoundSQL2 = WellCompoundSQL2 & "wc.Reg_ID_FK,  " & vblf & _
		"wc.Batch_Number_FK  " & vblf & _
		"FROM " & _
		" INV_WELL_COMPOUNDS WC " & _
		", INV_COMPOUNDS cpd " & _
		" WHERE " & _
		" wc.compound_id_fk = cpd.compound_id(+) " & _
		"AND "


		'"w.compound_id_fk AS COMPOUND_ID_FK, " & _
	    '"cpd.Substance_Name AS Substance_Name, " & vblf & _
		'if Application("RegServerName") <> "NULL" then
		'	ParentWellSQL = ParentWellSQL & "DECODE(w.Reg_ID_FK, NULL, NULL, (SELECT reg_numbers.reg_number||'-'||batches.batch_Number FROM reg_numbers, batches WHERE reg_numbers.reg_id = batches.reg_internal_id AND reg_numbers.reg_id= w.reg_id_fk AND batches.batch_number= w.Batch_Number_FK)) AS Reg_Batch_ID, "
		'	ParentWellSQL = ParentWellSQL & "DECODE(w.Reg_ID_FK, NULL, cpd.BASE64_CDX, (SELECT Structures.BASE64_CDX AS REG_BASE64_CDX FROM structures, batches, reg_numbers WHERE reg_numbers.reg_id = batches.reg_internal_id AND structures.CPD_Internal_ID = batches.CPD_Internal_ID AND reg_numbers.reg_id=w.reg_id_fk AND batches.batch_number=w.Batch_Number_FK)) AS Base64_CDX, "
		'else
		'	ParentWellSQL = ParentWellSQL & "cpd.BASE64_CDX AS BASE64_CDX, " & vblf
	    'End if
	    'ParentWellSQL = ParentWellSQL & "w.Reg_ID_FK,  " & vblf & _
		'"w.Batch_Number_FK,  " & vblf & _
		'", INV_COMPOUNDS cpd " & _
		'"WHERE w.compound_id_fk = cpd.compound_id(+) " & _

ParentWellSQL = "SELECT w.WELL_ID AS WELL_ID, " & _
		"w.well_format_id_fk AS WELL_FORMAT_ID_FK, " & _
		"(SELECT enum_value from inv_enumeration WHERE enum_id =  w.well_format_id_fk) AS WELL_FORMAT, " & _
		"w.plate_id_fk AS PLATE_ID_FK, " & _
		"p.plate_format_id_fk AS PLATE_FORMAT_ID_FK, " & _
		"(SELECT plate_format_name FROM inv_plate_format WHERE plate_format_id = p.plate_format_id_fk) AS PLATE_FORMAT_NAME, " & _
		"w.grid_position_id_fk AS GRID_POSITION_ID_FK, " & _
		"gp.name AS GRID_POSITION_NAME, " & _
		"p.plate_barcode AS PLATE_BARCODE, " & _
		"p.location_id_fk AS LOCATION_ID_FK, " & vblf & _
		"'<span id=""Parent Well:"" title=""""><A CLASS=""MenuLink"" HREF=""/cheminv/cheminv/BrowseInventory_frset.asp?PostRelay=1&GotoNode=' || p.location_id_fk || '&SelectContainer=' || w.plate_id_fk || '&SelectWell=' || w.WELL_ID || '&ClearNodes=0"" TITLE=""Parent Well"">' || gp.name  || '</a></span>&nbsp;' as ParentWellLink, " & vblf & _
		"'<span id=""Parent Plate:"" title=""""><A CLASS=""MenuLink"" HREF=""/cheminv/cheminv/BrowseInventory_frset.asp?PostRelay=1&GotoNode=' || p.location_id_fk || '&SelectContainer=' || w.plate_id_fk || '&ClearNodes=0"" TITLE=""Parent Plate"">' || p.plate_barcode  || '</a></span>&nbsp;' as ParentPlateLink " & vblf & _
		"FROM inv_wells w " & _
		", INV_PLATES p " & _
		", INV_GRID_POSITION gp " & _
		"WHERE w.grid_position_id_fk = gp.grid_position_id " & _
		"AND w.plate_id_fk = p.plate_id "


		'"'<span id=""Parent Well:"" title=""""><A CLASS=""MenuLink"" HREF=""/cheminv/cheminv/BrowseInventory_frset.asp?PostRelay=1&GotoNode=' || p.location_id_fk || '&SelectContainer=' || w.plate_id_fk || '&SelectWell=' || w.WELL_ID || '&ClearNodes=0"" TITLE=""Parent Well"" ONCLICK=""SelectLocationNode(0,' || p.location_id_fk  || ', 0, '''',' || w.plate_id_fk || ',1,' || w.well_id || ');"" onmouseover=""javascript:this.style.cursor=''hand'';"" onmouseout=""javascript:this.style.cursor=''default'';"">' || gp.name  || '</a></span>&nbsp;' as ParentWellLink, " & vblf & _
		'"'<span id=""Parent Plate:"" title=""""><A CLASS=""MenuLink"" HREF=""/cheminv/cheminv/BrowseInventory_frset.asp?PostRelay=1&GotoNode=' || p.location_id_fk || '&SelectContainer=' || w.plate_id_fk || '&ClearNodes=0"" TITLE=""Parent Plate"" ONCLICK=""SelectLocationNode(0,' || p.location_id_fk  || ', 0, '''',' || w.plate_id_fk || ',1,' || w.well_id || ');"" onmouseover=""javascript:this.style.cursor=''hand'';"" onmouseout=""javascript:this.style.cursor=''default'';"">' || p.plate_barcode  || '</a></span>&nbsp;' as ParentPlateLink " & vblf & _

%>