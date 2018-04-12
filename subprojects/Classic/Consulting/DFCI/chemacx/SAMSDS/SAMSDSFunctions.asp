<%
'***********************************inline settings***********************************
Dim conn
Dim cmd
Dim connStr
Dim DBMS

' Connection String
'connection_array = Application("base_connection" & Application("appkey"))
'connStr =  GetACXConStr()
udlpaths = Application("AppPath")
connStr = "FILE NAME=" & udlpaths & "\config\SAMSDS.udl"


' Distinguish Oracle and Access DBMS
'not supproted yet
'DBMS = connection_array(6)
DBMS = "ACCESS"

Set conn = Server.CreateObject("ADODB.Connection")
conn.ConnectionString = connStr
conn.Open
if err then 
	Response.Write "SAMSDSGet.asp connection error:" & err.Description
	Response.End
end if

Set cmd = Server.CreateObject("ADODB.Command")
cmd.ActiveConnection = conn

'SAMSDS Logo html path
SAMSDSLogo = "sigmalogo.gif"
CSLogo = "logo_cambridgesoft_250.gif"
'View msds button path
SAMSDSLinkImg = "viewmsds_60_17_btn.gif"



'***********************************functions***********************************

'Gets the SAMSDS PDF from an SAMSDSID
function GetSAMSDSBlob(ByVal rIDAutoNumber)
	
	
	Dim rs
	Dim sql
	matchlist = ""
	whereclause = ""   
	' Recordset Object
		Set rs = Server.CreateObject("ADODB.Recordset")
	    Select Case DBMS
			Case "ORACLE"
				sql = "select SAMSDSBlob from CHEMACXDB.SAMSDS where "
			Case else
				sql = "select SAMSDSBlob from SAMSDS where " 	
		End Select
		
	    AddWhereClausePredicate whereclause, "SAMSDS.IDAutoNumber", 131, 0, rIDAutoNumber
		
		cmd.ActiveConnection = conn
		cmd.CommandText = sql & whereclause
		
		rs.Open cmd
		
		if not rs.EOF then
			SAMSDSblob = rs("SAMSDSBlob").value			
		end if
		rs.close
		ClearParameters()
	GetSAMSDSBlob = SAMSDSblob
	
end function

'Gets the SAMSDS HTML from an SAMSDSID
function GetSAMSDSHTML(ByVal rIDAutoNumber)
	
	
	Dim rs
	Dim sql
	matchlist = ""
	whereclause = ""   
	' Recordset Object
		Set rs = Server.CreateObject("ADODB.Recordset")
	    Select Case DBMS
			Case "ORACLE"
				'not supported yet
				'sql = "select SAMSDSBlob from CHEMACXDB.SAMSDS where "
			Case else
				'first get the catalognumber, vendor letter and databasecontaining
				sqltranslateid = "select tblMsdsPrime.fldCatNum, tblMsdsPrime.fldCompany, udltranslation.udlfilename from tblMsdsPrime inner join udltranslation on udltranslation.mdbfilename = tblMsdsPrime.fldFileName where "
				
				'this will get the msds
				sql = "select fldMSDS from tblMSDS where " 	
		End Select
		
	    AddWhereClausePredicate whereclause, "tblMsdsPrime.IDAutoNumber", 131, 0, rIDAutoNumber

		cmd.ActiveConnection = conn
		cmd.CommandText = sqltranslateid & whereclause
		
		rs.Open cmd
		
		if not rs.EOF then
			rCatNum = rs("fldCatNum")
			rCompany = rs("fldCompany")
			rDB = rs("udlfilename")			
		end if
		
		rs.close
		ClearParameters()
		whereclause = ""
		
		'Need a different connection
		Set connhtml = Server.CreateObject("ADODB.Connection")
		'connhtml.ConnectionString = "Driver={Microsoft Access Driver (*.mdb)};" & _ 
		'							"Dbq=" & dbpath & rDB & ";"
		connhtml.ConnectionString = "FILE NAME=" & udlpaths & "\config\" & rDB

		connhtml.Open

	    AddWhereClausePredicate whereclause, "tblMSDS.fldCatNum", 200, len(rCatNum), rCatNum	    
		AddWhereClausePredicate whereclause, "tblMSDS.fldCompany", 200, len(rCompany), rCompany
				
		cmd.ActiveConnection = connhtml
		cmd.CommandText = sql & whereclause
		
		rs.Open cmd


		
		if not rs.EOF then
			SAMSDSHTML = rs("fldMSDS").value			
		end if
		
		rs.close
		ClearParameters()
		
		connhtml.Close
		Set connhtml = nothing
		
	GetSAMSDSHTML = SAMSDSHTML
	
end function

function SearchSupplierCAS(ByVal rSupplierID, ByVal rCASNumber)
	if isNumeric(rSupplierID) then
		AddWhereClausePredicate whereclause, "SupplierTranslation.ACXVendorID", 131, 0, rSupplierID
	end if
	if rCASNumber <> "true" then
		AddWhereClausePredicate whereclause, "tblMsdsPrime.fldCASNice", 200, len(rCASNumber), rCASNumber
	end if
	SearchSupplierCAS = GetSAMSDSID(whereclause)

end function

function SearchSupplierCatalog(ByVal rSupplierID, ByVal rCatalogNumber)
	if isNumeric(rSupplierID) then
		AddWhereClausePredicate whereclause, "SupplierTranslation.ACXVendorID", 131, 0, rSupplierID
	end if
	if rCatalogNumber <> "true" then
		AddWhereClausePredicate whereclause, "tblMsdsPrime.fldCatNum", 200, len(rCatalogNumber), rCatalogNumber
	end if
	SearchSupplierCatalog = GetSAMSDSID(whereclause)

end function

function SearchCAS(ByVal rCASNumber)
	if rCASNumber <> "true" then
		AddWhereClausePredicate whereclause, "tblMsdsPrime.fldCASNice", 200, len(rCASNumber), rCASNumber
	end if
	SearchCAS = GetSAMSDSID(whereclause)

end function


function SearchExhaustive(rSupplierID, rCatalogNumber, rCASNumber)
	
	Dim tempresult
	
	tempresult = SearchSupplierCatalog(rSupplierID, rCatalogNumber)
	
	If tempresult = "" then
		tempresult = SearchSupplierCAS(rSupplierID, rCASNumber)
	End If
	
	If tempresult = "" then
		tempresult = SearchCAS(rCASNumber)
	End If
	
	SearchExhaustive = tempresult
	
end function

'Builds a parametrized where clause and creates the
'associated parameters
Sub AddWhereClausePredicate(byref clause, Name, dataType, size, value)
	if len(clause) > 0 then clause = clause & "AND "
	clause = clause & Name & "=? "
	 
	Cmd.Parameters.Append Cmd.CreateParameter(Name, dataType, 1, size, value) 
End sub

'Clears all command parameters
'Parameter index gets shuffled down so this needs to be done backwards
Sub ClearParameters()
	for i = cmd.Parameters.count-1 to 0 step -1
		'response.write i & cmd.Parameters(i).Name & "<BR>"
		cmd.Parameters.Delete(i)
	next
End Sub



'Gets SAMSDSID/SAMSDSID List based on either a SupplierID/CatalogNumber Combo 
'or a CASNumber
'When all are passed the SupplierID/CatalogNumber take precedence
'If SupplierID/CatalogNumber returns no records and there is a CAS Number then
'a CAS Number search is performed
function GetSAMSDSID(ByVal whereclause)
	
	Dim rs
	Dim sql
	matchlist = ""		    
       
	' Recordset Object
		Set rs = Server.CreateObject("ADODB.Recordset")
	    Select Case DBMS
			Case "ORACLE"
				'not supported yet
				'sql = "SELECT IDAutoNumber FROM CHEMACXDB.SAMSDS SAMSDS, Supplier WHERE supplier.supplierid = SAMSDS.vendorid(+) AND " & whereclause
		
			Case else
				sql = "SELECT tblMSDSPrime.IDAutoNumber FROM tblMSDSPrime LEFT OUTER JOIN SupplierTranslation ON SupplierTranslation.fldCompany = tblMSDSPrime.fldCompany where " & whereclause
		
		End Select	
		sql = sql & " ORDER BY SupplierTranslation.VendorName"
				
		cmd.commandText = sql
		rs.open cmd
		
		if not (rs.EOF and rs.BOF) then
			matchlist = rs.GetString(2,,,",","")			
		end if
		
		rs.close
		ClearParameters()
		
		matchlist = trim(matchlist)
		If right(matchlist,1) = "," then
			matchlist  = mid(matchlist,1,len(matchlist)-1)
		End If
		
	GetSAMSDSID = matchlist
	
end function

function GetSearchType(ByVal rCASNumber, ByVal rSupplierID, ByVal rCatalogNumber, ByVal rSAMSDSID)
	
	rCASNumber = trim(rCASNumber)
	rSupplierID = trim(rSupplierID)
	rCatalogNumber = trim(rCatalogNumber)
	rSAMSDSID = trim(rSAMSDSID)	
	
	If rSAMSDSID <> "" then
		searchtype = "idsearch"	
	Else
		
		If rSupplierID <> "" and rCatalogNumber <> "" and rCASNumber = "" then
			searchtype = "suppliercatalog"
		Elseif rSupplierID <> "" and rCASNumber <> "" and rCatalogNumber = "" then
			searchtype = "suppliercas"
		Elseif rSupplierID = "" and rCatalogNumber = "" and rCASNumber <> "" then
			searchtype = "cas"
		Elseif rSupplierID <> "" and rCatalogNumber <> "" and rCASNumber <> "" then
			searchtype = "searchexhaustive"
		Else
			searchtype = "illegalcall"	
		End if
		
	End If
	'Response.Write "SearchType= " & searchtype & "<BR>"
	GetSearchType = searchtype
	
end function

'This function takes a string, trunctates to a specified length, and creates a 
'mouseover using a span tag
'This function is from ChemInventory
Function TruncateInSpan(strText, Length, id)
	Dim str
	str = "<span "
	'if the text contents are longer than the desired length 
	'then place the full text contents in the title popup box
	'and truncate the text inside the <span>
	if len(strText) > Length then 
		str = str & "title=""" & strText & """"
		str = str & " id=""" & id &""">"
		str = str & left(strText, Length-3) & "..."
	else
		str = str & "id=""" & id &""" title="""">" & strText
	end if
	str = str & "&nbsp;"
	str = str & "</span>"
	TruncateInSpan = str
End function

'***********************************subs***********************************


'Displays message providing match count and CAS for multiple hits
Sub DisplayMatchMessage(rListLength, rMatching)
	Response.Write "<H2>"
	Response.Write "<span style=""color:000099"">" & rListLength & "</span>"
	Response.Write  " matches for CAS "
	Response.Write "<span style=""color:000099"">" & rMatching & "</span>"
	Response.Write "</H1>"
End Sub


'Displays a blob that is pdf
Sub DisplayPDFSAMSDSBlob(ByVal rSAMSDSBlob)
	
	Response.ContentType = "application/pdf"
	Response.BinaryWrite rSAMSDSBlob

End Sub

'takes a list and determines output
Sub DisplaySAMSDSList(ByVal rSAMSDSIDList, ByVal rDisplayType)

	Dim SAMSDSlist
	
	If rDisplayType = "simplehtml" then
		Call GetSAMSDSInfoHTML(rSAMSDSIDList)
	Elseif rDisplayType = "xml" then
		Call GetSAMSDSInfoXML(rSAMSDSIDList)	
	End If
	
End Sub


'Returns the list in simple HTML for DisplaySAMSDSList Sub
Sub GetSAMSDSInfoHTML(ByVal rSAMSDSIDList)
	
	Dim rs
	Set rs = Server.CreateObject("ADODB.Recordset")
	Select Case DBMS
		Case "ORACLE"
			'not yet supported
			'sql = "SELECT SAMSDS.IDAutonumber, Supplier.Name, Product.ProdName, Product.CatalogNum, Product.ACX_ID, Supplier.SupplierID "
			'sql = sql & "FROM CHEMACXDB.SAMSDS, Supplier, Product WHERE SAMSDS.VendorID = Supplier.SupplierID AND SAMSDS.VendorID = Product.SupplierID AND SAMSDS.CatalogNum = Product.CatalogNum "
			'sql = sql & "AND SAMSDS.IDAutonumber IN (" & rSAMSDSIDList & ") "
		Case else
			'from msdx for reference - notice that acx-id won't exist
			'sql = "SELECT SAMSDS.IDAutonumber, Supplier.Name, Product.ProdName, Product.CatalogNum, Product.ACX_ID, Supplier.SupplierID "
			'sql = sql & "FROM (SAMSDS INNER JOIN Supplier ON SAMSDS.VendorID = Supplier.SupplierID) LEFT JOIN Product ON (SAMSDS.VendorID = Product.SupplierID) AND (SAMSDS.CatalogNum = Product.CatalogNum) "
			'sql = sql & "WHERE SAMSDS.IDAutonumber IN (" & rSAMSDSIDList & ") "
			
			sql = "SELECT tblMsdsPrime.IDAutonumber, SupplierTranslation.VendorName as name, tblMsdsPrime.fldPrimeName as prodname, tblMsdsPrime.fldCatNum as CatalogNum, SupplierTranslation.ACXVendorID as SupplierID "
			sql = sql & "FROM tblMsdsPrime INNER JOIN SupplierTranslation ON tblMsdsPrime.fldCompany = SupplierTranslation.fldCompany "
			sql = sql & "WHERE tblMsdsPrime.IDAutonumber IN (" & rSAMSDSIDList & ") "

	End Select
	
	sql = sql & "ORDER BY SupplierTranslation.VendorName, tblMsdsPrime.fldCatNum"
	'Response.Write sql
	' opening connection
	rs.Open sql, connStr, 2, 4
	
	currentVendorID = ""
	
	Response.Write "<table width=""400"" border=""0"" cellpadding=""3"" cellspacing=""0"">"
	
	if not (rs.EOF and rs.BOF) then
	
	while not rs.EOF
		
		newVendorID = Cstr(rs("SupplierID").value)
		
		Response.Write "<tr>"
		
		If newVendorID <> currentVendorID then
			'added this
			Response.Write "<td colspan=""4"" bgcolor=""#FFFFF0"">"

			Response.Write  "<b>" & rs("name").value & "</b>"
			currentVendorID = newVendorID
			Response.write "</td>"

		End If
		

		Response.Write "</tr>"
		Response.Write "<tr>"
		Response.Write "<td width=""95"">"
		Response.Write "&nbsp;" 
		Response.write "</td>"
		
		
		
		Response.Write "<td width=""60"">"
		Response.Write "<a href=""SAMSDSget.asp?" & maybeKillSession & "SAMSDSID=" & rs("IDAutonumber").value & """><img src=""" & SAMSDSLinkImg & """ border=""0""></a>"
		Response.write "</td>"
		
	
		Response.Write "<td width=""70"">"
		Response.Write rs("CatalogNum").value 
		Response.write "</td>"
	
		Response.Write "<td width=""175"" nowrap>"
		Response.Write TruncateInSpan(rs("ProdName").value, 24, "pname")
		Response.write "<br>"
		Response.Write "</td>"
		

		Response.Write "</tr>"
		rs.movenext
	wend
		
	end if
	
	
	rs.close
	Response.Write "</table>"
	
End Sub



'Returns the list in XML for DisplaySAMSDSList Sub
Sub GetSAMSDSInfoXML(ByVal rSAMSDSIDList)
	
	Dim rs
	Set rs = Server.CreateObject("ADODB.Recordset")
	Select Case DBMS
		Case "ORACLE"
			'not yet supported
			'sql = "SELECT SAMSDS.IDAutonumber, Supplier.Name, Product.ProdName, Product.CatalogNum, Product.ACX_ID, Supplier.SupplierID "
			'sql = sql & "FROM CHEMACXDB.SAMSDS, Supplier, Product WHERE SAMSDS.VendorID = Supplier.SupplierID AND SAMSDS.VendorID = Product.SupplierID AND SAMSDS.CatalogNum = Product.CatalogNum "
			'sql = sql & "AND SAMSDS.IDAutonumber IN (" & rSAMSDSIDList & ") "
		Case else
			sql = "SELECT tblMsdsPrime.IDAutonumber, SupplierTranslation.VendorName as name, tblMsdsPrime.fldPrimeName as prodname, tblMsdsPrime.fldCatNum as CatalogNum, SupplierTranslation.ACXVendorID as SupplierID "
			sql = sql & "FROM tblMsdsPrime INNER JOIN SupplierTranslation ON tblMsdsPrime.fldCompany = SupplierTranslation.fldCompany "
			sql = sql & "WHERE tblMsdsPrime.IDAutonumber IN (" & rSAMSDSIDList & ") "
			
			'old	
			'sql = "SELECT SAMSDS.IDAutonumber, Supplier.Name, Product.ProdName, Product.CatalogNum, Product.ACX_ID, Supplier.SupplierID "
			'sql = sql & "FROM (SAMSDS INNER JOIN Supplier ON SAMSDS.VendorID = Supplier.SupplierID) LEFT JOIN Product ON (SAMSDS.VendorID = Product.SupplierID) AND (SAMSDS.CatalogNum = Product.CatalogNum) "
			'sql = sql & "WHERE SAMSDS.IDAutonumber IN (" & rSAMSDSIDList & ") "
	End Select
	
	sql = sql & "ORDER BY SupplierTranslation.VendorName, tblMsdsPrime.fldCatNum"
	
	' opening connection
	rs.Open sql, connStr, 2, 4
	
	
	if not (rs.EOF and rs.BOF) then
	Response.Write "<?xml version=""1.0"" encoding=""ISO-8859-1""?>"
	Response.Write chr(13) & chr(10)
	
	Response.Write "<MSDSLIST>"
	
	while not rs.EOF
	
		Response.Write chr(13) & chr(10)
		Response.Write "<MSDS idautonumber=""" & rs("IDAutonumber").value & """>"
		
		Response.Write chr(13) & chr(10)
			
		Response.Write "<VENDOR id=""" & rs("SupplierID").value & """ name=""" & rs("name").value & """>"
		Response.Write "</VENDOR>"

		Response.Write chr(13) & chr(10)
		
		Response.Write "<PRODUCT catalognumber=""" & rs("CatalogNum").value  & """ productname=""" &  rs("ProdName").value & """>"
		Response.Write "</PRODUCT>"


		Response.Write chr(13) & chr(10)
		
		Response.Write "</MSDS>"
		rs.movenext
	wend
	
	Response.Write chr(13) & chr(10)

	Response.Write "</MSDSLIST>"
		
	end if
	
	
	rs.close
	
End Sub





'HTML Display of an Illegal Call
Sub IllegalSAMSDSCall()
	
	Response.Write "<h2>"
	Response.Write "You have made an illegal call."
	Response.Write "</h2>"
	
	Response.Write "The page SAMSDSget.asp requires one of the following: " & "<br><br>"
	
	Response.Write "<strong>a) A valid SupplierID / catalogNumber combination</strong>"  & "<br>" 
	Response.Write "<strong>b) A valid CASNumber</strong>" & "<br><br>"

	Response.Write "Providing valid values for all three parameters will first perform a search for the SupplierID / Catalog Number combination."
	Response.Write " If an MSDS is found it will be returned.  If no MSDS is found it will then attempt a CAS Number search."
	
End Sub

'Simple HTML Footer
Sub SAMSDSFooter()
	'Response.Write "<br><br><font size=""-2"">Viewing MSDS data requires Adobe Acrobat. <a href=""http://www.adobe.com/products/acrobat/readstep.html"">Click here</a> to download.</font>"
	Response.Write "</BODY>"
	Response.Write "</HTML>"
End Sub

'HTML Header accepting a page title, turning on and off the logo
'Also includes a style sheet
Sub SAMSDSHeader(byVal rTitle, byVal rUseLogo)
	
	If rTitle <> "" then
		rTitle = rTitle
	Else
		rTitle = "Sigma Aldrich MSDS"
	End If
					
	Response.Write "<HTML>"
	Response.Write "<HEAD>"
	Response.Write "<TITLE>" + rTitle + "</TITLE>"
	Response.Write "<script language=""javascript"" src=""choosecss.js""></script>"
	Response.Write "<script language=""javascript"">focus()</script>"
	Response.Write "</HEAD>"
	Response.Write "<BODY leftmargin=3 topmargin=3>"

	If rUseLogo then
		Response.write "<img src=""" + CSLogo + """ border=""0"">"
		Response.write "<img src=""" + SAMSDSLogo + """ border=""0"">"
		Response.Write "<br clear=""all""><br clear=""all"">"
	End If
	
	
End Sub

'Gives the message for no matches
Sub NoSAMSDS()
	Response.Write "<h2>"
	Response.Write "There are no Sigma Aldrich MSDS matching your query."
	Response.Write "</h2>"
End Sub

'Takes a supposed blob and displays it in pdf if is there
Sub ShowSAMSDS(ByVal SAMSDSHTML)

	If len(SAMSDSHTML) > 0 then
		DisplayMSDSHTML(SAMSDSHTML)
	Else
		Call NoSAMSDS
	End If
	
End Sub

'Displays a blob that is pdf
Sub DisplayMSDSHTML(ByVal rSAMSDSHTML)	

	'Response.ContentType = "application/pdf"
	Response.Write "<pre>"
	Response.write  rSAMSDSHTML
	Response.Write "</pre>"
	

End Sub







%>
