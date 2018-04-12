<!DOCTYPE HTML PUBLIC "-//IETF//DTD HTML//EN">
<% 
dbkey = request("dbname")
formgroup = request("formgroup")
connection_array = Application( "base_connection" & "chemacx")
ConnStr = connection_array(0) & "="  & connection_array(1)
Set AdoConn = Server.CreateObject("ADODB.Connection")
SQLQuery = "SELECT Product.*, Supplier.Name, Supplier.LogoPath FROM Product,Supplier WHERE Product.SupplierID= Supplier.SupplierID AND ProductId=" & request.querystring("prodid")
AdoConn.Open ConnStr
set ProdDetailRS = AdoConn.Execute(SQLQuery)
SQLQuery = "SELECT PropertyAlpha.* FROM PropertyAlpha WHERE PropertyAlpha.ProductID="& request.querystring("prodid")
set PropRS = AdoConn.Execute(SQLQuery)
set AdoConn= nothing
%>
<html>	
	<head>
		<script language="JavaScript">focus(); </script>
		<title>ChemACX - Product Detail</title>
	</head>
	<body bgcolor="#FFFFFF">
		<form name="proddet">
		<table cellspacing="0" cellpadding="0" bordercolor="#4A5AA9" border="1" align="center">
			<tr>
		    	<td align="center"><img SRC="graphics/<%=ProdDetailRS("logopath")%>"></td>
			</tr>
			<tr>
		    	<td align="center">
					<embed src="<%=Application("TempFileDirectoryHTTP" & dbkey)%>mt.cdx" border="0" width="300" height="200" ID="158" name="CDX" type="chemical/x-cdx" viewonly="true" dataurl="<%=Application("ActionForm" & dbkey)%>?dbname=chemacx&amp;formgroup=base_form_group&amp;dataaction=get_structure&amp;Table=Substance&amp;Field=Structure&amp;DisplayType=cdx&amp;StrucID=<%=request.querystring("strucid")%>">
				</td>
			</tr>
			<tr>
		    	<td>
					<table border="0">
						<tr>
							<td>
								<table border="0">
									<tr>
										<td align="right">
											<b>Supplier:</b>
										</td>
										<td>
											<%=ProdDetailRS("Name") %>
										</td>
									</tr>
									<tr>
										<td align="right">
											<b>Product Name:</b>
										</td>
										<td>
											<%=ProdDetailRS("ProdName") %>
										</td>
									</tr>
									<tr>
										<td align="right">
											<b>Description:</b>
										</td>
										<td>
											<%=ProdDetailRS("ProdDescrip") %>
										</td>
									</tr>
									<tr>
										<td align="right">
											<b>Catalog Number:</b>
										</td>
										<td>
											<%=ProdDetailRS("CatalogNum") %>
										</td>
									</tr>
								</table>		
							</td>
						</tr>
					</table>
				</td>		
			</tr>
			<tr>	
		    	<td>
					<table border="0">
						<tr>
							<th align="left">
								Additional Vendor Supplied Data:
							</th>
						</tr>
						<tr>
							<td>
								
									<textarea rows="8" cols="60"><% while not PropRS.EOF
									response.write  PropRS("Property") & ": " & PropRS("Value") & Chr(10) 
									PropRS.MoveNext
									wend 
									%></textarea>
								
							</td>
						</tr>
					</table>
				</td>
			</tr>
			<tr>
				<td align="center">
					<br><a href="Close" onclick="window.self.close(); return false;"><img src="<%=Application("NavButtonGifPath")%>close_btn.gif" alt="Close" border="0"></a><br>
				</td>
			</tr>
	</table>	
</form>
	</body>
</html>


