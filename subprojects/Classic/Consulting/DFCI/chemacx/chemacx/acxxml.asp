
<!--#INCLUDE VIRTUAL = "/chemacx/api/acxxml_func_vbs.asp"-->
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/ado.inc"-->
<!--#INCLUDE FILE = "../API/guiutils.asp"-->

<SCRIPT LANGUAGE=vbscript RUNAT=Server>
	'SYAN modified on 3/31/2004 to support parameterized SQL
	Dim conn
	Dim RS
	
	strucType = Session("Request_FormObjectChemACX")("structype")	
	GetACXConnection()
	'SQL = "SELECT TOP 100 ID FROM CSDOHitlist WHERE HitlistID= " & Session("hitlistIDChemACX")
	SQL = "SELECT TOP 100 ID FROM CSDOHitlist WHERE HitlistID=?" 
	SQL_Paramters = "HitlistID" & "|" & adInteger & "|" & adParamInput & "|" & "|" & Session("hitlistIDChemACX")
	
	'Response.Write sql
	'Response.end
	'Set RS = Conn.execute(sql)
	Set RS = GetRecordSet(SQL, SQL_Parameters)
	
	RS.MoveFirst
	' Make a list from the RS
	sList =  RS.GetString(,,,",")
	
	
	fieldName = "CsNum"
	bCrossRef = 0
	bSynonym = 0
	bProduct = 0
	bProdProperty = 0
	bPackage = 0
	bSupplier = 0
	bSupplierAddress = 0
	bSupplierEmail = 0
	bSupplierURL = 0
	bSupplierPhone = 0
	
	if Session("Request_FormObjectChemACX")("crossRef")= "1" then bCrossRef = 1
	if Session("Request_FormObjectChemACX")("synonym")= "1" then  bSynonym = 1
	if Session("Request_FormObjectChemACX")("product")= "1" then bProduct = 1
	if Session("Request_FormObjectChemACX")("prodProperty")= "1" then bProdProperty = 1
	if Session("Request_FormObjectChemACX")("package")= "1" then bPackage = 1
	if Session("Request_FormObjectChemACX")("supplier")= "1" then bSupplier = 1
	if Session("Request_FormObjectChemACX")("supplierAddress")= "1" then bSupplierAddress = 1
	if Session("Request_FormObjectChemACX")("supplierEmail")= "1" then bSupplierEmail = 1
	if Session("Request_FormObjectChemACX")("supplierURL")= "1" then bSupplierURL = 1
	if Session("Request_FormObjectChemACX")("supplierPhone")= "1" then bSupplierPhone = 1
					
					
	'Open the ADO Connection
	Set AdoConn = CreateAdoConnection()
			
	'Create the parser object
	Set myDOM = CreateObject("Microsoft.XMLDOM")
	myDOM.async = false
	'Create the root document element named <ACXdata>
	Set elemACXdata = myDOM.CreateElement("ACXdata")
	myDOM.appendChild elemACXdata
	
	if sList <> "" then
		GetSubstanceRS AdoConn, fieldName, sList, structype
		if bSynonym then GetSynonymRS AdoConn, fieldName, sList
		if bProduct then 
			GetProductRS AdoConn, fieldName, sList
			if bProdProperty then GetProdPropertyRS AdoConn, fieldName, sList
			if bPackage then GetPackageRS AdoConn, fieldName, sList
		End if
		if bSupplier then
			GetSupplierRS AdoConn, fieldName, sList
			if bSupplierAddress then GetSupplierAddressRS AdoConn, fieldName, sList
			if bSupplierEmail then GetSupplierEmailRS AdoConn, fieldName, sList
			if bSupplierURL then GetSupplierURLRS AdoConn, fieldName, sList
			if bSupplierPhone then GetSupplierPhoneRS AdoConn, fieldName, sList
		End if
	End if
	'Kill the ADO connection
	Set AdoConn= Nothing
	
	'Write the XML contents from the parser to the browser
	Response.ContentType = "text/xml"
	response.write "<?xml version=""1.0""?>"
	response.write "<!DOCTYPE ACXdata SYSTEM ""/chemacx/api/acx.dtd"">"
	response.write myDOM.xml
	response.end
</SCRIPT>

