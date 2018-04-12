<% 
' ASP code to support the Purchasing For Excel functionality in ChemOffice 8.0 or earlier
' Not clear if we need to preserve this any more, now that we're dropping it in ChemOffice 9.0
' But what if existing users still care?
if (NOT request.QueryString("shopcartID")="") then
scReqNum = request.QueryString("shopcartID")
Set fs = Server.CreateObject("Scripting.FileSystemObject")
tempDirPath =  Application("TempFileDirectory" & "chemacx") & "tempshopcart" & scReqNum  & ".txt"
Set oTextStream = fs.OpenTextFile(tempDirPath)
wddxPacket = oTextStream.ReadAll
oTextStream.Close
Set oTextStream = nothing
Set fs = nothing
'response.write "<BR>" & wddxPacket

Set MyDeser = Server.CreateObject("WDDX.Deserializer.1")
Set qty_ST = MyDeser.deserialize(wddxPacket)
packid_AR = qty_ST.getPropNames()
packid_list=""
For Each packid In packid_AR
packid_list = packid_list & packid & ","
Next
packid_list = left(packid_list,len(packid_list)-1)
'response.write packid_list

''get connection string from application variable
'connection_array = Application( "base_connection" & "chemacx")
'ConnStr = connection_array(0) & "="  & connection_array(1)
''response.write ConnStr
'Set AdoConn = Server.CreateObject("ADODB.Connection")

'SQLQuery = "SELECT [Package].[PackageID] AS packid, [Product].[SupplierID] AS supid, [Supplier].[ShortName] AS supname,[Product].[CatalogNum] AS catnum, [Product].[ProdName] AS descript," & Application("ProductSizeFieldName") & " AS thesize, [Package].[" & Application("listPricefield") &  "] AS listprice, [Package].[" & Application("pricefield") & "] AS price, [Package].[CSYMBOL] AS thecsymbol FROM (Package INNER JOIN Product ON Package.ProductID = Product.ProductID) INNER JOIN Supplier ON Product.SupplierID = Supplier.SupplierID WHERE Package.PackageID IN (" & packid_list & ")" 

'SQLQuery = "SELECT Substance.CAS AS CAS, Substance.CsNum AS strucid, Package.PackageID AS packid, Product.SupplierID AS supid, Supplier.ShortName AS supname,Product.CatalogNum AS catnum, Product.ProdName AS descript," & Application("ProductSizeFieldName") & " AS thesize, Package." & Application("listPricefield") &  " AS listprice, Package." & Application("pricefield") & " AS price, Package.CSYMBOL AS thecsymbol FROM Package, Product, Substance, Supplier WHERE Package.ProductID = Product.ProductID AND Product.CsNum = Substance.CSNum AND Product.SupplierID = Supplier.SupplierID AND Package.PackageID IN (" & packid_list & ")" 
SQLQuery = "SELECT Substance.CAS AS CAS, Substance.CsNum AS strucid, Package.PackageID AS packid, Product.SupplierID AS supid, Supplier.ShortName AS supname,Product.CatalogNum AS catnum, Product.ProdName AS descript," & Application("ProductSizeFieldName" & dbkey) & " AS thesize, Package." & Application("listPricefield") &  " AS listprice, Package." & Application("pricefield") & " AS price, Package.CSYMBOL AS thecsymbol FROM Package, Product, Substance, Supplier WHERE Package.ProductID = Product.ProductID AND Product.CsNum = Substance.CSNum AND Product.SupplierID = Supplier.SupplierID AND Package.PackageID IN (" & BuildInClause(packid_list) & ")" 
SQLQuery_Parameters = "InClause" & "|" & adVarChar & "|" & adParamInput & "|" & "|" & packid_list 'Name|Type|Direction|Size|Value

'AdoConn.Open ConnStr
'set ShopCartRS = AdoConn.Execute(SQLQuery)
set ShopCartRS = GetRecordSet(SQLQuery, SQLQuery_Parameters)

rowsarray = ShopCartRS.GetRows()
numItems=UBound(rowsarray, 2)+1
set AdoConn= nothing
'response.write numItems
sq= "'"
jj = "','"
qq = Chr(34)

ShopCartRS.MoveFirst
if NOT (ShopCartRS.EOF AND ShopCartRS.BOF) then
response.write "<SCRIPT language=JavaScript>"
	While Not ShopCartRS.EOF

	
'escape text fields
supname = Replace(ShopCartRS("supname"),"'" ,"@@")
supname = Replace(supname,qq,"~~")
descript = Replace(ShopCartRS("descript"),"'" ,"@@")
descript = Replace(descript,qq,"~~")

addtocart = "AddtoShopcartWRSRemote(" & qty_ST.getProp(ShopCartRS("packid")) & "," & sq & ShopCartRS("packid") & jj &  ShopCartRS("supid") & jj & supname & jj & ShopCartRS("catnum") & jj & descript & jj & ShopCartRS("thesize") & jj & ShopCartRS("price") & jj & ShopCartRS("listprice") & jj & ShopCartRS("thecsymbol") & jj & ShopCartRS("CAS") & jj & ShopCartRS("strucid") & sq & ");"
	response.write addtoCart
	ShopCartRS.MoveNext
	Wend
response.write "ViewCart();"	
response.write "</script>"
end if


end if%>
