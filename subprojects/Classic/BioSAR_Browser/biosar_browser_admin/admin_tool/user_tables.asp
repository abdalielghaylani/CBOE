<%@ Language=VBScript %>
<html>
<head>

	<!-- #INCLUDE FILE="app_entry.asp" -->
	<!-- #INCLUDE FILE="header.asp" -->
	<!-- #INCLUDE FILE="xmltemplate_func.asp" -->

	<script language="javascript">
		w_current_list = ""
		orig_current_list = ""
		w_avail_list = ""
		sExcel=""
		sBioViz = ""
		return_to_form=""
		
	
	</script>

<%	'remove any stored cookies unless moving from tab to table, or adding a base table or child table info.
	nullOutFormCookies()
	
	Session("AllowOrganizeFGSubmenu") = true
	Session("AllowOrganizePublicFGSubmenu") = true


	session("user_columns_cancel_url") = Request.ServerVariables("query_string") & "&cancel=true"
	
	Dim lGroupId

	lGroupId = Request("formgroup_id")
	Dim sOwner

	Set oConn = SysConnection
	Dim oRS
		if ENABLE_PUBLIC_FORMGROUPS and Session("SET_FORMGROUP_PUBLIC" & "biosar_browser") then '  and CurrentUserIsFormOwner(sOwner) then
			Dim bodyLoadFunc
			bodyLoadFunc = " onload=MainWindow.fill_lists()"
		end if
	Dim sAction
	Dim sDisplay
	Dim bShowMainForm
	Dim sGroupName
	Dim lBaseTableId

	sAction = Request("action")
	sDisplay = Request("display_type")

	Dim sName, sDesc

'***************************************************************************************
'CODE FOR ACTIONS FOR POSTS 
'***************************************************************************************
	' perform necessary actions
	if request("cancel") = "" then
		Select Case sAction
			Case "switch"
				' switch positions of child tables
				lid1 = request("table_id_1")
				lid2 = request("table_id_2")
			
				Set oRSNew = Server.CreateObject("ADODB.Recordset")
				Set oRSNew2 = Server.CreateObject("ADODB.Recordset")
				oRSNew.Open SQLFormgroupTableByPK(lid1, lgroupid), oConn, 1, 3
				oRSNew2.Open SQLFormgroupTableByPK(lid2, lgroupid), oConn, 1, 3
				corder1 = orsnew("table_order")
				corder2 = orsnew2("table_order")
				oRSNew("table_order") = corder2
				oRSNew2("table_order") = corder1
				orsnew.Update
				orsnew.Close
				orsnew2.Update
				orsnew2.close
				bShowMainForm = true
			Case "change_base_table"
				' set up new base table
			
				lBaseTableId = request("table_id")
				if lBaseTableId <> "" then
					Dim oRSMod
					'delete all the formitems for the formgroup 
					oConn.Execute SQLDeleteFormItemsByFormGroupId(lGroupId)
					' since this is a new base table, remove all other tables first
					oConn.Execute SQLDeleteFormgroupTablesByFormgroupId(lGroupId)
					Set oRSMod = Server.CreateObject("ADODB.Recordset")		
					' add base table id to DB_FORMGROUP
					oRSMod.Open SQLFormgroupById(lgroupid), oConn, 1, 3
					oRSMod("BASE_TABLE_ID") = lBaseTableId
					oRSMod.update
					oRSMod.close
					if not lGroupID <> "" then
						lGroupID = request("formgroup_id")
					end if
					' add base table info to DB_FORMGROUP_TABLES
					oRSMod.Open SQLFormgroupTableByPK(lBaseTableId, lGroupId), oConn, 1, 3
					oRSMod.AddNew
					oRSMod("FORMGROUP_ID") = lGroupid
					oRSMod("TABLE_ID") = lBaseTableID
					oRSMod("TABLE_REL_ORDER") = 1
					oRSMod("TABLE_ORDER") = 1
			
					oRSMod.update
					oRSMod.close		
				end if
				bShowMainForm = true		
			Case "change_child_tables"	
				'LJB 3/2005 Check if Session("ChangeList") is populated to determine if the child table selection has changed.
				if Session("ChangeList") <> "" then
					lBaseTableId = Request("base_table_id")
					Dim oExTables, oRSChild
					Dim vChildTable
					' find out which child tables already exist
					Set oExTables = Server.CreateObject("Scripting.Dictionary")
					Set oRSChild = oConn.execute(SQLFormgroupTablesByFormgroupId(lgroupId))
					Dim lCount ' count for column ordering
					lCount = 1 ' 1 is reserved for the base table
				
				
					Do until oRSChild.eof
						if CSTr(oRSChild("TABLE_ID")) = CSTr(lBaseTableId) THen
							' always keep table if it is the base table
							oExTables.Add CSTr(oRSChild("TABLE_ID")), "keep"
						else
							on error resume next
							oExTables.Add CSTr(oRSChild("TABLE_ID")), "old"
							lCount = lCount + 1
						end if
						oRSChild.movenext
					loop
					'LJB 3/2005 use Session("ChangeList") that is populated in user_choose_child_tables.asp to keep track of added or removed child tables.
					
					temp = split(Session("ChangeList"), ",", -1)
					for i = 0 to UBound(temp)
						vChildTable = temp(i)
						if oExTables.Exists(CStr(vChildTable)) Then
							RemoveChildTable vChildTable, lgroupid
							RemoveChildFormItems vChildTable, lgroupid
						Else
							'increment count by 1 so that new child table will have a higher count then the last
							lCount = lCount + 1
							AddChildTable vChildTable, lgroupid, lCount
							
						End If
					Next
				
					FillTableOrder lGroupId, lBaseTableId, 2
				end if
				bShowMainForm = true		
			Case "edit_formitems"
		
					dim lTableId
					lTableId = Request("table_id")
					Dim oExistingItems, oRSItems
					Dim vColFormId
					' find out which formitems already exist
					Set oExistingItems = Server.CreateObject("Scripting.Dictionary")
					
					Set oRSItems = oConn.execute(SQLFormItemsByTableIdAndFormGroupId(lTableId, lgroupId))
				

					lCount = 0
					Do until oRSItems.eof
						on error resume next
						if oRSItems("V_COLUMN_ID") <>"" then
							colunn_id = oRSItems("V_COLUMN_ID")
						else
							colunn_id = oRSItems("COLUMN_ID")
						end if
						oExistingItems.Add CSTr(colunn_id) & "_" & CStr(orsItems("FORM_ID")), "old"
						' get max existing count
						if clng(oRSItemS("column_order")) > lCount then lcount = clng(orsitems("column_order"))
						oRSItems.movenext
					loop
					
					For Each vColFormId in Request("formitems_disp")
						if oExistingItems.Exists(CStr(vColFormId)) Then
							oExistingItems.Item(vColFormId) = "keep"
						Else
							oExistingItems.Add vColFormId, "new"
						End If
					Next
					
					' get next available count
					lCount = lCount + 1
				
					For Each vColFormId in oExistingItems.Keys
						If oExistingItems.Item(vColFormId) = "old" Then
							RemoveFormItem vColFormId
						ElseIf oExistingItems.Item(vColFormId) = "new" then
							defColOrder = request.Form("DefColOrder_"  & vColFormId)
							if isNull(defColOrder) or Not defColOrder <> "" then
								defColOrder = 0
							end if
							AddFormItem vColFormId, lTableId, defColOrder
							lCount = lCount + 1
						End if
					Next	
					' now that all formitems have been added and/or deleted,
					' change display options and widths if advanced view was 
					' selected
					if sDisplay = "complex" then
						Set oRSMod = Server.CreateObject("ADODB.Recordset")
						oRSMod.Open SQLFormitemsByTableIdAndFormgroupId(lTableId, lGroupId), oConn, 1, 3
						Dim lColId, lFormId
						Do Until oRSMod.eof
							vColID = oRSMod("V_COLUMN_ID")
							lColId = oRSMod("COLUMN_ID")
							if vColID <> "" then
								lColID = vColID
							end if
							lFormId = oRSMod("FORM_ID")
							if request("display_type" & lColId & "_" & lFormId) <> "" Then
								oRSMod("DISP_TYP_ID") = request("display_type" & lColId & "_" & lFormId)
							end if
							if request("col_disp_opt" & lColId & "_" & lFormId) <> "" Then
								oRSMod("DISP_OPT_ID") = request("col_disp_opt" & lColId & "_" & lFormId)
							End If
							if request("width" & lColId & "_" & lFormId) <> "" Then
								oRSMod("WIDTH") = request("width" & lColId & "_" & lFormId)
							end if
							if request("height" & lColId & "_" & lFormId) <> "" Then
								oRSMod("HEIGHT") = request("height" & lColId & "_" & lFormId)
							end if
							if request("formatMask" & lColId & "_" & lFormId) <> "" Then
								oRSMod("FORMAT_MASK").value = ltrim(request("formatMask" & lColId & "_" & lFormId))
							end if
							
							'JHS 07/30/2007 trying new hyperlink functionality
							if request("LINK" & lColId & "_" & lFormId) <> "" or (oRSMod("LINK").value <> request("LINK" & lColId & "_" & lFormId)) Then
								oRSMod("LINK").value = ltrim(request("LINK" & lColId & "_" & lFormId))
							end if
							if request("LINKTEXT" & lColId & "_" & lFormId) <> "" or (oRSMod("LINKTEXT").value <> request("LINKTEXT" & lColId & "_" & lFormId))  Then
								oRSMod("LINKTEXT").value = ltrim(request("LINKTEXT" & lColId & "_" & lFormId))
							end if							
							oRSMod.MoveNext
						Loop
						oRSMod.UpdateBatch
						oRSMod.Close
					end if
					
					bShowMainForm = true
				Case edit_formitem_order
					
					bShowMainForm=true
				Case Else
					bShowMainForm = True
			End Select
		else ' not cancelled
			' cancelled
			
			bShowMainForm = true
		end if

'***************************************************************************************
'CODE FOR BUILDING THE PAGE
'***************************************************************************************
	' wipe out session variable so the form will be reloaded
	Application.Lock
		Application("FORM_GROUP" & Session("dbkey_admin_tools") & lGroupId) = ""
	Application.UnLock
	Session("LIST_RS") = ""
	Session("DETAIL_RS") = ""
	Session("DETAIL_RS_FULL") = ""
	
	'get stored formgruop informtion from db_formgroup table
	dim ors2
	Dim lBaseColumnId
	Dim lChildTableId
	Dim sPublic
	Set oRS = Server.CreateObject("ADODB.RECORDSET")
	oRS.Open SQLFormgroupById(lGroupId), oConn,adOpenStatic,adLockOptimistic
	sGroupID = oRS.Fields("FORMGROUP_ID")
	sGroupName = oRS.Fields("FORMGROUP_NAME")
	sOwner = oRS.Fields("USER_ID")
	
	theReferrer = Request.ServerVariables("HTTP_REFERER")
	if (inStr(theReferrer,"user_tables.asp")=0 and inStr(theReferrer,"user_columns.asp")=0 and  inStr(theReferrer,"user_column_order.asp")=0 and  inStr(theReferrer,"user_choose_child_table.asp")=0 and   inStr(theReferrer,"user_choose_base_table.asp")=0)  then
		'this means that the the post is not from the form administration, but from a form list view, detail view or from user_forms.asp
		if request("return_to_form")<> "" then
			session("return_to_form") = "true" 
		else
			session("return_to_form") = "false" 
		end if
		sBioVIZ=oRS.Fields("BIOVIZ")
		sExcel=oRS.Fields("EXCEL")
		session("sBioVIZ") =sBioVIZ 
		session("sExcel") =sExcel 
	else
		if (inStr(theReferrer,"user_columns.asp")>0 or   inStr(theReferrer,"user_column_order.asp")>0 or   inStr(theReferrer,"user_choose_child_table.asp")>0 or    inStr(theReferrer,"user_choose_base_table.asp")>0)  then
		'if the return is from an internal form administration link
			sBioVIZ =session("sBioVIZ")
			sExcel =session("sExcel")
		else
			'get values from tab post
			sBioVIZ= request("enable_bioviz")
			sExcel= request("enable_excel")
			session("sBioVIZ") =sBioVIZ 
			session("sExcel") =sExcel 
		end if
	end if
	Application.Lock
		Application("FORMGROUP_ALLOWS_BIOVIZ_INTEGRATION"& sGroupID) = sBioVIZ
		Application("FORMGROUP_ALLOWS_EXCEL_INTEGRATION"& sGroupID) = sExcel
		Application("FORMGROUP_OWNER" & request("dbname") & sGroupID) =sOwner
	Application.UnLock
	
	sDesc = oRS("DESCRIPTION")
	if oRS("IS_PUBLIC") = "Y" Then
		sPublic = "checked"
	Else
		sPublic = ""
	End If

	lBaseTableId = CnvLong(oRS("BASE_TABLE_ID"), "DB_TO_VB")	
	oRS.Close
	oRS.Open SQLExposedTableById(lBaseTableid), oConn,adOpenStatic,adLockOptimistic

	
	theReferrer = Request.ServerVariables("HTTP_REFERER")
		if (inStr(theReferrer,"user_tables.asp")=0 and  inStr(theReferrer,"user_columns.asp")=0 and  inStr(theReferrer,"user_column_order.asp")=0 and  inStr(theReferrer,"user_choose_child_table.asp")=0 and   inStr(theReferrer,"user_choose_base_table.asp")=0)  then

		if ENABLE_PUBLIC_FORMGROUPS and Session("SET_FORMGROUP_PUBLIC" & "biosar_browser") and (CurrentUserIsFormOwner(sOwner) or Session("EDIT_ANY_FORMGROUP" & dbkey) = true  or Session("EDIT_GRANTED_FORMGROUP" & dbkey) = true)then
			' display form allowing user selection
			' get all users and roles in the PEOPLE table that have BIOSAR_BROWSER roles,
			' and are not formgroup roles that we created
			' this is the list of all available users and roles
			dim w_avail_list
			dim w_current_list
			Dim sUserSQL, sRoleSQL
			Dim sBioSARRoles
			Dim oRSAllUsers, oRSAllRoles
			Dim sFormgroupRole
			Dim sFGRoleSQL
			dim oRSFGRole
			Dim sFGUsersSQL, sFGRolesSQL
			Dim oRSFGUSers, oRSFGRoles
			sBioSARRoles = GetAllBioSARRoles(dbkey, formgroup)
		
			sFormgroupRole = RolifyName(sGroupName, sGroupId)
		
			sFGRoleSQL = "select * from cs_security.security_roles where role_name = '" & sFormgroupRole & "'"
			' now find out if there are existing roles and users that have been enabled
			Set oRSFGRole = oCOnn.Execute(sFGRoleSQL)
		
			if HasRecords(oRSFgRole) Then
				sFGUsersSQL = "select distinct 'USER: ' || grantee as user_id from dba_role_privs, dba_users " & _
							  "where dba_role_privs.granted_role = '" & oRSFGRole("role_name") & "'" & _
							  " and dba_role_privs.grantee = dba_users.username"
				sFGRolesSql = "select distinct 'ROLE: ' || grantee as role_name from dba_role_privs, dba_roles " & _
							  "where dba_role_privs.granted_role = '" & oRSFGRole("role_name") & "'" & _
							  " and dba_role_privs.grantee = dba_roles.role and Not dba_roles.role='BIOSAR_MASTER_ROLE'"
			end if
			
			'DGB Rewrite for performance improvements:			  
			'sUserSql = "Select Distinct 'XUSER: ' || grantee as user_id from dba_role_privs " &_
			'		   " WHERE dba_role_privs.granted_role IN (" & sBioSARRoles & ")" & _
			'		   " AND Upper(grantee) IN (Select Upper(USER_ID) FROM CS_SECURITY.People)" & _
			'		   " AND dba_role_privs.granted_role NOT IN " & _
			'		   "(SELECT role_name from cs_security.security_roles, cs_security.biosar_browser_privileges " & _
			'		   " where cs_security.biosar_browser_privileges.role_internal_id = cs_security.security_roles.role_id" & _
			'		   " and cs_security.biosar_browser_privileges.is_formgroup_role = 1)"
					   
			sUserSql = "Select Distinct 'USER: ' || grantee as user_id from dba_role_privs, CS_SECURITY.People " &_
					   " WHERE dba_role_privs.granted_role IN (" & sBioSARRoles & ")" & _
					   " AND grantee =USER_ID " & _ 
					   " AND dba_role_privs.granted_role NOT IN " & _
					   "(SELECT role_name from cs_security.security_roles, cs_security.biosar_browser_privileges " & _
					   " where cs_security.biosar_browser_privileges.role_internal_id = cs_security.security_roles.role_id" & _
					   " and cs_security.biosar_browser_privileges.is_formgroup_role = 1)"		   
					   
					   
			'if HasRecords(oRSFGRole) then
			'	sUserSQl = sUserSQL & " and 'XUSER: ' || grantee not in (" & sFGUsersSQL & ")"
			'end if
			
			if HasRecords(oRSFGRole) then
				sUserSQl = sUserSQL & _ 
				" MINUS " & _
				"	select 'USER: ' || grantee as user_id " & _
 				"	from dba_role_privs, dba_users " & _
 				"	where dba_role_privs.granted_role = '" & oRSFGRole("role_name") & "' " & _
 				"	and dba_role_privs.grantee = dba_users.username"
			end if
			
			sRoleSql = "Select 'ROLE: ' || role_name as full_role_name from cs_security.security_roles, cs_security.biosar_browser_privileges " & _
					   " where cs_security.biosar_browser_privileges.role_internal_id = cs_security.security_roles.role_id" & _
					   " and cs_security.biosar_browser_privileges.is_formgroup_role != 1"
					 
			'if HasRecords(oRSFGRole) then
			'	sRoleSql = sRoleSql & " and 'ROLE: ' || role_name not in (" & sFGRolesSQL & ")"
			'end if
			
			if HasRecords(oRSFGRole) then	
				sRoleSql = sRoleSql & _
				" MINUS " & _
				"select 'ROLE: ' || grantee as role_name from dba_role_privs, dba_roles " & _
							  "where dba_role_privs.granted_role = '" & oRSFGRole("role_name") & "'" & _
							  " and dba_role_privs.grantee = dba_roles.role and Not dba_roles.role='BIOSAR_MASTER_ROLE'"	
			end if
			'DGB End of rewrite for performance improvement.
			
			
			Set oRSAllUsers = oConn.execute(sUserSql)
			Set oRSAllRoles = oConn.execute(sRoleSQL)
		
			if HasRecords(oRSFGRole) then
				Set oRSFGUsers = oConn.Execute(sFGUsersSQL)
				Set oRSFGRoles = oConn.Execute(sFGRolesSQL)
				w_current_list = ListRSVals(oRSFGUsers, "user_id", "")
				w_current_list = ListRSVals(oRSFGRoles, "role_name", w_current_list)
			end if
			w_avail_list = ListRSVals(oRSAllUsers, "user_id", "")
			w_avail_list = ListRSVals(oRSAllRoles, "full_role_name", w_avail_list)
			session("w_avail_list") = w_avail_list
			session("w_current_list") = w_current_list
			%>
					<script language="javascript">
						w_current_list = "<%=w_current_list%>"
						orig_current_list = w_current_list
						w_avail_list = "<%=w_avail_list%>"
					</script>
				<%
		else

			if (inStr(theReferrer,"user_columns.asp")>0 or   inStr(theReferrer,"user_column_order.asp")>0 or   inStr(theReferrer,"user_choose_child_table.asp")>0 or    inStr(theReferrer,"user_choose_base_table.asp")>0)  then
			'if the return is from an internal form administration link
				w_avail_list=session("w_avail_list")
				w_current_list=session("w_current_list")
			else
				'get values from tab post
				w_current_list = Request("current_roles_hidden")
				w_avail_list = Request("roles_hidden")
				session("w_avail_list") = w_avail_list
				session("w_current_list") = w_current_list
			end if
			%><script language="javascript">
				w_current_list = "<%=w_current_list%>"
				orig_current_list = w_current_list
				w_avail_list = "<%=w_avail_list%>"
			</script><%
			
		end if
		
	else
		if (inStr(theReferrer,"user_columns.asp")>0 or   inStr(theReferrer,"user_column_order.asp")>0 or   inStr(theReferrer,"user_choose_child_table.asp")>0 or    inStr(theReferrer,"user_choose_base_table.asp")>0)  then
		'if the return is from an internal form administration link
			w_avail_list=session("w_avail_list")
			w_current_list=session("w_current_list")
		else
			'get values from tab post
			w_current_list = Request("current_roles_hidden")
			w_avail_list = Request("roles_hidden")
			session("w_avail_list") = w_avail_list
			session("w_current_list") = w_current_list
		end if

		%><script language="javascript">
				w_current_list = "<%=w_current_list%>"
				orig_current_list = w_current_list
				w_avail_list = "<%=w_avail_list%>"
				
		</script><%	
		
		
	end if
	
	'Set info for Organization tab
	if Request("current_roles_hidden") <> "" then
		AddPublicFormToRootNode lgroupid
		sPublic="checked"
	else
		RemovePublicFormFromTree formgroup_id
		sPublic=""
	end if
	'get all public form categories that this form is associated with
	if sPublic="checked" then
		sql = "select node_name, node_id, item_id, parent_id, tree_item.id  from biosardb.tree_node, biosardb.tree_item where  tree_type_id = 2 and tree_item.node_id=tree_node.id and tree_item.item_id=" & lGroupId
		AddFormtoPublicCategoryHelp = "&qshelp=Click the public form category in which you want this form to appear."
		managePublicFormCategoriesHelp = "&qshelp=Select a form category and choose an action from the menu that appears. Click refresh to update the tree."
		MovePublicFormHelp = "&qshelp=Click the public category to which you want to move this form."

		Set oRSPublicCats = Server.CreateObject("ADODB.Recordset")
		oRSPublicCats.Open sql, oconn, 1, 3
		if not (oRSPublicCats.BOF and oRSPublicCats.EOF) then
			Do while not oRSPublicCats.EOF
				node_id = oRSPublicCats("NODE_ID")
				item_id = oRSPublicCats("Item_id")
				id = oRSPublicCats("id")
				node_name = oRSPublicCats("Node_name")
				parent_id = oRSPublicCats("parent_id")
				pathtoNode = getNodePath(node_id,oConn)
				node_name_withSpan ="<span title="& quotedstring(pathtoNode) & ">" & node_name & "</span>"
				
				remove_text = "<a class=""MenuLink"" href=""#"" onclick=""OpenDialog3('/biosar_browser/source/treeview.asp?formgroup=" & lgroupid & "&qsrelay=formgroup&dbname=biosar_browser&TreeTypeID=2&TreeMode=remove_item&JsCallBack=EchoItemRemovedWithRefreshForm&ItemID=" & item_id & "&NodeID=" & node_id & "&ClearNodes=1&ItemTypeID=2','MyAddDialog',3)"">delete</a>"
				move_text = "<a class=""MenuLink"" href=""#"" onclick=""OpenDialog3('/biosar_browser/source/treeview.asp?formgroup=" & lgroupid & MovePublicFormHelp & "&qsrelay=formgroup&dbname=biosar_browser&TreeID=5&TreeTypeID=2&TreeMode=move_item&JsCallBack=EchoItemMovedWithRefreshForm&ItemID=" & id & "&NodeID=" & node_id & "&ClearNodes=1&ItemTypeID=2','MyAddDialog',3)"">move</a>"
				if public_form_categories <> "" then
					public_form_categories = public_form_categories  & "<tr><td width = ""200"">" & node_name_withSpan & "</td><td width = ""80"">" & move_text & "&nbsp;&nbsp;&nbsp;" & remove_text & "</td></tr>"
				else
					public_form_categories ="<table border=0 width = ""300"">" & "<tr><td width = ""200"">" & node_name_withSpan & "</td><td width = ""80"">" & move_text & "&nbsp;&nbsp;&nbsp;" & remove_text  & "</td></tr>"
				end if
			oRSPublicCats.MoveNext
			loop
		end if
	else
		public_form_categories = "Form is not public. You can make the form public by adding users and roles in the Security Tab."
	end if
	
	'get all private (user) form categories that this form is associated with
	sql = "select distinct node_name, node_id, item_id, parent_id, tree_item.id from biosardb.tree_node, biosardb.tree_item where tree_type_id = " & Session("PRIVATE_CATEGORY_TREE_TYPE_ID") & " and tree_item.node_id=tree_node.id and tree_item.item_id=" & lGroupId
	AddFormtoCategoryHelp = "&qshelp=Click the form category in which you want this form to appear."
	manageFormCategoriesHelp = "&qshelp=Select a form category and choose an action from the menu that appears. Click refresh to update the tree."
	MoveFormHelp = "&qshelp=Click the form category to which you want to move this form."
	PrivateCatID =Session("PRIVATE_CATEGORY_TREE_TYPE_ID")
	Set oRSPrivateCats = Server.CreateObject("ADODB.Recordset")
	oRSPrivateCats.Open sql, oconn, 1, 3
	if not (oRSPrivateCats.BOF and oRSPrivateCats.EOF) then
		Do while not oRSPrivateCats.EOF
			node_id = oRSPrivateCats("NODE_ID")
			id = oRSPrivateCats("id")
			item_id = oRSPrivateCats("item_id")
			node_name = oRSPrivateCats("Node_name")
			parent_id = oRSPrivateCats("parent_id")
			pathtoNode = getNodePath(node_id,oConn)
			node_name_withSpan ="<span title="&  quotedstring(pathtoNode) & ">" & node_name & "</span>"
			
			remove_text = "<a class=""MenuLink"" href=""#"" onclick=""OpenDialog3('/biosar_browser/source/treeview.asp?formgroup=" & lgroupid & "&qsrelay=formgroup&dbname=biosar_browser&TreeTypeID=" & PrivateCatID & "&TreeMode=remove_item&JsCallBack=EchoItemRemovedWithRefreshForm&ItemID=" & item_id & "&NodeID=" & node_id & "&ClearNodes=1&ItemTypeID=2','MyAddDialog',3)"">delete</a>"
			move_text = "<a class=""MenuLink"" href=""#"" onclick=""OpenDialog3('/biosar_browser/source/treeview.asp?formgroup=" & lgroupid & MoveFormHelp & "&qsrelay=formgroup&dbname=biosar_browser&TreeID=6&TreeTypeID=" & PrivateCatID & "&TreeMode=move_item&JsCallBack=EchoItemMovedWithRefreshForm&ItemID=" & id & "&NodeID=" & node_id & "&ClearNodes=1&ItemTypeID=2','MyAddDialog',3)"">move</a>"
			
			if private_form_categories <> "" then
				private_form_categories = private_form_categories  & "<tr><td width = ""200"">" & node_name_withSpan & "</td><td width = ""80"">" & move_text & "&nbsp;&nbsp;&nbsp;" & remove_text & "</td></tr>"
			else
				private_form_categories ="<table border=0 width = ""300"">" & "<tr><td width = ""200"">" &  node_name_withSpan & "</td><td width = ""80"">" & move_text & "&nbsp;&nbsp;&nbsp;" & remove_text  & "</td></tr>"
			end if
		oRSPrivateCats.MoveNext
		loop
		private_form_categories = private_form_categories & "</td></tr></table>"
	end if
	%>

</head>



<title>Manage Forms</title>
<body onload="fill_lists()">
	<form name="template_form" action="user_forms.asp?formgroup=base_form_group&amp;dbname=biosar_browser&amp;formgroup_id=<%=lGroupId%>&amp;action=update_templates" method="post" ID="Form2">

		<table border="0">
			<td class="form_purpose_header" valign="bottom">Form: </td>
			<td class="form_purpose" valign="bottom"><%=sGroupName%>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</td>
			<td class="form_purpose_header" valign="bottom">Description: </td>
			<td class="form_purpose" valign="bottom"><%=sDesc%>&nbsp;&nbsp;</td>
			<input type="hidden" name="enable_bioviz" value="<%=sBioViz%>">
			<input type="hidden" name="enable_excel" value="<%=sExcel%>">
			<input type="hidden" name="can_make_public" value="0">
			<script language="javascript">
				document.write("<input type=hidden name='return_to_form' value='" + return_to_form + "'>")
			</script>
			<br>
		</table>
	</form>

	<table border="0" width="200">
		<tr>
			<td><hr></td>
		</tr>
	</table>
	<form name="admin_form" onSubmit="doUpdateRoles()" action="user_forms.asp?formgroup=base_form_group&amp;dbname=biosar_browser&amp;formgroup_id=<%=lGroupId%>&amp;action=update_roles_and_templates" method="post">
		<table border="1" cellspacing="0" cellpadding="5" ID="Table8">
			<input type="hidden" name="new_name" value="<%=sgroupName%>" ID="Hidden6">
			<input type="hidden" name="new_desc" value="<%=sDesc%>" ID="Hidden8">
			<script language="javascript">
				document.write("<input type=hidden name='return_to_form' value='" + return_to_form + "'>")
			</script>
			<input type="hidden" name="current_roles_hidden" value="<%=w_current_list%>">
			<input type="hidden" name="enable_bioviz" value="<%=sBioViz%>">
			<input type="hidden" name="enable_excel" value="<%=sExcel%>">
			<input type="hidden" name="child_table_list" value>
			<input type="hidden" name="roles_hidden" value="<%=w_avail_list%>">
			<%if ENABLE_PUBLIC_FORMGROUPS and Session("SET_FORMGROUP_PUBLIC" & "biosar_browser") and (CurrentUserIsFormOwner(sOwner) or Session("EDIT_ANY_FORMGROUP" & dbkey) = true  or Session("EDIT_GRANTED_FORMGROUP" & dbkey) = true)then%>
			<input type="hidden" name="can_make_public" value="1">
			<%else%>
			<input type="hidden" name="can_make_public" value="0">
			<%end if%>
	
	<form name="security_form">
		<input type="hidden" name="can_make_public" value="1" ID="Hidden10">
		</table>
	</form>
	
<%		'build tab control
		Session("sTab") = "0"
		if Session("sTab") = "" then Session("sTab") = "0"
		If sTab = "" Then sTab = Session("sTab") 
		Session("sTab") = Request.querystring("TB")
		set TabView = Server.CreateObject("VASPTB.ASPTabView")

		'create the tab view object
		TabView.Class = "TabView"
		TabView.ImagePath = "/biosar_browser/graphics/images/tabview"
		TabView.BackColor = "#d3d3d3"
		TabView.SelectedBackColor = "#adadad"
		TabView.SelectedForeColor = ""
		TabView.SelectedBold = True
		TabView.BodyBackground = "#cococo"
		TabView.TabWidth = 0
		TabView.StartTab = "0"
		'TabView.QueryString = "output_type=tab&dbname=" & dbkey & "&formgroup=base_form_group&formmode=admin" & "&formgroup_id=" & lgroupid
		TabView.LicenseKey = "7993-3E51-698B"
	
		
		'define all the tabs
		Set Tabx = TabView.Tabs.Add(0,"Tables","#","","Add/remove tables and fields")
		Tabx.Image = ""
		Tabx.SelectedImage = ""
		Tabx.ForceDHTML =True
		Tabx.DHTML = "onclick=tabPost('0','"& lGroupid & "') onmouseover=javascript:this.style.cursor='hand' onmouseout=javascript:this.style.cursor='default'"

		if Application("BIOVIZ_INTEGRATION")=1 or Application("EXCEL_INTEGRATION")=1 then
			Set Tabx = TabView.Tabs.Add(1,"Integration","#","","Add/remove integration features")
			
			Tabx.Image = ""
			Tabx.SelectedImage = ""
			Tabx.ForceDHTML =True
			Tabx.DHTML = "onclick=tabPost('1','"& lGroupid & "') onmouseover=javascript:this.style.cursor='hand' onmouseout=javascript:this.style.cursor='default'"

		end if
		
		if ENABLE_PUBLIC_FORMGROUPS and Session("SET_FORMGROUP_PUBLIC" & "biosar_browser") and (CurrentUserIsFormOwner(sOwner) or Session("EDIT_ANY_FORMGROUP" & dbkey) = true  or Session("EDIT_GRANTED_FORMGROUP" & dbkey) = true)then
			Set Tabx = TabView.Tabs.Add(2,"Security","#","","Add/remove form security")
			Tabx.Image = ""
			Tabx.SelectedImage = ""
			Tabx.ForceDHTML =True
			Tabx.DHTML = "onclick=tabPost('2','"& lGroupid & "') onmouseover=javascript:this.style.cursor='hand' onmouseout=javascript:this.style.cursor='default'"

		end if
		
		Set Tabx = TabView.Tabs.Add(3,"Organization","#","","Add/remove form from category")
		Tabx.Image = ""
		Tabx.SelectedImage = ""
		Tabx.ForceDHTML =True
		Tabx.DHTML = "onclick=tabPost('3','"& lGroupid & "') onmouseover=javascript:this.style.cursor='hand' onmouseout=javascript:this.style.cursor='default'"

		output = ""
		
		'output the tabs
		Response.Write TabView.ViewSource

		Set TabView = Nothing
		tab_name = Session("sTab") 
		
		'figure out which tab we are at and output the tab
		Select Case UCase(tab_name)
			Case "0"
				Response.Write "<br>"
				Response.Write "<table><tr><td>Select the base table and the child tables for this form.</td></tr></table>"
				Response.Write "<table border = 0 width = 200><tr><td><br><hr></td></tr></table>"
				
				GetTablesTabInfo()
				
			Case "1"
				if Application("BIOVIZ_INTEGRATION")=1 and Application("EXCEL_INTEGRATION")=1 then
					Response.Write "<br>"
					Response.Write "<table><tr><td>Click the checkboxes below to allow your search result to be exported to you local Microsoft Excel and/or ChemFinder BioViz.</td></tr></table>"
					Response.Write "<table border = 0 width = 200><tr><td><br><hr></td></tr></table>"
				end if
				if Application("BIOVIZ_INTEGRATION")=1 and Not Application("EXCEL_INTEGRATION")=1 then
					Response.Write "<br>"
					Response.Write "<table><tr><td>Click the checkboxes below to allow your search result to be exported to ChemFinder BioViz.</td></tr></table>"
					Response.Write "<table border = 0 width = 200><tr><td><br><hr></td></tr></table>"
				end if
				if Not Application("BIOVIZ_INTEGRATION")=1 and  Application("EXCEL_INTEGRATION")=1 then
					Response.Write "<br>"
					Response.Write "<table><tr><td>Click the checkboxes below to allow your search result to be exported to you local Microsoft Excel.</td></tr></table>"
					Response.Write "<table border = 0 width = 200><tr><td><br><hr></td></tr></table>"
				end if
				GetIntegrationTabInfo()
				
				
			case "2"
				if ENABLE_PUBLIC_FORMGROUPS and Session("SET_FORMGROUP_PUBLIC" & "biosar_browser") and (CurrentUserIsFormOwner(sOwner) or Session("EDIT_ANY_FORMGROUP" & dbkey) = true  or Session("EDIT_GRANTED_FORMGROUP" & dbkey) = true)then

					Response.Write "<br>"
					Response.Write "<table><tr><td>Select roles and users from the list below to make this form available to others.</td></tr></table>"
					Response.Write "<table border = 0 width = 200><tr><td><br><hr></td></tr></table>"

					GetSecurityTabInfo()
				end if
				
			case "3"
			
				Response.Write "<br>"
				Response.Write "<table><tr><td>Change or add form categories that this form will appear when you select Forms->Open Form. </td></tr></table>"
				Response.Write "<table border = 0 width = 200><tr><td><br><hr></td></tr></table>"

				GetOrganizationTabInfo()
				
			case else
			
				Response.Write "<br>"
				Response.Write "<table><tr><td>Select the base table and the child tables for this form.</td></tr></table>"
				Response.Write "<table border = 0 width = 200><tr><td><br><hr></td></tr></table>"
				
				GetTablesTabInfo()
				
		End Select
	
		'JHS 4/23/2007
		'CSBR-71038 is fixed in the cancel_form below
		'user_forms was looking for enable_excel and enable_bioviz requests which were not being posted
		'adding them to the form posts the correct info as the excel and bioviz status has 
		'already been determined at this point and put into a session object
		%>
	<form name="cancel_form" method="post" action="user_forms.asp?<%=session("user_forms_cancel_url")%>" ID="Form1">
		<input type="hidden" name="cancel" value>
		<input type="hidden" name="enable_excel" value="<%=session("sExcel")%>" />
		<input type="hidden" name="enable_bioviz" value="<%=session("sBioViz")%>" />
	</form>


<%'Tab  Functions

Sub GetTablesTabInfo()%>
	
	<%If SINGLE_CHILD_LEVEL Then
		if HasRecords(oRS) Then%>		
			
		<table border="0" width="400">
			<tr>
				<td class="table_description"><b><font size="+1">Base Table</font>&nbsp;&nbsp;<a class="MenuLink" href="javascript:basetableWarning()">[Edit Base Table]</a></b></td>
			</tr>
		</table>
			
			
		<%else%>
		<table border="0" width="400">
			<tr>
				<td class="table_description"><b><font size="+1">Base Table</font>&nbsp;&nbsp;<a class="MenuLink" href="user_choose_base_table.asp?dbname=biosar_browser&amp;formgroup=base_form_group&amp;formgroup_id=<%=lgroupid%>&amp;curr_id=<%=lBaseTableid%>">[Select Base Table]</a></b></td>
			</tr>
		</table>
			
			
		<%end if%>
		<table border="1" cellspacing="0" cellpadding="5" ID="Table1" class="table_list" width="400">
			<tr>
				<td class="table_cell_header">Table</td>
				<td class="table_cell_header">Fields</td>
				<td class="table_cell_header">Field Options</td>
				<td class="table_cell_header">Field Order</td>
			</tr>
			<%if HasRecords(oRS) Then%>
				<tr>
					<td><%=server.HTMLEncode(oRS.fields("DISPLAY_NAME"))%>&nbsp;&nbsp;</td>
					<td class="table_cell" align="center">
						<a class="MenuLink" href="user_columns.asp?dbname=biosar_browser&amp;formgroup=base_form_group&amp;display_type=simple&amp;formgroup_id=<%=lgroupid%>&amp;table_id=<%=lbasetableid%>">Edit</a>
					</td>
					<td class="table_cell" align="center">
						<a class="MenuLink" href="user_columns.asp?dbname=biosar_browser&amp;formgroup=base_form_group&amp;display_type=complex&amp;formgroup_id=<%=lgroupid%>&amp;table_id=<%=lbasetableid%>">Edit</a>
					</td>
					<td class="table_cell" align="center">
						<a class="MenuLink" class="MenuLink" href="user_column_order.asp?dbname=biosar_browser&amp;formgroup=base_form_group&amp;formgroup_id=<%=lgroupid%>&amp;table_id=<%=lbasetableid%>&amp;form_type=1">Query</a>
						 | 
						 <a class="MenuLink" href="user_column_order.asp?dbname=biosar_browser&amp;formgroup=base_form_group&amp;formgroup_id=<%=lgroupid%>&amp;table_id=<%=lbasetableid%>&amp;form_type=2">List</a>
						| 
						 <a class="MenuLink" href="user_column_order.asp?dbname=biosar_browser&amp;formgroup=base_form_group&amp;formgroup_id=<%=lgroupid%>&amp;table_id=<%=lbasetableid%>&amp;form_type=3">Detail</a>
					</td>
				</tr>
		 
			<%End IF%>
		</table>
		<%	oRS.Close
		end if%>	
			<br>
			
			<table border="0" width="200">
				<tr>
					<td><hr></td>
				</tr>
			</table>
			<%
			'query string parameters for displaying child tables in a tree
			sQSHelpText = "&QSHelp=Select the child tables to be shown on this form."
			sTreeMode = "&TreeID=8&TreeMode=multi_select"
			sItemCheckbox="&ItemCheckbox=userids"
			sJSCallback="&JsCallback=updateTableChangeList(this.name)"
			sItemFilterSelect="&ItemFilterSelect=select table_id" &_
				" from BIOSARDB.db_table, all_tables " &_
				" where is_exposed = 'Y' and " &_
				" table_id in (select child_table_id from BIOSARDB.db_relationship where BIOSARDB.db_relationship.table_id = " & lbasetableid & ") and "&_
				" db_table.table_short_name = all_tables.table_name" &_
				" UNION select table_id" &_
				" from BIOSARDB.db_table, all_views " &_
				" where is_exposed = 'Y' and " &_
				" table_id in (select child_table_id from BIOSARDB.db_relationship where BIOSARDB.db_relationship.table_id = " & lbasetableid & ") and "&_
				" db_table.table_short_name = all_views.view_name"
			sCheckboxSelect="&CheckboxSelect=select table_id from BIOSARDB.db_formgroup_tables where formgroup_id =" & lgroupid & " and table_order > 1"
			sClearNodes="&ClearNodes=0"
			sItemQSId="&ItemQSId=table_id"
			sBaseTableid ="&base_table_id=" & lbasetableid
			sFormgroup ="&formgroup_id=" & lgroupid & "&formgroup=base_form_group"
			srefreshPath="&refreshPath=/biosar_browser/biosar_browser_admin/admin_tool/user_choose_child_table.asp"
			sQsRelay="&QsRelay=formgroup,formgroup_id,base_table_id,refreshPath,qshelp"

			AppendtoChildTableURL = "&TreeTypeID=1&ItemTypeID=1" & sTreeMode & sJSCallback & sItemCheckbox & sItemFilterSelect & sCheckboxSelect & sQsRelay & sBaseTableid & sFormgroup & sItemQSId & sClearNodes & sShowRefresh & sQSHelpText & srefreshPath
			
			
			
			oRS.Open SQLFormgroupTablesByFormgroupId(lgroupid), oConn,adOpenStatic,adLockOptimistic
			if HasRecords(oRS) Then%>
			<table border="0" width="400">
				<tr>
					<td class="table_description"><b><font size="+1">Child Tables</font>&nbsp;&nbsp;<a class="MenuLink" href="user_choose_child_table.asp?dbname=biosar_browser<%=AppendtoChildTableURL%>">[Select Child Tables]</a></b></td>
				</tr>
			</table>
			<%else%>
			<table border="0" width="400">
				<tr>
					<td class="table_description"><b><font size="+1">Child Tables</font>&nbsp;&nbsp;<a class="MenuLink" href="user_choose_child_table.asp?dbname=biosar_browser<%=AppendtoChildTableURL%>">[Edit Child Tables]</a></b></td>
				</tr>
			</table>
			<%end if%>
			<table border="1" cellspacing="0" cellpadding="5" ID="Table2" class="table_list">
				<tr>
					<td class="table_cell_header">Table</td>
					<td class="table_cell_header">Fields</td>
					<td class="table_cell_header">Field Options</td>
					<td class="table_cell_header">Field Order</td>	
					<td class="table_cell_header">Table Order</td>
				</tr>	
	
	
	
	
			
		<%' for now, just do one level of child tables
		if HasRecords(oRS) Then
			' get dict so we know which formitems precede and follow
			set odict = server.CreateObject("scripting.dictionary")
			ors.movefirst
			lcount = 1
			do until ors.eof
				s = ors("table_id")
				if cstr(s) <> cstr(lBaseTableId) THen
					odict.Add cstr(lcount), s
					lcount = lCount + 1
				end if
				ors.movenext
			loop

			lmax = lcount - 1
			lcount = 1
			ors.movefirst
			Do until ors.eof
				lChildTableid = oRS.Fields("TABLE_ID")
				if cstr(lChildTableId) <> cstr(lBaseTableId) THen
				Set ors2 = oconn.execute(SQLExposedTableById(lchildtableid))%>
				
				<tr>
					<td><%=Server.HTMLEncode(oRS2.fields("DISPLAY_NAME"))%></td>
					<td class="table_cell" align="center">
						<a class="MenuLink" href="user_columns.asp?dbname=biosar_browser&amp;formgroup=base_form_group&amp;display_type=simple&amp;formgroup_id=<%=lgroupid%>&amp;table_id=<%=lchildtableid%>">Edit</a>
					</td>
					<td class="table_cell" align="center">
						<a class="MenuLink" href="user_columns.asp?dbname=biosar_browser&amp;formgroup=base_form_group&amp;display_type=complex&amp;formgroup_id=<%=lgroupid%>&amp;table_id=<%=lchildtableid%>">Edit</a>
					</td>
					<td class="table_cell" align="center">
						<a class="MenuLink" href="user_column_order.asp?dbname=biosar_browser&amp;formgroup=base_form_group&amp;formgroup_id=<%=lgroupid%>&amp;table_id=<%=lchildtableid%>&amp;form_type=1">Query</a>
						| 
						<a class="MenuLink" href="user_column_order.asp?dbname=biosar_browser&amp;formgroup=base_form_group&amp;formgroup_id=<%=lgroupid%>&amp;table_id=<%=lchildtableid%>&amp;form_type=2">List</a>
						| 
						<a class="MenuLink" href="user_column_order.asp?dbname=biosar_browser&amp;formgroup=base_form_group&amp;formgroup_id=<%=lgroupid%>&amp;table_id=<%=lchildtableid%>&amp;form_type=3">Detail</a>
					</td>
					<td class="table_cell" align="center">

						<%if lcount > 1 then%>
			
							<a class="MenuLink" href="user_tables.asp?dbname=biosar_browser&amp;formgroup=base_form_group&amp;formgroup_id=<%=lgroupid%>&amp;action=switch&amp;table_id_1=<%=oRS("table_id")%>&amp;table_id_2=<%=odict(cstr(lcount - 1))%>">
							<img src="up.gif" border="0" WIDTH="17" HEIGHT="22"></a>

						<%else%>
						&nbsp;
						<%end if
						if lcount < lMax then%>

							<a class="MenuLink" href="user_tables.asp?dbname=biosar_browser&amp;formgroup=base_form_group&amp;formgroup_id=<%=lgroupid%>&amp;action=switch&amp;table_id_1=<%=oRS("table_id")%>&amp;table_id_2=<%=odict(cstr(lcount + 1))%>">
							<img src="down.gif" border="0" WIDTH="17" HEIGHT="22"></a>

					<%else%>
					&nbsp;
					<%end if%>

					</td>
				</tr>
					

					<%lcount = lCount + 1
				end if
				ors.movenext
			loop
		end if%>
	

		</table><br>
<%end Sub


Sub GetIntegrationTabInfo()
	%>
	<form name="integration_form" action="user_forms.asp?formgroup=base_form_group&amp;dbname=biosar_browser&amp;formgroup_id=<%=lGroupId%>&amp;action=update_templates" method="post" ID="Form2">

<input type="hidden" name="stop_the_error" value="here">
	
	<table border="0" width="300">
		<%if Application("BIOVIZ_INTEGRATION")=1   then
				if sBioViz = "1" then
					isChecked = " checked"
				else
					isChecked = ""
				end if%>
				
		
				<tr>
					<strong>
						<td valign="top">Enable Export to ChemFinder BioViz</td><td>
						<% if detectIE5 then%>
						<input type="checkbox" name="enable_bioviz_cb" <%=isChecked%> onBlur="setIntegrationCookie('bioviz');return true">
						<%else%>
						<input type="checkbox" name="enable_bioviz_cb" <%=isChecked%> onChange="setIntegrationCookie('bioviz');return true">

						<%end if%>
						</td><td><a href="javascript:helpMsg('enable_bioviz')"><img border="0" src="/biosar_browser/graphics/help.gif" width="15" height="15"></a><td>
					</strong>
				</tr>
			
			<%end if
			if Application("EXCEL_INTEGRATION")=1   then
				if sExcel = "1" then
					isEChecked = " checked"
				else
					isEChecked = ""
				end if
			%>
				<tr>
					<strong>
						<td valign="top">Enable Export to Microsoft Excel</td><td>
						<% if detectIE5 then%>
						<input type="checkbox" name="enable_excel_cb" <%=isEChecked%> onBlur="setIntegrationCookie('excel');return true">
						<%else%>
						<input type="checkbox" name="enable_excel_cb" <%=isEChecked%> onChange="setIntegrationCookie('excel');return true">

						<%end if%>
						</td><td><a href="javascript:helpMsg('enable_excel')"><img border="0" src="/biosar_browser/graphics/help.gif" width="15" height="15"></a><td>
					</strong>
				</tr>
			
			<%end if%>
		</table>
	</form>
<%end Sub


Sub GetSecurityTabInfo()
		if ENABLE_PUBLIC_FORMGROUPS and Session("SET_FORMGROUP_PUBLIC" & "biosar_browser") and (CurrentUserIsFormOwner(sOwner) or Session("EDIT_ANY_FORMGROUP" & dbkey) = true  or Session("EDIT_GRANTED_FORMGROUP" & dbkey) = true)then

	%>
		
		
	
	<form name="security_form">
		<input type="hidden" name="can_make_public" value="1" ID="Hidden10">
				
				
				<tr>
					</tr>					<td align="center" colspan="2">
						<table width="544" ID="Table4">
							<tr>
								<td width="294">
									<table ID="Table5">
										<tr>
											<td width="436" nowrap colspan="2">
												<table border="0" width="100%" ID="Table6">
													
						  
													<input type="hidden" name="is_public" value="<%=sPublic%>" ID="Hidden3">
													<tr>
													  <td width="20%">All Users and Roles</td>
													  <td width="20%"></td>
													  <td width="20%">Permitted Users And Roles </td>
													</tr>
													<tr>
													  <td width="20%">
														<script language="javascript">
																buildLeftMultiSelectBox('roles', 6)
														</script>
													  </td>
													  <td width="20%">
															<table border="0" width="100%" ID="Table7">
																	<tr>
																		<td width="100%"><a href="javascript:addCurrentList()"><img SRC="/<%=Application("AppKey")%>/graphics/add_role_btn.gif" BORDER="0"></a></td>
																	</tr>
																	<tr>
																		<td width="100%"><a href="javascript:removeCurrentList()"><img SRC="/<%=Application("appkey")%>/graphics/remove_role_btn.gif" BORDER="0"></a></td>
																	</tr>
															</table>
													 </td>
												    <td width="20%">
														 <script language="javascript">
																buildRightMultiSelectBox('current_roles', 6)
														 </script>
													</td>
												  </tr>
												</table>
											</td>
										</tr>
									<tr>
										<td colspan="2" width="275">
										</td>
									</tr>
							</table>
					</td>
				</table>
			</table>
		</form>
		
	<%else%>
			<form name="security_form">
				<input type="hidden" name="can_make_public" value="0" ID="Hidden10">
			</form>
	<%end if ' Display Public Formgroups
end Sub


Sub GetOrganizationTabInfo()%>
	
	<table width="400">
		<tr>
			<td class="table_description" nowrap><b><font size="+1">Private Form Categories&nbsp;&nbsp;&nbsp;<a class="MenuLink" href="#" onclick="OpenDialog3('/biosar_browser/source/treeview.asp?dbname=biosar_browser<%=AddFormtoCategoryHelp%>&amp;TreeID=9&amp;TreeTypeID=<%=Session("PRIVATE_CATEGORY_TREE_TYPE_ID")%>&amp;ItemTypeId=2&amp;itemid=<%=lgroupid%>&amp;showitems=0&amp;ClearNodes=1&amp;TreeMode=add_item&amp;jscallback=EchoItemAddedWithRefreshForm','MySelectItemDiag',3)">[Add to Private Category]</a></font></b></td>
		</tr>
	</table>
	<table width="400" border="0" class="table_list">
		<tr>
			<td class="table_cell"><%=private_form_categories%></td>
		</tr>
	</table>
	<form name="org_refresh" method="post" action="%3cbr">
	<%if Session("SET_FORMGROUP_PUBLIC" & dbkey) = true then%>
		<table border="0" width="200">
			<tr>
				<td><br><hr></td>
			</tr>
		</table>
	<% if sPublic="checked" then%>
		<table width="400">
			<tr>
				<td class="table_description" nowrap><b><font size="+1">Public Form Categories&nbsp;&nbsp;&nbsp;<a class="MenuLink" href="#" onclick="OpenDialog3('/biosar_browser/source/treeview.asp?dbname=biosar_browser<%=AddFormtoPublicCategoryHelp%>&amp;TreeID=11&amp;TreeTypeID=2&amp;ItemTypeId=2&amp;itemid=<%=lgroupid%>&amp;showitems=0&amp;ClearNodes=1&amp;TreeMode=add_item&amp;jscallback=EchoItemAddedWithRefreshForm','MySelectItemDiag',3)">[Add to Public Category]</a></font></b></td>
			</tr>
		</table>
	<%else%>
		<table width="400">
			<tr>
				<td class="table_description" nowrap><b><font size="+1">Public Form Categories&nbsp;&nbsp;&nbsp;</font></b></td>
			</tr>
		</table>
	
	<%end if%>
		
	
		<table width="400" border="0" class="table_list">
			<tr>
				<td class="table_cell"><%=public_form_categories%></td>
			</tr>
		</table>
	<%end if%>
	</form>
<%End Sub%>



<script language="javascript">
					fill_lists()
				</script>	
<!-- #INCLUDE FILE="footer.asp" -->
</body>
</html>
<%' page support functions
function AddFormItem(ByVal ColFormId, ByVal TableId, ByVal ColOrder)
	Dim aAr
	aAr = Split(ColFormId, "_")
	Dim oRSNew
	Set oRSNew = Server.CreateObject("ADODB.Recordset")
	vColumn = false
	if inStr(aAr(0), "-")> 0 then
		vColumn = true
		clean_lColID = replace(aAr(0), VCOL_STRUCTURE, "")
		clean_lColID = replace(clean_lColID, VCOL_FORMULA, "")
		clean_lColID = replace(clean_lColID, VCOL_MOLWEIGHT, "")
		v_column_name = aAr(0)
	else
		clean_lColID =aAr(0)
		v_column_name = Null
	end if
	oRSNew.Open "BIOSARDB.db_form_item", oConn, 1, 3,adCmdTable
	oRSNew.AddNew
	oRSNew("TABLE_ID") = TableId
	oRSNew("COLUMN_ID") = clean_lColID
	oRSNew("V_COLUMN_ID") = v_column_name
	oRSNew("FORM_ID") = aAr(1)
	oRSNew("COLUMN_ORDER") = ColOrder
	Dim lDispType
	
	' set display type to first value in list (default)
	'LJB 6/18/2002 needed to add tableID to all structure/formula/displaytype column ids. 
	' Otherwise these columns are not unique and you can't have more then one structure in a formgroup
	' Response.Write "AddFormItem: " & aar(0) & "<br>"
	Select Case Clng(aAr(0))
		Case CLng(VCOL_STRUCTURE & clean_lColID)
			lDispType = DISP_TYP_STRUCTURE
		Case CLng(VCOL_FORMULA & clean_lColID)
			lDispType = DISP_TYP_FORMULA
		Case CLng(VCOL_MOLWEIGHT & clean_lColID)
			lDispType = DISP_TYP_MOLWEIGHT
		Case ELse
			Dim aAr2
			Dim oRsCol
			
			Set oRSCol = oConn.Execute(SQLColumnById(clean_lColID))
			
			aAr2 = Split(DisplaytypeIdsForColumn(oRSCol), ",")
			lDispType = aAr2(0)
	End Select
	oRSNew("DISP_TYP_ID") = lDispType
	Dim oRSTemp
	' get display options
	Set oRSTemp = oConn.Execute("select * from BIOSARDB.db_dtyp_dopt where disp_typ_id = " & lDispType)
	if not(oRSTemp.bof and oRSTemp.eof) then
		' default to plug-in for structure
		if lDispType= DISP_TYP_STRUCTURE then
			oRSNew("DISP_OPT_ID") = 2
		else
			oRSNew("DISP_OPT_ID") = oRSTemp("DISP_OPT_ID")
		end if
	end if
	Set oRSTemp = oconn.execute(SQLDisplayTypeById(lDispType))
	oRSNew("WIDTH") = oRSTemp("DEFAULT_WIDTH")
	oRSNew("HEIGHT") = oRSTemp("DEFAULT_HEIGHT")
	oRSNew.Update
	oRSNew.Close
end function

function RemoveFormItem(ByVal ColFormid)
	' remove a form item
	Dim aAr
	aAr = Split(ColFormId, "_")
	if inStr(aAr(0), "-")> 0 then
		oConn.Execute SQLDeleteFormItemByVirtualColumnIdAndFormId(aAr(0), aAr(1))
	else
		oConn.Execute SQLDeleteFormItemByColumnIdAndFormId(aAr(0), aAr(1))
	end if
	
end function

function AddChildTable(ByVal TableId, byval formgroupid, byVal Count)
	Dim defaultFormGroupID
	defaultFormGroupID = Application("DEFAULT_FORM_ID")
	Dim oRSNewTable
	Set oRSNewTable = Server.CreateObject("ADODB.Recordset")
	oRSNewTable.Open SQLFormgroupTableByPK(TableId, formgroupid), oConn, 1, 3
	oRSNewTable.addnew
	orsnewtable("TABLE_ID") = tableid
	oRSNewTable("formgroup_id") = formgroupid
	orsnewtable("table_order") = count
	oRSNewTable.Update
	oRSNewTable.Close
	'DCW: 2/17/2004 the following section of code was added to copy layout information from a default form.
	' get form ids from DB_FORM
	if defaultFormGroupID <> 0 then
		set rs = oconn.execute("select * from biosardb.db_form where formgroup_id = " & formgroupid)
		do until rs.eof
			this_formtype_id = clng(rs("formtype_id"))
			this_form_id = rs("form_id")
			sql = "insert into biosardb.db_form_item(form_id, table_id, column_id, disp_typ_id, disp_opt_id, width, height, column_order, v_column_id)" & _
					"select 	" & this_form_id & ", table_id, column_id, disp_typ_id, disp_opt_id, width, height, column_order, v_column_id " & _
					"from biosardb.db_vw_formitems_all where formgroup_id = " & defaultFormGroupID & " and table_id = " & TableId & " and formtype_id = " & this_formtype_id
			'response.Write sql
			oconn.execute(sql)
			rs.movenext
		loop
	end if
	' END DCW: 2/17/2004
end function

function RemoveChildTable(ByVal TableId, byval formgroupid)
	oConn.execute SQLDeleteFormgroupTablesByPK(TableId, formgroupid)
end function

function RemoveChildFormItems(ByVal TableId, byval formgroupid)
	oConn.execute SQLDeleteFormItemsByFormGroupIdAndTableID(TableId, formgroupid)
end function

function HTMLChildTables(ByVal GroupId, ByVal TableId, BYval indentlevel, ByRef oDictSelected)
	dim sret
	dim i
	Dim oRS
	Dim sCheck
	' Response.Write SQLCHildTablesByBaseTableId(TableId)
	' Response.end
	Set oRS = oConn.Execute (SQLChildTablesByBaseTableId(TableId))
	If HasRecords(oRS) Then
		Do until oRs.eof
			sCheck = ""
			if oDictSelected.Exists(cstr(oRS("TABLE_ID"))) Then
				sCheck = " checked"
			end if
			' output tables of this level
			sRet = sRet & "<br>"
			for i = 0 to indentlevel * 4
				sRet = sRet & "&nbsp;"
			next
			sRet = sRet & "<input type=checkbox name=selected_tables value=" & ors("table_id") & scheck & ">"
			if sCheck <> "" Then
				' output link
				sret = sret & "<a class=""MenuLink"" href=user_columns.asp?dbname=biosar_browser&formgroup=base_form_group&display_type=simple&formgroup_id=" & lgroupid & "&table_id=" & ors("table_id") & ">"
			end if
			sRet = sRet & oRS("DISPLAY_NAME")
			if sCheck <> "" then
				sRet = sRet & "</a>"
			end if
			sRet = sRet & "&nbsp;(" & oRS("DESCRIPTION") & ")"
			' now output any child tables via recursion
			sRet = sRet & HTMLChildTables(lGroupId, oRS("TABLE_ID"), indentlevel + 1, oDictSelected)
			ors.movenext
		loop
	End If
	HTMLChildTables = sRet
end function

function FillTableOrder(ByVal GroupId, ByVal BaseTableId, byVal currLevel)
	Dim oRS
	dim oRSCheck
	dim oRSUpdate
	
	Set oRS = oConn.Execute (SQLChildTablesByBaseTableId(BaseTableId))
	if HasRecords(oRS) Then
		do until oRS.eof
			Set oRScheck = Server.CreateObject("ADODB.Recordset")
			oRSCheck.Open SQLFormgroupTableByPK(oRS("TABLE_ID"), GroupId), oConn, 1, 3
			if HasRecords(oRSCHeck) Then
				oRSCheck("TABLE_REL_ORDER") = currLevel			
				oRSCheck.Update
				oRSCheck.Close	
			end if
			'LJB: 6/18/2002 This recursiveness was causing problems since it was looking for nested tables and picking
			'up things that shouldn't be in the current formgroup based on the baseid'
			If SINGLE_CHILD_LEVEL = true then
				'FillTableOrder GRoupId, oRS("TABLE_ID"), currLevel + 1
			end if
			oRS.MoveNext
		loop
	end if
end function

function CurrentUserIsFormOwner(ByVal FormOwner)
	if ucase(FormOwner) = ucase(Session("username" & "biosar_browser")) then
		CurrentUserIsFormOwner = true
	else
		CurrentUserIsFormOwner = false
	end if
end function

	

%>

