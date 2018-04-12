<%@ LANGUAGE="VBScript" %>
<%Response.Expires=0
'Copyright 1999-2003 CambridgeSoft Corporation. All rights reserved
%>
<html>

<head>
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/menubar.asp"-->
<!--#INCLUDE VIRTUAL = "/biosar_browser/source/menu_functions_vbs.asp"-->
<!--#INCLUDE VIRTUAL = "/biosar_browser/biosar_browser_admin/admin_tool/utilities.asp"-->
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/utility_func_vbs.asp"-->

<%
Dim nv_DEBUG
nv_DEBUG = False
'this is a variable for reg, but shouldn't be removed. 
commit_type = Request.QueryString("commit_type")
'some files add the following variable to the querystring to stop the navbar buttons from displaying
nav_override = Request.QueryString("nav_override")
'formmode = Request.QueryString("formmode")
dbkey = Request.QueryString("dbname")
'add your appkey here to avoid errors when dbkey is empty
formgroup = request("formgroup")
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
	
	if (!<%=Application("mainwindow")%>.mainFrame){
			MainWindow = <%=Application("mainwindow")%>
		}
		else{
			MainWindow = <%=Application("mainwindow")%>.mainFrame
		}

	
	var formmode = MainWindow.formmode

	var formgroup = "<%=formgroup%>"
	var dbkey = "<%=dbkey%>"
	
	var formgroupflag = MainWindow.formgroupflag
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
		formmode ="search"
	}
	
	formmode = formmode.toLowerCase()
	
	
</script> 



</head>
<title>BioSAR</title>
<body <%=Application("BODY_BACKGROUND")%>>

<table border="0">
	<tr><td><% 
	if Application("OPTIMIZE_RESULT_DISPLAY_AREA")= 1 then
		bOptimizeHeader = true
	else
		bOptimizeHeader = false
	end if
	formname= "<span title=""" & "Description: " & Application("DESCRIPTION" & request("dbname")& request("formgroup")) & " Owner:" & Application("FORMGROUP_OWNER" & request("dbname")& request("formgroup")) &  """>" & Application("formgroup_name" & request("dbname")& request("formgroup")) & "&nbsp;</span>"
	current_login=  UCase(Session("UserName" & request("dbname")))

		
	if bOptimizeHeader= false then%><table border="0" cellspacing="0" cellpadding="0">
				<tr><td valign="top">
		<a href="/biosar_browser/default.asp?dataaction=open_form&formgroup=base_form_group&dbname=BIOSAR_BROWSER" target="_top" title="Click to return to the home page"><img src="/<%=Application("AppKey")%>/graphics/logo_cs_biosarent.gif" border="0"></a></font>&nbsp;&nbsp; </td>
       
					
					<td>
                    <table align="left" cellspacing="0" height="20" border="0" width="500">
                      <tr>
                        <td align="center" width 150" nowrap> 
                      <font face="verdana" size="4"><strong><i>&nbsp;&nbsp; <%=formname%>             
                        <% 
                        if not formgroup = "base_form_group" then
							 Select Case UCase(request("formmode"))
								case "SEARCH"
									header_text ="&nbsp;Query Input Form"
								case "EDIT"
									header_text ="&nbsp;Result Detail View"
								case "LIST"
                      				header_text ="&nbsp;Result List View"
							end select
                       else
							Select Case UCase(request("page_name"))
								Case "ADMIN_EDIT_CONTENT_TYPE"
									header_text = "Schema Management"
								Case "ADMIN_EDIT_INDEX_TYPE"
									header_text = "Schema Management"
								Case "ADMIN_EDIT_LOOKUP"
									header_text = "Schema Management"
								Case "ADMIN_EDIT_REL"
									header_text = "Schema Management"
								Case "ADMIN_SCHEMA_CHOICE"
									header_text = "Schema Management"
								Case "ADMIN_SCHEMA_PASSWORD"
									header_text = "Schema Management"
								Case "ADMIN_TABLE_CHOICE"
									header_text = "Schema Management"
								Case "ADMIN_TABLE_LIST"
									header_text = "Schema Management"
								Case "ADMIN_TABLE_VIEW"
									header_text = "Schema Management"
								Case "USER_CHOOSE_BASE_TABLE"
									header_text = "Form Management"
								Case "USER_CHOOSE_CHILD_TABLE"
									header_text = "Form Management"
								Case "USER_COLUMN_ORDER"
									header_text = "Form Management"
								Case "USER_COLUMNS"
									header_text = "Form Management"
								Case "USER_FORMS"
									header_text = "Form Management"
								Case "USER_RENAME_FORM"
									header_text = "Form Management"
								Case "USER_NEW_FORM"
									header_text = "Form Management"
								Case "USER_TABLES"
									header_text = "Form Management"
							End Select
                       end if
                       Response.Write  header_text 
                       

                    %></i></font>
                   </td> </font>
                         <td width="400" align="left" nowrap><font face="verdana" size="2">&nbsp;<%
								response.Write "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Current Login: &nbsp;"  & current_login
							%>&nbsp;&nbsp;</td></font>
	     
                   </td> </font>
                      </tr>
                    </table>
					</td>
				</tr>
			</table></div>
		</td><%end if 'bOptimizeHeader%>
	</tr>
	
	<tr><td><table border="0" width="700" cellspacing="1">
				<tr>
				
					<td valign="top" align="left" nowrap>
					<%buildMenuBar request("formmode"),request("dbname"), request("formgroup")
					%>
					
					<%if bOptimizeHeader=true and not bAdmin=true then
					formgroup_name = Application("formgroup_name" & request("dbname")& request("formgroup"))
					if  formgroup_name <> "" and not UCase(formgroup_name) = "BASE_FORM_GROUP" then 
				
						full_form_string = "&nbsp;&nbsp;&nbsp;Form Name: " & formname & "<br>"
					else
						full_form_string = "" 
					end if
					
					curr_login_string = "&nbsp;&nbsp;&nbsp;Current Login: " & current_login%>
					<td width="300" align="left" nowrap><font face="verdana" size="2"><%=full_form_string%> <%=curr_login_string%> </font></td><%end if%>
		</tr><tr><td>
		
		<% 'build nav buttons based on page location
			if formgroup="base_form_group" then%>
			<br>
			<%
			' DGB modified onclick event to check for the presence of admin_form before submitting it.
			save_button = "<a href='#' onclick= 'MainWindow.ClearFormatMask();if (MainWindow.document.admin_form) MainWindow.document.admin_form.submit();return true;'><img src=""/biosar_browser/graphics/btn_savechanges_80.gif"" border=0></a>"
			cancel_button = "<a href='#' onclick= 'MainWindow.document.cancel_form.submit();return true;'><img src=""/biosar_browser/graphics/cancel_edit_btn.gif"" border=0></a>"
			done_button = "<a href='#' onclick= 'MainWindow.document.admin_form.submit();return true;'><img src=""/biosar_browser/graphics/btn_done_80.gif"" border=0></a>"
			
			'Response.Write request("page_name")
			Select Case UCase(request("page_name"))
				
				
				Case "ADMIN_EDIT_CONTENT_TYPE"
					header_text = "Schema Management"
					button_display = save_button  & cancel_button
				Case "ADMIN_EDIT_INDEX_TYPE"

					button_display = save_button  & cancel_button
				Case "ADMIN_EDIT_LOOKUP"

					button_display = save_button & cancel_button
				Case "ADMIN_EDIT_REL"

					button_display = save_button & cancel_button
				Case "ADMIN_SCHEMA_CHOICE"
					cancel_button = "<a href='#' onclick= 'MainWindow.document.cancel_form.submit();return true;'><img src=""/biosar_browser/graphics/cancel_edit_btn.gif"" border=0></a>"

					button_display = save_button & cancel_button
				Case "ADMIN_SCHEMA_PASSWORD"
					cancel_button = "<a href='#' onclick= 'MainWindow.history.back();return true;'><img src=""/biosar_browser/graphics/cancel_edit_btn.gif"" border=0></a>"
					save_button = "<a href='#' onclick= 'MainWindow.checkNameDescPass();return true;'><img src=""/biosar_browser/graphics/btn_savechanges_80.gif"" border=0></a>"
					button_display = save_button & cancel_button
				Case "ADMIN_TABLE_CHOICE"
					cancel_button = "<a href='#' onclick= 'MainWindow.history.back();return true;'><img src=""/biosar_browser/graphics/cancel_edit_btn.gif"" border=0></a>"

					button_display =  save_button & cancel_button
				Case "ADMIN_TABLE_LIST"
					'need addDeleteSchemas
					schema_add = "<a href='#' onclick= 'MainWindow.document.schema_form.submit();return true;'><img src=""/biosar_browser/graphics/adddelsch_btn.gif"" border=0></a>"
					table_refresh_button = "<a href='#' onclick= 'MainWindow.document.admin_refresh_form.submit();return true;'><img src=""/biosar_browser/graphics/refresh_all_tables_btn.gif"" border=0></a>"
					button_display= schema_add & table_refresh_button
				Case "ADMIN_TABLE_VIEW"
					table_refresh_button = "<a href='#' onclick= 'MainWindow.document.admin_refresh_form.submit();return true;'><img src=""/biosar_browser/graphics/refresh_table_btn.gif"" border=0></a>"
					'index_refresh_button = "<a href='#' onclick= 'MainWindow.document.admin_refresh_index_form.submit();return true;'><img src=""/biosar_browser/graphics/btn_refresh_indexes_80.gif"" border=0></a>"
					cancel_button = "<a href='#' onclick= 'MainWindow.document.cancel_form.submit();return true;'><img src=""/biosar_browser/graphics/cancel_edit_btn.gif"" border=0></a>"

					button_display = save_button  & table_refresh_button  &  cancel_button
					
				Case "USER_CHOOSE_BASE_TABLE"
						button_display = save_button & cancel_button
				Case "USER_CHOOSE_CHILD_TABLE"
					button_display = save_button& cancel_button
				Case "USER_COLUMN_ORDER"

					button_display = save_button  & cancel_button
				Case "USER_COLUMNS"
					cancel_button = "<a href='#' onclick= 'MainWindow.history.back();return true;'><img src=""/biosar_browser/graphics/cancel_edit_btn.gif"" border=0></a>"

					button_display = save_button  & cancel_button
				Case "USER_FORMS"
					new_button = "<a href='#' onclick= 'if (MainWindow.document.admin_form) MainWindow.document.admin_form.submit();return true;'><img src=""/biosar_browser/graphics/btn_create_new_form_80.gif"" border=0></a>"

					button_display = new_button
				Case "USER_RENAME_FORM"
					save_button = "<a href='#' onclick= 'MainWindow.checkNameDescRename();return true;'><img src=""/biosar_browser/graphics/btn_savechanges_80.gif"" border=0></a>"
					button_display = save_button & cancel_button
				Case "USER_NEW_FORM"
					cancel_button = "<a href='#' onclick= 'MainWindow.history.back();return true;'><img src=""/biosar_browser/graphics/cancel_edit_btn.gif"" border=0></a>"
					save_button = "<a href='#' onclick= 'MainWindow.checkNameDesc();return true;'><img src=""/biosar_browser/graphics/btn_savechanges_80.gif"" border=0></a>"

					
					button_display = save_button & cancel_button
				Case "USER_TABLES"
		
					if Session("SET_FORMGROUP_PUBLIC" & "biosar_browser")=True then
					
						'if UCase(Session("FORMGROUP_OWNER" & dbkey & request("formgroup_id"))) =  UCase(Session("UserName" & dbkey)) then
						
							if Session("CurrentLocation" & dbkey & request("formgroup_id")) <> ""  and Session("return_to_form") = "true" then
								save_button = "<a href='#' onclick= 'MainWindow.doSubmitAndRestore();return true;'><img src=""/biosar_browser/graphics/btn_savechanges_80.gif"" border=0></a>"
								cancel_button = "<a href='#' onclick= 'MainWindow.document.location.replace(&quot;" & Session("CurrentLocation" & dbkey & request("formgroup_id"))  & "&quot;)'><img src=""/biosar_browser/graphics/cancel_edit_btn.gif"" border=0></a>"
							else
						
								save_button = "<a href='#' onclick= 'MainWindow.doSubmitAndRestore();return true;'><img src=""/biosar_browser/graphics/btn_savechanges_80.gif"" border=0></a>"
							end if
							button_display = save_button & cancel_button
						'else
							'button_display =save_button
						'end if
					else
						
						if Session("CurrentLocation" & dbkey & request("formgroup_id")) <> ""  and Session("return_to_form") = "true" then
						
							done_button = "<a href='#' onclick= 'MainWindow.document.location.replace(&quot;" & Session("CurrentLocation" & dbkey & request("formgroup_id"))  & "&quot;)'><img src=""/biosar_browser/graphics/cancel_edit_btn.gif"" border=0></a>"
							save_button="<a href='#' onclick= 'MainWindow.doSubmitAndRestore2();return true;'><img src=""/biosar_browser/graphics/btn_savechanges_80.gif"" border=0></a>"
							cancel_button = "<a href='#' onclick= 'MainWindow.document.location.replace(&quot;" & Session("CurrentLocation" & dbkey & request("formgroup_id"))  & "&quot;)'><img src=""/biosar_browser/graphics/cancel_edit_btn.gif"" border=0></a>"

							button_display = save_button & cancel_button
						else
							save_button="<a href='#' onclick= 'MainWindow.doSubmitAndRestore2();return true;'><img src=""/biosar_browser/graphics/btn_savechanges_80.gif"" border=0></a>"
							if Session("CurrentLocation" & dbkey & request("formgroup_id")) <> "" then
								cancel_button = "<a href='#' onclick= 'MainWindow.document.location.replace(&quot;" & Session("CurrentLocation" & dbkey & request("formgroup_id"))  & "&quot;)'><img src=""/biosar_browser/graphics/cancel_edit_btn.gif"" border=0></a>"
							else
								cancel_button = "<a href='#' onclick= 'MainWindow.document.location.replace(&quot;" & "/biosar_browser/biosar_browser_admin/admin_tool/user_forms.asp?formgroup=base_form_group&dbname=biosar_browser&formgroup_id="& request("formgroup_id")  & "&quot;)' target=_main><img src=""/biosar_browser/graphics/cancel_edit_btn.gif"" border=0></a>"
							end if
							button_display = save_button & cancel_button

						end if
						
					end if
				
				  Case "REFRESH_FORMS"
					save_button = "<a href='#' onclick= 'MainWindow.doIt();return true;'><img src=""/biosar_browser/graphics/btn_savechanges_80.gif"" border=0></a>"
					done_button = "<a href='#' onclick=""MainWindow.location.replace('/biosar_browser/biosar_browser_admin/admin_tool/user_forms.asp?formgroup=base_form_group&dbname=biosar_browser')""><img src=""/biosar_browser/graphics/cancel_edit_btn.gif"" border=0></a>"

					button_display = save_button & done_button 			
				  Case "REFRESH_FORMS_WARNING"
					done_button = "<a href='#' onclick=""MainWindow.location.replace('/biosar_browser/biosar_browser_admin/admin_tool/user_forms.asp?formgroup=base_form_group&dbname=biosar_browser')""><img src=""/biosar_browser/graphics/cancel_edit_btn.gif"" border=0></a>"
					button_display =  done_button 				
				Case "REFRESH_FORM_LOG"
					done_button = "<a href='#' onclick=""MainWindow.location.replace('/biosar_browser/biosar_browser_admin/admin_tool/user_forms.asp?formgroup=base_form_group&dbname=biosar_browser')""><img src=""/biosar_browser/graphics/btn_done_80.gif"" border=0></a>"
					button_display =  done_button	
			End Select
			'output admin buttons
			Response.Write button_display
                       
                       
                       end if%>
		
		</td>
						 
						 </tr>
				</tr>
      
				<tr><%if bOptimizeHeader = false then%>
				
				<td width="750" valign="top" colspan="2" nowrap>
						<table border="0" bordercolor="Navy">
							<tr><td nowrap>
							<%
							if Not UCase(formgroup) = "BASE_FORM_GROUP" then%>
									<script language="JavaScript">
									<!--
										
									if (noNav ==false){ 
								
										if ((formgroupflag.toLowerCase() == "single_search")||(formgroupflag.toLowerCase() == "search")){
											var toomany = "<%=strTrueFalse(Session("TooManyHitsToDisplay" & dbkey & formgroup))%>"
											if (toomany.toLowerCase()=="true"){ 
												document.write (MainWindow.getButtons('mark_all_records'));
											}else{
												document.write (MainWindow.getButtons());
											}
											document.write (MainWindow.getCustomButtons());
											//document.write ('<a href="/<%=Application("AppKey")%>/<%=dbkey%>/mainpage.asp?dbname=<%=dbkey%>&Timer='+ "<%=Timer%>" + '"  target=_top><img src="/<%=Application("appkey")%>/graphics/mainMenu.gif" border="0"></a>')
										}
									 }  //end if(noNav ==false)
									// -->
								    </script>
							<%end if%>
							</td>
						</tr>
					</table>
				</td>
			</tr>
    
			<tr><td width="750" valign="top" colspan="2">
					<table border="0" cellspacing="3">
						<tr><td>
								<nobr>
								  
								<script language="JavaScript">
									formmode = "<%=request("formmode")%>"
									var hitCount = "<%=Session("hitlistRecordCount" & request("dbname") & request("formgroup"))%>";
									if(noNav ==false){
										//this calls the navigation buttons for navigating the record set
										if(formgroup.toLowerCase() !="base_form_group"){
											document.write (MainWindow.getRecordNav());
										
											if ((formmode.toLowerCase() == "list")||(formmode.toLowerCase() == "edit")){
												document.write ('<font face="verdana" size = "2">&nbsp;&nbsp;&nbsp;Records matching your query:&nbsp;' + hitCount+ '</font></td>');
											}
											else{
												document.write ('<font face="verdana" size = "2">&nbsp;&nbsp;&nbsp;Total Searchable Records: ' + MainWindow.db_record_count + '</font>');

											}
										}
									}
									// -->
								</script>
								</nobr>
							</td>
						</tr>
					</table>
				</td>
			</tr>
			<%else%>
			<td width="750" valign="top" colspan="2" nowrap>
						<table border="0" bordercolor="Navy" ID="Table1">
							<tr><td nowrap>
							<%
							if Not UCase(formgroup) = "BASE_FORM_GROUP" then%>
									<script language="JavaScript">
									<!--
										
									if (noNav ==false){ 
								
										if ((formgroupflag.toLowerCase() == "single_search")||(formgroupflag.toLowerCase() == "search")){
											document.write (MainWindow.getButtons());
											
											document.write (MainWindow.getCustomButtons());
											if(formgroup.toLowerCase() !="base_form_group"){
											document.write ('&nbsp;&nbsp;&nbsp;')
											document.write (MainWindow.getRecordNav());
											document.write ('<font face="verdana" size = "2">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Total Searchable Records: ' + MainWindow.db_record_count + '</font>');
										}
											//document.write ('<a href="/<%=Application("AppKey")%>/<%=dbkey%>/mainpage.asp?dbname=<%=dbkey%>&Timer='+ "<%=Timer%>" + '"  target=_top><img src="/<%=Application("appkey")%>/graphics/mainMenu.gif" border="0"></a>')
										}
									 }  //end if(noNav ==false)
									// -->
								    </script>
							<%end if%>
							</td>
						</tr>
					</table>
				</td>
			</tr>
			
			<%end if%>
		</table>
	</td>
</tr>
</table>
</body>
</html>