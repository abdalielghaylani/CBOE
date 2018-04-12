<!--#INCLUDE virtual = "/chemacx/api/apiutils.asp"-->
<!--#INCLUDE File = "MSDXFunctions.asp"-->

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
	
rMSDXID = Request("MSDXID")
rCASNumber = Request("CASNUmber")
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


searchtype = GetSearchType(rCASNumber, rSupplierID, rCatalogNumber, rMSDXID)

Select Case searchtype
	
	Case "illegalcall" 
		
		If rReturnType = "list" then
			Response.Write "-2"
		Else	
			Call MSDXHeader("Illegal Call", true)
			Call IllegalMSDXCall
			Call MSDXFooter
		End If
		Response.End
	
	Case "idsearch"
		
		MSDXBlob = GetMSDXBlob(rMSDXID)
		Call ShowMSDX(MSDXBlob)
		Response.End
		
	Case "suppliercatalog"
		
		MSDXIDList =  SearchSupplierCatalog(rSupplierID, rCatalogNumber)
	
	Case "suppliercas"
	
		MSDXIDList =  SearchSupplierCAS(rSupplierID, rCASNumber)
		
	Case "cas"
		MSDXIDList = SearchCAS(rCASNumber)
	
	Case "searchexhaustive"
	
		MSDXIDList = SearchExhaustive(rSupplierID, rCatalogNumber, rCASNumber)
		
	Case Else
		
		Response.Write "There was an error processing your request."
		
End Select


'---------------------------------------------------------------------
	MSDXIDList = MSDXIDList
	
	If rReturnType = "list" then
	
		If trim(MSDXIDList) = "" then
			Response.Write "-1"
			Response.end
		Else
			Response.Write MSDXIDList
			Response.End
		End If
	
	End If
	
	
	MSDXIDArray = split(MSDXIDList,",")

	maxcounter = ubound(MSDXIDArray)
	ListLength = maxcounter + 1
	
	'new xml support
	If rReturnType = "xml" then
		
		If ListLength = 0 then
	
			'Call MSDXHeader("MSDS for CAS Number " + rCASNumber, true)
			'Call NoMSDX
			'Call MSDXFooter
			
		Elseif ListLength > 0 then

			'Call MSDXHeader("MSDS for CAS Number " + rCASNumber, true)
			'Call DisplayMatchMessage(ListLength, rCASNumber)
			'Call DisplayMSDXList(MSDXIDList,"simplehtml")
			'Call MSDXFooter
			
			Call DisplayMSDXList(MSDXIDList,"xml")
				
		End If
			
		Response.End
	End If
	'end new xml support


	If ListLength = 0 then
	
		Call MSDXHeader("MSDS for CAS Number " + rCASNumber, true)
		Call NoMSDX
		Call MSDXFooter
		
	Elseif ListLength = 1 then

		MSDXBlob = GetMSDXBlob(MSDXIDArray(0))
	
		Call ShowMSDX(MSDXBlob)
		
	Elseif ListLength > 1 then

		Call MSDXHeader("MSDS for CAS Number " + rCASNumber, true)
		Call DisplayMatchMessage(ListLength, rCASNumber)
		Call DisplayMSDXList(MSDXIDList,"simplehtml")
		Call MSDXFooter
			
	End If



%>