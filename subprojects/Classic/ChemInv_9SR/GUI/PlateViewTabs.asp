<%
	' Figure out which tab to display based on querystring
	sPlateTab = Request.QueryString("TB")
	
	' Set the "Summary" tab as the default 
	' I also save the last tab in session
	if Session("sPlateTab") = "" then Session("sPlateTab") = "Summary"
	If sPlateTab = "" Then sPlateTab = Session("sPlateTab") 
	Session("sPlateTab") = sPlateTab
	
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
	TabView.StartTab = sPlateTab
	TabView.QueryString = ""
	TabView.LicenseKey = "7993-3E51-698B"

	' Add a Tab to the TabView component.
	' Syntax: TABVIEW.TABS.ADD([Key],[Text],[URL],[Target],[ToolTipText])
	' All arguments in the Add method are optional.
	' If the [URL] property is empty it will refresh the same page
	' passing in the QueryString TB=[Key] EG: my.asp?TB=TAB1

	Set Tabx = TabView.Tabs.Add("Summary","Summary","","","Plate Summary")
	Tabx.DHTML = "onclick=""postDataFunction('" & Tabx.Key & "');return false;"" onmouseover=javascript:this.style.cursor='hand' onmouseout=javascript:this.style.cursor='default'"

	Set Tabx = TabView.Tabs.Add("PlateViewer","Plate Viewer","","","Plate Viewer")
	Tabx.DHTML = "onclick=""postDataFunction('" & Tabx.Key & "');return false;"" onmouseover=javascript:this.style.cursor='hand' onmouseout=javascript:this.style.cursor='default'"

%>
<!--#INCLUDE VIRTUAL = "/cheminv/custom/gui/custom_plate_tabs.asp"-->
<%

	' Write out the HTML for the tabs
	TabView.Show
	
	' Clean up the TabView object
	TabView.Unload
	Set TabView = Nothing
%>

