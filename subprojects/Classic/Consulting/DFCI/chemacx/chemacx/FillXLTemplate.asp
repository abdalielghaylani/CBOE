<!--#INCLUDE FILE="Export2Excel_vbs.asp"-->
<!--#INCLUDE FILE="AdoToWddx.js"-->
<!--#INCLUDE VIRTUAL = "/chemacx/api/apiutils.asp"-->
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/ado.inc"-->
<!--#INCLUDE VIRTUAL = "/chemacx/api/apiutils.asp"-->
<%
'SYAN modified on 3/31/2004 to support parameterized SQL
Dim XLTemplate
Dim TargetSheet
Dim ServeMethod
Dim XlsHttpPath
Dim XlsFileName
Dim oHeadDict
Dim WddxPacket
Dim olitemsAdoRS

'Save the Header info to a Cookie and build Dictionary Objects to pass to Excel
' Header dictionary will contain user data from Cookie
	Set oHeadDict = Server.CreateObject("Scripting.Dictionary")
' Lineitem Dictionary will contain the lineitem data from the shopping cart
	Set oLiDict = Server.CreateObject("Scripting.Dictionary")
For Each objItem In Request.Form
	'Save form data to cookie (Do not save form variables with an uderscore prefix)
	If NOT (Left(objItem,1)= "_") then
		Response.Cookies("XLHeader")(objItem) = Request.Form(objItem)
		oHeadDict.Add objItem, Request.Form(objItem)
	end if
Next
Response.Cookies("XLHeader").Expires = DateAdd("yyyy",10,Date)

'Deserialize the line items received in the wddxpacket form field into ADO RS

WddxPacket = Request.Form("_wddxitems")
'Response.Write WddxPacket
'Response.end 
Set olitemsAdoRS = DeserializeToAdoRS(WddxPacket)

'get connection string from application variable
'connection_array = Application( "base_connection" & "chemacx")
'ConnStr = connection_array(0) & "="  & connection_array(1)

Set MyAdoConn = Server.CreateObject("ADODB.Connection")
MyAdoConn.Open GetACXConStr()
	
'Open the template with Excel
	XLTemplate = Application("AppPath") & "\chemacx\ExcelTemplates\" & Application("ExcelTemplate")
	Set XLApp = OpenXLTemplate(XLTemplate)
'Create MasterSheet
	Set MasterSheet = CreateXLSheet(XLApp,"MasterSheet","",True)
'Populate MasterSheet with with Header data from the Dictionary
	fontColorIndex = 5
	FillXlSheetFromDict XLApp,MasterSheet,oHeadDict,fontColorIndex

'Build line item field mapping dictionary
	strFieldMap= Request.Form("_lineitems")
	Set LiFieldMap = BuildFieldMapDict(strFieldMap)
	arrLiCells= LiFieldMap.Keys
	arrLiFieldNames= LiFieldMap.Items
	LiInitRow= CInt(Mid(arrLiCells(0),2,Len(arrLiCells(0))))
'Build dictionary to hold line item current row
Set LiCRowDict = Server.CreateObject("Scripting.Dictionary")
'Build Dictionary to hold a single row of line items
Set	LiDict = Server.CreateObject("Scripting.Dictionary")
	
'Loop through lineitems recordset
	olitemsAdoRS.MoveFirst
	lineitemNum = 1
While NOT olitemsAdoRS.EOF	
	'Lookup the orderfromid and orderfromname
	
	'SQLQuery = "SELECT Supplier.SupplierCode AS OrderFromName, Supplier.SupplierID AS OrderFromID FROM Supplier AS Supplier_1 INNER JOIN Supplier ON Supplier_1.Orderfrom = Supplier.SupplierID WHERE Supplier_1.SupplierID=" & olitemsAdoRS("vendorcode")
	'SQLQuery = "SELECT Supplier.SupplierCode AS OrderFromName, Supplier.SupplierID AS OrderFromID FROM Supplier AS Supplier_1 INNER JOIN Supplier ON Supplier_1.Orderfrom = Supplier.SupplierID WHERE Supplier_1.SupplierID=?"
	SQLQuery = "SELECT Supplier.SupplierCode AS OrderFromName, Supplier.SupplierID AS OrderFromID FROM Supplier Supplier_1, Supplier WHERE Supplier_1.Orderfrom = Supplier.SupplierID AND Supplier_1.SupplierID=?"
	SQLQuery_Parameters = "ID" & "|" & adInteger & "|" & adParamInput & "|" & "|" & olitemsAdoRS("vendorcode")
	'set OrderFromRS = MyAdoConn.Execute(SQLQuery)
	Set OrderFromRS = GetRecordSet(SQLQuery, SQLQuery_Parameters)
	
	'Check if a worksheet exists for this vendor
	TargetSheetName = OrderFromRS("orderFromName")
	vendorSheetExists = XlSheetExists(XLApp,TargetSheetName)
	if NOT vendorSheetExists then
		'Create the Worksheet for this vendor
			SourceSheetName = "MasterSheet"
			Set TempSheet = CreateXLSheet(XLApp,TargetSheetName,SourceSheetName,False)
		'Create the Vendor Address RecordSets
			supplierid = OrderFromRS("OrderFromID")
			debugoutput = False
			set VendorMailRS = GetACXSupplierContact(MyAdoConn, supplierID,"MAIL", debugoutput)
			'set VendorEmailRS = GetACXSupplierContact(MyAdoConn, supplierID,"EMAIL", debugoutput)
			'set VendorURLRS = GetACXSupplierContact(MyAdoConn, supplierID,"URL", debugoutput)
			set VendorPhoneRS = GetACXSupplierContact(MyAdoConn, supplierID,"PHONE", debugoutput)
			set VendorFaxRS = GetACXSupplierContact(MyAdoConn, supplierID,"FAX", debugoutput)	
		'Populate Sheet with Vendor contact information
			strFieldMap= Request.Form("_VendorMailRS")
			if len(strFieldMap)>0 then
				fontColorIndex= 5
				Set VendorFieldMap = BuildFieldMapDict(strFieldMap)
				FillXlSheetFromAdoRS XlApp,TempSheet,VendorMailRS,VendorFieldMap,fontColorIndex,1,1	
				Set VendorFieldMap = Nothing
			End if
		'Populate Sheet with Vendor phone information
			strFieldMap= Request.Form("_VendorPhoneRS")
			if len(strFieldMap)>0 then
				fontColorIndex= 5
				Set VendorFieldMap = BuildFieldMapDict(strFieldMap)
				FillXlSheetFromAdoRS XlApp,TempSheet,VendorPhoneRS,VendorFieldMap,fontColorIndex,1,1	
			end if
		'Populate Sheet with Vendor Fax information
			strFieldMap= Request.Form("_VendorFaxRS")
			if len(strFieldMap)>0 then
				fontColorIndex= 5
				Set VendorFieldMap = BuildFieldMapDict(strFieldMap)
				FillXlSheetFromAdoRS XlApp,TempSheet,VendorFaxRS,VendorFieldMap,fontColorIndex,1,1	
			end if
		
			LiCRowDict.Add TargetSheetName,LiInitRow
	Else
			Set TempSheet= GetWorkSheet(XLApp,TargetSheetName) 
			LiCRowDict(TargetSheetName)= LiCRowDict(TargetSheetName) + 1 
	End if
	'Loop the lineitems field mapping dictionary
	For intloop= 0 To Ubound(arrLiCells)
	CellCol= Left(arrLiCells(intloop),1)
	CellRow= LiCRowDict(TargetSheetName)
	TCell= Cstr(CellCol & CellRow) 
	Tvalue=  olitemsAdoRS(arrLiFieldNames(intloop))
	LiDict.Add TCell, Tvalue
	Next
	'Add line items entry to vendor worksheet
	FillXlSheetFromDict XLApp,TempSheet,LiDict,fontColorIndex
	LiDict.RemoveAll
	olitemsAdoRS.MoveNext
Wend
	
	set AdoConn= nothing
	Set MasterSheet = Nothing
	Set TempSheet = Nothing
'Delete the master sheet
	TargetSheetName= "MasterSheet"
	XLDeleteSheet XLApp,TargetSheetName
'Save the Xls file 
	XlsHttpPath= Application("TempFileDirectoryHTTPchemacx") & "Sessiondir/" & Session.sessionid   
	XlsFileNameRoot= "AcxReq"  
	bolAddTimeStamp= False
	ServerPath = SaveXLTemplateAs(XLApp,XlsHttpPath,XlsFileNameRoot,bolAddTimeStamp)
'Quit Excel
	QuitXLTemplate(XLApp)
	Set XLApp = Nothing
'Serve the xls file
	bServeLink = True 'True = get a link to xls file; False = stream the xls file
	ServeFile ServerPath, bServeLink
	

'***************************************************************************************
'* Returns ADO Recordset with contact information for ACX supplier
'* contactType can be:  Address, email, url, phone, fax
Function GetACXSupplierContact(byref AdoConn, byval supplierID,byval contactType, byval debugoutput)
Dim AdoRS
Dim SQLQuery
Dim theCase

	Select Case Ucase(contactType)
		Case "MAIL"
			'SQLQuery = "SELECT Supplier.SupplierID,Supplier.Name AS SupplierFullName, Supplier.ShortName AS SupplierShortName, Supplier.SupplierCode AS SupplierCodeName, SupplierAddr.Addr1, SupplierAddr.Addr2, SupplierAddr.City, SupplierAddr.State, SupplierAddr.Code AS ZipCode, SupplierAddr.Country FROM SupplierAddr INNER JOIN Supplier ON SupplierAddr.SupplierID = Supplier.SupplierID WHERE SupplierAddr.Type Is Null AND SupplierAddr.SupplierID=" & supplierID
			'SQLQuery = "SELECT Supplier.SupplierID,Supplier.Name AS SupplierFullName, Supplier.ShortName AS SupplierShortName, Supplier.SupplierCode AS SupplierCodeName, SupplierAddr.Addr1, SupplierAddr.Addr2, SupplierAddr.City, SupplierAddr.State, SupplierAddr.Code AS ZipCode, SupplierAddr.Country FROM SupplierAddr INNER JOIN Supplier ON SupplierAddr.SupplierID = Supplier.SupplierID WHERE SupplierAddr.Type Is Null AND SupplierAddr.SupplierID=?"
			SQLQuery = "SELECT Supplier.SupplierID,Supplier.Name AS SupplierFullName, Supplier.ShortName AS SupplierShortName, Supplier.SupplierCode AS SupplierCodeName, SupplierAddr.Addr1, SupplierAddr.Addr2, SupplierAddr.City, SupplierAddr.State, SupplierAddr.Code AS ZipCode, SupplierAddr.Country FROM SupplierAddr, Supplier WHERE SupplierAddr.SupplierID = Supplier.SupplierID AND SupplierAddr.Type Is Null AND SupplierAddr.SupplierID=?"
		Case "EMAIL"
			'SQLQuery = "SELECT Supplier.SupplierID,Supplier.Name AS SupplierFullName, Supplier.ShortName AS SupplierShortName, Supplier.SupplierCode AS SupplierCodeName, SupplierAddr.Addr1 AS email FROM SupplierAddr INNER JOIN Supplier ON SupplierAddr.SupplierID = Supplier.SupplierID WHERE SupplierAddr.Type = 'email' AND SupplierAddr.SupplierID=" & supplierID
			'SQLQuery = "SELECT Supplier.SupplierID,Supplier.Name AS SupplierFullName, Supplier.ShortName AS SupplierShortName, Supplier.SupplierCode AS SupplierCodeName, SupplierAddr.Addr1 AS email FROM SupplierAddr INNER JOIN Supplier ON SupplierAddr.SupplierID = Supplier.SupplierID WHERE SupplierAddr.Type = 'email' AND SupplierAddr.SupplierID=?"
			SQLQuery = "SELECT Supplier.SupplierID,Supplier.Name AS SupplierFullName, Supplier.ShortName AS SupplierShortName, Supplier.SupplierCode AS SupplierCodeName, SupplierAddr.Addr1 AS email FROM SupplierAddr, Supplier WHERE SupplierAddr.SupplierID = Supplier.SupplierID AND SupplierAddr.Type = 'email' AND SupplierAddr.SupplierID=?"		
		Case "URL"
			'SQLQuery = "SELECT Supplier.SupplierID,Supplier.Name AS SupplierFullName, Supplier.ShortName AS SupplierShortName, Supplier.SupplierCode AS SupplierCodeName, SupplierAddr.Addr1 AS url FROM SupplierAddr INNER JOIN Supplier ON SupplierAddr.SupplierID = Supplier.SupplierID WHERE SupplierAddr.Type = 'http' AND SupplierAddr.SupplierID=" & supplierID
			'SQLQuery = "SELECT Supplier.SupplierID,Supplier.Name AS SupplierFullName, Supplier.ShortName AS SupplierShortName, Supplier.SupplierCode AS SupplierCodeName, SupplierAddr.Addr1 AS url FROM SupplierAddr INNER JOIN Supplier ON SupplierAddr.SupplierID = Supplier.SupplierID WHERE SupplierAddr.Type = 'http' AND SupplierAddr.SupplierID=?"
			SQLQuery = "SELECT Supplier.SupplierID,Supplier.Name AS SupplierFullName, Supplier.ShortName AS SupplierShortName, Supplier.SupplierCode AS SupplierCodeName, SupplierAddr.Addr1 AS url FROM SupplierAddr, Supplier WHERE SupplierAddr.SupplierID = Supplier.SupplierID AND SupplierAddr.Type = 'http' AND SupplierAddr.SupplierID=?"
		Case "PHONE"
			'SQLQuery = "SELECT Supplier.SupplierID,Supplier.Name AS SupplierFullName, Supplier.ShortName AS SupplierShortName, Supplier.SupplierCode AS SupplierCodeName, SupplierPhoneID.type AS NumberType, SupplierPhoneID.CountryCode, SupplierPhoneID.AreaCode, SupplierPhoneID.PhoneNum, SupplierPhoneID.Location FROM SupplierPhoneID INNER JOIN Supplier ON SupplierPhoneID.SupplierID = Supplier.SupplierID WHERE SupplierPhoneID.Type='Tel' AND SupplierPhoneID.SupplierID=" & supplierID   
			'SQLQuery = "SELECT Supplier.SupplierID,Supplier.Name AS SupplierFullName, Supplier.ShortName AS SupplierShortName, Supplier.SupplierCode AS SupplierCodeName, SupplierPhoneID.type AS NumberType, SupplierPhoneID.CountryCode, SupplierPhoneID.AreaCode, SupplierPhoneID.PhoneNum, SupplierPhoneID.Location FROM SupplierPhoneID INNER JOIN Supplier ON SupplierPhoneID.SupplierID = Supplier.SupplierID WHERE SupplierPhoneID.Type='Tel' AND SupplierPhoneID.SupplierID=?"
			SQLQuery = "SELECT Supplier.SupplierID,Supplier.Name AS SupplierFullName, Supplier.ShortName AS SupplierShortName, Supplier.SupplierCode AS SupplierCodeName, SupplierPhoneID.type AS NumberType, SupplierPhoneID.CountryCode, SupplierPhoneID.AreaCode, SupplierPhoneID.PhoneNum, SupplierPhoneID.Location FROM SupplierPhoneID, Supplier WHERE SupplierPhoneID.SupplierID = Supplier.SupplierID AND SupplierPhoneID.Type='Tel' AND SupplierPhoneID.SupplierID=?"

		Case "FAX"
			'SQLQuery = "SELECT Supplier.SupplierID,Supplier.Name AS SupplierFullName, Supplier.ShortName AS SupplierShortName, Supplier.SupplierCode AS SupplierCodeName, SupplierPhoneID.type AS NumberType, SupplierPhoneID.CountryCode, SupplierPhoneID.AreaCode, SupplierPhoneID.PhoneNum, SupplierPhoneID.Location FROM SupplierPhoneID INNER JOIN Supplier ON SupplierPhoneID.SupplierID = Supplier.SupplierID WHERE SupplierPhoneID.Type='fax' AND SupplierPhoneID.SupplierID=" & supplierID   
			'SQLQuery = "SELECT Supplier.SupplierID,Supplier.Name AS SupplierFullName, Supplier.ShortName AS SupplierShortName, Supplier.SupplierCode AS SupplierCodeName, SupplierPhoneID.type AS NumberType, SupplierPhoneID.CountryCode, SupplierPhoneID.AreaCode, SupplierPhoneID.PhoneNum, SupplierPhoneID.Location FROM SupplierPhoneID INNER JOIN Supplier ON SupplierPhoneID.SupplierID = Supplier.SupplierID WHERE SupplierPhoneID.Type='fax' AND SupplierPhoneID.SupplierID=?"
			SQLQuery = "SELECT Supplier.SupplierID,Supplier.Name AS SupplierFullName, Supplier.ShortName AS SupplierShortName, Supplier.SupplierCode AS SupplierCodeName, SupplierPhoneID.type AS NumberType, SupplierPhoneID.CountryCode, SupplierPhoneID.AreaCode, SupplierPhoneID.PhoneNum, SupplierPhoneID.Location FROM SupplierPhoneID, Supplier WHERE SupplierPhoneID.SupplierID = Supplier.SupplierID AND SupplierPhoneID.Type='fax' AND SupplierPhoneID.SupplierID=?"
	End Select
	SQLQuery_Parameters = "ID" & "|" & adInteger & "|" & adParamInput & "|" & "|" & supplierID
	'Set AdoRS = AdoConn.Execute(SQLQuery)
	Set AdoRS = GetRecordSet(SQLQuery, SQLQuery_Parameters)
	
	if debugoutput then
		While NOT AdoRS.EOF
			For Each fld In AdoRS.Fields
				response.write fld.name & " =" & fld.value &"<BR>"
			Next
		AdoRS.MoveNext
		response.write "<BR>"
		Wend
	End if
Set GetACXSupplierContact = AdoRS
End Function

%>

