<!--#INCLUDE VIRTUAL = "/cheminv/api/apiUtils.asp"-->
<!--#INCLUDE VIRTUAL = "/cheminv/gui/guiUtils.asp"-->
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/upload.asp"-->
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/ado.inc"-->

<html>
<head>
<title><%=Application("appTitle")%> -- Create Plates From Pipette Log</title>
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
BarcodeDescID = oUpload.Item("BarcodeDescID").value.string
if InputQtyUnit = "null" then InputQtyUnit = ""
if InputConcUnit = "null" then InputConcUnit = ""
if InputSolventID = "null" then InputSolventID = ""

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

'preprocess the file if necessary (includes barcode validation)
PreProcess sPath, Template, oStream

'Response.Write ProblemBarcodes & ":pb"
oStream.Position = 0

'Response.Write oStream.Size & ":size<BR>"
'Response.Write oStream.ReadText(-1)
'Response.Write oStream.ReadText(-2)'
'Response.End

'check if the file was parsed at all
if oStream.Size = 0 then 
	bFileParseError = true
	errCount = errCount + 1
	sError = sError & "Error: The file could not be parsed using the specified import template.  Check the import template and try again.<BR>"
end if
if not bFileTemplateError  and not bFileParseError then

' get well format info for new plate
SQL = "SELECT * FROM inv_vw_well_format WHERE plate_format_id_fk = ?  order by row_index, col_index"
Set Cmd = GetCommand(Conn, SQL, adCmdText)
Cmd.Parameters.Append Cmd.CreateParameter("PlateFormat", 5, 1, 0, PlateFormat)
Set rsWell = Server.CreateObject("ADODB.recordset")
rsWell.CursorLocation = aduseClient
rsWell.LockType = adLockOptimistic
'Response.Write SQL & PlateFormat & "<BR>"
'Response.End
rsWell.Open Cmd
rsWell.ActiveConnection = Nothing

' get number of rows and columsn for the specified format
SQL = "SELECT row_count, col_count FROM inv_vw_plate_format WHERE plate_format_id = ?"
Set Cmd = GetCommand(Conn, SQL, adCmdText)
Cmd.Parameters.Append Cmd.CreateParameter("PlateFormat", 5, 1, 0, PlateFormat)
Set RS = Cmd.Execute
Rows = clng(RS("row_count"))
Cols = clng(RS("col_count"))

plateNum = 0	
Set oHTTP = Server.CreateObject("MSXML2.serverXMLHTTP")
oHTTP.setTimeouts 600000, 600000, 600000, 600000
Set oPlates = server.CreateObject("MSXML2.DOMDocument.6.0")
bNewPlate = true
bFirst = true
bNewSourcePlate = true

if ActionType="target" then

	While not oStream.EOS
		arrLogData = split(oStream.ReadText(-2),",")
		'don't read blank lines
		if ubound(arrLogData) > 0 then		
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

			'use the file fields over the input fields if it exists
			QtyInitial = iif((IsEmpty(FileQtyInitial) or FileQtyInitial=""),InputQtyInitial,FileQtyInitial)
			QtyUnit = iif((IsEmpty(FileQtyUnit) or FileQtyUnit=""),InputQtyUnit,FileQtyUnit)
			Concentration = iif((IsEmpty(FileConcentration) or FileConcentration=""),InputConcentration,FileConcentration)
			ConcUnit = iif((IsEmpty(FileConcUnit) or FileConcUnit=""),InputConcUnit,FileConcUnit)
			SolventID = iif((IsEmpty(FileSolventID) or FileSolventID=""),InputSolventID,FileSolventID)
			
			'set the target plate first time around			
			if bFirst then currTargetPlate = TargetPlateBarcode
			
			'if this is a new plate then create the previous plate
			if not bFirst and currTargetPlate <> TargetPlateBarcode then
				'only create problem free plates
				if instr(ProblemBarcodes,currTargetPlate) = 0 then
					'create plate xml
					rsWell.MoveFirst
					newPlateID = createPlate(oPlates)
					if newPlateID <> "-1" then
						PlateIDs = PlateIDs & newPlateID & "|"
					else
						if not oProblemBarcodes2.Exists(currTargetPlate) then oProblemBarcodes2.Add currTargetPlate,1
						'CreateErrorBarcodes = CreatErrorBarcodes &  currTargetPlate & ","
						errCount = errCount + 1
					end if
					Set oPlates = server.CreateObject("MSXML2.DOMDocument.6.0")
				end if
				currTargetPlate = TargetPlateBarcode
				bNewPlate = true
			end if
		
			'create plate xml document
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
					End With
				End With
			 
				' set plate information
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
			        if BarcodeDescID = "-1" then
						.setAttribute "PLATE_BARCODE", TargetPlateBarcode
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
			    ' add row and column nodes based on the plate format
				For j = 1 To Rows
					Set oRowNode = oPlates.createElement("ROW")
			        oRowNode.setAttribute "ID", j
				    oPlateNode.appendChild oRowNode
					For k = 1 To Cols
						Set oColNode = oPlates.createElement("COL")
						oColNode.setAttribute "ID", k
				        oRowNode.appendChild oColNode
					    ' add well node, fill based on plate format
			            rsWell.Filter = adFilterNone
				        rsWell.MoveFirst
					    rsWell.Filter = "ROW_INDEX = " & j & " and COL_INDEX = " & k
						Set oWellNode = oPlates.createElement("WELL")
						rsConcentration = rsWell("CONCENTRATION").Value
						if isNull(rsConcentration) then rsConcentration = ""
						rsConcUnit = rsWell("CONC_UNIT_FK").Value
						if isNull(rsConcUnit) then rsConcUnit = ""
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
				plateNum = plateNum + 1
			end if 
		
			'add target well compound and parent well info from the parent source well
			if instr(ProblemBarcodes,TargetPlateBarcode) = 0 then
				'add compound info defined by the log
				'check for new source plate
				if currSourcePlate <> SourcePlateBarcode then
					SQL = "SELECT compound_id_fk, reg_id_fk, batch_number_fk, well_id, sort_order FROM inv_vw_well_flat, inv_plates WHERE plate_barcode = ? and plate_id = plate_id_fk"
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
		
					set oColNode = oPlates.selectSinglenode("/PLATES/PLATE/descendant::COL[" & TargetWellNumber & "]")
					set oUpdateNode = oPlates.selectSinglenode("/PLATES/PLATE/descendant::COL[" & TargetWellNumber & "]/WELL[1]")
					ParentWellIDFK = oUpdateNode.GetAttribute("PARENT_WELL_ID_FK")
					GridPositionIDFK = oUpdateNode.GetAttribute("GRID_POSITION_ID_FK")
					'if this node has been updated then add another
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
			set oUpdateNode = oPlates.selectSinglenode("/PLATES/PLATE/descendant::COL[" & TargetWellNumber & "]/WELL[1]")
			With oUpdateNode
				.setAttribute "QTY_INITIAL", QtyInitial
				.setAttribute "QTY_REMAINING", QtyInitial
			    .setAttribute "QTY_UNIT_FK", QtyUnit
				.setAttribute "CONCENTRATION", Concentration
				.setAttribute "CONC_UNIT_FK", ConcUnit
				.setAttribute "SOLVENT_ID_FK", SolventID
				.setAttribute "FIELD_1", WField1
				.setAttribute "FIELD_2", WField2
				.setAttribute "FIELD_3", WField3
				.setAttribute "FIELD_4", WField4
				.setAttribute "FIELD_5", WField5
				.setAttribute "DATE_1", WDate1
				.setAttribute "DATE_2", WDate2
			end with

		end if
		'update well mapped data
		'Response.Write CreationDate & ":" & SourcePlateBarcode & ":" & SourceWellNumber & ":" & TargetPlateBarcode & ":" & TargetWellNumber & "<BR>"  
	wend
		if instr(ProblemBarcodes,currTargetPlate) = 0 then
			if plateExists(TargetPlateBarcode) then
				'ProblemBarcodes = ProblemBarcodes & TargetPlateBarcode & ":1,"
				if not oProblemBarcodes1.Exists(TargetPlateBarcode) then oProblemBarcodes1.Add TargetPlateBarcode,1
				errCount = errCount + 1
			else
				'create plate xml
				rsWell.MoveFirst
				If bDebug then oPlates.save(Server.MapPath("/" & Application("AppKey") & "/config/xml_Templates/debug02_CreatePlateFromPipetteLogXML.xml")) 
				
				newPlateID = createPlate(oPlates)
				if newPlateID <> "-1" then
					PlateIDs = PlateIDs & newPlateID & "|"
				else
					'ProblemBarcodes = ProblemBarcodes & TargetPlateBarcode & ":2,"
					if not oProblemBarcodes2.Exists(TargetPlateBarcode) then oProblemBarcodes2.Add TargetPlateBarcode,1
					errCount = errCount + 1
				end if
			end if
		end if

elseif ActionType = "source" then
	While not oStream.EOS

		arrLogData = split(oStream.ReadText(-2),",")
		'don't read blank lines
		if ubound(arrLogData) > 0 then
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

			'use the file fields over the input fields if it exists
			QtyInitial = iif((IsEmpty(FileQtyInitial) or FileQtyInitial=""),InputQtyInitial,FileQtyInitial)
			QtyUnit = iif((IsEmpty(FileQtyUnit) or FileQtyUnit=""),InputQtyUnit,FileQtyUnit)
			Concentration = iif((IsEmpty(FileConcentration) or FileConcentration=""),InputConcentration,FileConcentration)
			ConcUnit = iif((IsEmpty(FileConcUnit) or FileConcUnit=""),InputConcUnit,FileConcUnit)
			SolventID = iif((IsEmpty(FileSolventID) or FileSolventID=""),SolventID,FileSolventID)

			if bFirst then currSourcePlate = SourcePlateBarcode

			if not bFirst and currSourcePlate <> SourcePlateBarcode then
			'Response.Write currSourcePlate
			'Response.End
				'only create problem free plates
				if instr(ProblemBarcodes,currSourcePlate) = 0 then
					'check for existing barcode
					if plateExists(currSourcePlate) then
						'ProblemBarcodes = ProblemBarcodes & currSourcePlate & ":1,"
						if not oProblemBarcodes1.Exists(currSourcePlate) then oProblemBarcodes1.Add currSourcePlate,1
						errCount = errCount + 1
					else
						'create plate xml
						rsWell.MoveFirst
						newPlateID = createPlate(oPlates)
						if newPlateID <> "-1" then
							PlateIDs = PlateIDs & newPlateID & "|"
						else
							CreateErrorBarcodes = CreatErrorBarcodes &  currSourcePlate & ","
							errCount = errCount + 1
						end if
						Set oPlates = server.CreateObject("MSXML2.DOMDocument.6.0")
					end if
				end if
				currSourcePlate = SourcePlateBarcode
				bNewPlate = true
			end if
		
			'create plate xml document
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
					End With
				End With
			 
				' set plate information
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
					.setAttribute "SOLVENT_ID_FK", Solvent
			        if BarcodeDescID = "-1" then
						.setAttribute "PLATE_BARCODE", SourcePlateBarcode
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
			    ' add row and column nodes based on the plate format
				For j = 1 To Rows
					Set oRowNode = oPlates.createElement("ROW")
			        oRowNode.setAttribute "ID", j
				    oPlateNode.appendChild oRowNode
					For k = 1 To Cols
						Set oColNode = oPlates.createElement("COL")
						oColNode.setAttribute "ID", k
				        oRowNode.appendChild oColNode
					    ' add well node, fill based on plate format
			            rsWell.Filter = adFilterNone
				        rsWell.MoveFirst
					    rsWell.Filter = "ROW_INDEX = " & j & " and COL_INDEX = " & k
						Set oWellNode = oPlates.createElement("WELL")
						With oWellNode
							.setAttribute "WELL_FORMAT_ID_FK", rsWell("WELL_FORMAT_ID_FK").Value
				            .setAttribute "PLATE_FORMAT_ID_FK", rsWell("PLATE_FORMAT_ID_FK").Value
							.setAttribute "GRID_POSITION_ID_FK", rsWell("GRID_POSITION_ID").Value
							.setAttribute "CONCENTRATION", iif((IsNull(rsWell("CONCENTRATION").Value)),"",rsWell("CONCENTRATION").Value)
							.setAttribute "CONC_UNIT_FK", iif((IsNull(rsWell("CONC_UNIT_FK").Value)),"",rsWell("CONC_UNIT_FK").Value)
						End With
					    oColNode.appendChild oWellNode
					Next
				Next
				plateNum = plateNum + 1
			end if 
		'update well mapped data
		set oUpdateNode = oPlates.selectSinglenode("/PLATES/PLATE/descendant::COL[" & SourceWellNumber & "]/WELL[1]")
		With oUpdateNode
			.setAttribute "QTY_INITIAL", QtyInitial
			.setAttribute "QTY_REMAINING", QtyInitial
		    .setAttribute "QTY_UNIT_FK", QtyUnit
			.setAttribute "CONCENTRATION", Concentration
			.setAttribute "CONC_UNIT_FK", ConcUnit
			.setAttribute "SOLVENT_ID_FK", SolventID
			.setAttribute "FIELD_1", WField1
			.setAttribute "FIELD_2", WField2
			.setAttribute "FIELD_3", WField3
			.setAttribute "FIELD_4", WField4
			.setAttribute "FIELD_5", WField5
			.setAttribute "DATE_1", WDate1
			.setAttribute "DATE_2", WDate2
		end with
		
		
		end if
	wend
	'oPlates.save("c:\test.xml")
	'Response.End
		if instr(ProblemBarcodes,currSourcePlate) = 0 then
			'check for existing barcode
			if plateExists(currSourcePlate) then
				if not oProblemBarcodes1.Exists(currSourcePlate) then oProblemBarcodes1.Add currSourcePlate,1
				errCount = errCount + 1
			else
				'create plate
				rsWell.MoveFirst
				newPlateID = createPlate(oPlates)
				if newPlateID <> "-1" then
					PlateIDs = PlateIDs & newPlateID & "|"
				else
					if not oProblemBarcodes2.Exists(SourcePlateBarcode) then oProblemBarcodes2.Add SourcePlateBarcode,1
					errCount = errCount + 1
				end if
			end if
		end if

end if

'Response.Write PlateIDs
'Response.End

	if len(PlateIDs) = 0 then
		PlateIDs = "error"
	end if

arrNewPlateIDs = split(PlateIDs,"|")
else

end if
%>

<TABLE ALIGN=CENTER BORDER=0 CELLPADDING=0 CELLSPACING=0 BGCOLOR=#ffffff width=90%>
	<TR>
		<TD HEIGHT=50 VALIGN=MIDDLE align="left">
<%
			' hide the spinning benzene
			Response.write "<Script language=javascript>document.all.processingImage.style.display = 'none';</script>"		
			
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

			Select Case theAction
				Case "Exit"
					Response.Write "<center><SPAN class=""GuiFeedback"">Plate creation complete.</SPAN></center>"
					if errCount > 0 then
						Response.Write sError
					else
						Response.Write "<P><center><a HREF=""3"" onclick=""history.go(-2); return false;""><img SRC=""../../graphics/ok_dialog_btn.gif"" border=""0""></a></center>"
					end if
				Case "WriteAPIError"
					Response.Write "<P><CODE>ChemInv API Error: " & Application(out) & "</CODE></P>"
					Response.Write "<P><center><a HREF=""3"" onclick=""history.back(); return false;""><img SRC=""../../graphics/ok_dialog_btn.gif"" border=""0""></a></center>"
				Case "WriteOtherError"
					Response.Write "<P><CODE>Oracle Error: " & out & "</code></p>" 
					Response.Write "<P><center><a HREF=""3"" onclick=""history.back(); return false;""><img SRC=""../../graphics/ok_dialog_btn.gif"" border=""0""></a></center>"
				Case "MiscError"
					if problemBarcodes1 <> "" then sError = sError & "Error: Plate(s) - " & problemBarcodes1 & " - " & iif(oProblemBarcodes1.Count>1,"were","was") & " not created due to a barcode conflict.<BR>" 
					if problemBarcodes2 <> "" then sError = sError & "<span title=""" & err.Description & """>Error: Plate(s) - " & problemBarcodes2 & " - " & iif(oProblemBarcodes2.Count>1,"were","was") & " not created due to an Oracle error.</span><BR>" 
					if problemBarcodes3 <> "" then sError = sError & "Error: Source plate(s) - " & problemBarcodes3 & " - " & iif(oProblemBarcodes3.Count>1,"do","does") & " not exist in Inventory Manger.  Plate(s) - " & problemBarcodes4 & " - " & iif(oProblemBarcodes4.Count>1,"have","has") & " not been created.<BR>" 
					Response.Write "<SPAN class=""GuiFeedback""><center>" & sError & "</center></SPAN>"
					Response.Write "<P><center><a HREF=""3"" onclick=""history.back(); return false;""><img SRC=""../../graphics/ok_dialog_btn.gif"" border=""0""></a></center>"
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
			URL = Application("SERVER_TYPE") & Application("InvServerName") & "/cheminv/api/CreatePlateXML.asp"
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
		SQL = "SELECT count(*) AS theCount FROM inv_plates WHERE plate_barcode = ?"
		Set Cmd = GetCommand(Conn, SQL, adCmdText)
		Cmd.Parameters.Append Cmd.CreateParameter("PlateBarcode", 200, 1, 200, barcode)
		Set RS = Cmd.Execute
		if clng(RS("theCount")) > 0 then
			ret = true
		else
			ret = false
		end if
		plateExists = ret
End Function


Sub PreProcess(sPath, Template, ByRef oStream)

	Select Case Template
		Case "1"
			'CS Default
			'# Header Rows:	0
			'Delimiter:	comma												
			'Columns:	Source Plate Barcode, Source Well Number, Target Plate Barcode, Target Well Number, QTY_INITIAL, QTY_UNIT_FK, CONCENTRATION, CONC_UNIT_FK, SOLVENT_ID_FK, PFIELD_1, PFIELD_2, PFIELD_3, PFIELD_4, PFIELD_5, PDATE_1, PDATE_2, WFIELD_1, WFIELD_2, WFIELD_3, WFIELD_4, WFIELD_5, WDATE_1, WDATE_2
			headerLength = 0
			Delimiter = ","
			SourceBarcodeCol = 0
			SourceWellCol = 1
			TargetBarcodeCol = 2
			TargetWellCol = 3
			QtyInitialCol = 4 
			QtyUnitCol = 5 
			ConcentrationCol = 6 
			ConcUnitCol = 7 
			SolventIDCol = 8 
			PField1Col = 9 
			PField2Col = 10
			PField3Col = 11
			PField4Col = 12
			PField5Col = 13
			PDate1Col = 14
			PDate2Col = 15
			WField1Col = 16
			WField2Col = 17
			WField3Col = 18
			WField4Col = 19 
			WField5Col = 20
			WDate1Col = 21
			WDate2Col = 22
			bUseWellCoordinates = false
			NumColumns = 23
		Case "2"
			'2 Column
			'# Header Rows:	1
			'Delimiter:	tab													
			'Columns:	Target Plate Barcode	Source Plate Barcode
			headerLength = 1
			Delimiter = chr(9)
			SourceBarcodeCol = 1
			SourceWellCol = null
			TargetBarcodeCol = 0
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
			bUseWellCoordinates = false
			NumColumns = 14
		Case "5"
			'Plate 1
			'#Header Rows: 1
			'Delimiter: tab
			'Columns: Well Coordinate	NA	NA	Initial Qty	Solvent	Concentration	Lot	com
			headerLength = 1
			Delimiter = chr(9)
			SourceBarcodeCol = null
			SourceWellCol = 0
			TargetBarcodeCol = null
			TargetWellCol = null
			QtyInitialCol = 3
			QtyUnitCol = null 
			ConcentrationCol = 5
			ConcUnitCol = null 
			SolventIDCol = 4 
			PField1Col = null 
			PField2Col = null
			PField3Col = null
			PField4Col = null
			PField5Col = null
			PDate1Col = null
			PDate2Col = null
			WField1Col = 1
			WField2Col = 6
			WField3Col = null
			WField4Col = null 
			WField5Col = null
			WDate1Col = null
			WDate2Col = null
			bUseWellCoordinates = true
			NumColumns = 8
		Case "6"
			'Plate 1
			'#Header Rows: 1
			'Delimiter: tab
			'Columns: Well Coordinate	NA	NA	Initial Qty	Concentration	Solvent	Lot	com
			headerLength = 1
			Delimiter = chr(9)
			SourceBarcodeCol = null
			SourceWellCol = 0
			TargetBarcodeCol = null
			TargetWellCol = null
			QtyInitialCol = 3
			QtyUnitCol = null 
			ConcentrationCol = 4
			ConcUnitCol = null 
			SolventIDCol = 5 
			PField1Col = null 
			PField2Col = null
			PField3Col = null
			PField4Col = null
			PField5Col = null
			PDate1Col = null
			PDate2Col = null
			WField1Col = 1
			WField2Col = 6
			WField3Col = null
			WField4Col = null 
			WField5Col = null
			WDate1Col = null
			WDate2Col = null
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
			SourceWellCol = null
			TargetBarcodeCol = 2
			'mapping target quadrant to targetwell
			TargetWellCol = 3
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
			bUseWellCoordinates = false
			NumColumns = 7
	End Select

	if Template = "5" or Template = "6" then
		'custom processor for Nissan
		ConvertFileToCSFormat_custom1 sPath, oStream, HeaderLength, Delimiter, SourceBarcodeCol, SourceWellCol, TargetBarcodeCol, TargetWellCol, QtyInitialCol, QtyUnitCol, ConcentrationCol, ConcUnitCol, SolventIDCol, PField1Col, PField2Col, PField3Col, PField4Col, PField5Col, PDate1Col, PDate2Col, WField1Col, WField2Col, WField3Col, WField4Col, WField5Col, WDate1Col, WDate2Col, bUseWellCoordinates, NumColumns, Template
	else
		ConvertFileToCSFormat sPath, oStream, HeaderLength, Delimiter, SourceBarcodeCol, SourceWellCol, TargetBarcodeCol, TargetWellCol, QtyInitialCol, QtyUnitCol, ConcentrationCol, ConcUnitCol, SolventIDCol, PField1Col, PField2Col, PField3Col, PField4Col, PField5Col, PDate1Col, PDate2Col, WField1Col, WField2Col, WField3Col, WField4Col, WField5Col, WDate1Col, WDate2Col, bUseWellCoordinates, NumColumns, Template
	end if
	'oStream.Position = 0
	'Response.Write oStream.ReadText
	'Response.End
End Sub

'parses files into CS Format and updates the passed in stream
Sub ConvertFileToCSFormat(sPath, ByRef oStream, HeaderLength, Delimiter, SourceBarcodeCol, SourceWellCol, TargetBarcodeCol, TargetWellCol, QtyInitialCol, QtyUnitCol, ConcentrationCol, ConcUnitCol, SolventIDCol, PField1Col, PField2Col, PField3Col, PField4Col, PField5Col, PDate1Col, PDate2Col, WField1Col, WField2Col, WField3Col, WField4Col, WField5Col, WDate1Col, WDate2Col, bUseWellCoordinates, NumColumns, Template)

	'assume there is a column mapping
	bColumnMapping = true
	if (IsNull(SourceWellCol) or IsNull(TargetWellCol)) then
		'no column mapping so it must be a plate to plate mapping
		bColumnMapping = false
	end if
	
	Set oFSO = CreateObject("Scripting.FileSystemObject")
	Set oLog = oFSO.OpenTextFile(sPath, 1, True) '1 = ForReading
	
	'skip the header lines
	For i = 0 to headerLength - 1
		oLog.SkipLine
	next
	
	currSourceBarcode = ""
	currTargetBarcode = ""
	bFirst = true
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
		bValidBarcodes = true
		
		arrLogData = split(oLog.readline,Delimiter)
		'don't read blank lines
		if ubound(arrLogData) > 0 then
			'check that the number of columns in the file matches the number specified for this template
			if bFirst then 
				if (ubound(arrLogData)+1)<> NumColumns then
					errCount = errCount + 1
					sError = sError & "Error: File does not contain the correct number of columns for the selected Import Template.<BR>"			
					bFileTemplateError = true
				end if
				bFirst = false
			end if
			'Response.Write NumColumns & ":" & (ubound(arrLogData)+1) & "<BR>"
			if not bFileTemplateError then 
				SourceBarcode = arrLogData(SourceBarcodeCol)
				TargetBarcode = arrLogData(TargetBarcodeCol)
		
				'validate barcodes
				if currSourceBarcode <> SourceBarcode and ActionType <> "source" then
					if not plateExists(SourceBarcode) then
						'ProblemBarcodes = ProblemBarcodes & SourceBarcode & ":3,"
						'ProblemBarcodes = ProblemBarcodes & TargetBarcode & ":4,"
						if not oProblemBarcodes3.Exists(SourceBarcode) then oProblemBarcodes3.Add SourceBarcode,1
						if not oProblemBarcodes4.Exists(TargetBarcode) then oProblemBarcodes4.Add TargetBarcode,1
						errCount = errCount + 1
						bValidBarcodes = false
					end if		
				elseif instr(ProblemBarcodes, SourceBarcode)>0 then
					bValidBarcodes = false
				end if
				if currTargetBarcode <> TargetBarcode then
					if plateExists(TargetBarcode) then
						'ProblemBarcodes = ProblemBarcodes & TargetBarcode & ":1,"
						if not oProblemBarcodes1.Exists(TargetBarcode) then oProblemBarcodes1.Add TargetBarcode,1
						errCount = errCount + 1
						bValidBarcodes = false
					end if		
				elseif instr(ProblemBarcodes, TargetBarcode)>0 then
					bValidBarcodes = false
				end if
		
				'Response.Write "test" & ProblemBarcodes
				'create well row
				if bValidBarcodes then
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


					if bColumnMapping then
						SourceWell = arrLogData(SourceWellCol)
						TargetWell = arrLogData(TargetWellCol)

						if bUseWellCoordinates then
							'convert coords to well number
							if TargetBarcode <> currTargetBarcode then
								currTargetBarcode = TargetBarcode
								arrSourceRowsCols = split(GetNumRowsCols(SourceBarcode),",")
								sourceRows = arrSourceRowsCols(0)
								sourceCols = arrSourceRowsCols(1)
								SQL = "SELECT row_count, col_count FROM inv_vw_plate_format WHERE plate_format_id = ?"
								Set Cmd = GetCommand(Conn, SQL, adCmdText)
								Cmd.Parameters.Append Cmd.CreateParameter("PlateFormat", 5, 1, 0, PlateFormat)
								Set RS = Cmd.Execute
								targetRows = clng(RS("row_count"))
								targetCols = clng(RS("col_count"))
							end if
							TargetWellNum = ConvertCoord2Num(TargetWell, targetCols)
							SourceWellNum = ConvertCoord2Num(SourceWell, sourceCols)
						else
								'it is a well number	
								TargetWellNum = TargetWell
								SourceWellNum = SourceWell
						end if
						WriteCSFormatRow oStream, SourceBarcode, SourceWellNum, TargetBarcode, TargetWellNum, QtyInitial, QtyUnit, Concentration, ConcUnit, SolventID, PField1, PField2, PField3, PField4, PField5, PDate1, PDate2, WField1, WField2, WField3, WField4, WField5, WDate1, WDate2
					else

						if Template = "7" then 
							'source plate is always 96 well
							Rows = 8
							Cols = 12
							TargetQuadrant = arrLogData(TargetWellCol)

							'create z-index 384 well plate							
							wellNum = 1
							For i = 1 to Rows
								For j = 1 to Cols
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
									WriteCSFormatRow oStream, SourceBarcode, wellNum, TargetBarcode, TargetWellNum_actual, QtyInitial, QtyUnit, Concentration, ConcUnit, SolventID, PField1, PField2, PField3, PField4, PField5, PDate1, PDate2, WField1, WField2, WField3, WField4, WField5, WDate1, WDate2	
									wellNum = wellNum + 1
								Next
							Next		
						else
							if TargetBarcode <> currTargetBarcode then
								arrRowsCols = split(GetNumRowsCols(SourceBarcode),",")
								Rows = arrRowsCols(0)
								Cols = arrRowsCols(1)
							end if
							wellNum = 1
							For i = 1 to Rows
								For j = 1 to Cols
									WriteCSFormatRow oStream, SourceBarcode, wellNum, TargetBarcode, wellNum, QtyInitial, QtyUnit, Concentration, ConcUnit, SolventID, PField1, PField2, PField3, PField4, PField5, PDate1, PDate2, WField1, WField2, WField3, WField4, WField5, WDate1, WDate2	
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


Sub WriteCSFormatRow(ByRef oStream, SourceBarcode, SourceWellNum, TargetBarcode, TargetWellNum, QtyInitial, QtyUnit, Concentration, ConcUnit, SolventID, PField1, PField2, PField3, PField4, PField5, PDate1, PDate2, WField1, WField2, WField3, WField4, WField5, WDate1, WDate2)
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
		if IsNull(WDate2) then WDate2 = ""
		oStream.WriteText(WDate2 & ",")
		oStream.WriteText vbcrlf
End Sub

Function ConvertCoord2Num(coord, Cols)
	Row = Asc(UCase(Left(coord, 1))) - 64
    if len(coord)=2 then
		colLength = 1
	else
		colLength = 2
	end if
    Col = Right(coord, colLength)
    ConvertCoord2Num = ((Row-1)*Cols) + Col
End Function

'Returns a comma,delimited list of the number of rows and columns for a specific plate
Function GetNumRowsCols(PlateBarcode)

	SQL = "SELECT row_count, col_count FROM inv_vw_plate_format, inv_plates WHERE plate_format_id = plate_format_id_fk AND plate_barcode = ?"
	Set Cmd = GetCommand(Conn, SQL, adCmdText)
	Cmd.Parameters.Append Cmd.CreateParameter("PlateBarcode", 200 , 1, 200, PlateBarcode)
	Set RS = Cmd.Execute
	Rows = RS("row_count")
	Cols = RS("col_count")
	GetNumRowsCols = Rows & "," & Cols

End Function
'Returns a comma delimited list from the dictionary
Function Dict2List(ByRef oDict, numPerRow)
	checkRow = true
	list = ""
	if isEmpty(numPerRow) or isNull(numPerRow) then checkRow = false
	rowCount = 1
	for each key in oDict
		list = list & key & ","
		if checkRow and rowCount = numPerRow then 
			list = list & "<BR>"
			rowCount = 0
		end if
		rowCount = rowCount + 1
	next
	if len(list) > 0 then list = left(list,(len(list)-1))
	Dict2List = list
End Function
%>