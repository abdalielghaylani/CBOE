<%
	' Figure out which tab to display based on querystring
	sTab = Request.QueryString("TB")
	
	' Set the "Summary" tab as the default 
	' I also save the last tab in session
	if Session("sTab") = "" then Session("sTab") = "Summary"
	If sTab = "" Then sTab = Session("sTab") 
	Session("sTab") = sTab
	
	
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

	Set Tabx = TabView.Tabs.Add("Summary","Summary","","","Container summary")
	Tabx.DHTML = "onmouseover=javascript:this.style.cursor='hand' onmouseout=javascript:this.style.cursor='default'"

if RegID <> "" then
	' this is a reg substance. Show details from ChemReg.
	if sTab = "Substance" then sTab = "RegSubstance"
	Set Tabx = TabView.Tabs.Add("RegSubstance","Reg Substance","","","Registry Substance details")
	Tabx.DHTML = "onmouseover=javascript:this.style.cursor='hand' onmouseout=javascript:this.style.cursor='default'"
else
	'this is an inventory substance.  Show details form inv_compounds table	
	if sTab = "RegSubstance" then sTab = "Substance"
	Set Tabx = TabView.Tabs.Add("Substance","Substance","","","Substance details")
	Tabx.DHTML = "onmouseover=javascript:this.style.cursor='hand' onmouseout=javascript:this.style.cursor='default'"
End if

	Set Tabx = TabView.Tabs.Add("Supplier","Supplier","","","Supplier details")
	Tabx.DHTML = "onmouseover=javascript:this.style.cursor='hand' onmouseout=javascript:this.style.cursor='default'"

	Set Tabx = TabView.Tabs.Add("Quantities","Quantities","","","Container quantities")
	Tabx.DHTML = "onmouseover=javascript:this.style.cursor='hand' onmouseout=javascript:this.style.cursor='default'"

	Set Tabx = TabView.Tabs.Add("Comments","Comments","","","Container comments")
	Tabx.DHTML = "onmouseover=javascript:this.style.cursor='hand' onmouseout=javascript:this.style.cursor='default'"

	Set Tabx = TabView.Tabs.Add("Reservations","Reservations","","","View reservations placed on this container")
	Tabx.DHTML = "onmouseover=javascript:this.style.cursor='hand' onmouseout=javascript:this.style.cursor='default'"


	numSampleRequests  = 0
	if Application("ShowRequestSample") then
		GetInvConnection()
		SQL = "SELECT count(*) as numSampleRequests FROM inv_requests WHERE request_type_id_fk = 2 AND container_id_fk = ?"
		Set Cmd = GetCommand(Conn, SQL, adCmdText)
		Cmd.Parameters.Append Cmd.CreateParameter("ContainerID", 5, 1, 0, ContainerID)
		Set rsRequest = Cmd.Execute
		if not rsRequest.BOF or rsRequest.EOF then
			numSampleRequests = cint(rsRequest("numSampleRequests"))
		end if
	end if
	if RequestID <> "" or numSampleRequests > 0 then
		Set Tabx = TabView.Tabs.Add("Requests","Requests","","","View requests placed on this container")
		Tabx.DHTML = "onmouseover=javascript:this.style.cursor='hand' onmouseout=javascript:this.style.cursor='default'"
	end if

if NOT lcase(Application("HideOtherTab")) = "true" then
	tabName = Application("OtherTabText")
	TabHelp = Application("OtherTabHelpText")
	Set Tabx = TabView.Tabs.Add(tabName,tabName,"","",TabHelp)
	Tabx.DHTML = "onmouseover=javascript:this.style.cursor='hand' onmouseout=javascript:this.style.cursor='default'"
end if

%>
<!--#INCLUDE VIRTUAL = "/cheminv/custom/gui/custom_container_tabs.asp"-->

<%


	' Write out the HTML for the tabs
	TabView.Show
	
	' Clean up the TabView object
	TabView.Unload
	Set TabView = Nothing
%>

