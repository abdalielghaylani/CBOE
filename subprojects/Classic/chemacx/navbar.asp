<%@ LANGUAGE="VBScript" %>

<%Response.Expires=0%>
<html>

<head>
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/menubar.asp"-->
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
<SCRIPT LANGUAGE=javascript src="/chemacx/Choosecss.js"></SCRIPT>
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
					<td width="500" valign="top" align="left" nowrap>
						<%
						if Not nav_override = "true" then
							buildMenuBar request("formmode"),request("dbname"), request("formgroup") 
						end if
						%>
						<script language="JavaScript">
						    //if(noNav==false){
						    //if you need to add something to fixed buttons
						    //add a funciton to secure_nav.asp and call from here

						    //document.write('<A href=# onclick="MainWindow.ACXPrintCurrentPage()" border="0"><IMG border=0 src="' + MainWindow.button_gif_path + 'blueprint_btn.gif"></a>')
						    //document.write (MainWindow.getFixedButtons("Except:Print"));
						    //}
						</script>
					</td>
					<td align="left" width="200" nowrap>
					</td>
				</tr>
				<tr>
					<td width="650" valign="top" colspan="2" nowrap>
						<table border="0" bordercolor="Navy">
							<tr>
								<td nowrap>
									<script language="JavaScript">
									    if (noNav == false) { //meaning show the navbar please
									        //this is where the buttons are called.  It is based on the formgroupflag in the ini file for the particular formgroup.
									        //if you want to override the formgroupflag for whatever reason do so at the top of the file
									        //add whatever formgroupflags you need to this section
									        if ((formgroupflag.toLowerCase() == "single_search") || (formgroupflag.toLowerCase() == "search") || (formgroupflag.toLowerCase() == "global_search")) {
									            //standard cows buttons for this formmode - comment them out and add one at a time via above function if you need to modify
									            if ((formgroup == "base_form_group") || (formgroup == "acxml")) {
									                if (formmode.toLowerCase() == "search") {
									                    document.write('<a href="#" onclick="MainWindow.showProcessingImage();MainWindow.ACXSearch();return false"><img src="<%=Application("NavButtonGifPath")%>search_btn.gif" alt="Search" border="0"></a>')
									                }
									                document.write(MainWindow.getButtons("Except:search,retrieve_all"))
									            }
									            else {
									                if (formmode.toLowerCase() == "search") {
									                    document.write('<a href="#" onclick="MainWindow.showProcessingImage();MainWindow.ACXSearch();return false"><img src="<%=Application("NavButtonGifPath")%>search_btn.gif" alt="Search" border="0"></a>')
									                }
									                document.write(MainWindow.getButtons("Except:search,refine,edit_query,retrieve_all"));
									            }

									            //goes to secure_nav.asp to get custom buttons for this formmode of this application
									            document.write(MainWindow.getCustomButtons())

									            if (mainpage_enabled == 1) {
									                document.write('<a href="/<%=Application("AppKey")%>/<%=dbkey%>/mainpage.asp?dbname=<%=dbkey%>&Timer=' + "<%=Timer%>" + '"  target=_top><img src="/<%=Application("appkey")%>/graphics/mainMenu.gif" border="0"></a>')
									            }
									        }


									        if (formgroupflag.toLowerCase() == "manage_tables") {
									            document.write(MainWindow.getRegManageTablesButtons())
									            if (mainpage_enabled == 1) {
									                document.write('<a href="/<%=Application("AppKey")%>/<%=dbkey%>/mainpage.asp?dbname=<%=dbkey%>&Timer=' + "<%=Timer%>" + '"  target=_top><img src="/<%=Application("appkey")%>/graphics/mainMenu.gif" border="0"></a>')
									            }
									        }

									        if (formgroupflag.toLowerCase() == "user_security") {
									            document.write(MainWindow.getRegManageUsersButtons())
									            if (mainpage_enabled == 1) {
									                document.write('<a href="/<%=Application("AppKey")%>/<%=dbkey%>/mainpage.asp?dbname=<%=dbkey%>&Timer=' + "<%=Timer%>" + '"  target=_top><img src="/<%=Application("appkey")%>/graphics/mainMenu.gif" border="0"></a>')
									            } 
									        }

									        if (formgroupflag.toLowerCase() == "role_security") {
									            document.write(MainWindow.getRegManageRolesButtons())
									            if (mainpage_enabled == 1) {
									                document.write('<a href="/<%=Application("AppKey")%>/<%=dbkey%>/mainpage.asp?dbname=<%=dbkey%>&Timer=' + "<%=Timer%>" + '"  target=_top><img src="/<%=Application("appkey")%>/graphics/mainMenu.gif" border="0"></a>')
									            }
									        }
									        if (formgroupflag.toLowerCase() == "add_record") {
									            document.write(MainWindow.getAddRecordButtons())
									            if (mainpage_enabled == 1) {
									                document.write('<a href="/<%=Application("AppKey")%>/<%=dbkey%>/mainpage.asp?dbname=<%=dbkey%>&Timer=' + "<%=Timer%>" + '"  target=_top><img src="/<%=Application("appkey")%>/graphics/mainMenu.gif" border="0"></a>')
									            }
									        }

									        if (formgroupflag.toLowerCase() == "edit_record") {

									            document.write(MainWindow.getCustomButtons())
									            if (mainpage_enabled == 1) {
									                document.write('<a href="/<%=Application("AppKey")%>/<%=dbkey%>/mainpage.asp?dbname=<%=dbkey%>&Timer=' + "<%=Timer%>" + '"  target=_top><img src="/<%=Application("appkey")%>/graphics/mainMenu.gif" border="0"></a>')
									            }
									        }


									    }  //end if(noNav ==false)
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
								<script language="JavaScript">
								var hitCount = "<%=Session("hitlistRecordCount" & dbkey & formgroup)%>";
								var recCount = MainWindow.db_record_count
								if ((formmode.toLowerCase() == "search") || (formmode.toLowerCase() == "add_record")){
										document.write ("Total Substances:&nbsp;" + format(recCount));
								}
								else
                                				{
									document.write ("Substances matching your query:&nbsp;" + format(hitCount));
								}

                                function format(num){
    						var n = num.toString(), p = n.indexOf('.');
    						return n.replace(/\d(?=(?:\d{3})+(?:\.|$))/g, function($0, i){
        					return p<0 || i<p ? ($0+',') : $0;
    						});
						}
								</script>
							</td>
						</tr>
						<tr>
							<td>
								<nobr>
								<script language="JavaScript">
								    if (noNav == false) {
								        //this cals the navigation buttons for navigating the record set
								        if ((formmode.toLowerCase() == "list") || (formmode.toLowerCase() == "edit")) {
								            if (hitCount > 1) {
								                document.write(MainWindow.getRecordNav());
								            }
								            else {
								                document.write(MainWindow.getRecordNav("Except:resultview"));
								            }
								        }

								    }
								</script>
								</nobr>
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
