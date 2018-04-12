<%'Copyright 1999-2003 CambridgeSoft Corporation. All rights reserved
dbkey = request("dbname")
CDD_DEBUG = false	
CDD_RS_DEBUG = false
CDD_RESULT_DEBUG = false
Dim formgroup
formgroup = request("formgroup")
formmode = request("formmode")
Session("bypass_ini" & dbkey & formgroup)= true

if Not Session("add_childtable_criteria" & dbkey & formgroup) <> "" then
	Session("add_childtable_criteria"& dbkey & formgroup) = request("add_childtable_criteria")
	if request("add_childtable_criteria")= "LIMIT" then
		Session("show_child_data_toggles"& dbkey & formgroup) =true
	end if
end if

if Not (UCase(formgroup)="BASE_FORM_GROUP" or UCase(formgroup) = "MANAGE_USERS_FORM_GROUP" or  UCase(formgroup)="MANAGE_ROLES_FORM_GROUP" or UCase(formgroup)="MANAGE_TABLES_FORM_GROUP" ) then
	if not isArray(Application("FORM_GROUP" & dbkey & formgroup)) OR CDD_DEBUG = true or CDD_RESULT_DEBUG = true then
		
		populateFormDefArrays dbkey, formgroup
	end if
end if
column_layout = request("COLUMN_LAYOUT")
if column_layout = "" then
	column_layout =  Session("DEFAULT_COLUMN_LAYOUT")
	if column_layout = "" then
		column_layout =  Application("DEFAULT_COLUMN_LAYOUT")
	end if
end if
Session("DEFAULT_COLUMN_LAYOUT") = column_layout

if Application("biosarxsloverride")="FALSE" then
	usecss = "biosar"
	usecssl = "biosar"
else
	if lcase(Application("hasCustomCSS" & formgroup)) = "true" then
		usecss = formgroup & "detail.css"
		usecssl = formgroup & "list.css"
		usecsslcd = formgroup & "listCD.css"
		usecssquery = formgroup & "query.css"
	else
		usecss = "customdetail.css"
		usecssl = "customlist.css"
		usecsslcd = "customlistCD.css"
		usecssquery = "customquery.css"
	end if
end if
%>



<%
if lcase(formmode) = "edit"  then
%>	
<script language="javascript">
		var useStyleSheet = '<%=usecss%>';
</script>	
<script LANGUAGE="javascript" src="/biosar_browser/source/Choosedetailcss.js"></script>
<%
elseif lcase(formmode) = "search" then
%>
<script language="javascript">
		var useStyleSheet = '<%=usecssquery%>';
</script>	
<script LANGUAGE="javascript" src="/biosar_browser/source/Choosecss.js"></script>
<%
else
if lcase(column_layout) = "fields_down" then
%>

<script language="javascript">
		var useStyleSheet = '<%=usecsslcd%>';
</script>

<script LANGUAGE="javascript" src="/biosar_browser/source/ChoosecssCD.js"></script>
<%
else
%>
<script language="javascript">
		var useStyleSheet = '<%=usecssl%>';
</script>
<script LANGUAGE="javascript" src="/biosar_browser/source/Choosecss.js"></script>
<%end if
end if
'this overrides core in that when kPluginValue = "FALSE" the chemdraw files are not loaded.  
if UCase(strTrueFalse(GetFormGroupVal(dbkey, formgroup, kPluginValue))) = "FALSE" and (UCase(formmode) = "LIST" or UCase(formmode) = "EDIT") then%>
	<%if detectNS4 = true then%>
	<script language="JavaScript" src= "/cfserverasp/source/chemdraw_ns4.js"></script>
	<%else%>
	<script language="JavaScript" src= "/cfserverasp/source/chemdraw.js"></script>
	<%end if %>
	<script language="JavaScript"> 
	if (window.doPluginDetect != null) {
		cd_includeWrapperFile("/cfserverasp/source/", doPluginDetect)
	}
	else {
		cd_includeWrapperFile("/cfserverasp/source/")
	}
	</script>
<%end if%>
