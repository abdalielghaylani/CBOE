<!--#INCLUDE FILE="AdoToWddx.js"-->
<%
'get connection string from application variable
connection_array = Application( "base_connection" & "chemacx")
ConnStr = connection_array(0) & "="  & connection_array(1)


strucid= Request.QueryString("strucid")
plugin= Request.QueryString("plugin")
prodid = Request.QueryString("prodid")
return = Request.QueryString("return")
acx_id = Request.QueryString("acx_id")
Session("supplierSelect") = Request.QueryString("supplierid")
if NOT (return= "wddx" OR return="htmltable") then 
return = "acxframeset"
end if

if return ="acxframeset" then
	if plugin = "false" then
	formgroup = "basenp_form_group"
	else
	formgroup = "base_form_group"
	end if 
	
	if prodid = "" then
		if not acx_id = "" then
			if not Ucase(acx_id) =  "NA" then
				response.redirect Application("AppPathHTTP") & "/default.asp?formgroup=" & formgroup & "&dbname=chemacx&dataaction=query_string&field_type=TEXT&field_criteria=IN&full_field_name=Substance.ACX_ID&field_value=" & acx_id
			Else
				response.write  "Product cannot be accessed from ChemStore CD"
			End if 
		else
			response.redirect Application("AppPathHTTP") & "/default.asp?formgroup=" & formgroup & "&dbname=chemacx&dataaction=query_string&field_type=INTEGER&field_criteria=IN&full_field_name=Substance.Csnum&field_value=" & strucid
		End if
	
	else
	response.redirect Application("AppPathHTTP") & "/singleProductView.asp?formgroup=" & formgroup & "&dbname=chemacx&prodid=" & prodid 
	end if
else
	'get connection string from application variable
	connection_array = Application( "base_connection" & "chemacx")
	ConnStr = connection_array(0) & "="  & connection_array(1)
	Set AdoConn = Server.CreateObject("ADODB.Connection")
	AdoConn.Open ConnStr
	
	SQLQuery_1 = "SELECT [Package].[ProductID] AS ProductID, [Package].[PackageID] AS PackageID, [Product].[SupplierID] AS SupplierID, [Supplier].[ShortName] AS ShortName, [Product].[CatalogNum] AS CatalogNum, [Package].[Catalog2Num] AS Catalog2Num, [Product].[ProdName] AS ProdName, [Product].[ProdDescrip] AS ProdDescrip,[Package].[SIZE] AS [Size], [Package].[" & Application("Pricefield") &  "] AS Price"
	SQLQUERY_2 = ", [Package].[" & Application("LISTPRICEfield") &  "] AS ListPrice"
	SQLQuery_3 = " FROM (Package INNER JOIN Product ON [Package].[ProductID] = [Product].[ProductID]) INNER JOIN Supplier ON [Product].[SupplierID] = [Supplier].[SupplierID] WHERE ([Product].[CSNum]=" & strucid &")" & "  AND Supplier." & Application("tabWhereField") & " IN (" & Application("tab1WhereValue") & "," & Application("tab3WhereValue") & ")"

if Application("showListPrice") then
	SQLQuery_1 = SQLQuery_1 + SQLQuery_2
End if
sqlquery = SQLQuery_1 + SQLQuery_3

	'response.write sqlquery & "<br>"
	'response.end
	set skuRS = Server.CreateObject("ADODB.Recordset")
		skuRs.ActiveConnection = AdoConn
		skuRS.CursorType = 3
		skuRS.Source = SQLQuery
		skuRs.open
		skuCount = skuRs.RecordCount
	
	if return = "wddx" then
	skuRS_wddx= SerializeADORecordset(skuRS)
	response.write skuRS_wddx
	else%>
	<%skuRS.MoveFirst%>
<table Border="1" Cols="<%=skucount%>">
	<tr>
		<% For each oField in skuRS.Fields%>	
		<th>
			<%= oField.Name%>
		</th>
		<% Next%>
	</tr>
	<% Do while NOT skuRS.EOF%>
		<tr>
			<% For each oField in skuRS.Fields %>	
				<td>
					<%if isNull(oField) Then 
					response.write "&nbsp;"
					else
					response.write oField.value
					end if
					%>
				</td>
			<% Next 
			   skuRS.MoveNext	
			%>
		</tr>
	<%Loop%>
</table>
<%
skuRS.Close
Set skuRS = Nothing
%>


	<%end if%> 
<%end if%>





