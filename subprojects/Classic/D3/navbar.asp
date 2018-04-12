<%@ LANGUAGE="VBScript" %>
<%Response.Expires=0%>
<html>

<head>
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/menubar.asp" --> 
<%

Dim nv_DEBUG
nv_DEBUG = False

'this is a variable for reg, but shouldn't be removed. 
commit_type = Request.QueryString( "commit_type" )

' some files add the following variable to the querystring to stop the navbar
' buttons from displaying.
nav_override = Request.QueryString( "nav_override" )
dbkey = Request.QueryString( "dbname" )
formgroup = Request.QueryString( "formgroup" )
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
	MainWindow = <%=Application( "MainWindow" )%>
	formgroup = MainWindow.formgroup
	formgroupflag = MainWindow.formgroupflag
	uniqueid = MainWindow.uniqueid
	//MRE removed for public site
	//mainpage_enabled = MainWindow.mainpage_enabled
	mainpage_enabled = 0
	
	//alert('<%=Application("Main_Page" & "D3")%>')
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
	if ( "manage_users" == formmodeInput ){
		formmodeModule = "edit_record"
		formgroupflagModule = "user_security"
		noSupInfo = true
		noNav = false
	}
	else if ( "manage_passwords" == formmodeInput ){
		formmodeModule = "edit_record"
		formgroupflagModule = "password_security"
		noSupInfo=true
		noNav = false
	}
	else if ( "manage_users_new" == formmodeInput ){
		formmodeModule = "add_record"
		formgroupflagModule = "user_security"
		noSupInfo = true
		noNav = false
	}
	else if ( ( "search" == formmodeInput ) &&
		( "base_form_group" == formgroupModule ) )
	{
		formgroupflagModule = "search_parents"
		noSupInfo = false
		noNav = false
	}
	else if ( ( "refine" == formmodeInput ) &&
		( "base_form_group" == formgroupModule ) )
	{
		formgroupflagModule = "refine_parents"
		noSupInfo = false
		noNav = false
	}
	else if ( ( "list" == formmodeInput ) &&
		( "base_form_group" == formgroupModule ) )
	{
		formgroupflagModule = "parent_list"
		noSupInfo = false
		noNav = false
	}
	else if ( "add_parent" == formmodeInput ) {
		formgroupflagModule = "add_parent"
		noSupInfo = true
		noNav = false
	}
	else if ( "add_experiment" == formmodeInput ){
		formgroupflagModule = "add_experiment"
		noSupInfo = true
		noNav = false
	}
	else if ( "add_degradant" == formmodeInput ){
		formgroupflagModule = "add_degradant"
		noSupInfo = true
		noNav = false
	}
	else if ( "add_mechanism" == formmodeInput ){
		formgroupflagModule = "add_mechanism"
		noSupInfo = true
		noNav = false
	}
	else if ( "add_condition" == formmodeInput ){
		formgroupflagModule = "condition_list_admin"
		noSupInfo = true
		noNav = false
	}
	else if ( "add_fgroup" == formmodeInput ){
		formgroupflagModule = "fgroup_list_admin"
		noSupInfo = true
		noNav = false
	}
	else if ( "add_doclink" == formmodeInput ){
		formgroupflagModule = "add_doclink"
		noSupInfo = true
		noNav = false
	}
	else if ( "modify_condition" == formmodeInput ){
		formgroupflagModule = "modify_condition_text"
		noSupInfo = true
		noNav = false
	}
	else if ( "modify_fgroup" == formmodeInput ){
		formgroupflagModule = "modify_fgroup_text"
		noSupInfo = true
		noNav = false
	}
	else if ( "add_salt" == formmodeInput ){
		formgroupflagModule = "salt_list_admin"
		noSupInfo = true
		noNav = false
	}
	else if ( "modify_salt" == formmodeInput ){
		formgroupflagModule = "modify_salt_name"
		noSupInfo = true
		noNav = false
	}
	else if ( "add_status" == formmodeInput ){
		formgroupflagModule = "status_list_admin"
		noSupInfo = true
		noNav = false
	}
	else if ( "modify_status" == formmodeInput ){
		formgroupflagModule = "modify_status_text"
		noSupInfo = true
		noNav = false
	}
	else if ( "edit" == formmodeInput )
	{
		if ( "base_form_group" == formgroupModule )
		{
			formgroupflagModule = "parent_details"
			noSupInfo = false
			noNav = false
		}
		else if ( "addexperiment_form_group" == formgroupModule )
		{
			formgroupflagModule = "experiment_details"
			noSupInfo = true
			noNav = false
		}
		else if ( "adddegradant_form_group" == formgroupModule )
		{
			formgroupflagModule = "degradant_details"
			noSupInfo = true
			noNav = false
		}
		else if ( "addmechanism_form_group" == formgroupModule )
		{
			formgroupflagModule = "mechanism_details"
			noSupInfo = true
			noNav = false
		}
	}
	else if ( "edit_record" == formmodeInput ) {
		if ( "base_form_group" == formgroupModule ) {
			formgroupflagModule = "modify_parent"
			noSupInfo = true
			noNav = false
		}
		else if ( "addexperiment_form_group" == formgroupModule ) {
			formgroupflagModule = "modify_experiment"
			noSupInfo = true
			noNav = false
		}
		else if ( "adddegradant_form_group" == formgroupModule ) {
			formgroupflagModule = "modify_degradant"
			noSupInfo = true
			noNav = false
		}
		else if ( "addmechanism_form_group" == formgroupModule ) {
			formgroupflagModule = "modify_mechanism"
			noSupInfo = true
			noNav = false
		}
	}
	else if ( "select" == formmodeInput ) {
		if ( "select_salts" == formgroupModule ) {
			formgroupflagModule = "select_salts"
			noSupInfo = true
			noNav = false
		}
	}
	else if ( "manage_users" == formmodeInput ){
		formmodeModule = "edit_record"
		formgroupflagModule = "user_security"
		noSupInfo = true
		noNav = false
	}
	else if ( "manage_users_new" == formmodeInput ){
		formmodeModule = "add_record"
		formgroupflagModule = "user_security"
		noSupInfo = true
		noNav = false
	}
	else if ( "add_compounds" == formmodeInput ){
		formgroupflagModule = "add_compounds"
		noSupInfo = true
		noNav = false
	}

//0	document.write( '<%=Request.QueryString%><br>' )

//1	if ( false == noNav ) {
//1		document.write( 'noNav == false&nbsp; &nbsp;' )
//1	}
//1	else
//1	{
//1		document.write( 'noNav == true&nbsp; &nbsp;' )
//1	}
//1	if ( false == noSupInfo ) {
//1		document.write( 'noSupInfo == false&nbsp; &nbsp;' )
//1	}
//1	else
//1	{
//1		document.write( 'noSupInfo == true&nbsp; &nbsp;' )
//1	}
//1	document.write( 'mainpage_enabled = "' + mainpage_enabled + '"' )
//1	document.write( '&nbsp; &nbsp;formmodeInput = "' + formmodeInput + '"' )
//1	document.write( '&nbsp; &nbsp;formgroupInput = "' + formgroupInput + '"' )
//1	document.write( '&nbsp; &nbsp;formgroupflagInput = "' + formgroupflagInput + '"<br>' )
//1	document.write( '&nbsp; &nbsp;formmodeModule = "' + formmodeModule + '"' )
//1	document.write( '&nbsp; &nbsp;formgroupModule = "' + formgroupModule + '"' )
//1	document.write( '&nbsp; &nbsp;formgroupflagModule = "' + formgroupflagModule + '"<br>' )

</script> 


<title>Nav bar</title>
<LINK REL='stylesheet' TYPE='text/css' HREF='/<%=Application("appkey")%>/styles.css'>
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
							if ( formmodeModule == "experiment_details" ){
								imgsrc = pathBannerGif + "Banner_ExperimentDetails.gif"
								banner_text = "Degradation Experiment Details"
							}
							else if (formmodeModule == "manage_passwords"){
								noSupInfo=true
								imgsrc = pathBannerGif + "manage_users_bnr.gif"
								banner_text = "Manage Users"
							}
							else if ( formmodeModule == "add_parent" ){
								imgsrc = pathBannerGif + "Banner_AddParent.gif"
								banner_text = "Add Parent Compound"
							}
							else if ( formmodeModule == "add_experiment" ){
								imgsrc = pathBannerGif + "Banner_AddExperiment.gif"
								banner_text = "Add Degradation Experiment"
							}
							else if ( formmodeModule == "add_degradant" ){
								imgsrc = pathBannerGif + "Banner_AddDegradant.gif"
								banner_text = "Add Degradant Compound"
							}
							else if ( formmodeModule == "add_mechanism" ){
								imgsrc = pathBannerGif + "Banner_AddMechanism.gif"
								banner_text = "Add Degradation Mechanism"
							}

							//else if ( formmodeModule == "add_doclink" ){
							//	imgsrc = pathBannerGif + "Banner_AddDocLink.gif"
							//}

							else if ( "search" == formmodeModule ){
								if (( "user_security" == formgroupflagModule )||( "role_security" == formgroupflagModule )){
									imgsrc = pathBannerGif + "manage_users_bnr.gif"
									banner_text = ""
								}
								else if ( "commit" == formgroupflagModule ){
									imgsrc = pathBannerGif + "register_input_form_bnr.gif"
									banner_text = ""
								}
								else if ( "search_parents" == formgroupflagModule ) {
									imgsrc = pathBannerGif + "Banner_ParentSearch.gif"
									banner_text = "Parent Compound Search"
								}
							}

							else if ( "refine" == formmodeModule ){
								if ( "refine_parents" == formgroupflagModule ) {
									imgsrc = pathBannerGif + "Banner_RefineParentSearch.gif"
									banner_text = "Refine Parent Compound Search"
								}
							}

							else if ( formmodeModule == "edit" ){
								if ( "base_form_group" == formgroupModule ) {
									imgsrc = pathBannerGif + "Banner_ParentDetails.gif"
									banner_text = "Parent Compound Details"
								}
								else if ( "addexperiment_form_group" == formgroupModule ){
									imgsrc = pathBannerGif + "Banner_ExperimentDetails.gif"
									banner_text = "Degradation Experiment Details"
								}
								else if ( "adddegradant_form_group" == formgroupModule ){
									imgsrc = pathBannerGif + "Banner_DegradantDetails.gif"
									banner_text = "Degradant Compound Details"
								}
								else if ( "addmechanism_form_group" == formgroupModule ){
									imgsrc = pathBannerGif + "Banner_MechanismDetails.gif"
									banner_text = "Degradation Mechanism Details"
								}
							}
							else if ( formmodeModule == "edit_record" ){
								if (( formgroupflagModule =="user_security" )||( formgroupflagModule =="role_security" )){
									imgsrc = pathBannerGif + "manage_users_bnr.gif"
									banner_text = ""
								}
								else if ( formgroupflagModule =="manage_tables" ){
									imgsrc = pathBannerGif + "manage_tables_bnr.gif"
									banner_text = ""
								}
								else if ( "base_form_group" == formgroupModule ) {
									imgsrc = pathBannerGif + "Banner_EditParent.gif"
									banner_text = "Edit Parent Compound"
								}
								else if ( "addexperiment_form_group" == formgroupModule ) {
									imgsrc = pathBannerGif + "Banner_EditExperiment.gif"
									banner_text = "Edit Degradation Experiment"
								}
								else if ( "adddegradant_form_group" == formgroupModule ) {
									imgsrc = pathBannerGif + "Banner_EditDegradant.gif"
									banner_text = "Edit Degradant Compound"
								}
								else if ( "addmechanism_form_group" == formgroupModule ) {
									imgsrc = pathBannerGif + "Banner_EditMechanism.gif"
									banner_text = "Edit Degradation Mechansim"
								}
								else{
									imgsrc = pathBannerGif + "resultsform_bnr.gif"
									banner_text = "Results Form"
								}
							}
							else if ( "list" == formmodeModule ){
								if (( "user_security" == formgroupflagModule ) || ( "role_security" == formgroupflagModule )){
									imgsrc = pathBannerGif + "manage_users_bnr.gif"
									banner_text = "Manage Users"
								}
								else if ( "manage_tables" == formgroupflagModule ) {
									imgsrc = pathBannerGif + "manage_tables_bnr.gif"
									banner_text = "Manage Tables"
								}
								else if ( "base_form_group" == formgroupModule ) {
									imgsrc = pathBannerGif + "Banner_ParentResults.gif"
									banner_text = "Parent Compound Search Results"
								}
							}
							else if ( "add_condition" == formmodeModule ) {
								imgsrc = pathBannerGif + "Banner_ManageConditions.gif"
								banner_text = "Manage Degradation Condition List"
							}
							else if ( "modify_condition" == formmodeModule ) {
								imgsrc = pathBannerGif + "Banner_ChangeCondText.gif"
								banner_text = "Change Degradation Condition Text"
							}
							else if ( "add_fgroup" == formmodeModule ) {
								imgsrc = pathBannerGif + "Banner_ManageConditions.gif"
								banner_text = "Manage Functional Group List"
							}
							else if ( "modify_fgroup" == formmodeModule ) {
								imgsrc = pathBannerGif + "Banner_ChangeCondText.gif"
								banner_text = "Change Functional Group Text"
							}
							else if ( "add_status" == formmodeModule ) {
								imgsrc = pathBannerGif + "Banner_ManageStatuses.gif"
								banner_text = "Manage Status List"
							}
							else if ( "modify_status" == formmodeModule ) {
								imgsrc = pathBannerGif + "Banner_ChangeStatusText.gif"
								banner_text = "Change Status Text"
							}
							else if ( "add_salt" == formmodeModule ) {
								imgsrc = pathBannerGif + "Banner_ManageSalts.gif"
								banner_text = "Manage Parent Salt List"
							}
							else if ( "modify_salt" == formmodeModule ) {
								imgsrc = pathBannerGif + "Banner_ChangeSaltName.gif"
								banner_text = "Change Salt Name"
							}
							else if ( formmodeModule == "manage_users" ) {
								imgsrc = pathBannerGif + "manage_users_bnr.gif"
								banner_text = "Manage Users"
							}
							else if ( formmodeModule == "manage_tables" ) {
								imgsrc = pathBannerGif + "manage_tables_bnr.gif"
								banner_text = "Manage Tables"
							}
							else if ( formmodeModule == "add_record" ) {
								if (( formgroupflagModule =="user_security" )||( formgroupflagModule =="role_security" )){
									imgsrc = pathBannerGif + "manage_users_bnr.gif"
									banner_text = "Manage Users"
								}
								else if ( formgroupflagModule=="manage_tables" ){
									imgsrc = pathBannerGif + "manage_tables_bnr.gif"
									banner_text = "Manage Tables"
								}
								else {
									imgsrc = pathBannerGif + "add_records_bnr.gif"
									banner_text = "Add Records"
								}
							}
							else if ( formmodeModule == "select" ) {
								if ( formgroupflagModule == "select_salts" ) {
									imgsrc = pathBannerGif + "Banner_Blank.gif"
									banner_text = ""
								}
							}
							else if ( formmodeModule == "no_nav_view" ){
								imgsrc = pathBannerGif + "resultsform_bnr.gif"
								banner_text = "Results Form"
							}
							else if ( formmodeModule == "add_compounds" ){
								imgsrc = pathBannerGif + "resultsform_bnr.gif"
								banner_text = "Results Form"
							}
							if (usebannertext) {
								document.write ('<span class="mainheading">')
								document.write (banner_text)
								document.write ('<\/span>')
							}
							else
								{
									document.write ('<img src = "' + imgsrc + '" border = "0">')
								}
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
						<script language="JavaScript"><!--
						//2	document.write( '"' + mainpage_enabled + '"' )
						//2	document.write( '&nbsp; &nbsp;"' + formmodeModule + '"' )
						//2	document.write( '&nbsp; &nbsp;"' + formgroupModule + '"' )
						//2	document.write( '&nbsp; &nbsp;"' + formgroupflagModule + '"<br>' )

							if ( ( false == noNav ) && ( false == noSupInfo ) ) {
								document.write( MainWindow.getTotalDBRecords() );
								document.write( MainWindow.getComment() );
							}
							// -->
						</script>
						</font>
					</td>

					<td valign="top" align="left" nowrap colspan="2">
						<%
						if Not nav_override = "true" then 
							'MRE removed because we do not need a logout button, since they never logged in.
							'Session("menubar_buttons") = ""
							buildMenuBar request("formmode"),request("dbname"), request("formgroup") 
						end if 
						%>

						<!--						<script language="JavaScript"> 						//3	if ( false == noNav ) {						//3		document.write( '!noNav&nbsp; &nbsp;' )						//3	}						//3	else						//3	{						//3		document.write( 'noNav&nbsp; &nbsp;' )						//3	}						//3	if ( false == noSupInfo ) {						//3		document.write( '!noSupInfo&nbsp; &nbsp;' )						//3	}						//3	else						//3	{						//3		document.write( 'noSupInfo&nbsp; &nbsp;' )						//3	}						//3	document.write( '"' + mainpage_enabled + '"' )						//3	document.write( '&nbsp; &nbsp;"' + formmodeModule + '"' )						//3	document.write( '&nbsp; &nbsp;"' + formgroupModule + '"' )						//3	document.write( '&nbsp; &nbsp;"' + formgroupflagModule + '"' )						//	if ( false == noNav ){								//if you need to add something to fixed buttons								//add a function to secure_nav.asp and call from here						//		document.write( MainWindow.getFixedButtons() );						//	}						</script>						-->
					</td>

					<!---td align="left" width="200" nowrap>						<font face="Arial" color="#42426f" size="2">							<b>							<%							'if Application( "LoginRequired" &dbkey)=1 then							'	Response.Write "Current Login: " & UCase(Session( "UserName" & dbkey))							'end if							%>							</b>						</font>					</td--->
				</tr>

				<tr>
					<td width="650" valign="top" colspan="2" nowrap>
						<table border="0" bordercolor="navy">
							<!-- This table is for the major operation buttons: things								like Edit Record, Main Menu, New Query. -->
							<tr>
								<td nowrap>
									<script language="JavaScript">

									//4	if ( false == noNav ) {
									//4		document.write( '!noNav&nbsp; &nbsp;' )
									//4	}
									//4	else
									//4	{
									//4		document.write( 'noNav&nbsp; &nbsp;' )
									//4	}
									//4	if ( false == noSupInfo ) {
									//4		document.write( '!noSupInfo&nbsp; &nbsp;' )
									//4	}
									//4	else
									//4	{
									//4		document.write( 'noSupInfo&nbsp; &nbsp;' )
									//4	}
									//4	document.write( '"' + mainpage_enabled + '"' )
									//4	document.write( '&nbsp; &nbsp;"' + formmodeModule + '"' )
									//4	document.write( '&nbsp; &nbsp;"' + formgroupModule + '"' )
									//4	document.write( '&nbsp; &nbsp;"' + formgroupflagModule + '"<br>' )
										
										bShowMainPageButton = false

										if ( false == noNav ) { // meaning show the navbar please
											 // This is where the navbar buttons are put up.  It is based on the
											 // formgroupflag in the ini file for the particular formgroup.  If you
											 // want to override the formgroupflag for whatever reason do so at the
											 // top of the file.  Add whatever formgroupflags you need to this section.
											 
											if ( "search_parents" == formgroupflagModule ) {
												// Get custom buttons. Routine is in secure_nav.asp.
												document.write( MainWindow.getCustomButtons() )
												document.write( MainWindow.getButtons() )
												if ( 1 == mainpage_enabled ) {
													bShowMainPageButton = true
												}
											}
											else if ( "refine_parents" == formgroupflagModule ) {
												// Get custom buttons. Routine is in secure_nav.asp.
												document.write( MainWindow.getCustomButtons() )
												document.write( MainWindow.getButtons() )
												if ( 1 == mainpage_enabled ) {
													bShowMainPageButton = true
												}
											}
											else if ( "parent_list" == formgroupflagModule ) {
												// Get custom buttons. Routine is in secure_nav.asp.
												document.write( MainWindow.getCustomButtons() )
												document.write( MainWindow.getButtons() )
												if ( 1 == mainpage_enabled ) {
													bShowMainPageButton = true
												}
											}
											else if ( "parent_details" == formgroupflagModule )
											{
												// Get custom buttons. Routine is in secure_nav.asp.
												document.write( MainWindow.getCustomButtons() )

												document.write( MainWindow.getButtons() )

												if ( 1 == mainpage_enabled ) {
													bShowMainPageButton = true
												}
											}
											else if ( "modify_parent" == formgroupflagModule )
											{
												document.write(MainWindow.getModifyParentButtons())
												// Get custom buttons. Routine is in secure_nav.asp.
												//jhs document.write( MainWindow.getCustomButtons() )

												// Modify parent.  Don't draw up the standard Delete Record button.
												//jhs document.write( MainWindow.getButtons( "delete_record" ) )

												// Put up a Delete Record button with a different link.
												//var returnLoc = '<%=Session( "ReturnToExperimentDetails" & dbkey )%>'
												//outputval = '<a href="JavaScript:MainWindow.location.replace(&quot;' + returnLoc + '&quot;)">'

												//jhs var locDelete = '/<%=Application( "AppKey" )%>/<%=dbkey%>/DeleteConfirm.asp?dbname=<%=dbkey%>&formgroup=base_form_group&deltype=parent&MadeIn=DDNavbar'
												//jhs outputval = '<a href="JavaScript:MainWindow.location.replace(&quot;' + locDelete + '&quot;)"  onMouseOver="status=&quot;Delete the parent compound&quot;; return true;">'
												//jhs outputval = outputval + '<img SRC="/<%=Application( "AppKey" )%>/graphics/Button_DeleteRecord.gif" BORDER="0"></a>'
												//jhs document.write( outputval )

												if ( 1 == mainpage_enabled ) {
													bShowMainPageButton = true
												}
											}
											else if ( "experiment_details" == formgroupflagModule )
											{
												// Get custom buttons. Routine is in secure_nav.asp.
												<%if CBool( Session( "DD_ADD_RECORDS" & dbkey )) = True then
												'only show this button if we have the correct privileges%>
													document.write( MainWindow.getExperimentDetailsButtons() )
												<%end if%>

												if ( 1 == mainpage_enabled ) {
													bShowMainPageButton = true
												}
											}
											else if ( "modify_experiment" == formgroupflagModule )
											{
												document.write(MainWindow.getModifyExperimentButtons())
												// Get custom buttons. Routine is in secure_nav.asp.
												//JHS comment out 1 line
												//document.write( MainWindow.getCustomButtons() )

												// Editing the record.  I need to alter the responses elicited by
												// some of the buttons.  For all three types of objects (parent,
												// experiment, and degradant) I need to alter the Delete Record
												// button's response, because each object has associated with it
												// some Base64 drawing (structure or mechanism) which must also be
												// deleted.  In addition, deleting an experiment calls for deleting
												// all the degradants produced by that experiment, and deleting a
												// parent calls for deleting all the experiments performed on
												// that parent AND all the degradants of all those experiments.

												// Modify experiment.  Don't draw up the standard Delete Record button.
												//JHS comment out 1 line
												//document.write( MainWindow.getButtons( "delete_record" ) )

												// Put up a Delete Record button with a different link.
												//JHS comment out 4 lines
												//var locDelete = '/<%=Application( "AppKey" )%>/<%=dbkey%>/DeleteConfirm.asp?dbname=<%=dbkey%>&formgroup=base_form_group&deltype=experiment&MadeIn=DDNavbar'
												//outputval = '<a href="JavaScript:MainWindow.location.replace(&quot;' + locDelete + '&quot;)"  onMouseOver="status=&quot;Delete this experiment&quot;; return true;">'
												//outputval = outputval + '<img SRC="/<%=Application( "AppKey" )%>/graphics/Button_DeleteRecord.gif" BORDER="0"></a>'
												//document.write( outputval )

												if ( 1 == mainpage_enabled ) {
													bShowMainPageButton = true
												}
											}
											else if ( "degradant_details" == formgroupflagModule )
											{
												// Get custom buttons. Routine is in secure_nav.asp.
												document.write( MainWindow.getDegradantDetailsButtons() )

												if ( 1 == mainpage_enabled ) {
													bShowMainPageButton = true
												}
											}
											else if ( "modify_degradant" == formgroupflagModule )
											{
											
												// Get custom buttons. Routine is in secure_nav.asp.
												document.write( MainWindow.getCustomButtons() )

												// Modify degradant.  Don't draw up the standard Delete Record buttons.
												document.write( MainWindow.getButtons( "delete_record" ) )

												// Put up a Delete Record button with a different link.
												var locDelete = '/<%=Application( "AppKey" )%>/<%=dbkey%>/DeleteConfirm.asp?dbname=<%=dbkey%>&formgroup=base_form_group&deltype=degradant&MadeIn=DDNavbar'
												outputval = '<a href="JavaScript:MainWindow.location.replace(&quot;' + locDelete + '&quot;)"  onMouseOver="status=&quot;Delete the degradant/impurity&quot;; return true;">'
												outputval = outputval + '<img SRC="/<%=Application( "AppKey" )%>/graphics/Button_DeleteRecord.gif" BORDER="0"></a>'
												document.write( outputval )

												if ( 1 == mainpage_enabled ) {
													bShowMainPageButton = true
												}
											}
											else if ( "mechanism_details" == formgroupflagModule )
											{
												// Get custom buttons. Routine is in secure_nav.asp.
												document.write( MainWindow.getMechanismDetailsButtons() )

												if ( 1 == mainpage_enabled ) {
													bShowMainPageButton = true
												}
											}
											else if ( "modify_mechanism" == formgroupflagModule )
											{
												// Get custom buttons. Routine is in secure_nav.asp.
												document.write( MainWindow.getCustomButtons() )

												// Modify mechanism.  Don't draw up the standard Delete Record or Cancel buttons.
												document.write( MainWindow.getButtons( "delete_record,cancel_edit" ) )

												// Put up the cancel button and make it take the user to the experiment details display.
												var locCancel = '<%=Session( "ReturnToExperimentDetails" & dbkey )%>'
												outputval = '<a href="JavaScript:MainWindow.location.replace(&quot;' + locCancel + '&quot;)"  onMouseOver="status=&quot;Cancel the edit, changing nothing&quot;; return true;">'
												outputval = outputval + '<img SRC="/<%=Application( "AppKey" )%>/graphics/Button_Cancel.gif" BORDER="0"></a>'
												document.write( outputval )

												// Put up a Delete Record button with a different link.
												var locDelete = '/<%=Application( "AppKey" )%>/<%=dbkey%>/DeleteConfirm.asp?dbname=<%=dbkey%>&formgroup=base_form_group&deltype=mechanism&MadeIn=DDNavbar'
												outputval = '<a href="JavaScript:MainWindow.location.replace(&quot;' + locDelete + '&quot;)"  onMouseOver="status=&quot;Delete the mechanism&quot;; return true;">'
												outputval = outputval + '<img SRC="/<%=Application( "AppKey" )%>/graphics/Button_DeleteRecord.gif" BORDER="0"></a>'
												document.write( outputval )

												if ( 1 == mainpage_enabled ) {
													bShowMainPageButton = true
												}
											}
											else if ( "select_salts" == formgroupflagModule )
											{
												// Don't put up any buttons.
											}
											else if ( "add_parent" == formgroupflagModule ){
												document.write( MainWindow.getAddParentButtons())
												//MRE 11/26/04 added button. does the same thing as the cancel button, but it helps with usability
												if ( 1 == mainpage_enabled ) {
													bShowMainPageButton = true
												}
											}
											else if ( "add_experiment" == formgroupflagModule ){
												document.write( MainWindow.getAddExperimentButtons())
												if ( 1 == mainpage_enabled ) {
													bShowMainPageButton = true
												}
											}
											else if ( "add_degradant" == formgroupflagModule ){
												//alert("add_degradant")
												document.write( MainWindow.getAddDegradantButtons())
												if ( 1 == mainpage_enabled ) {
													bShowMainPageButton = true
												}
											}
											else if ( "add_mechanism" == formgroupflagModule ){
												document.write( MainWindow.getAddMechanismButtons())

												if ( 1 == mainpage_enabled ) {
													bShowMainPageButton = true
												}									
											}
											else if ( "add_doclink" == formgroupflagModule ){
												document.write( MainWindow.getAddDocLinkButtons())
												if ( 1 == mainpage_enabled ) {
													bShowMainPageButton = true
												}
											}

											else if ( "condition_list_admin" == formgroupflagModule )
											{
												// Get custom buttons. Routine is in secure_nav.asp.
												document.write( MainWindow.getConditionAdminButtons() )

												if ( 1 == mainpage_enabled ) {
													bShowMainPageButton = true
												}
											}
											else if ( "modify_condition_text" == formgroupflagModule )
											{
												// Get custom buttons. Routine is in secure_nav.asp.
												document.write( MainWindow.getModifyConditionTextButtons() )

												if ( 1 == mainpage_enabled ) {
													bShowMainPageButton = true
												}
											}
											else if ( "fgroup_list_admin" == formgroupflagModule )
											{
												// Get custom buttons. Routine is in secure_nav.asp.
												document.write( MainWindow.getFunctionalGroupAdminButtons() )

												if ( 1 == mainpage_enabled ) {
													bShowMainPageButton = true
												}
											}
											else if ( "modify_fgroup_text" == formgroupflagModule )
											{
												// Get custom buttons. Routine is in secure_nav.asp.
												document.write( MainWindow.getModifyFunctionalGroupTextButtons() )

												if ( 1 == mainpage_enabled ) {
													bShowMainPageButton = true
												}
											}
											else if ( "status_list_admin" == formgroupflagModule )
											{
												// Get custom buttons. Routine is in secure_nav.asp.
												document.write( MainWindow.getStatusAdminButtons() )

												if ( 1 == mainpage_enabled ) {
													bShowMainPageButton = true
												}
											}
											else if ( "modify_status_text" == formgroupflagModule )
											{
												// Get custom buttons. Routine is in secure_nav.asp.
												document.write( MainWindow.getModifyStatusTextButtons() )

												if ( 1 == mainpage_enabled ) {
													bShowMainPageButton = true
												}
											}
											else if ( "salt_list_admin" == formgroupflagModule )
											{
												// Get custom buttons. Routine is in secure_nav.asp.
												document.write( MainWindow.getSaltAdminButtons() )

												if ( 1 == mainpage_enabled ) {
													bShowMainPageButton = true
												}
											}
											else if ( "modify_salt_name" == formgroupflagModule )
											{
												// Get custom buttons. Routine is in secure_nav.asp.
												document.write( MainWindow.getModifySaltNameButtons() )

												if ( 1 == mainpage_enabled ) {
													bShowMainPageButton = true
												}
											}

											else if ( formgroupflagModule == "manage_tables" ){
												document.write (MainWindow.getRegManageTablesButtons())
												if ( 1 == mainpage_enabled ) {
													bShowMainPageButton = true
												}
											}
											else if ( formgroupflagModule == "user_security" ){
												document.write (MainWindow.getRegManageUsersButtons())
												if ( 1 == mainpage_enabled ) {
													bShowMainPageButton = true
												}
											}
											else if (formgroupflagModule == "password_security"){
												document.write (MainWindow.getRegManagePasswordsButtons())
												document.write ('<a href="/<%=Application("AppKey")%>/<%=dbkey%>/mainpage.asp?dbname=<%=dbkey%>&MadeIn=DDNavbar"  target=_top><img src="/<%=Application("appkey")%>/graphics/mainMenu.gif" border="0"></a>')
											}
											else if ( formgroupflagModule == "role_security" ){
												document.write (MainWindow.getRegManageRolesButtons())
												if ( 1 == mainpage_enabled ) {
													bShowMainPageButton = true
												}
											}
											else if ( formgroupflagModule == "add_compounds" ){
												document.write (MainWindow.getAddCompoundsButtons())
												if ( 1 == mainpage_enabled ) {
													bShowMainPageButton = true
												}									
											}
										}  //end if ( noNav == false )

										// Draw up the "Main menu" button if needed.
										if ( true == bShowMainPageButton ) {
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
											// We are putting buttons up on the nav bar.
											if ( ( "parent_list" == formgroupflagModule ) ||
												( "parent_details" == formgroupflagModule ) )
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
								
								<script>
									function setCookie( name, value ) {
										dbkey = "<%=dbkey%>"

										//if ( "false" == value ) {
										//	value = "0"
										//}
										//if ( "true" == value ) {
										//	value = "1"
										//}
										expires_date = new Date
										expires_date.setYear( expires_date.getYear() + 2 )
										window.document.cookie = name + dbkey + formgroup + "="  + escape( value ) + "; expires=" + expires_date.toGMTString() + ";"
										//alert(window.document.cookie)
										MainWindow.location.reload(); 
										return false;
									}
								</script>
								
								<td>

								<%								
								if (Request.Cookies( "UserResultsPrefs" & dbkey & formgroup) <> "DoNotShowStructs") then
								%>
									<script language="javascript">
										if ( noNav == false ) {
											if ( "parent_list" == formgroupflagModule ) {
												document.write('<a href="#" onclick="setCookie(\'UserResultsPrefs\',\'DoNotShowStructs\')" title="Removes chemical structures from the page."><img src="/<%=Application("appkey")%>/graphics/hide_structures_btn.gif" border="0"></a>')
											}
										}
									</script>
								<%
								else
								%>
										<script language="javascript">
										if ( noNav == false ) {
											if ( "parent_list" == formgroupflagModule ) {
												document.write('<a href="#" onclick="setCookie(\'UserResultsPrefs\',\'ShowStructs\')" title="Displays chemical structures on page."><img src="/<%=Application("appkey")%>/graphics/show_structures_btn.gif" border="0"></a>')
											}
										}
										</script>
								
								<%
								end if
							

								if (Request.Cookies("PrinterFriendly" & dbkey & formgroup)= "1") then
								%>
									<script language="javascript">
										if ( noNav == false ) {
											if ((formgroupflagModule == "parent_list" )||(formgroupflagModule == "parent_details")) {
												document.write('<a href="#" onclick="setCookie(\'PrinterFriendly\',\'0\')" title="Display user-interface buttons."><img src="/<%=Application("appkey")%>/graphics/screen_optimized_btn.gif" border="0"></a>')
											}
										}
									</script>
								<%
								else
								%>
									<script language="javascript">
										if ( noNav == false ) {
											if ((formgroupflagModule == "parent_list" )||(formgroupflagModule == "parent_details")) {
												document.write('<a href="#" onclick="setCookie(\'PrinterFriendly\',\'1\')" title="Hides user-interface buttons for printing."><img src="/<%=Application("appkey")%>/graphics/printer_friendly_btn.gif" border="0"></a>')
											}
										}
									</script>								
								<%
								end if
								%>
									
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
