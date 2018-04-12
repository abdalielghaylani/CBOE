
<%@ LANGUAGE=VBScript %>
<%response.expires = 0%>
<%' Copyright 1999-2003 CambridgeSoft Corporation. All rights reserved
%>
<%
'DGB Redirection to Login screen is now managed by Session on Start
'if Application("LoginRequired" & dbkey) =1 then
'	if Not Session("UserValidated" & dbkey) = 1 then  response.redirect "/" & Application("Appkey") & "/logged_out.asp"
'end if
Dim CDD_DEBUG
CDD_DEBUG = false

%>
<!DOCTYPE HTML PUBLIC "-//IETF//DTD HTML//EN">
<html>
<HEAD>
<!-- #INCLUDE FILE="app_entry.asp" -->
<!-- #INCLUDE FILE="header.asp" -->
<script language="javascript">


</script>
</HEAD>

<body>
<input type="hidden" name="logoff_form">

<form name=schema_form action="admin_schema_choice.asp?formgroup=base_form_group&dbname=biosar_browser" method="post"><input type="hidden" name = "post_schama_choice">
<input type = "hidden" name="schema_form">

</form>

<form name=admin_form action=admin_table_list.asp?action=get_schema_list&formgroup=base_form_group&dbname=biosar_browser&user_name=<%request("user_name")%> method="post">
<input type="hidden" name = "post_add_schema">
</form>
<form name=admin_refresh_form action="admin_table_list.asp?formgroup=base_form_group&dbname=biosar_browser&user_name=<%request("user_name")%>&action=refresh_table" method="post">
<input type="hidden" name = "post_refresh">
</form>


<% 	Session("AllowOrganizeProjectsSubmenu") = true

	
	Dim oRSSchema
	Dim oRSTable
	Dim oRSView
	Dim sOwner
	Dim noSchemas
	noSchemas=true
	sOwner = request("user_name")
	Set oConn = SysConnection
	If Request("cancel") = "" then
		Select Case Request("action")
			case "expose_schemas"
				Server.ScriptTimeout = 300 ' CSBR-159251: Increased the time out to 5 mins
				Call doExposeSchemas()
				outputSchemasAndTables
				' CSBR-159251: Reload the parent page so the Wait animation gets cleared
				Response.write "<script type=""text/javascript""> window.parent.location.reload(false); </script>"
			Case "change_password"
				sDesc =Request("new_desc") 
				sDesc=removeIllegalChars(sDesc)
				sPassword = Request("new_password")
				
				' first change schema description
				sOwner = Request("owner")
				'encrypt password
				
				sPassword =  SAR_CryptVBS(sPassword, UCase(trim(sOwner)))
				' escape "'" just incase it is part of the encryption.
				sPassword = replace(sPassword, "'", "''")
				oConn.Execute SQLUpdateSchemaDisplayNameAndPassword(sOwner, sDesc, sPassword)
				outputSchemasAndTables
						
			Case "expose_tables"
		
				table_names=Request("table_names")
				view_names=Request("view_names")
				call doExposeTables()
				
				outputSchemasAndTables
			Case "edit_table"
				' apply changes to table
				lTableId = Request("table_id")
				table_display_name=Request("table_display_name")
				table_display_name=removeIllegalChars(table_display_name)
				
				call doEditTable(lTableId,table_display_name,"")
				
				Dim oRSModCol
				Set oRSModCol = Server.CreateObject("ADODB.Recordset")
				oRSModCol.Open SQLColumnsByTableId(lTableId), oConn, 1, 3
				Dim lColId
				
				on error resume next
				bErrorWritten = false
				
				Do while not oRSModCol.EOF
					lColId = orsmodcol("COLUMN_ID").value
					
					isVisible = Request("column_is_visible" & lColId)
					
					if inStr(isVisible,"Y")>0 then
						setIsVisible = "Y"
					else
						setIsVisible = "N"
					end if
					
					oRSModCol("IS_VISIBLE").value = setIsVisible
					
					
					isPrimaryKey = request("primary_key" & lColID)
					
					if inStr(isPrimaryKey, "Y")> 0 then
						primary_key = replace(isPrimaryKey, "Y", "")
						call doEditPrimaryKey(lTableId,primary_key)
					end if 
					
					if Not Request("column_order" & lColId) <> "" then
						colOrder = 0
					else
						colOrder = Request("column_order" & lColId)
						if not isNumeric(colOrder) then
							colOrder = 0
						else
							colOrder = CLng(colOrder)
						end if
					end if
					oRSModCol("DEFAULT_COLUMN_ORDER").value = colOrder
					column_display_name = removeIllegalChars(Request("column_display_name" & lColId))
					oRSModCol("DISPLAY_NAME").value = column_display_name
					oRSModCol("DESCRIPTION").value = REquest("column_description" & lColId)
				
					if CheckTableError("ORA-01458") <> "" then
						if bErrorWritten = false then
							Response.Write "<font color=red>Updating of the table (" & UCase(TABLE_DISPLAY_NAME) & ") was not sucessful.</br> The following error(s) occurred: <br>" 
							Response.Write CheckTableError("ORA-01458")
							Response.Write "Please contact your Oracle administrator to resolve this issue.</br></font>" 
							bErrorWritten = true
						end if
						exit do
					end if
					oRSModCol.Update
					oRSModCol.Movenext
				loop
			
				oRSModCol.close
				
				
				'redisplay table list
				outputSchemasAndTables
				
				'grant select privileges		
				select_table_name = request("select_table_name")
				select_schema_name = request("select_schema_name")
				on error resume next
		
				grantTableSelectPriv select_schema_name,select_table_name 
				
			Case "get_schema_list"
				
				outputSchemasAndTables
				
			
				
			case "output_schema_tables"
				outputSchemasAndTables
			
			Case "refresh_table"
				call RefreshSchemas()
				
				outputSchemasAndTables
		
	


		End Select
	else
		outputSchemasAndTables()
	end if ' not cancelled
	
	%>
<P>&nbsp;</P>
<form name=cancel_form action=admin_table_list.asp?action=get_schema_list&formgroup=base_form_group&dbname=biosar_browser&user_name=<%request("user_name")%> method="post" ID="Form2">
<input type = "hidden" name="cancel_form">
</form>

<%
		


Sub outputSchemasAndTables()
	
	theCurrentOwner = WriteSchemaSelector()
		

End Sub

function WriteSchemaSelector()
	if not isObject(oConn) or oConn.State = 0 then
		Set oConn = SysConnection
	end if
	currentUserName = request("user_name")
	on error resume next
	Set oRSSchema = oConn.Execute(SQLAllExposedSchemas)
	
	if err.number = 0 then
	response.write "<div id=""TableAndSchemas"" class=""SHOW""><table><tr><td width = ""400""0>Expand a schema and choose a task to perform:<br><hr></td></tr></table>"
		if  NOT (oRSSchema.EOF AND oRSSchema.BOF) then 
			defaultOwner = oRSSchema("OWNER")
			on error resume next
			i=0
			j=1000
			Do While NOT oRSSchema.EOF
				theOutputOwner=""
				theOutputOwner =  oRSSchema("OWNER")
				
			
				'end maingrouping
				if UCase(theOutputOwner) = UCase(currentUserName) then
					response.write "<div class=""grouping"" onClick=""ShowHideDiv(" &  i & ")""><img src=""down2.gif"" id=""img" & i & """ border=""0"">&nbsp;"
					response.write theOutputOwner
					response.write "</img></div>"
					response.Write "<div id=""div" & i & """ class=""SHOW"">"
				else
					'Main Grouping
					response.write "<div class=""grouping"" onClick=""ShowHideDiv(" &  i & ")""><img src=""right.gif"" id=""img" & i & """ border=""0"">&nbsp;"
					response.write theOutputOwner
					response.write "</img></div>"
					response.Write "<div id=""div" & i & """ class=""HIDE"">"
				end if
				response.Write "<table>"
				Response.Write "<tr><td><img src=""space.gif""></img><a class=""MenuLink"" href=""admin_table_choice.asp?formgroup=base_form_group&dbname=biosar_browser&user_id=" & theOutputOwner & "&user_name=" & theOutputOwner & """>Edit Exposed Tables</a>"
				Response.Write "<tr><td><img src=""space.gif""></img><a class=""MenuLink"" href=""admin_schema_password.asp?formgroup=base_form_group&dbname=biosar_browser&user_id=" & theOutputOwner & "&user_name=" & theOutputOwner & """>Edit Schema Description/Password</a>"
				Response.Write "<tr><td>"
				                                                                                                                                                        Response.Write "<div class=""subgrouping"" onClick=""ShowHideDiv(" & j &  ")""><img src=""space.gif""><img src=""down2.gif"" id=""img" & (i + 1000) & """></img>&nbsp;Edit Table Fields<br> </div>"
				Response.Write  "<div id=""div" & j & """ class=""SHOW"">"
				response.Write "<table>"
				WriteTableSelector(theOutputOwner)
				Response.Write "</tr></table>"
				Response.Write "</div>"
				response.Write "</td></tr></table></div>"
				oRSSchema.MoveNext
				i = i + 1
				j = j + 1
			Loop 
			
		else
			Response.Write "	No Schemas Exposed. Click Add/Remove Schema to Add Schemas"
			defaultOwner=""
			currentUserName = ""
		end if
	response.write "</div></td></tr></table>"
	end if
	if currentUserName <> "" then
		WriteSchemaSelector = currentUserName
	else
		WriteSchemaSelector = defaultOwner
	end if
End function	 

Sub WriteEditTableOptions()
	%>
	<a class="MenuLink" href="#" onClick="doGoToTable()">Edit Table Fields</a>
	<%		
End Sub

Sub WriteTableSelector(theOutputOwner)
	
	if  theOutputOwner <> "" then
	Set oRSTable = oConn.Execute("select * from BIOSARDB.db_table where is_view = 'N' and owner = '" & theOutputOwner & "' and is_exposed = 'Y' order by display_name asc")
		Set oRSView = oConn.Execute("select * from BIOSARDB.db_table where is_view = 'Y' and owner = '" & theOutputOwner & "' and is_exposed = 'Y' order by display_name asc")
		Set oDict = AuthorizedTables(theOwner)
		if err.number = 0 then
			if  NOT (oRSTable.EOF AND oRSTable.BOF) then 
				Do While NOT oRSTable.EOF
					if oDict.Exists(cstr(oRSTable("TABLE_NAME"))) Or IGNORE_ADMIN_PERMISSIONS Then
					
						if cnvlong(oRSTable("BASE_COLUMN_ID"), "DB_TO_VB") = NULL_AS_LONG then
							addText = " *NO PRIMARY KEY DEFINED"
						else
							addText = ""
						end if
					end if
				
					Response.Write "<tr><td><img src=""space.gif""></img><img src=""space.gif""></img><a class=""MenuLink"" href=""/biosar_browser/biosar_browser_admin/admin_tool/admin_table_view.asp?formgroup=base_form_group&user_name=" + theOutputOwner + "&dbname=biosar_browser&table_id=" & oRSTable.Fields("TABLE_ID") & """>" & server.HTMLEncode(oRSTable.Fields("DISPLAY_NAME")) & " (" & oRSTable.Fields("TABLE_SHORT_NAME") & ") " & addText & "</a></td></tr>"
					oRSTable.MoveNext
				Loop 
			end if
			if  NOT (oRSView.EOF AND oRSView.BOF) then 
				Do While NOT oRSView.EOF
					if oDict.Exists(cstr(oRSView("TABLE_NAME"))) Or IGNORE_ADMIN_PERMISSIONS Then
					
						if cnvlong(oRSView("BASE_COLUMN_ID"), "DB_TO_VB") = NULL_AS_LONG then
							addText = " *NO PRIMARY KEY DEFINED"
						else
							addText = ""
						end if
					end if
					Response.Write "<tr><td><img src=""space.gif""></img><img src=""space.gif""></img><a class=""MenuLink"" href=""/biosar_browser/biosar_browser_admin/admin_tool/admin_table_view.asp?formgroup=base_form_group&user_name=" + theOutputOwner + "&dbname=biosar_browser&table_id=" & oRSView.Fields("TABLE_ID") & """>" &  server.HTMLEncode(oRSView.Fields("DISPLAY_NAME")) & " (" & oRSView.Fields("TABLE_SHORT_NAME") & ") " & addText & "</a></td></tr>"
					oRSView.MoveNext
				Loop 
			end if
		end if
	

	end if
End sub	 




%>
  
<!-- #INCLUDE FILE="footer.asp" -->



</body>
</html>

