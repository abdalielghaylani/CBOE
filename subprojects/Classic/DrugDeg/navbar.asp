<%@ LANGUAGE="VBScript" %>
<%Response.Expires=0%>
<html>

<head>
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/menubar.asp" --> 
<%
Dim nv_DEBUG
nv_DEBUG = False
'this is a variable for reg, but shouldn't be removed. 
commit_type = Request.QueryString("commit_type")
'some files add the following variable to the querystring to stop the navbar buttons from displaying
nav_override = Request.QueryString("nav_override")
'formmode = Request.QueryString("formmode")
dbkey = Request.QueryString("dbname")
formgroup = Request("formgroup")
'add your appkey here to avoid errors when dbkey is empty
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


//starndard variables - do not edit
	if (!top.main.mainFrame){
		MainWindow = <%=Application("mainwindow")%>
	}
	else{
		MainWindow = top.main.mainFrame
	}
	formgroupflag = MainWindow.formgroupflag
	var formgroup = MainWindow.formgroup
	uniqueid = MainWindow.uniqueid
	mainpage_enabled = MainWindow.mainpage_enabled
	//this is reg only, but don't remove
	commit_type = MainWindow.commit_type



	//defaults
	var noNav = false
	var noSupInfo = false
	var nav_override =  "<%=nav_override%>"
	if (nav_override == "true"){
		formmode = "<%=request("formmode")%>"
		noNav = true
	}
	else{
		formmode = MainWindow.formmode
	}
	
	formmode = formmode.toLowerCase()
	formgroup = formgroup.toLowerCase()
	formgroupflag = formgroupflag.toLowerCase()

	var banner_text;


	//start rewrite
	//completely reoragnized and rewritten
	//removed resetting noNav if it was just staying false anyway
	//removed noSup if it was just staying false anyway
	
	if (formmode == "manage_users"){
		formmode = "edit_record"
		formgroupflag="user_security"
		noSupInfo = true
		banner_text = "Manage Users"
	}
	if (formmode == "manage_users_new"){
		formmode = "add_record"
		formgroupflag = "user_security"
		noSupInfo = true
		banner_text = "Manage Users"
	}
	
	if (formgroup == "base_form_group"){
		if (formmode == "search"){
			banner_text = "Parent Compound Search"
			
		}
		else if (formmode == "refine"){
			banner_text = "Refine Parent Compound Search"			
		}
		else if (formmode == "list"){
			banner_text = "Parent Compound Search Results"
		}
		else if (formmode=="edit"){
			banner_text = "Parent Compound Details"
		}
		else if ( formmode=="edit_record"){
			noSupInfo = true
			banner_text = "Edit Parent Compound"
		}	
	}
	else if (formgroup == "addexperiment_form_group"){
	
		if (formmode == "edit")	{
			noSupInfo = true
			banner_text = "Degradation Experiment Details"
		}
		
		else if (formmode=="edit_record") {
			noSupInfo = true
			banner_text = "Edit Degradation Experiment"
		}
		else if (formmode == "add_experiment"){
			noSupInfo = true
			banner_text = "Add Degradation Experiment"
		}
	}
	else if (formgroup == "adddegradant_form_group"){
	
		if (formmode == "edit"){
			noSupInfo = true
			banner_text = "Degradant Compound Details"
		}
		else if (formmode == "add_degradant"){
			noSupInfo = true
			banner_text = "Add Degradant Compound"
		}
		else if (formmode == "edit_record"){
			noSupInfo = true
			banner_text = "Edit Degradant Compound"
		}
	}
	else if (formgroup == "addmechanism_form_group"){
		if (formmode == "edit"){
			noSupInfo = true
			banner_text = "Degradation Mechanism Details"
		}
		else if (formmode == "add_mechanism"){
			noSupInfo = true
			banner_text = "Add Degradation Mechanism"
		}
		else if (formmode == "edit_record"){
			noSupInfo = true
			banner_text = "Edit Degradation Mechansim"
		}
	}		

	else if (formgroup == "select_salts") {
		if (formmode == "select") {
			noSupInfo = true
			banner_text = "Select Salts"
		}
	} 
	else if (formgroup == "addparent_form_group") {
		if (formmode == "add_parent") {
			noSupInfo = true
			banner_text = "Add Parent Compound"
		}
	} 
	else if (formgroup == "condition_form_group"){
		if (formmode == "add_condition"){
			noSupInfo = true
			banner_text = "Manage Degradation Condition List"
		}	
		else if (formmode == "modify_condition"){
			noSupInfo = true
			banner_text = "Change Degradation Condition Text"
		}
	}
	else if (formgroup == "fgroup_form_group"){
		if (formmode == "add_fgroup"){
			noSupInfo = true
			banner_text = "Manage Functional Group List"
		}
		//doesn't seem to be an edit page but will leave as a placeholder
		else if (formmode == "modify_fgroup"){
			noSupInfo = true
			banner_text = "Manage Functional Group List"
		}	
	}
	else if (formgroup == "salt_form_group"){
		if (formmode == "add_salt"){
			noSupInfo = true
			banner_text = "Manage Parent Salt List"
		}
		else if (formmode == "modify_salt"){
			noSupInfo = true
			banner_text = "Change Salt Name"
		}	
	}

	//this formgroup does not appear to be available in the gui
	//the pages exist and they are in the ini so we will leave it in
	else if (formgroup == "status_form_group"){
		if (formmode == "add_status"){
			noSupInfo = true
			banner_text = "Manage Status List"
		}
		else if (formmode == "modify_status"){
			noSupInfo = true
			banner_text = "Change Status Text"
		}	
	}
	else if (formgroup == "gs_form_group"){
		if (formmode == "search"){
			banner_text = "DrugDeg  &amp; D3 Global Search"
			
		}
		else if (formmode == "refine"){
			banner_text = "Refine Global Search"			
		}
		else if (formmode == "list"){
			banner_text = "DrugDeg &amp; D3 Global Search Results"
		}
		else if (formmode == "edit"){
			banner_text = "Parent Compound Details"
		}
		else if ( formmode == "edit_record"){
			noSupInfo = true
			banner_text = "Edit Parent Compound"
		}	
	}
	else {
		//will just leave this as is
		if (formmode == "add_doclink"){
			noSupInfo = true
		}
	}
	//alert(formgroup + ' - ' + formmode);
	if (banner_text == ""){
		banner_text = "This is a bug."
	}

</script> 


<title>Nav bar</title>
<LINK REL='stylesheet' TYPE='text/css' HREF='/drugdeg/styles.css'>
</head>

<body <%=Application("BODY_BACKGROUND")%> bgProperties="fixed">
<table>
	<tr>
		<td>
			<table border="0" bordercolor="fuchsia" cellspacing="0" cellpadding="0">
				<!-- The table for the banner. -->
				<tr>
<!-- New -->
					<td valign="top" width="300">
						<img src="<%=application("logoimage")%>" alt="Drug Degradation" align="center">
					</td>
<!-- -->
<!-- Old >					<td width="86" height="44" valign="top">						<img src="/CFServerAsp/Source/graphics/cnco.gif" width="86" height="56" alt="cnco.gif (2252 bytes)" align="center">					</td><!-- -->
					<td width="300">
						<script language="javascript">
							var dbpath = "/<%=Application( "AppKey" )%>/<%=dbkey%>/";
							document.write ('<span class="mainheading">' + banner_text + '<\/span>');

						</script>
					</td>
					<td align="left" width="200" nowrap>
						<font face="Arial" color="#42426f" size="2">
							<b>
							<%
							if Application( "LoginRequired" &dbkey)=1 then
								Response.Write "Current Login: " & UCase(Session( "UserName" & dbkey))
							end if
							%>
							</b>
						</font>
					</td>
				</tr>
			</table>
		</td>
	</tr>

	<tr>
		<td>
			<table border="0" bordercolor="yellow" width="860" cellspacing="1">
				<tr>
					<td rowspan="3" width="150" valign="top">
						<!-- The cell for the total number of records, status comment,							and the number of records in the current list. -->
						<font face="Arial" color="#42426f" size="2">
						<script language="JavaScript">
							if ( ( false == noNav ) && ( false == noSupInfo ) ) {
								document.write( MainWindow.getTotalDBRecords() );
								document.write( MainWindow.getComment() );
							}
						</script>
						</font>
					</td>

					<td valign="top" align="left" nowrap colspan="2">
						<%
						if Not nav_override = "true" then 
							buildMenuBar request("formmode"),request("dbname"), request("formgroup") 
						end if 
						%>


					</td>
				</tr>

				<tr>
					<td width="650" valign="top" colspan="2" nowrap>
						<table border="0" bordercolor="navy">
							<!-- This table is for the major operation buttons: things								like Edit Record, Main Menu, New Query. -->
							<tr>
								<td nowrap>
									<script language="JavaScript">

										bShowMainPageButton = false

										if (noNav == false) { // meaning show the navbar please
											 // This is where the navbar buttons are put up.  It is based on the
											 // formgroupflag in the ini file for the particular formgroup.  If you
											 // want to override the formgroupflag for whatever reason do so at the
											 // top of the file.  Add whatever formgroupflags you need to this section.
											
											//there is only one page where this is false
											//this will set the default
											if (mainpage_enabled == 1) {
												bShowMainPageButton = true
											} 
											
											if (formgroup == "base_form_group"){
												//modifying parent record gets a special function call
												if ( formmode=="edit_record"){
													document.write(MainWindow.getModifyParentButtons())
												}
												//Else handlest the following
												//formgroup:base_form_group
												//formmode: search, refine, list, edit
												//to change the buttons for a specific formmode add an *else if* clause like the above
												else {
													document.write( MainWindow.getCustomButtons() )
													document.write( MainWindow.getButtons() )
												}	

											}							 
											else if (formgroup == "addexperiment_form_group"){
											
												if (formmode == "edit")	{
													document.write(MainWindow.getExperimentDetailsButtons())
												}
												
												else if (formmode=="edit_record") {
													document.write(MainWindow.getModifyExperimentButtons())
												}
												else if (formmode == "add_experiment"){
													document.write(MainWindow.getAddExperimentButtons())
												}
											} 
											else if (formgroup == "adddegradant_form_group"){
	
												if (formmode == "edit"){
													document.write( MainWindow.getDegradantDetailsButtons())
												}
												else if ( formmode == "add_degradant"){
													document.write( MainWindow.getAddDegradantButtons())
												}												
												else if (formmode == "edit_record"){
													document.write( MainWindow.getCustomButtons() )
													document.write( MainWindow.getButtons( "delete_record" ) )
													// Put up a Delete Record button with a different link.
													var locDelete = '/<%=Application( "AppKey" )%>/<%=dbkey%>/DeleteConfirm.asp?dbname=<%=dbkey%>&formgroup=base_form_group&deltype=degradant&MadeIn=DDNavbar'
													outputval = '<a href="JavaScript:MainWindow.location.replace(&quot;' + locDelete + '&quot;)"  onMouseOver="status=&quot;Delete the degradant/impurity&quot;; return true;">'
													outputval = outputval + '<img SRC="/<%=Application( "AppKey" )%>/graphics/Button_DeleteRecord.gif" BORDER="0"></a>'
													document.write( outputval )
												}	
											}											 
											else if (formgroup == "addmechanism_form_group"){
												if (formmode == "edit"){
													document.write( MainWindow.getMechanismDetailsButtons() )
												}
												else if (formmode == "add_mechanism"){
													document.write(MainWindow.getAddMechanismButtons())
												}
												else if (formmode == "edit_record"){
													document.write( MainWindow.getCustomButtons() )
													document.write( MainWindow.getButtons( "delete_record,cancel_edit" ) )

													// Custom cancel button: Puts the user to the experiment details display.
													var locCancel = '<%=Session( "ReturnToExperimentDetails" & dbkey )%>'
													outputval = '<a href="JavaScript:MainWindow.location.replace(&quot;' + locCancel + '&quot;)"  onMouseOver="status=&quot;Cancel the edit, changing nothing&quot;; return true;">'
													outputval = outputval + '<img SRC="/<%=Application( "AppKey" )%>/graphics/Button_Cancel.gif" BORDER="0"></a>'
													document.write( outputval )

													//Custom Delete button
													var locDelete = '/<%=Application( "AppKey" )%>/<%=dbkey%>/DeleteConfirm.asp?dbname=<%=dbkey%>&formgroup=base_form_group&deltype=mechanism&MadeIn=DDNavbar'
													outputval = '<a href="JavaScript:MainWindow.location.replace(&quot;' + locDelete + '&quot;)"  onMouseOver="status=&quot;Delete the mechanism&quot;; return true;">'
													outputval = outputval + '<img SRC="/<%=Application( "AppKey" )%>/graphics/Button_DeleteRecord.gif" BORDER="0"></a>'
													document.write( outputval )
												}
												
											}
											else if (formgroup == "select_salts") {
												if (formmode == "select") {
													// this also overrides the showmain page...
													//the only known one
													bShowMainPageButton = false
												}
											}
											else if (formgroup == "addparent_form_group"){
												if (formmode == "add_parent"){
													document.write( MainWindow.getAddParentButtons())
												}								
											}
											else if (formgroup == "condition_form_group"){
												if (formmode == "add_condition"){
													document.write( MainWindow.getConditionAdminButtons() )
												}
												else if (formmode == "modify_condition"){
													document.write( MainWindow.getModifyConditionTextButtons() )
												}
											}
											else if (formgroup == "fgroup_form_group"){
												if (formmode == "add_fgroup"){
													document.write( MainWindow.getFunctionalGroupAdminButtons() )
												}
												else if (formmode == "modify_fgroup"){
													document.write( MainWindow.getModifyFunctionalGroupTextButtons() )
												}
											}
											else if (formgroup == "salt_form_group"){
												if (formmode == "add_salt")												{
													document.write( MainWindow.getSaltAdminButtons() )
												}
												else if (formmode == "modify_salt"){
													document.write( MainWindow.getModifySaltNameButtons() )
												}										
											}
											else if (formgroup == "status_form_group"){
												if (formmode == "add_status"){
													document.write( MainWindow.getStatusAdminButtons() )
												}
												else if (formmode == "modify_status"){
													document.write( MainWindow.getModifyStatusTextButtons() )
												}										
											}
											else if (formgroup == "gs_form_group"){
												//modifying parent record gets a special function call
												if ( formmode=="edit_record"){
													document.write(MainWindow.getModifyParentButtons())
												}
												//Else handlest the following
												//formgroup:base_form_group
												//formmode: search, refine, list, edit
												//to change the buttons for a specific formmode add an *else if* clause like the above
												else {
													document.write( MainWindow.getCustomButtons() )
													document.write( MainWindow.getButtons() )
												}	

											}	
											//Not using any of the above formgroup so we will use 
											//	only the formgroupflag											 
											else {
												//this should be tested
												if (formmode == "add_doclink"){
													document.write( MainWindow.getAddDocLinkButtons())
												}
												//not sure of the formgroup
												else if (formmode = "add_compounds" ){
													document.write (MainWindow.getAddCompoundsButtons())								
												}
											}  //end if ( noNav == false )
										}
										
										// Draw up the "Main menu" button if needed.
										if (bShowMainPageButton == true ) {
											document.write( '<a href="/<%=Application( "AppKey" )%>/<%=dbkey%>/mainpage.asp?dbname=<%=dbkey%>&MadeIn=DDNavbar&Timer='+ "<%=Timer%>" + '"  target=_top  onMouseOver="status=&quot;Return to the main menu page&quot;; return true;"><img src="/<%=Application( "appkey" )%>/graphics/Button_MainMenu.gif" border="0"></a>')
										}

									// -->
									</script>
								</td>
							</tr>
						</table>
					</td>
				</tr>
    
				<tr>
					<td width="650" valign="top" colspan="2">
						<table border="0" bordercolor="aqua" cellspacing="3">
							<tr>
								<td>
									<nobr>
									<script language="JavaScript">
									<!--
										if ( noNav == false ) {
											//record navigation only appears in the base form_group 
											//for search result browsing list and edit (edit=details)
											//if (formgroup == "base_form_group"){
											if ((formgroup == "base_form_group")||(formgroup == "gs_form_group")){
												if ((formmode == "list") || (formmode == "edit")){
													document.write (MainWindow.getRecordNav());
												}
											}
										}
									// -->
									</script>
									</nobr>
								</td>
								
								<script>
									function setCookie( name, value ) {
										dbkey = "<%=dbkey%>"
										expires_date = new Date
										expires_date.setYear( expires_date.getYear() + 2 )
										window.document.cookie = name + dbkey + formgroup + "="  + escape( value ) + "; expires=" + expires_date.toGMTString() + ";"
										MainWindow.location.reload(); 
										return false;
									}
								</script>
								
								<td>

									<script language="javascript">
										var showstructstatus = "<%=Request.Cookies("UserResultsPrefs" & dbkey & formgroup)%>";
										if (noNav == false){
											//if we are in list view for a recordset then we want to use the showhide buttons
											if ((formgroup=="base_form_group") && (formmode=="list")){
												if (!(showstructstatus.toLowerCase() == "donotshowstructs")){
													document.write('<a href="#" onclick="setCookie(\'UserResultsPrefs\',\'DoNotShowStructs\')" title="Removes chemical structures from the page."><img src="/drugdeg/graphics/hide_structures_btn.gif" border="0"></a>')
												}
												else {
													document.write('<a href="#" onclick="setCookie(\'UserResultsPrefs\',\'ShowStructs\')" title="Displays chemical structures on page."><img src="/drugdeg/graphics/show_structures_btn.gif" border="0"></a>')
												}
											}
										}
									</script>
							
									<script language="javascript">
										var showprinterstatus = "<%=Request.Cookies("PrinterFriendly" & dbkey & formgroup)%>";
										if (noNav == false){
											//if we are in list view for a recordset then we want to use the showhide buttons
											if ((formgroup=="base_form_group") && ((formmode=="list")||(formmode=="edit"))){
												if (showprinterstatus.toLowerCase() == "1"){
													document.write('<a href="#" onclick="setCookie(\'PrinterFriendly\',\'0\')" title="Display user-interface buttons."><img src="/drugdeg/graphics/screen_optimized_btn.gif" border="0"></a>')
												}
												else {
													document.write('<a href="#" onclick="setCookie(\'PrinterFriendly\',\'1\')" title="Hides user-interface buttons for printing."><img src="/drugdeg/graphics/printer_friendly_btn.gif" border="0"></a>')
												}
											}
										}
									</script>
							
									
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
