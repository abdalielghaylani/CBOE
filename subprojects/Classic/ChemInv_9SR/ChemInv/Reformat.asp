<!--#INCLUDE VIRTUAL = "/cheminv/api/apiUtils.asp"-->
<%
Dim Conn
Dim RS

Call GetInvConnection()
SQL = "SELECT plate_id_fk, well_id, row_index, col_index, compound_id_fk, qty_unit_fk, concentration, " _
	& "conc_unit_fk, solvent_volume_unit_id_fk, molar_conc, reg_id_fk, batch_number_fk " _
	& "FROM inv_wells w, inv_grid_position g " _
	& "WHERE w.grid_position_id_fk = g.grid_position_id " _
		& "AND plate_id_fk IN (1095,1096,1100,1101) " _ 
		& "ORDER BY plate_id_fk, row_index, col_index "
Set RS = Conn.Execute(SQL)

Set oMap = Server.CreateObject("MSXML2.DOMDocument")
oMap.load(Server.MapPath("/cheminv/config/xml_Templates/4-96WellTo1-384Well.xml"))


Set oSourceWellNodes = oMap.selectNodes("/REFORMAT_MAP/SOURCE_PLATE/ROW/COL/WELL")
if oSourceWellNodes.length > 0 then
	For each oNode in oSourceWellNodes
		wellID = RS("well_ID")
		compoundIDFK = RS("compound_id_fk")
		qtyUnitFK = RS("qty_unit_fk")
		concentration = RS("concentration")
		concUnitFK = RS("conc_unit_fk")
		solventVolumeUnitIDFK = RS("solvent_volume_unit_id_fk")
		molarConc = RS("molar_conc")
		regIDFK = RS("reg_id_fk")
		batchNumberFK = RS("batch_number_fk")
		
		oNode.setAttribute "WELLID", RS("well_ID")
		if not isNull(compoundIDFK) then oNode.setAttribute "COMPOUND_ID_FK", compoundIDFK
		if not isNull(qtyUnitFK) then oNode.setAttribute "QTY_UNIT_FK", qtyUnitFK
		if not isNull(concentration) then oNode.setAttribute "CONCENTRATION", concentration
		if not isNull(concUnitFK) then oNode.setAttribute "CONC_UNIT_FK", concUnitFK
		if not isNull(solventVolumeUnitIDFK) then oNode.setAttribute "SOLVENT_VOLUME_UNIT_ID_FK", solventVolumeUnitIDFK
		if not isNull(molarConc) then oNode.setAttribute "MOLAR_CONC", molarConc
		if not isNull(regIDFK) then oNode.setAttribute "REG_ID_FK", regIDFK
		if not isNull(batchNumberFK) then oNode.setAttribute "BATCH_NUMBER_FK", batchNumberFK
		'Response.Write oNode.getAttribute("") & "<BR>"
		RS.MoveNext
	Next
end if

Set oTargetWellNodes = oMap.selectNodes("/REFORMAT_MAP/TARGET_PLATE/ROW/COL/SOURCE")
if oTargetWellNodes.length > 0 then
	For each oNode in oTargetWellNodes
		plateNum = oNode.getAttribute("PLATENUM")
		rowID = oNode.getAttribute("ROWID")
		colID = oNode.getAttribute("COLID")
		if len(plateNum) > 0 then
			'get the wellID
			Set oSourceWell = oMap.selectNodes("/REFORMAT_MAP/SOURCE_PLATE[@PLATENUM = " & plateNum & "]/ROW[@ID = '" & rowID & "']/COL[@ID = " & colID & "]/WELL")
			For each oNode2 in oSourceWell
				wellID = oNode2.getAttribute("WELLID")
				compoundIDFK = oNode2.getAttribute("COMPOUND_ID_FK")
				qtyUnitFK = oNode2.getAttribute("QTY_UNIT_FK")
				concentration = oNode2.getAttribute("CONCENTRATION")
				concUnitFK = oNode2.getAttribute("CONC_UNIT_FK")
				solventVolumeUnitIDFK = oNode2.getAttribute("SOLVENT_VOLUME_UNIT_ID_FK")
				molarConc = oNode2.getAttribute("MOLAR_CONC")
				regIDFK = oNode2.getAttribute("REG_ID_FK")
				batchNumberFK = oNode2.getAttribute("BATCH_NUMBER_FK")
				
				oNode.setAttribute "WELLID", wellID
				if not isNull(compoundIDFK) then oNode.setAttribute "COMPOUND_ID_FK", compoundIDFK
				if not isNull(qtyUnitFK) then oNode.setAttribute "QTY_UNIT_FK", qtyUnitFK
				if not isNull(concentration) then oNode.setAttribute "CONCENTRATION", concentration
				if not isNull(concUnitFK) then oNode.setAttribute "CONC_UNIT_FK", concUnitFK
				if not isNull(solventVolumeUnitIDFK) then oNode.setAttribute "SOLVENT_VOLUME_UNIT_ID_FK", solventVolumeUnitIDFK
				if not isNull(molarConc) then oNode.setAttribute "MOLAR_CONC", molarConc
				if not isNull(regIDFK) then oNode.setAttribute "REG_ID_FK", regIDFK
				if not isNull(batchNumberFK) then oNode.setAttribute "BATCH_NUMBER_FK", batchNumberFK

				'Response.Write plateNum & "=plateNum : " & rowID & "=rowID : " & colID & "=colID : " &  wellID & "=wellID<BR>"
			next			
		end if
	Next
end if

oMap.save(Server.MapPath("/cheminv/config/xml_templates/reformatTest.xml"))


%>