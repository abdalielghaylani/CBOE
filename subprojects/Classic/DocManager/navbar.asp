<%@ LANGUAGE="VBScript" %>
<%Response.Expires=0%>
<html>

<head>
<!--#include virtual="/cfserverasp/source/menubar.asp"-->
<%
Dim nv_DEBUG
Dim browser

nv_DEBUG = False
commit_type = Request.QueryString( "commit_type" )
nav_override = Request.QueryString( "nav_override" )
dbkey = Request.QueryString( "dbname" )

Set cnn = Server.CreateObject("ADODB.Connection")
with cnn
	.ConnectionString = Application("cnnStr")
end with
cnn.Open

set countRS = Server.CreateObject("ADODB.Recordset")
sql = "Select Count(*) From docmgr.docmgr_documents"

countRS.Open sql, cnn

%>

<script language="javascript">

	function getCookie( name ) {
		var cname = name + "=";
		var dc = document.cookie;

		if ( 0 < dc.length ) {
			begin = dc.indexOf( cname );
			if ( begin != -1 ) {
				begin += cname.length;
				end = dc.indexOf( ";", begin );
				if ( -1 == end ) {
					end = dc.length;
				}
				temp = unescape( dc.substring( begin, end ) );
				theResult = temp;
				return theResult;
			}
		}
		return null;	
	}


	//standard variables - do not edit
	//MainWindow = <%=Application( "MainWindow" )%>
	//JHS 1/4/2008
	//not having this was causing an error	
	if (!<%=Application("mainwindow")%>.mainFrame){
			MainWindow = <%=Application("mainwindow")%>
	}
	else{
		MainWindow = <%=Application("mainwindow")%>.mainFrame
	}
	
	
	formgroup = MainWindow.formgroup
	formgroupflag = MainWindow.formgroupflag

	uniqueid = MainWindow.uniqueid

	mainpage_enabled = MainWindow.mainpage_enabled
	//alert('<%=Application("Main_Page" & "Drugdeg")%>')
	//alert(MainWindow.mainpage_enabled)
	//alert(mainpage_enabled)
	//this is reg only, but don't remove
	commit_type = MainWindow.commit_type
	//defaults
	noNav = false
	noSupInfo = false
	nav_override = "<%=nav_override%>"

	// Get the formmode.
	if ( nav_override == "true" ) {
		formmode = "<%=request( "formmode" )%>"
		noNav = true
	}
	else {
		formmode = MainWindow.formmode
	}

	// Get the input formmode, formgroup, and formgroupflag.
	formmodeInput = formmode.toLowerCase()
	formgroupInput = formgroup.toLowerCase()
	formgroupflagInput = formgroupflag.toLowerCase()

	// By default, the module (just for use in this file) formmode, formgroup, and
	// formgroupflag are the same as the input versions.
	formmodeModule = formmodeInput
	formgroupModule = formgroupInput
	formgroupflagModule = formgroupflagInput


	// Using the input formmode, determine whether to show navigation buttons and supplementary
	// information, and perhaps change the module formmode and module formgroupflag.
	if ( ( "search" == formmodeModule ) &&
		( "base_form_group" == formgroupModule ) )
	{
		formgroupflagModule = "search_documents"
		noSupInfo = false
		noNav = false
	}
	else if ( ( "list" == formmodeModule ) &&
		( "base_form_group" == formgroupModule ) )
	{
		formgroupflagModule = "document_list"
		noSupInfo = false
		noNav = false
	}
	else if ( "edit" == formmodeModule )
	{
		if ( "base_form_group" == formgroupModule )
		{
			formgroupflagModule = "document_details"
			noSupInfo = false
			noNav = false
		}
	}
	else if ((formmodeModule == "refine")){
			formgroupflagModule = "refine_search"
			noSupInfo = false
			noNav = false
	}
	
	else {
		alert('formmodeModule= ' + formmodeModule)
		alert('formgroupModule= ' + formgroupModule)
		alert('formgroupflagModule= ' + formgroupflagModule)
	}

</script> 


<title>Nav bar</title>
</head>

<body <%=Application("BODY_BACKGROUND")%> bgProperties="fixed">

<table border="0">
	<tr>
		<td>
			<table border="0" bordercolor="fuchsia" cellspacing="0" cellpadding="0">
				<!-- The table for the banner. -->
				<tr>

					<td valign="top" width="300">
						<img src="/docmanager/docmanager/gifs/banner.gif" border="0">
					</td>

					<td width="275">
						<script language="javascript">
 							var imgsrc
							var pathBannerGif = "/<%=Application( "AppKey" )%>/graphics/"
							var dbpath = "/<%=Application( "AppKey" )%>/<%=dbkey%>/"
							
							//JHS addded 4/9/03
							//all banner text stuff in this was added
							var banner_text
							var usebannertext = true
							//JHS added 4/9/03 end
			
							// Showing banner at top of window based on
							// formmodeModule and formgroupflagModule.

							//used
							 if ( "refine" == formmodeModule ){
								if ( "refine_search" == formgroupflagModule ) {
									banner_text = "Refine Document Search"
								}
							}
							//used
							else if ( formmodeModule == "edit" ){
								if ( "base_form_group" == formgroupModule ) {
									banner_text = "Document Preview"
								}
							}
							else if ( "list" == formmodeModule ){
								//used
								if ( "base_form_group" == formgroupModule ) {
									banner_text = "Document Search Results"
								}
							}
							else if (formmodeModule=="search"){
								if ( "base_form_group" == formgroupModule ) {
									banner_text = "Document Search"
								}
							}

								document.write ('<font face = \"Arial\"  color=\"\#0099FF\" size=\"4\"><i>')
								document.write (banner_text)
								document.write ('<\/i><\/font>')
							

						</script>
					</td>
	
					<td align="left" width="225">
						<font face="Arial" color="#42426f" size="2">Current Login: <%=Session("UserName" & dbkey)%></font>
					</td>
				</tr>
			</table>
		</td>
	</tr>

	<tr>
		<td>
			<table border="0" bordercolor="yellow" width="750" cellspacing="1">
				<tr>
					<td width="100">
					</td>
					
					<td width="650" valign="top" colspan="2" nowrap>
						<table border="0" bordercolor="navy">
							<tr><td>
									<%
									if not nav_override = "true" then
										browser = Request.ServerVariables("HTTP_USER_AGENT")
										if instr(browser, "MSIE") > 0 then
											buildMenuBar request("formmode"), request("dbname"), request("formgroup")
										else
											if instr(browser, "4.") < 0 and (instr(browser, "5.") > 0 or instr(browser, "6.") > 0 or instr(browser, "7.") > 0) then
												buildMenuBar request("formmode"), request("dbname"), request("formgroup")
											end if
										end if
									end if
									%>
								</td>
							</tr>
						</table>
					</td>
				</tr>
				
				<tr>
					<td>
					</td>
					
					<td>
						<script language="JavaScript">
							if (noNav == false){ //show the navbar
								 //this is where the buttons are called.  It is based on the formgroupflag in the ini file for the particular formgroup.
								 //if you want to override the formgroupflag for whatever reason do so at the top of the file
								 //add whatever formgroupflags you need to this section
								if ((formgroupflag.toLowerCase() == "single_search")||(formgroupflag.toLowerCase() == "search")){
																			
									//goes to secure_nav.asp to get custom buttons for this formmode of this application
									document.write (MainWindow.getCustomButtons())
									//standard cows buttons for this formmode - comment them out and add one at a time via above function if you need to modify
									//document.write (MainWindow.getButtons());
									<%
									'JHS added 4/7/2003
									'vbscript
									if not Session("showselect") then
									%>
									document.write ('<a href="/<%=Application("AppKey")%>/<%=dbkey%>/mainpage.asp?Timer='+ "<%=Timer%>" + '"  target=_top><img src="/<%=Application("appkey")%>/graphics/mainMenu.gif" border="0"></a>')
									<%end if%>
									
									if (MainWindow.Submit_Docs == "True") {
										document.write ('<a href="/<%=Application("AppKey")%>/docmanager/src/locateDocs.asp" target="_top"><img src="/<%=Application("appkey")%>/graphics/submit_btn.gif" border="0"></a>')		
										if (MainWindow.BatchLoad_Docs == "True") {
											document.write ('<a href="/<%=Application("appkey")%>/docmanager/BatchLoadConfigForm.asp" target="_top"><img src="/<%=Application("appkey")%>/graphics/batchSubmission_btn.gif" border="0"></a>')
										}
									}

									if (MainWindow.View_History == "True") {
										document.write ('<a href="/<%=Application("appkey")%>/docmanager/RecentActivitiesFrameset.asp" target="_top"><img src="/<%=Application("appkey")%>/graphics/recent_activities_btn.gif" border="0"></a>')
									}
								}
							}
						</script>
					</td>
				</tr>  
				  
				<!-- This table is for the major operation buttons: things								like Edit Record, Main Menu, New Query. -->
				<tr><td></td>
					<td nowrap>
						
						<script language="JavaScript"> 
							if (MainWindow.document.cows_input_form != null) {
								if (window.navigator.appName == 'Netscape') {
									if (window.navigator.appVersion.indexOf('4.') >= 0) {
										document.write('<table border="0" width="500" cellspacing="0" cellpadding="0" nowrap>');
										document.write ('<tr><td align="left"><a href="/CBOEHelp/CBOEContextHelp/Doc Manager Webhelp/Default.htm" target="new"><img src="/docmanager/graphics/help_btn.gif" border="0"></a>')
										document.write('<a href="#" onclick="window.open(\'/docmanager/about.asp\', \'about\', \'width=560,height=450,status=no,resizable=yes\')"><img src="/cfserverasp/source/graphics/navbuttons/about_btn.gif" border="0"></a></td>')
										document.write('<td align="center"><a href="/<%=Application("appkey")%>/logoff.asp" target="_top"><img src="/<%=Application("appkey")%>/graphics/log_off_big_btn.gif" border="0"></a></td></tr></table>')

										//if you need to add something to fixed buttons
										//add a funciton to secure_nav.asp and call from here
									}
								}
	
								<%
								'JHS added 4/7/2003
								'vbscript
								if not Session("showselect") then
									excludedlist = "'help, preferences, print'"
								else
									excludedlist = "'help, preferences, print, log_off, home'"
								end if
								%>
							}
						</script>
					</td>
				</tr>
				
				<tr>
					<td>
					</td>
				
					<td width="650" valign="top" colspan="2">
						<table border="0" bordercolor="aqua" cellspacing="3">
							<tr>
								<td>
									<nobr>
									<script language="JavaScript">
									<!--
										if ( noNav == false ) {
											// We are putting buttons up on the nav bar.
											if ( ( "document_list" == formgroupflagModule ) ||
												( "document_details" == formgroupflagModule ) )
											{
												// We want to put up the record navigation buttons
												// (first, prev, next, last, that sort of thing).
												document.write (MainWindow.getRecordNav());
											}
										}
									// -->
									</script>
									</nobr>
								</td>
								<td>Total documents: 						
									<%if NOT (countRS.BOF and countRS.EOF)then%>
										<%=countRS.Fields(0)%>
									<%end if%>
								</td>
							</tr>
						</table>
					</td>
				</tr>
			</table>
		</td>
	</tr>
	
</table>
</body>
</html>
<%countRS.Close%>
<%Set countRS = nothing%>
<%cnn.Close%>
<%Set cnn = nothing%>