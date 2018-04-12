<%
Dim bDebugPrint
Dim ServiceID
Dim NumSearchesAllowed

bDebugPrint = false
ServiceID = Application("CSWebUsers_ServiceID")

if bDebugPrint then Response.Write "ServiceStartTime= " & Session("StartTime") & "<BR>"   

'**********************************************************************************
'Checks to see if user has exceeded allowed number of anonymous searches
function AllowAnonymousSearch(ServiceID, NoOfSearchesAllowed)
Dim SearchCountCookieName
Dim SearchCount
SearchCountCookieName = "SearchCount_" & ServiceID 	'Will be the name of the search counting cookie
SearchCount = GetSearchCount(ServiceID)	'calls for the number of searches
If SearchCount = "" then SearchCount = 0
	If ((CInt(SearchCount) < CInt(NoOfSearchesAllowed)) ) then
		if bDebugPrint then Response.Write  SearchCount & ":" & NoOfSearchesAllowed & "<BR>"
		AllowAnonymousSearch = True
	Else  
		if bDebugPrint then Response.Write  SearchCount & ":" & NoOfSearchesAllowed & "<BR>"
		AllowAnonymousSearch = False
	End If
End function

'*************************************************************************************
'Returns the value of search counter cookie
function GetSearchCount(ServiceID)
	Dim SearchCountCookieName
	SearchCountCookieName = "SearchCount_" & ServiceID 
	GetSearchCount = Request.Cookies(SearchCountCookieName)
	if bDebugPrint then Response.Write "Search counter cookie= " & GetSearchCount & "<BR>"
End function

'**********************************************************************************************
'This sub checks whether a counter cookie exists for a service and
'then creates it or adds 1 to the value
Sub IncrementSearchCount(ServiceID)
	Dim theCount
	Dim SearchCountCookieName
	
	SearchCountCookieName = "SearchCount_" & ServiceID
	SearchCookieContent = GetSearchCount(ServiceID)
	If Len(SearchCookieContent) > 0 then 
			theCount = SearchCookieContent + 1
		Else
			theCount = 1
		End If
		Response.Cookies(SearchCountCookieName) = theCount
		Response.Cookies(SearchCountCookieName).domain = "cambridgesoft.com"
		Response.Cookies(SearchCountCookieName).path = "/"
		Response.Cookies(SearchCountCookieName).expires = DateAdd("d", 365, Now())
End Sub

%>

