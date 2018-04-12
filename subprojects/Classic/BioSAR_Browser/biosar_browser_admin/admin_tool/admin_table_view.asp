<%@ Language=VBScript %>


<html>
<head>
<!-- #INCLUDE FILE="header.asp" -->
<!-- #INCLUDE FILE="xmltemplate_func.asp" -->
<!-- #INCLUDE FILE="display_sql_func.asp" -->

<script language="javascript">
	w_current_list = ""
	orig_current_list = ""
	w_avail_list = ""
	
	function test(id){
	alert(id)
	
	}

</script>

<%Response.ExpiresAbsolute = Now()

	
	'if the referer to this page is admin_table_list then null out all the table column cookies and the security cookies
	nullOutTableCookies()
	Session("AllowOrganizeProjectsSubmenu") = true


	Dim bShowAdmin
	bShowAdmin=true
	
	Dim bDebugGrantSqlTime
	bDebugGrantSqlTime = false
	
	session("admin_table_view_cancel_url") = Request.ServerVariables("query_string") & "&cancel=true"
	formgroup = "base_form_group"
	dbkey = "biosar_browser"
	
	Dim oRSTable
	Dim oRSCol
	Dim lTableId
	Dim bCSCartAddNote
	Dim bNIAddNote
	lTableId = Request("table_id")
	sOwner = request("user_name")
	Set oConn = SysConnection
	
	' perform requested action
	
	Dim lchildId
	Dim lBaseId
	if request("cancel") = "" then
		Select Case Request("action")
		
			Case "refresh_table"
				call RefreshSchemas()
			Case "edit_lookup"
				lBaseId = Request("base_col_id")
				nolookup_id_col = request("no_lookup_id_col")
				lookup_join = request("lookup_join_type")
				lookup_sort = request("lookup_sort_direct")
				lookup_id_col = Request("lookup_id_col")
				lookup_id_val=Request("lookup_id_val")
				
				Call EditLookup(lBaseId,nolookup_id_col,lookup_join,lookup_sort,lookup_id_col,lookup_id_val)
				
				
			Case "edit_rels"
				lChildId = Request("child_col_id")
			
				dim lParentColumnId
				lParentColumnId = request("parent_col_id")
				nParentColumnId = request("no_parent_col_id")
				join_type=request("join_type")
				
				call doEditRels(lChildId,lParentColumnId,nParentColumnId,join_type)
				
			Case "edit_content_type"
				lContentTypeId = Request("content_type_id")
				lBaseId = Request("base_col_id")
				lTableId = request("table_id")
				
				call doEditContentType(lContentTypeId,lBaseId,lTableId)
			
			Case "edit_index_type"
				lIndexTypeId = Request("index_type_id")
				lBaseId = Request("base_col_id")
				lTableId = request("table_id")
				
				call doEditIndexType(lIndexTypeId,lBaseId,lTableId)
		End Select
	end if ' not cancelled
	Set oRSTable = oConn.Execute(SQLExposedTableById(lTableId))
	Dim lBaseColId, bHasPK
	lBaseColId = oRSTable("BASE_COLUMN_ID")
	If cnvlong(lBaseColId, "DB_TO_VB") = NULL_AS_LONG then
		bHasPK = false
	else
		bHasPK = true
	End if
	
	Set oRSCol = oConn.Execute(SQLColumnsByTableId(lTableId))

	Dim oRSParentTables
	Set oRSparentTables = Server.CreateObject("ADODB.Recordset")
	oRSParentTables.Open SQLParentTablesByChildTableId(lTableId), oconn, 1, 3
	
	'get all projects that this table is assoicated with:
	AddTabletoProjectHelp = "&qshelp=Click the project category in which you want this table to appear."
	MoveTabletoProjectHelp = "&qshelp=Click the project category to which you want to move this table."
	manageProjectCategoriesHelp = "&qshelp=Select a project  and choose an action from the menu that appears. Click refresh to update the tree."

	sql = "select node_name,node_id,parent_id, tree_item.id,item_id from biosardb.tree_node, biosardb.tree_item where  tree_type_id = 1 and tree_item.node_id=tree_node.id and tree_item.item_id=" & lTableId

	Set oRSProjects = Server.CreateObject("ADODB.Recordset")
	oRSProjects.Open sql, oconn, 1, 3
	if not (oRSProjects.BOF and oRSProjects.EOF) then
		Do while not oRSProjects.EOF
			node_id = oRSProjects("NODE_ID")
			item_id = oRSProjects("item_id")
			id = oRSProjects("id")
			node_name = oRSProjects("Node_name")
			parent_id = oRSProjects("parent_id")
			pathtoNode = getNodePath(node_id,oConn)
			node_name_withSpan ="<span title="& quotedstring(pathtoNode) & ">" & node_name & "</span>"
			remove_text = "<a class=""MenuLink"" href=""#"" onclick=""OpenDialog3('/biosar_browser/source/treeview.asp?formgroup=base_form_group&qsrelay=formgroup&dbname=biosar_browser&TreeTypeID=1&TreeMode=remove_item&JsCallBack=EchoItemRemovedWithRefreshTable&ItemID=" & item_id & "&NodeID=" & node_id & "&ClearNodes=1&ItemTypeID=1','MyAddDialog',3)"">delete</a>"
			move_text = "<a class=""MenuLink"" href=""#"" onclick=""OpenDialog3('/biosar_browser/source/treeview.asp?formgroup=base_form_group" & MoveTabletoProjectHelp & "&qsrelay=formgroup&dbname=biosar_browser&TreeID=8&TreeTypeID=1&TreeMode=move_item&ShowItems=0&JsCallBack=EchoItemMovedWithRefreshTable&ItemID=" & id & "&NodeID=" & node_id & "&ClearNodes=1&ItemTypeID=1','MyAddDialog',3)"">move</a>"
			if project_categories <> "" then
				project_categories = project_categories  & "<tr><td width = ""200"">" & node_name_withSpan & "</td><td width = ""80"">" & move_text & "&nbsp;&nbsp;&nbsp;" & remove_text & "</td></tr>"
			else
				project_categories ="<table border=0 width = ""300"">" & "<tr><td width = ""200"">" & node_name_withSpan & "</td><td width = ""80"">" & move_text & "&nbsp;&nbsp;&nbsp;" & remove_text  & "</td></tr>"
			end if
		oRSProjects.MoveNext
		loop
		project_categories = project_categories & "</td></tr></table>"
	end if
	
	'table security
	if Application("TABLE_SECURITY") =1  then'add ability to apply grants for this table
		if inStr(Request.ServerVariables("HTTP_REFERER"),"admin_table_list.asp")>0  then
		
				'Get list of biosar roles that have the ability to add tables to a formgroup
				sCreateFGBioSARRoles = GetCreateFGBioSARRoles(dbkey, formgroup)
				sFGBioSARRoles = GetFGBioSARRoles(dbkey, formgroup)
				
				if sFGBioSARRoles <> "" then
					allRoles = sCreateFGBioSARRoles & "," & sFGBioSARRoles
				else
					allRoles = sCreateFGBioSARRoles
				end if
				
				FGRoles_Sql="Select role_name from cs_security.security_roles, cs_security.biosar_browser_privileges " & _
				" where cs_security.biosar_browser_privileges.role_internal_id = cs_security.security_roles.role_id" & _
				" and cs_security.biosar_browser_privileges.is_formgroup_role = 1" 
			
				
			
				' DIRECT GRANTS FORMGROUP_ROLES ONLY: Select ALL  ROLES (including  Formgroup Roles) with Direct Grant for SELECT privileges on this table. 
				sValidFGRolesWithDirectGrant	 = "select dba_tab_privs.grantee as ROLE_NAME  " &_
							" from dba_tab_privs " &_
							" where " &_
							" dba_tab_privs.privilege='SELECT' " &_
							" and dba_tab_privs.table_name ='" & oRSTable.Fields("TABLE_SHORT_NAME")  & "'" &_
							" and dba_tab_privs.grantor= '" & request("user_name")& "'" &_
							" INTERSECT " & FGRoles_Sql
							
				' DIRECT GRANTS BIOSAR ONLY: Select ALL  ROLES (including  Formgroup Roles) with Direct Grant for SELECT privileges on this table. 
				sValidRolesWithDirectGrant	 = "select dba_tab_privs.grantee as ROLE_NAME  " &_
							" from dba_tab_privs " &_
							" where " &_
							" dba_tab_privs.privilege='SELECT' " &_
							" and dba_tab_privs.table_name ='" & oRSTable.Fields("TABLE_SHORT_NAME")  & "'" &_
							" and dba_tab_privs.grantor= '" & request("user_name")& "'" &_
							" and dba_tab_privs.grantee IN (" & sCreateFGBioSARRoles & ")"
				
									
				'INDIRECT GRANTS: Select ALL Roles (including  Formgroup Roles) with INDIRECT Grant for SELECT privileges on this table. 
				sValidRolesWithIndirectGrant = "select   grantee as ROLE_NAME " &_
						" from dba_role_privs, dba_roles " &_
						" where dba_roles.role = dba_role_privs.grantee " &_
						" start with granted_role in (select grantee from dba_tab_privs where dba_tab_privs.privilege='SELECT'" &_
					    " and dba_tab_privs.table_name ='" & oRSTable.Fields("TABLE_SHORT_NAME")  & "'" &_
					    " and dba_tab_privs.grantor= '" & request("user_name")& "'" & ")" &_
						" connect by granted_role = PRIOR grantee "

				
				'Select VALID USERS:  Criteria: Get all users that have biosar roles (not a formgroup role)  	  
				sValidUsers = "select dba_role_privs.grantee as USER_ID " &_
							   " from dba_role_privs, cs_security.people  " &_
							   " where " &_
							   " dba_role_privs.grantee = cs_security.people.user_id  " &_
							   " and dba_role_privs.granted_role IN (" & sCreateFGBioSARRoles & ")" 
							   
			
								
				
				' Select ALL VALID BIOSAR USERS WITH SELECT privileges on this table.
				sValidUsersWithDirectGrant = "select distinct dba_tab_privs.grantee as USER_ID " &_
								" from dba_tab_privs,dba_role_privs,cs_security.people " &_
								" where " &_
								" dba_role_privs.grantee=dba_tab_privs.grantee " &_
								" and dba_role_privs.grantee = cs_security.people.user_id  " &_
								" and dba_tab_privs.table_name ='" & oRSTable.Fields("TABLE_SHORT_NAME")  & "'" &_
								" and dba_tab_privs.grantor= '" & request("user_name")& "'"  &_
								" INTERSECT " & sValidUsers
							   
				'Select all VALID BIOSAR USERS with INDIRECT GRANT on this table		   
				sValidUsersWithInDirectGrant = "select   grantee as USER_ID" &_
							" from dba_role_privs, cs_security.people " &_
							" where cs_security.people.user_id = dba_role_privs.grantee " &_
							" start with granted_role in (select grantee from dba_tab_privs where dba_tab_privs.privilege='SELECT'" &_
						    " and dba_tab_privs.table_name ='" & oRSTable.Fields("TABLE_SHORT_NAME")  & "'" &_
						    " and dba_tab_privs.grantor= '" & request("user_name")& "'" & ")" &_
							" connect by granted_role = PRIOR grantee" &_
							" INTERSECT " & sValidUsers
						
				'Select VALID ROLES:  Get All Biosar Roles that have ability to add tables to formgroups	  
				sValidRoles ="select dba_tab_privs.grantee as ROLE_NAME " &_
						"  from dba_tab_privs where " &_
						"  grantee IN (" & sCreateFGBioSARRoles & ")" 
						
				'Select VALID FORMGROUP ROLES:  Get All Public FormGroup Roles
				sValidFGRoles = FGRoles_Sql
				
				
				'Get users that should be on the left by doing a Subtract of  VALID USERS WITH SELECT privileges from VALID USERS	   
				sUsersNOGrant = "(" & sValidUsers & " MINUS " & sValidUsersWithDirectGrant & ") MINUS " & sValidUsersWithInDirectGrant
				'Get roles that are VALID but do not have grant
				sRolesNOGrant = sValidRoles & " MINUS " & sValidRolesWithDirectGrant & " MINUS " &  sValidRolesWithIndirectGrant
				
				'Get only Formgroup roles that have no gratn
				sFGRolesNOGrant =  sValidFGRoles & " MINUS " & sValidFGRolesWithDirectGrant
				
			
			

				
				
				if bDebugGrantSqlTime = true then
					dim myTime
					myTime = Timer()
				end if
				on error resume next
				Set oRSAllUsers = oConn.execute(sUsersNOGrant)
				if err.number <> 0 then
				
					if checkTableError("ORA-01437")<> "" then
						Response.Write "<font color=""red"">" & checkTableError("ORA-01437") & "</font>"
					end if
				end if
					if bDebugGrantSqlTime = true then
						Response.Write "<hr>"
						Response.Write "AVAILABLE USERS NO GRANT: Get users that should be on the left by doing a Subtract of  VALID USERS WITH SELECT privileges from VALID USERS" &"<br><br><br>"
						Response.Write "oRSAllUsers (time):  " & Timer()- myTime & "<br>"
						Response.Write "sUsersNOGrant (sql):  " &  sUsersNOGrant & "<br><br>"
						myTime = Timer()
						Response.Write "<br>***********************************<br>"
						Response.write TabSecurityList(oRSAllUsers, "user_id", "", "USER: ")
						Response.write "<br>***********************************<br>"
					else
						w_avail_list = TabSecurityList(oRSAllUsers, "user_id", "", "USER: ")
					end if
					
				Set oRSAllRoles = oConn.execute(sRolesNOGrant)
			
					if bDebugGrantSqlTime = true then
						Response.Write "<hr>"
						Response.Write "AVAILABLE ROLES NO GRANT: Get roles that are VALID but do not have grant" &"<br><br><br>"
						Response.Write "oRSAllRoles (time):  " & Timer()- myTime & "<br>"
						Response.Write "sRolesNOGrant (sql):  " &  sRolesNOGrant & "<br><br>"
						myTime = Timer()
						Response.Write "<br>***********************************<br>"
						Response.write TabSecurityList(oRSAllRoles, "role_name", "", "ROLE: ")
						w_avail_list = TabSecurityList(oRSAllRoles, "role_name", w_avail_list, "ROLE: ")
						Response.write "<br>***********************************<br>"
					else
						w_avail_list = TabSecurityList(oRSAllRoles, "role_name", w_avail_list, "ROLE: ")
					end if
					
				Set oRSFGRoles = oConn.execute(sFGRolesNOGrant)
				
					
					if bDebugGrantSqlTime = true then
						Response.Write "<hr>"
						Response.Write "AVAILABLE FG ROLES NO GRANT: Get only Formgroup roles that have no grant" &"<br><br><br>"
						Response.Write "oRSFGRoles (time):  " & Timer()- myTime & "<br>"
						Response.Write "sFGRolesNOGrant (sql):  " &  sFGRolesNOGrant & "<br><br>"
						myTime = Timer()
						Response.Write "<br>***********************************<br>"
						Response.write TabSecurityList_AppendOwner(oRSFGRoles, "role_name", "",oConn, "ROLE: ")
						w_avail_list = TabSecurityList_AppendOwner(oRSFGRoles, "role_name", w_avail_list,oConn, "ROLE: ")
						Response.write "<br>***********************************<br>"
					else
						w_avail_list = TabSecurityList_AppendOwner(oRSFGRoles, "role_name", w_avail_list,oConn, "ROLE: ")
					end if
					
				Set oRSTableUsers = oConn.Execute(sValidUsersWithDirectGrant)
				
					if bDebugGrantSqlTime = true then
						Response.Write "<hr>"
						Response.Write "CURRENT USERS DIRECT GRANT: Select ALL VALID BIOSAR USERS WITH SELECT privileges on this table." &  "<br><br><br>" 
						Response.Write "oRSTableUsers (time):  " & Timer()- myTime & "<br>"
						Response.Write "sValidUsersWithDirectGrant (sql):  " &  sValidUsersWithDirectGrant & "<br><br>"
						myTime = Timer()
						Response.Write "<br>***********************************<br>"
						Response.write TabSecurityList(oRSTableUsers, "user_id", "", "USER: ")
						w_current_list = TabSecurityList(oRSTableUsers, "user_id", "", "USER: ")
						Response.write "<br>***********************************<br>"
					else
						w_current_list = TabSecurityList(oRSTableUsers, "user_id", "", "USER: ")
					end if
					
				Set oRSTableRoles = oConn.Execute(sValidRolesWithDirectGrant)
				
					if bDebugGrantSqlTime = true then
						Response.Write "<hr>"
						Response.Write "CURRENT ROLE DIRECT GRANT BIOSAR ONLY: Select ALL  ROLES (including  Formgroup Roles) with Direct Grant for SELECT privileges on this table. " &  "<br><br><br>" 
						Response.Write "oRSTableRoles (time):  " & Timer()- myTime & "<br>"
						Response.Write "sValidRolesWithDirectGrant (sql):  " &  sValidRolesWithDirectGrant & "<br><br>"
						myTime = Timer()
						Response.Write "<br>***********************************<br>"
						Response.write TabSecurityList(oRSTableRoles, "role_name", "", "ROLE: ")
						w_current_list = TabSecurityList(oRSTableRoles, "role_name", w_current_list, "ROLE: ")
						Response.write "<br>***********************************<br>"
					else
						w_current_list = TabSecurityList(oRSTableRoles, "role_name", w_current_list, "ROLE: ")
					end if

				Set oRSTableRolesFG = oConn.Execute(sValidFGRolesWithDirectGrant)
				
					if bDebugGrantSqlTime = true then
						Response.Write "<hr>"
						Response.Write "CURRENT FG_ROLES DIRECT GRANT: Select  Formgroup Roles with Direct Grant for SELECT privileges on this table." &  "<br><br><br>" 
						Response.Write "oRSTableRolesFG (time):  " & Timer()- myTime & "<br>"
						Response.Write "sValidFGRolesWithDirectGrant (sql):  " &  sValidFGRolesWithDirectGrant & "<br><br>"
						myTime = Timer()
						Response.Write "<br>***********************************<br>"
					
						Response.write TabSecurityList_AppendOwner(oRSTableRolesFG, "role_name", "", oConn, "ROLE: ")
						w_current_list = TabSecurityList_AppendOwner(oRSTableRolesFG, "role_name", w_current_list, oConn, "ROLE: ")
						Response.write "<br>***********************************<br>"
					else
						w_current_list = TabSecurityList_AppendOwner(oRSTableRolesFG, "role_name", w_current_list, oConn, "ROLE: ")
					end if
							
				Set oRSTableNestedRoles = oConn.Execute(sValidRolesWithIndirectGrant)
				
					if bDebugGrantSqlTime = true then
						Response.Write "<hr>"
						Response.Write "CURRENT ROLES INDIRECT GRANT: Select ALL Roles (including  Formgroup Roles) with INDIRECT Grant for SELECT privileges on this table. " &  "<br><br><br>" 
						Response.Write "oRSTableNestedRoles (time):  " & Timer()- myTime & "<br>"
						Response.Write "sValidRolesWithIndirectGrant (sql):  " &  sValidRolesWithIndirectGrant & "<br><br>"
						myTime = Timer()
						Response.Write "<br>***********************************<br>"
						Response.write  TabSecurityList(oRSTableNestedRoles, "role_name", "", "ROLE NESTED: ")
						w_current_list = TabSecurityList(oRSTableNestedRoles, "role_name", w_current_list, "ROLE NESTED: ")
						Response.write "<br>***********************************<br>"
					else
						w_current_list = TabSecurityList(oRSTableNestedRoles, "role_name", w_current_list, "ROLE NESTED: ")
					end if	
					
				Set oRSUserNestedRoles = oConn.Execute(sValidUsersWithIndirectGrant)
				
					if bDebugGrantSqlTime = true then
						Response.Write "<hr>"
						Response.Write "CURRENT  USERS INDIRECT GRANT:  Select all VALID BIOSAR USERS with INDIRECT GRANT on this table	" &  "<br><br><br>" 
						Response.Write "oRSUserNestedRoles (time):  " & Timer()- myTime & "<br>"
						Response.Write "sValidUsersWithIndirectGrant (sql):  " &  sValidUsersWithInDirectGrant & "<br><br>"
						myTime = Timer()
						Response.Write "<br>***********************************<br>"
						Response.write TabSecurityList(oRSUserNestedRoles, "user_id", "", "USER NESTED: ")
						w_current_list = TabSecurityList(oRSUserNestedRoles, "user_id", w_current_list, "USER NESTED: ")
						Response.write "<br>***********************************<br>"
					else
						w_current_list = TabSecurityList(oRSUserNestedRoles, "user_id", w_current_list, "USER NESTED: ")
					end if
					
					
					%>
				
					<script language="javascript">
						w_current_list = "<%=w_current_list%>"
						orig_current_list = w_current_list
						w_avail_list = "<%=w_avail_list%>"
					</script>
				<%
				
		else
			w_current_list = Request("current_roles_hidden")
			w_avail_list = Request("roles_hidden")
			%>
				
					<script language="javascript">
						w_current_list = "<%=w_current_list%>"
						orig_current_list = w_current_list
						w_avail_list = "<%=w_avail_list%>"
					</script>
				<%
		
		end if
	end if
%>
</head>

<body onload="fill_lists()">
<form name="cancel_form" method="post" action="admin_table_list.asp?action=output_schema_tables&amp;user_name=<%=request("user_name")%>&amp;formgroup=base_form_group&amp;dbname=biosar_browser&amp;cancel=true">
			<input type="hidden" name="cancel_form">
			</form>
<form name="admin_refresh_form" method="post" action="admin_table_view.asp?formgroup=base_form_group&amp;dbname=biosar_browser&amp;action=refresh_table&amp;user_name=<%=request("user_name")%>&amp;table_id=<%=lTableId%>">
	  <!--'action="admin_table_view.asp?formgroup=base_form_group&dbname=biosar_browser&action=refresh_table&user_name=<%=request("user_name")%>&table_id=<%=lTableId%>&roles_hidden=<%=w_avail_list%>&current_roles_hidden=<%=w_current_list%>" > -->
	<input type="hidden" name="refresh_form">
</form>

<form name="admin_refresh_index_form" method="post" action="admin_table_view.asp?formgroup=base_form_group&amp;dbname=biosar_browser&amp;action=refresh_index&amp;user_name=<%=request("user_name")%>&amp;table_id=<%=lTableId%>">
	<input type="hidden" name="admin_refresh_index_form" ID="Hidden4">
</form>
<form name="admin_form" method="post" action="admin_table_list.asp?formgroup=base_form_group&amp;dbname=biosar_browser&amp;action=edit_table&amp;user_name=<%=request("user_name")%>&amp;table_id=<%=lTableId%>">
<%'POSTS for TABLE CHANGES%>


	<%	dim primarykey
		dim isVisible
		dim columnOrder
		dim columnName
		dim columnDescription
		dim displayName
		dim description
		primarykey= ""
		isVisible=""
		columnOrder=""
		columnName=""
		columnDescription=""
		displayName=""
		description=""
		
		TableChange = Request.Cookies("TableChanges")
		if  TableChange <> "" then
		'this means that a change occured to one of the columns so get the individual columns and write out hidden field for later posting
		
		TableChange_array = split(TableChange, "::CS::", -1)
		displayName = TableChange_array(0)
		description = TableChange_array(1)%>
			<input type="hidden" name="table_display_name" value="<%=displayName%>">
			<input type="hidden" name="table_description" value="<%=description%>">
		<%else%>
			<input type="hidden" name="table_display_name" value="<%=oRSTable.fields("DISPLAY_NAME")%>">
			<input type="hidden" name="table_description" value="<%=oRSTable.fields("DESCRIPTION")%>">
		<%end if
	
	
	'write out all current changes from cookies
	Colids= request.Cookies("ColumnChanges")
	if  Colids <> "" then

		'this means that a change occured to one of the columns so get the individual columns and write out hidden field for later posting
		
		Colid_array = split(Colids, ",", -1)
		for i = 0 to UBound (Colid_array)
			lColID = Colid_array(i)
			changeString =  request.Cookies("ColumnChanges" & lColID)
			changeStringtemp_array = split(ChangeString, "::CS::", -1)
			
			lcolID = changeStringtemp_array(0) 'this item is not needed in this context, but it is necessary elsewhere
			primaryKey = changeStringtemp_array(1)
			isVisible = changeStringtemp_array(2)
			
			columnOrder = changeStringtemp_array(3)
			columnName = changeStringtemp_array(4)
			columnDescription = changeStringtemp_array(5)
			%>		
			<input type="hidden" name="primary_key<%=lColId%>" value="<%=primaryKey%>">
			<input type="hidden" name="column_is_visible<%=lColId%>" value="<%=isVisible%>">	 
			<input type="hidden" name="column_order<%=lColId%>" value="<%=columnOrder%>">
			<input type="hidden" name="column_display_name<%=lColId%>" value="<%=columnName%>">
			<input type="hidden" name="column_description<%=lColId%>" value="<%=columnDescription%>">
		<%next
		'now go through and output all the others from the RS, skipping the ones that were set via cookies
		Do while not oRSCol.EOF
	
		lColId = oRSCol("COLUMN_ID")
		 if Not isDup(Colids, lColID) then
			
			if not isnull(lBaseColId) then
				if cstr(lColId) = cstr(lBaseColId) Then
					primaryKey =  "Y" 
				else
					primaryKey = "N" 
				end if
			end if
			
			isVisible = orscol.fields("IS_VISIBLE")
			columnOrder = orscol.fields("DEFAULT_COLUMN_ORDER")
			columnName = oRSCol.fields("DISPLAY_NAME")
			columnDescription = orscol.fields("DESCRIPTION")
		%>
			<input type="hidden" name="primary_key<%=lColId%>" value="<%=lColId%><%=primaryKey%>">
			<!--<input type="hidden" name="column_is_visible<%=lColId%>" value="<%=lColId%><%=isVisible%>">	 -->
			<input type="hidden" name="column_is_visible<%=lColId%>" value="<%=isVisible%>">	 
			<input type="hidden" name="column_order<%=lColId%>" value="<%=columnOrder%>">
			<input type="hidden" name="column_display_name<%=lColId%>" value="<%=columnName%>">
			<input type="hidden" name="column_description<%=lColId%>" value="<%=columnDescription%>">
		<%end if
		oRScol.movenext
		loop
		
	else
	
	'since there is no cookie, just add all the fields from the RS as hidden fields
		Do while not oRSCol.EOF
		lColId = oRSCol("COLUMN_ID")
		if not isnull(lBaseColId) then
			if cstr(lColId) = cstr(lBaseColId) Then
				primaryKey =  "Y" 
			else
				primaryKey = "N" 
			end if
		end if
		
		primaryKey = Boolean2Check(bbasecol)
		isVisible = orscol.fields("IS_VISIBLE")
		columnOrder = orscol.fields("DEFAULT_COLUMN_ORDER")
		columnName = oRSCol.fields("DISPLAY_NAME")
		columnDescription = orscol.fields("DESCRIPTION")%>
		
		<input type="hidden" name="primary_key<%=lColId%>" value="<%=lColId%><%=primaryKey%>">
		<input type="hidden" name="column_is_visible<%=lColId%>" value="<%=lColId%><%=isVisible%>">	 
		<input type="hidden" name="column_order<%=lColId%>" value="<%=columnOrder%>">
		<input type="hidden" name="column_display_name<%=lColId%>" value="<%=columnName%>">
		<input type="hidden" name="column_description<%=lColId%>" value="<%=columnDescription%>">
		
		<%oRScol.movenext
		loop
	end if%>
				
<%'POSTS for SECURITY CHANGES
%>

		<input type="hidden" name="current_roles_hidden" value="<%=w_current_list%>">
	
		<input type="hidden" name="roles_hidden" value="<%=w_avail_list%>">
	
	<input type="hidden" name="select_table_name" value="<%=oRSTable.Fields("TABLE_SHORT_NAME")%>">
	<input type="hidden" name="select_schema_name" value="<%=oRSTable.Fields("OWNER")%>">
</form>
<table border="0">

<td class="form_purpose_header">Table:</td><td class="form_purpose"><%=oRSTable.Fields("TABLE_NAME")%></td></table><br>
	<table border="0" width="200">
		<tr><td><hr></td></tr></table>
<%Session("sTab") = "0"
		if Session("sTab") = "" then Session("sTab") = "0"
		If sTab = "" Then sTab = Session("sTab") 
		Session("sTab") = request("TB")
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
		'TabView.QueryString = "output_type=tab&dbname=" & dbkey & "&formgroup=base_form_group&formmode=admin" & "&formgroup_id=" & lgroupid & "&table_id=" & ltableid
		TabView.LicenseKey = "7993-3E51-698B"
		
		
		'define all the tabs
		Set Tabx = TabView.Tabs.Add(0,"Tables","","","Alter table display and linking properties")
		Tabx.Image = ""
		Tabx.SelectedImage = ""
		Tabx.ForceDHTML =True
		Tabx.DHTML = "onclick=tabPostTable('0','"& lTableid & "') onmouseover=javascript:this.style.cursor='hand' onmouseout=javascript:this.style.cursor='default'"

		if Application("TABLE_SECURITY") = 1 then
			Set Tabx = TabView.Tabs.Add(1,"Security","","","Add/remove security")
			Tabx.Image = ""
			Tabx.SelectedImage = ""		
			Tabx.ForceDHTML =True
			Tabx.DHTML = "onclick=tabPostTable('1','"& lTableid & "') onmouseover=javascript:this.style.cursor='hand' onmouseout=javascript:this.style.cursor='default'"

		end if
		
		Set Tabx = TabView.Tabs.Add(2,"Organization","","","Add/remove table from category")
		Tabx.Image = ""
		Tabx.SelectedImage = ""
		Tabx.ForceDHTML =True
		Tabx.DHTML = "onclick=tabPostTable('2','"& lTableid & "') onmouseover=javascript:this.style.cursor='hand' onmouseout=javascript:this.style.cursor='default'"

		output = ""
		
		'output the tabs
		Response.Write TabView.ViewSource

		Set TabView = Nothing
		tab_name = Session("sTab") 
		
		'figure out which tab we are at and output the tab
		Select Case UCase(tab_name)
			Case "0"
				Response.Write "<br>"
				Response.Write "<table><tr><td></td></tr></table>"
				Response.Write "<table border = 0 width = 200><tr><td><br><hr></td></tr></table>"
				
				GetTablesTabInfo()
			case "1"
				if bShowAdmin = true then
					Response.Write "<br>"
					Response.Write "<table><tr><td>To add select privileges for this table to biosar roles or users that have the add_user_formgroup privilege select from the list on the left below. <br>If the list of permitted users and roles contains an item with the specifier USER NESTED, or ROLE NESTED, this means that the privileges are applied indirectly through a formgroup role or a non-biosar role. Grants to non-biosar roles cannot be revoked through this interface.</td></tr></table>"
					Response.Write "<table border = 0 width = 200><tr><td><br><hr></td></tr></table>"
						
					GetSecurityTabInfo()
				end if
			case "2"
				Response.Write "<br>"
				Response.Write "<table><tr><td>Change or add this table to a project categories.  Project categories are displayed when you build a forms and choose to add child tables. </td></tr></table>"
				Response.Write "<table border = 0 width = 200><tr><td><br><hr></td></tr></table>"

				GetOrganizationTabInfo()
			case else
				Response.Write "<br>"
				Response.Write "<table><tr><td></td></tr></table>"
				Response.Write "<table border = 0 width = 200><tr><td><br><hr></td></tr></table>"
				
				GetTablesTabInfo
		End Select
		
		
Sub GetTablesTabInfo()
			if not bHasPK then
			%>
				<table width="600" border="0"><tr><td class="warning_text">This table or view does not have a primary key defined. You must define a primary key before this table or view will be usable as a base table in forms.</td></tr></table>
			<%
			end if
			%>
			

			<form name="table_form">

			<table border="1" cellspacing="0" cellpadding="5">
			<tr><td class="table_cell_header" align="right">Name</td><td class="table_cell"><%=oRSTable.Fields("TABLE_SHORT_NAME")%></td></tr>
			<%TableChange = Request.Cookies("TableChanges")%>
			<%if detectIE5 then%>
				<%if  TableChange <> "" then
					'this means that a change occured to one of the columns so get the individual columns and write out hidden field for later posting
		
					TableChange_array = split(TableChange, "::CS::", -1)
					displayName = TableChange_array(0)
					description = TableChange_array(1)%>

					<tr><td class="table_cell_header" align="right">Display Name</td><td class="table_cell"><input type="textbox" name="table_display_name" size="50" maxlegth="50" value="<%=displayName%>" onBlur="tableChange()"></td></tr>
					<tr><td class="table_cell_header" align="right">Description</td><td class="table_cell"><input type="textbox" name="table_description" size="50" maxlegth="50" value="<%=description%>" onBlur="tableChange()"></td></tr>
			
				<%else%>
					<tr><td class="table_cell_header" align="right">Display Name</td><td class="table_cell"><input type="textbox" name="table_display_name" size="50" maxlegth="50" value="<%=oRSTable.fields("DISPLAY_NAME")%>" onBlur="tableChange()"></td></tr>
					<tr><td class="table_cell_header" align="right">Description</td><td class="table_cell"><input type="textbox" name="table_description" size="50" maxlegth="50" value="<%=oRSTable.fields("DESCRIPTION")%>" onBlur="tableChange()"></td></tr>
				<%end if%>
			<%else%>
				<%if  TableChange <> "" then
					'this means that a change occured to one of the columns so get the individual columns and write out hidden field for later posting
		
					TableChange_array = split(TableChange, "::CS::", -1)
					displayName = TableChange_array(0)
					description = TableChange_array(1)%>

					<tr><td class="table_cell_header" align="right">Display Name</td><td class="table_cell"><input type="textbox" name="table_display_name" size="50" maxlegth="50" value="<%=displayName%>" onChange="tableChange()"></td></tr>
					<tr><td class="table_cell_header" align="right">Description</td><td class="table_cell"><input type="textbox" name="table_description" size="50" maxlegth="50" value="<%=description%>" onChange="tableChange()"></td></tr>
			
				<%else%>
					<tr><td class="table_cell_header" align="right">Display Name</td><td class="table_cell"><input type="textbox" name="table_display_name" size="50" maxlegth="50" value="<%=oRSTable.fields("DISPLAY_NAME")%>" onChange="tableChange()"></td></tr>
					<tr><td class="table_cell_header" align="right">Description</td><td class="table_cell"><input type="textbox" name="table_description" size="50" maxlegth="50" value="<%=oRSTable.fields("DESCRIPTION")%>" onChange="tableChange()"></td></tr>
				<%end if%>
			<%end if%>
			
			
			<tr><td class="table_cell_header" align="right">Is View?</td><td class="table_cell"><%=oRSTable.Fields("IS_VIEW")%></td></tr>
			</table>



			</table><br>
			<table border="0" width="200">
					<tr><td><hr></td></tr></table>
			<table border="1" cellspacing="0" cellpadding="5" class="table_list">
			<tr>
			<td class="table_cell_header">
				Primary Key
			</td>
			<td class="table_cell_header">
				Visible?
			</td>
			<td class="table_cell_header">
				Default Column Order
			</td>
			<td class="table_cell_header">
				Name
			</td>

			<td class="table_cell_header">
				Display Name
			</td>
			<td class="table_cell_header">
				Description
			</td>
			<td class="table_cell_header">
				Lookup Table
			</td>
			<td class="table_cell_header">
				Parent Column
			</td>
			<td class="table_cell_header">
				Data type
			</td>
			<td class="table_cell_header">
				Index Type
			</td>

			<td class="table_cell_header">
				Content Type
			</td>
			</tr>
			<%
			dim lColId
			dim bBaseCol
			bBaseCol = False
			oRScol.requery
			'oRScol.movefirst 'need to do this since the admin_form was populated with the same RS
			Do while not oRSCol.EOF
				lColId = oRSCol("COLUMN_ID")
				if not isnull(lBaseColId) then
					if cstr(lColId) = cstr(lBaseColId) Then
						bBaseCol = true
					else
						bbasecol = false
					end if
				end if
			%>
				<tr>
				<% scale = oRSCol("SCALE")
				'check to see if there is a cookie for this column_id. If there is use that to output the values - otherwise use the RS.
				
				if request.Cookies("ColumnChanges") <> "" and request.Cookies("ColumnChanges" & lColID)  <> "" then
				
					changeString =  request.Cookies("ColumnChanges" & lColID)
					changeStringtemp_array = split(ChangeString, "::CS::", -1)
					colID = changeStringtemp_array(0) 'this is not needed in this context
					primaryKey = changeStringtemp_array(1)
					if inStr(primaryKey, "Y")>0 then
						bPKChecked = " checked"
					else
						bPKChecked = ""
					end if
					isVisible = changeStringtemp_array(2)
					if inStr(isVisible, "Y")>0 then
						bVisChecked = " checked"
					else
						bVisChecked = ""
					end if
					columnOrder = changeStringtemp_array(3)
					columnName = changeStringtemp_array(4)
					columnDescription = changeStringtemp_array(5)%>
					<% if detectIE5() then%>
							<td class="table_cell" align="center"><input type="radio" name="primary_key" value="<%=lColId%>" <%=bPKChecked%> onBlur="columnChange(<%=lColId%>)"></td>
							<td class="table_cell" align="center"><input type="checkbox" name="column_is_visible<%=lColId%>" <%=bVisChecked%> value="<%=lColId%><%=isVisible%>" onBlur="columnChange(<%=lColId%>)"></td>	 
							<td class="table_cell"><input name="column_order<%=lColId%>" type="text" maxlength="10" size="5" value="<%=columnOrder%>" onBlur="columnChange(<%=lColId%>)"></td>
							<td class="table_cell"><%=oRSCol.Fields("COLUMN_NAME")%></td>
							<td class="table_cell"><input name="column_display_name<%=lColId%>" type="text" maxlength="30" size="25" value="<%=columnName%>" onBlur="columnChange(<%=lColId%>)"></td>
							<td class="table_cell"><input name="column_description<%=lColId%>" type="text" maxlength="100" size="25" value="<%=columnDescription%>" onBlur="columnChange(<%=lColId%>)"></td>
					<%else%>
							<td class="table_cell" align="center"><input type="radio" name="primary_key" value="<%=lColId%>" <%=bPKChecked%> onBlur="columnChange(<%=lColId%>)"></td>
							<td class="table_cell" align="center"><input type="checkbox" name="column_is_visible<%=lColId%>" <%=bVisChecked%> value="<%=lColId%><%=isVisible%>" onBlur="columnChange(<%=lColId%>)"></td>	 
							<td class="table_cell"><input name="column_order<%=lColId%>" type="text" maxlength="10" size="5" value="<%=columnOrder%>" onBlur="columnChange(<%=lColId%>)"></td>
							<td class="table_cell"><%=oRSCol.Fields("COLUMN_NAME")%></td>
							<td class="table_cell"><input name="column_display_name<%=lColId%>" type="text" maxlength="30" size="25" value="<%=columnName%>" onBlur="columnChange(<%=lColId%>)"></td>
							<td class="table_cell"><input name="column_description<%=lColId%>" type="text" maxlength="100" size="25" value="<%=columnDescription%>" onBlur="columnChange(<%=lColId%>)"></td>

					<%end if%>
				
				<%else
					
					if detectIE5() then
						if  (oRSCol("DATATYPE") = "NUMBER" AND (isNull(scale) or scale = "0")) OR (oRSCol("DATATYPE") = "VARCHAR2") then%>
							<td class="table_cell" align="center"><input type="radio" name="primary_key" value="<%=lColId%>" <%=Boolean2Check(bbasecol)%> onBlur="columnChange(<%=lColId%>)"></td>
						<%else%>
							<td class="table_cell" align="center">&nbsp;</td>
						<%end if%>
						<td class="table_cell" align="center"><input type="checkbox" name="column_is_visible<%=lColId%>" value="<%=lColId%><%=orscol.fields("IS_VISIBLE")%>" <%=Bool2Check(orscol.fields("IS_VISIBLE"))%> onBlur="columnChange(<%=lColId%>)"></td>	 
						<td class="table_cell"><input name="column_order<%=lColId%>" type="text" maxlength="10" size="5" value="<%=orscol.fields("DEFAULT_COLUMN_ORDER")%>" onBlur="columnChange(<%=lColId%>)"></td>
						<td class="table_cell"><%=oRSCol.Fields("COLUMN_NAME")%></td>
						<td class="table_cell"><input name="column_display_name<%=lColId%>" type="text" maxlength="25" size="25" value="<%=left(oRSCol.fields("DISPLAY_NAME").value,30)%>" onBlur="columnChange(<%=lColId%>)"></td>
						<td class="table_cell"><input name="column_description<%=lColId%>" type="text" maxlength="100" size="25" value="<%=orscol.fields("DESCRIPTION")%>" onBlur="columnChange(<%=lColId%>)"></td>
		
					<%else
				
						if  (oRSCol("DATATYPE") = "NUMBER" AND (isNull(scale) or scale = "0"))OR (oRSCol("DATATYPE") = "VARCHAR2") then%>
							<td class="table_cell" align="center"><input type="radio" name="primary_key" value="<%=lColId%>" <%=Boolean2Check(bbasecol)%> onChange="columnChange(<%=lColId%>)"></td>
						<%else%>
							<td class="table_cell" align="center">&nbsp;</td>
						<%end if%>
						<td class="table_cell" align="center"><input type="checkbox" name="column_is_visible<%=lColId%>" value="<%=lColId%><%=orscol.fields("IS_VISIBLE")%>" <%=Bool2Check(orscol.fields("IS_VISIBLE"))%> onChange="columnChange(<%=lColId%>)"></td>	 
						<td class="table_cell"><input name="column_order<%=lColId%>" type="text" maxlength="10" size="5" value="<%=orscol.fields("DEFAULT_COLUMN_ORDER")%>" onChange="columnChange(<%=lColId%>)"></td>
						<td class="table_cell"><%=oRSCol.Fields("COLUMN_NAME")%></td>
						<td class="table_cell"><input name="column_display_name<%=lColId%>" type="text" maxlength="30" size="25" value="<%=left(oRSCol.fields("DISPLAY_NAME").value,30)%>" onChange="columnChange(<%=lColId%>)"></td>
						<td class="table_cell"><input name="column_description<%=lColId%>" type="text" maxlength="100" size="25" value="<%=orscol.fields("DESCRIPTION")%>" onChange="columnChange(<%=lColId%>)"></td>
						<%end if%>
				<%end if%>
				
				<%if  UCase(oRSCol("DATATYPE")) = "NUMBER" or UCase(oRSCol("DATATYPE")) = "VARCHAR2" or UCase(oRSCol("DATATYPE")) = "CHAR" then %>
					<td class="table_cell">
					<%=LookupTable(lColId, sOwner)%>
					<td class="table_cell">
					<%=ParentTable(lColId, sOwner)%>
					</td>
				<%else%>
					<td class="table_cell">
					&nbsp;
					<td class="table_cell">
					&nbsp;
					</td>
				<%end if%>
				<td class="table_cell"><%=orscol.fields("datatype")%></td>
				<td><%=IndexType(oRScol,lColID, sOwner,oRSCol("DATATYPE"))%>&nbsp;</td>
				<td><%=ContentType(lColID, oRScol("INDEX_TYPE_ID"), sOwner)%>&nbsp;</td>
				</tr>
			<%
				oRScol.movenext
			Loop
			%>
			</table>
			<table><tr><td colspan="2"><br>The following characters are not allowed in table or column display names and will be automatically removed: <%=Application("illegalFormCharctersHTML")%></td></tr></table>

			
			<% if bCSCartAddNote =true then%>
				<table><tr><td>**To view the field indexed by the Cartridge as a chemical structure:
				<tr><td>a. Set the content type for the Cartidge indexed field.</td></tr>
				<tr><td>b. Set a primary key for this table if not already set.</td></tr></table>
			<%end if%>

			<% if bNIAddNote =true then%>
				<table><tr><td>***To view the LOB field using one of the supported mime types:
				<tr><td>a. Set the content type for the LOB field.</td></tr>
				<tr><td>b. Set a primary key for this table if not already set.</td></tr></table>
			<%end if%>

			<% if bNIAddNote =true or bCSCartAddNote = true then%>
				<table ID="Table1"><tr><td>To display a  structure or lob field from a different table</td></tr>
				<tr><td>a. In the other table, click Lookup Table for a unique numeric field.</td></tr>
				<tr><td>b. Select this the primary key for this table as the linking column.</td></tr>
				<tr><td>c. Select the Cartridge indexed field as the  display column.</td></tr></table>
			<%end if%>
			

</td></tr></table></form>
<%End sub


Sub GetOrganizationTabInfo()
	%><br>
	<table width="400"><tr><td class="form_purpose_header" nowrap>Associated Project Categories&nbsp;&nbsp;&nbsp;<a class="MenuLink" href="#" onclick="OpenDialog3('/biosar_browser/source/treeview.asp?dbname=biosar_browser<%=AddTabletoProjectHelp%>&amp;TreeID=10&amp;TreeTypeID=1&amp;ItemTypeId=1&amp;ClearNodes=1&amp;itemid=<%=ltableid%>&amp;ShowItems=0&amp;TreeMode=add_item&amp;jscallback=EchoItemAddedWithRefreshTable','MySelectItemDiag',3)">[Add to Project Category]</a></font></b></td></tr></td></tr></table>
	<table width="400" border="0" class="table_list"><tr><td class="table_cell"><%=project_categories%></td></tr></table>
	<%
End Sub

Sub GetSecurityTabInfo%>

			
		<script language="javascript">
			w_current_list = "<%=w_current_list%>"
			orig_current_list = w_current_list
			w_avail_list = "<%=w_avail_list%>"
		</script>
	
	<form name="security_form"><table border="1" cellspacing="0" cellpadding="5" ID="Table3">
			<% 'this fields are only necessary to makes the functions used for user_tables.asp to work here%>
			
			<input type="hidden" name="new_name" value ID="Hidden1">
			<input type="hidden" name="new_desc" value ID="Hidden2">
			<input type="hidden" name="form_restore" value>
			<input type="hidden" name="can_make_public" value="1">
			<input type="hidden" name="is_public" value>
			

			<% 'end%>
				<hr><b>
				<font size="+1">Table Security</font>
				</b>
				
				
			
				
				<tr><td align="center" colspan="2">
				<table width="544" ID="Table4">
				<tr><td width="550">
				<table ID="Table5">
				<tr>
				<td width="436" nowrap colspan="2">
				<table border="0" width="436" ID="Table6">
				  
				  <tr>
				    <td width="220">BioSAR Users and Roles
				    </td>
				    <td width="20%">
				    </td>
				    <td width="220">BioSAR Users And Roles <br>with Select privileges on <%=oRSTable.Fields("TABLE_SHORT_NAME")%>
				    </td>
				  </tr>
				  <tr>
				    <td width="220">
				    <script language="javascript">
						buildLeftMultiSelectBox('roles', 6)
				    </script>
				   
				    </td>
				    <td width="20%">
				      <table border="0" width="100%" ID="Table7">
				        <tr>
				          <td width="100%">
				<a href="javascript:addCurrentList()"><img SRC="/<%=Application("AppKey")%>/graphics/add_role_btn.gif" BORDER="0"></a>
				          </td>
				        </tr>
				        <tr>
				          <td width="100%">
				<a href="javascript:removeCurrentList()"><img SRC="/<%=Application("appkey")%>/graphics/remove_role_btn.gif" BORDER="0"></a>
				          </td>
				        </tr>
				      </table>
				    </td>
				    <td width="220">
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
				</td></tr></table>
				</table>
			
		</form>
		
		<%
		'edd ability to apply grants
	
end sub%>

</form>

</body>
</html>
<script language="javascript">
					fill_lists()
				</script>	

<!-- #INCLUDE FILE="footer.asp" -->
<%


' page functions

function LookupTable(ByVal id, byval sOwner)
	' lookup table name
	dim sout

	sout = "<a class=MenuLink href=admin_edit_lookup.asp?formgroup=base_form_group&dbname=biosar_browser&table_id=" & lTableid & "&user_name=" & sOwner & "&column_id=" & id & ">"
	if not isnull(oRScol("LOOKUP_TABLE_ID")) then
		dim rs 
		set rs = oconn.execute(SQLExposedTableById(orsCol("LOOKUP_TABLE_ID")))
		sout = sout & rs("TABLE_NAME")
	else
		sout = sout & "Choose..."
	end if
	sout = sout & "</a>"
	LookupTable = sout
end function

function ContentType(ByVal column_id, ByVal IndextypeID, byval sOwner)
	' lookup table name
	dim sout
	dim table_name
	dim column_name
	
	
	If oRScol("DATATYPE") = "CLOB" or oRScol("DATATYPE")  = "BLOB" then
		current_content_type_id=oRScol("CONTENT_TYPE_ID")
		sout = "<a  class=MenuLink  href=admin_edit_content_type.asp?formgroup=base_form_group&dbname=biosar_browser&user_name=" & sOwner & "&table_id=" & lTableId & "&column_id=" & column_id & "&column_name=" & oRScol("COLUMN_NAME") & "&current_content_type_id=" & current_content_type_id &"&current_index_type_id=" & IndextypeID & ">"
		if not isnull(oRScol("CONTENT_TYPE_ID")) then
			dim rs 
			set rs = oconn.execute(SQLgetContentTypeName(oRScol("CONTENT_TYPE_ID")))
			sout = sout & rs("MIME_TYPE")
		else
			sout = sout & "Choose..."
		end if
		sout = sout & "</a>"
	end if
	ContentType = sout
end function

function IndexType(byRef oRSCol, ByVal column_id, byval sOwner, byVal datatype)
	' lookup table name
	dim sout
	If oRScol("DATATYPE") = "CLOB" or oRScol("DATATYPE") = "BLOB" then
		current_index_id = oRScol("INDEX_TYPE_ID")
		
		sout = "<a  class=MenuLink  href=admin_edit_index_type.asp?formgroup=base_form_group&dbname=biosar_browser&&user_name=" & sOwner & "&table_id=" & lTableId & "&column_name=" & oRScol("COLUMN_NAME") & "&column_id=" & column_id &  "&datatype=" & datatype & "&current_index_id=" & current_index_id &">"
		if not isnull(current_index_id) then
			dim rs 
			set rs = oconn.execute(SQLgetIndexTypeName(oRScol("INDEX_TYPE_ID")))
			if rs("INDEX_TYPE")="CS_CARTRIDGE" then
				sout = sout & rs("INDEX_TYPE") & "**<br>"
				bCSCartAddNote = true
				
			else
				bNIAddNote = true
				sout = sout & rs("INDEX_TYPE")& "***<br>"
			end if
		else
			sout = sout & "Choose..."
		end if
		sout = sout & "</a>"
	end if
	IndexType = sout
end function



function ParentTable(ByVal Id, byval sOwner)
	' Parent table name
	dim sout
	sout = "<a   class=MenuLink  href=admin_edit_rel.asp?formgroup=base_form_group&dbname=biosar_browser&table_id=" & lTableid & "&user_name=" & sOwner & "&column_id=" & Id & ">"
	if not (oRSParentTables.EOF and oRSParentTables.BOF) then
		oRSParentTables.movefirst
		oRSParentTables.Find("child_column_id = " & id)
		if not oRSParentTables.EOF then
			sout = sout & orsparenttables("parent_table_name") & _
			"." & orsparenttables("parent_column_name")
		else
			sout = sout & "Choose..."
		end if
	else
		sout = sout & "Choose..."
	end if
	sout = sout & "</a>"
	ParentTable = sout
end function


%>



<%if Application("TABLE_SECURITY") = 1 then%>
<script language="javascript">
	fill_lists()
</script><%end if%>
