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

<head>
<!--#INCLUDE FILE = "../source/biosar_header.asp"-->
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/cows_func_js.asp"-->
<!--#INCLUDE FILE="../source/secure_nav.asp"-->
<!--#INCLUDE FILE="../source/app_js.js"-->

<!--#INCLUDE FILE="../source/app_vbs.asp"-->
<!--#INCLUDE FILE = "universal_ss.asp"-->

<% 
useCollapse = Application("USE_COLLAPSE_INPUT_FORM")
if useCollapse then
 %>

<script type="text/javascript">
function togglenode(name) 
{	
	nam = "node" + name;
	but = "button" + name;
	
	if (name==1)
	{
		if (node100.style.display=="")
			{
				node100.style.display="none";
			 	document.getElementById(but).value="+";
			}
		else
			{
				node100.style.display="";
			 	document.getElementById(but).value="-";
			}
	}
	else
	{
		if (document.getElementById(nam).style.display=="")
			{
				document.getElementById(nam).style.display="none";	
				document.getElementById(but).value="+";
			}
		else
			{
				document.getElementById(nam).style.display="";	
				document.getElementById(but).value="-";
			}
	}
}
</script>

<%end if %>



<title>#DB_NAME Search/Refine Form</title>
</head>
<body <%=default_body%>>
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/header_vbs.asp"-->
<table <%=table_main_L1%>>
<tr><td>

<%
'uqb = usequerybuilder
if Application("USE_QUERY_BUILDER") = 1 then
	if UCASE(Request.QueryString("uqb")) = "FALSE" then
		Session("useQueryBuilder") = "FALSE"
		useQueryBuilder = false
	elseif UCASE(Request.QueryString("uqb")) = "TRUE" then
		Session("useQueryBuilder") = "TRUE"
		useQueryBuilder = true
	elseif UCASE(Request.QueryString("uqb")) = "" and Session("useQueryBuilder") = "FALSE" then
		Session("useQueryBuilder") = "FALSE"
		useQueryBuilder = false
	elseif UCASE(Request.QueryString("uqb")) = "" and Session("useQueryBuilder") = "TRUE" then
		Session("useQueryBuilder") = "TRUE"
		useQueryBuilder = true
	else
		Session("useQueryBuilder") = "FALSE"
		useQueryBuilder = false
	end if
else
	useQueryBuilder = False
end if

if Application("USE_QUERY_BUILDER") = 1 then
	if 	useQueryBuilder then
		qstr =  Replace(Request.QueryString, "&uqb=TRUE", "") & "&uqb=FALSE"
		inputtoggle = "<a href=""biosar_browser_input_form.asp?" & qstr& """>Standard Search</a>" & "<br><br>"
	else
		qstr =  Replace(Request.QueryString, "&uqb=FALSE", "") & "&uqb=TRUE"
		inputtoggle = "<a href=""biosar_browser_input_form.asp?" & qstr& """>Query Builder</a>" & "<br><br>"
	end if	
end if
if not useQueryBuilder then
	GetItems formgroup, dbkey,  "QUERY", "REPORT" 
else

%>
	<script language="javascript">
		function doQueryBuilder(){

			var tfts = cows_input_form["thefieldtosearch"].value;
			var totu = cows_input_form["theoperatortouse"].value;
			var tsv =  cows_input_form["thesearchvalue"].value;
			
			var opdisp = "";
			if (!(tsv == ""))
			{
				if (totu == "="){
					totu = "";
					opdisp = "=";
				}
				if (totu == "CONTAINS"){
					totu = "=";
					tsv = "*" + tsv + "*";
				}
				if (totu == "STARTS WITH"){
					totu = "=";
					tsv = tsv + "*";
				}
				if (totu == "ENDS WITH"){
					totu = "=";
					tsv = "*" + tsv;
				}

				cows_input_form[tfts].value =  totu + tsv;

				var curdispquery = cows_input_form["querydisplay"].value;
				
				var addbool = "";
				if (!(curdispquery == ""))
				{
					addbool = "AND\n"
				}
				
				curdispquery =  curdispquery + addbool + tfts + totu + opdisp + tsv + "\n";

				cows_input_form["querydisplay"].value = curdispquery;
				
				cows_input_form["thesearchvalue"].value = "";
			}
			else
			{
				alert("You must enter a search value to add it to your query.");
			}
		}
	</script>
<%
	GetItems formgroup, dbkey,  "QUERY", "QUERYBUILDER"

end if
%>
<%if useQueryBuilder then %>
<table border="0" cellpadding="0">
<tr><td width="350">

<%end if %>
<%	
Response.Write inputtoggle
Response.Write Session("LEFT" & "QUERY" & dbkey & formgroup)
%>

<%if useQueryBuilder then %>
</td>
<td>
<br /><br />
Query Parameters:<br />
<textarea name="querydisplay" cols="50" rows="20" readonly unselectable="on"></textarea>

</td>
</tr>
</table>
<%end if %>

<!--#INCLUDE VIRTUAL = "/cfserverasp/source/input_form_footer_vbs.asp"-->
</td></tr>
</table>
</body>
</html>
