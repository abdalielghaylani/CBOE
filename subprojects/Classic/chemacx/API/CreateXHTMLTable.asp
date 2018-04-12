<%@ EnableSessionState=False language="vbscript" %>

<!--#INCLUDE VIRTUAL = "/ChemACX/api/acxxml_func_vbs.asp"-->
<!--#INCLUDE VIRTUAL = "/chemacx/api/apiutils.asp"-->
<%
Response.Expires = -1
Response.AddHeader "Pragma", "no-cache"

Dim DBMS
csUserName = request("CSUserName")
csUserID = request("CSUserID")
numAcross = request("numAcross")
ListID = request("ListID")
CDXMLTemplateFile = request("CDXMLTemplateFile")
XHTLTemplateFile = request("XHTLTemplateFile")
action = Request("action")
responseType = request("responseType")
csUID = Request.Cookies("CS_UIDchemacx" )
hAlign = Request("hAlign")
vAlign = Request("vAlign")
borderWidth = Request("borderWidth")

if responseType = "" then responseType = "text/xml" 

if action = "create" then

	if ListID = "" then
		response.write "Error: ListID is a required parameter"
		response.end
	End if
	if numAcross = "" then numAcross = 3
	
	if XHTLTemplateFile = "" then XHTLTemplateFile = "simple.xml"

	sStructType = LCase(request("structType"))

	if NOT (sStructType = "cdx" OR sStructType = "smiles" OR sStructType= "base64cdx" OR sStructType="mol") then sStructType = "mol"  

	Select Case sStructType
		Case "cdx"
			mimeType = "chemical/x-cdx"
		Case "smiles"
			mimeType = "chemical/x-smiles"
		Case "base64cdx"
			mimeType = "chemical/x-cdx"
		Case "mol"
			mimeType = "chemical/x-mol"
	End Select

	if CDXMLTemplateFile <> "" then
		'Read the ChemDraw Settings into a DOM
		dim cdxmlDoc
		set cdxmlDoc = Server.CreateObject("Msxml2.DOMDocument")
		cdxmlDoc.async = false
		if cdxmlDoc.load(Server.MapPath("/chemacx/api") & "\B64CDX\" & CDXMLTemplateFile) then
			Set elemCDTemplate = cdxmlDoc.documentElement
		else
			Response.Write "XML parsing error while reading " & CDXMLTemplateFile & "<BR>"
			Response.Write cdxmlDoc.parseError.reason
			Response.End
		end if	
	else
		Set elemCDTemplate = Nothing
	End if
	
	'Read the Word html template into a DOM
	dim myDOM
	set myDOM = Server.CreateObject("Msxml2.DOMDocument")
	myDOM.async = false
	if myDOM.load(Server.MapPath("/chemacx/api") & "\XHTML\" & XHTLTemplateFile) then
		Set elemBody = myDOM.documentElement.selectSingleNode("body")
		if elemBody is nothing then
			Response.Write "Error:  <body> element not found in " & XHTLTemplateFile
			Response.End 
		End if
	else
		Response.Write "XML parsing error while reading " & XHTLTemplateFile & "<BR>"
		Response.Write myDOM.parseError.reason
		Response.End
	End if
	'Open the ADO Connection 
	Set AdoConn = Server.CreateObject("ADODB.Connection")
	
	'AdoConn.Open GetACXConStr() 
	
	AdoConn.Open "file name=c:\inetpub\wwwroot\chemoffice\chemacx\config\mol.udl"
	

	GenerateStructureTable AdoConn, ListID, sStructType 

	'Kill the ADO connection
	Set AdoConn= Nothing
	
	Response.ContentType = responseType
	
	'Write the XML contents from the parser to the browser
	myDom.save(Response)
	Response.end
End if

%>



<html>
<head>
<title>Create XHTML Structure Table from Saved ChemACX Hitlist</title>
<SCRIPT LANGUAGE=javascript src="/chemacx/Choosecss.js"></SCRIPT>
</head>
<body>
<center><h2>Create XHTML Structure Table from Saved ChemACX Hitlist</h2></center>
	<BR><BR>
	<table>
	<tr>
	<td width=20>
	</td>
	<td>
	<form method="POST" >
		<input type="hidden" name="action" value="create">
		<input type="hidden" name="csusername" value="invadmin">
		<input type="hidden" name="csuserid" value="invadmin">
	
	Hitlist Name:<Br>
		<%GetHitListNames()%>
	<!---Structure Type:<Br>
		<select name="structType">
			<option value="mol">MolFile 
			<option value="base64cdx">Base64 CDX
			<option value="cdx" >CDX
		</select><br>--->		
	ChemDraw Settings Template:<Br>
		<%GetFilePickList "/chemacx/api/B64CDX", "CDXMLTemplateFile", true%>
	Word Html Template:<Br>
		<%GetFilePickList "/chemacx/api/XHTML", "XHTLTemplateFile", false%>
	Response MymeType:<Br>
		<select name="responseType">
			<option value="text/xml">Xml Document 
			<option value="application/msword">MS Word Document 
			<option value="text/html">HTML Document
		</select><br><BR>
	Number of structures per row:
		<input type="text" size="2" name="numAcross" value="3"><Br>
	Horizontal Aligment:
		<select name="hAlign">
			<option value="center">Center 
			<option value="left">Left 
			<option value="right">Right
		</select><br>
	Vertical Aligment:
		<select name="vAlign">
			<option value="middle">Middle 
			<option value="top">Top
			<option value="bottom">Bottom
		</select><br>
	Border Width:
		<select name="borderWidth">
			<option value="0"> 
			<option value="1">1
			<option value="2">2
		</select><br><BR>		
		<br><br>
		<input type="submit" value="Submit">
	</form>
	</td>
	</tr>
	</table>
</body>
</html>





<SCRIPT LANGUAGE=vbscript RUNAT=Server>
'-------------------------------------------------------------------------------
' Purpose: Creates a Patent source html document for a given user hitlist 
' Inputs: ADO connection, Name of the user hitlist, desired structure type (cdx|base64cdx|smiles|mol) 
' Returns: Nothing
'-------------------------------------------------------------------------------
Sub GenerateStructureTable(byRef AdoConnection, byVal sListID, byVal sStructType)
	if sListID <> "" then
		If Application("BASE_DBMS") = "ORACLE" then
			'TODO: implement Oracle case
			'SQLQuery = "SELECT DISTINCT ACX_ID AS ACXnum, CsNum, CAS AS casNum FROM Substance WHERE CsNum IN(" & BuildInClause(sValueList) & ")"
			'SQLQuery_Parameters = "InClause" & "|" & adVarChar & "|" & adParamInput & "|" & "|" & sValueList 'Name|Type|Direction|Size|Value
			SQLQuery = "SELECT l.ID AS csNum, (SELECT base64_cdx from substance where csNum = l.ID) FROM userhilist l, userhitlistid lid WHERE l.hitlistid = lid.id and lid.id =" & sListID & " AND lid.user_id ='" & csUID & "'"
			SQLQuery_Parameters = ""	
		Else
			SQLQuery = "SELECT l.ID AS csNum, mol FROM substance s, userhitlist l, userhitlistid lid WHERE s.csnum = l.id and l.hitlistid = lid.id and lid.id =" & sListID 
			'SQLQuery = "SELECT l.ID AS csNum FROM userhitlist l, userhitlistid lid WHERE l.hitlistid = lid.id and lid.id =" & sListID & " AND lid.user_id ='" & csUID & "'" 
			SQLQuery_Parameters = ""
		End if
			
'Response.Write SQLQuery
'Response.end	 

		'Set SubstanceRS = GetRecordSet(SQLQuery, SQLQuery_Parameters)
		
		Set SubstanceRS = AdoConn.execute(SQLQuery)

'response.write SubstanceRS("mol")
'response.end

		Set elemTable = AddElementToParent(myDOM, elemBody, "table", "", True)
		AddAttributeToElement myDOM, elemTable, "border", borderWidth
		if not elemCDTemplate is nothing  then
			elemBody.appendChild elemCDTemplate
		end if
		if NOT (SubstanceRS.EOF AND SubstanceRS.BOF) then
			i = 0
			
			While Not SubstanceRS.EOF
				sid = SubstanceRS("CsNum")
				if i mod numAcross = 0 then 
					Set elemTR = AddElementToParent(myDOM, elemTable, "tr", "", True)
					AddAttributeToElement myDOM, elemTR, "valign", vAlign
				end if
				Set elemTD = AddElementToParent(myDOM, elemTR, "td", "", True)
				AddAttributeToElement myDOM, elemTD, "align", hAlign
				
				'Set elemSubstance = AddElementToParent(myDOM, elemTD, "substance", "", True)
				'AddAttributeToElement myDOM, elemSubstance, "id", SubstanceRS("csNum")
				'if not elemCdxml is nothing  then AddAttributeToElement myDOM,elemSubstance,"templateID","1"
				bstructure = 1
			
				'if sStructType = "cdx" OR sStructType = "smiles" OR sStructType = "base64cdx" OR sStructType = "mol" then
					
					'Set elemStructure = AddElementToParent(myDOM,elemSubstance,"structure","", True)
					'AddAttributeToElement myDOM, elemStructure, "mimeType", MimeType
					
					'if sStructType = "cdx" then	
					'	sStructURL ="http://" & Request.ServerVariables("HTTP_HOST") & "/ChemACX/chemacx/chemacx_action.asp?dbname=chemacx&dataaction=get_structure&formgroup=base_form_group&Table=substance&Field=structure&DisplayType=" & sStructType & "&StrucID=" & SubstanceRS("csNum") 
					'	AddAttributeToElement myDOM, elemStructure, "src", sStructURL
					'Elseif sStructType = "base64cdx" OR sStructType = "mol" OR sStructType = "smiles" then
					'	sType = sStructType
					'	if sStructType = "base64cdx" then 	sType = "rawbase64cdx" 
					'	
					'	credentials = "CSUserName=" & CSuserName & "&CSUserId="& CSUserid
					'	strucdata = CShttpRequest2("GET", Request.ServerVariables("HTTP_HOST"), "/ChemACX/chemacx/chemacx_action.asp?dbname=chemacx&dataaction=get_structure&formgroup=base_form_group&Table=substance&Field=structure&DisplayType=" & sType & "&StrucID=" & SubstanceRS("CsNum"), "ChemACX", credentials)
					'	if sType = "mol" then strucdata = replace(strucdata, vbcrlf, "\n")
							
					'	Set elemstrucdata = AddElementToParent(myDOM, elemStructure, "strucdata", strucdata, False)
					'End if
				'End if
				
				sType = sStructType	
				credentials = "CSUserName=" & CSuserName & "&CSUserId="& CSUserid
				'strucdata = CShttpRequest2("GET", Request.ServerVariables("HTTP_HOST"), "/ChemACX/chemacx/chemacx_action.asp?dbname=chemacx&dataaction=get_structure&formgroup=base_form_group&Table=substance&Field=structure&DisplayType=" & sType & "&StrucID=" & SubstanceRS("CsNum"), "ChemACX", credentials)
				strucdata = SubstanceRS("mol").value
				strucdata = replace(strucdata, vbcrlf, "\n")
				
				'strucdata = mid(strucdata,1,len(strucdata)-1)
				
				strucdata2 = "> <molid>\n" & sid & "\n\n"
				strucdata2 = strucdata2 & "$$$$\n"
				
				
				sddata = strucdata & strucdata2
				
				Set elemSubstance = AddElementToParent(myDOM, elemTD, "substance", "", true)
				AddAttributeToElement myDOM, elemSubstance, "id", sid
				Set elemP = AddElementToParent(myDOM, elemSubstance, "pre", sddata, false)
				
				i = i +1
				SubstanceRS.MoveNext
			Wend
			while (i mod numAcross)<>0	
				i = i + 1
				Set elemTD = AddElementToParent(myDOM, elemTR, "td", "", True)
			wend  
			Set elemSubstance = Nothing
			'Set elemStructure = Nothing
			'Set elemBase64cdx = Nothing
		end if
		Set SubstanceRS = Nothing
	End if
End Sub

Sub GetFilePickList(url,elm, bAddEmpty)
	Set fso = Server.CreateObject("Scripting.FileSystemObject")
	Set f = fso.GetFolder(server.MapPath(url))
	Set files = f.files
	Response.Write "<select name=""" & elm & """>" & vbcrlf
	if bAddEmpty then Response.Write "<option value="""">" 
	For each file in files
		Response.Write "<option value=""" & file.name & """>" & file.name & vbcrlf
	Next
	Response.Write "</select><br>" & vbcrlf
End sub

Sub GetHitListNames()
	'Open the ADO Connection 
	Set Conn = Server.CreateObject("ADODB.Connection")
	Conn.Open GetACXConStr()
	Set RS = Conn.Execute("select name as listName, id from userhitlistid lid where lid.user_id ='" & csUID & "' order by name") 
	Response.Write "<select name=""ListID"">" & vbcrlf
	While Not RS.EOF
		Response.Write "<option value=""" & RS("id") & """>" & RS("listName") & vbcrlf
		RS.MoveNext
	Wend
	Response.Write "</select><br>" & vbcrlf
End sub

</SCRIPT>


