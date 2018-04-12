<%@ Language=VBScript %>
<!--#INCLUDE VIRTUAL = "/cfserverasp/source/cs_security/cs_security_utils_vbs.asp"-->
<%
'Copyright 1999-2003 CambridgeSoft Corporation. All rights reserved	
Dim ldResults
Dim LDAPErrMsg
Dim node

dbkey = Request("dbkey")
LDAPUserName = Request("LDAPUserName")
isEdit = Request("isEdit")
if isEdit = "" then isEdit = false

Caption = "Get LDAP User"
if LDAPUserName <> "" then
	Caption = "Confirm LDAP User"
	on error resume next
	GetUserInfo LDAPUserName
	if err then
		Caption = "LDAP Error"
		LDAPErrMsg = err.Description
	end if
End if

Sub GetUserInfo(un)
	Dim svcAccount
	
	svcAccount = Application("ACTIVE_DIRECTORY_DOMAIN") & "\" & Application("LDAP_SERVICE_ACCOUNT_NAME")
	pwd = Application("LDAP_SERVICE_ACCOUNT_PWD")
	
	Set oLDAPAuthenticator = Server.CreateObject("CSSecurityLDAP.LDAPAuthenticator")	
	'Set ldResults = CreateObject("CSSecurityLDAP.LDAPRESULTS")
	
	on error resume next
	Set ldResults = oLDAPAuthenticator.GetUserInfo(Application("LDAPConfigXmlPath"), un, svcAccount, pwd)
	
	If Err then 
		err.Raise err.number, err.Source, err.Description  			
	end if 
End Sub


%>
<html>
<head>
	<title>Select LDAP User</title>
	<script LANGUAGE="javascript">
		window.focus();
		function Doit(){
			var e;
			
			if (opener.document.user_manager){
			   var tForm = opener.document.user_manager;
			   
			}
			else{
			   alert('Error:  The window that opened this dialog has been closed.')
			   return false;
			}	
				
			for (var i = 0; i< document.form1.elements.length; i++){
			   e = document.form1.elements[i]				
			   if (tForm.elements[e.name]) tForm.elements[e.name].value = e.value;
			}
			//opener.lockFields();
			window.close();
		}
	</script>

	<script LANGUAGE="javascript" src="/cfserverasp/source/cs_security/Choosecss.js"></script>
</head>
<body onload="if (form1.LDAPUserName) form1.LDAPUserName.focus();">
	<center>	

<form name="form1" METHOD="POST">

<br><br><br>
<table border="0">
	<tr>
		<td colspan="2">
			<span class="GuiFeedback"><%=Caption%></span><br><br>
		</td>
	</tr>
<%if LDAPUserName = "" then%> 
	<tr>
		<td align="right" nowrap>
			<span class="required">Enter LDAP User Name:</span>
		</td>
		<td>
			<input tabIndex="1" Name="LDAPUserName" TYPE="tetx" SIZE="35" Maxlength="30" VALUE>
		</td>
	</tr>
<%else%>
	<%if LDAPErrmsg <> "" then%>
	<tr>
		<td colspan="2">
			<%=LDAPErrMsg %>
		</td>
	</tr>
	<tr>
		<td colspan="2">
			&nbsp;
		</td>
	</tr>
	<%else%>
	
		
			<%
			'Response.write ldResults.XML.xml
			set nodes =  ldresults.XML.documentElement.childNodes
			for each node in nodes
				if node.nodeName = "username" then
					Response.Write "<tr><td>User Name: </td><td><input disabled onfocus=blur() size=35 type=text value=""" & node.text & """ name=UserName></td></tr>"
				End if
				Set mapToAttr = node.Attributes.getNamedItem("mapTo")
				Set displayNameAttr = node.Attributes.getNamedItem("displayName")
				If Not (maptoAttr Is Nothing) Then
				    Response.Write "<tr><td>" & displayNameAttr.text & ":</td><td><input disabled onfocus=blur() size=35 type=text value=""" & node.text & """" & " name=""" & mapToAttr.text &"""></td></tr>"
				End If
			next
			
			%>
		
	<%end if%>
<%end if%>		
	<tr>
		<td align="right" valign="bottom" colspan="2">
		     <%if LDAPUserName = "" then%>
				<a Class="MenuLink" Href="#" onclick="window.close(); return false;"><img SRC="/cfserverasp/source/graphics/navbuttons/cancel_dialog_btn.gif" border="0"></a>
			    <a Class="MenuLink" Href="#" onclick="form1.submit()"><img SRC="/cfserverasp/source/graphics/navbuttons/ok_dialog_btn.gif" border="0"></a>
		     <%else%>
				<%if LDAPErrMsg <> "" then%>
					<%if isEdit then%>
						<a Class="MenuLink" Href="#" onclick="window.close()"><img SRC="/cfserverasp/source/graphics/navbuttons/ok_dialog_btn.gif" border="0"></a>
					<%else%>
						<a Class="MenuLink" Href="#" onclick="history.back(-2)"><img SRC="/cfserverasp/source/graphics/navbuttons/ok_dialog_btn.gif" border="0"></a>
					<%end if%>
				<%else%>
					<a Class="MenuLink" Href="#" onclick="history.back(-2)"><img SRC="/cfserverasp/source/graphics/navbuttons/cancel_dialog_btn.gif" border="0"></a>
					<a Class="MenuLink" Href="#" onclick="Doit(); return false;"><img SRC="/cfserverasp/source/graphics/navbuttons/ok_dialog_btn.gif" border="0"></a>
				<%end if%>
		     <%end if%>
		     
		</td>
	</tr>
</table>
</form>
<br>
</center>
</body>
</html>
