<%
' ACX Look up link code
If ACX_ID <> "" then
	fieldName = "Substance.ACX_ID"
	fieldValue = ACX_ID 
Elseif CAS <> "" then
	fieldName = "Substance.CAS"
	fieldValue = CAS
Elseif SubstanceName <> "" then
	fieldName = "Synonym.Name"
	fieldValue = SubstanceName
End if
%>
<form name="acxPost" method="GET" action="<%=Application("SERVER_TYPE") & Application("ACXServerName")%>/chemacx/default.asp" target="_new">
	<input TYPE="hidden" name="dataaction" value="query_string">
	<input TYPE="hidden" name="dbname" value="chemacx">
	<input TYPE="hidden" name="formgroup" value="base_form_group">
	<input TYPE="hidden" name="field_type" value="TEXT">
	<input TYPE="hidden" name="full_field_name" value="<%=fieldName%>">
	<input type="hidden" name="field_value" value="<%=fieldValue%>">
</form>

