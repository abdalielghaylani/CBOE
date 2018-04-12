<%
'***********************************inline settings***********************************
Dim conn
Dim cmd
Dim connStr
Dim DBMS

' Connection String
connection_array = Application("base_connection" & Application("appkey"))
connStr =  GetACXConStr()

' Distinguish Oracle and Access DBMS
DBMS = connection_array(6)

Set conn = Server.CreateObject("ADODB.Connection")
conn.ConnectionString = connStr
conn.Open
if err then 
	Response.Write "MsdxGet.asp connection error:" & err.Description
	Response.End
end if

Set cmd = Server.CreateObject("ADODB.Command")
cmd.ActiveConnection = conn

'MSDX Logo html path
MSDXLogo = "logo_msdxdb_265.gif"
'View msds button path
MSDXLinkImg = "viewmsds_60_17_btn.gif"



'***********************************functions***********************************

'Gets the MSDX PDF from an MSDXID
function GetMSDXBlob(ByVal rIDAutoNumber)
	
	
	Dim rs
	Dim sql
	matchlist = ""
	whereclause = ""   
	' Recordset Object
		Set rs = Server.CreateObject("ADODB.Recordset")
	    Select Case DBMS
			Case "ORACLE"
				sql = "select MSDXBlob from CHEMACXDB.MSDX where "
			Case else
				sql = "select MSDXBlob from MSDX where " 	
		End Select
		
	    AddWhereClausePredicate whereclause, "MSDX.IDAutoNumber", 131, 0, rIDAutoNumber
		
		cmd.ActiveConnection = conn
		cmd.CommandText = sql & whereclause
		
		rs.Open cmd
		'CSBR-151546
		'Change made to give a message to user to install MSDS package if it is not existing  
		if  rs.EOF  then
			If inStr(Err.description,"ORA-00942") > 0 then
				Response.Write "<h2>"
				Response.Write "Please check whether MSDS package is installed and retry" 
				Response.Write "</h2>"
				Response.End	
			End if
		Elseif not (rs.EOF and rs.BOF) then 
			msdxblob = rs("MSDXBlob").value						
		End if
		rs.close
		ClearParameters()
	GetMSDXBlob = msdxblob
	
end function

function SearchSupplierCAS(ByVal rSupplierID, ByVal rCASNumber)
	if isNumeric(rSupplierID) then
		AddWhereClausePredicate whereclause, "MSDX.VendorID", 131, 0, rSupplierID
	end if
	if rCASNumber <> "true" then
		AddWhereClausePredicate whereclause, "MSDX.CAS", 200, len(rCASNumber), rCASNumber
	end if
	SearchSupplierCAS = GetMSDXID(whereclause)

end function

function SearchSupplierCatalog(ByVal rSupplierID, ByVal rCatalogNumber)	
	if isNumeric(rSupplierID) then
		AddWhereClausePredicate whereclause, "MSDX.VendorID", 131, 0, rSupplierID
	end if	
	if rCatalogNumber <> "true" then
		AddWhereClausePredicate whereclause, "MSDX.CatalogNum", 200, len(rCatalogNumber), rCatalogNumber
	end if	
	SearchSupplierCatalog = GetMSDXID(whereclause)
end function

function SearchCAS(ByVal rCASNumber)
	if rCASNumber <> "true" then
		AddWhereClausePredicate whereclause, "MSDX.CAS", 200, len(rCASNumber), rCASNumber
	end if
	SearchCAS = GetMSDXID(whereclause)

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



'Gets MSDXID/MSDXID List based on either a SupplierID/CatalogNumber Combo 
'or a CASNumber
'When all are passed the SupplierID/CatalogNumber take precedence
'If SupplierID/CatalogNumber returns no records and there is a CAS Number then
'a CAS Number search is performed
function GetMSDXID(ByVal whereclause)
	
	Dim rs
	Dim sql
	matchlist = ""		    
       
	' Recordset Object
		Set rs = Server.CreateObject("ADODB.Recordset")
	    Select Case DBMS
			Case "ORACLE"
				sql = "SELECT IDAutoNumber FROM CHEMACXDB.MSDX MSDX, Supplier WHERE supplier.supplierid = MSDX.vendorid(+) AND " & whereclause
		
			Case else
				sql = "SELECT IDAutoNumber FROM MSDX LEFT OUTER JOIN Supplier ON supplier.supplierid = MSDX.vendorid where " & whereclause
		
		End Select	
		sql = sql & " ORDER BY Supplier.SupplierType, Supplier.Name"
		
		'Response.Write sql & "<BR>"
		'for each p in cmd.Parameters
			'Response.Write p.Name & "=" & p.Value & "<BR>"
		'next
		cmd.ActiveConnection = conn
		cmd.commandText = sql
		On Error Resume Next
		Set rs = cmd.Execute
		'CSBR-151546
		'Change made to give a message to user to install MSDS package if it is not existing  
		if rs.EOF then
			If inStr(Err.description,"ORA-00942") > 0 then
				Response.Write "<h2>"
				Response.Write "Please check whether MSDS package is installed and retry" 
				Response.Write "</h2>"
				Response.End	
			End if
		Elseif not (rs.EOF and rs.BOF) then
			matchlist = rs.GetString(2,,,",","")					
		End if		
				
		rs.close
		ClearParameters()
		
		matchlist = trim(matchlist)
		If right(matchlist,1) = "," then
			matchlist  = mid(matchlist,1,len(matchlist)-1)
		End If
		
	GetMSDXID = matchlist
	
end function

function GetSearchType(ByVal rCASNumber, ByVal rSupplierID, ByVal rCatalogNumber, ByVal rMSDXID)
	
	rCASNumber = trim(rCASNumber)
	rSupplierID = trim(rSupplierID)
	rCatalogNumber = trim(rCatalogNumber)
	rMSDXID = trim(rMSDXID)	
	
	If rMSDXID <> "" then
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
	Response.Write "<span style=""color:FF3333"">" & rListLength & "</span>"
	Response.Write  " matches for CAS "
	Response.Write "<span style=""color:FF3333"">" & rMatching & "</span>"
	Response.Write "</H1>"
End Sub


'Displays a blob that is pdf
Sub DisplayPDFMSDXBlob(ByVal rMSDXBlob)
	
	Response.ContentType = "application/pdf"
	Response.BinaryWrite rMSDXBlob

End Sub

'takes a list and determines output
Sub DisplayMSDXList(ByVal rMSDXIDList, ByVal rDisplayType)

	Dim msdxlist
	
	If rDisplayType = "simplehtml" then
		Call GetMSDXInfoHTML(rMSDXIDList)
	Elseif rDisplayType = "xml" then
		Call GetMSDXInfoXML(rMSDXIDList)	
	End If
	
End Sub


'Returns the list in simple HTML for DisplayMSDXList Sub
Sub GetMSDXInfoHTML(ByVal rMSDXIDList)
	
	Dim rs
	Set rs = Server.CreateObject("ADODB.Recordset")
	Select Case DBMS
		Case "ORACLE"
			sql = "SELECT MSDX.IDAutonumber, Supplier.Name, Product.ProdName, Product.CatalogNum, Product.ACX_ID, Supplier.SupplierID "
			sql = sql & "FROM CHEMACXDB.MSDX, Supplier, Product WHERE MSDX.VendorID = Supplier.SupplierID AND MSDX.VendorID = Product.SupplierID AND MSDX.CatalogNum = Product.CatalogNum "
			sql = sql & "AND MSDX.IDAutonumber IN (" & rMSDXIDList & ") "
		Case else
			sql = "SELECT MSDX.IDAutonumber, Supplier.Name, Product.ProdName, Product.CatalogNum, Product.ACX_ID, Supplier.SupplierID "
			sql = sql & "FROM (MSDX INNER JOIN Supplier ON MSDX.VendorID = Supplier.SupplierID) LEFT JOIN Product ON (MSDX.VendorID = Product.SupplierID) AND (MSDX.CatalogNum = Product.CatalogNum) "
			sql = sql & "WHERE MSDX.IDAutonumber IN (" & rMSDXIDList & ") "
	End Select
	
	sql = sql & "ORDER BY Supplier.Name, Product.CatalogNum"
	' opening connection
	rs.Open sql, connStr, 2, 4
	
	currentVendorID = ""
	
	Response.Write "<table width=""400"" border=""0"" cellpadding=""3"" cellspacing=""0"">"
	'CSBR-151546
	'Change made to give a message to user to install MSDS package if it is not existing  
	
	if rs.EOF then
		If inStr(Err.description,"ORA-00942") > 0 then
			Response.Write "<h2>"
			Response.Write "Please check whether MSDS package is installed and retry" 
			Response.Write "</h2>"
			Response.End	
		End if
	Elseif not (rs.EOF and rs.BOF) then
	
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
		Response.Write "<a href=""MSDXget.asp?" & maybeKillSession & "MSDXID=" & rs("IDAutonumber").value & """><img src=""" & MSDXLinkImg & """ border=""0""></a>"
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



'Returns the list in XML for DisplayMSDXList Sub
Sub GetMSDXInfoXML(ByVal rMSDXIDList)
	
	Dim rs
	Set rs = Server.CreateObject("ADODB.Recordset")
	Select Case DBMS
		Case "ORACLE"
			sql = "SELECT MSDX.IDAutonumber, Supplier.Name, Product.ProdName, Product.CatalogNum, Product.ACX_ID, Supplier.SupplierID "
			sql = sql & "FROM CHEMACXDB.MSDX, Supplier, Product WHERE MSDX.VendorID = Supplier.SupplierID AND MSDX.VendorID = Product.SupplierID AND MSDX.CatalogNum = Product.CatalogNum "
			sql = sql & "AND MSDX.IDAutonumber IN (" & rMSDXIDList & ") "
		Case else
			sql = "SELECT MSDX.IDAutonumber, Supplier.Name, Product.ProdName, Product.CatalogNum, Product.ACX_ID, Supplier.SupplierID "
			sql = sql & "FROM (MSDX INNER JOIN Supplier ON MSDX.VendorID = Supplier.SupplierID) LEFT JOIN Product ON (MSDX.VendorID = Product.SupplierID) AND (MSDX.CatalogNum = Product.CatalogNum) "
			sql = sql & "WHERE MSDX.IDAutonumber IN (" & rMSDXIDList & ") "
	End Select
	
	sql = sql & "ORDER BY Supplier.Name, Product.CatalogNum"
	
	' opening connection
	rs.Open sql, connStr, 2, 4
	'CSBR-151546
	'Change made to give a message to user to install MSDS package if it is not existing  
	if  rs.EOF then
		If inStr(Err.description,"ORA-00942") > 0 then
			Response.Write "<h2>"
			Response.Write "Please check whether MSDS package is installed and retry" 
			Response.Write "</h2>"
			Response.End	
		End if	
	Elseif not (rs.EOF and rs.BOF) then
		
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
Sub IllegalMSDXCall()
	
	Response.Write "<h2>"
	Response.Write "You have made an illegal call."
	Response.Write "</h2>"
	
	Response.Write "The page MSDXget.asp requires one of the following: " & "<br><br>"
	
	Response.Write "<strong>a) A valid SupplierID / catalogNumber combination</strong>"  & "<br>" 
	Response.Write "<strong>b) A valid CASNumber</strong>" & "<br><br>"

	Response.Write "Providing valid values for all three parameters will first perform a search for the SupplierID / Catalog Number combination."
	Response.Write " If an MSDS is found it will be returned.  If no MSDS is found it will then attempt a CAS Number search."
	
End Sub

'Simple HTML Footer
Sub MSDXFooter()
	Response.Write "<br><br><font size=""-2"">Viewing MSDS data requires Adobe Acrobat. <a href=""http://www.adobe.com/products/acrobat/readstep.html"">Click here</a> to download.</font>"
	Response.Write "</BODY>"
	Response.Write "</HTML>"
End Sub

'HTML Header accepting a page title, turning on and off the logo
'Also includes a style sheet
Sub MSDXHeader(byVal rTitle, byVal rUseLogo)
	
	If rTitle <> "" then
		rTitle = rTitle
	Else
		rTitle = "ChemMSDX"
	End If
					
	Response.Write "<HTML>"
	Response.Write "<HEAD>"
	Response.Write "<TITLE>" + rTitle + "</TITLE>"
	Response.Write "<script language=""javascript"" src=""choosecss.js""></script>"
	Response.Write "<script language=""javascript"">focus()</script>"
	Response.Write "</HEAD>"
	Response.Write "<BODY leftmargin=3 topmargin=3>"

	If rUseLogo then
		Response.write "<img src=""" + MSDXLogo + """ border=""0"">"
		Response.Write "<br clear=""all""><br clear=""all"">"
	End If
	
	
End Sub

'Gives the message for no matches
Sub NoMSDX()
	Response.Write "<h2>"
	Response.Write "There are no MSDX matching your query."
	Response.Write "</h2>"
End Sub

'Takes a supposed blob and displays it in pdf if is there
Sub ShowMSDX(ByVal rMSDXBlob)

	If len(rMSDXBlob) > 0 then
		DisplayPDFMSDXBlob(rMSDXBlob)
	Else
		Call NoMSDX
	End If
	
End Sub

%>
