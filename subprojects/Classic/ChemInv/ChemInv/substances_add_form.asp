<%@ LANGUAGE=VBScript %>
<%response.expires = 0%>
<%' Copyright 2001-2002, CambridgeSoft Corp., All Rights Reserved%>
<%
if Application("LoginRequired" & dbkey) = 1 then
	if Not Session("UserValidated" & dbkey) = 1 then  response.redirect "/" & Application("Appkey") & "/logged_out.asp"
end if%>
<!DOCTYPE HTML PUBLIC "-//IETF//DTD HTML//EN">
<html>

<head><!--#INCLUDE VIRTUAL = "/cfserverasp/source/cows_func_js.asp"-->
<SCRIPT LANGUAGE=javascript src="/cheminv/Choosecss.js"></SCRIPT>
<title>sample Search/Refine Form</title>
</head>
<!--#INCLUDE FILE="../source/secure_nav.asp"-->
<!--#INCLUDE FILE="../source/app_js.js"-->
<!--#INCLUDE FILE="../source/app_vbs.asp"-->
<body>
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/header_vbs.asp"-->

<% 
commit_type = "full_commit" 
formmode = Request("formmode")
add_record_action= "Duplicate_Search_No_Add" 
add_order = "inv_compounds" 
return_location_overrride ="" 
%>
<script language = "javascript">
MainWindow.commit_type = "<%=commit_type%>"
</script>
<input type = "hidden" name = "inv_compounds.BASE64_CDX" value = "">
<input type = "hidden" name = "add_order" value ="<%=add_order%>">
<input type = "hidden" name = "add_record_action" value = "<%=add_record_action%>">	
<input type = "hidden" name = "commit_type" value = "<%=commit_type%>">	
<input type = "hidden" name = "return_location" value = "<%=return_location_overrride%>">	
<table border="0" width="500" cellpadding="0" cellspacing="0">
		  <tr>
			<td>
				&nbsp;
			</td>
			<td colspan=2>
				<span class="GuiFeedback">Enter data below and click the "Add Record" button.</span><BR><BR>
			</td>
		  </tr>
		  <tr>
		  	<td rowspan="2">
		  		<img src="../graphics/pixel.gif" width="60" height="1" alt border="0">
		  	</td>
			<td width="300" valign="top">
				<%ShowStrucInputField  dbkey, formgroup,"inv_compounds.Structure","5","520","220", "EXACT", "EXACT"%>
			</td>
			<td valign="top">
				<table border="0">
					<tr>
			        	<td>
							<span title="(eg. Acetonitrile)"><span class="required">Substance Name:</span></span><br>
							<%ShowInputField dbkey, formgroup, "inv_compounds.Substance_Name", "0","30"%>        
						</td>
		      		</tr>
					<tr>
			        	<td>
							<span title="(eg. 50-50-0)">CAS Registry#:</span><br>
							<%ShowInputField dbkey, formgroup, "inv_compounds.CAS", "0","30"%>        
						</td>
		      		</tr>
		      		<tr>
		        		<td>
							<span title="(e.g. X1001545-9)">ACX Number:</span><br>
							<%ShowInputField dbkey, formgroup, "inv_compounds.ACX_ID", "0","30"%>
						</td>
					</tr>
					<%
					'Set alt_IDDict = Session("Alt_IDDict")
					'Set RequiredAlt_IDDict = Session("RequiredAlt_IDDict")
					For each key in alt_ids_dict
						Response.Write "<TR><td>" & vblf
						if req_alt_ids_dict.Exists(Key) then
							requiredFields =  requiredFields & "," & Key & ";0:" & alt_ids_dict.Item(Key)
							Response.Write "<span class=""required"">"
						Else
							Response.Write "<span title="""">"
						end if
						Response.Write alt_ids_dict.Item(Key) & "</span><br>"
						ShowInputField dbkey, formgroup, key, "0","30"
						Response.Write "</td></TR>" & vblf 
					Next 
					%>
		    	</table>
			</td>
		  </tr>
		</table>
<script language="JavaScript">required_fields += "<%=requiredFields%>";</script>
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/input_form_footer_vbs.asp"-->
</body>
</html>
