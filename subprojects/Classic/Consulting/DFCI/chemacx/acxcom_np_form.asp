<title>CambridgeSoft's ChemACX.com - Search chemical catalogs online.</title>
<meta name ="keywords" content="reagent, reagents, ChemACX, ChemACX.com, chemical, chemicals, exchange, buy, order, laboratory reagent, amino, DNA, ACS Grade, USP, solvent, bulk chemicals, reagent grade, Chemical suppliers, Aldrich, VWR, Sigma,Fisher, pharmaceutical, pharmaceutical intermediate, drug precursor, synthesis, chemical intermediate, active pharmaceutical ingredient, clinical trials, pre-clinical testing, biochemical, protein sequencing, organic chemistry, analytical reagents, laboratory supplies, Material safety data sheets, MSDS, toxicity data, enzyme, clone, probes, life science, life sciences, biochemistry, biotechnology, genetics, cell biology, microbiology, discount, discounts, sale, Chemdex, Sciquest">

<meta name ="description" content="ChemACX.com is the only complete substructure searchable chemical database online.  Search and buy over 500,000 chemical compounds online from over 200 of the world's largest chemical suppliers.">

</head>
<body background="http://images.cambridgesoft.com/chemfinder/background_finderblue_1200.gif" leftmargin="9" topmargin="9">

<!--#INCLUDE FILE = "banner.asp"-->

<!--#INCLUDE FILE = "simplesearchbox.asp"-->

<TABLE valign="top" width="600" cellpadding="2">
<tr>
<td valign=top width="5">
<table cellpadding="0" cellspacing="1">

<!--#INCLUDE FILE = "leftnavigationlist.asp"-->

</table>

</td>
<td valign=top>
<p>
This Available Chemicals Xchange is a collection of 

<% 
if not Session("IsNet") then
	response.write "over 300"
end if
%>

catalogs from major chemical suppliers. 

<% 
if Session("IsNet") then
	response.write "It is provided free of charge to the chemical community as a demonstration of the sort of data available at the more comprehensive <a href=""http://chemfinder.cambridgesoft.com/chemicals/chemacxpro.asp"">ChemACX Pro</a> service. "
	response.write "</p><p>"
	response.write "ChemACX Net includes information from 30 of the premier chemical catalogs including those from Lancaster, TCI, ACROS Organics, and Alpha Aesar. In comparison, ChemACX Pro allows you to search over 300 catalogs, including those and many more."
end if
%>
</p>
<p>

In addition to the simple textual search above, ChemACX is <strong>structure searchable</strong>: you may draw your desired structure or seach by name, synonym, formula, or several other fields using the <a href="<%=Request.ServerVariables("SERVER_NAME")%>/chemacx/inputtoggle.asp?formgroup=base_form_group&dbname=chemacx">advanced substructure query form</a>.
</p>
<p>
As you browse, you may add items to your virtual <a href="shop" onclick="ViewCart(); return false"><img src="<%=Application("NavButtonGifPath")%>shopping_cart_2.gif" width="111" height="16" alt="" border="0"></a>, but this cart is provided as a convenience only. No chemicals are available for purchase directly from ChemACX.Com, and the shopping cart has no ordering capabilities. It is provided simply as a way to gather a list of the items you are interested in. It can then be printed out and delivered to your own purchasing department, for example.  Otherwise, you should contact individual vendors for information on pricing and availability of specific substances.
</p>
</td>
</tr>
</table>



<!--#INCLUDE FILE = "purchasingforexcel.asp"-->

