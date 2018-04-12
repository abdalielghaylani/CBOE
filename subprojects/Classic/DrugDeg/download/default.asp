<%@LANGUAGE = "VBScript"%>
<%


Response.Expires = 0
dbkey = "DrugDeg"
Session("wizard") = ""
Session( "CurrentLocation" & dbkey & formgroup ) = ""
Session( "CurrentLocation" & dbkey ) = ""
Session( "LastLocation" & dbkey ) = ""

if dbkey = "" then
	response.write "the name of the database (dbkey) is missing in the query string or form that requested this page."
	response.end
end if

if 1 = Application( "LoginRequired" & dbkey ) then
	if not 1 = Session( "UserValidated" & dbkey ) then
		response.redirect "/" & Application("appkey") & "/login.asp?dbname=" & dbkey & "&formgroup=base_form_group&perform_validate=0"
	end if
end if


%>
<script language="javascript">

if ( parent.location.href != window.location.href ) {
	parent.location.href = window.location.href;
}

function checkform()
{
if ((document.forms["NameForm"].FirstName.value == "")||(document.forms["NameForm"].LastName.value == "")||(document.forms["NameForm"].Email.value == "")||(document.forms["NameForm"].Company.value == "")){
	alert("All of the fields are required")
	return false;
}
else {
	return true;
}

}

</script>
<%
Session( "CurrentLocation" & dbkey & formgroup ) = ""
If Not Session( "CurrentUser" & dbkey ) <> "" then
	theuser = Session( "UserName" & dbkey )
	if theuser <> "" then
		Session( "CurrentUser" & dbkey ) = theuser
	end if
end if
%>
<html>

<head>
<%'if CBool(Application("UseCSSecurityApp")) = true then%>
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/cs_security/cs_security_utils.js" -->
<!-- script LANGUAGE="javascript" src="/cfserverasp/source/cs_security/cs_security_utils.js"></script -->
<%'end if%>

	<!--#INCLUDE VIRTUAL = "/cfserverasp/source/ado.inc" -->
	<!--#INCLUDE VIRTUAL = "/cfserverasp/source/server_const_vbs.asp" -->
	<!--#INCLUDE VIRTUAL = "/cfserverasp/source/search_func_vbs.asp" -->
	<!--#INCLUDE VIRTUAL = "/drugdeg/source/app_vbs.asp" -->

		<title>Download ChemDraw Plugin</title>

	<link REL="stylesheet" TYPE="text/css" HREF="/<%=Application("appkey")%>/styles.css">
</head>



<body <%=Application("BODY_BACKGROUND")%>>



<table>
	<tr>
		<td>
			<table border="0" bordercolor="fuchsia" cellspacing="0" cellpadding="0">
				<!-- The table for the banner. -->
				<tr>
					<td valign="top">
						<img src="/<%=Application("appkey")%>/graphics/D3_Logo_SideWeb.jpg" alt="Drug Degradation" align="center">
					</td>
					
				</tr>
			</table>
		</td>
	</tr>
</table>

<br>

This page will allow you to download the ChemDraw Pro Plugin so that you will be able properly enter data into
the D3 Database.  
<br>
After filling out the form a download link will be provided and an email will be sent with a
serial number that can be used for activation.
<br>
<font color="red">All fields are required</font>

<form name="NameForm" action="submit.asp" method="post" onsubmit="return checkform()">
<table border="0" cellpadding="3" cellspacing="0">
<tr><td>First Name</td><td><input type="text" name="FirstName" value="" size="40"></td></tr>

<tr><td>Last Name</td><td><input type="text" name="LastName" value="" size="40"></td></tr>

<tr><td>Email Address</td><td><input type="text" name="Email" value="" size="40"></td></tr>

<tr><td>Company</td><td><input type="text" name="Company" value="" size="40"></td></tr>
 <tr><td><input type="Submit" name="Submit" value="Submit"  onsubmit="return checkform()"></td><td></td></tr>  
</form>
<div align="center">
<table border="0" bordercolor="red" cellspacing="15">
	<tr>
		<td height="20" colspan="2">&nbsp;</td>
	</tr>
	
	<tr>
		<td valign="top"><a class="headinglink" href="/cs_security/login.asp?ClearCookies=true">Logout</a>
	
		</td>
		
		<td valign="top"><a href="#" class="headinglink" onclick="window.open('/<%=Application("appkey")%>/help/help.asp?formgroup=base_form_group&amp;dbname=drugdeg','help','width=800,height=600,scrollbars=yes,status=no,resizable=yes');">Help</a>
	
		</td>
	
		<td valign="top"><a href="#" class="headinglink" onclick="window.open('/<%=Application("appkey")%>/about.asp?formgroup=base_form_group&amp;dbname=drugdeg','about','width=560,height=450,status=no,resizable=yes')">About the database</a>
		</td>
	</tr>
	<tr>
		<td height="20" colspan="2">&nbsp;</td>
	</tr>
</table>
</div>

</body>

</html>
