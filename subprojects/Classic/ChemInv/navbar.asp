<%@ LANGUAGE="VBScript" %>

<%Response.Expires=0%>
<html>

<head>
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/menubar.asp"-->
<%
Dim nv_DEBUG
nv_DEBUG = False
'CSBR# 139459
'Purpose: To append portnumber to the hostname, if a non-standard port number is used.
Dim portNumber
Dim serverName
portNumber = Request.ServerVariables("SERVER_PORT")
serverName = Request.ServerVariables("SERVER_NAME")   
if portNumber <> "80" then
    serverName = serverName & ":" & portNumber 
end if
'End of change
if Session("returnaction") <> "" then
	'cancel_url = "http://" & Request.ServerVariables("Server_Name") & "/ChemInv/default.asp?TB=" & Session("ssTab") & "&formgroup=" & Session("returnaction") & "&dataaction=db&dbname=cheminv&Showbanner=True&formmode="
	cancel_url = Application("SERVER_TYPE") &  serverName  & "/ChemInv/default.asp?formgroup=" & Session("returnaction") & "&dataaction=db&dbname=cheminv&Showbanner=True&formmode=&ClearCurrentLocation=1"
else
	cancel_url = Application("SERVER_TYPE") &  serverName  & "/cs_security/home.asp"
end if
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
	function getCookie(name) {
		var cname = name  + "=";
		var dc = document.cookie;
		if (dc.length > 0) {
			begin = dc.indexOf(cname);
				if(begin != -1) {
					begin += cname.length;
					end = dc.indexOf(";", begin);
						if(end == -1) end = dc.length;
							 temp = unescape(dc.substring(begin, end));
							 theResult = temp;
							  return theResult;
				}
			}
		return null;	
	}	

	//starndard variables - do not edit
	//!DGB! change to accomodate the extra frame inside the main frame
	if (!top.main.mainFrame){
		MainWindow = <%=Application("mainwindow")%>
	}
	else{
		MainWindow = top.main.mainFrame
	}
	function Print() {
	    //alert(formmode.toLowerCase())
	    var win;
	    
	    if (formmode.toLowerCase() == "edit") {
	        top.main.TabFrame.focus();
	        win = top.main.TabFrame;
	    } else {
	        parent.frames["main"].focus();
	        win = parent.frames["main"];
	    }
		
	    if (/Chrome/.test(navigator.userAgent) && /Google Inc/.test(navigator.vendor)) {
	        win.print();
	    } else {
	        window.print();
	    }
	}
	formgroupflag = MainWindow.formgroupflag
	var formgroup = MainWindow.formgroup
	uniqueid = MainWindow.uniqueid
	mainpage_enabled = MainWindow.mainpage_enabled
	//this is reg only, but don't remove
	commit_type = MainWindow.commit_type

	//defaults
	noNav = false
	noSupInfo = false
	nav_override =  "<%=nav_override%>"
	if (nav_override == "true"){
		formmode = "<%=request("formmode")%>"
		noNav = true
	}
	else{
		formmode = MainWindow.formmode;
	}
	
	if( formmode )
	{
	    formmode = formmode.toLowerCase()
	}
	else
	{
	    formmode = "";
	}
	
	if (formmode.toLowerCase() == "manage_users"){
		formmode = "edit_record"
		formgroupflag="user_security"
		noSupInfo = true
		noNav = false
	}
	if (formmode.toLowerCase() == "manage_users_new"){
		formmode = "add_record"
		formgroupflag = "user_security"
		noSupInfo = true
		noNav = false
	}
	
	//add formmode of incoming link to catch
	if (formmode.toLowerCase() == "add_record"){
	//change to another standard cows formmode or continue using custom one. Set banner to display using formmode. The standard is add_record
		formmode = "add_record"
	//set formgroupflag so you can specify a custom button building function
		formgroupflag = "add_record"
	//specify whether you want database cound and comments shown a left of navbar
		noSupInfo = false
	//specify whether to show navbar at all.
		noNav = false
	}
	
	if(formmode.toLowerCase() == "edit_record"){
		formmode =  "edit_record"
		formgroupflag = "edit_record"
		noSupInfo = false
		noNav = false
	}
	
	var hitCount = MainWindow.totalrecords;
	var recCount = MainWindow.db_record_count;
</script> 


<title>Nav bar</title>
<SCRIPT LANGUAGE=javascript src="/cheminv/Choosecss.js"></SCRIPT>
</head>

<body <%=Application("BODY_BACKGROUND")%>>

<table border="0" cellspacing="0" cellpadding="0">	
	<tr>
		<td>
			<img src="graphics/pixel.gif" width="10" height="1" alt border="0">					
		</td>
		<td>
			<table border="0" width="860" cellspacing="1">
				<tr>
					<td valign="top" align="left" nowrap>
						<%
						if Not nav_override = "true" then
							buildMenuBar request("formmode"),request("dbname"), request("formgroup") 
						end if
						%>
					</td>
					<td align="left" width="200" nowrap>
					</td>
				</tr>
				<tr>
					<td  valign="top" colspan="2" nowrap>
						<table border="0" bordercolor="#666666" width="620">
							<tr>
								<td nowrap>
									<script language="JavaScript">
									if (noNav ==false){ 
										if ((formgroupflag.toLowerCase() == "single_search")||(formgroupflag.toLowerCase() == "search")||(formgroupflag.toLowerCase() == "global_search")){
											//standard cows buttons for this formmode - comment them out and add one at a time via above function if you need to modify
												if (formmode.toLowerCase() == "search"){
													if (MainWindow.document.cows_input_form.checkbox1){ 
														if ((formgroup == "plates_form_group") || (formgroup == "plate_compounds_form_group")){
															chkfg = "plate_compounds_form_group";
															unchkfg = "plates_form_group";
														}
														else{
															chkfg = "base_form_group";
															unchkfg = "containers_form_group";	
														}
														document.write (MainWindow.getButton("search", MainWindow.document.cows_input_form.checkbox1.checked ? chkfg : unchkfg));														
													}
													else{
														document.write (MainWindow.getButton("search"));
													}
													if (MainWindow.document.cows_input_form["inv_compounds.Conflicting_Fields"] && !(MainWindow.document.cows_input_form["inv_Locations.Location_Barcode"])) 
													{
														document.write (MainWindow.getButtons("Except:search"));
													}
													else
													{
														document.write (MainWindow.getButtons("Except:search,retrieve_all"));
													}
												}
												else{
													document.write (MainWindow.getButtons("Except:retrieve_all"));
												}
												//goes to secure_nav.asp to get custom buttons for this formmode of this application
												document.write (MainWindow.getCustomButtons())
										
										}
										
										if (formgroupflag.toLowerCase() == "add_record"){
											document.write (MainWindow.getAddRecordButtons())
											if(mainpage_enabled==1){
												document.write ('<a href="/<%=Application("AppKey")%>/<%=dbkey%>/mainpage.asp?dbname=<%=dbkey%>&Timer='+ "<%=Timer%>" + '"  target=_top><img src="/<%=Application("appkey")%>/graphics/mainMenu.gif" border="0"></a>')
											}									
										}
										
										if (formgroupflag.toLowerCase() == "edit_record"){
										
											document.write (MainWindow.getCustomButtons())
													
										}
		
		
									 }  //end if(noNav ==false)
								</script>
								<!--<a href="javascript:parent.window.close();"><img src="graphics/cancel_dialog_btn.gif" border="0" alt="Cancel" width="61" height="21"></a>-->
								<script language="JavaScript">
								var cancel_action = "";
								if (top.opener) {
									cancel_action = "parent.window.close();";
								}else{
									cancel_action = "parent.location.href='<%=cancel_url%>';";
								}
								//document.write("<a href=\"javascript:" + cancel_action + "\"><img src=\"graphics/cancel_dialog_btn.gif\" border=\"0\" alt=\"Cancel\" width=\"61\" height=\"21\"></a>");
								document.write("<img src=\"graphics/cancel_dialog_btn.gif\" border=\"0\" alt=\"Cancel\" width=\"61\" height=\"21\" style=\"cursor:hand\" onclick=\"" + cancel_action + "\">");								
								</script>
								</TD>
								<TD align="right">
								<script language="JavaScript">
								
								if ((formmode.toLowerCase() == "search") || (formmode.toLowerCase() == "add_record")){
									
									if ((formgroup.toLowerCase() == "containers_form_group") || (formgroup.toLowerCase() == "containers_np_form_group") ) {
										document.write ("Total Containers:&nbsp;" + recCount);
									}
									else if ((formgroup.toLowerCase() == "substances_form_group")||(formgroup.toLowerCase() == "base_form_group")){
										document.write ("Total Substances:&nbsp;" + recCount);
									}
									else if ((formgroup.toLowerCase() == "plates_form_group")){
										document.write ("Total Plates:&nbsp;" + recCount);
									}
								}
								else{
									if ((formgroup.toLowerCase() == "containers_form_group") || (formgroup.toLowerCase() == "containers_np_form_group") ) {
										document.write ("Containers matching your query:&nbsp;" + hitCount);
									}
									else if ((formgroup.toLowerCase() == "substances_form_group")||(formgroup.toLowerCase() == "base_form_group")||(formgroup.toLowerCase() == "plate_compounds_form_group")){
										document.write ("Substances matching your query:&nbsp;" + hitCount);
									}
									else if ((formgroup.toLowerCase() == "plates_form_group")){
										document.write ("Plates matching your query:&nbsp;" + hitCount);
									}
								}
								</script>
							</td>
						</tr>
					</table>
				</td>
			</tr>
			<tr>
				<td width="650" valign="top" colspan="2">
					<table border="0" cellspacing="3">
						<tr>
							<td>
							<nobr>
								<script language="JavaScript">
									if (!((formgroup.toLowerCase() == "containers_form_group") || (formgroup.toLowerCase() == "containers_np_form_group") || (formgroup.toLowerCase() == "plates_form_group") || (formgroup.toLowerCase() == "batches_form_group") || (formgroup.toLowerCase() == "plates_np_form_group"))){
										//this cals the navigation buttons for navigating the record set
										if ((formmode.toLowerCase() == "list")||(formmode.toLowerCase() == "edit")){	
											
											//if (hitCount > 1) {
												document.write (MainWindow.getRecordNav(""));
											//}
											//else{
											//	document.write (MainWindow.getRecordNav("Except:resultview"));
											//}
										}

									}
									else{
										if (formmode.toLowerCase() == "edit"){	
											if (hitCount > 1) {
												document.write (MainWindow.getRecordNav(""));
											}
											else{
												document.write (MainWindow.getRecordNav("Except:resultview"));
											}
										}
									}
								</script>
								</nobr>
								
							</td>
						</tr>
						<tr>
							<td>
								
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
