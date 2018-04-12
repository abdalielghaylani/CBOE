<%
Dim bDebugPrint 
bDebugPrint = true

function AllowAnonymousSearch(pSiteName, pNoOfSearchesAllowed)
SearchCountCookie = "SearchCount" & pSiteName 	'Will be the name of the search counting cookie
SearchCount = GetSearchCount(SearchCountCookie)	'calls for the number of searches
	If SearchCount < pNoOfSearchesAllowed then
		if bDebugPrint then Response.Write "Below maxcount= True"  & "<BR>"
		AllowAnonymousSearch = True
	Else  
		if bDebugPrint then Response.Write "Below maxcount= False"  & "<BR>"
		AllowAnonymousSearch = False
	End If
End function

'**********************************************************************************************
'This function checks whether a counter cookie exists for this site and
'then creates it or adds 1 to the value
function GetSearchCount(CookieName)
	SearchCookieContent = Request.Cookies(CookieName)
	If Len(SearchCookieContent) > 0 then 
		GetSearchCount = SearchCookieContent + 1
	Else
		GetSearchCount = 1
	End If
	if bDebugPrint then Response.Write "Free search counter= " & GetSearchCount & "<BR>"
	Response.Cookies(CookieName) = GetSearchCount
	Response.Cookies(CookieName).domain = "cambridgesoft.com"
	Response.Cookies(CookieName).path = "/"
	Response.Cookies(CookieName).expires = DateAdd("d", 365, Now())
End function
%>

