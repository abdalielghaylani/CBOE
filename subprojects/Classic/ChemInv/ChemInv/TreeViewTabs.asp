<%
	' Figure out which tab to display based on querystring
	treeTab = Request.QueryString("TB")

	' Set the "Browse" tab as the default 
	' I also save the last tab in session
	if Session("treeTab") = "" then Session("treeTab") = "Browse"
	If treeTab = "" Then treeTab = Session("treeTab") 
	Session("treeTab") = treeTab
	
	' Create TabView Object
	Set TabView = Server.CreateObject("VASPTB.ASPTabView")

	TabView.Class = "TabView"
	TabView.ImagePath = "../images/tabview"
	TabView.BackColor = "#e1e1e1"
	TabView.SelectedBackColor = "#adadad"
	TabView.SelectedForeColor = ""
	TabView.SelectedBold = False
	TabView.BodyBackground = "#ffffff"
	TabView.TabWidth = 0
	TabView.StartTab = sTab
	TabView.QueryString = "ClearNodes=0&TreeID=" & TreeID & "&MaybeLocSearch=" & MaybeLocSearch & "&formelm=" & formelm & "&elm1=" & elm1 & "&elm2=" & elm2 & "&elm3=" & elm3 & "&LocationPickerID=" & LocationPickerID & "&NodeURL=" & NodeURL & "&NodeTarget=" & NodeTarget & "&GotoNode=0" & "&isMultiSelectRacks=" & isMultiSelectRacks
	TabView.LicenseKey = "7993-3E51-698B"

	' Add a Tab to the TabView component.
	' Syntax: TABVIEW.TABS.ADD([Key],[Text],[URL],[Target],[ToolTipText])
	' All arguments in the Add method are optional.
	' If the [URL] property is empty it will refresh the same page
	' passing in the QueryString TB=[Key] EG: my.asp?TB=TAB1

	'Set Tabx = TabView.Tabs.Add("Browse","Browse","BrowseTree.asp?"&Request.QueryString&"&TB=Browse","","Browse the location tree")
	Set Tabx = TabView.Tabs.Add("Browse","Browse","","","Browse the location tree")
	Tabx.DHTML = "onmouseover=javascript:this.style.cursor='hand' onmouseout=javascript:this.style.cursor='default'"

	'Set Tabx = TabView.Tabs.Add("Search","Rack Search","BrowseTree.asp?"&Request.QueryString&"&TB=Search","","Search rack locations")
	'Set Tabx = TabView.Tabs.Add("Search","Rack Search","","","Search rack locations")
	Set Tabx = TabView.Tabs.Add("Search","Rack Search","","","Search rack locations")
	Tabx.DHTML = "onmouseover=javascript:this.style.cursor='hand' onmouseout=javascript:this.style.cursor='default'"

	' Write out the HTML for the tabs
	TabView.Show
	
	' Clean up the TabView object
	TabView.Unload
	Set TabView = Nothing
%>

