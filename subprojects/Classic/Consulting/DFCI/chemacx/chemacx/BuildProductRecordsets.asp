<!--#INCLUDE FILE = "../API/apiutils.asp"-->

<%
'SYAN modified to parameterize SQL statements 3/25/2004
	ShowProContent = Session("okOnService_" & Application("CSWebUsers_ServiceID"))
	'Set tab session variables
	If ShowProContent then 
		Session("tab1WhereValue")=Application("tab1WhereValue")
		Session("tab3WhereValue")=	Application("tab3WhereValue")
		Session("tab1gifname")=Application("tab1gifname")
		Session("tab2gifname")=Application("tab2gifname")
		Session("tab3gifname")=Application("tab3gifname")
		Session("tab1OptionText")=Application("tab1OptionText")
		Session("tab2OptionText")=Application("tab2OptionText")
		Session("tab3OptionText")=Application("tab3OptionText")
		' Since vendors w/o price are allowed  in shop cart we cannot allow orders
		Session("Buy_ACXChemACX") = false
		listpriceField = Application("listpricefield")
		priceField= Application("priceField")		
	else
		Session("tab1WhereValue")= 1
		Session("tab3WhereValue")=	0
		Session("tab1gifname")= "online_vendors"
		Session("tab2gifname")= "your_favorites"
		Session("tab3gifname")= "all_vendors"
		Session("tab1OptionText")= "ONLINE VENDORS:"
		Session("tab2OptionText")= "YOUR PREFERRED VENDORS:"
		Session("tab3OptionText")= "SUBSCRIPTION REQUIRED!"
		listpriceField = "PRICE"
		priceField= "PRICE"
	End if
	
	'Read variables from configuration ini file
	tabWhereField = Application("tabWhereField")
	tab1WhereValue = Session("tab1WhereValue")
	tab3WhereValue = Session("tab3WhereValue")	
	tab1gifname= Session("tab1gifname")
	tab2gifname= Session("tab2gifname")
	tab3gifname= Session("tab3gifname")
	tab1OptionText= Session("tab1OptionText")
	tab2OptionText= Session("tab2OptionText")
	tab3OptionText= Session("tab3OptionText")
	
	currencySymbolField= Application("currencySymbolField")
	usepricerobot = Application("usepricerobot")
	
	If  Application("DBMS" & dbkey) = "ORACLE" then
		BaseRS_SQL = "SELECT substance.ACX_ID as ACX_ID, substance.CAS as CAS, substance.HasMSDS as HasMSDS, product.prodname as ProdName, product.proddescrip as ProdDesc FROM substance, product WHERE product.csnum=substance.csnum and substance.csNum=? "  'pjd added fields for display' pjd changed sql "SELECT Substance.CAS, Substance.ACX_ID, Substance.HasMSDS FROM Substance WHERE Substance.CsNum=?"
	Else
		BaseRS_SQL = "SELECT Substance.CAS,Substance.ACX_ID, Substance.HasMSDS FROM Substance WHERE Substance.CsNum=?"
	End if
	BaseRS_Parameters = "BaseID" & "|" & adInteger & "|" & adParamInput & "|" & "|" & BaseID 'Name|Type|Direction|Size|Value
	
	If  Application("DBMS" & dbkey) = "ORACLE" then
		PSynRS_SQL = "SELECT ACX_Synonym.Name FROM Substance, ACX_Synonym WHERE Substance.SynonymID=ACX_Synonym.SynonymID AND Substance.CsNum=?"
	Else
		PSynRS_SQL = "SELECT Synonym.Name FROM Substance, Synonym WHERE Substance.SynonymID=Synonym.SynonymID AND Substance.CsNum=?" 
	End if
	PSynRS_Parameters = "BaseID" & "|" & adInteger & "|" & adParamInput & "|" & "|" & BaseID 'Name|Type|Direction|Size|Value
	
	MSDX_SQL = "Product.HasMSDS,"
	supnamefield = "ShortName"
	
	If  Application("DBMS" & dbkey) = "ORACLE" then
		AllProductRS_SQL = "SELECT " & MSDX_SQL  & " Product.ProdName,Product.CatalogNum,Product.ProdDescrip,Product.SupplierID,Product.ProductID AS PRODUCTID,Product.IsWWW AS iswww,Supplier." & supnamefield & " AS name FROM Product, Supplier WHERE Supplier.SupplierID=Product.SupplierID AND CsNum=?" & "  AND Supplier." & tabWhereField & " IN (" & BuildInClause(tab1WhereValue & "," & tab3WhereValue) & ")" 
		AllProductRS_Parameters = "BaseID" & "|" & adInteger & "|" & adParamInput & "|" & "|" & BaseID & ";" & _
									"InClause" & "|" & adVarChar & "|" & adParamInput & "|" & "|" & tab1WhereValue & "," & tab3WhereValue 'Name|Type|Direction|Size|Value
	Else
		AllProductRS_SQL = "SELECT " & MSDX_SQL  & " Product.ProdName,Product.CatalogNum,Product.ProdDescrip,Product.SupplierID,Product.ProductID AS PRODUCTID,Product.IsWWW AS iswww,Supplier." & supnamefield & " AS name FROM Product, Supplier WHERE Supplier.SupplierID=Product.SupplierID AND CsNum=" & BaseID & "  AND Supplier." & tabWhereField & " IN (" & tab1WhereValue & "," & tab3WhereValue & ")" 
		AllProductRS_Parameters = ""
	End if
	
	If  Application("DBMS" & dbkey) = "ORACLE" then
		SupplierList1_SQL= "SELECT DISTINCT Supplier." & supnamefield & " AS SupplierName, Supplier.SupplierID FROM Product, Supplier WHERE Product.SupplierID = Supplier.SupplierID AND Product.CSNum=?" & " AND Supplier." & tabWhereField & " IN (" & BuildInClause(tab1WhereValue) & ") ORDER BY Supplier." & supnamefield & ""
		SupplierList1_Parameters = "BaseID" & "|" & adInteger & "|" & adParamInput & "|" & "|" & BaseID & ";" & _
									"InClause" & "|" & adVarChar & "|" & adParamInput & "|" & "|" & tab1WhereValue 'Name|Type|Direction|Size|Value
	Else
		SupplierList1_SQL= "SELECT DISTINCT Supplier." & supnamefield & " AS SupplierName, Supplier.SupplierID FROM Product, Supplier WHERE Product.SupplierID = Supplier.SupplierID AND Product.CSNum= " & BaseID & " AND Supplier." & tabWhereField & " IN (" & tab1WhereValue & ") ORDER BY Supplier." & supnamefield & ""
		SupplierList1_Parameters = ""
	End if
	
	If  Application("DBMS" & dbkey) = "ORACLE" then
		SupplierList3_SQL= "SELECT DISTINCT  Supplier." & supnamefield & " AS SupplierName, Supplier.SupplierID FROM Product, Supplier WHERE Product.SupplierID = Supplier.SupplierID AND Product.CSNum=?" & " AND Supplier." & tabWhereField & " IN (" & BuildInClause(tab3WhereValue) & ") ORDER BY Supplier." & supnamefield & ""
		SupplierList3_Parameters = "BaseID" & "|" & adInteger & "|" & adParamInput & "|" & "|" & BaseID & ";" & _
									"InClause" & "|" & adVarChar & "|" & adParamInput & "|" & "|" & tab3WhereValue 'Name|Type|Direction|Size|Value
	Else
		SupplierList3_SQL= "SELECT DISTINCT  Supplier." & supnamefield & " AS SupplierName, Supplier.SupplierID FROM Product, Supplier WHERE Product.SupplierID = Supplier.SupplierID AND Product.CSNum=" & BaseID & " AND Supplier." & tabWhereField & " IN (" & tab3WhereValue & ") ORDER BY Supplier." & supnamefield & ""
		SupplierList3_SQL = ""
	End if
	
	If  Application("DBMS" & dbkey) = "ORACLE" then
		SuppCount_SQL= "SELECT DISTINCT Supplier.SupplierID FROM Product, Supplier WHERE Product.SupplierID = Supplier.SupplierID AND Product.CSNum=?" & " AND Supplier." & tabWhereField & " IN (" & BuildInClause(tab1WhereValue & "," & tab3WhereValue) & ")"
		SuppCount_Parameters = "BaseID" & "|" & adInteger & "|" & adParamInput & "|" & "|" & BaseID & ";" & _
									"InClause" & "|" & adVarChar & "|" & adParamInput & "|" & "|" & tab1WhereValue & "," & tab3WhereValue 'Name|Type|Direction|Size|Value
	Else
		SuppCount_SQL= "SELECT DISTINCT Supplier.SupplierID FROM Product, Supplier WHERE Product.SupplierID = Supplier.SupplierID AND Product.CSNum=" & BaseID & " AND Supplier." & tabWhereField & " IN (" & tab1WhereValue & ", " & tab3WhereValue & ")"
		SuppCount_Parameters = ""
	End if
	
	' SQL for Preferred supplier tab
	suplist= request.Cookies("acxprefsuplist")
	preferedList= Replace(suplist, ":", ",")
	
	If  Application("DBMS" & dbkey) = "ORACLE" then
		SupplierList2_SQL= "SELECT DISTINCT  Supplier." & supnamefield & " AS SupplierName, Supplier.SupplierID FROM Product, Supplier WHERE Product.SupplierID = Supplier.SupplierID AND Product.CSNum=?" & " AND Supplier.supplierID IN (" & BuildInClause(preferedList) & ") ORDER BY Supplier." & supnamefield & "" 	
		SupplierList2_Parameters = "BaseID" & "|" & adInteger & "|" & adParamInput & "|" & "|" & BaseID & ";" & _
										"InClause" & "|" & adVarChar & "|" & adParamInput & "|" & "|" & preferedList 'Name|Type|Direction|Size|Value
	Else
		SupplierList2_SQL= "SELECT DISTINCT  Supplier." & supnamefield & " AS SupplierName, Supplier.SupplierID FROM Product, Supplier WHERE Product.SupplierID = Supplier.SupplierID AND Product.CSNum=" & BaseID & " AND Supplier.supplierID IN (" & preferedList & ") ORDER BY Supplier." & supnamefield & "" 	
		SupplierList2_Parameters = "" 
	End if
	
	If  Application("DBMS" & dbkey) = "ORACLE" then
		limitedSupplierList1_SQL= "SELECT DISTINCT Supplier." & supnamefield & " AS SupplierName, Supplier.SupplierID FROM Product, Supplier WHERE Product.SupplierID = Supplier.SupplierID AND Product.CSNum=?" & " AND Supplier.supplierID IN (" & BuildInClause(Session("suppliersTosearch")) & ") ORDER BY Supplier." & supnamefield & ""
		limitedSupplierList1_Parameters = "BaseID" & "|" & adInteger & "|" & adParamInput & "|" & "|" & BaseID & ";" & _
										"InClause" & "|" & adVarChar & "|" & adParamInput & "|" & "|" & Session("suppliersTosearch") 'Name|Type|Direction|Size|Value
	Else
		limitedSupplierList1_SQL= "SELECT DISTINCT Supplier." & supnamefield & " AS SupplierName, Supplier.SupplierID FROM Product, Supplier WHERE Product.SupplierID = Supplier.SupplierID AND Product.CSNum= " & BaseID & " AND Supplier.supplierID IN (" & Session("suppliersTosearch") & ") ORDER BY Supplier." & supnamefield & ""
		limitedSupplierList1_Parameters = ""
	End if
	
	
	If  Application("DBMS" & dbkey) = "ORACLE" then
		limitedProductRS_SQL = "SELECT " & MSDX_SQL  & " Product.ProdName,Product.CatalogNum,Product.ProdDescrip,Product.SupplierID,Product.ProductID AS PRODUCTID,Product.IsWWW AS iswww,Supplier." & supnamefield & " AS name FROM Product, Supplier WHERE Supplier.SupplierID=Product.SupplierID AND CsNum=?" & "  AND Supplier.supplierID IN (" & BuildInClause(Session("suppliersTosearch")) & ")"
		limitedProductRS_Parameters = "BaseID" & "|" & adInteger & "|" & adParamInput & "|" & "|" & BaseID & ";" & _
										"InClause" & "|" & adVarChar & "|" & adParamInput & "|" & "|" & Session("suppliersTosearch") 'Name|Type|Direction|Size|Value
	Else
		limitedProductRS_SQL = "SELECT " & MSDX_SQL  & " Product.ProdName,Product.CatalogNum,Product.ProdDescrip,Product.SupplierID,Product.ProductID AS PRODUCTID,Product.IsWWW AS iswww,Supplier." & supnamefield & " AS name FROM Product, Supplier WHERE Supplier.SupplierID=Product.SupplierID AND CsNum=" & BaseID & "  AND Supplier.supplierID IN (" & Session("suppliersTosearch") & ")"
		limitedProductRS_Parameters = ""
	End if

	'Set BaseRS = GetDisplayRecordset(dbkey, formgroup, "Substance.*","Substance", BaseRS_SQL, BaseID , "SQL_FULL")
	Set BaseRS = GetRecordSet(BaseRS_SQL, BaseRS_Parameters)
	
	'Set PSynRS = GetDisplayRecordset(dbkey, formgroup, "Synonym","Synonym", PSynRS_SQL, BaseID, "SQL_FULL")
	Set PSynRS = GetRecordSet(PSynRS_SQL, PSynRS_Parameters)
	
	'Set SynRS= GetDisplayRecordset(dbkey, formgroup, "Name","Synonym", "", BaseID, "")
	If  Application("DBMS" & dbkey) = "ORACLE" then
		SynRS_SQL = "SELECT ACX_Synonym.Name FROM Substance, ACX_Synonym WHERE Substance.SynonymID=ACX_Synonym.SynonymID AND Substance.CsNum=?"
	Else
		PSynRS_SQL = "SELECT NAME FROM Synonym WHERE Synonym.CsNum=?" 
	End if
	SynRS_Parameters = "BaseID" & "|" & adInteger & "|" & adParamInput & "|" & "|" & BaseID
	Set SynRS = GetRecordSet(SynRS_SQL, SynRS_Parameters)

	CAS = BaseRS("CAS").value
	bHasMSDX = BaseRS("HasMSDS").value
	ACX_ID = BaseRS("ACX_ID").value
	ProdName = BaseRS("ProdName").value
	ProdDesc = BaseRS("ProdDesc").value
	if (PSynRS.EOF AND PSynRS.BOF) then 
		if (SynRS.EOF AND SynRS.BOF) then
			firstSyn = "No name available"
		else
			firstSyn = SynRS("Name")
		End if
	else 
		firstSyn = PSynRS("Name") 
	end if 	

bLimitSearch= request.Cookies("acxlimitsearch")
if  bLimitSearch = "0" OR blimitSearch = "" then
	'Set SupRS1 = GetDisplayRecordset(dbkey, formgroup, "Name","Product", SupplierList1_SQL, "0" , "SQL_FULL")
	Set SupRS1 = GetRecordSet(SupplierList1_SQL, SupplierList1_Parameters)
	
	
	SupRS1.MoveFirst
	if (SupRS1.EOF= True AND SupRS1.BOF= True) then ' First supplier list is empty
		numsuppliers1=0
		SupplierList1_WDDX = "---No Suppliers Found---"
		nolist1 = 1
	else ' There are suppliers on first list
		SupplierList1_WDDX = SerializeADORecordset(SupRS1)
		nolist1 = 0
	end if
	
	'Read the preferred supplier list from the cookie
	
	if len(suplist) <> 0 then 'A preferred supplier list is available	
		'Set SupRS2 = GetDisplayRecordset(dbkey, formgroup, "Name","Product", SupplierList2_SQL, "0" , "SQL_FULL")
		Set SupRS2 = GetRecordSet(SupplierList2_SQL, SupplierList2_Parameters)
	
		SupRS2.MoveFirst
		if (SupRS2.EOF= True AND SupRS2.BOF= True) then ' Second list is empty
			SupplierList2_WDDX = "---No Suppliers Found---"
			nolist2 = 1
		else ' There are suppliers on the second list 
			SupplierList2_WDDX = SerializeADORecordset(SupRS2)
			nolist2 = 0
		end if
	else ' No preferred suppliers found
		SupplierList2_WDDX = "Your preferred list is not defined!"
		nolist2 = 1
	end if
		
	'Set SupRS3 = GetDisplayRecordset(dbkey, formgroup, "Name","Product", SupplierList3_SQL, "0" , "SQL_FULL")
	Set SupRS3 = GetRecordSet(SupplierList3_SQL, SupplierList3_Parameters)
	
	SupRS3.MoveFirst
	if (SupRS3.EOF= True AND SupRS3.BOF= True) then
		numsuppliers3 =0
		SupplierList3_WDDX = "---No Suppliers Found---"
		nolist3 = 1
	else
		SupplierList3_WDDX = SerializeADORecordset(SupRS3)
		nolist3 = 0
	end if
	
	'Set SupCountRS = GetDisplayRecordset(dbkey, formgroup, "Name","Product", SuppCount_SQL, "0" , "SQL_FULL")
	Set SupCountRS = GetRecordSet(SuppCount_SQL, SuppCount_Parameters)

	rowsarray = SupCountRS.GetRows()
	numsuppliers= UBound(rowsarray, 2)+1
	ProductRS_SQL = AllProductRS_SQL
	ProductRS_Parameters = AllProductRS_Parameters
else 'the search is limited to a subset of suppliers
	firstOptionText = "FROM LIMITED SEARCH:"
	
	'Set SupRS1 = GetDisplayRecordset(dbkey, formgroup, "Name","Product", limitedSupplierList1_SQL, "0" , "SQL_FULL")
	Set SupRS1 = GetRecordSet(limitedSupplierList1_SQL, limitedSupplierList1_Parameters)

	SupRS1.MoveFirst
	if (SupRS1.EOF= True AND SupRS1.BOF= True) then
		numsuppliers =0
		SupplierList1_WDDX = "---No Suppliers Found---"
		nolist1 = 1
	else
		rowsarray = SupRS1.GetRows()
		numsuppliers=UBound(rowsarray, 2)+1
		SupplierList1_WDDX = SerializeADORecordset(SupRS1)
		nolist1 = 0
	end if
	ProductRS_SQL = limitedProductRS_SQL
	ProductRS_Parameters = limitedProductRS_Parameters
end if
'Response.Write "SQL= " &ProductRS_SQL
'Response.end
' Get product recordset

'Set ProdRS = GetDisplayRecordset(dbkey, formgroup, "Product.*","Product", ProductRS_SQL, BaseID , "SQL_FULL") 
Set ProdRS = GetRecordSet(ProductRS_SQL, ProductRS_Parameters)

ProdRS.MoveFirst	
if (ProdRS.EOF= True AND ProdRS.BOF= True) then
 	noSuppliersFound = 1	
Else	
	noSuppliersFound = 0	
	'convert the ProdRS from ADO to Serverside WDDX  by using the
	'JS serverside function AdoToWddxRS provided in the AdoToWddx.js include file.
	Set ProdSWRS = AdoToWddxRS(ProdRS)
	numProducts = ProdSWRS.getRowCount()
	
	if Ucase(usepricerobot) = "TRUE" then 'Get prices from remote server
		GetRemotePricing()
	else ' Get prices from local server
		for intRow= 1 to numProducts
			strprodlist = strprodlist & ProdSWRS.getField(intRow, "PRODUCTID") & ","
		next
		strprodlist= Left(strprodlist, Len(strprodlist)-1)
		' SQL to select package table info
		If  Application("DBMS" & dbkey) = "ORACLE" then
			localPriceRS_SQL = "SELECT PRODUCTID,PACKAGEID," & Application("ProductSizeFieldName" & dbkey) & " AS " & UCase(Application("ProductSizeFieldName" & dbkey)) & " ,CATALOG2NUM," & listpriceField & "  AS THELISTPRICE," & priceField & " AS THEPRICE ,SAVINGS," & currencySymbolField & " AS THECSYMBOL FROM Package WHERE ProductID IN (" & BuildInClause(strprodlist) & ") ORDER BY ProductID, PRICE"
			localPriceRS_SQL_Parameters = "InClause" & "|" & adVarChar & "|" & adParamInput & "|" & "|" & strprodlist 'Name|Type|Direction|Size|Value
		Else
			localPriceRS_SQL = "SELECT PRODUCTID,PACKAGEID," & Application("ProductSizeFieldName" & dbkey) & " AS " & UCase(Application("ProductSizeFieldName" & dbkey)) & " ,CATALOG2NUM," & listpriceField & "  AS THELISTPRICE," & priceField & " AS THEPRICE ,SAVINGS," & currencySymbolField & " AS THECSYMBOL FROM Package WHERE ProductID IN (" & strprodlist & ") ORDER BY ProductID, PRICE"
			localPriceRS_SQL_Parameters = ""
		End if
				
		'Response.Write localPriceRS_SQL
		'Response.end
		Set DataConn=GetConnection(dbkey, formgroup, "Substance")
		'Set prices_all_ado = DataConn.Execute(localPriceRS_SQL)
		Set prices_all_ado = GetRecordSet(localPriceRS_SQL, localPriceRS_SQL_Parameters)
		
		'convert the prices_all from ADO to Serverside WDDX  by using the
		'JS serverside function AdoToWddxRS provided in the AdoToWddx.js include file.
		if NOT (prices_all_Ado.BOF = True AND prices_all_ado.EOF = True) then
		
			Set prices_all = AdoToWddxRS(prices_all_ado)
		End if
	end if
	
	' use Js converter to transfer ProdSWRS to client side
	Set JsConv = Server.CreateObject("WDDX.JSConverter.1")
	JSCode = JsConv.ConvertData(ProdSWRS, "ProdWRS")
	response.write "<SCRIPT LANGUAGE=""JavaScript"">var prodshown;" & JSCode & "</SCRIPT>"
	set ProdSWRS = Nothing
	JsConv = ""
	set JsConv = Nothing

	' create pricesWRS_prodid recordsets from prices_all recordset
	Set TempPricesWRS = Server.CreateObject("WDDX.Recordset.1")
	 
	TempPricesWRS.addColumn("SUPPLIERID")
	TempPricesWRS.addColumn("PACKAGEID")
	TempPricesWRS.addColumn("SIZE")
	TempPricesWRS.addColumn("CATALOG2NUM")
	TempPricesWRS.addColumn("CATNUM")
	TempPricesWRS.addColumn("LISTPRICE")
	TempPricesWRS.addColumn("PRICE")
	TempPricesWRS.addColumn("SAVINGS")
	TempPricesWRS.addColumn("CAS")
	TempPricesWRS.addColumn("STRUCID")
	TempPricesWRS.addColumn("CSYMBOL")
	Set MyConv = Server.CreateObject("WDDX.JSConverter.1")
	row=0
	
	for ii=1 to prices_all.getRowCount()
		pid = prices_all.getField(ii,"PRODUCTID")
		if ii < prices_all.getRowCount() then nextpid = prices_all.getField(ii+1,"PRODUCTID") else nextpid =0
		clientRSname = "PricesWRS_" & pid
		TempPricesWRS.addRows(1)
		row = row+1
		TempPricesWRS.setField row,"SUPPLIERID", prices_all.getField(ii,"SUPPLIERID")
		TempPricesWRS.setField row,"PACKAGEID", prices_all.getField(ii,"PACKAGEID")
		TempPricesWRS.setField row,"SIZE", prices_all.getField(ii,"SIZE")
		TempPricesWRS.setField row,"CATALOG2NUM", prices_all.getField(ii,"CATALOG2NUM")
		TempPricesWRS.setField row,"CATNUM", prices_all.getField(ii,"CATNUM")
		TempPricesWRS.setField row,"LISTPRICE", prices_all.getField(ii,"THELISTPRICE")
		TempPricesWRS.setField row,"PRICE", prices_all.getField(ii,"THEPRICE")
		TempPricesWRS.setField row,"SAVINGS", prices_all.getField(ii,"SAVINGS")
		TempPricesWRS.setField row,"CAS", CAS
		TempPricesWRS.setField row,"STRUCID", BaseID
		TempPricesWRS.setField row,"CSYMBOL", prices_all.getField(ii,"THECSYMBOL")
		if  StrComp(pid,nextpid) then	 
			MyDynamicJSCode = MyConv.ConvertData(TempPricesWRS, clientRSname)
			response.write "<SCRIPT LANGUAGE=""JavaScript"">" & MyDynamicJSCode & "</SCRIPT>"
			TempPricesWRS = ""
			set TempPricesWRS = Nothing
			row =0 
			Set TempPricesWRS = Server.CreateObject("WDDX.Recordset.1")
			TempPricesWRS.addColumn("SUPPLIERID")
			TempPricesWRS.addColumn("PACKAGEID")
			TempPricesWRS.addColumn("SIZE")
			TempPricesWRS.addColumn("CATALOG2NUM")
			TempPricesWRS.addColumn("CATNUM")
			TempPricesWRS.addColumn("LISTPRICE")
			TempPricesWRS.addColumn("PRICE")
			TempPricesWRS.addColumn("SAVINGS")
			TempPricesWRS.addColumn("CAS")
			TempPricesWRS.addColumn("STRUCID")
			TempPricesWRS.addColumn("CSYMBOL")
		end if					  
	next
	MyConv = ""
	Set MyConv = Nothing
end if

' Prepare Zoom function
plugin_value =GetFormGroupVal(dbkey, formgroup, kPluginValue)
if  plugin_value  then
	zoomFunction = "getStrucZoomBtn('Substance.Structure" & BaseID & "')"
else
	zoomFunction = "getStrucZoomBtnNP_asp('SubstanceStructure_" &  BaseID & "',600,450)"
end if
%>


