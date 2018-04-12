<%

if Session("sTab2") = "" then Session("sTab2") = "Required"
If sTab = "" Then sTab = Session("sTab2") 
Session("sTab2") = sTab

' Uses the TabView control to set up the tabs for creating or editing a container
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

Set Tabx = TabView.Tabs.Add("Required","Required","","","Required container attributes")
Tabx.ForceDHTML = True
Tabx.DHTML = "onclick=postDataFunction('" & Tabx.Key & "') onmouseover=javascript:this.style.cursor='hand' onmouseout=javascript:this.style.cursor='default'"

'Set Tabx = TabView.Tabs.Add("Substance","Substance","","","Substance details")
'Tabx.ForceDHTML = True
'Tabx.DHTML = "onclick=postDataFunction('" & Tabx.Key & "') onmouseover=javascript:this.style.cursor='hand' onmouseout=javascript:this.style.cursor='default'"

if RegID <> "" then
	'if sTab = "Substance" then sTab = "RegSubstance"
	'CompoundID = ""
	' this is a reg substance. Show details from ChemReg.
	Set Tabx = TabView.Tabs.Add("RegSubstance","Reg Substance","","","Registry Substance details")
	Tabx.ForceDHTML = True
	Tabx.DHTML = "onclick=postDataFunction('" & Tabx.Key & "') onmouseover=javascript:this.style.cursor='hand' onmouseout=javascript:this.style.cursor='default'"
else
	'this is an inventory substance.  Show details form inv_compounds table	
	Set Tabx = TabView.Tabs.Add("Substance","Substance","","","Substance details")
	Tabx.ForceDHTML = True
	Tabx.DHTML = "onclick=postDataFunction('" & Tabx.Key & "') onmouseover=javascript:this.style.cursor='hand' onmouseout=javascript:this.style.cursor='default'"
End if



Set Tabx = TabView.Tabs.Add("Supplier","Supplier","","","Supplier details")
Tabx.ForceDHTML = True
Tabx.DHTML = "onclick=postDataFunction('" & Tabx.Key & "') onmouseover=javascript:this.style.cursor='hand' onmouseout=javascript:this.style.cursor='default'"

Set Tabx = TabView.Tabs.Add("Contents","Contents","","","Contents details")
Tabx.ForceDHTML = True
Tabx.DHTML = "onclick=postDataFunction('" & Tabx.Key & "') onmouseover=javascript:this.style.cursor='hand' onmouseout=javascript:this.style.cursor='default'"

Set Tabx = TabView.Tabs.Add("Optional","Optional","","","Optional container attributes")
Tabx.ForceDHTML = True
Tabx.DHTML = "onclick=postDataFunction('" & Tabx.Key & "') onmouseover=javascript:this.style.cursor='hand' onmouseout=javascript:this.style.cursor='default'"

Set Tabx = TabView.Tabs.Add("Owner","Owner","","","Container users")
Tabx.ForceDHTML = True
Tabx.DHTML = "onclick=postDataFunction('" & Tabx.Key & "') onmouseover=javascript:this.style.cursor='hand' onmouseout=javascript:this.style.cursor='default'"

Set Tabx = TabView.Tabs.Add("Comments","Comments","","","Container comments")
Tabx.ForceDHTML = True
Tabx.DHTML = "onclick=postDataFunction('" & Tabx.Key & "') onmouseover=javascript:this.style.cursor='hand' onmouseout=javascript:this.style.cursor='default'"

if NOT lcase(Application("HideOtherTab")) = "true" then
	tabName = Application("OtherTabText")
	TabHelp = Application("OtherTabHelpText")
	Set Tabx = TabView.Tabs.Add(tabName,tabName,"","",TabHelp)
	Tabx.ForceDHTML = True
	Tabx.DHTML = "onclick=postDataFunction('" & server.URLEncode(Tabx.Key) & "') onmouseover=javascript:this.style.cursor='hand' onmouseout=javascript:this.style.cursor='default'"
End if

TabView.Show
TabView.Unload
Set TabView = Nothing

%>
