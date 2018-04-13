<%@ LANGUAGE="VBScript" %>
<%Response.Expires=0
'Copyright 1999-2003 CambridgeSoft Corporation. All rights reserved
'DO NOT EDIT THIS FILE%>
<html>
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/menubar.asp"-->
<!--#INCLUDE VIRTUAL = "/chem_reg/source/menu_functions_vbs.asp"-->

<head>
<%
Dim nv_DEBUG
nv_DEBUG=False
commit_type = Request.QueryString("commit_type")
nav_override= Request.QueryString("nav_override")
' formmode = Request.QueryString("formmode")
dbkey = Request.QueryString("dbname")
%>
<script language="javascript">
//SYAN added on 6/21/2005 to fix CSBR-55876
var formmode = "";
var formgroupflag = "";
//End of SYAN modification

function getCookie(name){
	var cname = name  + "=";
	var dc= document.cookie;
	if(dc.length > 0){
		begin = dc.indexOf(cname);
			if(begin != -1){
				begin += cname.length;
				end = dc.indexOf(";",begin);
					if(end == -1) end = dc.length;
						 temp = unescape(dc.substring(begin,end));
						 theResult = temp
						 
						  return theResult
			}
		}
	return null;	
}	

	var MainWindow = <%=Application("MainWindow")%>
	formgroupflag = MainWindow.formgroupflag
	formgroup = MainWindow.formgroup
	uniqueid = MainWindow.uniqueid
	commit_type = MainWindow.commit_type
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
	if (formgroupflag.toLowerCase()=="manage_analytics"){
		noSupInfo=true
		noNav=false
	}
	if (formgroupflag.toLowerCase()=="add_analytics"){
		noSupInfo=true
		noNav=false
	}
	if (formmode.toLowerCase()=="get_registered"){
		noSupInfo=true
		noNav = true
		formmode = "add_record"
	}
	if (formmode.toLowerCase()=="manage_users"){
		formmode = "edit_record"
		formgroupflag="user_security"
		noSupInfo=true
		noNav = false
	}
	if (formmode.toLowerCase()=="manage_passwords"){
		formmode = "edit_record"
		formgroupflag="password_security"
		noSupInfo=true
		noNav = false
	}
	
	if (formmode.toLowerCase()=="manage_users_new"){
		formmode = "add_record"
		formgroupflag="user_security"
		noSupInfo=true
		noNav = false
	}

	if ((commit_type=="add_identifiers")||(commit_type=="batch_commit")||(commit_type=="add_salt")){
		formmode="add_compounds"
		}
		
	var excludeButtonList = ""
	var button_override=MainWindow.button_override
	if (button_override.length > 0) {
		if (excludeButtonList.length>0){
			excludeButtonList = excludeButtonList + "," + button_override.toLowerCase()
		}
		else{
			excludeButtonList =  button_override.toLowerCase()
		}
	}
	
</script> 


<title>Nav bar</title>
</head>

<body <%=Application("BODY_BACKGROUND")%>>

<table border="0" bordercolor="Red">
  <tr>
    <td><table border="0" cellspacing="0" cellpadding="0" height="44">
      <tr>
        <td valign="top"><font face="verdana">
        <i><img src="/chem_reg/graphics/logo_regsys_250.gif" align="center"></i></font>&nbsp;&nbsp;&nbsp;</td>
		 <td  >
                    <table  align= "left" cellspacing="0" height="20" border="0"  width = "500">
                      <tr>
                        <td width=""150"  align = "center" nowrap><i><font face = "Arial"   color="#42426f" size="4">
                        
                   <script language="javascript">
        
 
				var banner_text
  
				if (formmode.toLowerCase() == "search"){
						
						if (formgroupflag.toLowerCase() == "reg_commit"){
							banner_text = "Register Compounds Query Input Form"
						}
					
						else{
							banner_text ="Query Input Form"
						}
						
						
				}
				if (formmode.toLowerCase() == "analytics_data"){
					
					banner_text ="Query Input Form"
					
				}
				if (formmode.toLowerCase() == "manage_analytics"){
					
					banner_text ="Results Form View"
					
				}
				if (formmode.toLowerCase() == "ma_add"){
					formgroupflag ="ma_pares"
					banner_text ="Results Form View"
					//imgsrc = buttonGifPath + "maaform_bnr.gif"
					
				}
				if (formmode.toLowerCase() == "ma_delete"){
					formgroupflag ="ma_pares"
					banner_text ="Results Form View"
					//imgsrc = buttonGifPath + "madform_bnr.gif"
					
				}
				if (formmode.toLowerCase() == "sartable"){
					
					banner_text ="BioReg SAR Table"
					
				}
				if (formmode.toLowerCase() == "sartable_list"){
					
						banner_text ="BioReg SAR Table"
					
				}
				if (formmode.toLowerCase() == "edit_registry"){
					
						banner_text ="Edit Record Form"
					
				}
	
				if ((formmode.toLowerCase() == "manage_tables")||(formmode.toLowerCase() =="manage_analytics_tables")||(formmode.toLowerCase() =="manage_reg_tables")){
					noSupInfo=true
					banner_text ="Manage Tables"
					
				}
				if (formmode.toLowerCase() == "register"){
					banner_text ="Register Compounds Query Form"
				}
    
				if (formmode.toLowerCase() == "add_compounds"){
					if (formgroupflag.toLowerCase() == "reg_temp"){
					
						banner_text ="Add Records Input Form"
					}
					else{
						banner_text ="Add Records Input Form"
					}
				}
				if (formmode.toLowerCase() == "add_record"){
    
				 if ((formgroupflag.toLowerCase() =="user_security")||(formgroupflag.toLowerCase() =="role_security")){
						noSupInfo=true
						
						if ((formgroupflag.toLowerCase()=="manage_tables")||(formgroupflag.toLowerCase()=="manage_reg_tables")){
							noSupInfo=true
							banner_text ="Manage Tables"
							}
						else{
   
							if (formgroupflag.toLowerCase() == "reg_temp"){
								noSupInfo=true
								banner_text ="Add Records Input Form"
							}
							else{
								noSupInfo=true
								banner_text ="Add Records Input Form"
							}
						}
					}else
					{
					banner_text =" "
					}
				}
	

				if (formmode.toLowerCase() == "refine"){
					 banner_text ="Refine Query Form"
				}
				if (formmode.toLowerCase() == "edit"){
					if (formgroupflag.toLowerCase() == "reg_commit"){
						 banner_text ="Register Compounds Result Form"
					}
					else{
					 banner_text ="Results Form View"
					}
				}
    
				 if (formmode.toLowerCase() == "edit_record"){
					
						
						if ((formgroupflag.toLowerCase() =="manage_tables")|| (formgroupflag.toLowerCase() =="manage_reg_tables")){
							noSupInfo=true
							 banner_text ="Manage Tables"
							}
						else{
   
							if (formgroupflag.toLowerCase()== "reg_commit"){
								 banner_text ="Register Compounds Result Form"
							}
							else{
								 banner_text ="Results Form View"
							}
						}
					
				}
	
				if (formmode.toLowerCase() == "no_nav_view"){
					noSupInfo=true
					 banner_text ="Results Form View"
				}
    
				if (formmode.toLowerCase() == "list"){
					
						if ((formgroupflag.toLowerCase() =="manage_tables")||(formgroupflag.toLowerCase() =="manage_reg_tables")||(formgroupflag.toLowerCase() =="manage_analytical_tables")){
							noSupInfo=true
							 banner_text ="Manage Tables"
							}
						else{
   
							if (formgroupflag.toLowerCase() == "reg_commit"){
								 banner_text ="Register Compounds Result List"
							}
							else{
								banner_text ="Results List View"
							}
						}
					
				}
				 document.write ("&nbsp;&nbsp;" + banner_text)
    </script></td>
    <td width="300" align="left" border="0" nowrap><font face = "Arial"   color="#42426f" size = "2">&nbsp;<%
								Response.Write "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Current Login: &nbsp;" &  UCase(Session("UserName" & request("dbname")))
							%>&nbsp;&nbsp;</td></font>
	     
                   </td> </font>
                        
                      </tr>
                    </table>
					</td>
				</tr>
			</table>  <td>
      </tr>
    </table>
    </td>
  </tr>
  <tr>
    <td>
    <table border="0" width="860" cellspacing="1">
      <tr>
       
       <td width="450" valign="top" align="left" nowrap>
					
					<%if Not nav_override = "true" then
						buildMenuBar request("formmode"),request("dbname"), request("formgroup")
					end if %>
					</td>
					 <td rowspan="3" width="150" valign="top" nowrap>
			<font face="Arial" color="#42426f" size="2">
			
			<%If nv_DEBUG=True Then%>
			INI version : <%=Application("IniVersion" & dbkey)%>
			<%End If%>
			</font>
		</td>
		
      </tr>
      <tr>
        <td width="750" valign="top" colspan="2" nowrap>
			<table border="0" bordercolor="Navy">
				<tr>
					<td nowrap><script language="JavaScript"><!--
        if (noNav ==false){
         
        if (formgroupflag.toLowerCase() == "reg_temp"){
       
			document.write (MainWindow.getRegTempButtons())
			if (excludeButtonList.indexOf("main_menu") == -1) document.write ('<a href=/<%=Application("AppKey")%>/<%=dbkey%>/mainpage.asp?dbname=<%=dbkey%>&Timer='+ "<%=Timer%>" + '  target=_top><img src="/<%=Application("appkey")%>/graphics/mainMenu.gif" border="0"></a>')
		}
	
		if (formgroupflag.toLowerCase() == "reg_commit"){
			document.write (MainWindow.getRegCommitButtons())
			if((formmode.toLowerCase() != "edit_record")&&(formmode.toLowerCase() != "add_record")&&(formmode.toLowerCase() != "add_compounds")){
				if (formgroup.toLowerCase() != "review_register_form_group"){
					document.write (MainWindow.getButtons());
				}
				else{
					document.write (MainWindow.getButtons("new_search,edit_query,no_refine_dialog"));
				}
				
			}
			if (excludeButtonList.indexOf("main_menu") == -1) document.write ('<a href="/<%=Application("AppKey")%>/<%=dbkey%>/mainpage.asp?dbname=<%=dbkey%>&Timer='+ "<%=Timer%>" + '"  target=_top><img src="/<%=Application("appkey")%>/graphics/mainMenu.gif" border="0"></a>')
		}

		if (formgroupflag.toLowerCase() == "reg_search"){
			
			document.write (MainWindow.getRegSearchButtons())
			if((formmode != "edit_record")&&(formmode != "add_record")&&(formmode != "add_compounds")){
					if (formgroup.toLowerCase() != "approve_form_group"){
						if (excludeButtonList.indexOf("main_all") == -1) document.write (MainWindow.getButtons());
					}
					else{
						document.write (MainWindow.getButtons("new_search,edit_query,no_refine_dialog"));
					}
			}
			if (excludeButtonList.indexOf("main_menu") == -1) document.write ('<a href="/<%=Application("AppKey")%>/<%=dbkey%>/mainpage.asp?dbname=<%=dbkey%>&Timer='+ "<%=Timer%>" + '"  target=_top><img src="/<%=Application("appkey")%>/graphics/mainMenu.gif" border="0"></a>')
		}
		
		if (formgroupflag.toLowerCase() == "manage_tables"){
			document.write (MainWindow.getRegManageTablesButtons())
			document.write ('<a href="/<%=Application("AppKey")%>/<%=dbkey%>/mainpage.asp?dbname=<%=dbkey%>&Timer='+ "<%=Timer%>" + '"  target=_top><img src="/<%=Application("appkey")%>/graphics/mainMenu.gif" border="0"></a>')
		}
		
		if (formgroupflag.toLowerCase() == "manage_reg_tables"){
		
			document.write (MainWindow.getRegManageRegTablesButtons())
			document.write ('<a href="/<%=Application("AppKey")%>/<%=dbkey%>/mainpage.asp?dbname=<%=dbkey%>&Timer='+ "<%=Timer%>" + '"  target=_top><img src="/<%=Application("appkey")%>/graphics/mainMenu.gif" border="0"></a>')
		}
		
		if (formgroupflag.toLowerCase() == "user_security"){
				document.write (MainWindow.getRegManageUsersButtons())
				document.write ('<a href="/<%=Application("AppKey")%>/<%=dbkey%>/mainpage.asp?dbname=<%=dbkey%>&Timer='+ "<%=Timer%>" + '"  target=_top><img src="/<%=Application("appkey")%>/graphics/mainMenu.gif" border="0"></a>')
		}
		if (formgroupflag.toLowerCase() == "password_security"){
				document.write (MainWindow.getRegManagePasswordsButtons())
				document.write ('<a href="/<%=Application("AppKey")%>/<%=dbkey%>/mainpage.asp?dbname=<%=dbkey%>&Timer='+ "<%=Timer%>" + '"  target=_top><img src="/<%=Application("appkey")%>/graphics/mainMenu.gif" border="0"></a>')
		}
		
		if (formgroupflag.toLowerCase() == "role_security"){
			document.write (MainWindow.getRegManageRolesButtons())
			document.write ('<a href="/<%=Application("AppKey")%>/<%=dbkey%>/mainpage.asp?dbname=<%=dbkey%>&Timer='+ "<%=Timer%>" + '"  target=_top><img src="/<%=Application("appkey")%>/graphics/mainMenu.gif" border="0"></a>')
		}
		//Start Givaudan EVAL customization
		if (formgroupflag.toLowerCase() == "add_eval_data"){
				document.write (MainWindow.getAddEvalDataButtons())
				document.write ('<a href="/<%=Application("AppKey")%>/<%=dbkey%>/mainpage.asp?dbname=<%=dbkey%>&Timer='+ "<%=Timer%>" + '"  target=_top><img src="/<%=Application("appkey")%>/graphics/mainMenu.gif" border="0"></a>')
				
		}
		if (formgroupflag.toLowerCase() == "add_eval_data2"){
				document.write (MainWindow.getAddEvalDataButtons2())
				document.write ('<a href="/<%=Application("AppKey")%>/<%=dbkey%>/mainpage.asp?dbname=<%=dbkey%>&Timer='+ "<%=Timer%>" + '"  target=_top><img src="/<%=Application("appkey")%>/graphics/mainMenu.gif" border="0"></a>')
			
		}
		
		
		if (formgroupflag.toLowerCase() == "eval_data"){
				document.write (MainWindow.getEvalDataButtons())
				document.write ('<a href="/<%=Application("AppKey")%>/<%=dbkey%>/mainpage.asp?dbname=<%=dbkey%>&Timer='+ "<%=Timer%>" + '"  target=_top><img src="/<%=Application("appkey")%>/graphics/mainMenu.gif" border="0"></a>')
				
		}
		//End  Givaudan EVAL customization
		if (formgroupflag.toLowerCase() == "manage_analytics"){
		
			if(formmode.toLowerCase()=="analytics_data"){
				
				document.write ('<a href="/<%=Application("AppKey")%>/<%=dbkey%>/mainpage.asp?dbname=<%=dbkey%>&Timer='+ "<%=Timer%>" + '"  target=_top><img src="/<%=Application("appkey")%>/graphics/mainMenu.gif" border="0"></a>')

				}
				else{
					if(formmode.toLowerCase()=="add_analytics_data"){
				
						document.write ('<a href="/<%=Application("AppKey")%>/<%=dbkey%>/mainpage.asp?dbname=<%=dbkey%>&Timer='+ "<%=Timer%>" + '"  target=_top><img src="/<%=Application("appkey")%>/graphics/mainMenu.gif" border="0"></a>')
						document.write (MainWindow.getAnalyticsButtons())

					}
					else{
						noNav=true
						document.write (MainWindow.getAnalyticsButtons())
						document.write ('<a href="/<%=Application("AppKey")%>/<%=dbkey%>/mainpage.asp?dbname=<%=dbkey%>&Timer='+ "<%=Timer%>" + '"  target=_top><img src="/<%=Application("appkey")%>/graphics/mainMenu.gif" border="0"></a>')
				
					}
				}
			
		}
		if (formgroupflag.toLowerCase() == "manage_analytical_tables"){
			
			document.write (MainWindow.getRegManageAnalyticsTablesButtons())
			document.write ('<a href="/<%=Application("AppKey")%>/<%=dbkey%>/mainpage.asp?dbname=<%=dbkey%>&Timer='+ "<%=Timer%>" + '"  target=_top><img src="/<%=Application("appkey")%>/graphics/mainMenu.gif" border="0"></a>')
		}
		   
		if (formgroupflag.toLowerCase() == "sartable") {
			document.write (MainWindow.getSARTableButtons())
			document.write ('<a href="/<%=Application("AppKey")%>/<%=dbkey%>/mainpage.asp?dbname=<%=dbkey%>&Timer='+ "<%=Timer%>" + '"  target=_top><img src="/<%=Application("appkey")%>/graphics/mainMenu.gif" border="0"></a>')
		}
        if (formgroupflag.toLowerCase() == "ma_pares"){
     
			document.write (MainWindow.getMA_PaResButtons())
			document.write ('<a href="/<%=Application("AppKey")%>/<%=dbkey%>/mainpage.asp?dbname=<%=dbkey%>&Timer='+ "<%=Timer%>" + '"  target=_top><img src="/<%=Application("appkey")%>/graphics/mainMenu.gif" border="0"></a>')
		}
 }
// --></script>
		</td>
        </tr>
        </table>
      </td>
    </tr>
    <tr>
        <td width="800" valign="top" colspan="2"><table border="0" cellspacing="3">
          <tr>
            <td valign="top"><script language="JavaScript">
      <!--
      var hitCount = "<%=Session("hitlistRecordCount" & request("dbname") & request("formgroup"))%>";
if(noNav ==false){
	document.write (MainWindow.getRecordNav());
	
	if ((formgroup.toLowerCase()=="base_form_group") ||(formgroup.toLowerCase()=="batches_form_group") ||(formgroup.toLowerCase()=="approve_form_group") ||(formgroup.toLowerCase()=="review_register_form_group") ||(formgroup.toLowerCase()=="reg_ctrbt_commit_form_group")){
		if ((formmode.toLowerCase() == "search") || (formmode.toLowerCase() == "add_record")){		
			document.write ('<font face="verdana" size="2">&nbsp;&nbsp;&nbsp;Total Searchable Records: ' + MainWindow.db_record_count + '</font>');

		}else{
			document.write ('<font face="verdana" size="2">&nbsp;&nbsp;&nbsp;Records matching your query:&nbsp;' + hitCount+ '</font></td>');

		}
	}
}


// --></script></td>
          </tr>
        </table>
        </td>
      </tr>
    </table>
    </td>
  </tr>
</table>

<p>&nbsp;</p>

</body>
</html>