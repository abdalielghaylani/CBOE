<%
'Name: RS2HTML
'Description:	Given a recordset and an xml template file, generates
'				HTML displaying the data in a table
'Parameters:	rsXML:	This is a reference to a recordset.  The recordset
'						must be a client side recordset (adUseClient).  Note: it 
'						will be disconnected.
'				fieldFileName:	the path to the xml template file
'				(pageSize):	# of records in a page
'				(pageNum):	page number of the data to view
'				(rowNum):	row number of the first row to view or of the row to be included in the page
'				(sortFields):	fields to sort on along with the direction to sort, column sort fields for the outer table are first then 
'								its the sort fields for the inner tables.  The format is as follows:
'								outer_column_name <direction> | sub_rs_name:column_name <direction>; sub_rs_name:column_name <direction>
'								e.g. column1 asc|tableName1:column2 asc; tableName2:column3 desc				
'Notes:			You must add select a primary key field as "pageRowNum" in order to use paging
'				Arguments are optionalized by passing in a null value.			
'				If you pass in a pageSize you must pass in either a pageNum or a rowNum
Function RS2HTML(ByRef rsXML, ByRef fieldTemplate, pageSize, pageNum, rowNum, rowIndex, sortFields, ByRef arrRowIndex, base_id)
	on error resume next
	bTimer = false
	' Track specific records in a dictionary object
	if bTimer then theStart = timer	
	if not isObject(Session("dictRowIndex")) then
		Set Session("dictRowIndex") = Server.CreateObject("Scripting.Dictionary")
	end if
	Set dictRowIndex = Session("dictRowIndex")
	if isEmpty(Session("indexCounter")) then
		Session("indexCounter") = 0
	end if
	pageRowNum = Session("indexCounter")
	
	' sort the RS
	if rowIndex = Session("lastRowIndex") and not isNull(sortFields) and sortFields <> "" then

		'parse the sortFields
		arrSortFields = split(sortFields,"|")
		size = Ubound(arrSortFields)
		if size = 1 then
			outerSort = arrSortFields(0)
			innerSort = arrSortFields(1)
		else
			outerSort = arrSortFields(0)
			innerSort = ""
		end if
			
		'response.write Session("innerSort") & "=innerSort<BR>"
		'set the session variable with the inner sort
		Session("innerSort") = innerSort

		lastOuterSort = Session("lastOuterSort")
		'get the other half of the sort fields if its there
		if outerSort = "" and len(lastOuterSort)>0 then  outerSort = lastOuterSort
		'Response.Write outerSort & "=outerSort<BR>"
		if len(outerSort) > 0 then
			'do the outer sort
			currSort = GetSort(outerSort,lastOuterSort)
			if isEmpty(Session("allOuterSorts")) then
				Session("allOuterSorts") = currSort
			else
				Session("allOuterSorts") = Session("allOuterSorts") & "|" & currSort
			end if
			Session("lastOuterSort") = currSort
			
			'Response.Write currSort & "=currSort<BR>"
			rsXML.Sort = currSort
		end if
		'response.write Session("allOuterSorts") & "=allOuterSort<BR>"
	
		if isArray(arrRowIndex) then
			rsxml.movefirst
			if base_id <> "" then 
				arrRowIndex = rsXML.GetRows(,,base_id) 
			else 
				arrRowIndex = rsXML.GetRows(,,"pageRowNum") 
			end if 
		end if
	
	end if
	Session("lastRowIndex") = rowIndex

	' page the RS
	if not isNull(pageSize) then
	
		rsXML.ActiveConnection = nothing
		rsXML.PageSize = pageSize
		
		' get a specific record
		if pageSize = 1 and not isNull(rowIndex) then
			rsXML.Filter = "pageRowNum = " & dictRowIndex.Item(rowIndex)
		else
			if not isNull(pageNum) then
				gotoPage = pageNum
			else
				gotoPage = (rowNum \ pageSize) + 1
			end if
		
			rsXML.AbsolutePage = gotoPage
			pageRowNumStart = pageRowNum
			for i = 1 to pageSize
				if not rsXML.EOF then
					'assign negative a number to avoid conflict with an actual primary key
					theData = rsXML("pageRowNum")
					rsXML("pageRowNum") = pageRowNum
					rsXML.Update
					dictRowIndex.Add cstr(pageRowNum), cstr(theData)
					pageRowNum = pageRowNum - 1
					rsXML.MoveNext
				end if
			next
			
			Set Session("dictRowIndex") = dictRowIndex
			Session("indexCounter") = pageRowNum
			pageRowNumEnd = pageRowNum
			rsXML.Filter = "pageRowNum <= " & pageRowNumStart & " AND pageRowNum > " & pageRowNumEnd
		end if

		if err.number <> 0 then
				rs2html = "<BR>Error in RS2HTML: Source: " & err.Source & "<BR> Error Number: " &  err.number & "<br> Error Description: " &  err.description & "<BR>RS Source: " & rsXML.source & "<BR> Hitlist ID: " & Session("hitlistid" & dbkey & formgroup)
				exit function
		end if
	end if


	Set oData = Server.CreateObject("MSXML2.FreeThreadedDOMDocument.6.0")
	rsXML.Save oData, 1

	'check for structures that need to be written to files, write the files and update the xml
	Set oDisplayInfo = Server.CreateObject("MSXML2.FreeThreadedDOMDocument.6.0")
	if isObject(fieldTemplate) then
		oDisplayInfo.loadXML(fieldTemplate.xml)
	else
		if left(fieldTemplate,5) = "<?xml" then
			oDisplayInfo.loadXML(fieldTemplate)	
		else
			oDisplayInfo.load(fieldTemplate)
		end if
	end if

	appKey = Application("Appkey")
	dbKey = Session("dbkey")
	formgroup = Session("formgroup")
	oDisplayInfo.setProperty "SelectionLanguage","XPath"
	Set oStructureNodes = oDisplayInfo.selectNodes("/DOCUMENT/DISPLAY/descendant::FIELD[@IS_STRUCTURE='1' and @DATA_LOCATION='table']")
	For each oNode in oStructureNodes
		mimetype = oNode.getAttribute("MIMETYPE")
		if mimetype = "" then mimetype = "chemical/x-cdx"
		columnName = oNode.getAttribute("COLUMN_NAME")
		tableName = oNode.getAttribute("TABLE_NAME")
		baseColumnName = oNode.getAttribute("BASE_COLUMN_NAME")
		dataURL = "/" & appKey & "/get_structure.asp?dbname=" & dbKey 
		dataURL = dataURL & "&formgroup=" & formgroup 
		dataURL = dataURL & "&mime_type=" & mimetype 
		dataURL = dataURL & "&table_name=" & tableName 
		dataURL = dataURL & "&column_name=" & columnName  
		dataURL = dataURL & "&base_column_name=" & baseColumnName
		oNode.setAttribute "dataURL", dataURL
	Next

	Set oFileNodes = oDisplayInfo.selectNodes("/DOCUMENT/DISPLAY/descendant::FIELD[@IS_STRUCTURE='1' and string-length(@FILE_EXT)>0]")
	if oFileNodes.length > 0 then
		dim stream
		set stream = server.createObject("adodb.stream")
		stream.type = adTypeBinary
		Set oFSO = Server.CreateObject("Scripting.FileSystemObject")
		For each oNode in oFileNodes
			fileExt = oNode.getAttribute("FILE_EXT")
			valueColumn = oNode.getAttribute("VALUE_COLUMNS")
			httpFileLocation = Application("TempFileDirectoryHTTP" & Application("appkey")) & "SessionDir/" & Session.SessionID & "/"
			fileLocation = Application("TempFileDirectory" & Application("appkey")) & "SessionDir\" & Session.SessionID & "\"
			oNode.setAttribute "FILE_LOCATION", httpFileLocation
			do while not rsXML.EOF
				ID = replace(cstr(rsXML("PageRowNum")),"-","")
				fileName = fileLocation & "struct" & ID & fileExt
				if fileExt = ".cdx" then
					stream.open
					stream.write(rsXML.fields(valueColumn).value)
					stream.saveToFile fileName, adSaveCreateOverWrite
					stream.close
				elseif fileExt = ".xml" then
					set oTS = oFSO.CreateTextFile(fileName,True)
					oTS.Write(rsXML.fields(valueColumn).value)
					oTS.close
					Set oTS = nothing
				end if
				rsXML.moveNext
			loop
		Next
	end if
	
	RS2HTML = DOM2HTML(oData, oDisplayInfo)

	Set stream = nothing
	Set oFSO = nothing
	Set oData = nothing
	
	rsXML.Filter = ""
	If err.number <> 0 then
		Response.write "RS2HTML Error: " & err.number & " - " & err.description & "<BR>"
	End if
	
	if bTimer then
		theEnd = timer	
		logaction(theEnd-theStart&":widgetTotal")
	end if
end function


Function DOM2HTML(oData, ByRef fieldTemplate)
	on error resume next
	bTimer = false
	if Not isObject(Application("oDataTemp")) then
		Application.Lock
		Set Application("oDataTemp") = Server.CreateObject("MSXML2.FreeThreadedDOMDocument.6.0")
		Set Application("oMergeDoc1") = Server.CreateObject("MSXML2.FreeThreadedDOMDocument.6.0")
		Set Application("oMergeDoc2") = Server.CreateObject("MSXML2.FreeThreadedDOMDocument.6.0")
		
		Set oFilterColumnsTransformTemp = Server.CreateObject("MSXML2.FreeThreadedDOMDocument.6.0")
		oFilterColumnsTransformTemp.async = false
		Set oHTMLTransformTemp = Server.CreateObject("MSXML2.FreeThreadedDOMDocument.6.0")
		oHTMLTransformTemp.async = false
		
		oFilterColumnsTransformTemp.load(Server.MapPath("/cfserverasp/source/xml_source/FilterColumns.xsl"))
		
		
		'adding special xsl template stuff
		if UCase(Application("biosarxsloverride")) <> "TRUE" then
			oHTMLTransformTemp.load(Server.MapPath("/cfserverasp/source/xml_source/XML2HTML.xsl"))
		else
			oHTMLTransformTemp.load(Server.MapPath("/biosar_browser/config/xml_templates/XML2HTML.xsl"))
		end if
		Set oFilterTransformTemplate = Server.CreateObject("Msxml2.XSLTemplate.6.0")
		oFilterTransformTemplate.stylesheet = oFilterColumnsTransformTemp
		Set oHTMLTransformTemplate = Server.CreateObject("Msxml2.XSLTemplate.6.0")
		oHTMLTransformTemplate.stylesheet = oHTMLTransformTemp

		'Set Application("oFilterColumnsTransform") = oFilterColumnsTransformTemp
		'Set Application("oHTMLTransform")= oHTMLTransformTemp
		Set Application("oFilterTransformTemplate") = oFilterTransformTemplate
		Set Application("oHTMLTransformTemplate")= oHTMLTransformTemplate

		Set oFilterColumnsTransformTemp = nothing
		Set oHTMLTransformTemp = nothing

		Application.UnLock
	end if
	
	Set oDataTemp = Application("oDataTemp") 
	Set oMergeDoc1 = Application("oMergeDoc1") 
	Set oMergeDoc2 =  Application("oMergeDoc2") 
	'Set oHTMLTransform = Application("oHTMLTransform")
	'Set oFilterColumnsTransform = Application("oFilterColumnsTransform")
	Set oFilterTransformTemplate = Application("oFilterTransformTemplate")  
	Set oHTMLTransformTemplate = Application("oHTMLTransformTemplate")
	bDebug = false
	
	oData.save(oDataTemp)
	if bDebug then Set oInputData = oDataTemp
	
	oMergeDoc1.load(Server.MapPath("/cfserverasp/source/xml_source/MergeDoc.xml"))
	oMergeDoc1.documentElement.appendChild(oDataTemp.documentElement)
	if isObject(fieldTemplate) then
		oMergeDoc1.documentElement.appendChild(fieldTemplate.documentElement)
	else
		Set oDisplayInfo = Server.CreateObject("MSXML2.FreeThreadedDOMDocument.6.0")
		if left(fieldTemplate,5) = "<?xml" then
			oDisplayInfo.loadXML(fieldTemplate)	
		else
			oDisplayInfo.load(fieldTemplate)
		end if
		oMergeDoc1.documentElement.appendChild(oDisplayInfo.documentElement)
		Set oDisplayInfo = nothing
	end if
	if bDebug then Set oInitialMerge = oMergeDoc1

	oMergeDoc1.setProperty "SelectionLanguage","XPath"
	innerSort = Session("innerSort")
	lastInnerSort = Session("lastInnerSort")
	if innerSort = "" and lastInnerSort <> "" then 	innerSort = lastInnerSort

	'Response.Write Session("lastInnerSort") & "=lastInnerSort<BR>"
	'Response.Write Session("innerSort") & "=innerSort<BR>"

	if innerSort <> "" and  lastInnerSort = "" then
		Set oCurr = server.CreateObject("Scripting.Dictionary")		
		arrCurrFields = split(innerSort,";")
		numCurrFields = ubound(arrCurrFields)
		for i = 0 to numCurrFields
			arrCurr = split(arrCurrFields(i),":")
			currSort = GetSort(arrCurr(1),null)
		'response.write currSort& "<BR>"
		if isEmpty(Session("allInnerSorts")) then
			Session("allInnerSorts") = arrCurr(0) & "." & currSort
		else
			Session("allInnerSorts") = Session("allInnerSorts") & "|" & arrCurr(0) & "." & currSort
		end if
		'response.write Session("allInnerSorts") & "=allInnerSort<BR>"
			'oCurr.Add arrCurr(0),arrCurr(1)
			oCurr.Add arrCurr(0), currSort
		next

	elseif innerSort <> "" and lastInnerSort <> "" then
		Set oCurr = server.CreateObject("Scripting.Dictionary")		
		arrCurrFields = split(innerSort,";")
		numCurrFields = ubound(arrCurrFields)
		for i = 0 to numCurrFields
			arrCurr = split(arrCurrFields(i),":")
			'currSort = GetSort(arrCurr(1),null)
			oCurr.Add arrCurr(0),arrCurr(1)
			'oCurr.Add arrCurr(0), currSort
		next
		
		Set oLast = server.CreateObject("Scripting.Dictionary")		
		arrLastFields = split(lastInnerSort,";")
		numLastFields = ubound(arrLastFields)
		for i = 0 to numLastFields
			arrLast = split(arrLastFields(i),":")
			oLast.Add arrLast(0),arrLast(1)
		next
		
		arrCurrSubRS = oCurr.Keys
		for i = 0 to oCurr.Count -1 
			currSubRSName = arrCurrSubRS(i)
			if oLast.Exists(currSubRSName) then
				currSort = GetSort(oCurr.Item(currSubRSName),oLast.Item(currSubRSName))
		if isEmpty(Session("allInnerSorts")) then
			Session("allInnerSorts") = currSubRSName & "." & currSort
		else
			'put new items at the beginning of the list
			Session("allInnerSorts") = currSubRSName & "." & currSort & "|" & Session("allInnerSorts")  
		end if
		'response.write Session("allInnerSorts") & "=allInnerSort<BR>"

'response.write currSubRSName & " " & currSort & "test<BR>"
				'oLast.Item(currSubRSName) = oCurr.Item(currSubRSName)
				oLast.Item(currSubRSName) = currSort
				'if not isEmpty(lastOuterSort) and not bDir then
				'	lastSortField = left(lastOuterSort,inStr(lastOuterSort," ")-1)
				'	lastSortFieldDirection = Mid(lastOuterSort,inStr(lastOuterSort," ")+1)

				'	if currSortField = lastSortField then
				'		if lastSortFieldDirection = "asc" then
				'			currSortFieldDirection = "desc"
				'		end if
				'	end if
				'end if

				
			else
				'currSort = GetSort(oCurr.Item(currSubRSName),null)
				oLast.Add currSubRSName,oCurr.Item(currSubRSName)
				'oLast.Add currSubRSName, currSort
			end if
		next
		Set oCurr = oLast
	end if
	if isObject(oCurr) then
		if oCurr.Count > 0 then
			'build sort string from dictionary
			arrKeys = oCurr.Keys
			arrItems = oCurr.Items
			innerSort = ""
			for i = 0 to oCurr.Count - 1
				innerSort = innerSort & arrKeys(i) & ":" & arrItems(i) & ";"
			next	
			innerSort = left(innerSort,len(innerSort)-1)
		end if
	end if
	Session("lastInnerSort") = innerSort
	if len(innerSort) > 0 then
		'Sort the inner recordsets
		'parse the sort variables
		'arrSortFields = split(innerSort,";")
		'numSortFields = ubound(arrSortFields)
		numSortFields = oCurr.Count 
		
		'Set oSortDict = server.CreateObject("Scripting.Dictionary")		
		SubRSNameList = ""
		arrSubRSName = oCurr.Keys
		for i = 0 to numSortFields -1
			SubRSNameList = SubRSNameList & arrSubRSName(i) & ","
		next		
		'for i = 0 to numSortFields -1
		'	arrSubRSName = split(arrSortFields(i),":")
		'	oSortDict.Add arrSubRSName(0),arrSubRsName(1)
		'	SubRSNameList = SubRSNameList & arrSubRSName(0) & ","
		'next
		SubRSNameList = left(SubRSNameList,len(SubRSNameList)-1)
		
		Set oSubRSNodes = oMergeDoc1.selectNodes("/MERGEDOC/DOCUMENT/DISPLAY/descendant::SUB_RS[contains('" & SubRSNameList & "',@NAME)]")
		if oSubRSNodes.length > 0 then
			For each oNode in oSubRSNodes
				SubRSName = oNode.getAttribute("NAME")
				'arrField = split(oSortDict.Item(SubRSName)," ")
				arrField = split(oCurr.Item(SubRSName)," ")
				sortDir = "ascending"

				if ubound(arrField) > 0 then
					direction = arrField(1)
					if direction = "desc" then sortDir = "descending"
				end if
				oNode.setAttribute "SORT_COLUMN", arrField(0)
				oNode.setAttribute "SORT_DIRECTION", sortDir
			Next
		end if

	end if

	' Set the location of mt.cdx
	Set oUpdateNodes = oMergeDoc1.selectNodes("/MERGEDOC/DOCUMENT/DISPLAY/descendant::FIELD[@IS_STRUCTURE=1]")
	if oUpdateNodes.length > 0 then
		For each oNode in oUpdateNodes
			oNode.setAttribute "MT_LOCATION",Application("TempFileDirectoryHTTP" & Application("appkey"))
		Next
	end if
	' Set the date format
	Set oUpdateNodes = oMergeDoc1.selectNodes("/MERGEDOC/DOCUMENT/DISPLAY/descendant::FIELD[@DATA_TYPE='date']")
	if oUpdateNodes.length > 0 then
		For each oNode in oUpdateNodes
			Set oAttribute = oMergeDoc1.createAttribute("DATE_FORMAT")
			oAttribute.value = Application("DATE_FORMAT")
			oNode.setAttributeNode(oAttribute)
		Next
	end if
	'Write needed ASP variables to XML doc
	Set oReplaceNodes = oMergeDoc1.selectNodes("/MERGEDOC/DOCUMENT/ASP_VARIABLES/*")
	For Each oNode In oReplaceNodes
		Select Case lcase(oNode.getAttribute("TYPE"))
			Case "session"
				varName = oNode.getAttribute("NAME")
				oNode.text = eval("Session(""" & varName & """)")
			Case "application"
				varName = oNode.getAttribute("NAME")
				oNode.text = eval("Application(""" & varName & """)")
			Case "function"
				theCall = oNode.getAttribute("CALL")
				oNode.text = eval(theCall)
			Case else
				varName = oNode.getAttribute("NAME")
				oNode.text = eval(varName)
		End Select
	Next
	Set oFirstNodes = oMergeDoc1.selectNodes("//DISPLAY/descendant::FIELD[position() >= 2 and @SHOW='1'][1]")
	For Each oNode in oFirstNodes
		oNode.setAttribute "IS_FIRST","1"
	Next
	Set oFirstNodes = oMergeDoc1.selectNodes("//SUB_RS/descendant::FIELD[1]")
	For Each oNode in oFirstNodes
		oNode.setAttribute "IS_FIRST","1"
	Next

	Set oProcessor = oFilterTransformTemplate.createProcessor()
	oProcessor.input = oMergeDoc1
	if bDebug then Set oFinalMerge = oMergeDoc1
	if bTimer then theStart = timer	
	'oMergeDoc1.transformNodeToObject oFilterColumnsTransform, oMergeDoc2
	oProcessor.transform()
	oMergeDoc2.loadxml oProcessor.output
	if bTimer then
		theEnd = timer	
		logaction(theEnd-theStart&":filterColumns")
	end if
	if bDebug then Set oFiltered = oMergeDoc2
	
	' Post process data
	Set oReplaceNodes = oMergeDoc2.selectNodes("//FIELD[@POST_PROCESS='true']")
	For Each oNode In oReplaceNodes
	'for i=0 to (oReplaceNodes.length - 1)
	'	set oNode = oReplaceNodes.item(0)
		nodeText = oNode.text
		while instr(nodeText,"<!") > 0
			nodeText = evalASP(replace(nodeText,chr(10),"###"))
		wend
		oNode.text = nodeText
	Next
	if bDebug then Set oPostProcessed = oMergeDoc2

	if bDebug then
		oInputData.save(Server.MapPath("/" & Application("AppKey") & "/config/xml_Templates/debug_01_InputData.xml"))
		oInitialMerge.save(Server.MapPath("/" & Application("AppKey") & "/config/xml_Templates/debug_02_InitialMerge.xml"))
		oFinalMerge.save(Server.MapPath("/" & Application("AppKey") & "/config/xml_Templates/debug_03_FinalMerge.xml"))
		oFiltered.save(Server.MapPath("/" & Application("AppKey") & "/config/xml_Templates/debug_04_Filtered.xml"))
		oPostProcessed.save(Server.MapPath("/" & Application("AppKey") & "/config/xml_Templates/debug_05_PostProcessed.xml"))
	end if

	Set oProcessor = oHTMLTransformTemplate.createProcessor()
	oProcessor.input = oMergeDoc2
	oProcessor.transform()

	'DOM2HTML = oMergeDoc2.transformNode(oHTMLTransform)
	DOM2HTML = oProcessor.output
	
	Set oDataTemp = nothing
	Set oDisplayInfo = nothing
	Set oMergeDoc1 = nothing
	Set oMergeDoc2 = nothing

	'Set oFilterColumnsTransform = nothing
	'Set oHTMLTransform = nothing
	Set oProcessor = nothing
	Set oFilterTransformTemplate = nothing
	Set oHTMLTransformTemplate = nothing

	If err.number <> 0 then
		Response.write "DOM2HTML Error: " & err.number & " - " & err.description & "<BR>"
	End if

end function


Sub CreateXMLTemplateFromRS(ByRef rsXML, inputPath, structureForm)

	Set oData = Server.CreateObject("MSXML2.DOMDocument")
	
	rsXML.Save oData, 1
	Call CreateXMLTemplateFromDOM(oData,inputPath, structureForm)	
	'oData.save(Server.MapPath("/" & Application("AppKey") & "/xmlTemplates/testCreateFileData.xml"))

	Set oData = nothing

end sub

Sub CreateXMLTemplateFromDOM(domXML, inputPath, structureForm)

	Set oCreateFileTransform = Server.CreateObject("MSXML2.DOMDocument")
	Set oNewDoc = Server.CreateObject("MSXML2.DOMDocument")
	
	
	oCreateFileTransform.load("/cfserverasp/source/xml_source/CreateFile.xsl")
	domXML.transformNodeToObject oCreateFileTransform, oNewDoc	

	Set oReplaceNodes = oNewDoc.getElementsByTagName("FIELD")
	For Each oNode In oReplaceNodes
		oNode.setAttribute "VALUE_CLASS","default"
		oNode.setAttribute "COLSPAN","1"
		oNode.setAttribute "SHOW","1"
		if oNode.getAttribute("FORM_NAME") <> "" then
			oNode.setAttribute "FORM_NAME", structureForm
		end if
	Next

	oNewDoc.save(inputPath)

	Set oCreateFileTransform = nothing
	Set oNewDoc = nothing

end sub

Sub ShowNavBar(ByRef rsXML, pageSize, pageNum)
	Dim theNavBar 
	scriptName = Request.ServerVariables("SCRIPT_NAME")
	path = Server.MapPath("/" & Application("AppKey"))
	linkClass = "MenuLink"
	qs = ""

	'build qs
	isFirst = True
	for each param in Request.QueryString
		if param <> "pageNum" then
			if isFirst then
				isFirst = false
			else
				qs = qs & "&"
			end if
			qs = qs & param & "=" & Request.QueryString(param)
		end if
	next
	
	rsXML.pageSize = pageSize
	pageCount = rsXML.PageCount

		
	if isNull(pageNum) then pageNum = 1
	if cInt(pageNum) > pageCount then pageNum = pageCount
	Response.Write("Page " & pageNum & " of " & pageCount & "&nbsp;")

	'determine the page numbers for the bar
	if (pageNum mod 10) = 0 then
		firstPageInNav = pageNum - 9
	else
		firstPageInNav = pageNum - (pageNum mod 10) + 1
	end if
	lastPageInNav = firstPageInNav + 9
	if lastPageInNav > pageCount then lastPageInNav = pageCount

	
	theNavBar = "<SPAN CLASS=""" & linkClass & """>"
	if firstPageInNav > 10 then
		theNavBar = theNavBar & "<A HREF=""" & scriptName & "?pageNum=" & (firstPageInNav - 10) & "&" & qs & """ CLASS=""" & linkClass & """>&lt;&lt;</a>&nbsp;"
	end if
	if cInt(pageNum) > 1 then
		theNavBar = theNavBar & "<A HREF=""" & scriptName & "?pageNum=" & (pageNum - 1) & "&" & qs & """ CLASS=""" & linkClass & """>&lt;</a>&nbsp;"
	end if
	

	theNavBar = theNavBar & "["
	for i = firstPageInNav to lastPageInNav
		If i >= 10 then
			pad=""
		end if
		if i <> cInt(pageNum) then
			theNavBar = theNavBar & "<A HREF=""" & scriptname & "?pageNum=" & i & "&" & qs
			theNavBar = theNavBar & """ CLASS=""" & linkClass & """>" & pad & i & "</a>"
		else
			theNavBar = theNavBar & "" & pad & i & ""
		end if
		if i <> lastPageInNav then theNavBar = theNavBar & " "
	next
	theNavBar = theNavBar & "]"
	
	if cInt(pageNum) < lastPageInNav then
		theNavBar = theNavBar & "&nbsp;<A HREF=""" & scriptname & "?pageNum=" & (pageNum + 1) & "&" & qs
		theNavBar = theNavBar & """ CLASS=""" & linkClass & """><b>&gt;</b></a>"
	end if
	if lastPageInNav <> pageCount then
		theNavBar = theNavBar & "&nbsp;<A HREF=""" & scriptname & "?pageNum=" & (lastPageInNav + 1) & "&" & qs
		theNavBar = theNavBar & """ CLASS=""" & linkClass & """><b>&gt;&gt;</b></a>"
	end if
	
	Response.Write theNavBar
end sub

Function getQS(removeFieldName)
	isFirst = true
	for each param in Request.QueryString
		if param <> removeFieldName then
			if isFirst then
				isFirst = false
			else
				qs = qs & "&"
			end if
			qs = qs & param & "=" & Request.QueryString(param)
		end if
	next
	
	getQS = qs
end function

Function getSortURL()
	scriptName = Request.ServerVariables("SCRIPT_NAME")
	qs = getQS("sortFields")
	if len(qs) > 0 then
		getSortURL = scriptName & "?" & qs
	else
		getSortURL = scriptName
	end if
end function

Function evalASP(nodeText)
	on error resume next
	aspStart = instr(nodeText,"<!") + 2
	aspEnd = instr(nodeText,"!>")
	aspLength = aspEnd - aspStart
	aspText = mid(nodeText,aspStart,aspLength)
	aspValue = eval(aspText)
	if Err.number <> 0 then
		evalASP = "Invalid ASP function"
	else
		if aspStart - 2 = 1 then ' asp string starts node text
			preASPNodeText = ""
		else
			preASPNodeText = left(nodeText,aspStart-3)
		end if
		if aspEnd + 2 = len(nodeText) then ' asp string ends node text
			postASPNodeText = right(nodeText,1)
		else
			postASPNodeText = mid(nodeText,aspEnd + 2)
		end if	
		evalASP = preASPNodeText & aspValue & postASPNodeText
	end if
	on error goto 0
end function

' Creates new Field Element Node
' Parameters: DOMObject,Value_Columns, Value_Class, Header_Name, Header_Class, Sort_Column, Display_Name, Name_Class, Colspan, Width, Height, Show, Text
' returns Node object
Function CreateFieldNode(ByRef oDOM, Value_Columns, Value_Class, Header_Name, Header_Class, Sort_Column, Display_Name, Name_Class, Colspan, Width, Height, Show, Text)

	Set newNode = oDOM.createNode(1, "FIELD", "")
	if not isNull(Value_Columns) then newNode.setAttribute "VALUE_COLUMNS", Value_Columns
	if not isNull(Value_Class) then newNode.setAttribute "VALUE_CLASS", Value_Class
	if not isNull(Header_Name) then newNode.setAttribute "HEADER_NAME", Header_Name
	if not isNull(Header_Class) then newNode.setAttribute "HEADER_CLASS", Header_Class
	if not isNull(Sort_Column) then newNode.setAttribute "SORT_COLUMN", Sort_Column
	if not isNull(Display_Name) then newNode.setAttribute "DISPLAY_NAME", Display_Name
	if not isNull(Name_Class) then newNode.setAttribute "NAME_CLASS", Name_Class
	if not isNull(Colspan) then newNode.setAttribute "COLSPAN", Colspan
	if not isNull(WIDTH) then newNode.setAttribute "WIDTH", Width
	if not isNull(HEIGHT) then newNode.setAttribute "HEIGHT", Height
	if not isNull(SHOW) then newNode.setAttribute "SHOW", Show
	if not isNull(Text) then newNode.text = Text

	Set CreateFieldNode = newNode

	Set newNode = nothing	
end function

Function GetSort(currSort, lastSort)
	currSortField = currSort
	currSortFieldDirection = "asc"

	arrCurr = split(currSort," ")

	bDir = ubound(arrCurr) > 0
	if bDir then 
		currSortField = arrCurr(0)
		currSortFieldDirection = arrCurr(1)
	end if
		
	if (not isEmpty(lastSort) and not isNull(lastSort)) and not bDir  then
		'DGB sometimes lastSort does not contain the direction
		'which causes the split on the space to throw error below.
		if inStr(lastSort," ") > 0 then
			lastSortField = left(lastSort,inStr(lastSort," ")-1)
			lastSortFieldDirection = Mid(lastSort,inStr(lastSort," ")+1)
		else
			lastSortField = lastSort
			lastSortFieldDirection = "asc"
		End if
		if currSortField = lastSortField then
			if lastSortFieldDirection = "asc" then
				currSortFieldDirection = "desc"
			end if
		end if
	end if
			
	GetSort = currSortField & " " & currSortFieldDirection
end Function

'**********************************************************************
'****** LOGGING routines
'**********************************************************************
Sub LogAction(ByVal inputstr)
		on error goto 0
		filepath = Application("ServerDrive") & "\" & Application("ServerRoot") & "\cfwlog.txt"
		Set fs = Server.CreateObject("Scripting.FileSystemObject")
		Set a = fs.OpenTextFile(filepath, 8, True)  
		a.WriteLine Now & ": " & inputstr
		a.WriteLine " "
		a.close
End Sub

%>