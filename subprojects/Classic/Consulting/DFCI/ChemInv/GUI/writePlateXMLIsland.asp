<%
Response.Expires = -1
Dim colName_arr
Dim cellWidth
Dim regConn

bCheckSelected = false

'plateID = request("PlateID")
if Application("RegServerName") <> "NULL" then
SQL =	"SELECT DISTINCT v.WELL_ID, " & _
		"		REG_ID_FK, " & _
		"		BATCH_NUMBER_FK, " & _
		"		Decode(REG_ID_FK,NULL,NULL,inv_vw_reg_batches.RegBatchID) as RegBatchID, " & _
		"		(SELECT enum_value from inv_enumeration where enum_id = v.WELL_FORMAT_ID_FK) AS WellFormat, " & _
		"		v.CAS, " & _
		"		v.SUBSTANCE_NAME, " & _
		"		v.COMPOUND_ID_FK, " & _
		"		v.QTY_INITIAL, " & _
		"		v.QTY_REMAINING, " & _
		"		DECODE(v.Qty_Initial, NULL, NULL, v.Qty_Initial||' '||UOM.Unit_Abreviation) AS Amount_Initial, " & _
		"		DECODE(v.Qty_Remaining, NULL, NULL, v.Qty_Remaining||' '||UOM.Unit_Abreviation) AS Amount_Remaining, " & _
		"		v.WEIGHT, " & _
		"		DECODE(v.WEIGHT, NULL, NULL, v.WEIGHT||' '||UOW.Unit_Abreviation) AS Weight_String , " & _
		"		v.SOLVENT, " & _
		"		DECODE(v.concentration, NULL, NULL, v.concentration||' '||UOC.Unit_Abreviation) AS Concentration_String , " & _
		"		v.CONCENTRATION, " & _
		"		v.ROW_INDEX, " & _
		"		v.COL_INDEX, " & _
		"		v.ROW_NAME AS RowName," & _
		"		v.COL_NAME AS ColName, " & _
		"		v.NAME, " & _
		"		v.SORT_ORDER,  " & _
		"		DECODE(v.COMPOUND_ID_FK,NULL,DECODE(REG_ID_FK, NULL, NULL, Cscartridge.MolWeight(inv_vw_reg_structures.BASE64_CDX)), Cscartridge.MolWeight(inv_compounds.base64_cdx)) as mw " & _
		"FROM	CHEMINVDB2.INV_VW_WELL v, cheminvdb2.inv_vw_reg_batches , inv_compounds " & _
		"		, cheminvdb2.inv_vw_reg_structures " & _
		"		, INV_UNITS UOM " & _
		"		, INV_UNITS UOC " & _
		"		, INV_UNITS UOW " & _
		"WHERE reg_id_fk = inv_vw_reg_batches.regid (+) " & _
		"		AND	batch_number_fk = inv_vw_reg_batches.batchnumber (+)  " & _	
		"		AND	inv_vw_reg_batches.regid = inv_vw_reg_structures.regid (+) " & _	
		"		AND v.compound_id_fk = compound_id (+) " &_
		"		AND v.qty_unit_fk = UOM.unit_id(+) " & _
		"		AND v.conc_unit_fk = UOC.unit_id(+) " & _
		"		AND v.weight_unit_fk = UOW.unit_id(+) " & _
		"		AND v.plate_id_fk = ? " & _
		"ORDER BY sort_order"
'		"		0 as mw " & _
'		"		cscartridge.molweight(DECODE(v.COMPOUND_ID_FK,NULL,structures.base64_cdx,inv_compounds.base64_cdx)) as mw " & _
else
	SQL =	"SELECT DISTINCT v.WELL_ID, " & _
		"		REG_ID_FK, " & _
		"		BATCH_NUMBER_FK, " & _
		"		(SELECT enum_value from inv_enumeration where enum_id = v.WELL_FORMAT_ID_FK) AS WellFormat, " & _
		"		v.CAS, " & _
		"		v.SUBSTANCE_NAME, " & _
		"		v.COMPOUND_ID_FK, " & _
		"		v.QTY_INITIAL, " & _
		"		v.QTY_REMAINING, " & _
		"		DECODE(v.Qty_Initial, NULL, NULL, v.Qty_Initial||' '||UOM.Unit_Abreviation) AS Amount_Initial, " & _
		"		DECODE(v.Qty_Remaining, NULL, NULL, v.Qty_Remaining||' '||UOM.Unit_Abreviation) AS Amount_Remaining, " & _
		"		v.WEIGHT, " & _
		"		DECODE(v.WEIGHT, NULL, NULL, v.WEIGHT||' '||UOW.Unit_Abreviation) AS Weight_String , " & _
		"		v.SOLVENT, " & _
		"		v.CONCENTRATION, " & _
		"		DECODE(v.concentration, NULL, NULL, v.concentration||' '||UOC.Unit_Abreviation) AS Concentration_String , " & _
		"		v.ROW_INDEX, " & _
		"		v.COL_INDEX, " & _
		"		v.ROW_NAME AS RowName," & _
		"		v.COL_NAME AS ColName, " & _
		"		v.NAME, " & _
		"		v.SORT_ORDER,  " & _
		"		Cscartridge.MolWeight(inv_compounds.base64_cdx) as mw " & _
		"FROM	CHEMINVDB2.INV_VW_WELL v, inv_compounds " & _
		"		, INV_UNITS UOM " & _
		"		, INV_UNITS UOC " & _
		"		, INV_UNITS UOW " & _
		"WHERE v.compound_id_fk = compound_id (+) " &_
		"		AND v.qty_unit_fk = UOM.unit_id(+) " & _
		"		AND v.conc_unit_fk = UOC.unit_id(+) " & _
		"		AND v.weight_unit_fk = UOW.unit_id(+) " & _
		"		AND v.plate_id_fk = ? " & _
		"ORDER BY sort_order"

end if

'show the benzene gif in case writing this xml island takes a long time
Response.Write "<DIV ID=""waitGIF"" ALIGN=""center""><img src=""" & Application("ANIMATED_GIF_PATH") & """ WIDTH=""130"" HEIGHT=""100"" BORDER=""""></DIV>"
Response.Flush
'Response.Write(SQL)
'Response.end
Call GetInvCommand(SQL, adCmdText)
Cmd.Parameters.Append Cmd.CreateParameter("PlateID", 131, 1, 0, plateID)
Set RS = Server.CreateObject("ADODB.Recordset")
RS.CursorLocation = aduseClient
RS.LockType = adLockOptimistic

RS.Open Cmd
RS.ActiveConnection = Nothing

RS.filter = "COL_INDEX=1"

rowName_arr = RS.GetRows(adGetRowsRest, , "RowName")

numRows = Ubound(rowName_arr,2) + 1 
RS.filter = 0
RS.Movefirst
RS.filter = "ROW_INDEX=1"
colName_arr = RS.GetRows(adGetRowsRest, , "ColName")
NumCols = Ubound(colName_arr,2) + 1
cellWidth = 600/NumCols
cellWidthLucidaChars = cellWidth/6
FldArray = split(lcase(displayFields),",")
Response.Write "<xml ID=""xmlDoc""><plate>"
For currRow = 1 to numRows
	Response.Write currRow & ":test"
	For i = 0 to Ubound(FldArray)
		FldName = FldArray(i)
		RS.filter = 0
		RS.Movefirst
		RS.filter = "ROW_INDEX=" & currRow
		rowName = RS("ROWNAME") 
		Response.Write "<" & FldName & ">" & vblf
		Response.Write "<rowname>" & rowname & "</rowname>"
		wellCriterion = Request("WellCriterion")
		if len(wellCriterion) > 0 then
			key = left(wellCriterion,instr(wellCriterion,",")-1)
			value = right(wellCriterion,len(wellCriterion) - instr(wellCriterion,","))
			'Response.Write "key="&key&":value="&value&"<BR>" 
			bCheckSelected = true
		end if
		While NOT RS.EOF
			well_ID = RS("well_id").value	
			theValue = RS(FldName).value
			isSelected = false
			if bCheckSelected then
				keyValue = RS(key).Value
				if isNull(keyValue) then keyValue = ""	
				if cstr(keyValue) = cstr(value) then isSelected = true
			end if
			theValue = "<![CDATA[" & WrapWellContents(FldName, well_ID, theValue, cellWidthLucidaChars, isSelected) & "]]>"
			colIndex = RS("COL_INDEX")
			Response.Write "<col" & colIndex & ">" & theValue & "</col" & colIndex & ">" & vblf
			RS.MoveNext		
		Wend
		Response.Write "</" & FldName & ">" & vblf
	Next
Next
Response.Write "</plate></xml>"


%>
<SCRIPT LANGUAGE=vbscript RUNAT=Server>
Function WrapWellContents(FldName, wellID, strText, Length, isSelected)
	Dim str
	
	if (strText = "") OR IsNull(strText) then strText = "-"
	strText2 = "<a hfref=#>" & strText 
	'str = "<span onclick=""viewWell(" & wellID & ");"""
	str = "<span "
	if (len(strText) > Length) AND (strText <> "&nbsp;") then 
		str = str & "title=""" & strText & """>"
		str = str & "<a style=""font-size:7pt;"" href=""ViewWellFrame.asp?wellID=" & wellID &  "&filter=" & FldName & """ target=""wellJSFrame"">" & left(strText, Length)& "</a>"
	else
		if isSelected then
			str = str & "><a style=""font-size:7pt;"" class=""plateView"" style=""color:red;"" href=""ViewWellFrame.asp?wellID=" & wellID &  "&filter=" & FldName & """ target=""wellJSFrame"" title=""click to view well details"">" & strText & "</a>"
		else
			str = str & "><a style=""font-size:7pt;"" class=""plateView"" href=""ViewWellFrame.asp?wellID=" & wellID &  "&filter=" & FldName & """ target=""wellJSFrame"" title=""click to view well details"">" & strText & "</a>"
		end if

	end if
	
	str = str & "</span>"
	WrapWellContents = str
End function

</SCRIPT>



