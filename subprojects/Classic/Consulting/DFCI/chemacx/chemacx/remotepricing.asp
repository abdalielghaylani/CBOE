<%
Sub GetRemotePricing()
' build  comma separated lists of suppliers from the Products recordset
		ProdRS.MoveFirst
		while not ProdRS.EOF
			if Instr(strsuplist, ProdRS("SupplierId")) = 0 then
				strsuplist= strsuplist & ProdRS("SupplierId") & ","
			end if
			ProdRS.MoveNext
		Wend
		' remove the last comma
		strsuplist= Left(strsuplist, Len(strsuplist)-1)
		'response.write "<BR>Suplist= " & strsuplist 
		
		'post supplier list to price robot using ASPTEAR 1.0 
		Set xobj = Server.CreateObject("SOFTWING.ASPtear")
		Response.ContentType="text/html"
		' build the ASPTEAR payload (form variable to post)  
		strpayload = "suplist=" & strsuplist
		' build the url for the price robot
		theurl = "http://sales.chemacx.com/pricerobot.cfm"
		' URL, action (post=1 get=2), payload (name=value,...), username, password
		wddx_packet = xobj.Retrieve(theurl, 1, strpayload, "", "")
		xobj = ""
		set xobj = Nothing
		' price robot returns a WDDX packet containing a recordset with the following fields:
		' SUPLIERID, ISPRICEAVAIL, ISECOM, SORTORDER
		' Deserialize the WDDX packet into suppliers RS with ordering information   
		set MyDeser = Server.CreateObject("WDDX.Deserializer.1")
		set OrderedSuppWRS = MyDeser.deserialize(wddx_packet)
		'response.write "<br><pre>Ordered Supplier list= " & wddx_packet & "</pre><br>"
		MyDeser = ""
		set MyDeser = Nothing
	
		'loop through ProdSWRS to merge sorting info from price robot and generate prodlist for robot
		strprodlist = ""
		for intRow= 1 to ProdSWRS.getRowCount()
		strprodlist = strprodlist & ProdSWRS.getField(intRow, "ProductID") & ","
			for i= 1 to OrderedSuppWRS.getRowCount()
				if ProdSWRS.getField(intRow, "SupplierID") = OrderedSuppWRS.getField(i, "SUPPLIERID") then
				ProdSWRS.setField intRow, "displayorder", OrderedSuppWRS.getField(i,"SORTORDER")
				ProdSWRS.setField intRow, "isecomm", OrderedSuppWRS.getField(i,"ISECOMM")
				ProdSWRS.setField intRow, "iswww", OrderedSuppWRS.getField(i,"ISECOMM")
				ProdSWRS.setField intRow, "ispriceavail", OrderedSuppWRS.getField(i,"ISPRICEAVAIL")					
				end if
			next
		next
		' remove the last comma
		strprodlist= Left(strprodlist, Len(strprodlist)-1)
		'response.write "<BR>PRODLIST= " & strprodlist & "<BR>"
		' Kill the ordersupp recordset
		OrderedSuppWRS = ""
		set OrderedSuppWRS = Nothing

		'  fetch the prices structure from robot by pasing the product list		 
		'post supplier list to price robot using ASPTEAR 1.0 
		Set xobj = Server.CreateObject("SOFTWING.ASPtear")
		Response.ContentType="text/html"
		'build the ASPTEAR payload (form variable to post)  
		strpayload = "prodid=" & strprodlist
		'build the url for the price robot
		theurl = "http://sales.chemacx.com/pricerobot.cfm"
		'URL, action (post=1 get=2), payload (name=value,...), username, password
		wddx_packet_RS = xobj.Retrieve(theurl, 1, strpayload, "", "")
		
		'response.write "<br>Product Price Packet= " &  wddx_packet_RS & "<BR>"
		xobj = ""
		set xobj = nothing
		'price robot returns a WDDX packet containing a single recordset 
		'with the following fields:
		'PRODUCTID,SUPPLIERID,PACKAGEID,SIZE,CATALOG2NUM,CATNUM,LISTPRICE,PRICE,SAVINGS,ISECOM,CSYMBOL
		'Deserialize the WDDX packet into a WDDX recordset with all prices   
		Set MyDeser = Server.CreateObject("WDDX.Deserializer.1")
		Set prices_all = MyDeser.deserialize(wddx_packet_RS)
		MyDeser = ""
		set MyDeser = Nothing	
		sortfield= "displayorder"
		sortorder= "desc"
End Sub
%>