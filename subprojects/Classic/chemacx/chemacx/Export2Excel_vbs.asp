<%' Copyright 1998-2000, CambridgeSoft Corp., All Rights Reserved%>
<%

'*****************************************************************************	
'* Creates an Excel Application object and opens a Workbook
'*	
Function OpenXLTemplate(XlsPath)
	Set xlApplication = CreateObject("Excel.Application")
	xlApplication.Visible = False
	xlApplication.DisplayAlerts = False
	xlApplication.Workbooks.open XlsPath 
	Set OpenXLTemplate = xlApplication
End Function


'*****************************************************************************	
'* Saves the Xls file to a given path and filename
'* Returns a string with the Http path to the saved file
'* If filepath is ommited: defaults to execution directory
'* .xls is added to FileNameRoot
'* bolAddTimeStamp (True|False) will add a time stamp to FileNameRoot	
Function SaveXLTemplateAs(byref XLApplication,byval XlsHttpPath, byval XlsFileNameRoot,byval bolAddTimeStamp)
	if XlsHttpPath = "" then		
		strPath = "."
	else
		strPath = XlsHttpPath
	end if
	
	if  bolAddTimeStamp then
		strFileName = GenFileName(XlsFileNameRoot)& ".xls"
	else 
		strFileName = XlsFileNameRoot & ".xls"
	end if	
	strFullHttpPath = strPath & "/" & strFileName 
	strXlFilePath = Server.MapPath(strFullHttpPath)
		
	Set fso = Server.CreateObject("Scripting.FileSystemObject")
	if fso.FileExists(strXlFilePath) then
		fso.DeleteFile(strXlFilePath)
	end if
	XLApplication.ActiveWorkBook.SaveAs  strXlFilePath
	SaveXLTemplateAs = strFullHttpPath
End Function

'*****************************************************************************	
'* Quits Excel
'*	
Function QuitXLTemplate(byref XLApplication)
	xlApplication.Quit			' Close the Workbook
	Set xlApplication = Nothing
End Function


Function GetWorkSheet(byref xlApplication, byval TargetSheetName)
	Set GetWorkSheet = xlApplication.ActiveWorkBook.WorkSheets(TargetSheetName)
End Function


'*****************************************************************************	
'* Checks if a sheet exists in active Workbook of an Excel instance 
'* Returns True/False
Function XlSheetExists(byref xlApplication, byval TargetSheetName)
	TargetSheetExists = False
	For Each Sheet in xlApplication.ActiveWorkBook.Worksheets
		if Sheet.Name = TargetSheetName then
			TargetSheetExists = True
			Exit For
		End if
	Next
	XlSheetExists = TargetSheetExists
End Function



'*****************************************************************************	
'* Creates, if it doesn't already exists, an XL worksheet named by the target 
'* attribute by making a copy of the sheet in the Source attribute.
'* Returns the Excel sheet object.
Function CreateXLSheet(byref xlApplication, byval TargetSheetName, byval SourceSheetName, byval bolDeleteSource)
	'create the sheet
	if SourceSheetName = "" then
	Set SourceSheet = xlApplication.ActiveSheet
	Else
	Set SourceSheet = xlApplication.Worksheets(SourceSheetName)
	End if
	SourceSheet.Copy ,SourceSheet
	XlApplication.ActiveSheet.Name = TargetSheetName
	Set TargetSheet = xlApplication.Worksheets(TargetSheetName)

	'Delete the source sheet if requested
	if bolDeleteSource then
		SourceSheet.Delete
	End if

	Set CreateXLSheet = TargetSheet
End Function 



'*****************************************************************************	
'* Populates an Excel file from a dictionary object
'* Each key is interpreted as the target Excel cell
'* Each value is inserted into that cell
Function FillXlSheetFromDict(byref xlApplication, byref TargetSheet, byRef oDict, byval fontColorIndex)		
'Loop through the dictionary object and populate cells
	arrKeys = oDict.Keys
	arrItems = oDict.Items
	For intloop=0 To oDict.Count-1
		'response.write arrkeys(intloop) &" = " & arrItems(intloop) &"<BR>"
		TargetSheet.Range(arrkeys(intloop)).Value = arrItems(intloop)
		TargetSheet.Range(arrkeys(intloop)).Font.ColorIndex = fontColorIndex
	Next
End Function

'*********************************************************************************
'*Builds an associative array (Dictionary) with the cell positions and field names.
'*Returns a field map dictionary to be used in conjuction with an AdoRS to populate Excel
'*Requires a field map string of the form:  "B11:filed1,E11:field2,H11,field3..."
'*Where B11 defines the Excel row and column. field1 corresponds to field from Ado RS
Function BuildFieldMapDict(byval strFieldMap)
Dim arrCells()
Dim arrFields()

'Create the filed map dictionary
	Set FieldMapDict = Server.CreateObject("Scripting.Dictionary")

'Parse the Field Map string to the fields and cells arrays and build dictionary
	arrTemp = Split(strFieldMap,",")
	redim arrCells(Ubound(arrTemp))
	redim arrFields(Ubound(arrTemp))
		For intloop = 0 To Ubound(arrTemp)
			colonPos= InStr(arrTemp(intloop),":")
			afterColon= Len(arrTemp(intloop))-colonPos
			arrCells(intloop)= Left(arrTemp(intloop),colonPos-1)
			arrFields(intloop)= Mid(arrTemp(intloop),colonPos+1,afterColon)
			FieldMapDict.Add arrCells(intloop), arrFields(intloop)
		'response.write arrCells(intloop) & " = " & arrFields(intloop) & "<BR>"
		Next
	Set BuildFieldMapDict = FieldMapDict
	Set FieldMapDict = Nothing
End Function



'*****************************************************************************	
'* Populates an Excel file from an Ado Recordset
'* 
Function FillXlSheetFromAdoRS(byref XlApp,byref TargetSheet,byref AdoRS,byref FieldMapDict,byval fontColorIndex,byval AdoStartRow, byval AdoEndRow)
Dim XlStartRow
Dim AdoRowCount
Dim arrColLetters()
Dim arrColNums()

AdoRowCount = AdoEndRow - AdoStartRow

if (AdoRS.EOF= False AND AdoRS.BOF= False) then
'Extract the Column, Row and fieldName arrays from the dictionary
arrtemp = FieldMapDict.Keys
arrFieldNames = FieldMapDict.Items
arrtempLength= Ubound(arrtemp)
Redim arrColLetters(arrtempLength)
Redim arrRowNums(arrtempLength)
For intloop= 0 to arrtempLength
arrColLetters(intloop) = Left(arrtemp(intloop),1) 
arrRowNums(intloop) = CInt(Mid(arrtemp(intloop),2,Len(arrtemp(intloop))))
Next

XlStartRow = arrRowNums(0)
'Loop the AdoRS and populate the Target WorkSheet
	
	'AdoRS.MoveFirst
	' Move to start Row
	For intloop= 0 to AdoRowCount-1
	AdoRS.MoveNext
	Next
	AdoRow = 0
	Do 
		'Loop over the field names array 
		
		
		For intloop= 0 To Ubound(arrFieldNames)
			tempCell= arrColLetters(intloop) & Cstr(arrRowNums(intloop)+AdoRow) 
			if InStr(arrFieldNames(intloop),"~") = 0 then
			theValue = AdoRS(arrFieldNames(intloop))
			Else
			arrSubFields = Split(arrFieldNames(intloop),"~")
					thevalue = ""
					For intloop2 = 0 To Ubound(arrSubFields)
						'response.write intloop2 & " = " & arrSubFields(intloop2) & "<BR>"
					 	thevalue = thevalue & AdoRS(arrSubFields(intloop2)) & " "
					Next
			End If
			'response.write tempCell & " =" & theValue & "<BR>"
			TargetSheet.Range(tempCell).Value = theValue
			TargetSheet.Range(tempCell).Font.ColorIndex = fontColorIndex
		Next
	AdoRS.MoveNext
	AdoRow= AdoRow + 1
	Loop While AdoRow < AdoRowCount
End if
End Function

'***************************************************************************8
'Deletes the targetSheet from Excel template
'
Function XLDeleteSheet(byref xlApplication,byval TargetSheetName)
	Set WorkSheet = xlApplication.Worksheets(TargetSheetName)
	WorkSheet.Delete
	Set WorkSheet = Nothing
End Function


'*************************************************************************
'* Serve the newly created .xls file
'* Setting bServeLink to True serves a link to .doc file
'* Setting bServeLink to False redirects the browser to the .doc file
Function ServeFile(byval FileURL, byval bServeLink)
	if bServeLink then
	Response.Write("<HTML><BODY>")
	Response.Write("<BR><BR><BR><CENTER>")
	Response.Write("Click <A HRef=" & FileURL & ">Here</A> to get your ChemACX shopping cart as an Excel file<BR> (right-click to save it on your local disk)")
	Response.Write("</CENTER>")
	Response.Write("</BODY></HTML>")
	else
	Response.redirect FileURL
	end if
End Function

'*****************************************************************************	
'* Autogenerates a file name based on time stamp
'*	
Function GenFileName(RootName)
	dim fname
	fname = RootName
	systime=now()
	fname= fname & cstr(year(systime)) & cstr(month(systime)) & cstr(day(systime))
	fname= fname  & cstr(hour(systime)) & cstr(minute(systime)) & cstr(second(systime))
	GenFileName = fname
End Function
%>
