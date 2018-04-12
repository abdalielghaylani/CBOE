<%@ LANGUAGE="VBScript" %>
<%Response.Expires=0%>
<html>
<head>
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/menubar.asp"-->

<%'Copyright 1999-2003 CambridgeSoft Corporation. All rights reserved
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
	var MainWindow = <%=Application("MainWindow")%>
	formgroupflag = MainWindow.formgroupflag
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
		formmode = MainWindow.formmode
	}
	
	formmode = formmode.toLowerCase()
	
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
</script> 


<title>Nav bar</title>
</head>

<body>

<table border="0">
	<tr><td><table border="0" cellspacing="0" cellpadding="0">
				<tr><td valign="top">
        <img src="/cfserverasp/source/graphics/navbuttons/logo_cows_250.gif"></font>&nbsp;&nbsp;</td>
        
					
					<td>
                    <table align="left" cellspacing="0" height="20" border="0" width="300">
                      <tr>
                        <td width="150" align="center" nowrap> <strong>	<font face="Arial" color="#42426f" size="4">
                        
                        <i>               
                        <% Select Case UCase(request("formmode"))
                       case "SEARCH"
                       		response.write " Query Input Form"
                       case "EDIT"
                       		response.write " Result Detail View"
                       case "EDIT_RECORD"
                       		response.write " Edit Record Form"
                       case "EDIT_RECORD"
                       		response.write " Add Record Input Form"
                       case "LIST"
                      		 response.write " Result List View"
                       end select%></i></strong></font>
                   </td> </font>
                      </tr>
                    </table>
					</td>
				</tr>
			</table>
		</td>
	</tr>
	
	<tr><td><table border="0" width="860" cellspacing="1">
				<tr>
					<td width="450" valign="top" align="left" nowrap>
					<%if Not nav_override = "true" then
						buildMenuBar request("formmode"),request("dbname"), request("formgroup")
					end if %>
						
					</td>
		
					<td align="left" width="200" nowrap>
						<font face="Arial" color="#42426f" size="2">
							<b>
							<%
							if Application("LoginRequired" &dbkey)=1 then
								Response.Write "USER: " & UCase(Session("UserName" & dbkey))
							end if
							%>
							</b>
						</font>
					</td>
				</tr>
      
				<tr><td width="650" valign="top" colspan="2" nowrap>
						<table border="0" bordercolor="Navy">
							<tr><td nowrap>
									<script language="JavaScript">
									<!--
										
									if (noNav ==false){ //meaning show the navbar please
										 //this is where the buttons are called.  It is based on the formgroupflag in the ini file for the particular formgroup.
										 //if you want to override the formgroupflag for whatever reason do so at the top of the file
										 //add whatever formgroupflags you need to this section
										if ((formgroupflag.toLowerCase() == "single_search")||(formgroupflag.toLowerCase() == "search")){
											
										
											//standard cows buttons for this formmode - comment them out and add one at a time via above function if you need to modify
											document.write (MainWindow.getButtons());
											
											//goes to secure_nav.asp to get custom buttons for this formmode of this application
											document.write (MainWindow.getCustomButtons())
										
											if(mainpage_enabled==1){
												document.write ('<a href="/<%=Application("AppKey")%>/<%=dbkey%>/mainpage.asp?dbname=<%=dbkey%>&Timer='+ "<%=Timer%>" + '"  target=_top><img src="/<%=Application("appkey")%>/graphics/mainMenu.gif" border="0"></a>')
											}
										}
		
										if (formgroupflag.toLowerCase() == "manage_tables"){
											document.write (MainWindow.getRegManageTablesButtons())
											if(mainpage_enabled==1){
												document.write ('<a href="/<%=Application("AppKey")%>/<%=dbkey%>/mainpage.asp?dbname=<%=dbkey%>&Timer='+ "<%=Timer%>" + '"  target=_top><img src="/<%=Application("appkey")%>/graphics/mainMenu.gif" border="0"></a>')
											}
										}
		
										if (formgroupflag.toLowerCase() == "user_security"){
											document.write (MainWindow.getRegManageUsersButtons())
											if(mainpage_enabled==1){
												document.write ('<a href="/<%=Application("AppKey")%>/<%=dbkey%>/mainpage.asp?dbname=<%=dbkey%>&Timer='+ "<%=Timer%>" + '"  target=_top><img src="/<%=Application("appkey")%>/graphics/mainMenu.gif" border="0"></a>')
											}										}
		
										if (formgroupflag.toLowerCase() == "role_security"){
											document.write (MainWindow.getRegManageRolesButtons())
											if(mainpage_enabled==1){
												document.write ('<a href="/<%=Application("AppKey")%>/<%=dbkey%>/mainpage.asp?dbname=<%=dbkey%>&Timer='+ "<%=Timer%>" + '"  target=_top><img src="/<%=Application("appkey")%>/graphics/mainMenu.gif" border="0"></a>')
											}									
										}
										if (formgroupflag.toLowerCase() == "add_record"){
											document.write (MainWindow.getAddRecordButtons())
											if(mainpage_enabled==1){
												document.write ('<a href="/<%=Application("AppKey")%>/<%=dbkey%>/mainpage.asp?dbname=<%=dbkey%>&Timer='+ "<%=Timer%>" + '"  target=_top><img src="/<%=Application("appkey")%>/graphics/mainMenu.gif" border="0"></a>')
											}									
										}
										
										if (formgroupflag.toLowerCase() == "edit_record"){
										
											document.write (MainWindow.getCustomButtons())
											if(mainpage_enabled==1){
												document.write ('<a href="/<%=Application("AppKey")%>/<%=dbkey%>/mainpage.asp?dbname=<%=dbkey%>&Timer='+ "<%=Timer%>" + '"  target=_top><img src="/<%=Application("appkey")%>/graphics/mainMenu.gif" border="0"></a>')
											}									
										}
		
		
									 }  //end if(noNav ==false)
									// -->
								</script>
							</td>
						</tr>
					</table>
				</td>
			</tr>
    
			<tr><td width="650" valign="top" colspan="2">
					<table border="0" cellspacing="3">
						<tr><td><nobr>
								<script language="JavaScript">
									<!--
									var hitCount = "<%=Session("hitlistRecordCount" & dbkey & formgroup)%>";
									if(noNav ==false){
										//this cals the navigation buttons for navigating the record set
										document.write ('<td valign = "center"><font face="Arial" size = "2" color="#42426f">' + MainWindow.getRecordNav() + '</font></td>');
										
										if ((formmode.toLowerCase() == "search") || (formmode.toLowerCase() == "add_record")){
											document.write ('<td valign = "center"><font face="Arial" size = "2" color="#42426f">&nbsp;&nbsp;&nbsp;Total Searchable Records: ' + MainWindow.db_record_count + '</font></td>');
										}
										else{
											document.write ('<td valign = "center"><font face="Arial" size = "2" color="#42426f">&nbsp;&nbsp;&nbsp;Records matching your query:&nbsp;' + hitCount+ '</font></td>');
										}
										
									}

									// -->
								</script>
							</nobr></td>
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
