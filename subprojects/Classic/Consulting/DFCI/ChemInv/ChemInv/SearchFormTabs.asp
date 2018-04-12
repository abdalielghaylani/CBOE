
<%
	' Figure out which tab to display based on querystring
	sTab = Request.QueryString("TB")

	' Set the  default tab 
	if lcase(Request("formgroup")) = "gs_form_group" or lcase(Request("formgroup")) = "gs_np_form_group" then
		Session("ssTab") = "Global"
	elseif lcase(Request("formgroup")) = "batches_form_group" or lcase(Request("formgroup")) = "batches_np_form_group" then
		Session("ssTab") = "Batches"
	End if

	if Session("ssTab") = "" then Session("ssTab") = "Simple"
	If sTab = "" Then sTab = Session("ssTab") 
	Session("ssTab") = sTab
	
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
	TabView.QueryString = ""
	TabView.LicenseKey = "7993-3E51-698B"

	' Add a Tab to the TabView component.
	' Syntax: TABVIEW.TABS.ADD([Key],[Text],[URL],[Target],[ToolTipText])
	' All arguments in the Add method are optional.
	' If the [URL] property is empty it will refresh the same page
	' passing in the QueryString TB=[Key] EG: my.asp?TB=TAB1

	Set Tabx = TabView.Tabs.Add("Simple","Simple Search","","","Search using the most common container attributes")
	Tabx.DHTML = "onclick=""SwitchTab('" & Tabx.Key & "', 'containers_np_form_group'); return false"" onmouseover=javascript:this.style.cursor='hand' onmouseout=javascript:this.style.cursor='default'"
	
	Set Tabx = TabView.Tabs.Add("Advanced","Advanced Search","","","Search using any container attributes")
	Tabx.DHTML = "onclick=""SwitchTab('" & Tabx.Key & "', 'containers_np_form_group'); return false"" onmouseover=javascript:this.style.cursor='hand' onmouseout=javascript:this.style.cursor='default'"

if Session("isCDP") = "TRUE" then
	Set Tabx = TabView.Tabs.Add("Substructure","Substructure Search","","","Substructure search plus any container attribute")
	Tabx.DHTML = "onclick=""SwitchTab('" & Tabx.Key & "', 'base_form_group'); return false"" onmouseover=javascript:this.style.cursor='hand' onmouseout=javascript:this.style.cursor='default'"
end if	

	Set Tabx = TabView.Tabs.Add("Batches","Batch Search","","","Search for substances in ChemInv and ChemReg databases")
	if Session("isCDP") = "TRUE" then
		batchFG = "batches_form_group"
	else
		batchFG = "batches_np_form_group"
	end if
	Tabx.DHTML = "onclick=""SwitchTab('" & Tabx.Key & "', '" & batchFG & "'); return false"" onmouseover=javascript:this.style.cursor='hand' onmouseout=javascript:this.style.cursor='default'"

if NOT ((Application("RegServerName") = "NULL") AND (Application("ACXServerName") = "NULL") ) then
	Set Tabx = TabView.Tabs.Add("Global","Global Search","","","Search for substances in ChemInv, ChemReg and ChemACX databases")
	if Session("isCDP") = "TRUE" then
		globalFG = "gs_form_group"
	else
		globalFG = "gs_np_form_group"
	end if
	Tabx.DHTML = "onclick=""SwitchTab('" & Tabx.Key & "', '" & globalFG & "'); return false"" onmouseover=javascript:this.style.cursor='hand' onmouseout=javascript:this.style.cursor='default'"
End if

if Application("PLATES_ENABLED") then
	Set Tabx = TabView.Tabs.Add("Plate","Plate Search","","","Search Plates")
	if Session("isCDP") = "TRUE" then
		platesFG = "plates_form_group"
	else
		platesFG = "plates_np_form_group"
	end if
	Tabx.DHTML = "onclick=""SwitchTab('" & Tabx.Key & "', '" & platesFG & "'); return false"" onmouseover=javascript:this.style.cursor='hand' onmouseout=javascript:this.style.cursor='default'"
end if
if Application("UseCustomSearchFormTabs") then 
	%>
	<!--#INCLUDE VIRTUAL = "/cheminv/custom/cheminv/custom_searchFormTabs.asp"-->
	<%
End if

	' Write out the HTML for the tabs
	TabView.Show
	
	' Clean up the TabView object
	TabView.Unload
	Set TabView = Nothing
%>

