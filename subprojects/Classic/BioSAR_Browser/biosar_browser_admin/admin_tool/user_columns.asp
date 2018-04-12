<%@ Language=VBScript %>


<HTML>
<HEAD>
<!-- #INCLUDE FILE="header.asp" -->

<SCRIPT LANGUAGE="JavaScript">

function checkAll(field, thetype){
	var checkVar = ""
	if (thetype.toLowerCase() == "query"){
		checkVar =1}
	if (thetype.toLowerCase() == "list"){
		checkVar = 2}
	if (thetype.toLowerCase() == "detail"){
		checkVar =3}
	if (field){
		for (i = 0; i < field.length; i++){
	
			if (field[i].id==checkVar){
				field[i].checked = true ;
			}
		}
	}	
}

function uncheckAll(field, thetype){
	var checkVar = ""
	if (thetype.toLowerCase() == "query"){
		checkVar = 1}
	if (thetype.toLowerCase() == "list"){
		checkVar = 2}
	if (thetype.toLowerCase() == "detail"){
		checkVar = 3}
	if (field){
		for (i = 0; i < field.length; i++){
			if (field[i].id==checkVar){
				field[i].checked = false ;
			}
		}
	}
}

function checkAllAcross(field, thetype){
	if (field){
		for (i = 0; i < field.length; i++){
			var theItem = field[i].value
			if (theItem.indexOf(thetype)!=-1){
				field[i].checked = true ;
				
			}
		}
	}	
	
}

function uncheckAllAcross(field, thetype){
	
	if (field){
		for (i = 0; i < field.length; i++){
			var theItem = field[i].value
			if (theItem.indexOf(thetype)!=-1){
				field[i].checked = false ;
				
			}
		}
	}		
}
</script>

</HEAD>

<%	

Dim oRS
dim oRSFI
dim oRSForms
Dim oRSDisplayTypes
dim lTableId
dim lFormgroupId
dim lFormId
dim sTableName
dim bStructureTable
Dim lColid

Dim lQueryId
Dim lListid
Dim lDetailId

lFormgroupId = Request("formgroup_id")
lTableId = Request("table_id")

Set oConn = SysConnection
Set oRS = oConn.Execute(SQLExposedTableById(lTableId))
sTableName = ors.fields("Display_name")
Set oRSForms = oConn.Execute(SQLFormsByFormgroupId(lFormgroupId))
do until oRSForms.eof
	select case cstr(orsForms("formtype_id"))
		case "1"
			lQueryId = orsforms("form_Id")
		case "2"
			lListId = orsforms("form_Id")
		case "3"
			lDetailId = orsforms("form_Id")
	end select
	orsforms.movenext
loop
%>
<body>

<table border = 0><tr></tr><td class="form_purpose_header"  valign = "bottom">
	Fields for <%=server.HTMLEncode(sTableName)%>  
</td></tr>
</table><br>

	<table border = 0 width = 200>
	<tr><td><hr></td></tr></table>

<%

if request("display_type") = "simple" then
%>
	Select fields to display in query, list, and detail view.<p>
<%
Else
%>
	Select display options for query, list, and detail view.
<%
end if

query_uncheckedImage="<a class=MenuLink href=""javascript:void(0)"" onClick=""uncheckAll(document.admin_form.formitems_disp,'query')""><IMG SRC=""/biosar_browser/graphics/cb_not_checked.gif"" border = 0></a>"
query_checkedImage="<a class=MenuLink href=""javascript:void(0)""  onClick=""checkAll(document.admin_form.formitems_disp,'query')""><IMG SRC=""/biosar_browser/graphics/cb_checked.gif"" border = 0></a>"
query_uncheckspantag= "<span title = ""uncheck all fields in this column"">" & query_uncheckedImage & "</span>"
query_checkspantag="<span title = ""check all fields in this column"">"& query_checkedImage & "</span>"

list_uncheckedImage="<a class=MenuLink href=""javascript:void(0)""  onClick=""uncheckAll(document.admin_form.formitems_disp,'list')""><IMG SRC=""/biosar_browser/graphics/cb_not_checked.gif"" border = 0></a>"
list_checkedImage="<a class=MenuLink href=""javascript:void(0)""  onClick=""checkAll(document.admin_form.formitems_disp,'list')""><IMG SRC=""/biosar_browser/graphics/cb_checked.gif"" border = 0></a>"
list_uncheckspantag= "<span title = ""uncheck all fields in this column"">" & list_uncheckedImage& "</span>"
list_checkspantag="<span title = ""check all fields in this column"">"& list_checkedImage & "</span>"

detail_uncheckedImage="<a class=MenuLink href=""javascript:void(0)""  onClick=""uncheckAll(document.admin_form.formitems_disp,'detail')""><IMG SRC=""/biosar_browser/graphics/cb_not_checked.gif"" border = 0></a>"
detail_checkedImage="<a class=MenuLink href=""javascript:void(0)""  onClick=""checkAll(document.admin_form.formitems_disp,'detail')""><IMG SRC=""/biosar_browser/graphics/cb_checked.gif"" border = 0></a>"
detail_uncheckspantag= "<span title = ""uncheck all fields in this column"">" & detail_uncheckedImage & "</span>"
detail_checkspantag="<span title = ""check all fields in this column"">"& detail_checkedImage & "</span>"

%>



<form action="user_tables.asp?dbname=biosar_browser&formgroup=base_form_group&action=edit_formitems&formgroup_id=<%=lFormgroupId%>&table_id=<%=lTableId%>&display_type=<%=request("display_type")%>" id=form1 name=admin_form method=post>

<table border=1 cellspacing=0 cellpadding=5 class=table_list>
<tr>
<td class=table_cell_header>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</td>
<td class=table_cell_header>Field</td>
<td class=table_cell_header>Description</td>
<td class=table_cell_header>Query&nbsp;<%=query_uncheckspantag%><%=query_checkspantag%></td>
<td class=table_cell_header>List&nbsp;<%=list_uncheckspantag%><%=list_checkspantag%></td>
<td class=table_cell_header>Detail&nbsp;<%=detail_uncheckspantag%><%=detail_checkspantag%></td>
</tr>
<%
Dim sCheck
dim lDtypId
Set oRS = oConn.Execute(SQLExposedColumnsByTableId(lTableId))

'look only for structures so they appear first in the list.
Do until ors.eof	
	lColId = oRs("COLUMN_ID")
	if isNull(oRS("DEFAULT_COLUMN_ORDER")) then 
		defColOrder = 100
	else
		defColOrder = CLng(oRS("DEFAULT_COLUMN_ORDER")) + 3
	end if
	lLookupColumnID = oRs("LOOKUP_COLUMN_DISPLAY")
	index_type =  oRs("INDEX_TYPE_ID")

	content_type =  oRs("CONTENT_TYPE_ID")
	
	lLookupColumnID = oRs("LOOKUP_COLUMN_DISPLAY")
	datatype=oRS("DATATYPE")
	if lLookupColumnID <> "" then
		datatype_to_check = getLookupDataType(oConn,lLookupColumnID)
		content_type_to_check = getLookupContentType(oConn,lLookupColumnID)
	else
		datatype_to_check = datatype
		content_type_to_check = content_type
	end if
	if isStructureColumn(lColID, oConn) OR isStructureColumn(lLookupColumnID, oConn) then
		For n = 1 to 3
			Select Case n
				Case 1
					
					set oRSDisplayTypes = oConn.Execute(SQLDisplayTypeById(DISP_TYP_STRUCTURE))
					vlColId = VCOL_STRUCTURE & lColID
					lsName = "Structure"
					lsDesc = "Graphical Chemical Structure"
					defColOrder = "1"
				Case 2
					Set oRSDisplayTypes = oConn.Execute(SQLDisplayTypeById(DISP_TYP_FORMULA))
					vlColid = VCOL_FORMULA & lColID
					lsName = "Formula"
					lsDesc = "Chemical Formula"
					defColOrder = "2"
				Case 3
					Set oRSDisplayTypes = oConn.Execute(SQLDisplayTypeById(DISP_TYP_MOLWEIGHT))
					vlColId = VCOL_MOLWEIGHT & lColID
					lsName = "MolWeight"
					lsDesc = "Molecular Weight"
					defColOrder = "3"
			End Select
			
			struc_item_identifier= vlColId & "_" 
			struc_field_uncheckedImage="<a class=MenuLink href=""javascript:void(0)""  onClick=""uncheckAllAcross(document.admin_form.formitems_disp,&quot;" & struc_item_identifier & "&quot;)""><IMG SRC=""/biosar_browser/graphics/cb_not_checked.gif"" border = 0></a>"
			struc_field_checkedImage="<a class=MenuLink href=""javascript:void(0)""  onClick=""checkAllAcross(document.admin_form.formitems_disp,&quot;" & struc_item_identifier & "&quot;)""><IMG SRC=""/biosar_browser/graphics/cb_checked.gif"" border = 0></a>"
			struc_field_uncheckspantag= "<span title = ""uncheck all fields in this column"">" & struc_field_uncheckedImage & "</span>"
			struc_field_checkspantag="<span title = ""check all fields in this column"">"& struc_field_checkedImage & "</span>"

	%>
			<tr>
			<td class=table_cell><%=struc_field_uncheckspantag%><%=struc_field_checkspantag%></td>
			<td class=table_cell><%=server.HTMLEncode(lsName)%></td>
			<td class=table_cell><%=server.HTMLEncode(lsDesc)%></td>
			
				<td align=left valign=top class=table_cell><%=HTMLOutputOptions(defColOrder, vlColId, lQueryId, 1, oRSDisplayTypes, true, false)%></td>
			<%if not (content_type_to_check = "0" or content_type_to_check = "")then%>
				<td align=left valign=top class=table_cell><%=HTMLOutputOptions(defColOrder,vlColId, lListId, 2, oRSDisplayTypes, true, false)%></td>
				<td align=left valign=top class=table_cell><%=HTMLOutputOptions(defColOrder,vlColId, lDetailId, 3, oRSDisplayTypes, true, false)%></td>
			<% else%>
				<td align=left valign=top class=table_cell><%="Structure has unknown content_type and cannot be searched or displayed"%></td>

			<%end if%>
			</tr>
	<% 
	next
	end if
	ors.movenext
loop
'loop through again to get the non structure fields
oRs.requery
oRS.MoveFirst

Do until ors.eof	
	lColId = oRs("COLUMN_ID")
	lLookupColumnID = oRs("LOOKUP_COLUMN_DISPLAY")
	lLookupColumnID = oRs("LOOKUP_COLUMN_DISPLAY")
	datatype=oRS("DATATYPE")
	if lLookupColumnID <> "" then
		datatype_to_check = getLookupDataType(oConn,lLookupColumnID)
		content_type_to_check = getLookupContentType(oConn,lLookupColumnID)
	else
		datatype_to_check = datatype
		content_type_to_check = content_type
	end if
	defColOrder = oRS("DEFAULT_COLUMN_ORDER")
	if NOT isStructureColumn(lColID, oConn) and Not isStructureColumn(lLookupColumnID, oConn) then
	

		if isImageColumn(lColID, oConn) OR isImageColumn(lLookupColumnID, oConn) then
		
			Dim sDispTypIds
			
			sDispTypIds = DisplaytypeIdsForColumn(oRS)
			
			Set oRsdisplayTypes = oConn.Execute(SQLDisplayTypeById(sDispTypIds))
			item_identifier= lColId & "_" 
			field_uncheckedImage="<a class=MenuLink href=""javascript:void(0)""  onClick=""uncheckAllAcross(document.admin_form.formitems_disp,&quot;" & item_identifier & "&quot;)""><IMG SRC=""/biosar_browser/graphics/cb_not_checked.gif"" border = 0></a>"
			field_checkedImage="<a class=MenuLink href=""javascript:void(0)""  onClick=""checkAllAcross(document.admin_form.formitems_disp,&quot;" & item_identifier & "&quot;)""><IMG SRC=""/biosar_browser/graphics/cb_checked.gif"" border = 0></a>"
			field_uncheckspantag= "<span title = ""uncheck all fields in this column"">" & field_uncheckedImage & "</span>"
			field_checkspantag="<span title = ""check all fields in this column"">"& field_checkedImage & "</span>"

		%>
			<tr>
			<td class=table_cell><%=field_uncheckspantag%><%=field_checkspantag%></td>
			<td class=table_cell><%=MyHTMLEncode(ors.fields("display_name").value)%></td>
			<td class=table_cell><%=MyHTMLEncode(ors.fields("description").value)%></td>
			<td align=left valign=top class=table_cell><%=HTMLOutputOptions(defColOrder,lColId, lQueryId, 1, oRSDisplayTypes, false, true)%></td>
			<td align=left valign=top class=table_cell><%=HTMLOutputOptions(defColOrder,lColId, lListId, 2, oRSDisplayTypes, false, false)%></td>
			<td align=left valign=top class=table_cell><%=HTMLOutputOptions(defColOrder,lColId, lDetailId, 3, oRSDisplayTypes, false, false)%></td>
		<%else

		
			sDispTypIds = DisplaytypeIdsForColumn(oRS)
			
			Set oRsdisplayTypes = oConn.Execute(SQLDisplayTypeById(sDispTypIds))
			item_identifier= lColId & "_" 
			field_uncheckedImage="<a class=MenuLink href=""javascript:void(0)""  onClick=""uncheckAllAcross(document.admin_form.formitems_disp,&quot;" & item_identifier & "&quot;)""><IMG SRC=""/biosar_browser/graphics/cb_not_checked.gif"" border = 0></a>"
			field_checkedImage="<a class=MenuLink href=""javascript:void(0)""  onClick=""checkAllAcross(document.admin_form.formitems_disp,&quot;" & item_identifier & "&quot;)""><IMG SRC=""/biosar_browser/graphics/cb_checked.gif"" border = 0></a>"
			field_uncheckspantag= "<span title = ""uncheck all fields in this column"">" & field_uncheckedImage & "</span>"
			field_checkspantag="<span title = ""check all fields in this column"">"& field_checkedImage & "</span>"

		%>
			<tr>
			<td class=table_cell><%=field_uncheckspantag%><%=field_checkspantag%></td>
			<td class=table_cell><%=MyHTMLEncode(ors.fields("display_name").value)%></td>
			<td class=table_cell><%=MyHTMLEncode(ors.fields("description").value)%></td>
			<td align=left valign=top class=table_cell><%=HTMLOutputOptions(defColOrder,lColId, lQueryId, 1, oRSDisplayTypes, false, false)%></td>
			<td align=left valign=top class=table_cell><%=HTMLOutputOptions(defColOrder,lColId, lListId, 2, oRSDisplayTypes, false, false)%></td>
			<td align=left valign=top class=table_cell><%=HTMLOutputOptions(defColOrder,lColId, lDetailId, 3, oRSDisplayTypes, false, false)%></td>
			</tr>
	<%	end if
	end if
	ors.movenext
loop

%>

	
</table>

</form>
<P>&nbsp;</P>

<P>&nbsp;</P>

<!-- #INCLUDE FILE="footer.asp" -->

<%
' page functions

function HTMLOutputOptions(ByVal DefColOrder, ByVal column_id, byval form_id, byval formtype_id, byRef oRSDisplayType, byVal isVirtual, byVal disable)
	' this sucker outputs the checkboxes in simple mode, and the 
	' mini-form of display options in complex mode.  It's gnarly because
	' a lot of the business logic of when options are available are not
	' amenable to living in database tables.
	Dim lDispTyp
	Dim sDispTyp
	Dim sRet
	Dim display_type_id
	Dim sShow
	dim sWidth, sHeight, sDispTypeId, sDispOptId
	' set up display table
	sRet = sRet & "<table border=0 cellspacing=0 cellpadding=3 width='100%'>"
	' see if a formitem exists
	Dim oRSFormitem
	on error resume next
	if isVirtual = true then
		Set oRSFormitem = oConn.Execute(SQLFormitemsByVirtualColumnIdAndFormId(column_id, form_id))
		if err.number <> 0 then
			Response.Write SQLFormitemsByVirtualColumnIdAndFormId(column_id, form_id)
		end if
	else
		Set oRSFormitem = oConn.Execute(SQLFormitemsByColumnIdAndFormId(column_id, form_id))
		if err.number <> 0 then
			Response.Write SQLFormitemsByColumnIdAndFormId(column_id, form_id)
		end if
	end if
	
	if orsformitem.bof and orsformitem.eof then	
		' if no formitem exists, use default values
		sDispTypeId = "-1"
		sDispOptId = "-1"
		sShow = ""
		swidth = oRSDisplayType("DEFAULT_WIDTH")
		sheight = oRSDisplayType("DEFAULT_HEIGHT")
		sFormatMask = ""
		'JHS 07/30/2007 trying new hyperlink functionality
		sLink = ""
		sLinkText = ""
	else
		' else get values from the formitem
		sDispTypeId = cnvlong(orsformitem("DISP_TYP_ID"), "DB_TO_VB")
		sDispOptId = cnvlong(orsFormitem("DISP_OPT_ID"), "DB_TO_VB")
		sShow = " checked"
		swidth = cnvstring(orsformitem("WIDTH"), "DB_TO_VB")
		sheight = cnvstring(orsformitem("HEIGHT"), "DB_TO_VB")
		sFormatMask = cnvstring(orsformitem("FORMAT_MASK"), "DB_TO_VB")
		sColumnDataType = orsformitem("DATATYPE")
		'JHS 07/30/2007 trying new hyperlink functionality
		sLink = cnvstring(orsformitem("LINK"), "DB_TO_VB")
		sLinkText = cnvstring(orsformitem("LINKTEXT"), "DB_TO_VB")		
	end if
	' if display type is simple, just output checkbox
	if request("display_type") = "simple" then
		if Not disable = true then
			sRet = sRet & "<tr><td align=center><input type=checkbox name=formitems_disp value=" & column_id & "_" & form_id &	sshow & " id=" & formtype_id & ">"
			sRet = sRet & "<input type=""hidden"" name=""DefColOrder_" & column_id & "_" & form_id & """ value = """ & defColOrder & """></td></tr>"
		else
			sRet = sRet & "<tr><td>&nbsp;</td></tr>"
		end if
	else
	
		'DGB Here I introduce a substantial change of behaviour
		'Up to know, all fields appear in the Field Options dialog
		'regardless of whether they are shown in the form.  There are
		'several bugs related to field options not being shown or saved
		'when the fields are not set to be shown in the form.
		'The next four lines basically make it such that it is not 
		'possible to modify field options unless the field has
		'been previously set to be shown from the Edit Fields dialog
		'This means that field options can be edited or that fields
		'can be removed from the Field Options dialog, but they can
		'only be added from the Edit Fields dialog.
		'while this is a minor loss of functionality, it resolves
		'many otherwise confusing issues.
		if orsformitem.bof and orsformitem.eof then
			HTMLOutputOptions ="<center><font size=1 color=#666666>field not shown&nbsp;</font></center>"
			Exit function
		end if
		' display the whole enchilada
		if Not disable = true then
			sRet = sRet & "<tr><td align=right>Show:</td><td><input type=checkbox name=formitems_disp value=" & column_id & "_" & form_id & sshow & " id=" & formtype_id & ">"
			sRet = sRet & "<input type=""hidden"" name=""DefColOrder_" & column_id & "_" & form_id & """ value = """ & defColOrder & """></td></tr>"
			' first output display type, if this is a query
			If oRSDisplayType.bof and oRSDisplayType.eof then
				' no display types found, use default of text display
				if formtype_id = 1 then
					sRet = sret & "<input type=hidden name=display_type" & column_id & "_" & form_id & " value=1>TextBox</td></tr>"
				end if
				display_type_id = NULL_AS_LONG
			Else
				display_type_id = cnvlong(orsdisplaytype("DISP_TYP_ID"), "DB_TO_VB")
				' output select list of display types, if it's a query
				if formtype_id = 1 then
					sRet = sRet & "<tr><td align=right>As:</td><td>"
					' want to output a select box if more than one option, just plain text 
					' with a hidden input field otherwise
					' set sDispTypId if not already set, and we have a date field
					if sDispTypeId = "-1" and (display_type_id = 5 or display_type_id = 6) then
						' set to datepicker
						sDispTypeid = "6"
					end if
					dim n
					dim sSelectBegin, sSelectEnd, sTemp
					dim sFirstId, sFirstName
					sSelectBegin = "<select class=SelectBox name=display_type" & column_id & "_" & form_id & ">" 
					sSelectEnd = "</select>"
					n = 0
					do until oRSDisplayType.eof
						if n > 0 then
							if n = 1 then
								sTemp = "<option value=" & sFirstId
								if cstr(orsDisplayTYpe("disp_typ_id")) = cstr(sDispTypeId) Then
									sTemp = sTemp & " selected"
								end if
								sTemp = sTemp & ">" & sFirstName & "</option>"
							end if
							sTemp = sTemp & "<option value=" & oRSDisplayType("disp_typ_id")
							if cstr(orsDisplayType("disp_typ_id")) = cstr(sDispTypeId) THen
								sTemp = sTemp & " selected"
							end if
							stemp = stemp & ">" & oRSDisplayType("disp_typ_name") & "</option>"
						else
							sFirstId = oRSDisplayType("disp_typ_id")
							sFirstName = oRSDisplayType("disp_typ_name")
						End if
						oRSDisplayType.movenext
						n = n + 1
					loop
					oRSDisplayType.movefirst
					if n = 1 then
						sRet = sRet & "<input type=hidden name=display_type" & column_id & "_" & form_id & " value=" & sFirstId & ">" & sFirstName
					else
						sRet = sRet & sSelectBegin & sTemp & sSelectEnd
					end if
					sRet = Sret & "</td></tr>"
			else ' Add hyperlink display option for list and detail columns
				' Only for varchar2 columns
				'if Ucase(sColumnDataType) = "VARCHAR2" then
				'JHS 07/30/2007 trying new hyperlink functionality
				if Ucase(sColumnDataType) = "VARCHAR2" OR Ucase(sColumnDataType) = "NUMBER" then
					if sDispTypeId = 11 then
						s2 = " checked "
						s1 = " "
					else
						s2 = " "
						s1 = " checked "
					end if	
					sRet = sRet & "<tr><td align=right>As:</td><td><input" & s1 & "type=radio name=display_type" & column_id & "_" & form_id & " value=""1"">Text&nbsp;<input" & s2 & "type=radio name=display_type" & column_id & "_" & form_id & " value=""11"">Hyperlink <a href=""javascript:helpMsg('hyperlink')""><img border=""0"" src=""/biosar_browser/graphics/help.gif"" width=""15"" height=""15""></a></td></tr>"
				end if
							
				 
			end if
			end if
		end if
		' now output display options, if any exist
		if Not disable = true then
			Dim oRSOptions	
		
			sql = SQLOptionsByFormtypeAndDisplayType(formtype_id, display_type_id)
			Set oRSOptions = oConn.Execute(sql)
			' set default display option for structure - gif output
			if display_type_id = 7 and sDispOptID = "-1" then
				sDispOptId = 2
			end if
			if display_type_id=7 then 
				'do structure separately
				
				if (datatype_to_check="CLOB") and content_type_to_check = 5 then
					if not (oRSoptions.bof and orsoptions.eof) then
						sRet = sRet & "<tr><td align=right class=table_cell>Format:</td><td class=table_cell>"
						sRet = sret & "<select class=selectbox name=col_disp_opt" & column_id & "_" & form_id & ">"
						do until orsoptions.eof
							' add all options
							if display_type_id=7 and inStr(UCase(oRSOptions("DISPLAY_NAME")), "GIF")>0 then
								'only display gif option for base64_cdx for now
									if datatype="CLOB" and content_type_to_check = 5 then
											sRet = sRet & "<option value=" & oRSOptions("DISP_OPT_ID")
										if cstr(orsoptions("DISP_OPT_ID")) = cstr(sDispOptId) Then
											sRet = sRet & " selected"
										end if
										sRet = sRet & ">" & oRSOptions("DISPLAY_NAME") & "</option>"
									end if
							else
								sRet = sRet & "<option value=" & oRSOptions("DISP_OPT_ID")
								if cstr(orsoptions("DISP_OPT_ID")) = cstr(sDispOptId) Then
									sRet = sRet & " selected"
								end if
								sRet = sRet & ">" & oRSOptions("DISPLAY_NAME") & "</option>"
							end if
							orsoptions.movenext
						loop
						sRet = sRet & "</select></td></tr>"
					end if
			
				
				end if
			else
				if not (oRSoptions.bof and orsoptions.eof) then
					sRet = sRet & "<tr><td align=right class=table_cell>Format:</td><td class=table_cell>"
					sRet = sret & "<select class=selectbox name=col_disp_opt" & column_id & "_" & form_id & ">"
					do until orsoptions.eof
						' add all options
						
							sRet = sRet & "<option value=" & oRSOptions("DISP_OPT_ID")
							if cstr(orsoptions("DISP_OPT_ID")) = cstr(sDispOptId) Then
								sRet = sRet & " selected"
							end if
							sRet = sRet & ">" & oRSOptions("DISPLAY_NAME") & "</option>"
						
						orsoptions.movenext
					loop
					sRet = sRet & "</select></td></tr>"
				end if
			end if
		
			
			' now output width and height boxes. 
			' only display in the following cases:
			' - width and/or height are valid for display type
			' - always display for structure boxes
			if sWidth <> "-1"  or display_type_id = DISP_TYP_STRUCTURE  or display_type_id = 14 or display_type_id = 13 then 
				if formtype_id = FORM_QUERY and display_type_id = DISP_TYP_SELECT then
					' do nothing
				else
					'if sWidth= -1 then sWidth = ""
					sRet = sRet & "<tr><td align=right class=table_cell>Width:</td><td class=table_cell>"
					sret = sRet & "<input type=text size=3 name=width" & column_id & "_" & form_id & " value=" & swidth & ">"
					sRet = sret & "</td></tr>"
				end if
			end if
			if sHeight <> "-1"  or display_type_id = DISP_TYP_STRUCTURE or display_type_id = 14 or display_type_id = 13 then
				sRet = sRet & "<tr><td align=right class=table_cell>Height:</td><td class=table_cell>"
				sRet = sRet & "<input type=text size=3 name=height" & column_id & "_" & form_id & " value=" & sheight & ">"
				sret = sret & "</td></tr>"
			end if
			
			' Display the Format Mask text box
			
			if formtype_id = 1 then
				' do not display because this is query form
			else
				'if display_type_id <> 7 AND display_type_id <> 9 AND display_type_id <> 13 AND display_type_id <> 14 then
				' Display only for MW, Numerical and Date fields
				'jhs 10/30/2007 - fixes csbr-78839
				'if display_type_id = 10 OR Ucase(sColumnDataType) = "NUMBER" OR Ucase(sColumnDataType) = "DATE" then
				if display_type_id = 10 OR Ucase(sColumnDataType) = "NUMBER" OR Ucase(sColumnDataType) = "FLOAT" OR Ucase(sColumnDataType) = "DATE" then
					sRet = sRet & "<tr><td align=right class=table_cell><span title=""Enter a valid Oracle number or char format mask.  Refer to Oracle to_char() function documentation for syntax details."">Format Mask:</span></td><td class=table_cell>"
					sRet = sRet & "<input type=text size=10 name=formatMask" & column_id & "_" & form_id & " value=""" & sFormatMask & """>"
					sret = sret & "<a href=""javascript:helpMsg('format_mask')""><img border=""0"" src=""/biosar_browser/graphics/help.gif"" width=""15"" height=""15""></a></td></tr>"	
				end if
				
				'JHS 07/30/2007 trying new hyperlink functionality
				if Ucase(sColumnDataType) = "NUMBER" OR Ucase(sColumnDataType) = "VARCHAR2" then
					sRet = sRet & "<tr><td align=right class=table_cell><span title=""Enter a valid URL where the last character will be the field value."">URL:</span></td><td class=table_cell>"				
					sRet = sRet & "<input type=text size=10 name=LINK" & column_id & "_" & form_id & " value=""" & sLink & """>"	
					sret = sret & "</td></tr>"
					sRet = sRet & "<tr><td align=right class=table_cell><span title=""Enter a valid URL where the last character will be the field value."">Link Text:</span></td><td class=table_cell>"				
					sRet = sRet & "<input type=text size=10 name=LINKTEXT" & column_id & "_" & form_id & " value=""" & sLinkText & """>"	
					sret = sret & "</td></tr>"		
				end if
				
				
			end if

						
		end if
	end if
	sRet = sret & "</table>"
	HTMLOutputOptions = sRet
end function

%>



</body>
</html>