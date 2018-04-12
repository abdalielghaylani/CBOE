
<%@ LANGUAGE=VBScript %>
<%	Response.expires=0
'Copyright 1999-2003 CambridgeSoft Corporation. All rights reserved
dbkey="reg"
if Not Session("UserValidated" & dbkey) = 1 then
	response.redirect "../login.asp?dbname=reg&formgroup=base_form_group&perform_validate=0"
end if
%>

<!-- #include virtual="/cs_Security/variables.asp" -->
<!-- #include virtual="/cs_Security/functions.asp" -->

<%
PAGE_URL = "Registration Enterprise --Main Page"
PAGE_COLOR = color_purple
PAGE_APP = "logo_regsys_250.gif"
TOP_NAV = "<a href=""#"" onclick=""getHelpTopic('/" & Application("appkey") & "/help/help.asp','base_form_group','reg')""><b>Help</b></a>&nbsp;|&nbsp;<a href=""/cs_security/home.asp""><b>Home</b></a>"
%>

<!-- #include virtual="/cs_Security/header.asp" -->


<script language="javascript">
<!-- Hide from older browsers
if(parent.location.href != window.location.href) parent.location.href = window.location.href;

function go_get_sql_string(formgroup,sql_string,table_name,limit_access_to){
		document.cows_input_form.sql_string.value = sql_string
		document.cows_input_form.action= "/<%=Application("Appkey")%>/default.asp?dbname=reg&formgroup=" + formgroup + "&dataaction=get_sql_string&sql_source=session_sql_string&base_table_name=" + table_name + "&limit_access=" + limit_access_to
		document.cows_input_form.submit()
}

function go_get_sql_string2(formgroup,sql_string,table_name,limit_access_to,user_where){
		//document.cows_input_form.sql_string.value = sql_string + "'" + user_where.toUpperCase() + "')"
		//setCookie("PagingMove" + "reg","first_record",1)
		document.cows_input_form.sql_string.value = sql_string
		document.cows_input_form.action= "/<%=Application("Appkey")%>/default.asp?dbname=reg&formgroup=" + formgroup + "&dataaction=get_sql_string&sql_source=session_sql_string&table_name=" + table_name + "&limit_access=" + limit_access_to
		document.cows_input_form.submit()


}


// End script hiding --></script>
<%
Session("CurrentLocation" & dbkey & "base_form_group")= ""
Session("CurrentLocation" & dbkey & "approve_form_group")= ""
Session("CurrentLocation" & dbkey & "review_register_form_group")= ""
Session("CurrentLocation" & dbkey & "reg_ctrbt_commit_form_group")= ""
Session("CurrentLocation" & dbkey & "batch_ctrbt_form_group")= ""
Session("CurrentLocation" & dbkey & "identifiers_ctrbt_form_group")= ""
Session("CurrentLocation" & dbkey & "reg_ctrbt_form_group")= ""
Session("CurrentLocation" & dbkey & "add_analytics_ctrbt_form_group")= ""
Session("CurrentLocation" & dbkey & "manage_analytics_form_group")= ""
Session("CurrentLocation" & dbkey & "manage_users_form_group")= ""

If Not Session("CurrentUser" & dbkey) <> "" then
	theuser = Session("UserName" & dbkey)
	if theuser <> "" then
		Session("CurrentUser" & dbkey) = theuser
	end if
end if
%>
<script LANGUAGE="javascript">
var button_gif_path ="<%=Application("NavButtonGifPath")%>"
</script>

<!--#INCLUDE VIRTUAL = "/cfserverasp/source/cs_security/cs_security_utils.js" -->

<!--#INCLUDE VIRTUAL = "/cfserverasp/source/ado.inc" -->
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/server_const_vbs.asp" -->
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/search_func_vbs.asp" -->
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/utility_func_vbs.asp" -->
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/global_app_vbs.asp"-->
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/marked_hits_vbs.asp"-->
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/hitlist_management_func_vbs.asp"-->
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/manage_queries.asp"-->
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/set_max_users_vbs.asp"-->
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/menubar_vbs.asp"-->

<!--#INCLUDE VIRTUAL = "/cfserverasp/source/navbar_js.js" -->
<!--#INCLUDE FILE = "../source/app_js.js"-->
<!--#INCLUDE FILE = "../source/security_flags.asp"-->
<!--#INCLUDE FILE = "../source/main_page_func_vbs.asp"-->
<!--#INCLUDE FILE = "custom_functions.asp"-->


<%formgroup = "base_form_group"
dbkey="reg"

basetable = GetBaseTable(dbkey, formgroup, "basetable")
%>

<!--#INCLUDE FILE = "../source/app_vbs.asp"-->
<form name="cows_input_form" method="post" action="/<%=Application("appkey")%>/default.asp">
<input type="hidden" name="sql_string">
</form>
<!---<table border="1" cellspacing="0" cellpadding="0">      <tr>        <td valign="top"><font face="Arial" color="#ffffff">        </td> <td>                    <table width="400" border="0">                      <tr>                        <td bgcolor="#FFFFFF" nowrap> <font color="#FFFFFF" size="3">                        <b>&nbsp; <font face="Arial">&nbsp;Main Page</font></b></td>                        <td width="180" bgcolor="#FFFFFF" align="right" nowrap><font face="Arial" color="#42426f" size="2">&nbsp;<%								Response.Write "Current Login: &nbsp;" & UCase(Session("UserName" & request("dbname")))							%>&nbsp;&nbsp;</td></font>	                        </td> </font></tr></table></td></tr></table><br>-->
<table border="0" width="100%" cellpadding="0" style="border-collapse: collapse">
	<tr>
		<td align="right">
			Current login: <%=UCase(Session("UserName" & request("dbname")))%>&nbsp;&nbsp;
			<!--SYAN modified on 11/27/2006 to fix CSBR-68490-->
			<!--input type="button" value="Log off" onclick="document.location=('/cs_security/login.asp?ClearCookies=true');"-->
			<input type="button" value="Log off" onclick="document.location=('/<%=Application("appkey")%>/logoff.asp');" id=button1 name=button1>
			<!--End of SYAN modification-->
		</td>
	</tr>
</table>
<br> 
	
<table border="0" width="100%" style="border-collapse: collapse">
	<tr>	
		<td valign="top" width="50%">
			<%=renderBoxBegin ("Query and Reporting","")%>
					<table border="0" bordercolor="#FFFFFF">
						<%if bSEARCH_REG = True then 
							basetable = getbasetable(dbkey,formgroup,"basetable")
							if UCase(basetable) = "BATCHES" then
								if not Application("BATCHES" & "RecordCount" & "REG") <> "" then
									numCompounds=getRegCount(dbkey, formgroup)
									Application.Lock
										Application("BATCHES" & "RecordCount" & "REG") = numCompounds
									Application.UnLock
								else
									numCompounds=Application("BATCHES" & "RecordCount" & "REG")
								end if
							else
								if CBool(Application("Use_Session_Record_Counts")) = True then
									'if not Session("REG_NUMBERS" & "RecordCount" & "REG") <> "" then
										numCompounds=getRegCount(dbkey, formgroup)
										Session("REG_NUMBERS" & "RecordCount" & "REG") = numCompounds
									'else
										'numCompounds=Session("REG_NUMBERS" & "RecordCount" & "REG")
									'end if
								else
									if not Application("REG_NUMBERS" & "RecordCount" & "REG") <> "" then
										numCompounds=getRegCount(dbkey, formgroup)
										Application.Lock
											Application("REG_NUMBERS" & "RecordCount" & "REG") = numCompounds
										Application.UnLock
									else
										numCompounds=Application("REG_NUMBERS" & "RecordCount" & "REG")
									end if
								end if
							end if%>
						<tr height="25">
							<td>
								<font face="Arial" color="#42426f" size="2">
									<b><%=numCompounds%></b> registered compounds  
									<%If Application("APPROVED_FLAG_USED") = 1 then%> /<b><%=getApprovedCmpds(dbkey, formgroup,numCompounds )%></b> approved <%end if%>: 
								</font>
							</td>
							<td>
								<a class="HomeLink" href="javascript:document.location=('/<%=Application("appkey")%>/default.asp?formgroup=base_form_group&amp;dbname=reg');">Search</a>
                            </td>
						</tr>
						<%end if %>
						<%if bSEARCH_EVAL_DATA = True  AND CBool(Application("ANALYTICS_USED")) = True then%>
							<tr height="25">
								<td>
									<font face="Arial" color="#42426f" size="2">View analytical Data:</font>
								</td>
								<td>
									<a class="HomeLink" href="javascript:document.location=('/<%=Application("appkey")%>/default.asp?formgroup=manage_analytics_form_group&amp;dataaction=ANALYTICS_DATA&amp;dbname=reg');">Show Spreadsheet</a>
								</td>
							</tr>
						<% end if %>
						<%if bSEARCH_REG = True AND CBool(Application("SHOW_SAR_TABLE")) = True then%>
							<tr height="25">
								<td width="114" valign="top" height="30" bgcolor="#FFFFFF">
									<font face="Arial" color="#42426f" size="2">BioReg Data:</font>
								</td>
								<td width="95" valign="top" height="30">									
									<a class="HomeLink" href="javascript:document.location=('/<%=Application("appkey")%>/default.asp?formgroup=sartable_form_group&amp;dataaction=SARTABLE&amp;dbname=reg');">Show SAR Table</a>
								</td>
							</tr>
						<% end if %>
						</table>
			<%=renderBoxEnd%> 
		</td>
		<td width="20">
			&nbsp;&nbsp;&nbsp;&nbsp;
		</td>
		<td valign="top" width="50%">
			<%=renderBoxBegin ("Add to Temporary","")%>
			<table border="0" bgcolor="#FFFFFF" bordercolor="#FFFFFF">
			<% if bADD_COMPOUND_TEMP = True then %>
				
				<tr height="25">
					<td>
						<font face="Arial" color="#42426f" size="2">Pre-register a compound:</font>
					</td>
					<td>
						<a class="HomeLink" href="javascript:document.location=('/<%=Application("appkey")%>/default.asp?formgroup=reg_ctrbt_form_group&amp;dbname=reg&amp;dataaction=full_commit&amp;record_added=false');">Add Compound</a>
					</td>
				</tr>
				<%if CBool(Application("REAGENTS_TO_TEMP")) = True AND CBool(Application("Reagents_Used")) = True then %>
				<tr height="25">
					<td>
						<font face="Arial" color="#42426f" size="2">Add a reagent:</font>
					</td>
					<td>
						<a class="HomeLink" href="javascript:document.location=('/<%=Application("appkey")%>/default.asp?formgroup=add_reagent_ctrbt_form_group&amp;dbname=reg&amp;dataaction=add_reagent&amp;record_added=false');">Add Reagent</a>
					</td>
				</tr>
				<%end if%>
			<%end if%>
			<% if bADD_SALT_TEMP = True and UCase(Application("Batch_Level")) = "SALT" then %>
				<tr height="25">
					<td>
						<font face="Arial" color="#42426f" size="2">Add a salt:</font>
					</td>
					<td>
						<a class="HomeLink" href="javascript:document.location=('/<%=Application("appkey")%>/default.asp?formgroup=salt_ctrbt_form_group&amp;dbname=reg&amp;dataaction=add_salt&amp;record_added=false');">Add Salt</a>
					</td>
				</tr>
			<%end if%>
			<% if CBool(Application("BATCHES_TO_TEMP")) = True AND  bADD_BATCH_TEMP = True then %>
				<tr height="25">
					<td>
						<font face="Arial" color="#42426f" size="2">Add a batch:</font>
					</td>
					<td>
						<a class="HomeLink" href="javascript:document.location=('/<%=Application("appkey")%>/default.asp?formgroup=batch_ctrbt_form_group&amp;dbname=reg&amp;dataaction=batch_commit&amp;record_added=false');">Add Batch</a>
					</td>
				</tr>
			<%end if%>
			<% if CBool(Application("IDENTIFIERS_TO_TEMP")) = True AND bADD_IDENTIFIER_TEMP= True then %>
				<tr height="25">
					<td>
						<font face="Arial" color="#42426f" size="2">Add an Identifier:</font>
					</td>
					<td>
						<a class="HomeLink" href="javascript:document.location=('/<%=Application("appkey")%>/default.asp?formgroup=identifiers_ctrbt_form_group&amp;dbname=reg&amp;dataaction=add_identifiers&amp;record_added=false');">Add Identifier</a>
					</td>
				</tr>
			<%end if%>
			</table>
			<%=renderBoxEnd%> 
		</td>
	</tr>
	<tr>	
		<td valign="top" width="50%">
			<%=renderBoxBegin ("Registration","")%>
			<table border="0" bgcolor="#FFFFFF" bordercolor="#FFFFFF">
			<% if bREGISTER_TEMP = True or bSEARCH_TEMP = True then %>
				
				<tr height="25">
					<td>
						<font face="Arial" color="#42426f" size="2"><b><%=getTempTableCountAll(dbkey, formgroup,"all")%></b> temporary compounds:</font>
					</td>
					<td>
						<a class="HomeLink" href="javascript:document.location=('/<%=Application("appkey")%>/default.asp?formgroup=reg_ctrbt_commit_form_group&amp;dbname=reg');">Search Temp</a>
					</td>
				</tr>
			<% end if %>
			<% if bREGISTER_TEMP = True then 
			temptablesql=getTempTableSql(dbkey,formgroup)%>
				<tr height="25">
					<td>
						<font face="Arial" color="#42426f" size="2"><b><%=getTempTableCount(dbkey, formgroup)%></b> to review:</font>
					</td>
					<td>
						<a class="HomeLink" href="javascript:go_get_sql_string2(&quot;review_register_form_group&quot;,&quot;<%=temptablesql%>&quot;,&quot;temporary_structures&quot;,'');">Review/Register</a>
					</td>
				</tr>
			<% end if %>
			<% If Application("APPROVED_FLAG_USED") = 1 then
			basetable = getbasetable(dbkey, formgroup, "basetable")
			ApprovalCount =getApprovalCount(dbkey,formgroup)
				if bSET_APPROVED_FLAG = True then
					approved_sql_string=getApprovedSql(dbkey, formgroup)%>
				<tr height="25">
					<td>
						<font face="Arial" color="#42426f" size="2">
							<b><%=ApprovalCount%></b> <%response.write " awaiting approval:"%>
							
						</font>
					</td>
					<td>
						<%if UCase(basetable) = "BATCHES" then%>
							<a class="HomeLink" href="javascript:go_get_sql_string2('approve_form_group','<%=approved_sql_string%>','batches','')">Approve</a>
						<%else%>
							<a class="HomeLink" href="javascript:go_get_sql_string2('approve_form_group','<%=approved_sql_string%>','reg_numbers','')">Approve</a>
						<%end if%>
					</td>
				</tr>
			<%	end if
			end if
			If Application("QUALITY_CHECKED_FLAG_USED") = 1 then
				basetable = getbasetable(dbkey, formgroup, "basetable")
				QualityCheckCount =getQualityCheckCount(dbkey,formgroup)
				if bSET_QUALITY_CHECK_FLAG = True then
					qc_select_sql=getQualityCheckedSQL(dbkey,formgroup)%>
				<tr height="25">
					<td>
						<font face="Arial" color="#42426f" size="2">
							<b><%=QualityCheckCount%></b><%response.write  "awaiting quality check: "%>
						</font>
					</td>
					<td>
						<%if UCase(basetable) = "BATCHES" then%>
						<a class="HomeLink" href="javascript:go_get_sql_string2('base_form_group','<%=qc_select_sql%>','batches','')">Quality Check</a>
						<%else%>
						<a class="HomeLink" href="javascript:go_get_sql_string2('base_form_group','<%=qc_select_sql%>','reg_numbers','')">Quality Check</a>
						<%end if%>
					</td>
				</tr>
				<% end if
			end if
			if CBool(Application("BATCHES_TO_TEMP")) = False AND bADD_BATCH_TEMP = True then %>
				<tr height="25">
					<td>
						<font face="Arial" color="#42426f" size="2">Add a temporary batch:</font>
					</td>
					<td>
						<a class="HomeLink" href="javascript:document.location=('/<%=Application("appkey")%>/default.asp?formgroup=batch_ctrbt_form_group&amp;dbname=reg&amp;dataaction=batch_commit&amp;record_added=false');">Add Batch</a>
					</td>
				</tr>
			<%end if
			if CBool(Application("IDENTIFIERS_TO_TEMP")) = False AND bADD_IDENTIFIER_TEMP = True then %>
				<tr height="25">
					<td>
						<font face="Arial" color="#42426f" size="2">
							Add an identifier:
						</font>
					</td>
					<td>
						<a class="HomeLink" href="javascript:document.location=('/<%=Application("appkey")%>/default.asp?formgroup=identifiers_ctrbt_form_group&amp;dbname=reg&amp;dataaction=add_identifiers&amp;record_added=false');">Add Identifier</a>
					</td>
				</tr>
			<%end if
			if CBool(Application("REAGENTS_TO_TEMP")) = FALSE and CBool(Application("Reagents_Used")) = True then %>
				<tr height="25">
					<td>
						<font face="Arial" color="#42426f" size="2">
							Add a reagent:
						</font>
					</td>
					<td>
						<a class="HomeLink" href="javascript:document.location=('/<%=Application("appkey")%>/default.asp?formgroup=add_reagent_ctrbt_form_group&amp;dbname=reg&amp;dataaction=add_reagent&amp;record_added=false');">Add Reagent</a>
					</td>
				</tr>
			<%end if
			if  bADD_EVAL_DATA = True And CBool(Application("ANALYTICS_USED")) = True then%>
				<tr height="25">
					<td>
						<font face="Arial" color="#42426f" size="2">
							Add analytical data:
						</font>
					</td>
					<td>
						<a class="HomeLink" href="javascript:document.location=('/<%=Application("appkey")%>/default.asp?formgroup=add_analytics_ctrbt_form_group&amp;dbname=reg&amp;dataaction=add_analytics_data&amp;record_added=false');">Add Analytic Data</a>
					</td>
				</tr>
			<% end if
			if  Session("Insert_Frag_Eval_Data" & dbkey) = True then%>
				<tr height="25">
					<td>
						<font face="Arial" color="#42426f" size="2">
							Add fragrance eval data:
						</font>
					</td>
					<td>
						<a class="HomeLink" href="javascript:document.location=('/chem_reg/default.asp?formgroup=add_eval_data_form_group&amp;dbname=reg&amp;dataaction=add_eval_data&amp;record_added=false');">Add Eval Data</a>
					</td>
				</tr>
			<% end if %>	
			</table>		
			<%=renderBoxEnd%> 
		</td>
		<td width="20">
			&nbsp;&nbsp;&nbsp;&nbsp;
		</td>
		<td valign="top" width="50%">
			<%=renderBoxBegin ("Administration","")%>
			<table border="0" bgcolor="#FFFFFF" bordercolor="#FFFFFF">
		
			<%	bCOMPOUND_TYPES = (bEDIT_COMPOUND_TYPE_TABLE= True OR bADD_COMPOUND_TYPE_TABLE= True OR bDELETE_COMPOUND_TYPE_TABLE= True) AND CBool(Application("COMPOUND_TYPES_USED")) = true
				bSITES = (bEDIT_SITES_TABLE= True OR bADD_SITES_TABLE= True OR bDELETE_SITES_TABLE= True) AND CBool(Application("SITES_Used")) = true
				bPROJECTS =(bEDIT_PROJECTS_TABLE= True OR bADD_PROJECTS_TABLE= True OR bDELETE_PROJECTS_TABLE= True) AND CBool(Application("PROJECTS_Used")) = true
				bBATCH_PROJECTS =(bEDIT_BATCH_PROJECTS_TABLE= True OR bADD_BATCH_PROJECTS_TABLE= True OR bDELETE_BATCH_PROJECTS_TABLE= True) AND CBool(Application("BATCH_PROJECTS_Used")) = true
				bSALTS =(bEDIT_SALT_TABLE= True OR bADD_SALT_TABLE= True OR bDELETE_SALT_TABLE= True) AND CBool(Application("SALTS_Used")) = true
				bSOLVATES =(bEDIT_SOLVATES_TABLE= True OR bADD_SOLVATES_TABLE= True OR bDELETE_SOLVATES_TABLE= True) AND CBool(Application("SOLVATES_Used")) = true
				bNOTEBOOKS=(bEDIT_NOTEBOOKS_TABLE= True OR bADD_NOTEBOOKS_TABLE= True OR bDELETE_NOTEBOOKS_TABLE= True) AND CBool(Application("NOTEBOOK_USED")) = true
				bUTILIZATIONS=(bEDIT_UTILIZATIONS_TABLE= True OR bADD_UTILIZATIONS_TABLE= True OR bDELETE_UTILIZATIONS_TABLE= True) AND CBool(Application("UTILIZATION_PERMISSIONS_USED")) = true
				bPEOPLE = (bEDIT_PEOPLE_TABLE= True OR bADD_PEOPLE_TABLE= True OR bDELETE_PEOPLE_TABLE= True) AND CBool(Application("SHOW_SEC_TBLES_IN_USR_MGR")) = false
				bSEQUENCES = bEDIT_SEQUENCES_TABLE= True OR bADD_SEQUENCES_TABLE= True OR bDELETE_SEQUENCES_TABLE= True		

				If bSEQUENCES or bCOMPOUND_TYPES or bSITES or bPROJECTS or bBATCH_PROJECTS or bSALTS or bSOLVATES or bNOTEBOOKS or bUTILIZATIONS or bPEOPLE then%>
				<tr height="25">
					<td>
						<font face="Arial" color="#42426f" size="2">
							Manage application tables:
						</font>
					</td>
					<td>
						<a class="HomeLink" href="javascript:document.location=('/<%=Application("appkey")%>/default.asp?formgroup=manage_reg_tables_form_group&amp;dbname=reg&amp;dataaction=manage_reg_tables');">Manage Tables</a>
					</td>
				</tr>
			<%end if
			if Not CBool(Application("UseCSSecurityApp"))= true then
			 	bPEOPLE2 = (bEDIT_PEOPLE_TABLE= True OR bADD_PEOPLE_TABLE= True OR bDELETE_PEOPLE_TABLE= True) AND CBool(Application("SHOW_SEC_TBLES_IN_USR_MGR")) = true

				if bEDIT_USERS_TABLE = True or bPEOPLE2 then %>
				<tr height="25">
					<td>
						<font face="Arial" color="#42426f" size="2">
							Manage User accounts:
						</font>
					</td>
					<td valign="bottom">									
						<a class="HomeLink" href="javascript:document.location=('/<%=Application("appkey")%>/default.asp?formgroup=manage_users_form_group&amp;dbname=reg&amp;dataaction=manage_security');">Manage Users</a>
					</td>
				</tr>
			<%	end if
			else
				if Session("CSS_CREATE_USER" & dbkey) OR Session("CSS_EDIT_USER" & dbkey) OR Session("CSS_DELETE_USER" & dbkey) then%> 
			    <tr height="25">
					<td>
						<font face="Arial" color="#42426f" size="2">
							Manage User accounts:
						</font>
					</td>
					<td valign="bottom">
						<a class="HomeLink" href="#" onclick="javascript:OpenDialog('/chem_reg/cs_security/manageUsers.asp?dbkey=reg&amp;PrivTableName=CHEM_REG_PRIVILEGES', 'Musers', 2);">Manage Users</a>
					</td>
				</tr>
			    <%End if
				if Session("CSS_CREATE_ROLE" & dbkey) OR Session("CSS_EDIT_ROLE" & dbkey) OR Session("CSS_DELETE_ROLE" & dbkey) then%>
			    <tr height="25">
					<td>
						<font face="Arial" color="#42426f" size="2">
							Manage application Roles:
						</font>
					</td>
					<td valign="bottom">
						<a class="HomeLink" href="#" onclick="javascript:OpenDialog('/chem_reg/cs_security/manageRoles.asp?dbkey=reg&amp;PrivTableName=CHEM_REG_PRIVILEGES', 'Musers', 2);">Manage Roles</a>
					</td>
				</tr>
			    <%end if
			end if%>
			
			<%'SYAN modified on 4/18/2007 to fix CSBR-76706
			if Session("CSS_CHANGE_PASSWORD" & dbkey) then%>
				<tr height="25">
				   <td>
						<font face="Arial" color="#42426f" size="2">
							Change your password:
						</font>
					</td>
				    <td valign="bottom">
						<a class="HomeLink" href="#" onclick="javascript:OpenDialog('/chem_reg/cs_security/password.asp?dbkey=reg&amp;PrivTableName=CHEM_REG_PRIVILEGES', 'Mpwd', 2);">Change Password</a>
					</td>
				</tr>	
			<%end if%>
					
			<% If bADD_ANALYTICS_TABLES= True AND CBool(Application("ANALYTICS_USED")) = True then
				experimenttype_sql=getExperimentTypeSQL(dbkey,formgroup) %>
				<tr height="25">
					<td>
						<font face="Arial" color="#42426f" size="2">
							Define a new experiment:
						</font>
					</td>
					<td valign="bottom">									
						<a class="HomeLink" href="javascript:go_get_sql_string('manage_analytics_tables_form_group','<%=experimenttype_sql%>','experimenttype','add')">New Experiment</a>
					</td>
				</tr>
			<%end if
			If (bADD_ANALYTICS_TABLES= True or bEDIT_ANALYTICS_TABLES = True or bDELETE_ANALYTICS_TABLES = True) AND CBool(Application("ANALYTICS_USED")) = True then %>
   				<tr height="25">
   					<td>
   						<font face="Arial" color="#42426f" size="2">
   							Manage analytics tables:
						</font>
					</td>
   					<td valign="bottom">
   						<a class="HomeLink" href="javascript:document.location=('/<%=Application("appkey")%>/default.asp?formgroup=manage_analytics_tables_form_group&amp;dbname=reg&amp;dataaction=manage_analytics_tables');">Manage Analytics</a>
   					</td>
   				</tr>
			<%end if
				if CBool(Application("Workgroups_Used")) = True and (bADD_WORKGROUP = True OR bEDIT_WORKGROUP = True OR bDELETE_WORKGROUP = True)then 
					workgroup_sql=getWorkgroupSQL(dbkey,formgroup)%>
				<tr height="25">
					<td>
						<font face="Arial" color="#42426f" size="2">
							Manage workgroups:
						</font>
					</td>
					<td valign="bottom">									
						<a class="HomeLink" href="javascript:go_get_sql_string('manage_reg_tables_form_group','<%=workgroup_sql%>','people','workgroup')">Manage Workgroups</a>
					</td>
				</tr>
				<%end if
				'stop
			'SYAN modified on 11/6/2006 to fix CSBR-66529
			If (Application("PROJECT_LEVEL_ROW_SECURITY") = "1" AND bMANAGE_SYSTEM_DUPLICATES = True) then
			'If (Not UCase(Application("PRIMARY_STRWHERE")) = "COMPOUND_ONLY" AND bMANAGE_SYSTEM_DUPLICATES = True) then
			'End of SYAN modification
				SystemDuplicatesCount =getSystemDuplicateCount(dbkey,formgroup)
					duplicates_sql=get_duplicates_sql(dbkey,formgroup)%>
				<tr height="25">
					<td valign="bottom">
						<font face="Arial" color="#42426f" size="2">
							<b><%=SystemDuplicatesCount%></b> <%response.write  "duplicate entries: "%>							
						</font>
					</td>
					<td align="left" valign="bottom">
						<a class="HomeLink" href="javascript:go_get_sql_string2('base_form_group','<%=duplicates_sql%>','reg_numbers','')">View Duplicates</a>
					</td>
				</tr>
			<%end if%>	
			</table>
			<%=renderBoxEnd%> 
		</td>
	</tr>
	<tr>
		<td colspan="2">
			&nbsp;
		</td>
	</tr>
</table>
<br>
<div align="center">
	<input type="button" value="Log off" onclick="document.location=('/<%=Application("appkey")%>/logoff.asp');">
</div>

<!-- #include virtual="/cs_Security/footer.asp" -->