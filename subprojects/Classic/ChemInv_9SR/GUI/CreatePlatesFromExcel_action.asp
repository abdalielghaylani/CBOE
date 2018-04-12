<!--#INCLUDE VIRTUAL = "/cheminv/api/apiUtils.asp"-->
<!--#INCLUDE VIRTUAL = "/cheminv/gui/guiUtils.asp"-->
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/upload.asp"-->

<%
Dim oUpload
Dim sFileName
Dim sPath
Dim cnExcel
Dim rsData 
Dim bDebugPrint
Dim Conn
Dim Cmd
Dim errCount
Dim sError
bDebugPrint = false
errCount = 0
sError = "Error(s) occurred.<BR>"

' Instantiate Upload Class
Set oUpload = GetUpload()
' Grab the file name
sFileName = oUpload.Item("File1").FileName
' Compile path to save file to
sPath = Server.MapPath("\" & Application("AppKey") & "\custom\uploads\" & sFileName)

' Save the binary data to the file system
oUpload("File1").value.SaveAs sPath
' Release upload object from memory
Set oUpload = Nothing

' Create a new ADO Connection Object
Set cnExcel = Server.CreateObject("ADODB.Connection")
sConnStr = "Provider=MSDASQL.1;Persist Security " & _
           "Info=False;Extended Properties=" & Chr(34) & _
           "DSN=Excel Files;DBQ=" & sPath & ";" & Chr(34)
           
'cnExcel.ConnectionString = m_strConnectionString
' Set the Cursor location to Client Side
cnExcel.CursorLocation = 3 'adUseClient

On Error Resume Next
cnExcel.Open "DRIVER=Microsoft Excel Driver (*.xls);DBQ=" & sPath
On Error GoTo 0
if cnExcel.State = 0 then
	sError = "Error opening the Excel file. Please confirm the excel file is valid by opening the file in Microsoft Excel."
	Response.redirect "CreatePlatesFromExcel.asp?errmsg=" & sError
end if

Set objCat = CreateObject("ADOX.Catalog")
Set objCat.ActiveConnection = cnExcel
i = 0
For Each tbl In objCat.Tables
    if i = 0 then sSheetName = tbl.Name end if
    i = i + 1
Next
Set objCat = Nothing
%>

<html>
<head>
<title><%=Application("appTitle")%> -- Create Plates From Excel</title>
<script language=javascript src="/cheminv/Choosecss.js"></SCRIPT>
<script language=javascript src="/cheminv/gui/refreshGUI.js"></SCRIPT>
<script language="javascript" src="/cheminv/utils.js"></script>
<script language="JavaScript">
	window.focus();
</script>
</head>
<body>
<br><br><br><br><br><br>
<%
Response.Write "<div align=""center"" ID=""processingImage""><img src=""/cfserverasp/source/graphics/processing_Ybvl_Ysh_grey.gif"" WIDTH=""130"" HEIGHT=""100"" BORDER=""""></DIV>"
Response.Flush

' Obtain the recordset by executing SQL statement with sheet name
Set rsData = cnExcel.Execute("SELECT * FROM [" & sSheetName & "]")
' If records were returned the save the file
If rsData.RecordCount > 0 Then
	'use DOM doc to get around file already exists error
    Set oTransforms = server.CreateObject("MSXML2.DOMDOCUMENT.4.0")
    'rsData.Save sPath&".xml", adPersistXML
    'Response.End
    rsData.Save oTransforms, 1 'adPersistXML
    if bDebugPrint then oTransforms.save(sPath&".xml")
    'oTransforms.save(sPath&".xml")
    'Response.End
	oTransforms.setProperty "SelectionLanguage","XPath"
	oTransforms.setProperty "SelectionNamespaces","xmlns:s='uuid:BDC6E3F0-6DA3-11d1-A2A3-00AA00C14882' xmlns:rs='urn:schemas-microsoft-com:rowset' xmlns:z='#RowsetSchema'"
	'get column names, the names could change if the column row is changed, but we select by position
	Set nlColumns = oTransforms.selectNodes("//s:AttributeType")
	for each oNode in nlColumns
		if oNode.getAttribute("rs:number") = 1 then rowNumCol = oNode.getAttribute("name")
		if oNode.getAttribute("rs:number") = 2 then barcode1Col = oNode.getAttribute("name")
		if oNode.getAttribute("rs:number") = 3 then barcode2Col = oNode.getAttribute("name")
		if oNode.getAttribute("rs:number") = 4 then barcode3Col = oNode.getAttribute("name")
		if oNode.getAttribute("rs:number") = 5 then barcode4Col = oNode.getAttribute("name")
		if oNode.getAttribute("rs:number") = 6 then plateFormatCol = oNode.getAttribute("name")
		if oNode.getAttribute("rs:number") = 7 then plateTypeCol = oNode.getAttribute("name")
		if oNode.getAttribute("rs:number") = 8 then reformatMapCol = oNode.getAttribute("name")
		if oNode.getAttribute("rs:number") = 9 then barcodeCol = oNode.getAttribute("name")
		if oNode.getAttribute("rs:number") = 10 then numCopiesCol = oNode.getAttribute("name")
		if oNode.getAttribute("rs:number") = 11 then libraryCol = oNode.getAttribute("name")
		if oNode.getAttribute("rs:number") = 12 then locationCol = oNode.getAttribute("name")
		if oNode.getAttribute("rs:number") = 13 then statusCol = oNode.getAttribute("name")
	next

	Set nlTransforms = oTransforms.selectNodes("//rs:data/z:row")
	'prepare command for lookups
	Call GetInvCommand("SQLText", &H0001) 'adCmdText
	ServerName = Request.ServerVariables("Server_Name")
	Credentials = "&CSUserName=" & Session("UserName" & "cheminv") & "&CSUSerID=" & Session("UserID" & "cheminv")
	
	if nlTransforms.length = 0 then
	'Response.Write nlTransforms.length
	'Response.End
		sError = sError & "The data in the excel file appears to be incomplete or incorrect."
		bError = true	
		ExportToXML = False
	else
		for each oNode in nlTransforms
			bError = false
			rowNum = oNode.getAttribute(rowNumcol)
			sourceBarcode1 = oNode.getAttribute(barcode1Col)
			sourceBarcode2 = oNode.getAttribute(barcode2Col)
			sourceBarcode3 = oNode.getAttribute(barcode3Col)
			sourceBarcode4 = oNode.getAttribute(barcode4Col)
			reformatMap = oNode.getAttribute(reformatMapCol)
			'barcodePrefix = oNode.getAttribute("c8")
			'barcodeSuffix = oNode.getAttribute("c9")
			barcodes = oNode.getAttribute(barcodeCol)
			numCopies = oNode.getAttribute(numCopiesCol)
			library = oNode.getAttribute(libraryCol)
			location = oNode.getAttribute(locationCol)
			status = oNode.getAttribute(statusCol)
			
			'Response.Write sourceBarcode1 & ":" & sourceBarcode2 & ":" & sourceBarcode3 & ":" & sourceBarcode4 & ":" & reformatMap & ":" & barcodes & ":" & numCopies
			'Response.End

			SQL = "SELECT plate_id, plate_format_id_fk FROM " & Application("CHEMINV_USERNAME") & ".inv_plates WHERE plate_barcode = ?" 
			Cmd.CommandText = SQL
			Cmd.Parameters.Append Cmd.CreateParameter(, 200, 1, 100,sourceBarcode1)
			Set RS = Cmd.Execute
			if RS.BOF or RS.EOF then
				sError = sError & "Row " & rowNum & " could not be processed because one of Source Plate Barcode 1 is not valid.<BR><BR>"
				bError = true	
			else
				plateID1 = RS("plate_id").value
				plateFormatID1 = RS("plate_format_id_fk")
			end if
			Cmd.Parameters.Delete 0
			'Response.Write plateFormatID1
			'Response.End

			bDaughter = false
			If lcase(reformatMap) = "daughter" then
				bDaughter = true

				SQL = "SELECT xmldoc_id FROM " & Application("CHEMINV_USERNAME") & ".inv_xmldocs WHERE lower(name) = ?" 
				Cmd.CommandText = SQL
				Cmd.Parameters.Append Cmd.CreateParameter(, 200, 1, 200, lcase(reformatMap) & plateFormatID1)
				Set RS = Cmd.Execute
				if RS.BOF or RS.EOF then
					sError = sError & "Row " & rowNum & " could not be processed because the no daughtering map could be found.<BR><BR>"
					bError = true	
				else
					xmldocID = RS("xmldoc_id").value
				end if
				Cmd.Parameters.Delete 0
				'Response.write sError
				'Response.End
			else

				SQL = "SELECT xmldoc_id FROM " & Application("CHEMINV_USERNAME") & ".inv_xmldocs WHERE lower(name) = ?" 
				Cmd.CommandText = SQL
				Cmd.Parameters.Append Cmd.CreateParameter(, 200, 1, 200, lcase(reformatMap))
				Set RS = Cmd.Execute
				if RS.BOF or RS.EOF then
					sError = sError & "Row " & rowNum & " could not be processed because the plate template is not valid.<BR><BR>"
					bError = true	
				else
					xmldocID = RS("xmldoc_id").value
				end if
				Cmd.Parameters.Delete 0
			
			end if

			if not bDaughter then
				SQL = "SELECT plate_id, plate_format_id_fk FROM " & Application("CHEMINV_USERNAME") & ".inv_plates WHERE plate_barcode = ?" 
				Cmd.CommandText = SQL
				Cmd.Parameters.Append Cmd.CreateParameter(, 200, 1, 100,sourceBarcode2)
				Set RS = Cmd.Execute
				if RS.BOF or RS.EOF then
					sError = sError & "Row " & rowNum & " could not be processed because one of Source Plate Barcode 2 is not valid.<BR><BR>"
					bError = true	
				else
					plateID2 = RS("plate_id").value
				end if
				Cmd.Parameters.Delete 0

				Cmd.Parameters.Append Cmd.CreateParameter(, 200, 1, 100,sourceBarcode3)
				Set RS = Cmd.Execute
				if RS.BOF or RS.EOF then
					sError = sError & "Row " & rowNum & " could not be processed because one of Source Plate Barcode 3 is not valid.<BR><BR>"
					bError = true	
				else
					plateID3 = RS("plate_id").value
				end if
				Cmd.Parameters.Delete 0

				Cmd.Parameters.Append Cmd.CreateParameter(, 200, 1, 100,sourceBarcode4)
				Set RS = Cmd.Execute
				if RS.BOF or RS.EOF then
					sError = sError & "Row " & rowNum & " could not be processed because one of Source Plate Barcode 4 is not valid.<BR><BR>"
					bError = true	
				else
					plateID4 = RS("plate_id").value
				end if
				Cmd.Parameters.Delete 0
			end if
					
			'check # and uniqueness of barcodes
			'-- TODO: check for blank lines at end of file
			arrBarcodes = split(barcodes,",")
			'Response.Write barcodes & "TEST"
			'Response.End
			'arrBarcodes = split(barcodes,",")
			numBarcodes = ubound(arrBarcodes)+1
			if numBarcodes <> cInt(numCopies) then
				sError = sError & "Row " & rowNum & " could not be processed because the number of barcodes does not equal the number of copies.<BR><BR>"
				bError = true
			else
				QueryString = "Barcodes=" & barcodes & "&BarcodeType=plate"
				httpResponse = CShttpRequest2("POST", ServerName, "/cheminv/api/CheckDuplicateBarcode.asp", "ChemInv", QueryString)
				duplicates = httpResponse			
				'Response.Write duplicates &"<BR>"
				'Response.End
				'Response.Write barcodes & "<BR>"
				if len(duplicates) <> 0 then 
					if instr(duplicates,",") > 0 then
						sError = sError & "Row " & rowNum & " could not be processed because " & duplicates & "  are not unique barcodes.<BR>"
					else
						sError = sError & "Row " & rowNum & " could not be processed because " & duplicates & "  is not a unique barcode.<BR><BR>"
					end if
					bError = true
				end if
			end if
					
			SQL = "SELECT enum_id FROM " & Application("CHEMINV_USERNAME") & ".inv_enumeration WHERE eset_id_fk = 5 AND lower(enum_value) = ?" 
			Cmd.CommandText = SQL
			Cmd.Parameters.Append Cmd.CreateParameter(, 200, 1, 200, lcase(library))
			Set RS = Cmd.Execute
			if RS.BOF or RS.EOF then
				sError = sError & "Row " & rowNum & " library not updated because the library is not valid.<BR><BR>"
				bLibError = true	
			else
				libraryID = CLng(RS("enum_id"))
			end if
			Cmd.Parameters.Delete 0

			SQL = "SELECT location_id FROM " & Application("CHEMINV_USERNAME") & ".inv_locations WHERE lower(location_name) = ?" 
			Cmd.CommandText = SQL
			Cmd.Parameters.Append Cmd.CreateParameter(, 200, 1, 200, lcase(location))
			Set RS = Cmd.Execute
			if RS.BOF or RS.EOF then
				sError = sError & "Row " & rowNum & " could not be processed because the location is not valid.<BR><BR>"
				bError = true	
			else
				locationID = CLng(RS("location_id"))
				if rowNum = 1 then firstLocationID = locationID
			end if
			Cmd.Parameters.Delete 0
			
			SQL = "SELECT enum_id FROM " & Application("CHEMINV_USERNAME") & ".inv_enumeration WHERE eset_id_fk = 2 AND lower(enum_value) = ?" 
			Cmd.CommandText = SQL
			Cmd.Parameters.Append Cmd.CreateParameter(, 200, 1, 200, lcase(status))
			Set RS = Cmd.Execute
			if RS.BOF or RS.EOF then
				sError = sError & "Row " & rowNum & " status set to unknown because the status is not valid.<BR><BR>"
				bStatusError = true	
			else
				statusID = CLng(RS("enum_id"))
			end if
			Cmd.Parameters.Delete 0

			if not bError then
				if bDaughter then 
					plateIDList = plateID1
				else
					plateIDList = plateID1 & "," & plateID2 & "," & plateID3 & "," & plateID4
				end if
				QueryString = "SourcePlateIDList=" & plateIDList
				QueryString = QueryString & "&xmldoc_id=" & xmldocID 
				QueryString = QueryString & "&BarcodeList=" & barcodes
				QueryString = QueryString & "&PlateTypeID=" & "5"
				QueryString = QueryString & "&Amt=" & "0"
				QueryString = QueryString & "&AmtUnitID=" & "4"
				QueryString = QueryString & "&SolventID=" & "-1"
				QueryString = QueryString & "&SolventVolume=" & ""
				QueryString = QueryString & "&SolventVolumeUnitID=" & "4"
				QueryString = QueryString & "&LocationID=" & locationID
				QueryString = QueryString & "&NumTargetPlates=" & numCopies
				QueryString = QueryString & Credentials
				'Response.Write QueryString & "<BR>"
				'Response.End
				'httpResponse = CShttpRequest2("POST", ServerName, "/cheminv/api/ReformatPlate.asp", "ChemInv", QueryString)
				httpResponse = CShttpRequest3("POST", ServerName, "/cheminv/api/ReformatPlate.asp", "ChemInv", QueryString, 1*60*1000, 1*60*1000)
				out = trim(httpResponse)
				newPlateIDs = newPlateIDs & out & "|"
				'Response.Write out
				'Response.End
				
				if not bLibError or not bStatusError then
					ValuePairs = ""
					if not bLibError then ValuePairs = "library_id_fk%3D" & libraryID
					if not bStatusError and len(ValuePairs)>0 then 
						ValuePairs = ValuePairs & "::status_id_fk%3D" & statusID
					else
						ValuePairs = ValuePairs & "status_id_fk%3D" & statusID
					end if
					QueryString = "PlateIDs=" & out
					QueryString = QueryString & "&ValuePairs=" & ValuePairs
					QueryString = QueryString & Credentials
					'Response.Write QueryString & "<BR>"
					'Response.End

					'Update the plates
					httpResponse = CShttpRequest2("POST", ServerName, "/cheminv/api/UpdatePlate.asp", "ChemInv", QueryString)
					out2 = httpResponse			
					if out2 <> "0" then
						sError = sError & "Row " & rowNum & " plate creation had the following error: " & out2 & "<BR><BR>"
						bError = true
					end if
					'Response.Write out2
					'Response.End
				end if
			end if
			
			if bError or bLibError then errCount = errCount + 1		
			'Response.Write sError
			'Response.Write sourceBarcodeList & ":" & reformatmap & ":" & barcodePrefix & ":" & barcodeSuffix & ":" & numCopies & ":" & library & ":" & location & "<BR>"
			'Response.Write plateID1 & ":" & plateID2 & ":" & plateID3 & ":"  & plateID4 & ":" & xmldocID & ":" & libraryID & ":" & locationID & "<BR>"
		next
	end if
	
	if len(newPlateIDs) = 0 then
		newPlateIDs = 0
	else
		if instr(newPlateIDs,"|") then newPlateIDs = left(newPlateIDs,(len(newPlateIDs) -1 ))
	end if
    Set oDoc = nothing
    ' Set to successful
    ExportToXML = True
Else
    ' NO records found advise user and return false
    sError = "No Data to Save."
    ExportToXML = False
End If

' Close the Recordset and connection
rsData.Close
cnExcel.Close
' Release objects
Set rsData = Nothing
Set cnExcel = Nothing

'Response.End
arrNewPlateIDs = split(newPlateIDs,"|")
%>
<script language="javascript">document.all.processingImage.style.display = 'none';</script>
<table align=center border=0 cellpadding=0 cellspacing=0 bgcolor=#ffffff width=90%>
	<tr>
		<td height=50 valign=middle align="left">
<%
			If isNumeric(arrNewPlateIDs(0)) then
				if Clng(arrNewPlateIDs(0)) >= 0 and bError = false then
					theAction = "Exit"					
				Else
					theAction = "WriteAPIError"
				End if
			Else
				theAction = "WriteOtherError"
			End if
			'Response.Write("<br>action:"&theAction)
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
					Response.Write("<p><code>ChemInv API Error: ")
					if sError <> "" then
						Response.Write(sError & "<br>")
					end if
					Response.Write(Application(out) & "</code></p>")
					Response.Write "<P><center><a HREF=""3"" onclick=""history.back(); return false;""><img SRC=""../graphics/ok_dialog_btn.gif"" border=""0""></a></center>"
				Case "WriteOtherError"
					Response.Write "<P><CODE>Oracle Error: " & out & "</code></p>" 
					Response.Write "<P><center><a HREF=""3"" onclick=""history.back(); return false;""><img SRC=""../graphics/ok_dialog_btn.gif"" border=""0""></a></center>"
			End Select
%>
		</TD>
	</TR>
</TABLE>
</Body>			
