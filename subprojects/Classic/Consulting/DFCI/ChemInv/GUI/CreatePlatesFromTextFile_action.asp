<!--#INCLUDE VIRTUAL = "/cheminv/api/apiUtils.asp"-->
<!--#INCLUDE VIRTUAL = "/cheminv/gui/guiUtils.asp"-->
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/upload.asp"-->
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/ado.inc"-->

<html>
<head>
<title><%=Application("appTitle")%> -- Create Plates From Text File</title>
<SCRIPT LANGUAGE=javascript src="/cheminv/Choosecss.js"></SCRIPT>
<SCRIPT LANGUAGE=javascript src="/cheminv/gui/refreshGUI.js"></SCRIPT>
<script LANGUAGE="javascript" src="/cheminv/utils.js"></script>
<script language="JavaScript">
	window.focus();
</script>
</head>
<body>
<BR><BR><BR><BR><BR><BR>
<%
Response.Write "<DIV ALIGN=""center"" ID=""processingImage""><img src=""" & Application("ANIMATED_GIF_PATH") & """ WIDTH=""130"" HEIGHT=""100"" BORDER=""""></DIV>"
Response.Flush
%>

<%
'on error resume next

Dim oUpload
Dim sFileName
Dim sPath
Dim rsData 
Dim bDebug
Dim Conn
Dim Cmd
Dim errCount
Dim sError
Dim oFSO, oLog
Dim oStream
Dim Rows
Dim Cols
bDebug = false
bFileTemplateError = false
bFileParseError = false
errCount = 0
sError = "Error(s) occurred.<BR>"
ProblemBarcodes = ""
CreateErrorBarcodes = ""
ProblemSourceBarcodes = ""
Set oSolventDict = server.CreateObject("scripting.dictionary")
InvServerName = Application("InvServerName")

'-- set the script timeout for this page to 30 min
server.ScriptTimeout = (30 * 60)


'Problem barcode types
'1=Duplicate target barcode
'2=Plate couldn't be created barcode
'3=Non-existent source barcode
'4=Target barcode with non-existent source plate(s)
Set oProblemBarcodes1 = server.CreateObject("scripting.dictionary")
Set oProblemBarcodes2 = server.CreateObject("scripting.dictionary")
Set oProblemBarcodes3 = server.CreateObject("scripting.dictionary")
Set oProblemBarcodes4 = server.CreateObject("scripting.dictionary")

Set oUpload = GetUpload()

'get target plate variables
Template = oUpload.Item("Template").value.string
ActionType = oUpload.Item("ActionType").value.string
LocationID = oUpload.Item("iLocation_ID_FK").value.string
PlateType = oUpload.Item("iPlate_Type_ID_FK").value.string
PlateFormat = oUpload.Item("iPlate_Format_ID_FK").value.string
Status = oUpload.Item("iStatus_ID_FK").value.string
InputQtyUnit = oUpload.Item("iQty_Unit_FK").value.string
InputQtyInitial = oUpload.Item("iQty_Initial").value.string
InputConcUnit = oUpload.Item("iConc_Unit_FK").value.string
InputConcentration = oUpload.Item("iConcentration").value.string
InputSolventID = oUpload.Item("iSolvent_ID_FK").value.string
InputSolventVolume = oUpload.Item("iSolvent_Volume").value.string
InputSolventVolumeUnit = oUpload.Item("iSolvent_Volume_Unit_ID_FK").value.string
BarcodeDescID = oUpload.Item("BarcodeDescID").value.string
if InputQtyUnit = "null" then InputQtyUnit = ""
if InputConcUnit = "null" then InputConcUnit = ""
if InputSolventID = "null" then InputSolventID = ""
if InputSolventVolumeUnit = "null" then InputSolventUnit = ""

' Grab the file name
sFileName = oUpload.Item("File1").FileName
' Compile path to save file to
sPath = Server.MapPath("\" & Application("AppKey") & "\custom\uploads\" & sFileName)
' Save the binary data to the file system
oUpload("File1").value.SaveAs sPath
' Release upload object from memory
Set oUpload = Nothing

Call GetInvConnection()

Set oStream = server.CreateObject("ADODB.Stream")
oStream.Open 
oStream.Type = adTypeText
oStream.Charset = "ascii"

' get number of rows and columns for the specified format
SQL = "SELECT row_count, col_count FROM inv_vw_plate_format WHERE plate_format_id = ?"
Set Cmd = GetCommand(Conn, SQL, adCmdText)
Cmd.Parameters.Append Cmd.CreateParameter("PlateFormat", 5, 1, 0, PlateFormat)
Set RS = Cmd.Execute
Rows = cint(RS("row_count"))
Cols = cint(RS("col_count"))
targetRows = Rows
targetCols = Cols

'set the plate delimiter
plateDelimiter = "barcode"

'preprocess the file if necessary (includes barcode validation)
PreProcess sPath, Template, oStream, ActionType

'Response.Write ProblemBarcodes & ":pb"
oStream.Position = 0

'Response.Write oStream.Size & ":size<BR>" & template
'Response.Write oStream.ReadText(-1)
'Response.Write oStream.ReadText(-2)'
'Response.End

'check if the file was parsed at all
if oStream.Size = 0 and errCount = 0 then 
	bFileParseError = true
	errCount = errCount + 1
	sError = sError & "Error: The file could not be parsed using the specified import template.  Check the import template and try again.<BR>"
end if

if not bFileTemplateError and not bFileParseError then

	' get well format info for new plate
	SQL = "SELECT * FROM inv_vw_well_format WHERE plate_format_id_fk = ?  order by row_index, col_index"
	Set Cmd = GetCommand(Conn, SQL, adCmdText)
	Cmd.Parameters.Append Cmd.CreateParameter("PlateFormat", 5, 1, 0, PlateFormat)
	Set rsWell = Server.CreateObject("ADODB.recordset")
	rsWell.CursorLocation = aduseClient
	rsWell.LockType = adLockOptimistic
	rsWell.Open Cmd
	rsWell.ActiveConnection = Nothing
	Set Cmd = nothing
	
	plateNum = 0	
	Set oHTTP = Server.CreateObject("MSXML2.serverXMLHTTP")
	oHTTP.setTimeouts 600000, 600000, 600000, 600000
	Set oPlates = server.CreateObject("MSXML2.DOMDocument.4.0")
	bCreatePlate = false
	bNewPlate = true
	bFirst = true
	bNewSourcePlate = true
	
	currPlateNum = 1
	prevPlateNum = 1
	currPlateBarcode = ""
	prevPlateBarcode = ""
	nextPlateBarcode = ""
	wellNum = 1
	While not oStream.EOS

		arrLogData = split(oStream.ReadText(-2),",")
		'--don't read blank lines
		if ubound(arrLogData) > 0 then
			'--get the mapped data from the stream row
			SourcePlateBarcode = arrLogData(0)
			SourceWellNumber = arrLogData(1)
			TargetPlateBarcode = arrLogData(2)
			TargetWellNumber = arrLogData(3)
			FileQtyInitial = arrLogData(4)
			FileQtyUnit = arrLogData(5)
			FileConcentration = arrLogData(6)
			FileConcUnit = arrLogData(7)
			FileSolventID = arrLogData(8)
			PField1 = arrLogData(9)
			PField2 = arrLogData(10)
			PField3 = arrLogData(11)
			PField4 = arrLogData(12)
			PField5 = arrLogData(13)
			PDate1 = arrLogData(14)
			PDate2 = arrLogData(15)
			WField1 = arrLogData(16)
			WField2 = arrLogData(17)
			WField3 = arrLogData(18)
			WField4 = arrLogData(19)
			WField5 = arrLogData(20)
			WDate1 = arrLogData(21)
			WDate2 = arrLogData(22)
			FileSolventVol = arrLogData(23)
			FileSolventVolUnit = arrLogData(24)
			SolutionVol = arrLogData(25)
			CompoundID = arrLogData(26)
			RegID = arrLogData(27)
			BatchNumber = arrLogData(28)
			if( IsNumeric(arrLogData(29)) ) then
			    SolventDilutionVol = CDbl(arrLogData(29))
			else
			    SolventDilutionVol = 0.0
			end if

			'--use the input fields over the file fields if it exists
			'QtyInitial = iif((IsEmpty(InputQtyInitial) or InputQtyInitial=""),FileQtyInitial,InputQtyInitial)
			'QtyUnit = iif((IsEmpty(InputQtyUnit) or InputQtyUnit=""),FileQtyUnit,InputQtyUnit)
			Concentration = iif((IsEmpty(InputConcentration) or InputConcentration=""),FileConcentration,InputConcentration)
			ConcUnit = iif((IsEmpty(InputConcUnit) or InputConcUnit=""),FileConcUnit,InputConcUnit)
			'SolventID = iif((IsEmpty(InputSolventID) or InputSolventID=""),FileSolventID,InputSolventID)
			'SolventVol = iif((IsEmpty(InputSolventVolume) or InputSolventVolume=""),FileSolventVol,InputSolventVolume)
			'SolventVolUnit = iif((IsEmpty(InputSolventVolumeUnit) or InputSolventVolumeUnit=""),FileSolventVolUnit,InputSolventVolumeUnit)


			'-- set the file field to empty if the input field exists
			'-- the input field will be used for the entire plate 
			QtyInitial = iif((IsEmpty(InputQtyInitial) or InputQtyInitial=""),FileQtyInitial,"")
			QtyUnit = iif((IsEmpty(InputQtyUnit) or InputQtyUnit=""),FileQtyUnit,"")
			SolventID = iif((IsEmpty(InputSolventID) or InputSolventID=""),FileSolventID,"")
			SolventVol = iif((IsEmpty(InputSolventVolume) or InputSolventVolume=""),FileSolventVol,"")
			SolventVolUnit = iif((IsEmpty(InputSolventVolumeUnit) or InputSolventVolumeUnit=""),FileSolventVolUnit,"")
			
			if( IsNumeric(SolventVol)) then
			    SolventVol = CDbl(SolventVol) + SolventDilutionVol
            end if			    
			
			'QtyInitial = iif((IsEmpty(FileQtyInitial) or FileQtyInitial=""),InputQtyInitial,FileQtyInitial)
			'QtyUnit = iif((IsEmpty(FileQtyUnit) or FileQtyUnit=""),InputQtyUnit,FileQtyUnit)
			'Concentration = iif((IsEmpty(FileConcentration) or FileConcentration=""),InputConcentration,FileConcentration)
			'ConcUnit = iif((IsEmpty(FileConcUnit) or FileConcUnit=""),InputConcUnit,FileConcUnit)
			'SolventID = iif((IsEmpty(FileSolventID) or FileSolventID=""),InputSolventID,FileSolventID)
		
			'--set next plate barcode
			nextPlateBarcode = iif((ActionType = "target"), TargetPlateBarcode, SourcePlateBarcode)
			'--set the current plate barcode first time around			
			if bFirst then currPlateBarcode = nextPlateBarcode end if

			'--check for new plate if so flip bCreatePlate flag
			if plateDelimiter = "barcode" then
				if currPlateBarcode <> nextPlateBarcode then bCreateNewPlate = true end if
			elseif plateDelimiter = "numCols" then
				if wellNum > (Rows*Cols) then bCreateNewPlate = true end if
			end if

			'--new plate has been reached, so create the current plate
			if bCreateNewPlate then
				'--only create problem free plates
				if instr(ProblemBarcodes,currPlateBarcode) = 0 then
					'--send the plate to createplatexml.asp
					rsWell.MoveFirst
					newPlateID = createPlate(oPlates)
					if newPlateID <> "-1" then
						PlateIDs = PlateIDs & newPlateID & "|"
					else
						if not oProblemBarcodes2.Exists(currPlateBarcode) then oProblemBarcodes2.Add currPlateBarcode,1
						errCount = errCount + 1
					end if
					Set oPlates = server.CreateObject("MSXML2.DOMDocument.4.0")
				end if
				
				'--increment plate counter and plate barcode
				bCreateNewPlate = false
				prevPlateNum = currPlateNum
				prevPlateBarcode = currPlateBarcode
				currPlateNum = currPlateNum + 1
				currPlateBarcode = nextPlateBarcode
				wellNum = 1
				bNewPlate = true
			end if

			'--create plate xml document
			if bNewPlate  then
				bNewPlate = false
				bFirst = false

				With oPlates
					Set objPI = .createProcessingInstruction("xml", "version=""1.0""")
					.appendChild objPI
					Set oRootNode = .createElement("PLATES")
				    Set .documentElement = oRootNode
				    With oRootNode
						.setAttribute "CSUSERNAME", Session("UserName" & "cheminv")
						.setAttribute "CSUSERID", Session("UserID" & "cheminv")
						.setAttribute "REGISTERCOMPOUNDS", "false"
						.setAttribute "AMT_TAKEN", iif((IsEmpty(InputQtyInitial) or InputQtyInitial=""),"",InputQtyInitial)
						.setAttribute "AMT_TAKEN_UNIT_ID", iif((IsEmpty(InputQtyUnit) or InputQtyUnit=""),"",InputQtyUnit)
						.setAttribute "SOLVENT_ID", iif((IsEmpty(InputSolventID) or InputSolventID=""),"",InputSolventID)
						.setAttribute "SOLVENT_VOLUME", iif((IsEmpty(InputSolventVolume) or InputSolventVolume=""),"",InputSolventVolume)
						.setAttribute "SOLVENT_VOLUME_UNIT_ID", iif((IsEmpty(InputSolventVolumeUnit) or InputSolventVolumeUnit=""),"",InputSolventVolumeUnit)
					End With
				End With
			 
				'--set plate information
				Set oPlateNode = oPlates.createElement("PLATE")
			    oRootNode.appendChild oPlateNode
				With oPlateNode
					.setAttribute "PLATE_TYPE_ID_FK", PlateType
			        .setAttribute "PLATE_FORMAT_ID_FK", PlateFormat
				    .setAttribute "STATUS_ID_FK", Status
					.setAttribute "LOCATION_ID_FK", LocationID
					.setAttribute "QTY_INITIAL", QtyInitial
					.setAttribute "QTY_REMAINING", QtyInitial
			        .setAttribute "QTY_UNIT_FK", QtyUnit
					.setAttribute "CONCENTRATION", Concentration
					.setAttribute "CONC_UNIT_FK", ConcUnit
					.setAttribute "SOLVENT_ID_FK", SolventID
					.setAttribute "SOLVENT_VOLUME", SolventVol
					.setAttribute "SOLVENT_VOLUME_UNIT_ID_FK", SolventVolUnit
					.setAttribute "SOLUTION_VOLUME", SolutionVol
			        if BarcodeDescID = "-1" then
						.setAttribute "PLATE_BARCODE", currPlateBarcode
					else
						.setAttribute "BARCODE_DESC_ID", BarcodeDescID
					end if
					.setAttribute "FIELD_1", PField1
					.setAttribute "FIELD_2", PField2
					.setAttribute "FIELD_3", PField3
					.setAttribute "FIELD_4", PFiel4d
					.setAttribute "FIELD_5", PField5
					.setAttribute "DATE_1", PDate1
					.setAttribute "DATE_2", PDate2
				End With
			    '--add row and column nodes based on the plate format
				For j = 1 To Rows
					Set oRowNode = oPlates.createElement("ROW")
			        oRowNode.setAttribute "ID", j
				    oPlateNode.appendChild oRowNode
					For k = 1 To Cols
						Set oColNode = oPlates.createElement("COL")
						oColNode.setAttribute "ID", k
				        oRowNode.appendChild oColNode
					    '--add well node, fill based on plate format
			            rsWell.Filter = adFilterNone
				        rsWell.MoveFirst
					    rsWell.Filter = "ROW_INDEX = " & j & " and COL_INDEX = " & k
   						rsConcentration = rsWell("CONCENTRATION").Value
						if isNull(rsConcentration) then rsConcentration = ""
						rsConcUnit = rsWell("CONC_UNIT_FK").Value
						if isNull(rsConcUnit) then rsConcUnit = ""
						Set oWellNode = oPlates.createElement("WELL")
						With oWellNode
							.setAttribute "WELL_FORMAT_ID_FK", rsWell("WELL_FORMAT_ID_FK").Value
					        .setAttribute "PLATE_FORMAT_ID_FK", rsWell("PLATE_FORMAT_ID_FK").Value
							.setAttribute "GRID_POSITION_ID_FK", rsWell("GRID_POSITION_ID").Value
							.setAttribute "CONCENTRATION", rsConcentration
							.setAttribute "CONC_UNIT_FK", rsConcUnit
						End With
					    oColNode.appendChild oWellNode
					Next
				Next
			end if 

			if ActionType = "target" then
				'--add target well compound and parent well info from the parent source well
				if instr(ProblemBarcodes,TargetPlateBarcode) = 0 then
					'--add compound info defined by the log
					'--check for new source plate
					if currSourcePlate <> SourcePlateBarcode then
						SQL =   "SELECT iwc.compound_id_fk, iwc.reg_id_fk, iwc.batch_number_fk, iw.well_id, igp.sort_order, iw.solution_volume, iw.qty_remaining, iw.qty_unit_fk, iw.solvent_volume, iw.solvent_volume_unit_id_fk, iw.solvent_id_fk, iw.concentration, iw.conc_unit_fk " &_
						        "FROM inv_wells iw, inv_well_compounds iwc, inv_grid_position igp, inv_plates ip " &_
						        "WHERE iw.grid_position_id_fk = igp.grid_position_id " &_
						        "AND ip.plate_id = iw.plate_id_fk " &_
						        "AND iw.well_id = iwc.well_id_fk(+) " &_
						        "AND ip.plate_barcode = ? " &_
						        "ORDER BY igp.sort_order"
						'SQL = "SELECT compound_id_fk, reg_id_fk, batch_number_fk, well_id, sort_order FROM inv_vw_well_flat, inv_plates WHERE plate_barcode = ? and plate_id = plate_id_fk"
						Set Cmd = GetCommand(Conn, SQL, adCmdText)
						Cmd.Parameters.Append Cmd.CreateParameter("PlateBarcode", 200, 1, 255, SourcePlateBarcode)
						Set rsSourceWell = Server.CreateObject("ADODB.recordset")
						rsSourceWell.CursorLocation = aduseClient
						rsSourceWell.LockType = adLockOptimistic
						'Response.Write SQL & SourcePlateBarcode
						'Response.End
						rsSourceWell.Open Cmd
						rsSourceWell.ActiveConnection = Nothing
					end if

					currSourcePlate = SourcePlateBarcode									
					if instr(ProblemBarcodes, currSourcePlate) = 0 then
						rsSourceWell.Filter = adFilterNone
						rsSourceWell.MoveFirst
						rsSourceWell.Filter = "SORT_ORDER = " & SourceWellNumber

						CompoundID = rsSourceWell("COMPOUND_ID_FK")
						if isNull(CompoundID) then CompoundID = ""
						RegID = rsSourceWell("REG_ID_FK")
						if isNull(RegID) then RegID = ""
						BatchNumber = rsSourceWell("BATCH_NUMBER_FK")
						if isNull(BatchNumber) then BatchNumber = ""
						WellID = rsSourceWell("WELL_ID")
						
						WellConc = rsSourceWell("CONCENTRATION")
						if isNull(WellConc) then WellConc = ""
						WellConcUnit = rsSourceWell("CONC_UNIT_FK")
						if isNull(WellConcUnit) then WellConcUnit = ""						
						
						dPercentage = 0.0
						' If SolutionVol is passed and we have a corresponding value in the source well, compute
						' the percentage that's being removed.  Must have the source well solution_volume and
						' solvent_volume_unit_id_fk fields set to other than null, and the SolventVolUnit for the 
						' passed-in record must match that from the database
						if IsNumeric(SolutionVol) and IsNumeric(SolventVolUnit) and not isNull(rsSourceWell("solution_volume")) and not isNull(rsSourceWell("solvent_volume_unit_id_fk")) then
						    ' Make sure the units match up
						    if( CLng(SolventVolUnit) = CLng(rsSourceWell("solvent_volume_unit_id_fk")) ) then
						        dSourceSolutionVolume = CDbl(rsSourceWell("solution_volume"))						        
						        if dSourceSolutionVolume > 0 and dSourceSolutionVolume >= CDbl(SolutionVol) then
						            ' Calculate the ratio based on the existing solution_volume from the source well
    						        dPercentage = CDbl(SolutionVol) / dSourceSolutionVolume
	    					    end if
	    					end if
						end if
						
						' Calculate the solvent and qty to use
						if dPercentage > 0.0 then
						    ' If SolventVol is set, assume it's from a GUI override or in the text file.  Otherwise, 
						    ' calculate the SolventVol using the percentage
						    if (SolventVol = "") and not isNull(rsSourceWell("solvent_volume")) then						        
						        SolventVol = CDbl(rsSourceWell("solvent_volume")) * dPercentage
						    end if
    						
						    ' If QtyInitial is set, assume it's from a GUI override or in the text file.  Otherwise, 
						    ' calculate the initial qty using the percentage here
						    if (QtyInitial = "") and not isNull(rsSourceWell("qty_remaining")) then
						        QtyInitial = CDbl(rsSourceWell("qty_remaining")) * dPercentage 
    						    
						        ' They didn't pass QtyInitial, so QtyUnit has no meaning.  Set/override it 
						        ' with the source well value
						        if not isNull(rsSourceWell("qty_unit_fk")) then
						            QtyUnit = rsSourceWell("qty_unit_fk")
						        end if
						    end if
						end if
						
						' Pick up the same solvent as what was in the source well
						if SolventID = "" and not isNull(rsSourceWell("solvent_id_fk")) then
						    SolventID = rsSourceWell("solvent_id_fk")
						end if
			
						set oColNode = oPlates.selectSinglenode("/PLATES/PLATE/descendant::COL[" & TargetWellNumber & "]")
						set oUpdateNode = oPlates.selectSinglenode("/PLATES/PLATE/descendant::COL[" & TargetWellNumber & "]/WELL[1]")
						ParentWellIDFK = oUpdateNode.GetAttribute("PARENT_WELL_ID_FK")
						GridPositionIDFK = oUpdateNode.GetAttribute("GRID_POSITION_ID_FK")
						'--if this node has been updated then add another
						if len(ParentWellIDFK)>0 then
							Set oWellNode = oPlates.createElement("WELL")
							With oWellNode
								.setAttribute "PARENT_WELL_ID_FK", WellID
								.setAttribute "GRID_POSITION_ID_FK", GridPositionIDFK
								.setAttribute "COMPOUND_ID_FK", CompoundID
								.setAttribute "REG_ID_FK", RegID
								.setAttribute "BATCH_NUMBER_FK", BatchNumber
							End With
							oColNode.appendChild oWellNode
						else
							With oUpdateNode
								.setAttribute "COMPOUND_ID_FK", CompoundID
								.setAttribute "REG_ID_FK", RegID
								.setAttribute "BATCH_NUMBER_FK", BatchNumber
								.setAttribute "PARENT_WELL_ID_FK", WellID
							end with
						end if
					end if
				end if
			end if		

			currWellNumber = iif((ActionType = "target"), TargetWellNumber, SourceWellNumber)
			set oUpdateNode = oPlates.selectSinglenode("/PLATES/PLATE/descendant::COL[" & currWellNumber & "]/WELL[1]")
			With oUpdateNode
				.setAttribute "QTY_INITIAL", QtyInitial
				.setAttribute "QTY_REMAINING", QtyInitial
			    .setAttribute "QTY_UNIT_FK", QtyUnit
			    
			    if Concentration <> "" and not isEmpty(Concentration)then
			        '-- only override the plate format concentration if one is specified
					.setAttribute "CONCENTRATION", Concentration
					.setAttribute "CONC_UNIT_FK", ConcUnit
				else
				    ' Otherwise, use the well-level value.  Concentration should not change when daughtering a plate
				    .setAttribute "CONCENTRATION", WellConc
					.setAttribute "CONC_UNIT_FK", WellConcUnit				    
			    end if
				.setAttribute "SOLVENT_ID_FK", SolventID
				.setAttribute "FIELD_1", WField1
				.setAttribute "FIELD_2", WField2
				.setAttribute "FIELD_3", WField3
				.setAttribute "FIELD_4", WField4
				.setAttribute "FIELD_5", WField5
				.setAttribute "DATE_1", WDate1
				.setAttribute "DATE_2", WDate2
				.setAttribute "SOLVENT_VOLUME", SolventVol
				.setAttribute "SOLVENT_VOLUME_UNIT_ID_FK", SolventVolUnit
				.setAttribute "SOLUTION_VOLUME", SolutionVol
				' The NODBFIELD is to signify that this field is parsed during the upload and does not
				' correspond to an actual database column in inv_wells.  SolventDilutionVol must 
				' be handled separately, as we do not want to decrement more from the parent than
				' was actually used
				.setAttribute "SOLVENT_VOLUME_DILUTION_NODBFIELD", SolventDilutionVol
				.setAttribute "COMPOUND_ID_FK", CompoundID
				.setAttribute "REG_ID_FK", RegID
				.setAttribute "BATCH_NUMBER_FK", BatchNumber
			end with
			
			wellNum = wellNum + 1
		end if
	wend

	'--create the last plate
	if instr(ProblemBarcodes,currPlateBarcode) = 0 then
		'--create plate xml
		rsWell.MoveFirst
		newPlateID = createPlate(oPlates)
		if newPlateID <> "-1" then
			PlateIDs = PlateIDs & newPlateID & "|"
		else
			if not oProblemBarcodes2.Exists(SourcePlateBarcode) then oProblemBarcodes2.Add SourcePlateBarcode,1
			errCount = errCount + 1
		end if
	end if

	if len(PlateIDs) = 0 then
		PlateIDs = "error"
	end if
	arrNewPlateIDs = split(PlateIDs,"|")
end if
%>

<TABLE ALIGN=CENTER BORDER=0 CELLPADDING=0 CELLSPACING=0 BGCOLOR=#ffffff width=90%>
	<TR>
		<TD HEIGHT=50 VALIGN=MIDDLE align="left">
<%
			Response.write "<Script language=javascript>document.all.processingImage.style.display = 'none';</script>"		
			'Response.Write arrNewPlateIDs(0)
			If clng(errCount) > 0 then 
				theAction = "MiscError"
				problemBarcodes1 = ""
				problemBarcodes2 = ""
				problemBarcodes3 = ""
				problemBarcodes4 = ""
				if oProblemBarcodes1.Count > 0 then problemBarcodes1 = Dict2List(oProblemBarcodes1, 10)
				if oProblemBarcodes2.Count > 0 then problemBarcodes2 = Dict2List(oProblemBarcodes2, 10)
				if oProblemBarcodes3.Count > 0 then problemBarcodes3 = Dict2List(oProblemBarcodes3, 10)
				if oProblemBarcodes4.Count > 0 then problemBarcodes4 = Dict2List(oProblemBarcodes4, 10)
			elseIf isNumeric(arrNewPlateIDs(0)) then
				if Clng(arrNewPlateIDs(0)) >= 0 then
					theAction = "Exit"					
				Else
					theAction = "WriteAPIError"
				End if
			Else
				theAction = "WriteOtherError"
			End if
			'Response.Write("<br>action:" & theAction)
			Select Case theAction
				Case "Exit"
					Response.Write "<center><SPAN class=""GuiFeedback"">Plate creation complete.</SPAN></center>"
					if errCount > 0 then
						Response.Write sError
					else
						'Response.Write "<SCRIPT LANGUAGE=javascript>SelectLocationNode(0, " & firstLocationID & ", 0, '" & Session("TreeViewOpenNodes1") & "'," & arrNewPlateIDs(0) & ",1); opener.focus(); window.close();</SCRIPT>" 
						Response.Write "<P><center><a HREF=""3"" onclick=""history.go(-2); return false;""><img SRC=""../graphics/ok_dialog_btn.gif"" border=""0""></a></center>"
					end if
				Case "WriteAPIError"
					Response.Write "<P><CODE>ChemInv API Error: " & Application(out) & "</CODE></P>"
					Response.Write "<P><center><a HREF=""3"" onclick=""history.back(); return false;""><img SRC=""../graphics/ok_dialog_btn.gif"" border=""0""></a></center>"
				Case "WriteOtherError"
					Response.Write "<P><CODE>Oracle Error: " & out & "</code></p>" 
					Response.Write "<P><center><a HREF=""3"" onclick=""history.back(); return false;""><img SRC=""../graphics/ok_dialog_btn.gif"" border=""0""></a></center>"
				Case "MiscError"
					if problemBarcodes1 <> "" then sError = sError & "Error: Plate(s) - " & problemBarcodes1 & " - " & iif(oProblemBarcodes1.Count>1,"were","was") & " not created due to a barcode conflict.<BR>" 
					if problemBarcodes2 <> "" then sError = sError & "<span title=""" & err.Description & """>Error: Plate(s) - " & problemBarcodes2 & " - " & iif(oProblemBarcodes2.Count>1,"were","was") & " not created due to an Oracle error.</span><BR>" 
					if problemBarcodes3 <> "" then sError = sError & "Error: Source plate(s) - " & problemBarcodes3 & " - " & iif(oProblemBarcodes3.Count>1,"do","does") & " not exist in Inventory Manger.  Plate(s) - " & problemBarcodes4 & " - " & iif(oProblemBarcodes4.Count>1,"have","has") & " not been created.<BR>" 
					Response.Write "<SPAN class=""GuiFeedback""><center>" & sError & "</center></SPAN>"
					Response.Write "<P><center><a HREF=""3"" onclick=""history.back(); return false;""><img SRC=""../graphics/ok_dialog_btn.gif"" border=""0""></a></center>"
			End Select
%>
		</TD>
	</TR>
</TABLE>
</Body>			
<%Response.flush%>
<%
Function  createPlate(byRef oPlates)
		'write to the browser to keep it active
		Response.Write " "
		Response.Flush
		if err.Number <> 0 then
			Set oPlateBarcodeNL = oPlates.selectNodes("/PLATES/PLATE/@PLATE_BARCODE")
			if oPlateBarcodeNL.length > 0 then
				For each oNode in oPlateBarcodeNL
					if not oProblemBarcodes2.Exists(oNode.value) then oProblemBarcodes2.Add oNode.value,1			
				Next
			end if
			resultPlateIDs = "-1"
			errCount = errCount + 1
		else
			if Session("CfwTraceLevel") = "20" or Application("CfwTraceLevel") = "20" or bDebug then oPlates.save "c:\test" & plateNum & ".xml"
			URL = "http://" & Application("InvServerName") & "/cheminv/api/CreatePlateXML.asp"
			oHTTP.open "POST", URL, False
			oHTTP.send oPlates.xml
			sRet = oHTTP.responseText
			'Response.Write sRet
			'Response.End
			set oPlates = nothing

			Set oResultXML = server.CreateObject("MSXML2.DOMDocument")
			oResultXML.loadXML sRet
			'Response.Write sRet
			oResultXML.setProperty "SelectionLanguage","XPath"
			Set oPlateIDNL = oResultXML.selectNodes("/CREATE_PLATE_RESULTS/@PLATE_ID")
			if oPlateIDNL.length > 0 then
				For each oNode in oPlateIDNL
					resultPlateIDs = resultPlateIDs & oNode.value & ","
				Next
				resultPlateIDs = left(resultPlateIDs, len(resultPlateIDs)-1)
				'resultPlateIDs = oResultXML.selectSingleNode("/CREATE_PLATE_RESULTS/@PLATE_ID").value
			else
				resultPlateIDs = "-1"
			end if
			createPlate = resultPlateIDs
			Set oResultXML = nothing
		end if
		'Response.Write "test" & ":" & plateNum & ":" & PlateIDs
		'Response.end

End Function

Function plateExists(barcode)
		SQL = "SELECT 1 FROM inv_plates WHERE plate_barcode = ?"
		Set Cmd = GetCommand(Conn, SQL, adCmdText)
		Cmd.Parameters.Append Cmd.CreateParameter("PlateBarcode", 200, 1, 200, barcode)
		Set RS = Cmd.Execute
		if not RS.EOF then
		'if clng(RS("theCount")) > 0 then
			ret = true
		else
			ret = false
		end if
		plateExists = ret
End Function


Sub PreProcess(sPath, Template, ByRef oStream, ActionType)

	'set default values
	headerLength = 0
	Delimiter = ","
	SourceBarcodeCol = null
	SourceWellCol = null
	TargetBarcodeCol = null
	TargetWellCol = null
	QtyInitialCol = null
	QtyUnitCol = null 
	ConcentrationCol = null
	ConcUnitCol = null 
	SolventIDCol = null 
	PField1Col = null 
	PField2Col = null
	PField3Col = null
	PField4Col = null
	PField5Col = null
	PDate1Col = null
	PDate2Col = null
	WField1Col = null
	WField2Col = null
	WField3Col = null
	WField4Col = null 
	WField5Col = null
	WDate1Col = null
	WDate2Col = null
	SolventVolCol = null
	SolventVolUnitCol = null
	SolutionVolCol = null
	CompoundIDCol = null
	RegIDCol = null
	RegNumberCol = null
	BatchNumberCol = null
	SolventDilutionVolCol = null	
	bUseWellCoordinates = false
	NumColumns = 0

	Select Case Template
		
		Case "2"
			'2 Column
			'# Header Rows:	1
			'Delimiter:	tab													
			'Columns:	Target Plate Barcode	Source Plate Barcode
			headerLength = 1
			Delimiter = chr(9)
			SourceBarcodeCol = 1
			TargetBarcodeCol = 0
			bUseWellCoordinates = false
			NumColumns = 2
		Case "3"
			'Hitlist Format
			'# Header Rows:	0
			'Delimiter:	tab													
			'Columns:	Target Plate Barcode	Target Well Coordinate	Source Plate Barcode	Source Well Coordinate
			headerLength = 0
			Delimiter = chr(9)
			SourceBarcodeCol = 2
			SourceWellCol = 3
			TargetBarcodeCol = 0
			TargetWellCol = 1
			bUseWellCoordinates = true
			NumColumns = 4
		Case "4"
			'Unified Transfer Format
			'# Header Rows:	7													
			'Delimiter:	comma													
			'Columns:	Date Created	NA	NA	NA	Source Plate Barcode	Source Well Number	NA	NA	NA	Target Plate Barcode	Target Well Number	NA	NA	NA(amount taken in ul?)
			headerLength = 7
			Delimiter = ","
			SourceBarcodeCol = 4
			SourceWellCol = 5
			TargetBarcodeCol = 9
			TargetWellCol = 10
			bUseWellCoordinates = false
			NumColumns = 14
		Case "5"
			'Plate 1
			'#Header Rows: 1
			'Delimiter: tab
			'Columns: Well Coordinate	NA	NA	Initial Qty	Solvent	Concentration	Lot	com
			headerLength = 1
			Delimiter = chr(9)
			SourceWellCol = 0
			QtyInitialCol = 3
			ConcentrationCol = 5
			SolventIDCol = 4 
			WField1Col = 1
			WField2Col = 6
			bUseWellCoordinates = true
			NumColumns = 8
		Case "6"
			'Plate 1
			'#Header Rows: 1
			'Delimiter: tab
			'Columns: Well Coordinate	NA	NA	Initial Qty	Concentration	Solvent	Lot	com
			headerLength = 1
			Delimiter = chr(9)
			SourceWellCol = 0
			QtyInitialCol = 3
			ConcentrationCol = 4
			SolventIDCol = 5 
			WField1Col = 1
			WField2Col = 6
			bUseWellCoordinates = true
			NumColumns = 8
		Case "7"
			'BTK file format
			'#Header Rows: 0
			'Delimiter: comma
			'Columns: N/A, N/A, Target Plate Barcode, Target Quadrant, Source Plate Barcode, N/A, N/A
			headerLength = 0
			Delimiter = ","
			SourceBarcodeCol = 4
			TargetBarcodeCol = 2
			'mapping target quadrant to targetwell
			TargetWellCol = 3
			bUseWellCoordinates = false
			NumColumns = 7
		Case Else
			'defined by database
			Call GetInvCommand("{CALL " & Application("CHEMINV_USERNAME") & ".DATAMAPS.GetDataMap(?)}", adCmdText)	
			Cmd.Parameters.Append Cmd.CreateParameter("PDATAMAPID",131, 1, 0, Template)
			Cmd.Parameters("PDATAMAPID").Precision = 9	
			Cmd.Properties ("PLSQLRSet") = TRUE  
			Set RS = Cmd.Execute
			Cmd.Properties ("PLSQLRSet") = FALSE
			while (not RS.EOF)
				headerLength = cint(RS("num_header_rows"))
				Delimiter = RS("column_delimiter")
				NumColumns = cint(RS("num_columns"))
				bUseWellCoordinates = cbool(cint(RS("use_well_coordinates")))
				RS.MoveNext
			wend
			'translate tab
			if Delimiter = "tab" then Delimiter = chr(9) end if

			Call GetInvCommand("{CALL " & Application("CHEMINV_USERNAME") & ".DATAMAPS.MapToDefault(?)}", adCmdText)	
			Cmd.Parameters.Append Cmd.CreateParameter("PDATAMAPID",131, 1, 0, Template)
			Cmd.Parameters("PDATAMAPID").Precision = 9	
			Cmd.Properties ("PLSQLRSet") = TRUE  
			Set RS = Cmd.Execute
			Cmd.Properties ("PLSQLRSet") = FALSE
			while not RS.EOF
				colNum = cint(RS("col_num")) - 1
				select case  RS("default_col_num")
					case "1"
						SourceBarcodeCol = colNum
					case "2"
						SourceWellCol = colNum
					case "3"
						TargetBarcodeCol = colNum
					case "4"
						TargetWellCol = colNum
					case "5"
						QtyInitialCol = colNum
					case "6"
						QtyUnitCol = colNum
					case "7"
						ConcentrationCol = colNum
					case "8"
						ConcUnitCol = colNum
					case "9"
						SolventIDCol = colNum
					case "10"
						PField1Col = colNum
					case "11"
						PField2Col = colNum
					case "12"
						PField3Col = colNum
					case "13"
						PField4Col = colNum
					case "14"
						PField5Col = colNum
					case "15"
						PDate1Col = colNum
					case "16"
						PDate2Col = colNum
					case "17"
						WField1Col = colNum
					case "18"
						WField2Col = colNum
					case "19"
						WField3Col = colNum
					case "20"
						WField4Col = colNum
					case "21"
						WField5Col = colNum
					case "22"
						WDate1Col = colNum
					case "23"
						WDate2Col = colNum
					case "24"
						SolventVolCol = colNum
					case "25"
						SolventVolUnitCol = colNum
					case "26"
						SolutionVolCol = colNum
					case "27"
						CompoundIDCol = colNum
					case "28"
						RegIDCol = colNum
					case "29"
						RegNumberCol = colNum
					case "30"
						BatchNumberCol = colNum
					case "32"
						SolventDilutionVolCol = colNum
				end select
				RS.MoveNext
			wend

			'--set the plate delimiter to the number of columns if no barcode is provided
			if ActionType = "source" and isNull(SourceBarcodeCol) then plateDelimiter = "numCols" end if
	End Select

	if Template = "5" or Template = "6" then
		'custom processor for Nissan
		ConvertFileToCSFormat_custom1 sPath, oStream, HeaderLength, Delimiter, SourceBarcodeCol, SourceWellCol, TargetBarcodeCol, TargetWellCol, QtyInitialCol, QtyUnitCol, ConcentrationCol, ConcUnitCol, SolventIDCol,  PField1Col, PField2Col, PField3Col, PField4Col, PField5Col, PDate1Col, PDate2Col, WField1Col, WField2Col, WField3Col, WField4Col, WField5Col, WDate1Col, WDate2Col, bUseWellCoordinates, NumColumns, Template
	else
		ConvertFileToCSFormat sPath, oStream, HeaderLength, Delimiter, SourceBarcodeCol, SourceWellCol, TargetBarcodeCol, TargetWellCol, QtyInitialCol, QtyUnitCol, ConcentrationCol, ConcUnitCol, SolventIDCol, PField1Col, PField2Col, PField3Col, PField4Col, PField5Col, PDate1Col, PDate2Col, WField1Col, WField2Col, WField3Col, WField4Col, WField5Col, WDate1Col, WDate2Col, SolventVolCol, SolventVolUnitCol, SolutionVolCol, CompoundIDCol, RegIDCol, RegNumberCol, BatchNumberCol, SolventDilutionVolCol, bUseWellCoordinates, NumColumns, Template, ActionType
	end if
End Sub

'--parses files into CS Format and updates the passed in stream
Sub ConvertFileToCSFormat(sPath, ByRef oStream, HeaderLength, Delimiter, SourceBarcodeCol, SourceWellCol, TargetBarcodeCol, TargetWellCol, QtyInitialCol, QtyUnitCol, ConcentrationCol, ConcUnitCol, SolventIDCol, PField1Col, PField2Col, PField3Col, PField4Col, PField5Col, PDate1Col, PDate2Col, WField1Col, WField2Col, WField3Col, WField4Col, WField5Col, WDate1Col, WDate2Col, SolventVolCol, SolventVolUnitCol, SolutionVolCol, CompoundIDCol, RegIDCol, RegNumberCol, BatchNumberCol, SolventDilutionVolCol, bUseWellCoordinates, NumColumns, Template, ActionType)
	Dim localRows
	Dim localCols
	'--assume there is a column mapping
	bColumnMapping = true
	if ActionType = "target" then
		if (IsNull(SourceWellCol) or IsNull(TargetWellCol)) then
			'--no column mapping so it must be a plate to plate mapping
			bColumnMapping = false
		end if
	else
		if IsNull(SourceWellCol) then
			'--no column mapping so it must be a plate to plate mapping
			bColumnMapping = false
		end if
	end if
	
	Set oFSO = CreateObject("Scripting.FileSystemObject")
	Set oLogCheck = oFSO.OpenTextFile(sPath, 1, True) '1 = ForReading
	Set oLog = oFSO.OpenTextFile(sPath, 1, True) '1 = ForReading
	
	'--skip the header lines
	For i = 0 to headerLength - 1
		oLog.SkipLine
		oLogCheck.SkipLine
	next
	
	currSourceBarcode = ""
	currTargetBarcode = ""
	'--check that the number of columns in the file matches the number specified for this template
	While not (oLogCheck.AtEndofStream or bFileTemplateError)
		arrLogData = split(oLogCheck.readline,Delimiter)
		if (ubound(arrLogData)+1)<> NumColumns then
			errCount = errCount + 1
			sError = sError & "Error: The file could not be parsed using the specified import template.  Possible issues include:<BR>"
			sError = sError & "<table><tr><td width=""100""><td align=""left""><span class=""GuiFeedback""><li>Incorrect import template<li>One or more lines have the wrong number of columns for the selected import template.</span></td></tr></table><BR><BR>"
			bFileTemplateError = true
		end if
	wend
	set oLogCheck = nothing

	While not oLog.AtEndofStream
		SourceBarcode = null
		SourceWellNum = null 
		TargetBarcode = null 
		TargetWellNum = null 
		QtyInitial = null 
		QtyUnit = null 
		Concentration = null 
		ConcUnit = null 
		SolventID = null
		PField1 = null
		PField2 = null
		PField3 = null
		PField4 = null
		PField5 = null
		PDate1 = null
		PDate2 = null
		WField1 = null
		WField2 = null
		WField3 = null
		WField4 = null
		WField5 = null
		WDate1 = null
		WDate2 = null
		SolventVol = null
		SolventVolUnit = null
		SolutionVol = null
		CompoundID = null
		RegID = null
		RegNumber = null
		BatchNumber = null
		SolventDilutionVol = null
		bValidBarcodes = true
		
		arrLogData = split(oLog.readline,Delimiter)
		'--don't read blank lines
		if ubound(arrLogData) > 0 then

			if not bFileTemplateError then 
				bFirst = false
			
				bSourceExists = false
				bTargetExists = false
				if not isNull(SourceBarcodeCol) then bSourceExists = true
				if not isNull(TargetBarcodeCol) then bTargetExists = true
				
				if bSourceExists then
					SourceBarcode = arrLogData(SourceBarcodeCol)
				end if
				if bTargetExists then
					TargetBarcode = arrLogData(TargetBarcodeCol)
				end if
		
				if ActionType = "target" then
					'-- validate barcodes
					if bSourceExists then
						if currSourceBarcode <> SourceBarcode then
							if not plateExists(SourceBarcode) then
								'ProblemBarcodes = ProblemBarcodes & SourceBarcode & ":3,"
								'ProblemBarcodes = ProblemBarcodes & TargetBarcode & ":4,"
								if not oProblemBarcodes3.Exists(SourceBarcode) then oProblemBarcodes3.Add SourceBarcode,1
								if not oProblemBarcodes4.Exists(TargetBarcode) then oProblemBarcodes4.Add TargetBarcode,1
								errCount = errCount + 1
								bValidBarcodes = false
							end if		
						'elseif instr(ProblemBarcodes, SourceBarcode)>0 then
						elseif oProblemBarcodes3.Exists(SourceBarcode) then						
							bValidBarcodes = false
						end if
					end if
					if bTargetExists then
						if currTargetBarcode <> TargetBarcode then
							if plateExists(TargetBarcode) then
								'ProblemBarcodes = ProblemBarcodes & TargetBarcode & ":1,"
								if not oProblemBarcodes1.Exists(TargetBarcode) then oProblemBarcodes1.Add TargetBarcode,1
								errCount = errCount + 1
								bValidBarcodes = false
							end if		
						'elseif instr(ProblemBarcodes, TargetBarcode)>0 then
						elseif oProblemBarcodes1.Exists(TargetBarcode) then
							bValidBarcodes = false
						end if
					end if	
				end if
				
				'-- check source barcodes for duplicates
				if ActionType = "source" then
					if bSourceExists then
						if currSourceBarcode <> SourceBarcode then
							if plateExists(SourceBarcode) then
								'ProblemBarcodes = ProblemBarcodes & TargetBarcode & ":1,"
								if not oProblemBarcodes1.Exists(SourceBarcode) then oProblemBarcodes1.Add SourceBarcode,1
								errCount = errCount + 1
								bValidBarcodes = false
							end if		
						'elseif instr(ProblemBarcodes, SourceBarcode)>0 then
						elseif oProblemBarcodes1.Exists(SourceBarcode) then
							bValidBarcodes = false
						end if
					end if	
				
				end if
				'Response.Write "test" & ProblemBarcodes
				'create well row
				if bValidBarcodes then
				'Response.Write WField1Col & ":" & num & ":" & ubound(arrLogData) & "<BR>"
				'num = num + 1
				'Response.end
					if not isNull(QtyInitialCol) then QtyInitial = arrLogData(QtyInitialCol)
					if not isNull(QtyUnitCol) then QtyUnit = arrLogData(QtyUnitCol)
					if not isNull(ConcentrationCol) then Concentration = arrLogData(ConcentrationCol)
					if not isNull(ConcUnitCol) then ConcUnit = arrLogData(ConcUnitCol)
					if not isNull(SolventIDCol) then SolventID = arrLogData(SolventIDCol)
					if not isNull(PField1Col) then PField1 = arrLogData(PField1Col)
					if not isNull(PField2Col) then PField2 = arrLogData(PField2Col)
					if not isNull(PField3Col) then PField3 = arrLogData(PField3Col)
					if not isNull(PField4Col) then PField4 = arrLogData(PField4Col)
					if not isNull(PField5Col) then PField5 = arrLogData(PField5Col)
					if not isNull(PDate1Col) then PDate1 = arrLogData(PDate1Col)
					if not isNull(PDate2Col) then PDate2 = arrLogData(PDate2Col)
					if not isNull(WField1Col) then WField1 = arrLogData(WField1Col)
					if not isNull(WField2Col) then WField2 = arrLogData(WField2Col)
					if not isNull(WField3Col) then WField3 = arrLogData(WField3Col)
					if not isNull(WField4Col) then WField4 = arrLogData(WField4Col)
					if not isNull(WField5Col) then WField5 = arrLogData(WField5Col)
					if not isNull(WDate1Col) then WDate1 = arrLogData(WDate1Col)
					if not isNull(WDate2Col) then WDate2 = arrLogData(WDate2Col)
					if not isNull(SolventVolCol) then SolventVol = arrLogData(SolventVolCol)
					if not isNull(SolventVolUnitCol) then SolventVolUnit = arrLogData(SolventVolUnitCol)
					if not isNull(SolutionVolCol) then SolutionVol = arrLogData(SolutionVolCol)
					if not isNull(CompoundIDCol) then CompoundID = arrLogData(CompoundIDCol)
					if not isNull(RegIDCol) then RegID = arrLogData(RegIDCol)
					if not isNull(RegNumberCol) then RegNumber = arrLogData(RegNumberCol)
					if not isNull(BatchNumberCol) then BatchNumber = arrLogData(BatchNumberCol)
					if not isNull(SolventDilutionVolCol) then SolventDilutionVol = arrLogData(SolventDilutionVolCol)
					
					if bColumnMapping then
						SourceWell = arrLogData(SourceWellCol)
						if bTargetExists then
							TargetWell = arrLogData(TargetWellCol)
						else
							TargetWell = null
						end if

						if bUseWellCoordinates then
							'convert coords to well number
							if ActionType = "target" then
								if TargetBarcode <> currTargetBarcode then
									currTargetBarcode = TargetBarcode
									arrSourceRowsCols = split(GetNumRowsCols(SourceBarcode),",")
									sourceRows = arrSourceRowsCols(0)
									sourceCols = arrSourceRowsCols(1)
									'targetRows = Rows
									'targetCols = Cols
								end if
								if bTargetExists then TargetWellNum = ConvertCoord2Num(TargetWell, targetCols)
							else
								sourceRows = Rows
								sourceCols = Cols
								'Response.Write Cols & "test"
								'Response.End
							end if
							SourceWellNum = ConvertCoord2Num(SourceWell, sourceCols)
						else
							'it is a well number	
							TargetWellNum = TargetWell
							SourceWellNum = SourceWell
						end if
						WriteCSFormatRow oStream, SourceBarcode, SourceWellNum, TargetBarcode, TargetWellNum, QtyInitial, QtyUnit, Concentration, ConcUnit, SolventID, PField1, PField2, PField3, PField4, PField5, PDate1, PDate2, WField1, WField2, WField3, WField4, WField5, WDate1, WDate2, SolventVol, SolventVolUnit, SolutionVol, CompoundID, RegID, RegNumber, BatchNumber, SolventDilutionVol
					else

						if Template = "7" then 
							'source plate is always 96 well
							localRows = 8
							localCols = 12
							TargetQuadrant = arrLogData(TargetWellCol)

							'create z-index 384 well plate							
							wellNum = 1
							For i = 1 to localRows
								For j = 1 to localCols
									Select Case lcase(TargetQuadrant)
										Case "q1"
											TargetWellNum_actual = wellNum + (j-1) + ((i-1)*36)
										Case "q2"
											TargetWellNum_actual = wellNum + j + ((i-1)*36)
											'TargetWellNum_actual = TargetWellNum_actual + (12*(i-1))
										Case "q3"
											TargetWellNum_actual = wellNum + (j-1) + ((i-1)*36) + 24
										Case "q4"
											TargetWellNum_actual = wellNum + j + ((i-1)*36) + 24
											'TargetWellNum_actual = TargetWellNum_actual + (12*((Cols+1)\12))
									End Select
									WriteCSFormatRow oStream, SourceBarcode, wellNum, TargetBarcode, TargetWellNum_actual, QtyInitial, QtyUnit, Concentration, ConcUnit, SolventID, PField1, PField2, PField3, PField4, PField5, PDate1, PDate2, WField1, WField2, WField3, WField4, WField5, WDate1, WDate2, SolventVol, SolventVolUnit, SolutionVol, CompoundID, RegID, RegNumber, BatchNumber, SolventDilutionVol
									wellNum = wellNum + 1
								Next
							Next		
						else
							if TargetBarcode <> currTargetBarcode then
								arrRowsCols = split(GetNumRowsCols(SourceBarcode),",")
								localRows = arrRowsCols(0)
								localCols = arrRowsCols(1)
							end if
							wellNum = 1
							For i = 1 to localRows
								For j = 1 to localCols
									WriteCSFormatRow oStream, SourceBarcode, wellNum, TargetBarcode, wellNum, QtyInitial, QtyUnit, Concentration, ConcUnit, SolventID, PField1, PField2, PField3, PField4, PField5, PDate1, PDate2, WField1, WField2, WField3, WField4, WField5, WDate1, WDate2, SolventVol, SolventVolUnit, SolutionVol, CompoundID, RegID, RegNumber, BatchNumber, SolventDilutionVol
									wellNum = wellNum + 1
								Next
							Next		
						end if
					'end column mapping
					end if			
				'end valid barcodes	
				end if				
			'end template error							
			end if
		'end blank line if	
		end if
	wend
	'update current barcodes
	if currSourceBarcode <> SourceBarcode then 	currSourceBarcode = SourceBarcode
	if currTargetBarcode <> TargetBarcode then	currTargetBarcode = TargetBarcode


End Sub

'parses files into CS Format and updates the passed in stream
Sub ConvertFileToCSFormat_custom1(sPath, ByRef oStream, HeaderLength, Delimiter, SourceBarcodeCol, SourceWellCol, TargetBarcodeCol, TargetWellCol, QtyInitialCol, QtyUnitCol, ConcentrationCol, ConcUnitCol, SolventIDCol, PField1Col, PField2Col, PField3Col, PField4Col, PField5Col, PDate1Col, PDate2Col, WField1Col, WField2Col, WField3Col, WField4Col, WField5Col, WDate1Col, WDate2Col, bUseWellCoordinates, NumColumns, Template)

	Set oFSO = CreateObject("Scripting.FileSystemObject")
	Set oLog = oFSO.OpenTextFile(sPath, 1, True) '1 = ForReading
	
	'skip the header lines
	For i = 0 to headerLength - 1
		oLog.SkipLine
	next
	
	currSourceBarcode = ""
	currTargetBarcode = ""
	While not oLog.AtEndofStream
		SourceBarcode = null
		SourceWellNum = null 
		TargetBarcode = null 
		TargetWellNum = null 
		QtyInitial = null 
		QtyUnit = null 
		Concentration = null 
		ConcUnit = null 
		SolventID = null
		PField1 = null
		PField2 = null
		PField3 = null
		PField4 = null
		PField5 = null
		PDate1 = null
		PDate2 = null
		WField1 = null
		WField2 = null
		WField3 = null
		WField4 = null
		WField5 = null
		WDate1 = null
		WDate2 = null
		arrLogData = split(oLog.readline,Delimiter)
		if bFirst then 
			if (ubound(arrLogData)+1)<> NumColumns then
				errCount = errCount + 1
				sError = sError & "Error: File does not contain the correct number of columns for the selected Import Template.<BR>"			
				bFileTemplateError = true
			end if
			bFirst = false
		end if
		if not bFileTemplateError then 

			SourceBarcode = ""
			TargetBarcode = ""

			'create well row
			if not isNull(QtyInitialCol) then QtyInitial = arrLogData(QtyInitialCol)
			if not isNull(QtyUnitCol) then QtyUnit = arrLogData(QtyUnitCol)
			if not isNull(ConcentrationCol) then Concentration = arrLogData(ConcentrationCol)
			if not isNull(ConcUnitCol) then ConcUnit = arrLogData(ConcUnitCol)
			if not isNull(SolventIDCol) then SolventID = arrLogData(SolventIDCol)
			if not isNull(PField1Col) then PField1 = arrLogData(PField1Col)
			if not isNull(PField2Col) then PField2 = arrLogData(PField2Col)
			if not isNull(PField3Col) then PField3 = arrLogData(PField3Col)
			if not isNull(PField4Col) then PField4 = arrLogData(PField4Col)
			if not isNull(PField5Col) then PField5 = arrLogData(PField5Col)
			if not isNull(PDate1Col) then PDate1 = arrLogData(PDate1Col)
			if not isNull(PDate2Col) then PDate2 = arrLogData(PDate2Col)
			if not isNull(WField1Col) then WField1 = arrLogData(WField1Col)
			if not isNull(WField2Col) then WField2 = arrLogData(WField2Col)
			if not isNull(WField3Col) then WField3 = arrLogData(WField3Col)
			if not isNull(WField4Col) then WField4 = arrLogData(WField4Col)
			if not isNull(WField5Col) then WField5 = arrLogData(WField5Col)
			if not isNull(WDate1Col) then WDate1 = arrLogData(WDate1Col)
			if not isNull(WDate2Col) then WDate2 = arrLogData(WDate2Col)
		
 			SourceWell = arrLogData(SourceWellCol)
			'convert coords to well number
			SQL = "SELECT row_count, col_count FROM inv_vw_plate_format WHERE plate_format_id = ?"
			Set Cmd = GetCommand(Conn, SQL, adCmdText)
			Cmd.Parameters.Append Cmd.CreateParameter("PlateFormat", 5, 1, 0, PlateFormat)
			Set RS = Cmd.Execute
			sourceRows = clng(RS("row_count"))
			sourceCols = clng(RS("col_count"))
			SourceWellNum = ConvertCoord2Num(SourceWell, sourceCols)

			WriteCSFormatRow oStream, SourceBarcode, SourceWellNum, TargetBarcode, TargetWellNum, QtyInitial, QtyUnit, Concentration, ConcUnit, SolventID, PField1, PField2, PField3, PField4, PField5, PDate1, PDate2, WField1, WField2, WField3, WField4, WField5, WDate1, WDate2
		end if
	wend

End Sub


Sub WriteCSFormatRow(ByRef oStream, SourceBarcode, SourceWellNum, TargetBarcode, TargetWellNum, QtyInitial, QtyUnit, Concentration, ConcUnit, SolventID, PField1, PField2, PField3, PField4, PField5, PDate1, PDate2, WField1, WField2, WField3, WField4, WField5, WDate1, WDate2, SolventVol, SolventVolUnit, SolutionVol, CompoundID, RegID, RegNumber, BatchNumber, SolventDilutionVol)
		'Source Plate Barcode	
		oStream.WriteText(SourceBarcode & ",")
		'Source Well Number	
		oStream.WriteText(SourceWellNum & ",")
		'Target Plate Barcode	
		oStream.WriteText(TargetBarcode & ",")
		'Target Well Number	
		oStream.WriteText(TargetWellNum & ",")
		'QTY_INITIAL	
		oStream.WriteText(QtyInitial & ",")
		'QTY_UNIT_FK	
		oStream.WriteText(QtyUnit & ",")
		'CONCENTRATION	
		oStream.WriteText(Concentration & ",")
		'CONC_UNIT_FK	
		oStream.WriteText(ConcUnit & ",")
		'SOLVENT_ID_FK
		'check for solvent name if found then do a lookup and add if its not there
		if (not isNull(SolventID)) and (SolventID <> "") and (not isNumeric(SolventID)) then 
			solventName = SolventID
			If not oSolventDict.Exists(solventName) then
				QueryString = "TableName=inv_solvents"
				QueryString = QueryString & "&TableValue=" & SolventName
				QueryString = QueryString & "&InsertIfNotFound=true"
				httpResponse = CShttpRequest2("POST", InvServerName, "/cheminv/api/LookUpValue.asp", "ChemInv", QueryString)
				if instr(httpResponse,"Error")>0 then
					solventID = null
				else
					solventID = httpResponse
					oSolventDict.Add solventName,solventID
				end if
			else
				solventID = oSolventDict.Item(solventName)	
			end if
		end if
		oStream.WriteText(SolventID & ",")
		'PFIELD_1
		oStream.WriteText(PField1 & ",")
		'PFIELD_2
		oStream.WriteText(PField2 & ",")
		'PFIELD_3
		oStream.WriteText(PField3 & ",")
		'PFIELD_4
		oStream.WriteText(PField4 & ",")
		'PFIELD_5
		oStream.WriteText(PField5 & ",")
		'PDATE_1
		oStream.WriteText(PDate1 & ",")
		'PDATE_2
		oStream.WriteText(PDate2 & ",")
		'WFIELD_1
		oStream.WriteText(WField1 & ",")
		'WFIELD_2
		oStream.WriteText(WField2 & ",")
		'WFIELD_3
		oStream.WriteText(WField3 & ",")
		'WFIELD_4
		oStream.WriteText(WField4 & ",")
		'WFIELD_5
		oStream.WriteText(WField5 & ",")
		'WDATE_1
		oStream.WriteText(WDate1 & ",")
		'WDATE_2
		'if IsNull(WDate2) then WDate2 = ""
		oStream.WriteText(WDate2 & ",")
		oStream.WriteText(SolventVol & ",")
		oStream.WriteText(SolventVolUnit & ",")
		oStream.WriteText(SolutionVol & ",")
		oStream.WriteText(CompoundID & ",")
		'-- use RegID first, if none is mapped then lookup regnumber and write regID 
		if (not isNull(RegID)) or (RegID <> "") then 
			oStream.WriteText(RegID & ",")
		else
			if (not isNull(RegNumber)) and (RegNumber <> "") then
				SQL = "SELECT COUNT(*) as theCount, reg_id FROM reg_numbers WHERE reg_number = ? GROUP BY reg_id"
				Set Cmd = GetCommand(Conn, SQL, adCmdText)
				Cmd.Parameters.Append Cmd.CreateParameter("RegNumber", 200, 1, 255, RegNumber)
				Set RS = Cmd.Execute
				if not RS.EOF then
				    if cint(RS("theCount")) = 1 then
    					oStream.WriteText(RS("reg_id") & ",")				
				    else
					    oStream.WriteText(",")
					end if
			    else
		            oStream.WriteText(",")
				end if
			else 
				oStream.WriteText(",")				
			end if
		end if
		if isNull(BatchNumber) then BatchNumber = ""
		oStream.WriteText(BatchNumber & ",")
		if isNull(SolventDilutionVol) then SolventDilutionVol = ""
		oStream.WriteText(CStr(SolventDilutionVol))
		oStream.WriteText vbcrlf
End Sub

Function ConvertCoord2Num(tempCoord, Cols)
	coord = trim(tempCoord)
	strColumn = right(coord, Len(coord) - 1)
	Row = Asc(UCase(Left(coord, 1))) - 64
	Col = ""
	for i = 1 to Len(strColumn)
	    ' Only pull numeric digits
	    if Asc(Mid(strColumn,i,1)) > 47 and Asc(Mid(strColumn,i,1)) < 58 then
	        Col = Col & Mid(strColumn,i,1)
	    end if	    
	next    
    'Response.Write coord & ":" & (((Row-1)*cInt(Cols)) + Col) & ":" & Row & ":" & Cols & "<BR>"
    ConvertCoord2Num = ((Row-1)*cInt(Cols)) + Col
End Function

'Returns a comma,delimited list of the number of rows and columns for a specific plate
Function GetNumRowsCols(PlateBarcode)
	Dim Rows
	Dim Cols
	SQL = "SELECT row_count, col_count FROM inv_vw_plate_format, inv_plates WHERE plate_format_id = plate_format_id_fk AND plate_barcode = ?"
	Set Cmd = GetCommand(Conn, SQL, adCmdText)
	Cmd.Parameters.Append Cmd.CreateParameter("PlateBarcode", 200 , 1, 200, PlateBarcode)
	Set RS = Cmd.Execute
	Rows = RS("row_count")
	Cols = RS("col_count")
	GetNumRowsCols = Rows & "," & Cols

End Function

%>