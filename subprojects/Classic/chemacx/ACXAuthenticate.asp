<%
bIsSubscribedCorp = false
bIsSubscribedUser = false
bIsUnsubsribedUser = false

bIsSubscribedCorp = IsBrowserSubscribed(10, "ChemACX", True, 5, True)
If Not bIsSubscribedCorp then bIsSubscribedUser = IsBrowserSubscribed(9, "ChemACX", True, 5, True)
If Not bIsSubscribedUser AND NOT bIsSubscribedCorp then bIsUnsubsribedUser = IsBrowserSubscribed(1, "ChemACX", false, 5, false)

If bIsSubscribedCorp OR bIsSubscribedUser then 
	ShowPayContent = True
	ShowContent = True
Elseif bIsUnsubsribedUser then
	ShowPayContent = False
	ShowContent = True
Else
	ShowContent = False
	ShowPayContent = False
End if
if bDebugPrint then Response.Write "ShowPayContent= " & ShowPayContent & "<BR>"
if bDebugPrint then Response.Write "ShowContent= " & ShowContent & "<BR>"
%>