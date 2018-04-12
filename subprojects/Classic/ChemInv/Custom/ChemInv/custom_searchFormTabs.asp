<%
'  Add additional tabs here


Set Tabx = TabView.Tabs.Add("EHS","Safety","","","Search Environmental Health & Safety attributes")
	Tabx.DHTML = "onclick=""SwitchTab('" & Tabx.Key & "', 'containers_np_form_group'); return false"" onmouseover=javascript:this.style.cursor='hand' onmouseout=javascript:this.style.cursor='default'"


%>