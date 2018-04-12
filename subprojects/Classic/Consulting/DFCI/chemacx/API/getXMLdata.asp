<%@  EnableSessionState=False language="vbscript" %>
<!--#INCLUDE VIRTUAL = "/ChemACX/api/acxxml_func_vbs.asp"-->
<!--#INCLUDE VIRTUAL = "/chemacx/api/apiutils.asp"-->
<%
Dim DBMS
csUserName = request("CSUserName")
csUserID = request("CSUserID")

inputlist = request("valueList")
fieldName = request("fieldName")
if inputlist = "" then
response.write "Error: You must enter a list of CsNum, ACXprodID, or ACXpackID values"
response.end
End if
'if fieldName = "ACXprodID" then fieldName = "ProductID"
'if fieldName = "ACXpackID" then fieldName = "PackageID"

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

if request("crossRef")= "1" then bCrossRef = 1
sStructType = LCase(request("structType"))

if NOT (sStructType = "cdx" OR sStructType = "gif" OR sStructType= "base64cdx" OR sStructType="mol") then sStructType = ""  
if request("synonym")= "1" then  bSynonym = 1
if request("product")= "1" then bProduct = 1
if request("prodProperty")= "1" then bProdProperty = 1
if request("package")= "1" then bPackage = 1
if request("supplier")= "1" then bSupplier = 1
if request("supplierAddress")= "1" then bSupplierAddress = 1
if request("supplierEmail")= "1" then bSupplierEmail = 1
if request("supplierURL")= "1" then bSupplierURL = 1
if request("supplierPhone")= "1" then bSupplierPhone = 1

'Open the ADO Connection 
Set AdoConn = Server.CreateObject("ADODB.Connection")
AdoConn.Open GetACXConStr() 


'Create the parser object
Set myDOM = CreateObject("Microsoft.XMLDOM")
myDOM.async = false
'Create the root document element named <ACXdata>
Set elemACXdata = myDOM.CreateElement("ACXdata")
myDOM.appendChild elemACXdata

If bCrossRef OR fieldName = "CAS" OR fieldName = "ACX_ID" then 
	sList = GetCsNumList(AdoConn, fieldName, inputList)
	fieldName = "CsNum"
Else
	sList = inputList
End if

if fieldName <> "" then GetSubstanceRS AdoConn, fieldName, sList, sStructType
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

'Kill the ADO connection
Set AdoConn= Nothing

'Write the XML contents from the parser to the browser
Response.AddHeader "Pragma", "no-cache"
Response.ContentType = "text/xml"
Response.Expires = -1
Response.CharSet = "utf-8"
response.write "<?xml version='1.0'  encoding='UTF-8' ?>"
'response.write "<!DOCTYPE ACXdata SYSTEM ""http://localhost/ChemACX/api/acx.dtd"">"
'response.write myDOM.xml
myDom.save(Response)
%>

