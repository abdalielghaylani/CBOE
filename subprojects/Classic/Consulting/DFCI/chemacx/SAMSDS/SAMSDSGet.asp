<!--#INCLUDE virtual = "/chemacx/api/apiutils.asp"-->
<!--#INCLUDE File = "SAMSDSFunctions.asp"-->

<%
Dim maybeKillSession

Response.Expires = -1
Response.Buffer = True

if request("killsession") then 
	session.Abandon
	maybeKillSession = "killsession=true&"
else
	maybeKillSession = ""
end if
	
rSAMSDSID = Request("SAMSDSID")
rCASNumber = Request("CASNumber")
rSupplierID = Request("SupplierID")
rCatalogNumber = Request("CatalogNumber")

If Request("ReturnType") = "list" then
	rReturnType = "list"
Elseif Request("ReturnType") = "html" then
	rReturnType = "html"
Elseif Request("ReturnType") = "xml" then
	rReturnType = "xml"	
Else
	rReturnType = "html"
End If


searchtype = GetSearchType(rCASNumber, rSupplierID, rCatalogNumber, rSAMSDSID)

Select Case searchtype
	
	Case "illegalcall" 
		
		If rReturnType = "list" then
			Response.Write "-2"
		Else	
			Call SAMSDSHeader("Illegal Call", true)
			Call IllegalSAMSDSCall
			Call SAMSDSFooter
		End If
		Response.End
	
	Case "idsearch"
		
		'SAMSDSBlob = GetSAMSDSBlob(rSAMSDSID)
		'Call ShowSAMSDS(SAMSDSBlob)
		SAMSDSHTML = GetSAMSDSHTML(rSAMSDSID)
		Call SAMSDSHeader("", true)
		Call ShowSAMSDS(SAMSDSHTML)
		Call SAMSDSFooter
		Response.End
		
	Case "suppliercatalog"
		
		SAMSDSIDList =  SearchSupplierCatalog(rSupplierID, rCatalogNumber)
	
	Case "suppliercas"
	
		SAMSDSIDList =  SearchSupplierCAS(rSupplierID, rCASNumber)
		
	Case "cas"
		SAMSDSIDList = SearchCAS(rCASNumber)
	
	Case "searchexhaustive"
	
		SAMSDSIDList = SearchExhaustive(rSupplierID, rCatalogNumber, rCASNumber)
		
	Case Else
		
		Response.Write "There was an error processing your request."
		
End Select


'---------------------------------------------------------------------
	SAMSDSIDList = SAMSDSIDList
	
	If rReturnType = "list" then
	
		If trim(SAMSDSIDList) = "" then
			Response.Write "-1"
			Response.end
		Else
			Response.Write SAMSDSIDList
			Response.End
		End If
	
	End If
	
	
	SAMSDSIDArray = split(SAMSDSIDList,",")

	maxcounter = ubound(SAMSDSIDArray)
	ListLength = maxcounter + 1
	
	'new xml support
	If rReturnType = "xml" then
		
		If ListLength = 0 then
	
			'Call SAMSDSHeader("MSDS for CAS Number " + rCASNumber, true)
			'Call NoSAMSDS
			'Call SAMSDSFooter
			
		Elseif ListLength > 0 then

			
			Call DisplaySAMSDSList(SAMSDSIDList,"xml")
				
		End If
			
		Response.End
	End If
	'end new xml support


	If ListLength = 0 then
	
		Call SAMSDSHeader("MSDS for CAS Number " + rCASNumber, true)
		Call NoSAMSDS
		Call SAMSDSFooter
		
	Elseif ListLength = 1 then

		'SAMSDSBlob = GetSAMSDSBlob(SAMSDSIDArray(0))
		'Call ShowSAMSDS(SAMSDSBlob)
		SAMSDSHTML = GetSAMSDSHTML(SAMSDSIDArray(0))
		Call ShowSAMSDS(SAMSDSHTML)
		
		
	Elseif ListLength > 1 then

		Call SAMSDSHeader("MSDS for CAS Number " + rCASNumber, true)
		Call DisplayMatchMessage(ListLength, rCASNumber)
		Call DisplaySAMSDSList(SAMSDSIDList,"simplehtml")
		Call SAMSDSFooter
			
	End If



%>