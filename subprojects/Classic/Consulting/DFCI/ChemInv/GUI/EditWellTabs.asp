<%
if Session("sTab4") = "" then Session("sTab4") = "Required"
If sTab = "" Then sTab = Session("sTab4") 
Session("sTab4") = sTab


' Uses the TabView control to set up the tabs for creating or editing a container
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

Set Tabx = TabView.Tabs.Add("Required","Required","","","Required")
Tabx.ForceDHTML = True
Tabx.DHTML = "onclick=postDataFunction('" & Tabx.Key & "') onmouseover=javascript:this.style.cursor='hand' onmouseout=javascript:this.style.cursor='default'"

if RegID <> "" then
	' this is a reg substance. Show details from ChemReg.
	Set Tabx = TabView.Tabs.Add("RegSubstance","Reg Substance","","","Registry Substance details")
	Tabx.ForceDHTML = True
	Tabx.DHTML = "onclick=postDataFunction('" & Tabx.Key & "') onmouseover=javascript:this.style.cursor='hand' onmouseout=javascript:this.style.cursor='default'"
else
	Set Tabx = TabView.Tabs.Add("Substance","&nbsp;Substance","","","Substance")
	Tabx.ForceDHTML = True
	Tabx.DHTML = "onclick=postDataFunction('" & Tabx.Key & "') onmouseover=javascript:this.style.cursor='hand' onmouseout=javascript:this.style.cursor='default'"
end if

'if NOT lcase(Application("HideOtherTab")) = "true" then
'	tabName = Application("OtherTabText")
'	TabHelp = Application("OtherTabHelpText")
'	Set Tabx = TabView.Tabs.Add(tabName,tabName,"","",TabHelp)
'	Tabx.ForceDHTML = True
'	Tabx.DHTML = "onclick=postDataFunction('" & Tabx.Key & "') onmouseover=javascript:this.style.cursor='hand' onmouseout=javascript:this.style.cursor='default'"
'End if

TabView.Show
TabView.Unload
Set TabView = Nothing

%>
