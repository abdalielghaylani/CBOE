<%@ Language=VBScript %>

<HTML>
<HEAD>
<!-- #INCLUDE FILE="app_entry.asp" -->


<!-- #INCLUDE FILE="header.asp" -->
<!-- #INCLUDE FILE="xmltemplate_func.asp" -->
<!-- #INCLUDE FILE="display_sql_func.asp" -->



</HEAD>

<body>
<%
Server.ScriptTimeout = 1800
Session("AllowOrganizeFGSubmenu") = true
Session("AllowOrganizePublicFGSubmenu") = true

session("user_forms_cancel_url") = Request.ServerVariables("query_string") & "&cancel=true"
Set oConn = SysConnection

Set uConn = GetNewConnection( _
				Session("dbkey_admin_tools"), _
				"base_form_group", _
				"base_connection")
Dim lgroupid
If request("cancel") = "" then
	Select Case request("action")	
		Case "do_rename"
			' rename form
			lgroupid = Request("formgroup_id")
			RemoveFormToRootNode lgroupID
			Set oRS = oConn.Execute (SqlFormgroupByID(lgroupid))
			Dim bPublic
			bPublic = oRS("IS_PUBLIC")
			
			sName =request("new_name")
			sDesc =request("new_desc")
			
			sName=removeIllegalChars(sName)
			sDesc=removeIllegalChars(sDesc)
			
			oConn.Execute SQLUpdateFormgroup(sName, sDesc, bPublic, lgroupId),lRecsAffected, adCmdText + adExecuteNoRecords 
			' because role names depend on form names, we must
			' 1. Determine if an old role exists
			sOldRoleName = RolifyName(request("old_name"), lgroupId)
			sRoleName = RolifyName(request("new_name"), lgroupid)
			AddFormToRootNode lgroupID
			set ors = oconn.execute("select * from cs_security.security_roles where role_name = '" & sOldRoleName & "'")
			if hasrecords(ors) then
				' 1.5 Store the users and roles that were granted the old role
				sFGUsersSQL = "select distinct grantee as user_id from dba_role_privs, dba_users " & _
							"where dba_role_privs.granted_role = '" & soldrolename & "'" & _
							" and dba_role_privs.grantee = dba_users.username"
				sFGRolesSql = "select distinct grantee as role_name from dba_role_privs, dba_roles " & _
							"where dba_role_privs.granted_role = '" & soldrolename & "'" & _
							" and dba_role_privs.grantee = dba_roles.role"
				Set oRSFGUsers = oConn.Execute(sFGUsersSQL)
				Set oRSFGRoles = oConn.Execute(sFGRolesSQL)
				w_users_and_roles = ListRSVals(oRSFGUsers, "user_id", "")
				w_users_and_roles = ListRSVals(oRSFGRoles, "role_name", w_users_and_roles)
				' 2. Revoke the old role from any users or roles that possess it
				arr = split(w_users_and_roles, ",")
				for i = lbound(arr) to ubound(arr)
					oConn.Execute "revoke " & sOldRoleName & " from " & arr(i)
				next
				' 3. Store object grants that were given to the old role
				sObjsSql = "select distinct owner || '.' || table_name as full_name from dba_tab_privs where grantee = '" & sOldRoleName & "'"
				Set oRSObjs = oconn.execute(sObjsSql)
				
				w_objs = ListRSVals(oRSObjs, "full_name", "")
				' 4. Delete the old role
				oconn.execute "drop role " & soldrolename
				' 5. Create a new role
				oConn.Execute("create role " & sRoleName & " not identified")
				oConn.Execute("revoke " & sRoleName & " from system")				
				' 6. Update CS_SECURITY to reflect the new role name
				oconn.execute("update cs_security.security_roles set role_name = '" & sRoleName & "' where " & _
							   "role_name = '" & sOldRoleName & "'") 
				' 7. Re-grant all objects to new role
				dim arr2
				arr2 = split(w_objs, ",")
				CheckExistsPowerRole()
				Dim k
				for k = 0 to UBound(arr2)
					' you need a schema connection to grant selects on
					' the tables in a schema
					' 0 - schema name, 1 - table name
					arrTable = Split(arr2(k), ".")
					' DbgBreak arrTable(0)
					Set oSchemaConn = SchemaConnection(arrTable(0))
					on error resume next
					'this will skip over the views for bioviz
					oSchemaConn.Execute("grant select on " & arrTable(1) & " to " & sRoleName)
					oSchemaConn.Execute("grant select on " & arrTable(1) & " to " & "BIOSAR_MASTER_ROLE")
				next
				' 8. Re-grant this new role to all users who had the old role
				dim arr3
				arr3 = split(w_users_and_roles, ",")
				for i = 0 to ubound(arr3)
					oconn.execute("grant " & sRoleName & " to " & arr3(i))
				next
				
			end if
			
		Case "new_form"
		
			' createformgroup is in utilities.asp
			formgroup_name= request("formgroup_name")
			formgroup_description = request("formgroup_description")
			formgroup_name=removeIllegalChars(formgroup_name)
			formgroup_description=removeIllegalChars(formgroup_description)
			lgroupId=CreateFormGroup(formgroup_name, formgroup_description)
			AddFormToRootNode lgroupID
			
			new_form_action_path = "/biosar_browser/biosar_browser_admin/admin_tool/user_tables.asp?dbname=biosar_browser&formgroup=base_form_group&formgroup_id=" & lgroupID
			Response.redirect new_form_action_path
			
		case "duplicate_form"
			
			' duplicate form
			' get name of old formgroup
			lgroupid = request("formgroup_id")
			Set rs = oconn.execute(sqlformgroupbyid(lgroupid))
			soldName = rs("formgroup_name")
			soldDescription = rs("formgroup_name")
			sname = "Copy of " & soldName
			storename = sname
			' create new formgroup
			lNewGroupId = CreateFormGroup(sname, soldDescription)
			AddFormToRootNode lNewGroupId
			' copy information from DB_FORMGROUP
			' oConn.Execute "update cs_security.db_formgroup set is_public = (select is_public from cs_security.db_formgroup where formgroup_id = " & lgroupid & ") where formgroup_id = " & lNewGroupID
			oConn.Execute "update BIOSARDB.db_formgroup set description = (select description from BIOSARDB.db_formgroup where formgroup_id = " & lgroupid & ") where formgroup_id = " & lNewGroupID,lRecsAffected, adCmdText + adExecuteNoRecords 
			oConn.Execute "update BIOSARDB.db_formgroup set base_table_id = (select base_table_id from BIOSARDB.db_formgroup where formgroup_id = " & lgroupid & ") where formgroup_id = " & lNewGroupID,lRecsAffected, adCmdText + adExecuteNoRecords 
			' copy information from DB_FORM
			for i = 1 to 3
				oConn.Execute "update BIOSARDB.db_form set url = (select url from BIOSARDB.db_form where formgroup_id = " & lgroupid & " and formtype_id = " & i & ") where formgroup_id = " & lNewgroupid & " and formtype_id = " & i,lRecsAffected, adCmdText + adExecuteNoRecords 
			next
			' copy information from DB_FORMGROUP_TABLES
			' should be done with a stored procedure
			set rs = oconn.execute("select * from BIOSARDB.db_formgroup_tables where formgroup_id = " & lgroupid)
			if hasrecords(rs) then
				do until rs.eof
					dim str
					dim stableorder
					if isnull(rs("table_order")) then
						stableorder = "1"
					else
						stableorder = rs("table_order")
					end if
					str = "insert into BIOSARDB.db_formgroup_tables" & _
								  " (formgroup_id, table_id, table_order, table_rel_order)" & _
								  " values (" & lNewGroupId & "," & rs("table_id") & "," & stableorder & "," & rs("table_rel_order") & ")"
					' dbgbreak str
					oconn.execute str
					rs.movenext
				loop
			end if
			' get new form ids from DB_FORM
			' DCW: 2/18/2004 ... and add the current forms' form items to the new form.
			set rs = oconn.execute("select * from BIOSARDB.db_form where formgroup_id = " & lNewGroupid)
			do until rs.eof
				this_formtype_id = clng(rs("formtype_id"))
				this_form_id = rs("form_id")
				sql = "insert into biosardb.db_form_item(form_id, table_id, column_id, disp_typ_id, disp_opt_id, width, height, column_order, v_column_id, format_mask, link, linktext)" & _
						"select 	" & this_form_id & ", table_id, column_id, disp_typ_id, disp_opt_id, width, height, column_order, v_column_id, format_mask, link, linktext " & _
						"from biosardb.db_vw_formitems_all where formgroup_id = " & lgroupid & " and formtype_id = " & this_formtype_id
				'response.Write sql
				oconn.execute(sql)
				rs.movenext
			loop
			' copy security information
			sRoleName = RolifyName(storename, lNewGroupID)
			sParentRoleName = RolifyName(soldName, lgroupId)
			set ors = oconn.execute("select * from cs_security.security_roles where role_name = '" & sParentRoleName & "'")
			if hasrecords(ors) then
				' 1.5 Store the users and roles that were granted the parent role
				sFGUsersSQL = "select distinct grantee as user_id from dba_role_privs, dba_users " & _
							"where dba_role_privs.granted_role = '" & soldrolename & "'" & _
							" and dba_role_privs.grantee = dba_users.username"
				sFGRolesSql = "select distinct grantee as role_name from dba_role_privs, dba_roles " & _
							"where dba_role_privs.granted_role = '" & soldrolename & "'" & _
							" and dba_role_privs.grantee = dba_roles.role and NOT dba_roles.role='BIOSAR_MASTER_ROLE'"
				Set oRSFGUsers = oConn.Execute(sFGUsersSQL)
				Set oRSFGRoles = oConn.Execute(sFGRolesSQL)
				
				' 2. Store object grants that were given to the old role
				sObjsSql = "select distinct owner || '.' || table_name as full_name from dba_tab_privs where grantee = '" & sParentRoleName & "'"
				Set oRSObjs = oconn.execute(sObjsSql)
				w_objs = ListRSVals(oRSObjs, "full_name", "")
				' 5. Create a new role
				oConn.Execute("create role " & sRoleName & " not identified")
				oConn.Execute("revoke " & sRoleName & " from system")				
				' 6. Update CS_SECURITY to reflect the new role name
				oconn.execute("insert into cs_security.security_roles(role_name) values('" & sRoleName & "')")
				' 7. Re-grant all objects to new role
				
				arr2 = split(w_objs, ",")
				CheckExistsPowerRole()

				dim j
				for j = 0  to ubound(arr2)
					' you need a schema connection to grant selects on
					' the tables in a schema
					' 0 - schema name, 1 - table name
					arrTable = Split(arr2(j), ".")
					' DbgBreak arrTable(0)
					Set oSchemaConn = SchemaConnection(arrTable(0))
					on error resume next
					'this will skip over the views for bioviz
					oSchemaConn.Execute("grant select on " & arrTable(1) & " to " & sRoleName)
					oSchemaConn.Execute("grant select on " & arrTable(1) & " to " & "BIOSAR_MASTER_ROLE")

				next
				' 8. Grant role to user who is duplicating
				
			
				oconn.execute("grant " & sRoleName & " to " & UCase(Session("UserName" & Session("dbkey_admin_tools"))))
				
				

			end if
			lgroupid = lNewGroupID
		Case "update_templates"
			'Response.write "UT<BR><BR>"
			'Response.Write "QS=" & Request.QueryString & "<BR><BR>"
			'Response.Write "F=" & Request.Form & "<BR><BR>"
			'Response.Write "C=" & Request.Cookies & "<BR><BR>"
			'Response.end
			
			restore_form=request("form_restore2")
			lgroupid = request("formgroup_id")
			
			if Application("BIOVIZ_INTEGRATION")=1   then
				enable_bioviz = request("enable_bioviz")
				SetBioVizIntegration lGroupID, enable_bioviz
			end if
			if Application("EXCEL_INTEGRATION")=1   then
				enable_excel = request("enable_excel")
				SetExcelIntegration lGroupID, enable_excel
			end if
			
			CreateXMLTemplates(lgroupid)
			Dim sFormGroupName
			Dim oRSFormGroupName
			Set oRSFormGroupName = oConn.Execute("select formgroup_name,description from db_formgroup where formgroup_id = " & lgroupID)
			sFormGroupName = oRSFormGroupName("FORMGROUP_NAME")
			sFormGroupDescription = oRSFormGroupName("DESCRIPTION")
			
			'LJB 6/8 BEGIN formgroup role creation changes. All formgroups have a role created whether they are public or not.
			
			'grant basetable to biosardb and any cartridge indexes so that hitlist management and sdfilesearch will work
			'grant basetable to biosardb so that hitlist management will work
			GrantBTtoBiosardb lGroupID 
			
			'grant cartridge indexes to biosardb so that sdfilesearch will work
			GrantCartIndextoBiosardb lGroupID
			
			if request("new_name") <> "" then
				sName = Replace(request("new_name"), "'", "''")
			end if
			if request("new_desc") <> "" then
				sDesc = Replace(request("new_desc"), "'", "''")
			else
				sDesc=sFormGroupDescription
			end if
			'set Public Flag
			SetIsPublicFlag lGroupID, "N",sFormGroupName,sDesc
			
		
			
			'create formgroup role
			CreateFormgroupRole sFormGroupName,lGroupID,sName
			if not  request("current_roles_hidden") <> "" then
				GrantSelectOnViews(Session("username" & dbkey))
			end if
			'End formgroup role creation changes.
			if Session("return_to_form") = "true" then
				if Session("CurrentLocation" & dbkey & lGroupID)  <> "" then
					Response.redirect Session("CurrentLocation" & request("dbname") & lGroupID)
				end if
				Session("return_to_form")=""
			end if
			
		Case "update_roles_and_templates"
			'Response.write "URT<BR><BR>"
			'Response.Write "QS=" & Request.QueryString & "<BR><BR>"
			'Response.Write "F=" & Request.Form & "<BR><BR>"
			'Response.Write "C=" & Request.Cookies & "<BR><BR>"
			'Response.end
			
			
			restore_form=request("form_restore")
			lgroupid = request("formgroup_id")
			
			'Response.end
			if Application("BIOVIZ_INTEGRATION")=1   then
				enable_bioviz = request("enable_bioviz")
				SetBioVizIntegration lGroupID, enable_bioviz
			end if
			if Application("EXCEL_INTEGRATION")=1   then
				enable_excel = request("enable_excel")
				SetExcelIntegration lGroupID, enable_excel
			end if
			CreateXMLTemplates(lgroupid)
			Dim sGroupName
			Dim oRSName
			Set oRSName = oConn.Execute("select formgroup_name from db_formgroup where formgroup_id = " & lgroupID)
			sOldName = oRSName("FORMGROUP_NAME")
			
			Dim oRSCount1
			set cmd = Server.CreateObject("adodb.command")
			cmd.ActiveConnection =  oConn
			cmd.CommandType = adCmdText
			cmd.CommandText = "select count(*) from BIOSARDB.db_vw_formgroup_tables where TABLE_ORDER = 1 and TABLE_NAME is NULL and formgroup_id=" & lgroupID
			cmd.Parameters.Append cmd.CreateParameter("plGroupID", 5, 1, 0, lGroupID) 
			Set oRSCount1 = cmd.Execute() 
			if Clng(oRSCount1.Fields(0).Value) > 0  then
				set cmd = Server.CreateObject("adodb.command")
				cmd.ActiveConnection =  oConn
				' display warning if no formitems found for table
				%><script language = "javascript">	noBaseTableFieldsWarning()</script><%
			end if
			
			' we get here from the "Save Changes" button
			' check for no tables or no formitems
			Dim oRSCount
			set cmd = Server.CreateObject("adodb.command")
			cmd.ActiveConnection =  oConn
			cmd.CommandType = adCmdText
			cmd.CommandText = "select count(*) from BIOSARDB.db_vw_formitems_all where formgroup_id = ?"
			cmd.Parameters.Append cmd.CreateParameter("plGroupID", 5, 1, 0, lGroupID) 
			Set oRSCount = cmd.Execute() 
			
			if Clng(oRSCount.Fields(0).Value) < 1 then
				' display warning if no formitems found for table
				%><script language = "javascript">	noFieldsWarning()</script><%
			end if
			
			
			'LJB 6/8 BEGIN formgroup role creation changes. All formgroups have a role created whether they are public or not.
			
			'grant basetable to biosardb so that hitlist management will work
			GrantBTtoBiosardb lGroupID 
			
			'grant cartridge indexes to biosardb so that sdfilesearch will work
			GrantCartIndextoBiosardb lGroupID
			
			
			sName = Replace(request("new_name"), "'", "''")
			sDesc = Replace(request("new_desc"), "'", "''")
			if request("current_roles_hidden") <> "" then
			
				isPublic = "Y"
				AddPublicFormToRootNode lGroupID
			else
				isPublic = "N"
				RemovePublicFormFromTree lGroupID
			end if
			
			
			'set Public Flag
			SetIsPublicFlag lGroupID, isPublic,sName,sDesc
			
			'create formgroup role
			CreateFormgroupRole sOldName,lGroupID,sName
			
			'if the role isn't public then you need to grant the views to the user
			if not  request("current_roles_hidden") <> "" then
				GrantSelectOnViews(Session("username" & dbkey))
			end if
			
			if sSchemaErrs <> "" then
				%><script language="javascript">schemaErrors()</script><%
			end if
			if Session("return_to_form") = "true" then
				if Session("CurrentLocation" & dbkey & lGroupID)  <> "" then
					Response.redirect Session("CurrentLocation" & request("dbname") & lGroupID)
				end if
				Session("return_to_form") = ""
			end if
		Case "refresh_form"
			refresh_list = Request.Form("refresh_list")
			formgroup_id_list = Request.Form("formgroup_id_list")
			enable_BioViz_list =  Request.Form("enable_BioViz_list")
			enable_Excel_list = Request.Form("enable_Excel_list")
			
			
			refresh_a = split(refresh_list,",")
			fgID_a = split(formgroup_id_list,",")
			BioViz_a = split(enable_BioViz_list,",")
			Excel_a = split(enable_Excel_list,",")
			
			for f = 0 to Ubound(refresh_a)
				isError = ""
				refresh = refresh_a(f)
				lgroupid = fgID_a(f)
				enable_bioviz = BioViz_a(f)
				enable_excel = Excel_a(f)
				if refresh then
					if Application("BIOVIZ_INTEGRATION")=1   then
						SetBioVizIntegration lGroupID, enable_bioviz
					end if
					if Application("EXCEL_INTEGRATION")=1   then
						SetExcelIntegration lGroupID, enable_excel
					end if
					CreateXMLTemplates(lgroupid)
			
					Set oRSName = oConn.Execute("select formgroup_name, description, is_public from db_formgroup where formgroup_id = " & lgroupID)
					fgName = oRSName("FORMGROUP_NAME")
					fgDesc = oRSName("DESCRIPTION")
					fgIsPublic = oRSName("IS_PUBLIC")
					
					set cmd = Server.CreateObject("adodb.command")
					cmd.ActiveConnection =  oConn
					cmd.CommandType = adCmdText
					cmd.CommandText = "select count(*) from BIOSARDB.db_vw_formgroup_tables where TABLE_ORDER = 1 and TABLE_NAME is NULL and formgroup_id=" & lgroupID
					cmd.Parameters.Append cmd.CreateParameter("plGroupID", 5, 1, 0, lGroupID) 
					Set oRSCount1 = cmd.Execute() 
					if Clng(oRSCount1.Fields(0).Value) > 0  then
						set cmd = Server.CreateObject("adodb.command")
						cmd.ActiveConnection =  oConn
					'	' display warning if no formitems found for table
					'	\<script language = "javascript">	noBaseTableFieldsWarning()</script><
						isError = " No fields in the base table"
					end if
			
					' we get here from the "Save Changes" button
					' check for no tables or no formitems
					'Dim oRSCount
					set cmd = Server.CreateObject("adodb.command")
					cmd.ActiveConnection =  oConn
					cmd.CommandType = adCmdText
					cmd.CommandText = "select count(*) from BIOSARDB.db_vw_formitems_all where formgroup_id = ?"
					cmd.Parameters.Append cmd.CreateParameter("plGroupID", 5, 1, 0, lGroupID) 
					Set oRSCount = cmd.Execute() 
			
					if Clng(oRSCount.Fields(0).Value) < 1 then
						' display warning if no formitems found for table
					'	<script language = "javascript">	noFieldsWarning()</script>
						isError = " No fields in the formgroup"
					end if
			
			        if isError = "" then
						'grant basetable to biosardb so that hitlist management will work
						GrantBTtoBiosardb lGroupID 
			
						'grant cartridge indexes to biosardb so that sdfilesearch will work
						GrantCartIndextoBiosardb lGroupID
			
						'sName = Replace(fgName, "'", "''")
						'sDesc = Replace(fgDesc, "'", "''")
				
						' DCW 04/18/2006, the Is_public cannot and should not be changed in a refresh
						'if current_roles_hidden <> "" then
						'	isPublic = "Y"
						'	AddPublicFormToRootNode lGroupID
						'else
						'	isPublic = "N"
						'	RemovePublicFormFromTree lGroupID
						'end if
			
						'set Public Flag
						'SetIsPublicFlag lGroupID, isPublic,sName,sDesc
			
						' DCW 04/18/2006, I don't think this is necessary either and this code wouldn't do the right thing anyway in a refresh context.
						
						'if the role isn't public then you need to grant the views to the user
						'if not  current_roles_hidden <> "" then
						'	GrantSelectOnViews(Session("username" & dbkey))
						'end if
					end if
					'if sSchemaErrs <> "" then
					'	<script language="javascript">schemaErrors()</script>
					'end if
					if isError <> "" then
						Response.Write fgName & " Failed to refresh: " & isError & "<BR>"
					else
						Response.Write fgName & " has been refreshed." & "<BR>"
					end if
					Response.Flush  
				End if
			next
			
		%>	
			<SCRIPT LANGUAGE=javascript>
				window.onload = function(){loadframes()}
				var navbar_add_on = '&page_name=refresh_form_log&formgroup_id=&referer_page=';
				function loadframes(){
					loadUserInfoFrame()
					//loadNavBarFrame('&page_name=refresh_form_log&formgroup_id=&referer_page=')
					}
			</SCRIPT>
			<form name="cancel_form" method="post" action="user_forms.asp?formgroup=base_form_group&formgroup_id=&dbname=biosar_browser&cancel=true">
				<input type="hidden" name="cancel" value>
			</form>
		<%	
			
			
			
			Response.end						
		case "delete_form"
			
			lgroupid = request("formgroup_id")
			
			' display warning
			%><script language="javascript">confirmDeleteForm()</script><%	
			
			lgroupid = ""
			
		case "really_delete_form"
			lgroupid = request("formgroup_id")
			'first drop the role if one exists
			Set oRSName = oConn.Execute("select formgroup_name,is_public from db_formgroup where formgroup_id = " & lgroupID)
			sGroupName = oRSName("FORMGROUP_NAME")
			sIsPublic = oRSName("is_public")
			oRSName.close
			
			if sIsPublic = "Y" then
				RemovePublicFormFromTree lGroupID
			end if
			
			'LJB 6/9 there is always a role for every formgroup so delete it.
			sPrivTable = "BIOSAR_BROWSER_PRIVILEGES"
			Set rs = oConn.Execute("select privilege_table_id from cs_security.privilege_tables where privilege_table_name = '" & sPrivTable & "'")
			sPrivTableId = rs("privilege_table_id")
			sOldRoleName = RolifyName(sGroupName, lgroupid)
			Set rs = oConn.EXecute("select role_id from cs_security.security_roles where role_name = '" & sOldRoleName & "'")
			if HasRecords(rs) then
				sOldRoleId = rs("role_id")
			else
				sOldRoleId = -1
			End if
			rs.close
			' drop existing role
			' will generate error if role doesn't exist
			On Error Resume Next
			oConn.Execute("drop role " & sOldRoleName)
			oConn.Execute("commit")
			On Error Goto 0
			' delete from security_roles
			oConn.Execute("delete from cs_security.security_roles where role_name = '" & sOldRoleName & "'")
			' delete from privileges
			oConn.Execute("delete from cs_security." & sPrivTable & " where role_internal_id = " & sOldRoleID)
		
			'now drop all info about the formgorup
			oConn.Execute SQLDeleteFormItemsByFormgroupId(lgroupid)
			oConn.Execute SQLDeleteFormsByFormgroupId(lgroupid)
		
			' get all the display tables names, add the formgroup_id and execute delet
			if Application("BIOVIZ_INTEGRATION")=1 and  Application("FORMGROUP_BIOVIZ_INTEGRATION"& lgroupid) = 1  then
					DeleteBioVizTableViews(lgroupid)
			end if
			oConn.Execute SQLDeleteFormgroupTablesByFormgroupId(lgroupid)
			oConn.Execute SQLDeleteFormgroupById(lgroupid)
			set cmd = Server.CreateObject("adodb.command")
			cmd.ActiveConnection =  oConn
			cmd.CommandType = adCmdText
			DeleteXMLTemplates cmd,lgroupid
			RemoveFormToRootNode lgroupid
			
			lgroupid = ""
		case "refresh_warning"
			Response.Write "<BR><BR><table width=""65%""><tr><td><p>The refresh operation is an administrative function which is typically only required following a version upgrade or service release.</p>  <p>Some updates require that preexisting forms be opened and re-saved in order for new functionality to take effect.</p>  <p>While this task can be accomplished by manually editing and re-saving each individual form, the process may be too time consuming and error prone.  This  refresh function allows you to simultaneously refresh (re-save) multiple preexisting forms.  It is functionality equivalent to the manual operation that could be performed by editing a single form at a time. In addition, this interface allows enabling/disabling the Excel and/or BioViz export capabilities for multiple forms.</p><p>Note that refreshing multiple forms may still be time consuming and should not be done unless a recent software upgrade warrants it<p><td></tr></table>"
			Response.Write "<span title=""cancel""><a class=""MenuLink"" href=""/biosar_browser/biosar_browser_admin/admin_tool/user_forms.asp?formgroup=base_form_group&dbname=biosar_browser"">Cancel</a></span>"
			Response.Write "&nbsp;|&nbsp;<span title=""continue""><a class=""MenuLink"" href=""user_forms.asp?dbname=biosar_browser&formgroup=base_form_group&action=refresh"" >Continue</a></span>"
			Response.Write "<form name=""cancel_form"" method=""post"" action=""user_forms.asp?formgroup=base_form_group&formgroup_id=&dbname=biosar_browser&cancel=true"">"
		

		%>	
			<SCRIPT LANGUAGE=javascript>
				window.onload = function(){loadframes()}
				var navbar_add_on = '&page_name=refresh_forms_warning&formgroup_id=&referer_page=';
				function loadframes(){
					loadUserInfoFrame()
					//loadNavBarFrame('&page_name=refresh_forms_warning&formgroup_id=&referer_page=')
					}
			</SCRIPT>
			
		<%	
			
			'Response.Write "  <input type=""hidden"" name=""cancel"" value>"
			'Response.Write " </form>"
			'Response.Write "<script language=javascript>window.onload = function(){loadframes()}"
			'Response.Write " function loadframes(){loadUserInfoFrame();loadNavBarFrame('&page_name=refresh_forms&formgroup_id=&referer_page=')}"
			'Response.Write "</script>"
			Response.end
			
		case "refresh"
			server.transfer "refresh_forms.asp"	
	end select
else ' cancelled still must update templates since formitems may have been changed
	lgroupid=request("formgroup_id")
	if lgroupid <> "" then
		if Application("BIOVIZ_INTEGRATION")=1   then
			enable_bioviz = request("enable_bioviz")
			SetBioVizIntegration lGroupID, enable_bioviz
		end if
		if Application("EXCEL_INTEGRATION")=1   then
			enable_excel = request("enable_excel")
			SetExcelIntegration lGroupID, enable_excel
		end if
		CreateXMLTemplates lgroupid
	end if
end if

%>


Select form and click an action.<p>
<p>
<form name=user_form_action method =post action ="/biosar_browser/biosar_browser_admin/admin_tool/user_forms.asp?dbname=biosar_browser&formgroup=base_form_group">
<table border=0 cellspacing=0 cellpadding=5>
<tr>
<td>
<%
	Dim oRS
	Dim lUserId
	if Session("EDIT_ANY_FORMGROUP" & dbkey) = true then
		'DCW added the currently logged in user_id into this function call.
		sql=SQLFormgroupsALL(UCase(Session("UserName" & Session("dbkey_admin_tools"))))
	else
		if Session("EDIT_GRANTED_FORMGROUP" & dbkey) = true then
			Set user_conn = UserConnection
			valid_user_public_formgroup_ids= getAllUserPublicFormgroups(dbkey, formgroup,UCase(Session("UserName" & Session("dbkey_admin_tools"))), user_conn)
			'valid_public_formgroups_not_owned = getAllPublicFormgrupsNotOwned(dbkey, formgroup,Session("UserName" & dbkey), oConn,valid_user_public_formgroup_ids)
			sql= SQLGrantedFormgroups(UCase(Session("UserName" & Session("dbkey_admin_tools"))), valid_user_public_formgroup_ids)
		else
			if Session("DUPLICATE_GRANTED_FORMGROUP" & dbkey) = true then
				valid_user_public_formgroup_ids= getAllUserPublicFormgroups(dbkey, formgroup, UCase(Session("UserName" & Session("dbkey_admin_tools"))), user_conn)
				if valid_user_public_formgroup_ids <> "" then
					dim dup_only_form_groups
					dup_only_form_groups = getAllPublicFormgrupsNotOwned(dbkey, formgroup,Session("UserName" & dbkey), oConn,valid_user_public_formgroup_ids)
					
					sql= SQLGrantedFormgroups(UCase(Session("UserName" & Session("dbkey_admin_tools"))), valid_user_public_formgroup_ids) 
				else
					sql=SQLFormgroupsByUserId(ucase(Session("UserName" & Session("dbkey_admin_tools"))))
				end if
			else
				sql=SQLFormgroupsByUserId(ucase(Session("UserName" & Session("dbkey_admin_tools"))))
				
			end if
			
		end if
	End if
	Set oRS = oConn.Execute(sql)
	dim bgcolor
	bgcolor = ""
	
	if not(request("action") = "duplicate_form" or request("action") = "really_delete_form") then
		lgroupID = request("formgroup_id")
	end if
	if Not (oRS.BOF and oRS.EOF) then
		Response.Write "<FONT FACE=""Lucida Console, Courier New, courier""><SELECT class=""SelectBox"" name=""form_id"" onChange=""doSwitchFormGroupID(&quot;" & dup_only_form_groups & "&quot;);return true"" >"
		Response.Write "<option value=""0"">--Select a Form--</option>"
		Do While Not oRS.EOF
				formgroup_name = oRS("FORMGROUP_NAME") 
				formgroup_id = oRS("FORMGROUP_ID") 
				formgroup_owner = oRS("USER_ID") 
				on error resume next
				if CLng(formgroup_id) = CLng(lgroupID) then
					if request("action") = "really_delete_form" then
						isSelected = ""
					else
						isSelected = "selected = true"
					end if
				else
					isSelected = ""
				end if
				
				if err.number <> 0 then
					Response.Write formgroup_id
				end if
				Application.Lock
					Application("FORMGROUP_FULL_NAME" & dbkey & formgroup_id) = formgroup_name
					Application("FORMGROUP_OWNER" & dbkey & formgroup_id) = formgroup_owner
				Application.UnLock
				description = oRS("DeSCRIPTION") 
				'if not UCase(Session("UserName" & Session("dbkey_admin_tools"))) = UCase(formgroup_owner) then
					addName = " (" & lcase(formgroup_owner) & ")"
				'else
				'	addName = ""
				'end if
				fulldescription = oRS("DeSCRIPTION") 
				if truncatedDesc <> "" then
					truncatedDesc = truncatedDesc & "<input type=""hidden"" name=""formgroup_desc_" & formgroup_id & """ value=""" & fulldescription &"""><input type=""hidden"" name=""formgroup_name_" & formgroup_id & """  value=""" & formgroup_name &""">"
				else
					truncatedDesc =  "<input type=""hidden"" name=""formgroup_desc_" & formgroup_id & """  value=""" & fulldescription &"""><input type=""hidden"" name=""formgroup_name_" & formgroup_id & """  value=""" & formgroup_name &""">"
				end if
				if len(description)>50 then
					description = Left(description,50) & "..."
				end if
				presented_formgroup_name = Left(formgroup_name, 30)
				presented_formgroup_name = presented_formgroup_name & String(30-Len(formgroup_name), Chr(160))
				presented_description = Left(description,50) 
				presented_description = presented_description & String(50-Len(description), Chr(160))
				fullString = presented_formgroup_name & " : " & presented_description &  addName
				Response.Write "<option " & " value=""" & formgroup_id  & """ " & isSelected & "><FONT FACE=""Lucida Console, Courier New, courier"">" & Server.HTMLEncode(fullString) &  "</FONT></option>"
			oRS.MoveNext
		Loop
		Response.Write "</SELECT></FONT>"
		response.Write truncatedDesc
	else
		Response.Write "<SELECT class=""SelectBox"" name=""form_id"" onChange=""doSwitchFormGroupID();return true"" >"
		Response.Write "<option value =>--Click Create New  Form--</option>"
		Response.Write "</SELECT>"
	end if
	%>

	</td>
	</tr></table><br><table><tr>
	<%
	current_form_restriction = request("form_restrict")
	if  lgroupID <> "" then
		select case UCase(current_form_restriction)
			Case "DUP_ONLY"%>
				<td><span title="insufficient privileges for edit"><a class="MenuLinkDisabled" href="#" >Edit</a></span></td>
				<td>|</td>
				<td><span title="insufficient privileges for rename"><a class="MenuLinkDisabled" href="#" >Rename</a></span></td>
				<td>|</td>
				<td><span title="duplicate this form"><a class="MenuLink" href="#" onclick="doFormAction('duplicate_form');return false">Duplicate</a></span></td>
				<td>|</td>
				<td><span title="insufficient privileges for delete"><a class="MenuLinkDisabled" href="#" >Delete</a></span></td>
				<td>|</td>
				<td><span title="open this form"><a class="MenuLink" href="#" onclick="doFormAction('restore');return false">Open</a></span></td>
			<%Case Else%>
				
				<td><span title="edit this form"><a class="MenuLink" href="#" onclick="doFormAction('edit');return false">Edit</a></span></td>
				<td>|</td>
				<td><span title="rename this form"><a class="MenuLink" href="#" onclick="doFormAction('do_rename');return false">Rename</a></span></td>
				<td>|</td>
				<td><span title="duplicate this form"><a class="MenuLink" href="#" onclick="doFormAction('duplicate_form');return false">Duplicate</a></span></td>
				<td>|</td>
				<td><span title="delete this form"><a class="MenuLink" href="#" onclick="doFormAction('delete_form');return false">Delete</a></span></td>
				<td>|</td>
				<td><span title="open this form"><a class="MenuLink" href="#" onclick="doFormAction('restore');return false">Open</a></span></td>

		<%End Select
	else%>
				<td><span title="no form selected"><a class="MenuLinkDisabled" href="#" >Edit</a></span></td>
				<td>|</td>
				<td><span title="no form selected"><a class="MenuLinkDisabled" href="#" >Rename</a></span></td>
				<td>|</td>
				<td><span title="no form selected"><a class="MenuLinkDisabled" href="#" >Duplicate</a></span></td>
				<%if Session("EDIT_ANY_FORMGROUP" & dbkey) then%>
				<td>|</td>
				<td><span title="Refresh form meta data"><a class="MenuLink" href="#" onclick="doFormAction('refresh_warning');return false" >Refresh</a></span></td>
				<%end if%>
				<td>|</td>
				<td><span title="no form selected"><a class="MenuLinkDisabled" href="#" >Delete</a></span></td>
				<td>|</td>
				<td><span title="no form selected"><a class="MenuLinkDisabled" href="#">Open</a></span></td>
	
	<%end if
	
%>
	
	</tr></form></table>
	


<form action="user_new_form.asp?dbname=biosar_browser&formgroup=base_form_group" method=post  name=admin_form>
<input type=hidden name="cancel" value="">
</form>



<!-- #INCLUDE FILE="footer.asp" -->
</body></html>