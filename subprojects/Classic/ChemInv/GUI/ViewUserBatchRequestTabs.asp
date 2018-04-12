<%
sTab2 = Request.QueryString("TB")
if Session("sTab2") = "" then Session("sTab2") = "Requests"
If sTab2 = "" Then sTab2 = Session("sTab2") 
Session("sTab2") = sTab2

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
TabView.StartTab = sTab2
TabView.QueryString = ""
TabView.LicenseKey = "7993-3E51-698B"

' Add a Tab to the TabView component.
' Syntax: TABVIEW.TABS.ADD([Key],[Text],[URL],[Target],[ToolTipText])
' All arguments in the Add method are optional.
' If the [URL] property is empty it will refresh the same page
' passing in the QueryString TB=[Key] EG: my.asp?TB=TAB1

Set Tabx = TabView.Tabs.Add("Requests","My&nbsp;Requests","","","User Batch Requests")
Tabx.DHTML = "onmouseover=javascript:this.style.cursor='hand' onmouseout=javascript:this.style.cursor='default'"

Set Tabx = TabView.Tabs.Add("Reservations","My&nbsp;Reservations","","","User Batch Reservations")
Tabx.DHTML = "onmouseover=javascript:this.style.cursor='hand' onmouseout=javascript:this.style.cursor='default'"

TabView.Show
TabView.Unload
Set TabView = Nothing

%>
