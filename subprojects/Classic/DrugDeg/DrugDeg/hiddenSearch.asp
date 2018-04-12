<%@ Language=VBScript %>
<%
PrimaryKey = request("PrimaryKey")
%>
<HTML>
<HEAD>
<META NAME="GENERATOR" Content="Microsoft Visual Studio 6.0">

</HEAD>
<BODY onload="getAction('search')">
<%'Response.Write "w" & session("wizard")
Session("addParent") = true
%>

<script>
function getAction(action){
	document.cookie = "PagingMovedrugdegbase_form_group" + "="  + escape("first_record") 
	
	document.cows_input_form.action = "/drugdeg/drugdeg/drugdeg_action.asp?formmode=search&formgroup=base_form_group&dataaction=search&dbname=DRUGDEG"
	document.cows_input_form.submit();
}
</script>

<form name="cows_input_form" method="post" Action="/drugdeg/DrugDeg/DrugDeg_action.asp?formmode=search&formgroup=base_form_group&dataaction=search&dbname=DRUGDEG" onsubmit="getAction('search');return false;" >
	
	<input Type="Image"  name="default_action_image" src="/cfserverasp/source/graphics/navbuttons/pixel.gif" border="0" WIDTH="1" HEIGHT="1">
	

<input type = "hidden" name="CurrentLocation" value ="/drugdeg/DRUGDEG/Parent_input_form.asp?formgroup=base_form_group&formmode=search&dbname=DRUGDEG&QSDiff=44242">
<input Type="hidden"  name="blank_cdx" value = "VmpDRDAxMDAEAwIBAAAAAAAAAAAAAAAAAAAAAAMADgAAAENoZW1EcmF3IDUuMAgA
CAAAAE1ULkNEWAQCEAAAAP7/AAD+/wAAAgAAAAIAAAMyAAgA////////AAAAAAAA
//8AAAAA/////wAAAAD//wAAAAD/////AAAAAP////8AAP//AAkAAAEJCAAAAIwA
AACEAAIJCAAAAD0CAAAmAwIIEAAAAAAAAAAAAAAAAAAAAAAAAwgEAAAAeAAECAIA
eAAFCAQAAJoVAAYIBAAAAAQABwgEAAAAAQAICAQAAAACAAkIBAAAswIACggIAAMA
YAC0AAIACwgIAAQAAADwAAIADQgAAAAIeAAAAwAAAtAC0AAAAAAf5jDG/3//fyBi
MUIDZQhLBXsAAgAAAtAC0AAAAAAf5jDGAAEAZABkAAAAAQABAQEAAAABJw8AAQAB
AAAAAAAAAAAAAAAAAAIAGQGQAAAAAABAAAAAAAAAAAAAAQAAAAAAAAAAAAAAAAAA
AAAAASQAAAACAAMA5AQFAEFyaWFsBADkBA8AVGltZXMgTmV3IFJvbWFuAYABAAAA
AAAAAA==
">

<input type = "hidden" name="LastCurrentLocation" value ="/drugdeg/DRUGDEG/Parent_input_form.asp?formgroup=base_form_group&formmode=search&dbname=DRUGDEG&QSDiff=44242">
<input type="hidden" name="CurrentRecord" value = "1">
<input type="hidden" name="RefineType" value="">
<input type="hidden" name="MarkedHit" value="">
<input type="hidden" name="DataAction" value="search">
<input type="hidden" name="version" value="">
<input type="hidden" name="Plugin" value = "">
<input type="hidden" name="Mac3" value="False">
<input type="hidden" name="DBName" value="DRUGDEG">
<input type="hidden" name="SubSearchFields" value="">
<input type="hidden" name="ExactSearchFields" value="">
<input type="hidden" name="SimSearchFields" value="">
<input type="hidden" name="IdentitySearchFields" value="">
<input type="hidden" name="FormulaSearchFields" value="">
<input type="hidden" name="MolWeightSearchFields" value="">
<input type="hidden" name="RelationalSearchFields" value="DRUGDEG_PARENTS.PARENT_CMPD_KEY;1">
<input type="hidden" name="SQLEngine" value="CFW">
<input type="hidden" name="SearchStrategy" value="rel">

<input type = "hidden" name = "CurrentIndex" Value = >
<input type = "hidden" name = "NumberListView" Value = 5>
<input type = "hidden" name = "RecordCount" Value = 0>
<input type="hidden" name="CurrentSingleRecord" value = "">
<input type = "hidden" name = "row_id_table_names" value = "">

<script language = "javascript">
table_names = "DRUGDEG_PARENTS,DRUGDEG_DEGS,DRUGDEG_EXPTS,DRUGDEG_MECHS,DRUGDEG_BASE64,DRUGDEG_CONDS,DRUGDEG_SALTS,DRUGDEG_STATUSES,DRUGDEG_FGROUPS,DRUGDEG_DEG_BASE64"
table_names_array = table_names.split(",")
for (i=0;i<table_names_array.length;i++){

	document.write ('<input type = "hidden" name ="' + table_names_array[i].toLowerCase() + '_ROW_IDS" value = "">')
}
</script>

</tr>
</table>

<input type = "hidden" name = "order_by" value = "DRUGDEG_PARENTS.GENERIC_NAME">

<input type = "hidden" name = "sort_direction" value = "ASC">


<input type="hidden" name="DRUGDEG_PARENTS.PARENT_CMPD_KEY" value="<%=PrimaryKey%>">
			
</form>



<!-- <A href="#" onClick="getAction('search'); return false;">click</a> -->

</BODY>
</HTML>
