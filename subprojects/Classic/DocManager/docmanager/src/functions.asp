<%

'************************************************************************
'*PURPOSE	-- Write binary as file on disk. The binary could be		*
'*				upload JScriptTypeInfo object or binary written out 	*
'*				from Oracle BLOB										*
'*INPUT		-- Binary, fileName											*
'*OUTPUT	-- Full path include file name on disk						*
'************************************************************************
Function WriteBinaryToFile(bin, fileName)
	Dim fileTempFolder, subFolder, fileFullPath, htmlFullPath, fso
	
	Set fso = CreateObject("Scripting.FileSystemObject")
	if not fso.FolderExists(Session("sessionTempFolder")) then
		fso.CreateFolder(Session("sessionTempFolder"))
	end if
			
	subFolder = "at" & Month(Now) & Day(Now) & Hour(Now) & Minute(Now) & Second(Now)
	fileTempFolder = fso.BuildPath(Session("sessionTempFolder"), subFolder)

	fso.CreateFolder(fileTempFolder)
	fileFullPath = fileTempFolder & "\" & fileName	
	
	if typename(bin) = "JScriptTypeInfo" then 'when bin is the Field of Upload object
		bin.Value.SaveAs fileFullPath 
	else
	end if
	

	'this a few lines may be moved to somewhere else
	Session("fileTempFolder") = fileTempFolder
	Session("fileFullPath") = fileFullPath
	Session("fileName") = fileName
	Set Session("fileHexString") = bin.Value 'bin.Value is a byte()
	'----
	
	Set fso = nothing

	WriteBinaryToFile = fileFullPath

End Function

'************************************************************************
'*PURPOSE	-- Save .doc, .txt, .xls, .ppt as HTML for preview			*
'*INPUT		-- Full path of the to be saved file						*
'*OUTPUT	-- Full path of the HTML file								*
'************************************************************************
Function SaveFileAsHtml(fileFullPath)

	Dim returnStr
	Dim previewHtmlStr,	lastSlash, tempHttpStart, tempFileHttp, htmlHttp, fso, f
	
	returnStr = ""
	Set fso = CreateObject("Scripting.FileSystemObject") 

	previewHtmlStr = ""
	
	lastSlash = InStrRev(fileFullPath, "\")
	fileName = Right(fileFullPath, Len(fileFullPath) - lastSlash)
	
	Set f = fso.GetFile(fileFullPath)
	Session("docSize") = formatNumber(f.size/1000, 0)
	
	if LCase(Right(fileName, 4)) = ".txt" then
		'don't have to save as html for txt format
		htmlFullPath = fileFullPath
		
	'elseif LCase(Right(fileName, 4)) = ".doc" then
	elseif LCase(Right(fileName, 4)) = ".doc" or LCase(Right(fileName, 5)) = ".docx" then
	
		'htmlFullPath = replace(LCase(fileFullPath), ".doc", ".html")
		
		if LCase(Right(fileName, 5)) = ".docx" then
		    htmlFullPath = replace(LCase(fileFullPath), ".docx", ".html")
		else
		    htmlFullPath = replace(LCase(fileFullPath), ".doc", ".html")
		end if
		
		Dim wordAppCreated
		wordAppCreated = false
		
		'Often times Word App does not get created correctly on app start
		if UCase(TypeName(Application("wordApp"))) = "APPLICATION" OR UCase(TypeName(Application("wordApp"))) = "STRING" then
			wordAppCreated = true
		else
			returnStr = "ERROR: Office automation could not create Word.Application. " & err.number & " -- " & err.Description & "<br>Check DCOMCNFG settings. camsoft_admin, Interactive, IUSR, IWAM need to be granted Access and Launch permission for Word Document and in Default Security.  Then restart IIS."
		end if

		if wordAppCreated = true then
			Dim wordDoc
			'Give a bogus password to catch error for protected files
			on error resume next
			Set wordDoc = Application("wordApp").Documents.Open(fileFullPath, false, false, false, "bogusPassword")

			If err.number = 0 then
				
				'Session("textFound") = ""
				'on error resume next
				'Session("textFound") = ParseDocument(wordDoc, "WORD")
				
				on error resume next		
				wordDoc.SaveAs htmlFullPath, 8 'wdFormatHtml
								
				'get word document properties
				Session("docTitle") = wordDoc.BuiltInDocumentProperties("Title")
				Session("docAuthor") = wordDoc.BuiltInDocumentProperties("Author")
				Session("docLastModified") = wordDoc.BuiltInDocumentProperties("Last Save Time")
				Session("docComments") = wordDoc.BuiltInDocumentProperties("Comments")
			Elseif err.number = 5408 then
				returnStr = "ERROR: The file is password-protected. Please unprotect the file and try again."
			Else
				returnStr = "ERROR: " & err.number & " -- " & err.Description
			End if

			wordDoc.Close
						
			Set wordDoc = Nothing
			
		end if
			
	elseif LCase(Right(fileName, 4)) = ".ppt" or LCase(Right(fileName, 5)) = ".pptx" then
				
		'htmlFullPath = replace(LCase(fileFullPath), ".ppt", ".html")
		
		if LCase(Right(fileName, 5)) = ".pptx" then
		    htmlFullPath = replace(LCase(fileFullPath), ".pptx", ".html")
		else
		    htmlFullPath = replace(LCase(fileFullPath), ".ppt", ".html")
		end if
		
				
		Dim pptApp, pptPresentation
					
		Set pptApp = CreateObject("Powerpoint.Application")
		pptApp.Visible = true
		
		On Error Resume Next
		Set pptPresentation = pptApp.Presentations.Open(fileFullPath)

		If err.number = 0 then
			'Session("textFound") = ""
			'on error resume next
			'Session("textFound") = ParseDocument(pptPresentation, "POWERPOINT")

			pptPresentation.SaveAs htmlFullPath, 12 'ppSaveAsHTML
		Else
			returnStr = "ERROR: PowerPoint could not open this file. The file could be corrupted. Please correct it and retry."
		End if
		
					
		pptPresentation.Close
		pptApp.Quit
					
		'Delete the file in temp dir
		'fso.DeleteFolder(Session("sessionTempFolder"), true)
				
		Set pptPresentation = Nothing
		Set pptApp = Nothing
		
	elseif LCase(Right(fileName, 4)) = ".xls" or LCase(Right(fileName, 5)) = ".xlsx"  then

		'htmlFullPath = replace(LCase(fileFullPath), ".xls", ".html")
		
		if LCase(Right(fileName, 5)) = ".xlsx" then
		    htmlFullPath = replace(LCase(fileFullPath), ".xlsx", ".html")
		else
		    htmlFullPath = replace(LCase(fileFullPath), ".xls", ".html")
		end if

		Dim xlWorkBook
							
		'To fix CSBR-36657
		'Second parameter is UpdateLinks. Set to 0 to not update any links
		'bogusPassword is set to catch error for password-protected files.
		'IgnoreReadOnlyRecommended set to true
		on error resume next
		Set xlWorkBook = Application("xlApp").Workbooks.Open (fileFullPath, 0, false, , "bogusPassword", , True)
		
		If err.number = 0 then
			'Session("textFound") = ""
			'on error resume next
			'Session("textFound") = ParseDocument(xlWorkBook, "EXCEL")

			on error resume next		
			xlWorkBook.SaveAs htmlFullPath, 44 'xlHtml
		Elseif err.number = 1004 then
			'this has not been tested yet
			returnStr = "ERROR: The file is password protected. Please unprotect the file and try again." & "<br>" & _
						"Another possible cause of the error: Couldn't find the file on server. Check the temp file path."
		End if
		
		xlWorkBook.Close
			
		Set xlWorkBook = Nothing
		
	elseif LCase(Right(fileName, 4)) = ".pdf" then
					
		htmlFullPath = fileFullPath
							
	end if
				
	
	tempHttpStart = InStr(LCase(htmlFullPath), "\cfwtemp\")
	htmlHttp = Replace(Mid(htmlFullPath, tempHttpStart,  Len(htmlFullPath)), "\", "/")
	
	if returnStr = "" then
		returnStr = htmlHttp
	End if
	
	SaveFileAsHtml = returnStr
	
	Set fso = Nothing
	Set f = Nothing
	
End Function

Function ParseDocument(ByRef doc, ByVal fileType)
	Dim aRange
	Dim rangeText, cellText, shapeText, startPos, endPos
	Dim textFound
	Dim retVal
	
	retVal = ""
	
	Select Case fileType
	
		Case "WORD"
			For each Paragraph in doc.Paragraphs
				Set aRange = Paragraph.Range
				rangeText = pRange.Text
	
				startPos = Instr(rangeText, "SEP-")
	
				If startPos > 0 then
					endPos = InStr(startPos, rangeText, " ")
					if endPos = 0 then
						endPos = InStr(startPos, rangeText, Chr(10))
						if endPos = 0 then
							endPos = InStr(startPos, rangeText, Chr(13))
						end if
					end if
					textFound = Mid(rangeText, startPos, endPos - startPos)
					
					if retVal <> "" then
						retVal = retVal & "|" & textFound
					else
						retVal = textFound
					end if
				End If
			next
	
		Case "EXCEL"
		
			For each WorkSheet in doc.WorkSheets
				For each cell in WorkSheet.Cells
					cellText = cell.Value
	
					startPos = Instr(cellText, "SEP-")
	
					If startPos > 0 then
						endPos = InStr(startPos, cellText, " ")
						if endPos = 0 then
							endPos = InStr(startPos, cellText, Chr(10))
							if endPos = 0 then
								endPos = InStr(startPos, cellText, Chr(13))
								if endPos = 0 then
									endPos = InStr(startPos, cellText, Chr(23))
								end if
							end if
						end if
						
						if endPos = 0 then
							textFound = cellText
						else
							textFound = Mid(cellText, startPos, endPos - startPos)
						end if
						
						if retVal <> "" then
							retVal = retVal & "|" & textFound
						else
							retVal = textFound
						end if
					End If
				next
			next
		Case "POWERPOINT"
			
			for each slide in doc.Slides
				for each shape in slide.Shapes
					shapeText = shape.TextFrame.TextRange.Text
	
					startPos = Instr(shapeText, "SEP-")
	
					If startPos > 0 then
						endPos = InStr(startPos, shapeText, " ")
						if endPos = 0 then
							endPos = InStr(startPos, shapeText, Chr(10))
							if endPos = 0 then
								endPos = InStr(startPos, shapeText, Chr(13))
								if endPos = 0 then
									endPos = InStr(startPos, shapeText, Chr(23))
								end if
							end if
						end if
						
						if endPos = 0 then
							textFound = shapeText
						else
							textFound = Mid(shapeText, startPos, endPos - startPos)
						end if
						
						if retVal <> "" then
							retVal = retVal & "|" & textFound
						else
							retVal = textFound
						end if
					End If
				next
			next
	End Select
	
	ParseDocument = retVal	
	
End Function
%>

