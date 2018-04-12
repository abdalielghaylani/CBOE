<%



Function getAppShapeStatement(ByRef cmd, maxhits)

	if maxhits <> "" then
		maxhits_add=" AND ROWNUM < ? " 
		final_maxhits = CLng(maxhits) + 1
	else
		maxhits_add=""
	end if
    
	'1. grandchild table    
	   
    shapeStr = "SHAPE {select  package.packageid,package.productid,package.""SIZE"",package.catalog2num, "& _
    "                   '$'||ltrim(package.price, '$') price   " & _
    "                   FROM package,substance,product, (select id from chemacxdb.csdohitlist where hitlistid = ? " & maxhits_add & " order by id) hl  " & _
    "                   WHERE package.productid=product.productid " & _
    "                   AND hl.ID=substance.rowid " & _
    "			AND product.csnum=substance.csnum } AS PACKAGE_RS"

	'response.write "PackRS= " & shapeStr    
	cmd.CommandText = shapeStr
    cmd.Parameters.Append cmd.CreateParameter("hitlistid", 131, 1, 0,Session("hitlistID" & dbkey & formgroup))
    if maxhits <> "" then
        cmd.Parameters.Append cmd.CreateParameter("pMaxhits", 131, 1, 0,final_maxhits)
    end if
    Set RSPack = cmd.Execute
    'response.write "PackRSValue= " & RSPack("packageid").value

    'Delete the parameter so we can use the same command object with a different parameter later
    cmd.Parameters.Delete "hitlistid"
    if maxhits <> "" then
       cmd.Parameters.Delete "pMaxhits"	
    end if 
    
    '2. child table related to grandchild
    
    shapeStr = "SHAPE {select product.csnum,product.productid,product.prodname, " & _
	"				   product.proddescrip,product.catalognum,(Select Name from Supplier where Product.SupplierID=Supplier.SupplierID) as ""Product.supplierid"" " & _
    "                  from product, substance, (select id from chemacxdb.csdohitlist where hitlistid = ? " & maxhits_add & " order by id) hl " & _
    "                  where " & _
    "				   hl.ID=substance.rowid and product.csnum=substance.csnum } AS PRODUCT_RS " & _
    "           APPEND (PACKAGE_RS RELATE productid TO productid)"
    

    cmd.CommandText = shapeStr
    cmd.Parameters.Append cmd.CreateParameter("hitlistid", 131, 1, 0,Session("hitlistID" & dbkey & formgroup))
    if maxhits <> "" then
        cmd.Parameters.Append cmd.CreateParameter("pMaxhits", 131, 1, 0,final_maxhits)
    end if
    'response.write "ProdRS= " & shapeStr
    Set RSprod = cmd.Execute
    'response.write "ProdRSValue= " & RSProd("productid").value 
   
    'Delete the parameter so we can use the same command object with a different parameter later
    cmd.Parameters.Delete "hitlistid"
    if maxhits <> "" then
       cmd.Parameters.Delete "pMaxhits"	
    end if 
    
    '3. base table related to child table 
    
    'Use the alias RSsubstance as the child in the append clause
    shapeStr = "SHAPE {SELECT  substance.base64_cdx STRUCTURE,CsCartridge.MolWeight(BASE64_CDX)  MOLWEIGHT, CsCartridge.Formula(BASE64_CDX,'SORTABLE=YES') FORMULA " & _ 
    "					,substance.csnum, substance.CAS, substance.acx_id, substance.hasproducts " & _
    "                   FROM substance, (select id from chemacxdb.csdohitlist where hitlistid = ? " & maxhits_add & " order by id) hl " & _
    "                   where hl.ID=substance.rowid }" & _
    "           APPEND (PRODUCT_RS RELATE csnum TO csnum) AS SUBSTANCE_RS"
    
    cmd.CommandText = shapeStr
    cmd.Parameters.Append cmd.CreateParameter("hitlistid", 131, 1, 0,Session("hitlistID" & dbkey & formgroup))
    if maxhits <> "" then
        cmd.Parameters.Append cmd.CreateParameter("pMaxhits", 131, 1, 0,final_maxhits)
    end if
    
  
    'RS build with full shape statment
	Dim RS
    Set RS = Server.CreateObject("adodb.recordset")

	RS.LockType= 3
	RS.CursorType = 3 
			 
			 
	RS.Open cmd
	set getAppShapeStatement = RS
		

End function
%>