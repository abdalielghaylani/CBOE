<%
if Session("sTab3") = "" or cbool(Request("refresh")) then Session("sTab3") = "Plate"
If sTab = "" Then sTab = Session("sTab3") 
Session("sTab3") = sTab

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

Set Tabx = TabView.Tabs.Add("Plate","Required Plate Attributes","","","Required Plate Attributes")
Tabx.ForceDHTML = True
Tabx.DHTML = "onclick=postDataFunction('" & Tabx.Key & "') onmouseover=javascript:this.style.cursor='hand' onmouseout=javascript:this.style.cursor='default'"

if isEdit then
	Set Tabx = TabView.Tabs.Add("Well","&nbsp;Update Well Attributes","","","Update Well Attributes")
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
