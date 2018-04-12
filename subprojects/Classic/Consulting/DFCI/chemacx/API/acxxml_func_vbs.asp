<!--#INCLUDE VIRTUAL = "/cfserverasp/source/ado.inc"-->
<!--#INCLUDE FILE = "apiutils.asp"-->
<%
'SYAN modified to parameterize SQL statements 3/30/2004
'-------------------------------------------------------------------------------
' Purpose: Returns a list of CsNums from a list of either ProductIDs or PackageIDs  
' Inputs: ADO connection, FieldName=(CsNum|CAS|ACXID|ProductID|PackageID), comma delimited list of values
' Returns: comma delimited CsNums as string 
'-------------------------------------------------------------------------------
Function GetCsNumList(byRef AdoConn, byVal sFieldName, byVal sValueList)
	Dim thelist
	thelist = ""
	if sValueList <> "" then
		if Ucase(sFieldName) = "CSNUM" then
		theList = sValueList
		Else
			if UCase(sFieldName) = "PRODUCTID" then
				If Application("BASE_DBMS") = "ORACLE" then
					SQLQuery = "SELECT CsNum FROM Product WHERE ProductID IN(" & BuildInClause(sValueList) & ")"
					SQLQuery_Parameters = "InClause" & "|" & adVarChar & "|" & adParamInput & "|" & "|" & sValueList 'Name|Type|Direction|Size|Value
				Else
					SQLQuery = "SELECT CsNum FROM Product WHERE ProductID IN (" & sValueList & ")"
					SQLQuery_Parameters = ""
				End if
			Elseif UCase(sFieldName) = "PACKAGEID" then
				If Application("BASE_DBMS") = "ORACLE" then
					SQLQuery = "SELECT Product.CSNum FROM Package, Product WHERE Package.ProductID = Product.ProductID AND Package.PackageID IN (" & BuildInClause("'" & Replace(sValueList, ",", "','") & "'") & ")"
					SQLQuery_Parameters = "InClause" & "|" & adVarChar & "|" & adParamInput & "|" & "|" & sValueList 'Name|Type|Direction|Size|Value
				Else
					SQLQuery = "SELECT Product.CSNum FROM Package, Product WHERE Package.ProductID = Product.ProductID AND Package.PackageID IN (" & sValueList & ")"
					SQLQuery_Parameters = ""
				End if
			Elseif UCase(sFieldName) = "CAS" OR UCase(sFieldName) = "ACX_ID" then
				If Application("BASE_DBMS") = "ORACLE" then
					SQLQuery = "SELECT Substance.CSNum FROM Substance WHERE " & sFieldName & " IN (" & BuildInClause(sValueList) & ")"
					SQLQuery_Parameters = "InClause" & "|" & adVarChar & "|" & adParamInput & "|" & "|" & "'" & Replace(sValueList, ",", "','") & "'" 'Name|Type|Direction|Size|Value
				Else
					SQLQuery = "SELECT Substance.CSNum FROM Substance WHERE " & sFieldName & " IN ('" & Replace(sValueList, ",", "','") & "')"
					SQLQuery_Parameters = ""
				End if
			End if
			
			'Set CsNumListRS = AdoConn.Execute(SQLQuery)
			Set CsNumListRS = GetRecordSet(SQLQuery, SQLQuery_Parameters)
			
			if NOT (CsNumListRS.EOF AND CsNumListRS.BOF) then
				While Not CsNumListRS.EOF
					thelist = thelist & CsNumListRS("CsNum") & ","  
					CsNumListRS.MoveNext
				Wend
				thelist = Left(thelist,Len(thelist)-1)
			End if
		End if
	End if
	GetCsNumList = theList
End Function

'-------------------------------------------------------------------------------
' Purpose: Opens an ADO connection to the ChemACX database defined in the COWS chemacx.ini file 
' Inputs:
' Assumes: connection string details stored in Application variable
' Returns: An open ADO connection as object
'-------------------------------------------------------------------------------
Function CreateAdoConnection()
	ConnStr = GetACXConStr() 
	Set AdoConn = Server.CreateObject("ADODB.Connection")
	AdoConn.Open ConnStr
	Set CreateAdoConnection = AdoConn
	Set AdoConn = Nothing
End Function
'-------------------------------------------------------------------------------
' Purpose: Creates a set of  <substance> elements and attaches them below the ACXdata element. 
' Inputs: ADO connection, FieldName=(CsNum|ProductID|PackageID), comma delimited list of values 
' Returns: Nothing
'-------------------------------------------------------------------------------
Sub GetSubstanceRS(byRef AdoConnection, byVal sFieldName, byVal sValueList,sStructType)
	if sValueList <> "" then
		Select Case UCase(sFieldName)
			Case "CSNUM"
				If Application("BASE_DBMS") = "ORACLE" then
					SQLQuery = "SELECT DISTINCT ACX_ID AS ACXnum, CsNum, CAS AS casNum FROM Substance WHERE CsNum IN(" & BuildInClause(sValueList) & ")"
					SQLQuery_Parameters = "InClause" & "|" & adVarChar & "|" & adParamInput & "|" & "|" & sValueList 'Name|Type|Direction|Size|Value
				Else
					SQLQuery = "SELECT DISTINCT ACX_ID AS ACXnum, CsNum, CAS AS casNum FROM Substance WHERE CsNum IN(" & sValueList & ")"
					SQLQuery_Parameters = ""
				End if
			Case "PRODUCTID"
				'SQLQuery = "SELECT DISTINCT Substance.ACX_ID AS acxNum, Substance.CsNum, Substance.CAS AS casNum FROM Product INNER JOIN Substance ON Product.CSNum = Substance.CsNum WHERE Product.ProductID In (" & sValueList & ")"
				If Application("BASE_DBMS") = "ORACLE" then
					SQLQuery = "SELECT Product.CSNum FROM Package, Product WHERE Package.ProductID = Product.ProductID AND Package.PackageID IN (" & BuildInClause(sValueList) & ")"			
					SQLQuery_Parameters = "InClause" & "|" & adVarChar & "|" & adParamInput & "|" & "|" & sValueList 'Name|Type|Direction|Size|Value
				Else
					SQLQuery = "SELECT Product.CSNum FROM Package, Product WHERE Package.ProductID = Product.ProductID AND Package.PackageID IN (" & sValueList & ")"			
					SQLQuery_Parameters = ""
				End if
			Case "PACKAGEID"
				'SQLQuery = "SELECT DISTINCT Substance.ACX_ID AS acxNum, Substance.CsNum, Substance.CAS AS casNum FROM Package INNER JOIN (Product INNER JOIN Substance ON Product.CSNum = Substance.CsNum) ON Package.ProductID = Product.ProductID WHERE (Package.PackageID) In (" & sValueList & ")"
				If Application("BASE_DBMS") = "ORACLE" then
					SQLQuery = "SELECT DISTINCT Substance.ACX_ID AS acxNum, Substance.CsNum, Substance.CAS AS casNum FROM Package, Product, Substance WHERE Product.CSNum = Substance.CsNum AND Package.ProductID = Product.ProductID AND Package.PackageID IN (" & BuildInClause(sValueList) & ")"
					SQLQuery_Parameters = "InClause" & "|" & adVarChar & "|" & adParamInput & "|" & "|" & sValueList 'Name|Type|Direction|Size|Value
				Else
					SQLQuery = "SELECT DISTINCT Substance.ACX_ID AS acxNum, Substance.CsNum, Substance.CAS AS casNum FROM Package, Product, Substance WHERE Product.CSNum = Substance.CsNum AND Package.ProductID = Product.ProductID AND Package.PackageID IN (" & sValueList & ")"
					SQLQuery_Parameters = ""
				End if
		End Select
		'Response.Write SQLQuery
		'Response.end	 
		'Set SubstanceRS = AdoConn.Execute(SQLQuery)
		Set SubstanceRS = GetRecordSet(SQLQuery, SQLQuery_Parameters)
		
		if NOT (SubstanceRS.EOF AND SubstanceRS.BOF) then
			While Not SubstanceRS.EOF
				Set elemSubstance = AddElementToParent(myDOM, elemACXdata, "substance", "", True)
				AddAttributeToElement myDOM, elemSubstance, "acxNum", SubstanceRS("ACXNum")
				AddAttributeToElement myDOM, elemSubstance, "csNum", SubstanceRS("CsNum")
				AddAttributeToElement myDOM, elemSubstance, "casNum", SubstanceRS("CASNum")
				bstructure = 1
			
				if sStructType = "cdx" OR sStructType = "gif" OR sStructType = "base64cdx" OR sStructType = "mol" then
					
					Set elemStructure = AddElementToParent(myDOM,elemSubstance,"structure","", True)
					AddAttributeToElement myDOM, elemStructure, "type", sStructType
					if sStructType = "cdx" OR sStructType = "gif" then	
						sStructURL ="http://" & Request.ServerVariables("HTTP_HOST") & "/ChemACX/chemacx/chemacx_action.asp?dbname=chemacx&dataaction=get_structure&formgroup=base_form_group&Table=substance&Field=structure&DisplayType=" & sStructType & "&StrucID=" & SubstanceRS("CsNum") 
						AddAttributeToElement myDOM, elemStructure, "src", sStructURL
					Elseif sStructType = "base64cdx" OR sStructType = "mol" then
						if sStructType = "base64cdx" then 
							sType = "rawbase64cdx" 
						else
							sType = "mol"
						end if
						credentials = "CSUserName=" & CSuserName & "&CSUserId="& CSUserid
						strucdata = "" 'CSBR-93547 CShttpRequest2("GET", Request.ServerVariables("HTTP_HOST"), "/ChemACX/chemacx/chemacx_action.asp?dbname=chemacx&dataaction=get_structure&formgroup=base_form_group&Table=substance&Field=structure&DisplayType=" & sType & "&StrucID=" & SubstanceRS("CsNum"), "ChemACX", credentials)
						'Response.Clear
						'Response.ContentType = "text/html"
						'Response.Write Request.ServerVariables("HTTP_HOST")
						'Response.end
						Set elemstrucdata = AddElementToParent(myDOM, elemStructure, "strucdata", strucdata, False)
					End if
				End if
				SubstanceRS.MoveNext
			Wend
		Set elemSubstance = Nothing
		Set elemStructure = Nothing
		Set elemBase64cdx = Nothing
		end if
		Set SubstanceRS = Nothing
	End if
End Sub

'-------------------------------------------------------------------------------
' Purpose: Creates a set of  <product> elements and attaches them to the appropriate <substance> element based on csNum 
' Inputs: an open ADO connection to the ChemACX database
' Returns: Nothing
'-------------------------------------------------------------------------------
Sub GetProductRS(byRef AdoConnection, byVal sFieldName, byVal sValueList)
	if sValueList <> "" then
		Select Case UCase(sFieldName)
			Case "CSNUM"
				If Application("BASE_DBMS") = "ORACLE" then
					SQLQuery = "SELECT CsNum, ProductID AS ACXprodID, CatalogNum AS catNum,supplierID, prodName, ProdDescrip AS prodDescription FROM Product WHERE CsNum IN(" & BuildInClause(sValueList) & ")"
					SQLQuery_Parameters = "InClause" & "|" & adVarChar & "|" & adParamInput & "|" & "|" & sValueList 'Name|Type|Direction|Size|Value
				Else
					SQLQuery = "SELECT CsNum, ProductID AS ACXprodID, CatalogNum AS catNum,supplierID, prodName, ProdDescrip AS prodDescription FROM Product WHERE CsNum IN(" & sValueList & ")"
					SQLQuery_Parameters = ""
				End if
			Case "PRODUCTID"
				If Application("BASE_DBMS") = "ORACLE" then
					SQLQuery = "SELECT CsNum, ProductID AS ACXprodID, CatalogNum AS catNum,supplierID, prodName, ProdDescrip AS prodDescription FROM Product WHERE ProductID IN(" & BuildInClause(sValueList) & ")"
					SQLQuery_Parameters = "InClause" & "|" & adVarChar & "|" & adParamInput & "|" & "|" & sValueList 'Name|Type|Direction|Size|Value
				Else
					SQLQuery = "SELECT CsNum, ProductID AS ACXprodID, CatalogNum AS catNum,supplierID, prodName, ProdDescrip AS prodDescription FROM Product WHERE ProductID IN(" & sValueList & ")"
					SQLQuery_Parameters = ""
				End if
			Case "PACKAGEID"
				'SQLQuery = "SELECT DISTINCT Product.CSNum, Product.ProductID AS ACXprodID, Product.CatalogNum AS catNum, Product.SupplierID, Product.ProdName, Product.ProdDescrip AS prodDescription FROM Product INNER JOIN Package ON Product.ProductID = Package.ProductID WHERE Package.PackageID In (" & sValueList & ")"
				If Application("BASE_DBMS") = "ORACLE" then
					SQLQuery = "SELECT DISTINCT Product.CSNum, Product.ProductID AS ACXprodID, Product.CatalogNum AS catNum, Product.SupplierID, Product.ProdName, Product.ProdDescrip AS prodDescription FROM Product, Package WHERE Product.ProductID = Package.ProductID AND Package.PackageID IN (" & BuildInClause(sValueList) & ")"
					SQLQuery_Parameters = "InClause" & "|" & adVarChar & "|" & adParamInput & "|" & "|" & sValueList 'Name|Type|Direction|Size|Value
				Else
					SQLQuery = "SELECT DISTINCT Product.CSNum, Product.ProductID AS ACXprodID, Product.CatalogNum AS catNum, Product.SupplierID, Product.ProdName, Product.ProdDescrip AS prodDescription FROM Product, Package WHERE Product.ProductID = Package.ProductID AND Package.PackageID IN (" & sValueList & ")"
					SQLQuery_Parameters = ""
				End if
		End Select	 
		'Set productRS = AdoConn.Execute(SQLQuery)
		Set productRS = GetRecordSet(SQLQuery, SQLQuery_Parameters)
		
		if NOT (productRS.EOF AND productRS.BOF) then
			While Not productRS.EOF
				Set elemTempSubstance = GetElementbyAttrValue(myDOM,"substance","csNum",productRS("CsNum"))
				Set elemProduct = AddElementToParent(myDOM,elemTempSubstance,"product","", True)
				AddAttributeToElement myDOM, elemProduct, "ACXprodID", productRS("ACXprodID")
				AddAttributeToElement myDOM, elemProduct, "supplierID",productRS("supplierID")
	 			AddAttributeToElement myDOM, elemProduct, "catNum", productRS("catNum")
				Set elemProdName = AddElementToParent(myDOM,elemProduct,"prodName",productRS("prodName"), False)
				Set elemProdDescription = AddElementToParent(myDOM,elemProduct,"prodDescription",productRS("prodDescription"), False)
				productRS.MoveNext
			Wend
		Set elemTempSubstance = Nothing
		Set elemProduct = Nothing
		Set elemProdName = Nothing
		Set elemProdDescription = Nothing
		end if
		Set productRS = Nothing
	End if
End Sub
'-------------------------------------------------------------------------------
' Purpose: Creates a set of  <package> elements and attaches them to the appropriate <product> element based on ACXprodID 
' Inputs: an open ADO connection to the ChemACX database
' Returns: Nothing
'-------------------------------------------------------------------------------
Sub GetPackageRS(byRef AdoConnection, byVal sFieldName, byVal sValueList)
	if sValueList <> "" then
		Select Case UCase(sFieldName)
			Case "CSNUM"
				if DBMS = "ORACLE" then
					SQLQuery = "SELECT Package.PackageID AS ACXpackID, Package.""SIZE"" AS packSize, Package.PRICE AS packPrice, Package.CSYMBOL AS currencySymbol, Package.ProductID AS ACXprodID, PackageSizeConversion.Container_UOM_ID_FK, PackageSizeConversion.Container_UOM, PackageSizeConversion.Container_Count, PackageSizeConversion.Container_Qty_Max FROM Package, chemacxdb.PackageSizeConversion, Product WHERE PackageSizeConversion.SIZE_FK(+) = Package.""SIZE""  AND Product.ProductID = Package.ProductID AND Product.CSNum IN (" & BuildInClause(sValueList) & ")"
					SQLQuery_Parameters = "InClause" & "|" & adVarChar & "|" & adParamInput & "|" & "|" & sValueList 'Name|Type|Direction|Size|Value
				else
					SQLQuery = "SELECT Package.PackageID AS ACXpackID, Package.[Size] AS packSize, Package.PRICE AS packPrice, Package.CSYMBOL AS currencySymbol, Package.ProductID AS ACXprodID, PackageSizeConversion.Container_UOM_ID_FK, PackageSizeConversion.Container_UOM, PackageSizeConversion.Container_Count, PackageSizeConversion.Container_Qty_Max FROM (Package LEFT JOIN PackageSizeConversion ON PackageSizeConversion.SIZE_FK = Package.[Size]) INNER JOIN Product ON Product.ProductID = Package.ProductID WHERE Product.CSNum IN (" & sValueList & ")"
					SQLQuery_Parameters = ""
				end if
			Case "PRODUCTID"
				if DBMS = "ORACLE" then
					SQLQuery = "SELECT Package.PackageID AS ACXpackID, Package.""SIZE"" AS packSize, Package.PRICE AS packPrice, Package.CSYMBOL AS currencySymbol, Package.ProductID AS ACXprodID, PackageSizeConversion.Container_UOM_ID_FK, PackageSizeConversion.Container_UOM, PackageSizeConversion.Container_Count, PackageSizeConversion.Container_Qty_Max FROM Package, chemacxdb.PackageSizeConversion, Product WHERE PackageSizeConversion.SIZE_FK(+) = Package.""SIZE"" AND Product.ProductID = Package.ProductID AND Product.ProductID IN (" & BuildInClause(sValueList) & ")"				
					SQLQuery_Parameters = "InClause" & "|" & adVarChar & "|" & adParamInput & "|" & "|" & sValueList 'Name|Type|Direction|Size|Value
				else
					SQLQuery = "SELECT Package.PackageID AS ACXpackID, Package.[Size] AS packSize, Package.PRICE AS packPrice, Package.CSYMBOL AS currencySymbol, Package.ProductID AS ACXprodID, PackageSizeConversion.Container_UOM_ID_FK, PackageSizeConversion.Container_UOM, PackageSizeConversion.Container_Count, PackageSizeConversion.Container_Qty_Max FROM (Package LEFT JOIN PackageSizeConversion ON PackageSizeConversion.SIZE_FK = Package.[Size]) INNER JOIN Product ON Product.ProductID = Package.ProductID WHERE Product.ProductID IN (" & sValueList & ")"
					SQLQuery_Parameters = ""
				end if
			Case "PACKAGEID"
				if DBMS = "ORACLE" then
					SQLQuery = "SELECT Package.PackageID AS ACXpackID, Package.""SIZE"" AS packSize, Package.PRICE AS packPrice, Package.CSYMBOL AS currencySymbol, Package.ProductID AS ACXprodID, PackageSizeConversion.Container_UOM_ID_FK, PackageSizeConversion.Container_UOM, PackageSizeConversion.Container_Count, PackageSizeConversion.Container_Qty_Max FROM Package, chemacxdb.PackageSizeConversion WHERE PackageSizeConversion.SIZE_FK(+) = Package.""SIZE"" AND PackageID IN (" & BuildInClause(sValueList) & ")"				
					SQLQuery_Parameters = "InClause" & "|" & adVarChar & "|" & adParamInput & "|" & "|" & sValueList 'Name|Type|Direction|Size|Value
				else
					SQLQuery = "SELECT Package.PackageID AS ACXpackID, Package.[Size] AS packSize, Package.PRICE AS packPrice, Package.CSYMBOL AS currencySymbol, Package.ProductID AS ACXprodID, PackageSizeConversion.Container_UOM_ID_FK, PackageSizeConversion.Container_UOM, PackageSizeConversion.Container_Count, PackageSizeConversion.Container_Qty_Max FROM Package LEFT JOIN PackageSizeConversion ON PackageSizeConversion.SIZE_FK = Package.[Size] WHERE PackageID IN (" & sValueList & ")"
					SQLQuery_Parameters = ""
				end if
		End Select	 
	
		'Set packageRS = AdoConn.Execute(SQLQuery)
		Set packageRS = GetRecordSet(SQLQuery, SQLQuery_Parameters)

		if NOT (packageRS.EOF AND packageRS.BOF) then
			While Not packageRS.EOF
				Set elemTempProduct = GetElementbyAttrValue(myDOM,"product","ACXprodID",packageRS("ACXprodID"))
				Set elemPackage = AddElementToParent(myDOM,elemTempProduct,"package","", True)
				AddAttributeToElement myDOM, elemPackage, "ACXpackID", packageRS("ACXpackID")
				AddAttributeToElement myDOM, elemPackage, "ContainerSize", packageRS("Container_Qty_Max")
				AddAttributeToElement myDOM, elemPackage, "ContainerUOM", packageRS("Container_UOM")
				AddAttributeToElement myDOM, elemPackage, "ContainerUOMID", packageRS("Container_UOM_ID_FK")
				AddAttributeToElement myDOM, elemPackage, "ContainerCount", packageRS("Container_Count")
				Set elemPackSize = AddElementToParent(myDOM,elemPackage,"packSize",packageRS("packSize"), False)
				'MCD: added this bit of code to insert a '0' cost instead of an alpha string (e.g., "NA")
				tmpPrice = 0
				On Error Resume Next						'I know, this is horrid style
				tmpPrice = CDbl(packageRS("packPrice"))
				On Error Goto 0								'sue me.
				Set elemPackPrice = AddElementToParent(myDOM,elemPackage,"packPrice", tmpPrice, False)
				'Set elemPackPrice = AddElementToParent(myDOM,elemPackage,"packPrice", packageRS("packPrice"), False)	-- Replaced line
				'MCD: end changes
				Set elemCurrencySymbol = AddElementToParent(myDOM,elemPackage,"currencySymbol",packageRS("currencySymbol"), False)
				packageRS.MoveNext
			Wend
		Set elemTempProduct = Nothing
		Set elemPackage = Nothing
		Set elemPackSize = Nothing
		Set elemPackPrice = Nothing
		Set elemCurrencySymbol = Nothing
		end if
		Set packageRS = Nothing
	End if
End Sub
'-------------------------------------------------------------------------------
' Purpose: Creates a set of  <supplier> elements and attaches them below the <ACXdata> element 
' Inputs: an open ADO connection to the ChemACX database
' Returns: Nothing
'-------------------------------------------------------------------------------
Sub GetSupplierRS(byRef AdoConnection, byVal sFieldName, byVal sValueList)
	if sValueList <> "" then
		Select Case UCase(sFieldName)
			Case "CSNUM"
				If Application("BASE_DBMS") = "ORACLE" then
					SQLQuery = "SELECT DISTINCT Supplier.SupplierID AS supplierID, Supplier.Name AS supplierName, Supplier.ShortName AS supplierShortName, Supplier.SupplierCode AS supplierCode, Supplier.supplierType FROM Product, Supplier WHERE Product.SupplierID = Supplier.SupplierID AND Product.CSNum IN (" & BuildInClause(sValueList) & ")"
					SQLQuery_Parameters = "InClause" & "|" & adVarChar & "|" & adParamInput & "|" & "|" & sValueList 'Name|Type|Direction|Size|Value
				Else
					SQLQuery = "SELECT DISTINCT Supplier.SupplierID AS supplierID, Supplier.Name AS supplierName, Supplier.ShortName AS supplierShortName, Supplier.SupplierCode AS supplierCode, Supplier.supplierType FROM Product, Supplier WHERE Product.SupplierID = Supplier.SupplierID AND Product.CSNum IN (" & sValueList & ")"
					SQLQuery_Parameters = ""
				End if
			Case "PRODUCTID"
				If Application("BASE_DBMS") = "ORACLE" then
					SQLQuery = "SELECT DISTINCT Supplier.SupplierID AS supplierID, Supplier.Name AS supplierName, Supplier.ShortName AS supplierShortName, Supplier.SupplierCode AS supplierCode, Supplier.supplierType FROM Product, Supplier WHERE Product.SupplierID = Supplier.SupplierID AND Product.ProductID IN (" & BuildInClause(sValueList) & ")"
					SQLQuery_Parameters = "InClause" & "|" & adVarChar & "|" & adParamInput & "|" & "|" & sValueList 'Name|Type|Direction|Size|Value
				Else
					SQLQuery = "SELECT DISTINCT Supplier.SupplierID AS supplierID, Supplier.Name AS supplierName, Supplier.ShortName AS supplierShortName, Supplier.SupplierCode AS supplierCode, Supplier.supplierType FROM Product, Supplier WHERE Product.SupplierID = Supplier.SupplierID AND Product.ProductID IN (" & sValueList & ")"
					SQLQuery_Parameters = ""
				End if
			Case "PACKAGEID"
				If Application("BASE_DBMS") = "ORACLE" then
					SQLQuery = "SELECT DISTINCT Supplier.SupplierID AS supplierID, Supplier.Name AS supplierName, Supplier.ShortName AS supplierShortName, Supplier.SupplierCode AS supplierCode, Supplier.supplierType FROM Package, Supplier WHERE Package.SupplierID = Supplier.SupplierID AND Package.PackageID IN (" & BuildInClause(sValueList) & ")"
					SQLQuery_Parameters = "InClause" & "|" & adVarChar & "|" & adParamInput & "|" & "|" & sValueList 'Name|Type|Direction|Size|Value
				Else
					SQLQuery = "SELECT DISTINCT Supplier.SupplierID AS supplierID, Supplier.Name AS supplierName, Supplier.ShortName AS supplierShortName, Supplier.SupplierCode AS supplierCode, Supplier.supplierType FROM Package, Supplier WHERE Package.SupplierID = Supplier.SupplierID AND Package.PackageID IN (" & sValueList & ")"
					SQLQuery_Parameters = ""
				End if
			' blank means no restriction.  Returns all suppliers
			Case ""
				SQLQuery = "SELECT DISTINCT Supplier.SupplierID AS supplierID, Supplier.Name AS supplierName, Supplier.ShortName AS supplierShortName, Supplier.SupplierCode AS supplierCode, Supplier.supplierType FROM Supplier"
				SQLQuery_Parameters = "InClause" & "|" & adVarChar & "|" & adParamInput & "|" & "|" & sValueList 'Name|Type|Direction|Size|Value
		End Select	 
		
		'Set supplierRS = AdoConn.Execute(SQLQuery)
		Set supplierRS = GetRecordSet(SQLQuery, SQLQuery_Parameters)
		
		if NOT (supplierRS.EOF AND supplierRS.BOF) then
			While Not supplierRS.EOF
				Set elemSupplier = AddElementToParent(myDOM, elemACXdata,"supplier","", True)
				AddAttributeToElement myDOM, elemSupplier, "supplierID", supplierRS("supplierID")
				AddAttributeToElement myDOM, elemSupplier, "supplierCode", supplierRS("supplierCode")
				AddAttributeToElement myDOM, elemSupplier, "supplierType", supplierRS("supplierType")
				Set elemSupplierName = AddElementToParent(myDOM,elemSupplier,"supplierName",supplierRS("supplierName"), False)
				Set elemSupplierShortName = AddElementToParent(myDOM,elemSupplier,"supplierShortName",supplierRS("supplierShortName"), False)
				supplierRS.MoveNext
			Wend
		Set elemSupplier = Nothing
		Set elemSupplierName = Nothing
		Set elemSupplierShortName = Nothing
		end if
		Set supplierRS = Nothing
	End if
End Sub
'-------------------------------------------------------------------------------
' Purpose: Creates a set of <supplierAddress> elements and attaches them to the appropriate <supplier> element based on supplierID 
' Inputs: an open ADO connection to the ChemACX database
' Returns: Nothing
'-------------------------------------------------------------------------------
Sub GetSupplierAddressRS(byRef AdoConnection, byVal sFieldName, byVal sValueList)
	if sValueList <> "" then
		SQLselect = "SELECT DISTINCT SupplierAddr.supplierID, SupplierAddr.Addr1 AS address1, SupplierAddr.Addr2 AS address2, SupplierAddr.City, SupplierAddr.State, SupplierAddr.Code AS zipCode, SupplierAddr.Country "
		Select Case UCase(sFieldName)
			Case "CSNUM"
				If Application("BASE_DBMS") = "ORACLE" then
					SQLQuery = SQLselect & "FROM Product, SupplierAddr WHERE Product.SupplierID = SupplierAddr.SupplierID AND SupplierAddr.Type Is Null AND Product.CSNum IN (" & BuildInClause(sValueList) & ")"
					SQLQuery_Parameters = "InClause" & "|" & adVarChar & "|" & adParamInput & "|" & "|" & sValueList 'Name|Type|Direction|Size|Value
				Else
					SQLQuery = SQLselect & "FROM Product, SupplierAddr WHERE Product.SupplierID = SupplierAddr.SupplierID AND SupplierAddr.Type Is Null AND Product.CSNum IN (" & sValueList & ")"
					SQLQuery_Parameters = ""
				End if
			Case "PRODUCTID"
				If Application("BASE_DBMS") = "ORACLE" then
					SQLQuery = SQLselect & "FROM Product, SupplierAddr WHERE Product.SupplierID = SupplierAddr.SupplierID AND SupplierAddr.Type Is Null AND Product.ProductID IN (" & BuildInClause(sValueList) & ")"
					SQLQuery_Parameters = "InClause" & "|" & adVarChar & "|" & adParamInput & "|" & "|" & sValueList 'Name|Type|Direction|Size|Value
				Else
					SQLQuery = SQLselect & "FROM Product, SupplierAddr WHERE Product.SupplierID = SupplierAddr.SupplierID AND SupplierAddr.Type Is Null AND Product.ProductID IN (" & sValueList & ")"
					SQLQuery_Parameters = ""
				End if
			Case "PACKAGEID"
				If Application("BASE_DBMS") = "ORACLE" then
					SQLQuery = SQLselect & "FROM Package, SupplierAddr WHERE Package.SupplierID = SupplierAddr.SupplierID AND SupplierAddr.Type Is Null AND Package.PackageID IN (" & BuildInClause(sValueList) & ")"
					SQLQuery_Parameters = "InClause" & "|" & adVarChar & "|" & adParamInput & "|" & "|" & sValueList 'Name|Type|Direction|Size|Value
				Else
					SQLQuery = SQLselect & "FROM Package, SupplierAddr WHERE Package.SupplierID = SupplierAddr.SupplierID AND SupplierAddr.Type Is Null AND Package.PackageID IN (" & sValueList & ")"
					SQLQuery_Parameters = ""
				End if
		End Select	
	
		'Set supplierAddressRS = AdoConn.Execute(SQLQuery)
		Set supplierAddressRS = GetRecordSet(SQLQuery, SQLQuery_Parameters)

		if NOT (supplierAddressRS.EOF AND supplierAddressRS.BOF) then
			While Not supplierAddressRS.EOF
				Set elemTempSupplier = GetElementbyAttrValue(myDOM,"supplier","supplierID",supplierAddressRS("supplierID"))
				Set elemSupplierAddress = AddElementToParent(myDOM, elemTempSupplier,"supplierAddress","", True)
				Set elemAddress1 = AddElementToParent(myDOM,elemSupplierAddress,"address1",supplierAddressRS("address1"), False)
				Set elemAddress2 = AddElementToParent(myDOM,elemSupplierAddress,"address2",supplierAddressRS("address2"), False)
				Set city = AddElementToParent(myDOM,elemSupplierAddress,"city",supplierAddressRS("city"), False)
				Set state = AddElementToParent(myDOM,elemSupplierAddress,"state",supplierAddressRS("state"), False)	
				Set zipCode = AddElementToParent(myDOM,elemSupplierAddress,"zipCode",supplierAddressRS("zipCode"), False)
				Set country = AddElementToParent(myDOM,elemSupplierAddress,"country",supplierAddressRS("country"), False)
				supplierAddressRS.MoveNext
			Wend
		Set elemTempSupplier = Nothing
		Set elemSupplierAddress = Nothing
		Set elemAddress1 = Nothing
		Set elemAddress2 = Nothing
		Set city = Nothing
		Set state = Nothing
		Set zipCode = Nothing
		Set country = Nothing
		end if
		Set supplierAddressRS = Nothing
	End if
End Sub
'-------------------------------------------------------------------------------
' Purpose: Creates a set of <supplierEmail> elements and attaches then to the appropriate <supplier> element based on supplierID 
' Inputs: an open ADO connection to the ChemACX database
' Returns: Nothing
'-------------------------------------------------------------------------------
Sub GetSupplierEmailRS(byRef AdoConnection, byVal sFieldName, byVal sValueList)
	if sValueList <> "" then
		SQLselect = "SELECT DISTINCT SupplierAddr.supplierID, SupplierAddr.Addr1 AS supplierEmail "
		Select Case UCase(sFieldName)
			Case "CSNUM"
				If Application("BASE_DBMS") = "ORACLE" then
					SQLQuery = SQLselect & "FROM Product, SupplierAddr WHERE Product.SupplierID = SupplierAddr.SupplierID AND SupplierAddr.Type = 'email' AND Product.CSNum In (" & BuildInClause(sValueList) & ")"
					SQLQuery_Parameters = "InClause" & "|" & adVarChar & "|" & adParamInput & "|" & "|" & sValueList 'Name|Type|Direction|Size|Value
				Else
					SQLQuery = SQLselect & "FROM Product, SupplierAddr WHERE Product.SupplierID = SupplierAddr.SupplierID AND SupplierAddr.Type = 'email' AND Product.CSNum In (" & sValueList & ")"
					SQLQuery_Parameters = ""
				End if
			Case "PRODUCTID"
				If Application("BASE_DBMS") = "ORACLE" then
					SQLQuery = SQLselect & "FROM Product, SupplierAddr WHERE Product.SupplierID = SupplierAddr.SupplierID AND SupplierAddr.Type = 'email' AND Product.ProductID In (" & BuildInClause(sValueList) & ")"
					SQLQuery_Parameters = "InClause" & "|" & adVarChar & "|" & adParamInput & "|" & "|" & sValueList 'Name|Type|Direction|Size|Value
				Else
					SQLQuery = SQLselect & "FROM Product, SupplierAddr WHERE Product.SupplierID = SupplierAddr.SupplierID AND SupplierAddr.Type = 'email' AND Product.ProductID In (" & sValueList & ")"
					SQLQuery_Parameters = ""
				End if
			Case "PACKAGEID"
				If Application("BASE_DBMS") = "ORACLE" then
					SQLQuery = SQLselect & "FROM Package, SupplierAddr WHERE Package.SupplierID = SupplierAddr.SupplierID AND SupplierAddr.Type = 'email' AND Package.PackageID In (" & BuildInClause(sValueList) & ")"
					SQLQuery_Parameters = "InClause" & "|" & adVarChar & "|" & adParamInput & "|" & "|" & sValueList 'Name|Type|Direction|Size|Value
				Else
					SQLQuery = SQLselect & "FROM Package, SupplierAddr WHERE Package.SupplierID = SupplierAddr.SupplierID AND SupplierAddr.Type = 'email' AND Package.PackageID In (" & sValueList & ")"
					SQLQuery_Parameters = ""
				End if
		End Select	
	
		'Set supplierEmailRS = AdoConn.Execute(SQLQuery)
		Set supplierEmailRS = GetRecordSet(SQLQuery, SQLQuery_Parameters)
		
		if NOT (supplierEmailRS.EOF AND supplierEmailRS.BOF) then
			While Not supplierEmailRS.EOF
				Set elemTempSupplier = GetElementbyAttrValue(myDOM,"supplier","supplierID",supplierEmailRS("supplierID"))
				Set elemSupplierEmail = AddElementToParent(myDOM, elemTempSupplier,"supplierEmail",supplierEmailRS("supplierEmail"), False)
				supplierEmailRS.MoveNext
			Wend
		Set elemTempSupplier = Nothing
		Set elemSupplierEmail = Nothing
		end if
		Set supplierEmailRS = Nothing
	End if
End Sub
'-------------------------------------------------------------------------------
' Purpose: Creates a set of <supplierURL> elements and attaches them to the appropriate <supplier> element based on supplierID 
' Inputs: an open ADO connection to the ChemACX database
' Returns: Nothing
'-------------------------------------------------------------------------------
Sub GetSupplierURLRS(byRef AdoConnection, byVal sFieldName, byVal sValueList)
	if sValueList <> "" then
		SQLselect = "SELECT DISTINCT SupplierAddr.supplierID, SupplierAddr.Addr1 AS supplierURL "
		Select Case UCase(sFieldName)
			Case "CSNUM"
				If Application("BASE_DBMS") = "ORACLE" then
					SQLQuery = SQLselect & "FROM Product, SupplierAddr WHERE Product.SupplierID = SupplierAddr.SupplierID AND SupplierAddr.Type = 'http' AND Product.CSNum In (" & BuildInClause(sValueList) & ")"
					SQLQuery_Parameters = "InClause" & "|" & adVarChar & "|" & adParamInput & "|" & "|" & sValueList 'Name|Type|Direction|Size|Value
				Else
					SQLQuery = SQLselect & "FROM Product, SupplierAddr WHERE Product.SupplierID = SupplierAddr.SupplierID AND SupplierAddr.Type = 'http' AND Product.CSNum In (" & sValueList & ")"
					SQLQuery_Parameters = ""
				End if
			Case "PRODUCTID"
				If Application("BASE_DBMS") = "ORACLE" then
					SQLQuery = SQLselect & "FROM Product, SupplierAddr WHERE Product.SupplierID = SupplierAddr.SupplierID AND SupplierAddr.Type = 'http' AND Product.ProductID In (" & BuildInClause(sValueList) & ")"
					SQLQuery_Parameters = "InClause" & "|" & adVarChar & "|" & adParamInput & "|" & "|" & sValueList 'Name|Type|Direction|Size|Value
				Else
					SQLQuery = SQLselect & "FROM Product, SupplierAddr WHERE Product.SupplierID = SupplierAddr.SupplierID AND SupplierAddr.Type = 'http' AND Product.ProductID In (" & sValueList & ")"
					SQLQuery_Parameters = ""
				End if
			Case "PACKAGEID"
				If Application("BASE_DBMS") = "ORACLE" then
					SQLQuery = SQLselect & "FROM Package, SupplierAddr WHERE Package.SupplierID = SupplierAddr.SupplierID AND SupplierAddr.Type = 'http' AND Package.PackageID In (" & BuildInClause(sValueList) & ")"
					SQLQuery_Parameters = "InClause" & "|" & adVarChar & "|" & adParamInput & "|" & "|" & sValueList 'Name|Type|Direction|Size|Value
				Else
					SQLQuery = SQLselect & "FROM Package, SupplierAddr WHERE Package.SupplierID = SupplierAddr.SupplierID AND SupplierAddr.Type = 'http' AND Package.PackageID In (" & sValueList & ")"
					SQLQuery_Parameters = ""
				End if
		End Select	
	
		'Set supplierURLRS = AdoConn.Execute(SQLQuery)
		Set supplierURLRS = GetRecordSet(SQLQuery, SQLQuery_Parameters)

		if NOT (supplierURLRS.EOF AND supplierURLRS.BOF) then
			While Not supplierURLRS.EOF
				Set elemTempSupplier = GetElementbyAttrValue(myDOM,"supplier","supplierID",supplierURLRS("supplierID"))
				Set elemSupplierURL = AddElementToParent(myDOM, elemTempSupplier,"supplierURL",supplierURLRS("supplierURL"), False)
				supplierURLRS.MoveNext
			Wend
		Set elemTempSupplier = Nothing
		Set elemSupplierURL = Nothing
		end if
		Set supplierURLRS = Nothing
	End if
End Sub
'-------------------------------------------------------------------------------
' Purpose: Creates a set of <supplierPhone> elements and attaches them to the appropriate <supplier> element based on supplierID 
' Inputs: an open ADO connection to the ChemACX database
' Returns: Nothing
'-------------------------------------------------------------------------------
Sub GetSupplierPhoneRS(byRef AdoConnection, byVal sFieldName, byVal sValueList)
	if sValueList <> "" then
		SQLselect = "SELECT SupplierPhoneID.SupplierID, SupplierPhoneID.CountryCode, SupplierPhoneID.AreaCode, SupplierPhoneID.PhoneNum, SupplierPhoneID.Type, SupplierPhoneID.Location "
		Select Case UCase(sFieldName)
			Case "CSNUM"
				If Application("BASE_DBMS") = "ORACLE" then
					SQLQuery = SQLselect & "FROM Product, SupplierPhoneID WHERE Product.SupplierID = SupplierPhoneID.SupplierID AND Product.CSNum In (" & BuildInClause(sValueList) & ")"
					SQLQuery_Parameters = "InClause" & "|" & adVarChar & "|" & adParamInput & "|" & "|" & sValueList 'Name|Type|Direction|Size|Value
				Else
					SQLQuery = SQLselect & "FROM Product, SupplierPhoneID WHERE Product.SupplierID = SupplierPhoneID.SupplierID AND Product.CSNum In (" & sValueList & ")"
					SQLQuery_Parameters = ""
				End if
			Case "PRODUCTID"
				If Application("BASE_DBMS") = "ORACLE" then
					SQLQuery = SQLselect & "FROM Product, SupplierPhoneID WHERE Product.SupplierID = SupplierPhoneID.SupplierID AND Product.ProductID In (" & BuildInClause(sValueList) & ")"
					SQLQuery_Parameters = "InClause" & "|" & adVarChar & "|" & adParamInput & "|" & "|" & sValueList 'Name|Type|Direction|Size|Value
				Else
					SQLQuery = SQLselect & "FROM Product, SupplierPhoneID WHERE Product.SupplierID = SupplierPhoneID.SupplierID AND Product.ProductID In (" & sValueList & ")"
					SQLQuery_Parameters = ""
				End if
			Case "PACKAGEID"
				If Application("BASE_DBMS") = "ORACLE" then
					SQLQuery = SQLselect & "FROM Package, SupplierPhoneID WHERE Package.SupplierID = SupplierPhoneID.SupplierID AND Package.PackageID In (" & BuildInClause(sValueList) & ")"
					SQLQuery_Parameters = "InClause" & "|" & adVarChar & "|" & adParamInput & "|" & "|" & sValueList 'Name|Type|Direction|Size|Value
				Else
					SQLQuery = SQLselect & "FROM Package, SupplierPhoneID WHERE Package.SupplierID = SupplierPhoneID.SupplierID AND Package.PackageID In (" & sValueList & ")"
					SQLQuery_Parameters = ""
				End if
		End Select	
	
		'Set supplierPhoneRS = AdoConn.Execute(SQLQuery)
		Set supplierPhoneRS = GetRecordSet(SQLQuery, SQLQuery_Parameters)
		
		if NOT (supplierPhoneRS.EOF AND supplierPhoneRS.BOF) then
			While Not supplierPhoneRS.EOF
				Set elemTempSupplier = GetElementbyAttrValue(myDOM,"supplier","supplierID",supplierPhoneRS("supplierID"))
				Set elemSupplierPhone = AddElementToParent(myDOM, elemTempSupplier,"supplierPhone","", True)
				AddAttributeToElement myDOM, elemSupplierPhone, "location", supplierPhoneRS("location")
				AddAttributeToElement myDOM, elemSupplierPhone, "type", supplierPhoneRS("type")
				Set elemCountryCode = AddElementToParent(myDOM, elemSupplierPhone,"countryCode",supplierPhoneRS("countryCode"), False)
				Set elemAreaCode = AddElementToParent(myDOM, elemSupplierPhone,"areaCode",supplierPhoneRS("areaCode"), False)
				Set elemPhoneNum = AddElementToParent(myDOM, elemSupplierPhone,"phoneNum",supplierPhoneRS("phoneNum"), False)
				supplierPhoneRS.MoveNext
			Wend
		Set elemTempSupplier = Nothing
		Set elemSupplierPhone = Nothing
		Set elemCountryCode = Nothing
		Set elemAreaCode = Nothing
		Set elemPhoneNum = Nothing
		end if
		Set supplierPhoneRS = Nothing
	End if
End Sub
'-------------------------------------------------------------------------------
' Purpose: Creates a set of <prodProperty> elements and attaches them to the appropriate <product> element based on ACXprodID 
' Inputs: an open ADO connection to the ChemACX database
' Returns: Nothing
'-------------------------------------------------------------------------------
Sub GetProdPropertyRS(byRef AdoConnection, byVal sFieldName, byVal sValueList)
	if sValueList <> "" then
		Select Case UCase(sFieldName)
			Case "CSNUM"
				If Application("BASE_DBMS") = "ORACLE" then
					SQLQuery = "SELECT PropertyAlpha.Property, PropertyAlpha.Value, PropertyAlpha.ProductID AS ACXprodID FROM PropertyAlpha, Product WHERE PropertyAlpha.ProductID = Product.ProductID AND product.CSNum IN (" & BuildInClause(sValueList) & ")"
					SQLQuery_Parameters = "InClause" & "|" & adVarChar & "|" & adParamInput & "|" & "|" & sValueList 'Name|Type|Direction|Size|Value
				Else
					SQLQuery = "SELECT PropertyAlpha.Property, PropertyAlpha.Value, PropertyAlpha.ProductID AS ACXprodID FROM PropertyAlpha, Product WHERE PropertyAlpha.ProductID = Product.ProductID AND product.CSNum IN(" & sValueList & ")"
					SQLQuery_Parameters = ""
				End if
			Case "PRODUCTID"
				If Application("BASE_DBMS") = "ORACLE" then
					SQLQuery = "SELECT PropertyAlpha.Property, PropertyAlpha.Value, PropertyAlpha.ProductID AS ACXprodID FROM PropertyAlpha, Product WHERE PropertyAlpha.ProductID = Product.ProductID AND product.ProductID IN (" & BuildInClause(sValueList) & ")"
					SQLQuery_Parameters = "InClause" & "|" & adVarChar & "|" & adParamInput & "|" & "|" & sValueList 'Name|Type|Direction|Size|Value
				Else
					SQLQuery = "SELECT PropertyAlpha.Property, PropertyAlpha.Value, PropertyAlpha.ProductID AS ACXprodID FROM PropertyAlpha, Product WHERE PropertyAlpha.ProductID = Product.ProductID AND product.ProductID IN(" & sValueList & ")"
					SQLQuery_Parameters = ""
				End if
			Case "PACKAGEID"
				If Application("BASE_DBMS") = "ORACLE" then
					SQLQuery = "SELECT PropertyAlpha.Property, PropertyAlpha.Value, PropertyAlpha.ProductID AS ACXprodID FROM Package, PropertyAlpha, Product WHERE PropertyAlpha.ProductID = Product.ProductID AND Package.ProductID = Product.ProductID AND Package.PackageID IN (" & BuildInClause(sValueList) & ")"
					SQLQuery_Parameters = "InClause" & "|" & adVarChar & "|" & adParamInput & "|" & "|" & sValueList 'Name|Type|Direction|Size|Value
				Else
					SQLQuery = "SELECT PropertyAlpha.Property, PropertyAlpha.Value, PropertyAlpha.ProductID AS ACXprodID FROM Package, PropertyAlpha, Product WHERE PropertyAlpha.ProductID = Product.ProductID AND Package.ProductID = Product.ProductID AND Package.PackageID IN (" & sValueList & ")"
					SQLQuery_Parameters = ""
				End if
		End Select	
		
		'SQLQuery = "SELECT [PropertyAlpha].[Property], [PropertyAlpha].[Value], [PropertyAlpha].[ProductID] AS ACXprodID FROM PropertyAlpha INNER JOIN Product ON [PropertyAlpha].[ProductID] = [Product].[ProductID] WHERE [product].[CSNum] IN(" & sValueList & ")"
		'Set prodPropertyRS = AdoConn.Execute(SQLQuery)
		Set prodPropertyRS = GetRecordSet(SQLQuery, SQLQuery_Parameters)
		
		if NOT (prodPropertyRS.EOF AND prodPropertyRS.BOF) then
			While Not prodPropertyRS.EOF
				Set elemTempProduct = GetElementbyAttrValue(myDOM,"product","ACXprodID",prodPropertyRS("ACXprodID"))
				Set elemProdProperty = AddElementToParent(myDOM,elemTempProduct,"prodProperty",prodPropertyRS("value"), False)
				AddAttributeToElement myDOM, elemProdProperty, "property", prodPropertyRS("property")
				prodPropertyRS.MoveNext
			Wend
		Set elemTempProduct = Nothing
		Set elemProdProperty = Nothing
		end if
		Set prodPropertyRS = Nothing
	End if
End Sub
'-------------------------------------------------------------------------------
' Purpose: Creates a set of <substanceName> elements and attaches them to the appropriate <substance> element based on CsNum 
' Inputs: an open ADO connection to the ChemACX database
' Returns: Nothing
'-------------------------------------------------------------------------------
Sub GetSynonymRS(byRef AdoConnection, byVal sFieldName, byVal sValueList)
	if sValueList <> "" then
		Select Case UCase(sFieldName)
			Case "CSNUM"
				if DBMS = "ORACLE" then
					SQLQuery = "SELECT CsNum, Name FROM acx_Synonym WHERE CsNum IN(" & BuildInClause(sValueList) & ")"
					SQLQuery_Parameters = "InClause" & "|" & adVarChar & "|" & adParamInput & "|" & "|" & sValueList 'Name|Type|Direction|Size|Value
				else
					SQLQuery = "SELECT CsNum, Name FROM Synonym WHERE CsNum IN(" & sValueList & ")"
					SQLQuery_Parameters = ""
				end if
			Case "PRODUCTID"
				if DBMS = "ORACLE" then
					SQLQuery = "SELECT acx_Synonym.CsNum, acx_Synonym.Name FROM acx_Synonym, Product WHERE acx_Synonym.CsNum = Product.CSNum AND Product.ProductID IN (" & BuildInClause(sValueList) & ")"				
					SQLQuery_Parameters = "InClause" & "|" & adVarChar & "|" & adParamInput & "|" & "|" & sValueList 'Name|Type|Direction|Size|Value
				else
					SQLQuery = "SELECT Synonym.CsNum, Synonym.Name FROM Synonym INNER JOIN Product ON Synonym.CsNum = Product.CSNum WHERE Product.ProductID IN (" & sValueList & ")"
					SQLQuery_Parameters = ""
				end if
			Case "PACKAGEID"
				if DBMS = "ORACLE" then
					SQLQuery = "SELECT acx_Synonym.CsNum, acx_Synonym.Name FROM Package, acx_Synonym, Product WHERE acx_Synonym.CsNum = Product.CSNum AND Package.ProductID = Product.ProductID AND Package.PackageID IN (" & BuildInClause(sValueList) & ")"
					SQLQuery_Parameters = "InClause" & "|" & adVarChar & "|" & adParamInput & "|" & "|" & sValueList 'Name|Type|Direction|Size|Value
				else
					SQLQuery = "SELECT Synonym.CsNum, Synonym.Name FROM Package INNER JOIN (Synonym INNER JOIN Product ON Synonym.CsNum = Product.CSNum) ON Package.ProductID = Product.ProductID WHERE Package.PackageID IN (" & sValueList & ")"
					SQLQuery_Parameters = ""
				end if
		End Select	
	
		'Set SynonymRS = AdoConn.Execute(SQLQuery)
		Set SynonymRS = GetRecordSet(SQLQuery, SQLQuery_Parameters)
		
		if NOT (synonymRS.EOF AND synonymRS.BOF) then
			While Not synonymRS.EOF
				Set elemTempSubstance = GetElementbyAttrValue(myDOM,"substance","csNum",synonymRS("CsNum"))
				Set elemSubstanceName = AddElementToParent(myDOM,elemTempSubstance,"substanceName",synonymRS("Name"),False)
				AddAttributeToElement myDOM,elemSubstanceName,"type","synonym"
				synonymRS.MoveNext
			Wend
		Set elemTempSubstance = Nothing
		Set elemSubstanceName = Nothing
		end if
		Set SynonymRS = Nothing
	End if
End Sub

Function AddCData(str)
	str = "<![CDATA[" & str & "]]>"
	AddCData = str  
End function

'-------------------------------------------------------------------------------
' Purpose: Selects a single element node based on the value of a given attribute.
' Inputs: parser object, element name, attribure name, attribure value
' Returns: the desired element as object
'-------------------------------------------------------------------------------
Function GetElementbyAttrValue(byRef theDOM, byVal sElemName, byVal sAttrName, byVal sAttrValue)
	Dim elemDE
	Dim theElement
	
	Set elemDE = theDOM.documentElement
	Set theElement= elemDE.selectSingleNode("//" & sElemName & "[@" & sAttrName & "='" & sAttrValue & "']")
	Set GetElementbyAttrValue = theElement
	
	Set elemDE = Nothing
	Set theElement = Nothing
End Function 


'-------------------------------------------------------------------------------
' Purpose: 	Adds a child element to a given element.  The element may be empty or contain a text node.
' 			If no text node data is blank the element can be created as empty or not created at all.
' Inputs: parser object, parent element object, child element name, text node value, bolean to create empty element.
' Returns: The newly added child element as object
'-------------------------------------------------------------------------------
Function AddElementToParent(byRef theDOM,byRef elemParent,byVal sName,byVal sValue,byVal bCreateIfEmpty)
	Dim elemSubelement
	Set elemSubelement = Nothing
	Dim textSubelement
	if ((Len (sValue) > 0) OR bCreateIfEmpty) then 
		Set elemSubelement = theDOM.CreateElement(sName)
			if Len(sValue) > 0 then 
				Set textSubelement = theDOM.CreateTextNode(sValue)
				elemSubelement.appendChild(textSubelement)
			End if
		elemParent.appendChild(elemSubelement)
	End if
	Set AddElementToParent = elemSubelement

	Set elemSubelement =  Nothing
	Set textSubelement = Nothing
End Function

'-------------------------------------------------------------------------------
' Purpose: Adds an attribute name-value pair to a given element tag
' Inputs:  parser object, element node object, attribute name, attribute value
' Returns: Nothing
'-------------------------------------------------------------------------------
Sub AddAttributeToElement(byRef theDOM,byRef theElement, byVal sAttrName, byval sAttrValue)
	Dim attr
	if isNull(sAttrValue) then sAttrValue = ""
	'if Len(sAttrValue) >0  then
		Set attr = theDOM.CreateAttribute(sAttrName)
		theElement.setAttributeNode(attr)
		theElement.setAttribute sAttrName, sAttrValue
	'End if
	Set attr = nothing
End Sub

%>

