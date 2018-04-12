
<%
	' Figure out which tab to display based on querystring
	sTab = Request.QueryString("TB")
	if Session("ssTab") = "" then Session("ssTab") = "Compounds"
	If sTab = "" Then sTab = Session("ssTab") 
	Session("ssTab") = sTab
	
	
	' Create TabView Object
	Set TabView = Server.CreateObject("VASPTB.ASPTabView")

	TabView.Class = "TabView"
	TabView.ImagePath = "../images/tabview"
	TabView.BackColor = "#d3d3d3"
	TabView.SelectedBackColor = "#adadad"
	TabView.SelectedForeColor = ""
	TabView.SelectedBold = False
	TabView.BodyBackground = "#ffffff"
	TabView.TabWidth = 0
	TabView.StartTab = sTab
	TabView.QueryString = ""
	TabView.LicenseKey = "7993-3E51-698B"

	' Add a Tab to the TabView component.
	' Syntax: TABVIEW.TABS.ADD([Key],[Text],[URL],[Target],[ToolTipText])
	' All arguments in the Add method are optional.
	' If the [URL] property is empty it will refresh the same page
	' passing in the QueryString TB=[Key] EG: my.asp?TB=TAB1

	Set Tabx = TabView.Tabs.Add("Compounds","Compound Search","","","Search results return compounds")
	Tabx.DHTML = "onclick=""SwitchTab('" & Tabx.Key & "', 'base_form_group'); return false"" onmouseover=javascript:this.style.cursor='hand' onmouseout=javascript:this.style.cursor='default'"
	
	Set Tabx = TabView.Tabs.Add("Batches","Batch Search","","","Search results return batches")
	Tabx.DHTML = "onclick=""SwitchTab('" & Tabx.Key & "', 'batches_form_group'); return false"" onmouseover=javascript:this.style.cursor='hand' onmouseout=javascript:this.style.cursor='default'"

	' Write out the HTML for the tabs
	TabView.Show
	
	' Clean up the TabView object
	TabView.Unload
	Set TabView = Nothing
%>

