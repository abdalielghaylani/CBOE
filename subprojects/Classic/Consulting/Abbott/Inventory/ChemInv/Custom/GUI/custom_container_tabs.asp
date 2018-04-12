<%
'  Add additional container tabs here

If Session("INV_EDIT_EHS_DATA" & dbkey) then	
	Set Tabx = TabView.Tabs.Add("EHS","EH&S","","","View EH&S information for this container")
	Tabx.DHTML = "onclick=postDataFunction('" & Tabx.Key & "') onmouseover=javascript:this.style.cursor='hand' onmouseout=javascript:this.style.cursor='default'"
End if

%>