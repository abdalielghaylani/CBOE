<!--#INCLUDE VIRTUAL = "/chemacx/api/apiutils.asp"-->
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/display_func_vbs.asp"-->
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/ado.inc"-->

<!DOCTYPE HTML PUBLIC "-//IETF//DTD HTML//EN">
<%
'SYAN modified on 3/31/2004 to support parameterized SQL

Dim Conn
Dim RS
Response.Expires = -1
dbkey = request("dbname")
formgroup = request("formgroup")
BaseID = request.querystring("strucid")
isPlugin = Request("IsPlugin")
TableName = "Substance"
FieldName = "Structure"
FileRootName = TableName & FieldName & "_" & BaseID
GifWidth = 300
GifHeight= 300
GetACXConnection()

'SQLQuery = "SELECT Product.*, Supplier.Name, Supplier.LogoPath FROM Product,Supplier WHERE Product.SupplierID= Supplier.SupplierID AND ProductId=" & request.querystring("prodid")
SQLQuery = "SELECT Product.*, Supplier.Name, Supplier.LogoPath FROM Product,Supplier WHERE Product.SupplierID= Supplier.SupplierID AND ProductId=?"
SQLQuery_Parameters = "ID" & "|" & adInteger & "|" & adParamInput & "|" & "|" & request.querystring("prodid")
'set ProdDetailRS = Conn.Execute(SQLQuery)
Set ProdDetailRS = GetRecordSet(SQLQuery, SQLQuery_Parameters)

'SQLQuery = "SELECT PropertyAlpha.* FROM PropertyAlpha WHERE PropertyAlpha.ProductID="& request.querystring("prodid")
SQLQuery = "SELECT PropertyAlpha.* FROM PropertyAlpha WHERE PropertyAlpha.ProductID=?"
SQLQuery_Parameters = "ID" & "|" & adInteger & "|" & adParamInput & "|" & "|" & request.querystring("prodid")
'Response.Write sqlQuery
'set PropRS = Conn.Execute(SQLQuery)
Set PropRS = GetRecordSet(SQLQuery, SQLQuery_Parameters)

%>
<html>	
	<head>
		<script language="JavaScript">focus(); </script>
		<title>ChemACX - Product Detail</title>
		<script language="JavaScript" src="/cfserverasp/source/chemdraw.js"></script>
		<script>cd_includeWrapperFile("/cfserverasp/source/")</script>
	</head>
	<body bgcolor="#FFFFFF">
		<form name="proddet">
		<table cellspacing="0" cellpadding="0" bordercolor="#4A5AA9" border="1" align="center">
			<tr>
		    	<td align="center">
		    	<% logo =  ProdDetailRS("logopath")
		    	if NOT isNull(logo) then%>
		    		<img SRC="graphics/<%=logo%>">
		    	<%else%>
		    		&nbsp;
		    	<%end if%>
		    	</td>
			</tr>
			<tr>
		    	<td align="center">
					<% 
					if Not isPlugin then
						zoomGifDir = Application("TempFileDirectory" & dbkey) & FileRootName & "_" & gifWidth & "x" & gifHeight & ".gif"
						zoomGifPath = Application("TempFileDirectoryHTTP" & dbkey) & FileRootName & "_" & gifWidth & "x" & gifHeight & ".gif"
						Set checkFile = Server.CreateObject("Scripting.FileSystemObject")
						If NOT checkFile.FileExists(zoomGifDir) then
							ConvertCDXtoGIF dbkey, TableName, FieldName, BaseID, gifWidth, gifHeight	
						End if
						Set checkFile = nothing				
				%>	
									<img SRC="<%=zoomGifPath%>" Border="0">
				<%Else%>
									<script language="javascript">cd_insertObjectStr("<embed src='<%=initial_CDP_file%>' border='0' width='300' height='300' id='1' name='CDX' viewonly='true' type='chemical/x-cdx' dataurl='<%=Application("ActionForm" & dbkey)%>?dbname=chemacx&formgroup=base_form_group&dataaction=get_structure&Table=<%=TableName%>&Field=<%=FieldName%>&DisplayType=cdx&StrucID=<%=BaseID%>'>");</script> 
				<%End if %>		
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
								
									<textarea rows="8" cols="60" onfocus="blur()"><% while not PropRS.EOF
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
<%
set ProdDetailRS = Nothing
set PropRS = Nothing
Conn.Close
set Conn= nothing
%>
	</body>
</html>


